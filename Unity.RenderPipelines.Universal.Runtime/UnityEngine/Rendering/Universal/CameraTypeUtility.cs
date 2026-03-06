using System;
using System.Linq;

namespace UnityEngine.Rendering.Universal
{
	internal static class CameraTypeUtility
	{
		public static string GetName(this CameraRenderType type)
		{
			int num = (int)type;
			if (num < 0 || num >= CameraTypeUtility.s_CameraTypeNames.Length)
			{
				num = 0;
			}
			return CameraTypeUtility.s_CameraTypeNames[num];
		}

		private static string[] s_CameraTypeNames = Enum.GetNames(typeof(CameraRenderType)).ToArray<string>();
	}
}
