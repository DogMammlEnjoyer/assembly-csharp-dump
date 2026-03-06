using System;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem.Haptics
{
	internal struct DualMotorRumble
	{
		public float lowFrequencyMotorSpeed { readonly get; private set; }

		public float highFrequencyMotorSpeed { readonly get; private set; }

		public bool isRumbling
		{
			get
			{
				return !Mathf.Approximately(this.lowFrequencyMotorSpeed, 0f) || !Mathf.Approximately(this.highFrequencyMotorSpeed, 0f);
			}
		}

		public void PauseHaptics(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (!this.isRumbling)
			{
				return;
			}
			DualMotorRumbleCommand dualMotorRumbleCommand = DualMotorRumbleCommand.Create(0f, 0f);
			device.ExecuteCommand<DualMotorRumbleCommand>(ref dualMotorRumbleCommand);
		}

		public void ResumeHaptics(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (!this.isRumbling)
			{
				return;
			}
			this.SetMotorSpeeds(device, this.lowFrequencyMotorSpeed, this.highFrequencyMotorSpeed);
		}

		public void ResetHaptics(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (!this.isRumbling)
			{
				return;
			}
			this.SetMotorSpeeds(device, 0f, 0f);
		}

		public void SetMotorSpeeds(InputDevice device, float lowFrequency, float highFrequency)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			this.lowFrequencyMotorSpeed = Mathf.Clamp(lowFrequency, 0f, 1f);
			this.highFrequencyMotorSpeed = Mathf.Clamp(highFrequency, 0f, 1f);
			DualMotorRumbleCommand dualMotorRumbleCommand = DualMotorRumbleCommand.Create(this.lowFrequencyMotorSpeed, this.highFrequencyMotorSpeed);
			device.ExecuteCommand<DualMotorRumbleCommand>(ref dualMotorRumbleCommand);
		}
	}
}
