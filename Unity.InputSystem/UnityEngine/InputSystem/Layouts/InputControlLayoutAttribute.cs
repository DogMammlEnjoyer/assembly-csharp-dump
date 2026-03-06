using System;

namespace UnityEngine.InputSystem.Layouts
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class InputControlLayoutAttribute : Attribute
	{
		public Type stateType { get; set; }

		public string stateFormat { get; set; }

		public string[] commonUsages { get; set; }

		public string variants { get; set; }

		public bool isNoisy { get; set; }

		public bool canRunInBackground
		{
			get
			{
				return this.canRunInBackgroundInternal.Value;
			}
			set
			{
				this.canRunInBackgroundInternal = new bool?(value);
			}
		}

		public bool updateBeforeRender
		{
			get
			{
				return this.updateBeforeRenderInternal.Value;
			}
			set
			{
				this.updateBeforeRenderInternal = new bool?(value);
			}
		}

		public bool isGenericTypeOfDevice { get; set; }

		public string displayName { get; set; }

		public string description { get; set; }

		public bool hideInUI { get; set; }

		internal bool? canRunInBackgroundInternal;

		internal bool? updateBeforeRenderInternal;
	}
}
