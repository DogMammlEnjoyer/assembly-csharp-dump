using System;

namespace UnityEngine.ProBuilder
{
	internal static class Clipping
	{
		private static Clipping.OutCode ComputeOutCode(Rect rect, float x, float y)
		{
			Clipping.OutCode outCode = Clipping.OutCode.Inside;
			if (x < rect.xMin)
			{
				outCode |= Clipping.OutCode.Left;
			}
			else if (x > rect.xMax)
			{
				outCode |= Clipping.OutCode.Right;
			}
			if (y < rect.yMin)
			{
				outCode |= Clipping.OutCode.Bottom;
			}
			else if (y > rect.yMax)
			{
				outCode |= Clipping.OutCode.Top;
			}
			return outCode;
		}

		internal static bool RectContainsLineSegment(Rect rect, float x0, float y0, float x1, float y1)
		{
			Clipping.OutCode outCode = Clipping.ComputeOutCode(rect, x0, y0);
			Clipping.OutCode outCode2 = Clipping.ComputeOutCode(rect, x1, y1);
			bool result = false;
			while ((outCode | outCode2) != Clipping.OutCode.Inside)
			{
				if ((outCode & outCode2) != Clipping.OutCode.Inside)
				{
					return result;
				}
				float num = 0f;
				float num2 = 0f;
				Clipping.OutCode outCode3 = (outCode != Clipping.OutCode.Inside) ? outCode : outCode2;
				if ((outCode3 & Clipping.OutCode.Top) == Clipping.OutCode.Top)
				{
					num = x0 + (x1 - x0) * (rect.yMax - y0) / (y1 - y0);
					num2 = rect.yMax;
				}
				else if ((outCode3 & Clipping.OutCode.Bottom) == Clipping.OutCode.Bottom)
				{
					num = x0 + (x1 - x0) * (rect.yMin - y0) / (y1 - y0);
					num2 = rect.yMin;
				}
				else if ((outCode3 & Clipping.OutCode.Right) == Clipping.OutCode.Right)
				{
					num2 = y0 + (y1 - y0) * (rect.xMax - x0) / (x1 - x0);
					num = rect.xMax;
				}
				else if ((outCode3 & Clipping.OutCode.Left) == Clipping.OutCode.Left)
				{
					num2 = y0 + (y1 - y0) * (rect.xMin - x0) / (x1 - x0);
					num = rect.xMin;
				}
				if (outCode3 == outCode)
				{
					x0 = num;
					y0 = num2;
					outCode = Clipping.ComputeOutCode(rect, x0, y0);
				}
				else
				{
					x1 = num;
					y1 = num2;
					outCode2 = Clipping.ComputeOutCode(rect, x1, y1);
				}
			}
			return true;
		}

		[Flags]
		private enum OutCode
		{
			Inside = 0,
			Left = 1,
			Right = 2,
			Bottom = 4,
			Top = 8
		}
	}
}
