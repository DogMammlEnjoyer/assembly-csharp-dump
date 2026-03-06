using System;

namespace System.ComponentModel
{
	/// <summary>Specifies that the property can be used as an application setting.</summary>
	[AttributeUsage(AttributeTargets.Property)]
	[Obsolete("Use System.ComponentModel.SettingsBindableAttribute instead to work with the new settings model.")]
	public class RecommendedAsConfigurableAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.RecommendedAsConfigurableAttribute" /> class.</summary>
		/// <param name="recommendedAsConfigurable">
		///   <see langword="true" /> if the property this attribute is bound to can be used as an application setting; otherwise, <see langword="false" />.</param>
		public RecommendedAsConfigurableAttribute(bool recommendedAsConfigurable)
		{
			this.RecommendedAsConfigurable = recommendedAsConfigurable;
		}

		/// <summary>Gets a value indicating whether the property this attribute is bound to can be used as an application setting.</summary>
		/// <returns>
		///   <see langword="true" /> if the property this attribute is bound to can be used as an application setting; otherwise, <see langword="false" />.</returns>
		public bool RecommendedAsConfigurable { get; }

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
			RecommendedAsConfigurableAttribute recommendedAsConfigurableAttribute = obj as RecommendedAsConfigurableAttribute;
			return recommendedAsConfigurableAttribute != null && recommendedAsConfigurableAttribute.RecommendedAsConfigurable == this.RecommendedAsConfigurable;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A hash code for the current <see cref="T:System.ComponentModel.RecommendedAsConfigurableAttribute" />.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Indicates whether the value of this instance is the default value for the class.</summary>
		/// <returns>
		///   <see langword="true" /> if this instance is the default attribute for the class; otherwise, <see langword="false" />.</returns>
		public override bool IsDefaultAttribute()
		{
			return !this.RecommendedAsConfigurable;
		}

		/// <summary>Specifies that a property cannot be used as an application setting. This <see langword="static" /> field is read-only.</summary>
		public static readonly RecommendedAsConfigurableAttribute No = new RecommendedAsConfigurableAttribute(false);

		/// <summary>Specifies that a property can be used as an application setting. This <see langword="static" /> field is read-only.</summary>
		public static readonly RecommendedAsConfigurableAttribute Yes = new RecommendedAsConfigurableAttribute(true);

		/// <summary>Specifies the default value for the <see cref="T:System.ComponentModel.RecommendedAsConfigurableAttribute" />, which is <see cref="F:System.ComponentModel.RecommendedAsConfigurableAttribute.No" />. This <see langword="static" /> field is read-only.</summary>
		public static readonly RecommendedAsConfigurableAttribute Default = RecommendedAsConfigurableAttribute.No;
	}
}
