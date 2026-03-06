using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace Unity.Cinemachine
{
	[Obsolete("CinemachineInputProvider has been deprecated. Use InputAxisController instead.")]
	[AddComponentMenu("")]
	public class CinemachineInputProvider : MonoBehaviour, AxisState.IInputAxisProvider
	{
		public virtual float GetAxisValue(int axis)
		{
			if (base.enabled)
			{
				InputAction inputAction = this.ResolveForPlayer(axis, (axis == 2) ? this.ZAxis : this.XYAxis);
				if (inputAction != null)
				{
					switch (axis)
					{
					case 0:
						return inputAction.ReadValue<Vector2>().x;
					case 1:
						return inputAction.ReadValue<Vector2>().y;
					case 2:
						return inputAction.ReadValue<float>();
					}
				}
			}
			return 0f;
		}

		protected InputAction ResolveForPlayer(int axis, InputActionReference actionRef)
		{
			if (axis < 0 || axis >= 3)
			{
				return null;
			}
			if (actionRef == null || actionRef.action == null)
			{
				return null;
			}
			if (this.m_cachedActions == null || this.m_cachedActions.Length != 3)
			{
				this.m_cachedActions = new InputAction[3];
			}
			if (this.m_cachedActions[axis] != null && actionRef.action.id != this.m_cachedActions[axis].id)
			{
				this.m_cachedActions[axis] = null;
			}
			if (this.m_cachedActions[axis] == null)
			{
				this.m_cachedActions[axis] = actionRef.action;
				if (this.PlayerIndex != -1)
				{
					InputAction[] cachedActions = this.m_cachedActions;
					InputUser inputUser = InputUser.all[this.PlayerIndex];
					cachedActions[axis] = CinemachineInputProvider.<ResolveForPlayer>g__GetFirstMatch|7_0(inputUser, actionRef);
				}
				if (this.AutoEnableInputs && actionRef != null && actionRef.action != null)
				{
					actionRef.action.Enable();
				}
			}
			if (this.m_cachedActions[axis] != null && this.m_cachedActions[axis].enabled != actionRef.action.enabled)
			{
				if (actionRef.action.enabled)
				{
					this.m_cachedActions[axis].Enable();
				}
				else
				{
					this.m_cachedActions[axis].Disable();
				}
			}
			return this.m_cachedActions[axis];
		}

		protected virtual void OnDisable()
		{
			this.m_cachedActions = null;
		}

		[CompilerGenerated]
		internal static InputAction <ResolveForPlayer>g__GetFirstMatch|7_0(in InputUser user, InputActionReference aRef)
		{
			InputUser inputUser = user;
			return inputUser.actions.First((InputAction x) => x.id == aRef.action.id);
		}

		[Tooltip("Leave this at -1 for single-player games.  For multi-player games, set this to be the player index, and the actions will be read from that player's controls")]
		public int PlayerIndex = -1;

		[Tooltip("If set, Input Actions will be auto-enabled at start")]
		public bool AutoEnableInputs = true;

		[Tooltip("Vector2 action for XY movement")]
		public InputActionReference XYAxis;

		[Tooltip("Float action for Z movement")]
		public InputActionReference ZAxis;

		private const int NUM_AXES = 3;

		private InputAction[] m_cachedActions;
	}
}
