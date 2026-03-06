using System;
using System.Collections;
using System.Xml;
using Unity;

namespace System.Configuration
{
	/// <summary>Represents a configuration element within a configuration file.</summary>
	public abstract class ConfigurationElement
	{
		internal Configuration Configuration
		{
			get
			{
				return this._configuration;
			}
			set
			{
				this._configuration = value;
			}
		}

		internal virtual void InitFromProperty(PropertyInformation propertyInfo)
		{
			this.elementInfo = new ElementInformation(this, propertyInfo);
			this.Init();
		}

		/// <summary>Gets an <see cref="T:System.Configuration.ElementInformation" /> object that contains the non-customizable information and functionality of the <see cref="T:System.Configuration.ConfigurationElement" /> object.</summary>
		/// <returns>An <see cref="T:System.Configuration.ElementInformation" /> that contains the non-customizable information and functionality of the <see cref="T:System.Configuration.ConfigurationElement" />.</returns>
		public ElementInformation ElementInformation
		{
			get
			{
				if (this.elementInfo == null)
				{
					this.elementInfo = new ElementInformation(this, null);
				}
				return this.elementInfo;
			}
		}

		internal string RawXml
		{
			get
			{
				return this.rawXml;
			}
			set
			{
				if (this.rawXml == null || value != null)
				{
					this.rawXml = value;
				}
			}
		}

		/// <summary>Sets the <see cref="T:System.Configuration.ConfigurationElement" /> object to its initial state.</summary>
		protected internal virtual void Init()
		{
		}

		/// <summary>Gets the <see cref="T:System.Configuration.ConfigurationElementProperty" /> object that represents the <see cref="T:System.Configuration.ConfigurationElement" /> object itself.</summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationElementProperty" /> that represents the <see cref="T:System.Configuration.ConfigurationElement" /> itself.</returns>
		protected internal virtual ConfigurationElementProperty ElementProperty
		{
			get
			{
				if (this.elementProperty == null)
				{
					this.elementProperty = new ConfigurationElementProperty(this.ElementInformation.Validator);
				}
				return this.elementProperty;
			}
		}

		/// <summary>Gets the <see cref="T:System.Configuration.ContextInformation" /> object for the <see cref="T:System.Configuration.ConfigurationElement" /> object.</summary>
		/// <returns>The <see cref="T:System.Configuration.ContextInformation" /> for the <see cref="T:System.Configuration.ConfigurationElement" />.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The current element is not associated with a context.</exception>
		protected ContextInformation EvaluationContext
		{
			get
			{
				if (this.Configuration != null)
				{
					return this.Configuration.EvaluationContext;
				}
				throw new ConfigurationErrorsException("This element is not currently associated with any context.");
			}
		}

		/// <summary>Gets the collection of locked attributes.</summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationLockCollection" /> of locked attributes (properties) for the element.</returns>
		public ConfigurationLockCollection LockAllAttributesExcept
		{
			get
			{
				if (this.lockAllAttributesExcept == null)
				{
					this.lockAllAttributesExcept = new ConfigurationLockCollection(this, ConfigurationLockType.Attribute | ConfigurationLockType.Exclude);
				}
				return this.lockAllAttributesExcept;
			}
		}

		/// <summary>Gets the collection of locked elements.</summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationLockCollection" /> of locked elements.</returns>
		public ConfigurationLockCollection LockAllElementsExcept
		{
			get
			{
				if (this.lockAllElementsExcept == null)
				{
					this.lockAllElementsExcept = new ConfigurationLockCollection(this, ConfigurationLockType.Element | ConfigurationLockType.Exclude);
				}
				return this.lockAllElementsExcept;
			}
		}

		/// <summary>Gets the collection of locked attributes</summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationLockCollection" /> of locked attributes (properties) for the element.</returns>
		public ConfigurationLockCollection LockAttributes
		{
			get
			{
				if (this.lockAttributes == null)
				{
					this.lockAttributes = new ConfigurationLockCollection(this, ConfigurationLockType.Attribute);
				}
				return this.lockAttributes;
			}
		}

