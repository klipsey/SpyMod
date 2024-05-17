using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.HudOverlay;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using SpyMod.Spy.Content;
using System;

namespace SpyMod.Spy.Components
{
    public class SpyController : MonoBehaviour
    {
        private CharacterBody characterBody;
        private ModelSkinController skinController;
        private ChildLocator childLocator;
        private CharacterModel characterModel;
        private Animator animator;
        private SkillLocator skillLocator;
        private OverlayController overlayController;
        public bool isDiamondBack => skillLocator.primary.skillNameToken == SpySurvivor.SPY_PREFIX + "PRIMARY_REVOLVER_NAME";
        public bool isAmbassador => skillLocator.primary.skillNameToken == SpySurvivor.SPY_PREFIX + "PRIMARY_REVOLVER2_NAME";
        public bool isDefaultKnife => skillLocator.secondary.skillNameToken == SpySurvivor.SPY_PREFIX + "SECONDARY_KNIFE_NAME";
        public bool isBigEarner => skillLocator.secondary.skillNameToken == SpySurvivor.SPY_PREFIX + "SECONDARY_KNIFE2_NAME";
        public bool isDefaultCloak => skillLocator.special.skillNameToken == SpySurvivor.SPY_PREFIX + "SPECIAL_WATCH_NAME";
        public bool isSpecialDeadman => skillLocator.special.skillNameToken == SpySurvivor.SPY_PREFIX + "SPECIAL_WATCH2_NAME";
        public float cloakRecharge => skillLocator.special.rechargeStopwatch;
        public float maxCloakRecharge => skillLocator.special.finalRechargeInterval;
        public float maxCloakTimer = 0f;
        private bool hasPlayed = false;
        private bool isCloaked = false;
        private bool stopwatchOut = false;
        private bool hasSpun = true;
        private bool hasPunishedChainStab = true;
        private bool hasPlayedRecharge = true;

        private float spinTimer = 0f;
        private float gracePeriod = 0f;
        public float cloakTimer = 0f;

        private uint playID1;

        private ParticleSystem handElectricityEffect;
        private GameObject spinInstance;

        public Action onStealthChange;

