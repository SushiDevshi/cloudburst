using EntityStates;

namespace CloudBurst.Weapon.EntityStates.GrandParentBoss
{
    public class GroundSwipe : BasicMeleeAttack
    {
        protected override void PlayAnimation()
        {
            base.PlayCrossfade("Body", "MeleeSwing", "meleeSwing.playbackRate", this.duration, 1f);
        }
    }
}
