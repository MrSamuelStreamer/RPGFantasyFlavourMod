using HarmonyLib;
using Verse;

namespace VPE_Ranger
{
    [StaticConstructorOnStartup]
    public static class VPE_RangerStartPatch
    {
        static VPE_RangerStartPatch()
        {
            new Harmony("FarmerJoe.VPE_Ranger").PatchAll();
            Log.Message("VPE_Ranger Psycast patch successful");
        }
    }
}
