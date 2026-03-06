using System;
using Meta.XR.MRUtilityKit.Extensions;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class CompositeMaskMax : Mask2D
	{
		public override float SampleMask(Candidate c)
		{
			Vector3 vector = Float3X3.Multiply(Mask2D.GenerateAffineTransform(this.offsetX, this.offsetY, this.rotation, this.scaleX, this.scaleY, this.shearX, this.shearY), Vector3Extensions.FromVector2AndZ(c.localPos, 1f));
			vector /= vector.z;
			c.localPos = new Vector2(vector.x, vector.y);
			if (this.maskLayers.Length != 0)
			{
				return float.NegativeInfinity;
			}
			return 0f;
		}

		public override bool Check(Candidate c)
		{
			return true;
		}

		[SerializeField]
		private CompositeMaskAdd.MaskLayer[] maskLayers;
	}
}
