using R2API;
using RoR2;
using UnityEngine;

namespace CloudBurst.Items
{
    public sealed class Nokia
    {

        public static ItemIndex itemIndex { get; private set; }
        public Nokia()
        {
            {
                R2API.AssetPlus.Languages.AddToken("NOKIA_ITEM_TOKEN", "Nokia");
                R2API.AssetPlus.Languages.AddToken("NOKIA_ITEM_DESCRIPTION_TOKEN", "Gain an item on level up");
                R2API.AssetPlus.Languages.AddToken("NOKIA_ITEM_PICKUP_TOKEN", "Gain an item on level up");
                var itemDef = new ItemDef
                {
                    pickupModelPath = "Prefabs/PickupModels/PickupSoda",
                    pickupIconPath = "Textures/ItemIcons/texbirdeyeicon",
                    pickupToken = "NOKIA_ITEM_PICKUP_TOKEN",
                    nameToken = "NOKIA_ITEM_TOKEN",
                    descriptionToken = "NOKIA_ITEM_DESCRIPTION_TOKEN",
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
