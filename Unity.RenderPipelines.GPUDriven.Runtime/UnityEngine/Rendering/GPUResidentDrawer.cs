using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.SceneManagement;

namespace UnityEngine.Rendering
{
	public class GPUResidentDrawer
	{
		internal static GPUResidentDrawer instance
		{
			get
			{
				return GPUResidentDrawer.s_Instance;
			}
		}

		public static bool IsInstanceOcclusionCullingEnabled()
		{
			return GPUResidentDrawer.s_Instance != null && GPUResidentDrawer.s_Instance.settings.mode == GPUResidentDrawerMode.InstancedDrawing && GPUResidentDrawer.s_Instance.settings.enableOcclusionCulling;
		}

		public static void PostCullBeginCameraRendering(RenderRequestBatcherContext context)
		{
			GPUResidentDrawer gpuresidentDrawer = GPUResidentDrawer.s_Instance;
			if (gpuresidentDrawer == null)
			{
				return;
			}
			gpuresidentDrawer.batcher.PostCullBeginCameraRendering(context);
		}

		public static void OnSetupAmbientProbe()
		{
			GPUResidentDrawer gpuresidentDrawer = GPUResidentDrawer.s_Instance;
			if (gpuresidentDrawer == null)
			{
				return;
			}
			gpuresidentDrawer.batcher.OnSetupAmbientProbe();
		}

		public static void InstanceOcclusionTest(RenderGraph renderGraph, in OcclusionCullingSettings settings, ReadOnlySpan<SubviewOcclusionTest> subviewOcclusionTests)
		{
			GPUResidentDrawer gpuresidentDrawer = GPUResidentDrawer.s_Instance;
			if (gpuresidentDrawer == null)
			{
				return;
			}
			gpuresidentDrawer.batcher.InstanceOcclusionTest(renderGraph, settings, subviewOcclusionTests);
		}

		public static void UpdateInstanceOccluders(RenderGraph renderGraph, in OccluderParameters occluderParameters, ReadOnlySpan<OccluderSubviewUpdate> occluderSubviewUpdates)
		{
			GPUResidentDrawer gpuresidentDrawer = GPUResidentDrawer.s_Instance;
			if (gpuresidentDrawer == null)
			{
				return;
			}
			gpuresidentDrawer.batcher.UpdateInstanceOccluders(renderGraph, occluderParameters, occluderSubviewUpdates);
		}

		public static void ReinitializeIfNeeded()
		{
		}

		public static void RenderDebugOcclusionTestOverlay(RenderGraph renderGraph, DebugDisplayGPUResidentDrawer debugSettings, int viewInstanceID, TextureHandle colorBuffer)
		{
			GPUResidentDrawer gpuresidentDrawer = GPUResidentDrawer.s_Instance;
			if (gpuresidentDrawer == null)
			{
				return;
			}
			gpuresidentDrawer.batcher.occlusionCullingCommon.RenderDebugOcclusionTestOverlay(renderGraph, debugSettings, viewInstanceID, colorBuffer);
		}

		public static void RenderDebugOccluderOverlay(RenderGraph renderGraph, DebugDisplayGPUResidentDrawer debugSettings, Vector2 screenPos, float maxHeight, TextureHandle colorBuffer)
		{
			GPUResidentDrawer gpuresidentDrawer = GPUResidentDrawer.s_Instance;
			if (gpuresidentDrawer == null)
			{
				return;
			}
			gpuresidentDrawer.batcher.occlusionCullingCommon.RenderDebugOccluderOverlay(renderGraph, debugSettings, screenPos, maxHeight, colorBuffer);
		}

		internal static DebugRendererBatcherStats GetDebugStats()
		{
			GPUResidentDrawer gpuresidentDrawer = GPUResidentDrawer.s_Instance;
			if (gpuresidentDrawer == null)
			{
				return null;
			}
			return gpuresidentDrawer.m_BatchersContext.debugStats;
		}

