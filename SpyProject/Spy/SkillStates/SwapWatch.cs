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
            
            if (!this.spyController.stopwatchOut)
            {
                this.spyController.EnableWatchLayer();
            }
            else
            {
                this.spyController.DisableWatchLayer();
            }

            if (base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }
    }
}