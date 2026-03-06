using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[Conditional("UNITY_EDITOR")]
	public class HideIfGroupAttribute : PropertyGroupAttribute
	{
		public bool Animate
		{
			get
			{
				return this.AnimateVisibility;
			}
			set
			{
				this.AnimateVisibility = value;
			}
		}

		[Obsolete("Use the Condition member instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string MemberName
		{
			get
			{
				return this.Condition;
			}
			set
			{
				this.Condition = value;
			}
		}

		public string Condition
		{
			get
			{
				if (!string.IsNullOrEmpty(this.VisibleIf))
				{
					return this.VisibleIf;
				}
				return this.GroupName;
			}
			set
			{
				this.VisibleIf = value;
			}
		}

		public HideIfGroupAttribute(string path, bool animate = true) : base(path)
		{
			this.Animate = animate;
		}

		public HideIfGroupAttribute(string path, object value, bool animate = true) : base(path)
		{
			this.Value = value;
			this.Animate = animate;
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			HideIfGroupAttribute hideIfGroupAttribute = other as HideIfGroupAttribute;
			if (this.Value != null)
			{
				hideIfGroupAttribute.Value = this.Value;
			}
		}

		public object Value;
	}
}
