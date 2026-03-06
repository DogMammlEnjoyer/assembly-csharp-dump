using System;

namespace System.Configuration
{
	/// <summary>Provides validation of a string.</summary>
	public class StringValidator : ConfigurationValidatorBase
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.StringValidator" /> class, based on a supplied parameter.</summary>
		/// <param name="minLength">An integer that specifies the minimum length of the string value.</param>
		public StringValidator(int minLength)
		{
			this.minLength = minLength;
			this.maxLength = int.MaxValue;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.StringValidator" /> class, based on supplied parameters.</summary>
		/// <param name="minLength">An integer that specifies the minimum length of the string value.</param>
		/// <param name="maxLength">An integer that specifies the maximum length of the string value.</param>
		public StringValidator(int minLength, int maxLength)
		{
			this.minLength = minLength;
			this.maxLength = maxLength;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.StringValidator" /> class, based on supplied parameters.</summary>
		/// <param name="minLength">An integer that specifies the minimum length of the string value.</param>
		/// <param name="maxLength">An integer that specifies the maximum length of the string value.</param>
		/// <param name="invalidCharacters">A string that represents invalid characters.</param>
		public StringValidator(int minLength, int maxLength, string invalidCharacters)
		{
			this.minLength = minLength;
			this.maxLength = maxLength;
			if (invalidCharacters != null)
			{
				this.invalidCharacters = invalidCharacters.ToCharArray();
			}
		}

		/// <summary>Determines whether an object can be validated based on type.</summary>
		/// <param name="type">The object type.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="type" /> parameter matches a string; otherwise, <see langword="false" />.</returns>
		public override bool CanValidate(Type type)
		{
			return type == typeof(string);
		}

		/// <summary>Determines whether the value of an object is valid.</summary>
		/// <param name="value">The object value.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="value" /> is less than <paramref name="minValue" /> or greater than <paramref name="maxValue" /> as defined in the constructor.  
		/// -or-
		///  <paramref name="value" /> contains invalid characters.</exception>
		public override void Validate(object value)
		{
			if (value == null && this.minLength <= 0)
			{
				return;
			}
			string text = (string)value;
			if (text == null || text.Length < this.minLength)
			{
				throw new ArgumentException("The string must be at least " + this.minLength.ToString() + " characters long.");
			}
			if (text.Length > this.maxLength)
			{
				throw new ArgumentException("The string must be no more than " + this.maxLength.ToString() + " characters long.");
			}
			if (this.invalidCharacters != null && text.IndexOfAny(this.invalidCharacters) != -1)
			{
				throw new ArgumentException(string.Format("The string cannot contain any of the following characters: '{0}'.", this.invalidCharacters));
			}
		}

		private char[] invalidCharacters;

		private int maxLength;

		private int minLength;
	}
}
