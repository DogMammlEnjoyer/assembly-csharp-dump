using System;
using Unity.XR.GoogleVr;
using Unity.XR.Oculus.Input;
using Unity.XR.OpenVR;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.XR.WindowsMR.Input;

namespace UnityEngine.InputSystem.XR
{
	internal static class XRSupport
	{
		public static void Initialize()
		{
			InputSystem.RegisterLayout<PoseControl>("Pose", null);
			InputSystem.RegisterLayout<BoneControl>("Bone", null);
			InputSystem.RegisterLayout<EyesControl>("Eyes", null);
			InputSystem.RegisterLayout<XRHMD>(null, null);
			InputSystem.RegisterLayout<XRController>(null, null);
			InputSystem.onFindLayoutForDevice += XRLayoutBuilder.OnFindLayoutForDevice;
			string name = null;
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout<WMRHMD>(name, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("(Windows Mixed Reality HMD)|(Microsoft HoloLens)|(^(WindowsMR Headset))", true)));
			string name2 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout<WMRSpatialController>(name2, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("(^(Spatial Controller))|(^(OpenVR Controller\\(WindowsMR))", true)));
			string name3 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout<HololensHand>(name3, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("(^(Hand -))", true)));
			string name4 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout<OculusHMD>(name4, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(Oculus Rift)|^(Oculus Quest)|^(Oculus Go)", true)));
			string name5 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout<OculusTouchController>(name5, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("(^(Oculus Touch Controller))|(^(Oculus Quest Controller))", true)));
			string name6 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout<OculusRemote>(name6, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("Oculus Remote", true)));
			string name7 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout<OculusTrackingReference>(name7, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("((Tracking Reference)|(^(Oculus Rift [a-zA-Z0-9]* \\(Camera)))", true)));
			string name8 = "GearVR";
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout<OculusHMDExtended>(name8, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("Oculus HMD", true)));
			string name9 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout<GearVRTrackedController>(name9, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(Oculus Tracked Remote)", true)));
			string name10 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout<DaydreamHMD>(name10, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("Daydream HMD", true)));
			string name11 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout<DaydreamController>(name11, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(Daydream Controller)", true)));
			string name12 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout<OpenVRHMD>(name12, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(OpenVR Headset)|^(Vive Pro)", true)));
			string name13 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout<OpenVRControllerWMR>(name13, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(OpenVR Controller\\(WindowsMR)", true)));
			string name14 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			inputDeviceMatcher = inputDeviceMatcher.WithManufacturer("HTC", true);
			InputSystem.RegisterLayout<ViveWand>(name14, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(OpenVR Controller\\(((Vive. Controller)|(VIVE. Controller)|(Vive Controller)))", true)));
			string name15 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout<OpenVROculusTouchController>(name15, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(OpenVR Controller\\(Oculus)", true)));
			string name16 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			inputDeviceMatcher = inputDeviceMatcher.WithManufacturer("HTC", true);
			InputSystem.RegisterLayout<ViveTracker>(name16, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(VIVE Tracker)", true)));
			string name17 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			inputDeviceMatcher = inputDeviceMatcher.WithManufacturer("HTC", true);
			InputSystem.RegisterLayout<HandedViveTracker>(name17, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(OpenVR Controller\\(VIVE Tracker)", true)));
			string name18 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			inputDeviceMatcher = inputDeviceMatcher.WithManufacturer("HTC", true);
			InputSystem.RegisterLayout<ViveLighthouse>(name18, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("^(HTC V2-XD/XE)", true)));
		}
	}
}
