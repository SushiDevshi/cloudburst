using R2API;
using RoR2;
using UnityEngine;

namespace CloudBurst.Items
{
    public sealed class Grinder
    {
        public static ItemIndex itemIndex { get; private set; }
        public int stacking { get; private set; } = 5;
        public Grinder()
        {
            {
                R2API.AssetPlus.Languages.AddToken("PORTABLEGRINDER_ITEM_TOKEN", "Portable Grinder");
                R2API.AssetPlus.Languages.AddToken("PORTABLEGRINDER_ITEM_DESCRIPTION_TOKEN", "15% chance for bosses to drop a random boss item when killed");
                R2API.AssetPlus.Languages.AddToken("PORTABLEGRINDER_ITEM_PICKUP_TOKEN", "Chance for bosses to drop boss items when killed");
                var itemDef = new ItemDef
                {
                    pickupModelPath = "Prefabs/PickupModels/PickupSoda",
                    pickupIconPath = "Textures/ItemIcons/texbirdeyeicon",
                    pickupToken = "PORTABLEGRINDER_ITEM_DESCRIPTION_TOKEN",
                    nameToken = "PORTABLEGRINDER_ITEM_TOKEN",
                    descriptionToken = "PORTABLEGRINDER_ITEM_DESCRIPTION_TOKEN",
                    loreToken = "bop dop it be a BOX ",
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