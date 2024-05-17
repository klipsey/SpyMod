using RoR2;
using UnityEngine;
using SpyMod.Modules;
using RoR2.Projectile;
using RoR2.UI;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using R2API;
using UnityEngine.Rendering.PostProcessing;
using ThreeEyedGames;

namespace SpyMod.Spy.Content
{
    public static class SpyAssets
    {
        //AssetBundle
        internal static AssetBundle mainAssetBundle;

        //Materials
        internal static Material commandoMat;
        internal static Material sapperMat;
        //Shader
        internal static Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/HGStandard");

        //Effects
        internal static GameObject intoStealthEffect;

        internal static GameObject bloodSplatterEffect;
        internal static GameObject bloodExplosionEffect;
        internal static GameObject bloodSpurtEffect;

        internal static GameObject backStabEffect;

        internal static GameObject revolverTracer;
        internal static GameObject revolverTracerCrit;

        internal static GameObject knifeSwingEffect;
        internal static GameObject knifeHitEffect;

        internal static GameObject sapperExpiredEffect;

        internal static GameObject lightningEffect;

        internal static GameObject headshotOverlay;
        internal static GameObject headshotVisualizer;

        internal static GameObject spyCloakEffect;
        //Models
        internal static Mesh sapperMesh;
        //Projectiles
        internal static GameObject sapperPrefab;
        internal static GameObject sapperPrefabGhost;
        //Sounds
        internal static NetworkSoundEventDef knifeImpactSoundDef;

        //Colors
        internal static Color spyColor = new Color(98f / 255f, 116 / 255f, 111 / 255f);

        //Crosshair
        internal static GameObject defaultCrosshair;
        internal static GameObject watchCrosshair;
        public static void Init(AssetBundle assetBundle)
        {
            mainAssetBundle = assetBundle;
        }
        public static void InitAssets()
        {
            CreateMaterials();

            CreateModels();

            CreateEffects();

            CreateSounds();

            CreateProjectiles();

            CreateUI();
        }

        private static void CleanChildren(Transform startingTrans)
        {
            for (int num = startingTrans.childCount - 1; num >= 0; num--)
            {
                if (startingTrans.GetChild(num).childCount > 0)
                {
                    CleanChildren(startingTrans.GetChild(num));
                }
                Object.DestroyImmediate(startingTrans.GetChild(num).gameObject);
            }
        }

        private static void CreateMaterials()
        {
            sapperMat = mainAssetBundle.LoadAsset<Material>("matSapper");
        }

        private static void CreateModels()
        {
            sapperMesh = mainAssetBundle.LoadAsset<Mesh>("meshSapper");
        }
        #region effects
        private static void CreateEffects()
        {
            lightningEffect = mainAssetBundle.LoadAsset<GameObject>("CritLightning");

            bloodExplosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("DriverBloodExplosion", false);

            spyCloakEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2SmokeBombMini.prefab").WaitForCompletion().InstantiateClone("SpyCloak", false);
            spyCloakEffect.gameObject.GetComponent<EffectComponent>().applyScale = true;
            Component.DestroyImmediate(spyCloakEffect.gameObject.GetComponent<ShakeEmitter>());
            GameObject.DestroyImmediate(spyCloakEffect.transform.Find("Core").Find("Sparks").gameObject);
            spyCloakEffect.transform.Find("Core").Find("Smoke, Edge Circle").localScale = Vector3.one * 0.4f;
            Modules.Content.CreateAndAddEffectDef(spyCloakEffect);

            Material bloodMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodHumanLarge.mat").WaitForCompletion();
            Material bloodMat2 = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion();

            bloodExplosionEffect.transform.Find("Particles/LongLifeNoiseTrails").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/LongLifeNoiseTrails, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/Dash").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/Dash, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/DashRings").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion();
            bloodExplosionEffect.GetComponentInChildren<Light>().gameObject.SetActive(false);

            bloodExplosionEffect.GetComponentInChildren<PostProcessVolume>().sharedProfile = Addressables.LoadAssetAsync <PostProcessProfile>("RoR2/Base/title/ppLocalGold.asset").WaitForCompletion();

            Modules.Content.CreateAndAddEffectDef(bloodExplosionEffect);

            bloodSpurtEffect = mainAssetBundle.LoadAsset<GameObject>("BloodSpurtEffect");

            bloodSpurtEffect.transform.Find("Blood").GetComponent<ParticleSystemRenderer>().material = bloodMat2;
            bloodSpurtEffect.transform.Find("Trails").GetComponent<ParticleSystemRenderer>().trailMaterial = bloodMat2;

            knifeHitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/HitsparkBandit.prefab").WaitForCompletion().InstantiateClone("KnifeHitEffect");
            knifeHitEffect.AddComponent<NetworkIdentity>();
            Modules.Content.CreateAndAddEffectDef(knifeHitEffect);

            sapperExpiredEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/HitsparkBandit.prefab").WaitForCompletion().InstantiateClone("SapperExpiredEffect");
            sapperExpiredEffect.AddComponent<NetworkIdentity>();
            sapperExpiredEffect.GetComponent<EffectComponent>().soundName = "sfx_spy_sapper_remove";
            Modules.Content.CreateAndAddEffectDef(sapperExpiredEffect);

            knifeSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlash.prefab").WaitForCompletion().InstantiateClone("SpyKnifeSwing", false);
            knifeSwingEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressSwingTrail.mat").WaitForCompletion();
            var swing = knifeSwingEffect.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            swing.startLifetimeMultiplier *= 2f;

            revolverTracer = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoShotgun").InstantiateClone("SpyShotgunTracer", true);

            if (!revolverTracer.GetComponent<EffectComponent>()) revolverTracer.AddComponent<EffectComponent>();
            if (!revolverTracer.GetComponent<VFXAttributes>()) revolverTracer.AddComponent<VFXAttributes>();
            if (!revolverTracer.GetComponent<NetworkIdentity>()) revolverTracer.AddComponent<NetworkIdentity>();

            Material bulletMat = null;

            foreach (LineRenderer i in revolverTracer.GetComponentsInChildren<LineRenderer>())
            {
                if (i)
                {
                    bulletMat = UnityEngine.Object.Instantiate<Material>(i.material);
                    bulletMat.SetColor("_TintColor", new Color(0.68f, 0.58f, 0.05f));
                    i.material = bulletMat;
                    i.startColor = new Color(0.68f, 0.58f, 0.05f);
                    i.endColor = new Color(0.68f, 0.58f, 0.05f);
                }
            }

            revolverTracerCrit = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoShotgun").InstantiateClone("SpyShotgunTracerCritical", true);

            if (!revolverTracerCrit.GetComponent<EffectComponent>()) revolverTracerCrit.AddComponent<EffectComponent>();
            if (!revolverTracerCrit.GetComponent<VFXAttributes>()) revolverTracerCrit.AddComponent<VFXAttributes>();
            if (!revolverTracerCrit.GetComponent<NetworkIdentity>()) revolverTracerCrit.AddComponent<NetworkIdentity>();

            foreach (LineRenderer i in revolverTracerCrit.GetComponentsInChildren<LineRenderer>())
            {
                if (i)
                {
                    bulletMat = UnityEngine.Object.Instantiate<Material>(i.material);
                    bulletMat.SetColor("_TintColor", Color.yellow);
                    i.material = bulletMat;
                    i.startColor = new Color(0.8f, 0.24f, 0f);
                    i.endColor = new Color(0.8f, 0.24f, 0f);
                }
            }
            Modules.Content.CreateAndAddEffectDef(revolverTracer);
            Modules.Content.CreateAndAddEffectDef(revolverTracerCrit);

            intoStealthEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/LunarDetonatorConsume.prefab").WaitForCompletion().InstantiateClone("SpyAtomicEnd");
            intoStealthEffect.AddComponent<NetworkIdentity>();
            var fart = intoStealthEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fart.startColor = Color.black;
            fart = intoStealthEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().main;
            fart.startColor = new Color(184f / 255f, 226f / 255f, 61f / 255f);
            intoStealthEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", new Color(184f / 255f, 226f / 255f, 61f / 255f));
            intoStealthEffect.transform.GetChild(3).gameObject.SetActive(false);
            intoStealthEffect.transform.GetChild(4).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", new Color(184f / 255f, 226f / 255f, 61f / 255f));
            Material material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarSkillReplacements/matLunarNeedleImpactEffect.mat").WaitForCompletion());
            material.SetColor("_TintColor", new Color(184f / 255f, 226f / 255f, 61f / 255f));
            intoStealthEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            intoStealthEffect.transform.GetChild(6).gameObject.SetActive(false);
            Object.Destroy(intoStealthEffect.GetComponent<EffectComponent>());

            bloodSplatterEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSlamImpact.prefab").WaitForCompletion().InstantiateClone("SpySplat", true);
            bloodSplatterEffect.AddComponent<NetworkIdentity>();
            bloodSplatterEffect.transform.GetChild(0).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(1).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(2).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(3).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(4).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(5).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(6).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(7).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(8).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(9).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(10).gameObject.SetActive(false);
            bloodSplatterEffect.transform.Find("Decal").GetComponent<Decal>().Material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDecal.mat").WaitForCompletion();
            bloodSplatterEffect.transform.Find("Decal").GetComponent<AnimateShaderAlpha>().timeMax = 10f;
            bloodSplatterEffect.transform.GetChild(12).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(13).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(14).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(15).gameObject.SetActive(false);
            bloodSplatterEffect.transform.localScale = Vector3.one;
            SpyMod.Modules.Content.CreateAndAddEffectDef(bloodSplatterEffect);
        }

        #endregion

        #region projectiles
        private static void CreateProjectiles()
        {
            sapperPrefabGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/StickyBomb/StickyBombGhost.prefab").WaitForCompletion().InstantiateClone("SpySapperGhost", false);

            Material[] mat = new Material[1];
            mat[0] = sapperMat;
            sapperPrefabGhost.transform.Find("Cylinder").gameObject.GetComponent<MeshRenderer>().materials = mat;
            sapperPrefabGhost.transform.Find("Cylinder").gameObject.GetComponent<MeshFilter>().mesh = sapperMesh;
            sapperPrefabGhost.transform.Find("Cylinder").gameObject.GetComponent<Light>().color = spyColor;
            GameObject.DestroyImmediate(sapperPrefabGhost.transform.Find("Pulse").gameObject);
            lightningEffect.transform.SetParent(sapperPrefabGhost.transform, false);

            sapperPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/StickyBomb/StickyBomb.prefab").WaitForCompletion().InstantiateClone("SpySapper");
            sapperPrefab.AddComponent<NetworkIdentity>();
            Component.DestroyImmediate(sapperPrefab.GetComponent<ProjectileImpactExplosion>());
            Component.DestroyImmediate(sapperPrefab.GetComponent<LoopSound>());
            sapperPrefab.GetComponent<RTPCController>().rtpcString = string.Empty;
            sapperPrefab.gameObject.GetComponent<ProjectileController>().ghostPrefab = sapperPrefabGhost;
            sapperPrefab.gameObject.GetComponent<ProjectileController>().startSound = "sfx_spy_sapper_loop";
            sapperPrefab.gameObject.GetComponent<ProjectileDamage>().damageType |= DamageType.Shock5s;
            sapperPrefab.gameObject.GetComponent<ProjectileSimple>().lifetimeExpiredEffect = sapperExpiredEffect;
            ProjectileProximityBeamController projectileProximityBeamController = sapperPrefab.AddComponent<ProjectileProximityBeamController>();
            projectileProximityBeamController.enabled = true;
            projectileProximityBeamController.attackFireCount = 6;
            projectileProximityBeamController.attackInterval = 0.2f;
            projectileProximityBeamController.listClearInterval = 6f;
            projectileProximityBeamController.minAngleFilter = 0f;
            projectileProximityBeamController.maxAngleFilter = 180f;
            projectileProximityBeamController.attackRange = SpyConfig.sapperRange.Value;
            projectileProximityBeamController.inheritDamageType = true;
            projectileProximityBeamController.damageCoefficient = 1.2f;
            projectileProximityBeamController.procCoefficient = 1f;
            projectileProximityBeamController.bounces = 0;
            projectileProximityBeamController.lightningType = RoR2.Orbs.LightningOrb.LightningType.Ukulele;

            Modules.Content.AddProjectilePrefab(sapperPrefab);
        }
        #endregion

        #region sounds
        private static void CreateSounds()
        {
            knifeImpactSoundDef = Modules.Content.CreateAndAddNetworkSoundEventDef("sfx_driver_knife_hit");

        }
        #endregion

        private static void CreateUI()
        {
            defaultCrosshair = Assets.LoadCrosshair("Standard");
            watchCrosshair = Assets.LoadCrosshair("SimpleDot");

            headshotOverlay = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerScopeLightOverlay.prefab").WaitForCompletion().InstantiateClone("SpyHeadshotOverlay", false);
            SniperTargetViewer viewer = headshotOverlay.GetComponentInChildren<SniperTargetViewer>();
            headshotOverlay.transform.Find("ScopeOverlay").gameObject.SetActive(false);

            headshotVisualizer = viewer.visualizerPrefab.InstantiateClone("SpyHeadshotVisualizer", false);
            Image headshotImage = headshotVisualizer.transform.Find("Scaler/Rectangle").GetComponent<Image>();
            headshotVisualizer.transform.Find("Scaler/Outer").gameObject.SetActive(false);
            headshotImage.color = Color.red;

            viewer.visualizerPrefab = headshotVisualizer;
        }

