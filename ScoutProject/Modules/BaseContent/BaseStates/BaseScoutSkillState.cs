﻿using EntityStates;
using RoR2;
using ScoutMod.Scout.Components;
using ScoutMod.Scout.Content;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace ScoutMod.Modules.BaseStates
{
    public abstract class BaseScoutSkillState : BaseSkillState
    {
        protected ScoutController scoutController;

        protected bool isAtomic;
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
            if (!scoutController)
            {
                scoutController = base.GetComponent<ScoutController>();
            }
            if (scoutController)
            {
                isAtomic = scoutController.atomicDraining;
            }
        }
    }
}
