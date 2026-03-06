using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity;

namespace System.Text.RegularExpressions
{
	/// <summary>Represents the set of successful matches found by iteratively applying a regular expression pattern to the input string.</summary>
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(CollectionDebuggerProxy<Match>))]
	[Serializable]
	public class MatchCollection : IList<Match>, ICollection<Match>, IEnumerable<Match>, IEnumerable, IReadOnlyList<Match>, IReadOnlyCollection<Match>, IList, ICollection
	{
		internal MatchCollection(Regex regex, string input, int beginning, int length, int startat)
		{
			if (startat < 0 || startat > input.Length)
			{
				throw new ArgumentOutOfRangeException("startat", "Start index cannot be less than 0 or greater than input length.");
			}
			this._regex = regex;
			this._input = input;
			this._beginning = beginning;
			this._length = length;
			this._startat = startat;
			this._prevlen = -1;
			this._matches = new List<Match>();
			this._done = false;
		}

		/// <summary>Gets a value that indicates whether the collection is read only.</summary>
		/// <returns>
		///   <see langword="true" /> in all cases.</returns>
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>Gets the number of matches.</summary>
		/// <returns>The number of matches.</returns>
		/// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred.</exception>
		public int Count
		{
			get
			{
				this.EnsureInitialized();
				return this._matches.Count;
			}
		}

		/// <summary>Gets an individual member of the collection.</summary>
		/// <param name="i">Index into the <see cref="T:System.Text.RegularExpressions.Match" /> collection.</param>
		/// <returns>The captured substring at position <paramref name="i" /> in the collection.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="i" /> is less than 0 or greater than or equal to <see cref="P:System.Text.RegularExpressions.MatchCollection.Count" />.</exception>
		/// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred.</exception>
		public virtual Match this[int i]
		{
			get
			{
				if (i < 0)
				{
					throw new ArgumentOutOfRangeException("i");
				}
				Match match = this.GetMatch(i);
				if (match == null)
				{
					throw new ArgumentOutOfRangeException("i");
				}
				return match;
			}
		}

		/// <summary>Provides an enumerator that iterates through the collection.</summary>
		/// <returns>An object that contains all <see cref="T:System.Text.RegularExpressions.Match" /> objects within the <see cref="T:System.Text.RegularExpressions.MatchCollection" />.</returns>
		/// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred.</exception>
		public IEnumerator GetEnumerator()
		{
			return new MatchCollection.Enumerator(this);
		}

		IEnumerator<Match> IEnumerable<Match>.GetEnumerator()
		{
			return new MatchCollection.Enumerator(this);
		}

		private Match GetMatch(int i)
		{
			if (this._matches.Count > i)
			{
				return this._matches[i];
			}
			if (this._done)
			{
				return null;
			}
			for (;;)
			{
				Match match = this._regex.Run(false, this._prevlen, this._input, this._beginning, this._length, this._startat);
				if (!match.Success)
				{
					break;
				}
				this._matches.Add(match);
				this._prevlen = match.Length;
				this._startat = match._textpos;
				if (this._matches.Count > i)
				{
					return match;
				}
			}
			this._done = true;
			return null;
		}

		private void EnsureInitialized()
		{
			if (!this._done)
			{
				this.GetMatch(int.MaxValue);
			}
		}

		/// <summary>Gets a value indicating whether access to the collection is synchronized (thread-safe).</summary>
		/// <returns>
		///   <see langword="false" /> in all cases.</returns>
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets an object that can be used to synchronize access to the collection.</summary>
		/// <returns>An object that can be used to synchronize access to the collection. This property always returns the object itself.</returns>
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		/// <summary>Copies all the elements of the collection to the given array starting at the given index.</summary>
		/// <param name="array">The array the collection is to be copied into.</param>
		/// <param name="arrayIndex">The position in the array where copying is to begin.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="array" /> is a multi-dimensional array.</exception>
		/// <exception cref="T:System.IndexOutOfRangeException">
		///   <paramref name="arrayIndex" /> is outside the bounds of <paramref name="array" />.  
		/// -or-  
		/// <paramref name="arrayIndex" /> plus <see cref="P:System.Text.RegularExpressions.MatchCollection.Count" /> is outside the bounds of <paramref name="array" />.</exception>
		/// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred.</exception>
		public void CopyTo(Array array, int arrayIndex)
		{
			this.EnsureInitialized();
			((ICollection)this._matches).CopyTo(array, arrayIndex);
		}

		public void CopyTo(Match[] array, int arrayIndex)
		{
			this.EnsureInitialized();
			this._matches.CopyTo(array, arrayIndex);
		}

		int IList<Match>.IndexOf(Match item)
		{
			this.EnsureInitialized();
			return this._matches.IndexOf(item);
		}

		void IList<Match>.Insert(int index, Match item)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void IList<Match>.RemoveAt(int index)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		Match IList<Match>.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotSupportedException("Collection is read-only.");
			}
		}

		void ICollection<Match>.Add(Match item)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void ICollection<Match>.Clear()
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		bool ICollection<Match>.Contains(Match item)
		{
			this.EnsureInitialized();
			return this._matches.Contains(item);
		}

		bool ICollection<Match>.Remove(Match item)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void IList.Clear()
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		bool IList.Contains(object value)
		{
			return value is Match && ((ICollection<Match>)this).Contains((Match)value);
		}

		int IList.IndexOf(object value)
		{
			if (!(value is Match))
			{
				return -1;
			}
			return ((IList<Match>)this).IndexOf((Match)value);
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		bool IList.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotSupportedException("Collection is read-only.");
			}
		}

		internal MatchCollection()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly Regex _regex;

		private readonly List<Match> _matches;

		private bool _done;

		private readonly string _input;

		private readonly int _beginning;

		private readonly int _length;

		private int _startat;

		private int _prevlen;

		[Serializable]
		private sealed class Enumerator : IEnumerator<Match>, IDisposable, IEnumerator
		{
			internal Enumerator(MatchCollection collection)
			{
				this._collection = collection;
				this._index = -1;
			}

			public bool MoveNext()
			{
				if (this._index == -2)
				{
					return false;
				}
				this._index++;
				if (this._collection.GetMatch(this._index) == null)
				{
					this._index = -2;
					return false;
				}
				return true;
			}

			public Match Current
			{
				get
				{
					if (this._index < 0)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return this._collection.GetMatch(this._index);
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			void IEnumerator.Reset()
			{
				this._index = -1;
			}

			void IDisposable.Dispose()
			{
			}

			private readonly MatchCollection _collection;

			private int _index;
		}
	}
}
