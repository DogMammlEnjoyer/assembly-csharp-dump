using System;

namespace Oculus.Haptics
{
	internal static class Utils
	{
		internal static Ffi.Controller ControllerToFfiController(Controller controller)
		{
			Ffi.Controller result;
			switch (controller)
			{
			case Controller.Left:
				result = Ffi.Controller.Left;
				break;
			case Controller.Right:
				result = Ffi.Controller.Right;
				break;
			case Controller.Both:
				result = Ffi.Controller.Both;
				break;
			default:
				throw new ArgumentException(string.Format("Invalid controller selected: {0}.", controller));
			}
			return result;
		}

		internal static float Map(int input, int inMin, int inMax, int outMin, int outMax)
		{
			float num = (float)((input - inMin) * (outMax - outMin));
			float num2 = (float)(inMax - inMin);
			return num / num2 + (float)outMin;
		}
	}
}
