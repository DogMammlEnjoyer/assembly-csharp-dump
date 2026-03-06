using System;
using UnityEngine;

namespace Unity.XR.CoreUtils
{
	public abstract class ScriptableSettingsBase<T> : ScriptableSettingsBase where T : ScriptableObject
	{
		protected ScriptableSettingsBase()
		{
			if (ScriptableSettingsBase<T>.BaseInstance != null)
			{
				XRLoggingUtils.LogWarning(string.Format("ScriptableSingleton {0} already exists. This can happen if ", typeof(T)) + "there are two copies of the asset or if you query the singleton in a constructor.", ScriptableSettingsBase<T>.BaseInstance);
			}
		}

		protected static void Save(string savePathFormat)
		{
		}

		protected static string GetFilePath()
		{
			return typeof(T).Name;
		}

		protected static readonly bool HasCustomPath = typeof(T).IsDefined(typeof(ScriptableSettingsPathAttribute), true);

		protected static T BaseInstance;
	}
}
