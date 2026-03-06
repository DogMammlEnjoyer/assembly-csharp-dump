using System;
using Unity.Burst;

namespace UnityEngine.Animations.Rigging
{
	[BurstCompile]
	public struct OverrideTransformJob : IWeightedAnimationJob, IAnimationJob
	{
		public FloatProperty jobWeight { readonly get; set; }

		public void ProcessRootMotion(AnimationStream stream)
		{
		}

		public void ProcessAnimation(AnimationStream stream)
		{
			float num = this.jobWeight.Get(stream);
			if (num <= 0f)
			{
				AnimationRuntimeUtils.PassThrough(stream, this.driven);
				return;
			}
			AffineTransform affineTransform;
			if (this.source.IsValid(stream))
			{
				Vector3 t;
				Quaternion r;
				Vector3 vector;
				this.source.GetLocalTRS(stream, out t, out r, out vector);
				AffineTransform rhs = new AffineTransform(t, r);
				Quaternion rhs2 = this.cache.Get<Quaternion>(this.sourceToCurrSpaceRotIdx, 0);
				affineTransform = Quaternion.Inverse(rhs2) * (this.sourceInvLocalBindTx * rhs) * rhs2;
			}
			else
			{
				affineTransform = new AffineTransform(this.position.Get(stream), Quaternion.Euler(this.rotation.Get(stream)));
			}
			OverrideTransformJob.Space space = (OverrideTransformJob.Space)this.cache.GetRaw(this.spaceIdx, 0);
			float t2 = this.positionWeight.Get(stream) * num;
			float t3 = this.rotationWeight.Get(stream) * num;
			switch (space)
			{
			case OverrideTransformJob.Space.World:
			{
				Vector3 a;
				Quaternion a2;
				this.driven.GetGlobalTR(stream, out a, out a2);
				this.driven.SetGlobalTR(stream, Vector3.Lerp(a, affineTransform.translation, t2), Quaternion.Lerp(a2, affineTransform.rotation, t3), false);
				return;
			}
			case OverrideTransformJob.Space.Local:
			{
				Vector3 a3;
				Quaternion a4;
				Vector3 scale;
				this.driven.GetLocalTRS(stream, out a3, out a4, out scale);
				this.driven.SetLocalTRS(stream, Vector3.Lerp(a3, affineTransform.translation, t2), Quaternion.Lerp(a4, affineTransform.rotation, t3), scale, false);
				return;
			}
			case OverrideTransformJob.Space.Pivot:
			{
				Vector3 t4;
				Quaternion r2;
				Vector3 scale2;
				this.driven.GetLocalTRS(stream, out t4, out r2, out scale2);
				AffineTransform affineTransform2 = new AffineTransform(t4, r2);
				affineTransform = affineTransform2 * affineTransform;
				this.driven.SetLocalTRS(stream, Vector3.Lerp(affineTransform2.translation, affineTransform.translation, t2), Quaternion.Lerp(affineTransform2.rotation, affineTransform.rotation, t3), scale2, false);
				return;
			}
			default:
				return;
			}
		}

		internal void UpdateSpace(int space)
		{
			if ((int)this.cache.GetRaw(this.spaceIdx, 0) == space)
			{
				return;
			}
			this.cache.SetRaw((float)space, this.spaceIdx, 0);
			if (space == 2)
			{
				this.cache.Set<Quaternion>(this.sourceToPivotRot, this.sourceToCurrSpaceRotIdx, 0);
				return;
			}
			if (space == 1)
			{
				this.cache.Set<Quaternion>(this.sourceToLocalRot, this.sourceToCurrSpaceRotIdx, 0);
				return;
			}
			this.cache.Set<Quaternion>(this.sourceToWorldRot, this.sourceToCurrSpaceRotIdx, 0);
		}

		public ReadWriteTransformHandle driven;

		public ReadOnlyTransformHandle source;

		public AffineTransform sourceInvLocalBindTx;

		public Quaternion sourceToWorldRot;

		public Quaternion sourceToLocalRot;

		public Quaternion sourceToPivotRot;

		public CacheIndex spaceIdx;

		public CacheIndex sourceToCurrSpaceRotIdx;

		public Vector3Property position;

		public Vector3Property rotation;

		public FloatProperty positionWeight;

		public FloatProperty rotationWeight;

		public AnimationJobCache cache;

		public enum Space
		{
			World,
			Local,
			Pivot
		}
	}
}
