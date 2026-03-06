using System;

namespace Modio
{
	[Serializable]
	public class ModioSettings
	{
		public T GetPlatformSettings<T>() where T : IModioServiceSettings
		{
			if (this.PlatformSettings == null)
			{
				return default(T);
			}
			foreach (IModioServiceSettings modioServiceSettings in this.PlatformSettings)
			{
				if (modioServiceSettings is T)
				{
					return (T)((object)modioServiceSettings);
				}
			}
			return default(T);
		}

		public bool TryGetPlatformSettings<T>(out T settings) where T : IModioServiceSettings
		{
			if (this.PlatformSettings == null)
			{
				settings = default(T);
				return false;
			}
			foreach (IModioServiceSettings modioServiceSettings in this.PlatformSettings)
			{
				if (modioServiceSettings is T)
				{
					T t = (T)((object)modioServiceSettings);
					settings = t;
					return true;
				}
			}
			settings = default(T);
			return false;
		}

		public ModioSettings ShallowClone()
		{
			return base.MemberwiseClone() as ModioSettings;
		}

		public long GameId;

		public string APIKey;

		public string ServerURL;

		public string DefaultLanguage = "en";

		public LogLevel LogLevel = LogLevel.Warning;

		public IModioServiceSettings[] PlatformSettings;
	}
}
