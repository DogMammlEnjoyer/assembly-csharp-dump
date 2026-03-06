using System;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Action_Vibration_Source : SteamVR_Action_Out_Source
	{
		public event SteamVR_Action_Vibration.ActiveChangeHandler onActiveChange;

		public event SteamVR_Action_Vibration.ActiveChangeHandler onActiveBindingChange;

		public event SteamVR_Action_Vibration.ExecuteHandler onExecute;

		public override bool active
		{
			get
			{
				return this.activeBinding && base.setActive;
			}
		}

		public override bool activeBinding
		{
			get
			{
				return true;
			}
		}

		public override bool lastActive { get; protected set; }

		public override bool lastActiveBinding
		{
			get
			{
				return true;
			}
		}

		public float timeLastExecuted { get; protected set; }

		public override void Initialize()
		{
			base.Initialize();
			this.lastActive = true;
		}

		public override void Preinitialize(SteamVR_Action wrappingAction, SteamVR_Input_Sources forInputSource)
		{
			base.Preinitialize(wrappingAction, forInputSource);
			this.vibrationAction = (SteamVR_Action_Vibration)wrappingAction;
		}

		public void Execute(float secondsFromNow, float durationSeconds, float frequency, float amplitude)
		{
			if (SteamVR_Input.isStartupFrame)
			{
				return;
			}
			this.timeLastExecuted = Time.realtimeSinceStartup;
			EVRInputError evrinputError = OpenVR.Input.TriggerHapticVibrationAction(base.handle, secondsFromNow, durationSeconds, frequency, amplitude, this.inputSourceHandle);
			if (evrinputError != EVRInputError.None)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"<b>[SteamVR]</b> TriggerHapticVibrationAction (",
					base.fullPath,
					") error: ",
					evrinputError.ToString(),
					" handle: ",
					base.handle.ToString()
				}));
			}
			if (this.onExecute != null)
			{
				this.onExecute(this.vibrationAction, base.inputSource, secondsFromNow, durationSeconds, frequency, amplitude);
			}
		}

		protected SteamVR_Action_Vibration vibrationAction;
	}
}
