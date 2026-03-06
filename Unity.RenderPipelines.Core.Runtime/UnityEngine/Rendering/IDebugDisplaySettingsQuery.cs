using System;

namespace UnityEngine.Rendering
{
	public interface IDebugDisplaySettingsQuery
	{
		bool AreAnySettingsActive { get; }

		bool IsPostProcessingAllowed
		{
			get
			{
				return true;
			}
		}

		bool IsLightingActive
		{
			get
			{
				return true;
			}
		}

		bool TryGetScreenClearColor(ref Color color)
		{
			return false;
		}
	}
}
