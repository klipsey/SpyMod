﻿using SpyMod.Modules.BaseStates;
using SpyMod.Spy.SkillStates;

namespace SpyMod.Spy.Content
{
    public static class SpyStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(BaseSpySkillState));
            Modules.Content.AddEntityState(typeof(MainState));
            Modules.Content.AddEntityState(typeof(BaseSpyState));
            Modules.Content.AddEntityState(typeof(Shoot));
            Modules.Content.AddEntityState(typeof(Shoot2));
            Modules.Content.AddEntityState(typeof(ChargeKnife));
            Modules.Content.AddEntityState(typeof(Stab));
            Modules.Content.AddEntityState(typeof(SwapWatch));
            Modules.Content.AddEntityState(typeof(SwapWatchScepter));
            Modules.Content.AddEntityState(typeof(Cloak));
            Modules.Content.AddEntityState(typeof(CloakScepter));
            Modules.Content.AddEntityState(typeof(Sap));
            Modules.Content.AddEntityState(typeof(DecoySpawn));
        }
    }
}
