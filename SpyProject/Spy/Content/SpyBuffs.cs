using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SpyMod.Spy.Content
{
    public static class SpyBuffs
    {
        public static BuffDef spyWatchDebuff;
        public static BuffDef spyDiamondbackBuff;
        public static BuffDef armorBuff;
        public static void Init(AssetBundle assetBundle)
        {
            spyWatchDebuff = Modules.Content.CreateAndAddBuff("SpyWatchBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/texBuffSlow50Icon.tif").WaitForCompletion(),
                SpyAssets.spyColor, false, true, false);
            spyDiamondbackBuff = Modules.Content.CreateAndAddBuff("SpyCritBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/CritOnUse/texBuffFullCritIcon.tif").WaitForCompletion(),
                SpyAssets.spyColor, true, false, false);
            armorBuff = Modules.Content.CreateAndAddBuff("SpyArmorBuff", LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite, Color.white, false, false, false);
        }
    }
}