        private void Awake()
        {
            this.characterBody = this.GetComponent<CharacterBody>();
            ModelLocator modelLocator = this.GetComponent<ModelLocator>();
            this.childLocator = modelLocator.modelBaseTransform.GetComponentInChildren<ChildLocator>();
            this.animator = modelLocator.modelBaseTransform.GetComponentInChildren<Animator>();
            this.characterModel = modelLocator.modelBaseTransform.GetComponentInChildren<CharacterModel>();
            this.skillLocator = this.GetComponent<SkillLocator>();
            this.skinController = modelLocator.modelTransform.gameObject.GetComponent<ModelSkinController>();
            this.handElectricityEffect = this.childLocator.FindChild("CritLightning").gameObject.GetComponent<ParticleSystem>();
        }
        private void Start()
        {
            if (this.isDefaultCloak)
            {
                this.maxCloakTimer = SpyConfig.maxCloakDefault.Value;
            }
            else if (this.isSpecialDeadman)
            {
                this.maxCloakTimer = SpyConfig.maxCloakDead.Value;
            }

            this.cloakTimer = this.maxCloakTimer;

            if (this.characterBody && this.characterBody.master && this.characterBody.master.inventory)
            {
                this.characterBody.master.inventory.onItemAddedClient += this.Inventory_onItemAddedClient;
            }

            if (this.isBigEarner)
            {
                characterBody.baseMaxHealth *= 1 - SpyConfig.bigEarnerHealthPunishment.Value;
                characterBody.baseMaxShield *= 1 - SpyConfig.bigEarnerHealthPunishment.Value;
                characterBody.baseRegen *= 1 - SpyConfig.bigEarnerHealthPunishment.Value;
            }
        }
        private void FixedUpdate()
        {
            spinTimer -= Time.fixedDeltaTime;
            gracePeriod -= Time.fixedDeltaTime;

            if(isCloaked)
            {
                cloakTimer -= Time.fixedDeltaTime;
                this.onStealthChange?.Invoke();
                if (cloakTimer <= 0f)
                {
                    DisableWatchLayer();
                    Util.PlaySound("sfx_spy_uncloak_alt", base.gameObject);
                    ExitStealth();
                }
            }
            else if(!isCloaked && isDefaultCloak && cloakTimer < maxCloakTimer)
            {
                cloakTimer += Time.fixedDeltaTime;
                hasPlayedRecharge = false;
            }
            else if(cloakTimer >= maxCloakTimer && !hasPlayedRecharge)
            {
                cloakTimer = maxCloakTimer;
                Util.PlaySound("sfx_spy_recharged_cloak", base.gameObject);
                hasPlayedRecharge = true;
            }

            if (spinTimer <= 0f && !hasSpun)
            {
                hasSpun = true;
                GameObject.Destroy(this.spinInstance);
            }

            if (gracePeriod <= 0f && !hasPunishedChainStab)
            {
                this.skillLocator.secondary.DeductStock(1);
                hasPunishedChainStab = true;
            }

            if (!this.characterBody.HasBuff(SpyBuffs.spyDiamondbackBuff))
            {
                DeactivateCritLightning();
                hasPlayed = false;
            }
        }
        public void ResetChainStabPeriod()
        {
            hasPunishedChainStab = false;
            gracePeriod = SpyStaticValues.bigEarnerGracePeriod;
        }
        public void SpinGun()
        {
            this.spinInstance = GameObject.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoReloadFX.prefab").WaitForCompletion());
            this.spinInstance.transform.parent = this.childLocator.FindChild("Gun");
            this.spinInstance.transform.localRotation = Quaternion.Euler(new Vector3(0f, 80f, 0f));
            this.spinInstance.transform.localPosition = Vector3.zero;

            Util.PlaySound("sfx_spy_pistol_spin", this.gameObject);

            hasSpun = false;
            spinTimer = 0.5f;
        }
        public bool IsStopWatchOut()
        {
            return stopwatchOut;
        }
        public void EnableWatchLayer()
        {
            if (!this.animator || !this.childLocator) return;

            this.childLocator.FindChild("Watch").gameObject.SetActive(true);
            this.characterBody._defaultCrosshairPrefab = SpyAssets.watchCrosshair;
            this.animator.SetLayerWeight(this.animator.GetLayerIndex("LeftArm, Override"), 1f);
            stopwatchOut = true;
        }
        public void DisableWatchLayer()
        {
            if (NetworkServer.active)
            {
                if(characterBody.HasBuff(SpyBuffs.spyWatchDebuff)) characterBody.RemoveBuff(SpyBuffs.spyWatchDebuff);
            }

            if (!this.animator || !this.childLocator) return;

            this.childLocator.FindChild("Watch").gameObject.SetActive(false);
            this.characterBody._defaultCrosshairPrefab = SpyAssets.defaultCrosshair;
            this.animator.SetLayerWeight(this.animator.GetLayerIndex("LeftArm, Override"), 0f);
            stopwatchOut = false;
        }
        public void EnterStealth()
        {

            if(this.isSpecialDeadman)
            {
                DisableWatchLayer();
                cloakTimer = SpyConfig.maxCloakDead.Value;
                characterBody.skillLocator.special.stock--;
            }

            Util.PlaySound("sfx_spy_cloak", base.gameObject);

            EffectManager.SpawnEffect(SpyAssets.spyCloakEffect, new EffectData
            {
                origin = base.transform.position,
                scale = 0.5f
            }, transmit: true);

            isCloaked = true;
        }
        public void ExitStealth()
        {
            if(this.isSpecialDeadman)
            {
                Util.PlaySound("sfx_spy_uncloak", base.gameObject);
                cloakTimer = 0f;
            }

            EffectManager.SpawnEffect(SpyAssets.spyCloakEffect, new EffectData
            {
                origin = base.transform.position,
                scale = 0.5f
            }, transmit: true);

            isCloaked = false;

            if (NetworkServer.active)
            {
                if (characterBody.HasBuff(RoR2Content.Buffs.Cloak)) characterBody.RemoveBuff(RoR2Content.Buffs.Cloak);
                if (characterBody.HasBuff(RoR2Content.Buffs.CloakSpeed)) characterBody.RemoveBuff(RoR2Content.Buffs.CloakSpeed);
                if (characterBody.HasBuff(SpyBuffs.armorBuff)) characterBody.RemoveBuff(SpyBuffs.armorBuff);
            }
        }

        public void ActivateCritLightning()
        {
            if (!this.handElectricityEffect.isPlaying) this.handElectricityEffect.Play();
            if (!hasPlayed)
            {
                this.playID1 = Util.PlaySound("sfx_scout_atomic_duration", this.gameObject);
                hasPlayed = true;
            }
        }
        
        public void DeactivateCritLightning(bool willReturn = false)
        {
            if(willReturn) hasPlayed = false;
            if (this.handElectricityEffect.isPlaying) this.handElectricityEffect.Stop();
            AkSoundEngine.StopPlayingID(this.playID1);
        }

        private void Inventory_onItemAddedClient(ItemIndex itemIndex)
        {
            if (itemIndex == DLC1Content.Items.EquipmentMagazineVoid.itemIndex)
            {
                if(this.isDefaultCloak)
                {
                    this.maxCloakTimer = SpyConfig.maxCloakDefault.Value + this.characterBody.inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid);
                };
            }
        }

        private void OnDestroy()
        {
            AkSoundEngine.StopPlayingID(this.playID1);

            if (this.characterBody && this.characterBody.master && this.characterBody.master.inventory)
            {
                this.characterBody.master.inventory.onItemAddedClient -= this.Inventory_onItemAddedClient;
            }
        }
    }
}
