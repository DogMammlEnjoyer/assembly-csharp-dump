using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal.Internal
{
	internal sealed class RenderTargetBufferSystem
	{
		private ref RenderTargetBufferSystem.SwapBuffer backBuffer
		{
			get
			{
				if (!RenderTargetBufferSystem.m_AisBackBuffer)
				{
					return ref this.m_B;
				}
				return ref this.m_A;
			}
		}

		private ref RenderTargetBufferSystem.SwapBuffer frontBuffer
		{
			get
			{
				if (!RenderTargetBufferSystem.m_AisBackBuffer)
				{
					return ref this.m_A;
				}
				return ref this.m_B;
			}
		}

		public RenderTargetBufferSystem(string name)
		{
			this.m_A.name = name + "A";
			this.m_B.name = name + "B";
		}

		public void Dispose()
		{
			RTHandle rtMSAA = this.m_A.rtMSAA;
			if (rtMSAA != null)
			{
				rtMSAA.Release();
			}
			RTHandle rtMSAA2 = this.m_B.rtMSAA;
			if (rtMSAA2 != null)
			{
				rtMSAA2.Release();
			}
			RTHandle rtResolve = this.m_A.rtResolve;
			if (rtResolve != null)
			{
				rtResolve.Release();
			}
			RTHandle rtResolve2 = this.m_B.rtResolve;
			if (rtResolve2 == null)
			{
				return;
			}
			rtResolve2.Release();
		}

		public RTHandle PeekBackBuffer()
		{
			if (!this.m_AllowMSAA || this.backBuffer.msaa <= 1)
			{
				return this.backBuffer.rtResolve;
			}
			return this.backBuffer.rtMSAA;
		}

		public RTHandle GetBackBuffer(CommandBuffer cmd)
		{
			this.ReAllocate(cmd);
			return this.PeekBackBuffer();
		}

		public RTHandle GetFrontBuffer(CommandBuffer cmd)
		{
			if (!this.m_AllowMSAA && this.frontBuffer.msaa > 1)
			{
				this.frontBuffer.msaa = 1;
			}
			this.ReAllocate(cmd);
			if (!this.m_AllowMSAA || this.frontBuffer.msaa <= 1)
			{
				return this.frontBuffer.rtResolve;
			}
			return this.frontBuffer.rtMSAA;
		}

		public void Swap()
		{
			RenderTargetBufferSystem.m_AisBackBuffer = !RenderTargetBufferSystem.m_AisBackBuffer;
		}

		private void ReAllocate(CommandBuffer cmd)
		{
			RenderTextureDescriptor desc = RenderTargetBufferSystem.m_Desc;
			desc.msaaSamples = this.m_A.msaa;
			if (desc.msaaSamples > 1)
			{
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_A.rtMSAA, desc, this.m_FilterMode, TextureWrapMode.Clamp, 1, 0f, this.m_A.name);
			}
			desc.msaaSamples = this.m_B.msaa;
			if (desc.msaaSamples > 1)
			{
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_B.rtMSAA, desc, this.m_FilterMode, TextureWrapMode.Clamp, 1, 0f, this.m_B.name);
			}
			desc.msaaSamples = 1;
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_A.rtResolve, desc, this.m_FilterMode, TextureWrapMode.Clamp, 1, 0f, this.m_A.name);
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_B.rtResolve, desc, this.m_FilterMode, TextureWrapMode.Clamp, 1, 0f, this.m_B.name);
			cmd.SetGlobalTexture(this.m_A.name, this.m_A.rtResolve);
			cmd.SetGlobalTexture(this.m_B.name, this.m_B.rtResolve);
		}

		public void Clear()
		{
			RenderTargetBufferSystem.m_AisBackBuffer = true;
			this.m_AllowMSAA = (this.m_A.msaa > 1 || this.m_B.msaa > 1);
		}

		public void SetCameraSettings(RenderTextureDescriptor desc, FilterMode filterMode)
		{
			desc.depthStencilFormat = GraphicsFormat.None;
			RenderTargetBufferSystem.m_Desc = desc;
			this.m_FilterMode = filterMode;
			this.m_A.msaa = RenderTargetBufferSystem.m_Desc.msaaSamples;
			this.m_B.msaa = RenderTargetBufferSystem.m_Desc.msaaSamples;
			if (RenderTargetBufferSystem.m_Desc.msaaSamples > 1)
			{
				this.EnableMSAA(true);
			}
		}

		public RTHandle GetBufferA()
		{
			if (!this.m_AllowMSAA || this.m_A.msaa <= 1)
			{
				return this.m_A.rtResolve;
			}
			return this.m_A.rtMSAA;
		}

		public void EnableMSAA(bool enable)
		{
			this.m_AllowMSAA = enable;
			if (enable)
			{
				this.m_A.msaa = RenderTargetBufferSystem.m_Desc.msaaSamples;
				this.m_B.msaa = RenderTargetBufferSystem.m_Desc.msaaSamples;
			}
		}

		private RenderTargetBufferSystem.SwapBuffer m_A;

		private RenderTargetBufferSystem.SwapBuffer m_B;

		private static bool m_AisBackBuffer = true;

		private static RenderTextureDescriptor m_Desc;

		private FilterMode m_FilterMode;

		private bool m_AllowMSAA = true;

		private struct SwapBuffer
		{
			public RTHandle rtMSAA;

			public RTHandle rtResolve;

			public string name;

			public int msaa;
		}
	}
}
