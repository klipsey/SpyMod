using RoR2;
using UnityEngine;
using SpyMod.Modules;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using R2API;
using RoR2.UI;
using SpyMod.Spy.Components;

namespace SpyMod.Spy.Content
{
    public static class SpyCrosshair
    {
        internal static GameObject spyCrosshair;

        private static AssetBundle _assetBundle;
        public static void Init(AssetBundle assetBundle)
        {
            _assetBundle = assetBundle;
            CreateCrosshair();
        }

        private static void CreateCrosshair()
        {
            spyCrosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageCrosshair.prefab").WaitForCompletion().InstantiateClone("SpyCrosshair", false);
        }
    }
}