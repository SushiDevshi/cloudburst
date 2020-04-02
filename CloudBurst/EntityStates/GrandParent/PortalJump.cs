using System;
using System.Linq;
using EntityStates;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace CloudBurst.EntityStates.GrandParentBoss
{
    public class PortalJump : BaseState
    {
        private BullseyeSearch enemyFinder;
        public static float duration = 3f;
        public static float retreatDuration = 2.433f;
        public static float emergeDuration = 2.933f;
        public static float portalScaleDuration = 2f;
        public static float effectsDuration = 2f;
        private bool retreatDone;
        private bool teleported;
        private bool canMoveDuringTeleport;
        private bool hasEmerged;
        private HurtBox foundBullseye;
        public static float telezoneRadius;
        public static float skillDistance = 2000f;
        private float stopwatch;
        private Vector3 destinationPressence = Vector3.zero;
        private Vector3 startPressence = Vector3.zero;
        private Transform modelTransform;
        private Animator animator;
        private CharacterModel characterModel;
        private HurtBoxGroup hurtboxGroup;
        public static GameObject jumpInEffectPrefab;
        public static GameObject jumpOutEffectPrefab;
        public static Vector3 teleportOffset;
        private GrandparentEnergyFXController FXController;
        public override void OnEnter()
        {
            base.OnEnter();
            this.modelTransform = base.GetModelTransform();
            if (this.modelTransform)
            {
                this.animator = this.modelTransform.GetComponent<Animator>();
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
                this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
                base.PlayAnimation("Body", "Retreat", "retreat.playbackRate", PortalJump.retreatDuration);
                EffectData effectData = new EffectData
                {
                    origin = base.characterBody.modelLocator.modelTransform.GetComponent<ChildLocator>().FindChild("Portal").position
                };
                EffectManager.SpawnEffect(PortalJump.jumpInEffectPrefab, effectData, true);
            }
            this.FXController = base.characterBody.GetComponent<GrandparentEnergyFXController>();
            if (this.FXController)
            {
                this.FXController.portalObject = base.characterBody.modelLocator.modelTransform.GetComponent<ChildLocator>().FindChild("Portal").GetComponentInChildren<EffectComponent>().gameObject;
            }
        }

        // Token: 0x06003575 RID: 13685 RVA: 0x000E015C File Offset: 0x000DE35C
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;
            if (this.stopwatch >= PortalJump.retreatDuration && !this.retreatDone)
            {
                this.retreatDone = true;
                if (this.FXController)
                {
                    this.ScaleObject(this.FXController.portalObject, false);
                }
            }
            if (this.stopwatch >= PortalJump.retreatDuration + PortalJump.portalScaleDuration && !this.teleported)
            {
                this.teleported = true;
                this.canMoveDuringTeleport = true;
                if (this.FXController)
                {
                    this.FXController.portalObject.GetComponent<ObjectScaleCurve>().enabled = false;
                }
                this.DoTeleport();
            }
            if (base.characterMotor && base.characterDirection)
            {
                base.characterMotor.velocity = Vector3.zero;
            }
            if (this.canMoveDuringTeleport)
            {
                this.SetPosition(Vector3.Lerp(this.startPressence, this.destinationPressence, this.stopwatch / PortalJump.duration));
            }
            if (this.stopwatch >= PortalJump.retreatDuration + PortalJump.portalScaleDuration + PortalJump.duration && this.canMoveDuringTeleport)
            {
                this.canMoveDuringTeleport = false;
                if (this.FXController)
                {
                    this.FXController.portalObject.transform.position = base.characterBody.modelLocator.modelTransform.GetComponent<ChildLocator>().FindChild("Portal").position;
                    this.ScaleObject(this.FXController.portalObject, true);
                }
            }
            if (this.stopwatch >= PortalJump.retreatDuration + PortalJump.portalScaleDuration * 2f + PortalJump.duration && !this.hasEmerged)
            {
                this.hasEmerged = true;
                if (this.FXController)
                {
                    this.FXController.portalObject.GetComponent<ObjectScaleCurve>().enabled = false;
                }
                this.modelTransform = base.GetModelTransform();
                if (this.modelTransform)
                {
                    base.PlayAnimation("Body", "Emerge", "emerge.playbackRate", PortalJump.duration);
                    EffectData effectData = new EffectData
                    {
                        origin = base.characterBody.modelLocator.modelTransform.GetComponent<ChildLocator>().FindChild("Portal").position
                    };
                    EffectManager.SpawnEffect(PortalJump.jumpOutEffectPrefab, effectData, true);
                    if (this.characterModel)
                    {
                        this.characterModel.invisibilityCount--;
                    }
                    if (this.hurtboxGroup)
                    {
                        HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                        int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                        hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
                    }
                    if (base.characterMotor)
                    {
                        base.characterMotor.enabled = true;
                    }
                }
            }
            if (this.stopwatch >= PortalJump.retreatDuration + PortalJump.portalScaleDuration * 2f + PortalJump.duration + PortalJump.emergeDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        // Token: 0x06003576 RID: 13686 RVA: 0x000E0440 File Offset: 0x000DE640
        private void DoTeleport()
        {
            Ray aimRay = base.GetAimRay();
            this.enemyFinder = new BullseyeSearch();
            this.enemyFinder.maxDistanceFilter = PortalJump.skillDistance;
            this.enemyFinder.searchOrigin = aimRay.origin;
            this.enemyFinder.searchDirection = aimRay.direction;
            this.enemyFinder.filterByLoS = false;
            this.enemyFinder.sortMode = BullseyeSearch.SortMode.Distance;
            this.enemyFinder.teamMaskFilter = TeamMask.allButNeutral;
            if (base.teamComponent)
            {
                this.enemyFinder.teamMaskFilter.RemoveTeam(base.teamComponent.teamIndex);
            }
            this.enemyFinder.RefreshCandidates();
            this.foundBullseye = this.enemyFinder.GetResults().LastOrDefault<HurtBox>();
            this.modelTransform = base.GetModelTransform();
            if (this.characterModel)
            {
                this.characterModel.invisibilityCount++;
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
            if (base.characterMotor)
            {
                base.characterMotor.enabled = false;
            }
            Vector3 b = base.inputBank.moveVector * PortalJump.skillDistance;
            this.destinationPressence = base.transform.position;
            this.startPressence = base.transform.position;
            NodeGraph groundNodes = SceneInfo.instance.groundNodes;
            Vector3 position = this.startPressence + b;
            if (this.foundBullseye)
            {
                position = this.foundBullseye.transform.position;
            }
            NodeGraph.NodeIndex nodeIndex = groundNodes.FindClosestNode(position, base.characterBody.hullClassification);
            groundNodes.GetNodePosition(nodeIndex, out this.destinationPressence);
            this.destinationPressence += base.transform.position - base.characterBody.footPosition;
        }

        // Token: 0x06003577 RID: 13687 RVA: 0x000CDA76 File Offset: 0x000CBC76
        private void SetPosition(Vector3 newPosition)
        {
            if (base.characterMotor)
            {
                base.characterMotor.Motor.SetPositionAndRotation(newPosition, Quaternion.identity, true);
            }
        }

        // Token: 0x06003578 RID: 13688 RVA: 0x000E0628 File Offset: 0x000DE828
        private void ScaleObject(GameObject objectToScaleDown, bool scaleUp)
        {
            float valueEnd = scaleUp ? 1f : 0f;
            float valueStart = scaleUp ? 0f : 1f;
            ObjectScaleCurve component = objectToScaleDown.GetComponent<ObjectScaleCurve>();
            component.timeMax = PortalJump.portalScaleDuration;
            component.curveX = AnimationCurve.Linear(0f, valueStart, 1f, valueEnd);
            component.curveY = AnimationCurve.Linear(0f, valueStart, 1f, valueEnd);
            component.curveZ = AnimationCurve.Linear(0f, valueStart, 1f, valueEnd);
            component.overallCurve = AnimationCurve.EaseInOut(0f, valueStart, 1f, valueEnd);
            component.enabled = true;
        }

        // Token: 0x06003579 RID: 13689 RVA: 0x0002FF1F File Offset: 0x0002E11F
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
