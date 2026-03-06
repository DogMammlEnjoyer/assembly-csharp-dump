using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[DontApplyToListElements]
	[Conditional("UNITY_EDITOR")]
	public class DetailedInfoBoxAttribute : Attribute
	{
		public DetailedInfoBoxAttribute(string message, string details, InfoMessageType infoMessageType = InfoMessageType.Info, string visibleIf = null)
		{
			this.Message = message;
			this.Details = details;
			this.InfoMessageType = infoMessageType;
			this.VisibleIf = visibleIf;
		}

		public string Message;

		public string Details;

		public InfoMessageType InfoMessageType;

		public string VisibleIf;
	}
}
