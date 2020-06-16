using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EntityStates;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace CloudBurst.Weapon
{
    public class SpiritLift : BaseState
    {

        public override void OnEnter()
        {
            base.OnEnter();
            Collider[] array = Physics.OverlapBox(base.transform.position, base.transform.position, base.transform.rotation, LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Collide);
            int num = array.Length;

            for (int j = 0; j < num; j++)
            {
                HurtBox component = array[j].GetComponent<HurtBox>();
                if (component)
                {
                }
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

        }

        // Token: 0x06003584 RID: 13700 RVA: 0x000E3840 File Offset: 0x000E1A40
        public override void OnExit()
        {
            base.OnExit();
            
        }
    }
}