		/// <summary>Gets the collection of locked elements.</summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationLockCollection" /> of locked elements.</returns>
		public ConfigurationLockCollection LockElements
		{
			get
			{
				if (this.lockElements == null)
				{
					this.lockElements = new ConfigurationLockCollection(this, ConfigurationLockType.Element);
				}
				return this.lockElements;
			}
		}

		/// <summary>Gets or sets a value indicating whether the element is locked.</summary>
		/// <returns>
		///   <see langword="true" /> if the element is locked; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The element has already been locked at a higher configuration level.</exception>
		public bool LockItem
		{
			get
			{
				return this.lockItem;
			}
			set
			{
				this.lockItem = value;
			}
		}

		/// <summary>Adds the invalid-property errors in this <see cref="T:System.Configuration.ConfigurationElement" /> object, and in all subelements, to the passed list.</summary>
		/// <param name="errorList">An object that implements the <see cref="T:System.Collections.IList" /> interface.</param>
		[MonoTODO]
		protected virtual void ListErrors(IList errorList)
		{
			throw new NotImplementedException();
		}

		/// <summary>Sets a property to the specified value.</summary>
		/// <param name="prop">The element property to set.</param>
		/// <param name="value">The value to assign to the property.</param>
		/// <param name="ignoreLocks">
		///   <see langword="true" /> if the locks on the property should be ignored; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">Occurs if the element is read-only or <paramref name="ignoreLocks" /> is <see langword="true" /> but the locks cannot be ignored.</exception>
		[MonoTODO]
		protected void SetPropertyValue(ConfigurationProperty prop, object value, bool ignoreLocks)
		{
			try
			{
				if (value != null)
				{
					prop.Validate(value);
				}
			}
			catch (Exception inner)
			{
				throw new ConfigurationErrorsException(string.Format("The value for the property '{0}' on type {1} is not valid.", prop.Name, this.ElementInformation.Type), inner);
			}
		}

		internal ConfigurationPropertyCollection GetKeyProperties()
		{
			if (this.keyProps != null)
			{
				return this.keyProps;
			}
			ConfigurationPropertyCollection configurationPropertyCollection = new ConfigurationPropertyCollection();
			foreach (object obj in this.Properties)
			{
				ConfigurationProperty configurationProperty = (ConfigurationProperty)obj;
				if (configurationProperty.IsKey)
				{
					configurationPropertyCollection.Add(configurationProperty);
				}
			}
			return this.keyProps = configurationPropertyCollection;
		}

		internal ConfigurationElementCollection GetDefaultCollection()
		{
			if (this.defaultCollection != null)
			{
				return this.defaultCollection;
			}
			ConfigurationProperty configurationProperty = null;
			foreach (object obj in this.Properties)
			{
				ConfigurationProperty configurationProperty2 = (ConfigurationProperty)obj;
				if (configurationProperty2.IsDefaultCollection)
				{
					configurationProperty = configurationProperty2;
					break;
				}
			}
			if (configurationProperty != null)
			{
				this.defaultCollection = (this[configurationProperty] as ConfigurationElementCollection);
			}
			return this.defaultCollection;
		}

		/// <summary>Gets or sets a property or attribute of this configuration element.</summary>
		/// <param name="prop">The property to access.</param>
		/// <returns>The specified property, attribute, or child element.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationException">
		///   <paramref name="prop" /> is <see langword="null" /> or does not exist within the element.</exception>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">
		///   <paramref name="prop" /> is read only or locked.</exception>
		protected internal object this[ConfigurationProperty prop]
		{
			get
			{
				return this[prop.Name];
			}
			set
			{
				this[prop.Name] = value;
			}
		}

