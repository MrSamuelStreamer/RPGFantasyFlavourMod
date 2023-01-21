using Verse;

namespace VPE_Ranger
{
    public class HediffCompProperties_DoubleTap : HediffCompProperties
    {
        public ThingDef projectile;

        public HediffCompProperties_DoubleTap()
        {
            compClass = typeof(HediffComp_DoubleTap);
        }
    }
}
