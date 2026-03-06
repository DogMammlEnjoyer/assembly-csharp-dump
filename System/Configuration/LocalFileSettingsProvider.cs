using System;
using System.Collections.Specialized;
using System.Security.Permissions;

namespace System.Configuration
{
	/// <summary>Provides persistence for application settings classes.</summary>
	public class LocalFileSettingsProvider : SettingsProvider, IApplicationSettingsProvider
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.LocalFileSettingsProvider" /> class.</summary>
		public LocalFileSettingsProvider()
		{
			this.impl = new CustomizableFileSettingsProvider();
		}

		/// <summary>Returns the value of the named settings property for the previous version of the same application.</summary>
		/// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> that describes where the application settings property is used.</param>
		/// <param name="property">The <see cref="T:System.Configuration.SettingsProperty" /> whose value is to be returned.</param>
		/// <returns>A <see cref="T:System.Configuration.SettingsPropertyValue" /> representing the application setting if found; otherwise, <see langword="null" />.</returns>
		[MonoTODO]
		[FileIOPermission(SecurityAction.Assert, AllFiles = (FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery))]
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
		public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
		{
			return this.impl.GetPreviousVersion(context, property);
		}

		/// <summary>Returns the collection of setting property values for the specified application instance and settings property group.</summary>
		/// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application usage.</param>
		/// <param name="properties">A <see cref="T:System.Configuration.SettingsPropertyCollection" /> containing the settings property group whose values are to be retrieved.</param>
		/// <returns>A <see cref="T:System.Configuration.SettingsPropertyValueCollection" /> containing the values for the specified settings property group.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">A user-scoped setting was encountered but the current configuration only supports application-scoped settings.</exception>
		[MonoTODO]
		public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
		{
			return this.impl.GetPropertyValues(context, properties);
		}

		/// <summary>Initializes the provider.</summary>
		/// <param name="name">The friendly name of the provider.</param>
		/// <param name="values">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
		public override void Initialize(string name, NameValueCollection values)
		{
			if (name == null)
			{
				name = "LocalFileSettingsProvider";
			}
			if (values != null)
			{
				this.impl.ApplicationName = values["applicationName"];
			}
			base.Initialize(name, values);
		}

		/// <summary>Resets all application settings properties associated with the specified application to their default values.</summary>
		/// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application usage.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">A user-scoped setting was encountered but the current configuration only supports application-scoped settings.</exception>
		[MonoTODO]
		public void Reset(SettingsContext context)
		{
			this.impl.Reset(context);
		}

		/// <summary>Sets the values of the specified group of property settings.</summary>
		/// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application usage.</param>
		/// <param name="values">A <see cref="T:System.Configuration.SettingsPropertyValueCollection" /> representing the group of property settings to set.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">A user-scoped setting was encountered but the current configuration only supports application-scoped settings.  
		///  -or-  
		///  There was a general failure saving the settings to the configuration file.</exception>
		[MonoTODO]
		public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
		{
			this.impl.SetPropertyValues(context, values);
		}

		/// <summary>Attempts to migrate previous user-scoped settings from a previous version of the same application.</summary>
		/// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application usage.</param>
		/// <param name="properties">A <see cref="T:System.Configuration.SettingsPropertyCollection" /> containing the settings property group whose values are to be retrieved.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">A user-scoped setting was encountered but the current configuration only supports application-scoped settings.  
		///  -or-  
		///  The previous version of the configuration file could not be accessed.</exception>
		[MonoTODO]
		public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
		{
			this.impl.Upgrade(context, properties);
		}

		/// <summary>Gets or sets the name of the currently running application.</summary>
		/// <returns>A string that contains the application's display name.</returns>
		public override string ApplicationName
		{
			get
			{
				return this.impl.ApplicationName;
			}
			set
			{
				this.impl.ApplicationName = value;
			}
		}

		private CustomizableFileSettingsProvider impl;
	}
}
