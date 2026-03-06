using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
	public sealed class RawColorHistory : CameraHistoryItem
	{
		public override void OnCreate(BufferedRTHandleSystem owner, uint typeId)
		{
			base.OnCreate(owner, typeId);
			this.m_Ids[0] = base.MakeId(0U);
			this.m_Ids[1] = base.MakeId(1U);
		}

		public RTHandle GetCurrentTexture(int eyeIndex = 0)
		{
			if ((ulong)eyeIndex >= (ulong)((long)this.m_Ids.Length))
			{
				return null;
			}
			return base.GetCurrentFrameRT(this.m_Ids[eyeIndex]);
		}

		public RTHandle GetPreviousTexture(int eyeIndex = 0)
		{
			if ((ulong)eyeIndex >= (ulong)((long)this.m_Ids.Length))
			{
				return null;
			}
			return base.GetPreviousFrameRT(this.m_Ids[eyeIndex]);
		}

		private bool IsAllocated()
		{
			return this.GetCurrentTexture(0) != null;
		}

		private bool IsDirty(ref RenderTextureDescriptor desc)
		{
			return this.m_DescKey != Hash128.Compute<RenderTextureDescriptor>(ref desc);
		}

		private void Alloc(ref RenderTextureDescriptor desc, bool xrMultipassEnabled)
		{
			base.AllocHistoryFrameRT(this.m_Ids[0], 2, ref desc, RawColorHistory.m_Names[0]);
			if (xrMultipassEnabled)
			{
				base.AllocHistoryFrameRT(this.m_Ids[1], 2, ref desc, RawColorHistory.m_Names[1]);
			}
			this.m_Descriptor = desc;
			this.m_DescKey = Hash128.Compute<RenderTextureDescriptor>(ref desc);
		}

		public override void Reset()
		{
			for (int i = 0; i < this.m_Ids.Length; i++)
			{
				base.ReleaseHistoryFrameRT(this.m_Ids[i]);
			}
		}

		internal RenderTextureDescriptor GetHistoryDescriptor(ref RenderTextureDescriptor cameraDesc)
		{
			RenderTextureDescriptor result = cameraDesc;
			result.depthStencilFormat = GraphicsFormat.None;
			result.mipCount = 0;
			result.msaaSamples = 1;
			return result;
		}

		internal bool Update(ref RenderTextureDescriptor cameraDesc, bool xrMultipassEnabled = false)
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
					this.Alloc(ref historyDescriptor, xrMultipassEnabled);
					return true;
				}
			}
			return false;
		}

		private int[] m_Ids = new int[2];

		private static readonly string[] m_Names = new string[]
		{
			"RawColorHistory0",
			"RawColorHistory1"
		};

		private RenderTextureDescriptor m_Descriptor;

		private Hash128 m_DescKey;
	}
}
