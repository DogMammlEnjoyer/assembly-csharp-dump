using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace System.Configuration
{
	/// <summary>Acts as a base class for deriving concrete wrapper classes to implement the application settings feature in Window Forms applications.</summary>
	public abstract class ApplicationSettingsBase : SettingsBase, INotifyPropertyChanged
	{
		/// <summary>Initializes an instance of the <see cref="T:System.Configuration.ApplicationSettingsBase" /> class to its default state.</summary>
		protected ApplicationSettingsBase()
		{
			base.Initialize(this.Context, this.Properties, this.Providers);
		}

		/// <summary>Initializes an instance of the <see cref="T:System.Configuration.ApplicationSettingsBase" /> class using the supplied owner component.</summary>
		/// <param name="owner">The component that will act as the owner of the application settings object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="owner" /> is <see langword="null" />.</exception>
		protected ApplicationSettingsBase(IComponent owner) : this(owner, string.Empty)
		{
		}

		/// <summary>Initializes an instance of the <see cref="T:System.Configuration.ApplicationSettingsBase" /> class using the supplied settings key.</summary>
		/// <param name="settingsKey">A <see cref="T:System.String" /> that uniquely identifies separate instances of the wrapper class.</param>
		protected ApplicationSettingsBase(string settingsKey)
		{
			this.settingsKey = settingsKey;
			base.Initialize(this.Context, this.Properties, this.Providers);
		}

		/// <summary>Initializes an instance of the <see cref="T:System.Configuration.ApplicationSettingsBase" /> class using the supplied owner component and settings key.</summary>
		/// <param name="owner">The component that will act as the owner of the application settings object.</param>
		/// <param name="settingsKey">A <see cref="T:System.String" /> that uniquely identifies separate instances of the wrapper class.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="owner" /> is <see langword="null" />.</exception>
		protected ApplicationSettingsBase(IComponent owner, string settingsKey)
		{
			if (owner == null)
			{
				throw new ArgumentNullException();
			}
			this.providerService = (ISettingsProviderService)owner.Site.GetService(typeof(ISettingsProviderService));
			this.settingsKey = settingsKey;
			base.Initialize(this.Context, this.Properties, this.Providers);
		}

		/// <summary>Occurs after the value of an application settings property is changed.</summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>Occurs before the value of an application settings property is changed.</summary>
		public event SettingChangingEventHandler SettingChanging;

		/// <summary>Occurs after the application settings are retrieved from storage.</summary>
		public event SettingsLoadedEventHandler SettingsLoaded;

		/// <summary>Occurs before values are saved to the data store.</summary>
		public event SettingsSavingEventHandler SettingsSaving;

		/// <summary>Returns the value of the named settings property for the previous version of the same application.</summary>
		/// <param name="propertyName">A <see cref="T:System.String" /> containing the name of the settings property whose value is to be returned.</param>
		/// <returns>An <see cref="T:System.Object" /> containing the value of the specified <see cref="T:System.Configuration.SettingsProperty" /> if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Configuration.SettingsPropertyNotFoundException">The property does not exist. The property count is zero or the property cannot be found in the data store.</exception>
		public object GetPreviousVersion(string propertyName)
		{
			throw new NotImplementedException();
		}

		/// <summary>Refreshes the application settings property values from persistent storage.</summary>
		public void Reload()
		{
			if (this.PropertyValues != null)
			{
				this.PropertyValues.Clear();
			}
			foreach (object obj in this.Properties)
			{
				SettingsProperty settingsProperty = (SettingsProperty)obj;
				this.OnPropertyChanged(this, new PropertyChangedEventArgs(settingsProperty.Name));
			}
		}

		/// <summary>Restores the persisted application settings values to their corresponding default properties.</summary>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The configuration file could not be parsed.</exception>
		public void Reset()
		{
			if (this.Properties != null)
			{
				foreach (object obj in this.Providers)
				{
					IApplicationSettingsProvider applicationSettingsProvider = ((SettingsProvider)obj) as IApplicationSettingsProvider;
					if (applicationSettingsProvider != null)
					{
						applicationSettingsProvider.Reset(this.Context);
					}
				}
				this.InternalSave();
			}
			this.Reload();
		}

		/// <summary>Stores the current values of the application settings properties.</summary>
		public override void Save()
		{
			CancelEventArgs cancelEventArgs = new CancelEventArgs();
			this.OnSettingsSaving(this, cancelEventArgs);
			if (cancelEventArgs.Cancel)
			{
				return;
			}
			this.InternalSave();
		}

		private void InternalSave()
		{
			this.Context.CurrentSettings = this;
			foreach (object obj in this.Providers)
			{
				SettingsProvider settingsProvider = (SettingsProvider)obj;
				SettingsPropertyValueCollection settingsPropertyValueCollection = new SettingsPropertyValueCollection();
				foreach (object obj2 in this.PropertyValues)
				{
					SettingsPropertyValue settingsPropertyValue = (SettingsPropertyValue)obj2;
					if (settingsPropertyValue.Property.Provider == settingsProvider)
					{
						settingsPropertyValueCollection.Add(settingsPropertyValue);
					}
				}
				if (settingsPropertyValueCollection.Count > 0)
				{
					settingsProvider.SetPropertyValues(this.Context, settingsPropertyValueCollection);
				}
			}
			this.Context.CurrentSettings = null;
		}

		/// <summary>Updates application settings to reflect a more recent installation of the application.</summary>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The configuration file could not be parsed.</exception>
		public virtual void Upgrade()
		{
			if (this.Properties != null)
			{
				foreach (object obj in this.Providers)
				{
					SettingsProvider settingsProvider = (SettingsProvider)obj;
					IApplicationSettingsProvider applicationSettingsProvider = settingsProvider as IApplicationSettingsProvider;
					if (applicationSettingsProvider != null)
					{
						applicationSettingsProvider.Upgrade(this.Context, this.GetPropertiesForProvider(settingsProvider));
					}
				}
			}
			this.Reload();
		}

		private SettingsPropertyCollection GetPropertiesForProvider(SettingsProvider provider)
		{
			SettingsPropertyCollection settingsPropertyCollection = new SettingsPropertyCollection();
			foreach (object obj in this.Properties)
			{
				SettingsProperty settingsProperty = (SettingsProperty)obj;
				if (settingsProperty.Provider == provider)
				{
					settingsPropertyCollection.Add(settingsProperty);
				}
			}
			return settingsPropertyCollection;
		}

		/// <summary>Raises the <see cref="E:System.Configuration.ApplicationSettingsBase.PropertyChanged" /> event.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">A <see cref="T:System.ComponentModel.PropertyChangedEventArgs" /> that contains the event data.</param>
		protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(sender, e);
			}
		}

		/// <summary>Raises the <see cref="E:System.Configuration.ApplicationSettingsBase.SettingChanging" /> event.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">A <see cref="T:System.Configuration.SettingChangingEventArgs" /> that contains the event data.</param>
		protected virtual void OnSettingChanging(object sender, SettingChangingEventArgs e)
		{
			if (this.SettingChanging != null)
			{
				this.SettingChanging(sender, e);
			}
		}

		/// <summary>Raises the <see cref="E:System.Configuration.ApplicationSettingsBase.SettingsLoaded" /> event.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">A <see cref="T:System.Configuration.SettingsLoadedEventArgs" /> that contains the event data.</param>
		protected virtual void OnSettingsLoaded(object sender, SettingsLoadedEventArgs e)
		{
			if (this.SettingsLoaded != null)
			{
				this.SettingsLoaded(sender, e);
			}
		}

		/// <summary>Raises the <see cref="E:System.Configuration.ApplicationSettingsBase.SettingsSaving" /> event.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>
		protected virtual void OnSettingsSaving(object sender, CancelEventArgs e)
		{
			if (this.SettingsSaving != null)
			{
				this.SettingsSaving(sender, e);
			}
		}

		/// <summary>Gets the application settings context associated with the settings group.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingsContext" /> associated with the settings group.</returns>
		[Browsable(false)]
		public override SettingsContext Context
		{
			get
			{
				if (base.IsSynchronized)
				{
					Monitor.Enter(this);
				}
				SettingsContext result;
				try
				{
					if (this.context == null)
					{
						this.context = new SettingsContext();
						this.context["SettingsKey"] = "";
						Type type = base.GetType();
						this.context["GroupName"] = type.FullName;
						this.context["SettingsClassType"] = type;
					}
					result = this.context;
				}
				finally
				{
					if (base.IsSynchronized)
					{
						Monitor.Exit(this);
					}
				}
				return result;
			}
		}

		private void CacheValuesByProvider(SettingsProvider provider)
		{
			SettingsPropertyCollection settingsPropertyCollection = new SettingsPropertyCollection();
			foreach (object obj in this.Properties)
			{
				SettingsProperty settingsProperty = (SettingsProperty)obj;
				if (settingsProperty.Provider == provider)
				{
					settingsPropertyCollection.Add(settingsProperty);
				}
			}
			if (settingsPropertyCollection.Count > 0)
			{
				foreach (object obj2 in provider.GetPropertyValues(this.Context, settingsPropertyCollection))
				{
					SettingsPropertyValue settingsPropertyValue = (SettingsPropertyValue)obj2;
					if (this.PropertyValues[settingsPropertyValue.Name] != null)
					{
						this.PropertyValues[settingsPropertyValue.Name].PropertyValue = settingsPropertyValue.PropertyValue;
					}
					else
					{
						this.PropertyValues.Add(settingsPropertyValue);
					}
				}
			}
			this.OnSettingsLoaded(this, new SettingsLoadedEventArgs(provider));
		}

		private void InitializeSettings(SettingsPropertyCollection settings)
		{
		}

		private object GetPropertyValue(string propertyName)
		{
			SettingsProperty settingsProperty = this.Properties[propertyName];
			if (settingsProperty == null)
			{
				throw new SettingsPropertyNotFoundException(propertyName);
			}
			if (this.propertyValues == null)
			{
				this.InitializeSettings(this.Properties);
			}
			if (this.PropertyValues[propertyName] == null)
			{
				this.CacheValuesByProvider(settingsProperty.Provider);
			}
			return this.PropertyValues[propertyName].PropertyValue;
		}

		/// <summary>Gets or sets the value of the specified application settings property.</summary>
		/// <param name="propertyName">A <see cref="T:System.String" /> containing the name of the property to access.</param>
		/// <returns>If found, the value of the named settings property; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Configuration.SettingsPropertyNotFoundException">There are no properties associated with the current wrapper or the specified property could not be found.</exception>
		/// <exception cref="T:System.Configuration.SettingsPropertyIsReadOnlyException">An attempt was made to set a read-only property.</exception>
		/// <exception cref="T:System.Configuration.SettingsPropertyWrongTypeException">The value supplied is of a type incompatible with the settings property, during a set operation.</exception>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The configuration file could not be parsed.</exception>
		[MonoTODO]
		public override object this[string propertyName]
		{
			get
			{
				if (base.IsSynchronized)
				{
					lock (this)
					{
						return this.GetPropertyValue(propertyName);
					}
				}
				return this.GetPropertyValue(propertyName);
			}
			set
			{
				SettingsProperty settingsProperty = this.Properties[propertyName];
				if (settingsProperty == null)
				{
					throw new SettingsPropertyNotFoundException(propertyName);
				}
				if (settingsProperty.IsReadOnly)
				{
					throw new SettingsPropertyIsReadOnlyException(propertyName);
				}
				if (value != null && !settingsProperty.PropertyType.IsAssignableFrom(value.GetType()))
				{
					throw new SettingsPropertyWrongTypeException(propertyName);
				}
				if (this.PropertyValues[propertyName] == null)
				{
					this.CacheValuesByProvider(settingsProperty.Provider);
				}
				SettingChangingEventArgs settingChangingEventArgs = new SettingChangingEventArgs(propertyName, base.GetType().FullName, this.settingsKey, value, false);
				this.OnSettingChanging(this, settingChangingEventArgs);
				if (!settingChangingEventArgs.Cancel)
				{
					this.PropertyValues[propertyName].PropertyValue = value;
					this.OnPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}
		}

		/// <summary>Gets the collection of settings properties in the wrapper.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingsPropertyCollection" /> containing all the <see cref="T:System.Configuration.SettingsProperty" /> objects used in the current wrapper.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The associated settings provider could not be found or its instantiation failed.</exception>
		[Browsable(false)]
		public override SettingsPropertyCollection Properties
		{
			get
			{
				if (base.IsSynchronized)
				{
					Monitor.Enter(this);
				}
				SettingsPropertyCollection result;
				try
				{
					if (this.properties == null)
					{
						SettingsProvider settingsProvider = null;
						this.properties = new SettingsPropertyCollection();
						Type type = base.GetType();
						SettingsProviderAttribute[] array = (SettingsProviderAttribute[])type.GetCustomAttributes(typeof(SettingsProviderAttribute), false);
						if (array != null && array.Length != 0)
						{
							SettingsProvider settingsProvider2 = (SettingsProvider)Activator.CreateInstance(Type.GetType(array[0].ProviderTypeName));
							settingsProvider2.Initialize(null, null);
							if (settingsProvider2 != null && this.Providers[settingsProvider2.Name] == null)
							{
								this.Providers.Add(settingsProvider2);
								settingsProvider = settingsProvider2;
							}
						}
						foreach (PropertyInfo propertyInfo in type.GetProperties())
						{
							SettingAttribute[] array3 = (SettingAttribute[])propertyInfo.GetCustomAttributes(typeof(SettingAttribute), false);
							if (array3 != null && array3.Length != 0)
							{
								this.CreateSettingsProperty(propertyInfo, this.properties, ref settingsProvider);
							}
						}
					}
					result = this.properties;
				}
				finally
				{
					if (base.IsSynchronized)
					{
						Monitor.Exit(this);
					}
				}
				return result;
			}
		}

		private void CreateSettingsProperty(PropertyInfo prop, SettingsPropertyCollection properties, ref SettingsProvider local_provider)
		{
			SettingsAttributeDictionary settingsAttributeDictionary = new SettingsAttributeDictionary();
			SettingsProvider settingsProvider = null;
			object defaultValue = null;
			SettingsSerializeAs serializeAs = SettingsSerializeAs.String;
			bool flag = false;
			foreach (Attribute attribute in prop.GetCustomAttributes(false))
			{
				if (attribute is SettingsProviderAttribute)
				{
					string providerTypeName = ((SettingsProviderAttribute)attribute).ProviderTypeName;
					Type type = Type.GetType(providerTypeName);
					if (type == null)
					{
						string[] array = providerTypeName.Split('.', StringSplitOptions.None);
						if (array.Length > 1)
						{
							Assembly assembly = Assembly.Load(array[0]);
							if (assembly != null)
							{
								type = assembly.GetType(providerTypeName);
							}
						}
					}
					settingsProvider = (SettingsProvider)Activator.CreateInstance(type);
					settingsProvider.Initialize(null, null);
				}
				else if (attribute is DefaultSettingValueAttribute)
				{
					defaultValue = ((DefaultSettingValueAttribute)attribute).Value;
				}
				else if (attribute is SettingsSerializeAsAttribute)
				{
					serializeAs = ((SettingsSerializeAsAttribute)attribute).SerializeAs;
					flag = true;
				}
				else if (attribute is ApplicationScopedSettingAttribute || attribute is UserScopedSettingAttribute)
				{
					settingsAttributeDictionary.Add(attribute.GetType(), attribute);
				}
				else
				{
					settingsAttributeDictionary.Add(attribute.GetType(), attribute);
				}
			}
			if (!flag)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(prop.PropertyType);
				if (converter != null && (!converter.CanConvertFrom(typeof(string)) || !converter.CanConvertTo(typeof(string))))
				{
					serializeAs = SettingsSerializeAs.Xml;
				}
			}
			SettingsProperty settingsProperty = new SettingsProperty(prop.Name, prop.PropertyType, settingsProvider, false, defaultValue, serializeAs, settingsAttributeDictionary, false, false);
			if (this.providerService != null)
			{
				settingsProperty.Provider = this.providerService.GetSettingsProvider(settingsProperty);
			}
			if (settingsProvider == null)
			{
				if (local_provider == null)
				{
					local_provider = new LocalFileSettingsProvider();
					local_provider.Initialize(null, null);
				}
				settingsProperty.Provider = local_provider;
				settingsProvider = local_provider;
			}
			if (settingsProvider != null)
			{
				SettingsProvider settingsProvider2 = this.Providers[settingsProvider.Name];
				if (settingsProvider2 != null)
				{
					settingsProperty.Provider = settingsProvider2;
				}
			}
			properties.Add(settingsProperty);
			if (settingsProperty.Provider != null && this.Providers[settingsProperty.Provider.Name] == null)
			{
				this.Providers.Add(settingsProperty.Provider);
			}
		}

		/// <summary>Gets a collection of property values.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingsPropertyValueCollection" /> of property values.</returns>
		[Browsable(false)]
		public override SettingsPropertyValueCollection PropertyValues
		{
			get
			{
				if (base.IsSynchronized)
				{
					Monitor.Enter(this);
				}
				SettingsPropertyValueCollection result;
				try
				{
					if (this.propertyValues == null)
					{
						this.propertyValues = new SettingsPropertyValueCollection();
					}
					result = this.propertyValues;
				}
				finally
				{
					if (base.IsSynchronized)
					{
						Monitor.Exit(this);
					}
				}
				return result;
			}
		}

		/// <summary>Gets the collection of application settings providers used by the wrapper.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingsProviderCollection" /> containing all the <see cref="T:System.Configuration.SettingsProvider" /> objects used by the settings properties of the current settings wrapper.</returns>
		[Browsable(false)]
		public override SettingsProviderCollection Providers
		{
			get
			{
				if (base.IsSynchronized)
				{
					Monitor.Enter(this);
				}
				SettingsProviderCollection result;
				try
				{
					if (this.providers == null)
					{
						this.providers = new SettingsProviderCollection();
					}
					result = this.providers;
				}
				finally
				{
					if (base.IsSynchronized)
					{
						Monitor.Exit(this);
					}
				}
				return result;
			}
		}

		/// <summary>Gets or sets the settings key for the application settings group.</summary>
		/// <returns>A <see cref="T:System.String" /> containing the settings key for the current settings group.</returns>
		[Browsable(false)]
		public string SettingsKey
		{
			get
			{
				return this.settingsKey;
			}
			set
			{
				this.settingsKey = value;
			}
		}

		private string settingsKey;

		private SettingsContext context;

		private SettingsPropertyCollection properties;

		private ISettingsProviderService providerService;

		private SettingsPropertyValueCollection propertyValues;

		private SettingsProviderCollection providers;
	}
}