        #region helpers
        private static GameObject CreateImpactExplosionEffect(string effectName, Material bloodMat, Material decal, float scale = 1f)
        {
            GameObject newEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSlamImpact.prefab").WaitForCompletion().InstantiateClone(effectName, true);

            newEffect.transform.Find("Spikes, Small").gameObject.SetActive(false);

            newEffect.transform.Find("PP").gameObject.SetActive(false);
            newEffect.transform.Find("Point light").gameObject.SetActive(false);
            newEffect.transform.Find("Flash Lines").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOpaqueDustLargeDirectional.mat").WaitForCompletion();

            newEffect.transform.GetChild(3).GetComponent<ParticleSystemRenderer>().material = bloodMat;
            newEffect.transform.Find("Flash Lines, Fire").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            newEffect.transform.GetChild(6).GetComponent<ParticleSystemRenderer>().material = bloodMat;
            newEffect.transform.Find("Fire").GetComponent<ParticleSystemRenderer>().material = bloodMat;

            var boom = newEffect.transform.Find("Fire").GetComponent<ParticleSystem>().main;
            boom.startLifetimeMultiplier = 0.5f;
            boom = newEffect.transform.Find("Flash Lines, Fire").GetComponent<ParticleSystem>().main;
            boom.startLifetimeMultiplier = 0.3f;
            boom = newEffect.transform.GetChild(6).GetComponent<ParticleSystem>().main;
            boom.startLifetimeMultiplier = 0.4f;

            newEffect.transform.Find("Physics").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/MagmaWorm/matFracturedGround.mat").WaitForCompletion();

            newEffect.transform.Find("Decal").GetComponent<Decal>().Material = decal;
            newEffect.transform.Find("Decal").GetComponent<AnimateShaderAlpha>().timeMax = 10f;

            newEffect.transform.Find("FoamSplash").gameObject.SetActive(false);
            newEffect.transform.Find("FoamBilllboard").gameObject.SetActive(false);
            newEffect.transform.Find("Dust").gameObject.SetActive(false);
            newEffect.transform.Find("Dust, Directional").gameObject.SetActive(false);

            newEffect.transform.localScale = Vector3.one * scale;

            newEffect.AddComponent<NetworkIdentity>();

            ParticleSystemColorFromEffectData PSCFED = newEffect.AddComponent<ParticleSystemColorFromEffectData>();
            PSCFED.particleSystems = new ParticleSystem[]
            {
                newEffect.transform.Find("Fire").GetComponent<ParticleSystem>(),
                newEffect.transform.Find("Flash Lines, Fire").GetComponent<ParticleSystem>(),
                newEffect.transform.GetChild(6).GetComponent<ParticleSystem>(),
                newEffect.transform.GetChild(3).GetComponent<ParticleSystem>()
            };
            PSCFED.effectComponent = newEffect.GetComponent<EffectComponent>();

            SpyMod.Modules.Content.CreateAndAddEffectDef(newEffect);

            return newEffect;
        }
        public static Material CreateMaterial(string materialName, float emission, Color emissionColor, float normalStrength)
        {
            if (!commandoMat) commandoMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

            Material mat = UnityEngine.Object.Instantiate<Material>(commandoMat);
            Material tempMat = mainAssetBundle.LoadAsset<Material>(materialName);

            if (!tempMat) return commandoMat;

            mat.name = materialName;
            mat.SetColor("_Color", tempMat.GetColor("_Color"));
            mat.SetTexture("_MainTex", tempMat.GetTexture("_MainTex"));
            mat.SetColor("_EmColor", emissionColor);
            mat.SetFloat("_EmPower", emission);
            mat.SetTexture("_EmTex", tempMat.GetTexture("_EmissionMap"));
            mat.SetFloat("_NormalStrength", normalStrength);

            return mat;
        }

        public static Material CreateMaterial(string materialName)
        {
            return CreateMaterial(materialName, 0f);
        }

        public static Material CreateMaterial(string materialName, float emission)
        {
            return CreateMaterial(materialName, emission, Color.black);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor)
        {
            return CreateMaterial(materialName, emission, emissionColor, 0f);
        }
        #endregion
    }
}