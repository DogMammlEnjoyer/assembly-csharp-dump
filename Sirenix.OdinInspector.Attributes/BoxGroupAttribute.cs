using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class BoxGroupAttribute : PropertyGroupAttribute
	{
		public BoxGroupAttribute(string group, bool showLabel = true, bool centerLabel = false, float order = 0f) : base(group, order)
		{
			this.ShowLabel = showLabel;
			this.CenterLabel = centerLabel;
		}

		public BoxGroupAttribute() : this("_DefaultBoxGroup", false, false, 0f)
		{
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			BoxGroupAttribute boxGroupAttribute = other as BoxGroupAttribute;
			if (!this.ShowLabel || !boxGroupAttribute.ShowLabel)
			{
				this.ShowLabel = false;
				boxGroupAttribute.ShowLabel = false;
			}
			this.CenterLabel |= boxGroupAttribute.CenterLabel;
		}

		public bool ShowLabel;

		public bool CenterLabel;

		public string LabelText;
	}
}
