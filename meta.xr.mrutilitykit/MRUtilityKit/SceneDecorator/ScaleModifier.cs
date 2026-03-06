using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class ScaleModifier : Modifier
	{
		public override void ApplyModifier(GameObject decorationGO, MRUKAnchor sceneAnchor, SceneDecoration sceneDecoration, Candidate candidate)
		{
			Vector3 localScale = decorationGO.transform.localScale;
			localScale.x *= this.x.mask.SampleMask(candidate, this.x.limitMin, this.x.limitMax, this.x.scale, this.x.offset);
			localScale.y *= this.y.mask.SampleMask(candidate, this.y.limitMin, this.y.limitMax, this.y.scale, this.y.offset);
			localScale.z *= this.z.mask.SampleMask(candidate, this.z.limitMin, this.z.limitMax, this.z.scale, this.z.offset);
			decorationGO.transform.localScale = localScale;
		}

		[SerializeField]
		private ScaleModifier.AxisParameters x = new ScaleModifier.AxisParameters
		{
			limitMin = float.NegativeInfinity,
			limitMax = float.PositiveInfinity,
			scale = 1f
		};

		[SerializeField]
		private ScaleModifier.AxisParameters y = new ScaleModifier.AxisParameters
		{
			limitMin = float.NegativeInfinity,
			limitMax = float.PositiveInfinity,
			scale = 1f
		};

		[SerializeField]
		private ScaleModifier.AxisParameters z = new ScaleModifier.AxisParameters
		{
			limitMin = float.NegativeInfinity,
			limitMax = float.PositiveInfinity,
			scale = 1f
		};

		[Serializable]
		public struct AxisParameters
		{
			[SerializeField]
			public Mask mask;

			[SerializeField]
			public float limitMin;

			[SerializeField]
			public float limitMax;

			[SerializeField]
			public float scale;

			[SerializeField]
			public float offset;
		}
	}
}
