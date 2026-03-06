using System;
using System.Collections;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerToggleHistory : DebugUIHandlerToggle
	{
		internal override void SetWidget(DebugUI.Widget widget)
		{
			DebugUI.HistoryBoolField historyBoolField = widget as DebugUI.HistoryBoolField;
			int num = (historyBoolField != null) ? historyBoolField.historyDepth : 0;
			this.historyToggles = new Toggle[num];
			float num2 = (num > 0) ? (230f / (float)num) : 0f;
			for (int i = 0; i < num; i++)
			{
				Toggle toggle = Object.Instantiate<Toggle>(this.valueToggle, base.transform);
				Vector3 position = toggle.transform.position;
				position.x += (float)(i + 1) * num2;
				toggle.transform.position = position;
				Image component = toggle.transform.GetChild(0).GetComponent<Image>();
				component.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(-1f, -1f, 2f, 2f), Vector2.zero);
				component.color = new Color32(50, 50, 50, 120);
				component.transform.GetChild(0).GetComponent<Image>().color = new Color32(110, 110, 110, byte.MaxValue);
				this.historyToggles[i] = toggle.GetComponent<Toggle>();
			}
			base.SetWidget(widget);
		}

		protected internal override void UpdateValueLabel()
		{
			base.UpdateValueLabel();
			DebugUI.HistoryBoolField historyBoolField = this.m_Field as DebugUI.HistoryBoolField;
			int num = (historyBoolField != null) ? historyBoolField.historyDepth : 0;
			for (int i = 0; i < num; i++)
			{
				if (i < this.historyToggles.Length && this.historyToggles[i] != null)
				{
					this.historyToggles[i].isOn = historyBoolField.GetHistoryValue(i);
				}
			}
			if (base.isActiveAndEnabled)
			{
				base.StartCoroutine(this.RefreshAfterSanitization());
			}
		}

		private IEnumerator RefreshAfterSanitization()
		{
			yield return null;
			this.valueToggle.isOn = this.m_Field.getter();
			yield break;
		}

		private Toggle[] historyToggles;

		private const float k_XOffset = 230f;
	}
}
