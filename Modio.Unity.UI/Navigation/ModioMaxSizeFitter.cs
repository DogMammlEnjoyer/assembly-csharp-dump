using System;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Navigation
{
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	public class ModioMaxSizeFitter : MonoBehaviour, ILayoutElement
	{
		private float GetPreferredSize(RectTransform.Axis axis)
		{
			float num = this._maxSize[(int)axis];
			if (num < 0.01f || this.layoutPriority < 0)
			{
				return -1f;
			}
			RectTransform rect = (RectTransform)base.transform;
			int layoutPriority = this.layoutPriority;
			this._calculatingNestedSize = true;
			float preferredSize = LayoutUtility.GetPreferredSize(rect, (int)axis);
			this._calculatingNestedSize = false;
			return Mathf.Min(preferredSize, num);
		}

		public void CalculateLayoutInputHorizontal()
		{
		}

		public void CalculateLayoutInputVertical()
		{
		}

		public float minWidth
		{
			get
			{
				return -1f;
			}
		}

		public float preferredWidth
		{
			get
			{
				return this.GetPreferredSize(RectTransform.Axis.Horizontal);
			}
		}

		public float flexibleWidth
		{
			get
			{
				return -1f;
			}
		}

		public float minHeight
		{
			get
			{
				return -1f;
			}
		}

		public float preferredHeight
		{
			get
			{
				return this.GetPreferredSize(RectTransform.Axis.Vertical);
			}
		}

		public float flexibleHeight
		{
			get
			{
				return -1f;
			}
		}

		public int layoutPriority
		{
			get
			{
				if (!this._calculatingNestedSize)
				{
					return this._layoutPriority;
				}
				return -1;
			}
		}

		[SerializeField]
		[Tooltip("Leaving an axis at 0 will ignore it")]
		private Vector2 _maxSize;

		[SerializeField]
		private int _layoutPriority = 10;

		private bool _calculatingNestedSize;
	}
}
