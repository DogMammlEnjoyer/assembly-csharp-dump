using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	public abstract class ScriptableRenderPass : IRenderGraphRecorder
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void FrameCleanup(CommandBuffer cmd)
		{
			this.OnCameraCleanup(cmd);
		}

		public RenderPassEvent renderPassEvent { get; set; }

		[Obsolete("Use colorAttachmentHandles", true)]
		public RenderTargetIdentifier[] colorAttachments
		{
			get
			{
				throw new NotSupportedException("colorAttachments has been deprecated. Use colorAttachmentHandles instead.");
			}
		}

		[Obsolete("Use colorAttachmentHandle", true)]
		public RenderTargetIdentifier[] colorAttachment
		{
			get
			{
				throw new NotSupportedException("colorAttachment has been deprecated. Use colorAttachmentHandle instead.");
			}
		}

		[Obsolete("Use depthAttachmentHandle", true)]
		public RenderTargetIdentifier depthAttachment
		{
			get
			{
				throw new NotSupportedException("depthAttachment has been deprecated. Use depthAttachmentHandle instead.");
			}
		}

		public RTHandle[] colorAttachmentHandles
		{
			get
			{
				return this.m_ColorAttachments;
			}
		}

		public RTHandle colorAttachmentHandle
		{
			get
			{
				return this.m_ColorAttachments[0];
			}
		}

		public RTHandle depthAttachmentHandle
		{
			get
			{
				return this.m_DepthAttachment;
			}
		}

		public RenderBufferStoreAction[] colorStoreActions
		{
			get
			{
				return this.m_ColorStoreActions;
			}
		}

		public RenderBufferStoreAction depthStoreAction
		{
			get
			{
				return this.m_DepthStoreAction;
			}
		}

		internal bool[] overriddenColorStoreActions
		{
			get
			{
				return this.m_OverriddenColorStoreActions;
			}
		}

		internal bool overriddenDepthStoreAction
		{
			get
			{
				return this.m_OverriddenDepthStoreAction;
			}
		}

		public ScriptableRenderPassInput input
		{
			get
			{
				return this.m_Input;
			}
		}

		public ClearFlag clearFlag
		{
			get
			{
				return this.m_ClearFlag;
			}
		}

		public Color clearColor
		{
			get
			{
				return this.m_ClearColor;
			}
		}

		public bool requiresIntermediateTexture { get; set; }

		protected internal ProfilingSampler profilingSampler
		{
			get
			{
				if (this.m_RenderGraphSettings == null)
				{
					this.m_RenderGraphSettings = GraphicsSettings.GetRenderPipelineSettings<RenderGraphSettings>();
				}
				if (!this.m_RenderGraphSettings.enableRenderCompatibilityMode)
				{
					return null;
				}
				return this.m_ProfingSampler;
			}
			set
			{
				this.m_ProfingSampler = value;
				this.m_PassName = ((value != null) ? value.name : base.GetType().Name);
			}
		}

		protected internal string passName
		{
			get
			{
				return this.m_PassName;
			}
		}

		internal bool overrideCameraTarget { get; set; }

		internal bool isBlitRenderPass { get; set; }

		internal bool useNativeRenderPass { get; set; }

		internal int renderPassQueueIndex { get; set; }

		internal GraphicsFormat[] renderTargetFormat { get; set; }

		internal static DebugHandler GetActiveDebugHandler(UniversalCameraData cameraData)
		{
			DebugHandler debugHandler = cameraData.renderer.DebugHandler;
			if (debugHandler != null && debugHandler.IsActiveForCamera(cameraData.isPreviewCamera))
			{
				return debugHandler;
			}
			return null;
		}

		public ScriptableRenderPass()
		{
			this.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
			RTHandle[] array = new RTHandle[8];
			array[0] = ScriptableRenderPass.k_CameraTarget;
			this.m_ColorAttachments = array;
			this.m_DepthAttachment = ScriptableRenderPass.k_CameraTarget;
			this.m_InputAttachments = new RTHandle[8];
			this.m_InputAttachmentIsTransient = new bool[8];
			this.m_ColorStoreActions = new RenderBufferStoreAction[8];
			this.m_DepthStoreAction = RenderBufferStoreAction.Store;
			this.m_OverriddenColorStoreActions = new bool[8];
			this.m_OverriddenDepthStoreAction = false;
			this.m_ClearFlag = ClearFlag.None;
			this.m_ClearColor = Color.black;
			this.overrideCameraTarget = false;
			this.isBlitRenderPass = false;
			this.useNativeRenderPass = true;
			this.renderPassQueueIndex = -1;
			this.renderTargetFormat = new GraphicsFormat[8];
			this.profilingSampler = new ProfilingSampler(base.GetType().Name);
		}

		public void ConfigureInput(ScriptableRenderPassInput passInput)
		{
			this.m_Input = passInput;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public void ConfigureColorStoreAction(RenderBufferStoreAction storeAction, uint attachmentIndex = 0U)
		{
			this.m_ColorStoreActions[(int)attachmentIndex] = storeAction;
			this.m_OverriddenColorStoreActions[(int)attachmentIndex] = true;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public void ConfigureColorStoreActions(RenderBufferStoreAction[] storeActions)
		{
			int num = Math.Min(storeActions.Length, this.m_ColorStoreActions.Length);
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)num))
			{
				this.m_ColorStoreActions[(int)num2] = storeActions[(int)num2];
				this.m_OverriddenColorStoreActions[(int)num2] = true;
				num2 += 1U;
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public void ConfigureDepthStoreAction(RenderBufferStoreAction storeAction)
		{
			this.m_DepthStoreAction = storeAction;
			this.m_OverriddenDepthStoreAction = true;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		internal void ConfigureInputAttachments(RTHandle input, bool isTransient = false)
		{
			this.m_InputAttachments[0] = input;
			this.m_InputAttachmentIsTransient[0] = isTransient;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		internal void ConfigureInputAttachments(RTHandle[] inputs)
		{
			this.m_InputAttachments = inputs;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		internal void ConfigureInputAttachments(RTHandle[] inputs, bool[] isTransient)
		{
			this.ConfigureInputAttachments(inputs);
			this.m_InputAttachmentIsTransient = isTransient;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		internal void SetInputAttachmentTransient(int idx, bool isTransient)
		{
			this.m_InputAttachmentIsTransient[idx] = isTransient;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		internal bool IsInputAttachmentTransient(int idx)
		{
			return this.m_InputAttachmentIsTransient[idx];
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public void ResetTarget()
		{
			this.overrideCameraTarget = false;
			this.m_DepthAttachment = null;
			this.m_ColorAttachments[0] = null;
			for (int i = 1; i < this.m_ColorAttachments.Length; i++)
			{
				this.m_ColorAttachments[i] = null;
			}
		}

		[Obsolete("Use RTHandles for colorAttachment and depthAttachment", true)]
		public void ConfigureTarget(RenderTargetIdentifier colorAttachment, RenderTargetIdentifier depthAttachment)
		{
			throw new NotSupportedException("ConfigureTarget with RenderTargetIdentifier has been deprecated. Use RTHandles instead");
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public void ConfigureTarget(RTHandle colorAttachment, RTHandle depthAttachment)
		{
			this.overrideCameraTarget = true;
			this.m_DepthAttachment = depthAttachment;
			this.m_ColorAttachments[0] = colorAttachment;
			for (int i = 1; i < this.m_ColorAttachments.Length; i++)
			{
				this.m_ColorAttachments[i] = null;
			}
		}

		[Obsolete("Use RTHandles for colorAttachments and depthAttachment", true)]
		public void ConfigureTarget(RenderTargetIdentifier[] colorAttachments, RenderTargetIdentifier depthAttachment)
		{
			throw new NotSupportedException("ConfigureTarget with RenderTargetIdentifier has been deprecated. Use it with RTHandles instead");
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public void ConfigureTarget(RTHandle[] colorAttachments, RTHandle depthAttachment)
		{
			this.overrideCameraTarget = true;
			uint validColorBufferCount = RenderingUtils.GetValidColorBufferCount(colorAttachments);
			if ((ulong)validColorBufferCount > (ulong)((long)SystemInfo.supportedRenderTargetCount))
			{
				Debug.LogError("Trying to set " + validColorBufferCount.ToString() + " renderTargets, which is more than the maximum supported:" + SystemInfo.supportedRenderTargetCount.ToString());
			}
			if (colorAttachments.Length > this.m_ColorAttachments.Length)
			{
				Debug.LogError("Trying to set " + colorAttachments.Length.ToString() + " color attachments, which is more than the maximum supported:" + this.m_ColorAttachments.Length.ToString());
			}
			for (int i = 0; i < colorAttachments.Length; i++)
			{
				this.m_ColorAttachments[i] = colorAttachments[i];
			}
			for (int j = colorAttachments.Length; j < this.m_ColorAttachments.Length; j++)
			{
				this.m_ColorAttachments[j] = null;
			}
			this.m_DepthAttachment = depthAttachment;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		internal void ConfigureTarget(RTHandle[] colorAttachments, RTHandle depthAttachment, GraphicsFormat[] formats)
		{
			this.ConfigureTarget(colorAttachments, depthAttachment);
			for (int i = 0; i < formats.Length; i++)
			{
				this.renderTargetFormat[i] = formats[i];
			}
		}

		[Obsolete("Use RTHandle for colorAttachment", true)]
		public void ConfigureTarget(RenderTargetIdentifier colorAttachment)
		{
			throw new NotSupportedException("ConfigureTarget with RenderTargetIdentifier has been deprecated. Use it with RTHandles instead");
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public void ConfigureTarget(RTHandle colorAttachment)
		{
			this.ConfigureTarget(colorAttachment, ScriptableRenderPass.k_CameraTarget);
		}

		[Obsolete("Use RTHandles for colorAttachments", true)]
		public void ConfigureTarget(RenderTargetIdentifier[] colorAttachments)
		{
			throw new NotSupportedException("ConfigureTarget with RenderTargetIdentifier has been deprecated. Use it with RTHandles instead");
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public void ConfigureTarget(RTHandle[] colorAttachments)
		{
			this.ConfigureTarget(colorAttachments, ScriptableRenderPass.k_CameraTarget);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public void ConfigureClear(ClearFlag clearFlag, Color clearColor)
		{
			this.m_ClearFlag = clearFlag;
			this.m_ClearColor = clearColor;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public virtual void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public virtual void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
		}

		public virtual void OnCameraCleanup(CommandBuffer cmd)
		{
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public virtual void OnFinishCameraStackRendering(CommandBuffer cmd)
		{
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public virtual void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			Debug.LogWarning("Execute is not implemented, the pass " + this.ToString() + " won't be executed in the current render loop.");
		}

		public virtual void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			Debug.LogWarning("The render pass " + this.ToString() + " does not have an implementation of the RecordRenderGraph method. Please implement this method, or consider turning on Compatibility Mode (RenderGraph disabled) in the menu Edit > Project Settings > Graphics > URP. Otherwise the render pass will have no effect. For more information, refer to https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest/index.html?subfolder=/manual/customizing-urp.html.");
		}

		[Obsolete("Use RTHandles for source and destination", true)]
		public void Blit(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material = null, int passIndex = 0)
		{
			throw new NotSupportedException("Blit with RenderTargetIdentifier has been deprecated. Use RTHandles instead");
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public void Blit(CommandBuffer cmd, RTHandle source, RTHandle destination, Material material = null, int passIndex = 0)
		{
			if (material == null)
			{
				Blitter.BlitCameraTexture(cmd, source, destination, 0f, source.rt.filterMode == FilterMode.Bilinear);
				return;
			}
			Blitter.BlitCameraTexture(cmd, source, destination, material, passIndex);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe void Blit(CommandBuffer cmd, ref RenderingData data, Material material, int passIndex = 0)
		{
			ScriptableRenderer scriptableRenderer = *data.cameraData.renderer;
			this.Blit(cmd, scriptableRenderer.cameraColorTargetHandle, scriptableRenderer.GetCameraColorFrontBuffer(cmd), material, passIndex);
			scriptableRenderer.SwapColorBuffer(cmd);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe void Blit(CommandBuffer cmd, ref RenderingData data, RTHandle source, Material material, int passIndex = 0)
		{
			ScriptableRenderer scriptableRenderer = *data.cameraData.renderer;
			this.Blit(cmd, source, scriptableRenderer.cameraColorTargetHandle, material, passIndex);
		}

		public DrawingSettings CreateDrawingSettings(ShaderTagId shaderTagId, ref RenderingData renderingData, SortingCriteria sortingCriteria)
		{
			ContextContainer frameData = renderingData.frameData;
			UniversalRenderingData renderingData2 = frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			return RenderingUtils.CreateDrawingSettings(shaderTagId, renderingData2, cameraData, lightData, sortingCriteria);
		}

		public DrawingSettings CreateDrawingSettings(ShaderTagId shaderTagId, UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData, SortingCriteria sortingCriteria)
		{
			return RenderingUtils.CreateDrawingSettings(shaderTagId, renderingData, cameraData, lightData, sortingCriteria);
		}

		public DrawingSettings CreateDrawingSettings(List<ShaderTagId> shaderTagIdList, ref RenderingData renderingData, SortingCriteria sortingCriteria)
		{
			ContextContainer frameData = renderingData.frameData;
			UniversalRenderingData renderingData2 = frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			return RenderingUtils.CreateDrawingSettings(shaderTagIdList, renderingData2, cameraData, lightData, sortingCriteria);
		}

		public DrawingSettings CreateDrawingSettings(List<ShaderTagId> shaderTagIdList, UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData, SortingCriteria sortingCriteria)
		{
			return RenderingUtils.CreateDrawingSettings(shaderTagIdList, renderingData, cameraData, lightData, sortingCriteria);
		}

		public static bool operator <(ScriptableRenderPass lhs, ScriptableRenderPass rhs)
		{
			return lhs.renderPassEvent < rhs.renderPassEvent;
		}

		public static bool operator >(ScriptableRenderPass lhs, ScriptableRenderPass rhs)
		{
			return lhs.renderPassEvent > rhs.renderPassEvent;
		}

		internal static int GetRenderPassEventRange(RenderPassEvent renderPassEvent)
		{
			int num = RenderPassEventsEnumValues.values.Length;
			int num2 = 0;
			int num3 = 0;
			while (num3 < num && RenderPassEventsEnumValues.values[num2] != (int)renderPassEvent)
			{
				num2++;
				num3++;
			}
			if (num2 >= num)
			{
				Debug.LogError("GetRenderPassEventRange: invalid renderPassEvent value cannot be found in the RenderPassEvent enumeration");
				return 0;
			}
			if (num2 + 1 >= num)
			{
				return 50;
			}
			return RenderPassEventsEnumValues.values[num2 + 1] - (int)renderPassEvent;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public static RTHandle k_CameraTarget = RTHandles.Alloc(BuiltinRenderTextureType.CameraTarget);

		private RenderBufferStoreAction[] m_ColorStoreActions = new RenderBufferStoreAction[1];

		private RenderBufferStoreAction m_DepthStoreAction;

		private bool[] m_OverriddenColorStoreActions = new bool[1];

		private bool m_OverriddenDepthStoreAction;

		private ProfilingSampler m_ProfingSampler;

		private string m_PassName;

		private RenderGraphSettings m_RenderGraphSettings;

		internal NativeArray<int> m_ColorAttachmentIndices;

		internal NativeArray<int> m_InputAttachmentIndices;

		private RTHandle[] m_ColorAttachments;

		internal RTHandle[] m_InputAttachments = new RTHandle[8];

		internal bool[] m_InputAttachmentIsTransient = new bool[8];

		private RTHandle m_DepthAttachment;

		private ScriptableRenderPassInput m_Input;

		private ClearFlag m_ClearFlag;

		private Color m_ClearColor = Color.black;
	}
}
