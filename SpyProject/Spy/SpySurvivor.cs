using BepInEx.Configuration;
using SpyMod.Modules;
using SpyMod.Modules.Characters;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using RoR2.UI;
using R2API;
using R2API.Networking;
using UnityEngine.Networking;
using UnityEngine.UI;
using SpyMod.Spy.Components;
using SpyMod.Spy.Content;
using SpyMod.Spy.SkillStates;
using HG;
using EntityStates;
using R2API.Networking.Interfaces;
using System.Security.Principal;

namespace SpyMod.Spy
{
    public class SpySurvivor : SurvivorBase<SpySurvivor>
    {
        public override string assetBundleName => "spy";
        public override string bodyName => "SpyBody";
        public override string masterName => "SpyMonsterMaster";
        public override string modelPrefabName => "mdlSpy";
        public override string displayPrefabName => "SpyDisplay";

        public const string SPY_PREFIX = SpyPlugin.DEVELOPER_PREFIX + "_SPY_";
        public override string survivorTokenPrefix => SPY_PREFIX;

        internal static GameObject characterPrefab;

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = SPY_PREFIX + "NAME",
            subtitleNameToken = SPY_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texSpyIcon"),
            bodyColor = SpyAssets.spyColor,
            sortPosition = 5.99f,

            crosshair = Assets.LoadCrosshair("SimpleDot"),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 100f,
            healthRegen = 1.2f,
            armor = 0f,
            damage = 12f,

