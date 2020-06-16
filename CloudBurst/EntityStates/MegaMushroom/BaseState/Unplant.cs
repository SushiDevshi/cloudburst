using System;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace CloudBurst.Weapon.MegaMushroom
{
    // Token: 0x020008A2 RID: 2210
    public class UnPlant : BaseState
    {
        public static GameObject plantEffectPrefab = EntityStates.MiniMushroom.UnPlant.plantEffectPrefab;
        public static float baseDuration = EntityStates.MiniMushroom.UnPlant.baseDuration;
        public static string UnplantOutSoundString = EntityStates.MiniMushroom.UnPlant.UnplantOutSoundString;
        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            
            this.duration = UnPlant.baseDuration / this.attackSpeedStat;
            EffectManager.SimpleMuzzleFlash(UnPlant.plantEffectPrefab, base.gameObject, "BurrowCenter", false);
            Util.PlaySound(UnPlant.UnplantOutSoundString, base.gameObject);
            for (float num = 0f; num < 9f; num += 1f)
            {
                float num2 = 6.2831855f;
                Vector3 forward = new Vector3(Mathf.Cos(num / 9f * num2), 0f, Mathf.Sin(num / 9f * num2));
                ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("prefabs/projectiles/Sunder"), base.transform.position, Quaternion.LookRotation(forward), base.gameObject, this.damageStat * SporeGrenade.damageCoefficient, 0f, base.RollCrit(), DamageColorIndex.Default);
            }
            base.PlayAnimation("Plant", "PlantEnd", "PlantEnd.playbackRate", this.duration);
        }
        public override void OnExit()
        {
            base.PlayAnimation("Plant, Additive", "Empty");
            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

    }
}
