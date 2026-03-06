using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Action_Vector3_Source : SteamVR_Action_In_Source, ISteamVR_Action_Vector3, ISteamVR_Action_In_Source, ISteamVR_Action_Source
	{
		public event SteamVR_Action_Vector3.AxisHandler onAxis;

		public event SteamVR_Action_Vector3.ActiveChangeHandler onActiveChange;

		public event SteamVR_Action_Vector3.ActiveChangeHandler onActiveBindingChange;

		public event SteamVR_Action_Vector3.ChangeHandler onChange;

		public event SteamVR_Action_Vector3.UpdateHandler onUpdate;

		public Vector3 axis { get; protected set; }

		public Vector3 lastAxis { get; protected set; }

		public Vector3 delta { get; protected set; }

		public Vector3 lastDelta { get; protected set; }

		public override bool changed { get; protected set; }

		public override bool lastChanged { get; protected set; }

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
			this.vector3Action = (SteamVR_Action_Vector3)wrappingAction;
		}

		public override void Initialize()
		{
			base.Initialize();
			if (SteamVR_Action_Vector3_Source.actionData_size == 0U)
			{
				SteamVR_Action_Vector3_Source.actionData_size = (uint)Marshal.SizeOf(typeof(InputAnalogActionData_t));
			}
		}

		public void RemoveAllListeners()
		{
			if (this.onAxis != null)
			{
				Delegate[] invocationList = this.onAxis.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate @delegate in invocationList)
					{
						this.onAxis -= (SteamVR_Action_Vector3.AxisHandler)@delegate;
					}
				}
			}
			if (this.onUpdate != null)
			{
				Delegate[] invocationList = this.onUpdate.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate delegate2 in invocationList)
					{
						this.onUpdate -= (SteamVR_Action_Vector3.UpdateHandler)delegate2;
					}
				}
			}
			if (this.onChange != null)
			{
				Delegate[] invocationList = this.onChange.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate delegate3 in invocationList)
					{
						this.onChange -= (SteamVR_Action_Vector3.ChangeHandler)delegate3;
					}
				}
			}
		}

		public override void UpdateValue()
		{
			this.lastActionData = this.actionData;
			this.lastActive = this.active;
			this.lastAxis = this.axis;
			this.lastDelta = this.delta;
			EVRInputError analogActionData = OpenVR.Input.GetAnalogActionData(base.handle, ref this.actionData, SteamVR_Action_Vector3_Source.actionData_size, SteamVR_Input_Source.GetHandle(base.inputSource));
			if (analogActionData != EVRInputError.None)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"<b>[SteamVR]</b> GetAnalogActionData error (",
					base.fullPath,
					"): ",
					analogActionData.ToString(),
					" handle: ",
					base.handle.ToString()
				}));
			}
			base.updateTime = Time.realtimeSinceStartup;
			this.axis = new Vector3(this.actionData.x, this.actionData.y, this.actionData.z);
			this.delta = new Vector3(this.actionData.deltaX, this.actionData.deltaY, this.actionData.deltaZ);
			this.changed = false;
			if (this.active)
			{
				if (this.delta.magnitude > this.changeTolerance)
				{
					this.changed = true;
					base.changedTime = Time.realtimeSinceStartup + this.actionData.fUpdateTime;
					if (this.onChange != null)
					{
						this.onChange(this.vector3Action, base.inputSource, this.axis, this.delta);
					}
				}
				if (this.axis != Vector3.zero && this.onAxis != null)
				{
					this.onAxis(this.vector3Action, base.inputSource, this.axis, this.delta);
				}
				if (this.onUpdate != null)
				{
					this.onUpdate(this.vector3Action, base.inputSource, this.axis, this.delta);
				}
			}
			if (this.onActiveBindingChange != null && this.lastActiveBinding != this.activeBinding)
			{
				this.onActiveBindingChange(this.vector3Action, base.inputSource, this.activeBinding);
			}
			if (this.onActiveChange != null && this.lastActive != this.active)
			{
				this.onActiveChange(this.vector3Action, base.inputSource, this.activeBinding);
			}
		}

		protected static uint actionData_size;

		public float changeTolerance = Mathf.Epsilon;

		protected InputAnalogActionData_t actionData;

		protected InputAnalogActionData_t lastActionData;

		protected SteamVR_Action_Vector3 vector3Action;
	}
}
