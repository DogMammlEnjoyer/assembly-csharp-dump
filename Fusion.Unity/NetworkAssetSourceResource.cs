using System;
using System.Runtime.ExceptionServices;
using UnityEngine;

namespace Fusion
{
	[Serializable]
	public class NetworkAssetSourceResource<T> where T : Object
	{
		public void Acquire(bool synchronous)
		{
			if (this._acquireCount == 0)
			{
				this.LoadInternal(synchronous);
			}
			this._acquireCount++;
		}

		public void Release()
		{
			if (this._acquireCount <= 0)
			{
				throw new Exception("Asset is not loaded");
			}
			int num = this._acquireCount - 1;
			this._acquireCount = num;
			if (num == 0)
			{
				this.UnloadInternal();
			}
		}

		public bool IsCompleted
		{
			get
			{
				if (this._state == null)
				{
					return false;
				}
				ResourceRequest resourceRequest = this._state as ResourceRequest;
				return resourceRequest == null || resourceRequest.isDone;
			}
		}

		public T WaitForResult()
		{
			ResourceRequest resourceRequest = this._state as ResourceRequest;
			if (resourceRequest != null)
			{
				if (resourceRequest.isDone)
				{
					this.FinishAsyncOp(resourceRequest);
				}
				else
				{
					this._state = null;
					this.LoadInternal(true);
				}
			}
			if (this._state == null)
			{
				throw new InvalidOperationException(string.Format("Failed to load asset {0}: {1}[{2}]. Asset is null.", typeof(T), this.ResourcePath, this.SubObjectName));
			}
			T t = this._state as T;
			if (t != null)
			{
				return t;
			}
			ExceptionDispatchInfo exceptionDispatchInfo = this._state as ExceptionDispatchInfo;
			if (exceptionDispatchInfo != null)
			{
				exceptionDispatchInfo.Throw();
				throw new NotSupportedException();
			}
			throw new InvalidOperationException(string.Format("Failed to load asset {0}: {1}, SubObjectName: {2}", typeof(T), this.ResourcePath, this.SubObjectName));
		}

		private void FinishAsyncOp(ResourceRequest asyncOp)
		{
			try
			{
				Object @object = string.IsNullOrEmpty(this.SubObjectName) ? asyncOp.asset : NetworkAssetSourceResource<T>.LoadNamedResource(this.ResourcePath, this.SubObjectName);
				if (!@object)
				{
					throw new InvalidOperationException("Missing Resource: " + this.ResourcePath + ", SubObjectName: " + this.SubObjectName);
				}
				this._state = @object;
			}
			catch (Exception source)
			{
				this._state = ExceptionDispatchInfo.Capture(source);
			}
		}

		private static T LoadNamedResource(string resoucePath, string subObjectName)
		{
			foreach (T t in Resources.LoadAll<T>(resoucePath))
			{
				if (string.Equals(t.name, subObjectName, StringComparison.Ordinal))
				{
					return t;
				}
			}
			return default(T);
		}

		private void LoadInternal(bool synchronous)
		{
			try
			{
				if (synchronous)
				{
					this._state = (string.IsNullOrEmpty(this.SubObjectName) ? Resources.Load<T>(this.ResourcePath) : NetworkAssetSourceResource<T>.LoadNamedResource(this.ResourcePath, this.SubObjectName));
				}
				else
				{
					this._state = Resources.LoadAsync<T>(this.ResourcePath);
				}
				if (this._state == null)
				{
					this._state = new InvalidOperationException("Missing Resource: " + this.ResourcePath + ", SubObjectName: " + this.SubObjectName);
				}
			}
			catch (Exception source)
			{
				this._state = ExceptionDispatchInfo.Capture(source);
			}
		}

		private void UnloadInternal()
		{
			ResourceRequest resourceRequest = this._state as ResourceRequest;
			if (resourceRequest != null)
			{
				resourceRequest.completed += delegate(AsyncOperation op)
				{
				};
			}
			else
			{
				Object @object = this._state as Object;
			}
			this._state = null;
		}

		public string Description
		{
			get
			{
				return "Resource: " + this.ResourcePath + ((!string.IsNullOrEmpty(this.SubObjectName)) ? ("[" + this.SubObjectName + "]") : "");
			}
		}

		[UnityResourcePath(typeof(Object))]
		public string ResourcePath;

		public string SubObjectName;

		[NonSerialized]
		private object _state;

		[NonSerialized]
		private int _acquireCount;
	}
}
