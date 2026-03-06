using System;

namespace UnityEngine.UIElements
{
	public static class PointerType
	{
		internal static string GetPointerType(int pointerId)
		{
			bool flag = pointerId == PointerId.mousePointerId;
			string result;
			if (flag)
			{
				result = PointerType.mouse;
			}
			else
			{
				bool flag2 = pointerId >= PointerId.trackedPointerIdBase;
				if (flag2)
				{
					result = PointerType.tracked;
				}
				else
				{
					bool flag3 = pointerId >= PointerId.penPointerIdBase;
					if (flag3)
					{
						result = PointerType.pen;
					}
					else
					{
						bool flag4 = pointerId >= PointerId.touchPointerIdBase;
						if (flag4)
						{
							result = PointerType.touch;
						}
						else
						{
							result = PointerType.unknown;
						}
					}
				}
			}
			return result;
		}

		internal static bool IsDirectManipulationDevice(string pointerType)
		{
			return pointerType == PointerType.touch || pointerType == PointerType.pen;
		}

		public static readonly string mouse = "mouse";

		public static readonly string touch = "touch";

		public static readonly string pen = "pen";

		public static readonly string tracked = "tracked";

		public static readonly string unknown = "";
	}
}
