using System;
using EntityStates.GreaterWispMonster;

namespace EntityStates.ArchWispMonster
{
    public class ChargeSummon : ChargeCannons
    {
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                ArchWispSummon ArchWispSummon = new ArchWispSummon();
                this.outer.SetNextState(ArchWispSummon);
                return;
            }
        }
    }
}
