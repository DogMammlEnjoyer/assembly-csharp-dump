using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.RenderGraphModule
{
	public struct TextureDesc
	{
		public DepthBits depthBufferBits
		{
			get
			{
				return (DepthBits)GraphicsFormatUtility.GetDepthBits(this.format);
			}
			set
			{
				if (value != DepthBits.None)
				{
					this.format = GraphicsFormatUtility.GetDepthStencilFormat((int)value);
					return;
				}
				if (!GraphicsFormatUtility.IsDepthStencilFormat(this.format))
				{
					return;
				}
				this.format = GraphicsFormat.None;
			}
		}

		public GraphicsFormat colorFormat
		{
			get
			{
				if (!GraphicsFormatUtility.IsDepthStencilFormat(this.format))
				{
					return this.format;
				}
				return GraphicsFormat.None;
			}
			set
			{
				this.format = value;
			}
		}

		private void InitDefaultValues(bool dynamicResolution, bool xrReady)
		{
			this.useDynamicScale = dynamicResolution;
			this.vrUsage = VRTextureUsage.None;
			if (xrReady)
			{
				this.slices = TextureXR.slices;
				this.dimension = TextureXR.dimension;
			}
			else
			{
				this.slices = 1;
				this.dimension = TextureDimension.Tex2D;
			}
			this.discardBuffer = false;
		}

		public TextureDesc(int width, int height, bool dynamicResolution = false, bool xrReady = false)
		{
			this = default(TextureDesc);
			this.sizeMode = TextureSizeMode.Explicit;
			this.width = width;
			this.height = height;
			this.msaaSamples = MSAASamples.None;
			this.InitDefaultValues(dynamicResolution, xrReady);
		}

		public TextureDesc(Vector2 scale, bool dynamicResolution = false, bool xrReady = false)
		{
			this = default(TextureDesc);
			this.sizeMode = TextureSizeMode.Scale;
			this.scale = scale;
			this.msaaSamples = MSAASamples.None;
			this.dimension = TextureDimension.Tex2D;
			this.InitDefaultValues(dynamicResolution, xrReady);
		}

		public TextureDesc(ScaleFunc func, bool dynamicResolution = false, bool xrReady = false)
		{
			this = default(TextureDesc);
			this.sizeMode = TextureSizeMode.Functor;
			this.func = func;
			this.msaaSamples = MSAASamples.None;
			this.dimension = TextureDimension.Tex2D;
			this.InitDefaultValues(dynamicResolution, xrReady);
		}

		public TextureDesc(TextureDesc input)
		{
			this = input;
		}

		public TextureDesc(RenderTextureDescriptor input)
		{
			this.sizeMode = TextureSizeMode.Explicit;
			this.width = input.width;
			this.height = input.height;
			this.slices = input.volumeDepth;
			this.scale = Vector2.one;
			this.func = null;
			this.format = ((input.depthStencilFormat != GraphicsFormat.None) ? input.depthStencilFormat : input.graphicsFormat);
			this.filterMode = FilterMode.Bilinear;
			this.wrapMode = TextureWrapMode.Clamp;
			this.dimension = input.dimension;
			this.enableRandomWrite = input.enableRandomWrite;
			this.useMipMap = input.useMipMap;
			this.autoGenerateMips = input.autoGenerateMips;
			this.isShadowMap = (input.shadowSamplingMode != ShadowSamplingMode.None);
			this.anisoLevel = 1;
			this.mipMapBias = 0f;
			this.msaaSamples = (MSAASamples)input.msaaSamples;
			this.bindTextureMS = input.bindMS;
			this.useDynamicScale = input.useDynamicScale;
			this.useDynamicScaleExplicit = false;
			this.memoryless = input.memoryless;
			this.vrUsage = input.vrUsage;
			this.name = "UnNamedFromRenderTextureDescriptor";
			this.fastMemoryDesc = default(FastMemoryDesc);
			this.fastMemoryDesc.inFastMemory = false;
			this.fallBackToBlackTexture = false;
			this.disableFallBackToImportedTexture = true;
			this.clearBuffer = true;
			this.clearColor = Color.black;
			this.discardBuffer = false;
			this.enableShadingRate = input.enableShadingRate;
		}

		public TextureDesc(RenderTexture input)
		{
			this = new TextureDesc(input.descriptor);
			this.filterMode = input.filterMode;
			this.wrapMode = input.wrapMode;
			this.anisoLevel = input.anisoLevel;
			this.mipMapBias = input.mipMapBias;
			this.name = "UnNamedFromRenderTextureDescriptor";
		}

		public override int GetHashCode()
		{
			HashFNV1A32 hashFNV1A = HashFNV1A32.Create();
			int funcHashCode;
			switch (this.sizeMode)
			{
			case TextureSizeMode.Explicit:
				hashFNV1A.Append(this.width);
				hashFNV1A.Append(this.height);
				break;
			case TextureSizeMode.Scale:
				hashFNV1A.Append(this.scale);
				break;
			case TextureSizeMode.Functor:
				if (this.func != null)
				{
					funcHashCode = DelegateHashCodeUtils.GetFuncHashCode(this.func);
					hashFNV1A.Append(funcHashCode);
				}
				break;
			}
			hashFNV1A.Append(this.mipMapBias);
			hashFNV1A.Append(this.slices);
			funcHashCode = (int)this.format;
			hashFNV1A.Append(funcHashCode);
			funcHashCode = (int)this.filterMode;
			hashFNV1A.Append(funcHashCode);
			funcHashCode = (int)this.wrapMode;
			hashFNV1A.Append(funcHashCode);
			funcHashCode = (int)this.dimension;
			hashFNV1A.Append(funcHashCode);
			funcHashCode = (int)this.memoryless;
			hashFNV1A.Append(funcHashCode);
			funcHashCode = (int)this.vrUsage;
			hashFNV1A.Append(funcHashCode);
			hashFNV1A.Append(this.anisoLevel);
			hashFNV1A.Append(this.enableRandomWrite);
			hashFNV1A.Append(this.useMipMap);
			hashFNV1A.Append(this.autoGenerateMips);
			hashFNV1A.Append(this.isShadowMap);
			hashFNV1A.Append(this.bindTextureMS);
			hashFNV1A.Append(this.useDynamicScale);
			funcHashCode = (int)this.msaaSamples;
			hashFNV1A.Append(funcHashCode);
			hashFNV1A.Append(this.fastMemoryDesc.inFastMemory);
			hashFNV1A.Append(this.enableShadingRate);
			return hashFNV1A.value;
		}

		public Vector2Int CalculateFinalDimensions()
		{
			Vector2Int result;
			switch (this.sizeMode)
			{
			case TextureSizeMode.Explicit:
				result = new Vector2Int(this.width, this.height);
				break;
			case TextureSizeMode.Scale:
				result = RTHandles.CalculateDimensions(this.scale);
				break;
			case TextureSizeMode.Functor:
				result = RTHandles.CalculateDimensions(this.func);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}

		public TextureSizeMode sizeMode;

		public int width;

		public int height;

		public int slices;

		public Vector2 scale;

		public ScaleFunc func;

		public GraphicsFormat format;

		public FilterMode filterMode;

		public TextureWrapMode wrapMode;

		public TextureDimension dimension;

		public bool enableRandomWrite;

		public bool useMipMap;

		public bool autoGenerateMips;

		public bool isShadowMap;

		public int anisoLevel;

		public float mipMapBias;

		public MSAASamples msaaSamples;

		public bool bindTextureMS;

		public bool useDynamicScale;

		public bool useDynamicScaleExplicit;

		public RenderTextureMemoryless memoryless;

		public VRTextureUsage vrUsage;

		public bool enableShadingRate;

		public string name;

		public FastMemoryDesc fastMemoryDesc;

		public bool fallBackToBlackTexture;

		public bool disableFallBackToImportedTexture;

		public bool clearBuffer;

		public Color clearColor;

		public bool discardBuffer;
	}
}
