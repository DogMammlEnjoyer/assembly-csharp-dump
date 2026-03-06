using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class CompositeMaskAdd : Mask2D
	{
		public override float SampleMask(Candidate c)
		{
			Vector3 vector = Float3X3.Multiply(Mask2D.GenerateAffineTransform(this.offsetX, this.offsetY, this.rotation, this.scaleX, this.scaleY, this.shearX, this.shearY), new Vector3(c.localPos.x, c.localPos.y, 1f));
			vector /= vector.z;
			c.localPos = new Vector2(vector.x, vector.y);
			float num = 0f;
			foreach (CompositeMaskAdd.MaskLayer maskLayer in this.maskLayers)
			{
				num += maskLayer.SampleMask(c);
			}
			return num;
		}

		public override bool Check(Candidate c)
		{
			return true;
		}

		[SerializeField]
		private CompositeMaskAdd.MaskLayer[] maskLayers;

		[Serializable]
		public struct MaskLayer
		{
			public float SampleMask(Candidate c)
			{
				return this.mask.SampleMask(c, this.outputLimitMin, this.outputLimitMax, this.outputScale, this.outputOffset);
			}

			public Mask mask;

			public float outputScale;

			public float outputLimitMin;

			public float outputLimitMax;

			public float outputOffset;
		}
	}
}
