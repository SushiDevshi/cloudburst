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
        "ItemAPI",
        "ResourcesAPI",
        "PrefabAPI",
        "LanguageAPI",
        "BuffAPI",
        "LoadoutAPI",
        "DirectorAPI",
        "LanguageAPI",
    })]
    [BepInPlugin("com.AwokeinanEngima.CloudBurst", "CloudBurst", "1.0.0")]  


    public class Main : BaseUnityPlugin
    {
        private readonly Item grinder;
        private readonly Nokia nokia;
        private readonly Root root;
        private readonly MechanicalTrinket trinket;
        private readonly BrokenKeycard key;
        private readonly Pillow pillow;
        private readonly Sundial sundial;
        private readonly SCP scp;

        private readonly BrokenPrinter summonbox;
        private readonly UnstableQuantumReactor reactor;
        private readonly Lumpkin lum;

        public static List<ItemIndex> bossitemList = new List<ItemIndex>{
            ItemIndex.NovaOnLowHealth,
            ItemIndex.Knurl,                                              
            ItemIndex.BeetleGland,
            ItemIndex.TitanGoldDuringTP,
            ItemIndex.SprintWisp,
            ItemIndex.Incubator
            //Excluding pearls because those aren't boss items, they come from the Cleansing Pool 
        };

        public static List<BuffIndex> scpBuffList = new List<BuffIndex>{
            BuffIndex.NullifyStack,
            BuffIndex.ClayGoo,
            BuffIndex.BeetleJuice,
            BuffIndex.HealingDisabled
        };
        public static List<BuffIndex> eliteBuffList = new List<BuffIndex>{
            BuffIndex.AffixPoison,
            BuffIndex.AffixRed,
            BuffIndex.AffixHaunted,
            BuffIndex.AffixBlue,
            BuffIndex.AffixWhite
        };
        public void NokiaCall(Transform transform, int itemCount)
        {
            var dropList = Util.CheckRoll((5 * itemCount)) ? Run.instance.availableTier2DropList : Run.instance.availableTier1DropList;
            int nextItem = Run.instance.treasureRng.RangeInt(0, dropList.Count);

            PickupDropletController.CreatePickupDroplet(dropList[nextItem], transform.position, transform.forward * 155);
        }

        private ItemIndex GetRandomItem(List<ItemIndex> items)
        {
            int itemID = UnityEngine.Random.Range(0, items.Count);
            return items[itemID];
        }
        private List<PluginInfo> loadedPlugins;
        public Main()
        {
            //equips
            summonbox = new BrokenPrinter();
            reactor = new UnstableQuantumReactor();
            lum = new Lumpkin();
            //items
            grinder = new Item();
            key = new BrokenKeycard();
            nokia = new Nokia();
            root = new Root();
            trinket = new MechanicalTrinket();
            pillow = new Pillow();
            sundial = new Sundial();
            scp = new SCP();
        }

        private List<string> replacedFuncs;
        public static ManualLogSource logger;

        public static bool templarMod;
        public static bool archaicWispMod;
        public static bool deathMarkMod;
        public static bool solidIceMod;
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


            if (!archaicWispMod)
            {
                Enemies.Archwisps.BuildArchWisps();
            }                                       
            Enemies.GrandParent.BuildGrandParents();
            Enemies.MegaMushrum.BuildMegaMushrums();
            Enemies.ClayMan.BuildClayMen();

            Hook();                                                                                                                                                                               
            Misc.Modifications.Modify();
            Misc.MiscModifications.Modify();
        }

        private void CheckCompat()
        {
            replacedFuncs = new List<string>();
            //this.replacedFuncs.Add("com.rob.PlayableTemplar");
            this.replacedFuncs.Add("com.ThunderDownUnder.SolidIceWall");
            this.replacedFuncs.Add("com.Borbo.DeathMarkFix");
            this.replacedFuncs.Add("com.Rein.ReinDirectorCardDemoArchWisp"); //rein i BEG YOU please shorten that name PLEASE
            foreach (PluginInfo pluginInfo in loadedPlugins)
            {
                switch ((pluginInfo.Metadata.GUID))
                {
                    case "com.Borbo.DeathMarkFix":
                        deathMarkMod = true;
                        break; //done
                    case "com.ThunderDownUnder.SolidIceWall":
                        solidIceMod = true;
                        break; //done
                    case "com.rob.PlayableTemplar":
                        templarMod = true;
                        break; //this is the                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     case i don't get how to handle,
                    case "com.Rein.ReinDirectorCardDemoArchWisp":
                        archaicWispMod = true;                                                                                                                                    
                        break; //done
                }
                //oh no, i'm checking plugins for incompats, how evil i am!
                if (this.replacedFuncs.Contains(pluginInfo.Metadata.GUID))
                {
                    logger.LogWarning("Cloudburst may have conflicts with the following mod:" + pluginInfo.Metadata.Name + "");
                    //lmao, no.
                    //logger.LogWarning("If you do not choose to disable these mod(s), Cloudburst will disable certain features to provide a bug free experenice at the cost of content.");
                }
            }
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
            GlobalEventManager.OnInteractionsGlobal += GlobalEventManager_OnInteractionsGlobal;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManagerOnOnCharacterDeath;
            On.RoR2.GlobalEventManager.OnTeamLevelUp += GlobalEventManager_OnTeamLevelUp;
            //qol
            //On.RoR2.ItemCatalog.RegisterItem += ItemCatalog_RegisterItem;
            On.RoR2.BuffCatalog.RegisterBuff += BuffCatalog_RegisterBuff;
            On.EntityStates.Commando.DodgeState.OnEnter += DodgeState_OnEnter;
            On.RoR2.PickupPickerController.OnInteractionBegin += PickupPickerController_OnInteractionBegin;
        }

        private void GlobalEventManager_OnInteractionsGlobal(Interactor interactor, IInteractable interactable, GameObject gameObject)
        {
            CharacterBody characterBody = interactor.GetComponent<CharacterBody>();
            if (characterBody)
            {
                Inventory inventory = characterBody.inventory;
                CharacterMaster master = characterBody.master;
                if (inventory && master)
                {
                    int keyCardCount = inventory.GetItemCount(BrokenKeycard.itemIndex);
                    int sundialCount = inventory.GetItemCount(Sundial.itemIndex);
                    if (keyCardCount > 0)
                    {
                        master.GiveMoney(3U + (uint)(keyCardCount * 3));
                    }
                    if (sundialCount > 0 && Util.CheckRoll(15 + (sundialCount * 5), master))
                    {
                        Util.PlaySound("Play_item_use_gainArmor", characterBody .gameObject);
                        int h = UnityEngine.Random.Range(0, eliteBuffList.Count);
                        characterBody.AddTimedBuff(eliteBuffList[h], 5);
                    }
                }
            }
        }



        /*private void ItemCatalog_RegisterItem(On.RoR2.ItemCatalog.orig_RegisterItem orig, ItemIndex itemIndex, ItemDef itemDef)
        {
            if (itemIndex == ItemIndex.Incubator)
            {
                itemDef.tier = ItemTier.Tier2;
            }
            orig(itemIndex, itemDef);
        }*/
        #region GlobalEventManager
        private void GlobalEventManagerOnOnCharacterDeath(DamageReport damageReport)
        {
            if (!damageReport.attackerBody || !damageReport.victimBody || !damageReport.attacker || !damageReport.victim || !damageReport.attackerMaster || !damageReport.victimMaster || damageReport == null)
                return;
            if (damageReport.victim.body.isChampion && damageReport.attackerBody && damageReport.attackerBody.isPlayerControlled && damageReport != null)
            {
                CharacterBody attackerBody = damageReport.attackerBody;
                CharacterBody victimBody = damageReport.victimBody;
                CharacterMaster attackerMaster = damageReport.attackerMaster;
                CharacterMaster victimMaster = damageReport.victimMaster;
                Inventory attackerInventory = attackerMaster ? attackerMaster.inventory : null;
                Inventory victimInventory = victimMaster ? victimMaster.inventory : null;
                int GrinderCount = attackerInventory.GetItemCount(Item.itemIndex);
                if (GrinderCount > 0 && Util.CheckRoll(15 + (GrinderCount * 5)) && attackerMaster)
                {
                    Util.PlaySound("ui_obj_casinoChest_open", attackerBody.gameObject);
                    attackerInventory.GiveItem(GetRandomItem(bossitemList), 1);
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
                if (rootCount > 0 && Util.CheckRoll((17 + (rootCount * 3    )), attackerMaster) && attackerMaster && damageReport.victimMaster)
                {
                    Util.PlaySound("Play_nullifier_attack1_root", victimBody.gameObject);
                    victimBody.AddTimedBuff(BuffIndex.Cripple, 3);
                }
                if (scpCount > 0 && damageReport.victimBody && victimMaster)
                {
                    victimBody.AddTimedBuff(scpBuffList[scpRandom], (scpCount * 2));
                }                                                 
            }
        }

        #endregion
        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            if (self.HasBuff(Sundial.solarBuff))
            {
                self.baseArmor += 10;
            }
            orig(self);
        }

        private void CharacterBody_RemoveBuff(On.RoR2.CharacterBody.orig_RemoveBuff orig, CharacterBody self, BuffIndex buffType)
        {
            if (buffType == Sundial.solarBuff)
            {
                self.baseArmor -= 10;
            }
            orig(self, buffType);
        }

        private void PickupPickerController_OnInteractionBegin(On.RoR2.PickupPickerController.orig_OnInteractionBegin orig, PickupPickerController self, Interactor activator)
        {
            activator.GetComponent<CharacterBody>().AddTimedBuff(BuffIndex.ArmorBoost, 1);
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
            if (!deathMarkMod)
            {
                if (buffIndex == BuffIndex.DeathMark)
                {
                    buffDef.canStack = true;
                }
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

                            if (nokiaItemCount > 0 && NetworkServer.active && Util.CheckRoll(50, master))
                            {
                                NokiaCall( characterBody.transform, nokiaItemCount);
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
            if (index == Lumpkin.EquipIndex)
            {
                lum.Scream(self.characterBody);
                return true;
            }
            return orig(self, index); //must
        }

        private void GrantItem(On.RoR2.GenericPickupController.orig_GrantItem orig, GenericPickupController self, CharacterBody body, Inventory inventory)
        {
            orig(self, body, inventory);
            if (self && inventory && inventory.GetItemCount(Pillow.itemIndex) > 0)
            {
                body.AddTimedBuff(BuffIndex.Cloak, 2 * inventory.GetItemCount(Pillow.itemIndex));
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

                    obj.holdoutZoneController.baseRadius = obj.holdoutZoneController.baseRadius + (count * 3);
                }
            }
        }
    }
}
#endregion