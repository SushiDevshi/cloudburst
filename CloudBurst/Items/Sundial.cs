using R2API;
using RoR2;
using UnityEngine;

namespace CloudBurst.Items
{
    public sealed class Sundial
    {

        public static ItemIndex itemIndex { get; private set; }
        public static ProcType procType { get; private set; }
        public Sundial()
        {
            {
                R2API.AssetPlus.Languages.AddToken("SUNDIAL_ITEM_TOKEN", "Sundial");
                R2API.AssetPlus.Languages.AddToken("SUNDIAL_ITEM_DESCRIPTION_TOKEN", "Gain a buff while in direct sunlight");
                R2API.AssetPlus.Languages.AddToken("SUNDIAL_ITEM_PICKUP_TOKEN", "The sun shines on those who embrace it.");
                var itemDef = new ItemDef
                {
                    pickupModelPath = "Prefabs/PickupModels/PickupSoda",
                    pickupIconPath = "Textures/ItemIcons/texbirdeyeicon",
                    pickupToken = "SUNDIAL_ITEM_PICKUP_TOKEN",
                    nameToken = "SUNDIAL_ITEM_TOKEN",
                    descriptionToken = "SUNDIAL_ITEM_DESCRIPTION_TOKEN",
                    tier = ItemTier.Tier2
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
