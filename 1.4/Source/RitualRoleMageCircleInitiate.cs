using RimWorld;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack;



public class RitualRoleMageCircleInitiate : RitualRoleColonist
{
    public override bool AppliesToPawn(
        Pawn p,
        out string reason,
        TargetInfo selectedTarget,
        LordJob_Ritual ritual = null,
        RitualRoleAssignments assignments = null,
        Precept_Ritual precept = null,
        bool skipReason = false)
    {
        if (!base.AppliesToPawn(p, out reason, selectedTarget, ritual, assignments, precept, skipReason))
            return false;

        Precept_Ritual locatedPrecept = precept ??
                                        assignments?.Ritual ?? ritual?.Ritual ??
                                        p.ideo?.Ideo?.GetPrecept(RPGDefOf.MSSRPG_Sorting_Ritual) as Precept_Ritual;
        if (locatedPrecept?.def?.comps?.Find(c => c is PreceptComp_Backstories) is not PreceptComp_Backstories compBackstories)
        {
            if (!skipReason)
                reason = "MSSRPG_MessageRitualRoleNoValidBackstories".Translate((NamedArgument)LabelCap);
            return false;
        }

        if (compBackstories.childOnly && p.ageTracker.Adult)
        {
            if (!skipReason)
                reason = "MSSRPG_MessageRitualRoleMustBeChild".Translate((NamedArgument)(Thing)p);
            return false;
        }

        if (compBackstories.backstories.Contains(p.story.GetBackstory(BackstorySlot.Childhood)))
        {
            if (!skipReason)
                reason = "MSSRPG_MessageRitualRoleMustBeUninitiated".Translate((NamedArgument)(Thing)p);
            return false;
        }

        return true;
    }
}
