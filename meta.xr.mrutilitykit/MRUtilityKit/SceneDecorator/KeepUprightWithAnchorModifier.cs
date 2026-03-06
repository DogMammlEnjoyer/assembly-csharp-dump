using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class KeepUprightWithAnchorModifier : Modifier
	{
		public override void ApplyModifier(GameObject decorationGO, MRUKAnchor sceneAnchor, SceneDecoration sceneDecoration, Candidate candidate)
		{
			Quaternion quaternion = decorationGO.transform.rotation;
			Vector3 fromDirection = quaternion * this.uprightAxis;
			quaternion *= Quaternion.FromToRotation(fromDirection, sceneAnchor.transform.up);
			decorationGO.transform.rotation = quaternion;
		}

		[SerializeField]
		public Vector3 uprightAxis = new Vector3(0f, 1f, 0f);
	}
}
