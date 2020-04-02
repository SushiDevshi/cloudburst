using System;
using System.Collections.ObjectModel;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace CloudBurst.Weapon.EntityStates.GrandParentBoss
{
    public class Offspring : BaseState
    {
        [SerializeField]
        public GameObject SpawnerPodsPrefab = Resources.Load<GameObject>("prefabs/charactermasters/ParentPodMaster");

        public static float randomRadius = 8f;
        public static float maxRange = 9999f;
        private Animator animator;
        private ChildLocator childLocator;
        private Transform modelTransform;
        private float duration;
        public static float baseDuration = 3.5f;
        public static string attackSoundString;
        private float summonInterval;
        private static float summonDuration = 3.26f;
        public static int maxSummonCount = 5;
        private float summonTimer;
        private bool isSummoning;
        private int summonCount;
        public static GameObject spawnEffect;
        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
            this.modelTransform = base.GetModelTransform();
            this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
            this.duration = Offspring.baseDuration;
            Util.PlaySound(Offspring.attackSoundString, base.gameObject);
            this.summonInterval = Offspring.summonDuration / (float)Offspring.maxSummonCount;
            this.isSummoning = true;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (this.isSummoning)
            {
                this.summonTimer += Time.fixedDeltaTime;
                if (NetworkServer.active && this.summonTimer > 0f && this.summonCount < Offspring.maxSummonCount)
                {
                    this.summonCount++;
                    this.summonTimer -= this.summonInterval;
                    this.SpawnPods();
                }
            }
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }
        private void SpawnPods()
        {
            Vector3 point = Vector3.zero;
            Ray aimRay = base.GetAimRay();
            aimRay.origin += UnityEngine.Random.insideUnitSphere * Offspring.randomRadius;
            RaycastHit raycastHit;
            if (Physics.Raycast(aimRay, out raycastHit, (float)LayerIndex.world.mask))
            {
                point = raycastHit.point;
            }
            TeamIndex teamIndex = base.characterBody.GetComponent<TeamComponent>().teamIndex;
            TeamIndex enemyTeam;
            if (teamIndex != TeamIndex.Player)
            {
                if (teamIndex == TeamIndex.Monster)
                {
                    enemyTeam = TeamIndex.Player;
                }
                else
                {
                    enemyTeam = TeamIndex.Neutral;
                }
            }
            else
            {
                enemyTeam = TeamIndex.Monster;
            }
            point = base.transform.position;
            Transform transform = this.FindTargetFarthest(point, enemyTeam);
            if (transform)
            {
                DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscParentPod"), new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                    minDistance = 3f,
                    maxDistance = 20f,
                    spawnOnTarget = transform
                }, RoR2Application.rng);
                directorSpawnRequest.summonerBodyObject = base.gameObject;
                DirectorSpawnRequest directorSpawnRequest2 = directorSpawnRequest;
                directorSpawnRequest2.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest2.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult spawnResult)
                {
                    Inventory inventory = spawnResult.spawnedInstance.GetComponent<CharacterMaster>().inventory;
                    Inventory inventory2 = base.characterBody.inventory;
                    inventory.CopyEquipmentFrom(inventory2);
                }));
                DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                base.PlayAnimation("Body", "SpawnPodWarn", "spawnPodWarn.playbackRate", this.duration);
            }
        }
        private Transform FindTargetFarthest(Vector3 point, TeamIndex enemyTeam)
        {
            ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(enemyTeam);
            float num = 0f;
            Transform result = null;
            for (int i = 0; i < teamMembers.Count; i++)
            {
                float num2 = Vector3.Magnitude(teamMembers[i].transform.position - point);
                if (num2 > num && num2 < Offspring.maxRange)
                {
                    num = num2;
                    result = teamMembers[i].transform;
                }
            }
            return result;
        }
    }
}
