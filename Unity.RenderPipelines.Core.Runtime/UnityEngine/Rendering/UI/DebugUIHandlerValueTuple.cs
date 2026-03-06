using System;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerValueTuple : DebugUIHandlerWidget
	{
		protected override void OnEnable()
		{
			this.m_Timer = 0f;
		}

		public override bool OnSelection(bool fromNext, DebugUIHandlerWidget previous)
		{
			this.nameLabel.color = this.colorSelected;
			return true;
		}

		public override void OnDeselection()
		{
			this.nameLabel.color = this.colorDefault;
		}

		internal override void SetWidget(DebugUI.Widget widget)
		{
			this.m_Widget = widget;
			this.m_Field = base.CastWidget<DebugUI.ValueTuple>();
			this.nameLabel.text = this.m_Field.displayName;
			int numElements = this.m_Field.numElements;
			this.valueElements = new Text[numElements];
			this.valueElements[0] = this.valueLabel;
			float num = 230f / (float)numElements;
			for (int i = 1; i < numElements; i++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.valueLabel.gameObject, base.transform);
				gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
				RectTransform rectTransform = gameObject.transform as RectTransform;
				RectTransform rectTransform2 = this.nameLabel.transform as RectTransform;
				Vector2 vector = new Vector2(0f, 1f);
				rectTransform.anchorMin = vector;
				rectTransform.anchorMax = vector;
				rectTransform.sizeDelta = new Vector2(100f, 26f);
				Vector3 v = rectTransform2.anchoredPosition;
				v.x += (float)(i + 1) * num + 200f;
				rectTransform.anchoredPosition = v;
				rectTransform.pivot = new Vector2(0f, 1f);
				this.valueElements[i] = gameObject.GetComponent<Text>();
			}
		}

		internal virtual void UpdateValueLabels()
		{
			for (int i = 0; i < this.m_Field.numElements; i++)
			{
				if (i < this.valueElements.Length && this.valueElements[i] != null)
				{
					object value = this.m_Field.values[i].GetValue();
					this.valueElements[i].text = this.m_Field.values[i].FormatString(value);
					if (value is float)
					{
						this.valueElements[i].color = (((float)value == 0f) ? DebugUIHandlerValueTuple.k_ZeroColor : this.colorDefault);
					}
				}
			}
		}

		private void Update()
		{
			if (this.m_Field != null && this.m_Timer >= this.m_Field.refreshRate)
			{
				this.UpdateValueLabels();
				this.m_Timer -= this.m_Field.refreshRate;
			}
			this.m_Timer += Time.deltaTime;
		}

		public Text nameLabel;

		public Text valueLabel;

		protected internal DebugUI.ValueTuple m_Field;

		protected internal Text[] valueElements;

		private const float k_XOffset = 230f;

		private float m_Timer;

		private static readonly Color k_ZeroColor = Color.gray;
	}
}
