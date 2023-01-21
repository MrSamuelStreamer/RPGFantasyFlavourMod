using Verse;

namespace VPE_Ranger
{
    public class HediffCompProperties_RicochetArrow : HediffCompProperties
    {
        public int bounceCount = 5;

        public HediffCompProperties_RicochetArrow()
        {
            compClass = typeof(HediffComp_RicochetArrow);
        }
    }
}
