using System;
using System.IO;
using System.Runtime.Versioning;
using System.Xml;
using Unity;

namespace System.Configuration
{
	/// <summary>Represents a section within a configuration file.</summary>
	public abstract class ConfigurationSection : ConfigurationElement
	{
		internal string ExternalDataXml
		{
			get
			{
				return this.externalDataXml;
			}
		}

		internal IConfigurationSectionHandler SectionHandler
		{
			get
			{
				return this.section_handler;
			}
			set
			{
				this.section_handler = value;
			}
		}

		/// <summary>Gets a <see cref="T:System.Configuration.SectionInformation" /> object that contains the non-customizable information and functionality of the <see cref="T:System.Configuration.ConfigurationSection" /> object.</summary>
		/// <returns>A <see cref="T:System.Configuration.SectionInformation" /> that contains the non-customizable information and functionality of the <see cref="T:System.Configuration.ConfigurationSection" />.</returns>
		[MonoTODO]
		public SectionInformation SectionInformation
		{
			get
			{
				if (this.sectionInformation == null)
				{
					this.sectionInformation = new SectionInformation();
				}
				return this.sectionInformation;
			}
		}

		internal object ConfigContext
		{
			get
			{
				return this._configContext;
			}
			set
			{
				this._configContext = value;
			}
		}

		/// <summary>Returns a custom object when overridden in a derived class.</summary>
		/// <returns>The object representing the section.</returns>
		[MonoTODO("Provide ConfigContext. Likely the culprit of bug #322493")]
		protected internal virtual object GetRuntimeObject()
		{
			if (this.SectionHandler == null)
			{
				return this;
			}
			ConfigurationSection configurationSection = (this.sectionInformation != null) ? this.sectionInformation.GetParentSection() : null;
			object obj = (configurationSection != null) ? configurationSection.GetRuntimeObject() : null;
			if (base.RawXml == null)
			{
				return obj;
			}
			try
			{
				XmlReader reader = new ConfigXmlTextReader(new StringReader(base.RawXml), base.Configuration.FilePath);
				this.DoDeserializeSection(reader);
				if (!string.IsNullOrEmpty(this.SectionInformation.ConfigSource))
				{
					string text = this.SectionInformation.ConfigFilePath;
					if (!string.IsNullOrEmpty(text))
					{
						text = Path.GetDirectoryName(text);
					}
					else
					{
						text = string.Empty;
					}
					string path = Path.Combine(text, this.SectionInformation.ConfigSource);
					if (File.Exists(path))
					{
						base.RawXml = File.ReadAllText(path);
						this.SectionInformation.SetRawXml(base.RawXml);
					}
				}
			}
			catch
			{
			}
			XmlDocument xmlDocument = new ConfigurationXmlDocument();
			xmlDocument.LoadXml(base.RawXml);
			return this.SectionHandler.Create(obj, this.ConfigContext, xmlDocument.DocumentElement);
		}

		/// <summary>Indicates whether this configuration element has been modified since it was last saved or loaded when implemented in a derived class.</summary>
		/// <returns>
		///   <see langword="true" /> if the element has been modified; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		protected internal override bool IsModified()
		{
			return base.IsModified();
		}

		/// <summary>Resets the value of the <see cref="M:System.Configuration.ConfigurationElement.IsModified" /> method to <see langword="false" /> when implemented in a derived class.</summary>
		[MonoTODO]
		protected internal override void ResetModified()
		{
			base.ResetModified();
		}

		private ConfigurationElement CreateElement(Type t)
		{
			ConfigurationElement configurationElement = (ConfigurationElement)Activator.CreateInstance(t);
			configurationElement.Init();
			configurationElement.Configuration = base.Configuration;
			if (this.IsReadOnly())
			{
				configurationElement.SetReadOnly();
			}
			return configurationElement;
		}

		private void DoDeserializeSection(XmlReader reader)
		{
			reader.MoveToContent();
			string text = null;
			string text2 = null;
			while (reader.MoveToNextAttribute())
			{
				string localName = reader.LocalName;
				if (localName == "configProtectionProvider")
				{
					text = reader.Value;
				}
				else if (localName == "configSource")
				{
					text2 = reader.Value;
				}
			}
			if (text != null)
			{
				ProtectedConfigurationProvider provider = ProtectedConfiguration.GetProvider(text, true);
				XmlDocument xmlDocument = new ConfigurationXmlDocument();
				reader.MoveToElement();
				xmlDocument.Load(new StringReader(reader.ReadInnerXml()));
				reader = new XmlNodeReader(provider.Decrypt(xmlDocument));
				this.SectionInformation.ProtectSection(text);
				reader.MoveToContent();
			}
			if (text2 != null)
			{
				this.SectionInformation.ConfigSource = text2;
			}
			this.SectionInformation.SetRawXml(base.RawXml);
			if (this.SectionHandler == null)
			{
				this.DeserializeElement(reader, false);
			}
		}

		/// <summary>Reads XML from the configuration file.</summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> object, which reads from the configuration file.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">
		///   <paramref name="reader" /> found no elements in the configuration file.</exception>
		[MonoInternalNote("find the proper location for the decryption stuff")]
		protected internal virtual void DeserializeSection(XmlReader reader)
		{
			try
			{
				this.DoDeserializeSection(reader);
			}
			catch (ConfigurationErrorsException ex)
			{
				throw new ConfigurationErrorsException(string.Format("Error deserializing configuration section {0}: {1}", this.SectionInformation.Name, ex.Message));
			}
		}

