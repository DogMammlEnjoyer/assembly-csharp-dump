using System;
using System.Collections;
using System.Collections.Generic;
using Unity;

namespace System.Net.Http.Headers
{
	/// <summary>Represents a collection of header values.</summary>
	/// <typeparam name="T">The header collection type.</typeparam>
	public sealed class HttpHeaderValueCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T : class
	{
		internal HttpHeaderValueCollection(HttpHeaders headers, HeaderInfo headerInfo)
		{
			this.list = new List<T>();
			this.headers = headers;
			this.headerInfo = headerInfo;
		}

		/// <summary>Gets the number of headers in the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</summary>
		/// <returns>The number of headers in a collection</returns>
		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		internal List<string> InvalidValues
		{
			get
			{
				return this.invalidValues;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> instance is read-only.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> instance is read-only; otherwise, <see langword="false" />.</returns>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>Adds an entry to the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</summary>
		/// <param name="item">The item to add to the header collection.</param>
		public void Add(T item)
		{
			this.list.Add(item);
		}

		internal void AddRange(List<T> values)
		{
			this.list.AddRange(values);
		}

		internal void AddInvalidValue(string invalidValue)
		{
			if (this.invalidValues == null)
			{
				this.invalidValues = new List<string>();
			}
			this.invalidValues.Add(invalidValue);
		}

		/// <summary>Removes all entries from the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</summary>
		public void Clear()
		{
			this.list.Clear();
			this.invalidValues = null;
		}

		/// <summary>Determines if the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> contains an item.</summary>
		/// <param name="item">The item to find to the header collection.</param>
		/// <returns>
		///   <see langword="true" /> if the entry is contained in the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> instance; otherwise, <see langword="false" /></returns>
		public bool Contains(T item)
		{
			return this.list.Contains(item);
		}

		/// <summary>Copies the entire <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> to a compatible one-dimensional <see cref="T:System.Array" />, starting at the specified index of the target array.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			this.list.CopyTo(array, arrayIndex);
		}

		/// <summary>Parses and adds an entry to the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</summary>
		/// <param name="input">The entry to add.</param>
		public void ParseAdd(string input)
		{
			this.headers.AddValue(input, this.headerInfo, false);
		}

		/// <summary>Removes the specified item from the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</summary>
		/// <param name="item">The item to remove.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="item" /> was removed from the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> instance; otherwise, <see langword="false" /></returns>
		public bool Remove(T item)
		{
			return this.list.Remove(item);
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> object. object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			string text = string.Join<T>(this.headerInfo.Separator, this.list);
			if (this.invalidValues != null)
			{
				text += string.Join(this.headerInfo.Separator, this.invalidValues);
			}
			return text;
		}

		/// <summary>Determines whether the input could be parsed and added to the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</summary>
		/// <param name="input">The entry to validate.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="input" /> could be parsed and added to the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> instance; otherwise, <see langword="false" /></returns>
		public bool TryParseAdd(string input)
		{
			return this.headers.AddValue(input, this.headerInfo, true);
		}

		/// <summary>Returns an enumerator that iterates through the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</summary>
		/// <returns>An enumerator for the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> instance.</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		/// <summary>Returns an enumerator that iterates through the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</summary>
		/// <returns>An enumerator for the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> instance.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal T Find(Predicate<T> predicate)
		{
			return this.list.Find(predicate);
		}

		internal void Remove(Predicate<T> predicate)
		{
			T t = this.Find(predicate);
			if (t != null)
			{
				this.Remove(t);
			}
		}

		internal HttpHeaderValueCollection()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly List<T> list;

		private readonly HttpHeaders headers;

		private readonly HeaderInfo headerInfo;

		private List<string> invalidValues;
	}
}
