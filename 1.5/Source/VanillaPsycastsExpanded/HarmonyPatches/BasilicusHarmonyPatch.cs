using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using VanillaPsycastsExpanded;
using Verse;

namespace MSSRPG_VPE.HarmonyPatches
{
    [HarmonyPatch(typeof(PawnGen_Patch), "Postfix")]
    [HarmonyBefore("OskarPotocki.VanillaPsycastsExpanded")]
    public static class BasilicusHarmonyPatch
    {
        private static readonly FieldInfo BasilicusFieldInfo = AccessTools.Field(typeof(VPE_DefOf), nameof(VPE_DefOf.VPE_Basilicus));

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var edited = false;
            var bufferedInstructions = instructions.ToArray();

            for (var instructionIndex = 0; instructionIndex < bufferedInstructions.Length; instructionIndex++)
            {
                CodeInstruction instruction = bufferedInstructions[instructionIndex];
                if (instruction.opcode == OpCodes.Bne_Un_S &&
                    bufferedInstructions[instructionIndex - 1].opcode == OpCodes.Ldsfld &&
                    bufferedInstructions[instructionIndex - 1].operand is FieldInfo fieldInfoBasilicus &&
                    fieldInfoBasilicus == BasilicusFieldInfo)
                {
                    while (bufferedInstructions[instructionIndex].opcode != OpCodes.Call && instructionIndex > 0)
                    {
                        bufferedInstructions[instructionIndex].opcode = OpCodes.Nop;
                        instructionIndex--;
                    }
                    bufferedInstructions[instructionIndex].opcode = OpCodes.Nop;
                    edited = true;
                    Log.Message("Patched PsyCaster pawn spawning to not require Basilicus Bestower");
                    break;
                }
            }

            if (!edited)
            {
                Log.Warning("Failed to Patch PsyCaster pawn spawning to not require Basilicus Bestower");
            }

            return bufferedInstructions.AsEnumerable();
        }
    }
}
