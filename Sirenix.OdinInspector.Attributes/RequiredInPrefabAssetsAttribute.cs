using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[Obsolete("Use [RequiredIn(PrefabKind.PrefabAsset)] instead.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class RequiredInPrefabAssetsAttribute : Attribute
	{
		public RequiredInPrefabAssetsAttribute()
		{
			this.MessageType = InfoMessageType.Error;
		}

		public RequiredInPrefabAssetsAttribute(string errorMessage, InfoMessageType messageType)
		{
			this.ErrorMessage = errorMessage;
			this.MessageType = messageType;
		}

		public RequiredInPrefabAssetsAttribute(string errorMessage)
		{
			this.ErrorMessage = errorMessage;
			this.MessageType = InfoMessageType.Error;
		}

		public RequiredInPrefabAssetsAttribute(InfoMessageType messageType)
		{
			this.MessageType = messageType;
		}

		public string ErrorMessage;

		public InfoMessageType MessageType;
	}
}
