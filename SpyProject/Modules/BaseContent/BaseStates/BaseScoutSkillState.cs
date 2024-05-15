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
    public abstract class BaseSpySkillState : BaseSkillState
    {
        protected SpyController spyController;
        public virtual void AddRecoil2(float x1, float x2, float y1, float y2)
        {
            this.AddRecoil(x1, x2, y1, y2);
        }
        public override void OnEnter()
        {
            RefreshState();
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
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
