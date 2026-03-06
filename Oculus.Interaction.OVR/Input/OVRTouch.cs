using System;
using Meta.XR.Util;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	[Feature(Feature.Interaction)]
	public class OVRTouch : MonoBehaviour, IButton
	{
		public bool Value()
		{
			return OVRInput.Get(this._touch, this._controller);
		}

		[SerializeField]
		private OVRInput.Controller _controller;

		[SerializeField]
		private OVRInput.Touch _touch;
	}
}
