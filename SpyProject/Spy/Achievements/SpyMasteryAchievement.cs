using RoR2;
using SpyMod.Modules.Achievements;
using SpyMod.Spy;

namespace SpyMod.Spy.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, null)]
    public class SpyMasteryAchievement : BaseMasteryAchievement
    {
        public const string identifier = SpySurvivor.SPY_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = SpySurvivor.SPY_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => SpySurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}