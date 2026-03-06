using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.UI
{
	internal class DebugUIHandlerPersistentCanvas : MonoBehaviour
	{
		internal void Toggle(DebugUI.Value widget, string displayName = null)
		{
			int num = this.m_Items.FindIndex((DebugUIHandlerValue x) => x.GetWidget() == widget);
			if (num > -1)
			{
				CoreUtils.Destroy(this.m_Items[num].gameObject);
				this.m_Items.RemoveAt(num);
				return;
			}
			DebugUIHandlerValue component = Object.Instantiate<RectTransform>(this.valuePrefab, this.panel, false).gameObject.GetComponent<DebugUIHandlerValue>();
			component.SetWidget(widget);
			component.nameLabel.text = (string.IsNullOrEmpty(displayName) ? widget.displayName : displayName);
			this.m_Items.Add(component);
		}

		internal void Toggle(DebugUI.ValueTuple widget, int? forceTupleIndex = null)
		{
			DebugUI.ValueTuple valueTuple = this.m_ValueTupleWidgets.Find((DebugUI.ValueTuple x) => x == widget);
			int num = (valueTuple != null) ? valueTuple.pinnedElementIndex : -1;
			if (valueTuple != null)
			{
				this.m_ValueTupleWidgets.Remove(valueTuple);
				this.Toggle(widget.values[num], null);
			}
			if (forceTupleIndex != null)
			{
				num = forceTupleIndex.Value;
			}
			if (num + 1 < widget.numElements)
			{
				widget.pinnedElementIndex = num + 1;
				string text = widget.displayName;
				if (widget.parent is DebugUI.Foldout)
				{
					string[] columnLabels = (widget.parent as DebugUI.Foldout).columnLabels;
					if (columnLabels != null && widget.pinnedElementIndex < columnLabels.Length)
					{
						text = text + " (" + columnLabels[widget.pinnedElementIndex] + ")";
					}
				}
				this.Toggle(widget.values[widget.pinnedElementIndex], text);
				this.m_ValueTupleWidgets.Add(widget);
				return;
			}
			widget.pinnedElementIndex = -1;
		}

		internal bool IsEmpty()
		{
			return this.m_Items.Count == 0;
		}

		internal void Clear()
		{
			foreach (DebugUIHandlerValue debugUIHandlerValue in this.m_Items)
			{
				CoreUtils.Destroy(debugUIHandlerValue.gameObject);
			}
			this.m_Items.Clear();
		}

		public RectTransform panel;

		public RectTransform valuePrefab;

		private List<DebugUIHandlerValue> m_Items = new List<DebugUIHandlerValue>();

		private List<DebugUI.ValueTuple> m_ValueTupleWidgets = new List<DebugUI.ValueTuple>();
	}
}
