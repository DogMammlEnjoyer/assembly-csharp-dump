using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class ColorPaletteAttribute : Attribute
	{
		public ColorPaletteAttribute()
		{
			this.PaletteName = null;
			this.ShowAlpha = true;
		}

		public ColorPaletteAttribute(string paletteName)
		{
			this.PaletteName = paletteName;
			this.ShowAlpha = true;
		}

		public string PaletteName;

		public bool ShowAlpha;
	}
}
