using System;
using CloudBurst.Equipment;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.GreaterWispMonster
{
    public class ArchWispSummon : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Ray aimRay = base.GetAimRay();
            string text = "MuzzleLeft";
            string text2 = "MuzzleRight";
            this.duration = this.baseDuration / this.attackSpeedStat;
            if (this.effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(this.effectPrefab, base.gameObject, text, false);
                EffectManager.SimpleMuzzleFlash(this.effectPrefab, base.gameObject, text2, false);
            }
            base.PlayAnimation("Gesture", "FireCannons", "FireCannons.playbackRate", this.duration);
            if (base.isAuthority && base.modelLocator && base.modelLocator.modelTransform)
            {
                ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
                if (component)
                {
                    int childIndex = component.FindChildIndex(text);
                    int childIndex2 = component.FindChildIndex(text2);
                    Transform transform = component.FindChild(childIndex);
                    Transform transform2 = component.FindChild(childIndex2);
                    if (transform)
                    {
                        CharacterBody ccomponent = base.outer.commonComponents.characterBody;
                        GameObject gameObject = MasterCatalog.FindMasterPrefab("WispMaster");
                        GameObject bodyPrefab = BodyCatalog.FindBodyPrefab("WispBody");
                        var master = ccomponent.master;
                        GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, transform.position, transform.rotation);
                        CharacterMaster component2 = gameObject2.GetComponent<CharacterMaster>();

                        component2.teamIndex = TeamComponent.GetObjectTeam(ccomponent.gameObject);
                        AIOwnership component4 = gameObject2.GetComponent<AIOwnership>();
                        BaseAI component5 = gameObject2.GetComponent<BaseAI>();
                        if (component4)
                        {
                            component4.ownerMaster = master;
                        }
                        if (component5)
                        {
                            component5.leader.gameObject = master.gameObject;
                            component5.isHealer = false;
                            component5.fullVision = true;
                        }
                        Inventory component6 = gameObject2.GetComponent<Inventory>();
                        component6.CopyItemsFrom(master.inventory);
                        NetworkServer.Spawn(gameObject2);
                        CharacterBody body = component2.SpawnBody(bodyPrefab, transform2.position, transform2.rotation);
                    }
                    if (transform2)
                    {
                        CharacterBody ccomponent = base.outer.commonComponents.characterBody;
                        GameObject gameObject = MasterCatalog.FindMasterPrefab("WispMaster");
                        GameObject bodyPrefab = BodyCatalog.FindBodyPrefab("WispBody");
                        var master = ccomponent.master;
                        GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, transform.position, transform.rotation);
                        CharacterMaster component2 = gameObject2.GetComponent<CharacterMaster>();

                        component2.teamIndex = TeamComponent.GetObjectTeam(ccomponent.gameObject);
                        AIOwnership component4 = gameObject2.GetComponent<AIOwnership>();
                        BaseAI component5 = gameObject2.GetComponent<BaseAI>();
                        if (component4)
                        {
                            component4.ownerMaster = master;
                        }
                        if (component5)
                        {
                            component5.leader.gameObject = master.gameObject;
                            component5.isHealer = false;
                            component5.fullVision = true;
                        }
                        Inventory component6 = gameObject2.GetComponent<Inventory>();
                        component6.CopyItemsFrom(master.inventory);
                        NetworkServer.Spawn(gameObject2);
                        CharacterBody body = component2.SpawnBody(bodyPrefab, transform2.position, transform2.rotation);
                    }
                }
            }
        }
        public override void OnExit()
        {
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
        public GameObject effectPrefab;

        public float baseDuration = 2f;

        private float duration;
    }
}