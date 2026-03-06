using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	public class RTHandleSystem : IDisposable
	{
		public RTHandleProperties rtHandleProperties
		{
			get
			{
				return this.m_RTHandleProperties;
			}
		}

		public RTHandleSystem()
		{
			this.m_AutoSizedRTs = new HashSet<RTHandle>();
			this.m_ResizeOnDemandRTs = new HashSet<RTHandle>();
			this.m_MaxWidths = 1;
			this.m_MaxHeights = 1;
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		public void Initialize(int width, int height)
		{
			if (this.m_AutoSizedRTs.Count != 0)
			{
				string arg = "Unreleased RTHandles:";
				foreach (RTHandle rthandle in this.m_AutoSizedRTs)
				{
					arg = string.Format("{0}\n    {1}", arg, rthandle.name);
				}
				Debug.LogError(string.Format("RTHandleSystem.Initialize should only be called once before allocating any Render Texture. This may be caused by an unreleased RTHandle resource.\n{0}\n", arg));
			}
			this.m_MaxWidths = width;
			this.m_MaxHeights = height;
			this.m_HardwareDynamicResRequested = DynamicResolutionHandler.instance.RequestsHardwareDynamicResolution();
		}

		[Obsolete("useLegacyDynamicResControl is deprecated. Please use SetHardwareDynamicResolutionState() instead.")]
		public void Initialize(int width, int height, bool useLegacyDynamicResControl = false)
		{
			this.Initialize(width, height);
			if (useLegacyDynamicResControl)
			{
				this.m_HardwareDynamicResRequested = true;
			}
		}

		public void Release(RTHandle rth)
		{
			if (rth != null)
			{
				rth.Release();
			}
		}

		internal void Remove(RTHandle rth)
		{
			this.m_AutoSizedRTs.Remove(rth);
		}

		public void ResetReferenceSize(int width, int height)
		{
			this.m_MaxWidths = width;
			this.m_MaxHeights = height;
			this.SetReferenceSize(width, height, true);
		}

		public void SetReferenceSize(int width, int height)
		{
			this.SetReferenceSize(width, height, false);
		}

		public void SetReferenceSize(int width, int height, bool reset)
		{
			this.m_RTHandleProperties.previousViewportSize = this.m_RTHandleProperties.currentViewportSize;
			this.m_RTHandleProperties.previousRenderTargetSize = this.m_RTHandleProperties.currentRenderTargetSize;
			Vector2 b = new Vector2((float)this.GetMaxWidth(), (float)this.GetMaxHeight());
			width = Mathf.Max(width, 1);
			height = Mathf.Max(height, 1);
			bool flag = width > this.GetMaxWidth() || height > this.GetMaxHeight() || reset;
			if (flag)
			{
				this.Resize(width, height, flag);
			}
			this.m_RTHandleProperties.currentViewportSize = new Vector2Int(width, height);
			this.m_RTHandleProperties.currentRenderTargetSize = new Vector2Int(this.GetMaxWidth(), this.GetMaxHeight());
			if (this.m_RTHandleProperties.previousViewportSize.x == 0)
			{
				this.m_RTHandleProperties.previousViewportSize = this.m_RTHandleProperties.currentViewportSize;
				this.m_RTHandleProperties.previousRenderTargetSize = this.m_RTHandleProperties.currentRenderTargetSize;
				b = new Vector2((float)this.GetMaxWidth(), (float)this.GetMaxHeight());
			}
			Vector2 vector = this.CalculateRatioAgainstMaxSize(this.m_RTHandleProperties.currentViewportSize);
			if (DynamicResolutionHandler.instance.HardwareDynamicResIsEnabled() && this.m_HardwareDynamicResRequested)
			{
				this.m_RTHandleProperties.rtHandleScale = new Vector4(vector.x, vector.y, this.m_RTHandleProperties.rtHandleScale.x, this.m_RTHandleProperties.rtHandleScale.y);
				return;
			}
			Vector2 vector2 = this.m_RTHandleProperties.previousViewportSize / b;
			this.m_RTHandleProperties.rtHandleScale = new Vector4(vector.x, vector.y, vector2.x, vector2.y);
		}

		internal Vector2 CalculateRatioAgainstMaxSize(in Vector2Int viewportSize)
		{
			Vector2 vector = new Vector2((float)this.GetMaxWidth(), (float)this.GetMaxHeight());
			if (DynamicResolutionHandler.instance.HardwareDynamicResIsEnabled() && this.m_HardwareDynamicResRequested && viewportSize != DynamicResolutionHandler.instance.finalViewport)
			{
				Vector2 scales = viewportSize / DynamicResolutionHandler.instance.finalViewport;
				vector = DynamicResolutionHandler.instance.ApplyScalesOnSize(new Vector2Int(this.GetMaxWidth(), this.GetMaxHeight()), scales);
			}
			Vector2Int vector2Int = viewportSize;
			float x = (float)vector2Int.x / vector.x;
			vector2Int = viewportSize;
			return new Vector2(x, (float)vector2Int.y / vector.y);
		}

		public void SetHardwareDynamicResolutionState(bool enableHWDynamicRes)
		{
			if (enableHWDynamicRes != this.m_HardwareDynamicResRequested)
			{
				this.m_HardwareDynamicResRequested = enableHWDynamicRes;
				Array.Resize<RTHandle>(ref this.m_AutoSizedRTsArray, this.m_AutoSizedRTs.Count);
				this.m_AutoSizedRTs.CopyTo(this.m_AutoSizedRTsArray);
				int i = 0;
				int num = this.m_AutoSizedRTsArray.Length;
				while (i < num)
				{
					RTHandle rthandle = this.m_AutoSizedRTsArray[i];
					RenderTexture rt = rthandle.m_RT;
					if (rt)
					{
						rt.Release();
						rt.useDynamicScale = (this.m_HardwareDynamicResRequested && rthandle.m_EnableHWDynamicScale);
						rt.Create();
					}
					i++;
				}
			}
		}

		internal void SwitchResizeMode(RTHandle rth, RTHandleSystem.ResizeMode mode)
		{
			if (!rth.useScaling)
			{
				return;
			}
			if (mode != RTHandleSystem.ResizeMode.Auto)
			{
				if (mode == RTHandleSystem.ResizeMode.OnDemand)
				{
					this.m_AutoSizedRTs.Remove(rth);
					this.m_ResizeOnDemandRTs.Add(rth);
					return;
				}
			}
			else
			{
				if (this.m_ResizeOnDemandRTs.Contains(rth))
				{
					this.DemandResize(rth);
				}
				this.m_ResizeOnDemandRTs.Remove(rth);
				this.m_AutoSizedRTs.Add(rth);
			}
		}

		private void DemandResize(RTHandle rth)
		{
			RenderTexture rt = rth.m_RT;
			rth.referenceSize = new Vector2Int(this.m_MaxWidths, this.m_MaxHeights);
			Vector2Int rhs = rth.GetScaledSize(rth.referenceSize);
			rhs = Vector2Int.Max(Vector2Int.one, rhs);
			if (rt.width != rhs.x || rt.height != rhs.y)
			{
				rt.Release();
				rt.width = rhs.x;
				rt.height = rhs.y;
				rt.name = CoreUtils.GetRenderTargetAutoName(rt.width, rt.height, rt.volumeDepth, (rt.depthStencilFormat != GraphicsFormat.None) ? rt.depthStencilFormat : rt.graphicsFormat, rt.dimension, rth.m_Name, rt.useMipMap, rth.m_EnableMSAA, (MSAASamples)rt.antiAliasing, rt.useDynamicScale, rt.useDynamicScaleExplicit);
				rt.Create();
			}
		}

		public int GetMaxWidth()
		{
			return this.m_MaxWidths;
		}

		public int GetMaxHeight()
		{
			return this.m_MaxHeights;
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				Array.Resize<RTHandle>(ref this.m_AutoSizedRTsArray, this.m_AutoSizedRTs.Count);
				this.m_AutoSizedRTs.CopyTo(this.m_AutoSizedRTsArray);
				int i = 0;
				int num = this.m_AutoSizedRTsArray.Length;
				while (i < num)
				{
					RTHandle rth = this.m_AutoSizedRTsArray[i];
					this.Release(rth);
					i++;
				}
				this.m_AutoSizedRTs.Clear();
				Array.Resize<RTHandle>(ref this.m_AutoSizedRTsArray, this.m_ResizeOnDemandRTs.Count);
				this.m_ResizeOnDemandRTs.CopyTo(this.m_AutoSizedRTsArray);
				int j = 0;
				int num2 = this.m_AutoSizedRTsArray.Length;
				while (j < num2)
				{
					RTHandle rth2 = this.m_AutoSizedRTsArray[j];
					this.Release(rth2);
					j++;
				}
				this.m_ResizeOnDemandRTs.Clear();
				this.m_AutoSizedRTsArray = null;
			}
		}

		private void Resize(int width, int height, bool sizeChanged)
		{
			this.m_MaxWidths = Math.Max(width, this.m_MaxWidths);
			this.m_MaxHeights = Math.Max(height, this.m_MaxHeights);
			Vector2Int vector2Int = new Vector2Int(this.m_MaxWidths, this.m_MaxHeights);
			Array.Resize<RTHandle>(ref this.m_AutoSizedRTsArray, this.m_AutoSizedRTs.Count);
			this.m_AutoSizedRTs.CopyTo(this.m_AutoSizedRTsArray);
			int i = 0;
			int num = this.m_AutoSizedRTsArray.Length;
			while (i < num)
			{
				RTHandle rthandle = this.m_AutoSizedRTsArray[i];
				rthandle.referenceSize = vector2Int;
				RenderTexture rt = rthandle.m_RT;
				rt.Release();
				Vector2Int scaledSize = rthandle.GetScaledSize(vector2Int);
				rt.width = Mathf.Max(scaledSize.x, 1);
				rt.height = Mathf.Max(scaledSize.y, 1);
				rt.name = CoreUtils.GetRenderTargetAutoName(rt.width, rt.height, rt.volumeDepth, (rt.depthStencilFormat != GraphicsFormat.None) ? rt.depthStencilFormat : rt.graphicsFormat, rt.dimension, rthandle.m_Name, rt.useMipMap, rthandle.m_EnableMSAA, (MSAASamples)rt.antiAliasing, rt.useDynamicScale, rt.useDynamicScaleExplicit);
				rt.Create();
				i++;
			}
		}

		public RTHandle Alloc(int width, int height, int slices = 1, DepthBits depthBufferBits = DepthBits.None, GraphicsFormat colorFormat = GraphicsFormat.R8G8B8A8_SRGB, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureDimension dimension = TextureDimension.Tex2D, bool enableRandomWrite = false, bool useMipMap = false, bool autoGenerateMips = true, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, MSAASamples msaaSamples = MSAASamples.None, bool bindTextureMS = false, bool useDynamicScale = false, bool useDynamicScaleExplicit = false, RenderTextureMemoryless memoryless = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, string name = "")
		{
			GraphicsFormat format = (depthBufferBits != DepthBits.None) ? GraphicsFormatUtility.GetDepthStencilFormat((int)depthBufferBits) : colorFormat;
			return this.Alloc(width, height, format, wrapMode, wrapMode, wrapMode, slices, filterMode, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, memoryless, vrUsage, name);
		}

		public RTHandle Alloc(int width, int height, GraphicsFormat format, int slices = 1, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureDimension dimension = TextureDimension.Tex2D, bool enableRandomWrite = false, bool useMipMap = false, bool autoGenerateMips = true, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, MSAASamples msaaSamples = MSAASamples.None, bool bindTextureMS = false, bool useDynamicScale = false, bool useDynamicScaleExplicit = false, RenderTextureMemoryless memoryless = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, string name = "")
		{
			return this.Alloc(width, height, format, wrapMode, wrapMode, wrapMode, slices, filterMode, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, memoryless, vrUsage, name);
		}

		public RTHandle Alloc(int width, int height, TextureWrapMode wrapModeU, TextureWrapMode wrapModeV, TextureWrapMode wrapModeW = TextureWrapMode.Repeat, int slices = 1, DepthBits depthBufferBits = DepthBits.None, GraphicsFormat colorFormat = GraphicsFormat.R8G8B8A8_SRGB, FilterMode filterMode = FilterMode.Point, TextureDimension dimension = TextureDimension.Tex2D, bool enableRandomWrite = false, bool useMipMap = false, bool autoGenerateMips = true, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, MSAASamples msaaSamples = MSAASamples.None, bool bindTextureMS = false, bool useDynamicScale = false, bool useDynamicScaleExplicit = false, RenderTextureMemoryless memoryless = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, string name = "")
		{
			GraphicsFormat format = (depthBufferBits != DepthBits.None) ? GraphicsFormatUtility.GetDepthStencilFormat((int)depthBufferBits) : colorFormat;
			return this.Alloc(width, height, format, wrapModeU, wrapModeV, wrapModeW, slices, filterMode, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, memoryless, vrUsage, name);
		}

		public RTHandle Alloc(int width, int height, GraphicsFormat format, TextureWrapMode wrapModeU, TextureWrapMode wrapModeV, TextureWrapMode wrapModeW = TextureWrapMode.Repeat, int slices = 1, FilterMode filterMode = FilterMode.Point, TextureDimension dimension = TextureDimension.Tex2D, bool enableRandomWrite = false, bool useMipMap = false, bool autoGenerateMips = true, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, MSAASamples msaaSamples = MSAASamples.None, bool bindTextureMS = false, bool useDynamicScale = false, bool useDynamicScaleExplicit = false, RenderTextureMemoryless memoryless = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, string name = "")
		{
			RenderTexture rt = this.CreateRenderTexture(width, height, format, slices, filterMode, wrapModeU, wrapModeV, wrapModeW, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, memoryless, vrUsage, false, name);
			RTHandle rthandle = new RTHandle(this);
			rthandle.SetRenderTexture(rt, true);
			rthandle.useScaling = false;
			rthandle.m_EnableRandomWrite = enableRandomWrite;
			rthandle.m_EnableMSAA = (msaaSamples != MSAASamples.None);
			rthandle.m_EnableHWDynamicScale = useDynamicScale;
			rthandle.m_Name = name;
			rthandle.referenceSize = new Vector2Int(width, height);
			return rthandle;
		}

		private RenderTexture CreateRenderTexture(int width, int height, GraphicsFormat format, int slices, FilterMode filterMode, TextureWrapMode wrapModeU, TextureWrapMode wrapModeV, TextureWrapMode wrapModeW, TextureDimension dimension, bool enableRandomWrite, bool useMipMap, bool autoGenerateMips, bool isShadowMap, int anisoLevel, float mipMapBias, MSAASamples msaaSamples, bool bindTextureMS, bool useDynamicScale, bool useDynamicScaleExplicit, RenderTextureMemoryless memoryless, VRTextureUsage vrUsage, bool enableShadingRate, string name)
		{
			bool flag = msaaSamples != MSAASamples.None;
			if (!flag && bindTextureMS)
			{
				Debug.LogWarning("RTHandle allocated without MSAA but with bindMS set to true, forcing bindMS to false.");
				bindTextureMS = false;
			}
			if (flag && enableRandomWrite)
			{
				Debug.LogWarning("RTHandle that is MSAA-enabled cannot allocate MSAA RT with 'enableRandomWrite = true'.");
				enableRandomWrite = false;
			}
			bool flag2 = GraphicsFormatUtility.IsDepthStencilFormat(format);
			if (enableShadingRate && (isShadowMap || flag2))
			{
				Debug.LogWarning("RTHandle allocated with incompatible enableShadingRate, forcing enableShadingRate to false.");
				enableShadingRate = false;
			}
			ShadowSamplingMode shadowSamplingMode = ShadowSamplingMode.None;
			GraphicsFormat depthStencilFormat;
			GraphicsFormat colorFormat;
			GraphicsFormat stencilFormat;
			string renderTargetAutoName;
			if (isShadowMap)
			{
				int num = GraphicsFormatUtility.GetDepthBits(format);
				if (num < 16)
				{
					num = 16;
				}
				depthStencilFormat = GraphicsFormatUtility.GetDepthStencilFormat(num, 0);
				colorFormat = GraphicsFormat.None;
				stencilFormat = GraphicsFormat.None;
				shadowSamplingMode = ShadowSamplingMode.CompareDepths;
				renderTargetAutoName = CoreUtils.GetRenderTargetAutoName(width, height, slices, RenderTextureFormat.Shadowmap, name, useMipMap, flag, msaaSamples);
			}
			else if (flag2)
			{
				colorFormat = GraphicsFormat.None;
				depthStencilFormat = format;
				stencilFormat = this.GetStencilFormat(format);
				renderTargetAutoName = CoreUtils.GetRenderTargetAutoName(width, height, slices, format, dimension, name, useMipMap, flag, msaaSamples, useDynamicScale, useDynamicScaleExplicit);
			}
			else
			{
				colorFormat = format;
				depthStencilFormat = GraphicsFormat.None;
				stencilFormat = GraphicsFormat.None;
				renderTargetAutoName = CoreUtils.GetRenderTargetAutoName(width, height, slices, format, dimension, name, useMipMap, flag, msaaSamples, useDynamicScale, useDynamicScaleExplicit);
			}
			RenderTexture renderTexture = new RenderTexture(new RenderTextureDescriptor(width, height, colorFormat, depthStencilFormat)
			{
				msaaSamples = (int)msaaSamples,
				volumeDepth = slices,
				stencilFormat = stencilFormat,
				dimension = dimension,
				shadowSamplingMode = shadowSamplingMode,
				vrUsage = vrUsage,
				memoryless = memoryless,
				useMipMap = useMipMap,
				autoGenerateMips = autoGenerateMips,
				enableRandomWrite = enableRandomWrite,
				bindMS = bindTextureMS,
				useDynamicScale = (this.m_HardwareDynamicResRequested && useDynamicScale),
				useDynamicScaleExplicit = (this.m_HardwareDynamicResRequested && useDynamicScaleExplicit),
				enableShadingRate = enableShadingRate
			});
			renderTexture.name = renderTargetAutoName;
			renderTexture.anisoLevel = anisoLevel;
			renderTexture.mipMapBias = mipMapBias;
			renderTexture.hideFlags = HideFlags.HideAndDontSave;
			renderTexture.filterMode = filterMode;
			renderTexture.wrapModeU = wrapModeU;
			renderTexture.wrapModeV = wrapModeV;
			renderTexture.wrapModeW = wrapModeW;
			renderTexture.Create();
			return renderTexture;
		}

		public RTHandle Alloc(int width, int height, RTHandleAllocInfo info)
		{
			RenderTexture rt = this.CreateRenderTexture(width, height, info.format, info.slices, info.filterMode, info.wrapModeU, info.wrapModeV, info.wrapModeW, info.dimension, info.enableRandomWrite, info.useMipMap, info.autoGenerateMips, info.isShadowMap, info.anisoLevel, info.mipMapBias, info.msaaSamples, info.bindTextureMS, info.useDynamicScale, info.useDynamicScaleExplicit, info.memoryless, info.vrUsage, info.enableShadingRate, info.name);
			RTHandle rthandle = new RTHandle(this);
			rthandle.SetRenderTexture(rt, true);
			rthandle.useScaling = false;
			rthandle.m_EnableRandomWrite = info.enableRandomWrite;
			rthandle.m_EnableMSAA = (info.msaaSamples != MSAASamples.None);
			rthandle.m_EnableHWDynamicScale = info.useDynamicScale;
			rthandle.m_Name = info.name;
			rthandle.referenceSize = new Vector2Int(width, height);
			if (info.enableShadingRate)
			{
				rthandle.scaleFunc = ((Vector2Int refSize) => ShadingRateImage.GetAllocTileSize(refSize));
			}
			return rthandle;
		}

		public Vector2Int CalculateDimensions(Vector2 scaleFactor)
		{
			return RTHandleSystem.CalculateDimensions(scaleFactor, new Vector2Int(this.GetMaxWidth(), this.GetMaxHeight()));
		}

		private static Vector2Int CalculateDimensions(Vector2 scaleFactor, Vector2Int size)
		{
			return new Vector2Int(Mathf.Max(Mathf.RoundToInt(scaleFactor.x * (float)size.x), 1), Mathf.Max(Mathf.RoundToInt(scaleFactor.y * (float)size.y), 1));
		}

		public RTHandle Alloc(Vector2 scaleFactor, GraphicsFormat format, int slices = 1, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureDimension dimension = TextureDimension.Tex2D, bool enableRandomWrite = false, bool useMipMap = false, bool autoGenerateMips = true, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, MSAASamples msaaSamples = MSAASamples.None, bool bindTextureMS = false, bool useDynamicScale = false, bool useDynamicScaleExplicit = false, RenderTextureMemoryless memoryless = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, string name = "")
		{
			Vector2Int referenceSize = this.CalculateDimensions(scaleFactor);
			bool enableShadingRate = false;
			RTHandle rthandle = this.AllocAutoSizedRenderTexture(referenceSize.x, referenceSize.y, slices, format, filterMode, wrapMode, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, memoryless, vrUsage, enableShadingRate, name);
			rthandle.referenceSize = referenceSize;
			rthandle.scaleFactor = scaleFactor;
			return rthandle;
		}

		public RTHandle Alloc(Vector2 scaleFactor, int slices = 1, DepthBits depthBufferBits = DepthBits.None, GraphicsFormat colorFormat = GraphicsFormat.R8G8B8A8_SRGB, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureDimension dimension = TextureDimension.Tex2D, bool enableRandomWrite = false, bool useMipMap = false, bool autoGenerateMips = true, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, MSAASamples msaaSamples = MSAASamples.None, bool bindTextureMS = false, bool useDynamicScale = false, bool useDynamicScaleExplicit = false, RenderTextureMemoryless memoryless = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, string name = "")
		{
			GraphicsFormat format = (depthBufferBits != DepthBits.None) ? GraphicsFormatUtility.GetDepthStencilFormat((int)depthBufferBits) : colorFormat;
			return this.Alloc(scaleFactor, format, slices, filterMode, wrapMode, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, memoryless, vrUsage, name);
		}

		public RTHandle Alloc(Vector2 scaleFactor, RTHandleAllocInfo info)
		{
			int num = Mathf.Max(Mathf.RoundToInt(scaleFactor.x * (float)this.GetMaxWidth()), 1);
			int num2 = Mathf.Max(Mathf.RoundToInt(scaleFactor.y * (float)this.GetMaxHeight()), 1);
			RTHandle rthandle = this.AllocAutoSizedRenderTexture(num, num2, info);
			rthandle.referenceSize = new Vector2Int(num, num2);
			if (info.enableShadingRate)
			{
				rthandle.scaleFunc = ((Vector2Int refSize) => ShadingRateImage.GetAllocTileSize(RTHandleSystem.CalculateDimensions(scaleFactor, refSize)));
			}
			else
			{
				rthandle.scaleFactor = scaleFactor;
			}
			return rthandle;
		}

		public Vector2Int CalculateDimensions(ScaleFunc scaleFunc)
		{
			Vector2Int vector2Int = scaleFunc(new Vector2Int(this.GetMaxWidth(), this.GetMaxHeight()));
			return new Vector2Int(Mathf.Max(vector2Int.x, 1), Mathf.Max(vector2Int.y, 1));
		}

		public RTHandle Alloc(ScaleFunc scaleFunc, int slices = 1, DepthBits depthBufferBits = DepthBits.None, GraphicsFormat colorFormat = GraphicsFormat.R8G8B8A8_SRGB, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureDimension dimension = TextureDimension.Tex2D, bool enableRandomWrite = false, bool useMipMap = false, bool autoGenerateMips = true, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, MSAASamples msaaSamples = MSAASamples.None, bool bindTextureMS = false, bool useDynamicScale = false, bool useDynamicScaleExplicit = false, RenderTextureMemoryless memoryless = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, string name = "")
		{
			GraphicsFormat format = (depthBufferBits != DepthBits.None) ? GraphicsFormatUtility.GetDepthStencilFormat((int)depthBufferBits) : colorFormat;
			return this.Alloc(scaleFunc, format, slices, filterMode, wrapMode, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, memoryless, vrUsage, name);
		}

		public RTHandle Alloc(ScaleFunc scaleFunc, GraphicsFormat format, int slices = 1, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureDimension dimension = TextureDimension.Tex2D, bool enableRandomWrite = false, bool useMipMap = false, bool autoGenerateMips = true, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, MSAASamples msaaSamples = MSAASamples.None, bool bindTextureMS = false, bool useDynamicScale = false, bool useDynamicScaleExplicit = false, RenderTextureMemoryless memoryless = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, string name = "")
		{
			Vector2Int referenceSize = this.CalculateDimensions(scaleFunc);
			bool enableShadingRate = false;
			RTHandle rthandle = this.AllocAutoSizedRenderTexture(referenceSize.x, referenceSize.y, slices, format, filterMode, wrapMode, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, memoryless, vrUsage, enableShadingRate, name);
			rthandle.referenceSize = referenceSize;
			rthandle.scaleFunc = scaleFunc;
			return rthandle;
		}

		public RTHandle Alloc(ScaleFunc scaleFunc, RTHandleAllocInfo info)
		{
			Vector2Int vector2Int = scaleFunc(new Vector2Int(this.GetMaxWidth(), this.GetMaxHeight()));
			int num = Mathf.Max(vector2Int.x, 1);
			int num2 = Mathf.Max(vector2Int.y, 1);
			RTHandle rthandle = this.AllocAutoSizedRenderTexture(num, num2, info);
			rthandle.referenceSize = new Vector2Int(num, num2);
			if (info.enableShadingRate)
			{
				rthandle.scaleFunc = ((Vector2Int refSize) => ShadingRateImage.GetAllocTileSize(scaleFunc(refSize)));
			}
			else
			{
				rthandle.scaleFunc = scaleFunc;
			}
			return rthandle;
		}

		internal RTHandle AllocAutoSizedRenderTexture(int width, int height, int slices, GraphicsFormat format, FilterMode filterMode, TextureWrapMode wrapMode, TextureDimension dimension, bool enableRandomWrite, bool useMipMap, bool autoGenerateMips, bool isShadowMap, int anisoLevel, float mipMapBias, MSAASamples msaaSamples, bool bindTextureMS, bool useDynamicScale, bool useDynamicScaleExplicit, RenderTextureMemoryless memoryless, VRTextureUsage vrUsage, bool enableShadingRate, string name)
		{
			if (enableShadingRate)
			{
				Vector2Int allocTileSize = ShadingRateImage.GetAllocTileSize(width, height);
				width = allocTileSize.x;
				height = allocTileSize.y;
			}
			RenderTexture rt = this.CreateRenderTexture(width, height, format, slices, filterMode, wrapMode, wrapMode, wrapMode, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, memoryless, vrUsage, enableShadingRate, name);
			RTHandle rthandle = new RTHandle(this);
			rthandle.SetRenderTexture(rt, true);
			rthandle.m_EnableMSAA = (msaaSamples != MSAASamples.None);
			rthandle.m_EnableRandomWrite = enableRandomWrite;
			rthandle.useScaling = true;
			rthandle.m_EnableHWDynamicScale = useDynamicScale;
			rthandle.m_Name = name;
			this.m_AutoSizedRTs.Add(rthandle);
			return rthandle;
		}

		internal RTHandle AllocAutoSizedRenderTexture(int width, int height, RTHandleAllocInfo info)
		{
			if (info.enableShadingRate)
			{
				Vector2Int allocTileSize = ShadingRateImage.GetAllocTileSize(width, height);
				width = allocTileSize.x;
				height = allocTileSize.y;
			}
			RenderTexture rt = this.CreateRenderTexture(width, height, info.format, info.slices, info.filterMode, info.wrapModeU, info.wrapModeV, info.wrapModeW, info.dimension, info.enableRandomWrite, info.useMipMap, info.autoGenerateMips, info.isShadowMap, info.anisoLevel, info.mipMapBias, info.msaaSamples, info.bindTextureMS, info.useDynamicScale, info.useDynamicScaleExplicit, info.memoryless, info.vrUsage, info.enableShadingRate, info.name);
			RTHandle rthandle = new RTHandle(this);
			rthandle.SetRenderTexture(rt, true);
			rthandle.m_EnableMSAA = (info.msaaSamples != MSAASamples.None);
			rthandle.m_EnableRandomWrite = info.enableRandomWrite;
			rthandle.useScaling = true;
			rthandle.m_EnableHWDynamicScale = info.useDynamicScale;
			rthandle.m_Name = info.name;
			this.m_AutoSizedRTs.Add(rthandle);
			return rthandle;
		}

		public RTHandle Alloc(RenderTexture texture, bool transferOwnership = false)
		{
			RTHandle rthandle = new RTHandle(this);
			rthandle.SetRenderTexture(texture, transferOwnership);
			rthandle.m_EnableMSAA = false;
			rthandle.m_EnableRandomWrite = false;
			rthandle.useScaling = false;
			rthandle.m_EnableHWDynamicScale = false;
			rthandle.m_Name = texture.name;
			return rthandle;
		}

		public RTHandle Alloc(Texture texture)
		{
			RTHandle rthandle = new RTHandle(this);
			rthandle.SetTexture(texture);
			rthandle.m_EnableMSAA = false;
			rthandle.m_EnableRandomWrite = false;
			rthandle.useScaling = false;
			rthandle.m_EnableHWDynamicScale = false;
			rthandle.m_Name = texture.name;
			return rthandle;
		}

		public RTHandle Alloc(RenderTargetIdentifier texture)
		{
			return this.Alloc(texture, "");
		}

		public RTHandle Alloc(RenderTargetIdentifier texture, string name)
		{
			RTHandle rthandle = new RTHandle(this);
			rthandle.SetTexture(texture);
			rthandle.m_EnableMSAA = false;
			rthandle.m_EnableRandomWrite = false;
			rthandle.useScaling = false;
			rthandle.m_EnableHWDynamicScale = false;
			rthandle.m_Name = name;
			return rthandle;
		}

		private static RTHandle Alloc(RTHandle tex)
		{
			Debug.LogError("Allocation a RTHandle from another one is forbidden.");
			return null;
		}

		internal string DumpRTInfo()
		{
			string text = "";
			Array.Resize<RTHandle>(ref this.m_AutoSizedRTsArray, this.m_AutoSizedRTs.Count);
			this.m_AutoSizedRTs.CopyTo(this.m_AutoSizedRTsArray);
			int i = 0;
			int num = this.m_AutoSizedRTsArray.Length;
			while (i < num)
			{
				RenderTexture rt = this.m_AutoSizedRTsArray[i].rt;
				text = string.Format("{0}\nRT ({1})\t Format: {2} W: {3} H {4}\n", new object[]
				{
					text,
					i,
					rt.format,
					rt.width,
					rt.height
				});
				i++;
			}
			return text;
		}

		private GraphicsFormat GetStencilFormat(GraphicsFormat depthStencilFormat)
		{
			if (!GraphicsFormatUtility.IsStencilFormat(depthStencilFormat) || !SystemInfo.IsFormatSupported(GraphicsFormat.R8_UInt, GraphicsFormatUsage.StencilSampling))
			{
				return GraphicsFormat.None;
			}
			return GraphicsFormat.R8_UInt;
		}

		private bool m_HardwareDynamicResRequested;

		private HashSet<RTHandle> m_AutoSizedRTs;

		private RTHandle[] m_AutoSizedRTsArray;

		private HashSet<RTHandle> m_ResizeOnDemandRTs;

		private RTHandleProperties m_RTHandleProperties;

		private int m_MaxWidths;

		private int m_MaxHeights;

		internal enum ResizeMode
		{
			Auto,
			OnDemand
		}
	}
}
