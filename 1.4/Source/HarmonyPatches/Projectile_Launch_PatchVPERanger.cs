using System;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace VPE_Ranger
{
    [HarmonyPatch(typeof(Projectile), "Launch", new Type[]
    {
        typeof(Thing),
        typeof(Vector3),
        typeof(LocalTargetInfo),
        typeof(LocalTargetInfo),
        typeof(ProjectileHitFlags),
        typeof(bool),
        typeof(Thing),
        typeof(ThingDef)
    })]
    public static class Projectile_Launch_PatchVPERanger
    {
        private static readonly AccessTools.FieldRef<Projectile, Vector3> destination =
            AccessTools.FieldRefAccess<Projectile, Vector3>("destination");

        private static void Postfix(Projectile __instance, Thing launcher, Vector3 origin,
            ref LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, bool preventFriendlyFire, Thing equipment,
            ThingDef targetCoverDef)
        {
            if (!(__instance?.Launcher is Pawn instigator) ||
                !instigator.health.hediffSet.HasHediff(VPERanger_DefOf.MakaiRanger_BuffThree))
            {
                return;
            }

            HediffComp_ArcingShot comp = instigator.health.hediffSet
                .GetFirstHediffOfDef(VPERanger_DefOf.MakaiRanger_BuffThree).TryGetComp<HediffComp_ArcingShot>();
            if (comp.shotCount <= 0 || comp.isTriggered)
            {
                return;
            }

            destination(__instance) = new Vector3(destination(__instance).x, destination(__instance).y,
                destination(__instance).z + 7);
            comp.isTriggered = true;
        }
    }
}
