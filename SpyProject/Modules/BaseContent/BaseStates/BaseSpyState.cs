using EntityStates;
using RoR2;
using SpyMod.Spy.Components;
using SpyMod.Spy.Content;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace SpyMod.Modules.BaseStates
{
    public abstract class BaseSpyState : BaseState
    {
        protected SpyController spyController;

        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            RefreshState();
        }
        protected void RefreshState()
        {
            if (!spyController)
            {
                spyController = base.GetComponent<SpyController>();
            }
        }
    }
}
