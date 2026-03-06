using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using OVR.OpenVR;
using UnityEngine;
using UnityEngine.XR;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-ovrinput/")]
public static class OVRInput
{
	public static bool pluginSupportsActiveController
	{
		get
		{
			if (!OVRInput._pluginSupportsActiveControllerCached)
			{
				OVRInput._pluginSupportsActiveController = (true && OVRPlugin.version >= OVRInput._pluginSupportsActiveControllerMinVersion);
				OVRInput._pluginSupportsActiveControllerCached = true;
			}
			return OVRInput._pluginSupportsActiveController;
		}
	}

	static OVRInput()
	{
		OVRInput.controllers = new List<OVRInput.OVRControllerBase>
		{
			new OVRInput.OVRControllerGamepadPC(),
			new OVRInput.OVRControllerTouch(),
			new OVRInput.OVRControllerLTouch(),
			new OVRInput.OVRControllerRTouch(),
			new OVRInput.OVRControllerHands(),
			new OVRInput.OVRControllerLHand(),
			new OVRInput.OVRControllerRHand(),
			new OVRInput.OVRControllerRemote()
		};
		OVRInput.InitHapticInfo();
	}

	public static void Update()
	{
		OVRInput.connectedControllerTypes = OVRInput.Controller.None;
		OVRInput.stepType = OVRPlugin.Step.Render;
		OVRInput.fixedUpdateCount = 0;
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
		{
			OVRInput.UpdateXRControllerNodeIds();
			OVRInput.UpdateXRControllerHaptics();
		}
		for (int i = 0; i < OVRInput.controllers.Count; i++)
		{
			OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
			OVRInput.connectedControllerTypes |= ovrcontrollerBase.Update();
			if ((OVRInput.connectedControllerTypes & ovrcontrollerBase.controllerType) != OVRInput.Controller.None)
			{
				OVRInput.RawButton rawMask = OVRInput.RawButton.Any;
				OVRInput.RawTouch rawMask2 = OVRInput.RawTouch.Any;
				if (OVRInput.Get(rawMask, ovrcontrollerBase.controllerType) || OVRInput.Get(rawMask2, ovrcontrollerBase.controllerType))
				{
					OVRInput.activeControllerType = ovrcontrollerBase.controllerType;
				}
			}
		}
		if ((OVRInput.activeControllerType == OVRInput.Controller.LTouch || OVRInput.activeControllerType == OVRInput.Controller.RTouch) && (OVRInput.connectedControllerTypes & OVRInput.Controller.Touch) == OVRInput.Controller.Touch)
		{
			OVRInput.activeControllerType = OVRInput.Controller.Touch;
		}
		if ((OVRInput.activeControllerType == OVRInput.Controller.LHand || OVRInput.activeControllerType == OVRInput.Controller.RHand) && (OVRInput.connectedControllerTypes & OVRInput.Controller.Hands) == OVRInput.Controller.Hands)
		{
			OVRInput.activeControllerType = OVRInput.Controller.Hands;
		}
		if ((OVRInput.connectedControllerTypes & OVRInput.activeControllerType) == OVRInput.Controller.None)
		{
			OVRInput.activeControllerType = OVRInput.Controller.None;
		}
		if (OVRInput.activeControllerType == OVRInput.Controller.None && (OVRInput.connectedControllerTypes & OVRInput.Controller.Hands) != OVRInput.Controller.None)
		{
			OVRInput.activeControllerType = (OVRInput.connectedControllerTypes & OVRInput.Controller.Hands);
		}
		bool flag = OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus && OVRInput.pluginSupportsActiveController;
		if (OVRManager.instance != null && OVRManager.instance.IsSimultaneousHandsAndControllersSupported)
		{
			flag = (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus);
		}
		if (flag)
		{
			OVRInput.Controller controller = OVRInput.activeControllerType;
			OVRInput.connectedControllerTypes = (OVRInput.Controller)OVRPlugin.GetConnectedControllers();
			OVRInput.activeControllerType = (OVRInput.Controller)OVRPlugin.GetActiveController();
			if (OVRInput.activeControllerType == OVRInput.Controller.None && (controller & OVRInput.Controller.Hands) != OVRInput.Controller.None)
			{
				OVRInput.activeControllerType = controller;
				return;
			}
		}
		else if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
		{
			OVRInput.activeControllerType = OVRInput.connectedControllerTypes;
		}
	}

	public static void FixedUpdate()
	{
		if (OVRPlugin.nativeXrApi != OVRPlugin.XrApi.OpenXR)
		{
			OVRInput.stepType = OVRPlugin.Step.Physics;
			double predictionSeconds = (double)OVRInput.fixedUpdateCount * (double)Time.fixedDeltaTime / (double)Mathf.Max(Time.timeScale, 1E-06f);
			OVRInput.fixedUpdateCount++;
			OVRPlugin.UpdateNodePhysicsPoses(0, predictionSeconds);
		}
	}

	public static OVRInput.InteractionProfile GetCurrentInteractionProfile(OVRInput.Hand hand)
	{
		return (OVRInput.InteractionProfile)OVRPlugin.GetCurrentInteractionProfile((OVRPlugin.Hand)hand);
	}

	public static bool GetControllerOrientationTracked(OVRInput.Controller controllerType)
	{
		if (controllerType <= OVRInput.Controller.RTouch)
		{
			if (controllerType == OVRInput.Controller.LTouch)
			{
				return OVRPlugin.GetNodeOrientationTracked(OVRPlugin.Node.ControllerLeft);
			}
			if (controllerType == OVRInput.Controller.RTouch)
			{
				return OVRPlugin.GetNodeOrientationTracked(OVRPlugin.Node.ControllerRight);
			}
		}
		else
		{
			if (controllerType == OVRInput.Controller.LHand)
			{
				return OVRPlugin.GetNodeOrientationTracked(OVRPlugin.Node.HandLeft);
			}
			if (controllerType == OVRInput.Controller.RHand)
			{
				return OVRPlugin.GetNodeOrientationTracked(OVRPlugin.Node.HandRight);
			}
		}
		return false;
	}

	public static bool GetControllerOrientationValid(OVRInput.Controller controllerType)
	{
		if (controllerType <= OVRInput.Controller.RTouch)
		{
			if (controllerType == OVRInput.Controller.LTouch)
			{
				return OVRPlugin.GetNodeOrientationValid(OVRPlugin.Node.ControllerLeft);
			}
			if (controllerType == OVRInput.Controller.RTouch)
			{
				return OVRPlugin.GetNodeOrientationValid(OVRPlugin.Node.ControllerRight);
			}
		}
		else
		{
			if (controllerType == OVRInput.Controller.LHand)
			{
				return OVRPlugin.GetNodeOrientationValid(OVRPlugin.Node.HandLeft);
			}
			if (controllerType == OVRInput.Controller.RHand)
			{
				return OVRPlugin.GetNodeOrientationValid(OVRPlugin.Node.HandRight);
			}
		}
		return false;
	}

