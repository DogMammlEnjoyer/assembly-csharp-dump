using System;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class DebugRendererLists
	{
		public DebugRendererLists(DebugHandler debugHandler, FilteringSettings filteringSettings)
		{
			this.m_DebugHandler = debugHandler;
			this.m_FilteringSettings = filteringSettings;
		}

		private void CreateDebugRenderSetups(FilteringSettings filteringSettings)
		{
			DebugSceneOverrideMode sceneOverrideMode = this.m_DebugHandler.DebugDisplaySettings.renderingSettings.sceneOverrideMode;
			int num = (sceneOverrideMode == DebugSceneOverrideMode.SolidWireframe || sceneOverrideMode == DebugSceneOverrideMode.ShadedWireframe) ? 2 : 1;
			for (int i = 0; i < num; i++)
			{
				this.m_DebugRenderSetups.Add(new DebugRenderSetup(this.m_DebugHandler, i, filteringSettings));
			}
		}

		private void DisposeDebugRenderLists()
		{
			foreach (DebugRenderSetup debugRenderSetup in this.m_DebugRenderSetups)
			{
				debugRenderSetup.Dispose();
			}
			this.m_DebugRenderSetups.Clear();
			this.m_ActiveDebugRendererList.Clear();
			this.m_ActiveDebugRendererListHdl.Clear();
		}

		internal void CreateRendererListsWithDebugRenderState(ScriptableRenderContext context, ref CullingResults cullResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, ref RenderStateBlock renderStateBlock)
		{
			this.CreateDebugRenderSetups(filteringSettings);
			foreach (DebugRenderSetup debugRenderSetup in this.m_DebugRenderSetups)
			{
				DrawingSettings ds = debugRenderSetup.CreateDrawingSettings(drawingSettings);
				RenderStateBlock renderStateBlock2 = debugRenderSetup.GetRenderStateBlock(renderStateBlock);
				RendererList item = default(RendererList);
				RenderingUtils.CreateRendererListWithRenderStateBlock(context, ref cullResults, ds, filteringSettings, renderStateBlock2, ref item);
				this.m_ActiveDebugRendererList.Add(item);
			}
		}

		internal void CreateRendererListsWithDebugRenderState(RenderGraph renderGraph, ref CullingResults cullResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, ref RenderStateBlock renderStateBlock)
		{
			this.CreateDebugRenderSetups(filteringSettings);
			foreach (DebugRenderSetup debugRenderSetup in this.m_DebugRenderSetups)
			{
				DrawingSettings ds = debugRenderSetup.CreateDrawingSettings(drawingSettings);
				RenderStateBlock renderStateBlock2 = debugRenderSetup.GetRenderStateBlock(renderStateBlock);
				RendererListHandle item = default(RendererListHandle);
				RenderingUtils.CreateRendererListWithRenderStateBlock(renderGraph, ref cullResults, ds, filteringSettings, renderStateBlock2, ref item);
				this.m_ActiveDebugRendererListHdl.Add(item);
			}
		}

		internal void PrepareRendererListForRasterPass(IRasterRenderGraphBuilder builder)
		{
			foreach (RendererListHandle rendererListHandle in this.m_ActiveDebugRendererListHdl)
			{
				builder.UseRendererList(rendererListHandle);
			}
		}

		internal void DrawWithRendererList(RasterCommandBuffer cmd)
		{
			foreach (DebugRenderSetup debugRenderSetup in this.m_DebugRenderSetups)
			{
				debugRenderSetup.Begin(cmd);
				RendererList rendererList = default(RendererList);
				if (this.m_ActiveDebugRendererList.Count > 0)
				{
					rendererList = this.m_ActiveDebugRendererList[debugRenderSetup.GetIndex()];
				}
				else if (this.m_ActiveDebugRendererListHdl.Count > 0)
				{
					rendererList = this.m_ActiveDebugRendererListHdl[debugRenderSetup.GetIndex()];
				}
				debugRenderSetup.DrawWithRendererList(cmd, ref rendererList);
				debugRenderSetup.End(cmd);
			}
			this.DisposeDebugRenderLists();
		}

		private readonly DebugHandler m_DebugHandler;

		private readonly FilteringSettings m_FilteringSettings;

		private List<DebugRenderSetup> m_DebugRenderSetups = new List<DebugRenderSetup>(2);

		private List<RendererList> m_ActiveDebugRendererList = new List<RendererList>(2);

		private List<RendererListHandle> m_ActiveDebugRendererListHdl = new List<RendererListHandle>(2);
	}
}
