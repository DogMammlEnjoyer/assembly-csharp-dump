using System;
using System.Text;

namespace System
{
	/// <summary>Provides extension methods to work with string normalization.</summary>
	public static class StringNormalizationExtensions
	{
		/// <summary>Indicates whether the specified string is in Unicode normalization form C.</summary>
		/// <param name="value">A string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is in normalization form C; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="value" /> contains invalid Unicode characters.</exception>
		public static bool IsNormalized(this string strInput)
		{
			return strInput.IsNormalized(NormalizationForm.FormC);
		}

		/// <summary>Indicates whether a string is in a specified Unicode normalization form.</summary>
		/// <param name="value">A string.</param>
		/// <param name="normalizationForm">A Unicode normalization form.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is in normalization form <paramref name="normalizationForm" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="value" /> contains invalid Unicode characters.</exception>
		public static bool IsNormalized(this string strInput, NormalizationForm normalizationForm)
		{
			if (strInput == null)
			{
				throw new ArgumentNullException("strInput");
			}
			return strInput.IsNormalized(normalizationForm);
		}

		/// <summary>Normalizes a string to a Unicode normalization form C.</summary>
		/// <param name="value">The string to normalize.</param>
		/// <returns>A new string whose textual value is the same as <paramref name="value" /> but whose binary representation is in Unicode normalization form C.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="value" /> contains invalid Unicode characters.</exception>
		public static string Normalize(this string strInput)
		{
			return strInput.Normalize(NormalizationForm.FormC);
		}

		/// <summary>Normalizes a string to the specified Unicode normalization form.</summary>
		/// <param name="value">The string to normalize.</param>
		/// <param name="normalizationForm">The Unicode normalization form.</param>
		/// <returns>A new string whose textual value is the same as <paramref name="value" /> but whose binary representation is in the <paramref name="normalizationForm" /> normalization form.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="value" /> contains invalid Unicode characters.</exception>
		public static string Normalize(this string strInput, NormalizationForm normalizationForm)
		{
			if (strInput == null)
			{
				throw new ArgumentNullException("strInput");
			}
			return strInput.Normalize(normalizationForm);
		}
	}
}
