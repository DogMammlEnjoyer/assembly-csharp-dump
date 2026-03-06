using System;

namespace UnityEngine.Localization.Settings
{
	public interface IInitialize
	{
		void PostInitialization(LocalizationSettings settings);
	}
}
