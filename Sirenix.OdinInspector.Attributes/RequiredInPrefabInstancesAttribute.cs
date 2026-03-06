using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Use [RequiredIn(PrefabKind.PrefabInstance)] instead.", true)]
	public sealed class RequiredInPrefabInstancesAttribute : Attribute
	{
		public RequiredInPrefabInstancesAttribute()
		{
			this.MessageType = InfoMessageType.Error;
		}

		public RequiredInPrefabInstancesAttribute(string errorMessage, InfoMessageType messageType)
		{
			this.ErrorMessage = errorMessage;
			this.MessageType = messageType;
		}

		public RequiredInPrefabInstancesAttribute(string errorMessage)
		{
			this.ErrorMessage = errorMessage;
			this.MessageType = InfoMessageType.Error;
		}

		public RequiredInPrefabInstancesAttribute(InfoMessageType messageType)
		{
			this.MessageType = messageType;
		}

		public string ErrorMessage;

		public InfoMessageType MessageType;
	}
}
