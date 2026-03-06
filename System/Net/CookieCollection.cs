using System;
using System.Collections;
using System.Runtime.Serialization;

namespace System.Net
{
	/// <summary>Provides a collection container for instances of the <see cref="T:System.Net.Cookie" /> class.</summary>
	[Serializable]
	public class CookieCollection : ICollection, IEnumerable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.CookieCollection" /> class.</summary>
		public CookieCollection()
		{
			this.m_IsReadOnly = true;
		}

		internal CookieCollection(bool IsReadOnly)
		{
			this.m_IsReadOnly = IsReadOnly;
		}

		/// <summary>Gets a value that indicates whether a <see cref="T:System.Net.CookieCollection" /> is read-only.</summary>
		/// <returns>
		///   <see langword="true" /> if this is a read-only <see cref="T:System.Net.CookieCollection" />; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		public bool IsReadOnly
		{
			get
			{
				return this.m_IsReadOnly;
			}
		}

		/// <summary>Gets the <see cref="T:System.Net.Cookie" /> with a specific index from a <see cref="T:System.Net.CookieCollection" />.</summary>
		/// <param name="index">The zero-based index of the <see cref="T:System.Net.Cookie" /> to be found.</param>
		/// <returns>A <see cref="T:System.Net.Cookie" /> with a specific index from a <see cref="T:System.Net.CookieCollection" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than 0 or <paramref name="index" /> is greater than or equal to <see cref="P:System.Net.CookieCollection.Count" />.</exception>
		public Cookie this[int index]
		{
			get
			{
				if (index < 0 || index >= this.m_list.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return (Cookie)this.m_list[index];
			}
		}

		/// <summary>Gets the <see cref="T:System.Net.Cookie" /> with a specific name from a <see cref="T:System.Net.CookieCollection" />.</summary>
		/// <param name="name">The name of the <see cref="T:System.Net.Cookie" /> to be found.</param>
		/// <returns>The <see cref="T:System.Net.Cookie" /> with a specific name from a <see cref="T:System.Net.CookieCollection" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public Cookie this[string name]
		{
			get
			{
				foreach (object obj in this.m_list)
				{
					Cookie cookie = (Cookie)obj;
					if (string.Compare(cookie.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return cookie;
					}
				}
				return null;
			}
		}

		/// <summary>Adds a <see cref="T:System.Net.Cookie" /> to a <see cref="T:System.Net.CookieCollection" />.</summary>
		/// <param name="cookie">The <see cref="T:System.Net.Cookie" /> to be added to a <see cref="T:System.Net.CookieCollection" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="cookie" /> is <see langword="null" />.</exception>
		public void Add(Cookie cookie)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			this.m_version++;
			int num = this.IndexOf(cookie);
			if (num == -1)
			{
				this.m_list.Add(cookie);
				return;
			}
			this.m_list[num] = cookie;
		}

		/// <summary>Adds the contents of a <see cref="T:System.Net.CookieCollection" /> to the current instance.</summary>
		/// <param name="cookies">The <see cref="T:System.Net.CookieCollection" /> to be added.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="cookies" /> is <see langword="null" />.</exception>
		public void Add(CookieCollection cookies)
		{
			if (cookies == null)
			{
				throw new ArgumentNullException("cookies");
			}
			foreach (object obj in cookies)
			{
				Cookie cookie = (Cookie)obj;
				this.Add(cookie);
			}
		}

		/// <summary>Gets the number of cookies contained in a <see cref="T:System.Net.CookieCollection" />.</summary>
		/// <returns>The number of cookies contained in a <see cref="T:System.Net.CookieCollection" />.</returns>
		public int Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		/// <summary>Gets a value that indicates whether access to a <see cref="T:System.Net.CookieCollection" /> is thread safe.</summary>
		/// <returns>
		///   <see langword="true" /> if access to the <see cref="T:System.Net.CookieCollection" /> is thread safe; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets an object to synchronize access to the <see cref="T:System.Net.CookieCollection" />.</summary>
		/// <returns>An object to synchronize access to the <see cref="T:System.Net.CookieCollection" />.</returns>
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		/// <summary>Copies the elements of a <see cref="T:System.Net.CookieCollection" /> to an instance of the <see cref="T:System.Array" /> class, starting at a particular index.</summary>
		/// <param name="array">The target <see cref="T:System.Array" /> to which the <see cref="T:System.Net.CookieCollection" /> will be copied.</param>
		/// <param name="index">The zero-based index in the target <see cref="T:System.Array" /> where copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="array" /> is multidimensional.  
		/// -or-  
		/// The number of elements in this <see cref="T:System.Net.CookieCollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The elements in this <see cref="T:System.Net.CookieCollection" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
		public void CopyTo(Array array, int index)
		{
			this.m_list.CopyTo(array, index);
		}

		/// <summary>Copies the elements of this <see cref="T:System.Net.CookieCollection" /> to a <see cref="T:System.Net.Cookie" /> array starting at the specified index of the target array.</summary>
		/// <param name="array">The target <see cref="T:System.Net.Cookie" /> array to which the <see cref="T:System.Net.CookieCollection" /> will be copied.</param>
		/// <param name="index">The zero-based index in the target <see cref="T:System.Array" /> where copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="array" /> is multidimensional.  
		/// -or-  
		/// The number of elements in this <see cref="T:System.Net.CookieCollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The elements in this <see cref="T:System.Net.CookieCollection" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
		public void CopyTo(Cookie[] array, int index)
		{
			this.m_list.CopyTo(array, index);
		}

		internal DateTime TimeStamp(CookieCollection.Stamp how)
		{
			switch (how)
			{
			case CookieCollection.Stamp.Set:
				this.m_TimeStamp = DateTime.Now;
				break;
			case CookieCollection.Stamp.SetToUnused:
				this.m_TimeStamp = DateTime.MinValue;
				break;
			case CookieCollection.Stamp.SetToMaxUsed:
				this.m_TimeStamp = DateTime.MaxValue;
				break;
			}
			return this.m_TimeStamp;
		}

		internal bool IsOtherVersionSeen
		{
			get
			{
				return this.m_has_other_versions;
			}
		}

		internal int InternalAdd(Cookie cookie, bool isStrict)
		{
			int result = 1;
			if (isStrict)
			{
				IComparer comparer = Cookie.GetComparer();
				int num = 0;
				foreach (object obj in this.m_list)
				{
					Cookie cookie2 = (Cookie)obj;
					if (comparer.Compare(cookie, cookie2) == 0)
					{
						result = 0;
						if (cookie2.Variant <= cookie.Variant)
						{
							this.m_list[num] = cookie;
							break;
						}
						break;
					}
					else
					{
						num++;
					}
				}
				if (num == this.m_list.Count)
				{
					this.m_list.Add(cookie);
				}
			}
			else
			{
				this.m_list.Add(cookie);
			}
			if (cookie.Version != 1)
			{
				this.m_has_other_versions = true;
			}
			return result;
		}

		internal int IndexOf(Cookie cookie)
		{
			IComparer comparer = Cookie.GetComparer();
			int num = 0;
			foreach (object obj in this.m_list)
			{
				Cookie y = (Cookie)obj;
				if (comparer.Compare(cookie, y) == 0)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		internal void RemoveAt(int idx)
		{
			this.m_list.RemoveAt(idx);
		}

		/// <summary>Gets an enumerator that can iterate through a <see cref="T:System.Net.CookieCollection" />.</summary>
		/// <returns>An instance of an implementation of an <see cref="T:System.Collections.IEnumerator" /> interface that can iterate through a <see cref="T:System.Net.CookieCollection" />.</returns>
		public IEnumerator GetEnumerator()
		{
			return new CookieCollection.CookieCollectionEnumerator(this);
		}

		internal int m_version;

		private ArrayList m_list = new ArrayList();

		private DateTime m_TimeStamp = DateTime.MinValue;

		private bool m_has_other_versions;

		[OptionalField]
		private bool m_IsReadOnly;

		internal enum Stamp
		{
			Check,
			Set,
			SetToUnused,
			SetToMaxUsed
		}

		private class CookieCollectionEnumerator : IEnumerator
		{
			internal CookieCollectionEnumerator(CookieCollection cookies)
			{
				this.m_cookies = cookies;
				this.m_count = cookies.Count;
				this.m_version = cookies.m_version;
			}

			object IEnumerator.Current
			{
				get
				{
					if (this.m_index < 0 || this.m_index >= this.m_count)
					{
						throw new InvalidOperationException(SR.GetString("Enumeration has either not started or has already finished."));
					}
					if (this.m_version != this.m_cookies.m_version)
					{
						throw new InvalidOperationException(SR.GetString("Collection was modified; enumeration operation may not execute."));
					}
					return this.m_cookies[this.m_index];
				}
			}

			bool IEnumerator.MoveNext()
			{
				if (this.m_version != this.m_cookies.m_version)
				{
					throw new InvalidOperationException(SR.GetString("Collection was modified; enumeration operation may not execute."));
				}
				int num = this.m_index + 1;
				this.m_index = num;
				if (num < this.m_count)
				{
					return true;
				}
				this.m_index = this.m_count;
				return false;
			}

			void IEnumerator.Reset()
			{
				this.m_index = -1;
			}

			private CookieCollection m_cookies;

			private int m_count;

			private int m_index = -1;

			private int m_version;
		}
	}
}
