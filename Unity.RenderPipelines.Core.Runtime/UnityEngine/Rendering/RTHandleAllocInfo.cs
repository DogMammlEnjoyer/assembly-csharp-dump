using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	public struct RTHandleAllocInfo
	{
		public int slices { readonly get; set; }

		public GraphicsFormat format { readonly get; set; }

		public FilterMode filterMode { readonly get; set; }

		public TextureWrapMode wrapModeU { readonly get; set; }

		public TextureWrapMode wrapModeV { readonly get; set; }

		public TextureWrapMode wrapModeW { readonly get; set; }

		public TextureDimension dimension { readonly get; set; }

		public bool enableRandomWrite { readonly get; set; }

		public bool useMipMap { readonly get; set; }

		public bool autoGenerateMips { readonly get; set; }

		public bool isShadowMap { readonly get; set; }

		public int anisoLevel { readonly get; set; }

		public float mipMapBias { readonly get; set; }

		public MSAASamples msaaSamples { readonly get; set; }

		public bool bindTextureMS { readonly get; set; }

		public bool useDynamicScale { readonly get; set; }

		public bool useDynamicScaleExplicit { readonly get; set; }

		public RenderTextureMemoryless memoryless { readonly get; set; }

		public VRTextureUsage vrUsage { readonly get; set; }

		public bool enableShadingRate { readonly get; set; }

		public string name { readonly get; set; }

		public RTHandleAllocInfo(string name = "")
		{
			this.slices = 1;
			this.format = GraphicsFormat.R8G8B8A8_SRGB;
			this.filterMode = FilterMode.Point;
			this.wrapModeU = TextureWrapMode.Repeat;
			this.wrapModeV = TextureWrapMode.Repeat;
			this.wrapModeW = TextureWrapMode.Repeat;
			this.dimension = TextureDimension.Tex2D;
			this.enableRandomWrite = false;
			this.useMipMap = false;
			this.autoGenerateMips = true;
			this.isShadowMap = false;
			this.anisoLevel = 1;
			this.mipMapBias = 0f;
			this.msaaSamples = MSAASamples.None;
			this.bindTextureMS = false;
			this.useDynamicScale = false;
			this.useDynamicScaleExplicit = false;
			this.memoryless = RenderTextureMemoryless.None;
			this.vrUsage = VRTextureUsage.None;
			this.enableShadingRate = false;
			this.name = name;
		}
	}
}
