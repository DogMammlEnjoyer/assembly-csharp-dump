using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Modio.Unity.UI.Input
{
	public class ModioUIScrollViewControllerInput : Selectable
	{
		protected override void Awake()
		{
			base.Awake();
			this._scrollRect = base.GetComponent<ScrollRect>();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this._resetPositionOnEnable)
			{
				this._scrollRect.verticalNormalizedPosition = 1f;
			}
		}

		private void Update()
		{
			this._cachedPointerEventData.scrollDelta = ModioUIInput.GetRawCursor() * (this._inputSpeed * Time.unscaledDeltaTime);
			this._scrollRect.OnScroll(this._cachedPointerEventData);
		}

		private ScrollRect _scrollRect;

		private readonly PointerEventData _cachedPointerEventData = new PointerEventData(null);

		[SerializeField]
		private float _inputSpeed = 100f;

		[SerializeField]
		private bool _resetPositionOnEnable = true;
	}
}
