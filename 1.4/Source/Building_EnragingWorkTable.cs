using RimWorld;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack;

public class Building_EnragingWorkTable : Building_WorkTable
{
    public CompCausesEnrage enrageComp;

    public override void UsedThisTick()
    {
        base.UsedThisTick();
        enrageComp?.Notify_UsedThisTick();
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        enrageComp = GetComp<CompCausesEnrage>();
    }
}
