using System;
using Liv.Lck.Settings;
using Liv.Lck.Tablet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Liv.Lck.UI
{
	public class LckDoubleButtonTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
	{
		public event Action<bool> OnDown;

		public event Action<bool> OnEnter;

		public event Action<bool, bool> OnUp;

		public event Action<bool> OnExit;

		public void SetBackgroundColor(Color color)
		{
			this._background.color = color;
		}

		public void SetIconColor(Color color)
		{
			this._icon.color = color;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!this._isUsingColliders)
			{
				Action<bool> onDown = this.OnDown;
				if (onDown == null)
				{
					return;
				}
				onDown(this._isIncreaseButton);
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!this._isUsingColliders)
			{
				Action<bool> onEnter = this.OnEnter;
				if (onEnter == null)
				{
					return;
				}
				onEnter(this._isIncreaseButton);
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (!this._isUsingColliders)
			{
				Action<bool, bool> onUp = this.OnUp;
				if (onUp == null)
				{
					return;
				}
				onUp(this._isIncreaseButton, false);
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (!this._isUsingColliders)
			{
				Action<bool> onExit = this.OnExit;
				if (onExit == null)
				{
					return;
				}
				onExit(this._isIncreaseButton);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.tag == LckSettings.Instance.TriggerEnterTag && this.IsValidTap(other.ClosestPoint(base.transform.position)) && !LCKCameraController.ColliderButtonsInUse)
			{
				LCKCameraController.ColliderButtonsInUse = true;
				this._hasCollided = true;
				Action<bool> onDown = this.OnDown;
				if (onDown == null)
				{
					return;
				}
				onDown(this._isIncreaseButton);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.gameObject.tag == LckSettings.Instance.TriggerEnterTag && this._hasCollided)
			{
				Action<bool, bool> onUp = this.OnUp;
				if (onUp != null)
				{
					onUp(this._isIncreaseButton, true);
				}
				this._hasCollided = false;
				LCKCameraController.ColliderButtonsInUse = false;
			}
		}

		private bool IsValidTap(Vector3 tapPosition)
		{
			Vector3 to = tapPosition - base.transform.position;
			return Vector3.Angle(-base.transform.forward, to) < 65f;
		}

		[SerializeField]
		private bool _isUsingColliders;

		[SerializeField]
		private bool _isIncreaseButton;

		[SerializeField]
		private Image _background;

		[SerializeField]
		private Image _icon;

		private bool _hasCollided;
	}
}
