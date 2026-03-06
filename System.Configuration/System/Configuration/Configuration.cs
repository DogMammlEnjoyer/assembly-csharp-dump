using System;
using System.Collections;
using System.Configuration.Internal;
using System.IO;
using System.Runtime.Versioning;
using System.Security.Permissions;
using System.Xml;
using Unity;

namespace System.Configuration
{
	/// <summary>Represents a configuration file that is applicable to a particular computer, application, or resource. This class cannot be inherited.</summary>
	public sealed class Configuration
	{
		internal static event ConfigurationSaveEventHandler SaveStart;

		internal static event ConfigurationSaveEventHandler SaveEnd;

		internal Configuration(Configuration parent, string locationSubPath)
		{
			this.elementData = new Hashtable();
			base..ctor();
			this.parent = parent;
			this.system = parent.system;
			this.rootGroup = parent.rootGroup;
			this.locationSubPath = locationSubPath;
			this.configPath = parent.ConfigPath;
		}

		internal Configuration(InternalConfigurationSystem system, string locationSubPath)
		{
			this.elementData = new Hashtable();
			base..ctor();
			this.hasFile = true;
			this.system = system;
			system.InitForConfiguration(ref locationSubPath, out this.configPath, out this.locationConfigPath);
			Configuration configuration = null;
			if (locationSubPath != null)
			{
				configuration = new Configuration(system, locationSubPath);
				if (this.locationConfigPath != null)
				{
					configuration = configuration.FindLocationConfiguration(this.locationConfigPath, configuration);
				}
			}
			this.Init(system, this.configPath, configuration);
		}

		internal Configuration FindLocationConfiguration(string relativePath, Configuration defaultConfiguration)
		{
			Configuration configuration = defaultConfiguration;
			if (!string.IsNullOrEmpty(this.LocationConfigPath))
			{
				Configuration parentWithFile = this.GetParentWithFile();
				if (parentWithFile != null)
				{
					string configPathFromLocationSubPath = this.system.Host.GetConfigPathFromLocationSubPath(this.configPath, relativePath);
					configuration = parentWithFile.FindLocationConfiguration(configPathFromLocationSubPath, defaultConfiguration);
				}
			}
			string text = this.configPath.Substring(1) + "/";
			if (relativePath.StartsWith(text, StringComparison.Ordinal))
			{
				relativePath = relativePath.Substring(text.Length);
			}
			ConfigurationLocation configurationLocation = this.Locations.FindBest(relativePath);
			if (configurationLocation == null)
			{
				return configuration;
			}
			configurationLocation.SetParentConfiguration(configuration);
			return configurationLocation.OpenConfiguration();
		}

		internal void Init(IConfigSystem system, string configPath, Configuration parent)
		{
			this.system = system;
			this.configPath = configPath;
			this.streamName = system.Host.GetStreamName(configPath);
			this.parent = parent;
			if (parent != null)
			{
				this.rootGroup = parent.rootGroup;
			}
			else
			{
				this.rootGroup = new SectionGroupInfo();
				this.rootGroup.StreamName = this.streamName;
			}
			try
			{
				if (this.streamName != null)
				{
					this.Load();
				}
			}
			catch (XmlException ex)
			{
				throw new ConfigurationErrorsException(ex.Message, ex, this.streamName, 0);
			}
		}

		internal Configuration Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		internal Configuration GetParentWithFile()
		{
			Configuration configuration = this.Parent;
			while (configuration != null && !configuration.HasFile)
			{
				configuration = configuration.Parent;
			}
			return configuration;
		}

		internal string FileName
		{
			get
			{
				return this.streamName;
			}
		}

		internal IInternalConfigHost ConfigHost
		{
			get
			{
				return this.system.Host;
			}
		}

		internal string LocationConfigPath
		{
			get
			{
				return this.locationConfigPath;
			}
		}

		internal string GetLocationSubPath()
		{
			Configuration configuration = this.parent;
			string text = null;
			while (configuration != null)
			{
				text = configuration.locationSubPath;
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				configuration = configuration.parent;
			}
			return text;
		}

		internal string ConfigPath
		{
			get
			{
				return this.configPath;
			}
		}

