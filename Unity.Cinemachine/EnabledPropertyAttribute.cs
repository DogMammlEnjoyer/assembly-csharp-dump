using System;

namespace Unity.Cinemachine
{
	public sealed class EnabledPropertyAttribute : FoldoutWithEnabledButtonAttribute
	{
		public EnabledPropertyAttribute(string enabledProperty = "Enabled", string toggleText = "") : base(enabledProperty)
		{
			this.ToggleDisabledText = toggleText;
		}

		public string ToggleDisabledText;
	}
}
