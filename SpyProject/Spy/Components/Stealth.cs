/*
using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using SpyMod.Spy.Components;

namespace RobDriver.Modules.Components
{
    public class Stealth : MonoBehaviour
    {
        public HUD targetHUD;
        public SpyController spyController;

        public LanguageTextMeshController targetText;
        public GameObject durationDisplay;
        public Image durationBar;
        public Image durationBarRed;

        private void Start()
        {
            this.spyController = this.targetHUD?.targetBodyObject?.GetComponent<SpyController>();
            this.spyController.onAtomicChange += SetDisplay;

            this.durationDisplay.SetActive(false);
            SetDisplay();
        }

        private void OnDestroy()
        {
            if (this.spyController) this.spyController.onAtomicChange -= SetDisplay;

            this.targetText.token = string.Empty;
            this.durationDisplay.SetActive(false);
            GameObject.Destroy(this.durationDisplay);
        }

        private void Update()
        {
            if(targetText.token != string.Empty) { targetText.token = string.Empty; }
            if (this.spyController && this.spyController.atomicGauge >= 0f)
            {
                float fill = Util.Remap(this.spyController.atomicGauge, 0f, this.spyController.maxAtomicGauge, 0f, 1f);

                if (this.durationBarRed)
                {
                    if (fill >= 1f) this.durationBarRed.fillAmount = 1f;
                    this.durationBarRed.fillAmount = Mathf.Lerp(this.durationBarRed.fillAmount, fill, Time.deltaTime * 2f);
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

                if (this.spyController.atomicGauge <=  99f) this.durationBar.color = new Color(85f / 255f, 188f / 255f, 0f);
                else this.durationBar.color = Color.red;
            }
            else
            {
                this.durationDisplay.SetActive(false);
            }
        }
    }
}
*/