using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
using RoR2.HudOverlay;
using UnityEngine.AddressableAssets;
using static RoR2.CameraTargetParams;
using SpyMod.Spy.Content;
using SpyMod.Modules.BaseStates;

namespace SpyMod.Spy.SkillStates
{
    public class Shoot2 : BaseSpySkillState
    {
        public float damageCoefficient = SpyStaticValues.revolverDamageCoefficient;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.9f;
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

        private CameraParamsOverrideHandle camParamsOverrideHandle;
        private OverlayController overlayController;
        private float fireTimer;
        public override void OnEnter()
        {
            damageCoefficient = SpyStaticValues.ambassadorDamageCoefficient;
            base.OnEnter();

            if (this.characterBody.hasCloakBuff) spyController.ExitStealth();
            this.duration = Shoot.baseDuration / this.attackSpeedStat;
            this.characterBody.isSprinting = false;

            base.characterBody.SetAimTimer(2f);
            this.muzzleString = "GunMuzzle";

            this.isCrit = RollCrit();

            this.shootSoundString = this.isCrit ? "sfx_spy_revolver_shoot_crit" : "sfx_spy_revolver_shoot";
            if (base.isAuthority)
            {
                this.Fire();
            }

            this.overlayController = HudOverlayManager.AddOverlay(this.gameObject, new OverlayCreationParams
            {
                prefab = SpyAssets.headshotOverlay,
                childLocatorEntry = "ScopeContainer"
            });
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.fireTimer += Time.fixedDeltaTime;

                if (!this.inputBank.skill1.down && this.fireTimer >= this.duration)
                {
                    if(base.isAuthority)
                    {
                        this.outer.SetNextStateToMain();

                    }
                }
                else if (this.inputBank.skill1.down && this.fireTimer >= this.duration)
                {
                    base.characterBody.SetAimTimer(2f);
                    this.Fire();
                }
        }
        public void Fire()
        {
            this.PlayAnimation("Gesture, Override", "Shoot2", "Shoot.playbackRate", this.duration * 1.5f);

            this.fireTimer = 0f;
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
                bulletAttack.modifyOutgoingDamageCallback = delegate (BulletAttack _bulletAttack, ref BulletAttack.BulletHit hitInfo, DamageInfo damageInfo)
                {
                    if (BulletAttack.IsSniperTargetHit(hitInfo))
                    {
                        if (damageInfo.crit) damageInfo.damage *= 1.5f;
                        else damageInfo.crit = true;
                        damageInfo.damageColorIndex = DamageColorIndex.Sniper;
                        EffectData effectData = new EffectData
                        {
                            origin = hitInfo.point,
                            rotation = Quaternion.LookRotation(-hitInfo.direction)
                        };

                        effectData.SetHurtBoxReference(hitInfo.hitHurtBox);
                        EffectManager.SpawnEffect(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Common/VFX/WeakPointProcEffect.prefab").WaitForCompletion(), effectData, true);
                        Util.PlaySound("sfx_driver_headshot", base.gameObject);
                    }
                };
                bulletAttack.Fire();
            }

            base.characterBody.AddSpreadBloom(1.25f);
        }
        public override void OnExit()
        {
            base.OnExit();

            this.spyController.SpinGun();

            if (this.overlayController != null)
            {
                HudOverlayManager.RemoveOverlay(this.overlayController);
                this.overlayController = null;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
