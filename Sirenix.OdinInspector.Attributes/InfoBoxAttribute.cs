using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class InfoBoxAttribute : Attribute
	{
		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"icon",
			"HasDefinedIcon"
		})]
		public SdfIconType Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				this.icon = value;
				this.HasDefinedIcon = true;
			}
		}

		public bool HasDefinedIcon { get; private set; }

		public InfoBoxAttribute(string message, InfoMessageType infoMessageType = InfoMessageType.Info, string visibleIfMemberName = null)
		{
			this.Message = message;
			this.InfoMessageType = infoMessageType;
			this.VisibleIf = visibleIfMemberName;
		}

		public InfoBoxAttribute(string message, string visibleIfMemberName)
		{
			this.Message = message;
			this.InfoMessageType = InfoMessageType.Info;
			this.VisibleIf = visibleIfMemberName;
		}

		public InfoBoxAttribute(string message, SdfIconType icon, string visibleIfMemberName = null)
		{
			this.Message = message;
			this.Icon = icon;
			this.VisibleIf = visibleIfMemberName;
			this.InfoMessageType = InfoMessageType.None;
		}

		public string Message;

		public InfoMessageType InfoMessageType;

		public string VisibleIf;

		public bool GUIAlwaysEnabled;

		[ColorResolver]
		public string IconColor;

		private SdfIconType icon;
	}
}
