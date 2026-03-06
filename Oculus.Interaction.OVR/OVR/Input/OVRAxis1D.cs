using System;
using Meta.XR.Util;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.OVR.Input
{
	[Feature(Feature.Interaction)]
	public class OVRAxis1D : MonoBehaviour, IAxis1D
	{
		public float Value()
		{
			float num = OVRInput.Get(this._axis1D, this._controller);
			if (this._remapConfig.Enabled)
			{
				num = this._remapConfig.Curve.Evaluate(num);
			}
			return num;
		}

		[SerializeField]
		private OVRInput.Controller _controller;

		[SerializeField]
		private OVRInput.Axis1D _axis1D;

		[SerializeField]
		private OVRAxis1D.RemapConfig _remapConfig = new OVRAxis1D.RemapConfig
		{
			Enabled = false,
			Curve = AnimationCurve.Linear(0f, 0f, 1f, 1f)
		};

		[Serializable]
		public class RemapConfig
		{
			public bool Enabled;

			public AnimationCurve Curve;
		}
	}
}
