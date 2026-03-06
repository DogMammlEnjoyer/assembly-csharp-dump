using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public readonly struct CanStartDragArgs
	{
		internal CanStartDragArgs(VisualElement draggedElement, int id, IEnumerable<int> selectedIds)
		{
			this.draggedElement = draggedElement;
			this.id = id;
			this.selectedIds = selectedIds;
		}

		public readonly VisualElement draggedElement;

		public readonly int id;

		public readonly IEnumerable<int> selectedIds;
	}
}
