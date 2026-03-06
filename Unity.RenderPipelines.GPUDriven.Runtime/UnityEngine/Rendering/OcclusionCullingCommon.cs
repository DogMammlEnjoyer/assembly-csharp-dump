using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering
{
	internal class OcclusionCullingCommon : IDisposable
	{
		internal void Init(GPUResidentDrawerResources resources)
		{
			this.m_DebugOcclusionTestMaterial = CoreUtils.CreateEngineMaterial(resources.debugOcclusionTestPS);
			this.m_OccluderDebugViewMaterial = CoreUtils.CreateEngineMaterial(resources.debugOccluderPS);
			this.m_OcclusionDebugCS = resources.occlusionCullingDebugKernels;
			this.m_ClearOcclusionDebugKernel = this.m_OcclusionDebugCS.FindKernel("ClearOcclusionDebug");
			this.m_OccluderDepthPyramidCS = resources.occluderDepthPyramidKernels;
			this.m_OccluderDepthDownscaleKernel = this.m_OccluderDepthPyramidCS.FindKernel("OccluderDepthDownscale");
			this.m_SilhouettePlaneCache.Init();
			this.m_ViewIDToIndexMap = new NativeParallelHashMap<int, int>(64, Allocator.Persistent);
			this.m_OccluderContextData = new List<OccluderContext>();
			this.m_OccluderContextSlots = new NativeList<OcclusionCullingCommon.OccluderContextSlot>(64, Allocator.Persistent);
			this.m_FreeOccluderContexts = new NativeList<int>(64, Allocator.Persistent);
			this.m_ProfilingSamplerUpdateOccluders = new ProfilingSampler("UpdateOccluders");
			this.m_ProfilingSamplerOcclusionTestOverlay = new ProfilingSampler("OcclusionTestOverlay");
			this.m_ProfilingSamplerOccluderOverlay = new ProfilingSampler("OccluderOverlay");
			this.m_CommonShaderVariables = new NativeArray<OcclusionCullingCommonShaderVariables>(1, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.m_CommonConstantBuffer = new ComputeBuffer(1, UnsafeUtility.SizeOf<OcclusionCullingCommonShaderVariables>(), ComputeBufferType.Constant);
			this.m_DebugShaderVariables = new NativeArray<OcclusionCullingDebugShaderVariables>(1, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.m_DebugConstantBuffer = new ComputeBuffer(1, UnsafeUtility.SizeOf<OcclusionCullingDebugShaderVariables>(), ComputeBufferType.Constant);
		}

		internal static bool UseOcclusionDebug(in OccluderContext occluderCtx)
		{
			return occluderCtx.occlusionDebugOverlaySize != 0;
		}

		internal void PrepareCulling(ComputeCommandBuffer cmd, in OccluderContext occluderCtx, in OcclusionCullingSettings settings, in InstanceOcclusionTestSubviewSettings subviewSettings, in OcclusionTestComputeShader shader, bool useOcclusionDebug)
		{
			OccluderContext.SetKeyword(cmd, shader.cs, shader.occlusionDebugKeyword, useOcclusionDebug);
			DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
			this.m_CommonShaderVariables[0] = new OcclusionCullingCommonShaderVariables(ref occluderCtx, ref subviewSettings, debugStats != null && debugStats.occlusionOverlayCountVisible, debugStats != null && debugStats.overrideOcclusionTestToAlwaysPass);
			cmd.SetBufferData<OcclusionCullingCommonShaderVariables>(this.m_CommonConstantBuffer, this.m_CommonShaderVariables);
			cmd.SetComputeConstantBufferParam(shader.cs, OcclusionCullingCommon.ShaderIDs.OcclusionCullingCommonShaderVariables, this.m_CommonConstantBuffer, 0, this.m_CommonConstantBuffer.stride);
			this.DispatchDebugClear(cmd, settings.viewInstanceID);
		}

		internal static void SetDepthPyramid(ComputeCommandBuffer cmd, in OcclusionTestComputeShader shader, int kernel, in OccluderHandles occluderHandles)
		{
			cmd.SetComputeTextureParam(shader.cs, kernel, OcclusionCullingCommon.ShaderIDs._OccluderDepthPyramid, occluderHandles.occluderDepthPyramid);
		}

		internal static void SetDebugPyramid(ComputeCommandBuffer cmd, in OcclusionTestComputeShader shader, int kernel, in OccluderHandles occluderHandles)
		{
			cmd.SetComputeBufferParam(shader.cs, kernel, OcclusionCullingCommon.ShaderIDs._OcclusionDebugOverlay, occluderHandles.occlusionDebugOverlay);
		}

		public void RenderDebugOcclusionTestOverlay(RenderGraph renderGraph, DebugDisplayGPUResidentDrawer debugSettings, int viewInstanceID, TextureHandle colorBuffer)
		{
			if (debugSettings == null)
			{
				return;
			}
			if (!debugSettings.occlusionTestOverlayEnable)
			{
				return;
			}
			OcclusionCullingDebugOutput occlusionTestDebugOutput = this.GetOcclusionTestDebugOutput(viewInstanceID);
			if (occlusionTestDebugOutput.occlusionDebugOverlay == null)
			{
				return;
			}
			OcclusionCullingCommon.OcclusionTestOverlaySetupPassData occlusionTestOverlaySetupPassData;
			using (IComputeRenderGraphBuilder computeRenderGraphBuilder = renderGraph.AddComputePass<OcclusionCullingCommon.OcclusionTestOverlaySetupPassData>("OcclusionTestOverlay", out occlusionTestOverlaySetupPassData, this.m_ProfilingSamplerOcclusionTestOverlay, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\GPUDriven\\OcclusionCullingCommon.cs", 275))
			{
				computeRenderGraphBuilder.AllowPassCulling(false);
				occlusionTestOverlaySetupPassData.cb = occlusionTestDebugOutput.cb;
				computeRenderGraphBuilder.SetRenderFunc<OcclusionCullingCommon.OcclusionTestOverlaySetupPassData>(delegate(OcclusionCullingCommon.OcclusionTestOverlaySetupPassData data, ComputeGraphContext ctx)
				{
					OcclusionCullingCommon occlusionCullingCommon = GPUResidentDrawer.instance.batcher.occlusionCullingCommon;
					occlusionCullingCommon.m_DebugShaderVariables[0] = data.cb;
					ctx.cmd.SetBufferData<OcclusionCullingDebugShaderVariables>(occlusionCullingCommon.m_DebugConstantBuffer, occlusionCullingCommon.m_DebugShaderVariables);
					occlusionCullingCommon.m_DebugOcclusionTestMaterial.SetConstantBuffer(OcclusionCullingCommon.ShaderIDs.OcclusionCullingDebugShaderVariables, occlusionCullingCommon.m_DebugConstantBuffer, 0, occlusionCullingCommon.m_DebugConstantBuffer.stride);
				});
			}
			OcclusionCullingCommon.OcclusionTestOverlayPassData occlusionTestOverlayPassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<OcclusionCullingCommon.OcclusionTestOverlayPassData>("OcclusionTestOverlay", out occlusionTestOverlayPassData, this.m_ProfilingSamplerOcclusionTestOverlay, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\GPUDriven\\OcclusionCullingCommon.cs", 297))
			{
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				occlusionTestOverlayPassData.debugPyramid = renderGraph.ImportBuffer(occlusionTestDebugOutput.occlusionDebugOverlay, false);
				rasterRenderGraphBuilder.SetRenderAttachment(colorBuffer, 0, AccessFlags.Write);
				rasterRenderGraphBuilder.UseBuffer(occlusionTestOverlayPassData.debugPyramid, AccessFlags.Read);
				rasterRenderGraphBuilder.SetRenderFunc<OcclusionCullingCommon.OcclusionTestOverlayPassData>(delegate(OcclusionCullingCommon.OcclusionTestOverlayPassData data, RasterGraphContext ctx)
				{
					ctx.cmd.SetGlobalBuffer(OcclusionCullingCommon.ShaderIDs._OcclusionDebugOverlay, data.debugPyramid);
					CoreUtils.DrawFullScreen(ctx.cmd, this.m_DebugOcclusionTestMaterial, null, 0);
				});
			}
		}

		public void RenderDebugOccluderOverlay(RenderGraph renderGraph, DebugDisplayGPUResidentDrawer debugSettings, Vector2 screenPos, float maxHeight, TextureHandle colorBuffer)
		{
			if (debugSettings == null)
			{
				return;
			}
			if (!debugSettings.occluderDebugViewEnable)
			{
				return;
			}
			int viewInstanceID;
			if (!debugSettings.GetOccluderViewInstanceID(out viewInstanceID))
			{
				return;
			}
			RTHandle occluderDepthPyramid = this.GetOcclusionTestDebugOutput(viewInstanceID).occluderDepthPyramid;
			if (occluderDepthPyramid == null)
			{
				return;
			}
			Material occluderDebugViewMaterial = this.m_OccluderDebugViewMaterial;
			int passIndex = occluderDebugViewMaterial.FindPass("DebugOccluder");
			Vector2 vector = occluderDepthPyramid.referenceSize;
			float d = maxHeight / vector.y;
			vector *= d;
			Rect viewport = new Rect(screenPos.x, screenPos.y, vector.x, vector.y);
			OcclusionCullingCommon.OccluderOverlayPassData occluderOverlayPassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<OcclusionCullingCommon.OccluderOverlayPassData>("OccluderOverlay", out occluderOverlayPassData, this.m_ProfilingSamplerOccluderOverlay, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\GPUDriven\\OcclusionCullingCommon.cs", 353))
			{
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderAttachment(colorBuffer, 0, AccessFlags.Write);
				occluderOverlayPassData.debugMaterial = occluderDebugViewMaterial;
				occluderOverlayPassData.occluderTexture = occluderDepthPyramid;
				occluderOverlayPassData.viewport = viewport;
				occluderOverlayPassData.passIndex = passIndex;
				occluderOverlayPassData.validRange = debugSettings.occluderDebugViewRange;
				rasterRenderGraphBuilder.SetRenderFunc<OcclusionCullingCommon.OccluderOverlayPassData>(delegate(OcclusionCullingCommon.OccluderOverlayPassData data, RasterGraphContext ctx)
				{
					MaterialPropertyBlock tempMaterialPropertyBlock = ctx.renderGraphPool.GetTempMaterialPropertyBlock();
					tempMaterialPropertyBlock.SetTexture("_OccluderTexture", data.occluderTexture);
					tempMaterialPropertyBlock.SetVector("_ValidRange", data.validRange);
					ctx.cmd.SetViewport(data.viewport);
					ctx.cmd.DrawProcedural(Matrix4x4.identity, data.debugMaterial, data.passIndex, MeshTopology.Triangles, 3, 1, tempMaterialPropertyBlock);
				});
			}
		}

		private void DispatchDebugClear(ComputeCommandBuffer cmd, int viewInstanceID)
		{
			int index;
			if (!this.m_ViewIDToIndexMap.TryGetValue(viewInstanceID, out index))
			{
				return;
			}
			OccluderContext occluderContext = this.m_OccluderContextData[index];
			if (OcclusionCullingCommon.UseOcclusionDebug(occluderContext) && occluderContext.debugNeedsClear)
			{
				ComputeShader occlusionDebugCS = this.m_OcclusionDebugCS;
				int clearOcclusionDebugKernel = this.m_ClearOcclusionDebugKernel;
				cmd.SetComputeConstantBufferParam(occlusionDebugCS, OcclusionCullingCommon.ShaderIDs.OcclusionCullingCommonShaderVariables, this.m_CommonConstantBuffer, 0, this.m_CommonConstantBuffer.stride);
				cmd.SetComputeBufferParam(occlusionDebugCS, clearOcclusionDebugKernel, OcclusionCullingCommon.ShaderIDs._OcclusionDebugOverlay, occluderContext.occlusionDebugOverlay);
				Vector2Int size = occluderContext.occluderMipBounds[0].size;
				cmd.DispatchCompute(occlusionDebugCS, clearOcclusionDebugKernel, (size.x + 7) / 8, (size.y + 7) / 8, occluderContext.subviewCount);
				occluderContext.debugNeedsClear = false;
				this.m_OccluderContextData[index] = occluderContext;
			}
		}

		private OccluderHandles PrepareOccluders(RenderGraph renderGraph, in OccluderParameters occluderParams)
		{
			OccluderHandles result = default(OccluderHandles);
			TextureHandle depthTexture = occluderParams.depthTexture;
			if (depthTexture.IsValid())
			{
				int index;
				if (!this.m_ViewIDToIndexMap.TryGetValue(occluderParams.viewInstanceID, out index))
				{
					index = this.NewContext(occluderParams.viewInstanceID);
				}
				OccluderContext value = this.m_OccluderContextData[index];
				value.PrepareOccluders(occluderParams);
				result = value.Import(renderGraph);
				this.m_OccluderContextData[index] = value;
			}
			else
			{
				this.DeleteContext(occluderParams.viewInstanceID);
			}
			return result;
		}

		private void CreateFarDepthPyramid(ComputeCommandBuffer cmd, in OccluderParameters occluderParams, ReadOnlySpan<OccluderSubviewUpdate> occluderSubviewUpdates, in OccluderHandles occluderHandles)
		{
			int index;
			if (!this.m_ViewIDToIndexMap.TryGetValue(occluderParams.viewInstanceID, out index))
			{
				return;
			}
			NativeArray<Plane> subArray = this.m_SilhouettePlaneCache.GetSubArray(occluderParams.viewInstanceID);
			OccluderContext value = this.m_OccluderContextData[index];
			value.CreateFarDepthPyramid(cmd, occluderParams, occluderSubviewUpdates, occluderHandles, subArray, this.m_OccluderDepthPyramidCS, this.m_OccluderDepthDownscaleKernel);
			value.version++;
			this.m_OccluderContextData[index] = value;
			OcclusionCullingCommon.OccluderContextSlot value2 = this.m_OccluderContextSlots[index];
			value2.lastUsedFrameIndex = this.m_FrameIndex;
			this.m_OccluderContextSlots[index] = value2;
		}

		public unsafe bool UpdateInstanceOccluders(RenderGraph renderGraph, in OccluderParameters occluderParams, ReadOnlySpan<OccluderSubviewUpdate> occluderSubviewUpdates)
		{
			OccluderHandles occluderHandles = this.PrepareOccluders(renderGraph, occluderParams);
			if (!occluderHandles.occluderDepthPyramid.IsValid())
			{
				return false;
			}
			OcclusionCullingCommon.UpdateOccludersPassData updateOccludersPassData;
			using (IComputeRenderGraphBuilder computeRenderGraphBuilder = renderGraph.AddComputePass<OcclusionCullingCommon.UpdateOccludersPassData>("Update Occluders", out updateOccludersPassData, this.m_ProfilingSamplerUpdateOccluders, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\GPUDriven\\OcclusionCullingCommon.cs", 454))
			{
				computeRenderGraphBuilder.AllowGlobalStateModification(true);
				updateOccludersPassData.occluderParams = occluderParams;
				if (updateOccludersPassData.occluderSubviewUpdates == null)
				{
					updateOccludersPassData.occluderSubviewUpdates = new List<OccluderSubviewUpdate>();
				}
				else
				{
					updateOccludersPassData.occluderSubviewUpdates.Clear();
				}
				for (int i = 0; i < occluderSubviewUpdates.Length; i++)
				{
					updateOccludersPassData.occluderSubviewUpdates.Add(*occluderSubviewUpdates[i]);
				}
				updateOccludersPassData.occluderHandles = occluderHandles;
				computeRenderGraphBuilder.UseTexture(updateOccludersPassData.occluderParams.depthTexture, AccessFlags.Read);
				updateOccludersPassData.occluderHandles.UseForOccluderUpdate(computeRenderGraphBuilder);
				computeRenderGraphBuilder.SetRenderFunc<OcclusionCullingCommon.UpdateOccludersPassData>(delegate(OcclusionCullingCommon.UpdateOccludersPassData data, ComputeGraphContext context)
				{
					int count = data.occluderSubviewUpdates.Count;
					Span<OccluderSubviewUpdate> span = new Span<OccluderSubviewUpdate>(stackalloc byte[checked(unchecked((UIntPtr)count) * (UIntPtr)sizeof(OccluderSubviewUpdate))], count);
					int num = 0;
					for (int j = 0; j < data.occluderSubviewUpdates.Count; j++)
					{
						*span[j] = data.occluderSubviewUpdates[j];
						num |= 1 << data.occluderSubviewUpdates[j].subviewIndex;
					}
					GPUResidentBatcher batcher = GPUResidentDrawer.instance.batcher;
					batcher.occlusionCullingCommon.CreateFarDepthPyramid(context.cmd, data.occluderParams, span, data.occluderHandles);
					batcher.instanceCullingBatcher.InstanceOccludersUpdated(data.occluderParams.viewInstanceID, num);
				});
			}
			return true;
		}

		internal void UpdateSilhouettePlanes(int viewInstanceID, NativeArray<Plane> planes)
		{
			this.m_SilhouettePlaneCache.Update(viewInstanceID, planes, this.m_FrameIndex);
		}

		internal OcclusionCullingDebugOutput GetOcclusionTestDebugOutput(int viewInstanceID)
		{
			int index;
			if (this.m_ViewIDToIndexMap.TryGetValue(viewInstanceID, out index) && this.m_OccluderContextSlots[index].valid)
			{
				return this.m_OccluderContextData[index].GetDebugOutput();
			}
			return default(OcclusionCullingDebugOutput);
		}

		public unsafe void UpdateOccluderStats(DebugRendererBatcherStats debugStats)
		{
			debugStats.occluderStats.Clear();
			foreach (KeyValue<int, int> keyValue in this.m_ViewIDToIndexMap)
			{
				if (*keyValue.Value < this.m_OccluderContextSlots.Length && this.m_OccluderContextSlots[*keyValue.Value].valid)
				{
					DebugOccluderStats debugOccluderStats = default(DebugOccluderStats);
					debugOccluderStats.viewInstanceID = keyValue.Key;
					debugOccluderStats.subviewCount = this.m_OccluderContextData[*keyValue.Value].subviewCount;
					debugOccluderStats.occluderMipLayoutSize = this.m_OccluderContextData[*keyValue.Value].occluderMipLayoutSize;
					debugStats.occluderStats.Add(debugOccluderStats);
				}
			}
		}

		internal bool HasOccluderContext(int viewInstanceID)
		{
			return this.m_ViewIDToIndexMap.ContainsKey(viewInstanceID);
		}

		internal bool GetOccluderContext(int viewInstanceID, out OccluderContext occluderContext)
		{
			int index;
			if (this.m_ViewIDToIndexMap.TryGetValue(viewInstanceID, out index) && this.m_OccluderContextSlots[index].valid)
			{
				occluderContext = this.m_OccluderContextData[index];
				return true;
			}
			occluderContext = default(OccluderContext);
			return false;
		}

		internal void UpdateFrame()
		{
			for (int i = 0; i < this.m_OccluderContextData.Count; i++)
			{
				if (this.m_OccluderContextSlots[i].valid)
				{
					OccluderContext value = this.m_OccluderContextData[i];
					OcclusionCullingCommon.OccluderContextSlot occluderContextSlot = this.m_OccluderContextSlots[i];
					if (this.m_FrameIndex - occluderContextSlot.lastUsedFrameIndex >= OcclusionCullingCommon.s_MaxContextGCFrame)
					{
						this.DeleteContext(occluderContextSlot.viewInstanceID);
					}
					else
					{
						value.debugNeedsClear = true;
						this.m_OccluderContextData[i] = value;
					}
				}
			}
			this.m_SilhouettePlaneCache.FreeUnusedSlots(this.m_FrameIndex, OcclusionCullingCommon.s_MaxContextGCFrame);
			this.m_FrameIndex++;
		}

		private int NewContext(int viewInstanceID)
		{
			OcclusionCullingCommon.OccluderContextSlot value = new OcclusionCullingCommon.OccluderContextSlot
			{
				valid = true,
				viewInstanceID = viewInstanceID,
				lastUsedFrameIndex = this.m_FrameIndex
			};
			OccluderContext occluderContext = default(OccluderContext);
			int num;
			if (this.m_FreeOccluderContexts.Length > 0)
			{
				num = this.m_FreeOccluderContexts[this.m_FreeOccluderContexts.Length - 1];
				this.m_FreeOccluderContexts.RemoveAt(this.m_FreeOccluderContexts.Length - 1);
				this.m_OccluderContextData[num] = occluderContext;
				this.m_OccluderContextSlots[num] = value;
			}
			else
			{
				num = this.m_OccluderContextData.Count;
				this.m_OccluderContextData.Add(occluderContext);
				this.m_OccluderContextSlots.Add(value);
			}
			this.m_ViewIDToIndexMap.Add(viewInstanceID, num);
			return num;
		}

		private void DeleteContext(int viewInstanceID)
		{
			int index;
			if (!this.m_ViewIDToIndexMap.TryGetValue(viewInstanceID, out index) || !this.m_OccluderContextSlots[index].valid)
			{
				return;
			}
			this.m_OccluderContextData[index].Dispose();
			this.m_OccluderContextSlots[index] = new OcclusionCullingCommon.OccluderContextSlot
			{
				valid = false
			};
			this.m_FreeOccluderContexts.Add(index);
			this.m_ViewIDToIndexMap.Remove(viewInstanceID);
		}

		public void Dispose()
		{
			CoreUtils.Destroy(this.m_DebugOcclusionTestMaterial);
			CoreUtils.Destroy(this.m_OccluderDebugViewMaterial);
			for (int i = 0; i < this.m_OccluderContextData.Count; i++)
			{
				if (this.m_OccluderContextSlots[i].valid)
				{
					this.m_OccluderContextData[i].Dispose();
				}
			}
			this.m_SilhouettePlaneCache.Dispose();
			this.m_ViewIDToIndexMap.Dispose();
			this.m_FreeOccluderContexts.Dispose();
			this.m_OccluderContextData.Clear();
			this.m_OccluderContextSlots.Dispose();
			this.m_CommonShaderVariables.Dispose();
			this.m_CommonConstantBuffer.Release();
			this.m_DebugShaderVariables.Dispose();
			this.m_DebugConstantBuffer.Release();
		}

		private static readonly int s_MaxContextGCFrame = 8;

		private Material m_DebugOcclusionTestMaterial;

		private Material m_OccluderDebugViewMaterial;

		private ComputeShader m_OcclusionDebugCS;

		private int m_ClearOcclusionDebugKernel;

		private ComputeShader m_OccluderDepthPyramidCS;

		private int m_OccluderDepthDownscaleKernel;

		private int m_FrameIndex;

		private SilhouettePlaneCache m_SilhouettePlaneCache;

		private NativeParallelHashMap<int, int> m_ViewIDToIndexMap;

		private List<OccluderContext> m_OccluderContextData;

		private NativeList<OcclusionCullingCommon.OccluderContextSlot> m_OccluderContextSlots;

		private NativeList<int> m_FreeOccluderContexts;

		private NativeArray<OcclusionCullingCommonShaderVariables> m_CommonShaderVariables;

		private ComputeBuffer m_CommonConstantBuffer;

		private NativeArray<OcclusionCullingDebugShaderVariables> m_DebugShaderVariables;

		private ComputeBuffer m_DebugConstantBuffer;

		private ProfilingSampler m_ProfilingSamplerUpdateOccluders;

		private ProfilingSampler m_ProfilingSamplerOcclusionTestOverlay;

		private ProfilingSampler m_ProfilingSamplerOccluderOverlay;

		private struct OccluderContextSlot
		{
			public bool valid;

			public int lastUsedFrameIndex;

			public int viewInstanceID;
		}

		private static class ShaderIDs
		{
			public static readonly int OcclusionCullingCommonShaderVariables = Shader.PropertyToID("OcclusionCullingCommonShaderVariables");

			public static readonly int _OccluderDepthPyramid = Shader.PropertyToID("_OccluderDepthPyramid");

			public static readonly int _OcclusionDebugOverlay = Shader.PropertyToID("_OcclusionDebugOverlay");

			public static readonly int OcclusionCullingDebugShaderVariables = Shader.PropertyToID("OcclusionCullingDebugShaderVariables");
		}

		private class OcclusionTestOverlaySetupPassData
		{
			public OcclusionCullingDebugShaderVariables cb;
		}

		private class OcclusionTestOverlayPassData
		{
			public BufferHandle debugPyramid;
		}

		private struct DebugOccluderViewData
		{
			public int passIndex;

			public Rect viewport;

			public bool valid;
		}

		private class OccluderOverlayPassData
		{
			public Material debugMaterial;

			public RTHandle occluderTexture;

			public Rect viewport;

			public int passIndex;

			public Vector2 validRange;
		}

		private class UpdateOccludersPassData
		{
			public OccluderParameters occluderParams;

			public List<OccluderSubviewUpdate> occluderSubviewUpdates;

			public OccluderHandles occluderHandles;
		}
	}
}
