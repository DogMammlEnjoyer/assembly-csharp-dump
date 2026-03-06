using System;
using System.Configuration;

namespace System.Net.Configuration
{
	/// <summary>Represents the performance counter element in the <see langword="System.Net" /> configuration file that determines whether networking performance counters are enabled. This class cannot be inherited.</summary>
	public sealed class PerformanceCountersElement : ConfigurationElement
	{
		static PerformanceCountersElement()
		{
			PerformanceCountersElement.properties.Add(PerformanceCountersElement.enabledProp);
		}

		/// <summary>Gets or sets whether performance counters are enabled.</summary>
		/// <returns>
		///   <see langword="true" /> if performance counters are enabled; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("enabled", DefaultValue = "False")]
		public bool Enabled
		{
			get
			{
				return (bool)base[PerformanceCountersElement.enabledProp];
			}
			set
			{
				base[PerformanceCountersElement.enabledProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return PerformanceCountersElement.properties;
			}
		}

		private static ConfigurationProperty enabledProp = new ConfigurationProperty("enabled", typeof(bool), false);

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
