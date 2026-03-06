using System;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.Samples
{
	public class CarouselView : MonoBehaviour
	{
		public int CurrentChildIndex
		{
			get
			{
				return this._currentChildIndex;
			}
		}

		public RectTransform ContentArea
		{
			get
			{
				return this._content;
			}
		}

		protected virtual void Start()
		{
		}

		public void ScrollRight()
		{
			if (this._content.childCount <= 1)
			{
				return;
			}
			if (this._currentChildIndex > 0)
			{
				RectTransform currentChild = this.GetCurrentChild();
				this._content.GetChild(0).SetAsLastSibling();
				LayoutRebuilder.ForceRebuildLayoutImmediate(this._content);
				this.ScrollToChild(currentChild, 1f);
			}
			else
			{
				this._currentChildIndex++;
			}
			this._scrollVal = Time.time;
		}

		public void ScrollLeft()
		{
			if (this._content.childCount <= 1)
			{
				return;
			}
			if (this._currentChildIndex < this._content.childCount - 1)
			{
				RectTransform currentChild = this.GetCurrentChild();
				this._content.GetChild(this._content.childCount - 1).SetAsFirstSibling();
				LayoutRebuilder.ForceRebuildLayoutImmediate(this._content);
				this.ScrollToChild(currentChild, 1f);
			}
			else
			{
				this._currentChildIndex--;
			}
			this._scrollVal = Time.time;
		}

		private RectTransform GetCurrentChild()
		{
			return this._content.GetChild(this._currentChildIndex) as RectTransform;
		}

		private void ScrollToChild(RectTransform child, float amount01)
		{
			if (child == null)
			{
				return;
			}
			amount01 = Mathf.Clamp01(amount01);
			Vector3 b = this._viewport.TransformPoint(this._viewport.rect.center);
			Vector3 b2 = child.TransformPoint(child.rect.center) - b;
			if (b2.sqrMagnitude > 1E-45f)
			{
				Vector3 b3 = this._content.position - b2;
				float t = Mathf.Clamp01(this._easeCurve.Evaluate(amount01));
				this._content.position = Vector3.Lerp(this._content.position, b3, t);
			}
		}

		protected virtual void Update()
		{
			this._currentChildIndex = Mathf.Clamp(this._currentChildIndex, 0, this._content.childCount - 1);
			bool flag = this._content.childCount > 0;
			if (flag)
			{
				RectTransform currentChild = this.GetCurrentChild();
				this.ScrollToChild(currentChild, Time.time - this._scrollVal);
			}
			if (this._emptyCarouselVisuals != null)
			{
				this._emptyCarouselVisuals.SetActive(!flag);
			}
		}

		[SerializeField]
		private RectTransform _viewport;

		[SerializeField]
		private RectTransform _content;

		[SerializeField]
		private AnimationCurve _easeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[SerializeField]
		[Optional]
		private GameObject _emptyCarouselVisuals;

		private int _currentChildIndex;

		private float _scrollVal;
	}
}
