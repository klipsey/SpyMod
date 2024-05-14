﻿using BepInEx.Configuration;
using ScoutMod.Modules;
using ScoutMod.Modules.Characters;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using RoR2.UI;
using R2API;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.UI;
using R2API.Networking;
using ScoutMod.Scout.Components;
using ScoutMod.Scout.Content;
using ScoutMod.Scout.SkillStates;
using RobDriver.Modules.Components;
using HG;
using EntityStates;

namespace ScoutMod.Scout
{
    public class ScoutSurvivor : SurvivorBase<ScoutSurvivor>
    {
        public override string assetBundleName => "scout";
        public override string bodyName => "ScoutBody"; 
        public override string masterName => "ScoutMonsterMaster"; 
        public override string modelPrefabName => "mdlScout";
        public override string displayPrefabName => "ScoutDisplay";

        public const string SCOUT_PREFIX = SpyPlugin.DEVELOPER_PREFIX + "_SCOUT_";
        public override string survivorTokenPrefix => SCOUT_PREFIX;

        internal static GameObject characterPrefab;

        public static SteppedSkillDef batSkillDef;
        public static SkillDef spikeBallSkillDef;

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = SCOUT_PREFIX + "NAME",
            subtitleNameToken = SCOUT_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texScoutIcon"),
            bodyColor = new Color(184f / 255f, 226f / 255f, 61f / 255f),
            sortPosition = 5.99f,

            crosshair = Assets.LoadCrosshair("SimpleDot"),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 110f,
            healthRegen = 1.2f,
            armor = 0f,
            damage = 12f,

            damageGrowth = 2.4f,
            healthGrowth = 110f * 0.35f,

