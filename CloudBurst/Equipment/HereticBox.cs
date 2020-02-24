using R2API;
using RoR2;
using UnityEngine;

namespace CloudBurst.Equipment
{
    public sealed class HereticBox
    {
        public static EquipmentIndex EquipIndex { get; private set; }
        public HereticBox()
        {
            {
                R2API.AssetPlus.Languages.AddToken("HERETICSBOX_ITEM_TOKEN", "Heretic's box");
                R2API.AssetPlus.Languages.AddToken("HERETICSBOX_ITEM_DESCRIPTION_TOKEN", " Summon 4 powerful foes, kill them for a chance to gain power.");
                R2API.AssetPlus.Languages.AddToken("HERETICSBOX_ITEM_PICKUP_TOKEN", "The heretic's box...?");
                var equipDef = new EquipmentDef
                {
                    cooldown = 145,
                    pickupModelPath = "Prefabs/PickupModels/PickupSoda",
                    pickupIconPath = "Textures/ItemIcons/texSodaIcon",
                    pickupToken = "HERETICSBOX_ITEM_PICKUP_TOKEN",
                    nameToken = "HERETICSBOX_ITEM_TOKEN",
                    descriptionToken = "HERETICSBOX_ITEM_DESCRIPTION_TOKEN",
                    canDrop = true,
                    enigmaCompatible = true,
                    isLunar = true,
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
    }
}