		/// <summary>Gets the <see cref="T:System.Configuration.AppSettingsSection" /> object configuration section that applies to this <see cref="T:System.Configuration.Configuration" /> object.</summary>
		/// <returns>An <see cref="T:System.Configuration.AppSettingsSection" /> object representing the <see langword="appSettings" /> configuration section that applies to this <see cref="T:System.Configuration.Configuration" /> object.</returns>
		public AppSettingsSection AppSettings
		{
			get
			{
				return (AppSettingsSection)this.GetSection("appSettings");
			}
		}

		/// <summary>Gets a <see cref="T:System.Configuration.ConnectionStringsSection" /> configuration-section object that applies to this <see cref="T:System.Configuration.Configuration" /> object.</summary>
		/// <returns>A <see cref="T:System.Configuration.ConnectionStringsSection" /> configuration-section object representing the <see langword="connectionStrings" /> configuration section that applies to this <see cref="T:System.Configuration.Configuration" /> object.</returns>
		public ConnectionStringsSection ConnectionStrings
		{
			get
			{
				return (ConnectionStringsSection)this.GetSection("connectionStrings");
			}
		}

		/// <summary>Gets the physical path to the configuration file represented by this <see cref="T:System.Configuration.Configuration" /> object.</summary>
		/// <returns>The physical path to the configuration file represented by this <see cref="T:System.Configuration.Configuration" /> object.</returns>
		public string FilePath
		{
			get
			{
				if (this.streamName == null && this.parent != null)
				{
					return this.parent.FilePath;
				}
				return this.streamName;
			}
		}

		/// <summary>Gets a value that indicates whether a file exists for the resource represented by this <see cref="T:System.Configuration.Configuration" /> object.</summary>
		/// <returns>
		///   <see langword="true" /> if there is a configuration file; otherwise, <see langword="false" />.</returns>
		public bool HasFile
		{
			get
			{
				return this.hasFile;
			}
		}

		/// <summary>Gets the <see cref="T:System.Configuration.ContextInformation" /> object for the <see cref="T:System.Configuration.Configuration" /> object.</summary>
		/// <returns>The <see cref="T:System.Configuration.ContextInformation" /> object for the <see cref="T:System.Configuration.Configuration" /> object.</returns>
		public ContextInformation EvaluationContext
		{
			get
			{
				if (this.evaluationContext == null)
				{
					object ctx = this.system.Host.CreateConfigurationContext(this.configPath, this.GetLocationSubPath());
					this.evaluationContext = new ContextInformation(this, ctx);
				}
				return this.evaluationContext;
			}
		}

		/// <summary>Gets the locations defined within this <see cref="T:System.Configuration.Configuration" /> object.</summary>
		/// <returns>A <see cref="T:System.Configuration.ConfigurationLocationCollection" /> containing the locations defined within this <see cref="T:System.Configuration.Configuration" /> object.</returns>
		public ConfigurationLocationCollection Locations
		{
			get
			{
				if (this.locations == null)
				{
					this.locations = new ConfigurationLocationCollection();
				}
				return this.locations;
			}
		}

		/// <summary>Gets or sets a value indicating whether the configuration file has an XML namespace.</summary>
		/// <returns>
		///   <see langword="true" /> if the configuration file has an XML namespace; otherwise, <see langword="false" />.</returns>
		public bool NamespaceDeclared
		{
			get
			{
				return this.rootNamespace != null;
			}
			set
			{
				this.rootNamespace = (value ? "http://schemas.microsoft.com/.NetConfiguration/v2.0" : null);
			}
		}

		/// <summary>Gets the root <see cref="T:System.Configuration.ConfigurationSectionGroup" /> for this <see cref="T:System.Configuration.Configuration" /> object.</summary>
		/// <returns>The root section group for this <see cref="T:System.Configuration.Configuration" /> object.</returns>
		public ConfigurationSectionGroup RootSectionGroup
		{
			get
			{
				if (this.rootSectionGroup == null)
				{
					this.rootSectionGroup = new ConfigurationSectionGroup();
					this.rootSectionGroup.Initialize(this, this.rootGroup);
				}
				return this.rootSectionGroup;
			}
		}

		/// <summary>Gets a collection of the section groups defined by this configuration.</summary>
		/// <returns>A <see cref="T:System.Configuration.ConfigurationSectionGroupCollection" /> collection representing the collection of section groups for this <see cref="T:System.Configuration.Configuration" /> object.</returns>
		public ConfigurationSectionGroupCollection SectionGroups
		{
			get
			{
				return this.RootSectionGroup.SectionGroups;
			}
		}

