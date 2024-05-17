using RoR2;
using UnityEngine;
using EntityStates;
using R2API;
using SpyMod.Modules.BaseStates;
using SpyMod.Spy.Content;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace SpyMod.Spy.SkillStates
{
    public class Shoot : BaseSpySkillState
    {
        public float damageCoefficient = SpyStaticValues.revolverDamageCoefficient;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.7f;
        public static float force = 200f;
        public static float recoil = 2f;
        public static float range = 2000f;
        public static GameObject tracerEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerGoldGat");
        public static GameObject critTracerEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCaptainShotgun");

        protected float duration;
        protected string muzzleString;
        protected bool isCrit;
        protected int diamondbackStacks;

        protected virtual GameObject tracerPrefab => this.isCrit ? Shoot.critTracerEffectPrefab : Shoot.tracerEffectPrefab;
        public string shootSoundString = "Play_bandit2_R_fire";
        public virtual BulletAttack.FalloffModel falloff => BulletAttack.FalloffModel.DefaultBullet;

        public override void OnEnter()
        {
            base.OnEnter();
            if (this.characterBody.hasCloakBuff) spyController.ExitStealth();
            this.duration = Shoot.baseDuration / this.attackSpeedStat;
            this.characterBody.isSprinting = false;

            base.characterBody.SetAimTimer(2f);
            this.muzzleString = "GunMuzzle";

            this.isCrit = base.RollCrit();

            this.diamondbackStacks = this.characterBody.GetBuffCount(SpyBuffs.spyDiamondbackBuff);

            if(this.diamondbackStacks > 0)
            {
                if (NetworkServer.active) this.characterBody.RemoveBuff(SpyBuffs.spyDiamondbackBuff);
                if (this.isCrit) damageCoefficient *= 1.5f;
                else this.isCrit = true;
            }

            this.shootSoundString = this.isCrit ? "sfx_spy_revolver_shoot_crit" : "sfx_spy_revolver_shoot";
            if (base.isAuthority)
            {
                this.Fire();
            }

            this.duration = Shoot.baseDuration / this.attackSpeedStat;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public void Fire()
        {
            this.PlayAnimation("Gesture, Override", "Shoot", "Shoot.playbackRate", 1.2f / this.attackSpeedStat);

            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, this.gameObject, this.muzzleString, false);

            Util.PlaySound(this.shootSoundString, this.gameObject);

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                base.AddRecoil2(-1f * Shoot.recoil, -2f * Shoot.recoil, -0.5f * Shoot.recoil, 0.5f * Shoot.recoil);

                BulletAttack bulletAttack = new BulletAttack
                {
                    bulletCount = 1,
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = this.damageCoefficient * this.damageStat,
                    damageColorIndex = DamageColorIndex.Default,
                    falloffModel = this.falloff,
                    maxDistance = Shoot.range,
                    force = Shoot.force,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = this.characterBody.spreadBloomAngle * 2f,
                    isCrit = this.isCrit,
                    owner = base.gameObject,
                    muzzleName = muzzleString,
                    smartCollision = true,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = procCoefficient,
                    radius = 0.75f,
                    sniper = false,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    weapon = null,
                    tracerEffectPrefab = this.tracerPrefab,
                    spreadPitchScale = 1f,
                    spreadYawScale = 1f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
                };
                bulletAttack.Fire();
            }

            base.characterBody.AddSpreadBloom(1.25f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }

}