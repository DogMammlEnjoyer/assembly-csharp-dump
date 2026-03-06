using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class PropertyRangeAttribute : Attribute
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

		public PropertyRangeAttribute(double min, double max)
		{
			this.Min = ((min < max) ? min : max);
			this.Max = ((max > min) ? max : min);
		}

		public PropertyRangeAttribute(string minGetter, double max)
		{
			this.MinGetter = minGetter;
			this.Max = max;
		}

		public PropertyRangeAttribute(double min, string maxGetter)
		{
			this.Min = min;
			this.MaxGetter = maxGetter;
		}

		public PropertyRangeAttribute(string minGetter, string maxGetter)
		{
			this.MinGetter = minGetter;
			this.MaxGetter = maxGetter;
		}

		public double Min;

		public double Max;

		public string MinGetter;

		public string MaxGetter;
	}
}
