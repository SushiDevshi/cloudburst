using R2API;
using RoR2;
using UnityEngine;

namespace CloudBurst.Items
{
    public sealed class Pillow
    {
        public static ItemIndex itemIndex { get; private set; }
        public Pillow()
        {
            {
                R2API.AssetPlus.Languages.AddToken("PILLOW_ITEM_TOKEN", "Fluffy Pillow");
                R2API.AssetPlus.Languages.AddToken("PILLOW_ITEM_DESCRIPTION_TOKEN", "it does something");
                R2API.AssetPlus.Languages.AddToken("PILLOW_ITEM_PICKUP_TOKEN", "idk");
                var itemDef = new ItemDef
                {
                    pickupModelPath = "Prefabs/PickupModels/PickupSoda",
                    pickupIconPath = "Textures/ItemIcons/texbirdeyeicon",
                    pickupToken = "PILLOW_ITEM_PICKUP_TOKEN",
                    nameToken = "PILLOW_ITEM_TOKEN",
                    descriptionToken = "PILLOW_ITEM_DESCRIPTION_TOKEN",
                    tier = ItemTier.Tier1
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