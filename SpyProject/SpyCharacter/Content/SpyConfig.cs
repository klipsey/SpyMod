using BepInEx.Configuration;
using SpyMod.Modules;

namespace SpyMod.Spy.Content
{
    public static class SpyConfig
    {
        public static ConfigEntry<bool> forceUnlock;
        public static ConfigEntry<float> stabDamageCoefficient;
        public static ConfigEntry<float> maxCloakDefault;
        public static ConfigEntry<float> cloakHealthCost;
        public static ConfigEntry<float> maxCloakDead;
        public static ConfigEntry<float> bigEarnerHealthPunishment;
        public static ConfigEntry<bool> bigEarnerFullyResets;
        public static ConfigEntry<float> sapperRange;
        public static ConfigEntry<float> spyBackstabMultiplier;
        public static void Init()
        {
            string section = "01 - General";
            string section2 = "02 - Stats";

            //add more here or else you're cringe
            forceUnlock = Config.BindAndOptions(
                section,
                "Unlock Spy",
                false,
                "Unlock Spy.", true);

            stabDamageCoefficient = Config.BindAndOptionsSlider(section2, "Stab Damage", SpyStaticValues.stabDamageCoefficient, "", 0.01f, 5f, true);

            maxCloakDefault = Config.BindAndOptionsSlider(section2, "Cloak Max Cloak Duration", SpyStaticValues.maxCloakDurationDefault, "", 1f, 500f, true);

            cloakHealthCost = Config.BindAndOptionsSlider(section2, "Deadmans Cloak Max Damage Taken", SpyStaticValues.cloakHealthCost, "EX: 0.25 = 25% HP max", 0.01f, 0.99f, true);

            maxCloakDead = Config.BindAndOptionsSlider(section2, "Deadmans Cloak Duration", SpyStaticValues.maxCloakDurationDead, "", 1f, 500f, true);

            bigEarnerHealthPunishment = Config.BindAndOptionsSlider(section2, "Big Earner Health Punishment", SpyStaticValues.bigEarnerHealthPunishment, "EX: 0.25 = 25% max HP", 0.01f, 0.99f, true);

            bigEarnerFullyResets = Config.BindAndOptions(section2, "Big Earner Fully Resets", false, "Grants an actual reset on backstab", true);

            sapperRange = Config.BindAndOptionsSlider(section2, "Sapper Range", SpyStaticValues.sapperRange, "", 1f, 500f, true);

            spyBackstabMultiplier = Config.BindAndOptionsSlider(section2, "Backstab Multiplier", 5f, "Change spies backstab multiplier. Default 5x.", 0f, 9999f, false);
        }
    }
}
