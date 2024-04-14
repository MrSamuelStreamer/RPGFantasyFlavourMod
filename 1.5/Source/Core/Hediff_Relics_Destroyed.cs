using System.Linq;
using RimWorld;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack;

/**
 * A Hediff where the severity is the number of relics destroyed.
 */
public class Hediff_Relics_Destroyed : Hediff_Relics
{
    public override string Label => "MSSRPG_Common_Hediff_Relics_Destroyed_Label".Translate(Severity);

    public override int UpdateSeverity()
    {
        Severity = pawn.ideo?.Ideo?.GetAllPreceptsOfType<Precept_Relic>()?.Where(r => r.GeneratedRelic is { Destroyed: true, EverSeenByPlayer: true }).Count() ?? 0;
        return (int)Severity;
    }
}