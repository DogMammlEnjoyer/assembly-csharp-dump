using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class AnchorComponentDistanceMask : Mask
	{
		public override float SampleMask(Candidate c)
		{
			return c.anchorCompDists[(int)this.axis];
		}

		public override bool Check(Candidate c)
		{
			return true;
		}

		[SerializeField]
		public AnchorComponentDistanceMask.Axis axis;

		public enum Axis
		{
			X,
			Y,
			Z
		}
	}
}
