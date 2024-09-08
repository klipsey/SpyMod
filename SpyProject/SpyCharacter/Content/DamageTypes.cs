using HG;
using Newtonsoft.Json.Linq;
using R2API;
using RoR2;
using RoR2.Projectile;
using SpyMod.Modules;
using SpyMod.Spy.Components;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using static RoR2.DotController;

namespace SpyMod.Spy.Content
{
    public static class DamageTypes
    {
        public static DamageAPI.ModdedDamageType Default;
        public static DamageAPI.ModdedDamageType SpyBackStab;
        public static DamageAPI.ModdedDamageType SpyExecute;
        public static DamageAPI.ModdedDamageType BigEarner;
        internal static void Init()
        {
            Default = DamageAPI.ReserveDamageType();
            SpyBackStab = DamageAPI.ReserveDamageType();
            SpyExecute = DamageAPI.ReserveDamageType();
            BigEarner = DamageAPI.ReserveDamageType();
            Hook();
        }
        private static void Hook()
        {
            RoR2.GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
        }

        private static void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            DamageInfo damageInfo = damageReport.damageInfo;
            if (!damageReport.attackerBody || !damageReport.victimBody)
            {
                return;
            }
            HealthComponent victim = damageReport.victim;
            GameObject inflictorObject = damageInfo.inflictor;
            CharacterBody victimBody = damageReport.victimBody;
            EntityStateMachine victimMachine = victimBody.GetComponent<EntityStateMachine>();
            CharacterBody attackerBody = damageReport.attackerBody;
            GameObject attackerObject = damageReport.attacker.gameObject;
            if (NetworkServer.active)
            {
                if(damageInfo.HasModdedDamageType(SpyExecute) && (damageInfo.damageType & DamageType.DoT) != DamageType.DoT)
                {
                    if(victim.health <= victim.fullCombinedHealth * 0.2f)
                    {
                        DamageInfo spyExecuteDamage = new DamageInfo();
                        spyExecuteDamage.attacker = attackerObject;
                        spyExecuteDamage.inflictor = attackerObject;
                        spyExecuteDamage.damage = victim.health;
                        spyExecuteDamage.procCoefficient = 0f;
                        spyExecuteDamage.crit = true;
                        spyExecuteDamage.damageType = DamageType.DoT | DamageType.BonusToLowHealth | DamageType.BypassBlock | DamageType.BypassArmor;
                        spyExecuteDamage.AddModdedDamageType(SpyExecute);
                        spyExecuteDamage.canRejectForce = false;
                        spyExecuteDamage.damageColorIndex = DamageColorIndex.SuperBleed;
                        spyExecuteDamage.force = Vector3.zero;
                        spyExecuteDamage.dotIndex = DotController.DotIndex.None;
                        spyExecuteDamage.position = victimBody.corePosition;

                        victim.TakeDamage(spyExecuteDamage);
                    }
                }
            }
        }
    }
}