		/// <summary>Gets a collection of the sections defined by this <see cref="T:System.Configuration.Configuration" /> object.</summary>
		/// <returns>A collection of the sections defined by this <see cref="T:System.Configuration.Configuration" /> object.</returns>
		public ConfigurationSectionCollection Sections
		{
			get
			{
				return this.RootSectionGroup.Sections;
			}
		}

		/// <summary>Returns the specified <see cref="T:System.Configuration.ConfigurationSection" /> object.</summary>
		/// <param name="sectionName">The path to the section to be returned.</param>
		/// <returns>The specified <see cref="T:System.Configuration.ConfigurationSection" /> object.</returns>
		public ConfigurationSection GetSection(string sectionName)
		{
			string[] array = sectionName.Split('/', StringSplitOptions.None);
			if (array.Length == 1)
			{
				return this.Sections[array[0]];
			}
			ConfigurationSectionGroup configurationSectionGroup = this.SectionGroups[array[0]];
			int num = 1;
			while (configurationSectionGroup != null && num < array.Length - 1)
			{
				configurationSectionGroup = configurationSectionGroup.SectionGroups[array[num]];
				num++;
			}
			if (configurationSectionGroup != null)
			{
				return configurationSectionGroup.Sections[array[array.Length - 1]];
			}
			return null;
		}

		/// <summary>Gets the specified <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object.</summary>
		/// <param name="sectionGroupName">The path name of the <see cref="T:System.Configuration.ConfigurationSectionGroup" /> to return.</param>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationSectionGroup" /> specified.</returns>
		public ConfigurationSectionGroup GetSectionGroup(string sectionGroupName)
		{
			string[] array = sectionGroupName.Split('/', StringSplitOptions.None);
			ConfigurationSectionGroup configurationSectionGroup = this.SectionGroups[array[0]];
			int num = 1;
			while (configurationSectionGroup != null && num < array.Length)
			{
				configurationSectionGroup = configurationSectionGroup.SectionGroups[array[num]];
				num++;
			}
			return configurationSectionGroup;
		}

		internal ConfigurationSection GetSectionInstance(SectionInfo config, bool createDefaultInstance)
		{
			object obj = this.elementData[config];
			ConfigurationSection configurationSection = obj as ConfigurationSection;
			if (configurationSection != null || !createDefaultInstance)
			{
				return configurationSection;
			}
			object obj2 = config.CreateInstance();
			configurationSection = (obj2 as ConfigurationSection);
			if (configurationSection == null)
			{
				configurationSection = new DefaultSection
				{
					SectionHandler = (obj2 as IConfigurationSectionHandler)
				};
			}
			configurationSection.Configuration = this;
			ConfigurationSection configurationSection2 = null;
			if (this.parent != null)
			{
				configurationSection2 = this.parent.GetSectionInstance(config, true);
				configurationSection.SectionInformation.SetParentSection(configurationSection2);
			}
			configurationSection.SectionInformation.ConfigFilePath = this.FilePath;
			configurationSection.ConfigContext = this.system.Host.CreateDeprecatedConfigContext(this.configPath);
			string text = obj as string;
			configurationSection.RawXml = text;
			configurationSection.Reset(configurationSection2);
			if (text != null)
			{
				XmlTextReader xmlTextReader = new ConfigXmlTextReader(new StringReader(text), this.FilePath);
				configurationSection.DeserializeSection(xmlTextReader);
				xmlTextReader.Close();
				if (!string.IsNullOrEmpty(configurationSection.SectionInformation.ConfigSource) && !string.IsNullOrEmpty(this.FilePath))
				{
					configurationSection.DeserializeConfigSource(Path.GetDirectoryName(this.FilePath));
				}
			}
			this.elementData[config] = configurationSection;
			return configurationSection;
		}

		internal ConfigurationSectionGroup GetSectionGroupInstance(SectionGroupInfo group)
		{
			ConfigurationSectionGroup configurationSectionGroup = group.CreateInstance() as ConfigurationSectionGroup;
			if (configurationSectionGroup != null)
			{
				configurationSectionGroup.Initialize(this, group);
			}
			return configurationSectionGroup;
		}

		internal void SetConfigurationSection(SectionInfo config, ConfigurationSection sec)
		{
			this.elementData[config] = sec;
		}

		internal void SetSectionXml(SectionInfo config, string data)
		{
			this.elementData[config] = data;
		}

