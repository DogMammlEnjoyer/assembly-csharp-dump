using System;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class RectSizeConstraint : UIBehaviour
{
	protected virtual void LateUpdate()
	{
		if (this.target != null)
		{
			RectTransform rectTransform = (RectTransform)base.transform;
			rectTransform.sizeDelta = new Vector2(this.target.rect.width, this.target.rect.height);
			rectTransform.ForceUpdateRectTransforms();
		}
	}

	public RectTransform target;
}
