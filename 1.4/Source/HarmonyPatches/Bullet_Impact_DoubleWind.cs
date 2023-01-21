using HarmonyLib;
using RimWorld;
using Verse;

namespace VPE_Ranger
{
    [HarmonyPatch(typeof(Bullet))]
    [HarmonyPatch("Impact")]
    public class Bullet_Impact_DoubleWind
    {
        public static void Postfix(Thing hitThing, Bullet __instance)
        {
            if (!(__instance?.Launcher is Pawn pawn) ||
                !pawn.health.hediffSet.HasHediff(VPERanger_DefOf.MakaiRanger_BuffFour))
            {
                return;
            }

            if (!(__instance.usedTarget.Thing is Pawn pawnEn)) return;
            if (!pawnEn.health.hediffSet.HasHediff(VPERanger_DefOf.MakaiRanger_BuffFour_Mark) &&
                pawn.health.hediffSet.HasHediff(VPERanger_DefOf.MakaiRanger_BuffFour_Speed))
            {
                pawn.health.RemoveHediff(
                    pawn.health.hediffSet.GetFirstHediffOfDef(VPERanger_DefOf.MakaiRanger_BuffFour_Speed));
            }

            pawnEn.health.AddHediff(VPERanger_DefOf.MakaiRanger_BuffFour_Mark);
            if (pawn.health.hediffSet.HasHediff(VPERanger_DefOf.MakaiRanger_BuffFour_Speed))
            {
                pawn.health.hediffSet.GetFirstHediffOfDef(VPERanger_DefOf.MakaiRanger_BuffFour_Speed).Severity +=
                    0.5f;
                pawn.health.hediffSet.GetFirstHediffOfDef(VPERanger_DefOf.MakaiRanger_BuffFour_Speed)
                    .TryGetComp<HediffComp_Disappears>().ticksToDisappear = 625;
            }
            else
            {
                Hediff hediff = HediffMaker.MakeHediff(VPERanger_DefOf.MakaiRanger_BuffFour_Speed, pawn);
                hediff.Severity = 1;
                pawn.health.AddHediff(hediff);
            }
        }
    }
}
