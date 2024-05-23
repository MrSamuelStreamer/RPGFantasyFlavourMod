using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;
using RimQuest;
using RimWorld;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack.RimQuest.HarmonyPatches
{
    [HarmonyPatch(typeof(Main), "UpdateValidQuests")]
    public static class RimquestExtraQuestsPatch
    {
        private static readonly MethodInfo ValidateQuestMethod = AccessTools.Method(typeof(Main), "IsAcceptableQuest",
            [typeof(QuestScriptDef), typeof(bool)]);

        [HarmonyPostfix]
        public static void UpdateValidQuestsPostfix(bool saveVanilla)
        {
            if (!RPGAdventureFlavourPack.Settings.AddExtraRimQuests) return;
            QuestGiverDef[] monsterHuntQuestGivers = RPGAdventureFlavourPack.Settings.ExtraRimQuestGivers()
                .Select(DefDatabase<QuestGiverDef>.GetNamedSilentFail)
                .Where(q => q != null).ToArray();
            if (monsterHuntQuestGivers.Length == 0) return;

            List<string> logMessage = [];
            foreach (QuestScriptDef questScriptDef in DefDatabase<QuestScriptDef>.AllDefsListForReading
                         .OrderBy(Main.GetQuestReadableName)
                         .Where(questScriptDef =>
                             RPGAdventureFlavourPack.Settings.ExtraRimQuestsMatching().Any(f =>
                                 questScriptDef.defName.Contains(f))))
            {
                Main.Quests[questScriptDef] =
                    ValidateQuestMethod.Invoke(null, [questScriptDef, false]) as bool? ?? false;
                if (saveVanilla || !Main.VanillaQuestsValues.ContainsKey(questScriptDef))
                    Main.VanillaQuestsValues[questScriptDef] =
                        ValidateQuestMethod.Invoke(null, [questScriptDef, true]) as bool? ?? false;

                // Pick quest commonality based of inverse challenge rating i.e. harder => less common
                int commonality = questScriptDef.defaultChallengeRating <= 0
                    ? 2 // Default is -1 so when unsure we just pick 2 and hope that's fine
                    : Math.Abs(questScriptDef.defaultChallengeRating * -1 + 4);
                foreach (QuestGiverDef questGiver in monsterHuntQuestGivers)
                {
                    if (questGiver.questsScripts.Exists(q => q.def == questScriptDef)) continue;
                    questGiver.questsScripts.Add(new QuestGenOption(questScriptDef, commonality));
                    logMessage.Add($"Added {questScriptDef.defName} to {questGiver.defName}");
                }
            }

            if (logMessage.Count > 0) Log.Message(string.Join(", ", logMessage.ToStringSafeEnumerable()));
        }
    }

    [HarmonyPatch(typeof(Main), "GetQuestReadableName")]
    public static class RimquestExtraQuestsNamingPatches
    {
        [HarmonyPostfix]
        public static void GetQuestReadableName(ref string __result, QuestScriptDef questScriptDef)
        {
            __result = __result == "Opportunity Site"
                ? Regex.Replace(questScriptDef.defName.Replace("_", " "), "(\\B[A-Z])", " $1")
                : __result;
        }
    }
}
