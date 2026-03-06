using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Fusion
{
	[Serializable]
	public class NetworkAssetSourceAddressable<T> where T : Object
	{
		[Obsolete("Use RuntimeKey instead")]
		public AssetReference Address
		{
			get
			{
				if (string.IsNullOrEmpty(this.RuntimeKey))
				{
					return null;
				}
				return FusionAddressablesUtils.CreateAssetReference(this.RuntimeKey);
			}
			set
			{
				if (value.IsValid())
				{
					this.RuntimeKey = (string)value.RuntimeKey;
					return;
				}
				this.RuntimeKey = string.Empty;
			}
		}

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
				return this._op.IsDone;
			}
		}

		public T WaitForResult()
		{
			if (!this._op.IsDone)
			{
				try
				{
					this._op.WaitForCompletion();
				}
				catch (Exception ex) when (!Application.isPlaying && typeof(Exception) == ex.GetType())
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log("An exception was thrown when loading asset: " + this.RuntimeKey + "; since this method was called from the editor, it may be due to the fact that Addressables don't have edit-time load support. Please use EditorInstance instead.");
					}
					throw;
				}
			}
			if (this._op.OperationException != null)
			{
				throw new InvalidOperationException("Failed to load asset: " + this.RuntimeKey, this._op.OperationException);
			}
			return this.ValidateResult(this._op.Result);
		}

		private void LoadInternal(bool synchronous)
		{
			this._op = Addressables.LoadAssetAsync<Object>(this.RuntimeKey);
			if (!this._op.IsValid())
			{
				throw new Exception("Failed to load asset: " + this.RuntimeKey);
			}
			if (this._op.Status == AsyncOperationStatus.Failed)
			{
				throw new Exception("Failed to load asset: " + this.RuntimeKey, this._op.OperationException);
			}
			if (synchronous)
			{
				this._op.WaitForCompletion();
			}
		}

		private void UnloadInternal()
		{
			if (this._op.IsValid())
			{
				AsyncOperationHandle op = this._op;
				this._op = default(AsyncOperationHandle);
				Addressables.Release(op);
			}
		}

		private T ValidateResult(object result)
		{
			if (result == null)
			{
				throw new InvalidOperationException("Failed to load asset: " + this.RuntimeKey + "; asset is null");
			}
			if (typeof(T).IsSubclassOf(typeof(Component)))
			{
				if (!(result is GameObject))
				{
					throw new InvalidOperationException(string.Format("Failed to load asset: {0}; asset is not a GameObject, but a {1}", this.RuntimeKey, result.GetType()));
				}
				T component = ((GameObject)result).GetComponent<T>();
				if (!component)
				{
					throw new InvalidOperationException(string.Format("Failed to load asset: {0}; asset does not contain component {1}", this.RuntimeKey, typeof(T)));
				}
				return component;
			}
			else
			{
				T t = result as T;
				if (t != null)
				{
					return t;
				}
				throw new InvalidOperationException(string.Format("Failed to load asset: {0}; asset is not of type {1}, but {2}", this.RuntimeKey, typeof(T), result.GetType()));
			}
		}

		public string Description
		{
			get
			{
				return "RuntimeKey: " + this.RuntimeKey;
			}
		}

		[UnityAddressablesRuntimeKey]
		public string RuntimeKey;

		[NonSerialized]
		private int _acquireCount;

		[NonSerialized]
		private AsyncOperationHandle _op;
	}
}
