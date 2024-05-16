using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using SpyMod.Spy.Content;

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

        private float cloakTimer = 0f;
        private bool isCloaked = false;
        private bool stopwatchOut = false;

        public bool hasPlayed = false;

        private uint playID1;

        private ParticleSystem handElectricityEffect;

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
        }
        private void FixedUpdate()
        {
            cloakTimer -= Time.fixedDeltaTime;

            if (cloakTimer <= 0f && isCloaked)
            {
                ExitStealth();
            }

            if(!this.characterBody.HasBuff(SpyBuffs.spyDiamondbackBuff))
            {
                DeactivateCritLightning();
                hasPlayed = false;
            }
        }
        public bool IsStopWatchOut()
        {
            return stopwatchOut;
        }
        public void EnableWatchLayer()
        {
            if (!this.animator || !this.childLocator) return;

            this.childLocator.FindChild("Watch").gameObject.SetActive(true);
            this.animator.SetLayerWeight(this.animator.GetLayerIndex("LeftArm, Override"), 1f);
            stopwatchOut = true;
        }
        public void EnterStealth()
        {
            cloakTimer = 5f;
            isCloaked = true;
            Util.PlaySound("sfx_spy_cloak", base.gameObject);
            DisableWatchLayer();
            characterBody.skillLocator.special.stock--;
        }
        public void ExitStealth()
        {
            cloakTimer = 0f;
            isCloaked = false;
            if (NetworkServer.active)
            {
                Util.PlaySound("sfx_spy_uncloak", base.gameObject);
                characterBody.RemoveBuff(RoR2Content.Buffs.Cloak);
                characterBody.RemoveBuff(RoR2Content.Buffs.CloakSpeed);
                characterBody.RemoveBuff(SpyBuffs.armorBuff);
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
        
        public void DeactivateCritLightning()
        {
            if (this.handElectricityEffect.isPlaying) this.handElectricityEffect.Stop();
            AkSoundEngine.StopPlayingID(this.playID1);
        }
        public void DisableWatchLayer()
        {
            if (!this.animator || !this.childLocator) return;

            this.childLocator.FindChild("Watch").gameObject.SetActive(false);
            this.animator.SetLayerWeight(this.animator.GetLayerIndex("LeftArm, Override"), 0f);
            stopwatchOut = false;
        }

        private void OnDestroy()
        {
            AkSoundEngine.StopPlayingID(this.playID1);
        }
    }
}
