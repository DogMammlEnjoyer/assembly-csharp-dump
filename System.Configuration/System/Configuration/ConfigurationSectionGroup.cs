using System;
using System.Runtime.Versioning;
using Unity;

namespace System.Configuration
{
	/// <summary>Represents a group of related sections within a configuration file.</summary>
	public class ConfigurationSectionGroup
	{
		private Configuration Config
		{
			get
			{
				if (this.config == null)
				{
					throw new InvalidOperationException("ConfigurationSectionGroup cannot be edited until it is added to a Configuration instance as its descendant");
				}
				return this.config;
			}
		}

		internal void Initialize(Configuration config, SectionGroupInfo group)
		{
			if (this.initialized)
			{
				string str = "INTERNAL ERROR: this configuration section is being initialized twice: ";
				Type type = base.GetType();
				throw new SystemException(str + ((type != null) ? type.ToString() : null));
			}
			this.initialized = true;
			this.config = config;
			this.group = group;
		}

		internal void SetName(string name)
		{
			this.name = name;
		}

		/// <summary>Forces the declaration for this <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object.</summary>
		/// <param name="force">
		///   <see langword="true" /> if the <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object must be written to the file; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object is the root section group.  
		/// -or-
		///  The <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object has a location.</exception>
		[MonoTODO]
		public void ForceDeclaration(bool force)
		{
			this.require_declaration = force;
		}

		/// <summary>Forces the declaration for this <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object.</summary>
		public void ForceDeclaration()
		{
			this.ForceDeclaration(true);
		}

		/// <summary>Gets a value that indicates whether this <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object is declared.</summary>
		/// <returns>
		///   <see langword="true" /> if this <see cref="T:System.Configuration.ConfigurationSectionGroup" /> is declared; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		[MonoTODO]
		public bool IsDeclared
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value that indicates whether this <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object declaration is required.</summary>
		/// <returns>
		///   <see langword="true" /> if this <see cref="T:System.Configuration.ConfigurationSectionGroup" /> declaration is required; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public bool IsDeclarationRequired
		{
			get
			{
				return this.require_declaration;
			}
		}

		/// <summary>Gets the name property of this <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object.</summary>
		/// <returns>The name property of this <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object.</returns>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>Gets the section group name associated with this <see cref="T:System.Configuration.ConfigurationSectionGroup" />.</summary>
		/// <returns>The section group name of this <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object.</returns>
		[MonoInternalNote("Check if this is correct")]
		public string SectionGroupName
		{
			get
			{
				return this.group.XPath;
			}
		}

		/// <summary>Gets a <see cref="T:System.Configuration.ConfigurationSectionGroupCollection" /> object that contains all the <see cref="T:System.Configuration.ConfigurationSectionGroup" /> objects that are children of this <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object.</summary>
		/// <returns>A <see cref="T:System.Configuration.ConfigurationSectionGroupCollection" /> object that contains all the <see cref="T:System.Configuration.ConfigurationSectionGroup" /> objects that are children of this <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object.</returns>
		public ConfigurationSectionGroupCollection SectionGroups
		{
			get
			{
				if (this.groups == null)
				{
					this.groups = new ConfigurationSectionGroupCollection(this.Config, this.group);
				}
				return this.groups;
			}
		}

		/// <summary>Gets a <see cref="T:System.Configuration.ConfigurationSectionCollection" /> object that contains all of <see cref="T:System.Configuration.ConfigurationSection" /> objects within this <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object.</summary>
		/// <returns>A <see cref="T:System.Configuration.ConfigurationSectionCollection" /> object that contains all the <see cref="T:System.Configuration.ConfigurationSection" /> objects within this <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object.</returns>
		public ConfigurationSectionCollection Sections
		{
			get
			{
				if (this.sections == null)
				{
					this.sections = new ConfigurationSectionCollection(this.Config, this.group);
				}
				return this.sections;
			}
		}

		/// <summary>Gets or sets the type for this <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object.</summary>
		/// <returns>The type of this <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object is the root section group.  
		/// -or-
		///  The <see cref="T:System.Configuration.ConfigurationSectionGroup" /> object has a location.</exception>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The section or group is already defined at another level.</exception>
		public string Type
		{
			get
			{
				return this.type_name;
			}
			set
			{
				this.type_name = value;
			}
		}

		/// <summary>Indicates whether the current <see cref="T:System.Configuration.ConfigurationSectionGroup" /> instance should be serialized when the configuration object hierarchy is serialized for the specified target version of the .NET Framework.</summary>
		/// <param name="targetFramework">The target version of the .NET Framework.</param>
		/// <returns>
		///   <see langword="true" /> if the current section group should be serialized; otherwise, <see langword="false" />.</returns>
		protected internal virtual bool ShouldSerializeSectionGroupInTargetVersion(FrameworkName targetFramework)
		{
			ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		private bool require_declaration;

		private string name;

		private string type_name;

		private ConfigurationSectionCollection sections;

		private ConfigurationSectionGroupCollection groups;

		private Configuration config;

		private SectionGroupInfo group;

		private bool initialized;
	}
}
