using CloudBurst.Weapon.EntityStates.ArchWispMonster;
using EntityStates.GreaterWispMonster;

namespace CloudBurst.Weapon.ArchWispMonster.Weapon
{
    public class ChargeSummon : ChargeCannons
    {
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                ArchWispSummoner ArchWispSummon = new ArchWispSummoner();
                this.outer.SetNextState(ArchWispSummon);
                return;
            }
        }
    }
}
