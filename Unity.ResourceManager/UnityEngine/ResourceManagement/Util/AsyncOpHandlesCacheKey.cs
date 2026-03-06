using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.ResourceManagement.Util
{
	internal sealed class AsyncOpHandlesCacheKey : IOperationCacheKey, IEquatable<IOperationCacheKey>
	{
		public AsyncOpHandlesCacheKey(IList<AsyncOperationHandle> handles)
		{
			this.m_Handles = new HashSet<AsyncOperationHandle>(handles);
		}

		public override int GetHashCode()
		{
			return this.m_Handles.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as AsyncOpHandlesCacheKey);
		}

		public bool Equals(IOperationCacheKey other)
		{
			return this.Equals(other as AsyncOpHandlesCacheKey);
		}

		private bool Equals(AsyncOpHandlesCacheKey other)
		{
			return this == other || (other != null && this.m_Handles.SetEquals(other.m_Handles));
		}

		private readonly HashSet<AsyncOperationHandle> m_Handles;
	}
}
