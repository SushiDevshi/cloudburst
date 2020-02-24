using BepInEx;
using CloudBurst.Equipment;
using CloudBurst.Items;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

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
    [BepInPlugin("com.SushiDev.CloudBurst", "CloudBurst", "1.0.0")]


    public class CloudBurstPlugin : BaseUnityPlugin
    {
        #region Readonly items and equipment
        private readonly HereticBox box;
        private readonly BrokenPrinter summonbox;
        private readonly Grinder grinder;
        private readonly Lumpkin lumpkin;
        private readonly Nokia nokia;
        private readonly MechanicalTrinket trinket;
        #endregion

        public static List<ItemIndex> bossitemList = new List<ItemIndex>{
            ItemIndex.NovaOnLowHealth,
            ItemIndex.Knurl,
            ItemIndex.BeetleGland,
            ItemIndex.TitanGoldDuringTP,
            ItemIndex.SprintWisp,
            //Excluding pearls because those aren't boss items, they come from the Cleansing Pool 
        };

        #region Actual items and equipment 
        public CloudBurstPlugin()
        {
            box = new HereticBox();
            summonbox = new BrokenPrinter();
            grinder = new Grinder();
            lumpkin = new Lumpkin();
            nokia = new Nokia();
            trinket = new MechanicalTrinket();

        }
        #endregion
        public void NokiaCall(float offSet, Transform transform)
        {
            var dropList = Run.instance.availableTier1DropList;

            var nextItem = Run.instance.treasureRng.RangeInt(0, dropList.Count);

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
        //public static GameObject BanditDroneProjectile { get; private set; }
        //public static GameObject MolotovCocktail { get; private set; }
        //public static GameObject TheBandit { get; private set; }
        #endregion

        public void Awake()
        {
            #region Prefabs
            GameObject LoaderPylon = Resources.Load<GameObject>("prefabs/projectiles/loaderpylon");
            GameObject PlasmaBolt = Resources.Load<GameObject>("prefabs/projectiles/magelightningboltbasic");
            GameObject ArchWisp = Resources.Load<GameObject>("prefabs/characterbodies/archwispbody");
            GameObject Engineer = Resources.Load<GameObject>("prefabs/characterbodies/engibody");

            OverclockedPylonProjectile = Resources.Load<GameObject>("prefabs/projectiles/loaderpylon").InstantiateClone("OverclockedPylonProjectile", false);
            EngiMADProjectile = Resources.Load<GameObject>("prefabs/projectiles/commandogrenadeprojectile").InstantiateClone("EngiMADProjectile", false);
            //BanditDroneProjectile = Resources.Load<GameObject>("prefabs/projectiles/commandogrenadeprojectile").InstantiateClone("BanditDroneProjectile", false);
            //MolotovCocktail = Resources.Load<GameObject>("prefabs/projectiles/commandogrenadeprojectile").InstantiateClone("MolotovCocktail", false);
            //TheBandit = Resources.Load<GameObject>("prefabs/characterbodies/BanditBody").InstantiateClone("TheBandit", true);
            #endregion

            #region Registering various game objects to catalogs    
            MiscHelpers.RegisterNewProjectile(EngiMADProjectile);
            MiscHelpers.RegisterNewProjectile(OverclockedPylonProjectile);
            /*MiscHelpers.RegisterNewProjectile(BanditDroneProjectile);
            MiscHelpers.RegisterNewProjectile(MolotovCocktail);
            MiscHelpers.RegisterNewBody(TheBandit);*/
            #endregion

            #region Fetching Components
            //ProjectileController components 
            ProjectileController LoaderPylon_PC = LoaderPylon.GetComponent<ProjectileController>();
            ProjectileController PlasmaBolt_PC = PlasmaBolt.GetComponent<ProjectileController>();
            ProjectileController EngiMADProjectile_PC = EngiMADProjectile.GetComponent<ProjectileController>();

            //ProjectileImpactExplosion components
            ProjectileImpactExplosion EngiMADProjectile_PIE = GetComponent<ProjectileImpactExplosion>();

            //ProjectileSimple components
            ProjectileDamage EngiMADProjectile_PD = GetComponent<ProjectileDamage>();

            //ProjectileSimple components
            ProjectileSimple PlasmaBolt_PS = PlasmaBolt.GetComponent<ProjectileSimple>();

            //ProjectileProximityBeamController components
            ProjectileProximityBeamController PlasmaBolt_PPBC = PlasmaBolt.AddComponent<ProjectileProximityBeamController>();
            ProjectileProximityBeamController LoaderPylon_PPBC = LoaderPylon.GetComponent<ProjectileProximityBeamController>();

            //CharacterBody components
            CharacterBody ArchWisp_CB = ArchWisp.GetComponent<CharacterBody>();

            //CharacterSpawnCard components
            CharacterSpawnCard ArchWisp_CSC = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscarchwisp");
            #endregion

            #region Registering skills to the skill catalog
            LoadoutAPI.AddSkill(typeof(EntityStates.GreaterWispMonster.ArchWispSummon));
            LoadoutAPI.AddSkill(typeof(EntityStates.ArchWispMonster.ChargeSummon));
            LoadoutAPI.AddSkill(typeof(Weapon.MANDA));
            LoadoutAPI.AddSkill(typeof(Weapon.Shield));
            LoadoutAPI.AddSkill(typeof(Weapon.Shotgun));
            #endregion

            #region Tokens
            R2API.AssetPlus.Languages.AddToken("ARCHWISP_BODY_NAME", "Archaic Wisp");
            R2API.AssetPlus.Languages.AddToken("ENGINEER_SECONDARY_MAD_NAME", "Maintain and Defend");
            R2API.AssetPlus.Languages.AddToken("ENGINEER_SECONDARY_MAD_DESCRIPTION", "Fire a grenade that <style=cIsDamage>stuns</style> and <style=cIsUtility>spreads enemies in all directions</style>.");
            R2API.AssetPlus.Languages.AddToken("ENGINEER_PRIMARY_TR12GS_NAME", "TR12 Gauss Shotgun");
            R2API.AssetPlus.Languages.AddToken("ENGINEER_PRIMARY_TR12GS_DESCRIPTION", "bro its a shotgun bro bro");
            R2API.AssetPlus.Languages.AddToken("ENGINEER_UTILITY_MS_NAME", "Mobile Shield");
            R2API.AssetPlus.Languages.AddToken("ENGINEER_UTILITY_MS_DESCRIPTION", "Release an impenetrable shield that <style=cIsUtility>enemies cannot enter</style>.");
            #endregion

            #region Archwisps
            ArchWisp_CB.baseNameToken = "ARCHWISP_BODY_NAME";

            DirectorAPI.DirectorCardHolder ArchWisp_DCH = new DirectorAPI.DirectorCardHolder();
            DirectorCard ArchWisp_DC = new DirectorCard();

            ArchWisp_CSC.directorCreditCost = 350;
            ArchWisp_CSC.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Air;
            ArchWisp_CSC.noElites = false;

            ArchWisp_DC.allowAmbushSpawn = true;
            ArchWisp_DC.forbiddenUnlockable = "";
            ArchWisp_DC.minimumStageCompletions = 5;
            ArchWisp_DC.preventOverhead = false;
            ArchWisp_DC.requiredUnlockable = "";
            ArchWisp_DC.selectionWeight = 1;
            ArchWisp_DC.spawnCard = ArchWisp_CSC;
            ArchWisp_DC.spawnDistance = DirectorCore.MonsterSpawnDistance.Standard;

            ArchWisp_DCH.Card = ArchWisp_DC;
            ArchWisp_DCH.InteractableCategory = 0;
            ArchWisp_DCH.MonsterCategory = DirectorAPI.MonsterCategory.Minibosses;

            DirectorAPI.MonsterActions += delegate (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage)
            {
                if (!list.Contains(ArchWisp_DCH))
                {
                    list.Add(ArchWisp_DCH);
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
            ArchWisp_SL_P.activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.ArchWispMonster.ChargeSummon));
            #endregion

            #region Hooks
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlotOnPerformEquipmentAction;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManagerOnOnCharacterDeath;

            //On.RoR2.GlobalEventManager.OnTeamLevelUp += GlobalEventManagerOnTeamLevelUp;
            //On.RoR2.GenericPickupController.GrantItem += GrantItem;
            //On.RoR2.GlobalEventManager.OnHitEnemy += OnHitEnemy;
            #endregion

            #region Engineer loadout skills
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
            #endregion

            #region Plasma bolt modifications
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
            #endregion

            #region Misc vanilla modifications
            GameObject ArtificerIceWallPrefab = Resources.Load<GameObject>("prefabs/projectiles/mageicewallpillarprojectile");
            ArtificerIceWallPrefab.layer = 11;
            #endregion
        }
        #region Equipment Hook
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
            return orig(self, index); //must
        }
        #endregion

        #region Character death hook
        private void GlobalEventManagerOnOnCharacterDeath(DamageReport damageReport)
        {
            if (damageReport.victim.body.isChampion && damageReport.attackerBody && damageReport.attackerBody.isPlayerControlled)
            {
                CharacterMaster attackerMaster = damageReport.attackerMaster;
                Inventory inventory = attackerMaster ? attackerMaster.inventory : null;
                int GrinderCount = inventory.GetItemCount(Grinder.itemIndex);
                if (GrinderCount > 0 && Util.CheckRoll((1f - 1f / Mathf.Pow((float)(GrinderCount + 1), 0.15f)) * 100f, attackerMaster))
                {
                    inventory.GiveItem(GetRandomItem(bossitemList), 1);
                }
            }
        }
    }
}
#endregion