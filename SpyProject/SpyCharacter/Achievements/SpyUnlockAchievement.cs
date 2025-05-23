﻿using R2API;
using RoR2;
using RoR2.Achievements;
using SpyMod.Spy;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;

namespace SpyMod.Spy.Achievements
{
    [RegisterAchievement(identifier, unlockableIdentifier, null, 3u, null)]
    public class SpyUnlockAchievement : BaseAchievement
    {
        public const string identifier = SpySurvivor.SPY_PREFIX + "UNLOCK_ACHIEVEMENT";
        public const string unlockableIdentifier = SpySurvivor.SPY_PREFIX + "UNLOCK_ACHIEVEMENT";

        public override void OnInstall()
        {
            base.OnInstall();

            On.RoR2.HealthComponent.TakeDamageProcess += new On.RoR2.HealthComponent.hook_TakeDamageProcess(Check);
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            On.RoR2.HealthComponent.TakeDamageProcess -= new On.RoR2.HealthComponent.hook_TakeDamageProcess(Check);
        }

        private void Check(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig.Invoke(self, damageInfo);
            if (!self.alive || self.godMode || self.ospTimer > 0f)
            {
                return;
            }
            if (damageInfo.attacker)
            {
                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if(attackerBody)
                {
                    if (damageInfo.procChainMask.HasProc(ProcType.Backstab) && damageInfo.crit == true && attackerBody.canPerformBackstab)
                    {
                        base.Grant();
                    }
                }
            }

        }
    }
}
