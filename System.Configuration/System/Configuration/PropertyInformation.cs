using System;
using System.ComponentModel;
using Unity;

namespace System.Configuration
{
	/// <summary>Contains meta-information on an individual property within the configuration. This type cannot be inherited.</summary>
	public sealed class PropertyInformation
	{
		internal PropertyInformation(ConfigurationElement owner, ConfigurationProperty property)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			this.owner = owner;
			this.property = property;
		}

		/// <summary>Gets the <see cref="T:System.ComponentModel.TypeConverter" /> object related to the configuration attribute.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.TypeConverter" /> object.</returns>
		public TypeConverter Converter
		{
			get
			{
				return this.property.Converter;
			}
		}

		/// <summary>Gets an object containing the default value related to a configuration attribute.</summary>
		/// <returns>An object containing the default value of the configuration attribute.</returns>
		public object DefaultValue
		{
			get
			{
				return this.property.DefaultValue;
			}
		}

		/// <summary>Gets the description of the object that corresponds to a configuration attribute.</summary>
		/// <returns>The description of the configuration attribute.</returns>
		public string Description
		{
			get
			{
				return this.property.Description;
			}
		}

		/// <summary>Gets a value specifying whether the configuration attribute is a key.</summary>
		/// <returns>
		///   <see langword="true" /> if the configuration attribute is a key; otherwise, <see langword="false" />.</returns>
		public bool IsKey
		{
			get
			{
				return this.property.IsKey;
			}
		}

		/// <summary>Gets a value specifying whether the configuration attribute is locked.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.PropertyInformation" /> object is locked; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public bool IsLocked
		{
			get
			{
				return this.isLocked;
			}
			internal set
			{
				this.isLocked = value;
			}
		}

		/// <summary>Gets a value specifying whether the configuration attribute has been modified.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.PropertyInformation" /> object has been modified; otherwise, <see langword="false" />.</returns>
		public bool IsModified
		{
			get
			{
				return this.isModified;
			}
			internal set
			{
				this.isModified = value;
			}
		}

		/// <summary>Gets a value specifying whether the configuration attribute is required.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.PropertyInformation" /> object is required; otherwise, <see langword="false" />.</returns>
		public bool IsRequired
		{
			get
			{
				return this.property.IsRequired;
			}
		}

		/// <summary>Gets the line number in the configuration file related to the configuration attribute.</summary>
		/// <returns>A line number of the configuration file.</returns>
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
			internal set
			{
				this.lineNumber = value;
			}
		}

		/// <summary>Gets the name of the object that corresponds to a configuration attribute.</summary>
		/// <returns>The name of the <see cref="T:System.Configuration.PropertyInformation" /> object.</returns>
		public string Name
		{
			get
			{
				return this.property.Name;
			}
		}

		/// <summary>Gets the source file that corresponds to a configuration attribute.</summary>
		/// <returns>The source file of the <see cref="T:System.Configuration.PropertyInformation" /> object.</returns>
		public string Source
		{
			get
			{
				return this.source;
			}
			internal set
			{
				this.source = value;
			}
		}

		/// <summary>Gets the <see cref="T:System.Type" /> of the object that corresponds to a configuration attribute.</summary>
		/// <returns>The <see cref="T:System.Type" /> of the <see cref="T:System.Configuration.PropertyInformation" /> object.</returns>
		public Type Type
		{
			get
			{
				return this.property.Type;
			}
		}

		/// <summary>Gets a <see cref="T:System.Configuration.ConfigurationValidatorBase" /> object related to the configuration attribute.</summary>
		/// <returns>A <see cref="T:System.Configuration.ConfigurationValidatorBase" /> object.</returns>
		public ConfigurationValidatorBase Validator
		{
			get
			{
				return this.property.Validator;
			}
		}

		/// <summary>Gets or sets an object containing the value related to a configuration attribute.</summary>
		/// <returns>An object containing the value for the <see cref="T:System.Configuration.PropertyInformation" /> object.</returns>
		public object Value
		{
			get
			{
				if (this.origin == PropertyValueOrigin.Default)
				{
					if (!this.property.IsElement)
					{
						return this.DefaultValue;
					}
					ConfigurationElement configurationElement = (ConfigurationElement)Activator.CreateInstance(this.Type, true);
					configurationElement.InitFromProperty(this);
					if (this.owner != null && this.owner.IsReadOnly())
					{
						configurationElement.SetReadOnly();
					}
					this.val = configurationElement;
					this.origin = PropertyValueOrigin.Inherited;
				}
				return this.val;
			}
			set
			{
				this.val = value;
				this.isModified = true;
				this.origin = PropertyValueOrigin.SetHere;
			}
		}

		internal void Reset(PropertyInformation parentProperty)
		{
			if (parentProperty == null)
			{
				this.origin = PropertyValueOrigin.Default;
				return;
			}
			if (this.property.IsElement)
			{
				((ConfigurationElement)this.Value).Reset((ConfigurationElement)parentProperty.Value);
				return;
			}
			this.val = parentProperty.Value;
			this.origin = PropertyValueOrigin.Inherited;
		}

		internal bool IsElement
		{
			get
			{
				return this.property.IsElement;
			}
		}

		/// <summary>Gets a <see cref="T:System.Configuration.PropertyValueOrigin" /> object related to the configuration attribute.</summary>
		/// <returns>A <see cref="T:System.Configuration.PropertyValueOrigin" /> object.</returns>
		public PropertyValueOrigin ValueOrigin
		{
			get
			{
				return this.origin;
			}
		}

		internal string GetStringValue()
		{
			return this.property.ConvertToString(this.Value);
		}

		internal void SetStringValue(string value)
		{
			this.val = this.property.ConvertFromString(value);
			if (!object.Equals(this.val, this.DefaultValue))
			{
				this.origin = PropertyValueOrigin.SetHere;
			}
		}

		internal ConfigurationProperty Property
		{
			get
			{
				return this.property;
			}
		}

		internal PropertyInformation()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private bool isLocked;

		private bool isModified;

		private int lineNumber;

		private string source;

		private object val;

		private PropertyValueOrigin origin;

		private readonly ConfigurationElement owner;

		private readonly ConfigurationProperty property;
	}
}
