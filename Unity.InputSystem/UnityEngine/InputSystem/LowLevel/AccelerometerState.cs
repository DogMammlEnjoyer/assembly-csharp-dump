using System;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	internal struct AccelerometerState : IInputStateTypeInfo
	{
		public static FourCC kFormat
		{
			get
			{
				return new FourCC('A', 'C', 'C', 'L');
			}
		}

		public FourCC format
		{
			get
			{
				return AccelerometerState.kFormat;
			}
		}

		[InputControl(displayName = "Acceleration", processors = "CompensateDirection", noisy = true)]
		public Vector3 acceleration;
	}
}
