using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;

namespace System.Xml.Serialization.Configuration
{
	/// <summary>Handles the configuration for the <see cref="T:System.Xml.Serialization.XmlSchemaImporter" /> class. This class cannot be inherited.</summary>
	public sealed class SchemaImporterExtensionElement : ConfigurationElement
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Serialization.Configuration.SchemaImporterExtensionElement" /> class.</summary>
		public SchemaImporterExtensionElement()
		{
			this.properties.Add(this.name);
			this.properties.Add(this.type);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Serialization.Configuration.SchemaImporterExtensionElement" /> class and specifies the name and type of the extension.</summary>
		/// <param name="name">The name of the new extension. The name must be unique.</param>
		/// <param name="type">The type of the new extension, specified as a string.</param>
		public SchemaImporterExtensionElement(string name, string type) : this()
		{
			this.Name = name;
			base[this.type] = new SchemaImporterExtensionElement.TypeAndName(type);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Serialization.Configuration.SchemaImporterExtensionElement" /> class using the specified name and type.</summary>
		/// <param name="name">The name of the new extension. The name must be unique.</param>
		/// <param name="type">The <see cref="T:System.Type" /> of the new extension.</param>
		public SchemaImporterExtensionElement(string name, Type type) : this()
		{
			this.Name = name;
			this.Type = type;
		}

		/// <summary>Gets or sets the name of the extension.</summary>
		/// <returns>The name of the extension.</returns>
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get
			{
				return (string)base[this.name];
			}
			set
			{
				base[this.name] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return this.properties;
			}
		}

		/// <summary>Gets or sets the type of the extension.</summary>
		/// <returns>A type of the extension.</returns>
		[TypeConverter(typeof(SchemaImporterExtensionElement.TypeTypeConverter))]
		[ConfigurationProperty("type", IsRequired = true, IsKey = false)]
		public Type Type
		{
			get
			{
				return ((SchemaImporterExtensionElement.TypeAndName)base[this.type]).type;
			}
			set
			{
				base[this.type] = new SchemaImporterExtensionElement.TypeAndName(value);
			}
		}

		internal string Key
		{
			get
			{
				return this.Name;
			}
		}

		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private readonly ConfigurationProperty name = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsKey);

		private readonly ConfigurationProperty type = new ConfigurationProperty("type", typeof(Type), null, new SchemaImporterExtensionElement.TypeTypeConverter(), null, ConfigurationPropertyOptions.IsRequired);

		private class TypeAndName
		{
			public TypeAndName(string name)
			{
				this.type = Type.GetType(name, true, true);
				this.name = name;
			}

			public TypeAndName(Type type)
			{
				this.type = type;
			}

			public override int GetHashCode()
			{
				return this.type.GetHashCode();
			}

			public override bool Equals(object comparand)
			{
				return this.type.Equals(((SchemaImporterExtensionElement.TypeAndName)comparand).type);
			}

			public readonly Type type;

			public readonly string name;
		}

		private class TypeTypeConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				if (value is string)
				{
					return new SchemaImporterExtensionElement.TypeAndName((string)value);
				}
				return base.ConvertFrom(context, culture, value);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				if (!(destinationType == typeof(string)))
				{
					return base.ConvertTo(context, culture, value, destinationType);
				}
				SchemaImporterExtensionElement.TypeAndName typeAndName = (SchemaImporterExtensionElement.TypeAndName)value;
				if (typeAndName.name != null)
				{
					return typeAndName.name;
				}
				return typeAndName.type.AssemblyQualifiedName;
			}
		}
	}
}
