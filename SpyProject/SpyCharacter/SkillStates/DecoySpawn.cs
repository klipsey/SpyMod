using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace SpyMod.Spy.SkillStates
{
    public class DecoySpawn : BaseState
    {
        private float durationBeforeDeath = 0.5f;
        public override void OnEnter()
        {
            base.OnEnter();
            durationBeforeDeath = 0.5f;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= durationBeforeDeath && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if(NetworkServer.active)
            {
                DamageInfo executeDamage = new DamageInfo();
                executeDamage.damage = this.healthComponent.fullCombinedHealth * 2f;
                executeDamage.attacker = null;
                executeDamage.canRejectForce = false;
                executeDamage.crit = true;
                executeDamage.inflictor = null;
                executeDamage.damageColorIndex = DamageColorIndex.Default;
                executeDamage.damageType = DamageType.BypassArmor | DamageType.BypassArmor | DamageType.BypassOneShotProtection | DamageType.Silent;
                executeDamage.force = Vector3.zero;
                executeDamage.rejected = false;
                executeDamage.position = base.characterBody.corePosition;
                executeDamage.procChainMask = default(ProcChainMask);
                executeDamage.procCoefficient = 0f;
                base.healthComponent.TakeDamage(executeDamage);
            }
            base.OnExit();
        }

    }
}
