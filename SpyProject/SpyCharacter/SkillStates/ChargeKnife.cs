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
            base.PlayCrossfade("Gesture, Override", "ChargeStab", "Swing.playbackRate", 0.3f, 0.05f);
            Util.PlaySound("sfx_spy_knife_equip", this.gameObject);
            this.FindModelChild("Revolver").gameObject.SetActive(false);
            this.FindModelChild("Knife").gameObject.SetActive(true);
            if (this.characterBody.HasBuff(SpyBuffs.spyDiamondbackBuff))
            {
                this.spyController.DeactivateCritLightning(true);
            }
            this.spyController.SpinGun();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            base.characterBody.SetAimTimer(2f);

            if (base.isAuthority)
            {
                if (!IsKeyDownAuthority() && base.fixedAge >= 0.05)
                {
                    this.outer.SetNextState(new Stab());
                    return;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