	public static bool GetControllerPositionTracked(OVRInput.Controller controllerType)
	{
		if (controllerType <= OVRInput.Controller.RTouch)
		{
			if (controllerType == OVRInput.Controller.LTouch)
			{
				return OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.ControllerLeft);
			}
			if (controllerType == OVRInput.Controller.RTouch)
			{
				return OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.ControllerRight);
			}
		}
		else
		{
			if (controllerType == OVRInput.Controller.LHand)
			{
				return OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.HandLeft);
			}
			if (controllerType == OVRInput.Controller.RHand)
			{
				return OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.HandRight);
			}
		}
		return false;
	}

	public static bool GetControllerPositionValid(OVRInput.Controller controllerType)
	{
		if (controllerType <= OVRInput.Controller.RTouch)
		{
			if (controllerType == OVRInput.Controller.LTouch)
			{
				return OVRPlugin.GetNodePositionValid(OVRPlugin.Node.ControllerLeft);
			}
			if (controllerType == OVRInput.Controller.RTouch)
			{
				return OVRPlugin.GetNodePositionValid(OVRPlugin.Node.ControllerRight);
			}
		}
		else
		{
			if (controllerType == OVRInput.Controller.LHand)
			{
				return OVRPlugin.GetNodePositionValid(OVRPlugin.Node.HandLeft);
			}
			if (controllerType == OVRInput.Controller.RHand)
			{
				return OVRPlugin.GetNodePositionValid(OVRPlugin.Node.HandRight);
			}
		}
		return false;
	}

	public static bool AreHandPosesGeneratedByControllerData(OVRPlugin.Step stepId, OVRInput.Hand hand)
	{
		if (hand != OVRInput.Hand.HandLeft)
		{
			return hand == OVRInput.Hand.HandRight && OVRPlugin.AreHandPosesGeneratedByControllerData(stepId, OVRPlugin.Node.HandRight);
		}
		return OVRPlugin.AreHandPosesGeneratedByControllerData(stepId, OVRPlugin.Node.HandLeft);
	}

	public static bool EnableSimultaneousHandsAndControllers()
	{
		return OVRPlugin.SetSimultaneousHandsAndControllersEnabled(true);
	}

	public static bool DisableSimultaneousHandsAndControllers()
	{
		return OVRPlugin.SetSimultaneousHandsAndControllersEnabled(false);
	}

	public static OVRInput.ControllerInHandState GetControllerIsInHandState(OVRInput.Hand hand)
	{
		if (hand != OVRInput.Hand.HandLeft)
		{
			if (hand != OVRInput.Hand.HandRight)
			{
				return OVRInput.ControllerInHandState.NoHand;
			}
			if ((OVRInput.connectedControllerTypes & OVRInput.Controller.RHand) == OVRInput.Controller.None)
			{
				return OVRInput.ControllerInHandState.NoHand;
			}
			if (OVRPlugin.GetControllerIsInHand(OVRPlugin.Step.Render, OVRPlugin.Node.ControllerRight))
			{
				return OVRInput.ControllerInHandState.ControllerInHand;
			}
			return OVRInput.ControllerInHandState.ControllerNotInHand;
		}
		else
		{
			if ((OVRInput.connectedControllerTypes & OVRInput.Controller.LHand) == OVRInput.Controller.None)
			{
				return OVRInput.ControllerInHandState.NoHand;
			}
			if (OVRPlugin.GetControllerIsInHand(OVRPlugin.Step.Render, OVRPlugin.Node.ControllerLeft))
			{
				return OVRInput.ControllerInHandState.ControllerInHand;
			}
			return OVRInput.ControllerInHandState.ControllerNotInHand;
		}
	}

	public static OVRInput.Controller GetActiveControllerForHand(OVRInput.Handedness handedness)
	{
		if (handedness != OVRInput.Handedness.LeftHanded)
		{
			if (handedness != OVRInput.Handedness.RightHanded)
			{
				return OVRInput.Controller.None;
			}
			if ((OVRInput.activeControllerType & OVRInput.Controller.RTouch) != OVRInput.Controller.None)
			{
				return OVRInput.Controller.RTouch;
			}
			if ((OVRInput.activeControllerType & OVRInput.Controller.RHand) != OVRInput.Controller.None)
			{
				return OVRInput.Controller.RHand;
			}
		}
		else
		{
			if ((OVRInput.activeControllerType & OVRInput.Controller.LTouch) != OVRInput.Controller.None)
			{
				return OVRInput.Controller.LTouch;
			}
			if ((OVRInput.activeControllerType & OVRInput.Controller.LHand) != OVRInput.Controller.None)
			{
				return OVRInput.Controller.LHand;
			}
		}
		return OVRInput.Controller.None;
	}

	public static Vector3 GetLocalControllerPosition(OVRInput.Controller controllerType)
	{
		if (controllerType <= OVRInput.Controller.RTouch)
		{
			if (controllerType != OVRInput.Controller.LTouch)
			{
				if (controllerType == OVRInput.Controller.RTouch)
				{
					if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
					{
						return OVRPlugin.GetNodePose(OVRPlugin.Node.ControllerRight, OVRInput.stepType).ToOVRPose().position;
					}
					if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
					{
						return OVRInput.openVRControllerDetails[1].localPosition;
					}
					Vector3 result;
					if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.RightHand, NodeStatePropertyType.Position, OVRPlugin.Node.ControllerRight, OVRInput.stepType, out result))
					{
						return result;
					}
					return Vector3.zero;
				}
			}
			else
			{
				if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
				{
					return OVRPlugin.GetNodePose(OVRPlugin.Node.ControllerLeft, OVRInput.stepType).ToOVRPose().position;
				}
				if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
				{
					return OVRInput.openVRControllerDetails[0].localPosition;
				}
				Vector3 result2;
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.LeftHand, NodeStatePropertyType.Position, OVRPlugin.Node.ControllerLeft, OVRInput.stepType, out result2))
				{
					return result2;
				}
				return Vector3.zero;
			}
		}
		else if (controllerType != OVRInput.Controller.LHand)
		{
			if (controllerType == OVRInput.Controller.RHand)
			{
				if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
				{
					return OVRPlugin.GetNodePose(OVRPlugin.Node.HandRight, OVRInput.stepType).ToOVRPose().position;
				}
				if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
				{
					return OVRInput.openVRControllerDetails[1].localPosition;
				}
				Vector3 result3;
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.RightHand, NodeStatePropertyType.Position, OVRPlugin.Node.HandRight, OVRInput.stepType, out result3))
				{
					return result3;
				}
				return Vector3.zero;
			}
		}
		else
		{
			if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
			{
				return OVRPlugin.GetNodePose(OVRPlugin.Node.HandLeft, OVRInput.stepType).ToOVRPose().position;
			}
			if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
			{
				return OVRInput.openVRControllerDetails[0].localPosition;
			}
			Vector3 result4;
			if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.LeftHand, NodeStatePropertyType.Position, OVRPlugin.Node.HandLeft, OVRInput.stepType, out result4))
			{
				return result4;
			}
			return Vector3.zero;
		}
		return Vector3.zero;
	}

	public static Vector3 GetLocalControllerVelocity(OVRInput.Controller controllerType)
	{
		Vector3 zero = Vector3.zero;
		if (controllerType <= OVRInput.Controller.RTouch)
		{
			if (controllerType != OVRInput.Controller.LTouch)
			{
				if (controllerType == OVRInput.Controller.RTouch)
				{
					if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.RightHand, NodeStatePropertyType.Velocity, OVRPlugin.Node.ControllerRight, OVRInput.stepType, out zero))
					{
						return zero;
					}
					return Vector3.zero;
				}
			}
			else
			{
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.LeftHand, NodeStatePropertyType.Velocity, OVRPlugin.Node.ControllerLeft, OVRInput.stepType, out zero))
				{
					return zero;
				}
				return Vector3.zero;
			}
		}
		else if (controllerType != OVRInput.Controller.LHand)
		{
			if (controllerType == OVRInput.Controller.RHand)
			{
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.RightHand, NodeStatePropertyType.Velocity, OVRPlugin.Node.HandRight, OVRInput.stepType, out zero))
				{
					return zero;
				}
				return Vector3.zero;
			}
		}
		else
		{
			if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.LeftHand, NodeStatePropertyType.Velocity, OVRPlugin.Node.HandLeft, OVRInput.stepType, out zero))
			{
				return zero;
			}
			return Vector3.zero;
		}
		return Vector3.zero;
	}

	[Obsolete("Deprecated. Acceleration is not supported in OpenXR", false)]
	public static Vector3 GetLocalControllerAcceleration(OVRInput.Controller controllerType)
	{
		Vector3 zero = Vector3.zero;
		if (controllerType <= OVRInput.Controller.RTouch)
		{
			if (controllerType != OVRInput.Controller.LTouch)
			{
				if (controllerType == OVRInput.Controller.RTouch)
				{
					if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.RightHand, NodeStatePropertyType.Acceleration, OVRPlugin.Node.ControllerRight, OVRInput.stepType, out zero))
					{
						return zero;
					}
					return Vector3.zero;
				}
			}
			else
			{
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.LeftHand, NodeStatePropertyType.Acceleration, OVRPlugin.Node.ControllerLeft, OVRInput.stepType, out zero))
				{
					return zero;
				}
				return Vector3.zero;
			}
		}
		else if (controllerType != OVRInput.Controller.LHand)
		{
			if (controllerType == OVRInput.Controller.RHand)
			{
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.RightHand, NodeStatePropertyType.Acceleration, OVRPlugin.Node.HandRight, OVRInput.stepType, out zero))
				{
					return zero;
				}
				return Vector3.zero;
			}
		}
		else
		{
			if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.LeftHand, NodeStatePropertyType.Acceleration, OVRPlugin.Node.HandLeft, OVRInput.stepType, out zero))
			{
				return zero;
			}
			return Vector3.zero;
		}
		return Vector3.zero;
	}

	public static Quaternion GetLocalControllerRotation(OVRInput.Controller controllerType)
	{
		if (controllerType <= OVRInput.Controller.RTouch)
		{
			if (controllerType != OVRInput.Controller.LTouch)
			{
				if (controllerType == OVRInput.Controller.RTouch)
				{
					if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
					{
						return OVRPlugin.GetNodePose(OVRPlugin.Node.ControllerRight, OVRInput.stepType).ToOVRPose().orientation;
					}
					if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
					{
						return OVRInput.openVRControllerDetails[1].localOrientation;
					}
					Quaternion result;
					if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(XRNode.RightHand, NodeStatePropertyType.Orientation, OVRPlugin.Node.ControllerRight, OVRInput.stepType, out result))
					{
						return result;
					}
					return Quaternion.identity;
				}
			}
			else
			{
				if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
				{
					return OVRPlugin.GetNodePose(OVRPlugin.Node.ControllerLeft, OVRInput.stepType).ToOVRPose().orientation;
				}
				if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
				{
					return OVRInput.openVRControllerDetails[0].localOrientation;
				}
				Quaternion result2;
				if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(XRNode.LeftHand, NodeStatePropertyType.Orientation, OVRPlugin.Node.ControllerLeft, OVRInput.stepType, out result2))
				{
					return result2;
				}
				return Quaternion.identity;
			}
		}
		else if (controllerType != OVRInput.Controller.LHand)
		{
			if (controllerType == OVRInput.Controller.RHand)
			{
				if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
				{
					return OVRPlugin.GetNodePose(OVRPlugin.Node.HandRight, OVRInput.stepType).ToOVRPose().orientation;
				}
				if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
				{
					return OVRInput.openVRControllerDetails[1].localOrientation;
				}
				Quaternion result3;
				if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(XRNode.RightHand, NodeStatePropertyType.Orientation, OVRPlugin.Node.HandRight, OVRInput.stepType, out result3))
				{
					return result3;
				}
				return Quaternion.identity;
			}
		}
		else
		{
			if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
			{
				return OVRPlugin.GetNodePose(OVRPlugin.Node.HandLeft, OVRInput.stepType).ToOVRPose().orientation;
			}
			if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
			{
				return OVRInput.openVRControllerDetails[0].localOrientation;
			}
			Quaternion result4;
			if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(XRNode.LeftHand, NodeStatePropertyType.Orientation, OVRPlugin.Node.HandLeft, OVRInput.stepType, out result4))
			{
				return result4;
			}
			return Quaternion.identity;
		}
		return Quaternion.identity;
	}

	public static Vector3 GetLocalControllerAngularVelocity(OVRInput.Controller controllerType)
	{
		Vector3 zero = Vector3.zero;
		if (controllerType <= OVRInput.Controller.RTouch)
		{
			if (controllerType != OVRInput.Controller.LTouch)
			{
				if (controllerType == OVRInput.Controller.RTouch)
				{
					if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.RightHand, NodeStatePropertyType.AngularVelocity, OVRPlugin.Node.ControllerRight, OVRInput.stepType, out zero))
					{
						return zero;
					}
					return Vector3.zero;
				}
			}
			else
			{
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.LeftHand, NodeStatePropertyType.AngularVelocity, OVRPlugin.Node.ControllerLeft, OVRInput.stepType, out zero))
				{
					return zero;
				}
				return Vector3.zero;
			}
		}
		else if (controllerType != OVRInput.Controller.LHand)
		{
			if (controllerType == OVRInput.Controller.RHand)
			{
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.RightHand, NodeStatePropertyType.AngularVelocity, OVRPlugin.Node.HandRight, OVRInput.stepType, out zero))
				{
					return zero;
				}
				return Vector3.zero;
			}
		}
		else
		{
			if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.LeftHand, NodeStatePropertyType.AngularVelocity, OVRPlugin.Node.HandLeft, OVRInput.stepType, out zero))
			{
				return zero;
			}
			return Vector3.zero;
		}
		return Vector3.zero;
	}

	[Obsolete("Deprecated. Acceleration is not supported in OpenXR", false)]
	public static Vector3 GetLocalControllerAngularAcceleration(OVRInput.Controller controllerType)
	{
		Vector3 zero = Vector3.zero;
		if (controllerType <= OVRInput.Controller.RTouch)
		{
			if (controllerType != OVRInput.Controller.LTouch)
			{
				if (controllerType == OVRInput.Controller.RTouch)
				{
					if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.RightHand, NodeStatePropertyType.AngularAcceleration, OVRPlugin.Node.ControllerRight, OVRInput.stepType, out zero))
					{
						return zero;
					}
					return Vector3.zero;
				}
			}
			else
			{
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.LeftHand, NodeStatePropertyType.AngularAcceleration, OVRPlugin.Node.ControllerLeft, OVRInput.stepType, out zero))
				{
					return zero;
				}
				return Vector3.zero;
			}
		}
		else if (controllerType != OVRInput.Controller.LHand)
		{
			if (controllerType == OVRInput.Controller.RHand)
			{
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.RightHand, NodeStatePropertyType.AngularAcceleration, OVRPlugin.Node.HandRight, OVRInput.stepType, out zero))
				{
					return zero;
				}
				return Vector3.zero;
			}
		}
		else
		{
			if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.LeftHand, NodeStatePropertyType.AngularAcceleration, OVRPlugin.Node.HandLeft, OVRInput.stepType, out zero))
			{
				return zero;
			}
			return Vector3.zero;
		}
		return Vector3.zero;
	}

	public static bool GetLocalControllerStatesWithoutPrediction(OVRInput.Controller controllerType, out Vector3 position, out Quaternion rotation, out Vector3 velocity, out Vector3 angularVelocity)
	{
		position = Vector3.zero;
		rotation = Quaternion.identity;
		velocity = Vector3.zero;
		angularVelocity = Vector3.zero;
		if (OVRManager.loadedXRDevice != OVRManager.XRDevice.Oculus)
		{
			return false;
		}
		if (OVRPlugin.nativeXrApi != OVRPlugin.XrApi.OpenXR)
		{
			return false;
		}
		OVRPlugin.PoseStatef nodePoseStateImmediate;
		if (controllerType <= OVRInput.Controller.RTouch)
		{
			if (controllerType == OVRInput.Controller.LTouch)
			{
				nodePoseStateImmediate = OVRPlugin.GetNodePoseStateImmediate(OVRPlugin.Node.ControllerLeft);
				goto IL_83;
			}
			if (controllerType == OVRInput.Controller.RTouch)
			{
				nodePoseStateImmediate = OVRPlugin.GetNodePoseStateImmediate(OVRPlugin.Node.ControllerRight);
				goto IL_83;
			}
		}
		else
		{
			if (controllerType == OVRInput.Controller.LHand)
			{
				nodePoseStateImmediate = OVRPlugin.GetNodePoseStateImmediate(OVRPlugin.Node.HandLeft);
				goto IL_83;
			}
			if (controllerType == OVRInput.Controller.RHand)
			{
				nodePoseStateImmediate = OVRPlugin.GetNodePoseStateImmediate(OVRPlugin.Node.HandRight);
				goto IL_83;
			}
		}
		return false;
		IL_83:
		if (OVRInput.GetControllerPositionValid(controllerType))
		{
			position = nodePoseStateImmediate.Pose.ToOVRPose().position;
			velocity = nodePoseStateImmediate.Velocity.FromFlippedZVector3f();
		}
		if (OVRInput.GetControllerOrientationValid(controllerType))
		{
			rotation = nodePoseStateImmediate.Pose.ToOVRPose().orientation;
			angularVelocity = nodePoseStateImmediate.AngularVelocity.FromFlippedZVector3f();
		}
		return true;
	}

	public static OVRInput.Handedness GetDominantHand()
	{
		return (OVRInput.Handedness)OVRPlugin.GetDominantHand();
	}

	public static bool Get(OVRInput.Button virtualMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedButton(virtualMask, OVRInput.RawButton.None, controllerMask);
	}

	public static bool Get(OVRInput.RawButton rawMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedButton(OVRInput.Button.None, rawMask, controllerMask);
	}

	private static bool GetResolvedButton(OVRInput.Button virtualMask, OVRInput.RawButton rawMask, OVRInput.Controller controllerMask)
	{
		if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
		{
			controllerMask |= OVRInput.activeControllerType;
		}
		for (int i = 0; i < OVRInput.controllers.Count; i++)
		{
			OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
			if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
			{
				OVRInput.RawButton rawButton = rawMask | ovrcontrollerBase.ResolveToRawMask(virtualMask);
				if ((ovrcontrollerBase.currentState.Buttons & (uint)rawButton) != 0U)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool GetDown(OVRInput.Button virtualMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedButtonDown(virtualMask, OVRInput.RawButton.None, controllerMask);
	}

	public static bool GetDown(OVRInput.RawButton rawMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedButtonDown(OVRInput.Button.None, rawMask, controllerMask);
	}

	private static bool GetResolvedButtonDown(OVRInput.Button virtualMask, OVRInput.RawButton rawMask, OVRInput.Controller controllerMask)
	{
		bool result = false;
		if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
		{
			controllerMask |= OVRInput.activeControllerType;
		}
		for (int i = 0; i < OVRInput.controllers.Count; i++)
		{
			OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
			if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
			{
				OVRInput.RawButton rawButton = rawMask | ovrcontrollerBase.ResolveToRawMask(virtualMask);
				if ((ovrcontrollerBase.previousState.Buttons & (uint)rawButton) != 0U)
				{
					return false;
				}
				if ((ovrcontrollerBase.currentState.Buttons & (uint)rawButton) != 0U && (ovrcontrollerBase.previousState.Buttons & (uint)rawButton) == 0U)
				{
					result = true;
				}
			}
		}
		return result;
	}

	public static bool GetUp(OVRInput.Button virtualMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedButtonUp(virtualMask, OVRInput.RawButton.None, controllerMask);
	}

	public static bool GetUp(OVRInput.RawButton rawMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedButtonUp(OVRInput.Button.None, rawMask, controllerMask);
	}

	private static bool GetResolvedButtonUp(OVRInput.Button virtualMask, OVRInput.RawButton rawMask, OVRInput.Controller controllerMask)
	{
		bool result = false;
		if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
		{
			controllerMask |= OVRInput.activeControllerType;
		}
		for (int i = 0; i < OVRInput.controllers.Count; i++)
		{
			OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
			if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
			{
				OVRInput.RawButton rawButton = rawMask | ovrcontrollerBase.ResolveToRawMask(virtualMask);
				if ((ovrcontrollerBase.currentState.Buttons & (uint)rawButton) != 0U)
				{
					return false;
				}
				if ((ovrcontrollerBase.currentState.Buttons & (uint)rawButton) == 0U && (ovrcontrollerBase.previousState.Buttons & (uint)rawButton) != 0U)
				{
					result = true;
				}
			}
		}
		return result;
	}

	public static bool Get(OVRInput.Touch virtualMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedTouch(virtualMask, OVRInput.RawTouch.None, controllerMask);
	}

	public static bool Get(OVRInput.RawTouch rawMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedTouch(OVRInput.Touch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedTouch(OVRInput.Touch virtualMask, OVRInput.RawTouch rawMask, OVRInput.Controller controllerMask)
	{
		if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
		{
			controllerMask |= OVRInput.activeControllerType;
		}
		for (int i = 0; i < OVRInput.controllers.Count; i++)
		{
			OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
			if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
			{
				OVRInput.RawTouch rawTouch = rawMask | ovrcontrollerBase.ResolveToRawMask(virtualMask);
				if ((ovrcontrollerBase.currentState.Touches & (uint)rawTouch) != 0U)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool GetDown(OVRInput.Touch virtualMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedTouchDown(virtualMask, OVRInput.RawTouch.None, controllerMask);
	}

	public static bool GetDown(OVRInput.RawTouch rawMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedTouchDown(OVRInput.Touch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedTouchDown(OVRInput.Touch virtualMask, OVRInput.RawTouch rawMask, OVRInput.Controller controllerMask)
	{
		bool result = false;
		if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
		{
			controllerMask |= OVRInput.activeControllerType;
		}
		for (int i = 0; i < OVRInput.controllers.Count; i++)
		{
			OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
			if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
			{
				OVRInput.RawTouch rawTouch = rawMask | ovrcontrollerBase.ResolveToRawMask(virtualMask);
				if ((ovrcontrollerBase.previousState.Touches & (uint)rawTouch) != 0U)
				{
					return false;
				}
				if ((ovrcontrollerBase.currentState.Touches & (uint)rawTouch) != 0U && (ovrcontrollerBase.previousState.Touches & (uint)rawTouch) == 0U)
				{
					result = true;
				}
			}
		}
		return result;
	}

	public static bool GetUp(OVRInput.Touch virtualMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedTouchUp(virtualMask, OVRInput.RawTouch.None, controllerMask);
	}

	public static bool GetUp(OVRInput.RawTouch rawMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedTouchUp(OVRInput.Touch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedTouchUp(OVRInput.Touch virtualMask, OVRInput.RawTouch rawMask, OVRInput.Controller controllerMask)
	{
		bool result = false;
		if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
		{
			controllerMask |= OVRInput.activeControllerType;
		}
		for (int i = 0; i < OVRInput.controllers.Count; i++)
		{
			OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
			if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
			{
				OVRInput.RawTouch rawTouch = rawMask | ovrcontrollerBase.ResolveToRawMask(virtualMask);
				if ((ovrcontrollerBase.currentState.Touches & (uint)rawTouch) != 0U)
				{
					return false;
				}
				if ((ovrcontrollerBase.currentState.Touches & (uint)rawTouch) == 0U && (ovrcontrollerBase.previousState.Touches & (uint)rawTouch) != 0U)
				{
					result = true;
				}
			}
		}
		return result;
	}

	public static bool Get(OVRInput.NearTouch virtualMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedNearTouch(virtualMask, OVRInput.RawNearTouch.None, controllerMask);
	}

	public static bool Get(OVRInput.RawNearTouch rawMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedNearTouch(OVRInput.NearTouch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedNearTouch(OVRInput.NearTouch virtualMask, OVRInput.RawNearTouch rawMask, OVRInput.Controller controllerMask)
	{
		if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
		{
			controllerMask |= OVRInput.activeControllerType;
		}
		for (int i = 0; i < OVRInput.controllers.Count; i++)
		{
			OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
			if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
			{
				OVRInput.RawNearTouch rawNearTouch = rawMask | ovrcontrollerBase.ResolveToRawMask(virtualMask);
				if ((ovrcontrollerBase.currentState.NearTouches & (uint)rawNearTouch) != 0U)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool GetDown(OVRInput.NearTouch virtualMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedNearTouchDown(virtualMask, OVRInput.RawNearTouch.None, controllerMask);
	}

	public static bool GetDown(OVRInput.RawNearTouch rawMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedNearTouchDown(OVRInput.NearTouch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedNearTouchDown(OVRInput.NearTouch virtualMask, OVRInput.RawNearTouch rawMask, OVRInput.Controller controllerMask)
	{
		bool result = false;
		if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
		{
			controllerMask |= OVRInput.activeControllerType;
		}
		for (int i = 0; i < OVRInput.controllers.Count; i++)
		{
			OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
			if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
			{
				OVRInput.RawNearTouch rawNearTouch = rawMask | ovrcontrollerBase.ResolveToRawMask(virtualMask);
				if ((ovrcontrollerBase.previousState.NearTouches & (uint)rawNearTouch) != 0U)
				{
					return false;
				}
				if ((ovrcontrollerBase.currentState.NearTouches & (uint)rawNearTouch) != 0U && (ovrcontrollerBase.previousState.NearTouches & (uint)rawNearTouch) == 0U)
				{
					result = true;
				}
			}
		}
		return result;
	}

	public static bool GetUp(OVRInput.NearTouch virtualMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedNearTouchUp(virtualMask, OVRInput.RawNearTouch.None, controllerMask);
	}

	public static bool GetUp(OVRInput.RawNearTouch rawMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedNearTouchUp(OVRInput.NearTouch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedNearTouchUp(OVRInput.NearTouch virtualMask, OVRInput.RawNearTouch rawMask, OVRInput.Controller controllerMask)
	{
		bool result = false;
		if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
		{
			controllerMask |= OVRInput.activeControllerType;
		}
		for (int i = 0; i < OVRInput.controllers.Count; i++)
		{
			OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
			if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
			{
				OVRInput.RawNearTouch rawNearTouch = rawMask | ovrcontrollerBase.ResolveToRawMask(virtualMask);
				if ((ovrcontrollerBase.currentState.NearTouches & (uint)rawNearTouch) != 0U)
				{
					return false;
				}
				if ((ovrcontrollerBase.currentState.NearTouches & (uint)rawNearTouch) == 0U && (ovrcontrollerBase.previousState.NearTouches & (uint)rawNearTouch) != 0U)
				{
					result = true;
				}
			}
		}
		return result;
	}

	public static float Get(OVRInput.Axis1D virtualMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedAxis1D(virtualMask, OVRInput.RawAxis1D.None, controllerMask);
	}

	public static float Get(OVRInput.RawAxis1D rawMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedAxis1D(OVRInput.Axis1D.None, rawMask, controllerMask);
	}

	private static float GetResolvedAxis1D(OVRInput.Axis1D virtualMask, OVRInput.RawAxis1D rawMask, OVRInput.Controller controllerMask)
	{
		float num = 0f;
		if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
		{
			controllerMask |= OVRInput.activeControllerType;
		}
		for (int i = 0; i < OVRInput.controllers.Count; i++)
		{
			OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
			if (OVRManager.loadedXRDevice != OVRManager.XRDevice.Oculus)
			{
				ovrcontrollerBase.shouldApplyDeadzone = false;
			}
			if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
			{
				OVRInput.RawAxis1D rawAxis1D = rawMask | ovrcontrollerBase.ResolveToRawMask(virtualMask);
				if ((OVRInput.RawAxis1D.LIndexTrigger & rawAxis1D) != OVRInput.RawAxis1D.None)
				{
					float num2 = ovrcontrollerBase.currentState.LIndexTrigger;
					if (ovrcontrollerBase.shouldApplyDeadzone)
					{
						num2 = OVRInput.CalculateDeadzone(num2, OVRInput.AXIS_DEADZONE_THRESHOLD);
					}
					num = OVRInput.CalculateAbsMax(num, num2);
				}
				if ((OVRInput.RawAxis1D.RIndexTrigger & rawAxis1D) != OVRInput.RawAxis1D.None)
				{
					float num3 = ovrcontrollerBase.currentState.RIndexTrigger;
					if (ovrcontrollerBase.shouldApplyDeadzone)
					{
						num3 = OVRInput.CalculateDeadzone(num3, OVRInput.AXIS_DEADZONE_THRESHOLD);
					}
					num = OVRInput.CalculateAbsMax(num, num3);
				}
				if ((OVRInput.RawAxis1D.LHandTrigger & rawAxis1D) != OVRInput.RawAxis1D.None)
				{
					float num4 = ovrcontrollerBase.currentState.LHandTrigger;
					if (ovrcontrollerBase.shouldApplyDeadzone)
					{
						num4 = OVRInput.CalculateDeadzone(num4, OVRInput.AXIS_DEADZONE_THRESHOLD);
					}
					num = OVRInput.CalculateAbsMax(num, num4);
				}
				if ((OVRInput.RawAxis1D.RHandTrigger & rawAxis1D) != OVRInput.RawAxis1D.None)
				{
					float num5 = ovrcontrollerBase.currentState.RHandTrigger;
					if (ovrcontrollerBase.shouldApplyDeadzone)
					{
						num5 = OVRInput.CalculateDeadzone(num5, OVRInput.AXIS_DEADZONE_THRESHOLD);
					}
					num = OVRInput.CalculateAbsMax(num, num5);
				}
				if ((OVRInput.RawAxis1D.LIndexTriggerCurl & rawAxis1D) != OVRInput.RawAxis1D.None)
				{
					float lindexTriggerCurl = ovrcontrollerBase.currentState.LIndexTriggerCurl;
					num = OVRInput.CalculateAbsMax(num, lindexTriggerCurl);
				}
				if ((OVRInput.RawAxis1D.RIndexTriggerCurl & rawAxis1D) != OVRInput.RawAxis1D.None)
				{
					float rindexTriggerCurl = ovrcontrollerBase.currentState.RIndexTriggerCurl;
					num = OVRInput.CalculateAbsMax(num, rindexTriggerCurl);
				}
				if ((OVRInput.RawAxis1D.LIndexTriggerSlide & rawAxis1D) != OVRInput.RawAxis1D.None)
				{
					float lindexTriggerSlide = ovrcontrollerBase.currentState.LIndexTriggerSlide;
					num = OVRInput.CalculateAbsMax(num, lindexTriggerSlide);
				}
				if ((OVRInput.RawAxis1D.RIndexTriggerSlide & rawAxis1D) != OVRInput.RawAxis1D.None)
				{
					float rindexTriggerSlide = ovrcontrollerBase.currentState.RIndexTriggerSlide;
					num = OVRInput.CalculateAbsMax(num, rindexTriggerSlide);
				}
				if ((OVRInput.RawAxis1D.LThumbRestForce & rawAxis1D) != OVRInput.RawAxis1D.None)
				{
					float lthumbRestForce = ovrcontrollerBase.currentState.LThumbRestForce;
					num = OVRInput.CalculateAbsMax(num, lthumbRestForce);
				}
				if ((OVRInput.RawAxis1D.RThumbRestForce & rawAxis1D) != OVRInput.RawAxis1D.None)
				{
					float rthumbRestForce = ovrcontrollerBase.currentState.RThumbRestForce;
					num = OVRInput.CalculateAbsMax(num, rthumbRestForce);
				}
				if ((OVRInput.RawAxis1D.LStylusForce & rawAxis1D) != OVRInput.RawAxis1D.None)
				{
					float lstylusForce = ovrcontrollerBase.currentState.LStylusForce;
					num = OVRInput.CalculateAbsMax(num, lstylusForce);
				}
				if ((OVRInput.RawAxis1D.RStylusForce & rawAxis1D) != OVRInput.RawAxis1D.None)
				{
					float rstylusForce = ovrcontrollerBase.currentState.RStylusForce;
					num = OVRInput.CalculateAbsMax(num, rstylusForce);
				}
				if ((OVRInput.RawAxis1D.LIndexTriggerForce & rawAxis1D) != OVRInput.RawAxis1D.None)
				{
					float lindexTriggerForce = ovrcontrollerBase.currentState.LIndexTriggerForce;
					num = OVRInput.CalculateAbsMax(num, lindexTriggerForce);
				}
				if ((OVRInput.RawAxis1D.RIndexTriggerForce & rawAxis1D) != OVRInput.RawAxis1D.None)
				{
					float rindexTriggerForce = ovrcontrollerBase.currentState.RIndexTriggerForce;
					num = OVRInput.CalculateAbsMax(num, rindexTriggerForce);
				}
			}
		}
		return num;
	}

	public static Vector2 Get(OVRInput.Axis2D virtualMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedAxis2D(virtualMask, OVRInput.RawAxis2D.None, controllerMask);
	}

	public static Vector2 Get(OVRInput.RawAxis2D rawMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		return OVRInput.GetResolvedAxis2D(OVRInput.Axis2D.None, rawMask, controllerMask);
	}

	private static Vector2 GetResolvedAxis2D(OVRInput.Axis2D virtualMask, OVRInput.RawAxis2D rawMask, OVRInput.Controller controllerMask)
	{
		Vector2 vector = Vector2.zero;
		if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
		{
			controllerMask |= OVRInput.activeControllerType;
		}
		for (int i = 0; i < OVRInput.controllers.Count; i++)
		{
			OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
			if (OVRManager.loadedXRDevice != OVRManager.XRDevice.Oculus)
			{
				ovrcontrollerBase.shouldApplyDeadzone = false;
			}
			if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
			{
				OVRInput.RawAxis2D rawAxis2D = rawMask | ovrcontrollerBase.ResolveToRawMask(virtualMask);
				if ((OVRInput.RawAxis2D.LThumbstick & rawAxis2D) != OVRInput.RawAxis2D.None)
				{
					Vector2 vector2 = new Vector2(ovrcontrollerBase.currentState.LThumbstick.x, ovrcontrollerBase.currentState.LThumbstick.y);
					if (ovrcontrollerBase.shouldApplyDeadzone)
					{
						vector2 = OVRInput.CalculateDeadzone(vector2, OVRInput.AXIS_DEADZONE_THRESHOLD);
					}
					vector = OVRInput.CalculateAbsMax(vector, vector2);
				}
				if ((OVRInput.RawAxis2D.LTouchpad & rawAxis2D) != OVRInput.RawAxis2D.None)
				{
					Vector2 b = new Vector2(ovrcontrollerBase.currentState.LTouchpad.x, ovrcontrollerBase.currentState.LTouchpad.y);
					vector = OVRInput.CalculateAbsMax(vector, b);
				}
				if ((OVRInput.RawAxis2D.RThumbstick & rawAxis2D) != OVRInput.RawAxis2D.None)
				{
					Vector2 vector3 = new Vector2(ovrcontrollerBase.currentState.RThumbstick.x, ovrcontrollerBase.currentState.RThumbstick.y);
					if (ovrcontrollerBase.shouldApplyDeadzone)
					{
						vector3 = OVRInput.CalculateDeadzone(vector3, OVRInput.AXIS_DEADZONE_THRESHOLD);
					}
					vector = OVRInput.CalculateAbsMax(vector, vector3);
				}
				if ((OVRInput.RawAxis2D.RTouchpad & rawAxis2D) != OVRInput.RawAxis2D.None)
				{
					Vector2 b2 = new Vector2(ovrcontrollerBase.currentState.RTouchpad.x, ovrcontrollerBase.currentState.RTouchpad.y);
					vector = OVRInput.CalculateAbsMax(vector, b2);
				}
			}
		}
		return vector;
	}

	public static OVRInput.Controller GetConnectedControllers()
	{
		return OVRInput.connectedControllerTypes;
	}

	public static bool IsControllerConnected(OVRInput.Controller controller)
	{
		return (OVRInput.connectedControllerTypes & controller) == controller;
	}

	public static OVRInput.Controller GetActiveController()
	{
		return OVRInput.activeControllerType;
	}

	private static void StartVibration(float amplitude, float duration, XRNode controllerNode)
	{
		int num = (controllerNode == XRNode.LeftHand) ? 0 : 1;
		OVRInput.hapticInfos[num].hapticsDurationPlayed = 0f;
		OVRInput.hapticInfos[num].hapticAmplitude = amplitude;
		OVRInput.hapticInfos[num].hapticsDuration = duration;
		OVRInput.hapticInfos[num].playingHaptics = (amplitude != 0f);
		OVRInput.hapticInfos[num].node = controllerNode;
		if (amplitude <= 0f || duration <= 0f)
		{
			OVRInput.hapticInfos[num].playingHaptics = false;
		}
	}

	public static void SetOpenVRLocalPose(Vector3 leftPos, Vector3 rightPos, Quaternion leftRot, Quaternion rightRot)
	{
		OVRInput.openVRControllerDetails[0].localPosition = leftPos;
		OVRInput.openVRControllerDetails[0].localOrientation = leftRot;
		OVRInput.openVRControllerDetails[1].localPosition = rightPos;
		OVRInput.openVRControllerDetails[1].localOrientation = rightRot;
	}

	public static string GetOpenVRStringProperty(ETrackedDeviceProperty prop, uint deviceId = 0U)
	{
		ETrackedPropertyError etrackedPropertyError = ETrackedPropertyError.TrackedProp_Success;
		CVRSystem system = OpenVR.System;
		if (system == null)
		{
			return "";
		}
		uint stringTrackedDeviceProperty = system.GetStringTrackedDeviceProperty(deviceId, prop, null, 0U, ref etrackedPropertyError);
		if (stringTrackedDeviceProperty > 1U)
		{
			StringBuilder stringBuilder = new StringBuilder((int)stringTrackedDeviceProperty);
			system.GetStringTrackedDeviceProperty(deviceId, prop, stringBuilder, stringTrackedDeviceProperty, ref etrackedPropertyError);
			return stringBuilder.ToString();
		}
		if (etrackedPropertyError == ETrackedPropertyError.TrackedProp_Success)
		{
			return "<unknown>";
		}
		return etrackedPropertyError.ToString();
	}

	private static void UpdateXRControllerNodeIds()
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
		{
			OVRInput.openVRControllerDetails[0].deviceID = 64U;
			OVRInput.openVRControllerDetails[1].deviceID = 64U;
			CVRSystem system = OpenVR.System;
			if (system != null)
			{
				for (uint num = 0U; num < 64U; num += 1U)
				{
					if (system.GetTrackedDeviceClass(num) == ETrackedDeviceClass.Controller && system.IsTrackedDeviceConnected(num))
					{
						string openVRStringProperty = OVRInput.GetOpenVRStringProperty(ETrackedDeviceProperty.Prop_ControllerType_String, num);
						OVRInput.OpenVRController controllerType;
						if (openVRStringProperty == OVRInput.OPENVR_TOUCH_NAME)
						{
							controllerType = OVRInput.OpenVRController.OculusTouch;
						}
						else if (openVRStringProperty == OVRInput.OPENVR_VIVE_CONTROLLER_NAME)
						{
							controllerType = OVRInput.OpenVRController.ViveController;
						}
						else if (openVRStringProperty == OVRInput.OPENVR_WINDOWSMR_CONTROLLER_NAME)
						{
							controllerType = OVRInput.OpenVRController.WindowsMRController;
						}
						else
						{
							controllerType = OVRInput.OpenVRController.Unknown;
						}
						ETrackedControllerRole controllerRoleForTrackedDeviceIndex = system.GetControllerRoleForTrackedDeviceIndex(num);
						if (controllerRoleForTrackedDeviceIndex == ETrackedControllerRole.LeftHand)
						{
							system.GetControllerState(num, ref OVRInput.openVRControllerDetails[0].state, (uint)Marshal.SizeOf(typeof(VRControllerState_t)));
							OVRInput.openVRControllerDetails[0].deviceID = num;
							OVRInput.openVRControllerDetails[0].controllerType = controllerType;
							OVRInput.connectedControllerTypes |= OVRInput.Controller.LTouch;
						}
						else if (controllerRoleForTrackedDeviceIndex == ETrackedControllerRole.RightHand)
						{
							system.GetControllerState(num, ref OVRInput.openVRControllerDetails[1].state, (uint)Marshal.SizeOf(typeof(VRControllerState_t)));
							OVRInput.openVRControllerDetails[1].deviceID = num;
							OVRInput.openVRControllerDetails[1].controllerType = controllerType;
							OVRInput.connectedControllerTypes |= OVRInput.Controller.RTouch;
						}
					}
				}
			}
		}
	}

	private static void UpdateXRControllerHaptics()
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
		{
			for (int i = 0; i < OVRInput.NUM_HAPTIC_CHANNELS; i++)
			{
				if (OVRInput.hapticInfos[i].playingHaptics)
				{
					OVRInput.hapticInfos[i].hapticsDurationPlayed += Time.deltaTime;
					OVRInput.PlayHapticImpulse(OVRInput.hapticInfos[i].hapticAmplitude, OVRInput.hapticInfos[i].node);
					if (OVRInput.hapticInfos[i].hapticsDurationPlayed >= OVRInput.hapticInfos[i].hapticsDuration)
					{
						OVRInput.hapticInfos[i].playingHaptics = false;
					}
				}
			}
		}
	}

	private static void InitHapticInfo()
	{
		OVRInput.hapticInfos = new OVRInput.HapticInfo[OVRInput.NUM_HAPTIC_CHANNELS];
		for (int i = 0; i < OVRInput.NUM_HAPTIC_CHANNELS; i++)
		{
			OVRInput.hapticInfos[i] = new OVRInput.HapticInfo();
		}
	}

	private static void PlayHapticImpulse(float amplitude, XRNode deviceNode)
	{
		CVRSystem system = OpenVR.System;
		if (system != null && amplitude != 0f)
		{
			uint num = (deviceNode == XRNode.LeftHand) ? OVRInput.openVRControllerDetails[0].deviceID : OVRInput.openVRControllerDetails[1].deviceID;
			if (OVRInput.IsValidOpenVRDevice(num))
			{
				system.TriggerHapticPulse(num, 0U, (ushort)(OVRInput.OPENVR_MAX_HAPTIC_AMPLITUDE * amplitude));
			}
		}
	}

	private static bool IsValidOpenVRDevice(uint deviceId)
	{
		return deviceId >= 0U && deviceId < 64U;
	}

	public static void SetControllerVibration(float frequency, float amplitude, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
		{
			if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
			{
				controllerMask |= OVRInput.activeControllerType;
			}
			for (int i = 0; i < OVRInput.controllers.Count; i++)
			{
				OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
				if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
				{
					ovrcontrollerBase.SetControllerVibration(frequency, amplitude);
				}
			}
			return;
		}
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR && (controllerMask == OVRInput.Controller.LTouch || controllerMask == OVRInput.Controller.RTouch))
		{
			XRNode controllerNode = (controllerMask == OVRInput.Controller.LTouch) ? XRNode.LeftHand : XRNode.RightHand;
			OVRInput.StartVibration(amplitude, OVRInput.HAPTIC_VIBRATION_DURATION_SECONDS, controllerNode);
		}
	}

	public static void SetControllerLocalizedVibration(OVRInput.HapticsLocation hapticsLocationMask, float frequency, float amplitude, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
		{
			if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
			{
				controllerMask |= OVRInput.activeControllerType;
			}
			for (int i = 0; i < OVRInput.controllers.Count; i++)
			{
				OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
				if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
				{
					ovrcontrollerBase.SetControllerLocalizedVibration(hapticsLocationMask, frequency, amplitude);
				}
			}
			return;
		}
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR && (hapticsLocationMask & OVRInput.HapticsLocation.Hand) != OVRInput.HapticsLocation.None)
		{
			OVRInput.SetControllerVibration(frequency, amplitude, controllerMask);
		}
	}

	public static void SetControllerHapticsAmplitudeEnvelope(OVRInput.HapticsAmplitudeEnvelopeVibration hapticsVibration, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
		{
			controllerMask |= OVRInput.activeControllerType;
		}
		for (int i = 0; i < OVRInput.controllers.Count; i++)
		{
			OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
			if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
			{
				ovrcontrollerBase.SetControllerHapticsAmplitudeEnvelope(hapticsVibration);
			}
		}
	}

	public static int SetControllerHapticsPcm(OVRInput.HapticsPcmVibration hapticsVibration, OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
		{
			controllerMask |= OVRInput.activeControllerType;
		}
		int result = 0;
		for (int i = 0; i < OVRInput.controllers.Count; i++)
		{
			OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
			if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
			{
				result = ovrcontrollerBase.SetControllerHapticsPcm(hapticsVibration);
			}
		}
		return result;
	}

	public static float GetControllerSampleRateHz(OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
		{
			controllerMask |= OVRInput.activeControllerType;
		}
		for (int i = 0; i < OVRInput.controllers.Count; i++)
		{
			OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
			if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
			{
				return ovrcontrollerBase.GetControllerSampleRateHz();
			}
		}
		return 0f;
	}

	[Obsolete("Deprecated. The controller battery percentage data is no longer supported in OpenXR", false)]
	public static byte GetControllerBatteryPercentRemaining(OVRInput.Controller controllerMask = OVRInput.Controller.Active)
	{
		if ((controllerMask & OVRInput.Controller.Active) != OVRInput.Controller.None)
		{
			controllerMask |= OVRInput.activeControllerType;
		}
		byte result = 0;
		for (int i = 0; i < OVRInput.controllers.Count; i++)
		{
			OVRInput.OVRControllerBase ovrcontrollerBase = OVRInput.controllers[i];
			if (OVRInput.ShouldResolveController(ovrcontrollerBase.controllerType, controllerMask))
			{
				result = ovrcontrollerBase.GetBatteryPercentRemaining();
				break;
			}
		}
		return result;
	}

	private static Vector2 CalculateAbsMax(Vector2 a, Vector2 b)
	{
		float sqrMagnitude = a.sqrMagnitude;
		float sqrMagnitude2 = b.sqrMagnitude;
		if (sqrMagnitude >= sqrMagnitude2)
		{
			return a;
		}
		return b;
	}

	private static float CalculateAbsMax(float a, float b)
	{
		float num = (a >= 0f) ? a : (-a);
		float num2 = (b >= 0f) ? b : (-b);
		if (num >= num2)
		{
			return a;
		}
		return b;
	}

	private static Vector2 CalculateDeadzone(Vector2 a, float deadzone)
	{
		if (a.sqrMagnitude <= deadzone * deadzone)
		{
			return Vector2.zero;
		}
		a *= (a.magnitude - deadzone) / (1f - deadzone);
		if (a.sqrMagnitude > 1f)
		{
			return a.normalized;
		}
		return a;
	}

	private static float CalculateDeadzone(float a, float deadzone)
	{
		float num = (a >= 0f) ? a : (-a);
		if (num <= deadzone)
		{
			return 0f;
		}
		a *= (num - deadzone) / (1f - deadzone);
		if (a * a <= 1f)
		{
			return a;
		}
		if (a < 0f)
		{
			return -1f;
		}
		return 1f;
	}

	private static bool ShouldResolveController(OVRInput.Controller controllerType, OVRInput.Controller controllerMask)
	{
		bool result = false;
		if ((controllerType & controllerMask) == controllerType)
		{
			result = true;
		}
		if ((controllerMask & OVRInput.Controller.Touch) == OVRInput.Controller.Touch && (controllerType & OVRInput.Controller.Touch) != OVRInput.Controller.None && (controllerType & OVRInput.Controller.Touch) != OVRInput.Controller.Touch)
		{
			result = false;
		}
		if ((controllerMask & OVRInput.Controller.Hands) == OVRInput.Controller.Hands && (controllerType & OVRInput.Controller.Hands) != OVRInput.Controller.None && (controllerType & OVRInput.Controller.Hands) != OVRInput.Controller.Hands)
		{
			result = false;
		}
		return result;
	}

	public static readonly float AXIS_AS_BUTTON_THRESHOLD = 0.5f;

	public static readonly float AXIS_DEADZONE_THRESHOLD = 0.2f;

	public static List<OVRInput.OVRControllerBase> controllers;

	public static OVRInput.Controller activeControllerType = OVRInput.Controller.None;

	public static OVRInput.Controller connectedControllerTypes = OVRInput.Controller.None;

	public static OVRPlugin.Step stepType = OVRPlugin.Step.Render;

	public static int fixedUpdateCount = 0;

	private static bool _pluginSupportsActiveController = false;

	private static bool _pluginSupportsActiveControllerCached = false;

	private static Version _pluginSupportsActiveControllerMinVersion = new Version(1, 9, 0);

	private static int NUM_HAPTIC_CHANNELS = 2;

	private static OVRInput.HapticInfo[] hapticInfos;

	private static float OPENVR_MAX_HAPTIC_AMPLITUDE = 4000f;

	private static float HAPTIC_VIBRATION_DURATION_SECONDS = 2f;

	private static string OPENVR_TOUCH_NAME = "oculus_touch";

	private static string OPENVR_VIVE_CONTROLLER_NAME = "vive_controller";

	private static string OPENVR_WINDOWSMR_CONTROLLER_NAME = "holographic_controller";

	public static OVRInput.OpenVRControllerDetails[] openVRControllerDetails = new OVRInput.OpenVRControllerDetails[2];

	[Flags]
	public enum Button
	{
		None = 0,
		One = 1,
		Two = 2,
		Three = 4,
		Four = 8,
		Start = 256,
		Back = 512,
		PrimaryShoulder = 4096,
		PrimaryIndexTrigger = 8192,
		PrimaryHandTrigger = 16384,
		PrimaryThumbstick = 32768,
		PrimaryThumbstickUp = 65536,
		PrimaryThumbstickDown = 131072,
		PrimaryThumbstickLeft = 262144,
		PrimaryThumbstickRight = 524288,
		PrimaryTouchpad = 1024,
		SecondaryShoulder = 1048576,
		SecondaryIndexTrigger = 2097152,
		SecondaryHandTrigger = 4194304,
		SecondaryThumbstick = 8388608,
		SecondaryThumbstickUp = 16777216,
		SecondaryThumbstickDown = 33554432,
		SecondaryThumbstickLeft = 67108864,
		SecondaryThumbstickRight = 134217728,
		SecondaryTouchpad = 2048,
		DpadUp = 16,
		DpadDown = 32,
		DpadLeft = 64,
		DpadRight = 128,
		Up = 268435456,
		Down = 536870912,
		Left = 1073741824,
		Right = -2147483648,
		Any = -1
	}

	[Flags]
	public enum RawButton
	{
		None = 0,
		A = 1,
		B = 2,
		X = 256,
		Y = 512,
		Start = 1048576,
		Back = 2097152,
		LShoulder = 2048,
		LIndexTrigger = 268435456,
		LHandTrigger = 536870912,
		LThumbstick = 1024,
		LThumbstickUp = 16,
		LThumbstickDown = 32,
		LThumbstickLeft = 64,
		LThumbstickRight = 128,
		LTouchpad = 1073741824,
		RShoulder = 8,
		RIndexTrigger = 67108864,
		RHandTrigger = 134217728,
		RThumbstick = 4,
		RThumbstickUp = 4096,
		RThumbstickDown = 8192,
		RThumbstickLeft = 16384,
		RThumbstickRight = 32768,
		RTouchpad = -2147483648,
		DpadUp = 65536,
		DpadDown = 131072,
		DpadLeft = 262144,
		DpadRight = 524288,
		Any = -1
	}

	[Flags]
	public enum Touch
	{
		None = 0,
		One = 1,
		Two = 2,
		Three = 4,
		Four = 8,
		PrimaryIndexTrigger = 8192,
		PrimaryThumbstick = 32768,
		PrimaryThumbRest = 4096,
		PrimaryTouchpad = 1024,
		SecondaryIndexTrigger = 2097152,
		SecondaryThumbstick = 8388608,
		SecondaryThumbRest = 1048576,
		SecondaryTouchpad = 2048,
		Any = -1
	}

	[Flags]
	public enum RawTouch
	{
		None = 0,
		A = 1,
		B = 2,
		X = 256,
		Y = 512,
		LIndexTrigger = 4096,
		LThumbstick = 1024,
		LThumbRest = 2048,
		LTouchpad = 1073741824,
		RIndexTrigger = 16,
		RThumbstick = 4,
		RThumbRest = 8,
		RTouchpad = -2147483648,
		Any = -1
	}

	[Flags]
	public enum NearTouch
	{
		None = 0,
		PrimaryIndexTrigger = 1,
		PrimaryThumbButtons = 2,
		SecondaryIndexTrigger = 4,
		SecondaryThumbButtons = 8,
		Any = -1
	}

	[Flags]
	public enum RawNearTouch
	{
		None = 0,
		LIndexTrigger = 1,
		LThumbButtons = 2,
		RIndexTrigger = 4,
		RThumbButtons = 8,
		Any = -1
	}

	[Flags]
	public enum Axis1D
	{
		None = 0,
		PrimaryIndexTrigger = 1,
		PrimaryHandTrigger = 4,
		SecondaryIndexTrigger = 2,
		SecondaryHandTrigger = 8,
		PrimaryIndexTriggerCurl = 16,
		PrimaryIndexTriggerSlide = 32,
		PrimaryThumbRestForce = 64,
		PrimaryStylusForce = 128,
		SecondaryIndexTriggerCurl = 256,
		SecondaryIndexTriggerSlide = 512,
		SecondaryThumbRestForce = 1024,
		SecondaryStylusForce = 2048,
		PrimaryIndexTriggerForce = 4096,
		SecondaryIndexTriggerForce = 8192,
		Any = -1
	}

	[Flags]
	public enum RawAxis1D
	{
		None = 0,
		LIndexTrigger = 1,
		LHandTrigger = 4,
		RIndexTrigger = 2,
		RHandTrigger = 8,
		LIndexTriggerCurl = 16,
		LIndexTriggerSlide = 32,
		LThumbRestForce = 64,
		LStylusForce = 128,
		RIndexTriggerCurl = 256,
		RIndexTriggerSlide = 512,
		RThumbRestForce = 1024,
		RStylusForce = 2048,
		LIndexTriggerForce = 4096,
		RIndexTriggerForce = 8192,
		Any = -1
	}

	[Flags]
	public enum Axis2D
	{
		None = 0,
		PrimaryThumbstick = 1,
		PrimaryTouchpad = 4,
		SecondaryThumbstick = 2,
		SecondaryTouchpad = 8,
		Any = -1
	}

	[Flags]
	public enum RawAxis2D
	{
		None = 0,
		LThumbstick = 1,
		LTouchpad = 4,
		RThumbstick = 2,
		RTouchpad = 8,
		Any = -1
	}

	[Flags]
	public enum OpenVRButton : ulong
	{
		None = 0UL,
		Two = 2UL,
		Thumbstick = 4294967296UL,
		Grip = 4UL
	}

	[Flags]
	public enum Controller
	{
		None = 0,
		LTouch = 1,
		RTouch = 2,
		Touch = 3,
		Remote = 4,
		Gamepad = 16,
		Hands = 96,
		LHand = 32,
		RHand = 64,
		Active = -2147483648,
		All = -1
	}

	public enum Handedness
	{
		Unsupported,
		LeftHanded,
		RightHanded
	}

	public enum HapticsLocation
	{
		None,
		Hand,
		Thumb,
		Index = 4
	}

	public enum InteractionProfile
	{
		None,
		Touch,
		TouchPro,
		TouchPlus = 4
	}

	public enum Hand
	{
		None = -1,
		HandLeft,
		HandRight
	}

	public enum InputDeviceShowState
	{
		Always,
		ControllerInHandOrNoHand,
		ControllerInHand,
		ControllerNotInHand,
		NoHand
	}

	public enum ControllerInHandState
	{
		NoHand,
		ControllerInHand,
		ControllerNotInHand
	}

	public struct HapticsAmplitudeEnvelopeVibration
	{
		public int SamplesCount;

		public float[] Samples;

		public float Duration;
	}

	public struct HapticsPcmVibration
	{
		public int SamplesCount;

		public float[] Samples;

		public float SampleRateHz;

		public bool Append;
	}

	[Flags]
	public enum OpenVRController : ulong
	{
		Unknown = 0UL,
		OculusTouch = 1UL,
		ViveController = 2UL,
		WindowsMRController = 3UL
	}

	public struct OpenVRControllerDetails
	{
		public VRControllerState_t state;

		public OVRInput.OpenVRController controllerType;

		public uint deviceID;

		public Vector3 localPosition;

		public Quaternion localOrientation;
	}

	private class HapticInfo
	{
		public bool playingHaptics;

		public float hapticsDurationPlayed;

		public float hapticsDuration;

		public float hapticAmplitude;

		public XRNode node;
	}

	public abstract class OVRControllerBase
	{
		public OVRControllerBase()
		{
			this.ConfigureButtonMap();
			this.ConfigureTouchMap();
			this.ConfigureNearTouchMap();
			this.ConfigureAxis1DMap();
			this.ConfigureAxis2DMap();
		}

		public virtual OVRInput.Controller Update()
		{
			OVRPlugin.ControllerState6 controllerState;
			if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR && (this.controllerType & OVRInput.Controller.Touch) != OVRInput.Controller.None)
			{
				controllerState = this.GetOpenVRControllerState(this.controllerType);
			}
			else
			{
				controllerState = OVRPlugin.GetControllerState6((uint)this.controllerType);
			}
			if (controllerState.LIndexTrigger >= OVRInput.AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 268435456U;
			}
			if (controllerState.LHandTrigger >= OVRInput.AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 536870912U;
			}
			if (controllerState.LThumbstick.y >= OVRInput.AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 16U;
			}
			if (controllerState.LThumbstick.y <= -OVRInput.AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 32U;
			}
			if (controllerState.LThumbstick.x <= -OVRInput.AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 64U;
			}
			if (controllerState.LThumbstick.x >= OVRInput.AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 128U;
			}
			if (controllerState.RIndexTrigger >= OVRInput.AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 67108864U;
			}
			if (controllerState.RHandTrigger >= OVRInput.AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 134217728U;
			}
			if (controllerState.RThumbstick.y >= OVRInput.AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 4096U;
			}
			if (controllerState.RThumbstick.y <= -OVRInput.AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 8192U;
			}
			if (controllerState.RThumbstick.x <= -OVRInput.AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 16384U;
			}
			if (controllerState.RThumbstick.x >= OVRInput.AXIS_AS_BUTTON_THRESHOLD)
			{
				controllerState.Buttons |= 32768U;
			}
			this.previousState = this.currentState;
			this.currentState = controllerState;
			return (OVRInput.Controller)(this.currentState.ConnectedControllers & (uint)this.controllerType);
		}

		private OVRPlugin.ControllerState6 GetOpenVRControllerState(OVRInput.Controller controllerType)
		{
			OVRPlugin.ControllerState6 result = default(OVRPlugin.ControllerState6);
			if ((controllerType & OVRInput.Controller.LTouch) == OVRInput.Controller.LTouch && OVRInput.IsValidOpenVRDevice(OVRInput.openVRControllerDetails[0].deviceID))
			{
				VRControllerState_t state = OVRInput.openVRControllerDetails[0].state;
				if ((state.ulButtonPressed & 2UL) == 2UL)
				{
					result.Buttons |= 512U;
				}
				if ((state.ulButtonPressed & 4294967296UL) == 4294967296UL)
				{
					result.Buttons |= 1024U;
				}
				result.LIndexTrigger = state.rAxis1.x;
				if (OVRInput.openVRControllerDetails[0].controllerType == OVRInput.OpenVRController.OculusTouch || OVRInput.openVRControllerDetails[0].controllerType == OVRInput.OpenVRController.ViveController)
				{
					result.LThumbstick.x = state.rAxis0.x;
					result.LThumbstick.y = state.rAxis0.y;
				}
				else if (OVRInput.openVRControllerDetails[0].controllerType == OVRInput.OpenVRController.WindowsMRController)
				{
					result.LThumbstick.x = state.rAxis2.x;
					result.LThumbstick.y = state.rAxis2.y;
				}
				if (OVRInput.openVRControllerDetails[0].controllerType == OVRInput.OpenVRController.OculusTouch)
				{
					result.LHandTrigger = state.rAxis2.x;
				}
				else if (OVRInput.openVRControllerDetails[0].controllerType == OVRInput.OpenVRController.ViveController || OVRInput.openVRControllerDetails[0].controllerType == OVRInput.OpenVRController.WindowsMRController)
				{
					result.LHandTrigger = (float)(((state.ulButtonPressed & 4UL) == 4UL) ? 1 : 0);
				}
			}
			if ((controllerType & OVRInput.Controller.RTouch) == OVRInput.Controller.RTouch && OVRInput.IsValidOpenVRDevice(OVRInput.openVRControllerDetails[1].deviceID))
			{
				VRControllerState_t state2 = OVRInput.openVRControllerDetails[1].state;
				if ((state2.ulButtonPressed & 2UL) == 2UL)
				{
					result.Buttons |= 2U;
				}
				if ((state2.ulButtonPressed & 4294967296UL) == 4294967296UL)
				{
					result.Buttons |= 4U;
				}
				result.RIndexTrigger = state2.rAxis1.x;
				if (OVRInput.openVRControllerDetails[1].controllerType == OVRInput.OpenVRController.OculusTouch || OVRInput.openVRControllerDetails[1].controllerType == OVRInput.OpenVRController.ViveController)
				{
					result.RThumbstick.x = state2.rAxis0.x;
					result.RThumbstick.y = state2.rAxis0.y;
				}
				else if (OVRInput.openVRControllerDetails[1].controllerType == OVRInput.OpenVRController.WindowsMRController)
				{
					result.RThumbstick.x = state2.rAxis2.x;
					result.RThumbstick.y = state2.rAxis2.y;
				}
				if (OVRInput.openVRControllerDetails[1].controllerType == OVRInput.OpenVRController.OculusTouch)
				{
					result.RHandTrigger = state2.rAxis2.x;
				}
				else if (OVRInput.openVRControllerDetails[1].controllerType == OVRInput.OpenVRController.ViveController || OVRInput.openVRControllerDetails[1].controllerType == OVRInput.OpenVRController.WindowsMRController)
				{
					result.RHandTrigger = (float)(((state2.ulButtonPressed & 4UL) == 4UL) ? 1 : 0);
				}
			}
			return result;
		}

		public virtual void SetControllerVibration(float frequency, float amplitude)
		{
			OVRPlugin.SetControllerVibration((uint)this.controllerType, frequency, amplitude);
		}

		public virtual void SetControllerLocalizedVibration(OVRInput.HapticsLocation hapticsLocationMask, float frequency, float amplitude)
		{
			OVRPlugin.SetControllerLocalizedVibration((OVRPlugin.Controller)this.controllerType, (OVRPlugin.HapticsLocation)hapticsLocationMask, frequency, amplitude);
		}

		public virtual void SetControllerHapticsAmplitudeEnvelope(OVRInput.HapticsAmplitudeEnvelopeVibration hapticsVibration)
		{
			GCHandle gchandle = GCHandle.Alloc(hapticsVibration.Samples, GCHandleType.Pinned);
			try
			{
				OVRPlugin.HapticsAmplitudeEnvelopeVibration hapticsVibration2;
				hapticsVibration2.AmplitudeCount = (uint)hapticsVibration.SamplesCount;
				hapticsVibration2.Amplitudes = gchandle.AddrOfPinnedObject();
				hapticsVibration2.Duration = hapticsVibration.Duration;
				OVRPlugin.SetControllerHapticsAmplitudeEnvelope((OVRPlugin.Controller)this.controllerType, hapticsVibration2);
			}
			finally
			{
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
			}
		}

		public virtual int SetControllerHapticsPcm(OVRInput.HapticsPcmVibration hapticsVibration)
		{
			GCHandle gchandle = GCHandle.Alloc(hapticsVibration.Samples, GCHandleType.Pinned);
			GCHandle gchandle2 = GCHandle.Alloc(this.HapticsPcmSamplesConsumedCache, GCHandleType.Pinned);
			int result = 0;
			try
			{
				OVRPlugin.HapticsPcmVibration hapticsPcmVibration;
				hapticsPcmVibration.BufferSize = (uint)hapticsVibration.SamplesCount;
				hapticsPcmVibration.Buffer = gchandle.AddrOfPinnedObject();
				hapticsPcmVibration.SampleRateHz = hapticsVibration.SampleRateHz;
				hapticsPcmVibration.Append = (hapticsVibration.Append ? OVRPlugin.Bool.True : OVRPlugin.Bool.False);
				hapticsPcmVibration.SamplesConsumed = gchandle2.AddrOfPinnedObject();
				if (OVRPlugin.SetControllerHapticsPcm((OVRPlugin.Controller)this.controllerType, hapticsPcmVibration))
				{
					result = Marshal.ReadInt32(hapticsPcmVibration.SamplesConsumed);
				}
			}
			finally
			{
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
				if (gchandle2.IsAllocated)
				{
					gchandle2.Free();
				}
			}
			return result;
		}

		public virtual float GetControllerSampleRateHz()
		{
			float result;
			OVRPlugin.GetControllerSampleRateHz((OVRPlugin.Controller)this.controllerType, out result);
			return result;
		}

		[Obsolete("Deprecated. The controller battery percentage data is no longer supported in OpenXR", false)]
		public virtual byte GetBatteryPercentRemaining()
		{
			return 0;
		}

		public abstract void ConfigureButtonMap();

		public abstract void ConfigureTouchMap();

		public abstract void ConfigureNearTouchMap();

		public abstract void ConfigureAxis1DMap();

		public abstract void ConfigureAxis2DMap();

		public OVRInput.RawButton ResolveToRawMask(OVRInput.Button virtualMask)
		{
			return this.buttonMap.ToRawMask(virtualMask);
		}

		public OVRInput.RawTouch ResolveToRawMask(OVRInput.Touch virtualMask)
		{
			return this.touchMap.ToRawMask(virtualMask);
		}

		public OVRInput.RawNearTouch ResolveToRawMask(OVRInput.NearTouch virtualMask)
		{
			return this.nearTouchMap.ToRawMask(virtualMask);
		}

		public OVRInput.RawAxis1D ResolveToRawMask(OVRInput.Axis1D virtualMask)
		{
			return this.axis1DMap.ToRawMask(virtualMask);
		}

		public OVRInput.RawAxis2D ResolveToRawMask(OVRInput.Axis2D virtualMask)
		{
			return this.axis2DMap.ToRawMask(virtualMask);
		}

		public OVRInput.Controller controllerType;

		public OVRInput.OVRControllerBase.VirtualButtonMap buttonMap = new OVRInput.OVRControllerBase.VirtualButtonMap();

		public OVRInput.OVRControllerBase.VirtualTouchMap touchMap = new OVRInput.OVRControllerBase.VirtualTouchMap();

		public OVRInput.OVRControllerBase.VirtualNearTouchMap nearTouchMap = new OVRInput.OVRControllerBase.VirtualNearTouchMap();

		public OVRInput.OVRControllerBase.VirtualAxis1DMap axis1DMap = new OVRInput.OVRControllerBase.VirtualAxis1DMap();

		public OVRInput.OVRControllerBase.VirtualAxis2DMap axis2DMap = new OVRInput.OVRControllerBase.VirtualAxis2DMap();

		public OVRPlugin.ControllerState6 previousState;

		public OVRPlugin.ControllerState6 currentState;

		public bool shouldApplyDeadzone = true;

		private uint[] HapticsPcmSamplesConsumedCache = new uint[1];

		public class VirtualButtonMap
		{
			public OVRInput.RawButton ToRawMask(OVRInput.Button virtualMask)
			{
				OVRInput.RawButton rawButton = OVRInput.RawButton.None;
				if (virtualMask == OVRInput.Button.None)
				{
					return OVRInput.RawButton.None;
				}
				if ((virtualMask & OVRInput.Button.One) != OVRInput.Button.None)
				{
					rawButton |= this.One;
				}
				if ((virtualMask & OVRInput.Button.Two) != OVRInput.Button.None)
				{
					rawButton |= this.Two;
				}
				if ((virtualMask & OVRInput.Button.Three) != OVRInput.Button.None)
				{
					rawButton |= this.Three;
				}
				if ((virtualMask & OVRInput.Button.Four) != OVRInput.Button.None)
				{
					rawButton |= this.Four;
				}
				if ((virtualMask & OVRInput.Button.Start) != OVRInput.Button.None)
				{
					rawButton |= this.Start;
				}
				if ((virtualMask & OVRInput.Button.Back) != OVRInput.Button.None)
				{
					rawButton |= this.Back;
				}
				if ((virtualMask & OVRInput.Button.PrimaryShoulder) != OVRInput.Button.None)
				{
					rawButton |= this.PrimaryShoulder;
				}
				if ((virtualMask & OVRInput.Button.PrimaryIndexTrigger) != OVRInput.Button.None)
				{
					rawButton |= this.PrimaryIndexTrigger;
				}
				if ((virtualMask & OVRInput.Button.PrimaryHandTrigger) != OVRInput.Button.None)
				{
					rawButton |= this.PrimaryHandTrigger;
				}
				if ((virtualMask & OVRInput.Button.PrimaryThumbstick) != OVRInput.Button.None)
				{
					rawButton |= this.PrimaryThumbstick;
				}
				if ((virtualMask & OVRInput.Button.PrimaryThumbstickUp) != OVRInput.Button.None)
				{
					rawButton |= this.PrimaryThumbstickUp;
				}
				if ((virtualMask & OVRInput.Button.PrimaryThumbstickDown) != OVRInput.Button.None)
				{
					rawButton |= this.PrimaryThumbstickDown;
				}
				if ((virtualMask & OVRInput.Button.PrimaryThumbstickLeft) != OVRInput.Button.None)
				{
					rawButton |= this.PrimaryThumbstickLeft;
				}
				if ((virtualMask & OVRInput.Button.PrimaryThumbstickRight) != OVRInput.Button.None)
				{
					rawButton |= this.PrimaryThumbstickRight;
				}
				if ((virtualMask & OVRInput.Button.PrimaryTouchpad) != OVRInput.Button.None)
				{
					rawButton |= this.PrimaryTouchpad;
				}
				if ((virtualMask & OVRInput.Button.SecondaryShoulder) != OVRInput.Button.None)
				{
					rawButton |= this.SecondaryShoulder;
				}
				if ((virtualMask & OVRInput.Button.SecondaryIndexTrigger) != OVRInput.Button.None)
				{
					rawButton |= this.SecondaryIndexTrigger;
				}
				if ((virtualMask & OVRInput.Button.SecondaryHandTrigger) != OVRInput.Button.None)
				{
					rawButton |= this.SecondaryHandTrigger;
				}
				if ((virtualMask & OVRInput.Button.SecondaryThumbstick) != OVRInput.Button.None)
				{
					rawButton |= this.SecondaryThumbstick;
				}
				if ((virtualMask & OVRInput.Button.SecondaryThumbstickUp) != OVRInput.Button.None)
				{
					rawButton |= this.SecondaryThumbstickUp;
				}
				if ((virtualMask & OVRInput.Button.SecondaryThumbstickDown) != OVRInput.Button.None)
				{
					rawButton |= this.SecondaryThumbstickDown;
				}
				if ((virtualMask & OVRInput.Button.SecondaryThumbstickLeft) != OVRInput.Button.None)
				{
					rawButton |= this.SecondaryThumbstickLeft;
				}
				if ((virtualMask & OVRInput.Button.SecondaryThumbstickRight) != OVRInput.Button.None)
				{
					rawButton |= this.SecondaryThumbstickRight;
				}
				if ((virtualMask & OVRInput.Button.SecondaryTouchpad) != OVRInput.Button.None)
				{
					rawButton |= this.SecondaryTouchpad;
				}
				if ((virtualMask & OVRInput.Button.DpadUp) != OVRInput.Button.None)
				{
					rawButton |= this.DpadUp;
				}
				if ((virtualMask & OVRInput.Button.DpadDown) != OVRInput.Button.None)
				{
					rawButton |= this.DpadDown;
				}
				if ((virtualMask & OVRInput.Button.DpadLeft) != OVRInput.Button.None)
				{
					rawButton |= this.DpadLeft;
				}
				if ((virtualMask & OVRInput.Button.DpadRight) != OVRInput.Button.None)
				{
					rawButton |= this.DpadRight;
				}
				if ((virtualMask & OVRInput.Button.Up) != OVRInput.Button.None)
				{
					rawButton |= this.Up;
				}
				if ((virtualMask & OVRInput.Button.Down) != OVRInput.Button.None)
				{
					rawButton |= this.Down;
				}
				if ((virtualMask & OVRInput.Button.Left) != OVRInput.Button.None)
				{
					rawButton |= this.Left;
				}
				if ((virtualMask & OVRInput.Button.Right) != OVRInput.Button.None)
				{
					rawButton |= this.Right;
				}
				return rawButton;
			}

			public OVRInput.RawButton None;

			public OVRInput.RawButton One;

			public OVRInput.RawButton Two;

			public OVRInput.RawButton Three;

			public OVRInput.RawButton Four;

			public OVRInput.RawButton Start;

			public OVRInput.RawButton Back;

			public OVRInput.RawButton PrimaryShoulder;

			public OVRInput.RawButton PrimaryIndexTrigger;

			public OVRInput.RawButton PrimaryHandTrigger;

			public OVRInput.RawButton PrimaryThumbstick;

			public OVRInput.RawButton PrimaryThumbstickUp;

			public OVRInput.RawButton PrimaryThumbstickDown;

			public OVRInput.RawButton PrimaryThumbstickLeft;

			public OVRInput.RawButton PrimaryThumbstickRight;

			public OVRInput.RawButton PrimaryTouchpad;

			public OVRInput.RawButton SecondaryShoulder;

			public OVRInput.RawButton SecondaryIndexTrigger;

			public OVRInput.RawButton SecondaryHandTrigger;

			public OVRInput.RawButton SecondaryThumbstick;

			public OVRInput.RawButton SecondaryThumbstickUp;

			public OVRInput.RawButton SecondaryThumbstickDown;

			public OVRInput.RawButton SecondaryThumbstickLeft;

			public OVRInput.RawButton SecondaryThumbstickRight;

			public OVRInput.RawButton SecondaryTouchpad;

			public OVRInput.RawButton DpadUp;

			public OVRInput.RawButton DpadDown;

			public OVRInput.RawButton DpadLeft;

			public OVRInput.RawButton DpadRight;

			public OVRInput.RawButton Up;

			public OVRInput.RawButton Down;

			public OVRInput.RawButton Left;

			public OVRInput.RawButton Right;
		}

		public class VirtualTouchMap
		{
			public OVRInput.RawTouch ToRawMask(OVRInput.Touch virtualMask)
			{
				OVRInput.RawTouch rawTouch = OVRInput.RawTouch.None;
				if (virtualMask == OVRInput.Touch.None)
				{
					return OVRInput.RawTouch.None;
				}
				if ((virtualMask & OVRInput.Touch.One) != OVRInput.Touch.None)
				{
					rawTouch |= this.One;
				}
				if ((virtualMask & OVRInput.Touch.Two) != OVRInput.Touch.None)
				{
					rawTouch |= this.Two;
				}
				if ((virtualMask & OVRInput.Touch.Three) != OVRInput.Touch.None)
				{
					rawTouch |= this.Three;
				}
				if ((virtualMask & OVRInput.Touch.Four) != OVRInput.Touch.None)
				{
					rawTouch |= this.Four;
				}
				if ((virtualMask & OVRInput.Touch.PrimaryIndexTrigger) != OVRInput.Touch.None)
				{
					rawTouch |= this.PrimaryIndexTrigger;
				}
				if ((virtualMask & OVRInput.Touch.PrimaryThumbstick) != OVRInput.Touch.None)
				{
					rawTouch |= this.PrimaryThumbstick;
				}
				if ((virtualMask & OVRInput.Touch.PrimaryThumbRest) != OVRInput.Touch.None)
				{
					rawTouch |= this.PrimaryThumbRest;
				}
				if ((virtualMask & OVRInput.Touch.PrimaryTouchpad) != OVRInput.Touch.None)
				{
					rawTouch |= this.PrimaryTouchpad;
				}
				if ((virtualMask & OVRInput.Touch.SecondaryIndexTrigger) != OVRInput.Touch.None)
				{
					rawTouch |= this.SecondaryIndexTrigger;
				}
				if ((virtualMask & OVRInput.Touch.SecondaryThumbstick) != OVRInput.Touch.None)
				{
					rawTouch |= this.SecondaryThumbstick;
				}
				if ((virtualMask & OVRInput.Touch.SecondaryThumbRest) != OVRInput.Touch.None)
				{
					rawTouch |= this.SecondaryThumbRest;
				}
				if ((virtualMask & OVRInput.Touch.SecondaryTouchpad) != OVRInput.Touch.None)
				{
					rawTouch |= this.SecondaryTouchpad;
				}
				return rawTouch;
			}

			public OVRInput.RawTouch None;

			public OVRInput.RawTouch One;

			public OVRInput.RawTouch Two;

			public OVRInput.RawTouch Three;

			public OVRInput.RawTouch Four;

			public OVRInput.RawTouch PrimaryIndexTrigger;

			public OVRInput.RawTouch PrimaryThumbstick;

			public OVRInput.RawTouch PrimaryThumbRest;

			public OVRInput.RawTouch PrimaryTouchpad;

			public OVRInput.RawTouch SecondaryIndexTrigger;

			public OVRInput.RawTouch SecondaryThumbstick;

			public OVRInput.RawTouch SecondaryThumbRest;

			public OVRInput.RawTouch SecondaryTouchpad;
		}

		public class VirtualNearTouchMap
		{
			public OVRInput.RawNearTouch ToRawMask(OVRInput.NearTouch virtualMask)
			{
				OVRInput.RawNearTouch rawNearTouch = OVRInput.RawNearTouch.None;
				if (virtualMask == OVRInput.NearTouch.None)
				{
					return OVRInput.RawNearTouch.None;
				}
				if ((virtualMask & OVRInput.NearTouch.PrimaryIndexTrigger) != OVRInput.NearTouch.None)
				{
					rawNearTouch |= this.PrimaryIndexTrigger;
				}
				if ((virtualMask & OVRInput.NearTouch.PrimaryThumbButtons) != OVRInput.NearTouch.None)
				{
					rawNearTouch |= this.PrimaryThumbButtons;
				}
				if ((virtualMask & OVRInput.NearTouch.SecondaryIndexTrigger) != OVRInput.NearTouch.None)
				{
					rawNearTouch |= this.SecondaryIndexTrigger;
				}
				if ((virtualMask & OVRInput.NearTouch.SecondaryThumbButtons) != OVRInput.NearTouch.None)
				{
					rawNearTouch |= this.SecondaryThumbButtons;
				}
				return rawNearTouch;
			}

			public OVRInput.RawNearTouch None;

			public OVRInput.RawNearTouch PrimaryIndexTrigger;

			public OVRInput.RawNearTouch PrimaryThumbButtons;

			public OVRInput.RawNearTouch SecondaryIndexTrigger;

			public OVRInput.RawNearTouch SecondaryThumbButtons;
		}

		public class VirtualAxis1DMap
		{
			public OVRInput.RawAxis1D ToRawMask(OVRInput.Axis1D virtualMask)
			{
				OVRInput.RawAxis1D rawAxis1D = OVRInput.RawAxis1D.None;
				if (virtualMask == OVRInput.Axis1D.None)
				{
					return OVRInput.RawAxis1D.None;
				}
				if ((virtualMask & OVRInput.Axis1D.PrimaryIndexTrigger) != OVRInput.Axis1D.None)
				{
					rawAxis1D |= this.PrimaryIndexTrigger;
				}
				if ((virtualMask & OVRInput.Axis1D.PrimaryHandTrigger) != OVRInput.Axis1D.None)
				{
					rawAxis1D |= this.PrimaryHandTrigger;
				}
				if ((virtualMask & OVRInput.Axis1D.SecondaryIndexTrigger) != OVRInput.Axis1D.None)
				{
					rawAxis1D |= this.SecondaryIndexTrigger;
				}
				if ((virtualMask & OVRInput.Axis1D.SecondaryHandTrigger) != OVRInput.Axis1D.None)
				{
					rawAxis1D |= this.SecondaryHandTrigger;
				}
				if ((virtualMask & OVRInput.Axis1D.PrimaryIndexTriggerCurl) != OVRInput.Axis1D.None)
				{
					rawAxis1D |= this.PrimaryIndexTriggerCurl;
				}
				if ((virtualMask & OVRInput.Axis1D.PrimaryIndexTriggerSlide) != OVRInput.Axis1D.None)
				{
					rawAxis1D |= this.PrimaryIndexTriggerSlide;
				}
				if ((virtualMask & OVRInput.Axis1D.PrimaryThumbRestForce) != OVRInput.Axis1D.None)
				{
					rawAxis1D |= this.PrimaryThumbRestForce;
				}
				if ((virtualMask & OVRInput.Axis1D.PrimaryStylusForce) != OVRInput.Axis1D.None)
				{
					rawAxis1D |= this.PrimaryStylusForce;
				}
				if ((virtualMask & OVRInput.Axis1D.SecondaryIndexTriggerCurl) != OVRInput.Axis1D.None)
				{
					rawAxis1D |= this.SecondaryIndexTriggerCurl;
				}
				if ((virtualMask & OVRInput.Axis1D.SecondaryIndexTriggerSlide) != OVRInput.Axis1D.None)
				{
					rawAxis1D |= this.SecondaryIndexTriggerSlide;
				}
				if ((virtualMask & OVRInput.Axis1D.SecondaryThumbRestForce) != OVRInput.Axis1D.None)
				{
					rawAxis1D |= this.SecondaryThumbRestForce;
				}
				if ((virtualMask & OVRInput.Axis1D.SecondaryStylusForce) != OVRInput.Axis1D.None)
				{
					rawAxis1D |= this.SecondaryStylusForce;
				}
				if ((virtualMask & OVRInput.Axis1D.SecondaryIndexTriggerForce) != OVRInput.Axis1D.None)
				{
					rawAxis1D |= this.SecondaryIndexTriggerForce;
				}
				if ((virtualMask & OVRInput.Axis1D.PrimaryIndexTriggerForce) != OVRInput.Axis1D.None)
				{
					rawAxis1D |= this.PrimaryIndexTriggerForce;
				}
				return rawAxis1D;
			}

			public OVRInput.RawAxis1D None;

			public OVRInput.RawAxis1D PrimaryIndexTrigger;

			public OVRInput.RawAxis1D PrimaryHandTrigger;

			public OVRInput.RawAxis1D SecondaryIndexTrigger;

			public OVRInput.RawAxis1D SecondaryHandTrigger;

			public OVRInput.RawAxis1D PrimaryIndexTriggerCurl;

			public OVRInput.RawAxis1D PrimaryIndexTriggerSlide;

			public OVRInput.RawAxis1D PrimaryThumbRestForce;

			public OVRInput.RawAxis1D PrimaryStylusForce;

			public OVRInput.RawAxis1D SecondaryIndexTriggerCurl;

			public OVRInput.RawAxis1D SecondaryIndexTriggerSlide;

			public OVRInput.RawAxis1D SecondaryThumbRestForce;

			public OVRInput.RawAxis1D SecondaryStylusForce;

			public OVRInput.RawAxis1D PrimaryIndexTriggerForce;

			public OVRInput.RawAxis1D SecondaryIndexTriggerForce;
		}

		public class VirtualAxis2DMap
		{
			public OVRInput.RawAxis2D ToRawMask(OVRInput.Axis2D virtualMask)
			{
				OVRInput.RawAxis2D rawAxis2D = OVRInput.RawAxis2D.None;
				if (virtualMask == OVRInput.Axis2D.None)
				{
					return OVRInput.RawAxis2D.None;
				}
				if ((virtualMask & OVRInput.Axis2D.PrimaryThumbstick) != OVRInput.Axis2D.None)
				{
					rawAxis2D |= this.PrimaryThumbstick;
				}
				if ((virtualMask & OVRInput.Axis2D.PrimaryTouchpad) != OVRInput.Axis2D.None)
				{
					rawAxis2D |= this.PrimaryTouchpad;
				}
				if ((virtualMask & OVRInput.Axis2D.SecondaryThumbstick) != OVRInput.Axis2D.None)
				{
					rawAxis2D |= this.SecondaryThumbstick;
				}
				if ((virtualMask & OVRInput.Axis2D.SecondaryTouchpad) != OVRInput.Axis2D.None)
				{
					rawAxis2D |= this.SecondaryTouchpad;
				}
				return rawAxis2D;
			}

			public OVRInput.RawAxis2D None;

			public OVRInput.RawAxis2D PrimaryThumbstick;

			public OVRInput.RawAxis2D PrimaryTouchpad;

			public OVRInput.RawAxis2D SecondaryThumbstick;

			public OVRInput.RawAxis2D SecondaryTouchpad;
		}
	}

	public class OVRControllerTouch : OVRInput.OVRControllerBase
	{
		public OVRControllerTouch()
		{
			this.controllerType = OVRInput.Controller.Touch;
		}

		public override void ConfigureButtonMap()
		{
			this.buttonMap.None = OVRInput.RawButton.None;
			this.buttonMap.One = OVRInput.RawButton.A;
			this.buttonMap.Two = OVRInput.RawButton.B;
			this.buttonMap.Three = OVRInput.RawButton.X;
			this.buttonMap.Four = OVRInput.RawButton.Y;
			this.buttonMap.Start = OVRInput.RawButton.Start;
			this.buttonMap.Back = OVRInput.RawButton.None;
			this.buttonMap.PrimaryShoulder = OVRInput.RawButton.None;
			this.buttonMap.PrimaryIndexTrigger = OVRInput.RawButton.LIndexTrigger;
			this.buttonMap.PrimaryHandTrigger = OVRInput.RawButton.LHandTrigger;
			this.buttonMap.PrimaryThumbstick = OVRInput.RawButton.LThumbstick;
			this.buttonMap.PrimaryThumbstickUp = OVRInput.RawButton.LThumbstickUp;
			this.buttonMap.PrimaryThumbstickDown = OVRInput.RawButton.LThumbstickDown;
			this.buttonMap.PrimaryThumbstickLeft = OVRInput.RawButton.LThumbstickLeft;
			this.buttonMap.PrimaryThumbstickRight = OVRInput.RawButton.LThumbstickRight;
			this.buttonMap.PrimaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.SecondaryShoulder = OVRInput.RawButton.None;
			this.buttonMap.SecondaryIndexTrigger = OVRInput.RawButton.RIndexTrigger;
			this.buttonMap.SecondaryHandTrigger = OVRInput.RawButton.RHandTrigger;
			this.buttonMap.SecondaryThumbstick = OVRInput.RawButton.RThumbstick;
			this.buttonMap.SecondaryThumbstickUp = OVRInput.RawButton.RThumbstickUp;
			this.buttonMap.SecondaryThumbstickDown = OVRInput.RawButton.RThumbstickDown;
			this.buttonMap.SecondaryThumbstickLeft = OVRInput.RawButton.RThumbstickLeft;
			this.buttonMap.SecondaryThumbstickRight = OVRInput.RawButton.RThumbstickRight;
			this.buttonMap.SecondaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.DpadUp = OVRInput.RawButton.None;
			this.buttonMap.DpadDown = OVRInput.RawButton.None;
			this.buttonMap.DpadLeft = OVRInput.RawButton.None;
			this.buttonMap.DpadRight = OVRInput.RawButton.None;
			this.buttonMap.Up = OVRInput.RawButton.LThumbstickUp;
			this.buttonMap.Down = OVRInput.RawButton.LThumbstickDown;
			this.buttonMap.Left = OVRInput.RawButton.LThumbstickLeft;
			this.buttonMap.Right = OVRInput.RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			this.touchMap.None = OVRInput.RawTouch.None;
			this.touchMap.One = OVRInput.RawTouch.A;
			this.touchMap.Two = OVRInput.RawTouch.B;
			this.touchMap.Three = OVRInput.RawTouch.X;
			this.touchMap.Four = OVRInput.RawTouch.Y;
			this.touchMap.PrimaryIndexTrigger = OVRInput.RawTouch.LIndexTrigger;
			this.touchMap.PrimaryThumbstick = OVRInput.RawTouch.LThumbstick;
			this.touchMap.PrimaryThumbRest = OVRInput.RawTouch.LThumbRest;
			this.touchMap.PrimaryTouchpad = OVRInput.RawTouch.None;
			this.touchMap.SecondaryIndexTrigger = OVRInput.RawTouch.RIndexTrigger;
			this.touchMap.SecondaryThumbstick = OVRInput.RawTouch.RThumbstick;
			this.touchMap.SecondaryThumbRest = OVRInput.RawTouch.RThumbRest;
			this.touchMap.SecondaryTouchpad = OVRInput.RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			this.nearTouchMap.None = OVRInput.RawNearTouch.None;
			this.nearTouchMap.PrimaryIndexTrigger = OVRInput.RawNearTouch.LIndexTrigger;
			this.nearTouchMap.PrimaryThumbButtons = OVRInput.RawNearTouch.LThumbButtons;
			this.nearTouchMap.SecondaryIndexTrigger = OVRInput.RawNearTouch.RIndexTrigger;
			this.nearTouchMap.SecondaryThumbButtons = OVRInput.RawNearTouch.RThumbButtons;
		}

		public override void ConfigureAxis1DMap()
		{
			this.axis1DMap.None = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTrigger = OVRInput.RawAxis1D.LIndexTrigger;
			this.axis1DMap.PrimaryHandTrigger = OVRInput.RawAxis1D.LHandTrigger;
			this.axis1DMap.SecondaryIndexTrigger = OVRInput.RawAxis1D.RIndexTrigger;
			this.axis1DMap.SecondaryHandTrigger = OVRInput.RawAxis1D.RHandTrigger;
			this.axis1DMap.PrimaryIndexTriggerCurl = OVRInput.RawAxis1D.LIndexTriggerCurl;
			this.axis1DMap.PrimaryIndexTriggerSlide = OVRInput.RawAxis1D.LIndexTriggerSlide;
			this.axis1DMap.PrimaryThumbRestForce = OVRInput.RawAxis1D.LThumbRestForce;
			this.axis1DMap.PrimaryStylusForce = OVRInput.RawAxis1D.LStylusForce;
			this.axis1DMap.SecondaryIndexTriggerCurl = OVRInput.RawAxis1D.RIndexTriggerCurl;
			this.axis1DMap.SecondaryIndexTriggerSlide = OVRInput.RawAxis1D.RIndexTriggerSlide;
			this.axis1DMap.SecondaryThumbRestForce = OVRInput.RawAxis1D.RThumbRestForce;
			this.axis1DMap.SecondaryStylusForce = OVRInput.RawAxis1D.RStylusForce;
			this.axis1DMap.PrimaryIndexTriggerForce = OVRInput.RawAxis1D.LIndexTriggerForce;
			this.axis1DMap.SecondaryIndexTriggerForce = OVRInput.RawAxis1D.RIndexTriggerForce;
		}

		public override void ConfigureAxis2DMap()
		{
			this.axis2DMap.None = OVRInput.RawAxis2D.None;
			this.axis2DMap.PrimaryThumbstick = OVRInput.RawAxis2D.LThumbstick;
			this.axis2DMap.PrimaryTouchpad = OVRInput.RawAxis2D.LTouchpad;
			this.axis2DMap.SecondaryThumbstick = OVRInput.RawAxis2D.RThumbstick;
			this.axis2DMap.SecondaryTouchpad = OVRInput.RawAxis2D.RTouchpad;
		}

		[Obsolete("Deprecated. The controller battery percentage data is no longer supported in OpenXR", false)]
		public override byte GetBatteryPercentRemaining()
		{
			byte lbatteryPercentRemaining = this.currentState.LBatteryPercentRemaining;
			byte rbatteryPercentRemaining = this.currentState.RBatteryPercentRemaining;
			if (lbatteryPercentRemaining > rbatteryPercentRemaining)
			{
				return rbatteryPercentRemaining;
			}
			return lbatteryPercentRemaining;
		}
	}

	public class OVRControllerLTouch : OVRInput.OVRControllerBase
	{
		public OVRControllerLTouch()
		{
			this.controllerType = OVRInput.Controller.LTouch;
		}

		public override void ConfigureButtonMap()
		{
			this.buttonMap.None = OVRInput.RawButton.None;
			this.buttonMap.One = OVRInput.RawButton.X;
			this.buttonMap.Two = OVRInput.RawButton.Y;
			this.buttonMap.Three = OVRInput.RawButton.None;
			this.buttonMap.Four = OVRInput.RawButton.None;
			this.buttonMap.Start = OVRInput.RawButton.Start;
			this.buttonMap.Back = OVRInput.RawButton.None;
			this.buttonMap.PrimaryShoulder = OVRInput.RawButton.None;
			this.buttonMap.PrimaryIndexTrigger = OVRInput.RawButton.LIndexTrigger;
			this.buttonMap.PrimaryHandTrigger = OVRInput.RawButton.LHandTrigger;
			this.buttonMap.PrimaryThumbstick = OVRInput.RawButton.LThumbstick;
			this.buttonMap.PrimaryThumbstickUp = OVRInput.RawButton.LThumbstickUp;
			this.buttonMap.PrimaryThumbstickDown = OVRInput.RawButton.LThumbstickDown;
			this.buttonMap.PrimaryThumbstickLeft = OVRInput.RawButton.LThumbstickLeft;
			this.buttonMap.PrimaryThumbstickRight = OVRInput.RawButton.LThumbstickRight;
			this.buttonMap.PrimaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.SecondaryShoulder = OVRInput.RawButton.None;
			this.buttonMap.SecondaryIndexTrigger = OVRInput.RawButton.None;
			this.buttonMap.SecondaryHandTrigger = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstick = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickUp = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickDown = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickLeft = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickRight = OVRInput.RawButton.None;
			this.buttonMap.SecondaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.DpadUp = OVRInput.RawButton.None;
			this.buttonMap.DpadDown = OVRInput.RawButton.None;
			this.buttonMap.DpadLeft = OVRInput.RawButton.None;
			this.buttonMap.DpadRight = OVRInput.RawButton.None;
			this.buttonMap.Up = OVRInput.RawButton.LThumbstickUp;
			this.buttonMap.Down = OVRInput.RawButton.LThumbstickDown;
			this.buttonMap.Left = OVRInput.RawButton.LThumbstickLeft;
			this.buttonMap.Right = OVRInput.RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			this.touchMap.None = OVRInput.RawTouch.None;
			this.touchMap.One = OVRInput.RawTouch.X;
			this.touchMap.Two = OVRInput.RawTouch.Y;
			this.touchMap.Three = OVRInput.RawTouch.None;
			this.touchMap.Four = OVRInput.RawTouch.None;
			this.touchMap.PrimaryIndexTrigger = OVRInput.RawTouch.LIndexTrigger;
			this.touchMap.PrimaryThumbstick = OVRInput.RawTouch.LThumbstick;
			this.touchMap.PrimaryThumbRest = OVRInput.RawTouch.LThumbRest;
			this.touchMap.PrimaryTouchpad = OVRInput.RawTouch.None;
			this.touchMap.SecondaryIndexTrigger = OVRInput.RawTouch.None;
			this.touchMap.SecondaryThumbstick = OVRInput.RawTouch.None;
			this.touchMap.SecondaryThumbRest = OVRInput.RawTouch.None;
			this.touchMap.SecondaryTouchpad = OVRInput.RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			this.nearTouchMap.None = OVRInput.RawNearTouch.None;
			this.nearTouchMap.PrimaryIndexTrigger = OVRInput.RawNearTouch.LIndexTrigger;
			this.nearTouchMap.PrimaryThumbButtons = OVRInput.RawNearTouch.LThumbButtons;
			this.nearTouchMap.SecondaryIndexTrigger = OVRInput.RawNearTouch.None;
			this.nearTouchMap.SecondaryThumbButtons = OVRInput.RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			this.axis1DMap.None = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTrigger = OVRInput.RawAxis1D.LIndexTrigger;
			this.axis1DMap.PrimaryHandTrigger = OVRInput.RawAxis1D.LHandTrigger;
			this.axis1DMap.SecondaryIndexTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryHandTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerCurl = OVRInput.RawAxis1D.LIndexTriggerCurl;
			this.axis1DMap.PrimaryIndexTriggerSlide = OVRInput.RawAxis1D.LIndexTriggerSlide;
			this.axis1DMap.PrimaryThumbRestForce = OVRInput.RawAxis1D.LThumbRestForce;
			this.axis1DMap.PrimaryStylusForce = OVRInput.RawAxis1D.LStylusForce;
			this.axis1DMap.SecondaryIndexTriggerCurl = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerSlide = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryThumbRestForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryStylusForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerForce = OVRInput.RawAxis1D.LIndexTriggerForce;
			this.axis1DMap.SecondaryIndexTriggerForce = OVRInput.RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			this.axis2DMap.None = OVRInput.RawAxis2D.None;
			this.axis2DMap.PrimaryThumbstick = OVRInput.RawAxis2D.LThumbstick;
			this.axis2DMap.PrimaryTouchpad = OVRInput.RawAxis2D.LTouchpad;
			this.axis2DMap.SecondaryThumbstick = OVRInput.RawAxis2D.None;
			this.axis2DMap.SecondaryTouchpad = OVRInput.RawAxis2D.None;
		}

		[Obsolete("Deprecated. The controller battery percentage data is no longer supported in OpenXR", false)]
		public override byte GetBatteryPercentRemaining()
		{
			return this.currentState.LBatteryPercentRemaining;
		}
	}

	private class OVRControllerRTouch : OVRInput.OVRControllerBase
	{
		public OVRControllerRTouch()
		{
			this.controllerType = OVRInput.Controller.RTouch;
		}

		public override void ConfigureButtonMap()
		{
			this.buttonMap.None = OVRInput.RawButton.None;
			this.buttonMap.One = OVRInput.RawButton.A;
			this.buttonMap.Two = OVRInput.RawButton.B;
			this.buttonMap.Three = OVRInput.RawButton.None;
			this.buttonMap.Four = OVRInput.RawButton.None;
			this.buttonMap.Start = OVRInput.RawButton.Start;
			this.buttonMap.Back = OVRInput.RawButton.None;
			this.buttonMap.PrimaryShoulder = OVRInput.RawButton.None;
			this.buttonMap.PrimaryIndexTrigger = OVRInput.RawButton.RIndexTrigger;
			this.buttonMap.PrimaryHandTrigger = OVRInput.RawButton.RHandTrigger;
			this.buttonMap.PrimaryThumbstick = OVRInput.RawButton.RThumbstick;
			this.buttonMap.PrimaryThumbstickUp = OVRInput.RawButton.RThumbstickUp;
			this.buttonMap.PrimaryThumbstickDown = OVRInput.RawButton.RThumbstickDown;
			this.buttonMap.PrimaryThumbstickLeft = OVRInput.RawButton.RThumbstickLeft;
			this.buttonMap.PrimaryThumbstickRight = OVRInput.RawButton.RThumbstickRight;
			this.buttonMap.PrimaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.SecondaryShoulder = OVRInput.RawButton.None;
			this.buttonMap.SecondaryIndexTrigger = OVRInput.RawButton.None;
			this.buttonMap.SecondaryHandTrigger = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstick = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickUp = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickDown = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickLeft = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickRight = OVRInput.RawButton.None;
			this.buttonMap.SecondaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.DpadUp = OVRInput.RawButton.None;
			this.buttonMap.DpadDown = OVRInput.RawButton.None;
			this.buttonMap.DpadLeft = OVRInput.RawButton.None;
			this.buttonMap.DpadRight = OVRInput.RawButton.None;
			this.buttonMap.Up = OVRInput.RawButton.RThumbstickUp;
			this.buttonMap.Down = OVRInput.RawButton.RThumbstickDown;
			this.buttonMap.Left = OVRInput.RawButton.RThumbstickLeft;
			this.buttonMap.Right = OVRInput.RawButton.RThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			this.touchMap.None = OVRInput.RawTouch.None;
			this.touchMap.One = OVRInput.RawTouch.A;
			this.touchMap.Two = OVRInput.RawTouch.B;
			this.touchMap.Three = OVRInput.RawTouch.None;
			this.touchMap.Four = OVRInput.RawTouch.None;
			this.touchMap.PrimaryIndexTrigger = OVRInput.RawTouch.RIndexTrigger;
			this.touchMap.PrimaryThumbstick = OVRInput.RawTouch.RThumbstick;
			this.touchMap.PrimaryThumbRest = OVRInput.RawTouch.RThumbRest;
			this.touchMap.PrimaryTouchpad = OVRInput.RawTouch.None;
			this.touchMap.SecondaryIndexTrigger = OVRInput.RawTouch.None;
			this.touchMap.SecondaryThumbstick = OVRInput.RawTouch.None;
			this.touchMap.SecondaryThumbRest = OVRInput.RawTouch.None;
			this.touchMap.SecondaryTouchpad = OVRInput.RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			this.nearTouchMap.None = OVRInput.RawNearTouch.None;
			this.nearTouchMap.PrimaryIndexTrigger = OVRInput.RawNearTouch.RIndexTrigger;
			this.nearTouchMap.PrimaryThumbButtons = OVRInput.RawNearTouch.RThumbButtons;
			this.nearTouchMap.SecondaryIndexTrigger = OVRInput.RawNearTouch.None;
			this.nearTouchMap.SecondaryThumbButtons = OVRInput.RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			this.axis1DMap.None = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTrigger = OVRInput.RawAxis1D.RIndexTrigger;
			this.axis1DMap.PrimaryHandTrigger = OVRInput.RawAxis1D.RHandTrigger;
			this.axis1DMap.SecondaryIndexTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryHandTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerCurl = OVRInput.RawAxis1D.RIndexTriggerCurl;
			this.axis1DMap.PrimaryIndexTriggerSlide = OVRInput.RawAxis1D.RIndexTriggerSlide;
			this.axis1DMap.PrimaryThumbRestForce = OVRInput.RawAxis1D.RThumbRestForce;
			this.axis1DMap.PrimaryStylusForce = OVRInput.RawAxis1D.RStylusForce;
			this.axis1DMap.SecondaryIndexTriggerCurl = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerSlide = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryThumbRestForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryStylusForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerForce = OVRInput.RawAxis1D.RIndexTriggerForce;
			this.axis1DMap.SecondaryIndexTriggerForce = OVRInput.RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			this.axis2DMap.None = OVRInput.RawAxis2D.None;
			this.axis2DMap.PrimaryThumbstick = OVRInput.RawAxis2D.RThumbstick;
			this.axis2DMap.PrimaryTouchpad = OVRInput.RawAxis2D.RTouchpad;
			this.axis2DMap.SecondaryThumbstick = OVRInput.RawAxis2D.None;
			this.axis2DMap.SecondaryTouchpad = OVRInput.RawAxis2D.None;
		}

		[Obsolete("Deprecated. The controller battery percentage data is no longer supported in OpenXR", false)]
		public override byte GetBatteryPercentRemaining()
		{
			return this.currentState.RBatteryPercentRemaining;
		}
	}

	public class OVRControllerHands : OVRInput.OVRControllerBase
	{
		public OVRControllerHands()
		{
			this.controllerType = OVRInput.Controller.Hands;
		}

		public override void ConfigureButtonMap()
		{
			this.buttonMap.None = OVRInput.RawButton.None;
			this.buttonMap.One = OVRInput.RawButton.A;
			this.buttonMap.Two = OVRInput.RawButton.None;
			this.buttonMap.Three = OVRInput.RawButton.X;
			this.buttonMap.Four = OVRInput.RawButton.None;
			this.buttonMap.Start = OVRInput.RawButton.Start;
			this.buttonMap.Back = OVRInput.RawButton.None;
			this.buttonMap.PrimaryShoulder = OVRInput.RawButton.None;
			this.buttonMap.PrimaryIndexTrigger = OVRInput.RawButton.None;
			this.buttonMap.PrimaryHandTrigger = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstick = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstickUp = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstickDown = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstickLeft = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstickRight = OVRInput.RawButton.None;
			this.buttonMap.PrimaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.SecondaryShoulder = OVRInput.RawButton.None;
			this.buttonMap.SecondaryIndexTrigger = OVRInput.RawButton.None;
			this.buttonMap.SecondaryHandTrigger = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstick = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickUp = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickDown = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickLeft = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickRight = OVRInput.RawButton.None;
			this.buttonMap.SecondaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.DpadUp = OVRInput.RawButton.None;
			this.buttonMap.DpadDown = OVRInput.RawButton.None;
			this.buttonMap.DpadLeft = OVRInput.RawButton.None;
			this.buttonMap.DpadRight = OVRInput.RawButton.None;
			this.buttonMap.Up = OVRInput.RawButton.None;
			this.buttonMap.Down = OVRInput.RawButton.None;
			this.buttonMap.Left = OVRInput.RawButton.None;
			this.buttonMap.Right = OVRInput.RawButton.None;
		}

		public override void ConfigureTouchMap()
		{
			this.touchMap.None = OVRInput.RawTouch.None;
			this.touchMap.One = OVRInput.RawTouch.None;
			this.touchMap.Two = OVRInput.RawTouch.None;
			this.touchMap.Three = OVRInput.RawTouch.None;
			this.touchMap.Four = OVRInput.RawTouch.None;
			this.touchMap.PrimaryIndexTrigger = OVRInput.RawTouch.None;
			this.touchMap.PrimaryThumbstick = OVRInput.RawTouch.None;
			this.touchMap.PrimaryThumbRest = OVRInput.RawTouch.None;
			this.touchMap.PrimaryTouchpad = OVRInput.RawTouch.None;
			this.touchMap.SecondaryIndexTrigger = OVRInput.RawTouch.None;
			this.touchMap.SecondaryThumbstick = OVRInput.RawTouch.None;
			this.touchMap.SecondaryThumbRest = OVRInput.RawTouch.None;
			this.touchMap.SecondaryTouchpad = OVRInput.RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			this.nearTouchMap.None = OVRInput.RawNearTouch.None;
			this.nearTouchMap.PrimaryIndexTrigger = OVRInput.RawNearTouch.None;
			this.nearTouchMap.PrimaryThumbButtons = OVRInput.RawNearTouch.None;
			this.nearTouchMap.SecondaryIndexTrigger = OVRInput.RawNearTouch.None;
			this.nearTouchMap.SecondaryThumbButtons = OVRInput.RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			this.axis1DMap.None = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryHandTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryHandTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerCurl = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerSlide = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryThumbRestForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryStylusForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerCurl = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerSlide = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryThumbRestForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryStylusForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerForce = OVRInput.RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			this.axis2DMap.None = OVRInput.RawAxis2D.None;
			this.axis2DMap.PrimaryThumbstick = OVRInput.RawAxis2D.None;
			this.axis2DMap.PrimaryTouchpad = OVRInput.RawAxis2D.None;
			this.axis2DMap.SecondaryThumbstick = OVRInput.RawAxis2D.None;
			this.axis2DMap.SecondaryTouchpad = OVRInput.RawAxis2D.None;
		}

		[Obsolete("Deprecated. The controller battery percentage data is no longer supported in OpenXR", false)]
		public override byte GetBatteryPercentRemaining()
		{
			byte lbatteryPercentRemaining = this.currentState.LBatteryPercentRemaining;
			byte rbatteryPercentRemaining = this.currentState.RBatteryPercentRemaining;
			if (lbatteryPercentRemaining > rbatteryPercentRemaining)
			{
				return rbatteryPercentRemaining;
			}
			return lbatteryPercentRemaining;
		}
	}

	public class OVRControllerLHand : OVRInput.OVRControllerBase
	{
		public OVRControllerLHand()
		{
			this.controllerType = OVRInput.Controller.LHand;
		}

		public override void ConfigureButtonMap()
		{
			this.buttonMap.None = OVRInput.RawButton.None;
			this.buttonMap.One = OVRInput.RawButton.X;
			this.buttonMap.Two = OVRInput.RawButton.None;
			this.buttonMap.Three = OVRInput.RawButton.None;
			this.buttonMap.Four = OVRInput.RawButton.None;
			this.buttonMap.Start = OVRInput.RawButton.Start;
			this.buttonMap.Back = OVRInput.RawButton.None;
			this.buttonMap.PrimaryShoulder = OVRInput.RawButton.None;
			this.buttonMap.PrimaryIndexTrigger = OVRInput.RawButton.None;
			this.buttonMap.PrimaryHandTrigger = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstick = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstickUp = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstickDown = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstickLeft = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstickRight = OVRInput.RawButton.None;
			this.buttonMap.PrimaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.SecondaryShoulder = OVRInput.RawButton.None;
			this.buttonMap.SecondaryIndexTrigger = OVRInput.RawButton.None;
			this.buttonMap.SecondaryHandTrigger = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstick = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickUp = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickDown = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickLeft = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickRight = OVRInput.RawButton.None;
			this.buttonMap.SecondaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.DpadUp = OVRInput.RawButton.None;
			this.buttonMap.DpadDown = OVRInput.RawButton.None;
			this.buttonMap.DpadLeft = OVRInput.RawButton.None;
			this.buttonMap.DpadRight = OVRInput.RawButton.None;
			this.buttonMap.Up = OVRInput.RawButton.None;
			this.buttonMap.Down = OVRInput.RawButton.None;
			this.buttonMap.Left = OVRInput.RawButton.None;
			this.buttonMap.Right = OVRInput.RawButton.None;
		}

		public override void ConfigureTouchMap()
		{
			this.touchMap.None = OVRInput.RawTouch.None;
			this.touchMap.One = OVRInput.RawTouch.None;
			this.touchMap.Two = OVRInput.RawTouch.None;
			this.touchMap.Three = OVRInput.RawTouch.None;
			this.touchMap.Four = OVRInput.RawTouch.None;
			this.touchMap.PrimaryIndexTrigger = OVRInput.RawTouch.None;
			this.touchMap.PrimaryThumbstick = OVRInput.RawTouch.None;
			this.touchMap.PrimaryThumbRest = OVRInput.RawTouch.None;
			this.touchMap.PrimaryTouchpad = OVRInput.RawTouch.None;
			this.touchMap.SecondaryIndexTrigger = OVRInput.RawTouch.None;
			this.touchMap.SecondaryThumbstick = OVRInput.RawTouch.None;
			this.touchMap.SecondaryThumbRest = OVRInput.RawTouch.None;
			this.touchMap.SecondaryTouchpad = OVRInput.RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			this.nearTouchMap.None = OVRInput.RawNearTouch.None;
			this.nearTouchMap.PrimaryIndexTrigger = OVRInput.RawNearTouch.None;
			this.nearTouchMap.PrimaryThumbButtons = OVRInput.RawNearTouch.None;
			this.nearTouchMap.SecondaryIndexTrigger = OVRInput.RawNearTouch.None;
			this.nearTouchMap.SecondaryThumbButtons = OVRInput.RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			this.axis1DMap.None = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryHandTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryHandTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerCurl = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerSlide = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryThumbRestForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryStylusForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerCurl = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerSlide = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryThumbRestForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryStylusForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerForce = OVRInput.RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			this.axis2DMap.None = OVRInput.RawAxis2D.None;
			this.axis2DMap.PrimaryThumbstick = OVRInput.RawAxis2D.None;
			this.axis2DMap.PrimaryTouchpad = OVRInput.RawAxis2D.None;
			this.axis2DMap.SecondaryThumbstick = OVRInput.RawAxis2D.None;
			this.axis2DMap.SecondaryTouchpad = OVRInput.RawAxis2D.None;
		}

		[Obsolete("Deprecated. The controller battery percentage data is no longer supported in OpenXR", false)]
		public override byte GetBatteryPercentRemaining()
		{
			return this.currentState.LBatteryPercentRemaining;
		}
	}

	public class OVRControllerRHand : OVRInput.OVRControllerBase
	{
		public OVRControllerRHand()
		{
			this.controllerType = OVRInput.Controller.RHand;
		}

		public override void ConfigureButtonMap()
		{
			this.buttonMap.None = OVRInput.RawButton.None;
			this.buttonMap.One = OVRInput.RawButton.A;
			this.buttonMap.Two = OVRInput.RawButton.None;
			this.buttonMap.Three = OVRInput.RawButton.None;
			this.buttonMap.Four = OVRInput.RawButton.None;
			this.buttonMap.Start = OVRInput.RawButton.Start;
			this.buttonMap.Back = OVRInput.RawButton.None;
			this.buttonMap.PrimaryShoulder = OVRInput.RawButton.None;
			this.buttonMap.PrimaryIndexTrigger = OVRInput.RawButton.None;
			this.buttonMap.PrimaryHandTrigger = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstick = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstickUp = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstickDown = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstickLeft = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstickRight = OVRInput.RawButton.None;
			this.buttonMap.PrimaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.SecondaryShoulder = OVRInput.RawButton.None;
			this.buttonMap.SecondaryIndexTrigger = OVRInput.RawButton.None;
			this.buttonMap.SecondaryHandTrigger = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstick = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickUp = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickDown = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickLeft = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickRight = OVRInput.RawButton.None;
			this.buttonMap.SecondaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.DpadUp = OVRInput.RawButton.None;
			this.buttonMap.DpadDown = OVRInput.RawButton.None;
			this.buttonMap.DpadLeft = OVRInput.RawButton.None;
			this.buttonMap.DpadRight = OVRInput.RawButton.None;
			this.buttonMap.Up = OVRInput.RawButton.None;
			this.buttonMap.Down = OVRInput.RawButton.None;
			this.buttonMap.Left = OVRInput.RawButton.None;
			this.buttonMap.Right = OVRInput.RawButton.None;
		}

		public override void ConfigureTouchMap()
		{
			this.touchMap.None = OVRInput.RawTouch.None;
			this.touchMap.One = OVRInput.RawTouch.None;
			this.touchMap.Two = OVRInput.RawTouch.None;
			this.touchMap.Three = OVRInput.RawTouch.None;
			this.touchMap.Four = OVRInput.RawTouch.None;
			this.touchMap.PrimaryIndexTrigger = OVRInput.RawTouch.None;
			this.touchMap.PrimaryThumbstick = OVRInput.RawTouch.None;
			this.touchMap.PrimaryThumbRest = OVRInput.RawTouch.None;
			this.touchMap.PrimaryTouchpad = OVRInput.RawTouch.None;
			this.touchMap.SecondaryIndexTrigger = OVRInput.RawTouch.None;
			this.touchMap.SecondaryThumbstick = OVRInput.RawTouch.None;
			this.touchMap.SecondaryThumbRest = OVRInput.RawTouch.None;
			this.touchMap.SecondaryTouchpad = OVRInput.RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			this.nearTouchMap.None = OVRInput.RawNearTouch.None;
			this.nearTouchMap.PrimaryIndexTrigger = OVRInput.RawNearTouch.None;
			this.nearTouchMap.PrimaryThumbButtons = OVRInput.RawNearTouch.None;
			this.nearTouchMap.SecondaryIndexTrigger = OVRInput.RawNearTouch.None;
			this.nearTouchMap.SecondaryThumbButtons = OVRInput.RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			this.axis1DMap.None = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryHandTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryHandTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerCurl = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerSlide = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryThumbRestForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryStylusForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerCurl = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerSlide = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryThumbRestForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryStylusForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerForce = OVRInput.RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			this.axis2DMap.None = OVRInput.RawAxis2D.None;
			this.axis2DMap.PrimaryThumbstick = OVRInput.RawAxis2D.None;
			this.axis2DMap.PrimaryTouchpad = OVRInput.RawAxis2D.None;
			this.axis2DMap.SecondaryThumbstick = OVRInput.RawAxis2D.None;
			this.axis2DMap.SecondaryTouchpad = OVRInput.RawAxis2D.None;
		}

		[Obsolete("Deprecated. The controller battery percentage data is no longer supported in OpenXR", false)]
		public override byte GetBatteryPercentRemaining()
		{
			return this.currentState.RBatteryPercentRemaining;
		}
	}

	public class OVRControllerRemote : OVRInput.OVRControllerBase
	{
		public OVRControllerRemote()
		{
			this.controllerType = OVRInput.Controller.Remote;
		}

		public override void ConfigureButtonMap()
		{
			this.buttonMap.None = OVRInput.RawButton.None;
			this.buttonMap.One = OVRInput.RawButton.Start;
			this.buttonMap.Two = OVRInput.RawButton.Back;
			this.buttonMap.Three = OVRInput.RawButton.None;
			this.buttonMap.Four = OVRInput.RawButton.None;
			this.buttonMap.Start = OVRInput.RawButton.Start;
			this.buttonMap.Back = OVRInput.RawButton.Back;
			this.buttonMap.PrimaryShoulder = OVRInput.RawButton.None;
			this.buttonMap.PrimaryIndexTrigger = OVRInput.RawButton.None;
			this.buttonMap.PrimaryHandTrigger = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstick = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstickUp = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstickDown = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstickLeft = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstickRight = OVRInput.RawButton.None;
			this.buttonMap.PrimaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.SecondaryShoulder = OVRInput.RawButton.None;
			this.buttonMap.SecondaryIndexTrigger = OVRInput.RawButton.None;
			this.buttonMap.SecondaryHandTrigger = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstick = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickUp = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickDown = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickLeft = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstickRight = OVRInput.RawButton.None;
			this.buttonMap.SecondaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.DpadUp = OVRInput.RawButton.DpadUp;
			this.buttonMap.DpadDown = OVRInput.RawButton.DpadDown;
			this.buttonMap.DpadLeft = OVRInput.RawButton.DpadLeft;
			this.buttonMap.DpadRight = OVRInput.RawButton.DpadRight;
			this.buttonMap.Up = OVRInput.RawButton.DpadUp;
			this.buttonMap.Down = OVRInput.RawButton.DpadDown;
			this.buttonMap.Left = OVRInput.RawButton.DpadLeft;
			this.buttonMap.Right = OVRInput.RawButton.DpadRight;
		}

		public override void ConfigureTouchMap()
		{
			this.touchMap.None = OVRInput.RawTouch.None;
			this.touchMap.One = OVRInput.RawTouch.None;
			this.touchMap.Two = OVRInput.RawTouch.None;
			this.touchMap.Three = OVRInput.RawTouch.None;
			this.touchMap.Four = OVRInput.RawTouch.None;
			this.touchMap.PrimaryIndexTrigger = OVRInput.RawTouch.None;
			this.touchMap.PrimaryThumbstick = OVRInput.RawTouch.None;
			this.touchMap.PrimaryThumbRest = OVRInput.RawTouch.None;
			this.touchMap.PrimaryTouchpad = OVRInput.RawTouch.None;
			this.touchMap.SecondaryIndexTrigger = OVRInput.RawTouch.None;
			this.touchMap.SecondaryThumbstick = OVRInput.RawTouch.None;
			this.touchMap.SecondaryThumbRest = OVRInput.RawTouch.None;
			this.touchMap.SecondaryTouchpad = OVRInput.RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			this.nearTouchMap.None = OVRInput.RawNearTouch.None;
			this.nearTouchMap.PrimaryIndexTrigger = OVRInput.RawNearTouch.None;
			this.nearTouchMap.PrimaryThumbButtons = OVRInput.RawNearTouch.None;
			this.nearTouchMap.SecondaryIndexTrigger = OVRInput.RawNearTouch.None;
			this.nearTouchMap.SecondaryThumbButtons = OVRInput.RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			this.axis1DMap.None = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryHandTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryHandTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerCurl = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerSlide = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryThumbRestForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryStylusForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerCurl = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerSlide = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryThumbRestForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryStylusForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerForce = OVRInput.RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			this.axis2DMap.None = OVRInput.RawAxis2D.None;
			this.axis2DMap.PrimaryThumbstick = OVRInput.RawAxis2D.None;
			this.axis2DMap.PrimaryTouchpad = OVRInput.RawAxis2D.None;
			this.axis2DMap.SecondaryThumbstick = OVRInput.RawAxis2D.None;
			this.axis2DMap.SecondaryTouchpad = OVRInput.RawAxis2D.None;
		}
	}

	public class OVRControllerGamepadPC : OVRInput.OVRControllerBase
	{
		public OVRControllerGamepadPC()
		{
			this.controllerType = OVRInput.Controller.Gamepad;
		}

		public override void ConfigureButtonMap()
		{
			this.buttonMap.None = OVRInput.RawButton.None;
			this.buttonMap.One = OVRInput.RawButton.A;
			this.buttonMap.Two = OVRInput.RawButton.B;
			this.buttonMap.Three = OVRInput.RawButton.X;
			this.buttonMap.Four = OVRInput.RawButton.Y;
			this.buttonMap.Start = OVRInput.RawButton.Start;
			this.buttonMap.Back = OVRInput.RawButton.Back;
			this.buttonMap.PrimaryShoulder = OVRInput.RawButton.LShoulder;
			this.buttonMap.PrimaryIndexTrigger = OVRInput.RawButton.LIndexTrigger;
			this.buttonMap.PrimaryHandTrigger = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstick = OVRInput.RawButton.LThumbstick;
			this.buttonMap.PrimaryThumbstickUp = OVRInput.RawButton.LThumbstickUp;
			this.buttonMap.PrimaryThumbstickDown = OVRInput.RawButton.LThumbstickDown;
			this.buttonMap.PrimaryThumbstickLeft = OVRInput.RawButton.LThumbstickLeft;
			this.buttonMap.PrimaryThumbstickRight = OVRInput.RawButton.LThumbstickRight;
			this.buttonMap.PrimaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.SecondaryShoulder = OVRInput.RawButton.RShoulder;
			this.buttonMap.SecondaryIndexTrigger = OVRInput.RawButton.RIndexTrigger;
			this.buttonMap.SecondaryHandTrigger = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstick = OVRInput.RawButton.RThumbstick;
			this.buttonMap.SecondaryThumbstickUp = OVRInput.RawButton.RThumbstickUp;
			this.buttonMap.SecondaryThumbstickDown = OVRInput.RawButton.RThumbstickDown;
			this.buttonMap.SecondaryThumbstickLeft = OVRInput.RawButton.RThumbstickLeft;
			this.buttonMap.SecondaryThumbstickRight = OVRInput.RawButton.RThumbstickRight;
			this.buttonMap.SecondaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.DpadUp = OVRInput.RawButton.DpadUp;
			this.buttonMap.DpadDown = OVRInput.RawButton.DpadDown;
			this.buttonMap.DpadLeft = OVRInput.RawButton.DpadLeft;
			this.buttonMap.DpadRight = OVRInput.RawButton.DpadRight;
			this.buttonMap.Up = OVRInput.RawButton.LThumbstickUp;
			this.buttonMap.Down = OVRInput.RawButton.LThumbstickDown;
			this.buttonMap.Left = OVRInput.RawButton.LThumbstickLeft;
			this.buttonMap.Right = OVRInput.RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			this.touchMap.None = OVRInput.RawTouch.None;
			this.touchMap.One = OVRInput.RawTouch.None;
			this.touchMap.Two = OVRInput.RawTouch.None;
			this.touchMap.Three = OVRInput.RawTouch.None;
			this.touchMap.Four = OVRInput.RawTouch.None;
			this.touchMap.PrimaryIndexTrigger = OVRInput.RawTouch.None;
			this.touchMap.PrimaryThumbstick = OVRInput.RawTouch.None;
			this.touchMap.PrimaryThumbRest = OVRInput.RawTouch.None;
			this.touchMap.PrimaryTouchpad = OVRInput.RawTouch.None;
			this.touchMap.SecondaryIndexTrigger = OVRInput.RawTouch.None;
			this.touchMap.SecondaryThumbstick = OVRInput.RawTouch.None;
			this.touchMap.SecondaryThumbRest = OVRInput.RawTouch.None;
			this.touchMap.SecondaryTouchpad = OVRInput.RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			this.nearTouchMap.None = OVRInput.RawNearTouch.None;
			this.nearTouchMap.PrimaryIndexTrigger = OVRInput.RawNearTouch.None;
			this.nearTouchMap.PrimaryThumbButtons = OVRInput.RawNearTouch.None;
			this.nearTouchMap.SecondaryIndexTrigger = OVRInput.RawNearTouch.None;
			this.nearTouchMap.SecondaryThumbButtons = OVRInput.RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			this.axis1DMap.None = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTrigger = OVRInput.RawAxis1D.LIndexTrigger;
			this.axis1DMap.PrimaryHandTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTrigger = OVRInput.RawAxis1D.RIndexTrigger;
			this.axis1DMap.SecondaryHandTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerCurl = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerSlide = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryThumbRestForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryStylusForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerCurl = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerSlide = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryThumbRestForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryStylusForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerForce = OVRInput.RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			this.axis2DMap.None = OVRInput.RawAxis2D.None;
			this.axis2DMap.PrimaryThumbstick = OVRInput.RawAxis2D.LThumbstick;
			this.axis2DMap.PrimaryTouchpad = OVRInput.RawAxis2D.None;
			this.axis2DMap.SecondaryThumbstick = OVRInput.RawAxis2D.RThumbstick;
			this.axis2DMap.SecondaryTouchpad = OVRInput.RawAxis2D.None;
		}
	}

	private class OVRControllerGamepadAndroid : OVRInput.OVRControllerBase
	{
		public OVRControllerGamepadAndroid()
		{
			this.controllerType = OVRInput.Controller.Gamepad;
		}

		public override void ConfigureButtonMap()
		{
			this.buttonMap.None = OVRInput.RawButton.None;
			this.buttonMap.One = OVRInput.RawButton.A;
			this.buttonMap.Two = OVRInput.RawButton.B;
			this.buttonMap.Three = OVRInput.RawButton.X;
			this.buttonMap.Four = OVRInput.RawButton.Y;
			this.buttonMap.Start = OVRInput.RawButton.Start;
			this.buttonMap.Back = OVRInput.RawButton.Back;
			this.buttonMap.PrimaryShoulder = OVRInput.RawButton.LShoulder;
			this.buttonMap.PrimaryIndexTrigger = OVRInput.RawButton.LIndexTrigger;
			this.buttonMap.PrimaryHandTrigger = OVRInput.RawButton.None;
			this.buttonMap.PrimaryThumbstick = OVRInput.RawButton.LThumbstick;
			this.buttonMap.PrimaryThumbstickUp = OVRInput.RawButton.LThumbstickUp;
			this.buttonMap.PrimaryThumbstickDown = OVRInput.RawButton.LThumbstickDown;
			this.buttonMap.PrimaryThumbstickLeft = OVRInput.RawButton.LThumbstickLeft;
			this.buttonMap.PrimaryThumbstickRight = OVRInput.RawButton.LThumbstickRight;
			this.buttonMap.PrimaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.SecondaryShoulder = OVRInput.RawButton.RShoulder;
			this.buttonMap.SecondaryIndexTrigger = OVRInput.RawButton.RIndexTrigger;
			this.buttonMap.SecondaryHandTrigger = OVRInput.RawButton.None;
			this.buttonMap.SecondaryThumbstick = OVRInput.RawButton.RThumbstick;
			this.buttonMap.SecondaryThumbstickUp = OVRInput.RawButton.RThumbstickUp;
			this.buttonMap.SecondaryThumbstickDown = OVRInput.RawButton.RThumbstickDown;
			this.buttonMap.SecondaryThumbstickLeft = OVRInput.RawButton.RThumbstickLeft;
			this.buttonMap.SecondaryThumbstickRight = OVRInput.RawButton.RThumbstickRight;
			this.buttonMap.SecondaryTouchpad = OVRInput.RawButton.None;
			this.buttonMap.DpadUp = OVRInput.RawButton.DpadUp;
			this.buttonMap.DpadDown = OVRInput.RawButton.DpadDown;
			this.buttonMap.DpadLeft = OVRInput.RawButton.DpadLeft;
			this.buttonMap.DpadRight = OVRInput.RawButton.DpadRight;
			this.buttonMap.Up = OVRInput.RawButton.LThumbstickUp;
			this.buttonMap.Down = OVRInput.RawButton.LThumbstickDown;
			this.buttonMap.Left = OVRInput.RawButton.LThumbstickLeft;
			this.buttonMap.Right = OVRInput.RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			this.touchMap.None = OVRInput.RawTouch.None;
			this.touchMap.One = OVRInput.RawTouch.None;
			this.touchMap.Two = OVRInput.RawTouch.None;
			this.touchMap.Three = OVRInput.RawTouch.None;
			this.touchMap.Four = OVRInput.RawTouch.None;
			this.touchMap.PrimaryIndexTrigger = OVRInput.RawTouch.None;
			this.touchMap.PrimaryThumbstick = OVRInput.RawTouch.None;
			this.touchMap.PrimaryThumbRest = OVRInput.RawTouch.None;
			this.touchMap.PrimaryTouchpad = OVRInput.RawTouch.None;
			this.touchMap.SecondaryIndexTrigger = OVRInput.RawTouch.None;
			this.touchMap.SecondaryThumbstick = OVRInput.RawTouch.None;
			this.touchMap.SecondaryThumbRest = OVRInput.RawTouch.None;
			this.touchMap.SecondaryTouchpad = OVRInput.RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			this.nearTouchMap.None = OVRInput.RawNearTouch.None;
			this.nearTouchMap.PrimaryIndexTrigger = OVRInput.RawNearTouch.None;
			this.nearTouchMap.PrimaryThumbButtons = OVRInput.RawNearTouch.None;
			this.nearTouchMap.SecondaryIndexTrigger = OVRInput.RawNearTouch.None;
			this.nearTouchMap.SecondaryThumbButtons = OVRInput.RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			this.axis1DMap.None = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTrigger = OVRInput.RawAxis1D.LIndexTrigger;
			this.axis1DMap.PrimaryHandTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTrigger = OVRInput.RawAxis1D.RIndexTrigger;
			this.axis1DMap.SecondaryHandTrigger = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerCurl = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerSlide = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryThumbRestForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryStylusForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerCurl = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerSlide = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryThumbRestForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryStylusForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.PrimaryIndexTriggerForce = OVRInput.RawAxis1D.None;
			this.axis1DMap.SecondaryIndexTriggerForce = OVRInput.RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			this.axis2DMap.None = OVRInput.RawAxis2D.None;
			this.axis2DMap.PrimaryThumbstick = OVRInput.RawAxis2D.LThumbstick;
			this.axis2DMap.PrimaryTouchpad = OVRInput.RawAxis2D.None;
			this.axis2DMap.SecondaryThumbstick = OVRInput.RawAxis2D.RThumbstick;
			this.axis2DMap.SecondaryTouchpad = OVRInput.RawAxis2D.None;
		}
	}
}
