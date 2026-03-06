using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Cinemachine
{
	internal static class CinemachineDebug
	{
		public static StringBuilder SBFromPool()
		{
			if (CinemachineDebug.s_AvailableStringBuilders == null || CinemachineDebug.s_AvailableStringBuilders.Count == 0)
			{
				return new StringBuilder();
			}
			List<StringBuilder> list = CinemachineDebug.s_AvailableStringBuilders;
			StringBuilder stringBuilder = list[list.Count - 1];
			CinemachineDebug.s_AvailableStringBuilders.RemoveAt(CinemachineDebug.s_AvailableStringBuilders.Count - 1);
			stringBuilder.Length = 0;
			return stringBuilder;
		}

		public static void ReturnToPool(StringBuilder sb)
		{
			if (CinemachineDebug.s_AvailableStringBuilders == null)
			{
				CinemachineDebug.s_AvailableStringBuilders = new List<StringBuilder>();
			}
			CinemachineDebug.s_AvailableStringBuilders.Add(sb);
		}

		private static List<StringBuilder> s_AvailableStringBuilders;

		public static Action<CinemachineBrain> OnGUIHandlers;

		public static bool GameViewGuidesEnabled;
	}
}
