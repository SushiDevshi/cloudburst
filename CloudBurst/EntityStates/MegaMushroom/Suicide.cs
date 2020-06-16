using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace CloudBurst.Weapon.MegaMushroom
{
    public class Suicide : BaseState
    {
        public float baseDuration = 0.25f;
        public GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/SporeGrenadeGasImpact");
        public Transform modelTransform;
        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority)
            {
                base.characterBody.baseMoveSpeed = base.characterBody.baseMoveSpeed * 2;
                new BlastAttack
                {
                    attacker = base.gameObject,
                    inflictor = base.gameObject,
                    teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
                    baseDamage = this.damageStat * 2f,
                    baseForce = 60,
                    position = base.modelLocator.transform.position,
                    radius = 10,
                    procCoefficient = 1f,
                    falloffModel = BlastAttack.FalloffModel.Linear,
                    damageType = DamageType.SlowOnHit,
                    //crit = RollCrit()
                }.Fire();
                if (this.hitEffectPrefab)
                {
                    EffectManager.SpawnEffect(this.hitEffectPrefab, new EffectData
                    {
                        origin = base.transform.position,
                        scale = 10
                    }, false);
                }
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > this.baseDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                base.characterBody.baseMoveSpeed = base.characterBody.baseMoveSpeed / 2;
                return;
            }
        }
    }
}