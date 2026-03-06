using System;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[AddComponentMenu("XR/Locomotion/Legacy/Snap Turn Provider (Action-based)", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.ActionBasedSnapTurnProvider.html")]
	[Obsolete("ActionBasedSnapTurnProvider has been deprecated in version 3.0.0. Use SnapTurnProvider instead.")]
	public class ActionBasedSnapTurnProvider : SnapTurnProviderBase
	{
		public InputActionProperty leftHandSnapTurnAction
		{
			get
			{
				return this.m_LeftHandSnapTurnAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_LeftHandSnapTurnAction, value);
			}
		}

		public InputActionProperty rightHandSnapTurnAction
		{
			get
			{
				return this.m_RightHandSnapTurnAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_RightHandSnapTurnAction, value);
			}
		}

		protected void OnEnable()
		{
			this.m_LeftHandSnapTurnAction.EnableDirectAction();
			this.m_RightHandSnapTurnAction.EnableDirectAction();
		}

		protected void OnDisable()
		{
			this.m_LeftHandSnapTurnAction.DisableDirectAction();
			this.m_RightHandSnapTurnAction.DisableDirectAction();
		}

		protected override Vector2 ReadInput()
		{
			InputAction action = this.m_LeftHandSnapTurnAction.action;
			Vector2 a = (action != null) ? action.ReadValue<Vector2>() : Vector2.zero;
			InputAction action2 = this.m_RightHandSnapTurnAction.action;
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
		[Tooltip("The Input System Action that will be used to read Snap Turn data from the left hand controller. Must be a Value Vector2 Control.")]
		private InputActionProperty m_LeftHandSnapTurnAction = new InputActionProperty(new InputAction("Left Hand Snap Turn", InputActionType.Value, null, null, null, "Vector2"));

		[SerializeField]
		[Tooltip("The Input System Action that will be used to read Snap Turn data from the right hand controller. Must be a Value Vector2 Control.")]
		private InputActionProperty m_RightHandSnapTurnAction = new InputActionProperty(new InputAction("Right Hand Snap Turn", InputActionType.Value, null, null, null, "Vector2"));
	}
}
