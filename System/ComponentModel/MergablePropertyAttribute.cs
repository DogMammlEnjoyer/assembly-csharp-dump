using System;

namespace System.ComponentModel
{
	/// <summary>Specifies that this property can be combined with properties belonging to other objects in a Properties window.</summary>
	[AttributeUsage(AttributeTargets.All)]
	public sealed class MergablePropertyAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.MergablePropertyAttribute" /> class.</summary>
		/// <param name="allowMerge">
		///   <see langword="true" /> if this property can be combined with properties belonging to other objects in a Properties window; otherwise, <see langword="false" />.</param>
		public MergablePropertyAttribute(bool allowMerge)
		{
			this.AllowMerge = allowMerge;
		}

		/// <summary>Gets a value indicating whether this property can be combined with properties belonging to other objects in a Properties window.</summary>
		/// <returns>
		///   <see langword="true" /> if this property can be combined with properties belonging to other objects in a Properties window; otherwise, <see langword="false" />.</returns>
		public bool AllowMerge { get; }

		/// <summary>Indicates whether this instance and a specified object are equal.</summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			MergablePropertyAttribute mergablePropertyAttribute = obj as MergablePropertyAttribute;
			bool? flag = (mergablePropertyAttribute != null) ? new bool?(mergablePropertyAttribute.AllowMerge) : null;
			bool allowMerge = this.AllowMerge;
			return flag.GetValueOrDefault() == allowMerge & flag != null;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A hash code for the current <see cref="T:System.ComponentModel.MergablePropertyAttribute" />.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Determines if this attribute is the default.</summary>
		/// <returns>
		///   <see langword="true" /> if the attribute is the default value for this attribute class; otherwise, <see langword="false" />.</returns>
		public override bool IsDefaultAttribute()
		{
			return this.Equals(MergablePropertyAttribute.Default);
		}

		/// <summary>Specifies that a property can be combined with properties belonging to other objects in a Properties window. This <see langword="static" /> field is read-only.</summary>
		public static readonly MergablePropertyAttribute Yes = new MergablePropertyAttribute(true);

		/// <summary>Specifies that a property cannot be combined with properties belonging to other objects in a Properties window. This <see langword="static" /> field is read-only.</summary>
		public static readonly MergablePropertyAttribute No = new MergablePropertyAttribute(false);

		/// <summary>Specifies the default value, which is <see cref="F:System.ComponentModel.MergablePropertyAttribute.Yes" />, that is a property can be combined with properties belonging to other objects in a Properties window. This <see langword="static" /> field is read-only.</summary>
		public static readonly MergablePropertyAttribute Default = MergablePropertyAttribute.Yes;
	}
}
