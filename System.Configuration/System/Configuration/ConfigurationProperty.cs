using System;
using System.ComponentModel;
using Unity;

namespace System.Configuration
{
	/// <summary>Represents an attribute or a child of a configuration element. This class cannot be inherited.</summary>
	public sealed class ConfigurationProperty
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationProperty" /> class.</summary>
		/// <param name="name">The name of the configuration entity.</param>
		/// <param name="type">The type of the configuration entity.</param>
		public ConfigurationProperty(string name, Type type) : this(name, type, ConfigurationProperty.NoDefaultValue, TypeDescriptor.GetConverter(type), new DefaultValidator(), ConfigurationPropertyOptions.None, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationProperty" /> class.</summary>
		/// <param name="name">The name of the configuration entity.</param>
		/// <param name="type">The type of the configuration entity.</param>
		/// <param name="defaultValue">The default value of the configuration entity.</param>
		public ConfigurationProperty(string name, Type type, object defaultValue) : this(name, type, defaultValue, TypeDescriptor.GetConverter(type), new DefaultValidator(), ConfigurationPropertyOptions.None, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationProperty" /> class.</summary>
		/// <param name="name">The name of the configuration entity.</param>
		/// <param name="type">The type of the configuration entity.</param>
		/// <param name="defaultValue">The default value of the configuration entity.</param>
		/// <param name="options">One of the <see cref="T:System.Configuration.ConfigurationPropertyOptions" /> enumeration values.</param>
		public ConfigurationProperty(string name, Type type, object defaultValue, ConfigurationPropertyOptions options) : this(name, type, defaultValue, TypeDescriptor.GetConverter(type), new DefaultValidator(), options, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationProperty" /> class.</summary>
		/// <param name="name">The name of the configuration entity.</param>
		/// <param name="type">The type of the configuration entity.</param>
		/// <param name="defaultValue">The default value of the configuration entity.</param>
		/// <param name="typeConverter">The type of the converter to apply.</param>
		/// <param name="validator">The validator to use.</param>
		/// <param name="options">One of the <see cref="T:System.Configuration.ConfigurationPropertyOptions" /> enumeration values.</param>
		public ConfigurationProperty(string name, Type type, object defaultValue, TypeConverter typeConverter, ConfigurationValidatorBase validator, ConfigurationPropertyOptions options) : this(name, type, defaultValue, typeConverter, validator, options, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationProperty" /> class.</summary>
		/// <param name="name">The name of the configuration entity.</param>
		/// <param name="type">The type of the configuration entity.</param>
		/// <param name="defaultValue">The default value of the configuration entity.</param>
		/// <param name="typeConverter">The type of the converter to apply.</param>
		/// <param name="validator">The validator to use.</param>
		/// <param name="options">One of the <see cref="T:System.Configuration.ConfigurationPropertyOptions" /> enumeration values.</param>
		/// <param name="description">The description of the configuration entity.</param>
		public ConfigurationProperty(string name, Type type, object defaultValue, TypeConverter typeConverter, ConfigurationValidatorBase validator, ConfigurationPropertyOptions options, string description)
		{
			this.name = name;
			this.converter = ((typeConverter != null) ? typeConverter : TypeDescriptor.GetConverter(type));
			if (defaultValue != null)
			{
				if (defaultValue == ConfigurationProperty.NoDefaultValue)
				{
					TypeCode typeCode = Type.GetTypeCode(type);
					if (typeCode != TypeCode.Object)
					{
						if (typeCode != TypeCode.String)
						{
							defaultValue = Activator.CreateInstance(type);
						}
						else
						{
							defaultValue = string.Empty;
						}
					}
					else
					{
						defaultValue = null;
					}
				}
				else if (!type.IsAssignableFrom(defaultValue.GetType()))
				{
					if (!this.converter.CanConvertFrom(defaultValue.GetType()))
					{
						throw new ConfigurationErrorsException(string.Format("The default value for property '{0}' has a different type than the one of the property itself: expected {1} but was {2}", name, type, defaultValue.GetType()));
					}
					defaultValue = this.converter.ConvertFrom(defaultValue);
				}
			}
			this.default_value = defaultValue;
			this.flags = options;
			this.type = type;
			this.validation = ((validator != null) ? validator : new DefaultValidator());
			this.description = description;
		}

		/// <summary>Gets the <see cref="T:System.ComponentModel.TypeConverter" /> used to convert this <see cref="T:System.Configuration.ConfigurationProperty" /> into an XML representation for writing to the configuration file.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.TypeConverter" /> used to convert this <see cref="T:System.Configuration.ConfigurationProperty" /> into an XML representation for writing to the configuration file.</returns>
		/// <exception cref="T:System.Exception">This <see cref="T:System.Configuration.ConfigurationProperty" /> cannot be converted.</exception>
		public TypeConverter Converter
		{
			get
			{
				return this.converter;
			}
		}

		/// <summary>Gets the default value for this <see cref="T:System.Configuration.ConfigurationProperty" /> property.</summary>
		/// <returns>An <see cref="T:System.Object" /> that can be cast to the type specified by the <see cref="P:System.Configuration.ConfigurationProperty.Type" /> property.</returns>
		public object DefaultValue
		{
			get
			{
				return this.default_value;
			}
		}

		/// <summary>Gets a value indicating whether this <see cref="T:System.Configuration.ConfigurationProperty" /> is the key for the containing <see cref="T:System.Configuration.ConfigurationElement" /> object.</summary>
		/// <returns>
		///   <see langword="true" /> if this <see cref="T:System.Configuration.ConfigurationProperty" /> object is the key for the containing element; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool IsKey
		{
			get
			{
				return (this.flags & ConfigurationPropertyOptions.IsKey) > ConfigurationPropertyOptions.None;
			}
		}

		/// <summary>Gets a value indicating whether this <see cref="T:System.Configuration.ConfigurationProperty" /> is required.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.ConfigurationProperty" /> is required; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool IsRequired
		{
			get
			{
				return (this.flags & ConfigurationPropertyOptions.IsRequired) > ConfigurationPropertyOptions.None;
			}
		}

		/// <summary>Gets a value that indicates whether the property is the default collection of an element.</summary>
		/// <returns>
		///   <see langword="true" /> if the property is the default collection of an element; otherwise, <see langword="false" />.</returns>
		public bool IsDefaultCollection
		{
			get
			{
				return (this.flags & ConfigurationPropertyOptions.IsDefaultCollection) > ConfigurationPropertyOptions.None;
			}
		}

		/// <summary>Gets the name of this <see cref="T:System.Configuration.ConfigurationProperty" />.</summary>
		/// <returns>The name of the <see cref="T:System.Configuration.ConfigurationProperty" />.</returns>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>Gets the description associated with the <see cref="T:System.Configuration.ConfigurationProperty" />.</summary>
		/// <returns>A <see langword="string" /> value that describes the property.</returns>
		public string Description
		{
			get
			{
				return this.description;
			}
		}

		/// <summary>Gets the type of this <see cref="T:System.Configuration.ConfigurationProperty" /> object.</summary>
		/// <returns>A <see cref="T:System.Type" /> representing the type of this <see cref="T:System.Configuration.ConfigurationProperty" /> object.</returns>
		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		/// <summary>Gets the <see cref="T:System.Configuration.ConfigurationValidatorAttribute" />, which is used to validate this <see cref="T:System.Configuration.ConfigurationProperty" /> object.</summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationValidatorBase" /> validator, which is used to validate this <see cref="T:System.Configuration.ConfigurationProperty" />.</returns>
		public ConfigurationValidatorBase Validator
		{
			get
			{
				return this.validation;
			}
		}

		internal object ConvertFromString(string value)
		{
			if (this.converter != null)
			{
				return this.converter.ConvertFromInvariantString(value);
			}
			throw new NotImplementedException();
		}

		internal string ConvertToString(object value)
		{
			if (this.converter != null)
			{
				return this.converter.ConvertToInvariantString(value);
			}
			throw new NotImplementedException();
		}

		internal bool IsElement
		{
			get
			{
				return typeof(ConfigurationElement).IsAssignableFrom(this.type);
			}
		}

		internal ConfigurationCollectionAttribute CollectionAttribute
		{
			get
			{
				return this.collectionAttribute;
			}
			set
			{
				this.collectionAttribute = value;
			}
		}

		internal void Validate(object value)
		{
			if (this.validation != null)
			{
				this.validation.Validate(value);
			}
		}

		/// <summary>Indicates whether the assembly name for the configuration property requires transformation when it is serialized for an earlier version of the .NET Framework.</summary>
		/// <returns>
		///   <see langword="true" /> if the property requires assembly name transformation; otherwise, <see langword="false" />.</returns>
		public bool IsAssemblyStringTransformationRequired
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		/// <summary>Indicates whether the type name for the configuration property requires transformation when it is serialized for an earlier version of the .NET Framework.</summary>
		/// <returns>
		///   <see langword="true" /> if the property requires type-name transformation; otherwise, <see langword="false" />.</returns>
		public bool IsTypeStringTransformationRequired
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		/// <summary>Indicates whether the configuration property's parent configuration section is queried at serialization time to determine whether the configuration property should be serialized into XML.</summary>
		/// <returns>
		///   <see langword="true" /> if the parent configuration section should be queried; otherwise, <see langword="false" />.</returns>
		public bool IsVersionCheckRequired
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		internal static readonly object NoDefaultValue = new object();

		private string name;

		private Type type;

		private object default_value;

		private TypeConverter converter;

		private ConfigurationValidatorBase validation;

		private ConfigurationPropertyOptions flags;

		private string description;

		private ConfigurationCollectionAttribute collectionAttribute;
	}
}
