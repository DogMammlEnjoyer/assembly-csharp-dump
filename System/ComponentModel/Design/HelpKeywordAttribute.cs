using System;

namespace System.ComponentModel.Design
{
	/// <summary>Specifies the context keyword for a class or member. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
	[Serializable]
	public sealed class HelpKeywordAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.HelpKeywordAttribute" /> class.</summary>
		public HelpKeywordAttribute()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.HelpKeywordAttribute" /> class.</summary>
		/// <param name="keyword">The Help keyword value.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyword" /> is <see langword="null" />.</exception>
		public HelpKeywordAttribute(string keyword)
		{
			if (keyword == null)
			{
				throw new ArgumentNullException("keyword");
			}
			this.HelpKeyword = keyword;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.HelpKeywordAttribute" /> class from the given type.</summary>
		/// <param name="t">The type from which the Help keyword will be taken.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="t" /> is <see langword="null" />.</exception>
		public HelpKeywordAttribute(Type t)
		{
			if (t == null)
			{
				throw new ArgumentNullException("t");
			}
			this.HelpKeyword = t.FullName;
		}

		/// <summary>Gets the Help keyword supplied by this attribute.</summary>
		/// <returns>The Help keyword supplied by this attribute.</returns>
		public string HelpKeyword { get; }

		/// <summary>Determines whether two <see cref="T:System.ComponentModel.Design.HelpKeywordAttribute" /> instances are equal.</summary>
		/// <param name="obj">The <see cref="T:System.ComponentModel.Design.HelpKeywordAttribute" /> to compare with the current <see cref="T:System.ComponentModel.Design.HelpKeywordAttribute" />.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.ComponentModel.Design.HelpKeywordAttribute" /> is equal to the current <see cref="T:System.ComponentModel.Design.HelpKeywordAttribute" />; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return obj == this || (obj != null && obj is HelpKeywordAttribute && ((HelpKeywordAttribute)obj).HelpKeyword == this.HelpKeyword);
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A hash code for the current <see cref="T:System.ComponentModel.Design.HelpKeywordAttribute" />.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Determines whether the Help keyword is <see langword="null" />.</summary>
		/// <returns>
		///   <see langword="true" /> if the Help keyword is <see langword="null" />; otherwise, <see langword="false" />.</returns>
		public override bool IsDefaultAttribute()
		{
			return this.Equals(HelpKeywordAttribute.Default);
		}

		/// <summary>Represents the default value for <see cref="T:System.ComponentModel.Design.HelpKeywordAttribute" />. This field is read-only.</summary>
		public static readonly HelpKeywordAttribute Default = new HelpKeywordAttribute();
	}
}