		internal string GetSectionXml(SectionInfo config)
		{
			return this.elementData[config] as string;
		}

		internal void CreateSection(SectionGroupInfo group, string name, ConfigurationSection sec)
		{
			if (group.HasChild(name))
			{
				throw new ConfigurationErrorsException("Cannot add a ConfigurationSection. A section or section group already exists with the name '" + name + "'");
			}
			if (!this.HasFile && !sec.SectionInformation.AllowLocation)
			{
				throw new ConfigurationErrorsException("The configuration section <" + name + "> cannot be defined inside a <location> element.");
			}
			if (!this.system.Host.IsDefinitionAllowed(this.configPath, sec.SectionInformation.AllowDefinition, sec.SectionInformation.AllowExeDefinition))
			{
				object obj = (sec.SectionInformation.AllowExeDefinition != ConfigurationAllowExeDefinition.MachineToApplication) ? sec.SectionInformation.AllowExeDefinition : sec.SectionInformation.AllowDefinition;
				throw new ConfigurationErrorsException(string.Concat(new string[]
				{
					"The section <",
					name,
					"> can't be defined in this configuration file (the allowed definition context is '",
					(obj != null) ? obj.ToString() : null,
					"')."
				}));
			}
			if (sec.SectionInformation.Type == null)
			{
				sec.SectionInformation.Type = this.system.Host.GetConfigTypeName(sec.GetType());
			}
			SectionInfo sectionInfo = new SectionInfo(name, sec.SectionInformation);
			sectionInfo.StreamName = this.streamName;
			sectionInfo.ConfigHost = this.system.Host;
			group.AddChild(sectionInfo);
			this.elementData[sectionInfo] = sec;
			sec.Configuration = this;
		}

		internal void CreateSectionGroup(SectionGroupInfo parentGroup, string name, ConfigurationSectionGroup sec)
		{
			if (parentGroup.HasChild(name))
			{
				throw new ConfigurationErrorsException("Cannot add a ConfigurationSectionGroup. A section or section group already exists with the name '" + name + "'");
			}
			if (sec.Type == null)
			{
				sec.Type = this.system.Host.GetConfigTypeName(sec.GetType());
			}
			sec.SetName(name);
			SectionGroupInfo sectionGroupInfo = new SectionGroupInfo(name, sec.Type);
			sectionGroupInfo.StreamName = this.streamName;
			sectionGroupInfo.ConfigHost = this.system.Host;
			parentGroup.AddChild(sectionGroupInfo);
			this.elementData[sectionGroupInfo] = sec;
			sec.Initialize(this, sectionGroupInfo);
		}

		internal void RemoveConfigInfo(ConfigInfo config)
		{
			this.elementData.Remove(config);
		}

		/// <summary>Writes the configuration settings contained within this <see cref="T:System.Configuration.Configuration" /> object to the current XML configuration file.</summary>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The configuration file could not be written to.  
		/// -or-
		///  The configuration file has changed.</exception>
		public void Save()
		{
			this.Save(ConfigurationSaveMode.Modified, false);
		}

		/// <summary>Writes the configuration settings contained within this <see cref="T:System.Configuration.Configuration" /> object to the current XML configuration file.</summary>
		/// <param name="saveMode">A <see cref="T:System.Configuration.ConfigurationSaveMode" /> value that determines which property values to save.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The configuration file could not be written to.  
		/// -or-
		///  The configuration file has changed.</exception>
		public void Save(ConfigurationSaveMode saveMode)
		{
			this.Save(saveMode, false);
		}

		/// <summary>Writes the configuration settings contained within this <see cref="T:System.Configuration.Configuration" /> object to the current XML configuration file.</summary>
		/// <param name="saveMode">A <see cref="T:System.Configuration.ConfigurationSaveMode" /> value that determines which property values to save.</param>
		/// <param name="forceSaveAll">
		///   <see langword="true" /> to save even if the configuration was not modified; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The configuration file could not be written to.  
		/// -or-
		///  The configuration file has changed.</exception>
		public void Save(ConfigurationSaveMode saveMode, bool forceSaveAll)
		{
			if (!forceSaveAll && saveMode != ConfigurationSaveMode.Full && !this.HasValues(saveMode))
			{
				this.ResetModified();
				return;
			}
			ConfigurationSaveEventHandler saveStart = Configuration.SaveStart;
			ConfigurationSaveEventHandler saveEnd = Configuration.SaveEnd;
			object obj = null;
			Exception ex = null;
			Stream stream = this.system.Host.OpenStreamForWrite(this.streamName, null, ref obj);
			try
			{
				if (saveStart != null)
				{
					saveStart(this, new ConfigurationSaveEventArgs(this.streamName, true, null, obj));
				}
				this.Save(stream, saveMode, forceSaveAll);
				this.system.Host.WriteCompleted(this.streamName, true, obj);
			}
			catch (Exception ex)
			{
				this.system.Host.WriteCompleted(this.streamName, false, obj);
				throw;
			}
			finally
			{
				stream.Close();
				if (saveEnd != null)
				{
					saveEnd(this, new ConfigurationSaveEventArgs(this.streamName, false, ex, obj));
				}
			}
		}

