using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering
{
	internal class DebugDisplaySettingsRenderGraph : IDebugDisplaySettingsData, IDebugDisplaySettingsQuery
	{
		public DebugDisplaySettingsRenderGraph()
		{
			foreach (RenderGraph renderGraph in RenderGraph.GetRegisteredRenderGraphs())
			{
				renderGraph.debugParams.Reset();
			}
		}

		IDebugDisplaySettingsPanelDisposable IDebugDisplaySettingsData.CreatePanel()
		{
			return new DebugDisplaySettingsRenderGraph.SettingsPanel(this);
		}

		public bool AreAnySettingsActive
		{
			get
			{
				using (List<RenderGraph>.Enumerator enumerator = RenderGraph.GetRegisteredRenderGraphs().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.areAnySettingsActive)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		[DisplayInfo(name = "Rendering", order = 10)]
		private class SettingsPanel : DebugDisplaySettingsPanel
		{
			public SettingsPanel(DebugDisplaySettingsRenderGraph _)
			{
				DebugUI.Foldout foldout = new DebugUI.Foldout();
				foldout.displayName = "Render Graph";
				HelpURLAttribute customAttribute = typeof(DebugDisplaySettingsRenderGraph).GetCustomAttribute<HelpURLAttribute>();
				foldout.documentationUrl = ((customAttribute != null) ? customAttribute.URL : null);
				DebugUI.Foldout foldout2 = foldout;
				base.AddWidget(foldout2);
				bool flag = false;
				foreach (RenderGraph renderGraph in RenderGraph.GetRegisteredRenderGraphs())
				{
					flag = true;
					foreach (DebugUI.Widget item in renderGraph.GetWidgetList())
					{
						foldout2.children.Add(item);
					}
				}
				if (!flag)
				{
					foldout2.children.Add(new DebugUI.MessageBox
					{
						displayName = "Warning: The current render pipeline does not have Render Graphs Registered",
						style = DebugUI.MessageBox.Style.Warning
					});
				}
			}
		}
	}
}
