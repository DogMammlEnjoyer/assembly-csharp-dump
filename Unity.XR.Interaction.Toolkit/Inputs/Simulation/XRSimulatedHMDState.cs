using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
	[StructLayout(LayoutKind.Explicit, Size = 117)]
	public struct XRSimulatedHMDState : IInputStateTypeInfo
	{
		public static FourCC formatId
		{
			get
			{
				return new FourCC('X', 'R', 'S', 'H');
			}
		}

		public FourCC format
		{
			get
			{
				return XRSimulatedHMDState.formatId;
			}
		}

		public void Reset()
		{
			this.leftEyePosition = default(Vector3);
			this.leftEyeRotation = Quaternion.identity;
			this.rightEyePosition = default(Vector3);
			this.rightEyeRotation = Quaternion.identity;
			this.centerEyePosition = default(Vector3);
			this.centerEyeRotation = Quaternion.identity;
			this.trackingState = 0;
			this.isTracked = false;
			this.devicePosition = default(Vector3);
			this.deviceRotation = Quaternion.identity;
		}

		[InputControl(usage = "LeftEyePosition", offset = 0U)]
		[FieldOffset(0)]
		public Vector3 leftEyePosition;

		[InputControl(usage = "LeftEyeRotation", offset = 12U)]
		[FieldOffset(12)]
		public Quaternion leftEyeRotation;

		[InputControl(usage = "RightEyePosition", offset = 28U)]
		[FieldOffset(28)]
		public Vector3 rightEyePosition;

		[InputControl(usage = "RightEyeRotation", offset = 40U)]
		[FieldOffset(40)]
		public Quaternion rightEyeRotation;

		[InputControl(usage = "CenterEyePosition", offset = 56U)]
		[FieldOffset(56)]
		public Vector3 centerEyePosition;

		[InputControl(usage = "CenterEyeRotation", offset = 68U)]
		[FieldOffset(68)]
		public Quaternion centerEyeRotation;

		[InputControl(usage = "TrackingState", layout = "Integer", offset = 84U)]
		[FieldOffset(84)]
		public int trackingState;

		[InputControl(usage = "IsTracked", layout = "Button", offset = 88U)]
		[FieldOffset(88)]
		public bool isTracked;

		[InputControl(usage = "DevicePosition", offset = 89U)]
		[FieldOffset(89)]
		public Vector3 devicePosition;

		[InputControl(usage = "DeviceRotation", offset = 101U)]
		[FieldOffset(101)]
		public Quaternion deviceRotation;
	}
}
