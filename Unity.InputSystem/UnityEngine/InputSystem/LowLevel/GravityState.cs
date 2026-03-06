using System;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	internal struct GravityState : IInputStateTypeInfo
	{
		public static FourCC kFormat
		{
			get
			{
				return new FourCC('G', 'R', 'V', ' ');
			}
		}

		public FourCC format
		{
			get
			{
				return GravityState.kFormat;
			}
		}

		[InputControl(displayName = "Gravity", processors = "CompensateDirection", noisy = true)]
		public Vector3 gravity;
	}
}