		private void InsertIntoPlayerLoop()
		{
			PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
			bool flag = false;
			for (int i = 0; i < currentPlayerLoop.subSystemList.Length; i++)
			{
				PlayerLoopSystem playerLoopSystem = currentPlayerLoop.subSystemList[i];
				if (!flag && playerLoopSystem.type == typeof(PostLateUpdate))
				{
					List<PlayerLoopSystem> list = new List<PlayerLoopSystem>();
					foreach (PlayerLoopSystem playerLoopSystem2 in playerLoopSystem.subSystemList)
					{
						if (playerLoopSystem2.type == typeof(PostLateUpdate.FinishFrameRendering))
						{
							PlayerLoopSystem item = default(PlayerLoopSystem);
							item.updateDelegate = (PlayerLoopSystem.UpdateFunction)Delegate.Combine(item.updateDelegate, new PlayerLoopSystem.UpdateFunction(GPUResidentDrawer.PostPostLateUpdateStatic));
							item.type = base.GetType();
							list.Add(item);
							flag = true;
						}
						list.Add(playerLoopSystem2);
					}
					playerLoopSystem.subSystemList = list.ToArray();
					currentPlayerLoop.subSystemList[i] = playerLoopSystem;
				}
			}
			PlayerLoop.SetPlayerLoop(currentPlayerLoop);
		}

		private void RemoveFromPlayerLoop()
		{
			PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
			for (int i = 0; i < currentPlayerLoop.subSystemList.Length; i++)
			{
				PlayerLoopSystem playerLoopSystem = currentPlayerLoop.subSystemList[i];
				if (!(playerLoopSystem.type != typeof(PostLateUpdate)))
				{
					List<PlayerLoopSystem> list = new List<PlayerLoopSystem>();
					foreach (PlayerLoopSystem playerLoopSystem2 in playerLoopSystem.subSystemList)
					{
						if (playerLoopSystem2.type != base.GetType())
						{
							list.Add(playerLoopSystem2);
						}
					}
					playerLoopSystem.subSystemList = list.ToArray();
					currentPlayerLoop.subSystemList[i] = playerLoopSystem;
				}
			}
			PlayerLoop.SetPlayerLoop(currentPlayerLoop);
		}

		internal static bool IsEnabled()
		{
			return GPUResidentDrawer.s_Instance != null;
		}

		internal static GPUResidentDrawerSettings GetGlobalSettingsFromRPAsset()
		{
			IGPUResidentRenderPipeline igpuresidentRenderPipeline = GraphicsSettings.currentRenderPipeline as IGPUResidentRenderPipeline;
			if (igpuresidentRenderPipeline == null)
			{
				return default(GPUResidentDrawerSettings);
			}
			GPUResidentDrawerSettings gpuResidentDrawerSettings = igpuresidentRenderPipeline.gpuResidentDrawerSettings;
			if (GPUResidentDrawer.IsForcedOnViaCommandLine())
			{
				gpuResidentDrawerSettings.mode = GPUResidentDrawerMode.InstancedDrawing;
			}
			if (GPUResidentDrawer.IsOcclusionForcedOnViaCommandLine() || GPUResidentDrawer.ForceOcclusion)
			{
				gpuResidentDrawerSettings.enableOcclusionCulling = true;
			}
			return gpuResidentDrawerSettings;
		}

		internal static bool IsForcedOnViaCommandLine()
		{
			return false;
		}

		internal static bool IsOcclusionForcedOnViaCommandLine()
		{
			return false;
		}

		internal static bool MaintainContext { get; set; }

		internal static bool ForceOcclusion { get; set; }

		internal static void Reinitialize()
		{
			GPUResidentDrawer.Recreate(GPUResidentDrawer.GetGlobalSettingsFromRPAsset());
		}

		private static void CleanUp()
		{
			if (GPUResidentDrawer.s_Instance == null)
			{
				return;
			}
			GPUResidentDrawer.s_Instance.Dispose();
			GPUResidentDrawer.s_Instance = null;
		}

		private static void Recreate(GPUResidentDrawerSettings settings)
		{
			GPUResidentDrawer.CleanUp();
			string message;
			LogType severity;
			if (GPUResidentDrawer.IsGPUResidentDrawerSupportedBySRP(settings, out message, out severity))
			{
				GPUResidentDrawer.s_Instance = new GPUResidentDrawer(settings, 4096, 0);
				return;
			}
			GPUResidentDrawer.LogMessage(message, severity);
		}

		internal GPUResidentBatcher batcher
		{
			get
			{
				return this.m_Batcher;
			}
		}

		internal GPUResidentDrawerSettings settings
		{
			get
			{
				return this.m_Settings;
			}
		}

