using System;
using System.Configuration;
using System.Security;
using System.Security.Permissions;

namespace System.Runtime.Serialization.Configuration
{
	/// <summary>Handles the XML elements used to configure serialization by the <see cref="T:System.Runtime.Serialization.DataContractSerializer" />.</summary>
	public sealed class DataContractSerializerSection : ConfigurationSection
	{
		[SecurityCritical]
		[ConfigurationPermission(SecurityAction.Assert, Unrestricted = true)]
		internal static DataContractSerializerSection UnsafeGetSection()
		{
			DataContractSerializerSection dataContractSerializerSection = (DataContractSerializerSection)ConfigurationManager.GetSection(ConfigurationStrings.DataContractSerializerSectionPath);
			if (dataContractSerializerSection == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(SR.GetString("Failed to load configuration section for dataContractSerializer.")));
			}
			return dataContractSerializerSection;
		}

		/// <summary>Gets a collection of types added to the <see cref="P:System.Runtime.Serialization.DataContractSerializer.KnownTypes" /> property.</summary>
		/// <returns>A <see cref="T:System.Runtime.Serialization.Configuration.DeclaredTypeElementCollection" /> that contains the known types.</returns>
		[ConfigurationProperty("declaredTypes", DefaultValue = null)]
		public DeclaredTypeElementCollection DeclaredTypes
		{
			get
			{
				return (DeclaredTypeElementCollection)base["declaredTypes"];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new ConfigurationPropertyCollection
					{
						new ConfigurationProperty("declaredTypes", typeof(DeclaredTypeElementCollection), null, null, null, ConfigurationPropertyOptions.None)
					};
				}
				return this.properties;
			}
		}

		private ConfigurationPropertyCollection properties;
	}
}
