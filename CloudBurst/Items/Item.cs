using R2API;
using RoR2;
using UnityEngine;

namespace CloudBurst.Items
{
    public sealed class Item
    {
        public static ItemIndex itemIndex { get; private set; }
        public int stacking { get; private set; } = 5;

        public Item()
        {
            {
                LanguageAPI.Add("PORTABLEGRINDER_ITEM_TOKEN", "Portable Grinder");
                LanguageAPI.Add("PORTABLEGRINDER_ITEM_DESCRIPTION_TOKEN", "15% chance for bosses to drop a random boss item when killed");
                LanguageAPI.Add("PORTABLEGRINDER_ITEM_PICKUP_TOKEN", "Chance for bosses to drop boss items when killed");
                var itemDef = new ItemDef
                {
                    pickupModelPath = "Prefabs/PickupModels/PickupMystery",
                    pickupIconPath = "Textures/MiscIcons/texMysteryIcon",
                    pickupToken = "PORTABLEGRINDER_ITEM_DESCRIPTION_TOKEN",
                    nameToken = "PORTABLEGRINDER_ITEM_TOKEN",
                    descriptionToken = "PORTABLEGRINDER_ITEM_DESCRIPTION_TOKEN",
                    loreToken = "",
                    name = "PortableGrinder",
                    tier = ItemTier.Tier3
                };

                var prefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");

                var rule = new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localScale = new Vector3(0f, 0, 0f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localPos = new Vector3(0, 0f, 0f)
                };

                var item = new CustomItem(itemDef, new[] { rule });
                itemIndex = ItemAPI.Add(item);
            };
        }
    }
}
public sealed class MechanicalTrinket
{
    public static ItemIndex itemIndex { get; private set; }
    public MechanicalTrinket()
    {
        {
            LanguageAPI.Add("TRINKET_ITEM_TOKEN", "Mechanical Trinket");
            LanguageAPI.Add("TRINKET_ITEM_DESCRIPTION_TOKEN", "Increase teleporter radius.");
            LanguageAPI.Add("TRINKET_ITEM_PICKUP_TOKEN", "Increase teleporter radius.");
            var itemDef = new ItemDef
            {
                pickupModelPath = "Prefabs/PickupModels/PickupMystery",
                pickupIconPath = "Textures/MiscIcons/texMysteryIcon",
                pickupToken = "TRINKET_ITEM_PICKUP_TOKEN",
                nameToken = "TRINKET_ITEM_TOKEN",
                descriptionToken = "TRINKET_ITEM_DESCRIPTION_TOKEN",
                name = "MechanicalTrinket",
                tier = ItemTier.Tier2
            };

            var prefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");

            var rule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = prefab,
                childName = "Chest",
                localScale = new Vector3(0f, 0, 0f),
                localAngles = new Vector3(0f, 0f, 0f),
                localPos = new Vector3(0, 0f, 0f)
            };

            var item = new CustomItem(itemDef, new[] { rule });
            itemIndex = ItemAPI.Add(item);
        };
    }
}
public sealed class Sundial
{

    public static ItemIndex itemIndex { get; private set; }
    public static BuffIndex solarBuff { get; private set; }
    public static ProcType procType { get; private set; }
    public Sundial()
    {
        {
            LanguageAPI.Add("SUNDIAL_ITEM_TOKEN", "Sundial");
            LanguageAPI.Add("SUNDIAL_ITEM_DESCRIPTION_TOKEN", "Chance to gain light armor after being hit. +3 seconds per stack.");
            LanguageAPI.Add("SUNDIAL_ITEM_PICKUP_TOKEN", "No pain, no gain.");
            var itemDef = new ItemDef
            {
                pickupModelPath = "Prefabs/PickupModels/PickupMystery",
                pickupIconPath = "Textures/MiscIcons/texMysteryIcon",
                pickupToken = "SUNDIAL_ITEM_PICKUP_TOKEN",
                nameToken = "SUNDIAL_ITEM_TOKEN",
                name = "WeirdSundial",
                descriptionToken = "SUNDIAL_ITEM_DESCRIPTION_TOKEN",
                tier = ItemTier.Tier2,
            };

            BuffDef solarBuffDef = new BuffDef
            {
                buffColor = new Color(235, 182, 92),
                buffIndex = BuffIndex.Count,
                canStack = false,
                eliteIndex = EliteIndex.None,
                iconPath = "Textures/BuffIcons/texbuffpulverizeicon",
                isDebuff = false,
                name = "SolarGain"
            };
            solarBuff = BuffAPI.Add(new CustomBuff(solarBuffDef));
            //solarBuff = ItemAPI.Add(new CustomBuff(solarbuff.name, solarbuff));

            var prefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");

            var rule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = prefab,
                childName = "Chest",
                localScale = new Vector3(0f, 0, 0f),
                localAngles = new Vector3(0f, 0f, 0f),
                localPos = new Vector3(0, 0f, 0f)
            };

            var item = new CustomItem(itemDef, new[] { rule });
            itemIndex = ItemAPI.Add(item);
        };
    }
}
public sealed class Root
{

