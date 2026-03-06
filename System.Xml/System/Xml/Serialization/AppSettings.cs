using System;
using System.Collections.Specialized;
using System.Configuration;

namespace System.Xml.Serialization
{
	internal static class AppSettings
	{
		internal static bool? UseLegacySerializerGeneration
		{
			get
			{
				AppSettings.EnsureSettingsLoaded();
				return AppSettings.useLegacySerializerGeneration;
			}
		}

		private static void EnsureSettingsLoaded()
		{
			if (!AppSettings.settingsInitalized)
			{
				object obj = AppSettings.appSettingsLock;
				lock (obj)
				{
					if (!AppSettings.settingsInitalized)
					{
						NameValueCollection nameValueCollection = null;
						try
						{
							nameValueCollection = ConfigurationManager.AppSettings;
						}
						catch (ConfigurationErrorsException)
						{
						}
						finally
						{
							bool value;
							if (nameValueCollection == null || !bool.TryParse(nameValueCollection["System:Xml:Serialization:UseLegacySerializerGeneration"], out value))
							{
								AppSettings.useLegacySerializerGeneration = null;
							}
							else
							{
								AppSettings.useLegacySerializerGeneration = new bool?(value);
							}
							AppSettings.settingsInitalized = true;
						}
					}
				}
			}
		}

		private const string UseLegacySerializerGenerationAppSettingsString = "System:Xml:Serialization:UseLegacySerializerGeneration";

		private static bool? useLegacySerializerGeneration;

		private static volatile bool settingsInitalized = false;

		private static object appSettingsLock = new object();
	}
}
