using System;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	internal struct GyroscopeState : IInputStateTypeInfo
	{
		public static FourCC kFormat
		{
			get
			{
				return new FourCC('G', 'Y', 'R', 'O');
			}
		}

		public FourCC format
		{
			get
			{
				return GyroscopeState.kFormat;
			}
		}

		[InputControl(displayName = "Angular Velocity", processors = "CompensateDirection", noisy = true)]
		public Vector3 angularVelocity;
	}
}