    public static ItemIndex itemIndex { get; private set; }
    public static ProcType procType { get; private set; }
    public Root()
    {
        {
            LanguageAPI.Add("ROOT_ITEM_TOKEN", "Crippling Root");
            LanguageAPI.Add("ROOT_ITEM_DESCRIPTION_TOKEN", "Chance to cripple enemies for 3 seconds. +3 seconds per stack.");
            LanguageAPI.Add("ROOT_ITEM_PICKUP_TOKEN", "Chance to cripple enemies.");
            var itemDef = new ItemDef
            {
                pickupModelPath = "Prefabs/PickupModels/PickupMystery",
                pickupIconPath = "Textures/MiscIcons/texMysteryIcon",
                pickupToken = "ROOT_ITEM_PICKUP_TOKEN",
                nameToken = "ROOT_ITEM_TOKEN",
                descriptionToken = "ROOT_ITEM_DESCRIPTION_TOKEN",
                name = "Root",
                tier = ItemTier.Tier1
            };

            var prefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");

            var rule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = prefab,
                childName = "Chest",
                localScale = new Vector3(0f, 0, 0f),
                localAngles = new Vector3(0f, 0f, 0f),
                localPos = new Vector3(0, 0f, 0f)
            };

            var item = new CustomItem(itemDef, new[] { rule });
            itemIndex = ItemAPI.Add(item);
        };
    }
}
public sealed class Pillow
{
    public static ItemIndex itemIndex { get; private set; }
    public Pillow()
    {
        {
            LanguageAPI.Add("PILLOW_ITEM_TOKEN", "Fluffy Pillow");
            LanguageAPI.Add("PILLOW_ITEM_DESCRIPTION_TOKEN", "Gain speed when picking up an item. Five extra seconds per stack.");
            LanguageAPI.Add("PILLOW_ITEM_PICKUP_TOKEN", "Gain speed when picking up an item.");
            var itemDef = new ItemDef
            {
                pickupModelPath = "Prefabs/PickupModels/PickupMystery",
                pickupIconPath = "Textures/MiscIcons/texMysteryIcon",
                pickupToken = "PILLOW_ITEM_PICKUP_TOKEN",
                nameToken = "PILLOW_ITEM_TOKEN",
                descriptionToken = "PILLOW_ITEM_DESCRIPTION_TOKEN",
                name = "Pillow",
                tier = ItemTier.Tier1
            };

            var prefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");

            var rule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = prefab,
                childName = "Chest",
                localScale = new Vector3(0f, 0, 0f),
                localAngles = new Vector3(0f, 0f, 0f),
                localPos = new Vector3(0, 0f, 0f)
            };

            var item = new CustomItem(itemDef, new[] { rule });
            itemIndex = ItemAPI.Add(item);
        };
    }
}
public sealed class Nokia
{

