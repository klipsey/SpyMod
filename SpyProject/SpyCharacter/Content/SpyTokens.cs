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

            string lore = "As the rapid development of space empires surged across the stars, so too did the silent wars that burn between them. " +
                "Diplomatic facades masked the tension, but beneath the surface hides fierce political strife, alliances fracturing, and ideologies " +
                "clashing in the silence of space. " + Environment.NewLine + Environment.NewLine +
                "In this age of interstellar ambition, open conflict risked total annihilation. Leaders turned to the shadows to carry out their will instead. " +
                "Espionage became the weapon of choice—a silent war waged in data streams, whispered codes, and stolen secrets. " +
                "Entire civilizations were swayed not by fleets or firepower, but by a well-placed whisper, a corrupted protocol, a missing official." + Environment.NewLine + Environment.NewLine +
                "Spies became ghosts in the machinery of empire. Masters of deception, infiltration, and subversion, they slipped between borders unnoticed, " +
                "cutting into empires with surgical precision. Some served sovereigns. Others served ideals. A few served only themselves." + Environment.NewLine + Environment.NewLine +
                "Their victories are never paraded. Their names are never spoken. Their missions, buried by design. And yet, the rise and fall of entire star-nations have " +
                "turned on the actions of a few, unseen agents." + Environment.NewLine + Environment.NewLine +
                "A good spy is never caught. The impact they leave on history is untraceable, " +
                "but in the quiet shift of power, in the moment an empire rises or crumbles, their hand is always there, unseen... but undeniable.";

            string outro = "..and so he left, another mission completed.";
            string outroFailure = "..and so he vanished, without a trace.";
            
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
            Language.Add(prefix + "PRIMARY_REVOLVER_DESCRIPTION", $"Fire a bullet for " +
                $"<style=cIsDamage>{100f * SpyConfig.revolverDamageCoefficient.Value}% damage</style>. " +
                $"<style=cIsHealth>Backstab</style> kills grant <color=#62746f>Espionage</color>. Max 5 stacks.");

            Language.Add(prefix + "PRIMARY_REVOLVER2_NAME", "Ambassador");
            Language.Add(prefix + "PRIMARY_REVOLVER2_DESCRIPTION", $"Fire a bullet for " +
                $"<style=cIsDamage>{100f * SpyConfig.ambassadorDamageCoefficient.Value}% damage</style>." +
                $" Landing a headshot <style=cIsDamage>Critically Strikes</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_KNIFE_NAME", "Stab");
            Language.Add(prefix + "SECONDARY_KNIFE_DESCRIPTION", $"Prepare your <color=#62746f>Knife</color>. Release to swing for " +
                $"<style=cIsDamage>{100f * SpyConfig.stabDamageCoefficient.Value}% damage</style>.");

            Language.Add(prefix + "SECONDARY_KNIFE2_NAME", "Big Earner");
            Language.Add(prefix + "SECONDARY_KNIFE2_DESCRIPTION", Tokens.spyBigEarnerPrefix + $". Prepare your <color=#62746f>Knife</color>. " +
                $"Release to swing for <style=cIsDamage>{100f * SpyConfig.stabDamageCoefficient.Value}% damage</style>. " +
                $"<style=cIsHealth>Backstab</style> kills grant <style=cIsUtility>movement speed</style> and " +
                $"<style=cIsHealing>barrier</style> as well as <style=cIsUtility>resetting this abilities cooldown</style> for a short period.");
            #endregion

            #region Utility 
            Language.Add(prefix + "UTILITY_FLIP_NAME", "Sap");
            Language.Add(prefix + "UTILITY_FLIP_DESCRIPTION", $"<style=cIsUtility>Dash</style> in a direction or <style=cIsUtility>Flip</style> in the air. " +
                $"Plant a <color=#62746f>Sapper</color> on a nearby " +
                $"enemy, <style=cIsUtility>shocking</style> them and nearby enemies for <style=cIsUtility>5 seconds</style>.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_WATCH_NAME", "Cloak");
            Language.Add(prefix + "SPECIAL_WATCH_DESCRIPTION", $"Become <style=cIsUtility>cloaked</style> for up to " +
                $"<style=cIsUtility>{SpyConfig.maxCloakDurationDefault.Value} seconds</style>. " +
                $"While <style=cIsUtility>cloaked</style>, <color=#62746f>Spy</color> cannot shoot.");

            Language.Add(prefix + "SPECIAL_WATCH2_NAME", "Deadman's Watch");
            Language.Add(prefix + "SPECIAL_WATCH2_DESCRIPTION", $"Take out your <color=#62746f>Deadman's Watch</color>. " +
                $"Taking <style=cIsDamage>damage</style> grants <style=cIsUtility>invisiblity</style> for " +
                $"<style=cIsUtility>{SpyConfig.maxCloakDurationDead.Value} seconds</style>" +
                $" at the cost of up to <style=cIsHealth>{100f * SpyConfig.cloakHealthCost.Value}% HP</style>. " +
                $"While your <color=#62746f>Deadman's Watch</color> is out, <color=#62746f>Spy</color> cannot shoot.");

            Language.Add(prefix + "SPECIAL_SCEPTER_WATCH_NAME", "Cloak");
            Language.Add(prefix + "SPECIAL_SCEPTER_WATCH_DESCRIPTION", $"Become <style=cIsUtility>cloaked</style> for up to " +
                $"<style=cIsUtility>{SpyConfig.maxCloakDurationDefault.Value} seconds</style>. " +
                $"While <style=cIsUtility>cloaked</style>, <color=#62746f>Spy</color> cannot shoot." + Tokens.ScepterDescription("Decloak instantly."));

            Language.Add(prefix + "SPECIAL_SCEPTER_WATCH2_NAME", "Deadman's Watch Scepter");
            Language.Add(prefix + "SPECIAL_SCEPTER_WATCH2_DESCRIPTION", $"Take out your <color=#62746f>Deadman's Watch</color>. " +
                $"Taking <style=cIsDamage>damage</style> grants <style=cIsUtility>invisiblity</style> for " +
                $"<style=cIsUtility>{SpyConfig.maxCloakDurationDead.Value} seconds</style>" +
                $" at the cost of up to <style=cIsHealth>{100f * SpyConfig.cloakHealthCost.Value}% HP</style>. " +
                $"While your <color=#62746f>Deadman's Watch</color> is out, <color=#62746f>Spy</color> cannot shoot." + Tokens.ScepterDescription("<style=cIsHealth>Backstab</style> kills <style=cIsUtility>reset</style> <color=#62746f>Deadman's Watch</color> and the <style=cIsHealth>HP</style> cost is removed."));
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(SpyMasteryAchievement.identifier), "Spy: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(SpyMasteryAchievement.identifier), "As Spy, beat the game or obliterate on Monsoon.");

            Language.Add(Tokens.GetAchievementNameToken(SpyUnlockAchievement.identifier), "Dressed to Kill");
            Language.Add(Tokens.GetAchievementDescriptionToken(SpyUnlockAchievement.identifier), "Get a Backstab.");

            #endregion

            #endregion
        }
    }
}