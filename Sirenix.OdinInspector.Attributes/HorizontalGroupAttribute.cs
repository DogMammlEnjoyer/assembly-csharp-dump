using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class HorizontalGroupAttribute : PropertyGroupAttribute
	{
		public HorizontalGroupAttribute(string group, float width = 0f, int marginLeft = 0, int marginRight = 0, float order = 0f) : base(group, order)
		{
			this.Width = width;
			this.MarginLeft = (float)marginLeft;
			this.MarginRight = (float)marginRight;
		}

		public HorizontalGroupAttribute(float width = 0f, int marginLeft = 0, int marginRight = 0, float order = 0f) : this("_DefaultHorizontalGroup", width, marginLeft, marginRight, order)
		{
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			HorizontalGroupAttribute horizontalGroupAttribute = other as HorizontalGroupAttribute;
			if (horizontalGroupAttribute != null)
			{
				this.Title = (this.Title ?? horizontalGroupAttribute.Title);
				this.DisableAutomaticLabelWidth = (this.DisableAutomaticLabelWidth || horizontalGroupAttribute.DisableAutomaticLabelWidth);
				if (this.LabelWidth == 0f && horizontalGroupAttribute.LabelWidth != 0f)
				{
					this.LabelWidth = horizontalGroupAttribute.LabelWidth;
				}
				if (horizontalGroupAttribute.Gap != 3f)
				{
					this.Gap = horizontalGroupAttribute.Gap;
				}
			}
		}

		private const int DefaultHorizontalGroupGap = 3;

		public float Width;

		public float MarginLeft;

		public float MarginRight;

		public float PaddingLeft;

		public float PaddingRight;

		public float MinWidth;

		public float MaxWidth;

		public float Gap = 3f;

		public string Title;

		[LabelWidth(200f)]
		public bool DisableAutomaticLabelWidth;

		public float LabelWidth;
	}
}
