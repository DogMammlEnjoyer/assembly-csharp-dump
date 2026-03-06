using System;

namespace System.Configuration
{
	/// <summary>Used internally as the class that represents metadata about an individual configuration property.</summary>
	public class SettingsProperty
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsProperty" /> class, based on the supplied parameter.</summary>
		/// <param name="propertyToCopy">Specifies a copy of an existing <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
		public SettingsProperty(SettingsProperty propertyToCopy) : this(propertyToCopy.Name, propertyToCopy.PropertyType, propertyToCopy.Provider, propertyToCopy.IsReadOnly, propertyToCopy.DefaultValue, propertyToCopy.SerializeAs, new SettingsAttributeDictionary(propertyToCopy.Attributes), propertyToCopy.ThrowOnErrorDeserializing, propertyToCopy.ThrowOnErrorSerializing)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsProperty" /> class. based on the supplied parameter.</summary>
		/// <param name="name">Specifies the name of an existing <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
		public SettingsProperty(string name) : this(name, null, null, false, null, SettingsSerializeAs.String, new SettingsAttributeDictionary(), false, false)
		{
		}

		/// <summary>Creates a new instance of the <see cref="T:System.Configuration.SettingsProperty" /> class based on the supplied parameters.</summary>
		/// <param name="name">The name of the <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
		/// <param name="propertyType">The type of <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
		/// <param name="provider">A <see cref="T:System.Configuration.SettingsProvider" /> object to use for persistence.</param>
		/// <param name="isReadOnly">A <see cref="T:System.Boolean" /> value specifying whether the <see cref="T:System.Configuration.SettingsProperty" /> object is read-only.</param>
		/// <param name="defaultValue">The default value of the <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
		/// <param name="serializeAs">A <see cref="T:System.Configuration.SettingsSerializeAs" /> object. This object is an enumeration used to set the serialization scheme for storing application settings.</param>
		/// <param name="attributes">A <see cref="T:System.Configuration.SettingsAttributeDictionary" /> object.</param>
		/// <param name="throwOnErrorDeserializing">A Boolean value specifying whether an error will be thrown when the property is unsuccessfully deserialized.</param>
		/// <param name="throwOnErrorSerializing">A Boolean value specifying whether an error will be thrown when the property is unsuccessfully serialized.</param>
		public SettingsProperty(string name, Type propertyType, SettingsProvider provider, bool isReadOnly, object defaultValue, SettingsSerializeAs serializeAs, SettingsAttributeDictionary attributes, bool throwOnErrorDeserializing, bool throwOnErrorSerializing)
		{
			this.name = name;
			this.propertyType = propertyType;
			this.provider = provider;
			this.isReadOnly = isReadOnly;
			this.defaultValue = defaultValue;
			this.serializeAs = serializeAs;
			this.attributes = attributes;
			this.throwOnErrorDeserializing = throwOnErrorDeserializing;
			this.throwOnErrorSerializing = throwOnErrorSerializing;
		}

		/// <summary>Gets a <see cref="T:System.Configuration.SettingsAttributeDictionary" /> object containing the attributes of the <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingsAttributeDictionary" /> object.</returns>
		public virtual SettingsAttributeDictionary Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		/// <summary>Gets or sets the default value of the <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
		/// <returns>An object containing the default value of the <see cref="T:System.Configuration.SettingsProperty" /> object.</returns>
		public virtual object DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
			set
			{
				this.defaultValue = value;
			}
		}

		/// <summary>Gets or sets a value specifying whether a <see cref="T:System.Configuration.SettingsProperty" /> object is read-only.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.SettingsProperty" /> is read-only; otherwise, <see langword="false" />.</returns>
		public virtual bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
			set
			{
				this.isReadOnly = value;
			}
		}

		/// <summary>Gets or sets the name of the <see cref="T:System.Configuration.SettingsProperty" />.</summary>
		/// <returns>The name of the <see cref="T:System.Configuration.SettingsProperty" />.</returns>
		public virtual string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		/// <summary>Gets or sets the type for the <see cref="T:System.Configuration.SettingsProperty" />.</summary>
		/// <returns>The type for the <see cref="T:System.Configuration.SettingsProperty" />.</returns>
		public virtual Type PropertyType
		{
			get
			{
				return this.propertyType;
			}
			set
			{
				this.propertyType = value;
			}
		}

		/// <summary>Gets or sets the provider for the <see cref="T:System.Configuration.SettingsProperty" />.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingsProvider" /> object.</returns>
		public virtual SettingsProvider Provider
		{
			get
			{
				return this.provider;
			}
			set
			{
				this.provider = value;
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Configuration.SettingsSerializeAs" /> object for the <see cref="T:System.Configuration.SettingsProperty" />.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingsSerializeAs" /> object.</returns>
		public virtual SettingsSerializeAs SerializeAs
		{
			get
			{
				return this.serializeAs;
			}
			set
			{
				this.serializeAs = value;
			}
		}

		/// <summary>Gets or sets a value specifying whether an error will be thrown when the property is unsuccessfully deserialized.</summary>
		/// <returns>
		///   <see langword="true" /> if the error will be thrown when the property is unsuccessfully deserialized; otherwise, <see langword="false" />.</returns>
		public bool ThrowOnErrorDeserializing
		{
			get
			{
				return this.throwOnErrorDeserializing;
			}
			set
			{
				this.throwOnErrorDeserializing = value;
			}
		}

		/// <summary>Gets or sets a value specifying whether an error will be thrown when the property is unsuccessfully serialized.</summary>
		/// <returns>
		///   <see langword="true" /> if the error will be thrown when the property is unsuccessfully serialized; otherwise, <see langword="false" />.</returns>
		public bool ThrowOnErrorSerializing
		{
			get
			{
				return this.throwOnErrorSerializing;
			}
			set
			{
				this.throwOnErrorSerializing = value;
			}
		}

		private string name;

		private Type propertyType;

		private SettingsProvider provider;

		private bool isReadOnly;

		private object defaultValue;

		private SettingsSerializeAs serializeAs;

		private SettingsAttributeDictionary attributes;

		private bool throwOnErrorDeserializing;

		private bool throwOnErrorSerializing;
	}
}
