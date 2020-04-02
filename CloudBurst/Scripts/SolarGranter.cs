using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace CloudBurst
{
    public class SolarGranter : MonoBehaviour
    {
        public Transform referenceTransform;
        public float raycastFrequency;
        public BuffIndex buffIndex;
        private const float raycastLength = 1000f;
        private float timer;
        private bool hadSunlight;
        private Light sun;
        private CharacterBody characterBody;

        private void Start()
        {
            this.FindSun();
            this.characterBody = base.GetComponent<CharacterBody>();
        }
        private void FixedUpdate()
        {
            this.timer -= Time.fixedDeltaTime;
            if (this.timer <= 0f)
            {
                this.timer = 1f / this.raycastFrequency;
                this.CheckSunlight();
            }
        }
        private void FindSun()
        {
            this.sun = RenderSettings.sun;
        }
        private void CheckSunlight()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            if (!this.sun)
            {
                this.FindSun();
                if (!this.sun)
                {
                    return;
                }
            }
            bool flag = !Physics.Raycast(base.transform.position, -this.sun.transform.forward, 1000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
            if (this.hadSunlight && !flag)
            {
                this.characterBody.RemoveBuff(this.buffIndex);
            }
            else if (flag && !this.hadSunlight)
            {
                this.characterBody.AddBuff(this.buffIndex);
            }
            this.hadSunlight = flag;
        }
    }
}
