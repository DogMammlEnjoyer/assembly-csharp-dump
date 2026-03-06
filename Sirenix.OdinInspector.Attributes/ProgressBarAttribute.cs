using System;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class ProgressBarAttribute : Attribute
	{
		[Obsolete("Use the MinGetter member instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string MinMember
		{
			get
			{
				return this.MinGetter;
			}
			set
			{
				this.MinGetter = value;
			}
		}

		[Obsolete("Use the MaxGetter member instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string MaxMember
		{
			get
			{
				return this.MaxGetter;
			}
			set
			{
				this.MaxGetter = value;
			}
		}

		[Obsolete("Use the ColorGetter member instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string ColorMember
		{
			get
			{
				return this.ColorGetter;
			}
			set
			{
				this.ColorGetter = value;
			}
		}

		[Obsolete("Use the BackgroundColorGetter member instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string BackgroundColorMember
		{
			get
			{
				return this.BackgroundColorGetter;
			}
			set
			{
				this.BackgroundColorGetter = value;
			}
		}

		[Obsolete("Use the CustomValueStringGetter member instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string CustomValueStringMember
		{
			get
			{
				return this.CustomValueStringGetter;
			}
			set
			{
				this.CustomValueStringGetter = value;
			}
		}

		public ProgressBarAttribute(double min, double max, float r = 0.15f, float g = 0.47f, float b = 0.74f)
		{
			this.Min = min;
			this.Max = max;
			this.R = r;
			this.G = g;
			this.B = b;
			this.Height = 12;
			this.Segmented = false;
			this.drawValueLabel = true;
			this.DrawValueLabelHasValue = false;
			this.valueLabelAlignment = TextAlignment.Center;
			this.ValueLabelAlignmentHasValue = false;
		}

		public ProgressBarAttribute(string minGetter, double max, float r = 0.15f, float g = 0.47f, float b = 0.74f)
		{
			this.MinGetter = minGetter;
			this.Max = max;
			this.R = r;
			this.G = g;
			this.B = b;
			this.Height = 12;
			this.Segmented = false;
			this.drawValueLabel = true;
			this.DrawValueLabelHasValue = false;
			this.valueLabelAlignment = TextAlignment.Center;
			this.ValueLabelAlignmentHasValue = false;
		}

		public ProgressBarAttribute(double min, string maxGetter, float r = 0.15f, float g = 0.47f, float b = 0.74f)
		{
			this.Min = min;
			this.MaxGetter = maxGetter;
			this.R = r;
			this.G = g;
			this.B = b;
			this.Height = 12;
			this.Segmented = false;
			this.drawValueLabel = true;
			this.DrawValueLabelHasValue = false;
			this.valueLabelAlignment = TextAlignment.Center;
			this.ValueLabelAlignmentHasValue = false;
		}

		public ProgressBarAttribute(string minGetter, string maxGetter, float r = 0.15f, float g = 0.47f, float b = 0.74f)
		{
			this.MinGetter = minGetter;
			this.MaxGetter = maxGetter;
			this.R = r;
			this.G = g;
			this.B = b;
			this.Height = 12;
			this.Segmented = false;
			this.drawValueLabel = true;
			this.DrawValueLabelHasValue = false;
			this.valueLabelAlignment = TextAlignment.Center;
			this.ValueLabelAlignmentHasValue = false;
		}

		public bool DrawValueLabel
		{
			get
			{
				return this.drawValueLabel;
			}
			set
			{
				this.drawValueLabel = value;
				this.DrawValueLabelHasValue = true;
			}
		}

		public bool DrawValueLabelHasValue { get; private set; }

		public TextAlignment ValueLabelAlignment
		{
			get
			{
				return this.valueLabelAlignment;
			}
			set
			{
				this.valueLabelAlignment = value;
				this.ValueLabelAlignmentHasValue = true;
			}
		}

		public bool ValueLabelAlignmentHasValue { get; private set; }

		public Color Color
		{
			get
			{
				return new Color(this.R, this.G, this.B, 1f);
			}
		}

		public double Min;

		public double Max;

		public string MinGetter;

		public string MaxGetter;

		public float R;

		public float G;

		public float B;

		public int Height;

		public string ColorGetter;

		public string BackgroundColorGetter;

		public bool Segmented;

		public string CustomValueStringGetter;

		private bool drawValueLabel;

		private TextAlignment valueLabelAlignment;
	}
}
