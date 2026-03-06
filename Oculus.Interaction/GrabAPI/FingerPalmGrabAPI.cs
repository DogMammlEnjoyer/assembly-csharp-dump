using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.GrabAPI
{
	public class FingerPalmGrabAPI : IFingerAPI
	{
		[DllImport("InteractionSdk")]
		private static extern int isdk_FingerPalmGrabAPI_Create();

		[DllImport("InteractionSdk")]
		private static extern FingerPalmGrabAPI.ReturnValue isdk_FingerPalmGrabAPI_UpdateHandData(int handle, [In] FingerPalmGrabAPI.HandData data);

		[DllImport("InteractionSdk")]
		private static extern FingerPalmGrabAPI.ReturnValue isdk_FingerPalmGrabAPI_GetFingerIsGrabbing(int handle, HandFinger finger, out bool grabbing);

		[DllImport("InteractionSdk")]
		private static extern FingerPalmGrabAPI.ReturnValue isdk_FingerPalmGrabAPI_GetFingerIsGrabbingChanged(int handle, HandFinger finger, bool targetGrabState, out bool changed);

		[DllImport("InteractionSdk")]
		private static extern FingerPalmGrabAPI.ReturnValue isdk_FingerPalmGrabAPI_GetFingerGrabScore(int handle, HandFinger finger, out float score);

		[DllImport("InteractionSdk")]
		private static extern FingerPalmGrabAPI.ReturnValue isdk_FingerPalmGrabAPI_GetCenterOffset(int handle, out Vector3 score);

		[DllImport("InteractionSdk")]
		private static extern FingerPalmGrabAPI.ReturnValue isdk_FingerPalmGrabAPI_GetConfigParamFloat(int handle, PalmGrabParamID paramID, out float outVal);

		[DllImport("InteractionSdk")]
		private static extern FingerPalmGrabAPI.ReturnValue isdk_FingerPalmGrabAPI_SetConfigParamFloat(int handle, PalmGrabParamID paramID, float inVal);

		[DllImport("InteractionSdk")]
		private static extern FingerPalmGrabAPI.ReturnValue isdk_FingerPalmGrabAPI_GetConfigParamVec3(int handle, PalmGrabParamID paramID, out Vector3 outVal);

		[DllImport("InteractionSdk")]
		private static extern FingerPalmGrabAPI.ReturnValue isdk_FingerPalmGrabAPI_SetConfigParamVec3(int handle, PalmGrabParamID paramID, Vector3 inVal);

		public FingerPalmGrabAPI()
		{
			this.handData_ = new FingerPalmGrabAPI.HandData();
		}

		private int GetHandle()
		{
			if (this.apiHandle_ == -1)
			{
				this.apiHandle_ = FingerPalmGrabAPI.isdk_FingerPalmGrabAPI_Create();
			}
			return this.apiHandle_;
		}

		public bool GetFingerIsGrabbing(HandFinger finger)
		{
			bool result;
			FingerPalmGrabAPI.isdk_FingerPalmGrabAPI_GetFingerIsGrabbing(this.GetHandle(), finger, out result);
			return result;
		}

		public bool GetFingerIsGrabbingChanged(HandFinger finger, bool targetGrabState)
		{
			bool result;
			FingerPalmGrabAPI.isdk_FingerPalmGrabAPI_GetFingerIsGrabbingChanged(this.GetHandle(), finger, targetGrabState, out result);
			return result;
		}

		public float GetFingerGrabScore(HandFinger finger)
		{
			float result;
			FingerPalmGrabAPI.isdk_FingerPalmGrabAPI_GetFingerGrabScore(this.GetHandle(), finger, out result);
			return result;
		}

		public void Update(IHand hand)
		{
			Pose root;
			if (!hand.GetRootPose(out root))
			{
				return;
			}
			ReadOnlyHandJointPoses joints;
			if (!hand.GetJointPosesFromWrist(out joints))
			{
				return;
			}
			this.handData_.SetData(joints, root, hand.Handedness);
			FingerPalmGrabAPI.isdk_FingerPalmGrabAPI_UpdateHandData(this.GetHandle(), this.handData_);
		}

		public Vector3 GetWristOffsetLocal()
		{
			Vector3 result;
			FingerPalmGrabAPI.isdk_FingerPalmGrabAPI_GetCenterOffset(this.GetHandle(), out result);
			return result;
		}

		public void SetConfigParamFloat(PalmGrabParamID paramId, float paramVal)
		{
			FingerPalmGrabAPI.isdk_FingerPalmGrabAPI_SetConfigParamFloat(this.GetHandle(), paramId, paramVal);
		}

		public float GetConfigParamFloat(PalmGrabParamID paramId)
		{
			float result;
			FingerPalmGrabAPI.isdk_FingerPalmGrabAPI_GetConfigParamFloat(this.GetHandle(), paramId, out result);
			return result;
		}

		public void SetConfigParamVec3(PalmGrabParamID paramId, Vector3 paramVal)
		{
			FingerPalmGrabAPI.isdk_FingerPalmGrabAPI_SetConfigParamVec3(this.GetHandle(), paramId, paramVal);
		}

		public Vector3 GetConfigParamVec3(PalmGrabParamID paramId)
		{
			Vector3 result;
			FingerPalmGrabAPI.isdk_FingerPalmGrabAPI_GetConfigParamVec3(this.GetHandle(), paramId, out result);
			return result;
		}

		private int apiHandle_ = -1;

		private FingerPalmGrabAPI.HandData handData_;

		[StructLayout(LayoutKind.Sequential)]
		public class HandData
		{
			public HandData()
			{
				this.jointValues = new float[168];
			}

			public void SetData(IReadOnlyList<Pose> joints, Pose root, Handedness handedness)
			{
				int num = 0;
				for (int i = 0; i < 24; i++)
				{
					Pose pose = joints[i];
					this.jointValues[num++] = pose.rotation.x;
					this.jointValues[num++] = pose.rotation.y;
					this.jointValues[num++] = pose.rotation.z;
					this.jointValues[num++] = pose.rotation.w;
					this.jointValues[num++] = pose.position.x;
					this.jointValues[num++] = pose.position.y;
					this.jointValues[num++] = pose.position.z;
				}
				this._rootRotX = root.rotation.x;
				this._rootRotY = root.rotation.y;
				this._rootRotZ = root.rotation.z;
				this._rootRotW = root.rotation.w;
				this._rootPosX = root.position.x;
				this._rootPosY = root.position.y;
				this._rootPosZ = root.position.z;
				this._handedness = (int)handedness;
			}

			private const int NumHandJoints = 24;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 168, ArraySubType = UnmanagedType.R4)]
			private float[] jointValues;

			private float _rootRotX;

			private float _rootRotY;

			private float _rootRotZ;

			private float _rootRotW;

			private float _rootPosX;

			private float _rootPosY;

			private float _rootPosZ;

			private int _handedness;
		}

		private enum ReturnValue
		{
			Success,
			Failure = -1
		}
	}
}
