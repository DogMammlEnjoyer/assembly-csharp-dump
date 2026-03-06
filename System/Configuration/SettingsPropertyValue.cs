using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace System.Configuration
{
	/// <summary>Contains the value of a settings property that can be loaded and stored by an instance of <see cref="T:System.Configuration.SettingsBase" />.</summary>
	public class SettingsPropertyValue
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyValue" /> class, based on supplied parameters.</summary>
		/// <param name="property">Specifies a <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
		public SettingsPropertyValue(SettingsProperty property)
		{
			this.property = property;
			this.needPropertyValue = true;
			this.needSerializedValue = true;
		}

		/// <summary>Gets or sets whether the value of a <see cref="T:System.Configuration.SettingsProperty" /> object has been deserialized.</summary>
		/// <returns>
		///   <see langword="true" /> if the value of a <see cref="T:System.Configuration.SettingsProperty" /> object has been deserialized; otherwise, <see langword="false" />.</returns>
		public bool Deserialized
		{
			get
			{
				return this.deserialized;
			}
			set
			{
				this.deserialized = value;
			}
		}

		/// <summary>Gets or sets whether the value of a <see cref="T:System.Configuration.SettingsProperty" /> object has changed.</summary>
		/// <returns>
		///   <see langword="true" /> if the value of a <see cref="T:System.Configuration.SettingsProperty" /> object has changed; otherwise, <see langword="false" />.</returns>
		public bool IsDirty
		{
			get
			{
				return this.dirty;
			}
			set
			{
				this.dirty = value;
			}
		}

		/// <summary>Gets the name of the property from the associated <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
		/// <returns>The name of the <see cref="T:System.Configuration.SettingsProperty" /> object.</returns>
		public string Name
		{
			get
			{
				return this.property.Name;
			}
		}

		/// <summary>Gets the <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
		/// <returns>The <see cref="T:System.Configuration.SettingsProperty" /> object that describes the <see cref="T:System.Configuration.SettingsPropertyValue" /> object.</returns>
		public SettingsProperty Property
		{
			get
			{
				return this.property;
			}
		}

		/// <summary>Gets or sets the value of the <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
		/// <returns>The value of the <see cref="T:System.Configuration.SettingsProperty" /> object. When this value is set, the <see cref="P:System.Configuration.SettingsPropertyValue.IsDirty" /> property is set to <see langword="true" /> and <see cref="P:System.Configuration.SettingsPropertyValue.UsingDefaultValue" /> is set to <see langword="false" />.  
		///  When a value is first accessed from the <see cref="P:System.Configuration.SettingsPropertyValue.PropertyValue" /> property, and if the value was initially stored into the <see cref="T:System.Configuration.SettingsPropertyValue" /> object as a serialized representation using the <see cref="P:System.Configuration.SettingsPropertyValue.SerializedValue" /> property, the <see cref="P:System.Configuration.SettingsPropertyValue.PropertyValue" /> property will trigger deserialization of the underlying value.  As a side effect, the <see cref="P:System.Configuration.SettingsPropertyValue.Deserialized" /> property will be set to <see langword="true" />.  
		///  If this chain of events occurs in ASP.NET, and if an error occurs during the deserialization process, the error is logged using the health-monitoring feature of ASP.NET. By default, this means that deserialization errors will show up in the Application Event Log when running under ASP.NET. If this process occurs outside of ASP.NET, and if an error occurs during deserialization, the error is suppressed, and the remainder of the logic during deserialization occurs. If there is no serialized value to deserialize when the deserialization is attempted, then <see cref="T:System.Configuration.SettingsPropertyValue" /> object will instead attempt to return a default value if one was configured as defined on the associated <see cref="T:System.Configuration.SettingsProperty" /> instance. In this case, if the <see cref="P:System.Configuration.SettingsProperty.DefaultValue" /> property was set to either <see langword="null" />, or to the string "[null]", then the <see cref="T:System.Configuration.SettingsPropertyValue" /> object will initialize the <see cref="P:System.Configuration.SettingsPropertyValue.PropertyValue" /> property to either <see langword="null" /> for reference types, or to the default value for the associated value type.  On the other hand, if <see cref="P:System.Configuration.SettingsProperty.DefaultValue" /> property holds a valid object reference or string value (other than "[null]"), then the <see cref="P:System.Configuration.SettingsProperty.DefaultValue" /> property is returned instead.  
		///  If there is no serialized value to deserialize when the deserialization is attempted, and no default value was specified, then an empty string will be returned for string types. For all other types, a default instance will be returned by calling <see cref="M:System.Activator.CreateInstance(System.Type)" /> - for reference types this means an attempt will be made to create an object instance using the default constructor.  If this attempt fails, then <see langword="null" /> is returned.</returns>
		/// <exception cref="T:System.ArgumentException">While attempting to use the default value from the <see cref="P:System.Configuration.SettingsProperty.DefaultValue" /> property, an error occurred.  Either the attempt to convert <see cref="P:System.Configuration.SettingsProperty.DefaultValue" /> property to a valid type failed, or the resulting value was not compatible with the type defined by <see cref="P:System.Configuration.SettingsProperty.PropertyType" />.</exception>
		public object PropertyValue
		{
			get
			{
				if (this.needPropertyValue)
				{
					this.propertyValue = this.GetDeserializedValue(this.serializedValue);
					if (this.propertyValue == null)
					{
						this.propertyValue = this.GetDeserializedDefaultValue();
						this.serializedValue = null;
						this.needSerializedValue = true;
						this.defaulted = true;
					}
					this.needPropertyValue = false;
				}
				if (this.propertyValue != null && !(this.propertyValue is string) && !(this.propertyValue is DateTime) && !this.property.PropertyType.IsPrimitive)
				{
					this.dirty = true;
				}
				return this.propertyValue;
			}
			set
			{
				this.propertyValue = value;
				this.dirty = true;
				this.needPropertyValue = false;
				this.needSerializedValue = true;
				this.defaulted = false;
			}
		}

		/// <summary>Gets or sets the serialized value of the <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
		/// <returns>The serialized value of a <see cref="T:System.Configuration.SettingsProperty" /> object.</returns>
		/// <exception cref="T:System.ArgumentException">The serialization options for the property indicated the use of a string type converter, but a type converter was not available.</exception>
		public object SerializedValue
		{
			get
			{
				if ((this.needSerializedValue || this.IsDirty) && !this.UsingDefaultValue)
				{
					switch (this.property.SerializeAs)
					{
					case SettingsSerializeAs.String:
						this.serializedValue = TypeDescriptor.GetConverter(this.property.PropertyType).ConvertToInvariantString(this.propertyValue);
						break;
					case SettingsSerializeAs.Xml:
						if (this.propertyValue != null)
						{
							XmlSerializer xmlSerializer = new XmlSerializer(this.propertyValue.GetType());
							StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
							xmlSerializer.Serialize(stringWriter, this.propertyValue);
							this.serializedValue = stringWriter.ToString();
						}
						else
						{
							this.serializedValue = null;
						}
						break;
					case SettingsSerializeAs.Binary:
						if (this.propertyValue != null)
						{
							BinaryFormatter binaryFormatter = new BinaryFormatter();
							MemoryStream memoryStream = new MemoryStream();
							binaryFormatter.Serialize(memoryStream, this.propertyValue);
							this.serializedValue = memoryStream.ToArray();
						}
						else
						{
							this.serializedValue = null;
						}
						break;
					default:
						this.serializedValue = null;
						break;
					}
					this.needSerializedValue = false;
					this.dirty = false;
				}
				return this.serializedValue;
			}
			set
			{
				this.serializedValue = value;
				this.needPropertyValue = true;
				this.needSerializedValue = false;
			}
		}

		/// <summary>Gets a Boolean value specifying whether the value of the <see cref="T:System.Configuration.SettingsPropertyValue" /> object is the default value as defined by the <see cref="P:System.Configuration.SettingsProperty.DefaultValue" /> property value on the associated <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
		/// <returns>
		///   <see langword="true" /> if the value of the <see cref="T:System.Configuration.SettingsProperty" /> object is the default value; otherwise, <see langword="false" />.</returns>
		public bool UsingDefaultValue
		{
			get
			{
				return this.defaulted;
			}
		}

		internal object Reset()
		{
			this.propertyValue = this.GetDeserializedDefaultValue();
			this.dirty = true;
			this.defaulted = true;
			this.needPropertyValue = true;
			this.needSerializedValue = true;
			return this.propertyValue;
		}

		private object GetDeserializedDefaultValue()
		{
			if (this.property.DefaultValue == null)
			{
				if (this.property.PropertyType != null && this.property.PropertyType.IsValueType)
				{
					return Activator.CreateInstance(this.property.PropertyType);
				}
				return null;
			}
			else if (this.property.DefaultValue is string && ((string)this.property.DefaultValue).Length == 0)
			{
				if (this.property.PropertyType != typeof(string))
				{
					return Activator.CreateInstance(this.property.PropertyType);
				}
				return string.Empty;
			}
			else
			{
				if (this.property.DefaultValue is string && ((string)this.property.DefaultValue).Length > 0)
				{
					return this.GetDeserializedValue(this.property.DefaultValue);
				}
				if (!this.property.PropertyType.IsAssignableFrom(this.property.DefaultValue.GetType()))
				{
					return TypeDescriptor.GetConverter(this.property.PropertyType).ConvertFrom(null, CultureInfo.InvariantCulture, this.property.DefaultValue);
				}
				return this.property.DefaultValue;
			}
		}

		private object GetDeserializedValue(object serializedValue)
		{
			if (serializedValue == null)
			{
				return null;
			}
			object result = null;
			try
			{
				switch (this.property.SerializeAs)
				{
				case SettingsSerializeAs.String:
					if (serializedValue is string)
					{
						result = TypeDescriptor.GetConverter(this.property.PropertyType).ConvertFromInvariantString((string)serializedValue);
					}
					break;
				case SettingsSerializeAs.Xml:
				{
					XmlSerializer xmlSerializer = new XmlSerializer(this.property.PropertyType);
					StringReader input = new StringReader((string)serializedValue);
					result = xmlSerializer.Deserialize(XmlReader.Create(input));
					break;
				}
				case SettingsSerializeAs.Binary:
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					MemoryStream serializationStream;
					if (serializedValue is string)
					{
						serializationStream = new MemoryStream(Convert.FromBase64String((string)serializedValue));
					}
					else
					{
						serializationStream = new MemoryStream((byte[])serializedValue);
					}
					result = binaryFormatter.Deserialize(serializationStream);
					break;
				}
				}
			}
			catch (Exception ex)
			{
				if (this.property.ThrowOnErrorDeserializing)
				{
					throw ex;
				}
			}
			return result;
		}

		private readonly SettingsProperty property;

		private object propertyValue;

		private object serializedValue;

		private bool needSerializedValue;

		private bool needPropertyValue;

		private bool dirty;

		private bool defaulted;

		private bool deserialized;
	}
}
