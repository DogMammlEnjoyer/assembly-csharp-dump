using System;

namespace UnityEngine.Rendering
{
	internal struct RangeKey : IEquatable<RangeKey>
	{
		public bool Equals(RangeKey other)
		{
			return this.layer == other.layer && this.renderingLayerMask == other.renderingLayerMask && this.motionMode == other.motionMode && this.shadowCastingMode == other.shadowCastingMode && this.staticShadowCaster == other.staticShadowCaster && this.rendererPriority == other.rendererPriority && this.supportsIndirect == other.supportsIndirect;
		}

		public override int GetHashCode()
		{
			return (int)((((((((MotionVectorGenerationMode)13 * (MotionVectorGenerationMode)23 + (int)this.layer) * (MotionVectorGenerationMode)23 + (int)this.renderingLayerMask) * (MotionVectorGenerationMode)23 + (int)this.motionMode) * (MotionVectorGenerationMode)23 + (int)this.shadowCastingMode) * (MotionVectorGenerationMode)23 + (this.staticShadowCaster ? 1 : 0)) * (MotionVectorGenerationMode)23 + this.rendererPriority) * (MotionVectorGenerationMode)23 + (this.supportsIndirect ? 1 : 0));
		}

		public byte layer;

		public uint renderingLayerMask;

		public MotionVectorGenerationMode motionMode;

		public ShadowCastingMode shadowCastingMode;

		public bool staticShadowCaster;

		public int rendererPriority;

		public bool supportsIndirect;
	}
}
