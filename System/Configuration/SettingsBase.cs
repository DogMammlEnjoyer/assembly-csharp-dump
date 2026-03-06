using System;
using System.ComponentModel;

namespace System.Configuration
{
	/// <summary>Provides the base class used to support user property settings.</summary>
	public abstract class SettingsBase
	{
		/// <summary>Initializes internal properties used by <see cref="T:System.Configuration.SettingsBase" /> object.</summary>
		/// <param name="context">The settings context related to the settings properties.</param>
		/// <param name="properties">The settings properties that will be accessible from the <see cref="T:System.Configuration.SettingsBase" /> instance.</param>
		/// <param name="providers">The initialized providers that should be used when loading and saving property values.</param>
		public void Initialize(SettingsContext context, SettingsPropertyCollection properties, SettingsProviderCollection providers)
		{
			this.context = context;
			this.properties = properties;
			this.providers = providers;
		}

		/// <summary>Stores the current values of the settings properties.</summary>
		public virtual void Save()
		{
			if (this.sync)
			{
				lock (this)
				{
					this.SaveCore();
					return;
				}
			}
			this.SaveCore();
		}

		private void SaveCore()
		{
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
		}

		/// <summary>Provides a <see cref="T:System.Configuration.SettingsBase" /> class that is synchronized (thread safe).</summary>
		/// <param name="settingsBase">The class used to support user property settings.</param>
		/// <returns>A <see cref="T:System.Configuration.SettingsBase" /> class that is synchronized.</returns>
		public static SettingsBase Synchronized(SettingsBase settingsBase)
		{
			settingsBase.sync = true;
			return settingsBase;
		}

		/// <summary>Gets the associated settings context.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingsContext" /> associated with the settings instance.</returns>
		public virtual SettingsContext Context
		{
			get
			{
				return this.context;
			}
		}

		/// <summary>Gets a value indicating whether access to the object is synchronized (thread safe).</summary>
		/// <returns>
		///   <see langword="true" /> if access to the <see cref="T:System.Configuration.SettingsBase" /> is synchronized; otherwise, <see langword="false" />.</returns>
		[Browsable(false)]
		public bool IsSynchronized
		{
			get
			{
				return this.sync;
			}
		}

		/// <summary>Gets or sets the value of the specified settings property.</summary>
		/// <param name="propertyName">A <see cref="T:System.String" /> containing the name of the property to access.</param>
		/// <returns>If found, the value of the named settings property.</returns>
		/// <exception cref="T:System.Configuration.SettingsPropertyNotFoundException">There are no properties associated with the current object, or the specified property could not be found.</exception>
		/// <exception cref="T:System.Configuration.SettingsPropertyIsReadOnlyException">An attempt was made to set a read-only property.</exception>
		/// <exception cref="T:System.Configuration.SettingsPropertyWrongTypeException">The value supplied is of a type incompatible with the settings property, during a set operation.</exception>
		public virtual object this[string propertyName]
		{
			get
			{
				if (this.sync)
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
				if (this.sync)
				{
					lock (this)
					{
						this.SetPropertyValue(propertyName, value);
						return;
					}
				}
				this.SetPropertyValue(propertyName, value);
			}
		}

		/// <summary>Gets the collection of settings properties.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingsPropertyCollection" /> collection containing all the <see cref="T:System.Configuration.SettingsProperty" /> objects.</returns>
		public virtual SettingsPropertyCollection Properties
		{
			get
			{
				return this.properties;
			}
		}

		/// <summary>Gets a collection of settings property values.</summary>
		/// <returns>A collection of <see cref="T:System.Configuration.SettingsPropertyValue" /> objects representing the actual data values for the properties managed by the <see cref="T:System.Configuration.SettingsBase" /> instance.</returns>
		public virtual SettingsPropertyValueCollection PropertyValues
		{
			get
			{
				if (this.sync)
				{
					lock (this)
					{
						return this.values;
					}
				}
				return this.values;
			}
		}

		/// <summary>Gets a collection of settings providers.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingsProviderCollection" /> containing <see cref="T:System.Configuration.SettingsProvider" /> objects.</returns>
		public virtual SettingsProviderCollection Providers
		{
			get
			{
				return this.providers;
			}
		}

		private object GetPropertyValue(string propertyName)
		{
			SettingsProperty settingsProperty;
			if (this.Properties == null || (settingsProperty = this.Properties[propertyName]) == null)
			{
				throw new SettingsPropertyNotFoundException(string.Format("The settings property '{0}' was not found", propertyName));
			}
			if (this.values[propertyName] == null)
			{
				foreach (object obj in settingsProperty.Provider.GetPropertyValues(this.Context, this.Properties))
				{
					SettingsPropertyValue property = (SettingsPropertyValue)obj;
					this.values.Add(property);
				}
			}
			return this.PropertyValues[propertyName].PropertyValue;
		}

		private void SetPropertyValue(string propertyName, object value)
		{
			SettingsProperty settingsProperty;
			if (this.Properties == null || (settingsProperty = this.Properties[propertyName]) == null)
			{
				throw new SettingsPropertyNotFoundException(string.Format("The settings property '{0}' was not found", propertyName));
			}
			if (settingsProperty.IsReadOnly)
			{
				throw new SettingsPropertyIsReadOnlyException(string.Format("The settings property '{0}' is read only", propertyName));
			}
			if (settingsProperty.PropertyType != value.GetType())
			{
				throw new SettingsPropertyWrongTypeException(string.Format("The value supplied is of a type incompatible with the settings property '{0}'", propertyName));
			}
			this.PropertyValues[propertyName].PropertyValue = value;
		}

		private bool sync;

		private SettingsContext context;

		private SettingsPropertyCollection properties;

		private SettingsProviderCollection providers;

		private SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();
	}
}
