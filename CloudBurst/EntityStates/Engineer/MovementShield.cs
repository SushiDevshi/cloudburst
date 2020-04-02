using EntityStates;
using EntityStates.Engi.EngiBubbleShield;
using EntityStates.Engi.EngiWeapon;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace CloudBurst.Weapon
{
    public class Shield : BaseSkillState
    {
        private GameObject shieldInstance;
        public GameObject projectilePrefab;
        public float damageCoefficient;
        public float force;
        public float baseDuration = 2f;
        public string throwMineSoundString;
        private float duration = 20f;
        public override void OnEnter()
        {
            base.OnEnter();
            this.projectilePrefab = ProjectileCatalog.GetProjectilePrefab(ProjectileCatalog.FindProjectileIndex("EngiBubbleShield"));
            this.projectilePrefab.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
            if (base.isAuthority)
            {
                this.shieldInstance = UnityEngine.Object.Instantiate<GameObject>(this.projectilePrefab);
                ChildLocator component = this.shieldInstance.GetComponent<ChildLocator>();

                if (this.shieldInstance.GetComponent<ChildLocator>())
                {
                    component.FindChild(Deployed.childLocatorString).gameObject.SetActive(true);
                }
            }
        }
        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                EntityState.Destroy(this.shieldInstance.gameObject);
            }
            base.OnExit();
        }
        public override void Update()
        {
            base.Update();
            {
                this.shieldInstance.transform.position = Reflection.GetFieldValue<Vector3>(base.characterMotor.Motor, "_internalTransientPosition");

            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            bool flag = base.fixedAge >= this.duration && base.isAuthority;
            if (flag)
            {
                this.outer.SetNextStateToMain();
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }

    }
}
