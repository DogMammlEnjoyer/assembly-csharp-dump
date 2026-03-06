using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
	internal abstract class SingleHistoryBase : CameraHistoryItem
	{
		public override void OnCreate(BufferedRTHandleSystem owner, uint typeId)
		{
			base.OnCreate(owner, typeId);
			this.m_Id = base.MakeId(0U);
		}

		public RTHandle GetTexture(int frameIndex = 0)
		{
			if ((ulong)frameIndex >= (ulong)((long)this.GetHistoryFrameCount()))
			{
				return null;
			}
			return base.storage.GetFrameRT(this.m_Id, frameIndex);
		}

		public RTHandle GetCurrentTexture()
		{
			return base.GetCurrentFrameRT(this.m_Id);
		}

		public RTHandle GetPreviousTexture()
		{
			return this.GetTexture(1);
		}

		internal bool IsAllocated()
		{
			return this.GetTexture(0) != null;
		}

		internal bool IsDirty(ref RenderTextureDescriptor desc)
		{
			return this.m_DescKey != Hash128.Compute<RenderTextureDescriptor>(ref desc);
		}

		private void Alloc(ref RenderTextureDescriptor desc)
		{
			base.AllocHistoryFrameRT(this.m_Id, this.GetHistoryFrameCount(), ref desc, this.GetHistoryName());
			this.m_Descriptor = desc;
			this.m_DescKey = Hash128.Compute<RenderTextureDescriptor>(ref desc);
		}

		public override void Reset()
		{
			base.ReleaseHistoryFrameRT(this.m_Id);
		}

		internal bool Update(ref RenderTextureDescriptor cameraDesc)
		{
			if (cameraDesc.width > 0 && cameraDesc.height > 0 && cameraDesc.graphicsFormat != GraphicsFormat.None)
			{
				RenderTextureDescriptor historyDescriptor = this.GetHistoryDescriptor(ref cameraDesc);
				if (this.IsDirty(ref historyDescriptor))
				{
					this.Reset();
				}
				if (!this.IsAllocated())
				{
					this.Alloc(ref historyDescriptor);
					return true;
				}
			}
			return false;
		}

		protected abstract int GetHistoryFrameCount();

		protected abstract string GetHistoryName();

		protected abstract RenderTextureDescriptor GetHistoryDescriptor(ref RenderTextureDescriptor cameraDesc);

		private int m_Id;

		private RenderTextureDescriptor m_Descriptor;

		private Hash128 m_DescKey;
	}
}
