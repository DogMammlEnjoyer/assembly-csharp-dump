using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.RenderGraphModule
{
	[MovedFrom(true, "UnityEngine.Experimental.Rendering.RenderGraphModule", "UnityEngine.Rendering.RenderGraphModule", null)]
	public class RenderGraph
	{
		internal NativePassCompiler CompileNativeRenderGraph(int graphHash)
		{
			NativePassCompiler result;
			using (new ProfilingScope(this.m_RenderGraphContext.cmd, ProfilingSampler.Get<RenderGraphProfileId>(RenderGraphProfileId.CompileRenderGraph)))
			{
				if (this.nativeCompiler == null)
				{
					this.nativeCompiler = new NativePassCompiler(this.m_CompilationCache);
				}
				if (!this.nativeCompiler.Initialize(this.m_Resources, this.m_RenderPasses, this.m_DebugParameters, this.name, this.m_EnableCompilationCaching, graphHash, this.m_ExecutionCount))
				{
					this.nativeCompiler.Compile(this.m_Resources);
				}
				NativeList<PassData> passData = this.nativeCompiler.contextData.passData;
				int length = passData.Length;
				for (int i = 0; i < length; i++)
				{
					if (!passData.ElementAt(i).culled)
					{
						RenderGraphPass renderGraphPass = this.m_RenderPasses[i];
						this.m_RendererLists.AddRange(renderGraphPass.usedRendererListList);
					}
				}
				this.m_Resources.CreateRendererLists(this.m_RendererLists, this.m_RenderGraphContext.renderContext, this.m_RendererListCulling);
				result = this.nativeCompiler;
			}
			return result;
		}

		private void ExecuteNativeRenderGraph()
		{
			using (new ProfilingScope(this.m_RenderGraphContext.cmd, ProfilingSampler.Get<RenderGraphProfileId>(RenderGraphProfileId.ExecuteRenderGraph)))
			{
				this.nativeCompiler.ExecuteGraph(this.m_RenderGraphContext, this.m_Resources, this.m_RenderPasses);
				if (!this.m_RenderGraphContext.contextlessTesting)
				{
					this.m_RenderGraphContext.renderContext.ExecuteCommandBuffer(this.m_RenderGraphContext.cmd);
				}
				this.m_RenderGraphContext.cmd.Clear();
			}
		}

		public bool nativeRenderPassesEnabled { get; set; }

		internal static bool hasAnyRenderGraphWithNativeRenderPassesEnabled
		{
			get
			{
				using (List<RenderGraph>.Enumerator enumerator = RenderGraph.s_RegisteredGraphs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.nativeRenderPassesEnabled)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public string name { get; private set; } = "RenderGraph";

		internal void RequestCaptureDebugData(string executionName)
		{
			this.m_CaptureDebugDataForExecution = executionName;
		}

		internal RenderGraphState RenderGraphState
		{
			get
			{
				return this.m_RenderGraphState;
			}
			set
			{
				this.m_RenderGraphState = value;
			}
		}

		public static bool isRenderGraphViewerActive { get; internal set; }

		internal static bool enableValidityChecks { get; private set; }

		public RenderGraphDefaultResources defaultResources
		{
			get
			{
				return this.m_DefaultResources;
			}
		}

		public RenderGraph(string name = "RenderGraph")
		{
			this.name = name;
			RenderGraphGlobalSettings renderGraphGlobalSettings;
			if (GraphicsSettings.TryGetRenderPipelineSettings<RenderGraphGlobalSettings>(out renderGraphGlobalSettings))
			{
				this.m_EnableCompilationCaching = renderGraphGlobalSettings.enableCompilationCaching;
				if (this.m_EnableCompilationCaching)
				{
					this.m_CompilationCache = new RenderGraphCompilationCache();
				}
				RenderGraph.enableValidityChecks = renderGraphGlobalSettings.enableValidityChecks;
			}
			else
			{
				RenderGraph.enableValidityChecks = true;
			}
			this.m_TempMRTArrays = new RenderTargetIdentifier[RenderGraph.kMaxMRTCount][];
			for (int i = 0; i < RenderGraph.kMaxMRTCount; i++)
			{
				this.m_TempMRTArrays[i] = new RenderTargetIdentifier[i + 1];
			}
			this.m_Resources = new RenderGraphResourceRegistry(this.m_DebugParameters, this.m_FrameInformationLogger);
			RenderGraph.s_RegisteredGraphs.Add(this);
			RenderGraph.OnGraphRegisteredDelegate onGraphRegisteredDelegate = RenderGraph.onGraphRegistered;
			if (onGraphRegisteredDelegate != null)
			{
				onGraphRegisteredDelegate(this);
			}
			this.m_RenderGraphState = RenderGraphState.Idle;
			RenderGraph.RenderGraphExceptionMessages.enableCaller = true;
		}

		public void Cleanup()
		{
			this.ForceCleanup();
		}

		internal void ForceCleanup()
		{
			this.ClearCurrentCompiledGraph();
			this.m_Resources.Cleanup();
			this.m_DefaultResources.Cleanup();
			this.m_RenderGraphPool.Cleanup();
			RenderGraph.s_RegisteredGraphs.Remove(this);
			RenderGraph.OnGraphRegisteredDelegate onGraphRegisteredDelegate = RenderGraph.onGraphUnregistered;
			if (onGraphRegisteredDelegate != null)
			{
				onGraphRegisteredDelegate(this);
			}
			NativePassCompiler nativePassCompiler = this.nativeCompiler;
			if (nativePassCompiler != null)
			{
				nativePassCompiler.Cleanup();
			}
			RenderGraphCompilationCache compilationCache = this.m_CompilationCache;
			if (compilationCache != null)
			{
				compilationCache.Cleanup();
			}
			DelegateHashCodeUtils.ClearCache();
		}

		internal RenderGraphDebugParams debugParams
		{
			get
			{
				return this.m_DebugParameters;
			}
		}

		internal List<DebugUI.Widget> GetWidgetList()
		{
			return this.m_DebugParameters.GetWidgetList(this.name);
		}

		internal bool areAnySettingsActive
		{
			get
			{
				return this.m_DebugParameters.AreAnySettingsActive;
			}
		}

		public void RegisterDebug(DebugUI.Panel panel = null)
		{
			this.m_DebugParameters.RegisterDebug(this.name, panel);
		}

		public void UnRegisterDebug()
		{
			this.m_DebugParameters.UnRegisterDebug(this.name);
		}

		public static List<RenderGraph> GetRegisteredRenderGraphs()
		{
			return RenderGraph.s_RegisteredGraphs;
		}

		internal RenderGraph.DebugData GetDebugData(string executionName)
		{
			RenderGraph.DebugData result;
			if (this.m_DebugData.TryGetValue(executionName, out result))
			{
				return result;
			}
			return null;
		}

		public void EndFrame()
		{
			this.m_Resources.PurgeUnusedGraphicsResources();
			if (this.m_DebugParameters.logFrameInformation)
			{
				this.m_FrameInformationLogger.FlushLogs();
			}
			if (this.m_DebugParameters.logResources)
			{
				this.m_Resources.FlushLogs();
			}
			this.m_DebugParameters.ResetLogging();
		}

		public TextureHandle ImportTexture(RTHandle rt)
		{
			return this.m_Resources.ImportTexture(rt, false);
		}

		public TextureHandle ImportShadingRateImageTexture(RTHandle rt)
		{
			if (ShadingRateInfo.supportsPerImageTile)
			{
				return this.m_Resources.ImportTexture(rt, false);
			}
			return TextureHandle.nullHandle;
		}

		public TextureHandle ImportTexture(RTHandle rt, ImportResourceParams importParams)
		{
			return this.m_Resources.ImportTexture(rt, importParams, false);
		}

		public TextureHandle ImportTexture(RTHandle rt, RenderTargetInfo info, ImportResourceParams importParams = default(ImportResourceParams))
		{
			return this.m_Resources.ImportTexture(rt, info, importParams);
		}

		internal TextureHandle ImportTexture(RTHandle rt, bool isBuiltin)
		{
			return this.m_Resources.ImportTexture(rt, isBuiltin);
		}

		public TextureHandle ImportBackbuffer(RenderTargetIdentifier rt, RenderTargetInfo info, ImportResourceParams importParams = default(ImportResourceParams))
		{
			return this.m_Resources.ImportBackbuffer(rt, info, importParams);
		}

		public TextureHandle ImportBackbuffer(RenderTargetIdentifier rt)
		{
			RenderTargetInfo renderTargetInfo = default(RenderTargetInfo);
			renderTargetInfo.width = (renderTargetInfo.height = (renderTargetInfo.volumeDepth = (renderTargetInfo.msaaSamples = 1)));
			renderTargetInfo.format = GraphicsFormat.R8G8B8A8_SRGB;
			RenderGraphResourceRegistry resources = this.m_Resources;
			ImportResourceParams importResourceParams = default(ImportResourceParams);
			return resources.ImportBackbuffer(rt, renderTargetInfo, importResourceParams);
		}

		public TextureHandle CreateTexture(in TextureDesc desc)
		{
			return this.m_Resources.CreateTexture(desc, -1);
		}

		public TextureHandle CreateSharedTexture(in TextureDesc desc, bool explicitRelease = false)
		{
			return this.m_Resources.CreateSharedTexture(desc, explicitRelease);
		}

		public void RefreshSharedTextureDesc(TextureHandle handle, in TextureDesc desc)
		{
			this.m_Resources.RefreshSharedTextureDesc(handle, desc);
		}

		public void ReleaseSharedTexture(TextureHandle texture)
		{
			this.m_Resources.ReleaseSharedTexture(texture);
		}

		public TextureHandle CreateTexture(TextureHandle texture)
		{
			RenderGraphResourceRegistry resources = this.m_Resources;
			TextureDesc textureResourceDesc = this.m_Resources.GetTextureResourceDesc(texture.handle, false);
			return resources.CreateTexture(textureResourceDesc, -1);
		}

		public TextureHandle CreateTexture(TextureHandle texture, string name, bool clear = false)
		{
			TextureDesc textureDesc = this.GetTextureDesc(texture);
			textureDesc.name = name;
			textureDesc.clearBuffer = clear;
			return this.m_Resources.CreateTexture(textureDesc, -1);
		}

		public void CreateTextureIfInvalid(in TextureDesc desc, ref TextureHandle texture)
		{
			if (!texture.IsValid())
			{
				texture = this.m_Resources.CreateTexture(desc, -1);
			}
		}

		public TextureDesc GetTextureDesc(in TextureHandle texture)
		{
			return this.m_Resources.GetTextureResourceDesc(texture.handle, false);
		}

		public RenderTargetInfo GetRenderTargetInfo(TextureHandle texture)
		{
			RenderTargetInfo result;
			this.m_Resources.GetRenderTargetInfo(texture.handle, out result);
			return result;
		}

		public RendererListHandle CreateRendererList(in RendererListDesc desc)
		{
			return this.m_Resources.CreateRendererList(desc);
		}

		public RendererListHandle CreateRendererList(in RendererListParams desc)
		{
			return this.m_Resources.CreateRendererList(desc);
		}

		public RendererListHandle CreateShadowRendererList(ref ShadowDrawingSettings shadowDrawingSettings)
		{
			return this.m_Resources.CreateShadowRendererList(this.m_RenderGraphContext.renderContext, ref shadowDrawingSettings);
		}

		public RendererListHandle CreateGizmoRendererList(in Camera camera, in GizmoSubset gizmoSubset)
		{
			return this.m_Resources.CreateGizmoRendererList(this.m_RenderGraphContext.renderContext, camera, gizmoSubset);
		}

		public RendererListHandle CreateUIOverlayRendererList(in Camera camera)
		{
			RenderGraphResourceRegistry resources = this.m_Resources;
			ScriptableRenderContext renderContext = this.m_RenderGraphContext.renderContext;
			UISubset uisubset = UISubset.All;
			return resources.CreateUIOverlayRendererList(renderContext, camera, uisubset);
		}

		public RendererListHandle CreateUIOverlayRendererList(in Camera camera, in UISubset uiSubset)
		{
			return this.m_Resources.CreateUIOverlayRendererList(this.m_RenderGraphContext.renderContext, camera, uiSubset);
		}

		public RendererListHandle CreateWireOverlayRendererList(in Camera camera)
		{
			return this.m_Resources.CreateWireOverlayRendererList(this.m_RenderGraphContext.renderContext, camera);
		}

		public RendererListHandle CreateSkyboxRendererList(in Camera camera)
		{
			return this.m_Resources.CreateSkyboxRendererList(this.m_RenderGraphContext.renderContext, camera);
		}

		public RendererListHandle CreateSkyboxRendererList(in Camera camera, Matrix4x4 projectionMatrix, Matrix4x4 viewMatrix)
		{
			return this.m_Resources.CreateSkyboxRendererList(this.m_RenderGraphContext.renderContext, camera, projectionMatrix, viewMatrix);
		}

		public RendererListHandle CreateSkyboxRendererList(in Camera camera, Matrix4x4 projectionMatrixL, Matrix4x4 viewMatrixL, Matrix4x4 projectionMatrixR, Matrix4x4 viewMatrixR)
		{
			return this.m_Resources.CreateSkyboxRendererList(this.m_RenderGraphContext.renderContext, camera, projectionMatrixL, viewMatrixL, projectionMatrixR, viewMatrixR);
		}

		public BufferHandle ImportBuffer(GraphicsBuffer graphicsBuffer, bool forceRelease = false)
		{
			return this.m_Resources.ImportBuffer(graphicsBuffer, forceRelease);
		}

		public BufferHandle CreateBuffer(in BufferDesc desc)
		{
			return this.m_Resources.CreateBuffer(desc, -1);
		}

		public BufferHandle CreateBuffer(in BufferHandle graphicsBuffer)
		{
			RenderGraphResourceRegistry resources = this.m_Resources;
			BufferDesc bufferResourceDesc = this.m_Resources.GetBufferResourceDesc(graphicsBuffer.handle, false);
			return resources.CreateBuffer(bufferResourceDesc, -1);
		}

		public BufferDesc GetBufferDesc(in BufferHandle graphicsBuffer)
		{
			return this.m_Resources.GetBufferResourceDesc(graphicsBuffer.handle, false);
		}

		public RayTracingAccelerationStructureHandle ImportRayTracingAccelerationStructure(in RayTracingAccelerationStructure accelStruct, string name = null)
		{
			return this.m_Resources.ImportRayTracingAccelerationStructure(accelStruct, name);
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void CheckNotUsedWhenExecuting()
		{
			if (RenderGraph.enableValidityChecks && this.m_RenderGraphState == RenderGraphState.Executing)
			{
				throw new InvalidOperationException(RenderGraph.RenderGraphExceptionMessages.GetExceptionMessage(RenderGraphState.Executing));
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void CheckNotUsedWhenRecordingGraph()
		{
			if (RenderGraph.enableValidityChecks && this.m_RenderGraphState == RenderGraphState.RecordingGraph)
			{
				throw new InvalidOperationException(RenderGraph.RenderGraphExceptionMessages.GetExceptionMessage(RenderGraphState.RecordingGraph));
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void CheckNotUsedWhenRecordPassOrExecute()
		{
			if (RenderGraph.enableValidityChecks && (this.m_RenderGraphState == RenderGraphState.RecordingPass || this.m_RenderGraphState == RenderGraphState.Executing))
			{
				throw new InvalidOperationException(RenderGraph.RenderGraphExceptionMessages.GetExceptionMessage(RenderGraphState.RecordingPass | RenderGraphState.Executing));
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void CheckNotUsedWhenRecordingPass()
		{
			if (RenderGraph.enableValidityChecks && this.m_RenderGraphState == RenderGraphState.RecordingPass)
			{
				throw new InvalidOperationException(RenderGraph.RenderGraphExceptionMessages.GetExceptionMessage(RenderGraphState.RecordingPass));
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void CheckNotUsingNativeRenderPassCompiler()
		{
			if (RenderGraph.enableValidityChecks && this.nativeRenderPassesEnabled)
			{
				throw new InvalidOperationException("`AddRenderPass` is not compatible with the Native Render Pass Compiler. It is meant to be used with the HDRP Compiler. The APIs that are compatible with the Native Render Pass Compiler are AddUnsafePass, AddComputePass and AddRasterRenderPass.");
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void CheckNotUsedWhenActive()
		{
			if (RenderGraph.enableValidityChecks && (this.m_RenderGraphState & RenderGraphState.Active) != RenderGraphState.Idle)
			{
				throw new InvalidOperationException(RenderGraph.RenderGraphExceptionMessages.GetExceptionMessage(RenderGraphState.Active));
			}
		}

		public IRasterRenderGraphBuilder AddRasterRenderPass<PassData>(string passName, out PassData passData, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where PassData : class, new()
		{
			return this.AddRasterRenderPass<PassData>(passName, out passData, this.GetDefaultProfilingSampler(passName), file, line);
		}

		public IRasterRenderGraphBuilder AddRasterRenderPass<PassData>(string passName, out PassData passData, ProfilingSampler sampler, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where PassData : class, new()
		{
			this.m_RenderGraphState = RenderGraphState.RecordingPass;
			RasterRenderGraphPass<PassData> rasterRenderGraphPass = this.m_RenderGraphPool.Get<RasterRenderGraphPass<PassData>>();
			rasterRenderGraphPass.Initialize(this.m_RenderPasses.Count, this.m_RenderGraphPool.Get<PassData>(), passName, RenderGraphPassType.Raster, sampler);
			passData = rasterRenderGraphPass.data;
			this.m_RenderPasses.Add(rasterRenderGraphPass);
			this.m_builderInstance.Setup(rasterRenderGraphPass, this.m_Resources, this);
			return this.m_builderInstance;
		}

		public IComputeRenderGraphBuilder AddComputePass<PassData>(string passName, out PassData passData, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where PassData : class, new()
		{
			return this.AddComputePass<PassData>(passName, out passData, this.GetDefaultProfilingSampler(passName), file, line);
		}

		public IComputeRenderGraphBuilder AddComputePass<PassData>(string passName, out PassData passData, ProfilingSampler sampler, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where PassData : class, new()
		{
			this.m_RenderGraphState = RenderGraphState.RecordingPass;
			ComputeRenderGraphPass<PassData> computeRenderGraphPass = this.m_RenderGraphPool.Get<ComputeRenderGraphPass<PassData>>();
			computeRenderGraphPass.Initialize(this.m_RenderPasses.Count, this.m_RenderGraphPool.Get<PassData>(), passName, RenderGraphPassType.Compute, sampler);
			passData = computeRenderGraphPass.data;
			this.m_RenderPasses.Add(computeRenderGraphPass);
			this.m_builderInstance.Setup(computeRenderGraphPass, this.m_Resources, this);
			return this.m_builderInstance;
		}

		public IUnsafeRenderGraphBuilder AddUnsafePass<PassData>(string passName, out PassData passData, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where PassData : class, new()
		{
			return this.AddUnsafePass<PassData>(passName, out passData, this.GetDefaultProfilingSampler(passName), file, line);
		}

		public IUnsafeRenderGraphBuilder AddUnsafePass<PassData>(string passName, out PassData passData, ProfilingSampler sampler, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where PassData : class, new()
		{
			this.m_RenderGraphState = RenderGraphState.RecordingPass;
			UnsafeRenderGraphPass<PassData> unsafeRenderGraphPass = this.m_RenderGraphPool.Get<UnsafeRenderGraphPass<PassData>>();
			unsafeRenderGraphPass.Initialize(this.m_RenderPasses.Count, this.m_RenderGraphPool.Get<PassData>(), passName, RenderGraphPassType.Unsafe, sampler);
			unsafeRenderGraphPass.AllowGlobalState(true);
			passData = unsafeRenderGraphPass.data;
			this.m_RenderPasses.Add(unsafeRenderGraphPass);
			this.m_builderInstance.Setup(unsafeRenderGraphPass, this.m_Resources, this);
			return this.m_builderInstance;
		}

		public RenderGraphBuilder AddRenderPass<PassData>(string passName, out PassData passData, ProfilingSampler sampler, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where PassData : class, new()
		{
			this.m_RenderGraphState = RenderGraphState.RecordingPass;
			RenderGraphPass<PassData> renderGraphPass = this.m_RenderGraphPool.Get<RenderGraphPass<PassData>>();
			renderGraphPass.Initialize(this.m_RenderPasses.Count, this.m_RenderGraphPool.Get<PassData>(), passName, RenderGraphPassType.Legacy, sampler);
			renderGraphPass.AllowGlobalState(true);
			passData = renderGraphPass.data;
			this.m_RenderPasses.Add(renderGraphPass);
			return new RenderGraphBuilder(renderGraphPass, this.m_Resources, this);
		}

		public RenderGraphBuilder AddRenderPass<PassData>(string passName, out PassData passData, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where PassData : class, new()
		{
			return this.AddRenderPass<PassData>(passName, out passData, this.GetDefaultProfilingSampler(passName), file, line);
		}

		public void BeginRecording(in RenderGraphParameters parameters)
		{
			this.m_ExecutionExceptionWasRaised = false;
			this.m_RenderGraphState = RenderGraphState.RecordingGraph;
			this.m_CurrentFrameIndex = parameters.currentFrameIndex;
			this.m_CurrentExecutionName = ((parameters.executionName != null) ? parameters.executionName : "RenderGraphExecution");
			this.m_RendererListCulling = (parameters.rendererListCulling && !this.m_EnableCompilationCaching);
			RenderGraphResourceRegistry resources = this.m_Resources;
			int executionCount = this.m_ExecutionCount;
			this.m_ExecutionCount = executionCount + 1;
			resources.BeginRenderGraph(executionCount);
			if (this.m_DebugParameters.enableLogging)
			{
				this.m_FrameInformationLogger.Initialize(this.m_CurrentExecutionName);
			}
			this.m_DefaultResources.InitializeForRendering(this);
			this.m_RenderGraphContext.cmd = parameters.commandBuffer;
			this.m_RenderGraphContext.renderContext = parameters.scriptableRenderContext;
			this.m_RenderGraphContext.contextlessTesting = parameters.invalidContextForTesting;
			this.m_RenderGraphContext.renderGraphPool = this.m_RenderGraphPool;
			this.m_RenderGraphContext.defaultResources = this.m_DefaultResources;
			if (this.m_DebugParameters.immediateMode)
			{
				this.UpdateCurrentCompiledGraph(-1, true);
				this.LogFrameInformation();
				this.m_CurrentCompiledGraph.compiledPassInfos.Resize(this.m_CurrentCompiledGraph.compiledPassInfos.capacity, false);
				this.m_CurrentImmediatePassIndex = 0;
				for (int i = 0; i < 3; i++)
				{
					if (this.m_ImmediateModeResourceList[i] == null)
					{
						this.m_ImmediateModeResourceList[i] = new List<int>();
					}
					this.m_ImmediateModeResourceList[i].Clear();
				}
				this.m_Resources.BeginExecute(this.m_CurrentFrameIndex);
			}
		}

		public void EndRecordingAndExecute()
		{
			this.Execute();
		}

		public bool ResetGraphAndLogException(Exception e)
		{
			this.m_RenderGraphState = RenderGraphState.Idle;
			if (!this.m_RenderGraphContext.contextlessTesting)
			{
				Debug.LogError("Render Graph Execution error");
				if (!this.m_ExecutionExceptionWasRaised)
				{
					Debug.LogException(e);
				}
				this.m_ExecutionExceptionWasRaised = true;
			}
			return this.m_RenderGraphContext.contextlessTesting;
		}

		internal void Execute()
		{
			this.m_ExecutionExceptionWasRaised = false;
			this.m_RenderGraphState = RenderGraphState.Executing;
			try
			{
				if (!this.m_DebugParameters.immediateMode)
				{
					this.LogFrameInformation();
					int graphHash = 0;
					if (this.m_EnableCompilationCaching)
					{
						graphHash = this.ComputeGraphHash();
					}
					if (this.nativeRenderPassesEnabled)
					{
						this.CompileNativeRenderGraph(graphHash);
					}
					else
					{
						this.CompileRenderGraph(graphHash);
					}
					this.m_Resources.BeginExecute(this.m_CurrentFrameIndex);
					if (this.nativeRenderPassesEnabled)
					{
						this.ExecuteNativeRenderGraph();
					}
					else
					{
						this.ExecuteRenderGraph();
					}
					this.ClearGlobalBindings();
				}
			}
			catch (Exception e)
			{
				if (this.ResetGraphAndLogException(e))
				{
					throw;
				}
			}
			finally
			{
				if (this.m_DebugParameters.immediateMode)
				{
					this.ReleaseImmediateModeResources();
				}
				this.ClearCompiledGraph(this.m_CurrentCompiledGraph, this.m_EnableCompilationCaching);
				this.m_Resources.EndExecute();
				this.InvalidateContext();
				this.m_RenderGraphState = RenderGraphState.Idle;
			}
		}

		public void BeginProfilingSampler(ProfilingSampler sampler, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
		{
			if (sampler == null)
			{
				return;
			}
			RenderGraph.ProfilingScopePassData profilingScopePassData;
			using (RenderGraphBuilder renderGraphBuilder = this.AddRenderPass<RenderGraph.ProfilingScopePassData>("BeginProfile", out profilingScopePassData, null, file, line))
			{
				profilingScopePassData.sampler = sampler;
				renderGraphBuilder.AllowPassCulling(false);
				renderGraphBuilder.GenerateDebugData(false);
				renderGraphBuilder.SetRenderFunc<RenderGraph.ProfilingScopePassData>(delegate(RenderGraph.ProfilingScopePassData data, RenderGraphContext ctx)
				{
					data.sampler.Begin(ctx.cmd);
				});
			}
		}

		public void EndProfilingSampler(ProfilingSampler sampler, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
		{
			if (sampler == null)
			{
				return;
			}
			RenderGraph.ProfilingScopePassData profilingScopePassData;
			using (RenderGraphBuilder renderGraphBuilder = this.AddRenderPass<RenderGraph.ProfilingScopePassData>("EndProfile", out profilingScopePassData, null, file, line))
			{
				profilingScopePassData.sampler = sampler;
				renderGraphBuilder.AllowPassCulling(false);
				renderGraphBuilder.GenerateDebugData(false);
				renderGraphBuilder.SetRenderFunc<RenderGraph.ProfilingScopePassData>(delegate(RenderGraph.ProfilingScopePassData data, RenderGraphContext ctx)
				{
					data.sampler.End(ctx.cmd);
				});
			}
		}

		internal DynamicArray<RenderGraph.CompiledPassInfo> GetCompiledPassInfos()
		{
			return this.m_CurrentCompiledGraph.compiledPassInfos;
		}

		internal void ClearCurrentCompiledGraph()
		{
			this.ClearCompiledGraph(this.m_CurrentCompiledGraph, false);
		}

		private void ClearCompiledGraph(RenderGraph.CompiledGraph compiledGraph, bool useCompilationCaching)
		{
			this.ClearRenderPasses();
			this.m_Resources.Clear(this.m_ExecutionExceptionWasRaised);
			this.m_RendererLists.Clear();
			this.registeredGlobals.Clear();
			if (!useCompilationCaching && !this.nativeRenderPassesEnabled && compiledGraph != null)
			{
				compiledGraph.Clear();
			}
		}

		private void InvalidateContext()
		{
			this.m_RenderGraphContext.cmd = null;
			this.m_RenderGraphContext.renderGraphPool = null;
			this.m_RenderGraphContext.defaultResources = null;
		}

		internal void OnPassAdded(RenderGraphPass pass)
		{
			if (this.m_DebugParameters.immediateMode)
			{
				this.ExecutePassImmediately(pass);
			}
		}

		internal static event RenderGraph.OnGraphRegisteredDelegate onGraphRegistered;

		internal static event RenderGraph.OnGraphRegisteredDelegate onGraphUnregistered;

		internal static event RenderGraph.OnExecutionRegisteredDelegate onExecutionRegistered;

		internal static event RenderGraph.OnExecutionRegisteredDelegate onExecutionUnregistered;

		internal static event Action onDebugDataCaptured;

		internal int ComputeGraphHash()
		{
			int value;
			using (new ProfilingScope(ProfilingSampler.Get<RenderGraphProfileId>(RenderGraphProfileId.ComputeHashRenderGraph)))
			{
				HashFNV1A32 hashFNV1A = HashFNV1A32.Create();
				for (int i = 0; i < this.m_RenderPasses.Count; i++)
				{
					this.m_RenderPasses[i].ComputeHash(ref hashFNV1A, this.m_Resources);
				}
				value = hashFNV1A.value;
			}
			return value;
		}

		private void CountReferences()
		{
			DynamicArray<RenderGraph.CompiledPassInfo> compiledPassInfos = this.m_CurrentCompiledGraph.compiledPassInfos;
			DynamicArray<RenderGraph.CompiledResourceInfo>[] compiledResourcesInfos = this.m_CurrentCompiledGraph.compiledResourcesInfos;
			for (int i = 0; i < compiledPassInfos.size; i++)
			{
				RenderGraphPass renderGraphPass = this.m_RenderPasses[i];
				ref RenderGraph.CompiledPassInfo ptr = ref compiledPassInfos[i];
				for (int j = 0; j < 3; j++)
				{
					foreach (ResourceHandle resourceHandle in renderGraphPass.resourceReadLists[j])
					{
						ref RenderGraph.CompiledResourceInfo ptr2 = ref compiledResourcesInfos[j][resourceHandle.index];
						ptr2.imported = this.m_Resources.IsRenderGraphResourceImported(resourceHandle);
						ptr2.consumers.Add(i);
						ptr2.refCount++;
					}
					foreach (ResourceHandle resourceHandle2 in renderGraphPass.resourceWriteLists[j])
					{
						ref RenderGraph.CompiledResourceInfo ptr3 = ref compiledResourcesInfos[j][resourceHandle2.index];
						ptr3.imported = this.m_Resources.IsRenderGraphResourceImported(resourceHandle2);
						ptr3.producers.Add(i);
						ptr.hasSideEffect = ptr3.imported;
						ptr.refCount++;
					}
					foreach (ResourceHandle resourceHandle3 in renderGraphPass.transientResourceList[j])
					{
						ref RenderGraph.CompiledResourceInfo ptr4 = ref compiledResourcesInfos[j][resourceHandle3.index];
						ptr4.refCount++;
						ptr4.consumers.Add(i);
						ptr4.producers.Add(i);
					}
				}
			}
		}

		private unsafe void CullUnusedPasses()
		{
			if (this.m_DebugParameters.disablePassCulling)
			{
				if (this.m_DebugParameters.enableLogging)
				{
					this.m_FrameInformationLogger.LogLine("- Pass Culling Disabled -\n", Array.Empty<object>());
				}
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				DynamicArray<RenderGraph.CompiledResourceInfo> dynamicArray = this.m_CurrentCompiledGraph.compiledResourcesInfos[i];
				this.m_CullingStack.Clear();
				for (int j = 1; j < dynamicArray.size; j++)
				{
					if (dynamicArray[j].refCount == 0)
					{
						this.m_CullingStack.Push(j);
					}
				}
				while (this.m_CullingStack.Count != 0)
				{
					foreach (int index in dynamicArray[this.m_CullingStack.Pop()]->producers)
					{
						ref RenderGraph.CompiledPassInfo ptr = ref this.m_CurrentCompiledGraph.compiledPassInfos[index];
						RenderGraphPass renderGraphPass = this.m_RenderPasses[index];
						ptr.refCount--;
						if (ptr.refCount == 0 && !ptr.hasSideEffect && ptr.allowPassCulling)
						{
							ptr.culled = true;
							foreach (ResourceHandle resourceHandle in renderGraphPass.resourceReadLists[i])
							{
								ref RenderGraph.CompiledResourceInfo ptr2 = ref dynamicArray[resourceHandle.index];
								ptr2.refCount--;
								if (ptr2.refCount == 0)
								{
									this.m_CullingStack.Push(resourceHandle.index);
								}
							}
						}
					}
				}
			}
			this.LogCulledPasses();
		}

		private void UpdatePassSynchronization(ref RenderGraph.CompiledPassInfo currentPassInfo, ref RenderGraph.CompiledPassInfo producerPassInfo, int currentPassIndex, int lastProducer, ref int intLastSyncIndex)
		{
			currentPassInfo.syncToPassIndex = lastProducer;
			intLastSyncIndex = lastProducer;
			producerPassInfo.needGraphicsFence = true;
			if (producerPassInfo.syncFromPassIndex == -1)
			{
				producerPassInfo.syncFromPassIndex = currentPassIndex;
			}
		}

		private void UpdateResourceSynchronization(ref int lastGraphicsPipeSync, ref int lastComputePipeSync, int currentPassIndex, in RenderGraph.CompiledResourceInfo resource)
		{
			int latestProducerIndex = this.GetLatestProducerIndex(currentPassIndex, resource);
			if (latestProducerIndex != -1)
			{
				DynamicArray<RenderGraph.CompiledPassInfo> compiledPassInfos = this.m_CurrentCompiledGraph.compiledPassInfos;
				ref RenderGraph.CompiledPassInfo ptr = ref compiledPassInfos[currentPassIndex];
				if (this.m_CurrentCompiledGraph.compiledPassInfos[latestProducerIndex].enableAsyncCompute != ptr.enableAsyncCompute)
				{
					if (ptr.enableAsyncCompute)
					{
						if (latestProducerIndex > lastGraphicsPipeSync)
						{
							this.UpdatePassSynchronization(ref ptr, compiledPassInfos[latestProducerIndex], currentPassIndex, latestProducerIndex, ref lastGraphicsPipeSync);
							return;
						}
					}
					else if (latestProducerIndex > lastComputePipeSync)
					{
						this.UpdatePassSynchronization(ref ptr, compiledPassInfos[latestProducerIndex], currentPassIndex, latestProducerIndex, ref lastComputePipeSync);
					}
				}
			}
		}

		private int GetFirstValidConsumerIndex(int passIndex, in RenderGraph.CompiledResourceInfo info)
		{
			DynamicArray<RenderGraph.CompiledPassInfo> compiledPassInfos = this.m_CurrentCompiledGraph.compiledPassInfos;
			foreach (int num in info.consumers)
			{
				if (num > passIndex && !compiledPassInfos[num].culled)
				{
					return num;
				}
			}
			return -1;
		}

		private int FindTextureProducer(int consumerPass, in RenderGraph.CompiledResourceInfo info, out int index)
		{
			DynamicArray<RenderGraph.CompiledPassInfo> compiledPassInfos = this.m_CurrentCompiledGraph.compiledPassInfos;
			int result = 0;
			for (index = 0; index < info.producers.Count; index++)
			{
				int num = info.producers[index];
				if (!compiledPassInfos[num].culled)
				{
					return num;
				}
				if (num >= consumerPass)
				{
					return result;
				}
				result = num;
			}
			return result;
		}

		private unsafe int GetLatestProducerIndex(int passIndex, in RenderGraph.CompiledResourceInfo info)
		{
			int result = -1;
			DynamicArray<RenderGraph.CompiledPassInfo> compiledPassInfos = this.m_CurrentCompiledGraph.compiledPassInfos;
			foreach (int num in info.producers)
			{
				RenderGraph.CompiledPassInfo compiledPassInfo = *compiledPassInfos[num];
				if (num >= passIndex || compiledPassInfo.culled || compiledPassInfo.culledByRendererList)
				{
					return result;
				}
				result = num;
			}
			return result;
		}

		private int GetLatestValidReadIndex(in RenderGraph.CompiledResourceInfo info)
		{
			if (info.consumers.Count == 0)
			{
				return -1;
			}
			DynamicArray<RenderGraph.CompiledPassInfo> compiledPassInfos = this.m_CurrentCompiledGraph.compiledPassInfos;
			List<int> consumers = info.consumers;
			for (int i = consumers.Count - 1; i >= 0; i--)
			{
				if (!compiledPassInfos[consumers[i]].culled)
				{
					return consumers[i];
				}
			}
			return -1;
		}

		private int GetFirstValidWriteIndex(in RenderGraph.CompiledResourceInfo info)
		{
			if (info.producers.Count == 0)
			{
				return -1;
			}
			DynamicArray<RenderGraph.CompiledPassInfo> compiledPassInfos = this.m_CurrentCompiledGraph.compiledPassInfos;
			List<int> producers = info.producers;
			for (int i = 0; i < producers.Count; i++)
			{
				if (!compiledPassInfos[producers[i]].culled)
				{
					return producers[i];
				}
			}
			return -1;
		}

		private int GetLatestValidWriteIndex(in RenderGraph.CompiledResourceInfo info)
		{
			if (info.producers.Count == 0)
			{
				return -1;
			}
			DynamicArray<RenderGraph.CompiledPassInfo> compiledPassInfos = this.m_CurrentCompiledGraph.compiledPassInfos;
			List<int> producers = info.producers;
			for (int i = producers.Count - 1; i >= 0; i--)
			{
				if (!compiledPassInfos[producers[i]].culled)
				{
					return producers[i];
				}
			}
			return -1;
		}

		private void CreateRendererLists()
		{
			DynamicArray<RenderGraph.CompiledPassInfo> compiledPassInfos = this.m_CurrentCompiledGraph.compiledPassInfos;
			for (int i = 0; i < compiledPassInfos.size; i++)
			{
				ref RenderGraph.CompiledPassInfo ptr = ref compiledPassInfos[i];
				if (!ptr.culled)
				{
					this.m_RendererLists.AddRange(this.m_RenderPasses[ptr.index].usedRendererListList);
				}
			}
			this.m_Resources.CreateRendererLists(this.m_RendererLists, this.m_RenderGraphContext.renderContext, this.m_RendererListCulling);
		}

		internal bool GetImportedFallback(TextureDesc desc, out TextureHandle fallback)
		{
			fallback = TextureHandle.nullHandle;
			if (!desc.bindTextureMS)
			{
				if (desc.depthBufferBits != DepthBits.None)
				{
					fallback = this.defaultResources.whiteTexture;
				}
				else if (desc.clearColor == Color.black || desc.clearColor == default(Color))
				{
					if (desc.dimension == TextureXR.dimension)
					{
						fallback = this.defaultResources.blackTextureXR;
					}
					else if (desc.dimension == TextureDimension.Tex3D)
					{
						fallback = this.defaultResources.blackTexture3DXR;
					}
					else if (desc.dimension == TextureDimension.Tex2D)
					{
						fallback = this.defaultResources.blackTexture;
					}
				}
				else if (desc.clearColor == Color.white)
				{
					if (desc.dimension == TextureXR.dimension)
					{
						fallback = this.defaultResources.whiteTextureXR;
					}
					else if (desc.dimension == TextureDimension.Tex2D)
					{
						fallback = this.defaultResources.whiteTexture;
					}
				}
			}
			return fallback.IsValid();
		}

		private void AllocateCulledPassResources(ref RenderGraph.CompiledPassInfo passInfo)
		{
			int index = passInfo.index;
			RenderGraphPass renderGraphPass = this.m_RenderPasses[index];
			for (int i = 0; i < 3; i++)
			{
				DynamicArray<RenderGraph.CompiledResourceInfo> dynamicArray = this.m_CurrentCompiledGraph.compiledResourcesInfos[i];
				foreach (ResourceHandle resourceHandle in renderGraphPass.resourceWriteLists[i])
				{
					ref RenderGraph.CompiledResourceInfo ptr = ref dynamicArray[resourceHandle.index];
					int firstValidConsumerIndex = this.GetFirstValidConsumerIndex(index, ptr);
					int num2;
					int num = this.FindTextureProducer(firstValidConsumerIndex, ptr, out num2);
					if (firstValidConsumerIndex != -1 && index == num)
					{
						if (i == 0)
						{
							TextureResource textureResource = this.m_Resources.GetTextureResource(resourceHandle);
							TextureHandle textureHandle;
							if (!textureResource.desc.disableFallBackToImportedTexture && this.GetImportedFallback(textureResource.desc, out textureHandle))
							{
								ptr.imported = true;
								textureResource.imported = true;
								textureResource.graphicsResource = this.m_Resources.GetTexture(textureHandle);
								continue;
							}
							textureResource.desc.sizeMode = TextureSizeMode.Explicit;
							textureResource.desc.width = 1;
							textureResource.desc.height = 1;
							textureResource.desc.clearBuffer = true;
						}
						ptr.producers[num2 - 1] = firstValidConsumerIndex;
					}
				}
			}
		}

		private unsafe void UpdateResourceAllocationAndSynchronization()
		{
			int num = -1;
			int num2 = -1;
			DynamicArray<RenderGraph.CompiledPassInfo> compiledPassInfos = this.m_CurrentCompiledGraph.compiledPassInfos;
			DynamicArray<RenderGraph.CompiledResourceInfo>[] compiledResourcesInfos = this.m_CurrentCompiledGraph.compiledResourcesInfos;
			for (int i = 0; i < compiledPassInfos.size; i++)
			{
				ref RenderGraph.CompiledPassInfo ptr = ref compiledPassInfos[i];
				if (ptr.culledByRendererList)
				{
					this.AllocateCulledPassResources(ref ptr);
				}
				if (!ptr.culled)
				{
					RenderGraphPass renderGraphPass = this.m_RenderPasses[ptr.index];
					for (int j = 0; j < 3; j++)
					{
						DynamicArray<RenderGraph.CompiledResourceInfo> dynamicArray = compiledResourcesInfos[j];
						foreach (ResourceHandle resourceHandle in renderGraphPass.resourceReadLists[j])
						{
							this.UpdateResourceSynchronization(ref num, ref num2, i, dynamicArray[resourceHandle.index]);
						}
						foreach (ResourceHandle resourceHandle2 in renderGraphPass.resourceWriteLists[j])
						{
							this.UpdateResourceSynchronization(ref num, ref num2, i, dynamicArray[resourceHandle2.index]);
						}
					}
				}
			}
			for (int k = 0; k < 3; k++)
			{
				DynamicArray<RenderGraph.CompiledResourceInfo> dynamicArray2 = compiledResourcesInfos[k];
				for (int l = 1; l < dynamicArray2.size; l++)
				{
					RenderGraph.CompiledResourceInfo compiledResourceInfo = *dynamicArray2[l];
					bool flag = this.m_Resources.IsRenderGraphResourceShared((RenderGraphResourceType)k, l);
					bool flag2 = this.m_Resources.IsRenderGraphResourceForceReleased((RenderGraphResourceType)k, l);
					if (!compiledResourceInfo.imported || flag || flag2)
					{
						int firstValidWriteIndex = this.GetFirstValidWriteIndex(compiledResourceInfo);
						if (firstValidWriteIndex != -1)
						{
							compiledPassInfos[firstValidWriteIndex].resourceCreateList[k].Add(l);
						}
						int latestValidReadIndex = this.GetLatestValidReadIndex(compiledResourceInfo);
						int latestValidWriteIndex = this.GetLatestValidWriteIndex(compiledResourceInfo);
						int num3 = (firstValidWriteIndex != -1 || compiledResourceInfo.imported) ? Math.Max(latestValidWriteIndex, latestValidReadIndex) : -1;
						if (num3 != -1)
						{
							if (compiledPassInfos[num3].enableAsyncCompute)
							{
								int num4 = num3;
								int num5 = compiledPassInfos[num4].syncFromPassIndex;
								while (num5 == -1 && num4++ < compiledPassInfos.size - 1)
								{
									if (compiledPassInfos[num4].enableAsyncCompute)
									{
										num5 = compiledPassInfos[num4].syncFromPassIndex;
									}
								}
								if (num4 == compiledPassInfos.size)
								{
									if (!compiledPassInfos[num3].hasSideEffect)
									{
										RenderGraphPass renderGraphPass2 = this.m_RenderPasses[num3];
										string arg = "<unknown>";
										throw new InvalidOperationException(string.Format("{0} resource '{1}' in asynchronous pass '{2}' is missing synchronization on the graphics pipeline.", (RenderGraphResourceType)k, arg, renderGraphPass2.name));
									}
									num5 = num4;
								}
								int num6 = Math.Max(0, num5 - 1);
								while (compiledPassInfos[num6].culled)
								{
									num6 = Math.Max(0, num6 - 1);
								}
								compiledPassInfos[num6].resourceReleaseList[k].Add(l);
							}
							else
							{
								compiledPassInfos[num3].resourceReleaseList[k].Add(l);
							}
						}
						if (flag && (firstValidWriteIndex != -1 || num3 != -1))
						{
							this.m_Resources.UpdateSharedResourceLastFrameIndex(k, l);
						}
					}
				}
			}
		}

		private unsafe void UpdateAllSharedResourceLastFrameIndex()
		{
			for (int i = 0; i < 3; i++)
			{
				DynamicArray<RenderGraph.CompiledResourceInfo> dynamicArray = this.m_CurrentCompiledGraph.compiledResourcesInfos[i];
				int sharedResourceCount = this.m_Resources.GetSharedResourceCount((RenderGraphResourceType)i);
				for (int j = 1; j <= sharedResourceCount; j++)
				{
					RenderGraph.CompiledResourceInfo compiledResourceInfo = *dynamicArray[j];
					int latestValidReadIndex = this.GetLatestValidReadIndex(compiledResourceInfo);
					if (this.GetFirstValidWriteIndex(compiledResourceInfo) != -1 || latestValidReadIndex != -1)
					{
						this.m_Resources.UpdateSharedResourceLastFrameIndex(i, j);
					}
				}
			}
		}

		private bool AreRendererListsEmpty(List<RendererListHandle> rendererLists)
		{
			foreach (RendererListHandle rendererListHandle in rendererLists)
			{
				RendererList rendererList = this.m_Resources.GetRendererList(rendererListHandle);
				if (this.m_RenderGraphContext.renderContext.QueryRendererListStatus(rendererList) == RendererListStatus.kRendererListPopulated)
				{
					return false;
				}
			}
			return rendererLists.Count > 0;
		}

		private void TryCullPassAtIndex(int passIndex)
		{
			ref RenderGraph.CompiledPassInfo ptr = ref this.m_CurrentCompiledGraph.compiledPassInfos[passIndex];
			RenderGraphPass renderGraphPass = this.m_RenderPasses[passIndex];
			if (!ptr.culled && renderGraphPass.allowPassCulling && renderGraphPass.allowRendererListCulling && !ptr.hasSideEffect && this.AreRendererListsEmpty(renderGraphPass.usedRendererListList))
			{
				ptr.culled = (ptr.culledByRendererList = true);
			}
		}

		private unsafe void CullRendererLists()
		{
			DynamicArray<RenderGraph.CompiledPassInfo> compiledPassInfos = this.m_CurrentCompiledGraph.compiledPassInfos;
			for (int i = 0; i < compiledPassInfos.size; i++)
			{
				RenderGraph.CompiledPassInfo compiledPassInfo = *compiledPassInfos[i];
				if (!compiledPassInfo.culled && !compiledPassInfo.hasSideEffect && this.m_RenderPasses[i].usedRendererListList.Count > 0)
				{
					this.TryCullPassAtIndex(i);
				}
			}
		}

		private bool UpdateCurrentCompiledGraph(int graphHash, bool forceNoCaching = false)
		{
			bool result = false;
			if (this.m_EnableCompilationCaching && !forceNoCaching)
			{
				result = this.m_CompilationCache.GetCompilationCache(graphHash, this.m_ExecutionCount, out this.m_CurrentCompiledGraph);
			}
			else
			{
				this.m_CurrentCompiledGraph = this.m_DefaultCompiledGraph;
			}
			return result;
		}

		internal void CompileRenderGraph(int graphHash)
		{
			using (new ProfilingScope(this.m_RenderGraphContext.cmd, ProfilingSampler.Get<RenderGraphProfileId>(RenderGraphProfileId.CompileRenderGraph)))
			{
				bool flag = this.UpdateCurrentCompiledGraph(graphHash, false);
				if (!flag)
				{
					this.m_CurrentCompiledGraph.Clear();
					this.m_CurrentCompiledGraph.InitializeCompilationData(this.m_RenderPasses, this.m_Resources);
					this.CountReferences();
					this.CullUnusedPasses();
				}
				this.CreateRendererLists();
				if (!flag)
				{
					if (this.m_RendererListCulling)
					{
						this.CullRendererLists();
					}
					this.UpdateResourceAllocationAndSynchronization();
				}
				else
				{
					this.UpdateAllSharedResourceLastFrameIndex();
				}
				this.LogRendererListsCreation();
			}
		}

		private ref RenderGraph.CompiledPassInfo CompilePassImmediatly(RenderGraphPass pass)
		{
			DynamicArray<RenderGraph.CompiledPassInfo> compiledPassInfos = this.m_CurrentCompiledGraph.compiledPassInfos;
			if (this.m_CurrentImmediatePassIndex >= compiledPassInfos.size)
			{
				compiledPassInfos.Resize(compiledPassInfos.size * 2, false);
			}
			DynamicArray<RenderGraph.CompiledPassInfo> dynamicArray = compiledPassInfos;
			int currentImmediatePassIndex = this.m_CurrentImmediatePassIndex;
			this.m_CurrentImmediatePassIndex = currentImmediatePassIndex + 1;
			ref RenderGraph.CompiledPassInfo ptr = ref dynamicArray[currentImmediatePassIndex];
			ptr.Reset(pass, this.m_CurrentImmediatePassIndex - 1);
			ptr.enableAsyncCompute = false;
			for (int i = 0; i < 3; i++)
			{
				foreach (ResourceHandle resourceHandle in pass.transientResourceList[i])
				{
					ptr.resourceCreateList[i].Add(resourceHandle.index);
					ptr.resourceReleaseList[i].Add(resourceHandle.index);
				}
				foreach (ResourceHandle item in pass.resourceWriteLists[i])
				{
					if (!pass.transientResourceList[i].Contains(item) && !this.m_Resources.IsGraphicsResourceCreated(item))
					{
						ptr.resourceCreateList[i].Add(item.index);
						this.m_ImmediateModeResourceList[i].Add(item.index);
					}
				}
				foreach (ResourceHandle resourceHandle2 in pass.resourceReadLists[i])
				{
				}
			}
			foreach (RendererListHandle item2 in pass.usedRendererListList)
			{
				if (!this.m_Resources.IsRendererListCreated(item2))
				{
					this.m_RendererLists.Add(item2);
				}
			}
			this.m_Resources.CreateRendererLists(this.m_RendererLists, this.m_RenderGraphContext.renderContext, false);
			this.m_RendererLists.Clear();
			return ref ptr;
		}

		private void ExecutePassImmediately(RenderGraphPass pass)
		{
			this.ExecuteCompiledPass(this.CompilePassImmediatly(pass));
		}

		private void ExecuteCompiledPass(ref RenderGraph.CompiledPassInfo passInfo)
		{
			if (passInfo.culled)
			{
				return;
			}
			RenderGraphPass renderGraphPass = this.m_RenderPasses[passInfo.index];
			if (!renderGraphPass.HasRenderFunc())
			{
				throw new InvalidOperationException("RenderPass " + renderGraphPass.name + " was not provided with an execute function.");
			}
			try
			{
				using (new ProfilingScope(this.m_RenderGraphContext.cmd, renderGraphPass.customSampler))
				{
					this.LogRenderPassBegin(passInfo);
					using (new RenderGraphLogIndent(this.m_FrameInformationLogger, 1))
					{
						this.m_RenderGraphContext.executingPass = renderGraphPass;
						this.PreRenderPassExecute(passInfo, renderGraphPass, this.m_RenderGraphContext);
						renderGraphPass.Execute(this.m_RenderGraphContext);
						this.PostRenderPassExecute(ref passInfo, renderGraphPass, this.m_RenderGraphContext);
					}
				}
			}
			catch (Exception exception)
			{
				if (!this.m_RenderGraphContext.contextlessTesting)
				{
					this.m_ExecutionExceptionWasRaised = true;
					Debug.LogError(string.Format("Render Graph execution error at pass '{0}' ({1})", renderGraphPass.name, passInfo.index));
					Debug.LogException(exception);
				}
				throw;
			}
		}

		private void ExecuteRenderGraph()
		{
			using (new ProfilingScope(this.m_RenderGraphContext.cmd, ProfilingSampler.Get<RenderGraphProfileId>(RenderGraphProfileId.ExecuteRenderGraph)))
			{
				DynamicArray<RenderGraph.CompiledPassInfo> compiledPassInfos = this.m_CurrentCompiledGraph.compiledPassInfos;
				for (int i = 0; i < compiledPassInfos.size; i++)
				{
					this.ExecuteCompiledPass(compiledPassInfos[i]);
				}
			}
		}

		private void PreRenderPassSetRenderTargets(in RenderGraph.CompiledPassInfo passInfo, RenderGraphPass pass, InternalRenderGraphContext rgContext)
		{
			if (passInfo.hasShadingRateImage)
			{
				CommandBuffer cmd = rgContext.cmd;
				RenderGraphResourceRegistry resources = this.m_Resources;
				TextureAccess textureAccess = pass.shadingRateAccess;
				RenderTargetIdentifier renderTargetIdentifier = resources.GetTexture(textureAccess.textureHandle);
				CoreUtils.SetShadingRateImage(cmd, renderTargetIdentifier);
			}
			bool flag = pass.depthAccess.textureHandle.IsValid();
			if (!flag && pass.colorBufferMaxIndex == -1)
			{
				return;
			}
			TextureAccess[] colorBufferAccess = pass.colorBufferAccess;
			if (pass.colorBufferMaxIndex > 0)
			{
				RenderTargetIdentifier[] array = this.m_TempMRTArrays[pass.colorBufferMaxIndex];
				for (int i = 0; i <= pass.colorBufferMaxIndex; i++)
				{
					array[i] = this.m_Resources.GetTexture(colorBufferAccess[i].textureHandle);
				}
				if (flag)
				{
					CommandBuffer cmd2 = rgContext.cmd;
					RenderTargetIdentifier[] colorBuffers = array;
					RenderGraphResourceRegistry resources2 = this.m_Resources;
					TextureAccess textureAccess = pass.depthAccess;
					CoreUtils.SetRenderTarget(cmd2, colorBuffers, resources2.GetTexture(textureAccess.textureHandle));
					return;
				}
				throw new InvalidOperationException("Setting MRTs without a depth buffer is not supported.");
			}
			else if (flag)
			{
				TextureAccess textureAccess;
				if (pass.colorBufferMaxIndex > -1)
				{
					CommandBuffer cmd3 = rgContext.cmd;
					RTHandle texture = this.m_Resources.GetTexture(pass.colorBufferAccess[0].textureHandle);
					RenderGraphResourceRegistry resources3 = this.m_Resources;
					textureAccess = pass.depthAccess;
					CoreUtils.SetRenderTarget(cmd3, texture, resources3.GetTexture(textureAccess.textureHandle), 0, CubemapFace.Unknown, -1);
					return;
				}
				CommandBuffer cmd4 = rgContext.cmd;
				RenderGraphResourceRegistry resources4 = this.m_Resources;
				textureAccess = pass.depthAccess;
				CoreUtils.SetRenderTarget(cmd4, resources4.GetTexture(textureAccess.textureHandle), ClearFlag.None, 0, CubemapFace.Unknown, -1);
				return;
			}
			else
			{
				if (pass.colorBufferAccess[0].textureHandle.IsValid())
				{
					CoreUtils.SetRenderTarget(rgContext.cmd, this.m_Resources.GetTexture(pass.colorBufferAccess[0].textureHandle), ClearFlag.None, 0, CubemapFace.Unknown, -1);
					return;
				}
				throw new InvalidOperationException("Neither Depth nor color render targets are correctly setup at pass " + pass.name + ".");
			}
		}

		private void PreRenderPassExecute(in RenderGraph.CompiledPassInfo passInfo, RenderGraphPass pass, InternalRenderGraphContext rgContext)
		{
			this.m_PreviousCommandBuffer = rgContext.cmd;
			bool flag = false;
			for (int i = 0; i < 3; i++)
			{
				foreach (int index in passInfo.resourceCreateList[i])
				{
					flag |= this.m_Resources.CreatePooledResource(rgContext, i, index);
				}
			}
			if (passInfo.enableFoveatedRasterization)
			{
				rgContext.cmd.SetFoveatedRenderingMode(FoveatedRenderingMode.Enabled);
			}
			if (passInfo.hasShadingRateStates)
			{
				rgContext.cmd.SetShadingRateFragmentSize(pass.shadingRateFragmentSize);
				rgContext.cmd.SetShadingRateCombiner(ShadingRateCombinerStage.Primitive, pass.primitiveShadingRateCombiner);
				rgContext.cmd.SetShadingRateCombiner(ShadingRateCombinerStage.Fragment, pass.fragmentShadingRateCombiner);
			}
			this.PreRenderPassSetRenderTargets(passInfo, pass, rgContext);
			if (passInfo.enableAsyncCompute)
			{
				GraphicsFence fence = default(GraphicsFence);
				if (flag)
				{
					fence = rgContext.cmd.CreateGraphicsFence(GraphicsFenceType.AsyncQueueSynchronisation, SynchronisationStageFlags.AllGPUOperations);
				}
				if (!rgContext.contextlessTesting)
				{
					rgContext.renderContext.ExecuteCommandBuffer(rgContext.cmd);
				}
				rgContext.cmd.Clear();
				CommandBuffer commandBuffer = CommandBufferPool.Get(pass.name);
				commandBuffer.SetExecutionFlags(CommandBufferExecutionFlags.AsyncCompute);
				rgContext.cmd = commandBuffer;
				if (flag)
				{
					rgContext.cmd.WaitOnAsyncGraphicsFence(fence);
				}
			}
			if (passInfo.syncToPassIndex != -1)
			{
				rgContext.cmd.WaitOnAsyncGraphicsFence(this.m_CurrentCompiledGraph.compiledPassInfos[passInfo.syncToPassIndex].fence);
			}
		}

		private void PostRenderPassExecute(ref RenderGraph.CompiledPassInfo passInfo, RenderGraphPass pass, InternalRenderGraphContext rgContext)
		{
			if (passInfo.hasShadingRateStates || passInfo.hasShadingRateImage)
			{
				rgContext.cmd.ResetShadingRate();
			}
			foreach (ValueTuple<TextureHandle, int> valueTuple in pass.setGlobalsList)
			{
				rgContext.cmd.SetGlobalTexture(valueTuple.Item2, valueTuple.Item1);
			}
			if (passInfo.needGraphicsFence)
			{
				passInfo.fence = rgContext.cmd.CreateAsyncGraphicsFence();
			}
			if (passInfo.enableAsyncCompute)
			{
				rgContext.renderContext.ExecuteCommandBufferAsync(rgContext.cmd, ComputeQueueType.Background);
				CommandBufferPool.Release(rgContext.cmd);
				rgContext.cmd = this.m_PreviousCommandBuffer;
			}
			if (passInfo.enableFoveatedRasterization)
			{
				rgContext.cmd.SetFoveatedRenderingMode(FoveatedRenderingMode.Disabled);
			}
			this.m_RenderGraphPool.ReleaseAllTempAlloc();
			for (int i = 0; i < 3; i++)
			{
				foreach (int index in passInfo.resourceReleaseList[i])
				{
					this.m_Resources.ReleasePooledResource(rgContext, i, index);
				}
			}
		}

		private void ClearRenderPasses()
		{
			foreach (RenderGraphPass renderGraphPass in this.m_RenderPasses)
			{
				renderGraphPass.Release(this.m_RenderGraphPool);
			}
			this.m_RenderPasses.Clear();
		}

		private void ReleaseImmediateModeResources()
		{
			for (int i = 0; i < 3; i++)
			{
				foreach (int index in this.m_ImmediateModeResourceList[i])
				{
					this.m_Resources.ReleasePooledResource(this.m_RenderGraphContext, i, index);
				}
			}
		}

		private void LogFrameInformation()
		{
			if (this.m_DebugParameters.enableLogging)
			{
				this.m_FrameInformationLogger.LogLine("==== Render Graph Frame Information Log (" + this.m_CurrentExecutionName + ") ====", Array.Empty<object>());
				if (!this.m_DebugParameters.immediateMode)
				{
					this.m_FrameInformationLogger.LogLine("Number of passes declared: {0}\n", new object[]
					{
						this.m_RenderPasses.Count
					});
				}
			}
		}

		private void LogRendererListsCreation()
		{
			if (this.m_DebugParameters.enableLogging)
			{
				this.m_FrameInformationLogger.LogLine("Number of renderer lists created: {0}\n", new object[]
				{
					this.m_RendererLists.Count
				});
			}
		}

		private void LogRenderPassBegin(in RenderGraph.CompiledPassInfo passInfo)
		{
			if (this.m_DebugParameters.enableLogging)
			{
				RenderGraphPass renderGraphPass = this.m_RenderPasses[passInfo.index];
				this.m_FrameInformationLogger.LogLine("[{0}][{1}] \"{2}\"", new object[]
				{
					renderGraphPass.index,
					renderGraphPass.enableAsyncCompute ? "Compute" : "Graphics",
					renderGraphPass.name
				});
				using (new RenderGraphLogIndent(this.m_FrameInformationLogger, 1))
				{
					if (passInfo.syncToPassIndex != -1)
					{
						this.m_FrameInformationLogger.LogLine("Synchronize with [{0}]", new object[]
						{
							passInfo.syncToPassIndex
						});
					}
				}
			}
		}

		private void LogCulledPasses()
		{
			if (this.m_DebugParameters.enableLogging)
			{
				this.m_FrameInformationLogger.LogLine("Pass Culling Report:", Array.Empty<object>());
				using (new RenderGraphLogIndent(this.m_FrameInformationLogger, 1))
				{
					DynamicArray<RenderGraph.CompiledPassInfo> compiledPassInfos = this.m_CurrentCompiledGraph.compiledPassInfos;
					for (int i = 0; i < compiledPassInfos.size; i++)
					{
						if (compiledPassInfos[i].culled)
						{
							RenderGraphPass renderGraphPass = this.m_RenderPasses[i];
							this.m_FrameInformationLogger.LogLine("[{0}] {1}", new object[]
							{
								renderGraphPass.index,
								renderGraphPass.name
							});
						}
					}
					this.m_FrameInformationLogger.LogLine("\n", Array.Empty<object>());
				}
			}
		}

		private ProfilingSampler GetDefaultProfilingSampler(string name)
		{
			return null;
		}

		private void UpdateImportedResourceLifeTime(ref RenderGraph.DebugData.ResourceData data, List<int> passList)
		{
			foreach (int num in passList)
			{
				if (data.creationPassIndex == -1)
				{
					data.creationPassIndex = num;
				}
				else
				{
					data.creationPassIndex = Math.Min(data.creationPassIndex, num);
				}
				if (data.releasePassIndex == -1)
				{
					data.releasePassIndex = num;
				}
				else
				{
					data.releasePassIndex = Math.Max(data.releasePassIndex, num);
				}
			}
		}

		private void GenerateDebugData()
		{
			if (this.m_ExecutionExceptionWasRaised)
			{
				return;
			}
			if (!RenderGraph.isRenderGraphViewerActive)
			{
				this.CleanupDebugData();
				return;
			}
			RenderGraph.DebugData debugData;
			if (!this.m_DebugData.TryGetValue(this.m_CurrentExecutionName, out debugData))
			{
				RenderGraph.OnExecutionRegisteredDelegate onExecutionRegisteredDelegate = RenderGraph.onExecutionRegistered;
				if (onExecutionRegisteredDelegate != null)
				{
					onExecutionRegisteredDelegate(this, this.m_CurrentExecutionName);
				}
				debugData = new RenderGraph.DebugData();
				this.m_DebugData.Add(this.m_CurrentExecutionName, debugData);
				return;
			}
			if (this.m_CaptureDebugDataForExecution == null || !this.m_CaptureDebugDataForExecution.Equals(this.m_CurrentExecutionName))
			{
				return;
			}
			debugData.Clear();
			if (this.nativeRenderPassesEnabled)
			{
				this.nativeCompiler.GenerateNativeCompilerDebugData(ref debugData);
			}
			else
			{
				this.GenerateCompilerDebugData(ref debugData);
			}
			Action action = RenderGraph.onDebugDataCaptured;
			if (action != null)
			{
				action();
			}
			this.m_CaptureDebugDataForExecution = null;
		}

		private void GenerateCompilerDebugData(ref RenderGraph.DebugData debugData)
		{
			DynamicArray<RenderGraph.CompiledPassInfo> compiledPassInfos = this.m_CurrentCompiledGraph.compiledPassInfos;
			DynamicArray<RenderGraph.CompiledResourceInfo>[] compiledResourcesInfos = this.m_CurrentCompiledGraph.compiledResourcesInfos;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < compiledResourcesInfos[i].size; j++)
				{
					ref RenderGraph.CompiledResourceInfo ptr = ref compiledResourcesInfos[i][j];
					RenderGraph.DebugData.ResourceData resourceData = default(RenderGraph.DebugData.ResourceData);
					if (j != 0)
					{
						string renderGraphResourceName = this.m_Resources.GetRenderGraphResourceName((RenderGraphResourceType)i, j);
						resourceData.name = ((!string.IsNullOrEmpty(renderGraphResourceName)) ? renderGraphResourceName : "(unnamed)");
						resourceData.imported = this.m_Resources.IsRenderGraphResourceImported((RenderGraphResourceType)i, j);
					}
					else
					{
						resourceData.name = "<null>";
						resourceData.imported = true;
					}
					resourceData.creationPassIndex = -1;
					resourceData.releasePassIndex = -1;
					RenderGraphResourceType renderGraphResourceType = (RenderGraphResourceType)i;
					ResourceHandle resourceHandle = new ResourceHandle(j, renderGraphResourceType, false);
					if (j != 0 && resourceHandle.IsValid())
					{
						if (renderGraphResourceType == RenderGraphResourceType.Texture)
						{
							RenderTargetInfo renderTargetInfo;
							this.m_Resources.GetRenderTargetInfo(resourceHandle, out renderTargetInfo);
							resourceData.textureData = new RenderGraph.DebugData.TextureResourceData
							{
								width = renderTargetInfo.width,
								height = renderTargetInfo.height,
								depth = renderTargetInfo.volumeDepth,
								samples = renderTargetInfo.msaaSamples,
								format = renderTargetInfo.format,
								bindMS = renderTargetInfo.bindMS
							};
						}
						else if (renderGraphResourceType == RenderGraphResourceType.Buffer)
						{
							BufferDesc bufferResourceDesc = this.m_Resources.GetBufferResourceDesc(resourceHandle, true);
							resourceData.bufferData = new RenderGraph.DebugData.BufferResourceData
							{
								count = bufferResourceDesc.count,
								stride = bufferResourceDesc.stride,
								target = bufferResourceDesc.target,
								usage = bufferResourceDesc.usageFlags
							};
						}
					}
					resourceData.consumerList = new List<int>(ptr.consumers);
					resourceData.producerList = new List<int>(ptr.producers);
					if (resourceData.imported)
					{
						this.UpdateImportedResourceLifeTime(ref resourceData, resourceData.consumerList);
						this.UpdateImportedResourceLifeTime(ref resourceData, resourceData.producerList);
					}
					debugData.resourceLists[i].Add(resourceData);
				}
			}
			for (int k = 0; k < compiledPassInfos.size; k++)
			{
				ref RenderGraph.CompiledPassInfo ptr2 = ref compiledPassInfos[k];
				RenderGraphPass renderGraphPass = this.m_RenderPasses[ptr2.index];
				RenderGraph.DebugData.PassData passData = default(RenderGraph.DebugData.PassData);
				passData.name = renderGraphPass.name;
				passData.type = renderGraphPass.type;
				passData.culled = ptr2.culled;
				passData.async = ptr2.enableAsyncCompute;
				passData.generateDebugData = renderGraphPass.generateDebugData;
				passData.resourceReadLists = new List<int>[3];
				passData.resourceWriteLists = new List<int>[3];
				passData.syncFromPassIndex = ptr2.syncFromPassIndex;
				passData.syncToPassIndex = ptr2.syncToPassIndex;
				RenderGraph.DebugData.s_PassScriptMetadata.TryGetValue(renderGraphPass, out passData.scriptInfo);
				for (int l = 0; l < 3; l++)
				{
					passData.resourceReadLists[l] = new List<int>();
					passData.resourceWriteLists[l] = new List<int>();
					foreach (ResourceHandle resourceHandle2 in renderGraphPass.resourceReadLists[l])
					{
						passData.resourceReadLists[l].Add(resourceHandle2.index);
					}
					foreach (ResourceHandle resourceHandle3 in renderGraphPass.resourceWriteLists[l])
					{
						passData.resourceWriteLists[l].Add(resourceHandle3.index);
					}
					foreach (int index in ptr2.resourceCreateList[l])
					{
						RenderGraph.DebugData.ResourceData resourceData2 = debugData.resourceLists[l][index];
						if (!resourceData2.imported)
						{
							resourceData2.creationPassIndex = k;
							debugData.resourceLists[l][index] = resourceData2;
						}
					}
					foreach (int index2 in ptr2.resourceReleaseList[l])
					{
						RenderGraph.DebugData.ResourceData resourceData3 = debugData.resourceLists[l][index2];
						if (!resourceData3.imported)
						{
							resourceData3.releasePassIndex = k;
							debugData.resourceLists[l][index2] = resourceData3;
						}
					}
				}
				debugData.passList.Add(passData);
			}
		}

		private void CleanupDebugData()
		{
			foreach (KeyValuePair<string, RenderGraph.DebugData> keyValuePair in this.m_DebugData)
			{
				RenderGraph.OnExecutionRegisteredDelegate onExecutionRegisteredDelegate = RenderGraph.onExecutionUnregistered;
				if (onExecutionRegisteredDelegate != null)
				{
					onExecutionRegisteredDelegate(this, keyValuePair.Key);
				}
			}
			this.m_DebugData.Clear();
		}

		internal void SetGlobal(TextureHandle h, int globalPropertyId)
		{
			if (!h.IsValid())
			{
				throw new ArgumentException("Attempting to register an invalid texture handle as a global");
			}
			this.registeredGlobals[globalPropertyId] = h;
		}

		internal bool IsGlobal(int globalPropertyId)
		{
			return this.registeredGlobals.ContainsKey(globalPropertyId);
		}

		internal Dictionary<int, TextureHandle>.ValueCollection AllGlobals()
		{
			return this.registeredGlobals.Values;
		}

		internal TextureHandle GetGlobal(int globalPropertyId)
		{
			TextureHandle result;
			this.registeredGlobals.TryGetValue(globalPropertyId, out result);
			return result;
		}

		internal void ClearGlobalBindings()
		{
			foreach (KeyValuePair<int, TextureHandle> keyValuePair in this.registeredGlobals)
			{
				this.m_RenderGraphContext.cmd.SetGlobalTexture(keyValuePair.Key, this.defaultResources.blackTexture);
			}
		}

		[Conditional("UNITY_EDITOR")]
		private void AddPassDebugMetadata(RenderGraphPass renderPass, string file, int line)
		{
			if (this.m_CaptureDebugDataForExecution == null)
			{
				return;
			}
			for (int i = 0; i < this.k_PassNameDebugIgnoreList.Length; i++)
			{
				if (renderPass.name == this.k_PassNameDebugIgnoreList[i])
				{
					return;
				}
			}
			RenderGraph.DebugData.s_PassScriptMetadata.TryAdd(renderPass, new RenderGraph.DebugData.PassScriptInfo
			{
				filePath = file,
				line = line
			});
		}

		[Conditional("UNITY_EDITOR")]
		private void ClearPassDebugMetadata()
		{
			RenderGraph.DebugData.s_PassScriptMetadata.Clear();
		}

		private NativePassCompiler nativeCompiler;

		public static readonly int kMaxMRTCount = 8;

		internal RenderGraphResourceRegistry m_Resources;

		private RenderGraphObjectPool m_RenderGraphPool = new RenderGraphObjectPool();

		private RenderGraphBuilders m_builderInstance = new RenderGraphBuilders();

		internal List<RenderGraphPass> m_RenderPasses = new List<RenderGraphPass>(64);

		private List<RendererListHandle> m_RendererLists = new List<RendererListHandle>(32);

		private RenderGraphDebugParams m_DebugParameters = new RenderGraphDebugParams();

		private RenderGraphLogger m_FrameInformationLogger = new RenderGraphLogger();

		private RenderGraphDefaultResources m_DefaultResources = new RenderGraphDefaultResources();

		private Dictionary<int, ProfilingSampler> m_DefaultProfilingSamplers = new Dictionary<int, ProfilingSampler>();

		private InternalRenderGraphContext m_RenderGraphContext = new InternalRenderGraphContext();

		private CommandBuffer m_PreviousCommandBuffer;

		private List<int>[] m_ImmediateModeResourceList = new List<int>[3];

		private RenderGraphCompilationCache m_CompilationCache;

		private RenderTargetIdentifier[][] m_TempMRTArrays;

		private Stack<int> m_CullingStack = new Stack<int>();

		private string m_CurrentExecutionName;

		private int m_ExecutionCount;

		private int m_CurrentFrameIndex;

		private int m_CurrentImmediatePassIndex;

		private bool m_ExecutionExceptionWasRaised;

		private bool m_RendererListCulling;

		private bool m_EnableCompilationCaching;

		private RenderGraph.CompiledGraph m_DefaultCompiledGraph = new RenderGraph.CompiledGraph();

		private RenderGraph.CompiledGraph m_CurrentCompiledGraph;

		private string m_CaptureDebugDataForExecution;

		private RenderGraphState m_RenderGraphState;

		private Dictionary<string, RenderGraph.DebugData> m_DebugData = new Dictionary<string, RenderGraph.DebugData>();

		private static List<RenderGraph> s_RegisteredGraphs = new List<RenderGraph>();

		private const string k_BeginProfilingSamplerPassName = "BeginProfile";

		private const string k_EndProfilingSamplerPassName = "EndProfile";

		private Dictionary<int, TextureHandle> registeredGlobals = new Dictionary<int, TextureHandle>();

		private readonly string[] k_PassNameDebugIgnoreList = new string[]
		{
			"BeginProfile",
			"EndProfile"
		};

		internal struct CompiledResourceInfo
		{
			public void Reset()
			{
				if (this.producers == null)
				{
					this.producers = new List<int>();
				}
				if (this.consumers == null)
				{
					this.consumers = new List<int>();
				}
				this.producers.Clear();
				this.consumers.Clear();
				this.refCount = 0;
				this.imported = false;
			}

			public List<int> producers;

			public List<int> consumers;

			public int refCount;

			public bool imported;
		}

		[DebuggerDisplay("RenderPass: {name} (Index:{index} Async:{enableAsyncCompute})")]
		internal struct CompiledPassInfo
		{
			public void Reset(RenderGraphPass pass, int index)
			{
				this.name = pass.name;
				this.index = index;
				this.enableAsyncCompute = pass.enableAsyncCompute;
				this.allowPassCulling = pass.allowPassCulling;
				this.enableFoveatedRasterization = pass.enableFoveatedRasterization;
				this.hasShadingRateImage = (pass.hasShadingRateImage && !pass.enableFoveatedRasterization);
				this.hasShadingRateStates = (pass.hasShadingRateStates && !pass.enableFoveatedRasterization);
				if (this.resourceCreateList == null)
				{
					this.resourceCreateList = new List<int>[3];
					this.resourceReleaseList = new List<int>[3];
					for (int i = 0; i < 3; i++)
					{
						this.resourceCreateList[i] = new List<int>();
						this.resourceReleaseList[i] = new List<int>();
					}
				}
				for (int j = 0; j < 3; j++)
				{
					this.resourceCreateList[j].Clear();
					this.resourceReleaseList[j].Clear();
				}
				this.refCount = 0;
				this.culled = false;
				this.culledByRendererList = false;
				this.hasSideEffect = false;
				this.syncToPassIndex = -1;
				this.syncFromPassIndex = -1;
				this.needGraphicsFence = false;
			}

			public string name;

			public int index;

			public List<int>[] resourceCreateList;

			public List<int>[] resourceReleaseList;

			public GraphicsFence fence;

			public int refCount;

			public int syncToPassIndex;

			public int syncFromPassIndex;

			public bool enableAsyncCompute;

			public bool allowPassCulling;

			public bool needGraphicsFence;

			public bool culled;

			public bool culledByRendererList;

			public bool hasSideEffect;

			public bool enableFoveatedRasterization;

			public bool hasShadingRateImage;

			public bool hasShadingRateStates;
		}

		internal interface ICompiledGraph
		{
			void Clear();
		}

		internal class CompiledGraph : RenderGraph.ICompiledGraph
		{
			public CompiledGraph()
			{
				for (int i = 0; i < 3; i++)
				{
					this.compiledResourcesInfos[i] = new DynamicArray<RenderGraph.CompiledResourceInfo>();
				}
			}

			public void Clear()
			{
				for (int i = 0; i < 3; i++)
				{
					this.compiledResourcesInfos[i].Clear();
				}
				this.compiledPassInfos.Clear();
			}

			private void InitResourceInfosData(DynamicArray<RenderGraph.CompiledResourceInfo> resourceInfos, int count)
			{
				resourceInfos.Resize(count, false);
				for (int i = 0; i < resourceInfos.size; i++)
				{
					resourceInfos[i].Reset();
				}
			}

			public void InitializeCompilationData(List<RenderGraphPass> passes, RenderGraphResourceRegistry resources)
			{
				this.InitResourceInfosData(this.compiledResourcesInfos[0], resources.GetTextureResourceCount());
				this.InitResourceInfosData(this.compiledResourcesInfos[1], resources.GetBufferResourceCount());
				this.InitResourceInfosData(this.compiledResourcesInfos[2], resources.GetRayTracingAccelerationStructureResourceCount());
				this.compiledPassInfos.Resize(passes.Count, false);
				for (int i = 0; i < this.compiledPassInfos.size; i++)
				{
					this.compiledPassInfos[i].Reset(passes[i], i);
				}
			}

			public DynamicArray<RenderGraph.CompiledResourceInfo>[] compiledResourcesInfos = new DynamicArray<RenderGraph.CompiledResourceInfo>[3];

			public DynamicArray<RenderGraph.CompiledPassInfo> compiledPassInfos = new DynamicArray<RenderGraph.CompiledPassInfo>();

			public int lastExecutionFrame;
		}

		private class ProfilingScopePassData
		{
			public ProfilingSampler sampler;
		}

		internal delegate void OnGraphRegisteredDelegate(RenderGraph graph);

		internal delegate void OnExecutionRegisteredDelegate(RenderGraph graph, string executionName);

		internal class DebugData
		{
			public DebugData()
			{
				for (int i = 0; i < 3; i++)
				{
					this.resourceLists[i] = new List<RenderGraph.DebugData.ResourceData>();
				}
			}

			public void Clear()
			{
				this.passList.Clear();
				for (int i = 0; i < 3; i++)
				{
					this.resourceLists[i].Clear();
				}
			}

			public readonly List<RenderGraph.DebugData.PassData> passList = new List<RenderGraph.DebugData.PassData>();

			public readonly List<RenderGraph.DebugData.ResourceData>[] resourceLists = new List<RenderGraph.DebugData.ResourceData>[3];

			public bool isNRPCompiler;

			internal static readonly Dictionary<object, RenderGraph.DebugData.PassScriptInfo> s_PassScriptMetadata = new Dictionary<object, RenderGraph.DebugData.PassScriptInfo>();

			[DebuggerDisplay("PassDebug: {name}")]
			public struct PassData
			{
				public string name;

				public RenderGraphPassType type;

				public List<int>[] resourceReadLists;

				public List<int>[] resourceWriteLists;

				public bool culled;

				public bool async;

				public int nativeSubPassIndex;

				public int syncToPassIndex;

				public int syncFromPassIndex;

				public bool generateDebugData;

				public RenderGraph.DebugData.PassData.NRPInfo nrpInfo;

				public RenderGraph.DebugData.PassScriptInfo scriptInfo;

				public class NRPInfo
				{
					public RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo nativePassInfo;

					public List<int> textureFBFetchList = new List<int>();

					public List<int> setGlobals = new List<int>();

					public int width;

					public int height;

					public int volumeDepth;

					public int samples;

					public bool hasDepth;

					public class NativeRenderPassInfo
					{
						public string passBreakReasoning;

						public List<RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo.AttachmentInfo> attachmentInfos;

						public Dictionary<int, RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo.PassCompatibilityInfo> passCompatibility;

						public List<int> mergedPassIds;

						public class AttachmentInfo
						{
							public string resourceName;

							public string loadReason;

							public string storeReason;

							public string storeMsaaReason;

							public int attachmentIndex;

							public NativePassAttachment attachment;
						}

						public struct PassCompatibilityInfo
						{
							public string message;

							public bool isCompatible;
						}
					}
				}
			}

			public class BufferResourceData
			{
				public int count;

				public int stride;

				public GraphicsBuffer.Target target;

				public GraphicsBuffer.UsageFlags usage;
			}

			public class TextureResourceData
			{
				public int width;

				public int height;

				public int depth;

				public bool bindMS;

				public int samples;

				public GraphicsFormat format;

				public bool clearBuffer;
			}

			[DebuggerDisplay("ResourceDebug: {name} [{creationPassIndex}:{releasePassIndex}]")]
			public struct ResourceData
			{
				public string name;

				public bool imported;

				public int creationPassIndex;

				public int releasePassIndex;

				public List<int> consumerList;

				public List<int> producerList;

				public bool memoryless;

				public RenderGraph.DebugData.TextureResourceData textureData;

				public RenderGraph.DebugData.BufferResourceData bufferData;
			}

			public class PassScriptInfo
			{
				public string filePath;

				public int line;
			}
		}

		internal static class RenderGraphExceptionMessages
		{
			internal static string GetExceptionMessage(RenderGraphState state)
			{
				string higherCaller = RenderGraph.RenderGraphExceptionMessages.GetHigherCaller();
				string text;
				if (!RenderGraph.RenderGraphExceptionMessages.m_RenderGraphStateMessages.TryGetValue(state, out text))
				{
					if (!RenderGraph.RenderGraphExceptionMessages.enableCaller)
					{
						return "Invalid render graph state, impossible to log the exception.";
					}
					return "[" + higherCaller + "] Invalid render graph state, impossible to log the exception.";
				}
				else
				{
					if (!RenderGraph.RenderGraphExceptionMessages.enableCaller)
					{
						return text;
					}
					return "[" + higherCaller + "] " + text;
				}
			}

			private static string GetHigherCaller()
			{
				StackTrace stackTrace = new StackTrace(3, false);
				if (stackTrace.FrameCount > 0)
				{
					StackFrame frame = stackTrace.GetFrame(0);
					string text;
					if (frame == null)
					{
						text = null;
					}
					else
					{
						MethodBase method = frame.GetMethod();
						text = ((method != null) ? method.Name : null);
					}
					return text ?? "UnknownCaller";
				}
				return "UnknownCaller";
			}

			internal static bool enableCaller = true;

			internal const string k_RenderGraphExecutionError = "Render Graph Execution error";

			private static readonly Dictionary<RenderGraphState, string> m_RenderGraphStateMessages = new Dictionary<RenderGraphState, string>
			{
				{
					RenderGraphState.RecordingPass,
					"This API cannot be called when Render Graph records a pass, please call it within SetRenderFunc() or outside of AddUnsafePass()/AddComputePass()/AddRasterRenderPass()."
				},
				{
					RenderGraphState.RecordingGraph,
					"This API cannot be called during the Render Graph high-level recording step, please call it within AddUnsafePass()/AddComputePass()/AddRasterRenderPass() or outside of RecordRenderGraph()."
				},
				{
					RenderGraphState.RecordingPass | RenderGraphState.Executing,
					"This API cannot be called when Render Graph records a pass or executes it, please call it outside of AddUnsafePass()/AddComputePass()/AddRasterRenderPass()."
				},
				{
					RenderGraphState.Executing,
					"This API cannot be called during the Render Graph execution, please call it outside of SetRenderFunc()."
				},
				{
					RenderGraphState.Active,
					"This API cannot be called when Render Graph is active, please call it outside of RecordRenderGraph()."
				}
			};

			private const string k_ErrorDefaultMessage = "Invalid render graph state, impossible to log the exception.";
		}
	}
}
