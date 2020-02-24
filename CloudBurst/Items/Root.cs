using R2API;
using RoR2;
using UnityEngine;

namespace CloudBurst.Items
{
    public sealed class Root
    {

        public static ItemIndex itemIndex { get; private set; }
        public static ProcType procType { get; private set; }
        public Root()
        {
            {
                R2API.AssetPlus.Languages.AddToken("ROOT_ITEM_TOKEN", "root");
                R2API.AssetPlus.Languages.AddToken("ROOT_ITEM_DESCRIPTION_TOKEN", "it does something");
                R2API.AssetPlus.Languages.AddToken("ROOT_ITEM_PICKUP_TOKEN", "idk");
                var itemDef = new ItemDef
                {
                    pickupModelPath = "Prefabs/PickupModels/PickupSoda",
                    pickupIconPath = "Textures/ItemIcons/texbirdeyeicon",
                    pickupToken = "ROOT_ITEM_PICKUP_TOKEN",
                    nameToken = "ROOT_ITEM_TOKEN",
                    descriptionToken = "ROOT_ITEM_DESCRIPTION_TOKEN",
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