		internal void DeserializeConfigSource(string basePath)
		{
			string configSource = this.SectionInformation.ConfigSource;
			if (string.IsNullOrEmpty(configSource))
			{
				return;
			}
			if (Path.IsPathRooted(configSource))
			{
				throw new ConfigurationErrorsException("The configSource attribute must be a relative physical path.");
			}
			if (this.HasLocalModifications())
			{
				throw new ConfigurationErrorsException("A section using 'configSource' may contain no other attributes or elements.");
			}
			string text = Path.Combine(basePath, configSource);
			if (!File.Exists(text))
			{
				base.RawXml = null;
				this.SectionInformation.SetRawXml(null);
				throw new ConfigurationErrorsException(string.Format("Unable to open configSource file '{0}'.", text));
			}
			base.RawXml = File.ReadAllText(text);
			this.SectionInformation.SetRawXml(base.RawXml);
			this.DeserializeElement(new ConfigXmlTextReader(new StringReader(base.RawXml), text), false);
		}

		/// <summary>Creates an XML string containing an unmerged view of the <see cref="T:System.Configuration.ConfigurationSection" /> object as a single section to write to a file.</summary>
		/// <param name="parentElement">The <see cref="T:System.Configuration.ConfigurationElement" /> instance to use as the parent when performing the un-merge.</param>
		/// <param name="name">The name of the section to create.</param>
		/// <param name="saveMode">The <see cref="T:System.Configuration.ConfigurationSaveMode" /> instance to use when writing to a string.</param>
		/// <returns>An XML string containing an unmerged view of the <see cref="T:System.Configuration.ConfigurationSection" /> object.</returns>
		protected internal virtual string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
		{
			this.externalDataXml = null;
			ConfigurationElement configurationElement;
			if (parentElement != null)
			{
				configurationElement = this.CreateElement(base.GetType());
				configurationElement.Unmerge(this, parentElement, saveMode);
			}
			else
			{
				configurationElement = this;
			}
			configurationElement.PrepareSave(parentElement, saveMode);
			bool flag = configurationElement.HasValues(parentElement, saveMode);
			string result;
			using (StringWriter stringWriter = new StringWriter())
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
				{
					xmlTextWriter.Formatting = Formatting.Indented;
					if (flag)
					{
						configurationElement.SerializeToXmlElement(xmlTextWriter, name);
					}
					else if (saveMode == ConfigurationSaveMode.Modified && configurationElement.IsModified())
					{
						xmlTextWriter.WriteStartElement(name);
						xmlTextWriter.WriteEndElement();
					}
					xmlTextWriter.Close();
				}
				result = stringWriter.ToString();
			}
			string configSource = this.SectionInformation.ConfigSource;
			if (string.IsNullOrEmpty(configSource))
			{
				return result;
			}
			this.externalDataXml = result;
			string result2;
			using (StringWriter stringWriter2 = new StringWriter())
			{
				bool flag2 = !string.IsNullOrEmpty(name);
				using (XmlTextWriter xmlTextWriter2 = new XmlTextWriter(stringWriter2))
				{
					if (flag2)
					{
						xmlTextWriter2.WriteStartElement(name);
					}
					xmlTextWriter2.WriteAttributeString("configSource", configSource);
					if (flag2)
					{
						xmlTextWriter2.WriteEndElement();
					}
				}
				result2 = stringWriter2.ToString();
			}
			return result2;
		}

		/// <summary>Indicates whether the specified element should be serialized when the configuration object hierarchy is serialized for the specified target version of the .NET Framework.</summary>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> object that is a candidate for serialization.</param>
		/// <param name="elementName">The name of the <see cref="T:System.Configuration.ConfigurationElement" /> object as it occurs in XML.</param>
		/// <param name="targetFramework">The target version of the .NET Framework.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="element" /> should be serialized; otherwise, <see langword="false" />.</returns>
		protected internal virtual bool ShouldSerializeElementInTargetVersion(ConfigurationElement element, string elementName, FrameworkName targetFramework)
		{
			ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		/// <summary>Indicates whether the specified property should be serialized when the configuration object hierarchy is serialized for the specified target version of the .NET Framework.</summary>
		/// <param name="property">The <see cref="T:System.Configuration.ConfigurationProperty" /> object that is a candidate for serialization.</param>
		/// <param name="propertyName">The name of the <see cref="T:System.Configuration.ConfigurationProperty" /> object as it occurs in XML.</param>
		/// <param name="targetFramework">The target version of the .NET Framework.</param>
		/// <param name="parentConfigurationElement">The parent element of the property.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="property" /> should be serialized; otherwise, <see langword="false" />.</returns>
		protected internal virtual bool ShouldSerializePropertyInTargetVersion(ConfigurationProperty property, string propertyName, FrameworkName targetFramework, ConfigurationElement parentConfigurationElement)
		{
			ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		/// <summary>Indicates whether the current <see cref="T:System.Configuration.ConfigurationSection" /> instance should be serialized when the configuration object hierarchy is serialized for the specified target version of the .NET Framework.</summary>
		/// <param name="targetFramework">The target version of the .NET Framework.</param>
		/// <returns>
		///   <see langword="true" /> if the current section should be serialized; otherwise, <see langword="false" />.</returns>
		protected internal virtual bool ShouldSerializeSectionInTargetVersion(FrameworkName targetFramework)
		{
			ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		private SectionInformation sectionInformation;

		private IConfigurationSectionHandler section_handler;

		private string externalDataXml;

		private object _configContext;
	}
}
