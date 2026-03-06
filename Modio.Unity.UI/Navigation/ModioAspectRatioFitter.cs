using System;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Navigation
{
	[ExecuteAlways]
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	public class ModioAspectRatioFitter : MonoBehaviour, ILayoutSelfController, ILayoutController
	{
		private void OnEnable()
		{
			this.UpdateRect();
		}

		private void OnRectTransformDimensionsChange()
		{
			this.UpdateRect();
		}

		private void Update()
		{
			if (this._delayedSetDirty)
			{
				this._delayedSetDirty = false;
				this.UpdateRect();
			}
		}

		private void OnValidate()
		{
			this._delayedSetDirty = true;
		}

		private void UpdateRect()
		{
			this._tracker.Clear();
			RectTransform rectTransform = (RectTransform)base.transform;
			Vector2 vector = ((RectTransform)rectTransform.parent).rect.size - new Vector2((float)this._margin.horizontal, (float)this._margin.vertical);
			if (this._maxSize.x > 1f)
			{
				vector.x = Mathf.Min(vector.x, this._maxSize.x);
			}
			if (this._maxSize.y > 1f)
			{
				vector.y = Mathf.Min(vector.y, this._maxSize.y);
			}
			Vector2 vector2 = vector - this._additionalPadding;
			if (vector2.y * this._aspectRatio < vector2.x)
			{
				vector2.x = vector2.y * this._aspectRatio;
			}
			else
			{
				vector2.y = vector2.x / this._aspectRatio;
			}
			vector = vector2 + this._additionalPadding;
			this._tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDelta);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, vector.x);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, vector.y);
		}

		public void SetLayoutHorizontal()
		{
		}

		public void SetLayoutVertical()
		{
		}

		[SerializeField]
		private float _aspectRatio = 1.7777778f;

		[SerializeField]
		private RectOffset _margin;

		[SerializeField]
		private Vector2 _additionalPadding;

		[SerializeField]
		private Vector2 _maxSize;

		private DrivenRectTransformTracker _tracker;

		private bool _delayedSetDirty;
	}
}
