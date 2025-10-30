using System.Collections.Generic;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack;

public class CompProperties_CausesEnrage : CompProperties
{
    public float minRefireDays = 7f;
    public float preventEnrageDist = 10f;
    public List<PawnKindDef> spawnablePawnKinds;

    public CompProperties_CausesEnrage() => compClass = typeof(CompCausesEnrage);
}