		/// <summary>Gets or sets a property, attribute, or child element of this configuration element.</summary>
		/// <param name="propertyName">The name of the <see cref="T:System.Configuration.ConfigurationProperty" /> to access.</param>
		/// <returns>The specified property, attribute, or child element</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">
		///   <paramref name="prop" /> is read-only or locked.</exception>
		protected internal object this[string propertyName]
		{
			get
			{
				PropertyInformation propertyInformation = this.ElementInformation.Properties[propertyName];
				if (propertyInformation == null)
				{
					throw new InvalidOperationException("Property '" + propertyName + "' not found in configuration element");
				}
				return propertyInformation.Value;
			}
			set
			{
				PropertyInformation propertyInformation = this.ElementInformation.Properties[propertyName];
				if (propertyInformation == null)
				{
					throw new InvalidOperationException("Property '" + propertyName + "' not found in configuration element");
				}
				this.SetPropertyValue(propertyInformation.Property, value, false);
				propertyInformation.Value = value;
				this.modified = true;
			}
		}

		/// <summary>Gets the collection of properties.</summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationPropertyCollection" /> of properties for the element.</returns>
		protected internal virtual ConfigurationPropertyCollection Properties
		{
			get
			{
				if (this.map == null)
				{
					this.map = ElementMap.GetMap(base.GetType());
				}
				return this.map.Properties;
			}
		}

