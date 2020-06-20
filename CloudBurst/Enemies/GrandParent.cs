using CloudBurst.Weapon;
using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Skills;
using UnityEngine;
using static RoR2.CharacterAI.AISkillDriver;

namespace CloudBurst.Enemies
{
    //TODO:
    //Finish spawncard
    //Finish fixing skills

    [R2APISubmoduleDependency(new string[]
    {
        "LoadoutAPI",
        "AssetPlus",
        "DirectorAPI",

     })]
    internal sealed class GrandParent
    {
        private static GameObject grandParent;
        private static GameObject parentPod;
        private static GameObject grandParentMaster;
        private static SkillLocator skillLocator;
        private static GameObject swipeEffect;
        public static CharacterSpawnCard grandParentCharacterSpawnCard;
        public static void BuildGrandParents()
        {
            grandParent = Resources.Load<GameObject>("prefabs/characterbodies/GrandParentbody");
            grandParentMaster = Resources.Load<GameObject>("prefabs/charactermasters/GrandParentMaster");
            grandParentCharacterSpawnCard = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/titan/cscGrandParent");
            swipeEffect = Resources.Load<GameObject>("prefabs/effects/GrandparentGroundSwipeTrailEffect");
            parentPod = Resources.Load<GameObject>("prefabs/characterbodies/ParentPodBody");
            skillLocator = grandParent.GetComponent<SkillLocator>();
            BuildBody();
            BuildDirectorCard();
            RebuildSkillDrivers();
            RebuildSkills();
            FixGrandParentSwipeEffect();
            ModifyParentPod();
            //Main.logger.LogInfo("Built Grandparents!");

        }
        private static void FixGrandParentSwipeEffect()
        {
            if (!swipeEffect.HasComponent<EffectComponent>())
            {
                EffectComponent effectComponent = swipeEffect.AddComponent<EffectComponent>();
                effectComponent.positionAtReferencedTransform = true;
                effectComponent.parentToReferencedTransform = false;
                effectComponent.applyScale = false;
                effectComponent.disregardZScale = false;
            }
        }
        private static void BuildDirectorCard()
        {
            /*
            DirectorAPI.DirectorCardHolder grandParentDirectorCardHolder = new DirectorAPI.DirectorCardHolder();
            DirectorCard grandParentDirectorCard = new DirectorCard();

            grandParentCharacterSpawnCard.directorCreditCost = 350;
            grandParentCharacterSpawnCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Air;
            grandParentCharacterSpawnCard.noElites = false;

            //grandParentDirectorCard.
            grandParentDirectorCard.allowAmbushSpawn = true;
            grandParentDirectorCard.forbiddenUnlockable = "";
            grandParentDirectorCard.minimumStageCompletions = 4;
            grandParentDirectorCard.preventOverhead = true;
            grandParentDirectorCard.requiredUnlockable = "";
            grandParentDirectorCard.selectionWeight = 1;
            grandParentDirectorCard.spawnCard = grandParentCharacterSpawnCard;
            grandParentDirectorCard.spawnDistance = DirectorCore.MonsterSpawnDistance.Standard;

            grandParentDirectorCardHolder.Card = grandParentDirectorCard;
            grandParentDirectorCardHolder.InteractableCategory = 0;
            grandParentDirectorCardHolder.MonsterCategory = DirectorAPI.MonsterCategory.Minibosses;

            DirectorAPI.MonsterActions += delegate (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage)
            {

                if (!list.Contains(grandParentDirectorCardHolder))
                {
                    list.Add(grandParentDirectorCardHolder);
                }
            };*/
        }
        private static void BuildBody()
        {
            CharacterBody characterBody = grandParent.GetComponent<CharacterBody>();
            if (characterBody)
            {
                characterBody.baseAcceleration = 14f;
                characterBody.baseArmor = 29; //Base armor this character has, set to 20 if this character is melee 
                characterBody.baseAttackSpeed = 1; //Base attack speed, usually 1
                characterBody.baseCrit = 0;  //Base crit, usually one
                characterBody.baseDamage = 45; //Base damage
                characterBody.baseJumpCount = 0; //Base jump amount, set to 2 for a double jump. 
                characterBody.baseJumpPower = 0; //Base jump power
                characterBody.baseMaxHealth = 2100; //Base health, basically the health you have when you start a new run
                characterBody.baseMaxShield = 0; //Base shield, basically the same as baseMaxHealth but with shields
                characterBody.baseMoveSpeed = 0; //Base move speed, this is usual 7
                characterBody.baseNameToken = "GRANDPARENT_BODY_NAME"; //The base name token. 
                characterBody.subtitleNameToken = "GRANDPARENT_BODY_SUBTITLE"; //Set this to true if its a boss
                characterBody.baseRegen = 0; //Base health regen.
                characterBody.bodyFlags = (CharacterBody.BodyFlags.None); ///Base body flags, should be self explanatory 
                characterBody.crosshairPrefab = characterBody.crosshairPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/HuntressBody").GetComponent<CharacterBody>().crosshairPrefab; //The crosshair prefab.
                characterBody.hideCrosshair = false; //Whether or not to hide the crosshair
                characterBody.hullClassification = HullClassification.BeetleQueen; //The hull classification, usually used for AI
                characterBody.isChampion = true; //Set this to true if its A. A boss or B. A miniboss
                characterBody.levelArmor = 0; //Armor gained when leveling up.  
                characterBody.levelAttackSpeed = 0; //Attackspeed gained when leveling up. 
                characterBody.levelCrit = 0; //Crit chance gained when leveling up. 
                characterBody.levelDamage = 8f; //Damage gained when leveling up. 
                characterBody.levelArmor = 0; //Armor gained when leveling up. 
                characterBody.levelJumpPower = 0; //Jump power gained when leveling up. 
                characterBody.levelMaxHealth = 650; //Health gained when leveling up. 
                characterBody.levelMaxShield = 0; //Shield gained when leveling up. 
                characterBody.levelMoveSpeed = 0; //Move speed gained when leveling up. 
                characterBody.levelRegen = 0f; //Regen gained when leveling up. 
                                               //characterBody.portraitIcon = portrait; //The portrait icon, shows up in multiplayer and the death UI
                                               //characterBody.preferredPodPrefab = Resources.Load<GameObject>("prefabs/networkedobjects/robocratepod"); //The pod prefab this survivor spawns in. Options: Resources.Load<GameObject>("prefabs/networkedobjects/robocratepod"); Resources.Load<GameObject>("prefabs/networkedobjects/survivorpod"); 
            }
        }

