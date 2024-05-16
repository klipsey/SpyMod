using SpyMod.Spy.Content;

namespace SpyMod.Modules
{
    internal static class Tokens
    {
        public const string agilePrefix = "<style=cIsUtility>Agile</style>";

        public const string spyCritPrefix = "<color=#62746f>Espionage</color>";

        public static string agileKeyword = KeywordText("Agile", "The skill can be used while sprinting.");

        public static string spyCritKeyword = KeywordText("Espionage", "<style=cIsDamage>Backstab</style> kills with your <color=#62746f>Knife</color> guarantee a <style=cIsDamage>Critical Strike</style> for <color=#62746f>The Diamondback</color>. " +
            " Getting a <style=cIsDamage>Critical Strike</style> with an <color=#62746f>Espionage</color> stack increases the <style=cIsDamage>damage</style> by 50% instead. Max 5 stacks.");

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