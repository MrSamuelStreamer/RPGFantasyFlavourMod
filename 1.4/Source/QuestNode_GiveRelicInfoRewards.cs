using System.Linq;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack;

public class QuestNode_GiveRelicInfoRewards : QuestNode_GiveRewards
{
    protected override void RunInt()
    {
        base.RunInt();
        if (!ModLister.IdeologyInstalled) return;

        Slate slate = QuestGen.slate;
        Quest quest = QuestGen.quest;
        Precept_Relic relic = slate.Get<Precept_Relic>("relic");
        if (relic == null)
        {
            relic = Faction.OfPlayer.ideos.PrimaryIdeo.GetAllPreceptsOfType<Precept_Relic>().RandomElement();
            Log.Warning("No relic for QuestNode_GiveRelicInfoRewards from parent quest, picking random player relic");
        }

        foreach (QuestPart questPart in QuestGen.quest.PartsListForReading)
        {
            if (questPart is not QuestPart_Choice questPartChoice) continue;
            foreach (var rewards in questPartChoice.choices
                         .Select(choice => choice.rewards)
                         .Where(rewards => rewards.Count > 0))
            {
                rewards.Add(new Reward_RelicInfo
                {
                    relic = relic,
                    quest = quest
                });
            }
        }
    }
}
