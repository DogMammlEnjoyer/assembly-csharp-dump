using System;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	public class DebugBar : OverlayCanvasPanel
	{
		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this._buttonsAnchor = base.Append<Flex>("buttons");
			this._buttonsAnchor.LayoutStyle = Style.Load<LayoutStyle>("Buttons");
			Controller controller = base.Append<Controller>("leftbuttons");
			controller.LayoutStyle = Style.Load<LayoutStyle>("FillWithMargin");
			this._time = controller.Append<Label>("time");
			this._time.LayoutStyle = Style.Load<LayoutStyle>("BarTime");
			this._time.TextStyle = Style.Load<TextStyle>("BarTime");
			this._miniButtonsAnchor = controller.Append<Flex>("miniButtons");
			this._miniButtonsAnchor.LayoutStyle = Style.Load<LayoutStyle>("MiniButtons");
			base.SetExpectedPixelsPerUnit(1000f, 10f, 2.24f);
			base.Show();
		}

		public void RegisterPanel(DebugPanel panel)
		{
			if (panel == null)
			{
				return;
			}
			panel.OnVisibilityChangedEvent += this.OnPanelVisibilityChanged;
			this._panels.Add(panel);
			Toggle toggle = this._buttonsAnchor.Append<Toggle>("PanelButton");
			toggle.Icon = panel.Icon;
			toggle.LayoutStyle = Style.Load<LayoutStyle>("PanelButton");
			toggle.BackgroundStyle = Style.Load<ImageStyle>("PanelButtonBackground");
			toggle.IconStyle = Style.Load<ImageStyle>("PanelButtonIcon");
			toggle.Callback = new Action(panel.ToggleVisibility);
			this._panelToggles.Add(panel, toggle);
		}

		public Toggle RegisterControl(string buttonName, Texture2D icon, Action callback)
		{
			if (buttonName == null)
			{
				throw new ArgumentNullException("buttonName");
			}
			if (icon == null)
			{
				throw new ArgumentNullException("icon");
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			Toggle toggle = this._miniButtonsAnchor.Append<Toggle>(buttonName);
			toggle.LayoutStyle = Style.Load<LayoutStyle>("MiniButton");
			toggle.Icon = icon;
			toggle.IconStyle = Style.Load<ImageStyle>("MiniButtonIcon");
			toggle.Callback = callback;
			return toggle;
		}

		private void OnPanelVisibilityChanged(Controller controller)
		{
			DebugPanel debugPanel = controller as DebugPanel;
			Toggle toggle;
			if (debugPanel != null && this._panelToggles.TryGetValue(debugPanel, out toggle))
			{
				toggle.State = debugPanel.Visibility;
			}
		}

		private void Update()
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			int num = (int)(realtimeSinceStartup / 60f);
			int num2 = (int)(realtimeSinceStartup % 60f);
			string content = string.Format("{0:00}:{1:00}", num, num2);
			this._time.Content = content;
		}

		private List<DebugPanel> _panels = new List<DebugPanel>();

		private Dictionary<DebugPanel, Toggle> _panelToggles = new Dictionary<DebugPanel, Toggle>();

		private Flex _buttonsAnchor;

		private Flex _miniButtonsAnchor;

		private Label _time;
	}
}
