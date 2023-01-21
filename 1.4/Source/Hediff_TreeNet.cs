using UnityEngine;
using VanillaPsycastsExpanded;
using Verse;
using Verse.AI;

namespace VPE_Ranger
{
    public class Hediff_TreeNet : Hediff_Overlay
    {
        public override string OverlayPath => "Effects/BindingRope";

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            IntVec3 facingCell = pawn.Rotation.FacingCell;
            int ticksToDisappear = this.TryGetComp<HediffComp_Disappears>().ticksToDisappear;
            Job job = JobMaker.MakeJob(VPE_DefOf.VPE_StandFreeze);
            job.expiryInterval = ticksToDisappear;
            job.overrideFacing = pawn.Rotation;
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            pawn.pather.StopDead();
            pawn.stances.SetStance(new Stance_Stand(ticksToDisappear, facingCell, null));
        }

        public override void Draw()
        {
            Vector3 drawPos = pawn.DrawPos;
            drawPos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
            Matrix4x4 matrix = default(Matrix4x4);
            float num = 1.5f;
            matrix.SetTRS(drawPos, Quaternion.identity, new Vector3(num, 1f, num));
            Graphics.DrawMesh(MeshPool.plane10, matrix, base.OverlayMat, 0, null, 0, MatPropertyBlock);
        }
    }
}
