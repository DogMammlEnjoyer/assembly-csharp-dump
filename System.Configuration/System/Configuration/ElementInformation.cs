using System;
using System.Collections;
using Unity;

namespace System.Configuration
{
	/// <summary>Contains meta-information about an individual element within the configuration. This class cannot be inherited.</summary>
	public sealed class ElementInformation
	{
		internal ElementInformation(ConfigurationElement owner, PropertyInformation propertyInfo)
		{
			this.propertyInfo = propertyInfo;
			this.owner = owner;
			this.properties = new PropertyInformationCollection();
			foreach (object obj in owner.Properties)
			{
				ConfigurationProperty property = (ConfigurationProperty)obj;
				this.properties.Add(new PropertyInformation(owner, property));
			}
		}

		/// <summary>Gets the errors for the associated element and subelements</summary>
		/// <returns>The collection containing the errors for the associated element and subelements</returns>
		[MonoTODO]
		public ICollection Errors
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets a value indicating whether the associated <see cref="T:System.Configuration.ConfigurationElement" /> object is a <see cref="T:System.Configuration.ConfigurationElementCollection" /> collection.</summary>
		/// <returns>
		///   <see langword="true" /> if the associated <see cref="T:System.Configuration.ConfigurationElement" /> object is a <see cref="T:System.Configuration.ConfigurationElementCollection" /> collection; otherwise, <see langword="false" />.</returns>
		public bool IsCollection
		{
			get
			{
				return this.owner is ConfigurationElementCollection;
			}
		}

		/// <summary>Gets a value that indicates whether the associated <see cref="T:System.Configuration.ConfigurationElement" /> object cannot be modified.</summary>
		/// <returns>
		///   <see langword="true" /> if the associated <see cref="T:System.Configuration.ConfigurationElement" /> object cannot be modified; otherwise, <see langword="false" />.</returns>
		public bool IsLocked
		{
			get
			{
				return this.propertyInfo != null && this.propertyInfo.IsLocked;
			}
		}

		/// <summary>Gets a value indicating whether the associated <see cref="T:System.Configuration.ConfigurationElement" /> object is in the configuration file.</summary>
		/// <returns>
		///   <see langword="true" /> if the associated <see cref="T:System.Configuration.ConfigurationElement" /> object is in the configuration file; otherwise, <see langword="false" />.</returns>
		[MonoTODO("Support multiple levels of inheritance")]
		public bool IsPresent
		{
			get
			{
				return this.owner.IsElementPresent;
			}
		}

		/// <summary>Gets the line number in the configuration file where the associated <see cref="T:System.Configuration.ConfigurationElement" /> object is defined.</summary>
		/// <returns>The line number in the configuration file where the associated <see cref="T:System.Configuration.ConfigurationElement" /> object is defined.</returns>
		public int LineNumber
		{
			get
			{
				if (this.propertyInfo == null)
				{
					return 0;
				}
				return this.propertyInfo.LineNumber;
			}
		}

		/// <summary>Gets the source file where the associated <see cref="T:System.Configuration.ConfigurationElement" /> object originated.</summary>
		/// <returns>The source file where the associated <see cref="T:System.Configuration.ConfigurationElement" /> object originated.</returns>
		public string Source
		{
			get
			{
				if (this.propertyInfo == null)
				{
					return null;
				}
				return this.propertyInfo.Source;
			}
		}

		/// <summary>Gets the type of the associated <see cref="T:System.Configuration.ConfigurationElement" /> object.</summary>
		/// <returns>The type of the associated <see cref="T:System.Configuration.ConfigurationElement" /> object.</returns>
		public Type Type
		{
			get
			{
				if (this.propertyInfo == null)
				{
					return this.owner.GetType();
				}
				return this.propertyInfo.Type;
			}
		}

		/// <summary>Gets the object used to validate the associated <see cref="T:System.Configuration.ConfigurationElement" /> object.</summary>
		/// <returns>The object used to validate the associated <see cref="T:System.Configuration.ConfigurationElement" /> object.</returns>
		public ConfigurationValidatorBase Validator
		{
			get
			{
				if (this.propertyInfo == null)
				{
					return new DefaultValidator();
				}
				return this.propertyInfo.Validator;
			}
		}

		/// <summary>Gets a <see cref="T:System.Configuration.PropertyInformationCollection" /> collection of the properties in the associated <see cref="T:System.Configuration.ConfigurationElement" /> object.</summary>
		/// <returns>A <see cref="T:System.Configuration.PropertyInformationCollection" /> collection of the properties in the associated <see cref="T:System.Configuration.ConfigurationElement" /> object.</returns>
		public PropertyInformationCollection Properties
		{
			get
			{
				return this.properties;
			}
		}

		internal void Reset(ElementInformation parentInfo)
		{
			foreach (object obj in this.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj;
				PropertyInformation parentProperty = parentInfo.Properties[propertyInformation.Name];
				propertyInformation.Reset(parentProperty);
			}
		}

		internal ElementInformation()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly PropertyInformation propertyInfo;

		private readonly ConfigurationElement owner;

		private readonly PropertyInformationCollection properties;
	}
}
