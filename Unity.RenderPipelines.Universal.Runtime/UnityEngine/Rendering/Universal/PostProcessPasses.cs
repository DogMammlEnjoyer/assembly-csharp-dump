using System;
using UnityEngine.Rendering.Universal.Internal;

namespace UnityEngine.Rendering.Universal
{
	internal struct PostProcessPasses : IDisposable
	{
		public ColorGradingLutPass colorGradingLutPass
		{
			get
			{
				return this.m_ColorGradingLutPass;
			}
		}

		public PostProcessPass postProcessPass
		{
			get
			{
				return this.m_PostProcessPass;
			}
		}

		public PostProcessPass finalPostProcessPass
		{
			get
			{
				return this.m_FinalPostProcessPass;
			}
		}

		public RTHandle afterPostProcessColor
		{
			get
			{
				return this.m_AfterPostProcessColor;
			}
		}

		public RTHandle colorGradingLut
		{
			get
			{
				return this.m_ColorGradingLut;
			}
		}

		public bool isCreated
		{
			get
			{
				return this.m_CurrentPostProcessData != null;
			}
		}

		public PostProcessPasses(PostProcessData rendererPostProcessData, ref PostProcessParams postProcessParams)
		{
			this.m_ColorGradingLutPass = null;
			this.m_PostProcessPass = null;
			this.m_FinalPostProcessPass = null;
			this.m_CurrentPostProcessData = null;
			this.m_AfterPostProcessColor = null;
			this.m_ColorGradingLut = null;
			this.m_RendererPostProcessData = rendererPostProcessData;
			this.m_BlitMaterial = postProcessParams.blitMaterial;
			this.Recreate(rendererPostProcessData, ref postProcessParams);
		}

		public void Recreate(PostProcessData data, ref PostProcessParams ppParams)
		{
			if (this.m_RendererPostProcessData)
			{
				data = this.m_RendererPostProcessData;
			}
			if (data == this.m_CurrentPostProcessData)
			{
				return;
			}
			if (this.m_CurrentPostProcessData != null)
			{
				ColorGradingLutPass colorGradingLutPass = this.m_ColorGradingLutPass;
				if (colorGradingLutPass != null)
				{
					colorGradingLutPass.Cleanup();
				}
				PostProcessPass postProcessPass = this.m_PostProcessPass;
				if (postProcessPass != null)
				{
					postProcessPass.Cleanup();
				}
				PostProcessPass finalPostProcessPass = this.m_FinalPostProcessPass;
				if (finalPostProcessPass != null)
				{
					finalPostProcessPass.Cleanup();
				}
				this.m_ColorGradingLutPass = null;
				this.m_PostProcessPass = null;
				this.m_FinalPostProcessPass = null;
				this.m_CurrentPostProcessData = null;
			}
			if (data != null)
			{
				this.m_ColorGradingLutPass = new ColorGradingLutPass(RenderPassEvent.BeforeRenderingPrePasses, data);
				this.m_PostProcessPass = new PostProcessPass((RenderPassEvent)599, data, ref ppParams);
				this.m_FinalPostProcessPass = new PostProcessPass((RenderPassEvent)999, data, ref ppParams);
				this.m_CurrentPostProcessData = data;
			}
		}

		public void Dispose()
		{
			ColorGradingLutPass colorGradingLutPass = this.m_ColorGradingLutPass;
			if (colorGradingLutPass != null)
			{
				colorGradingLutPass.Cleanup();
			}
			PostProcessPass postProcessPass = this.m_PostProcessPass;
			if (postProcessPass != null)
			{
				postProcessPass.Cleanup();
			}
			PostProcessPass finalPostProcessPass = this.m_FinalPostProcessPass;
			if (finalPostProcessPass != null)
			{
				finalPostProcessPass.Cleanup();
			}
			RTHandle afterPostProcessColor = this.m_AfterPostProcessColor;
			if (afterPostProcessColor != null)
			{
				afterPostProcessColor.Release();
			}
			RTHandle colorGradingLut = this.m_ColorGradingLut;
			if (colorGradingLut == null)
			{
				return;
			}
			colorGradingLut.Release();
		}

		internal void ReleaseRenderTargets()
		{
			RTHandle afterPostProcessColor = this.m_AfterPostProcessColor;
			if (afterPostProcessColor != null)
			{
				afterPostProcessColor.Release();
			}
			PostProcessPass postProcessPass = this.m_PostProcessPass;
			if (postProcessPass != null)
			{
				postProcessPass.Dispose();
			}
			PostProcessPass finalPostProcessPass = this.m_FinalPostProcessPass;
			if (finalPostProcessPass != null)
			{
				finalPostProcessPass.Dispose();
			}
			RTHandle colorGradingLut = this.m_ColorGradingLut;
			if (colorGradingLut == null)
			{
				return;
			}
			colorGradingLut.Release();
		}

		private ColorGradingLutPass m_ColorGradingLutPass;

		private PostProcessPass m_PostProcessPass;

		private PostProcessPass m_FinalPostProcessPass;

		internal RTHandle m_AfterPostProcessColor;

		internal RTHandle m_ColorGradingLut;

		private PostProcessData m_RendererPostProcessData;

		private PostProcessData m_CurrentPostProcessData;

		private Material m_BlitMaterial;
	}
}
