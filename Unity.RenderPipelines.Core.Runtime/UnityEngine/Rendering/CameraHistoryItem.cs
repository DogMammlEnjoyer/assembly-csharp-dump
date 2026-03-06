using System;

namespace UnityEngine.Rendering
{
	public abstract class CameraHistoryItem : ContextItem
	{
		public virtual void OnCreate(BufferedRTHandleSystem owner, uint typeId)
		{
			this.m_owner = owner;
			this.m_TypeId = typeId;
		}

		protected BufferedRTHandleSystem storage
		{
			get
			{
				return this.m_owner;
			}
		}

		protected int MakeId(uint index)
		{
			return (int)((this.m_TypeId & 65535U) << 16 | (index & 65535U));
		}

		protected RTHandle AllocHistoryFrameRT(int id, int count, ref RenderTextureDescriptor desc, string name = "")
		{
			return this.AllocHistoryFrameRT(id, count, ref desc, FilterMode.Bilinear, name);
		}

		protected RTHandle AllocHistoryFrameRT(int id, int count, ref RenderTextureDescriptor desc, FilterMode filterMode, string name = "")
		{
			this.m_owner.AllocBuffer(id, count, ref desc, filterMode, TextureWrapMode.Clamp, false, 0, 0f, name);
			return this.GetCurrentFrameRT(0);
		}

		protected void ReleaseHistoryFrameRT(int id)
		{
			this.m_owner.ReleaseBuffer(id);
		}

		protected RTHandle GetPreviousFrameRT(int id)
		{
			return this.m_owner.GetFrameRT(id, 1);
		}

		protected RTHandle GetCurrentFrameRT(int id)
		{
			return this.m_owner.GetFrameRT(id, 0);
		}

		private BufferedRTHandleSystem m_owner;

		private uint m_TypeId = uint.MaxValue;
	}
}
