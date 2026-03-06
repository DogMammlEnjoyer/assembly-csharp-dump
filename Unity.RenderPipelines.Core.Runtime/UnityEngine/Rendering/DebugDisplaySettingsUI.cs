using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	public class DebugDisplaySettingsUI : IDebugData
	{
		private void Reset()
		{
			if (this.m_Settings != null)
			{
				this.m_Settings.Reset();
				this.UnregisterDebug();
				this.RegisterDebug(this.m_Settings);
				DebugManager.instance.RefreshEditor();
			}
		}

		public void RegisterDebug(IDebugDisplaySettings settings)
		{
			DebugManager debugManager = DebugManager.instance;
			List<IDebugDisplaySettingsPanelDisposable> panels = new List<IDebugDisplaySettingsPanelDisposable>();
			debugManager.RegisterData(this);
			this.m_Settings = settings;
			this.m_DisposablePanels = panels;
			this.m_Settings.Add(new DebugDisplaySettingsRenderGraph());
			Action<IDebugDisplaySettingsData> onExecute = delegate(IDebugDisplaySettingsData data)
			{
				IDebugDisplaySettingsPanelDisposable debugDisplaySettingsPanelDisposable = data.CreatePanel();
				DebugUI.Widget[] widgets = debugDisplaySettingsPanelDisposable.Widgets;
				DebugManager debugManager = debugManager;
				string panelName = debugDisplaySettingsPanelDisposable.PanelName;
				bool createIfNull = true;
				DebugDisplaySettingsPanel debugDisplaySettingsPanel = debugDisplaySettingsPanelDisposable as DebugDisplaySettingsPanel;
				DebugUI.Panel panel = debugManager.GetPanel(panelName, createIfNull, (debugDisplaySettingsPanel != null) ? debugDisplaySettingsPanel.Order : 0, false);
				ObservableList<DebugUI.Widget> children = panel.children;
				panel.flags = debugDisplaySettingsPanelDisposable.Flags;
				panels.Add(debugDisplaySettingsPanelDisposable);
				children.Add(widgets);
			};
			this.m_Settings.ForEach(onExecute);
		}

		public void UnregisterDebug()
		{
			DebugManager instance = DebugManager.instance;
			if (this.m_DisposablePanels != null)
			{
				foreach (IDebugDisplaySettingsPanelDisposable debugDisplaySettingsPanelDisposable in this.m_DisposablePanels)
				{
					DebugUI.Widget[] widgets = debugDisplaySettingsPanelDisposable.Widgets;
					string panelName = debugDisplaySettingsPanelDisposable.PanelName;
					ObservableList<DebugUI.Widget> children = instance.GetPanel(panelName, true, 0, false).children;
					debugDisplaySettingsPanelDisposable.Dispose();
					children.Remove(widgets);
				}
				this.m_DisposablePanels = null;
			}
			instance.UnregisterData(this);
		}

		public Action GetReset()
		{
			return new Action(this.Reset);
		}

		private IEnumerable<IDebugDisplaySettingsPanelDisposable> m_DisposablePanels;

		private IDebugDisplaySettings m_Settings;
	}
}
