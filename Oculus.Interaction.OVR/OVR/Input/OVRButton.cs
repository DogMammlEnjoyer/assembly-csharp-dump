using System;
using Meta.XR.Util;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.OVR.Input
{
	[Feature(Feature.Interaction)]
	public class OVRButton : MonoBehaviour, IButton
	{
		public bool Value()
		{
			return OVRInput.Get(this._button, this._controller);
		}

		[SerializeField]
		private OVRInput.Controller _controller;

		[SerializeField]
		private OVRInput.Button _button;
	}
}
