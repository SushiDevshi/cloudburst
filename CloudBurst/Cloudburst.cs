using BepInEx;
using CloudBurst.Equipment;
using CloudBurst.Items;
using CloudBurst.Weapon.ArchWispMonster;
using CloudBurst.Weapon.ArchWispMonster.Weapon;
using CloudBurst.Weapon.GrandParentBoss;
using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using static RoR2.CharacterAI.AISkillDriver;

namespace CloudBurst
{
    [BepInDependency("com.bepis.r2api")]
    [R2APISubmoduleDependency(new string[]
    {
        "EffectAPI",
        "LoadoutAPI",
        "ItemAPI",
        "ItemDropAPI",
        "AssetPlus",
        "ResourcesAPI",
        "DirectorAPI",
        "PrefabAPI",
    })]
    [BepInPlugin("com.TeamCloudburst.CloudBurst", "CloudBurst", "1.0.0")]


    public class Main : BaseUnityPlugin
    {
        public static BuffIndex SolarBuff;

        private readonly HereticBox box;
        private readonly BrokenPrinter summonbox;
        private readonly Grinder grinder;
        private readonly Lumpkin lumpkin;
        private readonly Nokia nokia;
        private readonly Sundial sundial;
        private readonly MechanicalTrinket trinket;
        private readonly Pillow pillow;
        private readonly UnstableQuantumReactor reactor;
        public Main()
        {
            //equips
            box = new HereticBox();
            summonbox = new BrokenPrinter();
            lumpkin = new Lumpkin();
            reactor = new UnstableQuantumReactor();
            //items
            grinder = new Grinder();
            nokia = new Nokia();
            trinket = new MechanicalTrinket();
            sundial = new Sundial();
            pillow = new Pillow();

        }

        public static List<ItemIndex> bossitemList = new List<ItemIndex>{
            ItemIndex.NovaOnLowHealth,
            ItemIndex.Knurl,
            ItemIndex.BeetleGland,
            ItemIndex.TitanGoldDuringTP,
            ItemIndex.SprintWisp,
            //Excluding pearls because those aren't boss items, they come from the Cleansing Pool 
        };
        public void NokiaCall(float offSet, Transform transform)
        {
            var dropList = Run.instance.availableTier1DropList;

            int nextItem = Run.instance.treasureRng.RangeInt(0, dropList.Count);

            PickupDropletController.CreatePickupDroplet(dropList[nextItem], transform.position, transform.forward * (20f + offSet));
        }

        private ItemIndex GetRandomItem(List<ItemIndex> items)
        {
            int itemID = UnityEngine.Random.Range(0, items.Count);
            return items[itemID];
        }

        #region Static game objects
        public static GameObject EngiMADProjectile { get; private set; }
        public static GameObject OverclockedPylonProjectile { get; private set; }
        #endregion  

