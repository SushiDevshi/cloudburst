using System;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace CloudBurst.Weapon.MegaMushroom
{
    public class InPlant : BaseState
    {
        public static GameObject burrowPrefab = EntityStates.MiniMushroom.InPlant.burrowPrefab;
        public static float baseDuration = EntityStates.MiniMushroom.InPlant.baseDuration;
        public static string burrowInSoundString = EntityStates.MiniMushroom.InPlant.burrowInSoundString;
        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = InPlant.baseDuration / this.attackSpeedStat;
            Util.PlaySound(InPlant.burrowInSoundString, base.gameObject);
            EffectManager.SimpleMuzzleFlash(InPlant.burrowPrefab, base.gameObject, "BurrowCenter", false);
            base.PlayAnimation("Plant", "PlantStart", "PlantStart.playbackRate", this.duration);
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
        }
        public override void OnExit()
        {
            base.PlayAnimation("Plant", "Empty");
            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextState(new Plant());
                return;
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
