using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Meta.XR.BuildingBlocks
{
	public class ControllerButtonsMapper : MonoBehaviour
	{
		public List<ControllerButtonsMapper.ButtonClickAction> ButtonClickActions
		{
			get
			{
				return this._buttonClickActions;
			}
			set
			{
				this._buttonClickActions = value;
			}
		}

		private void OnEnable()
		{
			foreach (ControllerButtonsMapper.ButtonClickAction buttonClickAction in this.ButtonClickActions)
			{
				if (!(buttonClickAction.InputActionReference == null))
				{
					buttonClickAction.InputActionReference.action.Enable();
					buttonClickAction.InputActionReference.action.performed += buttonClickAction.OnCallbackWithContext;
				}
			}
		}

		private void OnDisable()
		{
			foreach (ControllerButtonsMapper.ButtonClickAction buttonClickAction in this.ButtonClickActions)
			{
				if (!(buttonClickAction.InputActionReference == null))
				{
					buttonClickAction.InputActionReference.action.Disable();
					buttonClickAction.InputActionReference.action.performed -= buttonClickAction.OnCallbackWithContext;
				}
			}
		}

		private void Update()
		{
			foreach (ControllerButtonsMapper.ButtonClickAction buttonClickAction in this.ButtonClickActions)
			{
				if (ControllerButtonsMapper.IsActionTriggered(buttonClickAction))
				{
					UnityEvent callback = buttonClickAction.Callback;
					if (callback != null)
					{
						callback.Invoke();
					}
				}
			}
		}

		private static bool IsActionTriggered(ControllerButtonsMapper.ButtonClickAction buttonClickAction)
		{
			return ControllerButtonsMapper.IsLegacyInputActionTriggered(buttonClickAction.ButtonMode, buttonClickAction.Button) || ControllerButtonsMapper.IsNewInputSystemActionTriggered(buttonClickAction);
		}

		private static bool IsLegacyInputActionTriggered(ControllerButtonsMapper.ButtonClickAction.ButtonClickMode buttonMode, OVRInput.Button button)
		{
			return false;
		}

		private static bool IsNewInputSystemActionTriggered(ControllerButtonsMapper.ButtonClickAction buttonClickAction)
		{
			return buttonClickAction.InputActionReference != null && buttonClickAction.InputActionReference.action.triggered;
		}

		[SerializeField]
		private List<ControllerButtonsMapper.ButtonClickAction> _buttonClickActions;

		internal const bool UseNewInputSystem = true;

		internal const bool UseLegacyInputSystem = false;

		[Serializable]
		public struct ButtonClickAction
		{
			public void OnCallbackWithContext(InputAction.CallbackContext callbackContext)
			{
				UnityEvent<InputAction.CallbackContext> callbackWithContext = this.CallbackWithContext;
				if (callbackWithContext == null)
				{
					return;
				}
				callbackWithContext.Invoke(callbackContext);
			}

			public string Title;

			public OVRInput.Button Button;

			public ControllerButtonsMapper.ButtonClickAction.ButtonClickMode ButtonMode;

			public InputActionReference InputActionReference;

			public UnityEvent<InputAction.CallbackContext> CallbackWithContext;

			public UnityEvent Callback;

			public enum ButtonClickMode
			{
				OnButtonUp,
				OnButtonDown,
				OnButton
			}
		}
	}
}
