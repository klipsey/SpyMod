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
        internal static void Init()
        {
            Default = DamageAPI.ReserveDamageType();
            SpyBackStab = DamageAPI.ReserveDamageType();
            SpyExecute = DamageAPI.ReserveDamageType();
            Hook();
        }
        private static void Hook()
        {
        }
    }
}
