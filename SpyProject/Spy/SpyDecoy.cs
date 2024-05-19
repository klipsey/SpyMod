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
using RoR2.CharacterAI;

namespace SpyMod.Spy
{
    public class SpyDecoy : CharacterBase<SpyDecoy>
    {
        public override string assetBundleName => "spy";
        public override string bodyName => "SpyDecoyBody";
        public override string modelPrefabName => "mdlSpyDecoy";

        public const string SPY_PREFIX = SpyPlugin.DEVELOPER_PREFIX + "_SPY_DECOY_";

        internal static GameObject characterPrefab;

        public static GameObject decoyMasterPrefab;
        public static SpawnCard decoySpawnCard;

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = SPY_PREFIX + "NAME",
            subtitleNameToken = SPY_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texSpyIcon"),
            bodyColor = SpyAssets.spyColor,

            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            podPrefab = null,

            maxHealth = 1f,
            healthRegen = 0f,
            armor = 0f,
            damage = 0f,
            moveSpeed = 0f,
            moveSpeedGrowth = 0f,
            damageGrowth = 0f,
            healthGrowth = 0,


            jumpCount = 0,
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
        public override ItemDisplaysBase itemDisplays => null;
        public override AssetBundle assetBundle { get; protected set; }
        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
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
            base.InitializeCharacter();

            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            childLocator.FindChild("Knife").gameObject.SetActive(false);
            childLocator.FindChild("Watch").gameObject.SetActive(false);

            bodyPrefab.layer = LayerIndex.fakeActor.intVal;

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeCharacterMaster();

            characterPrefab = this.bodyPrefab;
        }

        public override void InitializeEntityStateMachines()
        {
            //clear existing state machines from your cloned body (probably commando)
            //omit all this if you want to just keep theirs
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //the main "Body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(EntityStates.GenericCharacterMain), typeof(SkillStates.DecoySpawn));
            //if you set up a custom main characterstate, set it up here
            //don't forget to register custom entitystates in your HenryStates.cs
        }

