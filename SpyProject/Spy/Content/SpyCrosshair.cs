using RoR2;
using UnityEngine;
using UnityEngine.UI;
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
            spyCrosshair.transform.GetChild(5).GetChild(0).gameObject.GetComponent<RectTransform>().position = new Vector2(-24f, -20f);
            spyCrosshair.transform.GetChild(5).GetChild(0).gameObject.GetComponent<RectTransform>().localScale *= 0.5f;
            spyCrosshair.transform.GetChild(5).GetChild(0).gameObject.GetComponent<Image>().color = SpyAssets.spyColor;
            spyCrosshair.transform.GetChild(5).GetChild(1).gameObject.GetComponent<RectTransform>().position = new Vector2(-12f, -20f);
            spyCrosshair.transform.GetChild(5).GetChild(1).gameObject.GetComponent<RectTransform>().localScale *= 0.5f;
            spyCrosshair.transform.GetChild(5).GetChild(1).gameObject.GetComponent<Image>().color = SpyAssets.spyColor;
            spyCrosshair.transform.GetChild(5).GetChild(2).gameObject.GetComponent<RectTransform>().position = new Vector2(0f, -20f);
            spyCrosshair.transform.GetChild(5).GetChild(2).gameObject.GetComponent<RectTransform>().localScale *= 0.5f;
            spyCrosshair.transform.GetChild(5).GetChild(2).gameObject.GetComponent<Image>().color = SpyAssets.spyColor;
            spyCrosshair.transform.GetChild(5).GetChild(3).gameObject.GetComponent<RectTransform>().position = new Vector2(12f, -20f);
            spyCrosshair.transform.GetChild(5).GetChild(3).gameObject.GetComponent<RectTransform>().localScale *= 0.5f;
            spyCrosshair.transform.GetChild(5).GetChild(3).gameObject.GetComponent<Image>().color = SpyAssets.spyColor;
            GameObject stock5 = spyCrosshair.transform.GetChild(5).GetChild(3).gameObject.InstantiateClone("Stock, 5", false);
            stock5.transform.SetParent(spyCrosshair.transform.GetChild(5));
            spyCrosshair.transform.GetChild(5).GetChild(4).gameObject.GetComponent<RectTransform>().position = new Vector2(24f, -20f);
            spyCrosshair.transform.GetChild(5).GetChild(4).gameObject.GetComponent<Image>().color = SpyAssets.spyColor;
            DiamondbackController diamondbackController = spyCrosshair.gameObject.AddComponent<DiamondbackController>();
            DiamondbackController.CritSpriteDisplay one = new DiamondbackController.CritSpriteDisplay();
            diamondbackController.critSpriteDisplays = new DiamondbackController.CritSpriteDisplay[5];
            one.target = spyCrosshair.transform.GetChild(5).GetChild(0).gameObject;
            one.minimumStockCountToBeValid = 1;
            one.maximumStockCountToBeValid = 5;
            diamondbackController.critSpriteDisplays[0] = one;
            one.target = spyCrosshair.transform.GetChild(5).GetChild(1).gameObject;
            one.minimumStockCountToBeValid = 2;
            one.maximumStockCountToBeValid = 5;
            diamondbackController.critSpriteDisplays[1] = one;
            one.target = spyCrosshair.transform.GetChild(5).GetChild(2).gameObject;
            one.minimumStockCountToBeValid = 3;
            one.maximumStockCountToBeValid = 5;
            diamondbackController.critSpriteDisplays[2] = one;
            one.target = spyCrosshair.transform.GetChild(5).GetChild(3).gameObject;
            one.minimumStockCountToBeValid = 4;
            one.maximumStockCountToBeValid = 5;
            diamondbackController.critSpriteDisplays[3] = one;
            one.target = spyCrosshair.transform.GetChild(5).GetChild(4).gameObject;
            one.minimumStockCountToBeValid = 5;
            one.maximumStockCountToBeValid = 5;
            diamondbackController.critSpriteDisplays[4] = one;
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