using Verse;

namespace VPE_Ranger
{
    public class HediffComp_RicochetArrow : HediffComp
    {
        public int ricochetCountLeft;

        public HediffCompProperties_RicochetArrow Props => (HediffCompProperties_RicochetArrow)props;

        public override string CompLabelInBracketsExtra
        {
            get
            {
                if (ricochetCountLeft > 0)
                {
                    return base.CompLabelInBracketsExtra + ricochetCountLeft + " left";
                }

                return base.CompLabelInBracketsExtra;
            }
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref ricochetCountLeft, "ricochetCountLeft", 1);
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            if (Props.bounceCount > 0)
            {
                ricochetCountLeft = Props.bounceCount;
            }
        }
    }
}