        public void Awake()
        {
            DoMod();
            //;
            //On.RoR2.GenericPickupController.GrantItem += GrantItem;
            //On.RoR2.GlobalEventManager.OnHitEnemy += OnHitEnemy;
        }
        private void Setup()
        {
            RegisterSkills();
            Tokens();
        }
        #region Setup
        private void RegisterSkills()
        {
            LoadoutAPI.AddSkill(typeof(ArchWispSummoner));
            LoadoutAPI.AddSkill(typeof(ChargeSummon));
            LoadoutAPI.AddSkill(typeof(PortalJump));
            LoadoutAPI.AddSkill(typeof(Weapon.MANDA));
            LoadoutAPI.AddSkill(typeof(Weapon.Shield));
            LoadoutAPI.AddSkill(typeof(Weapon.Shotgun));
        }
        private void Tokens()
        {
            R2API.AssetPlus.Languages.AddToken("ARCHWISP_BODY_NAME", "Archaic Wisp");
            R2API.AssetPlus.Languages.AddToken("ENGINEER_SECONDARY_MAD_NAME", "Maintain and Defend");
            R2API.AssetPlus.Languages.AddToken("ENGINEER_SECONDARY_MAD_DESCRIPTION", "Fire a grenade that <style=cIsDamage>stuns</style> and <style=cIsUtility>spreads enemies in all directions</style>.");
            R2API.AssetPlus.Languages.AddToken("ENGINEER_PRIMARY_TR12GS_NAME", "TR12 Gauss Shotgun");
            R2API.AssetPlus.Languages.AddToken("ENGINEER_PRIMARY_TR12GS_DESCRIPTION", "bro its a shotgun bro bro");
            R2API.AssetPlus.Languages.AddToken("ENGINEER_UTILITY_MS_NAME", "Mobile Shield");
            R2API.AssetPlus.Languages.AddToken("ENGINEER_UTILITY_MS_DESCRIPTION", "Release an impenetrable shield that <style=cIsUtility>enemies cannot enter</style>.");
        }
        #endregion
        private void DoMod()
        {
            Setup();
            AddContent();
            ModifyContent();
            Hook();
            
        }
        private void AddContent()
        {
            EngineerLoadoutSkills();
            CreateEngineerMADProjectile();
            ArchWisps();
            RegisterBuffs();
            RebuildGrandParent();
        }
        #region Add content
        private void RegisterBuffs()
        {
            BuffDef solarbuff = new BuffDef
            {
                buffColor = new Color(235, 182, 92),
                buffIndex = BuffIndex.Count,    
                canStack = false,
                eliteIndex = EliteIndex.None,
                iconPath = "Textures/BuffIcons/texbuffpulverizeicon",
                isDebuff = true,
                name = "Solar"
            };

            Main.SolarBuff = ItemAPI.Add(new CustomBuff(solarbuff.name, solarbuff));

        }
        private void CreateEngineerMADProjectile()
        {
            EngiMADProjectile = Resources.Load<GameObject>("prefabs/projectiles/commandogrenadeprojectile").InstantiateClone("EngiMADProjectile", false);
            ProjectileController EngiMADProjectileProjectileController = EngiMADProjectile.GetComponent<ProjectileController>();
            ProjectileImpactExplosion EngiMADProjectileProjectileImpactExplosion = EngiMADProjectile.GetComponent<ProjectileImpactExplosion>();
            ProjectileDamage EngiMADProjectileProjectileDamage = EngiMADProjectile.GetComponent<ProjectileDamage>();

            EngiMADProjectileProjectileController.ghostPrefab = Resources.Load<GameObject>("prefabs/projectileghosts/engigrenadeghost");
            EngiMADProjectileProjectileDamage.damageType = DamageType.Stun1s;
            EngiMADProjectileProjectileImpactExplosion.blastRadius = 50;
            EngiMADProjectileProjectileImpactExplosion.impactEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/engimineexplosion");

            BaseHelpers.RegisterNewProjectile(EngiMADProjectile);
        }
        private void EngineerLoadoutSkills()
        {
            
            GameObject Engineer = Resources.Load<GameObject>("prefabs/characterbodies/engibody");
            var Engineer_SL = Engineer.GetComponent<SkillLocator>();
            var Engineer_SL_SD_SF = Engineer_SL.secondary.skillFamily;
            var Engineer_SL_SD_SF_VS = Engineer_SL_SD_SF.variants;
            var Engineer_SL_UL_SF = Engineer_SL.utility.skillFamily;
            var Engineer_SL_UL_SF_VS = Engineer_SL_UL_SF.variants;
            var Engineer_SL_PY_SF = Engineer_SL.primary.skillFamily;
            var Engineer_SL_PY_SF_VS = Engineer_SL_PY_SF.variants;

            var MADDef = ScriptableObject.CreateInstance<SkillDef>();
            MADDef.activationState = new EntityStates.SerializableEntityStateType(typeof(Weapon.MANDA));
            MADDef.baseMaxStock = 3;
            MADDef.baseRechargeInterval = 5;
            MADDef.beginSkillCooldownOnSkillEnd = true;
            MADDef.canceledFromSprinting = true;
            MADDef.fullRestockOnAssign = false;
            MADDef.isBullets = false;
            MADDef.isCombatSkill = true;
            MADDef.mustKeyPress = true;
            MADDef.noSprint = true;
            MADDef.rechargeStock = 1;
            MADDef.requiredStock = 1;
            MADDef.shootDelay = 0.1f;
            MADDef.activationStateMachineName = Engineer_SL_SD_SF.defaultSkillDef.activationStateMachineName;
            MADDef.skillDescriptionToken = "ENGINEER_SECONDARY_MAD_DESCRIPTION";
            MADDef.skillNameToken = "ENGINEER_SECONDARY_MAD_NAME";
            MADDef.skillName = "ENGINEER_UTILITY_MS_NAME";
            MADDef.stockToConsume = 1;

            var MSDef = ScriptableObject.CreateInstance<SkillDef>();
            MSDef.activationState = new EntityStates.SerializableEntityStateType(typeof(Weapon.Shield));
            MSDef.baseMaxStock = 1;
            MSDef.baseRechargeInterval = 30;
            MSDef.beginSkillCooldownOnSkillEnd = true;
            MSDef.canceledFromSprinting = false;
            MSDef.fullRestockOnAssign = false;
            MSDef.isBullets = false;
            MSDef.isCombatSkill = true;
            MSDef.mustKeyPress = true;
            MSDef.noSprint = true;
            MSDef.rechargeStock = 1;
            MSDef.activationStateMachineName = Engineer_SL_UL_SF.defaultSkillDef.activationStateMachineName;
            MSDef.requiredStock = 1;
            MSDef.shootDelay = 0.1f;
            MSDef.skillDescriptionToken = "ENGINEER_UTILITY_MS_DESCRIPTION";
            MSDef.skillNameToken = "ENGINEER_UTILITY_MS_NAME";
            MSDef.skillName = "ENGINEER_UTILITY_MS_NAME";
            MSDef.stockToConsume = 1;

            var TR12GSDef = ScriptableObject.CreateInstance<SkillDef>();
            TR12GSDef.activationState = new EntityStates.SerializableEntityStateType(typeof(Weapon.Shotgun));
            TR12GSDef.baseMaxStock = 1;
            TR12GSDef.baseRechargeInterval = 0;
            TR12GSDef.beginSkillCooldownOnSkillEnd = true;
            TR12GSDef.canceledFromSprinting = false;
            TR12GSDef.fullRestockOnAssign = false;
            TR12GSDef.isBullets = false;
            TR12GSDef.isCombatSkill = true;
            TR12GSDef.mustKeyPress = false;
            TR12GSDef.noSprint = false;
            TR12GSDef.rechargeStock = 1;
            TR12GSDef.activationStateMachineName = Engineer_SL_PY_SF.defaultSkillDef.activationStateMachineName;
            TR12GSDef.requiredStock = 1;
            TR12GSDef.shootDelay = 0.5f;
            TR12GSDef.skillDescriptionToken = "ENGINEER_PRIMARY_TR12GS_DESCRIPTION";
            TR12GSDef.skillNameToken = "ENGINEER_PRIMARY_TR12GS_NAME";
            TR12GSDef.skillName = "ENGINEER_PRIMARY_TR12GS_NAME";
            TR12GSDef.stockToConsume = 1;

            SkillFamily.Variant MADvariant = new SkillFamily.Variant
            {
                skillDef = MADDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node("M2", false)
            };

            SkillFamily.Variant MovementShieldvariant = new SkillFamily.Variant
            {
                skillDef = MSDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node("Shift", false)
            };

            SkillFamily.Variant TR12GSvariant = new SkillFamily.Variant
            {
                skillDef = TR12GSDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node("M1", false)
            };

            //I have to do this here because I really don't want to move this block of code upwards
            LoadoutAPI.AddSkillDef(MADDef);
            LoadoutAPI.AddSkillDef(MSDef);
            LoadoutAPI.AddSkillDef(TR12GSDef);

            //Resizing arrays
            Array.Resize(ref Engineer_SL_SD_SF_VS, Engineer_SL_SD_SF_VS.Length + 1);
            Engineer_SL_SD_SF_VS[Engineer_SL_SD_SF_VS.Length - 1] = MADvariant;
            Engineer_SL_SD_SF.variants = Engineer_SL_SD_SF_VS;

            Array.Resize(ref Engineer_SL_UL_SF_VS, Engineer_SL_UL_SF_VS.Length + 1);
            Engineer_SL_UL_SF_VS[Engineer_SL_UL_SF_VS.Length - 1] = MovementShieldvariant;
            Engineer_SL_UL_SF.variants = Engineer_SL_UL_SF_VS;

            Array.Resize(ref Engineer_SL_PY_SF_VS, Engineer_SL_PY_SF_VS.Length + 1);
            Engineer_SL_PY_SF_VS[Engineer_SL_PY_SF_VS.Length - 1] = TR12GSvariant;
            Engineer_SL_PY_SF.variants = Engineer_SL_PY_SF_VS;
        }
        private void ArchWisps()
        {
            GameObject ArchWisp = Resources.Load<GameObject>("prefabs/characterbodies/archwispbody");
            CharacterBody ArchWispBody = ArchWisp.GetComponent<CharacterBody>();
            CharacterSpawnCard ArchWispCharacterSpawnCard = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscarchwisp");
            DirectorAPI.DirectorCardHolder ArchWispDirectorCardHolder = new DirectorAPI.DirectorCardHolder();
            DirectorCard ArchWispDirectorCard = new DirectorCard();

            ArchWispBody.baseNameToken = "ARCHWISP_BODY_NAME";  

            ArchWispCharacterSpawnCard.directorCreditCost = 350;
            ArchWispCharacterSpawnCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Air;
            ArchWispCharacterSpawnCard.noElites = false;

            //ArchWispDirectorCard.
            ArchWispDirectorCard.allowAmbushSpawn = true;
            ArchWispDirectorCard.forbiddenUnlockable = "";
            ArchWispDirectorCard.minimumStageCompletions = 4;
            ArchWispDirectorCard.preventOverhead = false;
            ArchWispDirectorCard.requiredUnlockable = "";
            ArchWispDirectorCard.selectionWeight = 1;
            ArchWispDirectorCard.spawnCard = ArchWispCharacterSpawnCard;
            ArchWispDirectorCard.spawnDistance = DirectorCore.MonsterSpawnDistance.Standard;

            ArchWispDirectorCardHolder.Card = ArchWispDirectorCard;
            ArchWispDirectorCardHolder.InteractableCategory = 0;
            ArchWispDirectorCardHolder.MonsterCategory = DirectorAPI.MonsterCategory.Minibosses;

            DirectorAPI.MonsterActions += delegate (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage)
            {
                if (!list.Contains(ArchWispDirectorCardHolder))
                {
                    list.Add(ArchWispDirectorCardHolder);
                }
            };
            var ArchWisp_SL = ArchWisp.GetComponent<SkillLocator>();
            var ArchWisp_SL_P_SF = ArchWisp_SL.primary.skillFamily;
            var ArchWisp_SL_P = ArchWisp_SL_P_SF.variants[ArchWisp_SL_P_SF.defaultVariantIndex].skillDef;

            ArchWisp_SL_P.rechargeStock = 1;
            ArchWisp_SL_P.shootDelay = .3f;
            ArchWisp_SL_P.stockToConsume = 1;
            ArchWisp_SL_P.baseMaxStock = 3;
            ArchWisp_SL_P.baseRechargeInterval = 5;
            ArchWisp_SL_P.activationState = new EntityStates.SerializableEntityStateType(typeof(ChargeSummon));
        }

