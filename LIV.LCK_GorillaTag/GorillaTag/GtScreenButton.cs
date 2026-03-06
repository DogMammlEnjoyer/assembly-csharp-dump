using System;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.GorillaTag
{
	public class GtScreenButton : MonoBehaviour
	{
		private void Start()
		{
			this._currentDefaultColor = this._defaultColor;
			this._currentActiveColor = this._activeColor;
		}

		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				this._isActive = value;
				this._currentDefaultColor = (value ? this._activeColor : this._defaultColor);
				this._currentActiveColor = (value ? this._activeColor : this._defaultColor);
				this._iconRenderer.color = this._currentDefaultColor;
			}
		}

		public void OnTapStarted()
		{
			if (this._isDisabled)
			{
				return;
			}
			UnityEvent unityEvent = this.onTapStarted;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this._iconRenderer.color = this._currentActiveColor;
		}

		public void OnTapEnded()
		{
			if (this._isDisabled)
			{
				return;
			}
			UnityEvent unityEvent = this.onTapEnded;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this._iconRenderer.color = this._currentDefaultColor;
		}

		public void DisableForDuration(float duration)
		{
			this._isDisabled = true;
			this._iconRenderer.enabled = false;
			this._triggerProcessor.BlockTapping();
			base.Invoke("ReEnableButton", duration);
		}

		private void ReEnableButton()
		{
			this._iconRenderer.color = this._currentDefaultColor;
			this._iconRenderer.enabled = true;
			this._triggerProcessor.ResetToDefault();
			this._isDisabled = false;
		}

		[SerializeField]
		private Color _defaultColor;

		[SerializeField]
		private Color _activeColor;

		[SerializeField]
		private SpriteRenderer _iconRenderer;

		[SerializeField]
		private GtColliderTriggerProcessor _triggerProcessor;

		[Header("Events")]
		public UnityEvent onTapStarted;

		public UnityEvent onTapEnded;

		private Color _currentDefaultColor;

		private Color _currentActiveColor;

		private bool _isDisabled;

		private bool _isActive;
	}
}
