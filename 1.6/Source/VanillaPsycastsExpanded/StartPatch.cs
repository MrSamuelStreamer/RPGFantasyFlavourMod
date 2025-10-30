using HarmonyLib;
using Verse;

namespace MSSRPG_VPE
{
    [StaticConstructorOnStartup]
    public static class StartPatch
    {
        static StartPatch()
        {
            new Harmony("MSSRPG_Psycast_Patch").PatchAll();
        }
    }
}
