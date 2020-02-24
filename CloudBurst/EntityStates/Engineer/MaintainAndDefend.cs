using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using CloudBurst;
using EntityStates.Engi.EngiWeapon;

namespace CloudBurst.Weapon
{
    public class MANDA : BaseState
    {
        public float force = 3f;
        public float duration;
        public float bulletCount = 3;
        public float baseDuration = .5f;
        public float recoilAmplitude = 0f;
        public bool buttonReleased;
        private Ray projectileRay;
        private Transform modelTransform;
        public override void OnEnter()
        {
            base.OnEnter();
            this.modelTransform = base.GetModelTransform();
            this.projectileRay = base.GetAimRay();
            this.duration = this.baseDuration / base.attackSpeedStat;
            if (this.modelTransform)
            {
                ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
                if (component)
                {
                    Transform transform = component.FindChild("MuzzleLeft");
                    if (transform)
                    {
                        this.projectileRay.origin = transform.position;
                    }
                }
            }
            base.AddRecoil(-1f * FireGrenades.recoilAmplitude, -2f * FireGrenades.recoilAmplitude, -1f * FireGrenades.recoilAmplitude, 1f * FireGrenades.recoilAmplitude);
            if (isAuthority)
            {
                GameObject projPrefab = CloudBurstPlugin.EngiMADProjectile;
                var projInfo = new FireProjectileInfo
                {
                    crit = RollCrit(),
                    damage = 1.5f,
                    owner = base.gameObject,
                    position = base.transform.position,
                    projectilePrefab = projPrefab,
                    rotation = Util.QuaternionSafeLookRotation(projectileRay.direction),
                    force = 5000,
                };
                ProjectileManager.instance.FireProjectile(projInfo);
                base.PlayCrossfade("Gesture Left Cannon, Additive", "FireGrenadeLeft", 0.1f);
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.buttonReleased |= !base.inputBank.skill1.down;
            bool flag = base.fixedAge >= this.duration && base.isAuthority;
            if (flag)
            {
                this.outer.SetNextStateToMain();
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
