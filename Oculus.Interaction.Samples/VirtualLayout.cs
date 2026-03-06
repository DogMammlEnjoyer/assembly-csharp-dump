using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class VirtualLayout : UIBehaviour
{
	protected override void OnEnable()
	{
		if (this._layoutParent == null)
		{
			return;
		}
		RectTransform[] componentsInChildren = this._layoutParent.gameObject.GetComponentsInChildren<RectTransform>();
		for (int i = 1; i < componentsInChildren.Length; i++)
		{
			RectTransform rectTransform = componentsInChildren[i];
			if (Application.isPlaying)
			{
				Object.Destroy(rectTransform.gameObject);
			}
			else
			{
				Object.DestroyImmediate(rectTransform.gameObject);
			}
		}
		RectTransform[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<RectTransform>();
		this._rectChildren = new List<RectTransform>();
		this._virtualLayoutChildren = new List<RectTransform>();
		for (int j = 1; j < componentsInChildren2.Length; j++)
		{
			RectTransform rectTransform2 = componentsInChildren2[j];
			if (!(rectTransform2.parent != (RectTransform)base.transform))
			{
				this._rectChildren.Add(rectTransform2);
				this.ResetChildTransform(rectTransform2);
				GameObject gameObject = new GameObject();
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				gameObject.name = rectTransform2.name;
				gameObject.AddComponent<RectTransform>();
				RectTransform rectTransform3 = (RectTransform)gameObject.transform;
				rectTransform3.SetParent(this._layoutParent, false);
				this.ResetChildTransform(rectTransform3);
				this._virtualLayoutChildren.Add(rectTransform3);
			}
		}
		this._layoutParent.ForceUpdateRectTransforms();
	}

	private void ResetChildTransform(RectTransform child)
	{
		child.localPosition = Vector3.zero;
		child.anchoredPosition = Vector2.zero;
		child.localScale = Vector3.one;
		child.localRotation = Quaternion.identity;
		child.anchorMin = Vector2.zero;
		child.anchorMax = Vector2.zero;
		child.pivot = new Vector2(0.5f, 0.5f);
	}

	protected override void OnDisable()
	{
		foreach (RectTransform rectTransform in this._virtualLayoutChildren)
		{
			if (Application.isPlaying)
			{
				Object.Destroy(rectTransform.gameObject);
			}
			else
			{
				Object.DestroyImmediate(rectTransform.gameObject);
			}
		}
	}

	private void LateUpdate()
	{
		if (this._layoutParent == null)
		{
			return;
		}
		((RectTransform)base.transform).anchoredPosition = this._layoutParent.anchoredPosition;
		for (int i = 0; i < this._virtualLayoutChildren.Count; i++)
		{
			RectTransform rectTransform = this._rectChildren[i];
			RectTransform rectTransform2 = this._virtualLayoutChildren[i];
			if (Application.isPlaying)
			{
				rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, rectTransform2.anchoredPosition, this.animationSpeed * Time.deltaTime);
				rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, rectTransform2.sizeDelta, this.animationSpeed * Time.deltaTime);
			}
			else
			{
				rectTransform.anchoredPosition = rectTransform2.anchoredPosition + this._layoutParent.anchoredPosition;
				rectTransform.sizeDelta = rectTransform2.sizeDelta;
			}
		}
	}

	public void InjectAllVirtualLayoutElement(RectTransform layoutParent)
	{
		this._layoutParent = layoutParent;
	}

	public float animationSpeed;

	[SerializeField]
	private RectTransform _layoutParent;

	private List<RectTransform> _rectChildren;

	private List<RectTransform> _virtualLayoutChildren;
}
