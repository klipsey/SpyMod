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
        public static float jumpDuration = 0.6f;
        public static float inAirDuration = 0f;
        public static float jumpPower = 15f;
        public static float minDampingStrength = 0.2f;
        public static float maxDampingStrength = 0.05f;

        private float previousAirControl;
        private float stopwatch;

        private float dampingStrength;
        private float dampingVelocity;

        public override void OnEnter()
        {
            base.OnEnter();

            dampingStrength = minDampingStrength;

            base.characterMotor.disableAirControlUntilCollision = false;

            base.PlayCrossfade("FullBody, Override", "Flip", "Flip.playbackRate", jumpDuration, 0.1f);

            previousAirControl = base.characterMotor.airControl;
            base.characterMotor.airControl = 1f;
            base.characterMotor.velocity.y = 0f;
            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.velocity += Vector3.up * jumpPower;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= jumpDuration)
            {
                stopwatch += Time.fixedDeltaTime;
                if (base.characterMotor.velocity.y <= 0f)
                {
                    base.characterMotor.velocity.y *= 0.2f;
                }
            }

            if (stopwatch >= jumpDuration || characterMotor.isGrounded && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {

            base.PlayAnimation("FullBody, Override", "BufferEmpty");

            base.characterMotor.airControl = previousAirControl;

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}

