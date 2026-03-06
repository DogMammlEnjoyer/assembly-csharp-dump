using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;

namespace Unity.XR.OpenVR
{
	internal static class InputLayoutLoader
	{
		static InputLayoutLoader()
		{
			InputLayoutLoader.RegisterInputLayouts();
		}

		public static void RegisterInputLayouts()
		{
			string name = "OpenVRHMD";
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout<XRHMD>(name, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(OpenVR Headset)|^(Vive Pro)", true)));
			string name2 = "OpenVRControllerWMR";
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout<XRController>(name2, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(OpenVR Controller\\(WindowsMR)", true)));
			string name3 = "ViveWand";
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			inputDeviceMatcher = inputDeviceMatcher.WithManufacturer("HTC", true);
			InputSystem.RegisterLayout<XRController>(name3, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(OpenVR Controller\\(((Vive Controller)|(VIVE Controller)))", true)));
			string name4 = "OpenVRViveCosmosController";
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			inputDeviceMatcher = inputDeviceMatcher.WithManufacturer("HTC", true);
			InputSystem.RegisterLayout<XRController>(name4, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(OpenVR Controller\\(((VIVE Cosmos Controller)|(Vive Cosmos Controller)|(vive_cosmos_controller)))", true)));
			string name5 = "OpenVRControllerIndex";
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			inputDeviceMatcher = inputDeviceMatcher.WithManufacturer("Valve", true);
			InputSystem.RegisterLayout<XRController>(name5, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(OpenVR Controller\\(Knuckles)", true)));
			string name6 = "OpenVROculusTouchController";
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			inputDeviceMatcher = inputDeviceMatcher.WithManufacturer("Oculus", true);
			InputSystem.RegisterLayout<XRController>(name6, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(OpenVR Controller\\(Oculus)", true)));
			string name7 = "HandedViveTracker";
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			inputDeviceMatcher = inputDeviceMatcher.WithManufacturer("HTC", true);
			InputSystem.RegisterLayout<XRController>(name7, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(OpenVR Controller\\(((Vive Tracker)|(VIVE Tracker)).+ - ((Left)|(Right)))", true)));
			string name8 = "ViveTracker";
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			inputDeviceMatcher = inputDeviceMatcher.WithManufacturer("HTC", true);
			InputSystem.RegisterLayout<XRController>(name8, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(OpenVR Controller\\(((Vive Tracker)|(VIVE Tracker)).+\\)(?! - Left| - Right))", true)));
			string name9 = "ViveTracker";
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			inputDeviceMatcher = inputDeviceMatcher.WithManufacturer("HTC", true);
			InputSystem.RegisterLayout<XRController>(name9, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(OpenVR Tracked Device\\(((Vive Tracker)|(VIVE Tracker)).+\\)(?! - Left| - Right))", true)));
			string name10 = "LogitechStylus";
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			inputDeviceMatcher = inputDeviceMatcher.WithManufacturer("Logitech", true);
			InputSystem.RegisterLayout<XRController>(name10, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("(OpenVR Controller\\(.+stylus)", true)));
			string name11 = "ViveLighthouse";
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			inputDeviceMatcher = inputDeviceMatcher.WithManufacturer("HTC", true);
			InputSystem.RegisterLayout<TrackedDevice>(name11, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(OpenVR Tracking Reference\\()", true)));
			string name12 = "ValveLighthouse";
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			inputDeviceMatcher = inputDeviceMatcher.WithManufacturer("Valve Corporation", true);
			InputSystem.RegisterLayout<TrackedDevice>(name12, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(OpenVR Tracking Reference\\()", true)));
		}
	}
}
