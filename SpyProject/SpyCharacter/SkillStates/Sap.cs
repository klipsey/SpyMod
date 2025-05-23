﻿using EntityStates;
using R2API;
using RoR2;
using RoR2.Projectile;
using SpyMod.Modules;
using SpyMod.Modules.BaseStates;
using SpyMod.Spy.Content;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SpyMod.Spy.SkillStates
{
    public class Sap : BaseSpySkillState
    {
        public float jumpDuration = 0.8f;
        public static float inAirDuration = 0f;
        public static float jumpPower = 18f;
        public static float minDampingStrength = 0.2f;
        public static float maxDampingStrength = 0.05f;

        private float previousAirControl;

        protected Vector3 slipVector = Vector3.zero;
        public float duration = 0.3f;
        public float speedCoefficient = 7f;
        private Vector3 cachedForward;

        private Quaternion slideRotation;

        private bool isFlip = false;
        private bool sapped = false;

        public override void OnEnter()
        {
            base.OnEnter();

            if(this.isGrounded)
            {
                this.slipVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
                this.cachedForward = this.characterDirection.forward;

                Animator anim = this.GetModelAnimator();

                Vector3 rhs = base.characterDirection ? base.characterDirection.forward : this.slipVector;
                Vector3 rhs2 = Vector3.Cross(Vector3.up, rhs);
                float num = Vector3.Dot(this.slipVector, rhs);
                float num2 = Vector3.Dot(this.slipVector, rhs2);
                anim.SetFloat("dashF", num);
                anim.SetFloat("dashR", num2);
                this.slideRotation = Quaternion.LookRotation(this.slipVector, this.cachedForward);

                base.PlayCrossfade("FullBody, Override", "Dash", "Dash.playbackRate", this.duration * 1.5f, 0.05f);
                base.PlayAnimation("Gesture, Override", "BufferEmpty");

                Util.PlaySound("sfx_driver_dash", this.gameObject);

                if (EntityStates.BrotherMonster.BaseSlideState.slideEffectPrefab && base.characterBody)
                {
                    Vector3 position = base.characterBody.corePosition;
                    Quaternion rotation = Quaternion.identity;
                    Transform transform = base.FindModelChild("Base");

                    if (transform)
                    {
                        position = transform.position;
                    }

                    if (base.characterDirection)
                    {
                        rotation = Util.QuaternionSafeLookRotation(this.slideRotation * base.characterDirection.forward, Vector3.up);
                    }

                    EffectManager.SimpleEffect(EntityStates.BrotherMonster.BaseSlideState.slideEffectPrefab, position, rotation, false);
                }
            }
            else
            {
                isFlip = true;
                base.characterMotor.disableAirControlUntilCollision = false;

                base.PlayCrossfade("FullBody, Override", "Flip", "Flip.playbackRate", jumpDuration, 0.1f);

                previousAirControl = base.characterMotor.airControl;
                base.characterMotor.airControl = 1f;
                base.characterMotor.velocity.y = 0f;
                base.characterMotor.Motor.ForceUnground();
                base.characterMotor.velocity += Vector3.up * jumpPower;

                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/FeatherEffect"), new EffectData
                {
                    origin = base.characterBody.footPosition
                }, true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.duration / 2f && !sapped)
            {
                sapped = true;
                SapEnemy();
            }

            if (this.isFlip)
            {
                if (base.isAuthority && base.fixedAge >= this.jumpDuration)
                {
                    this.outer.SetNextStateToMain();
                }
            }
            else
            {
                base.characterMotor.velocity = Vector3.zero;
                base.characterMotor.rootMotion = this.slipVector * (this.moveSpeedStat * this.speedCoefficient * Time.fixedDeltaTime) * Mathf.Cos(base.fixedAge / this.duration * 1.57079637f);

                if (base.isAuthority)
                {
                    if (base.characterDirection)
                    {
                        base.characterDirection.forward = this.cachedForward;
                    }
                }

                if (base.isAuthority && base.fixedAge >= this.duration)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        private void SapEnemy()
        {
            HurtBox[] hurtBoxes = new SphereSearch
            {
                origin = base.transform.position,
                radius = 20f,
                mask = LayerIndex.entityPrecise.mask
            }.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(base.characterBody.teamComponent.teamIndex)).OrderCandidatesByDistance().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes();

            if (hurtBoxes.Length > 0)
            {
                CharacterBody victim = hurtBoxes[0].healthComponent.body;
                bool alive = hurtBoxes[0].healthComponent.alive;
                Vector3 position = hurtBoxes[0].transform.position;
                Vector3 forward = victim.corePosition - position;
                float magnitude = forward.magnitude;
                Quaternion rotation = ((magnitude != 0f) ? Util.QuaternionSafeLookRotation(forward) : UnityEngine.Random.rotationUniform);
                ProjectileManager.instance.FireProjectile(SpyAssets.sapperPrefab, position, rotation, base.gameObject, 1f, 100f, base.RollCrit(), DamageColorIndex.Default, null, alive ? (magnitude * 5f) : (-1f));
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = 1f;
                damageInfo.attacker = base.gameObject;
                damageInfo.inflictor = null;
                damageInfo.force = Vector3.zero;
                damageInfo.crit = false;
                damageInfo.procChainMask = default(ProcChainMask);
                damageInfo.procCoefficient = 1f;
                damageInfo.position = position;
                damageInfo.damageColorIndex = DamageColorIndex.Item;
                damageInfo.damageType = DamageType.Shock5s;
                damageInfo.damageType.damageSource = DamageSource.Utility;
                Util.PlaySound("sfx_spy_sapper_plant", victim.gameObject);
                victim.healthComponent.TakeDamage(damageInfo);
            }
        }
        public override void OnExit()
        {
            if (this.isFlip)
            {
                base.PlayAnimation("FullBody, Override", "BufferEmpty");
                base.characterMotor.airControl = previousAirControl;
            }
            else
            {
                base.characterMotor.velocity *= 0.8f;
            }

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}

