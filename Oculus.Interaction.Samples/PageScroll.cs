using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageScroll : UIBehaviour
{
	public void SetPageIndex(int pageIndex)
	{
		int num = (pageIndex < 0) ? 0 : ((pageIndex > this._pages.Count - 1) ? (this._pages.Count - 1) : pageIndex);
		if (this._pageIndex != num)
		{
			this._pageIndex = num;
			this._pages[this._pageIndex].toggle.isOn = true;
		}
	}

	public void ScrollPage(int direction)
	{
		int pageIndex = this._pageIndex + direction;
		this.SetPageIndex(pageIndex);
	}

	protected override void OnEnable()
	{
		using (List<PageScroll.Page>.Enumerator enumerator = this._pages.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				PageScroll.Page page = enumerator.Current;
				page.toggle.onValueChanged.AddListener(delegate(bool <p0>)
				{
					this.ActiveToggleChanged(page.toggle);
				});
			}
		}
	}

	protected override void OnDisable()
	{
		foreach (PageScroll.Page page in this._pages)
		{
			page.toggle.onValueChanged.RemoveAllListeners();
		}
	}

	private void ActiveToggleChanged(Toggle toggle)
	{
		if (toggle == null)
		{
			return;
		}
		if (!toggle.isOn)
		{
			return;
		}
		int num = this._pages.FindIndex((PageScroll.Page page) => page.toggle == toggle);
		if (num < 0)
		{
			return;
		}
		this._pageIndex = num;
	}

	protected override void Start()
	{
		base.StartCoroutine(this.LateStart());
	}

	private IEnumerator LateStart()
	{
		yield return null;
		if (this._pages == null)
		{
			yield break;
		}
		this._pages[0].toggle.isOn = true;
		yield break;
	}

	protected virtual void Update()
	{
		this._pageAnim = Mathf.Lerp(this._pageAnim, (float)this._pageIndex, this.animationSpeed * Time.deltaTime);
		this._pageAnim = Mathf.Clamp(this._pageAnim, 0f, (float)(this._pages.Count - 1));
		this.UpdateVisial();
	}

	private void UpdateVisial()
	{
		if (Mathf.Abs(this._pageAnim - (float)this._pageIndex) < 0.005f)
		{
			Vector2 anchoredPosition = this._pages[this._pageIndex].container.anchoredPosition;
			this._pages[this._pageIndex].canvasGroup.alpha = 1f;
			this.SetOtherPagesTransparent(this._pageIndex, -1);
			this._contentContainer.anchoredPosition = anchoredPosition * new Vector2(-1f, 1f);
			return;
		}
		float num = Mathf.Clamp(this._pageAnim, 0f, (float)(this._pages.Count - 1));
		float num2 = Mathf.Floor(num);
		int num3 = (int)Mathf.Ceil(num);
		int num4 = (int)num2;
		int num5 = num3;
		float num6 = num - num2;
		Vector2 anchoredPosition2 = this._pages[num4].container.anchoredPosition;
		Vector2 anchoredPosition3 = this._pages[num5].container.anchoredPosition;
		this.SetOtherPagesTransparent(num4, num5);
		this._pages[num4].canvasGroup.alpha = this.alphaTransitionCurve.Evaluate(1f - num6);
		this._pages[num5].canvasGroup.alpha = this.alphaTransitionCurve.Evaluate(num6);
		this._contentContainer.anchoredPosition = Vector2.Lerp(anchoredPosition2, anchoredPosition3, num6) * new Vector2(-1f, 1f);
	}

	private void SetOtherPagesTransparent(int index0, int index1)
	{
		for (int i = 0; i < this._pages.Count; i++)
		{
			if (i != index0 && i != index1 && !(this._pages[i].canvasGroup == null))
			{
				this._pages[i].canvasGroup.alpha = 0f;
			}
		}
	}

	public void InjectAllPageScroll(ToggleGroup toggleGroup, RectTransform contentContainer, List<PageScroll.Page> pages, int pageIndex)
	{
		this.InjectToggleGroup(toggleGroup);
		this.InjectContentContainer(contentContainer);
		this.InjectPages(pages);
		this.InjectPageIndex(pageIndex);
	}

	public void InjectToggleGroup(ToggleGroup toggleGroup)
	{
		this._toggleGroup = toggleGroup;
	}

	public void InjectContentContainer(RectTransform contentContainer)
	{
		this._contentContainer = contentContainer;
	}

	public void InjectPages(List<PageScroll.Page> pages)
	{
		this._pages = pages;
	}

	public void InjectPageIndex(int pageIndex)
	{
		this._pageIndex = pageIndex;
	}

	[SerializeField]
	private ToggleGroup _toggleGroup;

	[SerializeField]
	private RectTransform _contentContainer;

	[SerializeField]
	private List<PageScroll.Page> _pages;

	[SerializeField]
	private int _pageIndex;

	public float animationSpeed;

	public AnimationCurve alphaTransitionCurve;

	private float _pageAnim;

	[Serializable]
	public struct Page
	{
		public Toggle toggle;

		public RectTransform container;

		public CanvasGroup canvasGroup;
	}
}
