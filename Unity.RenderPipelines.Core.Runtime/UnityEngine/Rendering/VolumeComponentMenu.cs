using System;

namespace UnityEngine.Rendering
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class VolumeComponentMenu : Attribute
	{
		public VolumeComponentMenu(string menu)
		{
			this.menu = menu;
		}

		public readonly string menu;
	}
}
