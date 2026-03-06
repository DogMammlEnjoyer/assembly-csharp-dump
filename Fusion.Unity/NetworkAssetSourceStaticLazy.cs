using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fusion
{
	[Serializable]
	public class NetworkAssetSourceStaticLazy<T> where T : Object
	{
		[Obsolete("Use Object instead")]
		public LazyLoadReference<T> Prefab
		{
			get
			{
				return this.Object;
			}
			set
			{
				this.Object = value;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return true;
			}
		}

		public void Acquire(bool synchronous)
		{
		}

		public void Release()
		{
		}

		public T WaitForResult()
		{
			if (this.Object.asset == null)
			{
				throw new InvalidOperationException("Missing static reference");
			}
			return this.Object.asset;
		}

		public string Description
		{
			get
			{
				if (this.Object.isBroken)
				{
					return "Static: (broken)";
				}
				if (this.Object.isSet)
				{
					string str = "Static: ";
					T t = this.Object.asset;
					return str + ((t != null) ? t.ToString() : null);
				}
				return "Static: (null)";
			}
		}

		[FormerlySerializedAs("Prefab")]
		public LazyLoadReference<T> Object;
	}
}
