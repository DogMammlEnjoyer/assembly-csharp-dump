using System;
using System.Collections.Generic;
using Meta.XR.Util;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	[Feature(Feature.Interaction)]
	public class OVRInputDeviceActiveState : MonoBehaviour, IActiveState
	{
		public bool Active
		{
			get
			{
				foreach (OVRInput.Controller controller in this._controllerTypes)
				{
					if (OVRInput.GetConnectedControllers() == controller)
					{
						return true;
					}
				}
				return false;
			}
		}

		public void InjectAllOVRInputDeviceActiveState(List<OVRInput.Controller> controllerTypes)
		{
			this.InjectControllerTypes(controllerTypes);
		}

		public void InjectControllerTypes(List<OVRInput.Controller> controllerTypes)
		{
			this._controllerTypes = controllerTypes;
		}

		[SerializeField]
		private List<OVRInput.Controller> _controllerTypes;
	}
}
