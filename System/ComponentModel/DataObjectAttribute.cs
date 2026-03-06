using System;

namespace System.ComponentModel
{
	/// <summary>Identifies a type as an object suitable for binding to an <see cref="T:System.Web.UI.WebControls.ObjectDataSource" /> object. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DataObjectAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DataObjectAttribute" /> class.</summary>
		public DataObjectAttribute() : this(true)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DataObjectAttribute" /> class and indicates whether an object is suitable for binding to an <see cref="T:System.Web.UI.WebControls.ObjectDataSource" /> object.</summary>
		/// <param name="isDataObject">
		///   <see langword="true" /> if the object is suitable for binding to an <see cref="T:System.Web.UI.WebControls.ObjectDataSource" /> object; otherwise, <see langword="false" />.</param>
		public DataObjectAttribute(bool isDataObject)
		{
			this.IsDataObject = isDataObject;
		}

		/// <summary>Gets a value indicating whether an object should be considered suitable for binding to an <see cref="T:System.Web.UI.WebControls.ObjectDataSource" /> object at design time.</summary>
		/// <returns>
		///   <see langword="true" /> if the object should be considered suitable for binding to an <see cref="T:System.Web.UI.WebControls.ObjectDataSource" /> object; otherwise, <see langword="false" />.</returns>
		public bool IsDataObject { get; }

		/// <summary>Determines whether this instance of <see cref="T:System.ComponentModel.DataObjectAttribute" /> fits the pattern of another object.</summary>
		/// <param name="obj">An object to compare with this instance of <see cref="T:System.ComponentModel.DataObjectAttribute" />.</param>
		/// <returns>
		///   <see langword="true" /> if this instance is the same as the instance specified by the <paramref name="obj" /> parameter; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DataObjectAttribute dataObjectAttribute = obj as DataObjectAttribute;
			return dataObjectAttribute != null && dataObjectAttribute.IsDataObject == this.IsDataObject;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return this.IsDataObject.GetHashCode();
		}

		/// <summary>Gets a value indicating whether the current value of the attribute is the default value for the attribute.</summary>
		/// <returns>
		///   <see langword="true" /> if the current value of the attribute is the default; otherwise, <see langword="false" />.</returns>
		public override bool IsDefaultAttribute()
		{
			return this.Equals(DataObjectAttribute.Default);
		}

		/// <summary>Indicates that the class is suitable for binding to an <see cref="T:System.Web.UI.WebControls.ObjectDataSource" /> object at design time. This field is read-only.</summary>
		public static readonly DataObjectAttribute DataObject = new DataObjectAttribute(true);

		/// <summary>Indicates that the class is not suitable for binding to an <see cref="T:System.Web.UI.WebControls.ObjectDataSource" /> object at design time. This field is read-only.</summary>
		public static readonly DataObjectAttribute NonDataObject = new DataObjectAttribute(false);

		/// <summary>Represents the default value of the <see cref="T:System.ComponentModel.DataObjectAttribute" /> class, which indicates that the class is suitable for binding to an <see cref="T:System.Web.UI.WebControls.ObjectDataSource" /> object at design time. This field is read-only.</summary>
		public static readonly DataObjectAttribute Default = DataObjectAttribute.NonDataObject;
	}
}
