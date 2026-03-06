using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem.XInput
{
	[InputControlLayout(displayName = "Xbox Controller")]
	public class XInputController : Gamepad
	{
		[InputControl(name = "buttonSouth", displayName = "A")]
		[InputControl(name = "buttonEast", displayName = "B")]
		[InputControl(name = "buttonWest", displayName = "X")]
		[InputControl(name = "buttonNorth", displayName = "Y")]
		[InputControl(name = "leftShoulder", displayName = "Left Bumper", shortDisplayName = "LB")]
		[InputControl(name = "rightShoulder", displayName = "Right Bumper", shortDisplayName = "RB")]
		[InputControl(name = "leftTrigger", shortDisplayName = "LT")]
		[InputControl(name = "rightTrigger", shortDisplayName = "RT")]
		[InputControl(name = "start", displayName = "Menu", alias = "menu")]
		[InputControl(name = "select", displayName = "View", alias = "view")]
		public ButtonControl menu { get; protected set; }

		public ButtonControl view { get; protected set; }

		public XInputController.DeviceSubType subType
		{
			get
			{
				if (!this.m_HaveParsedCapabilities)
				{
					this.ParseCapabilities();
				}
				return this.m_SubType;
			}
		}

		public XInputController.DeviceFlags flags
		{
			get
			{
				if (!this.m_HaveParsedCapabilities)
				{
					this.ParseCapabilities();
				}
				return this.m_Flags;
			}
		}

		protected override void FinishSetup()
		{
			base.FinishSetup();
			this.menu = base.startButton;
			this.view = base.selectButton;
		}

		private void ParseCapabilities()
		{
			if (!string.IsNullOrEmpty(base.description.capabilities))
			{
				XInputController.Capabilities capabilities = JsonUtility.FromJson<XInputController.Capabilities>(base.description.capabilities);
				this.m_SubType = capabilities.subType;
				this.m_Flags = capabilities.flags;
			}
			this.m_HaveParsedCapabilities = true;
		}

		private bool m_HaveParsedCapabilities;

		private XInputController.DeviceSubType m_SubType;

		private XInputController.DeviceFlags m_Flags;

		internal enum DeviceType
		{
			Gamepad
		}

		public enum DeviceSubType
		{
			Unknown,
			Gamepad,
			Wheel,
			ArcadeStick,
			FlightStick,
			DancePad,
			Guitar,
			GuitarAlternate,
			DrumKit,
			GuitarBass = 11,
			ArcadePad = 19
		}

		[Flags]
		public new enum DeviceFlags
		{
			ForceFeedbackSupported = 1,
			Wireless = 2,
			VoiceSupported = 4,
			PluginModulesSupported = 8,
			NoNavigation = 16
		}

		[Serializable]
		internal struct Capabilities
		{
			public XInputController.DeviceType type;

			public XInputController.DeviceSubType subType;

			public XInputController.DeviceFlags flags;
		}
	}
}