		/// <summary>Writes the configuration settings contained within this <see cref="T:System.Configuration.Configuration" /> object to the specified XML configuration file.</summary>
		/// <param name="filename">The path and file name to save the configuration file to.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The configuration file could not be written to.  
		/// -or-
		///  The configuration file has changed.</exception>
		public void SaveAs(string filename)
		{
			this.SaveAs(filename, ConfigurationSaveMode.Modified, false);
		}

		/// <summary>Writes the configuration settings contained within this <see cref="T:System.Configuration.Configuration" /> object to the specified XML configuration file.</summary>
		/// <param name="filename">The path and file name to save the configuration file to.</param>
		/// <param name="saveMode">A <see cref="T:System.Configuration.ConfigurationSaveMode" /> value that determines which property values to save.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The configuration file could not be written to.  
		/// -or-
		///  The configuration file has changed.</exception>
		public void SaveAs(string filename, ConfigurationSaveMode saveMode)
		{
			this.SaveAs(filename, saveMode, false);
		}

		/// <summary>Writes the configuration settings contained within this <see cref="T:System.Configuration.Configuration" /> object to the specified XML configuration file.</summary>
		/// <param name="filename">The path and file name to save the configuration file to.</param>
		/// <param name="saveMode">A <see cref="T:System.Configuration.ConfigurationSaveMode" /> value that determines which property values to save.</param>
		/// <param name="forceSaveAll">
		///   <see langword="true" /> to save even if the configuration was not modified; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="filename" /> is null or an empty string ("").</exception>
		[MonoInternalNote("Detect if file has changed")]
		public void SaveAs(string filename, ConfigurationSaveMode saveMode, bool forceSaveAll)
		{
			if (!forceSaveAll && saveMode != ConfigurationSaveMode.Full && !this.HasValues(saveMode))
			{
				this.ResetModified();
				return;
			}
			string directoryName = Path.GetDirectoryName(Path.GetFullPath(filename));
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			this.Save(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write), saveMode, forceSaveAll);
		}

