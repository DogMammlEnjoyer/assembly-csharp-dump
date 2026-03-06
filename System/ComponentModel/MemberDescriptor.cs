using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;

namespace System.ComponentModel
{
	/// <summary>Represents a class member, such as a property or event. This is an abstract base class.</summary>
	[ComVisible(true)]
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public abstract class MemberDescriptor
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.MemberDescriptor" /> class with the specified name of the member.</summary>
		/// <param name="name">The name of the member.</param>
		/// <exception cref="T:System.ArgumentException">The name is an empty string ("") or <see langword="null" />.</exception>
		protected MemberDescriptor(string name) : this(name, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.MemberDescriptor" /> class with the specified name of the member and an array of attributes.</summary>
		/// <param name="name">The name of the member.</param>
		/// <param name="attributes">An array of type <see cref="T:System.Attribute" /> that contains the member attributes.</param>
		/// <exception cref="T:System.ArgumentException">The name is an empty string ("") or <see langword="null" />.</exception>
		protected MemberDescriptor(string name, Attribute[] attributes)
		{
			this.lockCookie = new object();
			base..ctor();
			try
			{
				if (name == null || name.Length == 0)
				{
					throw new ArgumentException(SR.GetString("Invalid member name."));
				}
				this.name = name;
				this.displayName = name;
				this.nameHash = name.GetHashCode();
				if (attributes != null)
				{
					this.attributes = attributes;
					this.attributesFiltered = false;
				}
				this.originalAttributes = this.attributes;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.MemberDescriptor" /> class with the specified <see cref="T:System.ComponentModel.MemberDescriptor" />.</summary>
		/// <param name="descr">A <see cref="T:System.ComponentModel.MemberDescriptor" /> that contains the name of the member and its attributes.</param>
		protected MemberDescriptor(MemberDescriptor descr)
		{
			this.lockCookie = new object();
			base..ctor();
			this.name = descr.Name;
			this.displayName = this.name;
			this.nameHash = this.name.GetHashCode();
			this.attributes = new Attribute[descr.Attributes.Count];
			descr.Attributes.CopyTo(this.attributes, 0);
			this.attributesFiltered = true;
			this.originalAttributes = this.attributes;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.MemberDescriptor" /> class with the name in the specified <see cref="T:System.ComponentModel.MemberDescriptor" /> and the attributes in both the old <see cref="T:System.ComponentModel.MemberDescriptor" /> and the <see cref="T:System.Attribute" /> array.</summary>
		/// <param name="oldMemberDescriptor">A <see cref="T:System.ComponentModel.MemberDescriptor" /> that has the name of the member and its attributes.</param>
		/// <param name="newAttributes">An array of <see cref="T:System.Attribute" /> objects with the attributes you want to add to the member.</param>
		protected MemberDescriptor(MemberDescriptor oldMemberDescriptor, Attribute[] newAttributes)
		{
			this.lockCookie = new object();
			base..ctor();
			this.name = oldMemberDescriptor.Name;
			this.displayName = oldMemberDescriptor.DisplayName;
			this.nameHash = this.name.GetHashCode();
			ArrayList arrayList = new ArrayList();
			if (oldMemberDescriptor.Attributes.Count != 0)
			{
				foreach (object value in oldMemberDescriptor.Attributes)
				{
					arrayList.Add(value);
				}
			}
			if (newAttributes != null)
			{
				foreach (Attribute value2 in newAttributes)
				{
					arrayList.Add(value2);
				}
			}
			this.attributes = new Attribute[arrayList.Count];
			arrayList.CopyTo(this.attributes, 0);
			this.attributesFiltered = false;
			this.originalAttributes = this.attributes;
		}

		/// <summary>Gets or sets an array of attributes.</summary>
		/// <returns>An array of type <see cref="T:System.Attribute" /> that contains the attributes of this member.</returns>
		protected virtual Attribute[] AttributeArray
		{
			get
			{
				this.CheckAttributesValid();
				this.FilterAttributesIfNeeded();
				return this.attributes;
			}
			set
			{
				object obj = this.lockCookie;
				lock (obj)
				{
					this.attributes = value;
					this.originalAttributes = value;
					this.attributesFiltered = false;
					this.attributeCollection = null;
				}
			}
		}

		/// <summary>Gets the collection of attributes for this member.</summary>
		/// <returns>An <see cref="T:System.ComponentModel.AttributeCollection" /> that provides the attributes for this member, or an empty collection if there are no attributes in the <see cref="P:System.ComponentModel.MemberDescriptor.AttributeArray" />.</returns>
		public virtual AttributeCollection Attributes
		{
			get
			{
				this.CheckAttributesValid();
				AttributeCollection attributeCollection = this.attributeCollection;
				if (attributeCollection == null)
				{
					object obj = this.lockCookie;
					lock (obj)
					{
						attributeCollection = this.CreateAttributeCollection();
						this.attributeCollection = attributeCollection;
					}
				}
				return attributeCollection;
			}
		}

		/// <summary>Gets the name of the category to which the member belongs, as specified in the <see cref="T:System.ComponentModel.CategoryAttribute" />.</summary>
		/// <returns>The name of the category to which the member belongs. If there is no <see cref="T:System.ComponentModel.CategoryAttribute" />, the category name is set to the default category, <see langword="Misc" />.</returns>
		public virtual string Category
		{
			get
			{
				if (this.category == null)
				{
					this.category = ((CategoryAttribute)this.Attributes[typeof(CategoryAttribute)]).Category;
				}
				return this.category;
			}
		}

		/// <summary>Gets the description of the member, as specified in the <see cref="T:System.ComponentModel.DescriptionAttribute" />.</summary>
		/// <returns>The description of the member. If there is no <see cref="T:System.ComponentModel.DescriptionAttribute" />, the property value is set to the default, which is an empty string ("").</returns>
		public virtual string Description
		{
			get
			{
				if (this.description == null)
				{
					this.description = ((DescriptionAttribute)this.Attributes[typeof(DescriptionAttribute)]).Description;
				}
				return this.description;
			}
		}

		/// <summary>Gets a value indicating whether the member is browsable, as specified in the <see cref="T:System.ComponentModel.BrowsableAttribute" />.</summary>
		/// <returns>
		///   <see langword="true" /> if the member is browsable; otherwise, <see langword="false" />. If there is no <see cref="T:System.ComponentModel.BrowsableAttribute" />, the property value is set to the default, which is <see langword="true" />.</returns>
		public virtual bool IsBrowsable
		{
			get
			{
				return ((BrowsableAttribute)this.Attributes[typeof(BrowsableAttribute)]).Browsable;
			}
		}

		/// <summary>Gets the name of the member.</summary>
		/// <returns>The name of the member.</returns>
		public virtual string Name
		{
			get
			{
				if (this.name == null)
				{
					return "";
				}
				return this.name;
			}
		}

		/// <summary>Gets the hash code for the name of the member, as specified in <see cref="M:System.String.GetHashCode" />.</summary>
		/// <returns>The hash code for the name of the member.</returns>
		protected virtual int NameHashCode
		{
			get
			{
				return this.nameHash;
			}
		}

		/// <summary>Gets whether this member should be set only at design time, as specified in the <see cref="T:System.ComponentModel.DesignOnlyAttribute" />.</summary>
		/// <returns>
		///   <see langword="true" /> if this member should be set only at design time; <see langword="false" /> if the member can be set during run time.</returns>
		public virtual bool DesignTimeOnly
		{
			get
			{
				return DesignOnlyAttribute.Yes.Equals(this.Attributes[typeof(DesignOnlyAttribute)]);
			}
		}

		/// <summary>Gets the name that can be displayed in a window, such as a Properties window.</summary>
		/// <returns>The name to display for the member.</returns>
		public virtual string DisplayName
		{
			get
			{
				DisplayNameAttribute displayNameAttribute = this.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;
				if (displayNameAttribute == null || displayNameAttribute.IsDefaultAttribute())
				{
					return this.displayName;
				}
				return displayNameAttribute.DisplayName;
			}
		}

		private void CheckAttributesValid()
		{
			if (this.attributesFiltered && this.metadataVersion != TypeDescriptor.MetadataVersion)
			{
				this.attributesFilled = false;
				this.attributesFiltered = false;
				this.attributeCollection = null;
			}
		}

		/// <summary>Creates a collection of attributes using the array of attributes passed to the constructor.</summary>
		/// <returns>A new <see cref="T:System.ComponentModel.AttributeCollection" /> that contains the <see cref="P:System.ComponentModel.MemberDescriptor.AttributeArray" /> attributes.</returns>
		protected virtual AttributeCollection CreateAttributeCollection()
		{
			return new AttributeCollection(this.AttributeArray);
		}

		/// <summary>Compares this instance to the given object to see if they are equivalent.</summary>
		/// <param name="obj">The object to compare to the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if equivalent; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != base.GetType())
			{
				return false;
			}
			MemberDescriptor memberDescriptor = (MemberDescriptor)obj;
			this.FilterAttributesIfNeeded();
			memberDescriptor.FilterAttributesIfNeeded();
			if (memberDescriptor.nameHash != this.nameHash)
			{
				return false;
			}
			if (memberDescriptor.category == null != (this.category == null) || (this.category != null && !memberDescriptor.category.Equals(this.category)))
			{
				return false;
			}
			if (!LocalAppContextSwitches.MemberDescriptorEqualsReturnsFalseIfEquivalent)
			{
				if (memberDescriptor.description == null != (this.description == null) || (this.description != null && !memberDescriptor.description.Equals(this.description)))
				{
					return false;
				}
			}
			else if (memberDescriptor.description == null != (this.description == null) || (this.description != null && !memberDescriptor.category.Equals(this.description)))
			{
				return false;
			}
			if (memberDescriptor.attributes == null != (this.attributes == null))
			{
				return false;
			}
			bool result = true;
			if (this.attributes != null)
			{
				if (this.attributes.Length != memberDescriptor.attributes.Length)
				{
					return false;
				}
				for (int i = 0; i < this.attributes.Length; i++)
				{
					if (!this.attributes[i].Equals(memberDescriptor.attributes[i]))
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}

		/// <summary>When overridden in a derived class, adds the attributes of the inheriting class to the specified list of attributes in the parent class.</summary>
		/// <param name="attributeList">An <see cref="T:System.Collections.IList" /> that lists the attributes in the parent class. Initially, this is empty.</param>
		protected virtual void FillAttributes(IList attributeList)
		{
			if (this.originalAttributes != null)
			{
				foreach (Attribute value in this.originalAttributes)
				{
					attributeList.Add(value);
				}
			}
		}

		private void FilterAttributesIfNeeded()
		{
			if (!this.attributesFiltered)
			{
				IList list;
				if (!this.attributesFilled)
				{
					list = new ArrayList();
					try
					{
						this.FillAttributes(list);
						goto IL_34;
					}
					catch (ThreadAbortException)
					{
						throw;
					}
					catch (Exception)
					{
						goto IL_34;
					}
				}
				list = new ArrayList(this.attributes);
				IL_34:
				Hashtable hashtable = new Hashtable(list.Count);
				foreach (object obj in list)
				{
					Attribute attribute = (Attribute)obj;
					hashtable[attribute.TypeId] = attribute;
				}
				Attribute[] array = new Attribute[hashtable.Values.Count];
				hashtable.Values.CopyTo(array, 0);
				object obj2 = this.lockCookie;
				lock (obj2)
				{
					this.attributes = array;
					this.attributesFiltered = true;
					this.attributesFilled = true;
					this.metadataVersion = TypeDescriptor.MetadataVersion;
				}
			}
		}

		/// <summary>Finds the given method through reflection, searching only for public methods.</summary>
		/// <param name="componentClass">The component that contains the method.</param>
		/// <param name="name">The name of the method to find.</param>
		/// <param name="args">An array of parameters for the method, used to choose between overloaded methods.</param>
		/// <param name="returnType">The type to return for the method.</param>
		/// <returns>A <see cref="T:System.Reflection.MethodInfo" /> that represents the method, or <see langword="null" /> if the method is not found.</returns>
		protected static MethodInfo FindMethod(Type componentClass, string name, Type[] args, Type returnType)
		{
			return MemberDescriptor.FindMethod(componentClass, name, args, returnType, true);
		}

		/// <summary>Finds the given method through reflection, with an option to search only public methods.</summary>
		/// <param name="componentClass">The component that contains the method.</param>
		/// <param name="name">The name of the method to find.</param>
		/// <param name="args">An array of parameters for the method, used to choose between overloaded methods.</param>
		/// <param name="returnType">The type to return for the method.</param>
		/// <param name="publicOnly">Whether to restrict search to public methods.</param>
		/// <returns>A <see cref="T:System.Reflection.MethodInfo" /> that represents the method, or <see langword="null" /> if the method is not found.</returns>
		protected static MethodInfo FindMethod(Type componentClass, string name, Type[] args, Type returnType, bool publicOnly)
		{
			MethodInfo methodInfo;
			if (publicOnly)
			{
				methodInfo = componentClass.GetMethod(name, args);
			}
			else
			{
				methodInfo = componentClass.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, args, null);
			}
			if (methodInfo != null && !methodInfo.ReturnType.IsEquivalentTo(returnType))
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A hash code for the current <see cref="T:System.ComponentModel.MemberDescriptor" />.</returns>
		public override int GetHashCode()
		{
			return this.nameHash;
		}

		/// <summary>Retrieves the object that should be used during invocation of members.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> of the invocation target.</param>
		/// <param name="instance">The potential invocation target.</param>
		/// <returns>The object to be used during member invocations.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="type" /> or <paramref name="instance" /> is <see langword="null" />.</exception>
		protected virtual object GetInvocationTarget(Type type, object instance)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return TypeDescriptor.GetAssociation(type, instance);
		}

		/// <summary>Gets a component site for the given component.</summary>
		/// <param name="component">The component for which you want to find a site.</param>
		/// <returns>The site of the component, or <see langword="null" /> if a site does not exist.</returns>
		protected static ISite GetSite(object component)
		{
			if (!(component is IComponent))
			{
				return null;
			}
			return ((IComponent)component).Site;
		}

		/// <summary>Gets the component on which to invoke a method.</summary>
		/// <param name="componentClass">A <see cref="T:System.Type" /> representing the type of component this <see cref="T:System.ComponentModel.MemberDescriptor" /> is bound to. For example, if this <see cref="T:System.ComponentModel.MemberDescriptor" /> describes a property, this parameter should be the class that the property is declared on.</param>
		/// <param name="component">An instance of the object to call.</param>
		/// <returns>An instance of the component to invoke. This method returns a visual designer when the property is attached to a visual designer.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="componentClass" /> or <paramref name="component" /> is <see langword="null" />.</exception>
		[Obsolete("This method has been deprecated. Use GetInvocationTarget instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
		protected static object GetInvokee(Type componentClass, object component)
		{
			if (componentClass == null)
			{
				throw new ArgumentNullException("componentClass");
			}
			if (component == null)
			{
				throw new ArgumentNullException("component");
			}
			return TypeDescriptor.GetAssociation(componentClass, component);
		}

		private string name;

		private string displayName;

		private int nameHash;

		private AttributeCollection attributeCollection;

		private Attribute[] attributes;

		private Attribute[] originalAttributes;

		private bool attributesFiltered;

		private bool attributesFilled;

		private int metadataVersion;

		private string category;

		private string description;

		private object lockCookie;
	}
}
