using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerContainer : MonoBehaviour
	{
		internal DebugUIHandlerWidget GetFirstItem()
		{
			if (this.contentHolder.childCount == 0)
			{
				return null;
			}
			List<DebugUIHandlerWidget> activeChildren = this.GetActiveChildren();
			if (activeChildren.Count == 0)
			{
				return null;
			}
			return activeChildren[0];
		}

		internal DebugUIHandlerWidget GetLastItem()
		{
			if (this.contentHolder.childCount == 0)
			{
				return null;
			}
			List<DebugUIHandlerWidget> activeChildren = this.GetActiveChildren();
			if (activeChildren.Count == 0)
			{
				return null;
			}
			return activeChildren[activeChildren.Count - 1];
		}

		internal bool IsDirectChild(DebugUIHandlerWidget widget)
		{
			return this.contentHolder.childCount != 0 && this.GetActiveChildren().Count((DebugUIHandlerWidget x) => x == widget) > 0;
		}

		private List<DebugUIHandlerWidget> GetActiveChildren()
		{
			List<DebugUIHandlerWidget> list = new List<DebugUIHandlerWidget>();
			foreach (object obj in this.contentHolder)
			{
				Transform transform = (Transform)obj;
				DebugUIHandlerWidget item;
				if (transform.gameObject.activeInHierarchy && transform.TryGetComponent<DebugUIHandlerWidget>(out item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		[SerializeField]
		public RectTransform contentHolder;
	}
}
