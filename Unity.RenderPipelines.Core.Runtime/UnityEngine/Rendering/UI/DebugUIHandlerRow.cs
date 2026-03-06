using System;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerRow : DebugUIHandlerFoldout
	{
		protected override void OnEnable()
		{
			this.m_Timer = 0f;
		}

		private GameObject GetChild(int index)
		{
			if (index < 0)
			{
				return null;
			}
			if (base.gameObject.transform != null)
			{
				Transform child = base.gameObject.transform.GetChild(1);
				if (child != null && child.childCount > index)
				{
					return child.GetChild(index).gameObject;
				}
			}
			return null;
		}

		private bool TryGetChild(int index, out GameObject child)
		{
			child = this.GetChild(index);
			return child != null;
		}

		private bool IsActive(DebugUI.Table table, int index, GameObject child)
		{
			if (table == null || !table.GetColumnVisibility(index))
			{
				return false;
			}
			Transform transform = child.transform.Find("Value");
			Text text;
			return !(transform != null) || !transform.TryGetComponent<Text>(out text) || !string.IsNullOrEmpty(text.text);
		}

		protected void Update()
		{
			DebugUI.Table.Row row = base.CastWidget<DebugUI.Table.Row>();
			DebugUI.Table table = row.parent as DebugUI.Table;
			float num = 0.1f;
			bool flag = this.m_Timer >= num;
			if (flag)
			{
				this.m_Timer -= num;
			}
			this.m_Timer += Time.deltaTime;
			for (int i = 0; i < row.children.Count; i++)
			{
				GameObject gameObject;
				if (this.TryGetChild(i, out gameObject))
				{
					bool flag2 = this.IsActive(table, i, gameObject);
					if (gameObject != null)
					{
						gameObject.SetActive(flag2);
					}
					if (flag2 && flag)
					{
						DebugUIHandlerColor debugUIHandlerColor;
						if (gameObject.TryGetComponent<DebugUIHandlerColor>(out debugUIHandlerColor))
						{
							debugUIHandlerColor.UpdateColor();
						}
						DebugUIHandlerToggle debugUIHandlerToggle;
						if (gameObject.TryGetComponent<DebugUIHandlerToggle>(out debugUIHandlerToggle))
						{
							debugUIHandlerToggle.UpdateValueLabel();
						}
						DebugUIHandlerObjectList debugUIHandlerObjectList;
						if (gameObject.TryGetComponent<DebugUIHandlerObjectList>(out debugUIHandlerObjectList))
						{
							debugUIHandlerObjectList.UpdateValueLabel();
						}
					}
				}
			}
			DebugUIHandlerWidget debugUIHandlerWidget = this.GetChild(0).GetComponent<DebugUIHandlerWidget>();
			DebugUIHandlerWidget previousUIHandler = null;
			for (int j = 0; j < row.children.Count; j++)
			{
				debugUIHandlerWidget.previousUIHandler = previousUIHandler;
				GameObject gameObject2;
				if (this.TryGetChild(j, out gameObject2))
				{
					if (this.IsActive(table, j, gameObject2))
					{
						previousUIHandler = debugUIHandlerWidget;
					}
					bool flag3 = false;
					for (int k = j + 1; k < row.children.Count; k++)
					{
						GameObject child;
						if (this.TryGetChild(k, out child) && this.IsActive(table, k, child))
						{
							DebugUIHandlerWidget component = gameObject2.GetComponent<DebugUIHandlerWidget>();
							debugUIHandlerWidget.nextUIHandler = component;
							debugUIHandlerWidget = component;
							j = k - 1;
							flag3 = true;
							break;
						}
					}
					if (!flag3)
					{
						debugUIHandlerWidget.nextUIHandler = null;
						return;
					}
				}
			}
		}

		private float m_Timer;
	}
}
