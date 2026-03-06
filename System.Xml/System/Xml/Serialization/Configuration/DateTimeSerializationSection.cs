using System;
using System.ComponentModel;
using System.Configuration;

namespace System.Xml.Serialization.Configuration
{
	/// <summary>Handles configuration settings for XML serialization of <see cref="T:System.DateTime" /> instances.</summary>
	public sealed class DateTimeSerializationSection : ConfigurationSection
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Serialization.Configuration.DateTimeSerializationSection" /> class.</summary>
		public DateTimeSerializationSection()
		{
			this.properties.Add(this.mode);
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return this.properties;
			}
		}

		/// <summary>Gets or sets a value that determines the serialization format.</summary>
		/// <returns>One of the <see cref="T:System.Xml.Serialization.Configuration.DateTimeSerializationSection.DateTimeSerializationMode" /> values.</returns>
		[ConfigurationProperty("mode", DefaultValue = DateTimeSerializationSection.DateTimeSerializationMode.Roundtrip)]
		public DateTimeSerializationSection.DateTimeSerializationMode Mode
		{
			get
			{
				return (DateTimeSerializationSection.DateTimeSerializationMode)base[this.mode];
			}
			set
			{
				base[this.mode] = value;
			}
		}

		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private readonly ConfigurationProperty mode = new ConfigurationProperty("mode", typeof(DateTimeSerializationSection.DateTimeSerializationMode), DateTimeSerializationSection.DateTimeSerializationMode.Roundtrip, new EnumConverter(typeof(DateTimeSerializationSection.DateTimeSerializationMode)), null, ConfigurationPropertyOptions.None);

		/// <summary>Determines XML serialization format of <see cref="T:System.DateTime" /> objects.</summary>
		public enum DateTimeSerializationMode
		{
			/// <summary>Same as <see langword="Roundtrip" />.</summary>
			Default,
			/// <summary>The serializer examines individual <see cref="T:System.DateTime" /> instances to determine the serialization format: UTC, local, or unspecified.</summary>
			Roundtrip,
			/// <summary>The serializer formats all <see cref="T:System.DateTime" /> objects as local time. This is for version 1.0 and 1.1 compatibility.</summary>
			Local
		}
	}
}
