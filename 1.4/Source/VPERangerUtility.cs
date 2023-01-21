using System.Collections.Generic;
using Verse;

namespace VPE_Ranger
{
    public static class VPERangerUtility
    {
        public static IntVec3 RandomCellAroundCellBase(IntVec3 cell, int min, int max)
        {
            cell.x += Rand.RangeInclusive(min, max);
            cell.z += Rand.RangeInclusive(min, max);
            return cell;
        }

        public static List<Pawn> GetNearbyPawnFriendAndFoe(IntVec3 center, Map map, float radius)
        {
            List<Pawn> list = new List<Pawn>();
            float num = radius * radius;
            foreach (Pawn item in map.mapPawns.AllPawnsSpawned)
            {
                if (item.Spawned && !item.Dead)
                {
                    float num2 = item.Position.DistanceToSquared(center);
                    if (num2 <= num)
                    {
                        list.Add(item);
                    }
                }
            }

            return list;
        }
    }
}
