using System.Collections.Generic;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack;

public class QuestNode_SaveMonsterDetails : QuestNode
{
    public SlateRef<PawnKindDef> monsterKind;
    public SlateRef<IEnumerable<SitePartDefWithParams>> sitePartsParams;

    protected override bool TestRunInt(Slate slate)
    {
        SetVars(slate);
        return true;
    }

    protected override void RunInt() => SetVars(QuestGen.slate);

    private void SetVars(Slate slate)
    {
        PawnKindDef pawnKindDef = monsterKind.GetValue(slate);
        slate.Set("monsterLabel",
            pawnKindDef.label != null
                ? pawnKindDef.LabelCap.ToString()
                : pawnKindDef.race.label != null
                    ? pawnKindDef.race.LabelCap
                    : "RPGAdventureFlavourPack_FallbackMonsterLabel".TranslateSimple());
        slate.Set("monsterDescription",
            pawnKindDef.description ?? pawnKindDef.race.description ?? "RPGAdventureFlavourPack_FallbackMonsterDescription".TranslateSimple());
        foreach (SitePartDefWithParams sitePart in sitePartsParams.GetValue(slate))
        {
            if (sitePart.def.defName.StartsWith("MSSRPG_SlayerEncounter_"))
            {
                sitePart.parms.animalKind = pawnKindDef;
            }
        }
    }
}
