using System;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[AddComponentMenu("XR/Locomotion/Legacy/Continuous Move Provider (Action-based)", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.ActionBasedContinuousMoveProvider.html")]
	[Obsolete("ActionBasedContinuousMoveProvider has been deprecated in version 3.0.0. Use ContinuousMoveProvider instead.", false)]
	public class ActionBasedContinuousMoveProvider : ContinuousMoveProviderBase
	{
		public InputActionProperty leftHandMoveAction
		{
			get
			{
				return this.m_LeftHandMoveAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_LeftHandMoveAction, value);
			}
		}

		public InputActionProperty rightHandMoveAction
		{
			get
			{
				return this.m_RightHandMoveAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_RightHandMoveAction, value);
			}
		}

		protected void OnEnable()
		{
			this.m_LeftHandMoveAction.EnableDirectAction();
			this.m_RightHandMoveAction.EnableDirectAction();
		}

		protected void OnDisable()
		{
			this.m_LeftHandMoveAction.DisableDirectAction();
			this.m_RightHandMoveAction.DisableDirectAction();
		}

		protected override Vector2 ReadInput()
		{
			InputAction action = this.m_LeftHandMoveAction.action;
			Vector2 a = (action != null) ? action.ReadValue<Vector2>() : Vector2.zero;
			InputAction action2 = this.m_RightHandMoveAction.action;
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
		[Tooltip("The Input System Action that will be used to read Move data from the left hand controller. Must be a Value Vector2 Control.")]
		private InputActionProperty m_LeftHandMoveAction = new InputActionProperty(new InputAction("Left Hand Move", InputActionType.Value, null, null, null, "Vector2"));

		[SerializeField]
		[Tooltip("The Input System Action that will be used to read Move data from the right hand controller. Must be a Value Vector2 Control.")]
		private InputActionProperty m_RightHandMoveAction = new InputActionProperty(new InputAction("Right Hand Move", InputActionType.Value, null, null, null, "Vector2"));
	}
}
