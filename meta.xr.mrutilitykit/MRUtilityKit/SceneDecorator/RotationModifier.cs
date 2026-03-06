using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class RotationModifier : Modifier
	{
		public override void ApplyModifier(GameObject decorationGO, MRUKAnchor sceneAnchor, SceneDecoration sceneDecoration, Candidate candidate)
		{
			Vector3 axis = (this.localSpace ? decorationGO.transform.rotation : Quaternion.identity) * this.rotationAxis;
			decorationGO.transform.rotation *= Quaternion.AngleAxis(this.mask.SampleMask(candidate, this.limitMin, this.limitMax, this.scale, this.offset), axis);
		}

		[SerializeField]
		public Mask mask;

		[SerializeField]
		public float limitMin = float.NegativeInfinity;

		[SerializeField]
		public float limitMax = float.PositiveInfinity;

		[SerializeField]
		public float scale = 1f;

		[SerializeField]
		public float offset;

		[SerializeField]
		public Vector3 rotationAxis = new Vector3(0f, 1f, 0f);

		[SerializeField]
		public bool localSpace;
	}
}
