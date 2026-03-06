using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.GrabAPI
{
	public class FingerPinchGrabAPI : IFingerAPI
	{
		[DllImport("InteractionSdk")]
		private static extern int isdk_FingerPinchGrabAPI_Create();

		[DllImport("InteractionSdk")]
		private static extern FingerPinchGrabAPI.ReturnValue isdk_FingerPinchGrabAPI_UpdateHandData(int handle, [In] HandPinchData data);

		[DllImport("InteractionSdk")]
		private static extern FingerPinchGrabAPI.ReturnValue isdk_FingerPinchGrabAPI_UpdateHandWristHMDData(int handle, [In] HandPinchData data, in Vector3 wristForward, in Vector3 hmdForward);

		[DllImport("InteractionSdk", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern bool isdk_FingerPinchGrabAPI_GetString(int handle, [MarshalAs(UnmanagedType.LPStr)] string name, out IntPtr val);

		[DllImport("InteractionSdk")]
		private static extern FingerPinchGrabAPI.ReturnValue isdk_FingerPinchGrabAPI_GetFingerIsGrabbing(int handle, int index, out bool grabbing);

		[DllImport("InteractionSdk")]
		private static extern FingerPinchGrabAPI.ReturnValue isdk_FingerPinchGrabAPI_GetFingerPinchPercent(int handle, int index, out float pinchPercent);

		[DllImport("InteractionSdk")]
		private static extern FingerPinchGrabAPI.ReturnValue isdk_FingerPinchGrabAPI_GetFingerPinchDistance(int handle, int index, out float pinchDistance);

		[DllImport("InteractionSdk")]
		private static extern FingerPinchGrabAPI.ReturnValue isdk_FingerPinchGrabAPI_GetFingerIsGrabbingChanged(int handle, int index, bool targetState, out bool grabbing);

		[DllImport("InteractionSdk")]
		private static extern FingerPinchGrabAPI.ReturnValue isdk_FingerPinchGrabAPI_GetFingerGrabScore(int handle, HandFinger finger, out float outScore);

		[DllImport("InteractionSdk")]
		private static extern FingerPinchGrabAPI.ReturnValue isdk_FingerPinchGrabAPI_GetCenterOffset(int handle, out Vector3 outCenter);

		[DllImport("InteractionSdk")]
		private static extern int isdk_Common_GetVersion(out IntPtr versionStringPtr);

		[DllImport("InteractionSdk")]
		private static extern FingerPinchGrabAPI.ReturnValue isdk_FingerPinchGrabAPI_GetPinchGrabParam(int handle, PinchGrabParam paramId, out float outParam);

		[DllImport("InteractionSdk")]
		private static extern FingerPinchGrabAPI.ReturnValue isdk_FingerPinchGrabAPI_SetPinchGrabParam(int handle, PinchGrabParam paramId, float param);

		[DllImport("InteractionSdk")]
		private static extern FingerPinchGrabAPI.ReturnValue isdk_FingerPinchGrabAPI_IsPinchVisibilityGood(int handle, out bool outVal);

		public FingerPinchGrabAPI(IHmd hmd = null)
		{
			this._hmd = hmd;
		}

		private int GetHandle()
		{
			if (this._fingerPinchGrabApiHandle == -1)
			{
				this._fingerPinchGrabApiHandle = FingerPinchGrabAPI.isdk_FingerPinchGrabAPI_Create();
			}
			return this._fingerPinchGrabApiHandle;
		}

		public void SetPinchGrabParam(PinchGrabParam paramId, float paramVal)
		{
			FingerPinchGrabAPI.isdk_FingerPinchGrabAPI_SetPinchGrabParam(this.GetHandle(), paramId, paramVal);
		}

		public float GetPinchGrabParam(PinchGrabParam paramId)
		{
			float result;
			FingerPinchGrabAPI.isdk_FingerPinchGrabAPI_GetPinchGrabParam(this.GetHandle(), paramId, out result);
			return result;
		}

		public bool GetIsPinchVisibilityGood()
		{
			bool result;
			FingerPinchGrabAPI.isdk_FingerPinchGrabAPI_IsPinchVisibilityGood(this.GetHandle(), out result);
			return result;
		}

		public bool GetFingerIsGrabbing(HandFinger finger)
		{
			bool result;
			FingerPinchGrabAPI.isdk_FingerPinchGrabAPI_GetFingerIsGrabbing(this.GetHandle(), (int)finger, out result);
			return result;
		}

		public float GetFingerPinchPercent(HandFinger finger)
		{
			float result;
			FingerPinchGrabAPI.isdk_FingerPinchGrabAPI_GetFingerPinchPercent(this.GetHandle(), (int)finger, out result);
			return result;
		}

		public float GetFingerPinchDistance(HandFinger finger)
		{
			float result;
			FingerPinchGrabAPI.isdk_FingerPinchGrabAPI_GetFingerPinchDistance(this.GetHandle(), (int)finger, out result);
			return result;
		}

		public Vector3 GetWristOffsetLocal()
		{
			Vector3 result;
			FingerPinchGrabAPI.isdk_FingerPinchGrabAPI_GetCenterOffset(this.GetHandle(), out result);
			return result;
		}

		public bool GetFingerIsGrabbingChanged(HandFinger finger, bool targetPinchState)
		{
			bool result;
			FingerPinchGrabAPI.isdk_FingerPinchGrabAPI_GetFingerIsGrabbingChanged(this.GetHandle(), (int)finger, targetPinchState, out result);
			return result;
		}

		public float GetFingerGrabScore(HandFinger finger)
		{
			float result;
			FingerPinchGrabAPI.isdk_FingerPinchGrabAPI_GetFingerGrabScore(this.GetHandle(), finger, out result);
			return result;
		}

		public void Update(IHand hand)
		{
			ReadOnlyHandJointPoses handPoses;
			hand.GetJointPosesFromWrist(out handPoses);
			Pose wristPose;
			hand.GetJointPose(HandJointId.HandWristRoot, out wristPose);
			this.Update(handPoses, hand.Handedness, wristPose);
		}

		internal void Update(IReadOnlyList<Pose> handPoses, Handedness handedness, Pose wristPose)
		{
			if (handPoses.Count <= 0)
			{
				return;
			}
			this._pinchData.SetJoints(handPoses);
			Vector3 a = Vector3.forward;
			Vector3 vector = Vector3.forward;
			Pose pose;
			if (this._hmd != null && this._hmd.TryGetRootPose(out pose))
			{
				a = -1f * wristPose.forward;
				vector = -1f * pose.forward;
				if (handedness == Handedness.Right)
				{
					a = -a;
				}
			}
			FingerPinchGrabAPI.isdk_FingerPinchGrabAPI_UpdateHandWristHMDData(this.GetHandle(), this._pinchData, a, vector);
		}

		private int _fingerPinchGrabApiHandle = -1;

		private HandPinchData _pinchData = new HandPinchData();

		private IHmd _hmd;

		private enum ReturnValue
		{
			Success,
			Failure = -1
		}
	}
}
