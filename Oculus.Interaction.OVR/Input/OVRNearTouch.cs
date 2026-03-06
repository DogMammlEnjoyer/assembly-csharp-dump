using System;
using Meta.XR.Util;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	[Feature(Feature.Interaction)]
	public class OVRNearTouch : MonoBehaviour, IButton
	{
		public bool Value()
		{
			return OVRInput.Get(this._nearTouch, this._controller);
		}

		[SerializeField]
		private OVRInput.Controller _controller;

		[SerializeField]
		private OVRInput.NearTouch _nearTouch;
	}
}
