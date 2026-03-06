using System;

namespace UnityEngine.Localization.Settings
{
	[Serializable]
	public class CommandLineLocaleSelector : IStartupLocaleSelector
	{
		public string CommandLineArgument
		{
			get
			{
				return this.m_CommandLineArgument;
			}
			set
			{
				this.m_CommandLineArgument = value;
			}
		}

		public Locale GetStartupLocale(ILocalesProvider availableLocales)
		{
			if (string.IsNullOrEmpty(this.m_CommandLineArgument))
			{
				return null;
			}
			foreach (string text in Environment.GetCommandLineArgs())
			{
				if (text.StartsWith(this.m_CommandLineArgument, StringComparison.OrdinalIgnoreCase))
				{
					string text2 = text.Substring(this.m_CommandLineArgument.Length);
					Locale locale = availableLocales.GetLocale(text2);
					if (locale != null)
					{
						Debug.LogFormat("Found a matching locale({0}) for command line argument: `{1}`.", new object[]
						{
							text2,
							locale
						});
					}
					else
					{
						Debug.LogWarningFormat("Could not find a matching locale for command line argument: `{0}`", new object[]
						{
							text2
						});
					}
					return locale;
				}
			}
			return null;
		}

		[SerializeField]
		private string m_CommandLineArgument = "-language=";
	}
}
