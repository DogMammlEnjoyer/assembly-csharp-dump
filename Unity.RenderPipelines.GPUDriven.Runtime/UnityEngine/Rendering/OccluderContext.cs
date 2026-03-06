using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering
{
	internal struct OccluderContext : IDisposable
	{
		public int subviewCount
		{
			get
			{
				return this.subviewData.Length;
			}
		}

		public bool IsSubviewValid(int subviewIndex)
		{
			return subviewIndex < this.subviewCount && (this.subviewValidMask & 1 << subviewIndex) != 0;
		}

		public Vector2 depthBufferSizeInOccluderPixels
		{
			get
			{
				int num = 8;
				return new Vector2((float)this.depthBufferSize.x / (float)num, (float)this.depthBufferSize.y / (float)num);
			}
		}

		public void Dispose()
		{
			if (this.subviewData.IsCreated)
			{
				this.subviewData.Dispose();
			}
			if (this.occluderMipBounds.IsCreated)
			{
				this.occluderMipBounds.Dispose();
			}
			if (this.occluderDepthPyramid != null)
			{
				this.occluderDepthPyramid.Release();
				this.occluderDepthPyramid = null;
			}
			if (this.occlusionDebugOverlay != null)
			{
				this.occlusionDebugOverlay.Release();
				this.occlusionDebugOverlay = null;
			}
			if (this.constantBuffer != null)
			{
				this.constantBuffer.Release();
				this.constantBuffer = null;
			}
			if (this.constantBufferData.IsCreated)
			{
				this.constantBufferData.Dispose();
			}
		}

		private void UpdateMipBounds()
		{
			int num = 8;
			Vector2Int vector2Int = (this.depthBufferSize + (num - 1) * Vector2Int.one) / num;
			Vector2Int zero = Vector2Int.zero;
			Vector2Int zero2 = Vector2Int.zero;
			Vector2Int size = vector2Int;
			if (!this.occluderMipBounds.IsCreated)
			{
				this.occluderMipBounds = new NativeArray<OccluderMipBounds>(8, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
			for (int i = 0; i < 8; i++)
			{
				this.occluderMipBounds[i] = new OccluderMipBounds
				{
					offset = zero2,
					size = size
				};
				zero.x = Mathf.Max(zero.x, zero2.x + size.x);
				zero.y = Mathf.Max(zero.y, zero2.y + size.y);
				if (i == 0)
				{
					zero2.x = 0;
					zero2.y += size.y;
				}
				else
				{
					zero2.x += size.x;
				}
				size.x = (size.x + 1) / 2;
				size.y = (size.y + 1) / 2;
			}
			this.occluderMipLayoutSize = zero;
		}

		private void AllocateTexturesIfNecessary(bool debugOverlayEnabled)
		{
			Vector2Int vector2Int = new Vector2Int(this.occluderMipLayoutSize.x, this.occluderMipLayoutSize.y * this.subviewCount);
			if (this.occluderDepthPyramidSize.x < vector2Int.x || this.occluderDepthPyramidSize.y < vector2Int.y)
			{
				if (this.occluderDepthPyramid != null)
				{
					this.occluderDepthPyramid.Release();
				}
				this.occluderDepthPyramidSize = vector2Int;
				this.occluderDepthPyramid = RTHandles.Alloc(this.occluderDepthPyramidSize.x, this.occluderDepthPyramidSize.y, GraphicsFormat.R32_SFloat, 1, FilterMode.Point, TextureWrapMode.Clamp, TextureDimension.Tex2D, true, false, true, false, 1, 0f, MSAASamples.None, false, false, false, RenderTextureMemoryless.None, VRTextureUsage.None, "Occluder Depths");
			}
			int num = debugOverlayEnabled ? (vector2Int.x * vector2Int.y) : 0;
			if (this.occlusionDebugOverlaySize < num)
			{
				if (this.occlusionDebugOverlay != null)
				{
					this.occlusionDebugOverlay.Release();
				}
				this.occlusionDebugOverlaySize = num;
				this.debugNeedsClear = true;
				this.occlusionDebugOverlay = new GraphicsBuffer(GraphicsBuffer.Target.Structured, GraphicsBuffer.UsageFlags.None, this.occlusionDebugOverlaySize + 4, 4);
			}
			if (num == 0)
			{
				if (this.occlusionDebugOverlay != null)
				{
					this.occlusionDebugOverlay.Release();
					this.occlusionDebugOverlay = null;
				}
				this.occlusionDebugOverlaySize = num;
			}
			if (this.constantBuffer == null)
			{
				this.constantBuffer = new ComputeBuffer(1, UnsafeUtility.SizeOf<OccluderDepthPyramidConstants>(), ComputeBufferType.Constant);
			}
			if (!this.constantBufferData.IsCreated)
			{
				this.constantBufferData = new NativeArray<OccluderDepthPyramidConstants>(1, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
		}

		internal static void SetKeyword(ComputeCommandBuffer cmd, ComputeShader cs, in LocalKeyword keyword, bool value)
		{
			if (value)
			{
				cmd.EnableKeyword(cs, keyword);
				return;
			}
			cmd.DisableKeyword(cs, keyword);
		}

		private unsafe OccluderDepthPyramidConstants SetupFarDepthPyramidConstants(ReadOnlySpan<OccluderSubviewUpdate> occluderSubviewUpdates, NativeArray<Plane> silhouettePlanes)
		{
			OccluderDepthPyramidConstants result = default(OccluderDepthPyramidConstants);
			result._OccluderMipLayoutSizeX = (uint)this.occluderMipLayoutSize.x;
			result._OccluderMipLayoutSizeY = (uint)this.occluderMipLayoutSize.y;
			int length = occluderSubviewUpdates.Length;
			for (int i = 0; i < length; i++)
			{
				ref readonly OccluderSubviewUpdate ptr = ref occluderSubviewUpdates[i];
				int subviewIndex = ptr.subviewIndex;
				this.subviewData[subviewIndex] = OccluderDerivedData.FromParameters(ptr);
				this.subviewValidMask |= 1 << ptr.subviewIndex;
				Matrix4x4 inverse = (ptr.gpuProjMatrix * ptr.viewMatrix * Matrix4x4.Translate(-ptr.viewOffsetWorldSpace)).inverse;
				for (int j = 0; j < 16; j++)
				{
					*(ref result._InvViewProjMatrix.FixedElementField + (IntPtr)(16 * i + j) * 4) = inverse[j];
				}
				ref int ptr2 = ref result._SrcOffset.FixedElementField + (IntPtr)(4 * i) * 4;
				Vector2Int depthOffset = ptr.depthOffset;
				ptr2 = depthOffset.x;
				ref int ptr3 = ref result._SrcOffset.FixedElementField + (IntPtr)(4 * i + 1) * 4;
				depthOffset = ptr.depthOffset;
				ptr3 = depthOffset.y;
				*(ref result._SrcOffset.FixedElementField + (IntPtr)(4 * i + 2) * 4) = 0U;
				*(ref result._SrcOffset.FixedElementField + (IntPtr)(4 * i + 3) * 4) = 0U;
				result._SrcSliceIndices |= (uint)((uint)(ptr.depthSliceIndex & 15) << 4 * i);
				result._DstSubviewIndices |= (uint)((uint)subviewIndex << 4 * i);
			}
			for (int k = 0; k < 6; k++)
			{
				Plane plane = new Plane(Vector3.zero, 0f);
				if (k < silhouettePlanes.Length)
				{
					plane = silhouettePlanes[k];
				}
				*(ref result._SilhouettePlanes.FixedElementField + (IntPtr)(4 * k) * 4) = plane.normal.x;
				*(ref result._SilhouettePlanes.FixedElementField + (IntPtr)(4 * k + 1) * 4) = plane.normal.y;
				*(ref result._SilhouettePlanes.FixedElementField + (IntPtr)(4 * k + 2) * 4) = plane.normal.z;
				*(ref result._SilhouettePlanes.FixedElementField + (IntPtr)(4 * k + 3) * 4) = plane.distance;
			}
			result._SilhouettePlaneCount = (uint)silhouettePlanes.Length;
			return result;
		}

		public unsafe void CreateFarDepthPyramid(ComputeCommandBuffer cmd, in OccluderParameters occluderParams, ReadOnlySpan<OccluderSubviewUpdate> occluderSubviewUpdates, in OccluderHandles occluderHandles, NativeArray<Plane> silhouettePlanes, ComputeShader occluderDepthPyramidCS, int occluderDepthDownscaleKernel)
		{
			OccluderDepthPyramidConstants value = this.SetupFarDepthPyramidConstants(occluderSubviewUpdates, silhouettePlanes);
			LocalKeyword localKeyword = new LocalKeyword(occluderDepthPyramidCS, "USE_SRC");
			LocalKeyword localKeyword2 = new LocalKeyword(occluderDepthPyramidCS, "SRC_IS_ARRAY");
			LocalKeyword localKeyword3 = new LocalKeyword(occluderDepthPyramidCS, "SRC_IS_MSAA");
			bool depthIsArray = occluderParams.depthIsArray;
			RTHandle rthandle = occluderParams.depthTexture;
			bool flag = rthandle != null && rthandle.isMSAAEnabled;
			int num = 11;
			for (int i = 0; i < num - 1; i += 4)
			{
				cmd.SetComputeTextureParam(occluderDepthPyramidCS, occluderDepthDownscaleKernel, OccluderContext.ShaderIDs._DstDepth, occluderHandles.occluderDepthPyramid);
				bool flag2 = i == 0;
				OccluderContext.SetKeyword(cmd, occluderDepthPyramidCS, localKeyword, flag2);
				OccluderContext.SetKeyword(cmd, occluderDepthPyramidCS, localKeyword2, flag2 && depthIsArray);
				OccluderContext.SetKeyword(cmd, occluderDepthPyramidCS, localKeyword3, flag2 && flag);
				if (flag2)
				{
					cmd.SetComputeTextureParam(occluderDepthPyramidCS, occluderDepthDownscaleKernel, OccluderContext.ShaderIDs._SrcDepth, occluderParams.depthTexture);
				}
				value._MipCount = (uint)Math.Min(num - 1 - i, 4);
				Vector2Int vector2Int = Vector2Int.zero;
				for (int j = 0; j < 5; j++)
				{
					Vector2Int vector2Int2 = Vector2Int.zero;
					Vector2Int vector2Int3 = Vector2Int.zero;
					int num2 = i + j;
					if (num2 == 0)
					{
						vector2Int3 = occluderParams.depthSize;
					}
					else
					{
						int num3 = num2 - 3;
						if (0 <= num3 && num3 < 8)
						{
							vector2Int2 = this.occluderMipBounds[num3].offset;
							vector2Int3 = this.occluderMipBounds[num3].size;
						}
					}
					if (j == 0)
					{
						vector2Int = vector2Int3;
					}
					*(ref value._MipOffsetAndSize.FixedElementField + (IntPtr)(4 * j) * 4) = (uint)vector2Int2.x;
					*(ref value._MipOffsetAndSize.FixedElementField + (IntPtr)(4 * j + 1) * 4) = (uint)vector2Int2.y;
					*(ref value._MipOffsetAndSize.FixedElementField + (IntPtr)(4 * j + 2) * 4) = (uint)vector2Int3.x;
					*(ref value._MipOffsetAndSize.FixedElementField + (IntPtr)(4 * j + 3) * 4) = (uint)vector2Int3.y;
				}
				this.constantBufferData[0] = value;
				cmd.SetBufferData<OccluderDepthPyramidConstants>(this.constantBuffer, this.constantBufferData);
				cmd.SetComputeConstantBufferParam(occluderDepthPyramidCS, OccluderContext.ShaderIDs.OccluderDepthPyramidConstants, this.constantBuffer, 0, this.constantBuffer.stride);
				cmd.DispatchCompute(occluderDepthPyramidCS, occluderDepthDownscaleKernel, (vector2Int.x + 15) / 16, (vector2Int.y + 15) / 16, occluderSubviewUpdates.Length);
			}
		}

		public OccluderHandles Import(RenderGraph renderGraph)
		{
			RenderTargetInfo info = new RenderTargetInfo
			{
				width = this.occluderDepthPyramidSize.x,
				height = this.occluderDepthPyramidSize.y,
				volumeDepth = 1,
				msaaSamples = 1,
				format = GraphicsFormat.R32_SFloat,
				bindMS = false
			};
			OccluderHandles result = new OccluderHandles
			{
				occluderDepthPyramid = renderGraph.ImportTexture(this.occluderDepthPyramid, info, default(ImportResourceParams))
			};
			if (this.occlusionDebugOverlay != null)
			{
				result.occlusionDebugOverlay = renderGraph.ImportBuffer(this.occlusionDebugOverlay, false);
			}
			return result;
		}

		public void PrepareOccluders(in OccluderParameters occluderParams)
		{
			if (this.subviewCount != occluderParams.subviewCount)
			{
				if (this.subviewData.IsCreated)
				{
					this.subviewData.Dispose();
				}
				this.subviewData = new NativeArray<OccluderDerivedData>(occluderParams.subviewCount, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				this.subviewValidMask = 0;
			}
			this.depthBufferSize = occluderParams.depthSize;
			DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
			bool debugOverlayEnabled = debugStats != null && debugStats.occlusionOverlayEnabled;
			this.UpdateMipBounds();
			this.AllocateTexturesIfNecessary(debugOverlayEnabled);
		}

		internal unsafe OcclusionCullingDebugOutput GetDebugOutput()
		{
			OcclusionCullingDebugOutput result = new OcclusionCullingDebugOutput
			{
				occluderDepthPyramid = this.occluderDepthPyramid,
				occlusionDebugOverlay = this.occlusionDebugOverlay
			};
			result.cb._DepthSizeInOccluderPixels = this.depthBufferSizeInOccluderPixels;
			result.cb._OccluderMipLayoutSizeX = (uint)this.occluderMipLayoutSize.x;
			result.cb._OccluderMipLayoutSizeY = (uint)this.occluderMipLayoutSize.y;
			for (int i = 0; i < this.occluderMipBounds.Length; i++)
			{
				OccluderMipBounds occluderMipBounds = this.occluderMipBounds[i];
				*(ref result.cb._OccluderMipBounds.FixedElementField + (IntPtr)(4 * i) * 4) = (uint)occluderMipBounds.offset.x;
				*(ref result.cb._OccluderMipBounds.FixedElementField + (IntPtr)(4 * i + 1) * 4) = (uint)occluderMipBounds.offset.y;
				*(ref result.cb._OccluderMipBounds.FixedElementField + (IntPtr)(4 * i + 2) * 4) = (uint)occluderMipBounds.size.x;
				*(ref result.cb._OccluderMipBounds.FixedElementField + (IntPtr)(4 * i + 3) * 4) = (uint)occluderMipBounds.size.y;
			}
			return result;
		}

		public const int k_FirstDepthMipIndex = 3;

		public const int k_MaxOccluderMips = 8;

		public const int k_MaxSilhouettePlanes = 6;

		public const int k_MaxSubviewsPerView = 6;

		public int version;

		public Vector2Int depthBufferSize;

		public NativeArray<OccluderDerivedData> subviewData;

		public int subviewValidMask;

		public NativeArray<OccluderMipBounds> occluderMipBounds;

		public Vector2Int occluderMipLayoutSize;

		public Vector2Int occluderDepthPyramidSize;

		public RTHandle occluderDepthPyramid;

		public int occlusionDebugOverlaySize;

		public GraphicsBuffer occlusionDebugOverlay;

		public bool debugNeedsClear;

		public ComputeBuffer constantBuffer;

		public NativeArray<OccluderDepthPyramidConstants> constantBufferData;

		private static class ShaderIDs
		{
			public static readonly int _SrcDepth = Shader.PropertyToID("_SrcDepth");

			public static readonly int _DstDepth = Shader.PropertyToID("_DstDepth");

			public static readonly int OccluderDepthPyramidConstants = Shader.PropertyToID("OccluderDepthPyramidConstants");
		}
	}
}
