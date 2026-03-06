using System;
using System.Collections;

namespace System.Configuration
{
	/// <summary>Provides contextual information that the provider can use when persisting settings.</summary>
	[Serializable]
	public class SettingsContext : Hashtable
	{
		internal ApplicationSettingsBase CurrentSettings
		{
			get
			{
				return this.current;
			}
			set
			{
				this.current = value;
			}
		}

		[NonSerialized]
		private ApplicationSettingsBase current;
	}
}
