using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace VPE_Ranger
{
    [HarmonyPatch(typeof(Bullet))]
    [HarmonyPatch("Impact")]
    public static class Bullet_Impact_Spread
    {
        private static void Postfix(ref Thing hitThing, Bullet __instance)
        {
            if (!(__instance?.Launcher is Pawn pawn))
            {
                return;
            }

            if (!pawn.health.hediffSet.HasHediff(VPERanger_DefOf.MakaiRanger_BuffThree)) return;
            HediffComp_ArcingShot comp = pawn.health.hediffSet
                .GetFirstHediffOfDef(VPERanger_DefOf.MakaiRanger_BuffThree).TryGetComp<HediffComp_ArcingShot>();
            if (comp.shotCount <= 0 || comp.isBursted) return;
            List<IntVec3> possibleTargetCell = new List<IntVec3>();
            int count = Mathf.RoundToInt(5 * pawn.GetStatValue(StatDefOf.PsychicSensitivity));
            for (int i = 0; i < count; i++)
            {
                possibleTargetCell.Add(
                    VPERangerUtility.RandomCellAroundCellBase(__instance.intendedTarget.Cell, -5, 5));
            }

            for (int i = 0; i < possibleTargetCell.Count; i++)
            {
                Projectile shrapnelThing = (Projectile)GenSpawn.Spawn(__instance.def, __instance.Position,
                    __instance.Launcher.Map);
                Thing possibleThing = possibleTargetCell[i].GetFirstPawn(__instance.Launcher.Map);
                Thing possibleBuilding = possibleTargetCell[i].GetFirstBuilding(__instance.Launcher.Map);
                if (possibleThing != null)
                {
                    shrapnelThing.Launch(__instance.Launcher, possibleThing, possibleThing,
                        ProjectileHitFlags.IntendedTarget);
                }
                else if (possibleBuilding != null)
                {
                    shrapnelThing.Launch(__instance.Launcher, possibleBuilding, possibleBuilding,
                        ProjectileHitFlags.IntendedTarget);
                }
                else
                {
                    shrapnelThing.Launch(__instance.Launcher, possibleTargetCell[i], possibleTargetCell[i],
                        ProjectileHitFlags.NonTargetPawns);
                }
            }

            __instance.Launch(pawn, __instance.Position.ToVector3(), __instance.intendedTarget,
                __instance.intendedTarget, ProjectileHitFlags.IntendedTarget);
            comp.isBursted = true;
            comp.shotCount--;
        }
    }
}