        private static void RebuildSkillDrivers()
        {
            BaseHelpers.DestroySkillDrivers(grandParentMaster);

            AISkillDriver GroundSwipe = grandParentMaster.AddComponent<AISkillDriver>();
            AISkillDriver SpiritPull = grandParentMaster.AddComponent<AISkillDriver>();
            AISkillDriver Offspring = grandParentMaster.AddComponent<AISkillDriver>();
            AISkillDriver PortalJump = grandParentMaster.AddComponent<AISkillDriver>();
            AISkillDriver Path = grandParentMaster.AddComponent<AISkillDriver>();

            GroundSwipe.customName = "Ground Swipe";
            GroundSwipe.skillSlot = SkillSlot.Primary;
            //GroundSwipe.requiredSkill =
            GroundSwipe.requireSkillReady = true;
            GroundSwipe.requireEquipmentReady = false;
            GroundSwipe.moveTargetType = TargetType.CurrentEnemy;
            GroundSwipe.minUserHealthFraction = float.NegativeInfinity;
            GroundSwipe.maxUserHealthFraction = float.PositiveInfinity;
            GroundSwipe.minTargetHealthFraction = float.NegativeInfinity;
            GroundSwipe.maxTargetHealthFraction = float.PositiveInfinity;
            GroundSwipe.minDistance = 0;
            GroundSwipe.maxDistance = 15;
            GroundSwipe.selectionRequiresTargetLoS = false;
            GroundSwipe.activationRequiresTargetLoS = true;
            GroundSwipe.activationRequiresAimConfirmation = true;
            GroundSwipe.movementType = MovementType.ChaseMoveTarget;
            GroundSwipe.moveInputScale = 1;
            GroundSwipe.aimType = AimType.AtMoveTarget;
            GroundSwipe.ignoreNodeGraph = true;
            GroundSwipe.driverUpdateTimerOverride = -1;
            GroundSwipe.resetCurrentEnemyOnNextDriverSelection = false;
            GroundSwipe.noRepeat = false;
            GroundSwipe.shouldSprint = false;
            GroundSwipe.shouldFireEquipment = false;
            GroundSwipe.shouldTapButton = false;


            SpiritPull.customName = "Spirit Pull";
            SpiritPull.skillSlot = SkillSlot.Secondary;
            //SpiritPull.requiredSkill =
            SpiritPull.requireSkillReady = true;
            SpiritPull.requireEquipmentReady = false;
            SpiritPull.moveTargetType = TargetType.CurrentEnemy;
            SpiritPull.minUserHealthFraction = float.NegativeInfinity;
            SpiritPull.maxUserHealthFraction = float.PositiveInfinity;
            SpiritPull.minTargetHealthFraction = float.NegativeInfinity;
            SpiritPull.maxTargetHealthFraction = float.PositiveInfinity;
            SpiritPull.minDistance = 15;
            SpiritPull.maxDistance = 200;
            SpiritPull.selectionRequiresTargetLoS = false;
            SpiritPull.activationRequiresTargetLoS = false;
            SpiritPull.activationRequiresAimConfirmation = false;
            SpiritPull.movementType = MovementType.ChaseMoveTarget;
            SpiritPull.moveInputScale = 1;
            SpiritPull.aimType = AimType.AtMoveTarget;
            SpiritPull.ignoreNodeGraph = true;
            SpiritPull.driverUpdateTimerOverride = -1;
            SpiritPull.resetCurrentEnemyOnNextDriverSelection = false;
            SpiritPull.noRepeat = false;
            SpiritPull.shouldSprint = false;
            SpiritPull.shouldFireEquipment = false;
            SpiritPull.shouldTapButton = false;

            Offspring.customName = "Off Spring";
            Offspring.skillSlot = SkillSlot.Utility;
            //Offspring.requiredSkill = 
            Offspring.requireSkillReady = true;
            Offspring.requireEquipmentReady = false;
            Offspring.moveTargetType = TargetType.CurrentEnemy;
            Offspring.minUserHealthFraction = float.NegativeInfinity;
            Offspring.maxUserHealthFraction = float.PositiveInfinity;
            Offspring.minTargetHealthFraction = float.NegativeInfinity;
            Offspring.maxTargetHealthFraction = float.PositiveInfinity;
            Offspring.minDistance = 15;
            Offspring.maxDistance = float.PositiveInfinity;
            Offspring.selectionRequiresTargetLoS = false;
            Offspring.activationRequiresTargetLoS = false;
            Offspring.activationRequiresAimConfirmation = false;
            Offspring.movementType = MovementType.ChaseMoveTarget;
            Offspring.moveInputScale = 1;
            Offspring.aimType = AimType.AtMoveTarget;
            Offspring.ignoreNodeGraph = true;
            Offspring.driverUpdateTimerOverride = -1;
            Offspring.resetCurrentEnemyOnNextDriverSelection = false;
            Offspring.noRepeat = false;
            Offspring.shouldSprint = false;
            Offspring.shouldFireEquipment = false;
            Offspring.shouldTapButton = false;


            PortalJump.customName = "Portal Jump";
            PortalJump.skillSlot = SkillSlot.Special;
            //PortalJump. requiredSkill =
            PortalJump.requireSkillReady = true;
            PortalJump.requireEquipmentReady = false;
            PortalJump.moveTargetType = TargetType.CurrentEnemy;
            PortalJump.minUserHealthFraction = float.NegativeInfinity;
            PortalJump.maxUserHealthFraction = float.PositiveInfinity;
            PortalJump.minTargetHealthFraction = float.NegativeInfinity;
            PortalJump.maxTargetHealthFraction = float.PositiveInfinity;
            PortalJump.minDistance = 50;
            PortalJump.maxDistance = float.PositiveInfinity;
            PortalJump.selectionRequiresTargetLoS = false;
            PortalJump.activationRequiresTargetLoS = false;
            PortalJump.activationRequiresAimConfirmation = false;
            PortalJump.movementType = MovementType.ChaseMoveTarget;
            PortalJump.moveInputScale = 1;
            PortalJump.aimType = AimType.AtMoveTarget;
            PortalJump.ignoreNodeGraph = true;
            PortalJump.driverUpdateTimerOverride = -1;
            PortalJump.resetCurrentEnemyOnNextDriverSelection = false;
            PortalJump.noRepeat = false;
            PortalJump.shouldSprint = false;
            PortalJump.shouldFireEquipment = false;
            PortalJump.shouldTapButton = false;


            Path.customName = "Path";
            Path.skillSlot = SkillSlot.None;
            //Path.requiredSkill =
            Path.requireSkillReady = false;
            Path.requireEquipmentReady = false;
            Path.moveTargetType = TargetType.CurrentEnemy;
            Path.minUserHealthFraction = float.NegativeInfinity;
            Path.maxUserHealthFraction = float.PositiveInfinity;
            Path.minTargetHealthFraction = float.NegativeInfinity;
            Path.maxTargetHealthFraction = float.PositiveInfinity;
            Path.minDistance = 0;
            Path.maxDistance = float.PositiveInfinity;
            Path.selectionRequiresTargetLoS = false;
            Path.activationRequiresTargetLoS = false;
            Path.activationRequiresAimConfirmation = false;
            Path.movementType = MovementType.ChaseMoveTarget;
            Path.moveInputScale = 1;
            Path.aimType = AimType.AtMoveTarget;
            Path.ignoreNodeGraph = true;
            Path.driverUpdateTimerOverride = -1;
            Path.resetCurrentEnemyOnNextDriverSelection = false;
            Path.noRepeat = false;
            Path.shouldSprint = false;
            Path.shouldFireEquipment = false;
            Path.shouldTapButton = false;
        }
        private static void RebuildSkills()
        {
            BaseHelpers.DestroyGenericSkillComponents(grandParent);
            CreateSkillFamilies();
            CreatePrimary();
            CreateSecondary();
            CreateUtility();
            CreateSpecial();

        }

