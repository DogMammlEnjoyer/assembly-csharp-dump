using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[Conditional("UNITY_EDITOR")]
	public class ShowIfGroupAttribute : PropertyGroupAttribute
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

		public ShowIfGroupAttribute(string path, bool animate = true) : base(path)
		{
			this.Animate = animate;
		}

		public ShowIfGroupAttribute(string path, object value, bool animate = true) : base(path)
		{
			this.Value = value;
			this.Animate = animate;
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			ShowIfGroupAttribute showIfGroupAttribute = other as ShowIfGroupAttribute;
			if (this.Value != null)
			{
				showIfGroupAttribute.Value = this.Value;
			}
		}

		public object Value;
	}
}