		private GPUResidentDrawer(GPUResidentDrawerSettings settings, int maxInstanceCount, int maxTreeInstanceCount)
		{
			GPUResidentDrawerResources renderPipelineSettings = GraphicsSettings.GetRenderPipelineSettings<GPUResidentDrawerResources>();
			RenderPipelineAsset currentRenderPipeline = GraphicsSettings.currentRenderPipeline;
			this.m_Settings = settings;
			RenderersBatchersContextDesc renderersBatchersContextDesc = RenderersBatchersContextDesc.NewDefault();
			renderersBatchersContextDesc.instanceNumInfo = new InstanceNumInfo(maxInstanceCount, maxTreeInstanceCount);
			renderersBatchersContextDesc.supportDitheringCrossFade = settings.supportDitheringCrossFade;
			renderersBatchersContextDesc.smallMeshScreenPercentage = settings.smallMeshScreenPercentage;
			renderersBatchersContextDesc.enableBoundingSpheresInstanceData = settings.enableOcclusionCulling;
			renderersBatchersContextDesc.enableCullerDebugStats = true;
			InstanceCullingBatcherDesc instanceCullerBatcherDesc = InstanceCullingBatcherDesc.NewDefault();
			this.m_GPUDrivenProcessor = new GPUDrivenProcessor();
			this.m_BatchersContext = new RenderersBatchersContext(ref renderersBatchersContextDesc, this.m_GPUDrivenProcessor, renderPipelineSettings);
			this.m_Batcher = new GPUResidentBatcher(this.m_BatchersContext, instanceCullerBatcherDesc, this.m_GPUDrivenProcessor);
			this.m_Dispatcher = new ObjectDispatcher();
			this.m_Dispatcher.EnableTypeTracking<LODGroup>(ObjectDispatcher.TypeTrackingFlags.SceneObjects);
			this.m_Dispatcher.EnableTypeTracking<Mesh>(ObjectDispatcher.TypeTrackingFlags.Default);
			this.m_Dispatcher.EnableTypeTracking<Material>(ObjectDispatcher.TypeTrackingFlags.Default);
			this.m_Dispatcher.EnableTypeTracking<MeshRenderer>(ObjectDispatcher.TypeTrackingFlags.SceneObjects);
			this.m_Dispatcher.EnableTypeTracking<Camera>(ObjectDispatcher.TypeTrackingFlags.SceneObjects | ObjectDispatcher.TypeTrackingFlags.EditorOnlyObjects);
			this.m_Dispatcher.EnableTransformTracking<MeshRenderer>(ObjectDispatcher.TransformTrackingType.GlobalTRS);
			this.m_Dispatcher.EnableTransformTracking<LODGroup>(ObjectDispatcher.TransformTrackingType.GlobalTRS);
			SceneManager.sceneLoaded += this.OnSceneLoaded;
			RenderPipelineManager.beginContextRendering += this.OnBeginContextRendering;
			RenderPipelineManager.endContextRendering += this.OnEndContextRendering;
			RenderPipelineManager.beginCameraRendering += this.OnBeginCameraRendering;
			RenderPipelineManager.endCameraRendering += this.OnEndCameraRendering;
			Shader.EnableKeyword("USE_LEGACY_LIGHTMAPS");
			this.InsertIntoPlayerLoop();
		}

		private void Dispose()
		{
			SceneManager.sceneLoaded -= this.OnSceneLoaded;
			RenderPipelineManager.beginContextRendering -= this.OnBeginContextRendering;
			RenderPipelineManager.endContextRendering -= this.OnEndContextRendering;
			RenderPipelineManager.beginCameraRendering -= this.OnBeginCameraRendering;
			RenderPipelineManager.endCameraRendering -= this.OnEndCameraRendering;
			this.RemoveFromPlayerLoop();
			Shader.DisableKeyword("USE_LEGACY_LIGHTMAPS");
			this.m_Dispatcher.Dispose();
			this.m_Dispatcher = null;
			GPUResidentDrawer.s_Instance = null;
			GPUResidentBatcher batcher = this.m_Batcher;
			if (batcher != null)
			{
				batcher.Dispose();
			}
			this.m_BatchersContext.Dispose();
			this.m_GPUDrivenProcessor.Dispose();
			this.m_ContextIntPtr = IntPtr.Zero;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (mode == LoadSceneMode.Additive)
			{
				this.m_BatchersContext.UpdateAmbientProbeAndGpuBuffer(true);
			}
		}

