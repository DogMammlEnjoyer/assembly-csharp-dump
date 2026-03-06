using System;

namespace UnityEngine.VFX.Utility
{
	[AttributeUsage(AttributeTargets.Class)]
	public class VFXBinderAttribute : PropertyAttribute
	{
		public VFXBinderAttribute(string menuPath)
		{
			this.MenuPath = menuPath;
		}

		public string MenuPath;
	}
}