    public static ItemIndex itemIndex { get; private set; }
    public Nokia()
    {
        {
            LanguageAPI.Add("NOKIA_ITEM_TOKEN", "Interstellar Nokia");
            LanguageAPI.Add("NOKIA_ITEM_DESCRIPTION_TOKEN", "Chance to gain an item on level up. +5% chance to gain a green item per stack");
            LanguageAPI.Add("NOKIA_ITEM_PICKUP_TOKEN", "Chance to gain an item on level up");
            var itemDef = new ItemDef
            {
                pickupModelPath = "Prefabs/PickupModels/PickupMystery",
                pickupIconPath = "Textures/MiscIcons/texMysteryIcon",
                pickupToken = "NOKIA_ITEM_PICKUP_TOKEN",
                nameToken = "NOKIA_ITEM_TOKEN",
                descriptionToken = "NOKIA_ITEM_DESCRIPTION_TOKEN",
                name = "Nokia",
                tier = ItemTier.Tier3
            };

            var prefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");

            var rule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = prefab,
                childName = "Chest",
                localScale = new Vector3(0f, 0, 0f),
                localAngles = new Vector3(0f, 0f, 0f),
                localPos = new Vector3(0, 0f, 0f)
            };

            var item = new CustomItem(itemDef, new[] { rule });
            itemIndex = ItemAPI.Add(item);
        };
    }
}
public sealed class SCP
{

    public static ItemIndex itemIndex { get; private set; }
    public SCP()
    {
        {
            LanguageAPI.Add("SCP_ITEM_LORE_TOKEN", "Order: \u201C[REDACTED]\u201D\r\nTracking Number: [REDACTED]\r\nEstimated Delivery: [REDACTED]\r\nShipping Method: [REDACTED]\r\nShipping Address: [REDACTED], [REDACTED]\r\nShipping Details:\r\n\r\nSecure, contain, protect.\r\n");
            LanguageAPI.Add("SCP_ITEM_TOKEN", "[REDACTED]");
            LanguageAPI.Add("SCP_ITEM_DESCRIPTION_TOKEN", "[REDACTED]");
            LanguageAPI.Add("SCP_ITEM_PICKUP_TOKEN", "[REDACTED]");
            var itemDef = new ItemDef
            {
                pickupModelPath = "Prefabs/PickupModels/PickupMystery",
                pickupIconPath = "Textures/MiscIcons/texMysteryIcon",
                pickupToken = "SCP_ITEM_PICKUP_TOKEN",
                nameToken = "SCP_ITEM_TOKEN",
                descriptionToken = "SCP_ITEM_DESCRIPTION_TOKEN",
                name = "SCP",
                loreToken = "SCP_ITEM_LORE_TOKEN",
                tier = ItemTier.Tier3
            };

            var prefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");

            var rule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = prefab,
                childName = "Chest",
                localScale = new Vector3(0f, 0, 0f),
                localAngles = new Vector3(0f, 0f, 0f),
                localPos = new Vector3(0, 0f, 0f)
            };

            var item = new CustomItem(itemDef, new[] { rule });
            itemIndex = ItemAPI.Add(item);
        };
    }
}

public sealed class BrokenScepter
{

    public static ItemIndex itemIndex { get; private set; }
    public BrokenScepter()
    {
        {
            LanguageAPI.Add("BROKENSCEPTER_ITEM_TOKEN", "Broken scepter");
            LanguageAPI.Add("BROKENSCEPTER_ITEM_DESCRIPTION_TOKEN", "Gain unique benefits on level up.");
            LanguageAPI.Add("BROKENSCEPTER_ITEM_LORE_TOKEN", "Boy, her march is on the horizon. I ask of you, before my time is due, to unite the broken pieces, and return to me. Let the wrongs of the past be right.");
            LanguageAPI.Add("BROKENSCEPTER_ITEM_PICKUP_TOKEN", "Unite the broken pieces.");
            var itemDef = new ItemDef
            {
                pickupModelPath = "Prefabs/PickupModels/PickupSoda",
                pickupIconPath = "Textures/ItemIcons/texbirdeyeicon",
                pickupToken = "BROKENSCEPTER_ITEM_PICKUP_TOKEN",
                nameToken = "BROKENSCEPTER_ITEM_TOKEN",
                descriptionToken = "BROKENSCEPTER_ITEM_DESCRIPTION_TOKEN",
                loreToken = "BROKENSCEPTER_ITEM_LORE_TOKEN",
                name = "BrokenScepter",
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