		private static void PostPostLateUpdateStatic()
		{
			GPUResidentDrawer gpuresidentDrawer = GPUResidentDrawer.s_Instance;
			if (gpuresidentDrawer == null)
			{
				return;
			}
			gpuresidentDrawer.PostPostLateUpdate();
		}

		private void OnBeginContextRendering(ScriptableRenderContext context, List<Camera> cameras)
		{
			if (GPUResidentDrawer.s_Instance == null)
			{
				return;
			}
			if (this.m_ContextIntPtr == IntPtr.Zero)
			{
				this.m_ContextIntPtr = context.Internal_GetPtr();
				this.m_Batcher.OnBeginContextRendering();
			}
		}

		private void OnEndContextRendering(ScriptableRenderContext context, List<Camera> cameras)
		{
			if (GPUResidentDrawer.s_Instance == null)
			{
				return;
			}
			if (this.m_ContextIntPtr == context.Internal_GetPtr())
			{
				this.m_ContextIntPtr = IntPtr.Zero;
				this.m_Batcher.OnEndContextRendering();
			}
		}

		private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			this.m_Batcher.OnBeginCameraRendering(camera);
		}

		private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			this.m_Batcher.OnEndCameraRendering(camera);
		}

		private void PostPostLateUpdate()
		{
			this.m_BatchersContext.UpdateAmbientProbeAndGpuBuffer(false);
			TransformDispatchData transformChangesAndClear = this.m_Dispatcher.GetTransformChangesAndClear<LODGroup>(ObjectDispatcher.TransformTrackingType.GlobalTRS, Allocator.TempJob);
			TypeDispatchData typeChangesAndClear = this.m_Dispatcher.GetTypeChangesAndClear<LODGroup>(Allocator.TempJob, false, true);
			TypeDispatchData typeChangesAndClear2 = this.m_Dispatcher.GetTypeChangesAndClear<Mesh>(Allocator.TempJob, true, true);
			TypeDispatchData typeChangesAndClear3 = this.m_Dispatcher.GetTypeChangesAndClear<Camera>(Allocator.TempJob, false, true);
			TypeDispatchData typeChangesAndClear4 = this.m_Dispatcher.GetTypeChangesAndClear<Material>(Allocator.TempJob, false, true);
			TypeDispatchData typeChangesAndClear5 = this.m_Dispatcher.GetTypeChangesAndClear<MeshRenderer>(Allocator.TempJob, false, true);
			NativeList<int> nativeList;
			NativeList<int> nativeList2;
			NativeList<GPUDrivenPackedMaterialData> nativeList3;
			this.ClassifyMaterials(typeChangesAndClear4.changedID, out nativeList, out nativeList2, out nativeList3, Allocator.TempJob);
			NativeList<int> nativeList4 = this.FindUnsupportedRenderers(nativeList.AsArray());
			this.ProcessMaterials(typeChangesAndClear4.destroyedID, nativeList.AsArray());
			this.ProcessMeshes(typeChangesAndClear2.destroyedID);
			this.ProcessLODGroups(typeChangesAndClear.changedID, typeChangesAndClear.destroyedID, transformChangesAndClear.transformedID);
			this.ProcessCameras(typeChangesAndClear3.changedID, typeChangesAndClear3.destroyedID);
			this.ProcessRenderers(typeChangesAndClear5, nativeList4.AsArray());
			this.ProcessRendererMaterialAndMeshChanges(typeChangesAndClear5.changedID, nativeList2.AsArray(), nativeList3.AsArray(), typeChangesAndClear2.changedID);
			transformChangesAndClear.Dispose();
			typeChangesAndClear.Dispose();
			typeChangesAndClear2.Dispose();
			typeChangesAndClear4.Dispose();
			typeChangesAndClear3.Dispose();
			typeChangesAndClear5.Dispose();
			nativeList.Dispose();
			nativeList4.Dispose();
			nativeList2.Dispose();
			nativeList3.Dispose();
			this.m_BatchersContext.UpdateInstanceMotions();
			this.m_Batcher.UpdateFrame();
		}

		private void ProcessMaterials(NativeArray<int> destroyedID, NativeArray<int> unsupportedMaterials)
		{
			if (destroyedID.Length > 0)
			{
				this.m_Batcher.DestroyMaterials(destroyedID);
			}
			if (unsupportedMaterials.Length > 0)
			{
				this.m_Batcher.DestroyMaterials(unsupportedMaterials);
			}
		}

		private void ProcessCameras(NativeArray<int> changedIDs, NativeArray<int> destroyedIDs)
		{
			this.m_BatchersContext.UpdateCameras(changedIDs);
			this.m_BatchersContext.FreePerCameraInstanceData(destroyedIDs);
		}

		private void ProcessMeshes(NativeArray<int> destroyedID)
		{
			if (destroyedID.Length == 0)
			{
				return;
			}
			NativeList<InstanceHandle> instances = new NativeList<InstanceHandle>(Allocator.TempJob);
			this.ScheduleQueryMeshInstancesJob(destroyedID, instances).Complete();
			this.m_Batcher.DestroyDrawInstances(instances.AsArray());
			instances.Dispose();
			this.m_Batcher.DestroyMeshes(destroyedID);
		}

		private void ProcessLODGroups(NativeArray<int> changedID, NativeArray<int> destroyed, NativeArray<int> transformedID)
		{
			this.m_BatchersContext.DestroyLODGroups(destroyed);
			this.m_BatchersContext.UpdateLODGroups(changedID);
			this.m_BatchersContext.TransformLODGroups(transformedID);
		}

		private void ProcessRendererMaterialAndMeshChanges(NativeArray<int> excludedRenderers, NativeArray<int> changedMaterials, NativeArray<GPUDrivenPackedMaterialData> changedPackedMaterialDatas, NativeArray<int> changedMeshes)
		{
			if (changedMaterials.Length == 0 && changedMeshes.Length == 0)
			{
				return;
			}
			NativeHashSet<int> materialsWithChangedPackedMaterial = this.GetMaterialsWithChangedPackedMaterial(changedMaterials, changedPackedMaterialDatas, Allocator.TempJob);
			JobHandle jobHandle = this.m_Batcher.SchedulePackedMaterialCacheUpdate(changedMaterials, changedPackedMaterialDatas);
			if (materialsWithChangedPackedMaterial.Count == 0 && changedMeshes.Length == 0)
			{
				materialsWithChangedPackedMaterial.Dispose();
				jobHandle.Complete();
				return;
			}
			NativeArray<int> nativeArray = new NativeArray<int>(excludedRenderers, Allocator.TempJob);
			if (nativeArray.Length > 0)
			{
				nativeArray.ParallelSort().Complete();
			}
			ValueTuple<NativeList<int>, NativeList<int>> valueTuple = this.FindRenderersFromMaterialsOrMeshes(nativeArray, materialsWithChangedPackedMaterial, changedMeshes, Allocator.TempJob);
			NativeList<int> item = valueTuple.Item1;
			NativeList<int> item2 = valueTuple.Item2;
			materialsWithChangedPackedMaterial.Dispose();
			nativeArray.Dispose();
			jobHandle.Complete();
			if (item.Length == 0 && item2.Length == 0)
			{
				item.Dispose();
				item2.Dispose();
				return;
			}
			int length = item.Length;
			int length2 = item2.Length;
			int length3 = length + length2;
			NativeArray<InstanceHandle> instances = new NativeArray<InstanceHandle>(length3, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<int> nativeArray2 = new NativeArray<int>(length3, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<int>.Copy(item.AsArray(), nativeArray2, length);
			NativeArray<int>.Copy(item2.AsArray(), nativeArray2.GetSubArray(length, length2), length2);
			this.ScheduleQueryRendererGroupInstancesJob(nativeArray2, instances).Complete();
			this.m_Batcher.DestroyDrawInstances(instances);
			this.m_Batcher.UpdateRenderers(item.AsArray(), true);
			this.m_Batcher.UpdateRenderers(item2.AsArray(), false);
			item.Dispose();
			item2.Dispose();
		}

		private void ProcessRenderers(TypeDispatchData rendererChanges, NativeArray<int> unsupportedRenderers)
		{
			NativeArray<InstanceHandle> instances = new NativeArray<InstanceHandle>(rendererChanges.changedID.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			this.ScheduleQueryRendererGroupInstancesJob(rendererChanges.changedID, instances).Complete();
			this.m_Batcher.DestroyDrawInstances(instances);
			instances.Dispose();
			this.m_Batcher.UpdateRenderers(rendererChanges.changedID, false);
			this.FreeRendererGroupInstances(rendererChanges.destroyedID, unsupportedRenderers);
			TransformDispatchData transformChangesAndClear = this.m_Dispatcher.GetTransformChangesAndClear<MeshRenderer>(ObjectDispatcher.TransformTrackingType.GlobalTRS, Allocator.TempJob);
			NativeArray<InstanceHandle> instances2 = new NativeArray<InstanceHandle>(transformChangesAndClear.transformedID.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			this.ScheduleQueryRendererGroupInstancesJob(transformChangesAndClear.transformedID, instances2).Complete();
			this.TransformInstances(instances2, transformChangesAndClear.localToWorldMatrices);
			instances2.Dispose();
			transformChangesAndClear.Dispose();
		}

		private void TransformInstances(NativeArray<InstanceHandle> instances, NativeArray<Matrix4x4> localToWorldMatrices)
		{
			this.m_BatchersContext.UpdateInstanceTransforms(instances, localToWorldMatrices);
		}

		private void FreeInstances(NativeArray<InstanceHandle> instances)
		{
			this.m_Batcher.DestroyDrawInstances(instances);
			this.m_BatchersContext.FreeInstances(instances);
		}

		private void FreeRendererGroupInstances(NativeArray<int> rendererGroupIDs, NativeArray<int> unsupportedRendererGroupIDs)
		{
			this.m_Batcher.FreeRendererGroupInstances(rendererGroupIDs);
			if (unsupportedRendererGroupIDs.Length > 0)
			{
				this.m_Batcher.FreeRendererGroupInstances(unsupportedRendererGroupIDs);
				this.m_GPUDrivenProcessor.DisableGPUDrivenRendering(unsupportedRendererGroupIDs);
			}
		}

		private InstanceHandle AppendNewInstance(int rendererGroupID, in Matrix4x4 instanceTransform)
		{
			throw new NotImplementedException();
		}

		private JobHandle ScheduleQueryRendererGroupInstancesJob(NativeArray<int> rendererGroupIDs, NativeArray<InstanceHandle> instances)
		{
			return this.m_BatchersContext.ScheduleQueryRendererGroupInstancesJob(rendererGroupIDs, instances);
		}

		private JobHandle ScheduleQueryRendererGroupInstancesJob(NativeArray<int> rendererGroupIDs, NativeList<InstanceHandle> instances)
		{
			return this.m_BatchersContext.ScheduleQueryRendererGroupInstancesJob(rendererGroupIDs, instances);
		}

		private JobHandle ScheduleQueryRendererGroupInstancesJob(NativeArray<int> rendererGroupIDs, NativeArray<int> instancesOffset, NativeArray<int> instancesCount, NativeList<InstanceHandle> instances)
		{
			return this.m_BatchersContext.ScheduleQueryRendererGroupInstancesJob(rendererGroupIDs, instancesOffset, instancesCount, instances);
		}

		private JobHandle ScheduleQueryMeshInstancesJob(NativeArray<int> sortedMeshIDs, NativeList<InstanceHandle> instances)
		{
			return this.m_BatchersContext.ScheduleQueryMeshInstancesJob(sortedMeshIDs, instances);
		}

		private void ClassifyMaterials(NativeArray<int> materials, out NativeList<int> unsupportedMaterials, out NativeList<int> supportedMaterials, out NativeList<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas, Allocator allocator)
		{
			supportedMaterials = new NativeList<int>(materials.Length, allocator);
			unsupportedMaterials = new NativeList<int>(materials.Length, allocator);
			supportedPackedMaterialDatas = new NativeList<GPUDrivenPackedMaterialData>(materials.Length, allocator);
			if (materials.Length > 0)
			{
				NativeParallelHashMap<int, BatchMaterialID>.ReadOnly readOnly = this.m_Batcher.instanceCullingBatcher.batchMaterialHash.AsReadOnly();
				GPUResidentDrawerBurst.ClassifyMaterials(materials, readOnly, ref supportedMaterials, ref unsupportedMaterials, ref supportedPackedMaterialDatas);
			}
		}

		private NativeList<int> FindUnsupportedRenderers(NativeArray<int> unsupportedMaterials)
		{
			NativeList<int> result = new NativeList<int>(Allocator.TempJob);
			if (unsupportedMaterials.Length > 0)
			{
				GPUResidentDrawerBurst.FindUnsupportedRenderers(unsupportedMaterials, this.m_BatchersContext.sharedInstanceData.materialIDArrays, this.m_BatchersContext.sharedInstanceData.rendererGroupIDs, ref result);
			}
			return result;
		}

		private NativeHashSet<int> GetMaterialsWithChangedPackedMaterial(NativeArray<int> materials, NativeArray<GPUDrivenPackedMaterialData> packedMaterialDatas, Allocator allocator)
		{
			NativeHashSet<int> result = new NativeHashSet<int>(materials.Length, allocator);
			NativeParallelHashMap<int, GPUDrivenPackedMaterialData>.ReadOnly readOnly = this.batcher.instanceCullingBatcher.packedMaterialHash.AsReadOnly();
			GPUResidentDrawerBurst.GetMaterialsWithChangedPackedMaterial(materials, packedMaterialDatas, readOnly, ref result);
			return result;
		}

		[return: TupleElementNames(new string[]
		{
			"renderersWithMaterials",
			"renderersWithMeshes"
		})]
		private ValueTuple<NativeList<int>, NativeList<int>> FindRenderersFromMaterialsOrMeshes(NativeArray<int> sortedExcludeRenderers, NativeHashSet<int> materials, NativeArray<int> meshes, Allocator rendererListAllocator)
		{
			CPUSharedInstanceData.ReadOnly sharedInstanceData = this.m_BatchersContext.sharedInstanceData;
			NativeList<int> item = new NativeList<int>(sharedInstanceData.rendererGroupIDs.Length, rendererListAllocator);
			NativeList<int> item2 = new NativeList<int>(sharedInstanceData.rendererGroupIDs.Length, rendererListAllocator);
			new GPUResidentDrawer.FindRenderersFromMaterialOrMeshJob
			{
				materialIDs = materials.AsReadOnly(),
				materialIDArrays = sharedInstanceData.materialIDArrays,
				meshIDs = meshes.AsReadOnly(),
				meshIDArray = sharedInstanceData.meshIDs,
				rendererGroupIDs = sharedInstanceData.rendererGroupIDs,
				sortedExcludeRendererIDs = sortedExcludeRenderers.AsReadOnly(),
				selectedRenderGroupsForMaterials = item.AsParallelWriter(),
				selectedRenderGroupsForMeshes = item2.AsParallelWriter()
			}.ScheduleBatch(sharedInstanceData.rendererGroupIDs.Length, 128, default(JobHandle)).Complete();
			return new ValueTuple<NativeList<int>, NativeList<int>>(item, item2);
		}

		internal static bool IsProjectSupported()
		{
			string text;
			LogType logType;
			return GPUResidentDrawer.IsProjectSupported(out text, out logType);
		}

		internal static bool IsProjectSupported(out string message, out LogType severity)
		{
			message = string.Empty;
			severity = LogType.Log;
			if (Application.platform == RuntimePlatform.VisionOS)
			{
				message = GPUResidentDrawer.Strings.visionOSNotSupported;
				severity = LogType.Log;
				return false;
			}
			if (BatchRendererGroup.BufferTarget != BatchBufferTarget.RawBuffer)
			{
				severity = LogType.Warning;
				message = GPUResidentDrawer.Strings.rawBufferNotSupportedByPlatform;
				return false;
			}
			return true;
		}

		internal static bool IsGPUResidentDrawerSupportedBySRP(GPUResidentDrawerSettings settings, out string message, out LogType severity)
		{
			message = string.Empty;
			severity = LogType.Log;
			if (settings.mode == GPUResidentDrawerMode.Disabled)
			{
				message = GPUResidentDrawer.Strings.drawerModeDisabled;
				return false;
			}
			if (GPUResidentDrawer.IsForcedOnViaCommandLine() || GPUResidentDrawer.MaintainContext)
			{
				return true;
			}
			IGPUResidentRenderPipeline igpuresidentRenderPipeline = GraphicsSettings.currentRenderPipeline as IGPUResidentRenderPipeline;
			if (igpuresidentRenderPipeline == null)
			{
				message = GPUResidentDrawer.Strings.notGPUResidentRenderPipeline;
				severity = LogType.Warning;
				return false;
			}
			return igpuresidentRenderPipeline.IsGPUResidentDrawerSupportedBySRP(out message, out severity) && GPUResidentDrawer.IsProjectSupported(out message, out severity);
		}

		internal static void LogMessage(string message, LogType severity)
		{
			switch (severity)
			{
			case LogType.Error:
			case LogType.Exception:
				Debug.LogError(message);
				return;
			case LogType.Assert:
			case LogType.Log:
				break;
			case LogType.Warning:
				Debug.LogWarning(message);
				break;
			default:
				return;
			}
		}

		private static GPUResidentDrawer s_Instance;

		private IntPtr m_ContextIntPtr = IntPtr.Zero;

		private GPUResidentDrawerSettings m_Settings;

		private GPUDrivenProcessor m_GPUDrivenProcessor;

		private RenderersBatchersContext m_BatchersContext;

		private GPUResidentBatcher m_Batcher;

		private ObjectDispatcher m_Dispatcher;

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		private struct FindRenderersFromMaterialOrMeshJob : IJobParallelForBatch
		{
			public unsafe void Execute(int startIndex, int count)
			{
				int* ptr = stackalloc int[(UIntPtr)512];
				UnsafeList<int> unsafeList = new UnsafeList<int>(ptr, 128);
				unsafeList.Length = 0;
				int* ptr2 = stackalloc int[(UIntPtr)512];
				UnsafeList<int> unsafeList2 = new UnsafeList<int>(ptr2, 128);
				unsafeList2.Length = 0;
				for (int i = 0; i < count; i++)
				{
					int index = startIndex + i;
					int value = this.rendererGroupIDs[index];
					if (this.sortedExcludeRendererIDs.BinarySearch(value) < 0)
					{
						int value2 = this.meshIDArray[index];
						if (this.meshIDs.Contains(value2))
						{
							unsafeList2.AddNoResize(value);
						}
						else
						{
							SmallIntegerArray smallIntegerArray = this.materialIDArrays[index];
							for (int j = 0; j < smallIntegerArray.Length; j++)
							{
								int item = smallIntegerArray[j];
								if (this.materialIDs.Contains(item))
								{
									unsafeList.AddNoResize(value);
									break;
								}
							}
						}
					}
				}
				this.selectedRenderGroupsForMaterials.AddRangeNoResize((void*)ptr, unsafeList.Length);
				this.selectedRenderGroupsForMeshes.AddRangeNoResize((void*)ptr2, unsafeList2.Length);
			}

			public const int k_BatchSize = 128;

			[ReadOnly]
			public NativeHashSet<int>.ReadOnly materialIDs;

			[ReadOnly]
			public NativeArray<SmallIntegerArray>.ReadOnly materialIDArrays;

			[ReadOnly]
			public NativeArray<int>.ReadOnly meshIDs;

			[ReadOnly]
			public NativeArray<int>.ReadOnly meshIDArray;

			[ReadOnly]
			public NativeArray<int>.ReadOnly rendererGroupIDs;

			[ReadOnly]
			public NativeArray<int>.ReadOnly sortedExcludeRendererIDs;

			[WriteOnly]
			public NativeList<int>.ParallelWriter selectedRenderGroupsForMaterials;

			[WriteOnly]
			public NativeList<int>.ParallelWriter selectedRenderGroupsForMeshes;
		}

		private static class Strings
		{
			public static readonly string drawerModeDisabled = "GPUResidentDrawer Drawer mode is disabled. Enable it on your current RenderPipelineAsset";

			public static readonly string allowInEditModeDisabled = "GPUResidentDrawer The current mode does not allow the resident drawer. Check setting Allow In Edit Mode";

			public static readonly string notGPUResidentRenderPipeline = "GPUResidentDrawer Disabled due to current render pipeline not being of type IGPUResidentRenderPipeline";

			public static readonly string rawBufferNotSupportedByPlatform = string.Format("{0} The current platform does not support {1}", "GPUResidentDrawer", BatchBufferTarget.RawBuffer.GetType());

			public static readonly string kernelNotPresent = "GPUResidentDrawer Kernel not present, please ensure the player settings includes a supported graphics API.";

			public static readonly string batchRendererGroupShaderStrippingModeInvalid = "GPUResidentDrawer \"BatchRendererGroup Variants\" setting must be \"Keep All\".  The current setting will cause errors when building a player because all DOTS instancing shaders will be stripped To fix, modify Graphics settings and set \"BatchRendererGroup Variants\" to \"Keep All\".";

			public static readonly string visionOSNotSupported = "GPUResidentDrawer Disabled on VisionOS as it is non applicable. This platform uses a custom rendering path and doesn't go through the resident drawer.";
		}
	}
}
