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
        static UnityEngine.Random UnityRnd = new UnityEngine.Random();
        public static EquipmentIndex EquipIndex { get; private set; }

        public static List<GameObject> projectileList = new List<GameObject>{

            Resources.Load<GameObject>("prefabs/projectiles/ArtifactShellSeekingSolarFlare"),
            Resources.Load<GameObject>("prefabs/projectiles/BeetleQueenSpit"),
            Resources.Load<GameObject>("prefabs/projectiles/CommandoGrenadeProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/DroneRocket"),
            Resources.Load<GameObject>("prefabs/projectiles/Sawmerang"),
            Resources.Load<GameObject>("prefabs/projectiles/RoboBallProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/SMMaulingRockLarge"),
            Resources.Load<GameObject>("prefabs/projectiles/SMMaulingRockMedium"),
            Resources.Load<GameObject>("prefabs/projectiles/SMMaulingRockSmall"),
            Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/Sunder"),
            Resources.Load<GameObject>("prefabs/projectiles/SyringeProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/SuperRoboBallProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/SyringeProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/SyringeProjectileHealing"),
            Resources.Load<GameObject>("prefabs/projectiles/BellBall"),
            Resources.Load<GameObject>("prefabs/projectiles/CommandoGrenadeProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/CrocoDiseaseProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/CrocoSpit"),
            Resources.Load<GameObject>("prefabs/projectiles/DaggerProjectile"),

        };

    public UnstableQuantumReactor()
        {
            {
                R2API.AssetPlus.Languages.AddToken("UNSTABLEQUANTUMREACTOR_ITEM_TOKEN", "Unstable Quantum Reactor");
                R2API.AssetPlus.Languages.AddToken("UNSTABLEQUANTUMREACTOR_ITEM_DESCRIPTION_TOKEN", "Fire a random projectile.");
                R2API.AssetPlus.Languages.AddToken("UNSTABLEQUANTUMREACTOR_ITEM_PICKUP_TOKEN", "Fire a random projectile on use.");
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
            InputBankTest aimRay = user.inputBank;

            int ah = UnityEngine.Random.Range(0, projectileList.Count);


            var projInfo = new FireProjectileInfo
            {
                crit = false,
                damage = 1.5f,
                owner = user.gameObject,
                position = user.transform.position,
                projectilePrefab = projectileList[ah],
                rotation = Util.QuaternionSafeLookRotation(aimRay.aimDirection),
            };
            ProjectileManager.instance.FireProjectile(projInfo);
        }
    }
}