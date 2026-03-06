using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;

namespace System.ComponentModel
{
	/// <summary>Provides information about the characteristics for a component, such as its attributes, properties, and events. This class cannot be inherited.</summary>
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public sealed class TypeDescriptor
	{
		private TypeDescriptor()
		{
		}

		/// <summary>Gets or sets the provider for the Component Object Model (COM) type information for the target component.</summary>
		/// <returns>An <see cref="T:System.ComponentModel.IComNativeDescriptorHandler" /> instance representing the COM type information provider.</returns>
		[Obsolete("This property has been deprecated.  Use a type description provider to supply type information for COM types instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
		public static IComNativeDescriptorHandler ComNativeDescriptorHandler
		{
			[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
			get
			{
				TypeDescriptor.TypeDescriptionNode typeDescriptionNode = TypeDescriptor.NodeFor(TypeDescriptor.ComObjectType);
				TypeDescriptor.ComNativeDescriptionProvider comNativeDescriptionProvider;
				do
				{
					comNativeDescriptionProvider = (typeDescriptionNode.Provider as TypeDescriptor.ComNativeDescriptionProvider);
					typeDescriptionNode = typeDescriptionNode.Next;
				}
				while (typeDescriptionNode != null && comNativeDescriptionProvider == null);
				if (comNativeDescriptionProvider != null)
				{
					return comNativeDescriptionProvider.Handler;
				}
				return null;
			}
			[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
			set
			{
				TypeDescriptor.TypeDescriptionNode typeDescriptionNode = TypeDescriptor.NodeFor(TypeDescriptor.ComObjectType);
				while (typeDescriptionNode != null && !(typeDescriptionNode.Provider is TypeDescriptor.ComNativeDescriptionProvider))
				{
					typeDescriptionNode = typeDescriptionNode.Next;
				}
				if (typeDescriptionNode == null)
				{
					TypeDescriptor.AddProvider(new TypeDescriptor.ComNativeDescriptionProvider(value), TypeDescriptor.ComObjectType);
					return;
				}
				((TypeDescriptor.ComNativeDescriptionProvider)typeDescriptionNode.Provider).Handler = value;
			}
		}

		/// <summary>Gets the type of the Component Object Model (COM) object represented by the target component.</summary>
		/// <returns>The <see cref="T:System.Type" /> of the COM object represented by this component, or <see langword="null" /> for non-COM objects.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static Type ComObjectType
		{
			get
			{
				return typeof(TypeDescriptor.TypeDescriptorComObject);
			}
		}

		/// <summary>Gets a type that represents a type description provider for all interface types.</summary>
		/// <returns>A <see cref="T:System.Type" /> that represents a custom type description provider for all interface types.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static Type InterfaceType
		{
			get
			{
				return typeof(TypeDescriptor.TypeDescriptorInterface);
			}
		}

		internal static int MetadataVersion
		{
			get
			{
				return TypeDescriptor._metadataVersion;
			}
		}

		/// <summary>Occurs when the cache for a component is cleared.</summary>
		public static event RefreshEventHandler Refreshed;

		/// <summary>Adds class-level attributes to the target component type.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> of the target component.</param>
		/// <param name="attributes">An array of <see cref="T:System.Attribute" /> objects to add to the component's class.</param>
		/// <returns>The newly created <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> that was used to add the specified attributes.</returns>
		/// <exception cref="T:System.ArgumentNullException">One or both of the parameters is <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public static TypeDescriptionProvider AddAttributes(Type type, params Attribute[] attributes)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (attributes == null)
			{
				throw new ArgumentNullException("attributes");
			}
			TypeDescriptor.AttributeProvider attributeProvider = new TypeDescriptor.AttributeProvider(TypeDescriptor.GetProvider(type), attributes);
			TypeDescriptor.AddProvider(attributeProvider, type);
			return attributeProvider;
		}

		/// <summary>Adds class-level attributes to the target component instance.</summary>
		/// <param name="instance">An instance of the target component.</param>
		/// <param name="attributes">An array of <see cref="T:System.Attribute" /> objects to add to the component's class.</param>
		/// <returns>The newly created <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> that was used to add the specified attributes.</returns>
		/// <exception cref="T:System.ArgumentNullException">One or both of the parameters is <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public static TypeDescriptionProvider AddAttributes(object instance, params Attribute[] attributes)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (attributes == null)
			{
				throw new ArgumentNullException("attributes");
			}
			TypeDescriptor.AttributeProvider attributeProvider = new TypeDescriptor.AttributeProvider(TypeDescriptor.GetProvider(instance), attributes);
			TypeDescriptor.AddProvider(attributeProvider, instance);
			return attributeProvider;
		}

		/// <summary>Adds an editor table for the given editor base type.</summary>
		/// <param name="editorBaseType">The editor base type to add the editor table for. If a table already exists for this type, this method will do nothing.</param>
		/// <param name="table">The <see cref="T:System.Collections.Hashtable" /> to add.</param>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void AddEditorTable(Type editorBaseType, Hashtable table)
		{
			ReflectTypeDescriptionProvider.AddEditorTable(editorBaseType, table);
		}

		/// <summary>Adds a type description provider for a component class.</summary>
		/// <param name="provider">The <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> to add.</param>
		/// <param name="type">The <see cref="T:System.Type" /> of the target component.</param>
		/// <exception cref="T:System.ArgumentNullException">One or both of the parameters are <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public static void AddProvider(TypeDescriptionProvider provider, Type type)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			WeakHashtable providerTable = TypeDescriptor._providerTable;
			lock (providerTable)
			{
				TypeDescriptor.TypeDescriptionNode next = TypeDescriptor.NodeFor(type, true);
				TypeDescriptor.TypeDescriptionNode typeDescriptionNode = new TypeDescriptor.TypeDescriptionNode(provider);
				typeDescriptionNode.Next = next;
				TypeDescriptor._providerTable[type] = typeDescriptionNode;
				TypeDescriptor._providerTypeTable.Clear();
			}
			TypeDescriptor.Refresh(type);
		}

		/// <summary>Adds a type description provider for a single instance of a component.</summary>
		/// <param name="provider">The <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> to add.</param>
		/// <param name="instance">An instance of the target component.</param>
		/// <exception cref="T:System.ArgumentNullException">One or both of the parameters are <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public static void AddProvider(TypeDescriptionProvider provider, object instance)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			WeakHashtable providerTable = TypeDescriptor._providerTable;
			bool flag2;
			lock (providerTable)
			{
				flag2 = TypeDescriptor._providerTable.ContainsKey(instance);
				TypeDescriptor.TypeDescriptionNode next = TypeDescriptor.NodeFor(instance, true);
				TypeDescriptor.TypeDescriptionNode typeDescriptionNode = new TypeDescriptor.TypeDescriptionNode(provider);
				typeDescriptionNode.Next = next;
				TypeDescriptor._providerTable.SetWeak(instance, typeDescriptionNode);
				TypeDescriptor._providerTypeTable.Clear();
			}
			if (flag2)
			{
				TypeDescriptor.Refresh(instance, false);
			}
		}

		/// <summary>Adds a type description provider for a component class.</summary>
		/// <param name="provider">The <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> to add.</param>
		/// <param name="type">The <see cref="T:System.Type" /> of the target component.</param>
		/// <exception cref="T:System.ArgumentNullException">One or both of the parameters are <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void AddProviderTransparent(TypeDescriptionProvider provider, Type type)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			TypeDescriptor.AddProvider(provider, type);
		}

