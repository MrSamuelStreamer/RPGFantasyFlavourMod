using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimQuest;
using RimWorld;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack.RimQuest.HarmonyPatches
{
    [HarmonyPatch(typeof(Main), "UpdateValidQuests")]
    public static class WitcherMonsterHuntAndRimQuestIntegrationPatches
    {
        private static readonly MethodInfo ValidateQuestMethod = AccessTools.Method(typeof(Main), "IsAcceptableQuest",
            new[] { typeof(QuestScriptDef), typeof(bool) });

        [HarmonyPostfix]
        public static void UpdateValidQuestsPostfix(bool saveVanilla)
        {
            var monsterHuntQuestGivers = new[] { "RQ_MedievalQuestGiver", "RQ_TribalQuestGiver" }
                .Select(DefDatabase<QuestGiverDef>.GetNamedSilentFail)
                .Where(q => q != null).ToArray();
            if (monsterHuntQuestGivers.Length == 0) return;

            foreach (QuestScriptDef questScriptDef in DefDatabase<QuestScriptDef>.AllDefsListForReading
                         .OrderBy(Main.GetQuestReadableName).ToList().Where(questScriptDef =>
                             questScriptDef.defName.Contains("_MonsterEncounterQuest")))
            {
                Main.Quests[questScriptDef] =
                    ValidateQuestMethod.Invoke(null, new object[] { questScriptDef, false }) as bool? ?? false;
                if (saveVanilla || !Main.VanillaQuestsValues.ContainsKey(questScriptDef))
                    Main.VanillaQuestsValues[questScriptDef] =
                        ValidateQuestMethod.Invoke(null, new object[] { questScriptDef, true }) as bool? ?? false;

                // Pick quest commonality based of inverse challenge rating i.e. harder => less common
                var commonality = Math.Abs(questScriptDef.defaultChallengeRating * -1 + 4);
                foreach (QuestGiverDef questGiver in monsterHuntQuestGivers)
                {
                    if (questGiver.questsScripts.Exists(q => q.def == questScriptDef)) continue;
                    questGiver.questsScripts.Add(new QuestGenOption(questScriptDef, commonality));
                    Log.Message($"Added {questScriptDef.defName} to {questGiver.defName}");
                }
            }
        }
    }
}
