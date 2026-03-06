using System;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling
{
	internal readonly struct PooledObject<T> : IDisposable where T : class
	{
		internal PooledObject(T value, LinkedPool<T> pool)
		{
			this.m_ToReturn = value;
			this.m_Pool = pool;
		}

		void IDisposable.Dispose()
		{
			this.m_Pool.Release(this.m_ToReturn);
		}

		private readonly T m_ToReturn;

		private readonly LinkedPool<T> m_Pool;
	}
}