        #region skills
        public override void InitializeSkills()
        {
            Skills.CreateDecoySkillFamilies(bodyPrefab);
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

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController

            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin

            ////creating a new skindef as we did before

            ////adding the mesh replacements as above. 
            ////if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved

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
            GameObject decoyMasterPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianMaster.prefab").WaitForCompletion(), "SpyDecoyMaster");
            CharacterMaster characterMaster = decoyMasterPrefab.GetComponent<CharacterMaster>();
            characterMaster.bodyPrefab = this.bodyPrefab;

            BaseAI decoyAI = decoyMasterPrefab.GetComponent<BaseAI>();
            decoyAI.neverRetaliateFriendlies = true;
            decoyAI.aimVectorMaxSpeed *= 3f;
            decoyAI.fullVision = true;

            InitializeSkillDrivers(decoyMasterPrefab);

            SpyDecoy.decoyMasterPrefab = decoyMasterPrefab;
            Modules.Content.AddMasterPrefab(decoyMasterPrefab);
            CreateSpawnCard();

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);
        }
        private void CreateSpawnCard()
        {
            CharacterSpawnCard card = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            card.name = "cssSpyDecoy";
            card.prefab = decoyMasterPrefab;
            card.sendOverNetwork = true;
            card.hullSize = HullClassification.Human;
            card.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            card.requiredFlags = RoR2.Navigation.NodeFlags.None;
            card.forbiddenFlags = RoR2.Navigation.NodeFlags.NoCharacterSpawn;
            card.eliteRules = SpawnCard.EliteRules.Default;
            card.occupyPosition = false;

            SpyDecoy.decoySpawnCard = card;
        }
        protected virtual void InitializeSkillDrivers(GameObject masterPrefab)
        {
            foreach (AISkillDriver i in masterPrefab.GetComponentsInChildren<AISkillDriver>())
            {
                UnityEngine.Object.DestroyImmediate(i);
            }

            AISkillDriver biteDriver = masterPrefab.AddComponent<AISkillDriver>();
            biteDriver.customName = "DoNothing1";
            biteDriver.skillSlot = SkillSlot.None;
            biteDriver.requireSkillReady = true;
            biteDriver.minUserHealthFraction = float.NegativeInfinity;
            biteDriver.maxUserHealthFraction = float.PositiveInfinity;
            biteDriver.minTargetHealthFraction = float.NegativeInfinity;
            biteDriver.maxTargetHealthFraction = float.PositiveInfinity;
            biteDriver.minDistance = 0f;
            biteDriver.maxDistance = float.PositiveInfinity;
            biteDriver.activationRequiresAimConfirmation = false;
            biteDriver.activationRequiresTargetLoS = false;
            biteDriver.selectionRequiresTargetLoS = false;
            biteDriver.maxTimesSelected = -1;
            biteDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            biteDriver.movementType = AISkillDriver.MovementType.Stop;
            biteDriver.aimType = AISkillDriver.AimType.None;
            biteDriver.moveInputScale = 1f;
            biteDriver.ignoreNodeGraph = false;
            biteDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver spitDriver = masterPrefab.AddComponent<AISkillDriver>();
            spitDriver.customName = "DoNothing2";
            spitDriver.skillSlot = SkillSlot.None;
            spitDriver.requireSkillReady = true;
            spitDriver.minUserHealthFraction = float.NegativeInfinity;
            spitDriver.maxUserHealthFraction = float.PositiveInfinity;
            spitDriver.minTargetHealthFraction = float.NegativeInfinity;
            spitDriver.maxTargetHealthFraction = float.PositiveInfinity;
            spitDriver.minDistance = 0f;
            spitDriver.maxDistance = float.PositiveInfinity;
            spitDriver.activationRequiresAimConfirmation = false;
            spitDriver.activationRequiresTargetLoS = false;
            spitDriver.selectionRequiresTargetLoS = false;
            spitDriver.maxTimesSelected = -1;
            spitDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            spitDriver.movementType = AISkillDriver.MovementType.Stop;
            spitDriver.aimType = AISkillDriver.AimType.None;
            spitDriver.moveInputScale = 1f;
            spitDriver.ignoreNodeGraph = false;
            spitDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver strafeDriver = masterPrefab.AddComponent<AISkillDriver>();
            strafeDriver.customName = "DoNothing3";
            strafeDriver.skillSlot = SkillSlot.None;
            strafeDriver.requireSkillReady = true;
            strafeDriver.minUserHealthFraction = float.NegativeInfinity;
            strafeDriver.maxUserHealthFraction = float.PositiveInfinity;
            strafeDriver.minTargetHealthFraction = float.NegativeInfinity;
            strafeDriver.maxTargetHealthFraction = float.PositiveInfinity;
            strafeDriver.minDistance = 0f;
            strafeDriver.maxDistance = float.PositiveInfinity;
            strafeDriver.activationRequiresAimConfirmation = false;
            strafeDriver.activationRequiresTargetLoS = false;
            strafeDriver.selectionRequiresTargetLoS = false;
            strafeDriver.maxTimesSelected = -1;
            strafeDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            strafeDriver.movementType = AISkillDriver.MovementType.Stop;
            strafeDriver.aimType = AISkillDriver.AimType.None;
            strafeDriver.moveInputScale = 1f;
            strafeDriver.ignoreNodeGraph = false;
            strafeDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver chaseNearDriver = masterPrefab.AddComponent<AISkillDriver>();
            chaseNearDriver.customName = "DoNothing4";
            chaseNearDriver.skillSlot = SkillSlot.None;
            chaseNearDriver.requireSkillReady = true;
            chaseNearDriver.minUserHealthFraction = float.NegativeInfinity;
            chaseNearDriver.maxUserHealthFraction = float.PositiveInfinity;
            chaseNearDriver.minTargetHealthFraction = float.NegativeInfinity;
            chaseNearDriver.maxTargetHealthFraction = float.PositiveInfinity;
            chaseNearDriver.minDistance = 0f;
            chaseNearDriver.maxDistance = float.PositiveInfinity;
            chaseNearDriver.activationRequiresAimConfirmation = false;
            chaseNearDriver.activationRequiresTargetLoS = false;
            chaseNearDriver.selectionRequiresTargetLoS = false;
            chaseNearDriver.maxTimesSelected = -1;
            chaseNearDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chaseNearDriver.movementType = AISkillDriver.MovementType.Stop;
            chaseNearDriver.aimType = AISkillDriver.AimType.None;
            chaseNearDriver.moveInputScale = 1f;
            chaseNearDriver.ignoreNodeGraph = false;
            chaseNearDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver chaseFarDriver = masterPrefab.AddComponent<AISkillDriver>();
            chaseFarDriver.customName = "DoNothing5";
            chaseFarDriver.skillSlot = SkillSlot.None;
            chaseFarDriver.requireSkillReady = true;
            chaseFarDriver.minUserHealthFraction = float.NegativeInfinity;
            chaseFarDriver.maxUserHealthFraction = float.PositiveInfinity;
            chaseFarDriver.minTargetHealthFraction = float.NegativeInfinity;
            chaseFarDriver.maxTargetHealthFraction = float.PositiveInfinity;
            chaseFarDriver.minDistance = 0f;
            chaseFarDriver.maxDistance = float.PositiveInfinity;
            chaseFarDriver.activationRequiresAimConfirmation = false;
            chaseFarDriver.activationRequiresTargetLoS = false;
            chaseFarDriver.selectionRequiresTargetLoS = false;
            chaseFarDriver.maxTimesSelected = -1;
            chaseFarDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chaseFarDriver.movementType = AISkillDriver.MovementType.Stop;
            chaseFarDriver.aimType = AISkillDriver.AimType.None;
            chaseFarDriver.moveInputScale = 1f;
            chaseFarDriver.ignoreNodeGraph = false;
            chaseFarDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver idleDriver = masterPrefab.AddComponent<AISkillDriver>();
            idleDriver.customName = "DoNothing6";
            idleDriver.skillSlot = SkillSlot.None;
            idleDriver.requireSkillReady = true;
            idleDriver.minUserHealthFraction = float.NegativeInfinity;
            idleDriver.maxUserHealthFraction = float.PositiveInfinity;
            idleDriver.minTargetHealthFraction = float.NegativeInfinity;
            idleDriver.maxTargetHealthFraction = float.PositiveInfinity;
            idleDriver.minDistance = 0f;
            idleDriver.maxDistance = float.PositiveInfinity;
            idleDriver.activationRequiresAimConfirmation = false;
            idleDriver.activationRequiresTargetLoS = false;
            idleDriver.selectionRequiresTargetLoS = false;
            idleDriver.maxTimesSelected = -1;
            idleDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            idleDriver.movementType = AISkillDriver.MovementType.Stop;
            idleDriver.aimType = AISkillDriver.AimType.None;
            idleDriver.moveInputScale = 1f;
            idleDriver.ignoreNodeGraph = false;
            idleDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
        }
    }
}