using System;
using System.Configuration;
using System.Xml;

namespace System.Runtime.Serialization.Configuration
{
	/// <summary>Handles the XML elements used to configure XML serialization by the <see cref="T:System.Runtime.Serialization.DataContractSerializer" />.</summary>
	public sealed class ParameterElement : ConfigurationElement
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.Configuration.ParameterElement" /> class.</summary>
		public ParameterElement()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.Configuration.ParameterElement" /> class with the specified type name.</summary>
		/// <param name="typeName">The name of the parameter's type.</param>
		public ParameterElement(string typeName) : this()
		{
			if (string.IsNullOrEmpty(typeName))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
			}
			this.Type = typeName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.Configuration.ParameterElement" /> class with the specified index.</summary>
		/// <param name="index">Specifies a position in the collection of parameters.</param>
		public ParameterElement(int index) : this()
		{
			this.Index = index;
		}

		/// <summary>Gets or sets the position of the generic known type.</summary>
		/// <returns>The position of the parameter in the containing generic declared type.</returns>
		[ConfigurationProperty("index", DefaultValue = 0)]
		[IntegerValidator(MinValue = 0)]
		public int Index
		{
			get
			{
				return (int)base["index"];
			}
			set
			{
				base["index"] = value;
			}
		}

		/// <summary>Gets the collection of parameters.</summary>
		/// <returns>A <see cref="T:System.Runtime.Serialization.Configuration.ParameterElementCollection" /> that contains all parameters.</returns>
		[ConfigurationProperty("", DefaultValue = null, Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public ParameterElementCollection Parameters
		{
			get
			{
				return (ParameterElementCollection)base[""];
			}
		}

		protected override void PostDeserialize()
		{
			this.Validate();
		}

		protected override void PreSerialize(XmlWriter writer)
		{
			this.Validate();
		}

		/// <summary>Gets or sets the type name of the parameter of the generic known type.</summary>
		/// <returns>The type name of the parameter.</returns>
		[ConfigurationProperty("type", DefaultValue = "")]
		[StringValidator(MinLength = 0)]
		public string Type
		{
			get
			{
				return (string)base["type"];
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					value = string.Empty;
				}
				base["type"] = value;
			}
		}

		private void Validate()
		{
			PropertyInformationCollection propertyInformationCollection = base.ElementInformation.Properties;
			if (propertyInformationCollection["index"].ValueOrigin == PropertyValueOrigin.Default && propertyInformationCollection["type"].ValueOrigin == PropertyValueOrigin.Default)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(SR.GetString("Configuration parameter element must set either type or index.")));
			}
			if (propertyInformationCollection["index"].ValueOrigin != PropertyValueOrigin.Default && propertyInformationCollection["type"].ValueOrigin != PropertyValueOrigin.Default)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(SR.GetString("Configuration parameter element can set only one of either type or index.")));
			}
			if (propertyInformationCollection["index"].ValueOrigin != PropertyValueOrigin.Default && this.Parameters.Count > 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(SR.GetString("Configuration parameter element must only add params with type.")));
			}
		}

		internal Type GetType(string rootType, Type[] typeArgs)
		{
			return TypeElement.GetType(rootType, typeArgs, this.Type, this.Index, this.Parameters);
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new ConfigurationPropertyCollection
					{
						new ConfigurationProperty("index", typeof(int), 0, null, new IntegerValidator(0, int.MaxValue, false), ConfigurationPropertyOptions.None),
						new ConfigurationProperty("", typeof(ParameterElementCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection),
						new ConfigurationProperty("type", typeof(string), string.Empty, null, new StringValidator(0, int.MaxValue, null), ConfigurationPropertyOptions.None)
					};
				}
				return this.properties;
			}
		}

		internal readonly Guid identity = Guid.NewGuid();

		private ConfigurationPropertyCollection properties;
	}
}
