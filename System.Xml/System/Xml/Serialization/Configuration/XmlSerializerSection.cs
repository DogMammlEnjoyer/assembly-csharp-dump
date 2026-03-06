using System;
using System.Configuration;

namespace System.Xml.Serialization.Configuration
{
	/// <summary>Handles the XML elements used to configure XML serialization. </summary>
	public sealed class XmlSerializerSection : ConfigurationSection
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Serialization.Configuration.XmlSerializerSection" /> class. </summary>
		public XmlSerializerSection()
		{
			this.properties.Add(this.checkDeserializeAdvances);
			this.properties.Add(this.tempFilesLocation);
			this.properties.Add(this.useLegacySerializerGeneration);
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return this.properties;
			}
		}

		/// <summary>Gets or sets a value that determines whether an additional check of progress of the <see cref="T:System.Xml.Serialization.XmlSerializer" /> is done.</summary>
		/// <returns>
		///     <see langword="true" /> if the check is made; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		[ConfigurationProperty("checkDeserializeAdvances", DefaultValue = false)]
		public bool CheckDeserializeAdvances
		{
			get
			{
				return (bool)base[this.checkDeserializeAdvances];
			}
			set
			{
				base[this.checkDeserializeAdvances] = value;
			}
		}

		/// <summary>Returns the location that was specified for the creation of the temporary file.</summary>
		/// <returns>The location that was specified for the creation of the temporary file.</returns>
		[ConfigurationProperty("tempFilesLocation", DefaultValue = null)]
		public string TempFilesLocation
		{
			get
			{
				return (string)base[this.tempFilesLocation];
			}
			set
			{
				base[this.tempFilesLocation] = value;
			}
		}

		/// <summary>Gets or sets a value that indicates whether the specified object uses legacy serializer generation.</summary>
		/// <returns>
		///     <see langword="true" /> if the object uses legacy serializer generation; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("useLegacySerializerGeneration", DefaultValue = false)]
		public bool UseLegacySerializerGeneration
		{
			get
			{
				return (bool)base[this.useLegacySerializerGeneration];
			}
			set
			{
				base[this.useLegacySerializerGeneration] = value;
			}
		}

		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private readonly ConfigurationProperty checkDeserializeAdvances = new ConfigurationProperty("checkDeserializeAdvances", typeof(bool), false, ConfigurationPropertyOptions.None);

		private readonly ConfigurationProperty tempFilesLocation = new ConfigurationProperty("tempFilesLocation", typeof(string), null, null, new RootedPathValidator(), ConfigurationPropertyOptions.None);

		private readonly ConfigurationProperty useLegacySerializerGeneration = new ConfigurationProperty("useLegacySerializerGeneration", typeof(bool), false, ConfigurationPropertyOptions.None);
	}
}
