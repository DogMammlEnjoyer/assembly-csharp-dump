using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
	public sealed class TaaHistory : CameraHistoryItem
	{
		public override void OnCreate(BufferedRTHandleSystem owner, uint typeId)
		{
			base.OnCreate(owner, typeId);
			this.m_TaaAccumulationTextureIds[0] = base.MakeId(0U);
			this.m_TaaAccumulationTextureIds[1] = base.MakeId(1U);
		}

		public override void Reset()
		{
			for (int i = 0; i < this.m_TaaAccumulationTextureIds.Length; i++)
			{
				base.ReleaseHistoryFrameRT(this.m_TaaAccumulationTextureIds[i]);
				this.m_TaaAccumulationVersions[i] = -1;
			}
			this.m_Descriptor.width = 0;
			this.m_Descriptor.height = 0;
			this.m_Descriptor.graphicsFormat = GraphicsFormat.None;
			this.m_DescKey = Hash128.Compute(0);
		}

		public RTHandle GetAccumulationTexture(int eyeIndex = 0)
		{
			return base.GetCurrentFrameRT(this.m_TaaAccumulationTextureIds[eyeIndex]);
		}

		public int GetAccumulationVersion(int eyeIndex = 0)
		{
			return this.m_TaaAccumulationVersions[eyeIndex];
		}

		internal void SetAccumulationVersion(int eyeIndex, int version)
		{
			this.m_TaaAccumulationVersions[eyeIndex] = version;
		}

		private bool IsValid()
		{
			return this.GetAccumulationTexture(0) != null;
		}

		private bool IsDirty(ref RenderTextureDescriptor desc)
		{
			return this.m_DescKey != Hash128.Compute<RenderTextureDescriptor>(ref desc);
		}

		private void Alloc(ref RenderTextureDescriptor desc, bool xrMultipassEnabled)
		{
			base.AllocHistoryFrameRT(this.m_TaaAccumulationTextureIds[0], 1, ref desc, TaaHistory.m_TaaAccumulationNames[0]);
			if (xrMultipassEnabled)
			{
				base.AllocHistoryFrameRT(this.m_TaaAccumulationTextureIds[1], 1, ref desc, TaaHistory.m_TaaAccumulationNames[1]);
			}
			this.m_Descriptor = desc;
			this.m_DescKey = Hash128.Compute<RenderTextureDescriptor>(ref desc);
		}

		internal bool Update(ref RenderTextureDescriptor cameraDesc, bool xrMultipassEnabled = false)
		{
			if (cameraDesc.width > 0 && cameraDesc.height > 0 && cameraDesc.graphicsFormat != GraphicsFormat.None)
			{
				RenderTextureDescriptor renderTextureDescriptor = TemporalAA.TemporalAADescFromCameraDesc(ref cameraDesc);
				if (this.IsDirty(ref renderTextureDescriptor))
				{
					this.Reset();
				}
				if (!this.IsValid())
				{
					this.Alloc(ref renderTextureDescriptor, xrMultipassEnabled);
					return true;
				}
			}
			return false;
		}

		private int[] m_TaaAccumulationTextureIds = new int[2];

		private int[] m_TaaAccumulationVersions = new int[2];

		private static readonly string[] m_TaaAccumulationNames = new string[]
		{
			"TaaAccumulationTex0",
			"TaaAccumulationTex1"
		};

		private RenderTextureDescriptor m_Descriptor;

		private Hash128 m_DescKey;
	}
}
