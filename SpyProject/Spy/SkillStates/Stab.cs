using UnityEngine;
using EntityStates;
using SpyMod.Modules.BaseStates;
using RoR2;
using UnityEngine.AddressableAssets;
using SpyMod.Spy.Content;
using static R2API.DamageAPI;

namespace SpyMod.Spy.SkillStates
{
    public class Stab : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            RefreshState();
            hitboxGroupName = "KnifeHitbox";

            damageType = DamageType.Generic;
            damageCoefficient = SpyStaticValues.stabDamageCoefficient;
            procCoefficient = 1f;
            pushForce = 200f;
            bonusForce = Vector3.zero;
            baseDuration = 0.8f;

            //0-1 multiplier of baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0.2f;
            attackEndPercentTime = 0.3f;

            earlyExitPercentTime = 0.5f;

            hitStopDuration = 0.02f;
            attackRecoil = 2f / this.attackSpeedStat;
            hitHopVelocity = 4f;

            swingSoundString = "sfx_driver_swing";
            hitSoundString = "";
            muzzleString = "SwingMuzzle1";
            playbackRateParam = "Swing.playbackRate";
            swingEffectPrefab = SpyAssets.knifeSwingEffect;
            
            moddedDamageTypeHolder.Add(DamageTypes.BackStab);
            hitEffectPrefab = SpyAssets.knifeHitEffect;

            impactSound = SpyAssets.knifeImpactSoundDef.index;

            base.OnEnter();
            if (characterBody.hasCloakBuff) spyController.ExitStealth();
        }

        public override void OnExit()
        {
            base.OnExit();
            this.FindModelChild("Knife").gameObject.SetActive(false);
            this.FindModelChild("Revolver").gameObject.SetActive(true);
        }
        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }

        protected override void FireAttack()
        {
            if (base.isAuthority)
            {
                Vector3 direction = this.GetAimRay().direction;
                direction.y = Mathf.Max(direction.y, direction.y * 0.5f);
                this.FindModelChild("MeleePivot").rotation = Util.QuaternionSafeLookRotation(direction);
            }

            base.FireAttack();
        }

        protected override void PlaySwingEffect()
        {
            Util.PlaySound(this.swingSoundString, this.gameObject);
            if (this.swingEffectPrefab)
            {
                Transform muzzleTransform = this.FindModelChild(this.muzzleString);
                if (muzzleTransform)
                {
                    this.swingEffectPrefab = Object.Instantiate<GameObject>(this.swingEffectPrefab, muzzleTransform);
                }
            }
        }

        protected override void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "Stab", playbackRateParam, duration * 1.2f, 0.05f);
        }
    }
}