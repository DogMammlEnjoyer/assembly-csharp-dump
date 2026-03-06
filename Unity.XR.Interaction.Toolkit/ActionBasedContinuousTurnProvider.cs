using System;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[AddComponentMenu("XR/Locomotion/Legacy/Continuous Turn Provider (Action-based)", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.ActionBasedContinuousTurnProvider.html")]
	[Obsolete("ActionBasedContinuousTurnProvider has been deprecated in version 3.0.0. Use ContinuousTurnProvider instead.")]
	public class ActionBasedContinuousTurnProvider : ContinuousTurnProviderBase
	{
		public InputActionProperty leftHandTurnAction
		{
			get
			{
				return this.m_LeftHandTurnAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_LeftHandTurnAction, value);
			}
		}

		public InputActionProperty rightHandTurnAction
		{
			get
			{
				return this.m_RightHandTurnAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_RightHandTurnAction, value);
			}
		}

		protected void OnEnable()
		{
			this.m_LeftHandTurnAction.EnableDirectAction();
			this.m_RightHandTurnAction.EnableDirectAction();
		}

		protected void OnDisable()
		{
			this.m_LeftHandTurnAction.DisableDirectAction();
			this.m_RightHandTurnAction.DisableDirectAction();
		}

		protected override Vector2 ReadInput()
		{
			InputAction action = this.m_LeftHandTurnAction.action;
			Vector2 a = (action != null) ? action.ReadValue<Vector2>() : Vector2.zero;
			InputAction action2 = this.m_RightHandTurnAction.action;
			Vector2 b = (action2 != null) ? action2.ReadValue<Vector2>() : Vector2.zero;
			return a + b;
		}

		private void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
		{
			if (Application.isPlaying)
			{
				property.DisableDirectAction();
			}
			property = value;
			if (Application.isPlaying && base.isActiveAndEnabled)
			{
				property.EnableDirectAction();
			}
		}

		[SerializeField]
		[Tooltip("The Input System Action that will be used to read Turn data from the left hand controller. Must be a Value Vector2 Control.")]
		private InputActionProperty m_LeftHandTurnAction = new InputActionProperty(new InputAction("Left Hand Turn", InputActionType.Value, null, null, null, "Vector2"));

		[SerializeField]
		[Tooltip("The Input System Action that will be used to read Turn data from the right hand controller. Must be a Value Vector2 Control.")]
		private InputActionProperty m_RightHandTurnAction = new InputActionProperty(new InputAction("Right Hand Turn", InputActionType.Value, null, null, null, "Vector2"));
	}
}
