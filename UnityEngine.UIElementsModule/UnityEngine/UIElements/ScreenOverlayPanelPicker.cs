using System;

namespace UnityEngine.UIElements
{
	internal class ScreenOverlayPanelPicker
	{
		public bool TryPick(BaseRuntimePanel panel, int pointerId, Vector2 screenPosition, Vector2 delta, int? targetDisplay, out bool captured)
		{
			bool flag;
			if (targetDisplay != null)
			{
				int? num = targetDisplay;
				int targetDisplay2 = panel.targetDisplay;
				flag = !(num.GetValueOrDefault() == targetDisplay2 & num != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				captured = false;
				result = false;
			}
			else
			{
				BaseVisualElementPanel baseVisualElementPanel;
				captured = this.GetCapturingPanel(pointerId, out baseVisualElementPanel);
				bool flag3 = captured;
				if (flag3)
				{
					bool flag4 = baseVisualElementPanel == panel;
					if (flag4)
					{
						return true;
					}
				}
				else
				{
					Vector3 v;
					bool flag5 = panel.ScreenToPanel(screenPosition, delta, out v, false);
					if (flag5)
					{
						VisualElement visualElement = panel.Pick(v, pointerId);
						bool flag6 = visualElement != null;
						if (flag6)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		private bool GetCapturingPanel(int pointerId, out BaseVisualElementPanel capturingPanel)
		{
			IEventHandler capturingElement = RuntimePanel.s_EventDispatcher.pointerState.GetCapturingElement(pointerId);
			VisualElement visualElement = capturingElement as VisualElement;
			bool flag = visualElement != null;
			if (flag)
			{
				capturingPanel = visualElement.elementPanel;
			}
			else
			{
				capturingPanel = PointerDeviceState.GetPlayerPanelWithSoftPointerCapture(pointerId);
			}
			return capturingPanel != null;
		}
	}
}
