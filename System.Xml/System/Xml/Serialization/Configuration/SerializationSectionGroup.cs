using System;
using System.Configuration;

namespace System.Xml.Serialization.Configuration
{
	/// <summary>Handles the XML elements used to configure XML serialization.</summary>
	public sealed class SerializationSectionGroup : ConfigurationSectionGroup
	{
		/// <summary>Gets the object that represents the section that contains configuration elements for the <see cref="T:System.Xml.Serialization.XmlSchemaImporter" />.</summary>
		/// <returns>The <see cref="T:System.Xml.Serialization.Configuration.SchemaImporterExtensionsSection" /> that represents the <see langword="schemaImporterExtenstion" /> element in the configuration file.</returns>
		[ConfigurationProperty("schemaImporterExtensions")]
		public SchemaImporterExtensionsSection SchemaImporterExtensions
		{
			get
			{
				return (SchemaImporterExtensionsSection)base.Sections["schemaImporterExtensions"];
			}
		}

		/// <summary>Gets the object that represents the <see cref="T:System.DateTime" /> serialization configuration element.</summary>
		/// <returns>The <see cref="T:System.Xml.Serialization.Configuration.DateTimeSerializationSection" /> object that represents the configuration element.</returns>
		[ConfigurationProperty("dateTimeSerialization")]
		public DateTimeSerializationSection DateTimeSerialization
		{
			get
			{
				return (DateTimeSerializationSection)base.Sections["dateTimeSerialization"];
			}
		}

		/// <summary>Gets the object that represents the configuration group for the <see cref="T:System.Xml.Serialization.XmlSerializer" />.</summary>
		/// <returns>The <see cref="T:System.Xml.Serialization.Configuration.XmlSerializerSection" /> that represents the <see cref="T:System.Xml.Serialization.XmlSerializer" />.</returns>
		public XmlSerializerSection XmlSerializer
		{
			get
			{
				return (XmlSerializerSection)base.Sections["xmlSerializer"];
			}
		}
	}
}
