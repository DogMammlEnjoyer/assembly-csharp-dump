using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class RequiredAttribute : Attribute
	{
		public RequiredAttribute()
		{
			this.MessageType = InfoMessageType.Error;
		}

		public RequiredAttribute(string errorMessage, InfoMessageType messageType)
		{
			this.ErrorMessage = errorMessage;
			this.MessageType = messageType;
		}

		public RequiredAttribute(string errorMessage)
		{
			this.ErrorMessage = errorMessage;
			this.MessageType = InfoMessageType.Error;
		}

		public RequiredAttribute(InfoMessageType messageType)
		{
			this.MessageType = messageType;
		}

		public string ErrorMessage;

		public InfoMessageType MessageType;
	}
}
