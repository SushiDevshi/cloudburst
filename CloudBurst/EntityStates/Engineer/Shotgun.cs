using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using CloudBurst;
using EntityStates.Engi.EngiWeapon;

namespace CloudBurst.Weapon
{
    public class Shotgun : BaseSkillState
    {
        public bool buttonReleased;
        public float force = 3f;
        public float duration = 5;
        public float bulletCount = 3;
        public float baseDuration = 105f;
        public float recoilAmplitude = 0f;
        private Ray MuzzleLeftRay;
        private Ray MuzzleRightRay;
        private Transform modelTransform;
        public GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashengiturret");
        public GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/impactengiturret");
        public GameObject tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerengiturret");
        public override void OnEnter()
        {
            base.OnEnter();
            this.modelTransform = base.GetModelTransform();

            this.MuzzleLeftRay = base.GetAimRay();
            this.MuzzleRightRay = base.GetAimRay();

            this.duration = this.baseDuration / base.attackSpeedStat;
            if (this.modelTransform)
            {
                ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
                if (component)
                {
                    Transform transform2 = component.FindChild("MuzzleRight");
                    Transform transform = component.FindChild("MuzzleLeft");
                    if (transform)
                    {
                        this.MuzzleLeftRay.origin = transform.position;
                    }
                    if (transform2)
                    {
                        this.MuzzleRightRay.origin = transform2.position;
                    }
                }
            }
            base.AddRecoil(-1f * FireGrenades.recoilAmplitude, -2f * FireGrenades.recoilAmplitude, -1f * FireGrenades.recoilAmplitude, 1f * FireGrenades.recoilAmplitude);
            if (isAuthority)
            {
                if (effectPrefab)
                {
                    EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, "MuzzleLeft", false);
                }
                new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = this.MuzzleLeftRay.origin,
                    aimVector = this.MuzzleLeftRay.direction,
                    minSpread = 0f,
                    maxSpread = base.characterBody.spreadBloomAngle,
                    bulletCount = 5,
                    procCoefficient = 1 / 5f,
                    damage = 3f,
                    force = 10,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    tracerEffectPrefab = tracerEffectPrefab,
                    hitEffectPrefab = hitEffectPrefab,
                    muzzleName = "MuzzleLeft",
                    isCrit = RollCrit(),
                    stopperMask = LayerIndex.world.mask,
                    smartCollision = true,
                    maxDistance = 50f
                }.Fire();
                if (effectPrefab)
                {
                    EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, "MuzzleRight", false);
                }
                new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = this.MuzzleRightRay.origin,
                    aimVector = this.MuzzleRightRay.direction,
                    minSpread = 0f,
                    maxSpread = base.characterBody.spreadBloomAngle,
                    bulletCount = 5,
                    procCoefficient = 1 / 5f,
                    damage = 3f,
                    force = 10,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    tracerEffectPrefab = tracerEffectPrefab,
                    hitEffectPrefab = hitEffectPrefab,
                    muzzleName = "MuzzleRight",
                    isCrit = RollCrit(),
                    stopperMask = LayerIndex.world.mask,
                    smartCollision = true,
                    maxDistance = 50f
                }.Fire();
                base.PlayCrossfade("Gesture Left Cannon, Additive", "FireGrenadeLeft", this.duration);
                base.PlayCrossfade("Gesture Right Cannon, Additive", "FireGrenadeRight", this.duration);
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
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            InterruptPriority result;
            if (this.buttonReleased && base.fixedAge >= this.duration)
            {
                result = InterruptPriority.Any;
            }
            else
            {
                result = InterruptPriority.Skill;
            }
            return result;
        }
    }
}