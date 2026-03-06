using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	internal static class ScriptableSingletonCache<T> where T : ScriptableObject
	{
		public static T GetInstance(object user)
		{
			if (ScriptableSingletonCache<T>.s_Instance == null)
			{
				ScriptableSingletonCache<T>.s_Instance = ScriptableObject.CreateInstance<T>();
			}
			HashSet<object> hashSet;
			if (!ScriptableSingletonCache<T>.s_UsersPerInstance.TryGetValue(ScriptableSingletonCache<T>.s_Instance, out hashSet))
			{
				hashSet = new HashSet<object>();
				ScriptableSingletonCache<T>.s_UsersPerInstance.Add(ScriptableSingletonCache<T>.s_Instance, hashSet);
			}
			hashSet.Add(user);
			return ScriptableSingletonCache<T>.s_Instance;
		}

		public static void ReleaseInstance(object user)
		{
			if (ScriptableSingletonCache<T>.s_Instance == null)
			{
				return;
			}
			HashSet<object> hashSet;
			if (!ScriptableSingletonCache<T>.s_UsersPerInstance.TryGetValue(ScriptableSingletonCache<T>.s_Instance, out hashSet))
			{
				Object.Destroy(ScriptableSingletonCache<T>.s_Instance);
				return;
			}
			hashSet.Remove(user);
			if (hashSet.Count == 0)
			{
				ScriptableSingletonCache<T>.s_UsersPerInstance.Remove(ScriptableSingletonCache<T>.s_Instance);
				Object.Destroy(ScriptableSingletonCache<T>.s_Instance);
			}
		}

		private static T s_Instance;

		private static readonly Dictionary<ScriptableObject, HashSet<object>> s_UsersPerInstance = new Dictionary<ScriptableObject, HashSet<object>>();
	}
}
