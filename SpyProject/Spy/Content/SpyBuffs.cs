using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SpyMod.Spy.Content
{
    public static class SpyBuffs
    {
        public static BuffDef spyWatchDebuff;

        public static void Init(AssetBundle assetBundle)
        {
            spyWatchDebuff = Modules.Content.CreateAndAddBuff("SpyWatchBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/texBuffSlow50Icon.tif").WaitForCompletion(),
                SpyAssets.spyColor, false, true, false);
        }
    }
}
