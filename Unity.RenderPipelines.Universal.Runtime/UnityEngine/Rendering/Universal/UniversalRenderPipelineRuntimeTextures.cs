using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering.Universal
{
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[CategoryInfo(Name = "R: Runtime Textures", Order = 1000)]
	[HideInInspector]
	[Serializable]
	public class UniversalRenderPipelineRuntimeTextures : IRenderPipelineResources, IRenderPipelineGraphicsSettings
	{
		public int version
		{
			get
			{
				return this.m_Version;
			}
		}

		bool IRenderPipelineGraphicsSettings.isAvailableInPlayerBuild
		{
			get
			{
				return true;
			}
		}

		public Texture2D blueNoise64LTex
		{
			get
			{
				return this.m_BlueNoise64LTex;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_BlueNoise64LTex, value, "m_BlueNoise64LTex");
			}
		}

		public Texture2D bayerMatrixTex
		{
			get
			{
				return this.m_BayerMatrixTex;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_BayerMatrixTex, value, "m_BayerMatrixTex");
			}
		}

		public Texture2D debugFontTexture
		{
			get
			{
				return this.m_DebugFontTex;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_DebugFontTex, value, "m_DebugFontTex");
			}
		}

		public Texture2D stencilDitherTex
		{
			get
			{
				if (!this.m_StencilDitherTex)
				{
					this.m_StencilDitherTex = new Texture2D(2, 2, TextureFormat.Alpha8, false, true);
					this.m_StencilDitherTex.SetPixel(0, 0, Color.red * 0.25f);
					this.m_StencilDitherTex.SetPixel(1, 1, Color.red * 0.5f);
					this.m_StencilDitherTex.SetPixel(0, 1, Color.red * 0.75f);
					this.m_StencilDitherTex.SetPixel(1, 0, Color.red * 1f);
					this.m_StencilDitherTex.Apply();
				}
				return this.m_StencilDitherTex;
			}
		}

		[SerializeField]
		[HideInInspector]
		private int m_Version = 1;

		[SerializeField]
		[ResourcePath("Textures/BlueNoise64/L/LDR_LLL1_0.png", SearchType.ProjectPath)]
		private Texture2D m_BlueNoise64LTex;

		[SerializeField]
		[ResourcePath("Textures/BayerMatrix.png", SearchType.ProjectPath)]
		private Texture2D m_BayerMatrixTex;

		[SerializeField]
		[ResourcePath("Textures/DebugFont.tga", SearchType.ProjectPath)]
		private Texture2D m_DebugFontTex;

		private Texture2D m_StencilDitherTex;
	}
}
