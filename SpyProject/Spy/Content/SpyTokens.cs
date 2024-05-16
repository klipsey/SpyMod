using System;
using SpyMod.Modules;
using SpyMod.Spy;
using SpyMod.Spy.Achievements;

namespace SpyMod.Spy.Content
{
    public static class SpyTokens
    {
        public static void Init()
        {
            AddSpyTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Spy.txt");
            //todo guide
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddSpyTokens()
        {
            #region Spy
            string prefix = SpySurvivor.SPY_PREFIX;

            string desc = "The Spy is a squishy melee assassin who excels at killing high priority targets with ease.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Build up stacks of Espionage with Spy's knife and unleash crits with The Diamondback." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Stab is a devestating ability that can instantly kill weaker enemies." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Sap can be a great tool to get into Backstab range while stunning nearby enemies." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Your Deadman's Watch is a powerful tool but be careful to not hold it out for too long." + Environment.NewLine + Environment.NewLine;

            string lore = "Your Mother!";
            string outro = "Seduce me.";
            string outroFailure = "Congratulations, you're a failure.";
            
            Language.Add(prefix + "NAME", "Spy");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "Master of Espionage");
            Language.Add(prefix + "LORE", lore);
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Backstab");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", $"<color=#62746f>Spy</color> can <style=cIsHealth>Backstab</style> enemies with his <color=#62746f>Knife</color> <style=cIsDamage>Critically Striking</style> and instantly <style=cIsHealth>killing</style> weaker enemies.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_REVOLVER_NAME", "The Diamondback");
            Language.Add(prefix + "PRIMARY_REVOLVER_DESCRIPTION", Tokens.spyCritPrefix + $". Fire a bullet for <style=cIsDamage>{100f * SpyStaticValues.revolverDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_KNIFE_NAME", "Stab");
            Language.Add(prefix + "SECONDARY_KNIFE_DESCRIPTION", $"Prepare your <color=#62746f>Knife</color>. Release to swing for <style=cIsDamage>{100f * SpyStaticValues.stabDamageCoefficient}% damage</style>.");

            #endregion

            #region Utility 
            Language.Add(prefix + "UTILITY_FLIP_NAME", "Sap");
            Language.Add(prefix + "UTILITY_FLIP_DESCRIPTION", $"<style=cIsUtility>Dash</style> in a direction or <style=cIsUtility>Flip</style> in the air. Plant a <color=#62746f>Sapper</color> on a nearby " +
                $"enemy, <style=cIsUtility>shocking</style> them for 5 seconds and nearby enemies for 2.5 seconds.");

            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_WATCH_NAME", "Deadman's Watch");
            Language.Add(prefix + "SPECIAL_WATCH_DESCRIPTION", $"Take out your <color=#62746f>Deadman's Watch</color>. Taking <style=cIsDamage>damage</style> turns <color=#62746f>Spy</color> invisible at the cost of <style=cIsHealth>40% HP</style>. " +
                $"While your <color=#62746f>Deadman's Watch</color> is out, <color=#62746f>Spy</color> cannot shoot.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(SpyMasteryAchievement.identifier), "Spy: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(SpyMasteryAchievement.identifier), "As Spy, beat the game or obliterate on Monsoon.");

            Language.Add(Tokens.GetAchievementNameToken(SpyUnlockAchievement.identifier), "");
            Language.Add(Tokens.GetAchievementDescriptionToken(SpyUnlockAchievement.identifier), "");

            #endregion

            #endregion
        }
    }
}