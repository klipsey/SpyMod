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
            spyCrosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageCrosshair.prefab").WaitForCompletion().InstantiateClone("SeamCrosshair", false);
            spyCrosshair.transform.GetChild(5).GetChild(0).gameObject.GetComponent<RectTransform>().position = new Vector2(-24f, -20f);
            spyCrosshair.transform.GetChild(5).GetChild(0).gameObject.GetComponent<RectTransform>().localScale *= 0.5f;
            spyCrosshair.transform.GetChild(5).GetChild(1).gameObject.GetComponent<RectTransform>().position = new Vector2(-12f, -20f);
            spyCrosshair.transform.GetChild(5).GetChild(1).gameObject.GetComponent<RectTransform>().localScale *= 0.5f;
            spyCrosshair.transform.GetChild(5).GetChild(2).gameObject.GetComponent<RectTransform>().position = new Vector2(0f, -20f);
            spyCrosshair.transform.GetChild(5).GetChild(2).gameObject.GetComponent<RectTransform>().localScale *= 0.5f;
            spyCrosshair.transform.GetChild(5).GetChild(3).gameObject.GetComponent<RectTransform>().position = new Vector2(12f, -20f);
            spyCrosshair.transform.GetChild(5).GetChild(3).gameObject.GetComponent<RectTransform>().localScale *= 0.5f;
            GameObject stock5 = spyCrosshair.transform.GetChild(5).GetChild(3).gameObject.InstantiateClone("Stock, 5", false);
            stock5.transform.SetParent(spyCrosshair.transform.GetChild(5));
            spyCrosshair.transform.GetChild(5).GetChild(4).gameObject.GetComponent<RectTransform>().position = new Vector2(24f, -20f);
            DiamondbackController cross = spyCrosshair.gameObject.AddComponent<DiamondbackController>();
            DiamondbackController.CritSpriteDisplay one = new DiamondbackController.CritSpriteDisplay();
            cross.critSpriteDisplays = new DiamondbackController.CritSpriteDisplay[5];
            one.target = spyCrosshair.transform.GetChild(5).GetChild(0).gameObject;
            one.minimumStockCountToBeValid = 1;
            one.maximumStockCountToBeValid = 5;
            cross.critSpriteDisplays[0] = one;
            one.target = spyCrosshair.transform.GetChild(5).GetChild(1).gameObject;
            one.minimumStockCountToBeValid = 2;
            one.maximumStockCountToBeValid = 5;
            cross.critSpriteDisplays[1] = one;
            one.target = spyCrosshair.transform.GetChild(5).GetChild(2).gameObject;
            one.minimumStockCountToBeValid = 3;
            one.maximumStockCountToBeValid = 5;
            cross.critSpriteDisplays[2] = one;
            one.target = spyCrosshair.transform.GetChild(5).GetChild(3).gameObject;
            one.minimumStockCountToBeValid = 4;
            one.maximumStockCountToBeValid = 5;
            cross.critSpriteDisplays[3] = one;
            one.target = spyCrosshair.transform.GetChild(5).GetChild(4).gameObject;
            one.minimumStockCountToBeValid = 5;
            one.maximumStockCountToBeValid = 5;
            cross.critSpriteDisplays[4] = one;
            Object.Destroy(spyCrosshair.gameObject.GetComponent<CrosshairController>());
            Object.Destroy(spyCrosshair.transform.GetChild(0).gameObject);
            Object.Destroy(spyCrosshair.transform.GetChild(1).gameObject);
            Object.Destroy(spyCrosshair.transform.GetChild(2).gameObject);
            Object.Destroy(spyCrosshair.transform.GetChild(3).gameObject);
            Object.Destroy(spyCrosshair.transform.GetChild(4).gameObject);
            Object.Destroy(spyCrosshair.transform.GetChild(6).gameObject);
        }
    }
}