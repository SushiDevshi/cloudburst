using R2API;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CloudBurst.Equipment
{
    public sealed class UnstableQuantumReactor
    {
        public static EquipmentIndex EquipIndex { get; private set; }

        public UnstableQuantumReactor()
        {
            {
                R2API.AssetPlus.Languages.AddToken("UNSTABLEQUANTUMREACTOR_ITEM_TOKEN", "Unstable Quantum Reactor");
                R2API.AssetPlus.Languages.AddToken("UNSTABLEQUANTUMREACTOR_ITEM_DESCRIPTION_TOKEN", "Fire random projectiles for a short duration on use.");
                R2API.AssetPlus.Languages.AddToken("UNSTABLEQUANTUMREACTOR_ITEM_PICKUP_TOKEN", "Fire random projectiles for a short duration on use.");
                var equipDef = new EquipmentDef
                {
                    cooldown = 110,
                    pickupModelPath = "Prefabs/PickupModels/PickupSoda",
                    pickupIconPath = "Textures/ItemIcons/texbirdeyeicon",
                    pickupToken = "UNSTABLEQUANTUMREACTOR_ITEM_PICKUP_TOKEN",
                    nameToken = "UNSTABLEQUANTUMREACTOR_ITEM_TOKEN",
                    descriptionToken = "UNSTABLEQUANTUMREACTOR_ITEM_DESCRIPTION_TOKEN",
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
        public void BecomeUnstable(CharacterBody user)
        {
            GameObject projPrefab = Resources.Load<GameObject>("prefabs/Projectiles/loaderzapcone");
            var projInfo = new FireProjectileInfo
            {
                crit = false,
                damage = 1.5f,
                owner = user.gameObject,
                position = user.transform.position,
                projectilePrefab = projPrefab,
                rotation = Util.QuaternionSafeLookRotation(user.aimOrigin),
            };
            ProjectileManager.instance.FireProjectile(projInfo);
        }
    }
}