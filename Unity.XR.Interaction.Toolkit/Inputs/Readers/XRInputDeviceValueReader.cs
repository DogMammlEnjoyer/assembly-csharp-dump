using System;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
	public abstract class XRInputDeviceValueReader : ScriptableObject
	{
		public InputDeviceCharacteristics characteristics
		{
			get
			{
				return this.m_Characteristics;
			}
			set
			{
				this.m_Characteristics = value;
			}
		}

		[SerializeField]
		[Tooltip("Characteristics of the input device to read from. Controllers are either:\nHeld In Hand, Tracked Device, Controller, Left\nHeld In Hand, Tracked Device, Controller, Right")]
		private protected InputDeviceCharacteristics m_Characteristics;
	}
}
