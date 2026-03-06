using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public static class OVRNodeStateProperties
{
	public static bool IsHmdPresent()
	{
		if (OVRManager.OVRManagerinitialized && OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
		{
			return OVRPlugin.hmdPresent;
		}
		XRDisplaySubsystem currentDisplaySubsystem = OVRManager.GetCurrentDisplaySubsystem();
		return currentDisplaySubsystem != null && currentDisplaySubsystem.running;
	}

	public static bool GetNodeStatePropertyVector3(XRNode nodeType, NodeStatePropertyType propertyType, OVRPlugin.Node ovrpNodeType, OVRPlugin.Step stepType, out Vector3 retVec)
	{
		retVec = Vector3.zero;
		switch (propertyType)
		{
		case NodeStatePropertyType.Acceleration:
			if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
			{
				retVec = OVRPlugin.GetNodeAcceleration(ovrpNodeType, stepType).FromFlippedZVector3f();
				return true;
			}
			if (OVRNodeStateProperties.GetUnityXRNodeStateVector3(nodeType, NodeStatePropertyType.Acceleration, out retVec))
			{
				return true;
			}
			break;
		case NodeStatePropertyType.AngularAcceleration:
			if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
			{
				retVec = OVRPlugin.GetNodeAngularAcceleration(ovrpNodeType, stepType).FromFlippedZVector3f();
				return true;
			}
			if (OVRNodeStateProperties.GetUnityXRNodeStateVector3(nodeType, NodeStatePropertyType.AngularAcceleration, out retVec))
			{
				return true;
			}
			break;
		case NodeStatePropertyType.Velocity:
			if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
			{
				retVec = OVRPlugin.GetNodeVelocity(ovrpNodeType, stepType).FromFlippedZVector3f();
				return true;
			}
			if (OVRNodeStateProperties.GetUnityXRNodeStateVector3(nodeType, NodeStatePropertyType.Velocity, out retVec))
			{
				return true;
			}
			break;
		case NodeStatePropertyType.AngularVelocity:
			if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
			{
				retVec = OVRPlugin.GetNodeAngularVelocity(ovrpNodeType, stepType).FromFlippedZVector3f();
				return true;
			}
			if (OVRNodeStateProperties.GetUnityXRNodeStateVector3(nodeType, NodeStatePropertyType.AngularVelocity, out retVec))
			{
				return true;
			}
			break;
		case NodeStatePropertyType.Position:
			if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
			{
				retVec = OVRPlugin.GetNodePose(ovrpNodeType, stepType).ToOVRPose().position;
				return true;
			}
			if (OVRNodeStateProperties.GetUnityXRNodeStateVector3(nodeType, NodeStatePropertyType.Position, out retVec))
			{
				return true;
			}
			break;
		}
		return false;
	}

	public static bool GetNodeStatePropertyQuaternion(XRNode nodeType, NodeStatePropertyType propertyType, OVRPlugin.Node ovrpNodeType, OVRPlugin.Step stepType, out Quaternion retQuat)
	{
		retQuat = Quaternion.identity;
		if (propertyType == NodeStatePropertyType.Orientation)
		{
			if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
			{
				retQuat = OVRPlugin.GetNodePose(ovrpNodeType, stepType).ToOVRPose().orientation;
				return true;
			}
			if (OVRNodeStateProperties.GetUnityXRNodeStateQuaternion(nodeType, NodeStatePropertyType.Orientation, out retQuat))
			{
				return true;
			}
		}
		return false;
	}

	private static bool ValidateProperty(XRNode nodeType, ref XRNodeState requestedNodeState)
	{
		InputTracking.GetNodeStates(OVRNodeStateProperties.nodeStateList);
		if (OVRNodeStateProperties.nodeStateList.Count == 0)
		{
			return false;
		}
		bool result = false;
		requestedNodeState = OVRNodeStateProperties.nodeStateList[0];
		for (int i = 0; i < OVRNodeStateProperties.nodeStateList.Count; i++)
		{
			if (OVRNodeStateProperties.nodeStateList[i].nodeType == nodeType)
			{
				requestedNodeState = OVRNodeStateProperties.nodeStateList[i];
				result = true;
				break;
			}
		}
		return result;
	}

	private static bool GetUnityXRNodeStateVector3(XRNode nodeType, NodeStatePropertyType propertyType, out Vector3 retVec)
	{
		retVec = Vector3.zero;
		XRNodeState xrnodeState = default(XRNodeState);
		if (!OVRNodeStateProperties.ValidateProperty(nodeType, ref xrnodeState))
		{
			return false;
		}
		if (propertyType == NodeStatePropertyType.Acceleration)
		{
			if (xrnodeState.TryGetAcceleration(out retVec))
			{
				return true;
			}
		}
		else if (propertyType == NodeStatePropertyType.AngularAcceleration)
		{
			if (xrnodeState.TryGetAngularAcceleration(out retVec))
			{
				return true;
			}
		}
		else if (propertyType == NodeStatePropertyType.Velocity)
		{
			if (xrnodeState.TryGetVelocity(out retVec))
			{
				return true;
			}
		}
		else if (propertyType == NodeStatePropertyType.AngularVelocity)
		{
			if (xrnodeState.TryGetAngularVelocity(out retVec))
			{
				return true;
			}
		}
		else if (propertyType == NodeStatePropertyType.Position && xrnodeState.TryGetPosition(out retVec))
		{
			return true;
		}
		return false;
	}

	private static bool GetUnityXRNodeStateQuaternion(XRNode nodeType, NodeStatePropertyType propertyType, out Quaternion retQuat)
	{
		retQuat = Quaternion.identity;
		XRNodeState xrnodeState = default(XRNodeState);
		return OVRNodeStateProperties.ValidateProperty(nodeType, ref xrnodeState) && (propertyType == NodeStatePropertyType.Orientation && xrnodeState.TryGetRotation(out retQuat));
	}

	private static List<XRNodeState> nodeStateList = new List<XRNodeState>();
}
