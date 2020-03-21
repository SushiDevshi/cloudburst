using R2API;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;

namespace CloudBurst.Equipment
{
    public sealed class Lumpkin
    {
        public static EquipmentIndex EquipIndex { get; private set; }
        public Lumpkin()
        {
            {
                R2API.AssetPlus.Languages.AddToken("LUMPKIN_ITEM_LORE_TOKEN", "\"Lumpkin, one of the many commanders in the War of 2019, possessed a scream that could deafen his oppenents, and allies. No one knew how he could scream so loudly, until he was killed in the final battle of WW19 and had his lungs ripped from his chest. \r\n\r\nHis lungs, pictured above, allowed him to scream loudly without injuring himself.\"\r\n\r\n-Exhibit at The National WW19 Museum");
                R2API.AssetPlus.Languages.AddToken("LUMPKIN_ITEM_TOKEN", "The Lumpkin");
                R2API.AssetPlus.Languages.AddToken("LUMPKIN_ITEM_DESCRIPTION_TOKEN", "Release a Brazilian scream, stunning enemies within the immediate vicinity.");
                R2API.AssetPlus.Languages.AddToken("LUMPKIN_ITEM_PICKUP_TOKEN", "And his screams were Brazilian...");
                //yo
                var equipDef = new EquipmentDef
                {
                    cooldown = 45,
                    pickupModelPath = "Prefabs/PickupModels/PickupSoda",
                    pickupIconPath = "Textures/ItemIcons/texSodaIcon",
                    pickupToken = "LUMPKIN_ITEM_PICKUP_TOKEN",
                    nameToken = "LUMPKIN_ITEM_TOKEN",
                    descriptionToken = "LUMPKIN_ITEM_DESCRIPTION_TOKEN",
                    canDrop = true,
                    isLunar = false,
                    enigmaCompatible = true,
                    loreToken = "LUMPKIN_ITEM_LORE_TOKEN",
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
        public void Scream(CharacterBody Screamer)
        {
            BlastAttack blastAttack = new BlastAttack();
            blastAttack.baseDamage = Screamer.maxHealth * 2;
            blastAttack.baseForce = 150f;
            blastAttack.radius = 150;
            blastAttack.attacker = Screamer.master.gameObject;
            blastAttack.inflictor = Screamer.master.gameObject;
            blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
            blastAttack.procCoefficient = 0f;
            blastAttack.damageColorIndex = DamageColorIndex.Item;
            blastAttack.falloffModel = BlastAttack.FalloffModel.None;
            blastAttack.damageType = DamageType.Stun1s;
            blastAttack.Fire();
            
        }
    }
}