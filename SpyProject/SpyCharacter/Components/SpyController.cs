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
        public bool isDiamondBack => skillLocator.primary.skillNameToken == SpySurvivor.SPY_PREFIX + "PRIMARY_REVOLVER_NAME";
        public bool isAmbassador => skillLocator.primary.skillNameToken == SpySurvivor.SPY_PREFIX + "PRIMARY_REVOLVER2_NAME";
        public bool isDefaultKnife => skillLocator.secondary.skillNameToken == SpySurvivor.SPY_PREFIX + "SECONDARY_KNIFE_NAME";
        public bool isBigEarner => skillLocator.secondary.skillNameToken == SpySurvivor.SPY_PREFIX + "SECONDARY_KNIFE2_NAME";
        public bool isDefaultCloak => skillLocator.special.skillNameToken == SpySurvivor.SPY_PREFIX + "SPECIAL_WATCH_NAME" || skillLocator.special.skillNameToken == SpySurvivor.SPY_PREFIX + "SPECIAL_SCEPTER_WATCH_NAME";
        public bool isSpecialDeadman => skillLocator.special.skillNameToken == SpySurvivor.SPY_PREFIX + "SPECIAL_WATCH2_NAME" || skillLocator.special.skillNameToken == SpySurvivor.SPY_PREFIX + "SPECIAL_SCEPTER_WATCH2_NAME";
        public float cloakRecharge => skillLocator.special.rechargeStopwatch;
        public float maxCloakRecharge => skillLocator.special.finalRechargeInterval;

        public string currentSkinNameToken => this.skinController.skins[this.skinController.currentSkinIndex].nameToken;

        public string altSkinNameToken => SpySurvivor.SPY_PREFIX + "MASTERY_SKIN_NAME";

        public float maxCloakTimer = 0f;
        private bool hasPlayed = false;
        private bool isCloaked = false;
        private bool stopwatchOut = false;
        private bool hasSpun = true;
        private bool hasPunishedChainStab = true;
        private bool hasPlayedRecharge = true;
        public bool pauseTimer = false;

        private float spinTimer = 0f;
        private float gracePeriod = 0f;
        public float cloakTimer = 0f;

        private int chainStabComboCounter = 0;

        private uint playID1;

        private ParticleSystem handElectricityEffect;
        private ParticleSystem chainStabCombo;
        private ParticleSystem chainStabComboHand;
        private GameObject spinInstance;
        private GameObject muzzleTrail;

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

            this.Invoke("ApplySkin", 0.3f);
        }

        public void ApplySkin()
        {
            if(this.skinController)
            {
                if (this.currentSkinNameToken == this.altSkinNameToken)
                {
                    this.handElectricityEffect = this.childLocator.FindChild("CritLightning2").gameObject.GetComponent<ParticleSystem>();
                    this.chainStabCombo = this.childLocator.FindChild("ChainStabCombo2").gameObject.GetComponent<ParticleSystem>();
                    this.chainStabComboHand = this.childLocator.FindChild("ChainStabComboHand2").gameObject.GetComponent<ParticleSystem>();
                }
                else
                {
                    this.handElectricityEffect = this.childLocator.FindChild("CritLightning").gameObject.GetComponent<ParticleSystem>();
                    this.chainStabCombo = this.childLocator.FindChild("ChainStabCombo").gameObject.GetComponent<ParticleSystem>();
                    this.chainStabComboHand = this.childLocator.FindChild("ChainStabComboHand").gameObject.GetComponent<ParticleSystem>();
                }
            }
        }
        private void Start()
        {
            if (this.isDefaultCloak)
            {
                this.maxCloakTimer = SpyConfig.maxCloakDurationDefault.Value;
            }
            else if (this.isSpecialDeadman)
            {
                this.maxCloakTimer = SpyConfig.maxCloakDurationDead.Value;
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
        private void Update()
        {
            if(this.chainStabComboHand)
            {
                if (this.chainStabComboHand.isPlaying)
                {
                    Vector3 what = this.childLocator.FindChild("Gun").gameObject.transform.position;
                    what.y += 0.06f;
                    what.z -= 0.04f;
                    what.x -= 0.025f;
                    this.chainStabComboHand.gameObject.transform.position = what;
                }
            }
        }
        private void FixedUpdate()
        {
            spinTimer -= Time.fixedDeltaTime;
            gracePeriod -= Time.fixedDeltaTime;

            if(isCloaked && !pauseTimer)
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
                chainStabComboCounter = 0;
                if(muzzleTrail) GameObject.Destroy(muzzleTrail);
                if (this.chainStabCombo.isPlaying) this.chainStabCombo.Stop();
                if (this.chainStabComboHand.isPlaying) this.chainStabComboHand.Stop();
            }

            if (!this.characterBody.HasBuff(SpyBuffs.spyDiamondbackBuff))
            {
                DeactivateCritLightning();
                hasPlayed = false;
            }
        }
        public void ResetChainStabPeriod()
        {
            chainStabComboCounter++;
            hasPunishedChainStab = false;
            gracePeriod = SpyConfig.bigEarnerGracePeriod.Value + chainStabComboCounter / 2f;
            if(muzzleTrail) GameObject.Destroy(muzzleTrail.gameObject);
            Transform muzzleTransform = this.childLocator.FindChild("Gun");

            muzzleTrail = GameObject.Instantiate(SpyAssets.defaultMuzzleTrail, muzzleTransform);
            Color color;
            if (this.currentSkinNameToken == this.altSkinNameToken) color = Color.Lerp(Color.yellow, new Color(255f / 255f, 165f / 255f, 0f / 255f), chainStabComboCounter / 10f).RGBMultiplied(0.5f).AlphaMultiplied(0.5f);
            else  color = Color.Lerp(new Color(100f / 255f, 215f / 255f, 233f / 255f), new Color(20f / 255f, 255f/ 255f, 170f / 255f), chainStabComboCounter / 10f).RGBMultiplied(0.5f).AlphaMultiplied(0.5f);
            muzzleTrail.GetComponent<TrailRenderer>().startColor = color;
            muzzleTrail.GetComponent<TrailRenderer>().endColor = color;
            muzzleTrail.GetComponent<TrailRenderer>().startWidth = Mathf.Lerp(0.045f,  1.2f, chainStabComboCounter / SpyConfig.maxChainStabCombo.Value);

            if (!this.chainStabCombo.isPlaying && chainStabComboCounter >= SpyConfig.maxChainStabCombo.Value) chainStabCombo.Play();

            float remap = Util.Remap(chainStabComboCounter, 0f, 10f, 1f, 2f);
            var main2 = chainStabComboHand.main;
            main2.startLifetime = remap;
            main2.startSpeed = remap;
            main2.startSize = remap;

            if (!this.chainStabComboHand.isPlaying) chainStabComboHand.Play();
        }
        public void SpinGun()
        {
            if(this.spinInstance) GameObject.Destroy(this.spinInstance);
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
                cloakTimer = SpyConfig.maxCloakDurationDead.Value;
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
            if (this.handElectricityEffect)
            {
                if (!this.handElectricityEffect.isPlaying) this.handElectricityEffect.Play();
                if (!hasPlayed)
                {
                    this.playID1 = Util.PlaySound("sfx_scout_atomic_duration", this.gameObject);
                    hasPlayed = true;
                }
            }
        }
        
        public void DeactivateCritLightning(bool willReturn = false)
        {
            if(this.handElectricityEffect)
            {
                if (willReturn) hasPlayed = false;
                if (this.handElectricityEffect.isPlaying) this.handElectricityEffect.Stop();
                AkSoundEngine.StopPlayingID(this.playID1);
            }
        }

        private void Inventory_onItemAddedClient(ItemIndex itemIndex)
        {
            if (itemIndex == DLC1Content.Items.EquipmentMagazineVoid.itemIndex)
            {
                if(this.isDefaultCloak)
                {
                    this.maxCloakTimer = SpyConfig.maxCloakDurationDefault.Value + this.characterBody.inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid);
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
