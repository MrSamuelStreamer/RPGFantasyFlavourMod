using RimWorld;

namespace MrSamuelStreamer.RPGAdventureFlavourPack;

[DefOf]
public class RPGDefOf
{
    [MayRequireIdeology] public static PreceptDef MSSRPG_Sorting_Ritual;

    static RPGDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(RPGDefOf));
}
