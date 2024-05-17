using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using SpyMod.Spy.Components;
using SpyMod.Spy.Content;

namespace SpyMod.Spy.Components
{
    public class CloakHudController : MonoBehaviour
    {
        public HUD targetHUD;
        public SpyController spyController;

        public LanguageTextMeshController targetText;
        public GameObject durationDisplay;
        public Image durationBar;
        public Image durationBarColor;

        private void Start()
        {
            this.spyController = this.targetHUD?.targetBodyObject?.GetComponent<SpyController>();
            this.spyController.onStealthChange += SetDisplay;

            this.durationDisplay.SetActive(false);
            SetDisplay();
        }

        private void OnDestroy()
        {
            if (this.spyController) this.spyController.onStealthChange -= SetDisplay;

            this.targetText.token = string.Empty;
            this.durationDisplay.SetActive(false);
            GameObject.Destroy(this.durationDisplay);
        }

        private void Update()
        {
            if(targetText.token != string.Empty) { targetText.token = string.Empty; }

            if(this.spyController && this.spyController.maxCloakTimer > 0f)
            {
                float fill;
                if (this.spyController.isSpecialDeadman)
                {
                    if (this.spyController.cloakRecharge > 0f) fill = Util.Remap(this.spyController.cloakRecharge, 0f, this.spyController.maxCloakRecharge, 0f, 1f);
                    else fill = 1f;
                }
                else fill = Util.Remap(this.spyController.cloakTimer, 0f, this.spyController.maxCloakTimer, 0f, 1f);

                if (this.durationBarColor)
                {
                    if (fill >= 1f) this.durationBarColor.fillAmount = 1f;
                    this.durationBarColor.fillAmount = Mathf.Lerp(this.durationBarColor.fillAmount, fill, Time.deltaTime * 2f);
                }

                this.durationBar.fillAmount = fill;
            }
        }

        private void SetDisplay()
        {
            if (this.spyController)
            {
                this.durationDisplay.SetActive(true);
                this.targetText.token = string.Empty;

                this.durationBar.color = SpyAssets.spyColor;
            }
            else
            {
                this.durationDisplay.SetActive(false);
            }
        }
    }
}