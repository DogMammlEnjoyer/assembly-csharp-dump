using System;
using Meta.XR.Util;
using UnityEngine;

namespace Oculus.Interaction
{
	[Feature(Feature.Interaction)]
	public class OVRControllerMatchesProfileActiveState : MonoBehaviour, IActiveState
	{
		public bool Active
		{
			get
			{
				return OVRInput.GetCurrentInteractionProfile((this._controller.HasFlag(OVRInput.Controller.LTouch) || this._controller.HasFlag(OVRInput.Controller.LHand)) ? OVRInput.Hand.HandLeft : OVRInput.Hand.HandRight) == this._profile;
			}
		}

		public void InjectAllOVRControllerSupportsPressure(OVRInput.Controller controller)
		{
			this._controller = controller;
		}

		[SerializeField]
		private OVRInput.Controller _controller;

		[SerializeField]
		private OVRInput.InteractionProfile _profile;
	}
}
