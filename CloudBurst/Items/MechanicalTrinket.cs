using R2API;
using RoR2;
using UnityEngine;

namespace CloudBurst.Items
{
    public sealed class MechanicalTrinket
    {
        public static ItemIndex itemIndex { get; private set; }
        public MechanicalTrinket()
        {
            {
                R2API.AssetPlus.Languages.AddToken("TRINKET_ITEM_TOKEN", "Mechanical Trinket");
                R2API.AssetPlus.Languages.AddToken("TRINKET_ITEM_DESCRIPTION_TOKEN", "Shorten teleporter charge time by 5% (+5%)");
                R2API.AssetPlus.Languages.AddToken("TRINKET_ITEM_PICKUP_TOKEN", "Shorten teleporter charge time ");
                var itemDef = new ItemDef
                {
                    pickupModelPath = "Prefabs/PickupModels/PickupSoda",
                    pickupIconPath = "Textures/ItemIcons/texbirdeyeicon",
                    pickupToken = "TRINKET_ITEM_PICKUP_TOKEN",
                    nameToken = "TRINKET_ITEM_TOKEN",
                    descriptionToken = "TRINKET_ITEM_DESCRIPTION_TOKEN",
                    tier = ItemTier.Tier3
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

                var item = new CustomItem(itemDef, new[] { rule });
                itemIndex = ItemAPI.Add(item);
            };
        }
    }
}