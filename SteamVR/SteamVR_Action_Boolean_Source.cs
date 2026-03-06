using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Action_Boolean_Source : SteamVR_Action_In_Source, ISteamVR_Action_Boolean, ISteamVR_Action_In_Source, ISteamVR_Action_Source
	{
		public event SteamVR_Action_Boolean.StateDownHandler onStateDown;

		public event SteamVR_Action_Boolean.StateUpHandler onStateUp;

		public event SteamVR_Action_Boolean.StateHandler onState;

		public event SteamVR_Action_Boolean.ActiveChangeHandler onActiveChange;

		public event SteamVR_Action_Boolean.ActiveChangeHandler onActiveBindingChange;

		public event SteamVR_Action_Boolean.ChangeHandler onChange;

		public event SteamVR_Action_Boolean.UpdateHandler onUpdate;

		public bool state
		{
			get
			{
				return this.active && this.actionData.bState;
			}
		}

		public bool stateDown
		{
			get
			{
				return this.active && this.actionData.bState && this.actionData.bChanged;
			}
		}

		public bool stateUp
		{
			get
			{
				return this.active && !this.actionData.bState && this.actionData.bChanged;
			}
		}

		public override bool changed
		{
			get
			{
				return this.active && this.actionData.bChanged;
			}
			protected set
			{
			}
		}

		public bool lastState
		{
			get
			{
				return this.lastActionData.bState;
			}
		}

		public bool lastStateDown
		{
			get
			{
				return this.lastActionData.bState && this.lastActionData.bChanged;
			}
		}

		public bool lastStateUp
		{
			get
			{
				return !this.lastActionData.bState && this.lastActionData.bChanged;
			}
		}

		public override bool lastChanged
		{
			get
			{
				return this.lastActionData.bChanged;
			}
			protected set
			{
			}
		}

		public override ulong activeOrigin
		{
			get
			{
				if (this.active)
				{
					return this.actionData.activeOrigin;
				}
				return 0UL;
			}
		}

		public override ulong lastActiveOrigin
		{
			get
			{
				return this.lastActionData.activeOrigin;
			}
		}

		public override bool active
		{
			get
			{
				return this.activeBinding && this.action.actionSet.IsActive(base.inputSource);
			}
		}

		public override bool activeBinding
		{
			get
			{
				return this.actionData.bActive;
			}
		}

		public override bool lastActive { get; protected set; }

		public override bool lastActiveBinding
		{
			get
			{
				return this.lastActionData.bActive;
			}
		}

		public override void Preinitialize(SteamVR_Action wrappingAction, SteamVR_Input_Sources forInputSource)
		{
			base.Preinitialize(wrappingAction, forInputSource);
			this.booleanAction = (SteamVR_Action_Boolean)wrappingAction;
		}

		public override void Initialize()
		{
			base.Initialize();
			if (SteamVR_Action_Boolean_Source.actionData_size == 0U)
			{
				SteamVR_Action_Boolean_Source.actionData_size = (uint)Marshal.SizeOf(typeof(InputDigitalActionData_t));
			}
		}

		public void RemoveAllListeners()
		{
			if (this.onStateDown != null)
			{
				Delegate[] invocationList = this.onStateDown.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate @delegate in invocationList)
					{
						this.onStateDown -= (SteamVR_Action_Boolean.StateDownHandler)@delegate;
					}
				}
			}
			if (this.onStateUp != null)
			{
				Delegate[] invocationList = this.onStateUp.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate delegate2 in invocationList)
					{
						this.onStateUp -= (SteamVR_Action_Boolean.StateUpHandler)delegate2;
					}
				}
			}
			if (this.onState != null)
			{
				Delegate[] invocationList = this.onState.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate delegate3 in invocationList)
					{
						this.onState -= (SteamVR_Action_Boolean.StateHandler)delegate3;
					}
				}
			}
		}

		public override void UpdateValue()
		{
			this.lastActionData = this.actionData;
			this.lastActive = this.active;
			EVRInputError digitalActionData = OpenVR.Input.GetDigitalActionData(this.action.handle, ref this.actionData, SteamVR_Action_Boolean_Source.actionData_size, this.inputSourceHandle);
			if (digitalActionData != EVRInputError.None)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"<b>[SteamVR]</b> GetDigitalActionData error (",
					this.action.fullPath,
					"): ",
					digitalActionData.ToString(),
					" handle: ",
					this.action.handle.ToString()
				}));
			}
			if (this.changed)
			{
				base.changedTime = Time.realtimeSinceStartup + this.actionData.fUpdateTime;
			}
			base.updateTime = Time.realtimeSinceStartup;
			if (this.active)
			{
				if (this.onStateDown != null && this.stateDown)
				{
					this.onStateDown(this.booleanAction, base.inputSource);
				}
				if (this.onStateUp != null && this.stateUp)
				{
					this.onStateUp(this.booleanAction, base.inputSource);
				}
				if (this.onState != null && this.state)
				{
					this.onState(this.booleanAction, base.inputSource);
				}
				if (this.onChange != null && this.changed)
				{
					this.onChange(this.booleanAction, base.inputSource, this.state);
				}
				if (this.onUpdate != null)
				{
					this.onUpdate(this.booleanAction, base.inputSource, this.state);
				}
			}
			if (this.onActiveBindingChange != null && this.lastActiveBinding != this.activeBinding)
			{
				this.onActiveBindingChange(this.booleanAction, base.inputSource, this.activeBinding);
			}
			if (this.onActiveChange != null && this.lastActive != this.active)
			{
				this.onActiveChange(this.booleanAction, base.inputSource, this.activeBinding);
			}
		}

		protected static uint actionData_size;

		protected InputDigitalActionData_t actionData;

		protected InputDigitalActionData_t lastActionData;

		protected SteamVR_Action_Boolean booleanAction;
	}
}
