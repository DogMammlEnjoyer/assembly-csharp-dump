using System;
using System.Collections;
using System.Collections.Generic;

namespace WebSocketSharp.Net
{
	public class HttpListenerPrefixCollection : ICollection<string>, IEnumerable<string>, IEnumerable
	{
		internal HttpListenerPrefixCollection(HttpListener listener)
		{
			this._listener = listener;
			this._prefixes = new List<string>();
		}

		public int Count
		{
			get
			{
				return this._prefixes.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public void Add(string uriPrefix)
		{
			this._listener.CheckDisposed();
			HttpListenerPrefix.CheckPrefix(uriPrefix);
			bool flag = this._prefixes.Contains(uriPrefix);
			if (!flag)
			{
				bool isListening = this._listener.IsListening;
				if (isListening)
				{
					EndPointManager.AddPrefix(uriPrefix, this._listener);
				}
				this._prefixes.Add(uriPrefix);
			}
		}

		public void Clear()
		{
			this._listener.CheckDisposed();
			bool isListening = this._listener.IsListening;
			if (isListening)
			{
				EndPointManager.RemoveListener(this._listener);
			}
			this._prefixes.Clear();
		}

		public bool Contains(string uriPrefix)
		{
			this._listener.CheckDisposed();
			bool flag = uriPrefix == null;
			if (flag)
			{
				throw new ArgumentNullException("uriPrefix");
			}
			return this._prefixes.Contains(uriPrefix);
		}

		public void CopyTo(string[] array, int offset)
		{
			this._listener.CheckDisposed();
			this._prefixes.CopyTo(array, offset);
		}

		public IEnumerator<string> GetEnumerator()
		{
			return this._prefixes.GetEnumerator();
		}

		public bool Remove(string uriPrefix)
		{
			this._listener.CheckDisposed();
			bool flag = uriPrefix == null;
			if (flag)
			{
				throw new ArgumentNullException("uriPrefix");
			}
			bool flag2 = !this._prefixes.Contains(uriPrefix);
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool isListening = this._listener.IsListening;
				if (isListening)
				{
					EndPointManager.RemovePrefix(uriPrefix, this._listener);
				}
				result = this._prefixes.Remove(uriPrefix);
			}
			return result;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._prefixes.GetEnumerator();
		}

		private HttpListener _listener;

		private List<string> _prefixes;
	}
}
