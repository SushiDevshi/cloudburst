using R2API;
using RoR2;
using RoR2.CharacterAI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CloudBurst.Equipment
{
    public sealed class BrokenPrinter
    {
        public static EquipmentIndex EquipIndex { get; private set; }

        public BrokenPrinter()
        {
            {
                R2API.AssetPlus.Languages.AddToken("SUMMONERBOX_ITEM_LORE_TOKEN", "As we discussed on the phone, my grandfather's printer is broken, it prints out odd things, such as humanoid creatures. The things the printer prints seem to be hollow, and violent. I attempted to print a pen using two pencils the other day, and the printer printed what looked to be a person. These people seem to be confused, and angry. I'm sending the printer in the hope that you'll be able to fix it.");
                R2API.AssetPlus.Languages.AddToken("SUMMONERBOX_ITEM_TOKEN", "Broken Printer");
                R2API.AssetPlus.Languages.AddToken("SUMMONERBOX_ITEM_DESCRIPTION_TOKEN", "Summon a powerful but fragile ally that aids you in combat.");
                R2API.AssetPlus.Languages.AddToken("SUMMONERBOX_ITEM_PICKUP_TOKEN", "Summon a powerful but fragile ally that aids you in combat.");
                var equipDef = new EquipmentDef
                {
                    cooldown = 110,
                    pickupModelPath = "Prefabs/PickupModels/PickupSoda",
                    pickupIconPath = "Textures/ItemIcons/texbirdeyeicon",
                    pickupToken = "SUMMONERBOX_ITEM_PICKUP_TOKEN",
                    nameToken = "SUMMONERBOX_ITEM_TOKEN",
                    descriptionToken = "SUMMONERBOX_ITEM_DESCRIPTION_TOKEN",
                    canDrop = true,
                    enigmaCompatible = true,
                    isLunar = false,
                    loreToken = ""
                };

                var prefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupSoda");

                var rule = new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localScale = new Vector3(0.15f, 0.15f, 0.15f),
                    localAngles = new Vector3(0f, 180f, 0f),
                    localPos = new Vector3(-0.35f, -0.1f, 0f)
                };

                var equip = new CustomEquipment(equipDef, new[] { rule });
                EquipIndex = ItemAPI.Add(equip);
            };
        }
        public void SummonMjnion(CharacterBody user)
        {
            CharacterMaster characterMaster;
            characterMaster = new MasterSummon
            {
                masterPrefab = MasterCatalog.FindMasterPrefab("MercMonsterMaster"),
                position = user.footPosition + user.transform.up,
                rotation = user.transform.rotation,
                summonerBodyObject = null,
                ignoreTeamMemberLimit = true,
                teamIndexOverride = user.teamComponent.teamIndex

            }.Perform();
            characterMaster.bodyPrefab = user.master.bodyPrefab;
            characterMaster.Respawn(characterMaster.GetBody().footPosition, Quaternion.identity);

            characterMaster.inventory.CopyItemsFrom(user.inventory);
            characterMaster.inventory.ResetItem(ItemIndex.AutoCastEquipment);
            characterMaster.inventory.ResetItem(ItemIndex.BeetleGland);
            characterMaster.inventory.CopyEquipmentFrom(user.inventory);
        }
    }
}