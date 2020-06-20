﻿using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace CloudBurst.Weapon.MegaMushroom
{
    public class Plant : BaseState
    {
        public static float healFraction = EntityStates.MiniMushroom.Plant.healFraction;
        public static float baseMaxDuration = EntityStates.MiniMushroom.Plant.baseMaxDuration;
        public static float baseMinDuration = EntityStates.MiniMushroom.Plant.baseMinDuration;
        public static float mushroomRadius = EntityStates.MiniMushroom.Plant.mushroomRadius;
        public static string healSoundLoop = EntityStates.MiniMushroom.Plant.healSoundLoop;
        public static string healSoundStop = EntityStates.MiniMushroom.Plant.healSoundStop;
        private float maxDuration;
        private float minDuration;
        private GameObject mushroomWard;
        private uint soundID;
        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("Plant", "PlantLoop");
            this.maxDuration = Plant.baseMaxDuration / this.attackSpeedStat;
            this.minDuration = Plant.baseMinDuration / this.attackSpeedStat;
            this.soundID = Util.PlaySound(Plant.healSoundLoop, base.characterBody.modelLocator.modelTransform.gameObject);
            if (!NetworkServer.active)
            {
                return;
            }
            for (float num = 0f; num < 9f; num += 1f)
            {
                float num2 = 6.2831855f;
                Vector3 forward = new Vector3(Mathf.Cos(num / 9f * num2), 0f, Mathf.Sin(num / 9f * num2));
                var projInfo = new FireProjectileInfo
                {
                    crit = base.RollCrit(),
                    damage = this.damageStat * (SporeGrenade.damageCoefficient * 1.2f),
                    owner = base.gameObject,
                    position = base.transform.position,
                    projectilePrefab = Resources.Load<GameObject>("prefabs/projectiles/Sunder"),
                    rotation = Quaternion.LookRotation(forward),
                    damageColorIndex = DamageColorIndex.Default,
                    force = 2500,
                    procChainMask = default
                };
                ProjectileManager.instance.FireProjectile(projInfo);
            }
            if (this.mushroomWard == null)
            {
                this.mushroomWard = Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/MiniMushroomWard"), base.characterBody.footPosition, Quaternion.identity);
                this.mushroomWard.GetComponent<TeamFilter>().teamIndex = base.teamComponent.teamIndex;
                if (this.mushroomWard)
                {
                    HealingWard component = this.mushroomWard.GetComponent<HealingWard>();
                    component.healFraction = healFraction;
                    component.healPoints = 0f;
                    component.Networkradius = mushroomRadius;
                }
                NetworkServer.Spawn(this.mushroomWard);
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {

                bool flag = base.inputBank.moveVector.sqrMagnitude > 0.1f;
                if (base.fixedAge > this.maxDuration || (base.fixedAge > this.minDuration) || (flag))
                {
                    this.outer.SetNextState(new UnPlant());
                }
            }
        }
        public override void OnExit()
        {
            base.PlayAnimation("Plant", "Empty");
            AkSoundEngine.StopPlayingID(this.soundID);
            Util.PlaySound(Plant.healSoundStop, base.gameObject);
            if (this.mushroomWard)
            {
                EntityState.Destroy(this.mushroomWard);
            }
            base.OnExit();
        }

    }
}
