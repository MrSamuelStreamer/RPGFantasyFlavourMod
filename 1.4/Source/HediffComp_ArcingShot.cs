using Verse;

namespace VPE_Ranger
{
    public class HediffComp_ArcingShot : HediffComp
    {
        public int shotCount;

        public bool isTriggered = false;

        public bool isBursted = false;
        public HediffCompProperties_ArcingShot Props => (HediffCompProperties_ArcingShot)props;

        public override string CompLabelInBracketsExtra
        {
            get
            {
                if (shotCount > 0)
                {
                    return base.CompLabelInBracketsExtra + shotCount + " left";
                }

                return base.CompLabelInBracketsExtra;
            }
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref shotCount, "shotCount", 1);
            Scribe_Values.Look(ref isTriggered, "isTriggered", false);
            Scribe_Values.Look(ref isBursted, "isBursted", false);
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            if (Props.shotCount > 0)
            {
                shotCount = Props.shotCount;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (Pawn.IsHashIntervalTick(300) && isTriggered)
            {
                isTriggered = false;
                isBursted = false;
            }

            if (shotCount <= 0)
            {
                Pawn.health.RemoveHediff(parent);
            }
        }
    }
}