        #endregion

        #region Modify content
        private void ModifyContent()
        {
            MakeIceWallSolid();
            ModifyPlasmaBolt();
        }
        private void ModifyPlasmaBolt()
        {
            GameObject LoaderPylon = Resources.Load<GameObject>("prefabs/projectiles/loaderpylon");
            GameObject PlasmaBolt = Resources.Load<GameObject>("prefabs/projectiles/magelightningboltbasic");


            ProjectileController PlasmaBolt_PC = PlasmaBolt.GetComponent<ProjectileController>();
            ProjectileProximityBeamController PlasmaBolt_PPBC = PlasmaBolt.AddComponent<ProjectileProximityBeamController>();
            ProjectileProximityBeamController LoaderPylon_PPBC = LoaderPylon.GetComponent<ProjectileProximityBeamController>();

            PlasmaBolt_PC.ghostPrefab = Resources.Load<GameObject>("prefabs/projectileghosts/electricwormseekerghost");

            PlasmaBolt_PPBC.attackFireCount = LoaderPylon_PPBC.attackFireCount;
            PlasmaBolt_PPBC.attackRange = 10;
            PlasmaBolt_PPBC.bounces = 0;
            PlasmaBolt_PPBC.damageCoefficient = 0.5f;
            PlasmaBolt_PPBC.lightningType = RoR2.Orbs.LightningOrb.LightningType.Ukulele;
            PlasmaBolt_PPBC.listClearInterval = LoaderPylon_PPBC.listClearInterval;
            PlasmaBolt_PPBC.maxAngleFilter = LoaderPylon_PPBC.maxAngleFilter;
            PlasmaBolt_PPBC.minAngleFilter = LoaderPylon_PPBC.minAngleFilter;
            PlasmaBolt_PPBC.procCoefficient = 0.2f;
        }

