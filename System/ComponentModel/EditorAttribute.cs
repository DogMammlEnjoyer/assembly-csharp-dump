using System;
using System.Globalization;

namespace System.ComponentModel
{
	/// <summary>Specifies the editor to use to change a property. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	public sealed class EditorAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.EditorAttribute" /> class with the default editor, which is no editor.</summary>
		public EditorAttribute()
		{
			this.EditorTypeName = string.Empty;
			this.EditorBaseTypeName = string.Empty;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.EditorAttribute" /> class with the type name and base type name of the editor.</summary>
		/// <param name="typeName">The fully qualified type name of the editor.</param>
		/// <param name="baseTypeName">The fully qualified type name of the base class or interface to use as a lookup key for the editor. This class must be or derive from <see cref="T:System.Drawing.Design.UITypeEditor" />.</param>
		public EditorAttribute(string typeName, string baseTypeName)
		{
			typeName.ToUpper(CultureInfo.InvariantCulture);
			this.EditorTypeName = typeName;
			this.EditorBaseTypeName = baseTypeName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.EditorAttribute" /> class with the type name and the base type.</summary>
		/// <param name="typeName">The fully qualified type name of the editor.</param>
		/// <param name="baseType">The <see cref="T:System.Type" /> of the base class or interface to use as a lookup key for the editor. This class must be or derive from <see cref="T:System.Drawing.Design.UITypeEditor" />.</param>
		public EditorAttribute(string typeName, Type baseType)
		{
			typeName.ToUpper(CultureInfo.InvariantCulture);
			this.EditorTypeName = typeName;
			this.EditorBaseTypeName = baseType.AssemblyQualifiedName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.EditorAttribute" /> class with the type and the base type.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> that represents the type of the editor.</param>
		/// <param name="baseType">The <see cref="T:System.Type" /> of the base class or interface to use as a lookup key for the editor. This class must be or derive from <see cref="T:System.Drawing.Design.UITypeEditor" />.</param>
		public EditorAttribute(Type type, Type baseType)
		{
			this.EditorTypeName = type.AssemblyQualifiedName;
			this.EditorBaseTypeName = baseType.AssemblyQualifiedName;
		}

		/// <summary>Gets the name of the base class or interface serving as a lookup key for this editor.</summary>
		/// <returns>The name of the base class or interface serving as a lookup key for this editor.</returns>
		public string EditorBaseTypeName { get; }

		/// <summary>Gets the name of the editor class in the <see cref="P:System.Type.AssemblyQualifiedName" /> format.</summary>
		/// <returns>The name of the editor class in the <see cref="P:System.Type.AssemblyQualifiedName" /> format.</returns>
		public string EditorTypeName { get; }

		/// <summary>Gets a unique ID for this attribute type.</summary>
		/// <returns>A unique ID for this attribute type.</returns>
		public override object TypeId
		{
			get
			{
				if (this._typeId == null)
				{
					string text = this.EditorBaseTypeName;
					int num = text.IndexOf(',');
					if (num != -1)
					{
						text = text.Substring(0, num);
					}
					this._typeId = base.GetType().FullName + text;
				}
				return this._typeId;
			}
		}

		/// <summary>Returns whether the value of the given object is equal to the current <see cref="T:System.ComponentModel.EditorAttribute" />.</summary>
		/// <param name="obj">The object to test the value equality of.</param>
		/// <returns>
		///   <see langword="true" /> if the value of the given object is equal to that of the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			EditorAttribute editorAttribute = obj as EditorAttribute;
			return editorAttribute != null && editorAttribute.EditorTypeName == this.EditorTypeName && editorAttribute.EditorBaseTypeName == this.EditorBaseTypeName;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private string _typeId;
	}
}
