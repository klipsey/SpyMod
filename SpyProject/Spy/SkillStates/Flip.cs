using EntityStates;
using R2API;
using RoR2;
using SpyMod.Modules;
using SpyMod.Modules.BaseStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SpyMod.Spy.SkillStates
{
    public class Flip : BaseSpySkillState
    {
        public float jumpDuration = 0.8f;
        public static float inAirDuration = 0f;
        public static float jumpPower = 15f;
        public static float minDampingStrength = 0.2f;
        public static float maxDampingStrength = 0.05f;

        private float previousAirControl;

        protected Vector3 slipVector = Vector3.zero;
        public float duration = 0.3f;
        public float speedCoefficient = 7f;
        private Vector3 cachedForward;

        private bool isFlip = false;

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

                base.PlayCrossfade("FullBody, Override", "Dash", "Dash.playbackRate", this.duration * 1.5f, 0.05f);
                base.PlayAnimation("Gesture, Override", "BufferEmpty");

                Util.PlaySound("sfx_driver_dash", this.gameObject);
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
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(this.isFlip)
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

        public override void OnExit()
        {
            if (isFlip)
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
            if (this.isFlip) return InterruptPriority.Skill;
            else return InterruptPriority.Frozen;
        }
    }
}

