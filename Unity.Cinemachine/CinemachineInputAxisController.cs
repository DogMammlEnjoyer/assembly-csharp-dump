using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace Unity.Cinemachine
{
	[ExecuteAlways]
	[SaveDuringPlay]
	[AddComponentMenu("Cinemachine/Helpers/Cinemachine Input Axis Controller")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineInputAxisController.html")]
	public class CinemachineInputAxisController : InputAxisControllerBase<CinemachineInputAxisController.Reader>
	{
		protected override void Reset()
		{
			base.Reset();
			this.PlayerIndex = -1;
			this.AutoEnableInputs = true;
		}

		protected override void InitializeControllerDefaultsForAxis(in IInputAxisOwner.AxisDescriptor axis, InputAxisControllerBase<CinemachineInputAxisController.Reader>.Controller controller)
		{
			CinemachineInputAxisController.SetControlDefaultsForAxis setControlDefaults = CinemachineInputAxisController.SetControlDefaults;
			if (setControlDefaults == null)
			{
				return;
			}
			setControlDefaults(axis, ref controller);
		}

		private void Update()
		{
			if (Application.isPlaying)
			{
				base.UpdateControllers();
			}
		}

		[Tooltip("Leave this at -1 for single-player games.  For multi-player games, set this to be the player index, and the actions will be read from that player's controls")]
		public int PlayerIndex = -1;

		[Tooltip("If set, Input Actions will be auto-enabled at start")]
		public bool AutoEnableInputs = true;

		internal static CinemachineInputAxisController.SetControlDefaultsForAxis SetControlDefaults;

		public CinemachineInputAxisController.Reader.ControlValueReader ReadControlValueOverride;

		internal delegate void SetControlDefaultsForAxis(in IInputAxisOwner.AxisDescriptor axis, ref InputAxisControllerBase<CinemachineInputAxisController.Reader>.Controller controller);

		[Serializable]
		public sealed class Reader : IInputAxisReader
		{
			public float GetValue(Object context, IInputAxisOwner.AxisDescriptor.Hints hint)
			{
				float num = 0f;
				if (this.InputAction != null)
				{
					CinemachineInputAxisController cinemachineInputAxisController = context as CinemachineInputAxisController;
					if (cinemachineInputAxisController != null)
					{
						num = this.ResolveAndReadInputAction(cinemachineInputAxisController, hint) * this.Gain;
					}
				}
				if (Time.deltaTime <= 0f || !this.CancelDeltaTime)
				{
					return num;
				}
				return num / Time.deltaTime;
			}

			private float ResolveAndReadInputAction(CinemachineInputAxisController context, IInputAxisOwner.AxisDescriptor.Hints hint)
			{
				if (this.m_CachedAction != null && this.InputAction.action.id != this.m_CachedAction.id)
				{
					this.m_CachedAction = null;
				}
				if (this.m_CachedAction == null)
				{
					this.m_CachedAction = this.InputAction.action;
					if (context.PlayerIndex != -1)
					{
						InputUser inputUser = InputUser.all[context.PlayerIndex];
						this.m_CachedAction = CinemachineInputAxisController.Reader.<ResolveAndReadInputAction>g__GetFirstMatch|6_0(inputUser, this.InputAction);
					}
					if (context.AutoEnableInputs && this.m_CachedAction != null)
					{
						this.m_CachedAction.Enable();
					}
				}
				if (this.m_CachedAction != null && this.m_CachedAction.enabled != this.InputAction.action.enabled)
				{
					if (this.InputAction.action.enabled)
					{
						this.m_CachedAction.Enable();
					}
					else
					{
						this.m_CachedAction.Disable();
					}
				}
				if (this.m_CachedAction == null)
				{
					return 0f;
				}
				if (context.ReadControlValueOverride != null)
				{
					return context.ReadControlValueOverride(this.m_CachedAction, hint, context, new CinemachineInputAxisController.Reader.ControlValueReader(this.ReadInput));
				}
				return this.ReadInput(this.m_CachedAction, hint, context, null);
			}

			private float ReadInput(InputAction action, IInputAxisOwner.AxisDescriptor.Hints hint, Object context, CinemachineInputAxisController.Reader.ControlValueReader defaultReader)
			{
				InputControl activeControl = action.activeControl;
				if (activeControl != null)
				{
					try
					{
						if (activeControl.valueType == typeof(Vector2) || action.expectedControlType == "Vector2")
						{
							Vector2 vector = action.ReadValue<Vector2>();
							return (hint == IInputAxisOwner.AxisDescriptor.Hints.Y) ? vector.y : vector.x;
						}
						return action.ReadValue<float>();
					}
					catch (InvalidOperationException)
					{
						Debug.LogError(string.Concat(new string[]
						{
							"An action in ",
							context.name,
							" is mapped to a ",
							activeControl.valueType.Name,
							" control.  CinemachineInputAxisController.Reader can only handle float or Vector2 types.  To handle other types you can install a custom handler for CinemachineInputAxisController.ReadControlValueOverride."
						}));
					}
				}
				return 0f;
			}

			[CompilerGenerated]
			internal static InputAction <ResolveAndReadInputAction>g__GetFirstMatch|6_0(in InputUser user, InputActionReference aRef)
			{
				InputUser inputUser = user;
				IEnumerator<InputAction> enumerator = inputUser.actions.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.id == aRef.action.id)
					{
						return enumerator.Current;
					}
				}
				return null;
			}

			[Tooltip("Action for the Input package (if used).")]
			public InputActionReference InputAction;

			[Tooltip("The input value is multiplied by this amount prior to processing.  Controls the input power.  Set it to a negative value to invert the input")]
			public float Gain = 1f;

			internal InputAction m_CachedAction;

			[Tooltip("Enable this if the input value is inherently dependent on frame time.  For example, mouse deltas will naturally be bigger for longer frames, so in this case the default deltaTime scaling should be canceled.")]
			public bool CancelDeltaTime;

			public delegate float ControlValueReader(InputAction action, IInputAxisOwner.AxisDescriptor.Hints hint, Object context, CinemachineInputAxisController.Reader.ControlValueReader defaultReader);
		}
	}
}
