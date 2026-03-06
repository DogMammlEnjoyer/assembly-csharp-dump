using System;
using Meta.XR.Util;
using UnityEngine;

namespace Oculus.Interaction.OVR.Input
{
	[Feature(Feature.Interaction)]
	public class OVRButtonActiveState : MonoBehaviour, IActiveState
	{
		public bool Active
		{
			get
			{
				return OVRInput.Get(this._button, OVRInput.Controller.Active);
			}
		}

		[SerializeField]
		private OVRInput.Button _button;
	}
}
