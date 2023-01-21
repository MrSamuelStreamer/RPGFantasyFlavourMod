using Verse;

namespace VPE_Ranger
{
    public class HediffCompProperties_ArcingShot : HediffCompProperties
    {
        public int shotCount = 5;

        public HediffCompProperties_ArcingShot()
        {
            compClass = typeof(HediffComp_ArcingShot);
        }
    }
}
