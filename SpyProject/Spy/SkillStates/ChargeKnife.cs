using UnityEngine;
using RoR2;
using EntityStates;
using SpyMod.Modules.BaseStates;
using SpyMod.Spy.Content;

namespace SpyMod.Spy.SkillStates
{
    public class ChargeKnife : BaseSpySkillState
    {
        public override void OnEnter()
        {
            RefreshState();
            base.OnEnter();
            base.PlayCrossfade("Gesture, Override", "ChargeStab", "Swing.playbackRate", 0.3f, 0.1f);
            Util.PlaySound("sfx_spy_knife_equip", this.gameObject);
            this.FindModelChild("Revolver").gameObject.SetActive(false);
            this.FindModelChild("Knife").gameObject.SetActive(true);
            if(this.characterBody.HasBuff(SpyBuffs.spyDiamondbackBuff)) this.spyController.DeactivateCritLightning();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                if (!this.inputBank.skill2.down && base.fixedAge >= 0.1f)
                {
                    this.outer.SetNextState(new Stab());
                    return;
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
