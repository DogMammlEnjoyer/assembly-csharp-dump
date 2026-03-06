using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
	internal struct ReflectionProbeManager : IDisposable
	{
		public RenderTexture atlasRT
		{
			get
			{
				return this.m_AtlasTexture0;
			}
		}

		public RTHandle atlasRTHandle
		{
			get
			{
				return this.m_AtlasTexture0Handle;
			}
		}

		public static ReflectionProbeManager Create()
		{
			ReflectionProbeManager result = default(ReflectionProbeManager);
			result.Init();
			return result;
		}

		private void Init()
		{
			int maxVisibleReflectionProbes = UniversalRenderPipeline.maxVisibleReflectionProbes;
			this.m_Resolution = 1;
			GraphicsFormat graphicsFormat = GraphicsFormat.B10G11R11_UFloatPack32;
			if (!SystemInfo.IsFormatSupported(graphicsFormat, GraphicsFormatUsage.Render))
			{
				graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;
			}
			this.m_AtlasTexture0 = new RenderTexture(new RenderTextureDescriptor
			{
				width = this.m_Resolution.x,
				height = this.m_Resolution.y,
				volumeDepth = 1,
				dimension = TextureDimension.Tex2D,
				graphicsFormat = graphicsFormat,
				useMipMap = false,
				msaaSamples = 1
			});
			this.m_AtlasTexture0.name = "URP Reflection Probe Atlas";
			this.m_AtlasTexture0.filterMode = FilterMode.Bilinear;
			this.m_AtlasTexture0.hideFlags = HideFlags.HideAndDontSave;
			this.m_AtlasTexture0.Create();
			this.m_AtlasTexture0Handle = RTHandles.Alloc(this.m_AtlasTexture0, true);
			this.m_AtlasTexture1 = new RenderTexture(this.m_AtlasTexture0.descriptor);
			this.m_AtlasTexture1.name = "URP Reflection Probe Atlas";
			this.m_AtlasTexture1.filterMode = FilterMode.Bilinear;
			this.m_AtlasTexture1.hideFlags = HideFlags.HideAndDontSave;
			this.m_AtlasAllocator = new BuddyAllocator(math.floorlog2(SystemInfo.maxTextureSize) - 2, 2, Allocator.Persistent);
			this.m_Cache = new Dictionary<int, ReflectionProbeManager.CachedProbe>(maxVisibleReflectionProbes);
			this.m_WarningCache = new Dictionary<int, int>(maxVisibleReflectionProbes);
			this.m_NeedsUpdate = new List<int>(maxVisibleReflectionProbes);
			this.m_NeedsRemove = new List<int>(maxVisibleReflectionProbes);
			this.m_BoxMax = new Vector4[maxVisibleReflectionProbes];
			this.m_BoxMin = new Vector4[maxVisibleReflectionProbes];
			this.m_ProbePosition = new Vector4[maxVisibleReflectionProbes];
			this.m_MipScaleOffset = new Vector4[maxVisibleReflectionProbes * 7];
		}

		public unsafe void UpdateGpuData(CommandBuffer cmd, ref CullingResults cullResults)
		{
			NativeArray<VisibleReflectionProbe> visibleReflectionProbes = cullResults.visibleReflectionProbes;
			int num = math.min(visibleReflectionProbes.Length, UniversalRenderPipeline.maxVisibleReflectionProbes);
			int renderedFrameCount = Time.renderedFrameCount;
			foreach (KeyValuePair<int, ReflectionProbeManager.CachedProbe> keyValuePair in this.m_Cache)
			{
				int num2;
				ReflectionProbeManager.CachedProbe cachedProbe;
				keyValuePair.Deconstruct(out num2, out cachedProbe);
				int item = num2;
				ReflectionProbeManager.CachedProbe cachedProbe2 = cachedProbe;
				if (Math.Abs(cachedProbe2.lastUsed - renderedFrameCount) > 1 || !cachedProbe2.texture || cachedProbe2.size != cachedProbe2.texture.width)
				{
					this.m_NeedsRemove.Add(item);
					for (int i = 0; i < 7; i++)
					{
						if (*(ref cachedProbe2.dataIndices.FixedElementField + (IntPtr)i * 4) != -1)
						{
							this.m_AtlasAllocator.Free(new BuddyAllocation(*(ref cachedProbe2.levels.FixedElementField + (IntPtr)i * 4), *(ref cachedProbe2.dataIndices.FixedElementField + (IntPtr)i * 4)));
						}
					}
				}
			}
			foreach (int key in this.m_NeedsRemove)
			{
				this.m_Cache.Remove(key);
			}
			this.m_NeedsRemove.Clear();
			foreach (KeyValuePair<int, int> keyValuePair2 in this.m_WarningCache)
			{
				int num2;
				int num3;
				keyValuePair2.Deconstruct(out num2, out num3);
				int item2 = num2;
				if (Math.Abs(num3 - renderedFrameCount) > 1)
				{
					this.m_NeedsRemove.Add(item2);
				}
			}
			foreach (int key2 in this.m_NeedsRemove)
			{
				this.m_WarningCache.Remove(key2);
			}
			this.m_NeedsRemove.Clear();
			bool flag = false;
			int2 @int = math.int2(0, 0);
			for (int j = 0; j < num; j++)
			{
				VisibleReflectionProbe visibleReflectionProbe = visibleReflectionProbes[j];
				Texture texture = visibleReflectionProbe.texture;
				int instanceID = visibleReflectionProbe.reflectionProbe.GetInstanceID();
				ReflectionProbeManager.CachedProbe cachedProbe3;
				bool flag2 = this.m_Cache.TryGetValue(instanceID, out cachedProbe3);
				if (texture)
				{
					if (!flag2)
					{
						cachedProbe3.size = texture.width;
						int num4 = math.ceillog2(cachedProbe3.size * 4) + 1;
						int num5 = this.m_AtlasAllocator.levelCount + 2 - num4;
						cachedProbe3.mipCount = math.min(num4, 7);
						cachedProbe3.texture = texture;
						int k;
						for (k = 0; k < cachedProbe3.mipCount; k++)
						{
							int num6 = math.min(num5 + k, this.m_AtlasAllocator.levelCount - 1);
							BuddyAllocation buddyAllocation;
							if (!this.m_AtlasAllocator.TryAllocate(num6, out buddyAllocation))
							{
								break;
							}
							*(ref cachedProbe3.levels.FixedElementField + (IntPtr)k * 4) = buddyAllocation.level;
							*(ref cachedProbe3.dataIndices.FixedElementField + (IntPtr)k * 4) = buddyAllocation.index;
							int4 int2 = (int4)(this.GetScaleOffset(num6, buddyAllocation.index, true, false) * this.m_Resolution.xyxy);
							@int = math.max(@int, int2.zw + int2.xy);
						}
						if (k < cachedProbe3.mipCount)
						{
							if (!this.m_WarningCache.ContainsKey(instanceID))
							{
								flag = true;
							}
							this.m_WarningCache[instanceID] = renderedFrameCount;
							for (int l = 0; l < k; l++)
							{
								this.m_AtlasAllocator.Free(new BuddyAllocation(*(ref cachedProbe3.levels.FixedElementField + (IntPtr)l * 4), *(ref cachedProbe3.dataIndices.FixedElementField + (IntPtr)l * 4)));
							}
							for (int m = 0; m < 7; m++)
							{
								*(ref cachedProbe3.dataIndices.FixedElementField + (IntPtr)m * 4) = -1;
							}
							goto IL_4B0;
						}
						while (k < 7)
						{
							*(ref cachedProbe3.dataIndices.FixedElementField + (IntPtr)k * 4) = -1;
							k++;
						}
					}
					if ((!flag2 || cachedProbe3.updateCount != texture.updateCount) | cachedProbe3.hdrData != visibleReflectionProbe.hdrData)
					{
						cachedProbe3.updateCount = texture.updateCount;
						this.m_NeedsUpdate.Add(instanceID);
					}
					if (visibleReflectionProbe.reflectionProbe.mode == ReflectionProbeMode.Realtime && visibleReflectionProbe.reflectionProbe.refreshMode == ReflectionProbeRefreshMode.EveryFrame)
					{
						cachedProbe3.lastUsed = -1;
					}
					else
					{
						cachedProbe3.lastUsed = renderedFrameCount;
					}
					cachedProbe3.hdrData = visibleReflectionProbe.hdrData;
					this.m_Cache[instanceID] = cachedProbe3;
				}
				IL_4B0:;
			}
			if (math.any(this.m_Resolution < @int))
			{
				@int = math.max(this.m_Resolution, math.ceilpow2(@int));
				RenderTextureDescriptor descriptor = this.m_AtlasTexture0.descriptor;
				descriptor.width = @int.x;
				descriptor.height = @int.y;
				this.m_AtlasTexture1.width = @int.x;
				this.m_AtlasTexture1.height = @int.y;
				this.m_AtlasTexture1.Create();
				if (this.m_AtlasTexture0.width != 1)
				{
					if (SystemInfo.copyTextureSupport != CopyTextureSupport.None)
					{
						Graphics.CopyTexture(this.m_AtlasTexture0, 0, 0, 0, 0, this.m_Resolution.x, this.m_Resolution.y, this.m_AtlasTexture1, 0, 0, 0, 0);
					}
					else
					{
						Graphics.Blit(this.m_AtlasTexture0, this.m_AtlasTexture1, this.m_Resolution / @int, Vector2.zero);
					}
				}
				this.m_AtlasTexture0.Release();
				RenderTexture atlasTexture = this.m_AtlasTexture1;
				RenderTexture atlasTexture2 = this.m_AtlasTexture0;
				this.m_AtlasTexture0 = atlasTexture;
				this.m_AtlasTexture1 = atlasTexture2;
				this.m_Resolution = @int;
			}
			int num7 = 0;
			for (int n = 0; n < num; n++)
			{
				VisibleReflectionProbe visibleReflectionProbe2 = visibleReflectionProbes[n];
				int instanceID2 = visibleReflectionProbe2.reflectionProbe.GetInstanceID();
				int num8 = n - num7;
				ReflectionProbeManager.CachedProbe cachedProbe4;
				if (!this.m_Cache.TryGetValue(instanceID2, out cachedProbe4) || !visibleReflectionProbe2.texture)
				{
					num7++;
				}
				else
				{
					this.m_BoxMax[num8] = new Vector4(visibleReflectionProbe2.bounds.max.x, visibleReflectionProbe2.bounds.max.y, visibleReflectionProbe2.bounds.max.z, visibleReflectionProbe2.blendDistance);
					this.m_BoxMin[num8] = new Vector4(visibleReflectionProbe2.bounds.min.x, visibleReflectionProbe2.bounds.min.y, visibleReflectionProbe2.bounds.min.z, (float)visibleReflectionProbe2.importance);
					this.m_ProbePosition[num8] = new Vector4(visibleReflectionProbe2.localToWorldMatrix.m03, visibleReflectionProbe2.localToWorldMatrix.m13, visibleReflectionProbe2.localToWorldMatrix.m23, (float)((visibleReflectionProbe2.isBoxProjection ? 1 : -1) * cachedProbe4.mipCount));
					for (int num9 = 0; num9 < cachedProbe4.mipCount; num9++)
					{
						this.m_MipScaleOffset[num8 * 7 + num9] = this.GetScaleOffset(*(ref cachedProbe4.levels.FixedElementField + (IntPtr)num9 * 4), *(ref cachedProbe4.dataIndices.FixedElementField + (IntPtr)num9 * 4), false, false);
					}
				}
			}
			if (flag)
			{
				Debug.LogWarning("A number of reflection probes have been skipped due to the reflection probe atlas being full.\nTo fix this, you can decrease the number or resolution of probes.");
			}
			using (new ProfilingScope(cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.UpdateReflectionProbeAtlas)))
			{
				cmd.SetRenderTarget(this.m_AtlasTexture0);
				foreach (int key3 in this.m_NeedsUpdate)
				{
					ReflectionProbeManager.CachedProbe cachedProbe5 = this.m_Cache[key3];
					for (int num10 = 0; num10 < cachedProbe5.mipCount; num10++)
					{
						int num11 = *(ref cachedProbe5.levels.FixedElementField + (IntPtr)num10 * 4);
						int dataIndex = *(ref cachedProbe5.dataIndices.FixedElementField + (IntPtr)num10 * 4);
						float4 scaleOffset = this.GetScaleOffset(num11, dataIndex, true, !SystemInfo.graphicsUVStartsAtTop);
						int num12 = (1 << this.m_AtlasAllocator.levelCount + 1 - num11) - 2;
						Blitter.BlitCubeToOctahedral2DQuadWithPadding(cmd, cachedProbe5.texture, new Vector2((float)num12, (float)num12), scaleOffset, num10, true, 2, new Vector4?(cachedProbe5.hdrData));
					}
				}
				cmd.SetGlobalVectorArray(ReflectionProbeManager.ShaderProperties.BoxMin, this.m_BoxMin);
				cmd.SetGlobalVectorArray(ReflectionProbeManager.ShaderProperties.BoxMax, this.m_BoxMax);
				cmd.SetGlobalVectorArray(ReflectionProbeManager.ShaderProperties.ProbePosition, this.m_ProbePosition);
				cmd.SetGlobalVectorArray(ReflectionProbeManager.ShaderProperties.MipScaleOffset, this.m_MipScaleOffset);
				cmd.SetGlobalFloat(ReflectionProbeManager.ShaderProperties.Count, (float)(num - num7));
				cmd.SetGlobalTexture(ReflectionProbeManager.ShaderProperties.Atlas, this.m_AtlasTexture0);
			}
			this.m_NeedsUpdate.Clear();
		}

		private float4 GetScaleOffset(int level, int dataIndex, bool includePadding, bool yflip)
		{
			int num = 1 << this.m_AtlasAllocator.levelCount + 1 - level;
			uint2 v = SpaceFillingCurves.DecodeMorton2D((uint)dataIndex);
			float2 @float = (float)(num - (includePadding ? 0 : 2)) / this.m_Resolution;
			float2 float2 = (v * (float)num + (float)(includePadding ? 0 : 1)) / this.m_Resolution;
			if (yflip)
			{
				float2.y = 1f - float2.y - @float.y;
			}
			return math.float4(@float, float2);
		}

		public void Dispose()
		{
			if (this.m_AtlasTexture0)
			{
				this.m_AtlasTexture0.Release();
				this.m_AtlasTexture0Handle.Release();
			}
			this.m_AtlasAllocator.Dispose();
			Object.DestroyImmediate(this.m_AtlasTexture0);
			Object.DestroyImmediate(this.m_AtlasTexture1);
			this = default(ReflectionProbeManager);
		}

		private int2 m_Resolution;

		private RenderTexture m_AtlasTexture0;

		private RenderTexture m_AtlasTexture1;

		private RTHandle m_AtlasTexture0Handle;

		private BuddyAllocator m_AtlasAllocator;

		private Dictionary<int, ReflectionProbeManager.CachedProbe> m_Cache;

		private Dictionary<int, int> m_WarningCache;

		private List<int> m_NeedsUpdate;

		private List<int> m_NeedsRemove;

		private Vector4[] m_BoxMax;

		private Vector4[] m_BoxMin;

		private Vector4[] m_ProbePosition;

		private Vector4[] m_MipScaleOffset;

		private const int k_MaxMipCount = 7;

		private const string k_ReflectionProbeAtlasName = "URP Reflection Probe Atlas";

		private struct CachedProbe
		{
			public uint updateCount;

			public Hash128 imageContentsHash;

			public int size;

			public int mipCount;

			[FixedBuffer(typeof(int), 7)]
			public ReflectionProbeManager.CachedProbe.<dataIndices>e__FixedBuffer dataIndices;

			[FixedBuffer(typeof(int), 7)]
			public ReflectionProbeManager.CachedProbe.<levels>e__FixedBuffer levels;

			public Texture texture;

			public int lastUsed;

			public Vector4 hdrData;

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 28)]
			public struct <dataIndices>e__FixedBuffer
			{
				public int FixedElementField;
			}

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 28)]
			public struct <levels>e__FixedBuffer
			{
				public int FixedElementField;
			}
		}

		private static class ShaderProperties
		{
			public static readonly int BoxMin = Shader.PropertyToID("urp_ReflProbes_BoxMin");

			public static readonly int BoxMax = Shader.PropertyToID("urp_ReflProbes_BoxMax");

			public static readonly int ProbePosition = Shader.PropertyToID("urp_ReflProbes_ProbePosition");

			public static readonly int MipScaleOffset = Shader.PropertyToID("urp_ReflProbes_MipScaleOffset");

			public static readonly int Count = Shader.PropertyToID("urp_ReflProbes_Count");

			public static readonly int Atlas = Shader.PropertyToID("urp_ReflProbes_Atlas");
		}
	}
}
