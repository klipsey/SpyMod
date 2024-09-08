using UnityEngine;
using EntityStates;
using SpyMod.Modules.BaseStates;
using RoR2;
using UnityEngine.AddressableAssets;
using SpyMod.Spy.Content;
using UnityEngine.Networking;

namespace SpyMod.Spy.SkillStates
{
    public class SwapWatch : BaseSpySkillState
    {
        public override void OnEnter()
        {
            RefreshState();
            base.OnEnter();
            Util.PlaySound("sfx_scout_swap_weapon", this.gameObject);
            
            if (!this.spyController.IsStopWatchOut())
            {
                if (NetworkServer.active) characterBody.AddBuff(SpyBuffs.spyWatchDebuff);
                this.spyController.EnableWatchLayer();
            }
            else
            {
                if (NetworkServer.active) characterBody.RemoveBuff(SpyBuffs.spyWatchDebuff);
                this.spyController.DisableWatchLayer();
            }

            if (base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }
    }
}