		/// <summary>Compares the current <see cref="T:System.Configuration.ConfigurationElement" /> instance to the specified object.</summary>
		/// <param name="compareTo">The object to compare with.</param>
		/// <returns>
		///   <see langword="true" /> if the object to compare with is equal to the current <see cref="T:System.Configuration.ConfigurationElement" /> instance; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public override bool Equals(object compareTo)
		{
			ConfigurationElement configurationElement = compareTo as ConfigurationElement;
			if (configurationElement == null)
			{
				return false;
			}
			if (base.GetType() != configurationElement.GetType())
			{
				return false;
			}
			foreach (object obj in this.Properties)
			{
				ConfigurationProperty prop = (ConfigurationProperty)obj;
				if (!object.Equals(this[prop], configurationElement[prop]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Gets a unique value representing the current <see cref="T:System.Configuration.ConfigurationElement" /> instance.</summary>
		/// <returns>A unique value representing the current <see cref="T:System.Configuration.ConfigurationElement" /> instance.</returns>
		public override int GetHashCode()
		{
			int num = 0;
			foreach (object obj in this.Properties)
			{
				ConfigurationProperty prop = (ConfigurationProperty)obj;
				object obj2 = this[prop];
				if (obj2 != null)
				{
					num += obj2.GetHashCode();
				}
			}
			return num;
		}

		internal virtual bool HasLocalModifications()
		{
			foreach (object obj in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj;
				if (propertyInformation.ValueOrigin == PropertyValueOrigin.SetHere && propertyInformation.IsModified)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Reads XML from the configuration file.</summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> that reads from the configuration file.</param>
		/// <param name="serializeCollectionKey">
		///   <see langword="true" /> to serialize only the collection key properties; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The element to read is locked.  
		/// -or-
		///  An attribute of the current node is not recognized.  
		/// -or-
		///  The lock status of the current node cannot be determined.</exception>
		protected internal virtual void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
		{
			Hashtable hashtable = new Hashtable();
			reader.MoveToContent();
			this.elementPresent = true;
			while (reader.MoveToNextAttribute())
			{
				PropertyInformation propertyInformation = this.ElementInformation.Properties[reader.LocalName];
				if (propertyInformation == null || (serializeCollectionKey && !propertyInformation.IsKey))
				{
					if (reader.LocalName == "lockAllAttributesExcept")
					{
						this.LockAllAttributesExcept.SetFromList(reader.Value);
					}
					else if (reader.LocalName == "lockAllElementsExcept")
					{
						this.LockAllElementsExcept.SetFromList(reader.Value);
					}
					else if (reader.LocalName == "lockAttributes")
					{
						this.LockAttributes.SetFromList(reader.Value);
					}
					else if (reader.LocalName == "lockElements")
					{
						this.LockElements.SetFromList(reader.Value);
					}
					else if (reader.LocalName == "lockItem")
					{
						this.LockItem = (reader.Value.ToLowerInvariant() == "true");
					}
					else if (!(reader.LocalName == "xmlns") && (!(this is ConfigurationSection) || !(reader.LocalName == "configSource")) && !this.OnDeserializeUnrecognizedAttribute(reader.LocalName, reader.Value))
					{
						throw new ConfigurationErrorsException("Unrecognized attribute '" + reader.LocalName + "'.", reader);
					}
				}
				else
				{
					if (hashtable.ContainsKey(propertyInformation))
					{
						throw new ConfigurationErrorsException("The attribute '" + propertyInformation.Name + "' may only appear once in this element.", reader);
					}
					try
					{
						string value = reader.Value;
						this.ValidateValue(propertyInformation.Property, value);
						propertyInformation.SetStringValue(value);
					}
					catch (ConfigurationErrorsException)
					{
						throw;
					}
					catch (ConfigurationException)
					{
						throw;
					}
					catch (Exception ex)
					{
						throw new ConfigurationErrorsException(string.Format("The value for the property '{0}' is not valid. The error is: {1}", propertyInformation.Name, ex.Message), reader);
					}
					hashtable[propertyInformation] = propertyInformation.Name;
					ConfigXmlTextReader configXmlTextReader = reader as ConfigXmlTextReader;
					if (configXmlTextReader != null)
					{
						propertyInformation.Source = configXmlTextReader.Filename;
						propertyInformation.LineNumber = configXmlTextReader.LineNumber;
					}
				}
			}
			reader.MoveToElement();
			if (!reader.IsEmptyElement)
			{
				int depth = reader.Depth;
				reader.ReadStartElement();
				reader.MoveToContent();
				PropertyInformation propertyInformation2;
				for (;;)
				{
					if (reader.NodeType != XmlNodeType.Element)
					{
						reader.Skip();
					}
					else
					{
						propertyInformation2 = this.ElementInformation.Properties[reader.LocalName];
						if (propertyInformation2 == null || (serializeCollectionKey && !propertyInformation2.IsKey))
						{
							if (!this.OnDeserializeUnrecognizedElement(reader.LocalName, reader))
							{
								if (propertyInformation2 != null)
								{
									break;
								}
								ConfigurationElementCollection configurationElementCollection = this.GetDefaultCollection();
								if (configurationElementCollection == null || !configurationElementCollection.OnDeserializeUnrecognizedElement(reader.LocalName, reader))
								{
									break;
								}
							}
						}
						else
						{
							if (!propertyInformation2.IsElement)
							{
								goto Block_22;
							}
							if (hashtable.Contains(propertyInformation2))
							{
								goto Block_23;
							}
							((ConfigurationElement)propertyInformation2.Value).DeserializeElement(reader, serializeCollectionKey);
							hashtable[propertyInformation2] = propertyInformation2.Name;
							if (depth == reader.Depth)
							{
								reader.Read();
							}
						}
					}
					if (depth >= reader.Depth)
					{
						goto IL_367;
					}
				}
				throw new ConfigurationErrorsException("Unrecognized element '" + reader.LocalName + "'.", reader);
				Block_22:
				throw new ConfigurationErrorsException("Property '" + propertyInformation2.Name + "' is not a ConfigurationElement.");
				Block_23:
				throw new ConfigurationErrorsException("The element <" + propertyInformation2.Name + "> may only appear once in this section.", reader);
			}
			reader.Skip();
			IL_367:
			this.modified = false;
			foreach (object obj in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation3 = (PropertyInformation)obj;
				if (!string.IsNullOrEmpty(propertyInformation3.Name) && propertyInformation3.IsRequired && !hashtable.ContainsKey(propertyInformation3) && this.ElementInformation.Properties[propertyInformation3.Name] == null)
				{
					object obj2 = this.OnRequiredPropertyNotFound(propertyInformation3.Name);
					if (!object.Equals(obj2, propertyInformation3.DefaultValue))
					{
						propertyInformation3.Value = obj2;
						propertyInformation3.IsModified = false;
					}
				}
			}
			this.PostDeserialize();
		}

		/// <summary>Gets a value indicating whether an unknown attribute is encountered during deserialization.</summary>
		/// <param name="name">The name of the unrecognized attribute.</param>
		/// <param name="value">The value of the unrecognized attribute.</param>
		/// <returns>
		///   <see langword="true" /> when an unknown attribute is encountered while deserializing; otherwise, <see langword="false" />.</returns>
		protected virtual bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			return false;
		}

		/// <summary>Gets a value indicating whether an unknown element is encountered during deserialization.</summary>
		/// <param name="elementName">The name of the unknown subelement.</param>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> being used for deserialization.</param>
		/// <returns>
		///   <see langword="true" /> when an unknown element is encountered while deserializing; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The element identified by <paramref name="elementName" /> is locked.  
		/// -or-
		///  One or more of the element's attributes is locked.  
		/// -or-
		///  <paramref name="elementName" /> is unrecognized, or the element has an unrecognized attribute.  
		/// -or-
		///  The element has a Boolean attribute with an invalid value.  
		/// -or-
		///  An attempt was made to deserialize a property more than once.  
		/// -or-
		///  An attempt was made to deserialize a property that is not a valid member of the element.  
		/// -or-
		///  The element cannot contain a CDATA or text element.</exception>
		protected virtual bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
		{
			return false;
		}

		/// <summary>Throws an exception when a required property is not found.</summary>
		/// <param name="name">The name of the required attribute that was not found.</param>
		/// <returns>None.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">In all cases.</exception>
		protected virtual object OnRequiredPropertyNotFound(string name)
		{
			throw new ConfigurationErrorsException("Required attribute '" + name + "' not found.");
		}

		/// <summary>Called before serialization.</summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> that will be used to serialize the <see cref="T:System.Configuration.ConfigurationElement" />.</param>
		protected virtual void PreSerialize(XmlWriter writer)
		{
		}

		/// <summary>Called after deserialization.</summary>
		protected virtual void PostDeserialize()
		{
		}

		/// <summary>Used to initialize a default set of values for the <see cref="T:System.Configuration.ConfigurationElement" /> object.</summary>
		protected internal virtual void InitializeDefault()
		{
		}

		/// <summary>Indicates whether this configuration element has been modified since it was last saved or loaded, when implemented in a derived class.</summary>
		/// <returns>
		///   <see langword="true" /> if the element has been modified; otherwise, <see langword="false" />.</returns>
		protected internal virtual bool IsModified()
		{
			if (this.modified)
			{
				return true;
			}
			foreach (object obj in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj;
				if (propertyInformation.IsElement)
				{
					ConfigurationElement configurationElement = propertyInformation.Value as ConfigurationElement;
					if (configurationElement != null && configurationElement.IsModified())
					{
						this.modified = true;
						break;
					}
				}
			}
			return this.modified;
		}

		/// <summary>Sets the <see cref="M:System.Configuration.ConfigurationElement.IsReadOnly" /> property for the <see cref="T:System.Configuration.ConfigurationElement" /> object and all subelements.</summary>
		protected internal virtual void SetReadOnly()
		{
			this.readOnly = true;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Configuration.ConfigurationElement" /> object is read-only.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.ConfigurationElement" /> object is read-only; otherwise, <see langword="false" />.</returns>
		public virtual bool IsReadOnly()
		{
			return this.readOnly;
		}

		/// <summary>Resets the internal state of the <see cref="T:System.Configuration.ConfigurationElement" /> object, including the locks and the properties collections.</summary>
		/// <param name="parentElement">The parent node of the configuration element.</param>
		protected internal virtual void Reset(ConfigurationElement parentElement)
		{
			this.elementPresent = false;
			if (parentElement != null)
			{
				this.ElementInformation.Reset(parentElement.ElementInformation);
				return;
			}
			this.InitializeDefault();
		}

		/// <summary>Resets the value of the <see cref="M:System.Configuration.ConfigurationElement.IsModified" /> method to <see langword="false" /> when implemented in a derived class.</summary>
		protected internal virtual void ResetModified()
		{
			this.modified = false;
			foreach (object obj in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj;
				propertyInformation.IsModified = false;
				ConfigurationElement configurationElement = propertyInformation.Value as ConfigurationElement;
				if (configurationElement != null)
				{
					configurationElement.ResetModified();
				}
			}
		}

		/// <summary>Writes the contents of this configuration element to the configuration file when implemented in a derived class.</summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> that writes to the configuration file.</param>
		/// <param name="serializeCollectionKey">
		///   <see langword="true" /> to serialize only the collection key properties; otherwise, <see langword="false" />.</param>
		/// <returns>
		///   <see langword="true" /> if any data was actually serialized; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The current attribute is locked at a higher configuration level.</exception>
		protected internal virtual bool SerializeElement(XmlWriter writer, bool serializeCollectionKey)
		{
			this.PreSerialize(writer);
			if (serializeCollectionKey)
			{
				ConfigurationPropertyCollection keyProperties = this.GetKeyProperties();
				foreach (object obj in keyProperties)
				{
					ConfigurationProperty configurationProperty = (ConfigurationProperty)obj;
					writer.WriteAttributeString(configurationProperty.Name, configurationProperty.ConvertToString(this[configurationProperty.Name]));
				}
				return keyProperties.Count > 0;
			}
			bool flag = false;
			foreach (object obj2 in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj2;
				if (!propertyInformation.IsElement)
				{
					if (this.saveContext == null)
					{
						throw new InvalidOperationException();
					}
					if (this.saveContext.HasValue(propertyInformation))
					{
						writer.WriteAttributeString(propertyInformation.Name, propertyInformation.GetStringValue());
						flag = true;
					}
				}
			}
			foreach (object obj3 in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation2 = (PropertyInformation)obj3;
				if (propertyInformation2.IsElement)
				{
					ConfigurationElement configurationElement = (ConfigurationElement)propertyInformation2.Value;
					if (configurationElement != null)
					{
						flag = (configurationElement.SerializeToXmlElement(writer, propertyInformation2.Name) || flag);
					}
				}
			}
			return flag;
		}

		/// <summary>Writes the outer tags of this configuration element to the configuration file when implemented in a derived class.</summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> that writes to the configuration file.</param>
		/// <param name="elementName">The name of the <see cref="T:System.Configuration.ConfigurationElement" /> to be written.</param>
		/// <returns>
		///   <see langword="true" /> if writing was successful; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.Exception">The element has multiple child elements.</exception>
		protected internal virtual bool SerializeToXmlElement(XmlWriter writer, string elementName)
		{
			if (this.saveContext == null)
			{
				throw new InvalidOperationException();
			}
			if (!this.saveContext.HasValues())
			{
				return false;
			}
			if (elementName != null && elementName != "")
			{
				writer.WriteStartElement(elementName);
			}
			bool result = this.SerializeElement(writer, false);
			if (elementName != null && elementName != "")
			{
				writer.WriteEndElement();
			}
			return result;
		}

		/// <summary>Modifies the <see cref="T:System.Configuration.ConfigurationElement" /> object to remove all values that should not be saved.</summary>
		/// <param name="sourceElement">A <see cref="T:System.Configuration.ConfigurationElement" /> at the current level containing a merged view of the properties.</param>
		/// <param name="parentElement">The parent <see cref="T:System.Configuration.ConfigurationElement" />, or <see langword="null" /> if this is the top level.</param>
		/// <param name="saveMode">One of the enumeration values that determines which property values to include.</param>
		protected internal virtual void Unmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
		{
			if (parentElement != null && sourceElement.GetType() != parentElement.GetType())
			{
				throw new ConfigurationErrorsException("Can't unmerge two elements of different type");
			}
			bool flag = saveMode == ConfigurationSaveMode.Minimal || saveMode == ConfigurationSaveMode.Modified;
			foreach (object obj in sourceElement.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj;
				if (propertyInformation.ValueOrigin != PropertyValueOrigin.Default)
				{
					PropertyInformation propertyInformation2 = this.ElementInformation.Properties[propertyInformation.Name];
					object value = propertyInformation.Value;
					if (parentElement == null || !parentElement.HasValue(propertyInformation.Name))
					{
						propertyInformation2.Value = value;
					}
					else if (value != null)
					{
						object obj2 = parentElement[propertyInformation.Name];
						if (!propertyInformation.IsElement)
						{
							if (!object.Equals(value, obj2) || saveMode == ConfigurationSaveMode.Full || (saveMode == ConfigurationSaveMode.Modified && propertyInformation.ValueOrigin == PropertyValueOrigin.SetHere))
							{
								propertyInformation2.Value = value;
							}
						}
						else
						{
							ConfigurationElement configurationElement = (ConfigurationElement)value;
							if (!flag || configurationElement.IsModified())
							{
								if (obj2 == null)
								{
									propertyInformation2.Value = value;
								}
								else
								{
									ConfigurationElement parentElement2 = (ConfigurationElement)obj2;
									((ConfigurationElement)propertyInformation2.Value).Unmerge(configurationElement, parentElement2, saveMode);
								}
							}
						}
					}
				}
			}
		}

		internal bool HasValue(string propName)
		{
			PropertyInformation propertyInformation = this.ElementInformation.Properties[propName];
			return propertyInformation != null && propertyInformation.ValueOrigin > PropertyValueOrigin.Default;
		}

		internal bool IsReadFromConfig(string propName)
		{
			PropertyInformation propertyInformation = this.ElementInformation.Properties[propName];
			return propertyInformation != null && propertyInformation.ValueOrigin == PropertyValueOrigin.SetHere;
		}

		internal bool IsElementPresent
		{
			get
			{
				return this.elementPresent;
			}
		}

		private void ValidateValue(ConfigurationProperty p, string value)
		{
			ConfigurationValidatorBase validator;
			if (p == null || (validator = p.Validator) == null)
			{
				return;
			}
			if (!validator.CanValidate(p.Type))
			{
				throw new ConfigurationErrorsException(string.Format("Validator does not support type {0}", p.Type));
			}
			validator.Validate(p.ConvertFromString(value));
		}

		internal bool HasValue(ConfigurationElement parent, PropertyInformation prop, ConfigurationSaveMode mode)
		{
			if (prop.ValueOrigin == PropertyValueOrigin.Default)
			{
				return false;
			}
			if (mode == ConfigurationSaveMode.Modified && prop.ValueOrigin == PropertyValueOrigin.SetHere && prop.IsModified)
			{
				return true;
			}
			object obj = (parent != null && parent.HasValue(prop.Name)) ? parent[prop.Name] : prop.DefaultValue;
			if (!prop.IsElement)
			{
				return !object.Equals(prop.Value, obj);
			}
			ConfigurationElement configurationElement = (ConfigurationElement)prop.Value;
			ConfigurationElement parent2 = (ConfigurationElement)obj;
			return configurationElement.HasValues(parent2, mode);
		}

		internal virtual bool HasValues(ConfigurationElement parent, ConfigurationSaveMode mode)
		{
			if (mode == ConfigurationSaveMode.Full)
			{
				return true;
			}
			if (this.modified && mode == ConfigurationSaveMode.Modified)
			{
				return true;
			}
			foreach (object obj in this.ElementInformation.Properties)
			{
				PropertyInformation prop = (PropertyInformation)obj;
				if (this.HasValue(parent, prop, mode))
				{
					return true;
				}
			}
			return false;
		}

		internal virtual void PrepareSave(ConfigurationElement parent, ConfigurationSaveMode mode)
		{
			this.saveContext = new ConfigurationElement.SaveContext(this, parent, mode);
			foreach (object obj in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj;
				if (propertyInformation.IsElement)
				{
					ConfigurationElement configurationElement = (ConfigurationElement)propertyInformation.Value;
					if (parent == null || !parent.HasValue(propertyInformation.Name))
					{
						configurationElement.PrepareSave(null, mode);
					}
					else
					{
						ConfigurationElement parent2 = (ConfigurationElement)parent[propertyInformation.Name];
						configurationElement.PrepareSave(parent2, mode);
					}
				}
			}
		}

		/// <summary>Gets a reference to the top-level <see cref="T:System.Configuration.Configuration" /> instance that represents the configuration hierarchy that the current <see cref="T:System.Configuration.ConfigurationElement" /> instance belongs to.</summary>
		/// <returns>The top-level <see cref="T:System.Configuration.Configuration" /> instance that the current <see cref="T:System.Configuration.ConfigurationElement" /> instance belongs to.</returns>
		public Configuration CurrentConfiguration
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets a value that indicates whether the <see cref="P:System.Configuration.ConfigurationElement.CurrentConfiguration" /> property is <see langword="null" />.</summary>
		/// <returns>false if the <see cref="P:System.Configuration.ConfigurationElement.CurrentConfiguration" /> property is <see langword="null" />; otherwise, <see langword="true" />.</returns>
		protected bool HasContext
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		/// <summary>Returns the transformed version of the specified assembly name.</summary>
		/// <param name="assemblyName">The name of the assembly.</param>
		/// <returns>The transformed version of the assembly name. If no transformer is available, the <paramref name="assemblyName" /> parameter value is returned unchanged. The <see cref="P:System.Configuration.Configuration.TypeStringTransformer" /> property is <see langword="null" /> if no transformer is available.</returns>
		protected virtual string GetTransformedAssemblyString(string assemblyName)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>Returns the transformed version of the specified type name.</summary>
		/// <param name="typeName">The name of the type.</param>
		/// <returns>The transformed version of the specified type name. If no transformer is available, the <paramref name="typeName" /> parameter value is returned unchanged. The <see cref="P:System.Configuration.Configuration.TypeStringTransformer" /> property is <see langword="null" /> if no transformer is available.</returns>
		protected virtual string GetTransformedTypeString(string typeName)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		private string rawXml;

		private bool modified;

		private ElementMap map;

		private ConfigurationPropertyCollection keyProps;

		private ConfigurationElementCollection defaultCollection;

		private bool readOnly;

		private ElementInformation elementInfo;

		private ConfigurationElementProperty elementProperty;

		private Configuration _configuration;

		private bool elementPresent;

		private ConfigurationLockCollection lockAllAttributesExcept;

		private ConfigurationLockCollection lockAllElementsExcept;

		private ConfigurationLockCollection lockAttributes;

		private ConfigurationLockCollection lockElements;

		private bool lockItem;

		private ConfigurationElement.SaveContext saveContext;

		private class SaveContext
		{
			public SaveContext(ConfigurationElement element, ConfigurationElement parent, ConfigurationSaveMode mode)
			{
				this.Element = element;
				this.Parent = parent;
				this.Mode = mode;
			}

			public bool HasValues()
			{
				return this.Mode == ConfigurationSaveMode.Full || this.Element.HasValues(this.Parent, this.Mode);
			}

			public bool HasValue(PropertyInformation prop)
			{
				return this.Mode == ConfigurationSaveMode.Full || this.Element.HasValue(this.Parent, prop, this.Mode);
			}

			public readonly ConfigurationElement Element;

			public readonly ConfigurationElement Parent;

			public readonly ConfigurationSaveMode Mode;
		}
	}
}
