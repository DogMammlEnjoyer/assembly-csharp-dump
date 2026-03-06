using System;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace UnityEngine.Rendering
{
	internal static class SceneExtensions
	{
		public static string GetGUID(this Scene scene)
		{
			return (string)SceneExtensions.s_SceneGUID.GetValue(scene);
		}

		private static PropertyInfo s_SceneGUID = typeof(Scene).GetProperty("guid", BindingFlags.Instance | BindingFlags.NonPublic);
	}
}