            damageGrowth = 2.4f,
            healthGrowth = 100f * 0.35f,


            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "Model",
                    dontHotpoo = true,
                },
                new CustomRendererInfo
                {
                    childName = "Revolver",
                    dontHotpoo = true,
                },
                new CustomRendererInfo
                {
                    childName = "Knife",
                    dontHotpoo = true,
                },
                new CustomRendererInfo
                {
                    childName = "Watch",
                    dontHotpoo = true,
                },
                new CustomRendererInfo
                {
                    childName = "Tie",
                    dontHotpoo = true,
                }

        };

        public override UnlockableDef characterUnlockableDef => SpyUnlockables.characterUnlockableDef;

        public override ItemDisplaysBase itemDisplays => new SpyItemDisplay();
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
            SpyConfig.Init();

            SpyUnlockables.Init();

            SpyCrosshair.Init(assetBundle);

            base.InitializeCharacter();

            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            childLocator.FindChild("Knife").gameObject.SetActive(false);
            childLocator.FindChild("Watch").gameObject.SetActive(false);
            DamageTypes.Init();

            SpyStates.Init();
            SpyTokens.Init();

            SpyAssets.InitAssets();

            SpyBuffs.Init(assetBundle);

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
            bodyPrefab.AddComponent<SpyController>();
        }
        public void AddHitboxes()
        {
            Prefabs.SetupHitBoxGroup(characterModelObject, "KnifeHitbox", "KnifeHitbox");
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
            Prefabs.AddEntityStateMachine(bodyPrefab, "Watch");
        }

        #region skills
        public override void InitializeSkills()
        {
            bodyPrefab.AddComponent<SpyPassive>();
            Skills.CreateSkillFamilies(bodyPrefab);
            AddPassiveSkills();
            AddPrimarySkills();
            AddSecondarySkills();
            AddUtilitySkills();
            AddSpecialSkills();
        }

        private void AddPassiveSkills()
        {
            SpyPassive passive = bodyPrefab.GetComponent<SpyPassive>();

            SkillLocator skillLocator = bodyPrefab.GetComponent<SkillLocator>();

            skillLocator.passiveSkill.enabled = false;

            passive.doubleJumpPassive = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = SPY_PREFIX + "PASSIVE_NAME",
                skillNameToken = SPY_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = SPY_PREFIX + "PASSIVE_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpyPassive"),
                keywordTokens = new string[] { },
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
            SkillDef Shoot = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "The Diamondback",
                skillNameToken = SPY_PREFIX + "PRIMARY_REVOLVER_NAME",
                skillDescriptionToken = SPY_PREFIX + "PRIMARY_REVOLVER_DESCRIPTION",
                keywordTokens = new string[] { Tokens.spyCritKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpyRevolver"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(Shoot)),

                activationStateMachineName = "Weapon",
                interruptPriority = InterruptPriority.Any,

                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                rechargeStock = 0,
                requiredStock = 1,
                stockToConsume = 0,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = false,
                mustKeyPress = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,
            });

            Skills.AddPrimarySkills(bodyPrefab, Shoot);
        }

        private void AddSecondarySkills()
        {
            SkillDef Knife = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Stab",
                skillNameToken = SPY_PREFIX + "SECONDARY_KNIFE_NAME",
                skillDescriptionToken = SPY_PREFIX + "SECONDARY_KNIFE_DESCRIPTION",
                keywordTokens = new string[] { Tokens.agileKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpyStab"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(ChargeKnife)),

                activationStateMachineName = "Weapon",
                interruptPriority = InterruptPriority.Skill,

                baseMaxStock = 1,
                baseRechargeInterval = 7f,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = true,
                mustKeyPress = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            Skills.AddSecondarySkills(bodyPrefab, Knife);
        }

        private void AddUtilitySkills()
        {
            SkillDef flip = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Flip",
                skillNameToken = SPY_PREFIX + "UTILITY_FLIP_NAME",
                skillDescriptionToken = SPY_PREFIX + "UTILITY_FLIP_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpyFlip"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(Flip)),
                activationStateMachineName = "Weapon2",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 4f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = true,

            });

            Skills.AddUtilitySkills(bodyPrefab, flip);
        }

        private void AddSpecialSkills()
        {
            SkillDef Watch = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Watch",
                skillNameToken = SPY_PREFIX + "SPECIAL_WATCH_NAME",
                skillDescriptionToken = SPY_PREFIX + "SPECIAL_WATCH_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpyWatch"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SwapWatch)),
                activationStateMachineName = "Watch",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 16f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            Skills.AddSpecialSkills(bodyPrefab, Watch);
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
            SpyAI.Init(bodyPrefab, masterName);

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);
        }

        private void AddHooks()
        {
            //HUD.onHudTargetChangedGlobal += HUDSetup;
            On.RoR2.UI.LoadoutPanelController.Rebuild += LoadoutPanelController_Rebuild;
            On.RoR2.HealthComponent.TakeDamage += new On.RoR2.HealthComponent.hook_TakeDamage(HealthComponent_TakeDamage);
            RoR2.GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private static void LoadoutPanelController_Rebuild(On.RoR2.UI.LoadoutPanelController.orig_Rebuild orig, LoadoutPanelController self)
        {
            orig(self);

            if (self.currentDisplayData.bodyIndex == BodyCatalog.FindBodyIndex("SpyBody"))
            {
                foreach (LanguageTextMeshController i in self.gameObject.GetComponentsInChildren<LanguageTextMeshController>())
                {
                    if (i && i.token == "LOADOUT_SKILL_MISC") i.token = "Passive";
                }
            }
        }
        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self)
            {
                if (self.HasBuff(SpyBuffs.spyWatchDebuff))
                {
                    self.attackSpeed -= 0.3f;
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
            CharacterBody attackerBody = null;
            TeamIndex teamIndex = TeamIndex.None;
            Vector3 vector = Vector3.zero;
            bool flag = false;
            if (damageInfo.attacker)
            {
                attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if ((bool)attackerBody)
                {
                    teamIndex = attackerBody.teamComponent.teamIndex;
                    vector = attackerBody.corePosition - damageInfo.position;
                }
            }
            if (damageInfo.damage > 0)
            {
                EntityStateMachine victimMachine = victimBody.GetComponent<EntityStateMachine>();
                if (victimBody && victimBody.baseNameToken == "KENKO_SPY_NAME")
                {
                    SpyController spyController = victimBody.GetComponent<SpyController>();
                    if (spyController)
                    {
                        if (spyController.stopwatchOut)
                        {
                            if (self.gameObject.TryGetComponent<NetworkIdentity>(out var identity))
                            {
                                new SyncStealth(identity.netId, self.gameObject).Send(NetworkDestination.Clients);
                            }
                            damageInfo.damage = victimBody.healthComponent.health * 0.4f;
                            victimBody.RemoveBuff(SpyBuffs.spyWatchDebuff);
                            victimBody.AddBuff(RoR2Content.Buffs.Cloak);
                            victimBody.AddBuff(RoR2Content.Buffs.CloakSpeed);
                            flag = true;
                        }
                    }
                }
                if (attackerBody)
                {
                    if (attackerBody.canPerformBackstab && (damageInfo.damageType & DamageType.DoT) != DamageType.DoT && (damageInfo.procChainMask.HasProc(ProcType.Backstab) || BackstabManager.IsBackstab(-vector, victimBody)))
                    {
                        if (damageInfo.HasModdedDamageType(DamageTypes.BackStab))
                        {
                            damageInfo.crit = true;
                            damageInfo.procChainMask.AddProc(ProcType.Backstab);
                            damageInfo.damageType |= DamageType.BypassArmor;
                            Util.PlaySound("sfx_spy_crit", attackerBody.gameObject);
                            if (victimBody.isBoss)
                            {
                                damageInfo.damage *= 3f;
                            }
                            else if (victimBody.isElite)
                            {
                                damageInfo.damage *= 3f;
                                damageInfo.damage += (victimBody.maxHealth * 0.5f);
                            }
                            else
                            {
                                damageInfo.damage += victimBody.healthComponent.fullCombinedHealth;
                            }
                        }
                        else
                        {
                            damageInfo.damageType |= DamageType.DoT;
                        }
                    }
                }
            }
            orig.Invoke(self, damageInfo);
            if(flag) victimBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 1f);
        }
        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (damageReport.attackerBody && damageReport.attackerMaster && damageReport.victim)
            {
                if (damageReport.attackerBody.baseNameToken == "KENKO_SPY_NAME" &&
                damageReport.damageInfo.HasModdedDamageType(DamageTypes.BackStab) && damageReport.damageInfo.procChainMask.HasProc(ProcType.Backstab))
                {
                    SpyController spy = damageReport.attackerBody.GetComponent<SpyController>();
                    if (spy) spy.ActivateCritLightning();
                    if (NetworkServer.active)
                    {
                        damageReport.attackerBody.AddBuff(SpyBuffs.spyDiamondbackBuff);
                    }

                    if (damageReport.victim.gameObject.TryGetComponent<NetworkIdentity>(out var identity))
                    {
                        new SyncStabExplosion(identity.netId, damageReport.victim.gameObject).Send(NetworkDestination.Clients);
                    }
                }
            }
            /*
            internal static void HUDSetup(HUD hud)
            {
                if (hud.targetBodyObject && hud.targetMaster && hud.targetMaster.bodyPrefab == SpySurvivor.characterPrefab)
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

                    GameObject chargeBarAmmo = GameObject.Instantiate(SpyAssets.mainAssetBundle.LoadAsset<GameObject>("WeaponChargeBar"));
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

                    Stealth atomicTrackerComponent = atomicTracker.AddComponent<Stealth>();

                    atomicTrackerComponent.targetHUD = hud;
                    atomicTrackerComponent.targetText = atomicTracker.transform.Find("LevelDisplayRoot").Find("PrefixText").gameObject.GetComponent<LanguageTextMeshController>();
                    atomicTrackerComponent.durationDisplay = chargeBarAmmo;
                    atomicTrackerComponent.durationBar = chargeBarAmmo.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>();
                    atomicTrackerComponent.durationBarRed = chargeBarAmmo.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();

                }
            }
            */
        }
    }
}