            jumpCount = 2,
        };

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = assetBundle.LoadMaterial("matScout"),
                },
                new CustomRendererInfo
                {
                    childName = "ScatterGunMesh",
                    material = assetBundle.LoadMaterial("matShotgun"),
                },
                new CustomRendererInfo
                {
                    childName = "BatMesh",
                    material = assetBundle.LoadMaterial("matScout"),
                },
                new CustomRendererInfo
                {
                    childName = "BackBatMesh",
                    material = assetBundle.LoadMaterial("matScout"),
                }

        };

        public override UnlockableDef characterUnlockableDef => ScoutUnlockables.characterUnlockableDef;

        public override ItemDisplaysBase itemDisplays => new ScoutItemDisplays();
        public override AssetBundle assetBundle { get; protected set; }
        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }
        public override void Initialize()
        {

            //uncomment if you have multiple characters
            //ConfigEntry<bool> characterEnabled = Config.CharacterEnableConfig("Survivors", "Henry");

            //if (!characterEnabled.Value)
            //    return;

            //need the character unlockable before you initialize the survivordef

            base.Initialize();
        }

        public override void InitializeCharacter()
        {
            ScoutConfig.Init();

            ScoutUnlockables.Init();

            base.InitializeCharacter();

            DamageTypes.Init();

            ScoutStates.Init();
            ScoutTokens.Init();

            ScoutAssets.Init(assetBundle);
            ScoutBuffs.Init(assetBundle);

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeCharacterMaster();

            AdditionalBodySetup();

            characterPrefab = bodyPrefab;
            AddHooks();
        }

        private void AdditionalBodySetup()
        {
            AddHitboxes();
            bool tempAdd(CharacterBody body) => body.HasBuff(ScoutBuffs.scoutAtomicBuff);
            float pee(CharacterBody body) => 2f * body.radius;
            bodyPrefab.AddComponent<ScoutController>();
            TempVisualEffectAPI.AddTemporaryVisualEffect(ScoutAssets.atomicEffect, pee, tempAdd);
        }
        public void AddHitboxes()
        {
            Prefabs.SetupHitBoxGroup(characterModelObject, "Bat", "BatHitbox");
        }

        public override void InitializeEntityStateMachines()
        {
            //clear existing state machines from your cloned body (probably commando)
            //omit all this if you want to just keep theirs
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //the main "Body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(SkillStates.MainState), typeof(EntityStates.SpawnTeleporterState));
            //if you set up a custom main characterstate, set it up here
            //don't forget to register custom entitystates in your HenryStates.cs

            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon2");
        }

        #region skills
        public override void InitializeSkills()
        {
            bodyPrefab.AddComponent<ScoutPassive>();
            Skills.CreateSkillFamilies(bodyPrefab);
            AddPassiveSkills();
            AddPrimarySkills();
            AddSecondarySkills();
            AddUtilitySkills();
            AddSpecialSkills();
        }

        private void AddPassiveSkills()
        {
            ScoutPassive passive = bodyPrefab.GetComponent<ScoutPassive>();

            SkillLocator skillLocator = bodyPrefab.GetComponent<SkillLocator>();

            skillLocator.passiveSkill.enabled = false;

            passive.doubleJumpPassive = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = SCOUT_PREFIX + "PASSIVE_NAME",
                skillNameToken = SCOUT_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = SCOUT_PREFIX + "PASSIVE_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texDoubleJumpIcon"),
                keywordTokens = new string[] {},
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            Skills.AddPassiveSkills(passive.passiveSkillSlot.skillFamily, passive.doubleJumpPassive);
        }

        private void AddPrimarySkills()
        {
            ReloadSkillDef Shoot = Skills.CreateReloadSkillDef(new ReloadSkillDefInfo
            {
                skillName = "SplatterGun",
                skillNameToken = SCOUT_PREFIX + "PRIMARY_SPLATTERGUN_NAME",
                skillDescriptionToken = SCOUT_PREFIX + "PRIMARY_SPLATTERGUN_DESCRIPTION",
                keywordTokens = new string[] { Tokens.agileKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texShotgunIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(Shoot)),
                reloadState = new SerializableEntityStateType(typeof(EnterReload)),

                activationStateMachineName = "Weapon",
                interruptPriority = InterruptPriority.Skill,
                reloadInterruptPriority = InterruptPriority.Any,

                baseMaxStock = 2,
                baseRechargeInterval = 0f,
                rechargeStock = 0,
                requiredStock = 1,
                stockToConsume = 1,
                graceDuration = 0.1f,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = false,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = false,
                mustKeyPress = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            Skills.AddPrimarySkills(bodyPrefab, Shoot);

            batSkillDef = Skills.CreateSkillDef<SteppedSkillDef>(new SkillDefInfo
                (
                    "Bonk",
                    SCOUT_PREFIX + "PRIMARY_BONK_NAME",
                    SCOUT_PREFIX + "PRIMARY_BONK_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("texSwingIcon"),
                    new EntityStates.SerializableEntityStateType(typeof(SkillStates.Swing)),
                    "Weapon"
                ));
            batSkillDef.stepCount = 2;
            batSkillDef.stepGraceDuration = 1f;
        }

        private void AddSecondarySkills()
        {
            SkillDef Cleaver = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Toxic Cleaver",
                skillNameToken = SCOUT_PREFIX + "SECONDARY_CLEAVER_NAME",
                skillDescriptionToken = SCOUT_PREFIX + "SECONDARY_CLEAVER_DESCRIPTION",
                keywordTokens = new string[] { Tokens.agileKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texButcherKnifeIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(ThrowCleaver)),

                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = false,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = false,
                mustKeyPress = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            Skills.AddSecondarySkills(bodyPrefab, Cleaver);

            spikeBallSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Atomic Spikeball",
                skillNameToken = SCOUT_PREFIX + "SECONDARY_SPIKEDBALL_NAME",
                skillDescriptionToken = SCOUT_PREFIX + "SECONDARY_SPIKEDBALL_DESCRIPTION",
                keywordTokens = new string[] {Tokens.agileKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texBaseballIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(HitBaseball)),

                activationStateMachineName = "Weapon2",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseMaxStock = 1,
                baseRechargeInterval = 6,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = false,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = false,
                mustKeyPress = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });
        }

        private void AddUtilitySkills()
        {
            SkillDef atomicBlast = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Atomic Blast",
                skillNameToken = SCOUT_PREFIX + "UTILITY_ATOMICBLAST_NAME",
                skillDescriptionToken = SCOUT_PREFIX + "UTILITY_ATOMICBLAST_DESCRIPTION",
                keywordTokens = new string[] { Tokens.agileKeyword, Tokens.miniCritsKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texAtomicIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(ActivateAtomic)),
                activationStateMachineName = "Weapon2",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 0f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });

            Skills.AddUtilitySkills(bodyPrefab, atomicBlast);
        }

        private void AddSpecialSkills()
        {
            SkillDef Swap = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Swap",
                skillNameToken = SCOUT_PREFIX + "SPECIAL_SWAP_NAME",
                skillDescriptionToken = SCOUT_PREFIX + "SPECIAL_SWAP_DESCRIPTION",
                keywordTokens = new string[] { Tokens.agileKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSwapIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SwapWeapon)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 0.1f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = false,
                dontAllowPastMaxStocks = true,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            Skills.AddSpecialSkills(bodyPrefab, Swap);
        }
        #endregion skills

        #region skins
        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Skins.CreateSkinDef("DEFAULT_SKIN",
                assetBundle.LoadAsset<Sprite>("texDefaultSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
            //pass in meshes as they are named in your assetbundle
            //currently not needed as with only 1 skin they will simply take the default meshes
            //uncomment this when you have another skin
            //defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySword",
            //    "meshHenryGun",
            //    "meshHenry");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin

            ////creating a new skindef as we did before
            //SkinDef masterySkin = Modules.Skins.CreateSkinDef(HENRY_PREFIX + "MASTERY_SKIN_NAME",
            //    assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
            //    defaultRendererinfos,
            //    prefabCharacterModel.gameObject,
            //    HenryUnlockables.masterySkinUnlockableDef);

            ////adding the mesh replacements as above. 
            ////if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            //masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySwordAlt",
            //    null,//no gun mesh replacement. use same gun mesh
            //    "meshHenryAlt");

            ////masterySkin has a new set of RendererInfos (based on default rendererinfos)
            ////you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            //masterySkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[2].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");

            ////here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            //masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            //{
            //    new SkinDef.GameObjectActivation
            //    {
            //        gameObject = childLocator.FindChildGameObject("GunModel"),
            //        shouldActivate = false,
            //    }
            //};
            ////simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            //skins.Add(masterySkin);

            #endregion

            skinController.skins = skins.ToArray();
        }
        #endregion skins


        //Character Master is what governs the AI of your character when it is not controlled by a player (artifact of vengeance, goobo)
        public override void InitializeCharacterMaster()
        {
            //if you're lazy or prototyping you can simply copy the AI of a different character to be used
            //Modules.Prefabs.CloneDopplegangerMaster(bodyPrefab, masterName, "Merc");

            //how to set up AI in code
            ScoutAI.Init(bodyPrefab, masterName);

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);
        }

        private void AddHooks()
        {
            HUD.onHudTargetChangedGlobal += HUDSetup;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.UI.LoadoutPanelController.Rebuild += LoadoutPanelController_Rebuild;
            On.RoR2.HealthComponent.TakeDamage += new On.RoR2.HealthComponent.hook_TakeDamage(HealthComponent_TakeDamage);
        }

        private static void LoadoutPanelController_Rebuild(On.RoR2.UI.LoadoutPanelController.orig_Rebuild orig, LoadoutPanelController self)
        {
            orig(self);

            if (self.currentDisplayData.bodyIndex == BodyCatalog.FindBodyIndex("ScoutBody"))
            {
                foreach (LanguageTextMeshController i in self.gameObject.GetComponentsInChildren<LanguageTextMeshController>())
                {
                    if (i && i.token == "LOADOUT_SKILL_MISC") i.token = "Passive";
                }
            }
        }
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (!NetworkServer.active)
            {
                return;
            }
            CharacterBody victimBody = self.body;
            EntityStateMachine victimMachine = victimBody.GetComponent<EntityStateMachine>();
            if (victimBody && victimBody.baseNameToken == "KENKO_SCOUT_NAME")
            {
                ScoutController scoutController = victimBody.GetComponent<ScoutController>();
                if (!scoutController.InGracePeriod())
                {
                    scoutController.FillAtomic(-10f, false);
                }
            }
            if (damageInfo.HasModdedDamageType(DamageTypes.AtomicCrits))
            {
                damageInfo.damage *= 1.25f;
                damageInfo.damageType |= DamageType.WeakOnHit;
            }
            if (damageInfo.HasModdedDamageType(DamageTypes.CleaverBonus))
            {
                if (victimMachine && (victimMachine.state is EntityStates.StunState || victimBody.HasBuff(ScoutBuffs.scoutStunMarker)))
                {
                    damageInfo.crit = true;
                    damageInfo.damageType &= ~DamageType.BlightOnHit;
                    damageInfo.damageType |= DamageType.PoisonOnHit;
                    Util.PlaySound("sfx_driver_blood_gurgle", self.gameObject);
                }
            }
            orig.Invoke(self, damageInfo);
            if (victimBody && victimBody.baseNameToken == "KENKO_SCOUT_NAME") victimBody.RecalculateStats();
        }
        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (!self || self.baseNameToken != "KENKO_SCOUT_NAME")
            {
                return;
            }
            ScoutController s = self.GetComponent<ScoutController>();
            HealthComponent healthComponent = self.GetComponent<HealthComponent>();
            SkillLocator skillLocator = self.GetComponent<SkillLocator>();
            if(s && healthComponent && !self.HasBuff(ScoutBuffs.scoutAtomicBuff))
            {
                if(s.atomicGauge > 0)
                {
                    self.moveSpeed += Util.Remap(s.atomicGauge, 0f, s.maxAtomicGauge, 0f, 3f);
                }
            }
            else if (self.HasBuff(ScoutBuffs.scoutAtomicBuff))
            {
                self.moveSpeed += 3f;
                self.attackSpeed += 1f;
            }
        }
        internal static void HUDSetup(HUD hud)
        {
            if (hud.targetBodyObject && hud.targetMaster && hud.targetMaster.bodyPrefab == ScoutSurvivor.characterPrefab)
            {
                if (!hud.targetMaster.hasAuthority) return;

                Transform skillsContainer = hud.equipmentIcons[0].gameObject.transform.parent;

                // ammo display for atomic
                Transform healthbarContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("BarRoots").Find("LevelDisplayCluster");

                GameObject atomicTracker = GameObject.Instantiate(healthbarContainer.gameObject, hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster"));
                atomicTracker.name = "AmmoTracker";
                atomicTracker.transform.SetParent(hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras"));

                GameObject.DestroyImmediate(atomicTracker.transform.GetChild(0).gameObject);
                MonoBehaviour.Destroy(atomicTracker.GetComponentInChildren<LevelText>());
                MonoBehaviour.Destroy(atomicTracker.GetComponentInChildren<ExpBar>());

                atomicTracker.transform.Find("LevelDisplayRoot").Find("ValueText").gameObject.SetActive(false);
                GameObject.DestroyImmediate(atomicTracker.transform.Find("ExpBarRoot").gameObject);

                atomicTracker.transform.Find("LevelDisplayRoot").GetComponent<RectTransform>().anchoredPosition = new Vector2(-12f, 0f);

                RectTransform rect = atomicTracker.GetComponent<RectTransform>();
                rect.localScale = new Vector3(0.8f, 0.8f, 1f);
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(0f, 0f);
                rect.offsetMin = new Vector2(120f, -40f);
                rect.offsetMax = new Vector2(120f, -40f);
                rect.pivot = new Vector2(0.5f, 0f);
                //positional data doesnt get sent to clients? Manually making offsets works..
                rect.anchoredPosition = new Vector2(50f, 0f);
                rect.localPosition = new Vector3(120f, -40f, 0f);

                GameObject chargeBarAmmo = GameObject.Instantiate(ScoutAssets.mainAssetBundle.LoadAsset<GameObject>("WeaponChargeBar"));
                chargeBarAmmo.name = "AtomicGauge";
                chargeBarAmmo.transform.SetParent(hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras"));

                rect = chargeBarAmmo.GetComponent<RectTransform>();

                rect.localScale = new Vector3(0.75f, 0.1f, 1f);
                rect.anchorMin = new Vector2(100f, 2f);
                rect.anchorMax = new Vector2(100f, 2f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(100f, 2f);
                rect.localPosition = new Vector3(100f, 2f, 0f);
                rect.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

                AtomicGauge atomicTrackerComponent = atomicTracker.AddComponent<AtomicGauge>();

                atomicTrackerComponent.targetHUD = hud;
                atomicTrackerComponent.targetText = atomicTracker.transform.Find("LevelDisplayRoot").Find("PrefixText").gameObject.GetComponent<LanguageTextMeshController>();
                atomicTrackerComponent.durationDisplay = chargeBarAmmo;
                atomicTrackerComponent.durationBar = chargeBarAmmo.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>();
                atomicTrackerComponent.durationBarRed = chargeBarAmmo.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();

            }
        }
    }
}