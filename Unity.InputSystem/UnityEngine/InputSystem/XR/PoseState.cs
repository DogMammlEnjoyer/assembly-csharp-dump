using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.XR;

namespace UnityEngine.InputSystem.XR
{
	[StructLayout(LayoutKind.Explicit, Size = 60)]
	public struct PoseState : IInputStateTypeInfo
	{
		public FourCC format
		{
			get
			{
				return PoseState.s_Format;
			}
		}

		public PoseState(bool isTracked, InputTrackingState trackingState, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
		{
			this.isTracked = isTracked;
			this.trackingState = trackingState;
			this.position = position;
			this.rotation = rotation;
			this.velocity = velocity;
			this.angularVelocity = angularVelocity;
		}

		internal const int kSizeInBytes = 60;

		internal static readonly FourCC s_Format = new FourCC('P', 'o', 's', 'e');

		[InputControl(displayName = "Is Tracked", layout = "Button", sizeInBits = 8U)]
		[FieldOffset(0)]
		public bool isTracked;

		[InputControl(displayName = "Tracking State", layout = "Integer")]
		[FieldOffset(4)]
		public InputTrackingState trackingState;

		[InputControl(displayName = "Position", noisy = true)]
		[FieldOffset(8)]
		public Vector3 position;

		[InputControl(displayName = "Rotation", noisy = true)]
		[FieldOffset(20)]
		public Quaternion rotation;

		[InputControl(displayName = "Velocity", noisy = true)]
		[FieldOffset(36)]
		public Vector3 velocity;

		[InputControl(displayName = "Angular Velocity", noisy = true)]
		[FieldOffset(48)]
		public Vector3 angularVelocity;
	}
}
