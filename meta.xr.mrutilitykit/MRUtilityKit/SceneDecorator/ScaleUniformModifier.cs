using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class ScaleUniformModifier : Modifier
	{
		public override void ApplyModifier(GameObject decorationGO, MRUKAnchor sceneAnchor, SceneDecoration sceneDecoration, Candidate candidate)
		{
			decorationGO.transform.localScale *= this.mask.SampleMask(candidate, this.limitMin, this.limitMax, this.scale, this.offset);
		}

		[SerializeField]
		public Mask mask;

		[SerializeField]
		public float limitMin;

		[SerializeField]
		public float limitMax;

		[SerializeField]
		public float scale = 1f;

		[SerializeField]
		public float offset;
	}
}
