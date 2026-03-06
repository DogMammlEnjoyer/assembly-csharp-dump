using System;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(isGenericTypeOfDevice = true)]
	public class Sensor : InputDevice
	{
		public float samplingFrequency
		{
			get
			{
				QuerySamplingFrequencyCommand querySamplingFrequencyCommand = QuerySamplingFrequencyCommand.Create();
				if (base.ExecuteCommand<QuerySamplingFrequencyCommand>(ref querySamplingFrequencyCommand) >= 0L)
				{
					return querySamplingFrequencyCommand.frequency;
				}
				throw new NotSupportedException(string.Format("Device '{0}' does not support querying sampling frequency", this));
			}
			set
			{
				SetSamplingFrequencyCommand setSamplingFrequencyCommand = SetSamplingFrequencyCommand.Create(value);
				base.ExecuteCommand<SetSamplingFrequencyCommand>(ref setSamplingFrequencyCommand);
			}
		}
	}
}
