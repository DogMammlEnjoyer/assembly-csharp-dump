using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HelpBoxAttribute : PropertyAttribute
	{
		public string Message { get; private set; }

		public object Value { get; private set; }

		public HelpBoxAttribute.MessageType Type { get; private set; }

		public ConditionalHideAttribute.DisplayMode Display { get; private set; }

		public HelpBoxAttribute(string message)
		{
			this.Message = message;
			this.Type = HelpBoxAttribute.MessageType.Info;
			this.Value = null;
			this.Display = ConditionalHideAttribute.DisplayMode.Always;
		}

		public HelpBoxAttribute(string message, HelpBoxAttribute.MessageType type)
		{
			this.Message = message;
			this.Type = type;
			this.Value = null;
			this.Display = ConditionalHideAttribute.DisplayMode.Always;
		}

		public HelpBoxAttribute(string message, HelpBoxAttribute.MessageType type, object value)
		{
			this.Message = message;
			this.Type = type;
			this.Value = value;
			this.Display = ConditionalHideAttribute.DisplayMode.ShowIfTrue;
		}

		public HelpBoxAttribute(string message, HelpBoxAttribute.MessageType type, object value, ConditionalHideAttribute.DisplayMode display)
		{
			this.Message = message;
			this.Type = type;
			this.Value = value;
			this.Display = display;
		}

		public enum MessageType
		{
			None,
			Info,
			Warning,
			Error
		}

		public delegate bool HelpBoxCondition();
	}
}
