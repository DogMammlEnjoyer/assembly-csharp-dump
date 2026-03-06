using System;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	internal struct AttitudeState : IInputStateTypeInfo
	{
		public static FourCC kFormat
		{
			get
			{
				return new FourCC('A', 'T', 'T', 'D');
			}
		}

		public FourCC format
		{
			get
			{
				return AttitudeState.kFormat;
			}
		}

		[InputControl(displayName = "Attitude", processors = "CompensateRotation", noisy = true)]
		public Quaternion attitude;
	}
}
