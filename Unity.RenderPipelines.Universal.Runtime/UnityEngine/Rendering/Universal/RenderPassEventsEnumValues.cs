using System;

namespace UnityEngine.Rendering.Universal
{
	internal static class RenderPassEventsEnumValues
	{
		static RenderPassEventsEnumValues()
		{
			Array array = Enum.GetValues(typeof(RenderPassEvent));
			RenderPassEventsEnumValues.values = new int[array.Length];
			int num = 0;
			foreach (object obj in array)
			{
				int num2 = (int)obj;
				RenderPassEventsEnumValues.values[num] = num2;
				num++;
			}
		}

		public static int[] values;
	}
}
