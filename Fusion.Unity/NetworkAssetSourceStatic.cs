using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fusion
{
	[Serializable]
	public class NetworkAssetSourceStatic<T> where T : Object
	{
		[Obsolete("Use Asset instead")]
		public T Prefab
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
			if (this.Object == null)
			{
				throw new InvalidOperationException("Missing static reference");
			}
			return this.Object;
		}

		public string Description
		{
			get
			{
				if (this.Object)
				{
					string str = "Static: ";
					T t = this.Object;
					return str + ((t != null) ? t.ToString() : null);
				}
				return "Static: (null)";
			}
		}

		[FormerlySerializedAs("Prefab")]
		public T Object;
	}
}
