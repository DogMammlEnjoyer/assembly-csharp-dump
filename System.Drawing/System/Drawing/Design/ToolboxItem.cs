using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Unity;

namespace System.Drawing.Design
{
	/// <summary>Provides a base implementation of a toolbox item.</summary>
	[MonoTODO("Implementation is incomplete.")]
	[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
	[PermissionSet(SecurityAction.InheritanceDemand, Unrestricted = true)]
	[Serializable]
	public class ToolboxItem : ISerializable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Design.ToolboxItem" /> class.</summary>
		public ToolboxItem()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Design.ToolboxItem" /> class that creates the specified type of component.</summary>
		/// <param name="toolType">The type of <see cref="T:System.ComponentModel.IComponent" /> that the toolbox item creates.</param>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Drawing.Design.ToolboxItem" /> was locked.</exception>
		public ToolboxItem(Type toolType)
		{
			this.Initialize(toolType);
		}

		/// <summary>Gets or sets the name of the assembly that contains the type or types that the toolbox item creates.</summary>
		/// <returns>An <see cref="T:System.Reflection.AssemblyName" /> that indicates the assembly containing the type or types to create.</returns>
		public AssemblyName AssemblyName
		{
			get
			{
				return (AssemblyName)this.properties["AssemblyName"];
			}
			set
			{
				this.SetValue("AssemblyName", value);
			}
		}

		/// <summary>Gets or sets a bitmap to represent the toolbox item in the toolbox.</summary>
		/// <returns>A <see cref="T:System.Drawing.Bitmap" /> that represents the toolbox item in the toolbox.</returns>
		public Bitmap Bitmap
		{
			get
			{
				return (Bitmap)this.properties["Bitmap"];
			}
			set
			{
				this.SetValue("Bitmap", value);
			}
		}

		/// <summary>Gets or sets the display name for the toolbox item.</summary>
		/// <returns>The display name for the toolbox item.</returns>
		public string DisplayName
		{
			get
			{
				return this.GetValue("DisplayName");
			}
			set
			{
				this.SetValue("DisplayName", value);
			}
		}

		/// <summary>Gets or sets the filter that determines whether the toolbox item can be used on a destination component.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> of <see cref="T:System.ComponentModel.ToolboxItemFilterAttribute" /> objects.</returns>
		public ICollection Filter
		{
			get
			{
				ICollection collection = (ICollection)this.properties["Filter"];
				if (collection == null)
				{
					collection = new ToolboxItemFilterAttribute[0];
				}
				return collection;
			}
			set
			{
				this.SetValue("Filter", value);
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Drawing.Design.ToolboxItem" /> is currently locked.</summary>
		/// <returns>
		///   <see langword="true" /> if the toolbox item is locked; otherwise, <see langword="false" />.</returns>
		public virtual bool Locked
		{
			get
			{
				return this.locked;
			}
		}

		/// <summary>Gets or sets the fully qualified name of the type of <see cref="T:System.ComponentModel.IComponent" /> that the toolbox item creates when invoked.</summary>
		/// <returns>The fully qualified type name of the type of component that this toolbox item creates.</returns>
		public string TypeName
		{
			get
			{
				return this.GetValue("TypeName");
			}
			set
			{
				this.SetValue("TypeName", value);
			}
		}

		/// <summary>Gets or sets the company name for this <see cref="T:System.Drawing.Design.ToolboxItem" />.</summary>
		/// <returns>A <see cref="T:System.String" /> that specifies the company for this <see cref="T:System.Drawing.Design.ToolboxItem" />.</returns>
		public string Company
		{
			get
			{
				return (string)this.properties["Company"];
			}
			set
			{
				this.SetValue("Company", value);
			}
		}

		/// <summary>Gets the component type for this <see cref="T:System.Drawing.Design.ToolboxItem" />.</summary>
		/// <returns>A <see cref="T:System.String" /> that specifies the component type for this <see cref="T:System.Drawing.Design.ToolboxItem" />.</returns>
		public virtual string ComponentType
		{
			get
			{
				return ".NET Component";
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Reflection.AssemblyName" /> for the toolbox item.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.AssemblyName" /> objects.</returns>
		public AssemblyName[] DependentAssemblies
		{
			get
			{
				return (AssemblyName[])this.properties["DependentAssemblies"];
			}
			set
			{
				AssemblyName[] array = new AssemblyName[value.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = value[i];
				}
				this.SetValue("DependentAssemblies", array);
			}
		}

		/// <summary>Gets or sets the description for this <see cref="T:System.Drawing.Design.ToolboxItem" />.</summary>
		/// <returns>A <see cref="T:System.String" /> that specifies the description for this <see cref="T:System.Drawing.Design.ToolboxItem" />.</returns>
		public string Description
		{
			get
			{
				return (string)this.properties["Description"];
			}
			set
			{
				this.SetValue("Description", value);
			}
		}

		/// <summary>Gets a value indicating whether the toolbox item is transient.</summary>
		/// <returns>
		///   <see langword="true" />, if this toolbox item should not be stored in any toolbox database when an application that is providing a toolbox closes; otherwise, <see langword="false" />.</returns>
		public bool IsTransient
		{
			get
			{
				object obj = this.properties["IsTransient"];
				return obj != null && (bool)obj;
			}
			set
			{
				this.SetValue("IsTransient", value);
			}
		}

		/// <summary>Gets a dictionary of properties.</summary>
		/// <returns>A dictionary of name/value pairs (the names are property names and the values are property values).</returns>
		public IDictionary Properties
		{
			get
			{
				return this.properties;
			}
		}

		/// <summary>Gets the version for this <see cref="T:System.Drawing.Design.ToolboxItem" />.</summary>
		/// <returns>A <see cref="T:System.String" /> that specifies the version for this <see cref="T:System.Drawing.Design.ToolboxItem" />.</returns>
		public virtual string Version
		{
			get
			{
				return string.Empty;
			}
		}

		/// <summary>Throws an exception if the toolbox item is currently locked.</summary>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Drawing.Design.ToolboxItem" /> is locked.</exception>
		protected void CheckUnlocked()
		{
			if (this.locked)
			{
				throw new InvalidOperationException("The ToolboxItem is locked");
			}
		}

		/// <summary>Creates the components that the toolbox item is configured to create.</summary>
		/// <returns>An array of created <see cref="T:System.ComponentModel.IComponent" /> objects.</returns>
		public IComponent[] CreateComponents()
		{
			return this.CreateComponents(null);
		}

		/// <summary>Creates the components that the toolbox item is configured to create, using the specified designer host.</summary>
		/// <param name="host">The <see cref="T:System.ComponentModel.Design.IDesignerHost" /> to use when creating the components.</param>
		/// <returns>An array of created <see cref="T:System.ComponentModel.IComponent" /> objects.</returns>
		public IComponent[] CreateComponents(IDesignerHost host)
		{
			this.OnComponentsCreating(new ToolboxComponentsCreatingEventArgs(host));
			IComponent[] array = this.CreateComponentsCore(host);
			this.OnComponentsCreated(new ToolboxComponentsCreatedEventArgs(array));
			return array;
		}

		/// <summary>Creates a component or an array of components when the toolbox item is invoked.</summary>
		/// <param name="host">The <see cref="T:System.ComponentModel.Design.IDesignerHost" /> to host the toolbox item.</param>
		/// <returns>An array of created <see cref="T:System.ComponentModel.IComponent" /> objects.</returns>
		protected virtual IComponent[] CreateComponentsCore(IDesignerHost host)
		{
			if (host == null)
			{
				throw new ArgumentNullException("host");
			}
			Type type = this.GetType(host, this.AssemblyName, this.TypeName, true);
			IComponent[] result;
			if (type == null)
			{
				result = new IComponent[0];
			}
			else
			{
				result = new IComponent[]
				{
					host.CreateComponent(type)
				};
			}
			return result;
		}

		/// <summary>Creates an array of components when the toolbox item is invoked.</summary>
		/// <param name="host">The designer host to use when creating components.</param>
		/// <param name="defaultValues">A dictionary of property name/value pairs of default values with which to initialize the component.</param>
		/// <returns>An array of created <see cref="T:System.ComponentModel.IComponent" /> objects.</returns>
		protected virtual IComponent[] CreateComponentsCore(IDesignerHost host, IDictionary defaultValues)
		{
			IComponent[] array = this.CreateComponentsCore(host);
			foreach (Component component in array)
			{
				(host.GetDesigner(component) as IComponentInitializer).InitializeNewComponent(defaultValues);
			}
			return array;
		}

		/// <summary>Creates the components that the toolbox item is configured to create, using the specified designer host and default values.</summary>
		/// <param name="host">The <see cref="T:System.ComponentModel.Design.IDesignerHost" /> to use when creating the components.</param>
		/// <param name="defaultValues">A dictionary of property name/value pairs of default values with which to initialize the component.</param>
		/// <returns>An array of created <see cref="T:System.ComponentModel.IComponent" /> objects.</returns>
		public IComponent[] CreateComponents(IDesignerHost host, IDictionary defaultValues)
		{
			this.OnComponentsCreating(new ToolboxComponentsCreatingEventArgs(host));
			IComponent[] array = this.CreateComponentsCore(host, defaultValues);
			this.OnComponentsCreated(new ToolboxComponentsCreatedEventArgs(array));
			return array;
		}

		/// <summary>Filters a property value before returning it.</summary>
		/// <param name="propertyName">The name of the property to filter.</param>
		/// <param name="value">The value against which to filter the property.</param>
		/// <returns>A filtered property value.</returns>
		protected virtual object FilterPropertyValue(string propertyName, object value)
		{
			if (!(propertyName == "AssemblyName"))
			{
				if (!(propertyName == "DisplayName") && !(propertyName == "TypeName"))
				{
					if (!(propertyName == "Filter"))
					{
						return value;
					}
					if (value != null)
					{
						return value;
					}
					return new ToolboxItemFilterAttribute[0];
				}
				else
				{
					if (value != null)
					{
						return value;
					}
					return string.Empty;
				}
			}
			else
			{
				if (value != null)
				{
					return (value as ICloneable).Clone();
				}
				return null;
			}
		}

		/// <summary>Loads the state of the toolbox item from the specified serialization information object.</summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to load from.</param>
		/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that indicates the stream characteristics.</param>
		protected virtual void Deserialize(SerializationInfo info, StreamingContext context)
		{
			this.AssemblyName = (AssemblyName)info.GetValue("AssemblyName", typeof(AssemblyName));
			this.Bitmap = (Bitmap)info.GetValue("Bitmap", typeof(Bitmap));
			this.Filter = (ICollection)info.GetValue("Filter", typeof(ICollection));
			this.DisplayName = info.GetString("DisplayName");
			this.locked = info.GetBoolean("Locked");
			this.TypeName = info.GetString("TypeName");
		}

		/// <summary>Determines whether two <see cref="T:System.Drawing.Design.ToolboxItem" /> instances are equal.</summary>
		/// <param name="obj">The <see cref="T:System.Drawing.Design.ToolboxItem" /> to compare with the current <see cref="T:System.Drawing.Design.ToolboxItem" />.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Drawing.Design.ToolboxItem" /> is equal to the current <see cref="T:System.Drawing.Design.ToolboxItem" />; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			ToolboxItem toolboxItem = obj as ToolboxItem;
			return toolboxItem != null && (obj == this || (toolboxItem.AssemblyName.Equals(this.AssemblyName) && toolboxItem.Locked.Equals(this.locked) && toolboxItem.TypeName.Equals(this.TypeName) && toolboxItem.DisplayName.Equals(this.DisplayName) && toolboxItem.Bitmap.Equals(this.Bitmap)));
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A hash code for the current <see cref="T:System.Drawing.Design.ToolboxItem" />.</returns>
		public override int GetHashCode()
		{
			return (this.TypeName + this.DisplayName).GetHashCode();
		}

		/// <summary>Enables access to the type associated with the toolbox item.</summary>
		/// <param name="host">The designer host to query for <see cref="T:System.ComponentModel.Design.ITypeResolutionService" />.</param>
		/// <returns>The type associated with the toolbox item.</returns>
		public Type GetType(IDesignerHost host)
		{
			return this.GetType(host, this.AssemblyName, this.TypeName, false);
		}

		/// <summary>Creates an instance of the specified type, optionally using a specified designer host and assembly name.</summary>
		/// <param name="host">The <see cref="T:System.ComponentModel.Design.IDesignerHost" /> for the current document. This can be <see langword="null" />.</param>
		/// <param name="assemblyName">An <see cref="T:System.Reflection.AssemblyName" /> that indicates the assembly that contains the type to load. This can be <see langword="null" />.</param>
		/// <param name="typeName">The name of the type to create an instance of.</param>
		/// <param name="reference">A value indicating whether or not to add a reference to the assembly that contains the specified type to the designer host's set of references.</param>
		/// <returns>An instance of the specified type, if it can be located.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="typeName" /> is not specified.</exception>
		protected virtual Type GetType(IDesignerHost host, AssemblyName assemblyName, string typeName, bool reference)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			if (host == null)
			{
				return null;
			}
			ITypeResolutionService typeResolutionService = host.GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
			Type result = null;
			if (typeResolutionService != null)
			{
				typeResolutionService.GetAssembly(assemblyName, true);
				if (reference)
				{
					typeResolutionService.ReferenceAssembly(assemblyName);
				}
				result = typeResolutionService.GetType(typeName, true);
			}
			else
			{
				Assembly assembly = Assembly.Load(assemblyName);
				if (assembly != null)
				{
					result = assembly.GetType(typeName);
				}
			}
			return result;
		}

		/// <summary>Initializes the current toolbox item with the specified type to create.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> that the toolbox item creates.</param>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Drawing.Design.ToolboxItem" /> was locked.</exception>
		public virtual void Initialize(Type type)
		{
			this.CheckUnlocked();
			if (type == null)
			{
				return;
			}
			this.AssemblyName = type.Assembly.GetName();
			this.DisplayName = type.Name;
			this.TypeName = type.FullName;
			Image image = null;
			object[] customAttributes = type.GetCustomAttributes(true);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				ToolboxBitmapAttribute toolboxBitmapAttribute = customAttributes[i] as ToolboxBitmapAttribute;
				if (toolboxBitmapAttribute != null)
				{
					image = toolboxBitmapAttribute.GetImage(type);
					break;
				}
			}
			if (image == null)
			{
				image = ToolboxBitmapAttribute.GetImageFromResource(type, null, false);
			}
			if (image != null)
			{
				this.Bitmap = (image as Bitmap);
				if (this.Bitmap == null)
				{
					this.Bitmap = new Bitmap(image);
				}
			}
			this.Filter = type.GetCustomAttributes(typeof(ToolboxItemFilterAttribute), true);
		}

		/// <summary>For a description of this member, see the <see cref="M:System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)" /> method.</summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.Serialize(info, context);
		}

		/// <summary>Locks the toolbox item and prevents changes to its properties.</summary>
		public virtual void Lock()
		{
			this.locked = true;
		}

		/// <summary>Raises the <see cref="E:System.Drawing.Design.ToolboxItem.ComponentsCreated" /> event.</summary>
		/// <param name="args">A <see cref="T:System.Drawing.Design.ToolboxComponentsCreatedEventArgs" /> that provides data for the event.</param>
		protected virtual void OnComponentsCreated(ToolboxComponentsCreatedEventArgs args)
		{
			if (this.ComponentsCreated != null)
			{
				this.ComponentsCreated(this, args);
			}
		}

		/// <summary>Raises the <see cref="E:System.Drawing.Design.ToolboxItem.ComponentsCreating" /> event.</summary>
		/// <param name="args">A <see cref="T:System.Drawing.Design.ToolboxComponentsCreatingEventArgs" /> that provides data for the event.</param>
		protected virtual void OnComponentsCreating(ToolboxComponentsCreatingEventArgs args)
		{
			if (this.ComponentsCreating != null)
			{
				this.ComponentsCreating(this, args);
			}
		}

		/// <summary>Saves the state of the toolbox item to the specified serialization information object.</summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to save to.</param>
		/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that indicates the stream characteristics.</param>
		protected virtual void Serialize(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("AssemblyName", this.AssemblyName);
			info.AddValue("Bitmap", this.Bitmap);
			info.AddValue("Filter", this.Filter);
			info.AddValue("DisplayName", this.DisplayName);
			info.AddValue("Locked", this.locked);
			info.AddValue("TypeName", this.TypeName);
		}

		/// <summary>Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Drawing.Design.ToolboxItem" />.</summary>
		/// <returns>A <see cref="T:System.String" /> that represents the current <see cref="T:System.Drawing.Design.ToolboxItem" />.</returns>
		public override string ToString()
		{
			return this.DisplayName;
		}

		/// <summary>Validates that an object is of a given type.</summary>
		/// <param name="propertyName">The name of the property to validate.</param>
		/// <param name="value">Optional value against which to validate.</param>
		/// <param name="expectedType">The expected type of the property.</param>
		/// <param name="allowNull">
		///   <see langword="true" /> to allow <see langword="null" />; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />, and <paramref name="allowNull" /> is <see langword="false" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="value" /> is not the type specified by <paramref name="expectedType" />.</exception>
		protected void ValidatePropertyType(string propertyName, object value, Type expectedType, bool allowNull)
		{
			if (!allowNull && value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value != null && !expectedType.Equals(value.GetType()))
			{
				throw new ArgumentException(Locale.GetText("Type mismatch between value ({0}) and expected type ({1}).", new object[]
				{
					value.GetType(),
					expectedType
				}), "value");
			}
		}

		/// <summary>Validates a property before it is assigned to the property dictionary.</summary>
		/// <param name="propertyName">The name of the property to validate.</param>
		/// <param name="value">The value against which to validate.</param>
		/// <returns>The value used to perform validation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />, and <paramref name="propertyName" /> is "IsTransient".</exception>
		protected virtual object ValidatePropertyValue(string propertyName, object value)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(propertyName);
			if (num <= 1629252038U)
			{
				if (num <= 982935374U)
				{
					if (num != 278446637U)
					{
						if (num != 982935374U)
						{
							return value;
						}
						if (!(propertyName == "TypeName"))
						{
							return value;
						}
					}
					else
					{
						if (!(propertyName == "IsTransient"))
						{
							return value;
						}
						this.ValidatePropertyType(propertyName, value, typeof(bool), false);
						return value;
					}
				}
				else if (num != 1561053712U)
				{
					if (num != 1629252038U)
					{
						return value;
					}
					if (!(propertyName == "AssemblyName"))
					{
						return value;
					}
					this.ValidatePropertyType(propertyName, value, typeof(AssemblyName), true);
					return value;
				}
				else
				{
					if (!(propertyName == "DependentAssemblies"))
					{
						return value;
					}
					this.ValidatePropertyType(propertyName, value, typeof(AssemblyName[]), true);
					return value;
				}
			}
			else if (num <= 1725856265U)
			{
				if (num != 1651150918U)
				{
					if (num != 1725856265U)
					{
						return value;
					}
					if (!(propertyName == "Description"))
					{
						return value;
					}
				}
				else
				{
					if (!(propertyName == "Bitmap"))
					{
						return value;
					}
					this.ValidatePropertyType(propertyName, value, typeof(Bitmap), true);
					return value;
				}
			}
			else if (num != 3250523996U)
			{
				if (num != 4104765591U)
				{
					if (num != 4176258230U)
					{
						return value;
					}
					if (!(propertyName == "DisplayName"))
					{
						return value;
					}
				}
				else
				{
					if (!(propertyName == "Filter"))
					{
						return value;
					}
					this.ValidatePropertyType(propertyName, value, typeof(ToolboxItemFilterAttribute[]), true);
					if (value == null)
					{
						return new ToolboxItemFilterAttribute[0];
					}
					return value;
				}
			}
			else if (!(propertyName == "Company"))
			{
				return value;
			}
			this.ValidatePropertyType(propertyName, value, typeof(string), true);
			if (value == null)
			{
				value = string.Empty;
			}
			return value;
		}

		private void SetValue(string propertyName, object value)
		{
			this.CheckUnlocked();
			this.properties[propertyName] = this.ValidatePropertyValue(propertyName, value);
		}

		private string GetValue(string propertyName)
		{
			string text = (string)this.properties[propertyName];
			if (text != null)
			{
				return text;
			}
			return string.Empty;
		}

		/// <summary>Occurs immediately after components are created.</summary>
		public event ToolboxComponentsCreatedEventHandler ComponentsCreated;

		/// <summary>Occurs when components are about to be created.</summary>
		public event ToolboxComponentsCreatingEventHandler ComponentsCreating;

		/// <summary>Gets or sets the original bitmap that will be used in the toolbox for this item.</summary>
		/// <returns>A <see cref="T:System.Drawing.Bitmap" /> that represents the toolbox item in the toolbox.</returns>
		public Bitmap OriginalBitmap
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		private bool locked;

		private Hashtable properties = new Hashtable();
	}
}
