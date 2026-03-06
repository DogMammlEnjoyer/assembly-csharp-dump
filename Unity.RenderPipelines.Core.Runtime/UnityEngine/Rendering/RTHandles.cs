using System;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	public static class RTHandles
	{
		public static int maxWidth
		{
			get
			{
				return RTHandles.s_DefaultInstance.GetMaxWidth();
			}
		}

		public static int maxHeight
		{
			get
			{
				return RTHandles.s_DefaultInstance.GetMaxHeight();
			}
		}

		public static RTHandleProperties rtHandleProperties
		{
			get
			{
				return RTHandles.s_DefaultInstance.rtHandleProperties;
			}
		}

		public static Vector2Int CalculateDimensions(Vector2 scaleFactor)
		{
			return RTHandles.s_DefaultInstance.CalculateDimensions(scaleFactor);
		}

		public static Vector2Int CalculateDimensions(ScaleFunc scaleFunc)
		{
			return RTHandles.s_DefaultInstance.CalculateDimensions(scaleFunc);
		}

		public static RTHandle Alloc(int width, int height, int slices = 1, DepthBits depthBufferBits = DepthBits.None, GraphicsFormat colorFormat = GraphicsFormat.R8G8B8A8_SRGB, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureDimension dimension = TextureDimension.Tex2D, bool enableRandomWrite = false, bool useMipMap = false, bool autoGenerateMips = true, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, MSAASamples msaaSamples = MSAASamples.None, bool bindTextureMS = false, bool useDynamicScale = false, bool useDynamicScaleExplicit = false, RenderTextureMemoryless memoryless = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, string name = "")
		{
			return RTHandles.s_DefaultInstance.Alloc(width, height, slices, depthBufferBits, colorFormat, filterMode, wrapMode, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, memoryless, vrUsage, name);
		}

		public static RTHandle Alloc(int width, int height, GraphicsFormat format, int slices = 1, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureDimension dimension = TextureDimension.Tex2D, bool enableRandomWrite = false, bool useMipMap = false, bool autoGenerateMips = true, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, MSAASamples msaaSamples = MSAASamples.None, bool bindTextureMS = false, bool useDynamicScale = false, bool useDynamicScaleExplicit = false, RenderTextureMemoryless memoryless = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, string name = "")
		{
			return RTHandles.s_DefaultInstance.Alloc(width, height, format, slices, filterMode, wrapMode, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, memoryless, vrUsage, name);
		}

		public static RTHandle Alloc(int width, int height, TextureWrapMode wrapModeU, TextureWrapMode wrapModeV, TextureWrapMode wrapModeW = TextureWrapMode.Repeat, int slices = 1, DepthBits depthBufferBits = DepthBits.None, GraphicsFormat colorFormat = GraphicsFormat.R8G8B8A8_SRGB, FilterMode filterMode = FilterMode.Point, TextureDimension dimension = TextureDimension.Tex2D, bool enableRandomWrite = false, bool useMipMap = false, bool autoGenerateMips = true, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, MSAASamples msaaSamples = MSAASamples.None, bool bindTextureMS = false, bool useDynamicScale = false, bool useDynamicScaleExplicit = false, RenderTextureMemoryless memoryless = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, string name = "")
		{
			return RTHandles.s_DefaultInstance.Alloc(width, height, wrapModeU, wrapModeV, wrapModeW, slices, depthBufferBits, colorFormat, filterMode, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, memoryless, vrUsage, name);
		}

		public static RTHandle Alloc(int width, int height, RTHandleAllocInfo info)
		{
			return RTHandles.s_DefaultInstance.Alloc(width, height, info);
		}

		public static RTHandle Alloc(in RenderTextureDescriptor descriptor, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, string name = "")
		{
			return RTHandles.s_DefaultInstance.Alloc(descriptor.width, descriptor.height, RTHandles.GetRTHandleAllocInfo(descriptor, filterMode, wrapMode, anisoLevel, mipMapBias, name));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static GraphicsFormat GetFormat(GraphicsFormat colorFormat, GraphicsFormat depthStencilFormat)
		{
			if (depthStencilFormat != GraphicsFormat.None)
			{
				return depthStencilFormat;
			}
			return colorFormat;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static RTHandleAllocInfo GetRTHandleAllocInfo(in RenderTextureDescriptor desc, FilterMode filterMode, TextureWrapMode wrapMode, int anisoLevel, float mipMapBias, string name)
		{
			RTHandleAllocInfo result = new RTHandleAllocInfo(name);
			result.slices = desc.volumeDepth;
			RenderTextureDescriptor renderTextureDescriptor = desc;
			result.format = RTHandles.GetFormat(renderTextureDescriptor.graphicsFormat, desc.depthStencilFormat);
			result.filterMode = filterMode;
			result.wrapModeU = wrapMode;
			result.wrapModeV = wrapMode;
			result.wrapModeW = wrapMode;
			result.dimension = desc.dimension;
			renderTextureDescriptor = desc;
			result.enableRandomWrite = renderTextureDescriptor.enableRandomWrite;
			renderTextureDescriptor = desc;
			result.useMipMap = renderTextureDescriptor.useMipMap;
			renderTextureDescriptor = desc;
			result.autoGenerateMips = renderTextureDescriptor.autoGenerateMips;
			result.isShadowMap = (desc.shadowSamplingMode != ShadowSamplingMode.None);
			result.anisoLevel = anisoLevel;
			result.mipMapBias = mipMapBias;
			result.msaaSamples = (MSAASamples)desc.msaaSamples;
			renderTextureDescriptor = desc;
			result.bindTextureMS = renderTextureDescriptor.bindMS;
			renderTextureDescriptor = desc;
			result.useDynamicScale = renderTextureDescriptor.useDynamicScale;
			renderTextureDescriptor = desc;
			result.useDynamicScaleExplicit = renderTextureDescriptor.useDynamicScaleExplicit;
			result.memoryless = desc.memoryless;
			result.vrUsage = desc.vrUsage;
			renderTextureDescriptor = desc;
			result.enableShadingRate = renderTextureDescriptor.enableShadingRate;
			return result;
		}

		public static RTHandle Alloc(Vector2 scaleFactor, int slices = 1, DepthBits depthBufferBits = DepthBits.None, GraphicsFormat colorFormat = GraphicsFormat.R8G8B8A8_SRGB, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureDimension dimension = TextureDimension.Tex2D, bool enableRandomWrite = false, bool useMipMap = false, bool autoGenerateMips = true, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, MSAASamples msaaSamples = MSAASamples.None, bool bindTextureMS = false, bool useDynamicScale = false, bool useDynamicScaleExplicit = false, RenderTextureMemoryless memoryless = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, string name = "")
		{
			return RTHandles.s_DefaultInstance.Alloc(scaleFactor, slices, depthBufferBits, colorFormat, filterMode, wrapMode, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, memoryless, vrUsage, name);
		}

		public static RTHandle Alloc(Vector2 scaleFactor, GraphicsFormat format, int slices = 1, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureDimension dimension = TextureDimension.Tex2D, bool enableRandomWrite = false, bool useMipMap = false, bool autoGenerateMips = true, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, MSAASamples msaaSamples = MSAASamples.None, bool bindTextureMS = false, bool useDynamicScale = false, bool useDynamicScaleExplicit = false, RenderTextureMemoryless memoryless = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, string name = "")
		{
			return RTHandles.s_DefaultInstance.Alloc(scaleFactor, format, slices, filterMode, wrapMode, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, memoryless, vrUsage, name);
		}

		public static RTHandle Alloc(Vector2 scaleFactor, in RenderTextureDescriptor descriptor, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, string name = "")
		{
			return RTHandles.s_DefaultInstance.Alloc(scaleFactor, RTHandles.GetRTHandleAllocInfo(descriptor, filterMode, wrapMode, anisoLevel, mipMapBias, name));
		}

		public static RTHandle Alloc(Vector2 scaleFactor, RTHandleAllocInfo info)
		{
			return RTHandles.s_DefaultInstance.Alloc(scaleFactor, info);
		}

		public static RTHandle Alloc(ScaleFunc scaleFunc, int slices = 1, DepthBits depthBufferBits = DepthBits.None, GraphicsFormat colorFormat = GraphicsFormat.R8G8B8A8_SRGB, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureDimension dimension = TextureDimension.Tex2D, bool enableRandomWrite = false, bool useMipMap = false, bool autoGenerateMips = true, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, MSAASamples msaaSamples = MSAASamples.None, bool bindTextureMS = false, bool useDynamicScale = false, bool useDynamicScaleExplicit = false, RenderTextureMemoryless memoryless = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, string name = "")
		{
			return RTHandles.s_DefaultInstance.Alloc(scaleFunc, slices, depthBufferBits, colorFormat, filterMode, wrapMode, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, memoryless, vrUsage, name);
		}

		public static RTHandle Alloc(ScaleFunc scaleFunc, GraphicsFormat format, int slices = 1, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureDimension dimension = TextureDimension.Tex2D, bool enableRandomWrite = false, bool useMipMap = false, bool autoGenerateMips = true, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, MSAASamples msaaSamples = MSAASamples.None, bool bindTextureMS = false, bool useDynamicScale = false, bool useDynamicScaleExplicit = false, RenderTextureMemoryless memoryless = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, string name = "")
		{
			return RTHandles.s_DefaultInstance.Alloc(scaleFunc, format, slices, filterMode, wrapMode, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, memoryless, vrUsage, name);
		}

		public static RTHandle Alloc(ScaleFunc scaleFunc, in RenderTextureDescriptor descriptor, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, string name = "")
		{
			return RTHandles.s_DefaultInstance.Alloc(scaleFunc, RTHandles.GetRTHandleAllocInfo(descriptor, filterMode, wrapMode, anisoLevel, mipMapBias, name));
		}

		public static RTHandle Alloc(ScaleFunc scaleFunc, RTHandleAllocInfo info)
		{
			return RTHandles.s_DefaultInstance.Alloc(scaleFunc, info);
		}

		public static RTHandle Alloc(Texture tex)
		{
			return RTHandles.s_DefaultInstance.Alloc(tex);
		}

		public static RTHandle Alloc(RenderTexture tex, bool transferOwnership = false)
		{
			return RTHandles.s_DefaultInstance.Alloc(tex, transferOwnership);
		}

		public static RTHandle Alloc(RenderTargetIdentifier tex)
		{
			return RTHandles.s_DefaultInstance.Alloc(tex);
		}

		public static RTHandle Alloc(RenderTargetIdentifier tex, string name)
		{
			return RTHandles.s_DefaultInstance.Alloc(tex, name);
		}

		private static RTHandle Alloc(RTHandle tex)
		{
			Debug.LogError("Allocation a RTHandle from another one is forbidden.");
			return null;
		}

		public static void Initialize(int width, int height)
		{
			RTHandles.s_DefaultInstance.Initialize(width, height);
		}

		[Obsolete("useLegacyDynamicResControl is deprecated. Please use SetHardwareDynamicResolutionState() instead.")]
		public static void Initialize(int width, int height, bool useLegacyDynamicResControl = false)
		{
			RTHandles.s_DefaultInstance.Initialize(width, height, useLegacyDynamicResControl);
		}

		public static void Release(RTHandle rth)
		{
			RTHandles.s_DefaultInstance.Release(rth);
		}

		public static void SetHardwareDynamicResolutionState(bool hwDynamicResRequested)
		{
			RTHandles.s_DefaultInstance.SetHardwareDynamicResolutionState(hwDynamicResRequested);
		}

		public static void SetReferenceSize(int width, int height)
		{
			RTHandles.s_DefaultInstance.SetReferenceSize(width, height);
		}

		public static void ResetReferenceSize(int width, int height)
		{
			RTHandles.s_DefaultInstance.ResetReferenceSize(width, height);
		}

		public static Vector2 CalculateRatioAgainstMaxSize(int width, int height)
		{
			RTHandleSystem rthandleSystem = RTHandles.s_DefaultInstance;
			Vector2Int vector2Int = new Vector2Int(width, height);
			return rthandleSystem.CalculateRatioAgainstMaxSize(vector2Int);
		}

		private static RTHandleSystem s_DefaultInstance = new RTHandleSystem();
	}
}