        private void MakeIceWallSolid()
        {
            GameObject ArtificerIceWallPrefab = Resources.Load<GameObject>("prefabs/projectiles/mageicewallpillarprojectile");
            ArtificerIceWallPrefab.layer = 11;
        }
        #endregion

        #region GrandParent
        private void RebuildGrandParent()
        {
            RebuildGrandParentSkillDrivers();
            RebuildSkills();
        }
        private void RebuildGrandParentSkillDrivers()
        {
            GameObject GrandParentMaster = Resources.Load<GameObject>("prefabs/charactermasters/GrandparentMaster");

            AISkillDriver GroundSwipe = GrandParentMaster.AddComponent<AISkillDriver>();
            AISkillDriver SpiritPull = GrandParentMaster.AddComponent<AISkillDriver>();
            AISkillDriver Offspring = GrandParentMaster.AddComponent<AISkillDriver>();
            AISkillDriver PortalJump = GrandParentMaster.AddComponent<AISkillDriver>();
            AISkillDriver Path = GrandParentMaster.AddComponent<AISkillDriver>();

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
        private void RebuildSkills()
        {
            GameObject GrandParent = Resources.Load<GameObject>("prefabs/characterbodies/GrandParentBody");
            GP_DestroyGenericSkillComponents(GrandParent);
            GP_CreateSkillFamilies(GrandParent.GetComponent<SkillLocator>(), GrandParent);
            GP_CreatePrimary(GrandParent.GetComponent<SkillLocator>());
            GP_CreateSecondary(GrandParent.GetComponent<SkillLocator>());
            GP_CreateUtility(GrandParent.GetComponent<SkillLocator>());
            GP_CreateSpecial(GrandParent.GetComponent<SkillLocator>());

        }
        private void GP_DestroyGenericSkillComponents(GameObject gameObject)
        {
            foreach (GenericSkill skill in gameObject.GetComponentsInChildren<GenericSkill>())
            {
                DestroyImmediate(skill);
            }
        }
        private void GP_CreateSkillFamilies(SkillLocator skillLocator, GameObject myCharacter)
        {
            skillLocator.SetFieldValue<GenericSkill[]>("allSkills", new GenericSkill[0]);
            {
                skillLocator.primary = myCharacter.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.primary.SetFieldValue("_skillFamily", newFamily);
            }
            {
                skillLocator.secondary = myCharacter.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.secondary.SetFieldValue("_skillFamily", newFamily);
            }
            {
                skillLocator.utility = myCharacter.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.utility.SetFieldValue("_skillFamily", newFamily);
            }
            {
                skillLocator.special = myCharacter.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.special.SetFieldValue("_skillFamily", newFamily);
            }
        }
        private void GP_CreatePrimary(SkillLocator skillLocator)
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
        private void GP_CreateSecondary(SkillLocator skillLocator)
        {
            SkillDef secondarySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            secondarySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.GrandParentBoss.SpiritPull));
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

