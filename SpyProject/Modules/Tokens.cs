using SpyMod.Spy.Content;

namespace SpyMod.Modules
{
    internal static class Tokens
    {
        public const string agilePrefix = "<style=cIsUtility>Agile</style>";

        public const string spyCritPrefix = "<color=#62746f>Espionage</color>";

        public const string spyBigEarnerPrefix = "<color=#62746f>Opportunist</color>";

        public const string spyBackstabPrefix = "<style=cIsHealth>Backstab</style>";

        public static string agileKeyword = KeywordText("Agile", "The skill can be used while sprinting.");

        public static string spyCritKeyword = KeywordText("Espionage", $"Your next shot has <style=cIsDamage>100% Crit Chance</style>. " +
            $"Rolling a <style=cIsDamage>Crit</style> with <color=#62746f>Espionage</color> stacks increases <style=cIsDamage>damage by 2x</style> instead. " +
            $"<style=cIsHealth>Backstabs</style> will always grant <color=#62746f>Espionage</color> stacks on champion enemies.");
       
        public static string spyBigEarnerKeyword = KeywordText("Opportunist", $"Your <style=cIsHealth>HP</style> and <style=cIsHealing>health regneration</style> are <style=cDeath>permanently reduced by {SpyConfig.bigEarnerHealthPunishment.Value * 100f}%</style>. " +
            "<style=cIsHealth>Backstabs</style> that don't kill champion enemies grant <style=cIsUtility>movement speed</style> and <style=cIsHealing>barrier</style>. ");
        
        public static string spyBackstabKeyword = KeywordText("Backstab", $"Deals <style=cIsDamage>2x damage</style> against champion enemies and <style=cIsHealth>30% HP</style> or more to elites.");

        public static string DamageText(string text)
        {
            return $"<style=cIsDamage>{text}</style>";
        }
        public static string DamageValueText(float value)
        {
            return $"<style=cIsDamage>{value * 100}% damage</style>";
        }
        public static string UtilityText(string text)
        {
            return $"<style=cIsUtility>{text}</style>";
        }
        public static string RedText(string text) => HealthText(text);
        public static string HealthText(string text)
        {
            return $"<style=cIsHealth>{text}</style>";
        }
        public static string KeywordText(string keyword, string sub)
        {
            return $"<style=cKeywordName>{keyword}</style><style=cSub>{sub}</style>";
        }
        public static string ScepterDescription(string desc)
        {
            return $"\n<color=#d299ff>SCEPTER: {desc}</color>";
        }

        public static string GetAchievementNameToken(string identifier)
        {
            return $"ACHIEVEMENT_{identifier.ToUpperInvariant()}_NAME";
        }
        public static string GetAchievementDescriptionToken(string identifier)
        {
            return $"ACHIEVEMENT_{identifier.ToUpperInvariant()}_DESCRIPTION";
        }
    }
}