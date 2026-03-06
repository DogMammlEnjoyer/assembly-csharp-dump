using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
	[Conditional("UNITY_EDITOR")]
	public class TableColumnWidthAttribute : Attribute
	{
		public TableColumnWidthAttribute(int width, bool resizable = true)
		{
			this.Width = width;
			this.Resizable = resizable;
		}

		public int Width;

		public bool Resizable = true;
	}
}
