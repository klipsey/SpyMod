using BepInEx.Configuration;
using SpyMod.Modules;

namespace SpyMod.Spy.Content
{
    public static class SpyConfig
    {
        public static ConfigEntry<bool> forceUnlock;
        public static ConfigEntry<bool> gainAtomicGaugeDuringAtomicBlast;

        public static void Init()
        {
            string section = "Spy";

            //add more here or else you're cringe
            forceUnlock = Config.BindAndOptions(
                section,
                "Unlock Spy",
                false,
                "Unlock Spy.", true);

            gainAtomicGaugeDuringAtomicBlast = Config.BindAndOptions(
                section,
                "Gain Gauge During Atomic",
                false,
                "Lets you fill Atomic Core while it drains.", false);
        }
    }
}
