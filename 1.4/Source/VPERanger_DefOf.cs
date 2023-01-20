using RimWorld;
using Verse;

namespace VPE_Ranger
{
    [DefOf]
    internal static class VPERanger_DefOf
    {
#pragma warning disable CS0649
        public static HediffDef MakaiRanger_BuffTwo;
        public static HediffDef MakaiRanger_BuffThree;
        public static HediffDef MakaiRanger_BuffFour;
        public static HediffDef MakaiRanger_BuffFour_Mark;
        public static HediffDef MakaiRanger_BuffFour_Speed;
#pragma warning restore CS0649

        static VPERanger_DefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(VPERanger_DefOf));
    }
}
