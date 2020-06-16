using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using CloudBurst.Equipment;
using CloudBurst.Items;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

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
        "PrefabAPI",
        "LanguageAPI",
        "BuffAPI"
    })]
    [BepInPlugin("com.AwokeinanEngima.CloudBurst", "CloudBurst", "1.0.0")]


    public class Main : BaseUnityPlugin
    {
        private readonly Item grinder;
        private readonly Nokia nokia;
        private readonly Root root;
        private readonly MechanicalTrinket trinket;
        private readonly Pillow pillow;
        private readonly Sundial sundial;
        private readonly SCP scp;

        private readonly BrokenPrinter summonbox;
        private readonly UnstableQuantumReactor reactor;

        public static List<ItemIndex> bossitemList = new List<ItemIndex>{
            ItemIndex.NovaOnLowHealth,
            ItemIndex.Knurl,
            ItemIndex.BeetleGland,
            ItemIndex.TitanGoldDuringTP,
            ItemIndex.SprintWisp,
            //Excluding pearls because those aren't boss items, they come from the Cleansing Pool 
        };

        public static List<BuffIndex> scpBuffList = new List<BuffIndex>{
            BuffIndex.PulverizeBuildup,
            BuffIndex.Poisoned,
            BuffIndex.Nullified,
            BuffIndex.HealingDisabled,
            BuffIndex.Blight,
            BuffIndex.OnFire
        };
        public void NokiaCall(float offSet, Transform transform, int itemCount)
        {
            var dropList = Util.CheckRoll((5 * itemCount)) ? Run.instance.availableTier1DropList : Run.instance.availableTier2DropList;

            int nextItem = Run.instance.treasureRng.RangeInt(0, dropList.Count);

            PickupDropletController.CreatePickupDroplet(dropList[nextItem], transform.position, transform.forward * (20f + offSet));
        }

        private ItemIndex GetRandomItem(List<ItemIndex> items)
        {
            int itemID = UnityEngine.Random.Range(0, items.Count);
            return items[itemID];
        }
        public static List<PluginInfo> loadedPlugins;
        public Main()
        {
            //equips
            summonbox = new BrokenPrinter();
            reactor = new UnstableQuantumReactor();
            //items
            grinder = new Item();
            nokia = new Nokia();
            root = new Root();
            trinket = new MechanicalTrinket();
            pillow = new Pillow();
            sundial = new Sundial();
            scp = new SCP();
        }

        private List<string> replacedFuncs;
        public static ManualLogSource logger;

        private bool templarMod;
        private bool archaicWispMod;
        private bool deathMarkMod;
        private bool solidIceMod;
        public void Awake()
        {
            if (logger == null)
            {
                logger = this.Logger;
            }

            loadedPlugins = new List<PluginInfo>();
            foreach (PluginInfo plugin in Chainloader.PluginInfos.Values)
            {
                if (plugin != null)
                {
                    loadedPlugins.Add(plugin);
                }                          
            }
            CheckCompat();                                                                                                                                                                                                              
            

            Enemies.Archwisps.BuildArchWisps();
            Enemies.GrandParent.BuildGrandParents();
            Enemies.MegaMushrum.BuildMegaMushrums();
            Enemies.ClayMan.BuildClayMen();

            Hook();                                                                                                                                                                               
            Misc.Modifications.Modify();
            Misc.MiscModifications.Modify();
        }

        private void CheckCompat()
        {
            /*this.replacedFuncs.Add("com.rob.PlayableTemplar");
            this.replacedFuncs.Add("com.ThunderDownUnder.SolidIceWall");
            this.replacedFuncs.Add("com.Borbo.DeathMarkFix");
            this.replacedFuncs.Add("com.Rein.ReinDirectorCardDemoArchWisp"); //rein i BEG YOU please shorten that name PLEASE
            foreach (PluginInfo pluginInfo in loadedPlugins)
            {
                switch ((pluginInfo.Metadata.GUID))
                {
                    case "com.Borbo.DeathMarkFix":
                        this.deathMarkMod = true;
                        break;
                    case "com.ThunderDownUnder.SolidIceWall":
                        this.solidIceMod = true;
                        break;
                    case "com.rob.PlayableTemplar":
                        this.templarMod = true;
                        break;
                    case "com.Rein.ReinDirectorCardDemoArchWisp":
                        this.archaicWispMod = true;
                        break;
                }
                //oh no, i'm checking plugins for incompats, how evil i am!
                if (this.replacedFuncs.Contains(pluginInfo.Metadata.GUID))
                {
                    logger.LogWarning("Cloudburst may have conflicts with the following mod:" + pluginInfo.Metadata.Name + "");
                    //logger.LogWarning("If you do not choose to disable these mod(s), Cloudburst will disable certain features to provide a bug free experenice at the cost of content.");
                      
                
                }
            }*/
        }

        #region Hooks
        private void Hook()       
        {
            //stuff
            RoR2.TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteractionOnTeleporterBeginChargingGlobal;
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlotOnPerformEquipmentAction;
            //custom buffs
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats; ;
            On.RoR2.CharacterBody.RemoveBuff += CharacterBody_RemoveBuff;
            //items
            On.RoR2.GenericPickupController.GrantItem += GrantItem;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManagerOnOnCharacterDeath;
            On.RoR2.GlobalEventManager.OnTeamLevelUp += GlobalEventManager_OnTeamLevelUp;
            //qol
            
            On.RoR2.BuffCatalog.RegisterBuff += BuffCatalog_RegisterBuff;
            On.EntityStates.Commando.DodgeState.OnEnter += DodgeState_OnEnter;
            On.RoR2.PickupPickerController.OnInteractionBegin += PickupPickerController_OnInteractionBegin;
        }
        #region GlobalEventManager
        private void GlobalEventManagerOnOnCharacterDeath(DamageReport damageReport)
        {
            if (!damageReport.attackerBody || !damageReport.victimBody || !damageReport.attacker || !damageReport.victim || !damageReport.attackerMaster || !damageReport.victimMaster || damageReport == null)
                return;
            if (damageReport.victim.body.isChampion && damageReport.attackerBody && damageReport.attackerBody.isPlayerControlled && damageReport != null)
            {
                CharacterMaster attackerMaster = damageReport.attackerMaster;
                Inventory inventory = attackerMaster ? attackerMaster.inventory : null;
                int GrinderCount = inventory.GetItemCount(Item.itemIndex);
                if (GrinderCount > 0 && Util.CheckRoll(15 + (GrinderCount * 5)) && attackerMaster)
                {
                    inventory.GiveItem(GetRandomItem(bossitemList), 1);
                }
            }
        }
        private void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            if (!damageReport.attackerBody || !damageReport.victimBody || !damageReport.attacker || !damageReport.victim || !damageReport.attackerMaster || !damageReport.victimMaster || damageReport == null)
                return;
            if (damageReport.attackerBody && damageReport != null)
            {
                CharacterBody attackerBody = damageReport.attackerBody;
                CharacterBody victimBody = damageReport.victimBody;
                CharacterMaster attackerMaster = damageReport.attackerMaster;
                CharacterMaster victimMaster = damageReport.victimMaster;
                Inventory attackerInventory = attackerMaster ? attackerMaster.inventory : null;
                Inventory victimInventory = victimMaster ? victimMaster.inventory : null;
                int scpRandom = UnityEngine.Random.Range(0, scpBuffList.Count);
                int rootCount = attackerInventory.GetItemCount(Root.itemIndex);
                int scpCount = attackerInventory.GetItemCount(SCP.itemIndex);
                int sundialCount = victimInventory.GetItemCount(Sundial.itemIndex);
                if (rootCount > 0 && Util.CheckRoll((30 + rootCount * 5), attackerMaster) && attackerMaster && damageReport.victimMaster)
                {
                    damageReport.victimBody.AddTimedBuff(BuffIndex.Cripple, 3);
                }
                if (sundialCount > 0 && Util.CheckRoll((45), victimMaster) && damageReport.victimBody && victimMaster)
                {
                    damageReport.victimBody.AddTimedBuff(Sundial.solarBuff, (sundialCount * 3));
                }
                if (scpCount > 0 && damageReport.victimBody && victimMaster)
                {
                    damageReport.victimBody.AddTimedBuff(scpBuffList[scpRandom], (scpCount * 3));
                }
            }
        }
        #endregion
        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            if (self.HasBuff(Sundial.solarBuff))
            {
                self.baseArmor += 20;
            }
            orig(self);
        }

        private void CharacterBody_RemoveBuff(On.RoR2.CharacterBody.orig_RemoveBuff orig, CharacterBody self, BuffIndex buffType)
        {
            if (buffType == Sundial.solarBuff)
            {
                self.baseArmor -= 20;
            }
            orig(self, buffType);
        }

        private void PickupPickerController_OnInteractionBegin(On.RoR2.PickupPickerController.orig_OnInteractionBegin orig, PickupPickerController self, Interactor activator)
        {
            activator.GetComponent<CharacterBody>().AddTimedBuff(BuffIndex.ArmorBoost, 10);
            orig(self, activator);
        }

        private void DodgeState_OnEnter(On.EntityStates.Commando.DodgeState.orig_OnEnter orig, EntityStates.Commando.DodgeState self)
        {
            self.outer.commonComponents.characterBody.isSprinting = true;
            orig(self);
        }

        private void BuffCatalog_RegisterBuff(On.RoR2.BuffCatalog.orig_RegisterBuff orig, BuffIndex buffIndex, BuffDef buffDef)
        {
            //Fixes deathmark
            if (buffIndex == BuffIndex.DeathMark)
            {
                buffDef.canStack = true;
            }
            orig(buffIndex, buffDef);
        }

        private void GlobalEventManager_OnTeamLevelUp(On.RoR2.GlobalEventManager.orig_OnTeamLevelUp orig, TeamIndex teamIndex)
        {
            ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
            for (int i = 0; i < teamMembers.Count; i++)
            {

                TeamComponent teamComponent = teamMembers[i];
                if (teamComponent)
                {
                    CharacterBody characterBody = teamComponent.GetComponent<CharacterBody>();
                    if (characterBody)
                    {
                        CharacterMaster master = characterBody.master;
                        if (master)
                        {
                            int nokiaItemCount = master.inventory.GetItemCount(Nokia.itemIndex);
                            //int ancientItemCount = master.inventory.GetItemCount(BrokenScepter.itemIndex);

                            if (nokiaItemCount > 0 && NetworkServer.active)
                            {
                                NokiaCall(10, characterBody.transform, nokiaItemCount);
                            }

                        }
                    }
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
            if (index == UnstableQuantumReactor.EquipIndex)
            {
                reactor.BecomeUnstable(self.characterBody);
                return true;
            }
            return orig(self, index); //must
        }

        private void GrantItem(On.RoR2.GenericPickupController.orig_GrantItem orig, GenericPickupController self, CharacterBody body, Inventory inventory)
        {
            orig(self, body, inventory);
            if (self && inventory && inventory.GetItemCount(Pillow.itemIndex) > 0)
            {
                body.AddTimedBuff(BuffIndex.CloakSpeed, 5 * inventory.GetItemCount(Pillow.itemIndex));
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

                    obj.holdoutZoneController.baseRadius = obj.holdoutZoneController.baseRadius + (count * 5);
                }
            }
        }
    }
}
#endregion