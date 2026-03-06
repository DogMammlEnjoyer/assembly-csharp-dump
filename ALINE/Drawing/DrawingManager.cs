using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Drawing
{
	[ExecuteAlways]
	[AddComponentMenu("")]
	public class DrawingManager : MonoBehaviour
	{
		public static DrawingManager instance
		{
			get
			{
				if (DrawingManager._instance == null)
				{
					DrawingManager.Init();
				}
				return DrawingManager._instance;
			}
		}

		public static void Init()
		{
			if (DrawingManager._instance != null)
			{
				return;
			}
			GameObject gameObject = new GameObject("RetainedGizmos")
			{
				hideFlags = (HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset)
			};
			DrawingManager._instance = gameObject.AddComponent<DrawingManager>();
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(gameObject);
			}
		}

		private void RefreshRenderPipelineMode()
		{
			if (((RenderPipelineManager.currentPipeline != null) ? RenderPipelineManager.currentPipeline.GetType() : null) == typeof(UniversalRenderPipeline))
			{
				this.detectedRenderPipeline = DetectedRenderPipeline.URP;
				return;
			}
			this.detectedRenderPipeline = DetectedRenderPipeline.BuiltInOrCustom;
		}

		private void OnEnable()
		{
			if (DrawingManager._instance == null)
			{
				DrawingManager._instance = this;
			}
			if (DrawingManager._instance != this)
			{
				return;
			}
			this.actuallyEnabled = true;
			if (this.gizmos == null)
			{
				this.gizmos = new DrawingData();
			}
			this.gizmos.frameRedrawScope = new RedrawScope(this.gizmos);
			Draw.builder = this.gizmos.GetBuiltInBuilder(false);
			Draw.ingame_builder = this.gizmos.GetBuiltInBuilder(true);
			this.commandBuffer = new CommandBuffer();
			this.commandBuffer.name = "ALINE Gizmos";
			Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(this.PostRender));
			RenderPipelineManager.beginContextRendering += this.BeginContextRendering;
			RenderPipelineManager.beginCameraRendering += this.BeginCameraRendering;
			RenderPipelineManager.endCameraRendering += this.EndCameraRendering;
		}

		private void BeginContextRendering(ScriptableRenderContext context, List<Camera> cameras)
		{
			this.RefreshRenderPipelineMode();
		}

		private void BeginFrameRendering(ScriptableRenderContext context, Camera[] cameras)
		{
			this.RefreshRenderPipelineMode();
		}

		private void BeginCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			if (this.detectedRenderPipeline == DetectedRenderPipeline.URP)
			{
				UniversalAdditionalCameraData universalAdditionalCameraData = camera.GetUniversalAdditionalCameraData();
				if (universalAdditionalCameraData != null)
				{
					ScriptableRenderer scriptableRenderer = universalAdditionalCameraData.scriptableRenderer;
					if (this.renderPassFeature == null)
					{
						this.renderPassFeature = ScriptableObject.CreateInstance<AlineURPRenderPassFeature>();
					}
					this.renderPassFeature.AddRenderPasses(scriptableRenderer);
				}
			}
		}

		private void OnDisable()
		{
			if (!this.actuallyEnabled)
			{
				return;
			}
			this.actuallyEnabled = false;
			this.commandBuffer.Dispose();
			this.commandBuffer = null;
			Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(this.PostRender));
			RenderPipelineManager.beginContextRendering -= this.BeginContextRendering;
			RenderPipelineManager.beginCameraRendering -= this.BeginCameraRendering;
			RenderPipelineManager.endCameraRendering -= this.EndCameraRendering;
			if (this.gizmos != null)
			{
				Draw.builder.DiscardAndDisposeInternal();
				Draw.ingame_builder.DiscardAndDisposeInternal();
				this.gizmos.ClearData();
			}
			if (this.renderPassFeature != null)
			{
				Object.DestroyImmediate(this.renderPassFeature);
				this.renderPassFeature = null;
			}
		}

		private void OnEditorUpdate()
		{
			this.framePassed = true;
			this.CleanupIfNoCameraRendered();
		}

		private void Update()
		{
			if (this.actuallyEnabled)
			{
				this.CleanupIfNoCameraRendered();
			}
		}

		private void CleanupIfNoCameraRendered()
		{
			if (Time.frameCount > this.lastFrameCount + 1)
			{
				this.CheckFrameTicking();
				this.gizmos.PostRenderCleanup();
			}
			if (Time.realtimeSinceStartup - this.lastFrameTime > 10f)
			{
				Draw.builder.DiscardAndDisposeInternal();
				Draw.ingame_builder.DiscardAndDisposeInternal();
				Draw.builder = this.gizmos.GetBuiltInBuilder(false);
				Draw.ingame_builder = this.gizmos.GetBuiltInBuilder(true);
				this.lastFrameTime = Time.realtimeSinceStartup;
				DrawingManager.RemoveDestroyedGizmoDrawers();
			}
			if (this.lastFilterFrame - Time.frameCount > 5)
			{
				this.lastFilterFrame = Time.frameCount;
				DrawingManager.RemoveDestroyedGizmoDrawers();
			}
		}

		internal void ExecuteCustomRenderPass(ScriptableRenderContext context, Camera camera)
		{
			this.commandBuffer.Clear();
			this.SubmitFrame(camera, new DrawingData.CommandBufferWrapper
			{
				cmd = this.commandBuffer
			}, true);
			context.ExecuteCommandBuffer(this.commandBuffer);
		}

		internal void ExecuteCustomRenderGraphPass(DrawingData.CommandBufferWrapper cmd, Camera camera)
		{
			this.SubmitFrame(camera, cmd, true);
		}

		private void EndCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			if (this.detectedRenderPipeline == DetectedRenderPipeline.BuiltInOrCustom)
			{
				this.ExecuteCustomRenderPass(context, camera);
			}
		}

		private void PostRender(Camera camera)
		{
			this.commandBuffer.Clear();
			this.SubmitFrame(camera, new DrawingData.CommandBufferWrapper
			{
				cmd = this.commandBuffer
			}, false);
			Graphics.ExecuteCommandBuffer(this.commandBuffer);
		}

		private void CheckFrameTicking()
		{
			if (Time.frameCount != this.lastFrameCount)
			{
				this.framePassed = true;
				this.lastFrameCount = Time.frameCount;
				this.lastFrameTime = Time.realtimeSinceStartup;
				this.previousFrameRedrawScope = this.gizmos.frameRedrawScope;
				this.gizmos.frameRedrawScope = new RedrawScope(this.gizmos);
				Draw.builder.DisposeInternal();
				Draw.ingame_builder.DisposeInternal();
				Draw.builder = this.gizmos.GetBuiltInBuilder(false);
				Draw.ingame_builder = this.gizmos.GetBuiltInBuilder(true);
			}
			else if (this.framePassed && Application.isPlaying)
			{
				this.previousFrameRedrawScope.Draw();
			}
			if (this.framePassed)
			{
				this.gizmos.TickFramePreRender();
				this.framePassed = false;
			}
		}

		internal void SubmitFrame(Camera camera, DrawingData.CommandBufferWrapper cmd, bool usingRenderPipeline)
		{
			bool flag = false;
			bool allowCameraDefault = DrawingManager.allowRenderToRenderTextures || DrawingManager.drawToAllCameras || camera.targetTexture == null || flag;
			this.CheckFrameTicking();
			this.Submit(camera, cmd, usingRenderPipeline, allowCameraDefault);
			this.gizmos.PostRenderCleanup();
		}

		private bool ShouldDrawGizmos(Object obj)
		{
			return true;
		}

		private static void RemoveDestroyedGizmoDrawers()
		{
			int num = 0;
			for (int i = 0; i < DrawingManager.gizmoDrawers.Count; i++)
			{
				IDrawGizmos drawGizmos = DrawingManager.gizmoDrawers[i];
				if (drawGizmos as MonoBehaviour)
				{
					DrawingManager.gizmoDrawers[num] = drawGizmos;
					num++;
				}
			}
			DrawingManager.gizmoDrawers.RemoveRange(num, DrawingManager.gizmoDrawers.Count - num);
		}

		private void Submit(Camera camera, DrawingData.CommandBufferWrapper cmd, bool usingRenderPipeline, bool allowCameraDefault)
		{
			bool allowGizmos = false;
			Draw.builder.DisposeInternal();
			Draw.ingame_builder.DisposeInternal();
			this.gizmos.Render(camera, allowGizmos, cmd, allowCameraDefault);
			Draw.builder = this.gizmos.GetBuiltInBuilder(false);
			Draw.ingame_builder = this.gizmos.GetBuiltInBuilder(true);
		}

		public static void Register(IDrawGizmos item)
		{
			Type type = item.GetType();
			bool flag;
			if (!DrawingManager.gizmoDrawerTypes.TryGetValue(type, out flag))
			{
				BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
				MethodInfo methodInfo;
				if ((methodInfo = type.GetMethod("DrawGizmos", bindingAttr)) == null)
				{
					methodInfo = (type.GetMethod("Pathfinding.Drawing.IDrawGizmos.DrawGizmos", bindingAttr) ?? type.GetMethod("Drawing.IDrawGizmos.DrawGizmos", bindingAttr));
				}
				MethodInfo methodInfo2 = methodInfo;
				if (methodInfo2 == null)
				{
					throw new Exception("Could not find the DrawGizmos method in type " + type.Name);
				}
				flag = (methodInfo2.DeclaringType != typeof(MonoBehaviourGizmos));
				DrawingManager.gizmoDrawerTypes[type] = flag;
			}
			if (!flag)
			{
				return;
			}
			DrawingManager.gizmoDrawers.Add(item);
		}

		public static CommandBuilder GetBuilder(bool renderInGame = false)
		{
			return DrawingManager.instance.gizmos.GetBuilder(renderInGame);
		}

		public static CommandBuilder GetBuilder(RedrawScope redrawScope, bool renderInGame = false)
		{
			return DrawingManager.instance.gizmos.GetBuilder(redrawScope, renderInGame);
		}

		public static CommandBuilder GetBuilder(DrawingData.Hasher hasher, RedrawScope redrawScope = default(RedrawScope), bool renderInGame = false)
		{
			return DrawingManager.instance.gizmos.GetBuilder(hasher, redrawScope, renderInGame);
		}

		public static RedrawScope GetRedrawScope()
		{
			RedrawScope result = new RedrawScope(DrawingManager.instance.gizmos);
			result.DrawUntilDispose();
			return result;
		}

		public DrawingData gizmos;

		private static List<IDrawGizmos> gizmoDrawers = new List<IDrawGizmos>();

		private static Dictionary<Type, bool> gizmoDrawerTypes = new Dictionary<Type, bool>();

		private static DrawingManager _instance;

		private bool framePassed;

		private int lastFrameCount = int.MinValue;

		private float lastFrameTime = float.PositiveInfinity;

		private int lastFilterFrame;

		[SerializeField]
		private bool actuallyEnabled;

		private RedrawScope previousFrameRedrawScope;

		public static bool allowRenderToRenderTextures = false;

		public static bool drawToAllCameras = false;

		public static float lineWidthMultiplier = 1f;

		private CommandBuffer commandBuffer;

		[NonSerialized]
		private DetectedRenderPipeline detectedRenderPipeline;

		private HashSet<ScriptableRenderer> scriptableRenderersWithPass = new HashSet<ScriptableRenderer>();

		private AlineURPRenderPassFeature renderPassFeature;

		private static readonly ProfilerMarker MarkerALINE = new ProfilerMarker("ALINE");

		private static readonly ProfilerMarker MarkerCommandBuffer = new ProfilerMarker("Executing command buffer");

		private static readonly ProfilerMarker MarkerFrameTick = new ProfilerMarker("Frame Tick");

		private static readonly ProfilerMarker MarkerFilterDestroyedObjects = new ProfilerMarker("Filter destroyed objects");

		internal static readonly ProfilerMarker MarkerRefreshSelectionCache = new ProfilerMarker("Refresh Selection Cache");

		private static readonly ProfilerMarker MarkerGizmosAllowed = new ProfilerMarker("GizmosAllowed");

		private static readonly ProfilerMarker MarkerDrawGizmos = new ProfilerMarker("DrawGizmos");

		private static readonly ProfilerMarker MarkerSubmitGizmos = new ProfilerMarker("Submit Gizmos");

		private const float NO_DRAWING_TIMEOUT_SECS = 10f;

		private readonly Dictionary<Type, bool> typeToGizmosEnabled = new Dictionary<Type, bool>();
	}
}
