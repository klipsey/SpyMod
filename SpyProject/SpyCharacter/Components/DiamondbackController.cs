using System;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.UI;
using RoR2.UI;
using RoR2;
using SpyMod.Spy.Content;

namespace SpyMod.Spy.Components
{
    public class DiamondbackController : MonoBehaviour
    {
        [Serializable]
        public struct CritSpriteDisplay
        {
            public GameObject target;

            public int minimumStockCountToBeValid;

            public int maximumStockCountToBeValid;
        }
        public CritSpriteDisplay[] critSpriteDisplays;
        public RectTransform rectTransform { get; private set; }

        public HudElement hudElement { get; private set; }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            hudElement = GetComponent<HudElement>();
            SetCritDisplays();
        }
        private void SetCritDisplays()
        {
            if (!hudElement.targetCharacterBody)
            {
                return;
            }
            for (int i = 0; i < critSpriteDisplays.Length; i++)
            {
                bool active = false;
                CritSpriteDisplay critSpriteDisplay = critSpriteDisplays[i];
                int critBuff = hudElement.targetCharacterBody.GetBuffCount(SpyBuffs.spyDiamondbackBuff);
                if (critBuff > 0 && critBuff >= critSpriteDisplay.minimumStockCountToBeValid && (critBuff <= critSpriteDisplay.maximumStockCountToBeValid || critSpriteDisplay.maximumStockCountToBeValid < 0))
                {
                    active = true;
                }
                critSpriteDisplay.target.SetActive(active);
            }
        }

        private void LateUpdate()
        {
            SetCritDisplays();
        }
    }

}