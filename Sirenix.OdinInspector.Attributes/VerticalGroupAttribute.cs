using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class VerticalGroupAttribute : PropertyGroupAttribute
	{
		public VerticalGroupAttribute(string groupId, float order = 0f) : base(groupId, order)
		{
		}

		public VerticalGroupAttribute(float order = 0f) : this("_DefaultVerticalGroup", order)
		{
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			VerticalGroupAttribute verticalGroupAttribute = other as VerticalGroupAttribute;
			if (verticalGroupAttribute != null)
			{
				if (verticalGroupAttribute.PaddingTop != 0f)
				{
					this.PaddingTop = verticalGroupAttribute.PaddingTop;
				}
				if (verticalGroupAttribute.PaddingBottom != 0f)
				{
					this.PaddingBottom = verticalGroupAttribute.PaddingBottom;
				}
			}
		}

		public float PaddingTop;

		public float PaddingBottom;
	}
}
