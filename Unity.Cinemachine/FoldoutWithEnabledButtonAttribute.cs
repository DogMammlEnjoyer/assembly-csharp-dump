using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	public class FoldoutWithEnabledButtonAttribute : PropertyAttribute
	{
		public FoldoutWithEnabledButtonAttribute(string enabledProperty = "Enabled")
		{
			this.EnabledPropertyName = enabledProperty;
		}

		public string EnabledPropertyName;
	}
}
