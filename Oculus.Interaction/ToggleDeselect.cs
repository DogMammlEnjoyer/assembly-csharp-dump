using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Oculus.Interaction
{
	public class ToggleDeselect : Toggle
	{
		public bool ClearStateOnDrag
		{
			get
			{
				return this._clearStateOnDrag;
			}
			set
			{
				this._clearStateOnDrag = value;
			}
		}

		public void OnBeginDrag(PointerEventData pointerEventData)
		{
			if (!this._clearStateOnDrag)
			{
				return;
			}
			this.InstantClearState();
			this.DoStateTransition(Selectable.SelectionState.Normal, true);
			ExecuteEvents.ExecuteHierarchy<IBeginDragHandler>(base.transform.parent.gameObject, pointerEventData, ExecuteEvents.beginDragHandler);
		}

		[SerializeField]
		private bool _clearStateOnDrag;
	}
}
