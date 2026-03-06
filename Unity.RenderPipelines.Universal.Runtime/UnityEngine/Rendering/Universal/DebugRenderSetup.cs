using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class DebugRenderSetup : IDisposable
	{
		private DebugDisplaySettingsMaterial MaterialSettings
		{
			get
			{
				return this.m_DebugHandler.DebugDisplaySettings.materialSettings;
			}
		}

		private DebugDisplaySettingsRendering RenderingSettings
		{
			get
			{
				return this.m_DebugHandler.DebugDisplaySettings.renderingSettings;
			}
		}

		private DebugDisplaySettingsLighting LightingSettings
		{
			get
			{
				return this.m_DebugHandler.DebugDisplaySettings.lightingSettings;
			}
		}

		internal void Begin(RasterCommandBuffer cmd)
		{
			DebugSceneOverrideMode sceneOverrideMode = this.RenderingSettings.sceneOverrideMode;
			if (sceneOverrideMode == DebugSceneOverrideMode.Wireframe)
			{
				cmd.SetWireframe(true);
				return;
			}
			if (sceneOverrideMode - DebugSceneOverrideMode.SolidWireframe > 1)
			{
				return;
			}
			if (this.m_Index == 1)
			{
				cmd.SetWireframe(true);
			}
		}

		internal void End(RasterCommandBuffer cmd)
		{
			DebugSceneOverrideMode sceneOverrideMode = this.RenderingSettings.sceneOverrideMode;
			if (sceneOverrideMode == DebugSceneOverrideMode.Wireframe)
			{
				cmd.SetWireframe(false);
				return;
			}
			if (sceneOverrideMode - DebugSceneOverrideMode.SolidWireframe > 1)
			{
				return;
			}
			if (this.m_Index == 1)
			{
				cmd.SetWireframe(false);
			}
		}

		internal DebugRenderSetup(DebugHandler debugHandler, int index, FilteringSettings filteringSettings)
		{
			this.m_DebugHandler = debugHandler;
			this.m_FilteringSettings = filteringSettings;
			this.m_Index = index;
		}

		internal void CreateRendererList(ScriptableRenderContext context, ref CullingResults cullResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, ref RenderStateBlock renderStateBlock, ref RendererList rendererList)
		{
			RenderingUtils.CreateRendererListWithRenderStateBlock(context, ref cullResults, drawingSettings, filteringSettings, renderStateBlock, ref rendererList);
		}

		internal void CreateRendererList(RenderGraph renderGraph, ref CullingResults cullResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, ref RenderStateBlock renderStateBlock, ref RendererListHandle rendererListHdl)
		{
			RenderingUtils.CreateRendererListWithRenderStateBlock(renderGraph, ref cullResults, drawingSettings, filteringSettings, renderStateBlock, ref rendererListHdl);
		}

		internal void DrawWithRendererList(RasterCommandBuffer cmd, ref RendererList rendererList)
		{
			if (rendererList.isValid)
			{
				cmd.DrawRendererList(rendererList);
			}
		}

		internal DrawingSettings CreateDrawingSettings(DrawingSettings drawingSettings)
		{
			if (this.MaterialSettings.vertexAttributeDebugMode > DebugVertexAttributeMode.None)
			{
				Material replacementMaterial = this.m_DebugHandler.ReplacementMaterial;
				DrawingSettings result = drawingSettings;
				result.overrideMaterial = replacementMaterial;
				result.overrideMaterialPassIndex = 0;
				return result;
			}
			return drawingSettings;
		}

		internal RenderStateBlock GetRenderStateBlock(RenderStateBlock renderStateBlock)
		{
			switch (this.RenderingSettings.sceneOverrideMode)
			{
			case DebugSceneOverrideMode.Overdraw:
			{
				bool flag = this.m_FilteringSettings.renderQueueRange == RenderQueueRange.opaque || this.m_FilteringSettings.renderQueueRange == RenderQueueRange.all;
				bool flag2 = this.m_FilteringSettings.renderQueueRange == RenderQueueRange.transparent || this.m_FilteringSettings.renderQueueRange == RenderQueueRange.all;
				bool flag3 = this.m_DebugHandler.DebugDisplaySettings.renderingSettings.overdrawMode == DebugOverdrawMode.Opaque || this.m_DebugHandler.DebugDisplaySettings.renderingSettings.overdrawMode == DebugOverdrawMode.All;
				bool flag4 = this.m_DebugHandler.DebugDisplaySettings.renderingSettings.overdrawMode == DebugOverdrawMode.Transparent || this.m_DebugHandler.DebugDisplaySettings.renderingSettings.overdrawMode == DebugOverdrawMode.All;
				BlendMode destinationColorBlendMode = ((flag && flag3) || (flag2 && flag4)) ? BlendMode.One : BlendMode.Zero;
				RenderTargetBlendState blendState = new RenderTargetBlendState(ColorWriteMask.All, BlendMode.One, destinationColorBlendMode, BlendMode.One, BlendMode.Zero, BlendOp.Add, BlendOp.Add);
				renderStateBlock.blendState = new BlendState
				{
					blendState0 = blendState
				};
				renderStateBlock.mask = RenderStateMask.Blend;
				break;
			}
			case DebugSceneOverrideMode.Wireframe:
				renderStateBlock.rasterState = new RasterState(CullMode.Off, 0, 0f, true);
				renderStateBlock.mask = RenderStateMask.Raster;
				break;
			case DebugSceneOverrideMode.SolidWireframe:
			case DebugSceneOverrideMode.ShadedWireframe:
				if (this.m_Index == 1)
				{
					renderStateBlock.rasterState = new RasterState(CullMode.Back, -1, -1f, true);
					renderStateBlock.mask = RenderStateMask.Raster;
				}
				break;
			}
			return renderStateBlock;
		}

		internal int GetIndex()
		{
			return this.m_Index;
		}

		public void Dispose()
		{
		}

		private readonly DebugHandler m_DebugHandler;

		private readonly FilteringSettings m_FilteringSettings;

		private readonly int m_Index;
	}
}
