using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Modio.Unity.UI.Navigation
{
	public class ModioViewportRestraintChild : MonoBehaviour, ISelectHandler, IEventSystemHandler
	{
		private void Awake()
		{
			this._viewportRestraint = base.GetComponentInParent<ModioViewportRestraint>();
			if (this._viewportRestraint == null)
			{
				base.enabled = false;
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			if (eventData is PointerEventData)
			{
				return;
			}
			this.MoveToSelected();
		}

		private void MoveToSelected()
		{
			if (this._overrideFocusTo != null)
			{
				this._viewportRestraint.ChildSelected(this._overrideFocusTo);
			}
			RectTransform rectTransform = base.transform as RectTransform;
			if (rectTransform != null)
			{
				this._viewportRestraint.ChildSelected(rectTransform);
			}
		}

		[SerializeField]
		private RectTransform _overrideFocusTo;

		private ModioViewportRestraint _viewportRestraint;
	}
}