		private void Save(Stream stream, ConfigurationSaveMode mode, bool forceUpdateAll)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(new StreamWriter(stream));
			xmlTextWriter.Formatting = Formatting.Indented;
			try
			{
				xmlTextWriter.WriteStartDocument();
				if (this.rootNamespace != null)
				{
					xmlTextWriter.WriteStartElement("configuration", this.rootNamespace);
				}
				else
				{
					xmlTextWriter.WriteStartElement("configuration");
				}
				if (this.rootGroup.HasConfigContent(this))
				{
					this.rootGroup.WriteConfig(this, xmlTextWriter, mode);
				}
				foreach (object obj in this.Locations)
				{
					ConfigurationLocation configurationLocation = (ConfigurationLocation)obj;
					if (configurationLocation.OpenedConfiguration == null)
					{
						xmlTextWriter.WriteRaw("\n");
						xmlTextWriter.WriteRaw(configurationLocation.XmlContent);
					}
					else
					{
						xmlTextWriter.WriteStartElement("location");
						xmlTextWriter.WriteAttributeString("path", configurationLocation.Path);
						if (!configurationLocation.AllowOverride)
						{
							xmlTextWriter.WriteAttributeString("allowOverride", "false");
						}
						configurationLocation.OpenedConfiguration.SaveData(xmlTextWriter, mode, forceUpdateAll);
						xmlTextWriter.WriteEndElement();
					}
				}
				this.SaveData(xmlTextWriter, mode, forceUpdateAll);
				xmlTextWriter.WriteEndElement();
				this.ResetModified();
			}
			finally
			{
				xmlTextWriter.Flush();
				xmlTextWriter.Close();
			}
		}

		private void SaveData(XmlTextWriter tw, ConfigurationSaveMode mode, bool forceUpdateAll)
		{
			this.rootGroup.WriteRootData(tw, this, mode);
		}

		private bool HasValues(ConfigurationSaveMode mode)
		{
			foreach (object obj in this.Locations)
			{
				ConfigurationLocation configurationLocation = (ConfigurationLocation)obj;
				if (configurationLocation.OpenedConfiguration != null && configurationLocation.OpenedConfiguration.HasValues(mode))
				{
					return true;
				}
			}
			return this.rootGroup.HasValues(this, mode);
		}

		private void ResetModified()
		{
			foreach (object obj in this.Locations)
			{
				ConfigurationLocation configurationLocation = (ConfigurationLocation)obj;
				if (configurationLocation.OpenedConfiguration != null)
				{
					configurationLocation.OpenedConfiguration.ResetModified();
				}
			}
			this.rootGroup.ResetModified(this);
		}

		private bool Load()
		{
			if (string.IsNullOrEmpty(this.streamName))
			{
				return true;
			}
			Stream stream = null;
			try
			{
				stream = this.system.Host.OpenStreamForRead(this.streamName);
				if (stream == null)
				{
					return false;
				}
			}
			catch
			{
				return false;
			}
			using (XmlTextReader xmlTextReader = new ConfigXmlTextReader(stream, this.streamName))
			{
				this.ReadConfigFile(xmlTextReader, this.streamName);
			}
			this.ResetModified();
			return true;
		}

		private void ReadConfigFile(XmlReader reader, string fileName)
		{
			reader.MoveToContent();
			if (reader.NodeType != XmlNodeType.Element || reader.Name != "configuration")
			{
				this.ThrowException("Configuration file does not have a valid root element", reader);
			}
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					if (reader.LocalName == "xmlns")
					{
						this.rootNamespace = reader.Value;
					}
					else
					{
						this.ThrowException(string.Format("Unrecognized attribute '{0}' in root element", reader.LocalName), reader);
					}
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return;
			}
			reader.ReadStartElement();
			reader.MoveToContent();
			if (reader.LocalName == "configSections")
			{
				if (reader.HasAttributes)
				{
					this.ThrowException("Unrecognized attribute in <configSections>.", reader);
				}
				this.rootGroup.ReadConfig(this, fileName, reader);
			}
			this.rootGroup.ReadRootData(reader, this, true);
		}

		internal void ReadData(XmlReader reader, bool allowOverride)
		{
			this.rootGroup.ReadData(this, reader, allowOverride);
		}

		private void ThrowException(string text, XmlReader reader)
		{
			IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
			throw new ConfigurationErrorsException(text, this.streamName, (xmlLineInfo != null) ? xmlLineInfo.LineNumber : 0);
		}

		internal Configuration()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Specifies a function delegate that is used to transform assembly strings in configuration files.</summary>
		/// <returns>A delegate that transforms type strings. The default value is <see langword="null" />.</returns>
		public Func<string, string> AssemblyStringTransformer
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
			[ConfigurationPermission(SecurityAction.Demand, Unrestricted = true)]
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		/// <summary>Specifies the targeted version of the .NET Framework when a version earlier than the current one is targeted.</summary>
		/// <returns>The name of the targeted version of the .NET Framework. The default value is <see langword="null" />, which indicates that the current version is targeted.</returns>
		public FrameworkName TargetFramework
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
			[ConfigurationPermission(SecurityAction.Demand, Unrestricted = true)]
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		/// <summary>Specifies a function delegate that is used to transform type strings in configuration files.</summary>
		/// <returns>A delegate that transforms type strings. The default value is <see langword="null" />.</returns>
		public Func<string, string> TypeStringTransformer
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
			[ConfigurationPermission(SecurityAction.Demand, Unrestricted = true)]
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		private Configuration parent;

		private Hashtable elementData;

		private string streamName;

		private ConfigurationSectionGroup rootSectionGroup;

		private ConfigurationLocationCollection locations;

		private SectionGroupInfo rootGroup;

		private IConfigSystem system;

		private bool hasFile;

		private string rootNamespace;

		private string configPath;

		private string locationConfigPath;

		private string locationSubPath;

		private ContextInformation evaluationContext;
	}
}
