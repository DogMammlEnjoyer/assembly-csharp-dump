using System;
using Meta.XR.Util;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.OVR.Input
{
	[Feature(Feature.Interaction)]
	public class OVRAxis2D : MonoBehaviour, IAxis2D
	{
		public Vector2 Value()
		{
			return OVRInput.Get(this._axis2D, this._controller);
		}

		[SerializeField]
		private OVRInput.Controller _controller;

		[SerializeField]
		private OVRInput.Axis2D _axis2D;
	}
}
