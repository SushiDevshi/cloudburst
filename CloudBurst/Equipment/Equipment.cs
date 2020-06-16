using R2API;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;

namespace CloudBurst.Equipment
{
    public sealed class Equipment
    {
        public static EquipmentIndex EquipIndex { get; private set; }

        public Equipment()
        {
            {
                LanguageAPI.Add("HERETICSBOX_ITEM_TOKEN", "Heretic's box");
                LanguageAPI.Add("HERETICSBOX_ITEM_DESCRIPTION_TOKEN", " Summon 4 powerful foes, kill them for a chance to gain power.");
                LanguageAPI.Add("HERETICSBOX_ITEM_PICKUP_TOKEN", "The heretic's box...?");
                var equipDef = new EquipmentDef
                {
                    cooldown = 145,
                    pickupModelPath = "Prefabs/PickupModels/PickupSoda",
                    pickupIconPath = "Textures/ItemIcons/texSodaIcon",
                    pickupToken = "HERETICSBOX_ITEM_PICKUP_TOKEN",
                    nameToken = "HERETICSBOX_ITEM_TOKEN",
                    descriptionToken = "HERETICSBOX_ITEM_DESCRIPTION_TOKEN",
                    canDrop = true,
                    enigmaCompatible = true,
                    isLunar = true,
                    name = "HereticBox",
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
    }

    public sealed class Lumpkin
    {
        public static EquipmentIndex EquipIndex { get; private set; }
        public Lumpkin()
        {
            {
                LanguageAPI.Add("LUMPKIN_ITEM_LORE_TOKEN", "\"Lumpkin, one of the many commanders in the War of 2019, possessed a scream that could deafen his oppenents, and allies. It was unknown how he could scream so loudly, until he was killed in the final battle of WW19 and had his lungs ripped from his chest. \r\n\r\nHis lungs, pictured above, allowed him to scream loudly without injuring himself.\"\r\n\r\n-Exhibit at The National WW19 Museum");
                LanguageAPI.Add("LUMPKIN_ITEM_TOKEN", "The Lumpkin");
                LanguageAPI.Add("LUMPKIN_ITEM_DESCRIPTION_TOKEN", "Release a Brazilian scream, stunning enemies within the immediate vicinity.");
                LanguageAPI.Add("LUMPKIN_ITEM_PICKUP_TOKEN", "And his screams were Brazilian...");
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
                    name = "Lumpkin",
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
            blastAttack.attacker = Screamer.gameObject;
            blastAttack.inflictor = Screamer.gameObject;
            blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
            blastAttack.procCoefficient = 0f;
            blastAttack.damageColorIndex = DamageColorIndex.Item;
            blastAttack.falloffModel = BlastAttack.FalloffModel.None;
            blastAttack.damageType = DamageType.Stun1s;
            blastAttack.Fire();
        }
    }

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
                LanguageAPI.Add("UNSTABLEQUANTUMREACTOR_ITEM_TOKEN", "Unstable Quantum Reactor");
                LanguageAPI.Add("UNSTABLEQUANTUMREACTOR_ITEM_DESCRIPTION_TOKEN", "Fire a random projectile.");
                LanguageAPI.Add("UNSTABLEQUANTUMREACTOR_ITEM_PICKUP_TOKEN", "Fire a random projectile on use.");
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
                    loreToken = "",
                    name = "UnstableReactor"

                };

                var prefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupSoda");
                //unstable reactor go BRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR
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
                damage = 5 * user.damage,
                owner = user.gameObject,
                position = user.transform.position,
                projectilePrefab = projectileList[ah],
                rotation = Util.QuaternionSafeLookRotation(aimRay.aimDirection),
            };
            ProjectileManager.instance.FireProjectile(projInfo);
        }
    }
    public sealed class BrokenPrinter
    {
        public static EquipmentIndex EquipIndex { get; private set; }

        public BrokenPrinter()
        {
            {
                LanguageAPI.Add("SUMMONERBOX_ITEM_LORE_TOKEN", "Order: \u201CBroken Printer\u201D\r\nTracking Number: 72******\r\nEstimated Delivery: 08\\25\\2057\\\r\nShipping Method:  High Priority\r\nShipping Address: 836 Lane, Lab [42], Mars\r\nShipping Details:\r\n\r\nI can't speak for the rest of the researchers here, but I have no clue why you decided to send this here. We don't know what's wrong with this thing, and we don't know if there\u2019s anything wrong with it, it\u2019s completely alien. I don\u2019t think a person actually made this thing, there are materials in it that we\u2019ve never seen before.\r\n\r\nI\u2019m sending this back to you because it\u2019s more trouble than it\u2019s worth to keep here, and frankly those things it prints are horrifyingly hollow.\r\n");
                LanguageAPI.Add("SUMMONERBOX_ITEM_TOKEN", "Broken Printer");
                LanguageAPI.Add("SUMMONERBOX_ITEM_DESCRIPTION_TOKEN", "Summon a powerful but fragile ally that aids you in combat.");
                LanguageAPI.Add("SUMMONERBOX_ITEM_PICKUP_TOKEN", "Summon a powerful but fragile ally that aids you in combat.");
                var equipDef = new EquipmentDef
                {
                    cooldown = 110,
                    pickupModelPath = "Prefabs/PickupModels/PickupSoda",
                    pickupIconPath = "Textures/ItemIcons/texbirdeyeicon",
                    pickupToken = "SUMMONERBOX_ITEM_PICKUP_TOKEN",
                    nameToken = "SUMMONERBOX_ITEM_TOKEN",
                    descriptionToken = "SUMMONERBOX_ITEM_DESCRIPTION_TOKEN",
                    canDrop = true,
                    enigmaCompatible = true,
                    isLunar = false,
                    name = "SummonerBox",
                    loreToken = "SUMMONERBOX_ITEM_LORE_TOKEN"
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
        public void SummonMjnion(CharacterBody user)
        {
            CharacterMaster characterMaster;
            characterMaster = new MasterSummon
            {
                masterPrefab = MasterCatalog.GetMasterPrefab(MasterCatalog.FindAiMasterIndexForBody(user.bodyIndex)),
                position = user.footPosition + user.transform.up,
                rotation = user.transform.rotation,
                summonerBodyObject = null,
                ignoreTeamMemberLimit = true,
                teamIndexOverride = user.teamComponent.teamIndex

            }.Perform();
            characterMaster.bodyPrefab = user.master.bodyPrefab;
            characterMaster.Respawn(characterMaster.GetBody().footPosition, Quaternion.identity);

            characterMaster.inventory.CopyItemsFrom(user.inventory);
            characterMaster.inventory.ResetItem(ItemIndex.AutoCastEquipment);
            characterMaster.inventory.ResetItem(ItemIndex.BeetleGland);
            characterMaster.inventory.GiveItem(ItemIndex.BoostDamage, 5);
            characterMaster.inventory.GiveItem(ItemIndex.CutHp, 1);
            characterMaster.inventory.CopyEquipmentFrom(user.inventory);
        }
    }
}

