using RoR2;
using UnityEngine;
using SpyMod.Spy;
using SpyMod.Spy.Achievements;

namespace SpyMod.Spy.Content
{
    public static class SpyUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            if(false == true)
            {
                characterUnlockableDef = Modules.Content.CreateAndAddUnlockableDef(SpyUnlockAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(SpyUnlockAchievement.unlockableIdentifier),
                SpySurvivor.instance.assetBundle.LoadAsset<Sprite>("texSpyIcon"));
            }
        }
    }
}
