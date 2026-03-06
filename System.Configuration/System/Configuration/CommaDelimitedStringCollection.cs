using System;
using System.Collections.Specialized;

namespace System.Configuration
{
	/// <summary>Represents a collection of string elements separated by commas. This class cannot be inherited.</summary>
	public sealed class CommaDelimitedStringCollection : StringCollection
	{
		/// <summary>Gets a value that specifies whether the collection has been modified.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.CommaDelimitedStringCollection" /> has been modified; otherwise, <see langword="false" />.</returns>
		public bool IsModified
		{
			get
			{
				if (this.modified)
				{
					return true;
				}
				string text = this.ToString();
				return text != null && text.GetHashCode() != this.originalStringHash;
			}
		}

		/// <summary>Gets a value indicating whether the collection object is read-only.</summary>
		/// <returns>
		///   <see langword="true" /> if the specified string element in the <see cref="T:System.Configuration.CommaDelimitedStringCollection" /> is read-only; otherwise, <see langword="false" />.</returns>
		public new bool IsReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		/// <summary>Gets or sets a string element in the collection based on the index.</summary>
		/// <param name="index">The index of the string element in the collection.</param>
		/// <returns>A string element in the collection.</returns>
		public new string this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				if (this.readOnly)
				{
					throw new ConfigurationErrorsException("The configuration is read only");
				}
				base[index] = value;
				this.modified = true;
			}
		}

		/// <summary>Adds a string to the comma-delimited collection.</summary>
		/// <param name="value">A string value.</param>
		public new void Add(string value)
		{
			if (this.readOnly)
			{
				throw new ConfigurationErrorsException("The configuration is read only");
			}
			base.Add(value);
			this.modified = true;
		}

		/// <summary>Adds all the strings in a string array to the collection.</summary>
		/// <param name="range">An array of strings to add to the collection.</param>
		public new void AddRange(string[] range)
		{
			if (this.readOnly)
			{
				throw new ConfigurationErrorsException("The configuration is read only");
			}
			base.AddRange(range);
			this.modified = true;
		}

		/// <summary>Clears the collection.</summary>
		public new void Clear()
		{
			if (this.readOnly)
			{
				throw new ConfigurationErrorsException("The configuration is read only");
			}
			base.Clear();
			this.modified = true;
		}

		/// <summary>Creates a copy of the collection.</summary>
		/// <returns>A copy of the <see cref="T:System.Configuration.CommaDelimitedStringCollection" />.</returns>
		public CommaDelimitedStringCollection Clone()
		{
			CommaDelimitedStringCollection commaDelimitedStringCollection = new CommaDelimitedStringCollection();
			string[] array = new string[base.Count];
			base.CopyTo(array, 0);
			commaDelimitedStringCollection.AddRange(array);
			commaDelimitedStringCollection.originalStringHash = this.originalStringHash;
			return commaDelimitedStringCollection;
		}

		/// <summary>Adds a string element to the collection at the specified index.</summary>
		/// <param name="index">The index in the collection at which the new element will be added.</param>
		/// <param name="value">The value of the new element to add to the collection.</param>
		public new void Insert(int index, string value)
		{
			if (this.readOnly)
			{
				throw new ConfigurationErrorsException("The configuration is read only");
			}
			base.Insert(index, value);
			this.modified = true;
		}

		/// <summary>Removes a string element from the collection.</summary>
		/// <param name="value">The string to remove.</param>
		public new void Remove(string value)
		{
			if (this.readOnly)
			{
				throw new ConfigurationErrorsException("The configuration is read only");
			}
			base.Remove(value);
			this.modified = true;
		}

		/// <summary>Sets the collection object to read-only.</summary>
		public void SetReadOnly()
		{
			this.readOnly = true;
		}

		/// <summary>Returns a string representation of the object.</summary>
		/// <returns>A string representation of the object.</returns>
		public override string ToString()
		{
			if (base.Count == 0)
			{
				return null;
			}
			string[] array = new string[base.Count];
			base.CopyTo(array, 0);
			return string.Join(",", array);
		}

		internal void UpdateStringHash()
		{
			string text = this.ToString();
			if (text == null)
			{
				this.originalStringHash = 0;
				return;
			}
			this.originalStringHash = text.GetHashCode();
		}

		private bool modified;

		private bool readOnly;

		private int originalStringHash;
	}
}
