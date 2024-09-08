using UnityEngine;
using EntityStates;
using SpyMod.Modules.BaseStates;
using RoR2;
using UnityEngine.AddressableAssets;
using SpyMod.Spy.Content;
using UnityEngine.Networking;

namespace SpyMod.Spy.SkillStates
{
    public class Cloak : BaseSpySkillState
    {
        private float baseDuration = 1f;
        private float duration;
        private bool willWait = false;

        protected virtual bool sceptered
        {
            get
            {
                return false;
            }
        }
        public override void OnEnter()
        {
            RefreshState();
            base.OnEnter();

            if (!sceptered) duration = baseDuration / attackSpeedStat;
            else duration = 0f;
            if (!this.spyController.IsStopWatchOut())
            {
                if(this.spyController.cloakTimer >= (this.spyController.maxCloakTimer * 0.2f))
                {
                    if (NetworkServer.active)
                    {
                        characterBody.AddBuff(SpyBuffs.spyWatchDebuff);
                        characterBody.AddBuff(RoR2Content.Buffs.Cloak);
                        characterBody.AddBuff(RoR2Content.Buffs.CloakSpeed);
                        characterBody.AddBuff(SpyBuffs.armorBuff);
                    }
                    this.spyController.EnableWatchLayer();
                    this.spyController.EnterStealth();
                }
            }
            else
            {
                Util.PlaySound("sfx_spy_uncloak_alt", base.gameObject);
                willWait = true;
                this.spyController.pauseTimer = true;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (willWait)
            {
                this.spyController.pauseTimer = false;
                this.spyController.ExitStealth();
                this.spyController.DisableWatchLayer();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}