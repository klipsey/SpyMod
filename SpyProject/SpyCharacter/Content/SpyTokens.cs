using System;
using SpyMod.Modules;
using SpyMod.Spy;
using SpyMod.Spy.Achievements;
using UnityEngine.UIElements;

namespace SpyMod.Spy.Content
{
    public static class SpyTokens
    {
        public static void Init()
        {
            AddSpyTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            Language.PrintOutput("Spy.txt");
            Language.usingLanguageFolder = true;    
            Language.printingEnabled = true;
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
            desc = desc + "< ! > Cloak can be a great way to both engage and escape enemies but requires proper resource management." + Environment.NewLine + Environment.NewLine;

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
            Language.Add(prefix + "PASSIVE_DESCRIPTION", $"<color=#62746f>Spy</color> can <style=cIsHealth>Backstab</style> enemies with his <color=#62746f>Knife</color>, " +
                $"causing a <style=cIsDamage>Critical Strike</style> that instantly <style=cIsHealth>kills</style> weaker enemies.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_REVOLVER_NAME", "Diamondback");
            Language.Add(prefix + "PRIMARY_REVOLVER_DESCRIPTION", $"Fire a bullet for <style=cIsDamage>{100f * SpyStaticValues.revolverDamageCoefficient}% damage</style>. " +
                $"<style=cIsHealth>Backstab</style> kills grant <color=#62746f>Espionage</color>. Max 5 stacks.");

            Language.Add(prefix + "PRIMARY_REVOLVER2_NAME", "Ambassador");
            Language.Add(prefix + "PRIMARY_REVOLVER2_DESCRIPTION", $"Fire a bullet for <style=cIsDamage>{100f * SpyStaticValues.ambassadorDamageCoefficient}% damage</style>. Landing a headshot <style=cIsDamage>Critically Strikes</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_KNIFE_NAME", "Stab");
            Language.Add(prefix + "SECONDARY_KNIFE_DESCRIPTION", $"Prepare your <color=#62746f>Knife</color>. Release to swing for <style=cIsDamage>{100f * SpyConfig.stabDamageCoefficient.Value}% damage</style>.");

            Language.Add(prefix + "SECONDARY_KNIFE2_NAME", "Big Earner");
            Language.Add(prefix + "SECONDARY_KNIFE2_DESCRIPTION", Tokens.spyBigEarnerPrefix + $". Prepare your <color=#62746f>Knife</color>. Release to swing for <style=cIsDamage>{100f * SpyConfig.stabDamageCoefficient.Value}% damage</style>. " +
                $"<style=cIsHealth>Backstab</style> kills grant <style=cIsUtility>movement speed</style> and <style=cIsHealing>barrier</style> as well as <style=cIsUtility>resetting this abilities cooldown</style> for a short period.");
            #endregion

            #region Utility 
            Language.Add(prefix + "UTILITY_FLIP_NAME", "Sap");
            Language.Add(prefix + "UTILITY_FLIP_DESCRIPTION", $"<style=cIsUtility>Dash</style> in a direction or <style=cIsUtility>Flip</style> in the air. Plant a <color=#62746f>Sapper</color> on a nearby " +
                $"enemy, <style=cIsUtility>shocking</style> them and nearby enemies for <style=cIsUtility>5 seconds</style>.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_WATCH_NAME", "Cloak");
            Language.Add(prefix + "SPECIAL_WATCH_DESCRIPTION", $"Become <style=cIsUtility>cloaked</style> for up to <style=cIsUtility>{SpyConfig.maxCloakDefault.Value} seconds</style>. " +
                $"While <style=cIsUtility>cloaked</style>, <color=#62746f>Spy</color> cannot shoot.");

            Language.Add(prefix + "SPECIAL_WATCH2_NAME", "Deadman's Watch");
            Language.Add(prefix + "SPECIAL_WATCH2_DESCRIPTION", $"Take out your <color=#62746f>Deadman's Watch</color>. Taking <style=cIsDamage>damage</style> grants <style=cIsUtility>invisiblity</style> for <style=cIsUtility>{SpyConfig.maxCloakDead.Value} seconds</style>" +
                $" at the cost of up to <style=cIsHealth>{100f * SpyConfig.cloakHealthCost.Value}% HP</style>. " +
                $"While your <color=#62746f>Deadman's Watch</color> is out, <color=#62746f>Spy</color> cannot shoot.");

            Language.Add(prefix + "SPECIAL_SCEPTER_WATCH_NAME", "Cloak");
            Language.Add(prefix + "SPECIAL_SCEPTER_WATCH_DESCRIPTION", $"Become <style=cIsUtility>cloaked</style> for up to <style=cIsUtility>{SpyConfig.maxCloakDefault.Value} seconds</style>. " +
                $"While <style=cIsUtility>cloaked</style>, <color=#62746f>Spy</color> cannot shoot." + Tokens.ScepterDescription("Decloak instantly."));

            Language.Add(prefix + "SPECIAL_SCEPTER_WATCH2_NAME", "Deadman's Watch Scepter");
            Language.Add(prefix + "SPECIAL_SCEPTER_WATCH2_DESCRIPTION", $"Take out your <color=#62746f>Deadman's Watch</color>. Taking <style=cIsDamage>damage</style> grants <style=cIsUtility>invisiblity</style> for <style=cIsUtility>{SpyConfig.maxCloakDead.Value} seconds</style>" +
                $" at the cost of up to <style=cIsHealth>{100f * SpyConfig.cloakHealthCost.Value}% HP</style>. " +
                $"While your <color=#62746f>Deadman's Watch</color> is out, <color=#62746f>Spy</color> cannot shoot." + Tokens.ScepterDescription("<style=cIsHealth>Backstab</style> kills <style=cIsUtility>reset</style> <color=#62746f>Deadman's Watch</color> and the <style=cIsHealth>HP</style> cost is removed."));
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(SpyMasteryAchievement.identifier), "Spy: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(SpyMasteryAchievement.identifier), "As Spy, beat the game or obliterate on Monsoon.");
            /*
            Language.Add(Tokens.GetAchievementNameToken(SpyUnlockAchievement.identifier), "Dressed to Kill");
            Language.Add(Tokens.GetAchievementDescriptionToken(SpyUnlockAchievement.identifier), "Get a Backstab.");
            */
            #endregion

            #endregion
        }
    }
}