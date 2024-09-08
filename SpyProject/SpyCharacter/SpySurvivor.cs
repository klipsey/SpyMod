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
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using SpyMod.Spy.Components;
using SpyMod.Spy.Content;
using SpyMod.Spy.SkillStates;
using HG;
using EntityStates;
using R2API.Networking.Interfaces;
using EmotesAPI;
using System.Runtime.CompilerServices;

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

        public static SkillDef cloakScepterSkillDef;

        public static SkillDef deadmanScepterSkillDef;

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = SPY_PREFIX + "NAME",
            subtitleNameToken = SPY_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texSpyIcon"),
            bodyColor = SpyAssets.spyColor,
            sortPosition = 7f,

            crosshair = Modules.CharacterAssets.LoadCrosshair("Standard"),
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
                },
                new CustomRendererInfo
                {
                    childName = "Visor",
                    dontHotpoo = true,
                }

        };

        public override UnlockableDef characterUnlockableDef => SpyUnlockables.characterUnlockableDef;

        public override ItemDisplaysBase itemDisplays => new SpyItemDisplays();
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

            base.InitializeCharacter();

            SpyCrosshair.Init(assetBundle);

            CameraParams.InitializeParams();

            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            childLocator.FindChild("Knife").gameObject.SetActive(false);
            childLocator.FindChild("Watch").gameObject.SetActive(false);
            DamageTypes.Init();

            SpyStates.Init();
            SpyTokens.Init();

            SpyAssets.Init(assetBundle);

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
            if (SpyPlugin.scepterInstalled) InitializeScepter();
        }

        private void AddPassiveSkills()
        {
            SpyPassive passive = bodyPrefab.GetComponent<SpyPassive>();

            SkillLocator skillLocator = bodyPrefab.GetComponent<SkillLocator>();

            skillLocator.passiveSkill.enabled = false;

            passive.spyPassive = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = SPY_PREFIX + "PASSIVE_NAME",
                skillNameToken = SPY_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = SPY_PREFIX + "PASSIVE_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpyPassive"),
                keywordTokens = new string[] { Tokens.spyBackstabKeyword  },
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

            Skills.AddPassiveSkills(passive.passiveSkillSlot.skillFamily, passive.spyPassive);
        }

        private void AddPrimarySkills()
        {
            SpySkillDef Shoot = Skills.CreateSkillDef<SpySkillDef>(new SkillDefInfo
            {
                skillName = "Diamondback",
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

            SpySkillDef Shoot2 = Skills.CreateSkillDef<SpySkillDef>(new SkillDefInfo
            {
                skillName = "Ambassador",
                skillNameToken = SPY_PREFIX + "PRIMARY_REVOLVER2_NAME",
                skillDescriptionToken = SPY_PREFIX + "PRIMARY_REVOLVER2_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpyAmbassador"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(Shoot2)),

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

            Skills.AddPrimarySkills(bodyPrefab, Shoot, Shoot2);
        }

        private void AddSecondarySkills()
        {
            SpySkillDef Knife = Skills.CreateSkillDef<SpySkillDef>(new SkillDefInfo
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
                baseRechargeInterval = 5f,
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

            SpySkillDef Knife2 = Skills.CreateSkillDef<SpySkillDef>(new SkillDefInfo
            {
                skillName = "BigEarner",
                skillNameToken = SPY_PREFIX + "SECONDARY_KNIFE2_NAME",
                skillDescriptionToken = SPY_PREFIX + "SECONDARY_KNIFE2_DESCRIPTION",
                keywordTokens = new string[] { Tokens.agileKeyword, Tokens.spyBigEarnerKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpyStab2"),

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

            Skills.AddSecondarySkills(bodyPrefab, Knife, Knife2);
        }

        private void AddUtilitySkills()
        {
            SkillDef flip = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Sap",
                skillNameToken = SPY_PREFIX + "UTILITY_FLIP_NAME",
                skillDescriptionToken = SPY_PREFIX + "UTILITY_FLIP_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpyFlip"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(Sap)),
                activationStateMachineName = "Weapon2",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 8f,
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
                skillName = "Cloak",
                skillNameToken = SPY_PREFIX + "SPECIAL_WATCH_NAME",
                skillDescriptionToken = SPY_PREFIX + "SPECIAL_WATCH_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpyCloak"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(Cloak)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 0f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = true,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            SkillDef Watch2 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "DeadmansWatch",
                skillNameToken = SPY_PREFIX + "SPECIAL_WATCH2_NAME",
                skillDescriptionToken = SPY_PREFIX + "SPECIAL_WATCH2_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpyWatch"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SwapWatch)),
                activationStateMachineName = "Watch",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 10f,
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

            Skills.AddSpecialSkills(bodyPrefab, Watch, Watch2);
        }

        private void InitializeScepter()
        {
            cloakScepterSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Cloak Scepter",
                skillNameToken = SPY_PREFIX + "SPECIAL_SCEPTER_WATCH_NAME",
                skillDescriptionToken = SPY_PREFIX + "SPECIAL_SCEPTER_WATCH_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpyCloak"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(CloakScepter)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 0f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = true,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            deadmanScepterSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Deadman's Watch Scepter",
                skillNameToken = SPY_PREFIX + "SPECIAL_SCEPTER_WATCH2_NAME",
                skillDescriptionToken = SPY_PREFIX + "SPECIAL_SCEPTER_WATCH2_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpyWatch"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SwapWatchScepter)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 10f,
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

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(cloakScepterSkillDef, bodyName, SkillSlot.Special, 0);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(deadmanScepterSkillDef, bodyName, SkillSlot.Special, 1);
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
            defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "meshSpy",
                "meshRevolver",
                "meshKnife",
                "meshWatch",
                "meshTie",
                "meshVisor");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController

            defaultSkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("Tie"),
                    shouldActivate = true,
                }
            };

            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin

            ////creating a new skindef as we did before
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(SPY_PREFIX + "MASTERY_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMonsoonSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                SpyUnlockables.masterySkinUnlockableDef);

            ////adding the mesh replacements as above. 
            ////if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "meshSpyAlt",
                "meshRevolverAlt",//no gun mesh replacement. use same gun mesh
                "meshKnifeAlt",
                "meshWatchAlt",
                null,
                "meshVisorAlt");

            ////masterySkin has a new set of RendererInfos (based on default rendererinfos)
            ////you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            masterySkin.rendererInfos[0].defaultMaterial = SpyAssets.spyMonsoonMat;
            masterySkin.rendererInfos[1].defaultMaterial = SpyAssets.spyMonsoonMat;
            masterySkin.rendererInfos[2].defaultMaterial = SpyAssets.spyMonsoonMat;
            masterySkin.rendererInfos[3].defaultMaterial = SpyAssets.spyMonsoonMat;
            masterySkin.rendererInfos[5].defaultMaterial = SpyAssets.spyVisorMonsoonMat;

            ////here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("Tie"),
                    shouldActivate = false,
                }
            };
            ////simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            skins.Add(masterySkin);

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
            HUD.onHudTargetChangedGlobal += HUDSetup;
            On.RoR2.UI.LoadoutPanelController.Rebuild += LoadoutPanelController_Rebuild;
            On.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
            RoR2.GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            if(SpyPlugin.emotesInstalled) Emotes();
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.bodyIndex == BodyCatalog.FindBodyIndex("SpyBody"))
            {
                SpyController spy = sender.gameObject.GetComponent<SpyController>();
                if (spy)
                {
                    if (sender.HasBuff(SpyBuffs.armorBuff)) args.armorAdd += 100f;
                    if (sender.HasBuff(SpyBuffs.spyBigEarnerBuff)) args.moveSpeedMultAdd += (sender.GetBuffCount(SpyBuffs.spyBigEarnerBuff) * 0.15f);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void Emotes()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();
                var skele = SpyAssets.mainAssetBundle.LoadAsset<GameObject>("spy_emoteskeleton");
                CustomEmotesAPI.ImportArmature(SpySurvivor.characterPrefab, skele);
            };
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
        private void HealthComponent_TakeDamageProcess(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (!NetworkServer.active)
            {
                return;
            }
            if (!self.alive || self.godMode || self.ospTimer > 0f)
            {
                return;
            }
            CharacterBody victimBody = self.body;
            CharacterBody attackerBody = null;
            Vector3 vector = Vector3.zero;
            if (damageInfo.attacker)
            {
                attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if (attackerBody)
                {
                    vector = attackerBody.corePosition - damageInfo.position;
                }
            }
            if (damageInfo.damage > 0 && !damageInfo.rejected && victimBody)
            {
                if (victimBody.bodyIndex == BodyCatalog.FindBodyIndex("SpyBody"))
                {
                    SpyController spyController = victimBody.GetComponent<SpyController>();
                    if (spyController)
                    {
                        if (spyController.IsStopWatchOut() && spyController.isSpecialDeadman)
                        {
                            if (self.gameObject.TryGetComponent<NetworkIdentity>(out var identity))
                            {
                                new SyncStealth(identity.netId, self.gameObject).Send(NetworkDestination.Clients);
                            }
                            
                            damageInfo.rejected = true;

                            DamageInfo stealthInfo = new DamageInfo();
                            if (damageInfo.damage >= (victimBody.healthComponent.combinedHealth) * SpyConfig.cloakHealthCost.Value)
                            {
                                stealthInfo.damage = (victimBody.healthComponent.combinedHealth) * SpyConfig.cloakHealthCost.Value;
                                stealthInfo.damageType = DamageType.BypassArmor | DamageType.Silent | DamageType.NonLethal;
                            }
                            else
                            {
                                stealthInfo.damage = damageInfo.damage;
                                stealthInfo.damageType = DamageType.Silent | DamageType.NonLethal;
                            }
                            stealthInfo.attacker = null;
                            stealthInfo.canRejectForce = true;
                            stealthInfo.crit = false;
                            stealthInfo.inflictor = null;
                            stealthInfo.damageColorIndex = DamageColorIndex.Default;
                            stealthInfo.force = Vector3.zero;
                            stealthInfo.rejected = false;
                            stealthInfo.position = victimBody.corePosition;
                            stealthInfo.procChainMask = default(ProcChainMask);
                            stealthInfo.procCoefficient = 0f;

                            if (victimBody.skillLocator.special.skillNameToken != SPY_PREFIX + "SPECIAL_SCEPTER_WATCH2_NAME")
                            {
                                victimBody.healthComponent.TakeDamage(stealthInfo);
                            }

                            MasterSummon masterSummon = new MasterSummon();
                            masterSummon.masterPrefab = SpyDecoy.decoyMasterPrefab;
                            masterSummon.ignoreTeamMemberLimit = true;
                            masterSummon.teamIndexOverride = TeamIndex.Player;
                            masterSummon.summonerBodyObject = victimBody.gameObject;
                            masterSummon.position = victimBody.previousPosition;
                            masterSummon.rotation = Util.QuaternionSafeLookRotation(victimBody.characterDirection.forward);

                            CharacterMaster decoyMaster;
                            decoyMaster = masterSummon.Perform();

                            Util.CleanseBody(victimBody, true, false, false, true, true, true);
                            if(victimBody.HasBuff(SpyBuffs.spyWatchDebuff)) victimBody.RemoveBuff(SpyBuffs.spyWatchDebuff);
                            victimBody.AddBuff(RoR2Content.Buffs.Cloak);
                            victimBody.AddBuff(RoR2Content.Buffs.CloakSpeed);
                            victimBody.AddBuff(SpyBuffs.armorBuff);
                        }
                    }
                }

                if (attackerBody && attackerBody.bodyIndex == BodyCatalog.FindBodyIndex("SpyBody") && !damageInfo.HasModdedDamageType(DamageTypes.SpyExecute))
                {
                    if (attackerBody.canPerformBackstab && (damageInfo.damageType & DamageType.DoT) != DamageType.DoT && (damageInfo.procChainMask.HasProc(ProcType.Backstab) || BackstabManager.IsBackstab(-vector, victimBody)))
                    {
                        if (damageInfo.HasModdedDamageType(DamageTypes.SpyBackStab))
                        {
                            float initialDamage = damageInfo.damage;
                            if (damageInfo.crit) damageInfo.damage += initialDamage * 1.5f;
                            damageInfo.damage *= SpyConfig.spyBackstabMultiplier.Value;
                            damageInfo.crit = true;
                            damageInfo.procChainMask.AddProc(ProcType.Backstab);
                            damageInfo.damageType |= DamageType.BypassArmor;
                            damageInfo.damageType |= DamageType.Silent;
                            damageInfo.AddModdedDamageType(DamageTypes.SpyExecute);
                            Util.PlaySound("sfx_spy_crit", attackerBody.gameObject);

                            if (victimBody.healthComponent)
                            {
                                SpyController spy = attackerBody.GetComponent<SpyController>();
                                if (victimBody.isChampion || victimBody.isBoss && spy)
                                {
                                    if (spy.isDiamondBack)
                                    {
                                        for(int i = attackerBody.GetBuffCount(SpyBuffs.spyDiamondbackBuff); i < 3; i++)
                                        {
                                            if(attackerBody.GetBuffCount(SpyBuffs.spyDiamondbackBuff) < 5) attackerBody.AddBuff(SpyBuffs.spyDiamondbackBuff);
                                        }
                                    }

                                    if(spy.isBigEarner)
                                    {
                                        int num = 5;
                                        float num2 = 2f;
                                        attackerBody.ClearTimedBuffs(SpyBuffs.spyBigEarnerBuff);
                                        for (int i = 0; i < num; i++)
                                        {
                                            attackerBody.AddTimedBuff(SpyBuffs.spyBigEarnerBuff, num2 * (i + 1) / num);
                                        }

                                        attackerBody.healthComponent.AddBarrier((attackerBody.healthComponent.fullHealth + attackerBody.healthComponent.fullShield) * 0.2f);
                                    }
                                }
                                else if (victimBody.isElite && spy)
                                {
                                    if (spy.isDiamondBack)
                                    {
                                        if (attackerBody.GetBuffCount(SpyBuffs.spyDiamondbackBuff) < 5) attackerBody.AddBuff(SpyBuffs.spyDiamondbackBuff);
                                    }
                                }
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
        }
        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            CharacterBody attackerBody = damageReport.attackerBody;
            if (attackerBody && damageReport.attackerMaster && damageReport.victim)
            {
                if (attackerBody.bodyIndex == BodyCatalog.FindBodyIndex("SpyBody") && damageReport.damageInfo.HasModdedDamageType(DamageTypes.SpyExecute))
                {
                    SpyController spy = attackerBody.GetComponent<SpyController>();
                    if(spy)
                    {
                        if(attackerBody.skillLocator.special.skillNameToken == SPY_PREFIX + "SPECIAL_SCEPTER_WATCH2_NAME")
                        {
                            attackerBody.skillLocator.special.Reset();
                        }
                        if (spy.isDiamondBack)
                        {
                            if (attackerBody.GetBuffCount(SpyBuffs.spyDiamondbackBuff) <= 0) spy.ActivateCritLightning();
                            if (NetworkServer.active)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    if(attackerBody.GetBuffCount(SpyBuffs.spyDiamondbackBuff) < 5) attackerBody.AddBuff(SpyBuffs.spyDiamondbackBuff);
                                }
                            }
                        }

                        if(spy.isBigEarner)
                        {
                            if (NetworkServer.active && damageReport.attacker.TryGetComponent<NetworkIdentity>(out var identityStab))
                            {
                                new SyncResetStab(identityStab.netId).Send(NetworkDestination.Clients);
                            }

                            if(!SpyConfig.bigEarnerFullyResets.Value) spy.ResetChainStabPeriod();
                            int num = 5;
                            float num2 = 2f;
                            attackerBody.ClearTimedBuffs(SpyBuffs.spyBigEarnerBuff);
                            for(int i = 0; i < num; i++)
                            {
                                attackerBody.AddTimedBuff(SpyBuffs.spyBigEarnerBuff, num2 * (i + 1) / num);
                            }

                            attackerBody.healthComponent.AddBarrier((attackerBody.healthComponent.fullHealth + attackerBody.healthComponent.fullShield) * 0.1f);

                            EffectData effectData = new EffectData();
                            effectData.origin = attackerBody.corePosition;
                            CharacterMotor characterMotor = attackerBody.characterMotor;
                            bool flag = false;
                            if ((bool)characterMotor)
                            {
                                Vector3 moveDirection = characterMotor.moveDirection;
                                if (moveDirection != Vector3.zero)
                                {
                                    effectData.rotation = Util.QuaternionSafeLookRotation(moveDirection);
                                    flag = true;
                                }
                            }
                            if (!flag)
                            {
                                effectData.rotation = attackerBody.transform.rotation;
                            }
                            EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MoveSpeedOnKillActivate"), effectData, transmit: true);
                        }
                    }

                    if (damageReport.victim.gameObject.TryGetComponent<NetworkIdentity>(out var identity))
                    {
                        new SyncStabExplosion(identity.netId, damageReport.victim.gameObject).Send(NetworkDestination.Clients);
                    }
                }
            }
        }


            internal static void HUDSetup(HUD hud)
            {
            if (hud.targetBodyObject && hud.targetMaster && hud.targetMaster.bodyPrefab == SpySurvivor.characterPrefab)
            {
                if (!hud.targetMaster.hasAuthority) return;
                Transform skillsContainer = hud.equipmentIcons[0].gameObject.transform.parent;

                // ammo display for atomic
                Transform healthbarContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("BarRoots").Find("LevelDisplayCluster");

                GameObject stealthTracker = GameObject.Instantiate(healthbarContainer.gameObject, hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster"));
                stealthTracker.name = "AmmoTracker";
                stealthTracker.transform.SetParent(hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras"));

                GameObject.DestroyImmediate(stealthTracker.transform.GetChild(0).gameObject);
                MonoBehaviour.Destroy(stealthTracker.GetComponentInChildren<LevelText>());
                MonoBehaviour.Destroy(stealthTracker.GetComponentInChildren<ExpBar>());

                stealthTracker.transform.Find("LevelDisplayRoot").Find("ValueText").gameObject.SetActive(false);
                GameObject.DestroyImmediate(stealthTracker.transform.Find("ExpBarRoot").gameObject);

                stealthTracker.transform.Find("LevelDisplayRoot").GetComponent<RectTransform>().anchoredPosition = new Vector2(-12f, 0f);

                RectTransform rect = stealthTracker.GetComponent<RectTransform>();
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
                chargeBarAmmo.name = "StealthMeter";
                chargeBarAmmo.transform.SetParent(hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras"));

                rect = chargeBarAmmo.GetComponent<RectTransform>();

                rect.localScale = new Vector3(0.75f, 0.1f, 1f);
                rect.anchorMin = new Vector2(100f, 2f);
                rect.anchorMax = new Vector2(100f, 2f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(100f, 2f);
                rect.localPosition = new Vector3(100f, 2f, 0f);
                rect.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

                CloakHudController stealthComponent = stealthTracker.AddComponent<CloakHudController>();

                stealthComponent.targetHUD = hud;
                stealthComponent.targetText = stealthTracker.transform.Find("LevelDisplayRoot").Find("PrefixText").gameObject.GetComponent<LanguageTextMeshController>();
                stealthComponent.durationDisplay = chargeBarAmmo;
                stealthComponent.durationBar = chargeBarAmmo.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>();
                stealthComponent.durationBarColor = chargeBarAmmo.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();

                if (!hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("SpyCrosshair"))
                {
                    GameObject seamstressCrosshair = UnityEngine.Object.Instantiate(SpyCrosshair.spyCrosshair, hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas"));
                    seamstressCrosshair.name = "SpyCrosshair";
                    seamstressCrosshair.gameObject.GetComponent<HudElement>().targetBodyObject = hud.targetBodyObject;
                    seamstressCrosshair.gameObject.GetComponent<HudElement>().targetCharacterBody = hud.targetBodyObject.GetComponent<CharacterBody>();
                }
            }
        }
    }
}