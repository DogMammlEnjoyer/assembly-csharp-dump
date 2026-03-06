using System;
using System.Globalization;

namespace System.Collections
{
	/// <summary>Supplies a hash code for an object, using a hashing algorithm that ignores the case of strings.</summary>
	[Obsolete("Please use StringComparer instead.")]
	[Serializable]
	public class CaseInsensitiveHashCodeProvider : IHashCodeProvider
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.CaseInsensitiveHashCodeProvider" /> class using the <see cref="P:System.Threading.Thread.CurrentCulture" /> of the current thread.</summary>
		public CaseInsensitiveHashCodeProvider()
		{
			this._compareInfo = CultureInfo.CurrentCulture.CompareInfo;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.CaseInsensitiveHashCodeProvider" /> class using the specified <see cref="T:System.Globalization.CultureInfo" />.</summary>
		/// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use for the new <see cref="T:System.Collections.CaseInsensitiveHashCodeProvider" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="culture" /> is <see langword="null" />.</exception>
		public CaseInsensitiveHashCodeProvider(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			this._compareInfo = culture.CompareInfo;
		}

		/// <summary>Gets an instance of <see cref="T:System.Collections.CaseInsensitiveHashCodeProvider" /> that is associated with the <see cref="P:System.Threading.Thread.CurrentCulture" /> of the current thread and that is always available.</summary>
		/// <returns>An instance of <see cref="T:System.Collections.CaseInsensitiveHashCodeProvider" /> that is associated with the <see cref="P:System.Threading.Thread.CurrentCulture" /> of the current thread.</returns>
		public static CaseInsensitiveHashCodeProvider Default
		{
			get
			{
				return new CaseInsensitiveHashCodeProvider();
			}
		}

		/// <summary>Gets an instance of <see cref="T:System.Collections.CaseInsensitiveHashCodeProvider" /> that is associated with <see cref="P:System.Globalization.CultureInfo.InvariantCulture" /> and that is always available.</summary>
		/// <returns>An instance of <see cref="T:System.Collections.CaseInsensitiveHashCodeProvider" /> that is associated with <see cref="P:System.Globalization.CultureInfo.InvariantCulture" />.</returns>
		public static CaseInsensitiveHashCodeProvider DefaultInvariant
		{
			get
			{
				CaseInsensitiveHashCodeProvider result;
				if ((result = CaseInsensitiveHashCodeProvider.s_invariantCaseInsensitiveHashCodeProvider) == null)
				{
					result = (CaseInsensitiveHashCodeProvider.s_invariantCaseInsensitiveHashCodeProvider = new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture));
				}
				return result;
			}
		}

		/// <summary>Returns a hash code for the given object, using a hashing algorithm that ignores the case of strings.</summary>
		/// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
		/// <returns>A hash code for the given object, using a hashing algorithm that ignores the case of strings.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="obj" /> is <see langword="null" />.</exception>
		public int GetHashCode(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			string text = obj as string;
			if (text == null)
			{
				return obj.GetHashCode();
			}
			return this._compareInfo.GetHashCode(text, CompareOptions.IgnoreCase);
		}

		private static volatile CaseInsensitiveHashCodeProvider s_invariantCaseInsensitiveHashCodeProvider;

		private readonly CompareInfo _compareInfo;
	}
}