        private void GP_CreateUtility(SkillLocator skillLocator)
        {
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
        private void GP_CreateSpecial(SkillLocator skillLocator)
        {
            SkillDef specialSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            specialSkillDef.activationState = new SerializableEntityStateType(typeof(PortalJump));
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
        #endregion

        #region Mega mushroom
        private void CreateMegaMushroom()
        {
            RebuildGrandParentSkillDrivers();
            RebuildSkills();
        }
        private void MM_RebuildSkillDrivers()
        {
            GameObject MiniMushroomMaster = Resources.Load<GameObject>("prefabs/charactermasters/MiniMushroomMaster");

            AISkillDriver SporeGrenade = MiniMushroomMaster.AddComponent<AISkillDriver>();
            AISkillDriver SpiritPull = MiniMushroomMaster.AddComponent<AISkillDriver>();
            AISkillDriver Offspring = MiniMushroomMaster.AddComponent<AISkillDriver>();

            SporeGrenade.customName = "Spore Grenade";
            SporeGrenade.skillSlot = SkillSlot.Primary;
            //GroundSwipe.requiredSkill =
            SporeGrenade.requireSkillReady = true;
            SporeGrenade.requireEquipmentReady = false;
            SporeGrenade.moveTargetType = TargetType.CurrentEnemy;
            SporeGrenade.minUserHealthFraction = 0.5f;
            SporeGrenade.maxUserHealthFraction = float.PositiveInfinity;
            SporeGrenade.minTargetHealthFraction = float.NegativeInfinity;
            SporeGrenade.maxTargetHealthFraction = float.PositiveInfinity;
            SporeGrenade.minDistance = 0;
            SporeGrenade.maxDistance = 60;
            SporeGrenade.selectionRequiresTargetLoS = false;
            SporeGrenade.activationRequiresTargetLoS = true;
            SporeGrenade.activationRequiresAimConfirmation = true;
            SporeGrenade.movementType = MovementType.StrafeMovetarget;
            SporeGrenade.moveInputScale = 1;
            SporeGrenade.aimType = AimType.AtMoveTarget;
            SporeGrenade.ignoreNodeGraph = false;
            SporeGrenade.driverUpdateTimerOverride = -1;
            SporeGrenade.resetCurrentEnemyOnNextDriverSelection = false;
            SporeGrenade.noRepeat = false;
            SporeGrenade.shouldSprint = false;
            SporeGrenade.shouldFireEquipment = false;
            SporeGrenade.shouldTapButton = false;

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

        }
        private void MM_RebuildSkills()
        {
            GameObject GrandParent = Resources.Load<GameObject>("prefabs/characterbodies/GrandParentBody");
            MM_DestroyGenericSkillComponents(GrandParent);
            MM_CreateSkillFamilies(GrandParent.GetComponent<SkillLocator>(), GrandParent);
            MM_CreatePrimary(GrandParent.GetComponent<SkillLocator>());
            MM_CreateSecondary(GrandParent.GetComponent<SkillLocator>());
            MM_CreateUtility(GrandParent.GetComponent<SkillLocator>());
            MM_CreateSpecial(GrandParent.GetComponent<SkillLocator>());

        }
        private void MM_DestroyGenericSkillComponents(GameObject gameObject)
        {
            foreach (GenericSkill skill in gameObject.GetComponentsInChildren<GenericSkill>())
            {
                DestroyImmediate(skill);
            }
        }
        private void MM_CreateSkillFamilies(SkillLocator skillLocator, GameObject myCharacter)
        {
            skillLocator.SetFieldValue<GenericSkill[]>("allSkills", new GenericSkill[0]);
            {
                skillLocator.primary = myCharacter.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.primary.SetFieldValue("_skillFamily", newFamily);
            }
            {
                skillLocator.secondary = myCharacter.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.secondary.SetFieldValue("_skillFamily", newFamily);
            }
            {
                skillLocator.utility = myCharacter.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.utility.SetFieldValue("_skillFamily", newFamily);
            }
            {
                skillLocator.special = myCharacter.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.special.SetFieldValue("_skillFamily", newFamily);
            }
        }
        private void MM_CreatePrimary(SkillLocator skillLocator)
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
        private void MM_CreateSecondary(SkillLocator skillLocator)
        {
            SkillDef secondarySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            secondarySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.GrandParentBoss.SpiritPull));
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

        private void MM_CreateUtility(SkillLocator skillLocator)
        {
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
        private void MM_CreateSpecial(SkillLocator skillLocator)
        {
            SkillDef specialSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            specialSkillDef.activationState = new SerializableEntityStateType(typeof(PortalJump));
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
        #endregion

        #region Hooks
        private void Hook()
        {
            RoR2.TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteractionOnTeleporterBeginChargingGlobal;
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlotOnPerformEquipmentAction;
            //custom buffs
            On.RoR2.CharacterBody.RecalculateStats += RecalculateStats;
            On.RoR2.CharacterBody.RemoveBuff += RemoveBuff;
            //items
            On.RoR2.GenericPickupController.GrantItem += GrantItem;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManagerOnOnCharacterDeath;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBodyOnInventoryChanged;
            On.RoR2.GlobalEventManager.OnTeamLevelUp += GlobalEventManager_OnTeamLevelUp;
        }

        private void GlobalEventManager_OnTeamLevelUp(On.RoR2.GlobalEventManager.orig_OnTeamLevelUp orig, TeamIndex teamIndex)
        {
            int connectedPlayers = PlayerCharacterMasterController.instances.Count;
            for (int i = 0; i < connectedPlayers; i++)
            {
                CharacterMaster characterMaster = PlayerCharacterMasterController.instances[i].master;
                int nokiaCount = characterMaster.inventory.GetItemCount(Nokia.itemIndex);
                if (characterMaster.hasBody && nokiaCount > 0)
                {
                    NokiaCall(0, characterMaster.bodyPrefab.GetComponent<CharacterBody>().transform);
                }
            }
            orig(teamIndex);
        }
        private bool EquipmentSlotOnPerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentIndex index)
        {
            if (index == BrokenPrinter.EquipIndex)
            {
                summonbox.SummonMjnion(self.characterBody);
                return true;
            }
            if (index == Lumpkin.EquipIndex)
            {
                lumpkin.Scream(self.characterBody);
                return true;
            }
            if (index == UnstableQuantumReactor.EquipIndex)
            {
                reactor.BecomeUnstable(self.characterBody);
                return true;
            }
            return orig(self, index); //must
        }
        private void GlobalEventManagerOnOnCharacterDeath(DamageReport damageReport)
        {
            if (damageReport.victim.body.isChampion && damageReport.attackerBody && damageReport.attackerBody.isPlayerControlled && damageReport != null)
            {
                CharacterMaster attackerMaster = damageReport.attackerMaster;
                Inventory inventory = attackerMaster ? attackerMaster.inventory : null;
                int GrinderCount = inventory.GetItemCount(Grinder.itemIndex);
                if (GrinderCount > 0 && Util.CheckRoll((1f - 1f / Mathf.Pow((float)(GrinderCount + 1), 15f)) * 100f, attackerMaster))
                {
                    inventory.GiveItem(GetRandomItem(bossitemList), 1);
                }
            }
        }

        private void RemoveBuff(On.RoR2.CharacterBody.orig_RemoveBuff orig, CharacterBody self, BuffIndex buffIndex)
        {
            orig(self, buffIndex);
            if (self && self.inventory && buffIndex == Main.SolarBuff)
            {
                self.baseMoveSpeed = self.baseMoveSpeed - 3;
                self.baseRegen = self.baseRegen - 1;
                self.baseCrit = self.baseCrit - 3;
            }
        }
        private void RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (self && self.inventory && self.HasBuff(Main.SolarBuff))
            {
                self.baseMoveSpeed = self.baseMoveSpeed + 3;
                self.baseRegen = self.baseRegen + 1;
                self.baseCrit = self.baseCrit + 3;
            }
        }
        private void GrantItem(On.RoR2.GenericPickupController.orig_GrantItem orig, GenericPickupController self, CharacterBody body, Inventory inventory)
        {
            orig(self, body, inventory);
            if (self && inventory && inventory.GetItemCount(Pillow.itemIndex) > 0)
            {
                body.AddTimedBuff(BuffIndex.CloakSpeed, 5 * inventory.GetItemCount(Pillow.itemIndex));
            }
        }
        private void CharacterBodyOnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            int sundialAmount = self.inventory.GetItemCount(Sundial.itemIndex);
            if (0 < sundialAmount)
            {
                SolarGranter Sun =  self.AddComponent<SolarGranter>();
                Sun.referenceTransform = self.transform;
                Sun.buffIndex = Main.SolarBuff;
                Sun.raycastFrequency = 0.5f;
                
            }   
        }
        private void TeleporterInteractionOnTeleporterBeginChargingGlobal(TeleporterInteraction obj)
        {
            ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
            int count = 0;
            foreach (TeamComponent tc in teamMembers)
            {
                if (Util.LookUpBodyNetworkUser(tc.body))
                {
                    count += tc.body.inventory.GetItemCount(MechanicalTrinket.itemIndex);
                }
            }
            if (count > 0)
            {
                foreach (TeamComponent tc in teamMembers)
                {
                    obj.holdoutZoneController.radiusIndicator.transform.localScale = new Vector3(obj.holdoutZoneController.baseRadius * 2f, obj.holdoutZoneController.baseRadius * 2f, obj.holdoutZoneController.baseRadius * 2f);
                    obj.holdoutZoneController.baseRadius = obj.holdoutZoneController.baseRadius * 1.5f  * count;
                }
            }
        }
    }
}
#endregion