using System;
using System.Linq;
using HarmonyLib;
using RimWorld.Planet;

namespace MrSamuelStreamer.RPGAdventureFlavourPack.HarmonyPatches;

[HarmonyPatch(typeof(Caravan), "Label", MethodType.Getter)]
public static class CaravanValue
{
    [HarmonyPostfix]
    public static string Postfix(string current, Caravan __instance)
    {
        return RPGAdventureFlavourPack.Settings.ShowCaravanLoot
            ? $"{current} (Loot: ${Math.Floor(__instance.Goods.Sum(g => g.MarketValue * g.stackCount))})"
            : current;
    }
}
