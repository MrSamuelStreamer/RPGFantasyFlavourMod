using System.Linq;
using RimWorld;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack;

/**
 * A Hediff where the severity is the number of relics collected.
 */
public class Hediff_Relics : HediffWithComps
{
    public override string Label => "MSSRPG_Common_Hediff_Relics_Collected_Label".Translate(Severity);

    public override void Tick()
    {
        base.Tick();
        if (pawn.IsHashIntervalTick(2123)) UpdateSeverity();
    }

    public override void PostMake()
    {
        base.PostMake();
        UpdateSeverity();
    }

    public virtual int UpdateSeverity()
    {
        Severity = pawn.ideo?.Ideo?.GetAllPreceptsOfType<Precept_Relic>()?.Where(r => r.RelicInPlayerPossession).Count() ?? 0;
        return (int)Severity;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref severityInt, "severity");
    }
}
