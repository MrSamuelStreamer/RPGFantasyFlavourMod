using RimWorld;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack;

public class RitualStageAction_PickBackstory : RitualStageAction
{
    [MustTranslate] public string text;
    public MessageTypeDef messageTypeDef;
    public bool adultBackstory;

    public override void Apply(LordJob_Ritual ritual)
    {
        PreceptComp_Backstories preceptCompBackstories = ritual.Ritual.def.comps.Find(c => c is PreceptComp_Backstories) as PreceptComp_Backstories;
        BackstoryDef newBackstory = preceptCompBackstories?.backstories?.RandomElement();
        if (newBackstory == null) return;
        Pawn initiate = ritual.PawnWithRole("initiate");
        if (adultBackstory)
        {
            initiate.story.Adulthood = newBackstory;
        }
        else
        {
            initiate.story.Childhood = newBackstory;
        }

        Messages.Message(text.Formatted((NamedArgument)ritual.Ritual.Label, (NamedArgument)initiate.Name.ToStringShort, newBackstory.titleShort).CapitalizeFirst(),
            ritual.selectedTarget, messageTypeDef, false);
    }

    public override void ExposeData()
    {
        Scribe_Defs.Look(ref messageTypeDef, "messageTypeDef");
        Scribe_Values.Look(ref text, "text");
        Scribe_Values.Look(ref adultBackstory, "childBackstory");
    }
}
