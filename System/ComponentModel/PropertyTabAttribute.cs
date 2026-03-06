using System;
using System.Reflection;

namespace System.ComponentModel
{
	/// <summary>Identifies the property tab or tabs to display for the specified class or classes.</summary>
	[AttributeUsage(AttributeTargets.All)]
	public class PropertyTabAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.PropertyTabAttribute" /> class.</summary>
		public PropertyTabAttribute()
		{
			this.TabScopes = Array.Empty<PropertyTabScope>();
			this._tabClassNames = Array.Empty<string>();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.PropertyTabAttribute" /> class using the specified type of tab.</summary>
		/// <param name="tabClass">The type of tab to create.</param>
		public PropertyTabAttribute(Type tabClass) : this(tabClass, PropertyTabScope.Component)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.PropertyTabAttribute" /> class using the specified tab class name.</summary>
		/// <param name="tabClassName">The assembly qualified name of the type of tab to create. For an example of this format convention, see <see cref="P:System.Type.AssemblyQualifiedName" />.</param>
		public PropertyTabAttribute(string tabClassName) : this(tabClassName, PropertyTabScope.Component)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.PropertyTabAttribute" /> class using the specified type of tab and tab scope.</summary>
		/// <param name="tabClass">The type of tab to create.</param>
		/// <param name="tabScope">A <see cref="T:System.ComponentModel.PropertyTabScope" /> that indicates the scope of this tab. If the scope is <see cref="F:System.ComponentModel.PropertyTabScope.Component" />, it is shown only for components with the corresponding <see cref="T:System.ComponentModel.PropertyTabAttribute" />. If it is <see cref="F:System.ComponentModel.PropertyTabScope.Document" />, it is shown for all components on the document.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="tabScope" /> is not <see cref="F:System.ComponentModel.PropertyTabScope.Document" /> or <see cref="F:System.ComponentModel.PropertyTabScope.Component" />.</exception>
		public PropertyTabAttribute(Type tabClass, PropertyTabScope tabScope)
		{
			this._tabClasses = new Type[]
			{
				tabClass
			};
			if (tabScope < PropertyTabScope.Document)
			{
				throw new ArgumentException(SR.Format("Scope must be PropertyTabScope.Document or PropertyTabScope.Component", Array.Empty<object>()), "tabScope");
			}
			this.TabScopes = new PropertyTabScope[]
			{
				tabScope
			};
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.PropertyTabAttribute" /> class using the specified tab class name and tab scope.</summary>
		/// <param name="tabClassName">The assembly qualified name of the type of tab to create. For an example of this format convention, see <see cref="P:System.Type.AssemblyQualifiedName" />.</param>
		/// <param name="tabScope">A <see cref="T:System.ComponentModel.PropertyTabScope" /> that indicates the scope of this tab. If the scope is <see cref="F:System.ComponentModel.PropertyTabScope.Component" />, it is shown only for components with the corresponding <see cref="T:System.ComponentModel.PropertyTabAttribute" />. If it is <see cref="F:System.ComponentModel.PropertyTabScope.Document" />, it is shown for all components on the document.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="tabScope" /> is not <see cref="F:System.ComponentModel.PropertyTabScope.Document" /> or <see cref="F:System.ComponentModel.PropertyTabScope.Component" />.</exception>
		public PropertyTabAttribute(string tabClassName, PropertyTabScope tabScope)
		{
			this._tabClassNames = new string[]
			{
				tabClassName
			};
			if (tabScope < PropertyTabScope.Document)
			{
				throw new ArgumentException(SR.Format("Scope must be PropertyTabScope.Document or PropertyTabScope.Component", Array.Empty<object>()), "tabScope");
			}
			this.TabScopes = new PropertyTabScope[]
			{
				tabScope
			};
		}

		/// <summary>Gets the types of tabs that this attribute uses.</summary>
		/// <returns>An array of types indicating the types of tabs that this attribute uses.</returns>
		/// <exception cref="T:System.TypeLoadException">The types specified by the <see cref="P:System.ComponentModel.PropertyTabAttribute.TabClassNames" /> property could not be found.</exception>
		public Type[] TabClasses
		{
			get
			{
				if (this._tabClasses == null && this._tabClassNames != null)
				{
					this._tabClasses = new Type[this._tabClassNames.Length];
					for (int i = 0; i < this._tabClassNames.Length; i++)
					{
						int num = this._tabClassNames[i].IndexOf(',');
						string text = null;
						string text2;
						if (num != -1)
						{
							text2 = this._tabClassNames[i].Substring(0, num).Trim();
							text = this._tabClassNames[i].Substring(num + 1).Trim();
						}
						else
						{
							text2 = this._tabClassNames[i];
						}
						this._tabClasses[i] = Type.GetType(text2, false);
						if (this._tabClasses[i] == null)
						{
							if (text == null)
							{
								throw new TypeLoadException(SR.Format("Couldn't find type {0}", text2));
							}
							Assembly assembly = Assembly.Load(text);
							if (assembly != null)
							{
								this._tabClasses[i] = assembly.GetType(text2, true);
							}
						}
					}
				}
				return this._tabClasses;
			}
		}

		/// <summary>Gets the names of the tab classes that this attribute uses.</summary>
		/// <returns>The names of the tab classes that this attribute uses.</returns>
		protected string[] TabClassNames
		{
			get
			{
				string[] tabClassNames = this._tabClassNames;
				return (string[])((tabClassNames != null) ? tabClassNames.Clone() : null);
			}
		}

		/// <summary>Gets an array of tab scopes of each tab of this <see cref="T:System.ComponentModel.PropertyTabAttribute" />.</summary>
		/// <returns>An array of <see cref="T:System.ComponentModel.PropertyTabScope" /> objects that indicate the scopes of the tabs.</returns>
		public PropertyTabScope[] TabScopes { get; private set; }

		/// <summary>Returns a value indicating whether this instance is equal to a specified object.</summary>
		/// <param name="other">An object to compare to this instance, or <see langword="null" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="other" /> refers to the same <see cref="T:System.ComponentModel.PropertyTabAttribute" /> instance; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.TypeLoadException">The types specified by the <see cref="P:System.ComponentModel.PropertyTabAttribute.TabClassNames" /> property of the <paramref name="other" /> parameter could not be found.</exception>
		public override bool Equals(object other)
		{
			return other is PropertyTabAttribute && this.Equals((PropertyTabAttribute)other);
		}

		/// <summary>Returns a value indicating whether this instance is equal to a specified attribute.</summary>
		/// <param name="other">A <see cref="T:System.ComponentModel.PropertyTabAttribute" /> to compare to this instance, or <see langword="null" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.ComponentModel.PropertyTabAttribute" /> instances are equal; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.TypeLoadException">The types specified by the <see cref="P:System.ComponentModel.PropertyTabAttribute.TabClassNames" /> property of the <paramref name="other" /> parameter cannot be found.</exception>
		public bool Equals(PropertyTabAttribute other)
		{
			if (other == this)
			{
				return true;
			}
			if (other.TabClasses.Length != this.TabClasses.Length || other.TabScopes.Length != this.TabScopes.Length)
			{
				return false;
			}
			for (int i = 0; i < this.TabClasses.Length; i++)
			{
				if (this.TabClasses[i] != other.TabClasses[i] || this.TabScopes[i] != other.TabScopes[i])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Gets the hash code for this object.</summary>
		/// <returns>The hash code for the object the attribute belongs to.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Initializes the attribute using the specified names of tab classes and array of tab scopes.</summary>
		/// <param name="tabClassNames">An array of fully qualified type names of the types to create for tabs on the Properties window.</param>
		/// <param name="tabScopes">The scope of each tab. If the scope is <see cref="F:System.ComponentModel.PropertyTabScope.Component" />, it is shown only for components with the corresponding <see cref="T:System.ComponentModel.PropertyTabAttribute" />. If it is <see cref="F:System.ComponentModel.PropertyTabScope.Document" />, it is shown for all components on the document.</param>
		/// <exception cref="T:System.ArgumentException">One or more of the values in <paramref name="tabScopes" /> is not <see cref="F:System.ComponentModel.PropertyTabScope.Document" /> or <see cref="F:System.ComponentModel.PropertyTabScope.Component" />.  
		///  -or-  
		///  The length of the <paramref name="tabClassNames" /> and <paramref name="tabScopes" /> arrays do not match.  
		///  -or-  
		///  <paramref name="tabClassNames" /> or <paramref name="tabScopes" /> is <see langword="null" />.</exception>
		protected void InitializeArrays(string[] tabClassNames, PropertyTabScope[] tabScopes)
		{
			this.InitializeArrays(tabClassNames, null, tabScopes);
		}

		/// <summary>Initializes the attribute using the specified names of tab classes and array of tab scopes.</summary>
		/// <param name="tabClasses">The types of tabs to create.</param>
		/// <param name="tabScopes">The scope of each tab. If the scope is <see cref="F:System.ComponentModel.PropertyTabScope.Component" />, it is shown only for components with the corresponding <see cref="T:System.ComponentModel.PropertyTabAttribute" />. If it is <see cref="F:System.ComponentModel.PropertyTabScope.Document" />, it is shown for all components on the document.</param>
		/// <exception cref="T:System.ArgumentException">One or more of the values in <paramref name="tabScopes" /> is not <see cref="F:System.ComponentModel.PropertyTabScope.Document" /> or <see cref="F:System.ComponentModel.PropertyTabScope.Component" />.  
		///  -or-  
		///  The length of the <paramref name="tabClassNames" /> and <paramref name="tabScopes" /> arrays do not match.  
		///  -or-  
		///  <paramref name="tabClassNames" /> or <paramref name="tabScopes" /> is <see langword="null" />.</exception>
		protected void InitializeArrays(Type[] tabClasses, PropertyTabScope[] tabScopes)
		{
			this.InitializeArrays(null, tabClasses, tabScopes);
		}

		private void InitializeArrays(string[] tabClassNames, Type[] tabClasses, PropertyTabScope[] tabScopes)
		{
			if (tabClasses != null)
			{
				if (tabScopes != null && tabClasses.Length != tabScopes.Length)
				{
					throw new ArgumentException("tabClasses must have the same number of items as tabScopes");
				}
				this._tabClasses = (Type[])tabClasses.Clone();
			}
			else if (tabClassNames != null)
			{
				if (tabScopes != null && tabClassNames.Length != tabScopes.Length)
				{
					throw new ArgumentException("tabClasses must have the same number of items as tabScopes");
				}
				this._tabClassNames = (string[])tabClassNames.Clone();
				this._tabClasses = null;
			}
			else if (this._tabClasses == null && this._tabClassNames == null)
			{
				throw new ArgumentException("An array of tab type names or tab types must be specified");
			}
			if (tabScopes != null)
			{
				for (int i = 0; i < tabScopes.Length; i++)
				{
					if (tabScopes[i] < PropertyTabScope.Document)
					{
						throw new ArgumentException("Scope must be PropertyTabScope.Document or PropertyTabScope.Component");
					}
				}
				this.TabScopes = (PropertyTabScope[])tabScopes.Clone();
				return;
			}
			this.TabScopes = new PropertyTabScope[tabClasses.Length];
			for (int j = 0; j < this.TabScopes.Length; j++)
			{
				this.TabScopes[j] = PropertyTabScope.Component;
			}
		}

		private Type[] _tabClasses;

		private string[] _tabClassNames;
	}
}
