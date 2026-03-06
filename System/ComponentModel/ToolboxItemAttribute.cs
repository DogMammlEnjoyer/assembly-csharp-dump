using System;
using System.Globalization;

namespace System.ComponentModel
{
	/// <summary>Represents an attribute of a toolbox item.</summary>
	[AttributeUsage(AttributeTargets.All)]
	public class ToolboxItemAttribute : Attribute
	{
		/// <summary>Gets a value indicating whether the current value of the attribute is the default value for the attribute.</summary>
		/// <returns>
		///   <see langword="true" /> if the current value of the attribute is the default; otherwise, <see langword="false" />.</returns>
		public override bool IsDefaultAttribute()
		{
			return this.Equals(ToolboxItemAttribute.Default);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.ToolboxItemAttribute" /> class and specifies whether to use default initialization values.</summary>
		/// <param name="defaultType">
		///   <see langword="true" /> to create a toolbox item attribute for a default type; <see langword="false" /> to associate no default toolbox item support for this attribute.</param>
		public ToolboxItemAttribute(bool defaultType)
		{
			if (defaultType)
			{
				this._toolboxItemTypeName = "System.Drawing.Design.ToolboxItem, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.ToolboxItemAttribute" /> class using the specified name of the type.</summary>
		/// <param name="toolboxItemTypeName">The names of the type of the toolbox item and of the assembly that contains the type.</param>
		public ToolboxItemAttribute(string toolboxItemTypeName)
		{
			toolboxItemTypeName.ToUpper(CultureInfo.InvariantCulture);
			this._toolboxItemTypeName = toolboxItemTypeName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.ToolboxItemAttribute" /> class using the specified type of the toolbox item.</summary>
		/// <param name="toolboxItemType">The type of the toolbox item.</param>
		public ToolboxItemAttribute(Type toolboxItemType)
		{
			this._toolboxItemType = toolboxItemType;
			this._toolboxItemTypeName = toolboxItemType.AssemblyQualifiedName;
		}

		/// <summary>Gets or sets the type of the toolbox item.</summary>
		/// <returns>The type of the toolbox item.</returns>
		/// <exception cref="T:System.ArgumentException">The type cannot be found.</exception>
		public Type ToolboxItemType
		{
			get
			{
				if (this._toolboxItemType == null && this._toolboxItemTypeName != null)
				{
					try
					{
						this._toolboxItemType = Type.GetType(this._toolboxItemTypeName, true);
					}
					catch (Exception innerException)
					{
						throw new ArgumentException(SR.Format("Failed to create ToolboxItem of type: {0}", this._toolboxItemTypeName), innerException);
					}
				}
				return this._toolboxItemType;
			}
		}

		/// <summary>Gets or sets the name of the type of the current <see cref="T:System.Drawing.Design.ToolboxItem" />.</summary>
		/// <returns>The fully qualified type name of the current toolbox item.</returns>
		public string ToolboxItemTypeName
		{
			get
			{
				if (this._toolboxItemTypeName == null)
				{
					return string.Empty;
				}
				return this._toolboxItemTypeName;
			}
		}

		/// <summary>Returns a value that indicates whether this instance is equal to a specified object.</summary>
		/// <param name="obj">An <see cref="T:System.Object" /> to compare with this instance or a null reference (<see langword="Nothing" /> in Visual Basic).</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> equals the type and value of this instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ToolboxItemAttribute toolboxItemAttribute = obj as ToolboxItemAttribute;
			return toolboxItemAttribute != null && toolboxItemAttribute.ToolboxItemTypeName == this.ToolboxItemTypeName;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			if (this._toolboxItemTypeName != null)
			{
				return this._toolboxItemTypeName.GetHashCode();
			}
			return base.GetHashCode();
		}

		private Type _toolboxItemType;

		private string _toolboxItemTypeName;

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.ToolboxItemAttribute" /> class and sets the type to the default, <see cref="T:System.Drawing.Design.ToolboxItem" />. This field is read-only.</summary>
		public static readonly ToolboxItemAttribute Default = new ToolboxItemAttribute("System.Drawing.Design.ToolboxItem, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.ToolboxItemAttribute" /> class and sets the type to <see langword="null" />. This field is read-only.</summary>
		public static readonly ToolboxItemAttribute None = new ToolboxItemAttribute(false);
	}
}
