using System;

namespace UnityEngine.VFX
{
	[Serializable]
	internal abstract class EventAttributeValue<T> : EventAttribute
	{
		protected EventAttributeValue(Func<VFXEventAttribute, int, bool> hasFunc, Action<VFXEventAttribute, int, T> applyFunc)
		{
			this.m_HasFunc = hasFunc;
			this.m_ApplyFunc = applyFunc;
		}

		public sealed override bool ApplyToVFX(VFXEventAttribute eventAttribute)
		{
			if (!this.m_HasFunc(eventAttribute, this.id))
			{
				return false;
			}
			this.m_ApplyFunc(eventAttribute, this.id, this.value);
			return true;
		}

		private readonly Func<VFXEventAttribute, int, bool> m_HasFunc;

		private readonly Action<VFXEventAttribute, int, T> m_ApplyFunc;

		public T value;
	}
}
