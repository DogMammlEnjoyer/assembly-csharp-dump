using System;

namespace UnityEngine.ProBuilder
{
	public struct PickerOptions
	{
		public bool depthTest { readonly get; set; }

		public RectSelectMode rectSelectMode { readonly get; set; }

		public static PickerOptions Default
		{
			get
			{
				return PickerOptions.k_Default;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is PickerOptions && this.Equals((PickerOptions)obj);
		}

		public bool Equals(PickerOptions other)
		{
			return this.depthTest == other.depthTest && this.rectSelectMode == other.rectSelectMode;
		}

		public override int GetHashCode()
		{
			return this.depthTest.GetHashCode() * 397 ^ (int)this.rectSelectMode;
		}

		public static bool operator ==(PickerOptions a, PickerOptions b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(PickerOptions a, PickerOptions b)
		{
			return !a.Equals(b);
		}

		private static readonly PickerOptions k_Default = new PickerOptions
		{
			depthTest = true,
			rectSelectMode = RectSelectMode.Partial
		};
	}
}
