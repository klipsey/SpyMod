using BepInEx.Configuration;
using SpyMod.Modules;

namespace SpyMod.Spy.Content
{
    public static class SpyConfig
    {
        public static ConfigEntry<bool> forceUnlock;

        public static ConfigEntry<float> maxHealth;
        public static ConfigEntry<float> healthRegen;
        public static ConfigEntry<float> armor;
        public static ConfigEntry<float> shield;

        public static ConfigEntry<int> jumpCount;

        public static ConfigEntry<float> damage;
        public static ConfigEntry<float> attackSpeed;
        public static ConfigEntry<float> crit;

        public static ConfigEntry<float> moveSpeed;
        public static ConfigEntry<float> acceleration;
        public static ConfigEntry<float> jumpPower;

        public static ConfigEntry<bool> autoCalculateLevelStats;

        public static ConfigEntry<float> healthGrowth;
        public static ConfigEntry<float> regenGrowth;
        public static ConfigEntry<float> armorGrowth;
        public static ConfigEntry<float> shieldGrowth;

        public static ConfigEntry<float> damageGrowth;
        public static ConfigEntry<float> attackSpeedGrowth;
        public static ConfigEntry<float> critGrowth;

        public static ConfigEntry<float> moveSpeedGrowth;
        public static ConfigEntry<float> jumpPowerGrowth;

        public static ConfigEntry<float> stabDamageCoefficient;
        public static ConfigEntry<float> cloakHealthCost;
        public static ConfigEntry<float> bigEarnerHealthPunishment;
        public static ConfigEntry<bool> bigEarnerFullyResets;
        public static ConfigEntry<float> sapperRange;
        public static ConfigEntry<float> spyBackstabMultiplier;

        public static ConfigEntry<float> revolverDamageCoefficient;

        public static ConfigEntry<float> diamondBackCritBonus;

        public static ConfigEntry<float> ambassadorDamageCoefficient;

        public static ConfigEntry<float> bigEarnerGracePeriod;

        public static ConfigEntry<float> maxCloakDurationDefault;

        public static ConfigEntry<float> maxCloakDurationDead;

        public static ConfigEntry<float> maxChainStabCombo;
        public static void Init()
        {
            string section = "Stats - 01";
            string section2 = "QOL - 02";

            damage = Config.BindAndOptions(section, "Change Base Damage Value", 12f);

            maxHealth = Config.BindAndOptions(section, "Change Max Health Value", 100f);
            healthRegen = Config.BindAndOptions(section, "Change Health Regen Value", 1.2f);
            armor = Config.BindAndOptions(section, "Change Armor Value", 0f);
            shield = Config.BindAndOptions(section, "Change Shield Value", 0f);

            jumpCount = Config.BindAndOptions(section, "Change Jump Count", 1);

            attackSpeed = Config.BindAndOptions(section, "Change Attack Speed Value", 1f);
            crit = Config.BindAndOptions(section, "Change Crit Value", 1f);

            moveSpeed = Config.BindAndOptions(section, "Change Move Speed Value", 7f);
            acceleration = Config.BindAndOptions(section, "Change Acceleration Value", 80f);
            jumpPower = Config.BindAndOptions(section, "Change Jump Power Value", 15f);

            autoCalculateLevelStats = Config.BindAndOptions(section, "Auto Calculate Level Stats", true);

            healthGrowth = Config.BindAndOptions(section, "Change Health Growth Value", 0.35f);
            regenGrowth = Config.BindAndOptions(section, "Change Regen Growth Value", 0.2f);
            armorGrowth = Config.BindAndOptions(section, "Change Armor Growth Value", 0f);
            shieldGrowth = Config.BindAndOptions(section, "Change Shield Growth Value", 0f);

            damageGrowth = Config.BindAndOptions(section, "Change Damage Growth Value", 0.2f);
            attackSpeedGrowth = Config.BindAndOptions(section, "Change Attack Speed Growth Value", 0f);
            critGrowth = Config.BindAndOptions(section, "Change Crit Growth Value", 0f);

            moveSpeedGrowth = Config.BindAndOptions(section, "Change Move Speed Growth Value", 0f);
            jumpPowerGrowth = Config.BindAndOptions(section, "Change Jump Power Growth Value", 0f);

            revolverDamageCoefficient = Config.BindAndOptionsSlider(
                section2,
                "Revolver Damage Coefficient",
                3.2f,
                "Change revolver damage coefficient. Default 3.2.",
                0f,
                10f,
                true
            );

            diamondBackCritBonus = Config.BindAndOptionsSlider(
                section2,
                "Diamondback Crit Bonus",
                2f,
                "Change Diamondback crit bonus. Default 2 (200%).",
                0f,
                1f,
                true
            );

            ambassadorDamageCoefficient = Config.BindAndOptionsSlider(
                section2,
                "Ambassador Damage Coefficient",
                2.6f,
                "Change Ambassador damage coefficient. Default 0.85x.",
                0f,
                10f,
                true
            );

            bigEarnerGracePeriod = Config.BindAndOptionsSlider(
                section2,
                "Big Earner Grace Period",
                2f,
                "Change Big Earner grace period in seconds. Default 2s.",
                0f,
                10f,
                true
            );

            stabDamageCoefficient = Config.BindAndOptionsSlider(
                section2,
                "Stab Damage",
                5f,
                "Change stab damage coefficient. Default 5.",
                0.01f,
                5f,
                true
            );

            spyBackstabMultiplier = Config.BindAndOptionsSlider(
                section2,
                "Backstab Multiplier",
                5f,
                "Change spies backstab multiplier. Default 5x.",
                0f,
                9999f,
                false
            );

            maxCloakDurationDefault = Config.BindAndOptionsSlider(
                section2,
                "Max Cloak Duration",
                8f,
                "Change default max cloak duration. Default 8s.",
                1f,
                500f,
                true
            );

            maxCloakDurationDead = Config.BindAndOptionsSlider(
                section2,
                "Max Cloak Duration",
                6f,
                "Change max cloak duration for Deadmans watch. Default 6s.",
                1f,
                500f,
                true
            );

            cloakHealthCost = Config.BindAndOptionsSlider(
                section2,
                "Deadmans Cloak Max Damage Taken",
                0.25f,
                "Change maximum health cost for Deadman's Cloak. EX: 0.25 = 25% HP max.",
                0.01f,
                0.99f,
                true
            );

            bigEarnerHealthPunishment = Config.BindAndOptionsSlider(
                section2,
                "Big Earner Health Punishment",
                0.25f,
                "Change health punishment for Big Earner. EX: 0.25 = 25% max HP.",
                0.01f,
                0.99f,
                true
            );

            bigEarnerFullyResets = Config.BindAndOptions(
                section2,
                "Big Earner Fully Resets",
                false,
                "Enable or disable full reset on backstab for Big Earner.",
                true
            );

            sapperRange = Config.BindAndOptionsSlider(
                section2,
                "Sapper Range",
                5f,
                "Change sapper range. Default 5.",
                1f,
                500f,
                true
            );

            maxChainStabCombo = Config.BindAndOptionsSlider(
                section2,
                "Max Chain Stab Combo",
                3f,
                "Change max chain stab combo count. Default 3.",
                0f,
                10f,
                true
            );
            spyBackstabMultiplier = Config.BindAndOptionsSlider(section2, "Backstab Multiplier", 5f, "Change spies backstab multiplier. Default 5x.", 0f, 9999f, false);

            //add more here or else you're cringe
            forceUnlock = Config.BindAndOptions(
                section2,
                "Unlock Spy",
                false,
                "Unlock Spy.", true);
        }
    }
}
