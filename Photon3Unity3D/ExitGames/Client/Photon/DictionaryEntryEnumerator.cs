using System;
using System.Collections;
using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public struct DictionaryEntryEnumerator : IEnumerator<DictionaryEntry>, IEnumerator, IDisposable
	{
		public DictionaryEntryEnumerator(Dictionary<object, object>.Enumerator original)
		{
			this.enumerator = original;
		}

		object IEnumerator.Current
		{
			get
			{
				KeyValuePair<object, object> keyValuePair = this.enumerator.Current;
				object key = keyValuePair.Key;
				keyValuePair = this.enumerator.Current;
				return new DictionaryEntry(key, keyValuePair.Value);
			}
		}

		public DictionaryEntry Current
		{
			get
			{
				KeyValuePair<object, object> keyValuePair = this.enumerator.Current;
				object key = keyValuePair.Key;
				keyValuePair = this.enumerator.Current;
				return new DictionaryEntry(key, keyValuePair.Value);
			}
		}

		public object Key
		{
			get
			{
				KeyValuePair<object, object> keyValuePair = this.enumerator.Current;
				return keyValuePair.Key;
			}
		}

		public object Value
		{
			get
			{
				KeyValuePair<object, object> keyValuePair = this.enumerator.Current;
				return keyValuePair.Value;
			}
		}

		public bool MoveNext()
		{
			return this.enumerator.MoveNext();
		}

		public void Reset()
		{
			((IEnumerator)this.enumerator).Reset();
		}

		public void Dispose()
		{
		}

		private Dictionary<object, object>.Enumerator enumerator;
	}
}