		/// <summary>Adds a type description provider for a single instance of a component.</summary>
		/// <param name="provider">The <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> to add.</param>
		/// <param name="instance">An instance of the target component.</param>
		/// <exception cref="T:System.ArgumentNullException">One or both of the parameters are <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void AddProviderTransparent(TypeDescriptionProvider provider, object instance)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			TypeDescriptor.AddProvider(provider, instance);
		}

		private static void CheckDefaultProvider(Type type)
		{
			object internalSyncObject;
			if (TypeDescriptor._defaultProviders == null)
			{
				internalSyncObject = TypeDescriptor._internalSyncObject;
				lock (internalSyncObject)
				{
					if (TypeDescriptor._defaultProviders == null)
					{
						TypeDescriptor._defaultProviders = new Hashtable();
					}
				}
			}
			if (TypeDescriptor._defaultProviders.ContainsKey(type))
			{
				return;
			}
			internalSyncObject = TypeDescriptor._internalSyncObject;
			lock (internalSyncObject)
			{
				if (TypeDescriptor._defaultProviders.ContainsKey(type))
				{
					return;
				}
				TypeDescriptor._defaultProviders[type] = null;
			}
			object[] customAttributes = type.GetCustomAttributes(typeof(TypeDescriptionProviderAttribute), false);
			bool flag2 = false;
			for (int i = customAttributes.Length - 1; i >= 0; i--)
			{
				Type type2 = Type.GetType(((TypeDescriptionProviderAttribute)customAttributes[i]).TypeName);
				if (type2 != null && typeof(TypeDescriptionProvider).IsAssignableFrom(type2))
				{
					TypeDescriptor.AddProvider((TypeDescriptionProvider)Activator.CreateInstance(type2), type);
					flag2 = true;
				}
			}
			if (!flag2)
			{
				Type baseType = type.BaseType;
				if (baseType != null && baseType != type)
				{
					TypeDescriptor.CheckDefaultProvider(baseType);
				}
			}
		}

		/// <summary>Creates a primary-secondary association between two objects.</summary>
		/// <param name="primary">The primary <see cref="T:System.Object" />.</param>
		/// <param name="secondary">The secondary <see cref="T:System.Object" />.</param>
		/// <exception cref="T:System.ArgumentNullException">One or both of the parameters are <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="primary" /> is equal to <paramref name="secondary" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public static void CreateAssociation(object primary, object secondary)
		{
			if (primary == null)
			{
				throw new ArgumentNullException("primary");
			}
			if (secondary == null)
			{
				throw new ArgumentNullException("secondary");
			}
			if (primary == secondary)
			{
				throw new ArgumentException(SR.GetString("Cannot create an association when the primary and secondary objects are the same."));
			}
			if (TypeDescriptor._associationTable == null)
			{
				object internalSyncObject = TypeDescriptor._internalSyncObject;
				lock (internalSyncObject)
				{
					if (TypeDescriptor._associationTable == null)
					{
						TypeDescriptor._associationTable = new WeakHashtable();
					}
				}
			}
			IList list = (IList)TypeDescriptor._associationTable[primary];
			if (list == null)
			{
				WeakHashtable associationTable = TypeDescriptor._associationTable;
				lock (associationTable)
				{
					list = (IList)TypeDescriptor._associationTable[primary];
					if (list == null)
					{
						list = new ArrayList(4);
						TypeDescriptor._associationTable.SetWeak(primary, list);
					}
					goto IL_112;
				}
			}
			for (int i = list.Count - 1; i >= 0; i--)
			{
				WeakReference weakReference = (WeakReference)list[i];
				if (weakReference.IsAlive && weakReference.Target == secondary)
				{
					throw new ArgumentException(SR.GetString("The primary and secondary objects are already associated with each other."));
				}
			}
			IL_112:
			IList obj = list;
			lock (obj)
			{
				list.Add(new WeakReference(secondary));
			}
		}

		/// <summary>Creates an instance of the designer associated with the specified component and of the specified type of designer.</summary>
		/// <param name="component">An <see cref="T:System.ComponentModel.IComponent" /> that specifies the component to associate with the designer.</param>
		/// <param name="designerBaseType">A <see cref="T:System.Type" /> that represents the type of designer to create.</param>
		/// <returns>An <see cref="T:System.ComponentModel.Design.IDesigner" /> that is an instance of the designer for the component, or <see langword="null" /> if no designer can be found.</returns>
		public static IDesigner CreateDesigner(IComponent component, Type designerBaseType)
		{
			Type type = null;
			IDesigner result = null;
			AttributeCollection attributes = TypeDescriptor.GetAttributes(component);
			for (int i = 0; i < attributes.Count; i++)
			{
				DesignerAttribute designerAttribute = attributes[i] as DesignerAttribute;
				if (designerAttribute != null)
				{
					Type type2 = Type.GetType(designerAttribute.DesignerBaseTypeName);
					if (type2 != null && type2 == designerBaseType)
					{
						ISite site = component.Site;
						bool flag = false;
						if (site != null)
						{
							ITypeResolutionService typeResolutionService = (ITypeResolutionService)site.GetService(typeof(ITypeResolutionService));
							if (typeResolutionService != null)
							{
								flag = true;
								type = typeResolutionService.GetType(designerAttribute.DesignerTypeName);
							}
						}
						if (!flag)
						{
							type = Type.GetType(designerAttribute.DesignerTypeName);
						}
						if (type != null)
						{
							break;
						}
					}
				}
			}
			if (type != null)
			{
				result = (IDesigner)SecurityUtils.SecureCreateInstance(type, null, true);
			}
			return result;
		}

		/// <summary>Creates a new event descriptor that is identical to an existing event descriptor by dynamically generating descriptor information from a specified event on a type.</summary>
		/// <param name="componentType">The type of the component the event lives on.</param>
		/// <param name="name">The name of the event.</param>
		/// <param name="type">The type of the delegate that handles the event.</param>
		/// <param name="attributes">The attributes for this event.</param>
		/// <returns>An <see cref="T:System.ComponentModel.EventDescriptor" /> that is bound to a type.</returns>
		[ReflectionPermission(SecurityAction.LinkDemand, Flags = ReflectionPermissionFlag.MemberAccess)]
		public static EventDescriptor CreateEvent(Type componentType, string name, Type type, params Attribute[] attributes)
		{
			return new ReflectEventDescriptor(componentType, name, type, attributes);
		}

		/// <summary>Creates a new event descriptor that is identical to an existing event descriptor, when passed the existing <see cref="T:System.ComponentModel.EventDescriptor" />.</summary>
		/// <param name="componentType">The type of the component for which to create the new event.</param>
		/// <param name="oldEventDescriptor">The existing event information.</param>
		/// <param name="attributes">The new attributes.</param>
		/// <returns>A new <see cref="T:System.ComponentModel.EventDescriptor" /> that has merged the specified metadata attributes with the existing metadata attributes.</returns>
		[ReflectionPermission(SecurityAction.LinkDemand, Flags = ReflectionPermissionFlag.MemberAccess)]
		public static EventDescriptor CreateEvent(Type componentType, EventDescriptor oldEventDescriptor, params Attribute[] attributes)
		{
			return new ReflectEventDescriptor(componentType, oldEventDescriptor, attributes);
		}

		/// <summary>Creates an object that can substitute for another data type.</summary>
		/// <param name="provider">The service provider that provides a <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> service. This parameter can be <see langword="null" />.</param>
		/// <param name="objectType">The <see cref="T:System.Type" /> of object to create.</param>
		/// <param name="argTypes">An optional array of parameter types to be passed to the object's constructor. This parameter can be <see langword="null" /> or an array of zero length.</param>
		/// <param name="args">An optional array of parameter values to pass to the object's constructor. If not <see langword="null" />, the number of elements must be the same as <paramref name="argTypes" />.</param>
		/// <returns>An instance of the substitute data type if an associated <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> is found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="objectType" /> is <see langword="null" />, or <paramref name="args" /> is <see langword="null" /> when <paramref name="argTypes" /> is not <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="argTypes" /> and <paramref name="args" /> have different number of elements.</exception>
		public static object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
		{
			if (objectType == null)
			{
				throw new ArgumentNullException("objectType");
			}
			if (argTypes != null)
			{
				if (args == null)
				{
					throw new ArgumentNullException("args");
				}
				if (argTypes.Length != args.Length)
				{
					throw new ArgumentException(SR.GetString("The number of elements in the Type and Object arrays must match."));
				}
			}
			object obj = null;
			if (provider != null)
			{
				TypeDescriptionProvider typeDescriptionProvider = provider.GetService(typeof(TypeDescriptionProvider)) as TypeDescriptionProvider;
				if (typeDescriptionProvider != null)
				{
					obj = typeDescriptionProvider.CreateInstance(provider, objectType, argTypes, args);
				}
			}
			if (obj == null)
			{
				obj = TypeDescriptor.NodeFor(objectType).CreateInstance(provider, objectType, argTypes, args);
			}
			return obj;
		}

		/// <summary>Creates and dynamically binds a property descriptor to a type, using the specified property name, type, and attribute array.</summary>
		/// <param name="componentType">The <see cref="T:System.Type" /> of the component that the property is a member of.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="type">The <see cref="T:System.Type" /> of the property.</param>
		/// <param name="attributes">The new attributes for this property.</param>
		/// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptor" /> that is bound to the specified type and that has the specified metadata attributes merged with the existing metadata attributes.</returns>
		[ReflectionPermission(SecurityAction.LinkDemand, Flags = ReflectionPermissionFlag.MemberAccess)]
		public static PropertyDescriptor CreateProperty(Type componentType, string name, Type type, params Attribute[] attributes)
		{
			return new ReflectPropertyDescriptor(componentType, name, type, attributes);
		}

		/// <summary>Creates a new property descriptor from an existing property descriptor, using the specified existing <see cref="T:System.ComponentModel.PropertyDescriptor" /> and attribute array.</summary>
		/// <param name="componentType">The <see cref="T:System.Type" /> of the component that the property is a member of.</param>
		/// <param name="oldPropertyDescriptor">The existing property descriptor.</param>
		/// <param name="attributes">The new attributes for this property.</param>
		/// <returns>A new <see cref="T:System.ComponentModel.PropertyDescriptor" /> that has the specified metadata attributes merged with the existing metadata attributes.</returns>
		[ReflectionPermission(SecurityAction.LinkDemand, Flags = ReflectionPermissionFlag.MemberAccess)]
		public static PropertyDescriptor CreateProperty(Type componentType, PropertyDescriptor oldPropertyDescriptor, params Attribute[] attributes)
		{
			if (componentType == oldPropertyDescriptor.ComponentType && ((ExtenderProvidedPropertyAttribute)oldPropertyDescriptor.Attributes[typeof(ExtenderProvidedPropertyAttribute)]).ExtenderProperty is ReflectPropertyDescriptor)
			{
				return new ExtendedPropertyDescriptor(oldPropertyDescriptor, attributes);
			}
			return new ReflectPropertyDescriptor(componentType, oldPropertyDescriptor, attributes);
		}

		[Conditional("DEBUG")]
		private static void DebugValidate(Type type, AttributeCollection attributes, AttributeCollection debugAttributes)
		{
		}

		[Conditional("DEBUG")]
		private static void DebugValidate(AttributeCollection attributes, AttributeCollection debugAttributes)
		{
		}

		[Conditional("DEBUG")]
		private static void DebugValidate(AttributeCollection attributes, Type type)
		{
		}

		[Conditional("DEBUG")]
		private static void DebugValidate(AttributeCollection attributes, object instance, bool noCustomTypeDesc)
		{
		}

		[Conditional("DEBUG")]
		private static void DebugValidate(TypeConverter converter, Type type)
		{
		}

		[Conditional("DEBUG")]
		private static void DebugValidate(TypeConverter converter, object instance, bool noCustomTypeDesc)
		{
		}

		[Conditional("DEBUG")]
		private static void DebugValidate(EventDescriptorCollection events, Type type, Attribute[] attributes)
		{
		}

		[Conditional("DEBUG")]
		private static void DebugValidate(EventDescriptorCollection events, object instance, Attribute[] attributes, bool noCustomTypeDesc)
		{
		}

		[Conditional("DEBUG")]
		private static void DebugValidate(PropertyDescriptorCollection properties, Type type, Attribute[] attributes)
		{
		}

		[Conditional("DEBUG")]
		private static void DebugValidate(PropertyDescriptorCollection properties, object instance, Attribute[] attributes, bool noCustomTypeDesc)
		{
		}

		private static ArrayList FilterMembers(IList members, Attribute[] attributes)
		{
			ArrayList arrayList = null;
			int count = members.Count;
			for (int i = 0; i < count; i++)
			{
				bool flag = false;
				for (int j = 0; j < attributes.Length; j++)
				{
					if (TypeDescriptor.ShouldHideMember((MemberDescriptor)members[i], attributes[j]))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					if (arrayList == null)
					{
						arrayList = new ArrayList(count);
						for (int k = 0; k < i; k++)
						{
							arrayList.Add(members[k]);
						}
					}
				}
				else if (arrayList != null)
				{
					arrayList.Add(members[i]);
				}
			}
			return arrayList;
		}

		/// <summary>Returns an instance of the type associated with the specified primary object.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> of the target component.</param>
		/// <param name="primary">The primary object of the association.</param>
		/// <returns>An instance of the secondary type that has been associated with the primary object if an association exists; otherwise, <paramref name="primary" /> if no specified association exists.</returns>
		/// <exception cref="T:System.ArgumentNullException">One or both of the parameters are <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static object GetAssociation(Type type, object primary)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (primary == null)
			{
				throw new ArgumentNullException("primary");
			}
			object obj = primary;
			if (!type.IsInstanceOfType(primary))
			{
				Hashtable associationTable = TypeDescriptor._associationTable;
				if (associationTable != null)
				{
					IList list = (IList)associationTable[primary];
					if (list != null)
					{
						IList obj2 = list;
						lock (obj2)
						{
							for (int i = list.Count - 1; i >= 0; i--)
							{
								object target = ((WeakReference)list[i]).Target;
								if (target == null)
								{
									list.RemoveAt(i);
								}
								else if (type.IsInstanceOfType(target))
								{
									obj = target;
								}
							}
						}
					}
				}
				if (obj == primary)
				{
					IComponent component = primary as IComponent;
					if (component != null)
					{
						ISite site = component.Site;
						if (site != null && site.DesignMode)
						{
							IDesignerHost designerHost = site.GetService(typeof(IDesignerHost)) as IDesignerHost;
							if (designerHost != null)
							{
								object designer = designerHost.GetDesigner(component);
								if (designer != null && type.IsInstanceOfType(designer))
								{
									obj = designer;
								}
							}
						}
					}
				}
			}
			return obj;
		}

		/// <summary>Returns a collection of attributes for the specified type of component.</summary>
		/// <param name="componentType">The <see cref="T:System.Type" /> of the target component.</param>
		/// <returns>An <see cref="T:System.ComponentModel.AttributeCollection" /> with the attributes for the type of the component. If the component is <see langword="null" />, this method returns an empty collection.</returns>
		public static AttributeCollection GetAttributes(Type componentType)
		{
			if (componentType == null)
			{
				return new AttributeCollection(null);
			}
			return TypeDescriptor.GetDescriptor(componentType, "componentType").GetAttributes();
		}

		/// <summary>Returns the collection of attributes for the specified component.</summary>
		/// <param name="component">The component for which you want to get attributes.</param>
		/// <returns>An <see cref="T:System.ComponentModel.AttributeCollection" /> containing the attributes for the component. If <paramref name="component" /> is <see langword="null" />, this method returns an empty collection.</returns>
		public static AttributeCollection GetAttributes(object component)
		{
			return TypeDescriptor.GetAttributes(component, false);
		}

		/// <summary>Returns a collection of attributes for the specified component and a Boolean indicating that a custom type descriptor has been created.</summary>
		/// <param name="component">The component for which you want to get attributes.</param>
		/// <param name="noCustomTypeDesc">
		///   <see langword="true" /> to use a baseline set of attributes from the custom type descriptor if <paramref name="component" /> is of type <see cref="T:System.ComponentModel.ICustomTypeDescriptor" />; otherwise, <see langword="false" />.</param>
		/// <returns>An <see cref="T:System.ComponentModel.AttributeCollection" /> with the attributes for the component. If the component is <see langword="null" />, this method returns an empty collection.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static AttributeCollection GetAttributes(object component, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				return new AttributeCollection(null);
			}
			ICollection collection = TypeDescriptor.GetDescriptor(component, noCustomTypeDesc).GetAttributes();
			if (component is ICustomTypeDescriptor)
			{
				if (noCustomTypeDesc)
				{
					ICustomTypeDescriptor extendedDescriptor = TypeDescriptor.GetExtendedDescriptor(component);
					if (extendedDescriptor != null)
					{
						ICollection attributes = extendedDescriptor.GetAttributes();
						collection = TypeDescriptor.PipelineMerge(0, collection, attributes, component, null);
					}
				}
				else
				{
					collection = TypeDescriptor.PipelineFilter(0, collection, component, null);
				}
			}
			else
			{
				IDictionary cache = TypeDescriptor.GetCache(component);
				collection = TypeDescriptor.PipelineInitialize(0, collection, cache);
				ICustomTypeDescriptor extendedDescriptor2 = TypeDescriptor.GetExtendedDescriptor(component);
				if (extendedDescriptor2 != null)
				{
					ICollection attributes2 = extendedDescriptor2.GetAttributes();
					collection = TypeDescriptor.PipelineMerge(0, collection, attributes2, component, cache);
				}
				collection = TypeDescriptor.PipelineFilter(0, collection, component, cache);
			}
			AttributeCollection attributeCollection = collection as AttributeCollection;
			if (attributeCollection == null)
			{
				Attribute[] array = new Attribute[collection.Count];
				collection.CopyTo(array, 0);
				attributeCollection = new AttributeCollection(array);
			}
			return attributeCollection;
		}

		internal static IDictionary GetCache(object instance)
		{
			return TypeDescriptor.NodeFor(instance).GetCache(instance);
		}

		/// <summary>Returns the name of the class for the specified component using the default type descriptor.</summary>
		/// <param name="component">The <see cref="T:System.Object" /> for which you want the class name.</param>
		/// <returns>A <see cref="T:System.String" /> containing the name of the class for the specified component.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="component" /> is <see langword="null" />.</exception>
		public static string GetClassName(object component)
		{
			return TypeDescriptor.GetClassName(component, false);
		}

		/// <summary>Returns the name of the class for the specified component using a custom type descriptor.</summary>
		/// <param name="component">The <see cref="T:System.Object" /> for which you want the class name.</param>
		/// <param name="noCustomTypeDesc">
		///   <see langword="true" /> to consider custom type description information; otherwise, <see langword="false" />.</param>
		/// <returns>A <see cref="T:System.String" /> containing the name of the class for the specified component.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="component" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static string GetClassName(object component, bool noCustomTypeDesc)
		{
			return TypeDescriptor.GetDescriptor(component, noCustomTypeDesc).GetClassName();
		}

		/// <summary>Returns the name of the class for the specified type.</summary>
		/// <param name="componentType">The <see cref="T:System.Type" /> of the target component.</param>
		/// <returns>A <see cref="T:System.String" /> containing the name of the class for the specified component type.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="componentType" /> is <see langword="null" />.</exception>
		public static string GetClassName(Type componentType)
		{
			return TypeDescriptor.GetDescriptor(componentType, "componentType").GetClassName();
		}

		/// <summary>Returns the name of the specified component using the default type descriptor.</summary>
		/// <param name="component">The <see cref="T:System.Object" /> for which you want the class name.</param>
		/// <returns>A <see cref="T:System.String" /> containing the name of the specified component, or <see langword="null" /> if there is no component name.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="component" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		public static string GetComponentName(object component)
		{
			return TypeDescriptor.GetComponentName(component, false);
		}

		/// <summary>Returns the name of the specified component using a custom type descriptor.</summary>
		/// <param name="component">The <see cref="T:System.Object" /> for which you want the class name.</param>
		/// <param name="noCustomTypeDesc">
		///   <see langword="true" /> to consider custom type description information; otherwise, <see langword="false" />.</param>
		/// <returns>The name of the class for the specified component, or <see langword="null" /> if there is no component name.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="component" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static string GetComponentName(object component, bool noCustomTypeDesc)
		{
			return TypeDescriptor.GetDescriptor(component, noCustomTypeDesc).GetComponentName();
		}

		/// <summary>Returns a type converter for the type of the specified component.</summary>
		/// <param name="component">A component to get the converter for.</param>
		/// <returns>A <see cref="T:System.ComponentModel.TypeConverter" /> for the specified component.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="component" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		public static TypeConverter GetConverter(object component)
		{
			return TypeDescriptor.GetConverter(component, false);
		}

		/// <summary>Returns a type converter for the type of the specified component with a custom type descriptor.</summary>
		/// <param name="component">A component to get the converter for.</param>
		/// <param name="noCustomTypeDesc">
		///   <see langword="true" /> to consider custom type description information; otherwise, <see langword="false" />.</param>
		/// <returns>A <see cref="T:System.ComponentModel.TypeConverter" /> for the specified component.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="component" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static TypeConverter GetConverter(object component, bool noCustomTypeDesc)
		{
			return TypeDescriptor.GetDescriptor(component, noCustomTypeDesc).GetConverter();
		}

		/// <summary>Returns a type converter for the specified type.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> of the target component.</param>
		/// <returns>A <see cref="T:System.ComponentModel.TypeConverter" /> for the specified type.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="type" /> is <see langword="null" />.</exception>
		public static TypeConverter GetConverter(Type type)
		{
			return TypeDescriptor.GetDescriptor(type, "type").GetConverter();
		}

		private static object ConvertFromInvariantString(Type type, string stringValue)
		{
			return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(stringValue);
		}

		/// <summary>Returns the default event for the specified type of component.</summary>
		/// <param name="componentType">The <see cref="T:System.Type" /> of the target component.</param>
		/// <returns>An <see cref="T:System.ComponentModel.EventDescriptor" /> with the default event, or <see langword="null" /> if there are no events.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="componentType" /> is <see langword="null" />.</exception>
		public static EventDescriptor GetDefaultEvent(Type componentType)
		{
			if (componentType == null)
			{
				return null;
			}
			return TypeDescriptor.GetDescriptor(componentType, "componentType").GetDefaultEvent();
		}

		/// <summary>Returns the default event for the specified component.</summary>
		/// <param name="component">The component to get the event for.</param>
		/// <returns>An <see cref="T:System.ComponentModel.EventDescriptor" /> with the default event, or <see langword="null" /> if there are no events.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="component" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		public static EventDescriptor GetDefaultEvent(object component)
		{
			return TypeDescriptor.GetDefaultEvent(component, false);
		}

		/// <summary>Returns the default event for a component with a custom type descriptor.</summary>
		/// <param name="component">The component to get the event for.</param>
		/// <param name="noCustomTypeDesc">
		///   <see langword="true" /> to consider custom type description information; otherwise, <see langword="false" />.</param>
		/// <returns>An <see cref="T:System.ComponentModel.EventDescriptor" /> with the default event, or <see langword="null" /> if there are no events.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="component" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static EventDescriptor GetDefaultEvent(object component, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				return null;
			}
			return TypeDescriptor.GetDescriptor(component, noCustomTypeDesc).GetDefaultEvent();
		}

		/// <summary>Returns the default property for the specified type of component.</summary>
		/// <param name="componentType">A <see cref="T:System.Type" /> that represents the class to get the property for.</param>
		/// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptor" /> with the default property, or <see langword="null" /> if there are no properties.</returns>
		public static PropertyDescriptor GetDefaultProperty(Type componentType)
		{
			if (componentType == null)
			{
				return null;
			}
			return TypeDescriptor.GetDescriptor(componentType, "componentType").GetDefaultProperty();
		}

		/// <summary>Returns the default property for the specified component.</summary>
		/// <param name="component">The component to get the default property for.</param>
		/// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptor" /> with the default property, or <see langword="null" /> if there are no properties.</returns>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		public static PropertyDescriptor GetDefaultProperty(object component)
		{
			return TypeDescriptor.GetDefaultProperty(component, false);
		}

		/// <summary>Returns the default property for the specified component with a custom type descriptor.</summary>
		/// <param name="component">The component to get the default property for.</param>
		/// <param name="noCustomTypeDesc">
		///   <see langword="true" /> to consider custom type description information; otherwise, <see langword="false" />.</param>
		/// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptor" /> with the default property, or <see langword="null" /> if there are no properties.</returns>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static PropertyDescriptor GetDefaultProperty(object component, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				return null;
			}
			return TypeDescriptor.GetDescriptor(component, noCustomTypeDesc).GetDefaultProperty();
		}

		internal static ICustomTypeDescriptor GetDescriptor(Type type, string typeName)
		{
			if (type == null)
			{
				throw new ArgumentNullException(typeName);
			}
			return TypeDescriptor.NodeFor(type).GetTypeDescriptor(type);
		}

		internal static ICustomTypeDescriptor GetDescriptor(object component, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				throw new ArgumentException("component");
			}
			if (component is TypeDescriptor.IUnimplemented)
			{
				throw new NotSupportedException(SR.GetString("The object {0} is being remoted by a proxy that does not support interface discovery.  This type of remoted object is not supported.", new object[]
				{
					component.GetType().FullName
				}));
			}
			ICustomTypeDescriptor customTypeDescriptor = TypeDescriptor.NodeFor(component).GetTypeDescriptor(component);
			ICustomTypeDescriptor customTypeDescriptor2 = component as ICustomTypeDescriptor;
			if (!noCustomTypeDesc && customTypeDescriptor2 != null)
			{
				customTypeDescriptor = new TypeDescriptor.MergedTypeDescriptor(customTypeDescriptor2, customTypeDescriptor);
			}
			return customTypeDescriptor;
		}

		internal static ICustomTypeDescriptor GetExtendedDescriptor(object component)
		{
			if (component == null)
			{
				throw new ArgumentException("component");
			}
			return TypeDescriptor.NodeFor(component).GetExtendedTypeDescriptor(component);
		}

		/// <summary>Gets an editor with the specified base type for the specified component.</summary>
		/// <param name="component">The component to get the editor for.</param>
		/// <param name="editorBaseType">A <see cref="T:System.Type" /> that represents the base type of the editor you want to find.</param>
		/// <returns>An instance of the editor that can be cast to the specified editor type, or <see langword="null" /> if no editor of the requested type can be found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="component" /> or <paramref name="editorBaseType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		public static object GetEditor(object component, Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(component, editorBaseType, false);
		}

		/// <summary>Returns an editor with the specified base type and with a custom type descriptor for the specified component.</summary>
		/// <param name="component">The component to get the editor for.</param>
		/// <param name="editorBaseType">A <see cref="T:System.Type" /> that represents the base type of the editor you want to find.</param>
		/// <param name="noCustomTypeDesc">A flag indicating whether custom type description information should be considered.</param>
		/// <returns>An instance of the editor that can be cast to the specified editor type, or <see langword="null" /> if no editor of the requested type can be found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="component" /> or <paramref name="editorBaseType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static object GetEditor(object component, Type editorBaseType, bool noCustomTypeDesc)
		{
			if (editorBaseType == null)
			{
				throw new ArgumentNullException("editorBaseType");
			}
			return TypeDescriptor.GetDescriptor(component, noCustomTypeDesc).GetEditor(editorBaseType);
		}

		/// <summary>Returns an editor with the specified base type for the specified type.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> of the target component.</param>
		/// <param name="editorBaseType">A <see cref="T:System.Type" /> that represents the base type of the editor you are trying to find.</param>
		/// <returns>An instance of the editor object that can be cast to the given base type, or <see langword="null" /> if no editor of the requested type can be found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="type" /> or <paramref name="editorBaseType" /> is <see langword="null" />.</exception>
		public static object GetEditor(Type type, Type editorBaseType)
		{
			if (editorBaseType == null)
			{
				throw new ArgumentNullException("editorBaseType");
			}
			return TypeDescriptor.GetDescriptor(type, "type").GetEditor(editorBaseType);
		}

		/// <summary>Returns the collection of events for a specified type of component.</summary>
		/// <param name="componentType">The <see cref="T:System.Type" /> of the target component.</param>
		/// <returns>An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> with the events for this component.</returns>
		public static EventDescriptorCollection GetEvents(Type componentType)
		{
			if (componentType == null)
			{
				return new EventDescriptorCollection(null, true);
			}
			return TypeDescriptor.GetDescriptor(componentType, "componentType").GetEvents();
		}

		/// <summary>Returns the collection of events for a specified type of component using a specified array of attributes as a filter.</summary>
		/// <param name="componentType">The <see cref="T:System.Type" /> of the target component.</param>
		/// <param name="attributes">An array of type <see cref="T:System.Attribute" /> that you can use as a filter.</param>
		/// <returns>An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> with the events that match the specified attributes for this component.</returns>
		public static EventDescriptorCollection GetEvents(Type componentType, Attribute[] attributes)
		{
			if (componentType == null)
			{
				return new EventDescriptorCollection(null, true);
			}
			EventDescriptorCollection eventDescriptorCollection = TypeDescriptor.GetDescriptor(componentType, "componentType").GetEvents(attributes);
			if (attributes != null && attributes.Length != 0)
			{
				ArrayList arrayList = TypeDescriptor.FilterMembers(eventDescriptorCollection, attributes);
				if (arrayList != null)
				{
					eventDescriptorCollection = new EventDescriptorCollection((EventDescriptor[])arrayList.ToArray(typeof(EventDescriptor)), true);
				}
			}
			return eventDescriptorCollection;
		}

		/// <summary>Returns the collection of events for the specified component.</summary>
		/// <param name="component">A component to get the events for.</param>
		/// <returns>An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> with the events for this component.</returns>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		public static EventDescriptorCollection GetEvents(object component)
		{
			return TypeDescriptor.GetEvents(component, null, false);
		}

		/// <summary>Returns the collection of events for a specified component with a custom type descriptor.</summary>
		/// <param name="component">A component to get the events for.</param>
		/// <param name="noCustomTypeDesc">
		///   <see langword="true" /> to consider custom type description information; otherwise, <see langword="false" />.</param>
		/// <returns>An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> with the events for this component.</returns>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static EventDescriptorCollection GetEvents(object component, bool noCustomTypeDesc)
		{
			return TypeDescriptor.GetEvents(component, null, noCustomTypeDesc);
		}

		/// <summary>Returns the collection of events for a specified component using a specified array of attributes as a filter.</summary>
		/// <param name="component">A component to get the events for.</param>
		/// <param name="attributes">An array of type <see cref="T:System.Attribute" /> that you can use as a filter.</param>
		/// <returns>An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> with the events that match the specified attributes for this component.</returns>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		public static EventDescriptorCollection GetEvents(object component, Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(component, attributes, false);
		}

		/// <summary>Returns the collection of events for a specified component using a specified array of attributes as a filter and using a custom type descriptor.</summary>
		/// <param name="component">A component to get the events for.</param>
		/// <param name="attributes">An array of type <see cref="T:System.Attribute" /> to use as a filter.</param>
		/// <param name="noCustomTypeDesc">
		///   <see langword="true" /> to consider custom type description information; otherwise, <see langword="false" />.</param>
		/// <returns>An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> with the events that match the specified attributes for this component.</returns>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static EventDescriptorCollection GetEvents(object component, Attribute[] attributes, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				return new EventDescriptorCollection(null, true);
			}
			ICustomTypeDescriptor descriptor = TypeDescriptor.GetDescriptor(component, noCustomTypeDesc);
			ICollection collection;
			if (component is ICustomTypeDescriptor)
			{
				collection = descriptor.GetEvents(attributes);
				if (noCustomTypeDesc)
				{
					ICustomTypeDescriptor extendedDescriptor = TypeDescriptor.GetExtendedDescriptor(component);
					if (extendedDescriptor != null)
					{
						ICollection events = extendedDescriptor.GetEvents(attributes);
						collection = TypeDescriptor.PipelineMerge(2, collection, events, component, null);
					}
				}
				else
				{
					collection = TypeDescriptor.PipelineFilter(2, collection, component, null);
					collection = TypeDescriptor.PipelineAttributeFilter(2, collection, attributes, component, null);
				}
			}
			else
			{
				IDictionary cache = TypeDescriptor.GetCache(component);
				collection = descriptor.GetEvents(attributes);
				collection = TypeDescriptor.PipelineInitialize(2, collection, cache);
				ICustomTypeDescriptor extendedDescriptor2 = TypeDescriptor.GetExtendedDescriptor(component);
				if (extendedDescriptor2 != null)
				{
					ICollection events2 = extendedDescriptor2.GetEvents(attributes);
					collection = TypeDescriptor.PipelineMerge(2, collection, events2, component, cache);
				}
				collection = TypeDescriptor.PipelineFilter(2, collection, component, cache);
				collection = TypeDescriptor.PipelineAttributeFilter(2, collection, attributes, component, cache);
			}
			EventDescriptorCollection eventDescriptorCollection = collection as EventDescriptorCollection;
			if (eventDescriptorCollection == null)
			{
				EventDescriptor[] array = new EventDescriptor[collection.Count];
				collection.CopyTo(array, 0);
				eventDescriptorCollection = new EventDescriptorCollection(array, true);
			}
			return eventDescriptorCollection;
		}

		private static string GetExtenderCollisionSuffix(MemberDescriptor member)
		{
			string result = null;
			ExtenderProvidedPropertyAttribute extenderProvidedPropertyAttribute = member.Attributes[typeof(ExtenderProvidedPropertyAttribute)] as ExtenderProvidedPropertyAttribute;
			if (extenderProvidedPropertyAttribute != null)
			{
				IExtenderProvider provider = extenderProvidedPropertyAttribute.Provider;
				if (provider != null)
				{
					string text = null;
					IComponent component = provider as IComponent;
					if (component != null && component.Site != null)
					{
						text = component.Site.Name;
					}
					if (text == null || text.Length == 0)
					{
						text = (Interlocked.Increment(ref TypeDescriptor._collisionIndex) - 1).ToString(CultureInfo.InvariantCulture);
					}
					result = string.Format(CultureInfo.InvariantCulture, "_{0}", text);
				}
			}
			return result;
		}

		/// <summary>Returns the fully qualified name of the component.</summary>
		/// <param name="component">The <see cref="T:System.ComponentModel.Component" /> to find the name for.</param>
		/// <returns>The fully qualified name of the specified component, or <see langword="null" /> if the component has no name.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="component" /> is <see langword="null" />.</exception>
		public static string GetFullComponentName(object component)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component");
			}
			return TypeDescriptor.GetProvider(component).GetFullComponentName(component);
		}

		private static Type GetNodeForBaseType(Type searchType)
		{
			if (searchType.IsInterface)
			{
				return TypeDescriptor.InterfaceType;
			}
			if (searchType == TypeDescriptor.InterfaceType)
			{
				return null;
			}
			return searchType.BaseType;
		}

		/// <summary>Returns the collection of properties for a specified type of component.</summary>
		/// <param name="componentType">A <see cref="T:System.Type" /> that represents the component to get properties for.</param>
		/// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> with the properties for a specified type of component.</returns>
		public static PropertyDescriptorCollection GetProperties(Type componentType)
		{
			if (componentType == null)
			{
				return new PropertyDescriptorCollection(null, true);
			}
			return TypeDescriptor.GetDescriptor(componentType, "componentType").GetProperties();
		}

		/// <summary>Returns the collection of properties for a specified type of component using a specified array of attributes as a filter.</summary>
		/// <param name="componentType">The <see cref="T:System.Type" /> of the target component.</param>
		/// <param name="attributes">An array of type <see cref="T:System.Attribute" /> to use as a filter.</param>
		/// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> with the properties that match the specified attributes for this type of component.</returns>
		public static PropertyDescriptorCollection GetProperties(Type componentType, Attribute[] attributes)
		{
			if (componentType == null)
			{
				return new PropertyDescriptorCollection(null, true);
			}
			PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetDescriptor(componentType, "componentType").GetProperties(attributes);
			if (attributes != null && attributes.Length != 0)
			{
				ArrayList arrayList = TypeDescriptor.FilterMembers(propertyDescriptorCollection, attributes);
				if (arrayList != null)
				{
					propertyDescriptorCollection = new PropertyDescriptorCollection((PropertyDescriptor[])arrayList.ToArray(typeof(PropertyDescriptor)), true);
				}
			}
			return propertyDescriptorCollection;
		}

		/// <summary>Returns the collection of properties for a specified component.</summary>
		/// <param name="component">A component to get the properties for.</param>
		/// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> with the properties for the specified component.</returns>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		public static PropertyDescriptorCollection GetProperties(object component)
		{
			return TypeDescriptor.GetProperties(component, false);
		}

		/// <summary>Returns the collection of properties for a specified component using the default type descriptor.</summary>
		/// <param name="component">A component to get the properties for.</param>
		/// <param name="noCustomTypeDesc">
		///   <see langword="true" /> to not consider custom type description information; otherwise, <see langword="false" />.</param>
		/// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> with the properties for a specified component.</returns>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static PropertyDescriptorCollection GetProperties(object component, bool noCustomTypeDesc)
		{
			return TypeDescriptor.GetPropertiesImpl(component, null, noCustomTypeDesc, true);
		}

		/// <summary>Returns the collection of properties for a specified component using a specified array of attributes as a filter.</summary>
		/// <param name="component">A component to get the properties for.</param>
		/// <param name="attributes">An array of type <see cref="T:System.Attribute" /> to use as a filter.</param>
		/// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> with the properties that match the specified attributes for the specified component.</returns>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		public static PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(component, attributes, false);
		}

		/// <summary>Returns the collection of properties for a specified component using a specified array of attributes as a filter and using a custom type descriptor.</summary>
		/// <param name="component">A component to get the properties for.</param>
		/// <param name="attributes">An array of type <see cref="T:System.Attribute" /> to use as a filter.</param>
		/// <param name="noCustomTypeDesc">
		///   <see langword="true" /> to consider custom type description information; otherwise, <see langword="false" />.</param>
		/// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> with the events that match the specified attributes for the specified component.</returns>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="component" /> is a cross-process remoted object.</exception>
		public static PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes, bool noCustomTypeDesc)
		{
			return TypeDescriptor.GetPropertiesImpl(component, attributes, noCustomTypeDesc, false);
		}

		private static PropertyDescriptorCollection GetPropertiesImpl(object component, Attribute[] attributes, bool noCustomTypeDesc, bool noAttributes)
		{
			if (component == null)
			{
				return new PropertyDescriptorCollection(null, true);
			}
			ICustomTypeDescriptor descriptor = TypeDescriptor.GetDescriptor(component, noCustomTypeDesc);
			ICollection collection;
			if (component is ICustomTypeDescriptor)
			{
				collection = (noAttributes ? descriptor.GetProperties() : descriptor.GetProperties(attributes));
				if (noCustomTypeDesc)
				{
					ICustomTypeDescriptor extendedDescriptor = TypeDescriptor.GetExtendedDescriptor(component);
					if (extendedDescriptor != null)
					{
						ICollection secondary = noAttributes ? extendedDescriptor.GetProperties() : extendedDescriptor.GetProperties(attributes);
						collection = TypeDescriptor.PipelineMerge(1, collection, secondary, component, null);
					}
				}
				else
				{
					collection = TypeDescriptor.PipelineFilter(1, collection, component, null);
					collection = TypeDescriptor.PipelineAttributeFilter(1, collection, attributes, component, null);
				}
			}
			else
			{
				IDictionary cache = TypeDescriptor.GetCache(component);
				collection = (noAttributes ? descriptor.GetProperties() : descriptor.GetProperties(attributes));
				collection = TypeDescriptor.PipelineInitialize(1, collection, cache);
				ICustomTypeDescriptor extendedDescriptor2 = TypeDescriptor.GetExtendedDescriptor(component);
				if (extendedDescriptor2 != null)
				{
					ICollection secondary2 = noAttributes ? extendedDescriptor2.GetProperties() : extendedDescriptor2.GetProperties(attributes);
					collection = TypeDescriptor.PipelineMerge(1, collection, secondary2, component, cache);
				}
				collection = TypeDescriptor.PipelineFilter(1, collection, component, cache);
				collection = TypeDescriptor.PipelineAttributeFilter(1, collection, attributes, component, cache);
			}
			PropertyDescriptorCollection propertyDescriptorCollection = collection as PropertyDescriptorCollection;
			if (propertyDescriptorCollection == null)
			{
				PropertyDescriptor[] array = new PropertyDescriptor[collection.Count];
				collection.CopyTo(array, 0);
				propertyDescriptorCollection = new PropertyDescriptorCollection(array, true);
			}
			return propertyDescriptorCollection;
		}

		/// <summary>Returns the type description provider for the specified type.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> of the target component.</param>
		/// <returns>A <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> associated with the specified type.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="type" /> is <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static TypeDescriptionProvider GetProvider(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return TypeDescriptor.NodeFor(type, true);
		}

		/// <summary>Returns the type description provider for the specified component.</summary>
		/// <param name="instance">An instance of the target component.</param>
		/// <returns>A <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> associated with the specified component.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="instance" /> is <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static TypeDescriptionProvider GetProvider(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return TypeDescriptor.NodeFor(instance, true);
		}

		internal static TypeDescriptionProvider GetProviderRecursive(Type type)
		{
			return TypeDescriptor.NodeFor(type, false);
		}

		/// <summary>Returns a <see cref="T:System.Type" /> that can be used to perform reflection, given a class type.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> of the target component.</param>
		/// <returns>A <see cref="T:System.Type" /> of the specified class.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="type" /> is <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static Type GetReflectionType(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return TypeDescriptor.NodeFor(type).GetReflectionType(type);
		}

		/// <summary>Returns a <see cref="T:System.Type" /> that can be used to perform reflection, given an object.</summary>
		/// <param name="instance">An instance of the target component.</param>
		/// <returns>A <see cref="T:System.Type" /> for the specified object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="instance" /> is <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static Type GetReflectionType(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return TypeDescriptor.NodeFor(instance).GetReflectionType(instance);
		}

		private static TypeDescriptor.TypeDescriptionNode NodeFor(Type type)
		{
			return TypeDescriptor.NodeFor(type, false);
		}

		private static TypeDescriptor.TypeDescriptionNode NodeFor(Type type, bool createDelegator)
		{
			TypeDescriptor.CheckDefaultProvider(type);
			TypeDescriptor.TypeDescriptionNode typeDescriptionNode = null;
			Type type2 = type;
			while (typeDescriptionNode == null)
			{
				typeDescriptionNode = (TypeDescriptor.TypeDescriptionNode)TypeDescriptor._providerTypeTable[type2];
				if (typeDescriptionNode == null)
				{
					typeDescriptionNode = (TypeDescriptor.TypeDescriptionNode)TypeDescriptor._providerTable[type2];
				}
				if (typeDescriptionNode == null)
				{
					Type nodeForBaseType = TypeDescriptor.GetNodeForBaseType(type2);
					if (type2 == typeof(object) || nodeForBaseType == null)
					{
						WeakHashtable providerTable = TypeDescriptor._providerTable;
						lock (providerTable)
						{
							typeDescriptionNode = (TypeDescriptor.TypeDescriptionNode)TypeDescriptor._providerTable[type2];
							if (typeDescriptionNode == null)
							{
								typeDescriptionNode = new TypeDescriptor.TypeDescriptionNode(new ReflectTypeDescriptionProvider());
								TypeDescriptor._providerTable[type2] = typeDescriptionNode;
							}
							continue;
						}
					}
					if (createDelegator)
					{
						typeDescriptionNode = new TypeDescriptor.TypeDescriptionNode(new DelegatingTypeDescriptionProvider(nodeForBaseType));
						WeakHashtable providerTable = TypeDescriptor._providerTable;
						lock (providerTable)
						{
							TypeDescriptor._providerTypeTable[type2] = typeDescriptionNode;
							continue;
						}
					}
					type2 = nodeForBaseType;
				}
			}
			return typeDescriptionNode;
		}

		private static TypeDescriptor.TypeDescriptionNode NodeFor(object instance)
		{
			return TypeDescriptor.NodeFor(instance, false);
		}

		private static TypeDescriptor.TypeDescriptionNode NodeFor(object instance, bool createDelegator)
		{
			TypeDescriptor.TypeDescriptionNode typeDescriptionNode = (TypeDescriptor.TypeDescriptionNode)TypeDescriptor._providerTable[instance];
			if (typeDescriptionNode == null)
			{
				Type type = instance.GetType();
				if (type.IsCOMObject)
				{
					type = TypeDescriptor.ComObjectType;
				}
				if (createDelegator)
				{
					typeDescriptionNode = new TypeDescriptor.TypeDescriptionNode(new DelegatingTypeDescriptionProvider(type));
				}
				else
				{
					typeDescriptionNode = TypeDescriptor.NodeFor(type);
				}
			}
			return typeDescriptionNode;
		}

		private static void NodeRemove(object key, TypeDescriptionProvider provider)
		{
			WeakHashtable providerTable = TypeDescriptor._providerTable;
			lock (providerTable)
			{
				TypeDescriptor.TypeDescriptionNode typeDescriptionNode = (TypeDescriptor.TypeDescriptionNode)TypeDescriptor._providerTable[key];
				TypeDescriptor.TypeDescriptionNode typeDescriptionNode2 = typeDescriptionNode;
				while (typeDescriptionNode2 != null && typeDescriptionNode2.Provider != provider)
				{
					typeDescriptionNode2 = typeDescriptionNode2.Next;
				}
				if (typeDescriptionNode2 != null)
				{
					if (typeDescriptionNode2.Next != null)
					{
						typeDescriptionNode2.Provider = typeDescriptionNode2.Next.Provider;
						typeDescriptionNode2.Next = typeDescriptionNode2.Next.Next;
						if (typeDescriptionNode2 == typeDescriptionNode && typeDescriptionNode2.Provider is DelegatingTypeDescriptionProvider)
						{
							TypeDescriptor._providerTable.Remove(key);
						}
					}
					else if (typeDescriptionNode2 != typeDescriptionNode)
					{
						Type type = key as Type;
						if (type == null)
						{
							type = key.GetType();
						}
						typeDescriptionNode2.Provider = new DelegatingTypeDescriptionProvider(type.BaseType);
					}
					else
					{
						TypeDescriptor._providerTable.Remove(key);
					}
					TypeDescriptor._providerTypeTable.Clear();
				}
			}
		}

		private static ICollection PipelineAttributeFilter(int pipelineType, ICollection members, Attribute[] filter, object instance, IDictionary cache)
		{
			IList list = members as ArrayList;
			if (filter == null || filter.Length == 0)
			{
				return members;
			}
			if (cache != null && (list == null || list.IsReadOnly))
			{
				TypeDescriptor.AttributeFilterCacheItem attributeFilterCacheItem = cache[TypeDescriptor._pipelineAttributeFilterKeys[pipelineType]] as TypeDescriptor.AttributeFilterCacheItem;
				if (attributeFilterCacheItem != null && attributeFilterCacheItem.IsValid(filter))
				{
					return attributeFilterCacheItem.FilteredMembers;
				}
			}
			if (list == null || list.IsReadOnly)
			{
				list = new ArrayList(members);
			}
			ArrayList arrayList = TypeDescriptor.FilterMembers(list, filter);
			if (arrayList != null)
			{
				list = arrayList;
			}
			if (cache != null)
			{
				ICollection filteredMembers;
				if (pipelineType != 1)
				{
					if (pipelineType != 2)
					{
						filteredMembers = null;
					}
					else
					{
						EventDescriptor[] array = new EventDescriptor[list.Count];
						list.CopyTo(array, 0);
						filteredMembers = new EventDescriptorCollection(array, true);
					}
				}
				else
				{
					PropertyDescriptor[] array2 = new PropertyDescriptor[list.Count];
					list.CopyTo(array2, 0);
					filteredMembers = new PropertyDescriptorCollection(array2, true);
				}
				TypeDescriptor.AttributeFilterCacheItem value = new TypeDescriptor.AttributeFilterCacheItem(filter, filteredMembers);
				cache[TypeDescriptor._pipelineAttributeFilterKeys[pipelineType]] = value;
			}
			return list;
		}

		private static ICollection PipelineFilter(int pipelineType, ICollection members, object instance, IDictionary cache)
		{
			IComponent component = instance as IComponent;
			ITypeDescriptorFilterService typeDescriptorFilterService = null;
			if (component != null)
			{
				ISite site = component.Site;
				if (site != null)
				{
					typeDescriptorFilterService = (site.GetService(typeof(ITypeDescriptorFilterService)) as ITypeDescriptorFilterService);
				}
			}
			IList list = members as ArrayList;
			if (typeDescriptorFilterService == null)
			{
				return members;
			}
			if (cache != null && (list == null || list.IsReadOnly))
			{
				TypeDescriptor.FilterCacheItem filterCacheItem = cache[TypeDescriptor._pipelineFilterKeys[pipelineType]] as TypeDescriptor.FilterCacheItem;
				if (filterCacheItem != null && filterCacheItem.IsValid(typeDescriptorFilterService))
				{
					return filterCacheItem.FilteredMembers;
				}
			}
			OrderedDictionary orderedDictionary = new OrderedDictionary(members.Count);
			bool flag;
			if (pipelineType != 0)
			{
				if (pipelineType - 1 > 1)
				{
					flag = false;
				}
				else
				{
					foreach (object obj in members)
					{
						MemberDescriptor memberDescriptor = (MemberDescriptor)obj;
						string name = memberDescriptor.Name;
						if (orderedDictionary.Contains(name))
						{
							string extenderCollisionSuffix = TypeDescriptor.GetExtenderCollisionSuffix(memberDescriptor);
							if (extenderCollisionSuffix != null)
							{
								orderedDictionary[name + extenderCollisionSuffix] = memberDescriptor;
							}
							MemberDescriptor memberDescriptor2 = (MemberDescriptor)orderedDictionary[name];
							extenderCollisionSuffix = TypeDescriptor.GetExtenderCollisionSuffix(memberDescriptor2);
							if (extenderCollisionSuffix != null)
							{
								orderedDictionary.Remove(name);
								orderedDictionary[memberDescriptor2.Name + extenderCollisionSuffix] = memberDescriptor2;
							}
						}
						else
						{
							orderedDictionary[name] = memberDescriptor;
						}
					}
					if (pipelineType == 1)
					{
						flag = typeDescriptorFilterService.FilterProperties(component, orderedDictionary);
					}
					else
					{
						flag = typeDescriptorFilterService.FilterEvents(component, orderedDictionary);
					}
				}
			}
			else
			{
				foreach (object obj2 in members)
				{
					Attribute attribute = (Attribute)obj2;
					orderedDictionary[attribute.TypeId] = attribute;
				}
				flag = typeDescriptorFilterService.FilterAttributes(component, orderedDictionary);
			}
			if (list == null || list.IsReadOnly)
			{
				list = new ArrayList(orderedDictionary.Values);
			}
			else
			{
				list.Clear();
				foreach (object value in orderedDictionary.Values)
				{
					list.Add(value);
				}
			}
			if (flag && cache != null)
			{
				ICollection filteredMembers;
				switch (pipelineType)
				{
				case 0:
				{
					Attribute[] array = new Attribute[list.Count];
					try
					{
						list.CopyTo(array, 0);
					}
					catch (InvalidCastException)
					{
						throw new ArgumentException(SR.GetString("Expected types in the collection to be of type {0}.", new object[]
						{
							typeof(Attribute).FullName
						}));
					}
					filteredMembers = new AttributeCollection(array);
					break;
				}
				case 1:
				{
					PropertyDescriptor[] array2 = new PropertyDescriptor[list.Count];
					try
					{
						list.CopyTo(array2, 0);
					}
					catch (InvalidCastException)
					{
						throw new ArgumentException(SR.GetString("Expected types in the collection to be of type {0}.", new object[]
						{
							typeof(PropertyDescriptor).FullName
						}));
					}
					filteredMembers = new PropertyDescriptorCollection(array2, true);
					break;
				}
				case 2:
				{
					EventDescriptor[] array3 = new EventDescriptor[list.Count];
					try
					{
						list.CopyTo(array3, 0);
					}
					catch (InvalidCastException)
					{
						throw new ArgumentException(SR.GetString("Expected types in the collection to be of type {0}.", new object[]
						{
							typeof(EventDescriptor).FullName
						}));
					}
					filteredMembers = new EventDescriptorCollection(array3, true);
					break;
				}
				default:
					filteredMembers = null;
					break;
				}
				TypeDescriptor.FilterCacheItem value2 = new TypeDescriptor.FilterCacheItem(typeDescriptorFilterService, filteredMembers);
				cache[TypeDescriptor._pipelineFilterKeys[pipelineType]] = value2;
				cache.Remove(TypeDescriptor._pipelineAttributeFilterKeys[pipelineType]);
			}
			return list;
		}

		private static ICollection PipelineInitialize(int pipelineType, ICollection members, IDictionary cache)
		{
			if (cache != null)
			{
				bool flag = true;
				ICollection collection = cache[TypeDescriptor._pipelineInitializeKeys[pipelineType]] as ICollection;
				if (collection != null && collection.Count == members.Count)
				{
					IEnumerator enumerator = collection.GetEnumerator();
					IEnumerator enumerator2 = members.GetEnumerator();
					while (enumerator.MoveNext() && enumerator2.MoveNext())
					{
						if (enumerator.Current != enumerator2.Current)
						{
							flag = false;
							break;
						}
					}
				}
				if (!flag)
				{
					cache.Remove(TypeDescriptor._pipelineMergeKeys[pipelineType]);
					cache.Remove(TypeDescriptor._pipelineFilterKeys[pipelineType]);
					cache.Remove(TypeDescriptor._pipelineAttributeFilterKeys[pipelineType]);
					cache[TypeDescriptor._pipelineInitializeKeys[pipelineType]] = members;
				}
			}
			return members;
		}

		private static ICollection PipelineMerge(int pipelineType, ICollection primary, ICollection secondary, object instance, IDictionary cache)
		{
			if (secondary == null || secondary.Count == 0)
			{
				return primary;
			}
			if (cache != null)
			{
				ICollection collection = cache[TypeDescriptor._pipelineMergeKeys[pipelineType]] as ICollection;
				if (collection != null && collection.Count == primary.Count + secondary.Count)
				{
					IEnumerator enumerator = collection.GetEnumerator();
					IEnumerator enumerator2 = primary.GetEnumerator();
					bool flag = true;
					while (enumerator2.MoveNext() && enumerator.MoveNext())
					{
						if (enumerator2.Current != enumerator.Current)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						IEnumerator enumerator3 = secondary.GetEnumerator();
						while (enumerator3.MoveNext() && enumerator.MoveNext())
						{
							if (enumerator3.Current != enumerator.Current)
							{
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						return collection;
					}
				}
			}
			ArrayList arrayList = new ArrayList(primary.Count + secondary.Count);
			foreach (object value in primary)
			{
				arrayList.Add(value);
			}
			foreach (object value2 in secondary)
			{
				arrayList.Add(value2);
			}
			if (cache != null)
			{
				ICollection value3;
				switch (pipelineType)
				{
				case 0:
				{
					Attribute[] array = new Attribute[arrayList.Count];
					arrayList.CopyTo(array, 0);
					value3 = new AttributeCollection(array);
					break;
				}
				case 1:
				{
					PropertyDescriptor[] array2 = new PropertyDescriptor[arrayList.Count];
					arrayList.CopyTo(array2, 0);
					value3 = new PropertyDescriptorCollection(array2, true);
					break;
				}
				case 2:
				{
					EventDescriptor[] array3 = new EventDescriptor[arrayList.Count];
					arrayList.CopyTo(array3, 0);
					value3 = new EventDescriptorCollection(array3, true);
					break;
				}
				default:
					value3 = null;
					break;
				}
				cache[TypeDescriptor._pipelineMergeKeys[pipelineType]] = value3;
				cache.Remove(TypeDescriptor._pipelineFilterKeys[pipelineType]);
				cache.Remove(TypeDescriptor._pipelineAttributeFilterKeys[pipelineType]);
			}
			return arrayList;
		}

		private static void RaiseRefresh(object component)
		{
			RefreshEventHandler refreshEventHandler = Volatile.Read<RefreshEventHandler>(ref TypeDescriptor.Refreshed);
			if (refreshEventHandler != null)
			{
				refreshEventHandler(new RefreshEventArgs(component));
			}
		}

		private static void RaiseRefresh(Type type)
		{
			RefreshEventHandler refreshEventHandler = Volatile.Read<RefreshEventHandler>(ref TypeDescriptor.Refreshed);
			if (refreshEventHandler != null)
			{
				refreshEventHandler(new RefreshEventArgs(type));
			}
		}

		/// <summary>Clears the properties and events for the specified component from the cache.</summary>
		/// <param name="component">A component for which the properties or events have changed.</param>
		public static void Refresh(object component)
		{
			TypeDescriptor.Refresh(component, true);
		}

		private static void Refresh(object component, bool refreshReflectionProvider)
		{
			if (component == null)
			{
				return;
			}
			bool flag = false;
			if (refreshReflectionProvider)
			{
				Type type = component.GetType();
				WeakHashtable providerTable = TypeDescriptor._providerTable;
				lock (providerTable)
				{
					foreach (object obj in TypeDescriptor._providerTable)
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
						Type type2 = dictionaryEntry.Key as Type;
						if ((type2 != null && type.IsAssignableFrom(type2)) || type2 == typeof(object))
						{
							TypeDescriptor.TypeDescriptionNode typeDescriptionNode = (TypeDescriptor.TypeDescriptionNode)dictionaryEntry.Value;
							while (typeDescriptionNode != null && !(typeDescriptionNode.Provider is ReflectTypeDescriptionProvider))
							{
								flag = true;
								typeDescriptionNode = typeDescriptionNode.Next;
							}
							if (typeDescriptionNode != null)
							{
								ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = (ReflectTypeDescriptionProvider)typeDescriptionNode.Provider;
								if (reflectTypeDescriptionProvider.IsPopulated(type))
								{
									flag = true;
									reflectTypeDescriptionProvider.Refresh(type);
								}
							}
						}
					}
				}
			}
			IDictionary cache = TypeDescriptor.GetCache(component);
			if (flag || cache != null)
			{
				if (cache != null)
				{
					for (int i = 0; i < TypeDescriptor._pipelineFilterKeys.Length; i++)
					{
						cache.Remove(TypeDescriptor._pipelineFilterKeys[i]);
						cache.Remove(TypeDescriptor._pipelineMergeKeys[i]);
						cache.Remove(TypeDescriptor._pipelineAttributeFilterKeys[i]);
					}
				}
				Interlocked.Increment(ref TypeDescriptor._metadataVersion);
				TypeDescriptor.RaiseRefresh(component);
			}
		}

		/// <summary>Clears the properties and events for the specified type of component from the cache.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> of the target component.</param>
		public static void Refresh(Type type)
		{
			if (type == null)
			{
				return;
			}
			bool flag = false;
			WeakHashtable providerTable = TypeDescriptor._providerTable;
			lock (providerTable)
			{
				foreach (object obj in TypeDescriptor._providerTable)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					Type type2 = dictionaryEntry.Key as Type;
					if ((type2 != null && type.IsAssignableFrom(type2)) || type2 == typeof(object))
					{
						TypeDescriptor.TypeDescriptionNode typeDescriptionNode = (TypeDescriptor.TypeDescriptionNode)dictionaryEntry.Value;
						while (typeDescriptionNode != null && !(typeDescriptionNode.Provider is ReflectTypeDescriptionProvider))
						{
							flag = true;
							typeDescriptionNode = typeDescriptionNode.Next;
						}
						if (typeDescriptionNode != null)
						{
							ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = (ReflectTypeDescriptionProvider)typeDescriptionNode.Provider;
							if (reflectTypeDescriptionProvider.IsPopulated(type))
							{
								flag = true;
								reflectTypeDescriptionProvider.Refresh(type);
							}
						}
					}
				}
			}
			if (flag)
			{
				Interlocked.Increment(ref TypeDescriptor._metadataVersion);
				TypeDescriptor.RaiseRefresh(type);
			}
		}

		/// <summary>Clears the properties and events for the specified module from the cache.</summary>
		/// <param name="module">The <see cref="T:System.Reflection.Module" /> that represents the module to refresh. Each <see cref="T:System.Type" /> in this module will be refreshed.</param>
		public static void Refresh(Module module)
		{
			if (module == null)
			{
				return;
			}
			Hashtable hashtable = null;
			WeakHashtable providerTable = TypeDescriptor._providerTable;
			lock (providerTable)
			{
				foreach (object obj in TypeDescriptor._providerTable)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					Type type = dictionaryEntry.Key as Type;
					if ((type != null && type.Module.Equals(module)) || type == typeof(object))
					{
						TypeDescriptor.TypeDescriptionNode typeDescriptionNode = (TypeDescriptor.TypeDescriptionNode)dictionaryEntry.Value;
						while (typeDescriptionNode != null && !(typeDescriptionNode.Provider is ReflectTypeDescriptionProvider))
						{
							if (hashtable == null)
							{
								hashtable = new Hashtable();
							}
							hashtable[type] = type;
							typeDescriptionNode = typeDescriptionNode.Next;
						}
						if (typeDescriptionNode != null)
						{
							ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = (ReflectTypeDescriptionProvider)typeDescriptionNode.Provider;
							foreach (Type type2 in reflectTypeDescriptionProvider.GetPopulatedTypes(module))
							{
								reflectTypeDescriptionProvider.Refresh(type2);
								if (hashtable == null)
								{
									hashtable = new Hashtable();
								}
								hashtable[type2] = type2;
							}
						}
					}
				}
			}
			if (hashtable != null && TypeDescriptor.Refreshed != null)
			{
				foreach (object obj2 in hashtable.Keys)
				{
					TypeDescriptor.RaiseRefresh((Type)obj2);
				}
			}
		}

		/// <summary>Clears the properties and events for the specified assembly from the cache.</summary>
		/// <param name="assembly">The <see cref="T:System.Reflection.Assembly" /> that represents the assembly to refresh. Each <see cref="T:System.Type" /> in this assembly will be refreshed.</param>
		public static void Refresh(Assembly assembly)
		{
			if (assembly == null)
			{
				return;
			}
			Module[] modules = assembly.GetModules();
			for (int i = 0; i < modules.Length; i++)
			{
				TypeDescriptor.Refresh(modules[i]);
			}
		}

		/// <summary>Removes an association between two objects.</summary>
		/// <param name="primary">The primary <see cref="T:System.Object" />.</param>
		/// <param name="secondary">The secondary <see cref="T:System.Object" />.</param>
		/// <exception cref="T:System.ArgumentNullException">One or both of the parameters are <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public static void RemoveAssociation(object primary, object secondary)
		{
			if (primary == null)
			{
				throw new ArgumentNullException("primary");
			}
			if (secondary == null)
			{
				throw new ArgumentNullException("secondary");
			}
			Hashtable associationTable = TypeDescriptor._associationTable;
			if (associationTable != null)
			{
				IList list = (IList)associationTable[primary];
				if (list != null)
				{
					IList obj = list;
					lock (obj)
					{
						for (int i = list.Count - 1; i >= 0; i--)
						{
							object target = ((WeakReference)list[i]).Target;
							if (target == null || target == secondary)
							{
								list.RemoveAt(i);
							}
						}
					}
				}
			}
		}

		/// <summary>Removes all associations for a primary object.</summary>
		/// <param name="primary">The primary <see cref="T:System.Object" /> in an association.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="primary" /> is <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public static void RemoveAssociations(object primary)
		{
			if (primary == null)
			{
				throw new ArgumentNullException("primary");
			}
			Hashtable associationTable = TypeDescriptor._associationTable;
			if (associationTable != null)
			{
				associationTable.Remove(primary);
			}
		}

		/// <summary>Removes a previously added type description provider that is associated with the specified type.</summary>
		/// <param name="provider">The <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> to remove.</param>
		/// <param name="type">The <see cref="T:System.Type" /> of the target component.</param>
		/// <exception cref="T:System.ArgumentNullException">One or both of the parameters are <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public static void RemoveProvider(TypeDescriptionProvider provider, Type type)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			TypeDescriptor.NodeRemove(type, provider);
			TypeDescriptor.RaiseRefresh(type);
		}

		/// <summary>Removes a previously added type description provider that is associated with the specified object.</summary>
		/// <param name="provider">The <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> to remove.</param>
		/// <param name="instance">An instance of the target component.</param>
		/// <exception cref="T:System.ArgumentNullException">One or both of the parameters are <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public static void RemoveProvider(TypeDescriptionProvider provider, object instance)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			TypeDescriptor.NodeRemove(instance, provider);
			TypeDescriptor.RaiseRefresh(instance);
		}

		/// <summary>Removes a previously added type description provider that is associated with the specified type.</summary>
		/// <param name="provider">The <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> to remove.</param>
		/// <param name="type">The <see cref="T:System.Type" /> of the target component.</param>
		/// <exception cref="T:System.ArgumentNullException">One or both of the parameters are <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void RemoveProviderTransparent(TypeDescriptionProvider provider, Type type)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			TypeDescriptor.RemoveProvider(provider, type);
		}

		/// <summary>Removes a previously added type description provider that is associated with the specified object.</summary>
		/// <param name="provider">The <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> to remove.</param>
		/// <param name="instance">An instance of the target component.</param>
		/// <exception cref="T:System.ArgumentNullException">One or both of the parameters are <see langword="null" />.</exception>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void RemoveProviderTransparent(TypeDescriptionProvider provider, object instance)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			TypeDescriptor.RemoveProvider(provider, instance);
		}

		private static bool ShouldHideMember(MemberDescriptor member, Attribute attribute)
		{
			if (member == null || attribute == null)
			{
				return true;
			}
			Attribute attribute2 = member.Attributes[attribute.GetType()];
			if (attribute2 == null)
			{
				return !attribute.IsDefaultAttribute();
			}
			return !attribute.Match(attribute2);
		}

		/// <summary>Sorts descriptors using the name of the descriptor.</summary>
		/// <param name="infos">An <see cref="T:System.Collections.IList" /> that contains the descriptors to sort.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="infos" /> is <see langword="null" />.</exception>
		public static void SortDescriptorArray(IList infos)
		{
			if (infos == null)
			{
				throw new ArgumentNullException("infos");
			}
			ArrayList.Adapter(infos).Sort(TypeDescriptor.MemberDescriptorComparer.Instance);
		}

		[Conditional("DEBUG")]
		internal static void Trace(string message, params object[] args)
		{
		}

		private static WeakHashtable _providerTable = new WeakHashtable();

		private static Hashtable _providerTypeTable = new Hashtable();

		private static volatile Hashtable _defaultProviders = new Hashtable();

		private static volatile WeakHashtable _associationTable;

		private static int _metadataVersion;

		private static int _collisionIndex;

		private static BooleanSwitch TraceDescriptor = new BooleanSwitch("TypeDescriptor", "Debug TypeDescriptor.");

		private const int PIPELINE_ATTRIBUTES = 0;

		private const int PIPELINE_PROPERTIES = 1;

		private const int PIPELINE_EVENTS = 2;

		private static readonly Guid[] _pipelineInitializeKeys = new Guid[]
		{
			Guid.NewGuid(),
			Guid.NewGuid(),
			Guid.NewGuid()
		};

		private static readonly Guid[] _pipelineMergeKeys = new Guid[]
		{
			Guid.NewGuid(),
			Guid.NewGuid(),
			Guid.NewGuid()
		};

		private static readonly Guid[] _pipelineFilterKeys = new Guid[]
		{
			Guid.NewGuid(),
			Guid.NewGuid(),
			Guid.NewGuid()
		};

		private static readonly Guid[] _pipelineAttributeFilterKeys = new Guid[]
		{
			Guid.NewGuid(),
			Guid.NewGuid(),
			Guid.NewGuid()
		};

		private static object _internalSyncObject = new object();

		private sealed class AttributeProvider : TypeDescriptionProvider
		{
			internal AttributeProvider(TypeDescriptionProvider existingProvider, params Attribute[] attrs) : base(existingProvider)
			{
				this._attrs = attrs;
			}

			public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
			{
				return new TypeDescriptor.AttributeProvider.AttributeTypeDescriptor(this._attrs, base.GetTypeDescriptor(objectType, instance));
			}

			private Attribute[] _attrs;

			private class AttributeTypeDescriptor : CustomTypeDescriptor
			{
				internal AttributeTypeDescriptor(Attribute[] attrs, ICustomTypeDescriptor parent) : base(parent)
				{
					this._attributeArray = attrs;
				}

				public override AttributeCollection GetAttributes()
				{
					AttributeCollection attributes = base.GetAttributes();
					Attribute[] attributeArray = this._attributeArray;
					Attribute[] array = new Attribute[attributes.Count + attributeArray.Length];
					int count = attributes.Count;
					attributes.CopyTo(array, 0);
					for (int i = 0; i < attributeArray.Length; i++)
					{
						bool flag = false;
						for (int j = 0; j < attributes.Count; j++)
						{
							if (array[j].TypeId.Equals(attributeArray[i].TypeId))
							{
								flag = true;
								array[j] = attributeArray[i];
								break;
							}
						}
						if (!flag)
						{
							array[count++] = attributeArray[i];
						}
					}
					Attribute[] array2;
					if (count < array.Length)
					{
						array2 = new Attribute[count];
						Array.Copy(array, 0, array2, 0, count);
					}
					else
					{
						array2 = array;
					}
					return new AttributeCollection(array2);
				}

				private Attribute[] _attributeArray;
			}
		}

		private sealed class ComNativeDescriptionProvider : TypeDescriptionProvider
		{
			internal ComNativeDescriptionProvider(IComNativeDescriptorHandler handler)
			{
				this._handler = handler;
			}

			internal IComNativeDescriptorHandler Handler
			{
				get
				{
					return this._handler;
				}
				set
				{
					this._handler = value;
				}
			}

			public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
			{
				if (objectType == null)
				{
					throw new ArgumentNullException("objectType");
				}
				if (instance == null)
				{
					return null;
				}
				if (!objectType.IsInstanceOfType(instance))
				{
					throw new ArgumentException("instance");
				}
				return new TypeDescriptor.ComNativeDescriptionProvider.ComNativeTypeDescriptor(this._handler, instance);
			}

			private IComNativeDescriptorHandler _handler;

			private sealed class ComNativeTypeDescriptor : ICustomTypeDescriptor
			{
				internal ComNativeTypeDescriptor(IComNativeDescriptorHandler handler, object instance)
				{
					this._handler = handler;
					this._instance = instance;
				}

				AttributeCollection ICustomTypeDescriptor.GetAttributes()
				{
					return this._handler.GetAttributes(this._instance);
				}

				string ICustomTypeDescriptor.GetClassName()
				{
					return this._handler.GetClassName(this._instance);
				}

				string ICustomTypeDescriptor.GetComponentName()
				{
					return null;
				}

				TypeConverter ICustomTypeDescriptor.GetConverter()
				{
					return this._handler.GetConverter(this._instance);
				}

				EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
				{
					return this._handler.GetDefaultEvent(this._instance);
				}

				PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
				{
					return this._handler.GetDefaultProperty(this._instance);
				}

				object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
				{
					return this._handler.GetEditor(this._instance, editorBaseType);
				}

				EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
				{
					return this._handler.GetEvents(this._instance);
				}

				EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
				{
					return this._handler.GetEvents(this._instance, attributes);
				}

				PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
				{
					return this._handler.GetProperties(this._instance, null);
				}

				PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
				{
					return this._handler.GetProperties(this._instance, attributes);
				}

				object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
				{
					return this._instance;
				}

				private IComNativeDescriptorHandler _handler;

				private object _instance;
			}
		}

		private sealed class AttributeFilterCacheItem
		{
			internal AttributeFilterCacheItem(Attribute[] filter, ICollection filteredMembers)
			{
				this._filter = filter;
				this.FilteredMembers = filteredMembers;
			}

			internal bool IsValid(Attribute[] filter)
			{
				if (this._filter.Length != filter.Length)
				{
					return false;
				}
				for (int i = 0; i < filter.Length; i++)
				{
					if (this._filter[i] != filter[i])
					{
						return false;
					}
				}
				return true;
			}

			private Attribute[] _filter;

			internal ICollection FilteredMembers;
		}

		private sealed class FilterCacheItem
		{
			internal FilterCacheItem(ITypeDescriptorFilterService filterService, ICollection filteredMembers)
			{
				this._filterService = filterService;
				this.FilteredMembers = filteredMembers;
			}

			internal bool IsValid(ITypeDescriptorFilterService filterService)
			{
				return this._filterService == filterService;
			}

			private ITypeDescriptorFilterService _filterService;

			internal ICollection FilteredMembers;
		}

		private interface IUnimplemented
		{
		}

		private sealed class MemberDescriptorComparer : IComparer
		{
			public int Compare(object left, object right)
			{
				return string.Compare(((MemberDescriptor)left).Name, ((MemberDescriptor)right).Name, false, CultureInfo.InvariantCulture);
			}

			public static readonly TypeDescriptor.MemberDescriptorComparer Instance = new TypeDescriptor.MemberDescriptorComparer();
		}

		private sealed class MergedTypeDescriptor : ICustomTypeDescriptor
		{
			internal MergedTypeDescriptor(ICustomTypeDescriptor primary, ICustomTypeDescriptor secondary)
			{
				this._primary = primary;
				this._secondary = secondary;
			}

			AttributeCollection ICustomTypeDescriptor.GetAttributes()
			{
				AttributeCollection attributes = this._primary.GetAttributes();
				if (attributes == null)
				{
					attributes = this._secondary.GetAttributes();
				}
				return attributes;
			}

			string ICustomTypeDescriptor.GetClassName()
			{
				string className = this._primary.GetClassName();
				if (className == null)
				{
					className = this._secondary.GetClassName();
				}
				return className;
			}

			string ICustomTypeDescriptor.GetComponentName()
			{
				string componentName = this._primary.GetComponentName();
				if (componentName == null)
				{
					componentName = this._secondary.GetComponentName();
				}
				return componentName;
			}

			TypeConverter ICustomTypeDescriptor.GetConverter()
			{
				TypeConverter converter = this._primary.GetConverter();
				if (converter == null)
				{
					converter = this._secondary.GetConverter();
				}
				return converter;
			}

			EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
			{
				EventDescriptor defaultEvent = this._primary.GetDefaultEvent();
				if (defaultEvent == null)
				{
					defaultEvent = this._secondary.GetDefaultEvent();
				}
				return defaultEvent;
			}

			PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
			{
				PropertyDescriptor defaultProperty = this._primary.GetDefaultProperty();
				if (defaultProperty == null)
				{
					defaultProperty = this._secondary.GetDefaultProperty();
				}
				return defaultProperty;
			}

			object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
			{
				if (editorBaseType == null)
				{
					throw new ArgumentNullException("editorBaseType");
				}
				object editor = this._primary.GetEditor(editorBaseType);
				if (editor == null)
				{
					editor = this._secondary.GetEditor(editorBaseType);
				}
				return editor;
			}

			EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
			{
				EventDescriptorCollection events = this._primary.GetEvents();
				if (events == null)
				{
					events = this._secondary.GetEvents();
				}
				return events;
			}

			EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
			{
				EventDescriptorCollection events = this._primary.GetEvents(attributes);
				if (events == null)
				{
					events = this._secondary.GetEvents(attributes);
				}
				return events;
			}

			PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
			{
				PropertyDescriptorCollection properties = this._primary.GetProperties();
				if (properties == null)
				{
					properties = this._secondary.GetProperties();
				}
				return properties;
			}

			PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
			{
				PropertyDescriptorCollection properties = this._primary.GetProperties(attributes);
				if (properties == null)
				{
					properties = this._secondary.GetProperties(attributes);
				}
				return properties;
			}

			object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
			{
				object propertyOwner = this._primary.GetPropertyOwner(pd);
				if (propertyOwner == null)
				{
					propertyOwner = this._secondary.GetPropertyOwner(pd);
				}
				return propertyOwner;
			}

			private ICustomTypeDescriptor _primary;

			private ICustomTypeDescriptor _secondary;
		}

		private sealed class TypeDescriptionNode : TypeDescriptionProvider
		{
			internal TypeDescriptionNode(TypeDescriptionProvider provider)
			{
				this.Provider = provider;
			}

			public override object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
			{
				if (objectType == null)
				{
					throw new ArgumentNullException("objectType");
				}
				if (argTypes != null)
				{
					if (args == null)
					{
						throw new ArgumentNullException("args");
					}
					if (argTypes.Length != args.Length)
					{
						throw new ArgumentException(SR.GetString("The number of elements in the Type and Object arrays must match."));
					}
				}
				return this.Provider.CreateInstance(provider, objectType, argTypes, args);
			}

			public override IDictionary GetCache(object instance)
			{
				if (instance == null)
				{
					throw new ArgumentNullException("instance");
				}
				return this.Provider.GetCache(instance);
			}

			public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
			{
				if (instance == null)
				{
					throw new ArgumentNullException("instance");
				}
				return new TypeDescriptor.TypeDescriptionNode.DefaultExtendedTypeDescriptor(this, instance);
			}

			protected internal override IExtenderProvider[] GetExtenderProviders(object instance)
			{
				if (instance == null)
				{
					throw new ArgumentNullException("instance");
				}
				return this.Provider.GetExtenderProviders(instance);
			}

			public override string GetFullComponentName(object component)
			{
				if (component == null)
				{
					throw new ArgumentNullException("component");
				}
				return this.Provider.GetFullComponentName(component);
			}

			public override Type GetReflectionType(Type objectType, object instance)
			{
				if (objectType == null)
				{
					throw new ArgumentNullException("objectType");
				}
				return this.Provider.GetReflectionType(objectType, instance);
			}

			public override Type GetRuntimeType(Type objectType)
			{
				if (objectType == null)
				{
					throw new ArgumentNullException("objectType");
				}
				return this.Provider.GetRuntimeType(objectType);
			}

			public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
			{
				if (objectType == null)
				{
					throw new ArgumentNullException("objectType");
				}
				if (instance != null && !objectType.IsInstanceOfType(instance))
				{
					throw new ArgumentException("instance");
				}
				return new TypeDescriptor.TypeDescriptionNode.DefaultTypeDescriptor(this, objectType, instance);
			}

			public override bool IsSupportedType(Type type)
			{
				if (type == null)
				{
					throw new ArgumentNullException("type");
				}
				return this.Provider.IsSupportedType(type);
			}

			internal TypeDescriptor.TypeDescriptionNode Next;

			internal TypeDescriptionProvider Provider;

			private struct DefaultExtendedTypeDescriptor : ICustomTypeDescriptor
			{
				internal DefaultExtendedTypeDescriptor(TypeDescriptor.TypeDescriptionNode node, object instance)
				{
					this._node = node;
					this._instance = instance;
				}

				AttributeCollection ICustomTypeDescriptor.GetAttributes()
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					if (reflectTypeDescriptionProvider != null)
					{
						return reflectTypeDescriptionProvider.GetExtendedAttributes(this._instance);
					}
					ICustomTypeDescriptor extendedTypeDescriptor = provider.GetExtendedTypeDescriptor(this._instance);
					if (extendedTypeDescriptor == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetExtendedTypeDescriptor"
						}));
					}
					AttributeCollection attributes = extendedTypeDescriptor.GetAttributes();
					if (attributes == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetAttributes"
						}));
					}
					return attributes;
				}

				string ICustomTypeDescriptor.GetClassName()
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					if (reflectTypeDescriptionProvider != null)
					{
						return reflectTypeDescriptionProvider.GetExtendedClassName(this._instance);
					}
					ICustomTypeDescriptor extendedTypeDescriptor = provider.GetExtendedTypeDescriptor(this._instance);
					if (extendedTypeDescriptor == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetExtendedTypeDescriptor"
						}));
					}
					string text = extendedTypeDescriptor.GetClassName();
					if (text == null)
					{
						text = this._instance.GetType().FullName;
					}
					return text;
				}

				string ICustomTypeDescriptor.GetComponentName()
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					if (reflectTypeDescriptionProvider != null)
					{
						return reflectTypeDescriptionProvider.GetExtendedComponentName(this._instance);
					}
					ICustomTypeDescriptor extendedTypeDescriptor = provider.GetExtendedTypeDescriptor(this._instance);
					if (extendedTypeDescriptor == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetExtendedTypeDescriptor"
						}));
					}
					return extendedTypeDescriptor.GetComponentName();
				}

				TypeConverter ICustomTypeDescriptor.GetConverter()
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					if (reflectTypeDescriptionProvider != null)
					{
						return reflectTypeDescriptionProvider.GetExtendedConverter(this._instance);
					}
					ICustomTypeDescriptor extendedTypeDescriptor = provider.GetExtendedTypeDescriptor(this._instance);
					if (extendedTypeDescriptor == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetExtendedTypeDescriptor"
						}));
					}
					TypeConverter converter = extendedTypeDescriptor.GetConverter();
					if (converter == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetConverter"
						}));
					}
					return converter;
				}

				EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					if (reflectTypeDescriptionProvider != null)
					{
						return reflectTypeDescriptionProvider.GetExtendedDefaultEvent(this._instance);
					}
					ICustomTypeDescriptor extendedTypeDescriptor = provider.GetExtendedTypeDescriptor(this._instance);
					if (extendedTypeDescriptor == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetExtendedTypeDescriptor"
						}));
					}
					return extendedTypeDescriptor.GetDefaultEvent();
				}

				PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					if (reflectTypeDescriptionProvider != null)
					{
						return reflectTypeDescriptionProvider.GetExtendedDefaultProperty(this._instance);
					}
					ICustomTypeDescriptor extendedTypeDescriptor = provider.GetExtendedTypeDescriptor(this._instance);
					if (extendedTypeDescriptor == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetExtendedTypeDescriptor"
						}));
					}
					return extendedTypeDescriptor.GetDefaultProperty();
				}

				object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
				{
					if (editorBaseType == null)
					{
						throw new ArgumentNullException("editorBaseType");
					}
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					if (reflectTypeDescriptionProvider != null)
					{
						return reflectTypeDescriptionProvider.GetExtendedEditor(this._instance, editorBaseType);
					}
					ICustomTypeDescriptor extendedTypeDescriptor = provider.GetExtendedTypeDescriptor(this._instance);
					if (extendedTypeDescriptor == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetExtendedTypeDescriptor"
						}));
					}
					return extendedTypeDescriptor.GetEditor(editorBaseType);
				}

				EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					if (reflectTypeDescriptionProvider != null)
					{
						return reflectTypeDescriptionProvider.GetExtendedEvents(this._instance);
					}
					ICustomTypeDescriptor extendedTypeDescriptor = provider.GetExtendedTypeDescriptor(this._instance);
					if (extendedTypeDescriptor == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetExtendedTypeDescriptor"
						}));
					}
					EventDescriptorCollection events = extendedTypeDescriptor.GetEvents();
					if (events == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetEvents"
						}));
					}
					return events;
				}

				EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					if (reflectTypeDescriptionProvider != null)
					{
						return reflectTypeDescriptionProvider.GetExtendedEvents(this._instance);
					}
					ICustomTypeDescriptor extendedTypeDescriptor = provider.GetExtendedTypeDescriptor(this._instance);
					if (extendedTypeDescriptor == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetExtendedTypeDescriptor"
						}));
					}
					EventDescriptorCollection events = extendedTypeDescriptor.GetEvents(attributes);
					if (events == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetEvents"
						}));
					}
					return events;
				}

				PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					if (reflectTypeDescriptionProvider != null)
					{
						return reflectTypeDescriptionProvider.GetExtendedProperties(this._instance);
					}
					ICustomTypeDescriptor extendedTypeDescriptor = provider.GetExtendedTypeDescriptor(this._instance);
					if (extendedTypeDescriptor == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetExtendedTypeDescriptor"
						}));
					}
					PropertyDescriptorCollection properties = extendedTypeDescriptor.GetProperties();
					if (properties == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetProperties"
						}));
					}
					return properties;
				}

				PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					if (reflectTypeDescriptionProvider != null)
					{
						return reflectTypeDescriptionProvider.GetExtendedProperties(this._instance);
					}
					ICustomTypeDescriptor extendedTypeDescriptor = provider.GetExtendedTypeDescriptor(this._instance);
					if (extendedTypeDescriptor == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetExtendedTypeDescriptor"
						}));
					}
					PropertyDescriptorCollection properties = extendedTypeDescriptor.GetProperties(attributes);
					if (properties == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetProperties"
						}));
					}
					return properties;
				}

				object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					if (reflectTypeDescriptionProvider != null)
					{
						return reflectTypeDescriptionProvider.GetExtendedPropertyOwner(this._instance, pd);
					}
					ICustomTypeDescriptor extendedTypeDescriptor = provider.GetExtendedTypeDescriptor(this._instance);
					if (extendedTypeDescriptor == null)
					{
						throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
						{
							this._node.Provider.GetType().FullName,
							"GetExtendedTypeDescriptor"
						}));
					}
					object obj = extendedTypeDescriptor.GetPropertyOwner(pd);
					if (obj == null)
					{
						obj = this._instance;
					}
					return obj;
				}

				private TypeDescriptor.TypeDescriptionNode _node;

				private object _instance;
			}

			private struct DefaultTypeDescriptor : ICustomTypeDescriptor
			{
				internal DefaultTypeDescriptor(TypeDescriptor.TypeDescriptionNode node, Type objectType, object instance)
				{
					this._node = node;
					this._objectType = objectType;
					this._instance = instance;
				}

				AttributeCollection ICustomTypeDescriptor.GetAttributes()
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					AttributeCollection attributes;
					if (reflectTypeDescriptionProvider != null)
					{
						attributes = reflectTypeDescriptionProvider.GetAttributes(this._objectType);
					}
					else
					{
						ICustomTypeDescriptor typeDescriptor = provider.GetTypeDescriptor(this._objectType, this._instance);
						if (typeDescriptor == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetTypeDescriptor"
							}));
						}
						attributes = typeDescriptor.GetAttributes();
						if (attributes == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetAttributes"
							}));
						}
					}
					return attributes;
				}

				string ICustomTypeDescriptor.GetClassName()
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					string text;
					if (reflectTypeDescriptionProvider != null)
					{
						text = reflectTypeDescriptionProvider.GetClassName(this._objectType);
					}
					else
					{
						ICustomTypeDescriptor typeDescriptor = provider.GetTypeDescriptor(this._objectType, this._instance);
						if (typeDescriptor == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetTypeDescriptor"
							}));
						}
						text = typeDescriptor.GetClassName();
						if (text == null)
						{
							text = this._objectType.FullName;
						}
					}
					return text;
				}

				string ICustomTypeDescriptor.GetComponentName()
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					string componentName;
					if (reflectTypeDescriptionProvider != null)
					{
						componentName = reflectTypeDescriptionProvider.GetComponentName(this._objectType, this._instance);
					}
					else
					{
						ICustomTypeDescriptor typeDescriptor = provider.GetTypeDescriptor(this._objectType, this._instance);
						if (typeDescriptor == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetTypeDescriptor"
							}));
						}
						componentName = typeDescriptor.GetComponentName();
					}
					return componentName;
				}

				TypeConverter ICustomTypeDescriptor.GetConverter()
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					TypeConverter converter;
					if (reflectTypeDescriptionProvider != null)
					{
						converter = reflectTypeDescriptionProvider.GetConverter(this._objectType, this._instance);
					}
					else
					{
						ICustomTypeDescriptor typeDescriptor = provider.GetTypeDescriptor(this._objectType, this._instance);
						if (typeDescriptor == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetTypeDescriptor"
							}));
						}
						converter = typeDescriptor.GetConverter();
						if (converter == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetConverter"
							}));
						}
					}
					return converter;
				}

				EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					EventDescriptor defaultEvent;
					if (reflectTypeDescriptionProvider != null)
					{
						defaultEvent = reflectTypeDescriptionProvider.GetDefaultEvent(this._objectType, this._instance);
					}
					else
					{
						ICustomTypeDescriptor typeDescriptor = provider.GetTypeDescriptor(this._objectType, this._instance);
						if (typeDescriptor == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetTypeDescriptor"
							}));
						}
						defaultEvent = typeDescriptor.GetDefaultEvent();
					}
					return defaultEvent;
				}

				PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					PropertyDescriptor defaultProperty;
					if (reflectTypeDescriptionProvider != null)
					{
						defaultProperty = reflectTypeDescriptionProvider.GetDefaultProperty(this._objectType, this._instance);
					}
					else
					{
						ICustomTypeDescriptor typeDescriptor = provider.GetTypeDescriptor(this._objectType, this._instance);
						if (typeDescriptor == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetTypeDescriptor"
							}));
						}
						defaultProperty = typeDescriptor.GetDefaultProperty();
					}
					return defaultProperty;
				}

				object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
				{
					if (editorBaseType == null)
					{
						throw new ArgumentNullException("editorBaseType");
					}
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					object editor;
					if (reflectTypeDescriptionProvider != null)
					{
						editor = reflectTypeDescriptionProvider.GetEditor(this._objectType, this._instance, editorBaseType);
					}
					else
					{
						ICustomTypeDescriptor typeDescriptor = provider.GetTypeDescriptor(this._objectType, this._instance);
						if (typeDescriptor == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetTypeDescriptor"
							}));
						}
						editor = typeDescriptor.GetEditor(editorBaseType);
					}
					return editor;
				}

				EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					EventDescriptorCollection events;
					if (reflectTypeDescriptionProvider != null)
					{
						events = reflectTypeDescriptionProvider.GetEvents(this._objectType);
					}
					else
					{
						ICustomTypeDescriptor typeDescriptor = provider.GetTypeDescriptor(this._objectType, this._instance);
						if (typeDescriptor == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetTypeDescriptor"
							}));
						}
						events = typeDescriptor.GetEvents();
						if (events == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetEvents"
							}));
						}
					}
					return events;
				}

				EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					EventDescriptorCollection events;
					if (reflectTypeDescriptionProvider != null)
					{
						events = reflectTypeDescriptionProvider.GetEvents(this._objectType);
					}
					else
					{
						ICustomTypeDescriptor typeDescriptor = provider.GetTypeDescriptor(this._objectType, this._instance);
						if (typeDescriptor == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetTypeDescriptor"
							}));
						}
						events = typeDescriptor.GetEvents(attributes);
						if (events == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetEvents"
							}));
						}
					}
					return events;
				}

				PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					PropertyDescriptorCollection properties;
					if (reflectTypeDescriptionProvider != null)
					{
						properties = reflectTypeDescriptionProvider.GetProperties(this._objectType);
					}
					else
					{
						ICustomTypeDescriptor typeDescriptor = provider.GetTypeDescriptor(this._objectType, this._instance);
						if (typeDescriptor == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetTypeDescriptor"
							}));
						}
						properties = typeDescriptor.GetProperties();
						if (properties == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetProperties"
							}));
						}
					}
					return properties;
				}

				PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					PropertyDescriptorCollection properties;
					if (reflectTypeDescriptionProvider != null)
					{
						properties = reflectTypeDescriptionProvider.GetProperties(this._objectType);
					}
					else
					{
						ICustomTypeDescriptor typeDescriptor = provider.GetTypeDescriptor(this._objectType, this._instance);
						if (typeDescriptor == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetTypeDescriptor"
							}));
						}
						properties = typeDescriptor.GetProperties(attributes);
						if (properties == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetProperties"
							}));
						}
					}
					return properties;
				}

				object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
				{
					TypeDescriptionProvider provider = this._node.Provider;
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = provider as ReflectTypeDescriptionProvider;
					object obj;
					if (reflectTypeDescriptionProvider != null)
					{
						obj = reflectTypeDescriptionProvider.GetPropertyOwner(this._objectType, this._instance, pd);
					}
					else
					{
						ICustomTypeDescriptor typeDescriptor = provider.GetTypeDescriptor(this._objectType, this._instance);
						if (typeDescriptor == null)
						{
							throw new InvalidOperationException(SR.GetString("The type description provider {0} has returned null from {1} which is illegal.", new object[]
							{
								this._node.Provider.GetType().FullName,
								"GetTypeDescriptor"
							}));
						}
						obj = typeDescriptor.GetPropertyOwner(pd);
						if (obj == null)
						{
							obj = this._instance;
						}
					}
					return obj;
				}

				private TypeDescriptor.TypeDescriptionNode _node;

				private Type _objectType;

				private object _instance;
			}
		}

		[TypeDescriptionProvider("System.Windows.Forms.ComponentModel.Com2Interop.ComNativeDescriptor, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
		private sealed class TypeDescriptorComObject
		{
		}

		private sealed class TypeDescriptorInterface
		{
		}
	}
}
