using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class CellularNoiseMask : Mask2D
	{
		public override float SampleMask(Candidate c)
		{
			Vector3 vector = Float3X3.Multiply(Mask2D.GenerateAffineTransform(this.offsetX, this.offsetY, this.rotation, this.scaleX, this.scaleY, this.shearX, this.shearY), new Vector3(c.localPos.x, c.localPos.y, 1f));
			vector /= vector.z;
			return Mathf.Abs(WorleyNoise.cellular(new Vector2(vector.x, vector.z)).x);
		}

		public override bool Check(Candidate c)
		{
			return true;
		}
	}
}
