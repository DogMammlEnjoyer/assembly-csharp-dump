using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class ConstantMask : Mask
	{
		public override float SampleMask(Candidate c)
		{
			return this.constant;
		}

		public override bool Check(Candidate c)
		{
			return true;
		}

		[SerializeField]
		public float constant;
	}
}
