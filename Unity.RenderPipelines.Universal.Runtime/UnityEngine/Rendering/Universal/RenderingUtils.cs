using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	public static class RenderingUtils
	{
		internal static AttachmentDescriptor emptyAttachment
		{
			get
			{
				return RenderingUtils.s_EmptyAttachment;
			}
		}

		[Obsolete("Use Blitter.BlitCameraTexture instead of CommandBuffer.DrawMesh(fullscreenMesh, ...)")]
		public static Mesh fullscreenMesh
		{
			get
			{
				if (RenderingUtils.s_FullscreenMesh != null)
				{
					return RenderingUtils.s_FullscreenMesh;
				}
				float y = 1f;
				float y2 = 0f;
				RenderingUtils.s_FullscreenMesh = new Mesh
				{
					name = "Fullscreen Quad"
				};
				RenderingUtils.s_FullscreenMesh.SetVertices(new List<Vector3>
				{
					new Vector3(-1f, -1f, 0f),
					new Vector3(-1f, 1f, 0f),
					new Vector3(1f, -1f, 0f),
					new Vector3(1f, 1f, 0f)
				});
				RenderingUtils.s_FullscreenMesh.SetUVs(0, new List<Vector2>
				{
					new Vector2(0f, y2),
					new Vector2(0f, y),
					new Vector2(1f, y2),
					new Vector2(1f, y)
				});
				RenderingUtils.s_FullscreenMesh.SetIndices(new int[]
				{
					0,
					1,
					2,
					2,
					1,
					3
				}, MeshTopology.Triangles, 0, false);
				RenderingUtils.s_FullscreenMesh.UploadMeshData(true);
				return RenderingUtils.s_FullscreenMesh;
			}
		}

		internal static bool useStructuredBuffer
		{
			get
			{
				return false;
			}
		}

		internal static bool SupportsLightLayers(GraphicsDeviceType type)
		{
			return true;
		}

		private static Material errorMaterial
		{
			get
			{
				if (RenderingUtils.s_ErrorMaterial == null)
				{
					try
					{
						RenderingUtils.s_ErrorMaterial = new Material(Shader.Find("Hidden/Universal Render Pipeline/FallbackError"));
					}
					catch
					{
					}
				}
				return RenderingUtils.s_ErrorMaterial;
			}
		}

		public static void SetViewAndProjectionMatrices(CommandBuffer cmd, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix, bool setInverseMatrices)
		{
			RenderingUtils.SetViewAndProjectionMatrices(CommandBufferHelpers.GetRasterCommandBuffer(cmd), viewMatrix, projectionMatrix, setInverseMatrices);
		}

		public static void SetViewAndProjectionMatrices(RasterCommandBuffer cmd, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix, bool setInverseMatrices)
		{
			Matrix4x4 value = projectionMatrix * viewMatrix;
			cmd.SetGlobalMatrix(ShaderPropertyId.viewMatrix, viewMatrix);
			cmd.SetGlobalMatrix(ShaderPropertyId.projectionMatrix, projectionMatrix);
			cmd.SetGlobalMatrix(ShaderPropertyId.viewAndProjectionMatrix, value);
			if (setInverseMatrices)
			{
				Matrix4x4 matrix4x = Matrix4x4.Inverse(viewMatrix);
				Matrix4x4 matrix4x2 = Matrix4x4.Inverse(projectionMatrix);
				Matrix4x4 value2 = matrix4x * matrix4x2;
				cmd.SetGlobalMatrix(ShaderPropertyId.inverseViewMatrix, matrix4x);
				cmd.SetGlobalMatrix(ShaderPropertyId.inverseProjectionMatrix, matrix4x2);
				cmd.SetGlobalMatrix(ShaderPropertyId.inverseViewAndProjectionMatrix, value2);
			}
		}

		internal static void SetScaleBiasRt(RasterCommandBuffer cmd, in UniversalCameraData cameraData, RTHandle rTHandle)
		{
			float num = (cameraData.cameraType != CameraType.Game || !(rTHandle.nameID == BuiltinRenderTextureType.CameraTarget) || !(cameraData.camera.targetTexture == null)) ? -1f : 1f;
			Vector4 value = (num < 0f) ? new Vector4(num, 1f, -1f, 1f) : new Vector4(num, 0f, 1f, 1f);
			cmd.SetGlobalVector(Shader.PropertyToID("_ScaleBiasRt"), value);
		}

		internal unsafe static void SetScaleBiasRt(RasterCommandBuffer cmd, in RenderingData renderingData)
		{
			CameraData cameraData = renderingData.cameraData;
			ScriptableRenderer scriptableRenderer = *cameraData.renderer;
			CameraData cameraData2 = renderingData.cameraData;
			float num = (*cameraData2.cameraType != CameraType.Game || !(scriptableRenderer.cameraColorTargetHandle.nameID == BuiltinRenderTextureType.CameraTarget) || !(cameraData2.camera->targetTexture == null)) ? -1f : 1f;
			Vector4 value = (num < 0f) ? new Vector4(num, 1f, -1f, 1f) : new Vector4(num, 0f, 1f, 1f);
			cmd.SetGlobalVector(Shader.PropertyToID("_ScaleBiasRt"), value);
		}

		internal static void Blit(CommandBuffer cmd, RTHandle source, Rect viewport, RTHandle destination, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction, ClearFlag clearFlag, Color clearColor, Material material, int passIndex = 0)
		{
			Vector2 v = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
			CoreUtils.SetRenderTarget(cmd, destination, loadAction, storeAction, ClearFlag.None, Color.clear, 0, CubemapFace.Unknown, -1);
			cmd.SetViewport(viewport);
			Blitter.BlitTexture(cmd, source, v, material, passIndex);
		}

		internal static void Blit(CommandBuffer cmd, RTHandle source, Rect viewport, RTHandle destinationColor, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RTHandle destinationDepthStencil, RenderBufferLoadAction depthStencilLoadAction, RenderBufferStoreAction depthStencilStoreAction, ClearFlag clearFlag, Color clearColor, Material material, int passIndex = 0)
		{
			Vector2 v = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
			CoreUtils.SetRenderTarget(cmd, destinationColor, colorLoadAction, colorStoreAction, destinationDepthStencil, depthStencilLoadAction, depthStencilStoreAction, clearFlag, clearColor, 0, CubemapFace.Unknown, -1);
			cmd.SetViewport(viewport);
			Blitter.BlitTexture(cmd, source, v, material, passIndex);
		}

		internal static void FinalBlit(CommandBuffer cmd, UniversalCameraData cameraData, RTHandle source, RTHandle destination, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction, Material material, int passIndex)
		{
			bool flag = !cameraData.isSceneViewCamera;
			if (cameraData.xr.enabled)
			{
				flag = (new RenderTargetIdentifier(destination.nameID, 0, CubemapFace.Unknown, -1) == new RenderTargetIdentifier(cameraData.xr.renderTarget, 0, CubemapFace.Unknown, -1));
			}
			Vector2 vector = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
			Vector4 scaleBias = (flag && cameraData.targetTexture == null && SystemInfo.graphicsUVStartsAtTop) ? new Vector4(vector.x, -vector.y, 0f, vector.y) : new Vector4(vector.x, vector.y, 0f, 0f);
			CoreUtils.SetRenderTarget(cmd, destination, loadAction, storeAction, ClearFlag.None, Color.clear, 0, CubemapFace.Unknown, -1);
			if (flag)
			{
				cmd.SetViewport(cameraData.pixelRect);
			}
			if (GL.wireframe && cameraData.isSceneViewCamera)
			{
				cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, loadAction, storeAction, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
				if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan)
				{
					cmd.SetWireframe(false);
					cmd.Blit(source, destination);
					cmd.SetWireframe(true);
					return;
				}
				cmd.Blit(source, destination);
				return;
			}
			else
			{
				if (source.rt == null)
				{
					Blitter.BlitTexture(cmd, source.nameID, scaleBias, material, passIndex);
					return;
				}
				Blitter.BlitTexture(cmd, source, scaleBias, material, passIndex);
				return;
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		internal static void CreateRendererParamsObjectsWithError(ref CullingResults cullResults, Camera camera, FilteringSettings filterSettings, SortingCriteria sortFlags, ref RendererListParams param)
		{
			SortingSettings sortingSettings = new SortingSettings(camera)
			{
				criteria = sortFlags
			};
			DrawingSettings drawSettings = new DrawingSettings(RenderingUtils.m_LegacyShaderPassNames[0], sortingSettings)
			{
				perObjectData = PerObjectData.None,
				overrideMaterial = RenderingUtils.errorMaterial,
				overrideMaterialPassIndex = 0
			};
			for (int i = 1; i < RenderingUtils.m_LegacyShaderPassNames.Count; i++)
			{
				drawSettings.SetShaderPassName(i, RenderingUtils.m_LegacyShaderPassNames[i]);
			}
			param = new RendererListParams(cullResults, drawSettings, filterSettings);
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		internal static void CreateRendererListObjectsWithError(ScriptableRenderContext context, ref CullingResults cullResults, Camera camera, FilteringSettings filterSettings, SortingCriteria sortFlags, ref RendererList rl)
		{
			if (RenderingUtils.errorMaterial == null)
			{
				rl = RendererList.nullRendererList;
				return;
			}
			RendererListParams rendererListParams = default(RendererListParams);
			rl = context.CreateRendererList(ref rendererListParams);
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		internal static void CreateRendererListObjectsWithError(RenderGraph renderGraph, ref CullingResults cullResults, Camera camera, FilteringSettings filterSettings, SortingCriteria sortFlags, ref RendererListHandle rl)
		{
			if (RenderingUtils.errorMaterial == null)
			{
				rl = default(RendererListHandle);
				return;
			}
			RendererListParams rendererListParams = default(RendererListParams);
			rl = renderGraph.CreateRendererList(rendererListParams);
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		internal static void DrawRendererListObjectsWithError(RasterCommandBuffer cmd, ref RendererList rl)
		{
			cmd.DrawRendererList(rl);
		}

		internal unsafe static void CreateRendererListWithRenderStateBlock(ScriptableRenderContext context, ref CullingResults cullResults, DrawingSettings ds, FilteringSettings fs, RenderStateBlock rsb, ref RendererList rl)
		{
			RendererListParams rendererListParams = default(RendererListParams);
			NativeArray<RenderStateBlock> value = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<RenderStateBlock>((void*)(&rsb), 1, Allocator.None);
			ShaderTagId none = ShaderTagId.none;
			NativeArray<ShaderTagId> value2 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ShaderTagId>((void*)(&none), 1, Allocator.None);
			rendererListParams = new RendererListParams(cullResults, ds, fs)
			{
				tagValues = new NativeArray<ShaderTagId>?(value2),
				stateBlocks = new NativeArray<RenderStateBlock>?(value)
			};
			rl = context.CreateRendererList(ref rendererListParams);
		}

		internal static void CreateRendererListWithRenderStateBlock(RenderGraph renderGraph, ref CullingResults cullResults, DrawingSettings ds, FilteringSettings fs, RenderStateBlock rsb, ref RendererListHandle rl)
		{
			RenderingUtils.s_ShaderTagValues[0] = ShaderTagId.none;
			RenderingUtils.s_RenderStateBlocks[0] = rsb;
			NativeArray<ShaderTagId> value = new NativeArray<ShaderTagId>(RenderingUtils.s_ShaderTagValues, Allocator.Temp);
			NativeArray<RenderStateBlock> value2 = new NativeArray<RenderStateBlock>(RenderingUtils.s_RenderStateBlocks, Allocator.Temp);
			RendererListParams rendererListParams = new RendererListParams(cullResults, ds, fs)
			{
				tagValues = new NativeArray<ShaderTagId>?(value),
				stateBlocks = new NativeArray<RenderStateBlock>?(value2),
				isPassTagName = false
			};
			rl = renderGraph.CreateRendererList(rendererListParams);
		}

		internal static void ClearSystemInfoCache()
		{
			RenderingUtils.m_RenderTextureFormatSupport.Clear();
		}

		public static bool SupportsRenderTextureFormat(RenderTextureFormat format)
		{
			bool flag;
			if (!RenderingUtils.m_RenderTextureFormatSupport.TryGetValue(format, out flag))
			{
				flag = SystemInfo.SupportsRenderTextureFormat(format);
				RenderingUtils.m_RenderTextureFormatSupport.Add(format, flag);
			}
			return flag;
		}

		[Obsolete("Use SystemInfo.IsFormatSupported instead.", false)]
		public static bool SupportsGraphicsFormat(GraphicsFormat format, FormatUsage usage)
		{
			GraphicsFormatUsage usage2 = (GraphicsFormatUsage)(1 << (int)usage);
			return SystemInfo.IsFormatSupported(format, usage2);
		}

		internal static int GetLastValidColorBufferIndex(RenderTargetIdentifier[] colorBuffers)
		{
			int num = colorBuffers.Length - 1;
			while (num >= 0 && !(colorBuffers[num] != 0))
			{
				num--;
			}
			return num;
		}

		internal static uint GetValidColorBufferCount(RTHandle[] colorBuffers)
		{
			uint num = 0U;
			if (colorBuffers != null)
			{
				foreach (RTHandle rthandle in colorBuffers)
				{
					if (rthandle != null && rthandle.nameID != 0)
					{
						num += 1U;
					}
				}
			}
			return num;
		}

		internal static bool IsMRT(RTHandle[] colorBuffers)
		{
			return RenderingUtils.GetValidColorBufferCount(colorBuffers) > 1U;
		}

		internal static bool Contains(RenderTargetIdentifier[] source, RenderTargetIdentifier value)
		{
			for (int i = 0; i < source.Length; i++)
			{
				if (source[i] == value)
				{
					return true;
				}
			}
			return false;
		}

		internal static int IndexOf(RTHandle[] source, RenderTargetIdentifier value)
		{
			for (int i = 0; i < source.Length; i++)
			{
				if (source[i] == value)
				{
					return i;
				}
			}
			return -1;
		}

		internal static int IndexOf(RTHandle[] source, RTHandle value)
		{
			return RenderingUtils.IndexOf(source, value.nameID);
		}

		internal static uint CountDistinct(RTHandle[] source, RTHandle value)
		{
			uint num = 0U;
			for (int i = 0; i < source.Length; i++)
			{
				if (source[i] != null && source[i].nameID != 0 && source[i].nameID != value.nameID)
				{
					num += 1U;
				}
			}
			return num;
		}

		internal static int LastValid(RTHandle[] source)
		{
			for (int i = source.Length - 1; i >= 0; i--)
			{
				if (source[i] != null && source[i].nameID != 0)
				{
					return i;
				}
			}
			return -1;
		}

		internal static bool Contains(ClearFlag a, ClearFlag b)
		{
			return (a & b) == b;
		}

		internal static bool SequenceEqual(RTHandle[] left, RTHandle[] right)
		{
			if (left.Length != right.Length)
			{
				return false;
			}
			for (int i = 0; i < left.Length; i++)
			{
				RTHandle rthandle = left[i];
				RenderTargetIdentifier? renderTargetIdentifier = (rthandle != null) ? new RenderTargetIdentifier?(rthandle.nameID) : null;
				RTHandle rthandle2 = right[i];
				if (renderTargetIdentifier != ((rthandle2 != null) ? new RenderTargetIdentifier?(rthandle2.nameID) : null))
				{
					return false;
				}
			}
			return true;
		}

		internal static bool MultisampleDepthResolveSupported()
		{
			return Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.OSXPlayer && SystemInfo.supportsMultisampleResolveDepth && SystemInfo.supportsMultisampleResolveStencil;
		}

		internal static bool RTHandleNeedsReAlloc(RTHandle handle, in TextureDesc descriptor, bool scaled)
		{
			if (handle == null || handle.rt == null)
			{
				return true;
			}
			if (handle.useScaling != scaled)
			{
				return true;
			}
			if (!scaled && (handle.rt.width != descriptor.width || handle.rt.height != descriptor.height))
			{
				return true;
			}
			if (handle.rt.enableShadingRate)
			{
				GraphicsFormat graphicsFormat = handle.rt.graphicsFormat;
				TextureDesc textureDesc = descriptor;
				if (graphicsFormat != textureDesc.colorFormat)
				{
					return true;
				}
			}
			RenderTextureDescriptor descriptor2 = handle.rt.descriptor;
			GraphicsFormat graphicsFormat2 = (descriptor2.depthStencilFormat != GraphicsFormat.None) ? descriptor2.depthStencilFormat : descriptor2.graphicsFormat;
			bool flag = descriptor2.shadowSamplingMode != ShadowSamplingMode.None;
			return graphicsFormat2 != descriptor.format || descriptor2.dimension != descriptor.dimension || descriptor2.volumeDepth != descriptor.slices || descriptor2.enableRandomWrite != descriptor.enableRandomWrite || descriptor2.enableShadingRate != descriptor.enableShadingRate || descriptor2.useMipMap != descriptor.useMipMap || descriptor2.autoGenerateMips != descriptor.autoGenerateMips || flag != descriptor.isShadowMap || descriptor2.msaaSamples != (int)descriptor.msaaSamples || descriptor2.bindMS != descriptor.bindTextureMS || descriptor2.useDynamicScale != descriptor.useDynamicScale || descriptor2.useDynamicScaleExplicit != descriptor.useDynamicScaleExplicit || descriptor2.memoryless != descriptor.memoryless || handle.rt.filterMode != descriptor.filterMode || handle.rt.wrapMode != descriptor.wrapMode || handle.rt.anisoLevel != descriptor.anisoLevel || Mathf.Abs(handle.rt.mipMapBias - descriptor.mipMapBias) > Mathf.Epsilon || handle.name != descriptor.name;
		}

		internal unsafe static RenderTargetIdentifier GetCameraTargetIdentifier(ref RenderingData renderingData)
		{
			ref CameraData ptr = ref renderingData.cameraData;
			RenderTargetIdentifier result = (*ptr.targetTexture != null) ? new RenderTargetIdentifier(*ptr.targetTexture) : BuiltinRenderTextureType.CameraTarget;
			if (ptr.xr.enabled)
			{
				if (ptr.xr.singlePassEnabled)
				{
					result = ptr.xr.renderTarget;
				}
				else
				{
					int textureArraySlice = ptr.xr.GetTextureArraySlice(0);
					result = new RenderTargetIdentifier(ptr.xr.renderTarget, 0, CubemapFace.Unknown, textureArraySlice);
				}
			}
			return result;
		}

		[Obsolete("This method will be removed in a future release. Please use ReAllocateHandleIfNeeded instead. #from(2023.3)")]
		public static bool ReAllocateIfNeeded(ref RTHandle handle, in RenderTextureDescriptor descriptor, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, string name = "")
		{
			TextureDesc textureDesc = RTHandleResourcePool.CreateTextureDesc(descriptor, TextureSizeMode.Explicit, anisoLevel, 0f, filterMode, wrapMode, name);
			if (!RenderingUtils.RTHandleNeedsReAlloc(handle, textureDesc, false))
			{
				return false;
			}
			if (handle != null && handle.rt != null)
			{
				RenderingUtils.AddStaleResourceToPoolOrRelease(RTHandleResourcePool.CreateTextureDesc(handle.rt.descriptor, TextureSizeMode.Explicit, handle.rt.anisoLevel, handle.rt.mipMapBias, handle.rt.filterMode, handle.rt.wrapMode, handle.name), handle);
			}
			if (UniversalRenderPipeline.s_RTHandlePool.TryGetResource(textureDesc, out handle, true))
			{
				return true;
			}
			handle = RTHandles.Alloc(descriptor, filterMode, wrapMode, isShadowMap, anisoLevel, mipMapBias, name);
			return true;
		}

		[Obsolete("This method will be removed in a future release. Please use ReAllocateHandleIfNeeded instead. #from(2023.3)")]
		public static bool ReAllocateIfNeeded(ref RTHandle handle, Vector2 scaleFactor, in RenderTextureDescriptor descriptor, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, string name = "")
		{
			bool flag = handle != null && handle.useScaling && handle.scaleFactor == scaleFactor;
			TextureDesc textureDesc = RTHandleResourcePool.CreateTextureDesc(descriptor, TextureSizeMode.Scale, anisoLevel, 0f, filterMode, wrapMode, "");
			if (flag && !RenderingUtils.RTHandleNeedsReAlloc(handle, textureDesc, true))
			{
				return false;
			}
			if (handle != null && handle.rt != null)
			{
				RenderingUtils.AddStaleResourceToPoolOrRelease(RTHandleResourcePool.CreateTextureDesc(handle.rt.descriptor, TextureSizeMode.Scale, handle.rt.anisoLevel, handle.rt.mipMapBias, handle.rt.filterMode, handle.rt.wrapMode, ""), handle);
			}
			if (UniversalRenderPipeline.s_RTHandlePool.TryGetResource(textureDesc, out handle, true))
			{
				return true;
			}
			handle = RTHandles.Alloc(scaleFactor, descriptor, filterMode, wrapMode, isShadowMap, anisoLevel, mipMapBias, name);
			return true;
		}

		[Obsolete("This method will be removed in a future release. Please use ReAllocateHandleIfNeeded instead. #from(2023.3)")]
		public static bool ReAllocateIfNeeded(ref RTHandle handle, ScaleFunc scaleFunc, in RenderTextureDescriptor descriptor, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, string name = "")
		{
			bool flag = handle != null && handle.useScaling && handle.scaleFactor == Vector2.zero;
			TextureDesc textureDesc = RTHandleResourcePool.CreateTextureDesc(descriptor, TextureSizeMode.Functor, anisoLevel, 0f, filterMode, wrapMode, "");
			if (flag && !RenderingUtils.RTHandleNeedsReAlloc(handle, textureDesc, true))
			{
				return false;
			}
			if (handle != null && handle.rt != null)
			{
				RenderingUtils.AddStaleResourceToPoolOrRelease(RTHandleResourcePool.CreateTextureDesc(handle.rt.descriptor, TextureSizeMode.Functor, handle.rt.anisoLevel, handle.rt.mipMapBias, handle.rt.filterMode, handle.rt.wrapMode, ""), handle);
			}
			if (UniversalRenderPipeline.s_RTHandlePool.TryGetResource(textureDesc, out handle, true))
			{
				return true;
			}
			handle = RTHandles.Alloc(scaleFunc, descriptor, filterMode, wrapMode, isShadowMap, anisoLevel, mipMapBias, name);
			return true;
		}

		public static bool ReAllocateHandleIfNeeded(ref RTHandle handle, in RenderTextureDescriptor descriptor, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, int anisoLevel = 1, float mipMapBias = 0f, string name = "")
		{
			TextureDesc textureDesc = RTHandleResourcePool.CreateTextureDesc(descriptor, TextureSizeMode.Explicit, anisoLevel, 0f, filterMode, wrapMode, name);
			if (!RenderingUtils.RTHandleNeedsReAlloc(handle, textureDesc, false))
			{
				return false;
			}
			if (handle != null && handle.rt != null)
			{
				RenderingUtils.AddStaleResourceToPoolOrRelease(RTHandleResourcePool.CreateTextureDesc(handle.rt.descriptor, TextureSizeMode.Explicit, handle.rt.anisoLevel, handle.rt.mipMapBias, handle.rt.filterMode, handle.rt.wrapMode, handle.name), handle);
			}
			if (UniversalRenderPipeline.s_RTHandlePool.TryGetResource(textureDesc, out handle, true))
			{
				return true;
			}
			RTHandleAllocInfo info = RenderingUtils.CreateRTHandleAllocInfo(descriptor, filterMode, wrapMode, anisoLevel, mipMapBias, name);
			handle = RTHandles.Alloc(descriptor.width, descriptor.height, info);
			return true;
		}

		public static bool ReAllocateHandleIfNeeded(ref RTHandle handle, TextureDesc descriptor, string name)
		{
			descriptor.name = name;
			descriptor.sizeMode = TextureSizeMode.Explicit;
			if (!RenderingUtils.RTHandleNeedsReAlloc(handle, descriptor, false))
			{
				return false;
			}
			if (handle != null && handle.rt != null)
			{
				RenderingUtils.AddStaleResourceToPoolOrRelease(RTHandleResourcePool.CreateTextureDesc(handle.rt.descriptor, TextureSizeMode.Explicit, handle.rt.anisoLevel, handle.rt.mipMapBias, handle.rt.filterMode, handle.rt.wrapMode, handle.name), handle);
			}
			if (UniversalRenderPipeline.s_RTHandlePool.TryGetResource(descriptor, out handle, true))
			{
				return true;
			}
			RTHandleAllocInfo info = RenderingUtils.CreateRTHandleAllocInfo(descriptor, name);
			handle = RTHandles.Alloc(descriptor.width, descriptor.height, info);
			return true;
		}

		public static bool ReAllocateHandleIfNeeded(ref RTHandle handle, Vector2 scaleFactor, in RenderTextureDescriptor descriptor, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, int anisoLevel = 1, float mipMapBias = 0f, string name = "")
		{
			bool flag = handle != null && handle.useScaling && handle.scaleFactor == scaleFactor;
			TextureDesc textureDesc = RTHandleResourcePool.CreateTextureDesc(descriptor, TextureSizeMode.Scale, anisoLevel, 0f, filterMode, wrapMode, "");
			if (flag && !RenderingUtils.RTHandleNeedsReAlloc(handle, textureDesc, true))
			{
				return false;
			}
			if (handle != null && handle.rt != null)
			{
				RenderingUtils.AddStaleResourceToPoolOrRelease(RTHandleResourcePool.CreateTextureDesc(handle.rt.descriptor, TextureSizeMode.Scale, handle.rt.anisoLevel, handle.rt.mipMapBias, handle.rt.filterMode, handle.rt.wrapMode, ""), handle);
			}
			if (UniversalRenderPipeline.s_RTHandlePool.TryGetResource(textureDesc, out handle, true))
			{
				return true;
			}
			RTHandleAllocInfo info = RenderingUtils.CreateRTHandleAllocInfo(descriptor, filterMode, wrapMode, anisoLevel, mipMapBias, name);
			handle = RTHandles.Alloc(scaleFactor, info);
			return true;
		}

		public static bool ReAllocateHandleIfNeeded(ref RTHandle handle, ScaleFunc scaleFunc, in RenderTextureDescriptor descriptor, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, int anisoLevel = 1, float mipMapBias = 0f, string name = "")
		{
			bool flag = handle != null && handle.useScaling && handle.scaleFactor == Vector2.zero;
			TextureDesc textureDesc = RTHandleResourcePool.CreateTextureDesc(descriptor, TextureSizeMode.Functor, anisoLevel, 0f, filterMode, wrapMode, "");
			if (flag && !RenderingUtils.RTHandleNeedsReAlloc(handle, textureDesc, true))
			{
				return false;
			}
			if (handle != null && handle.rt != null)
			{
				RenderingUtils.AddStaleResourceToPoolOrRelease(RTHandleResourcePool.CreateTextureDesc(handle.rt.descriptor, TextureSizeMode.Functor, handle.rt.anisoLevel, handle.rt.mipMapBias, handle.rt.filterMode, handle.rt.wrapMode, ""), handle);
			}
			if (UniversalRenderPipeline.s_RTHandlePool.TryGetResource(textureDesc, out handle, true))
			{
				return true;
			}
			RTHandleAllocInfo info = RenderingUtils.CreateRTHandleAllocInfo(descriptor, filterMode, wrapMode, anisoLevel, mipMapBias, name);
			handle = RTHandles.Alloc(scaleFunc, info);
			return true;
		}

		public static bool SetMaxRTHandlePoolCapacity(int capacity)
		{
			if (UniversalRenderPipeline.s_RTHandlePool == null)
			{
				return false;
			}
			UniversalRenderPipeline.s_RTHandlePool.staleResourceCapacity = capacity;
			return true;
		}

		internal static void AddStaleResourceToPoolOrRelease(TextureDesc desc, RTHandle handle)
		{
			if (!UniversalRenderPipeline.s_RTHandlePool.AddResourceToPool(desc, handle, Time.frameCount))
			{
				RTHandles.Release(handle);
			}
		}

		public static DrawingSettings CreateDrawingSettings(ShaderTagId shaderTagId, ref RenderingData renderingData, SortingCriteria sortingCriteria)
		{
			UniversalRenderingData renderingData2 = renderingData.frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = renderingData.frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = renderingData.frameData.Get<UniversalLightData>();
			return RenderingUtils.CreateDrawingSettings(shaderTagId, renderingData2, cameraData, lightData, sortingCriteria);
		}

		public static DrawingSettings CreateDrawingSettings(ShaderTagId shaderTagId, UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData, SortingCriteria sortingCriteria)
		{
			RenderGraphSettings renderGraphSettings;
			bool flag = !GraphicsSettings.TryGetRenderPipelineSettings<RenderGraphSettings>(out renderGraphSettings) || !renderGraphSettings.enableRenderCompatibilityMode;
			Camera camera = cameraData.camera;
			SortingSettings sortingSettings = new SortingSettings(camera)
			{
				criteria = sortingCriteria
			};
			return new DrawingSettings(shaderTagId, sortingSettings)
			{
				perObjectData = renderingData.perObjectData,
				mainLightIndex = lightData.mainLightIndex,
				enableDynamicBatching = renderingData.supportsDynamicBatching,
				enableInstancing = (camera.cameraType != CameraType.Preview),
				lodCrossFadeStencilMask = ((flag && renderingData.stencilLodCrossFadeEnabled) ? 12 : 0)
			};
		}

		public static DrawingSettings CreateDrawingSettings(List<ShaderTagId> shaderTagIdList, ref RenderingData renderingData, SortingCriteria sortingCriteria)
		{
			UniversalRenderingData renderingData2 = renderingData.frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = renderingData.frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = renderingData.frameData.Get<UniversalLightData>();
			return RenderingUtils.CreateDrawingSettings(shaderTagIdList, renderingData2, cameraData, lightData, sortingCriteria);
		}

		public static DrawingSettings CreateDrawingSettings(List<ShaderTagId> shaderTagIdList, UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData, SortingCriteria sortingCriteria)
		{
			if (shaderTagIdList == null || shaderTagIdList.Count == 0)
			{
				Debug.LogWarning("ShaderTagId list is invalid. DrawingSettings is created with default pipeline ShaderTagId");
				return RenderingUtils.CreateDrawingSettings(new ShaderTagId("UniversalPipeline"), renderingData, cameraData, lightData, sortingCriteria);
			}
			DrawingSettings result = RenderingUtils.CreateDrawingSettings(shaderTagIdList[0], renderingData, cameraData, lightData, sortingCriteria);
			for (int i = 1; i < shaderTagIdList.Count; i++)
			{
				result.SetShaderPassName(i, shaderTagIdList[i]);
			}
			return result;
		}

		internal static Vector4 GetFinalBlitScaleBias(RTHandle source, RTHandle destination, UniversalCameraData cameraData)
		{
			Vector2 vector = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
			if (cameraData.IsRenderTargetProjectionMatrixFlipped(destination, null))
			{
				return new Vector4(vector.x, vector.y, 0f, 0f);
			}
			return new Vector4(vector.x, -vector.y, 0f, vector.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static RTHandleAllocInfo CreateRTHandleAllocInfo(in RenderTextureDescriptor descriptor, FilterMode filterMode, TextureWrapMode wrapMode, int anisoLevel, float mipMapBias, string name)
		{
			RenderTextureDescriptor renderTextureDescriptor = descriptor;
			GraphicsFormat graphicsFormat;
			if (renderTextureDescriptor.graphicsFormat == GraphicsFormat.None)
			{
				graphicsFormat = descriptor.depthStencilFormat;
			}
			else
			{
				renderTextureDescriptor = descriptor;
				graphicsFormat = renderTextureDescriptor.graphicsFormat;
			}
			GraphicsFormat format = graphicsFormat;
			RTHandleAllocInfo result = default(RTHandleAllocInfo);
			result.slices = descriptor.volumeDepth;
			result.format = format;
			result.filterMode = filterMode;
			result.wrapModeU = wrapMode;
			result.wrapModeV = wrapMode;
			result.wrapModeW = wrapMode;
			result.dimension = descriptor.dimension;
			renderTextureDescriptor = descriptor;
			result.enableRandomWrite = renderTextureDescriptor.enableRandomWrite;
			renderTextureDescriptor = descriptor;
			result.enableShadingRate = renderTextureDescriptor.enableShadingRate;
			renderTextureDescriptor = descriptor;
			result.useMipMap = renderTextureDescriptor.useMipMap;
			renderTextureDescriptor = descriptor;
			result.autoGenerateMips = renderTextureDescriptor.autoGenerateMips;
			result.anisoLevel = anisoLevel;
			result.mipMapBias = mipMapBias;
			result.isShadowMap = (descriptor.shadowSamplingMode != ShadowSamplingMode.None);
			result.msaaSamples = (MSAASamples)descriptor.msaaSamples;
			renderTextureDescriptor = descriptor;
			result.bindTextureMS = renderTextureDescriptor.bindMS;
			renderTextureDescriptor = descriptor;
			result.useDynamicScale = renderTextureDescriptor.useDynamicScale;
			renderTextureDescriptor = descriptor;
			result.useDynamicScaleExplicit = renderTextureDescriptor.useDynamicScaleExplicit;
			result.memoryless = descriptor.memoryless;
			result.vrUsage = descriptor.vrUsage;
			renderTextureDescriptor = descriptor;
			result.enableShadingRate = renderTextureDescriptor.enableShadingRate;
			result.name = name;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static RTHandleAllocInfo CreateRTHandleAllocInfo(in TextureDesc descriptor, string name)
		{
			return new RTHandleAllocInfo
			{
				slices = descriptor.slices,
				format = descriptor.format,
				filterMode = descriptor.filterMode,
				wrapModeU = descriptor.wrapMode,
				wrapModeV = descriptor.wrapMode,
				wrapModeW = descriptor.wrapMode,
				dimension = descriptor.dimension,
				enableRandomWrite = descriptor.enableRandomWrite,
				enableShadingRate = descriptor.enableShadingRate,
				useMipMap = descriptor.useMipMap,
				autoGenerateMips = descriptor.autoGenerateMips,
				anisoLevel = descriptor.anisoLevel,
				mipMapBias = descriptor.mipMapBias,
				isShadowMap = descriptor.isShadowMap,
				msaaSamples = descriptor.msaaSamples,
				bindTextureMS = descriptor.bindTextureMS,
				useDynamicScale = descriptor.useDynamicScale,
				useDynamicScaleExplicit = descriptor.useDynamicScaleExplicit,
				memoryless = descriptor.memoryless,
				vrUsage = descriptor.vrUsage,
				enableShadingRate = descriptor.enableShadingRate,
				name = name
			};
		}

		private static List<ShaderTagId> m_LegacyShaderPassNames = new List<ShaderTagId>
		{
			new ShaderTagId("Always"),
			new ShaderTagId("ForwardBase"),
			new ShaderTagId("PrepassBase"),
			new ShaderTagId("Vertex"),
			new ShaderTagId("VertexLMRGBM"),
			new ShaderTagId("VertexLM")
		};

		private static AttachmentDescriptor s_EmptyAttachment = new AttachmentDescriptor(GraphicsFormat.None);

		private static Mesh s_FullscreenMesh = null;

		private static Material s_ErrorMaterial;

		private static ShaderTagId[] s_ShaderTagValues = new ShaderTagId[1];

		private static RenderStateBlock[] s_RenderStateBlocks = new RenderStateBlock[1];

		private static Dictionary<RenderTextureFormat, bool> m_RenderTextureFormatSupport = new Dictionary<RenderTextureFormat, bool>();
	}
}
