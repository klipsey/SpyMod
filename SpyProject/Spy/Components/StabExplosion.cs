using UnityEngine;
using RoR2;

namespace SpyMod.Spy.Components
{
    public class StabExplosion : MonoBehaviour
    {
        private void Awake()
        {
            CharacterBody characterBody = this.GetComponent<CharacterBody>();

            if (this.transform)
            {
                EffectManager.SpawnEffect(Content.SpyAssets.bloodExplosionEffect, new EffectData
                {
                    origin = this.transform.position,
                    rotation = Quaternion.identity,
                    scale = 1f
                }, false);

                GameObject.Instantiate(Content.SpyAssets.bloodSpurtEffect, this.transform);
                Util.PlaySound("sfx_blood_gurgle", base.gameObject);
            }
        }

        private void LateUpdate()
        {
            if (this.transform) this.transform.localScale = Vector3.zero;
        }
    }
}