        private static void CreateSkillFamilies()
        {
            skillLocator.SetFieldValue<GenericSkill[]>("allSkills", new GenericSkill[0]);
            {
                skillLocator.primary = grandParent.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.primary.SetFieldValue("_skillFamily", newFamily);
            }
            {
                skillLocator.secondary = grandParent.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.secondary.SetFieldValue("_skillFamily", newFamily);
            }
            {
                skillLocator.utility = grandParent.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.utility.SetFieldValue("_skillFamily", newFamily);
            }
            {
                skillLocator.special = grandParent.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.special.SetFieldValue("_skillFamily", newFamily);
            }
        }
        private static void CreatePrimary()
        {
            SkillDef primary = ScriptableObject.CreateInstance<SkillDef>();
            primary.activationState = new SerializableEntityStateType(typeof(EntityStates.GrandParentBoss.GroundSwipe)); //You can also "steal" another skill by setting this to another skill, ex: "new SerializableEntityStateType(typeof(EntityStates.Commando.CombatDodge))
            primary.activationStateMachineName = "Weapon"; //Setting this to "Body" overrides your current movement
            primary.baseMaxStock = 1; //How many charges you hold at once
            primary.baseRechargeInterval = 0f; //how long it takes to recharge a stock
            primary.beginSkillCooldownOnSkillEnd = true;
            primary.canceledFromSprinting = false; //if set to true, sprinting will immediately end the skill
            primary.fullRestockOnAssign = true;
            primary.interruptPriority = InterruptPriority.Skill; //The priority of the skill (If performing an important skill, lower priority skills won't activate)
            primary.isBullets = false; //if set to true, when the cooldown ends it will reload ALL stock
            primary.isCombatSkill = true;
            primary.mustKeyPress = false; //If true, you MUST press the key each time for it to activate, otherwise it auto-fires
            primary.noSprint = false; //Whether or not to stop sprinting when activating the attack
            primary.rechargeStock = 1;
            primary.requiredStock = 1;
            primary.shootDelay = 0.1f;
            primary.stockToConsume = 0;
            primary.skillDescriptionToken = "a"; //Description for the skill, you can use <style>'s to add colors
            primary.skillName = "a"; //The game-readable name for the skill
            primary.skillNameToken = "cumsick god"; //The visible name of the skill
            //primary.icon = Sprite.Create(this.HURT, new Rect(0, 0, this.HURT.width, this.HURT.height), new Vector2(.5f, .5f));

            LoadoutAPI.AddSkillDef(primary);
            SkillFamily primarySkillFamily = skillLocator.primary.skillFamily;

            primarySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = primary,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(primary.skillNameToken, false, null)

            };
        }
        private static void CreateSecondary()
        {
            LoadoutAPI.AddSkill(typeof(SpiritLift));
            SkillDef secondarySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            secondarySkillDef.activationState = new SerializableEntityStateType(typeof(SpiritLift));
            secondarySkillDef.activationStateMachineName = "Weapon";
            secondarySkillDef.baseMaxStock = 1;
            secondarySkillDef.baseRechargeInterval = 5f;
            secondarySkillDef.beginSkillCooldownOnSkillEnd = true;
            secondarySkillDef.canceledFromSprinting = false;
            secondarySkillDef.fullRestockOnAssign = true;
            secondarySkillDef.interruptPriority = InterruptPriority.PrioritySkill;
            secondarySkillDef.isBullets = false;
            secondarySkillDef.isCombatSkill = true;
            secondarySkillDef.mustKeyPress = false;
            secondarySkillDef.noSprint = false;
            secondarySkillDef.rechargeStock = 1;
            secondarySkillDef.requiredStock = 1;
            secondarySkillDef.shootDelay = 0.5f;
            secondarySkillDef.stockToConsume = 1;
            secondarySkillDef.skillDescriptionToken = "AAAAAAAAAAAAAAAAAAAAAA";
            secondarySkillDef.skillName = "aaa";
            secondarySkillDef.skillNameToken = "aa";

            LoadoutAPI.AddSkillDef(secondarySkillDef);
            SkillFamily secondarySkillFamily = skillLocator.secondary.skillFamily;

            secondarySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = secondarySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(secondarySkillDef.skillNameToken, false, null)

            };
        }
        private static void CreateUtility()
        {               
            LoadoutAPI.AddSkill(typeof(Weapon.GrandParentBoss.PortalJump));
            SkillDef utilitySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            utilitySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.GrandParentBoss.Offspring));
            utilitySkillDef.activationStateMachineName = "Weapon";
            utilitySkillDef.baseMaxStock = 1;
            utilitySkillDef.baseRechargeInterval = 3f;
            utilitySkillDef.beginSkillCooldownOnSkillEnd = true;
            utilitySkillDef.canceledFromSprinting = false;
            utilitySkillDef.fullRestockOnAssign = true;
            utilitySkillDef.interruptPriority = InterruptPriority.PrioritySkill;
            utilitySkillDef.isBullets = false;
            utilitySkillDef.isCombatSkill = false;
            utilitySkillDef.mustKeyPress = false;
            utilitySkillDef.noSprint = false;
            utilitySkillDef.rechargeStock = 1;
            utilitySkillDef.requiredStock = 1;
            utilitySkillDef.shootDelay = 0.5f;
            utilitySkillDef.stockToConsume = 1;
            utilitySkillDef.skillDescriptionToken = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
            utilitySkillDef.skillName = "AAAAAAAAAAAAAA";
            utilitySkillDef.skillNameToken = "AAAAAAAAAAAA";
            //utilitySkillDef.icon = Sprite.Create(this.OVERCLOCK, new Rect(0, 0, this.OVERCLOCK.width, this.OVERCLOCK.height), new Vector2(.5f, .5f));

            LoadoutAPI.AddSkillDef(utilitySkillDef);
            SkillFamily utilitySkillFamily = skillLocator.utility.skillFamily;

            utilitySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = utilitySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(utilitySkillDef.skillNameToken, false, null)

            };
        }
        private static void CreateSpecial()
        {
            LoadoutAPI.AddSkill(typeof(Weapon.GrandParentBoss.PortalJump));
            SkillDef specialSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            specialSkillDef.activationState = new SerializableEntityStateType(typeof(Weapon.GrandParentBoss.PortalJump));
            specialSkillDef.activationStateMachineName = "Weapon";
            specialSkillDef.baseMaxStock = 1;
            specialSkillDef.baseRechargeInterval = 12f;
            specialSkillDef.beginSkillCooldownOnSkillEnd = false;
            specialSkillDef.canceledFromSprinting = false;
            specialSkillDef.fullRestockOnAssign = false;
            specialSkillDef.interruptPriority = InterruptPriority.Any;
            specialSkillDef.isBullets = false;
            specialSkillDef.isCombatSkill = true;
            specialSkillDef.mustKeyPress = false;
            specialSkillDef.noSprint = false;
            specialSkillDef.rechargeStock = 1;
            specialSkillDef.requiredStock = 1;
            specialSkillDef.shootDelay = 1f;
            specialSkillDef.stockToConsume = 1;
            specialSkillDef.skillDescriptionToken = "AAAAAAAAAAAAAAAAAAAA";
            specialSkillDef.skillName = "AAAAAAAAAAAAA";
            specialSkillDef.skillNameToken = "AAAAAAAAAAAAAA";

            LoadoutAPI.AddSkillDef(specialSkillDef);
            SkillFamily specialSkillFamily = skillLocator.special.skillFamily;


            specialSkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = specialSkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(specialSkillDef.skillNameToken, false, null)

            };
        }
        private static void ModifyParentPod()
        {
            BuildPodBody();
            ModifySpawnerPodsController();
        }
        private static void ModifySpawnerPodsController()
        {
            SpawnerPodsController podsController = parentPod.GetComponent<SpawnerPodsController>();
            if (podsController)
            {
                podsController.incubationDuration = 6;
            }
         }
        private static void BuildPodBody()
        {
            CharacterBody characterBody = parentPod.GetComponent<CharacterBody>();
            if (characterBody)
            {
                characterBody.baseAcceleration = 14f;
                characterBody.baseArmor = 20; //Base armor this character has, set to 20 if this character is melee 
                characterBody.baseAttackSpeed = 1; //Base attack speed, usually 1
                characterBody.baseCrit = 0;  //Base crit, usually one
                characterBody.baseDamage = 0; //Base damage
                characterBody.baseJumpCount = 0; //Base jump amount, set to 2 for a double jump. 
                characterBody.baseJumpPower = 0; //Base jump power
                characterBody.baseMaxHealth = 300; //Base health, basically the health you have when you start a new run
                characterBody.baseMaxShield = 0; //Base shield, basically the same as baseMaxHealth but with shields
                characterBody.baseMoveSpeed = 0; //Base move speed, this is usual 7
                characterBody.baseNameToken = "GRANDPA, RENT_BODY_NAME"; //The base name token. 
                characterBody.subtitleNameToken = "GRANDPARENT_BODY_SUBTITLE"; //Set this to true if its a boss
                characterBody.baseRegen = 0; //Base health regen.
                characterBody.bodyFlags = (CharacterBody.BodyFlags.None); ///Base body flags, should be self explanatory 
                characterBody.crosshairPrefab = characterBody.crosshairPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/HuntressBody").GetComponent<CharacterBody>().crosshairPrefab; //The crosshair prefab.
                characterBody.hideCrosshair = false; //Whether or not to hide the crosshair
                characterBody.hullClassification = HullClassification.Human; //The hull classification, usually used for AI
                characterBody.isChampion = true; //Set this to true if its A. A boss or B. A miniboss
                characterBody.levelArmor = 0; //Armor gained when leveling up.  
                characterBody.levelAttackSpeed = 0; //Attackspeed gained when leveling up. 
                characterBody.levelCrit = 0; //Crit chance gained when leveling up. 
                characterBody.levelDamage = 0; //Damage gained when leveling up. 
                characterBody.levelArmor = 0; //Armor gained when leveling up. 
                characterBody.levelJumpPower = 0; //Jump power gained when leveling up. 
                characterBody.levelMaxHealth = 100; //Health gained when leveling up. 
                characterBody.levelMaxShield = 0; //Shield gained when leveling up. 
                characterBody.levelMoveSpeed = 0; //Move speed gained when leveling up. 
                characterBody.levelRegen = 0f; //Regen gained when leveling up. 
                                               //characterBody.portraitIcon = portrait; //The portrait icon, shows up in multiplayer and the death UI
                                               //characterBody.preferredPodPrefab = Resources.Load<GameObject>("prefabs/networkedobjects/robocratepod"); //The pod prefab this survivor spawns in. Options: Resources.Load<GameObject>("prefabs/networkedobjects/robocratepod"); Resources.Load<GameObject>("prefabs/networkedobjects/survivorpod"); 
            }

        }
    }
}

