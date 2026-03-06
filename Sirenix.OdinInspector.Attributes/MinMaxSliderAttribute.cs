using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class MinMaxSliderAttribute : Attribute
	{
		[Obsolete("Use the MinValueGetter member instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string MinMember
		{
			get
			{
				return this.MinValueGetter;
			}
			set
			{
				this.MinValueGetter = value;
			}
		}

		[Obsolete("Use the MaxValueGetter member instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string MaxMember
		{
			get
			{
				return this.MaxValueGetter;
			}
			set
			{
				this.MaxValueGetter = value;
			}
		}

		[Obsolete("Use the MinMaxValueGetter member instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string MinMaxMember
		{
			get
			{
				return this.MinMaxValueGetter;
			}
			set
			{
				this.MinMaxValueGetter = value;
			}
		}

		public MinMaxSliderAttribute(float minValue, float maxValue, bool showFields = false)
		{
			this.MinValue = minValue;
			this.MaxValue = maxValue;
			this.ShowFields = showFields;
		}

		public MinMaxSliderAttribute(string minValueGetter, float maxValue, bool showFields = false)
		{
			this.MinValueGetter = minValueGetter;
			this.MaxValue = maxValue;
			this.ShowFields = showFields;
		}

		public MinMaxSliderAttribute(float minValue, string maxValueGetter, bool showFields = false)
		{
			this.MinValue = minValue;
			this.MaxValueGetter = maxValueGetter;
			this.ShowFields = showFields;
		}

		public MinMaxSliderAttribute(string minValueGetter, string maxValueGetter, bool showFields = false)
		{
			this.MinValueGetter = minValueGetter;
			this.MaxValueGetter = maxValueGetter;
			this.ShowFields = showFields;
		}

		public MinMaxSliderAttribute(string minMaxValueGetter, bool showFields = false)
		{
			this.MinMaxValueGetter = minMaxValueGetter;
			this.ShowFields = showFields;
		}

		public float MinValue;

		public float MaxValue;

		public string MinValueGetter;

		public string MaxValueGetter;

		public string MinMaxValueGetter;

		public bool ShowFields;
	}
}
