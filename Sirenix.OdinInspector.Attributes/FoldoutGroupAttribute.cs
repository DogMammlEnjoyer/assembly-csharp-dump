using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class FoldoutGroupAttribute : PropertyGroupAttribute
	{
		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"expanded",
			"HasDefinedExpanded"
		})]
		public bool Expanded
		{
			get
			{
				return this.expanded;
			}
			set
			{
				this.expanded = value;
				this.HasDefinedExpanded = true;
			}
		}

		public bool HasDefinedExpanded { get; private set; }

		public FoldoutGroupAttribute(string groupName, float order = 0f) : base(groupName, order)
		{
		}

		public FoldoutGroupAttribute(string groupName, bool expanded, float order = 0f) : base(groupName, order)
		{
			this.expanded = expanded;
			this.HasDefinedExpanded = true;
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			FoldoutGroupAttribute foldoutGroupAttribute = other as FoldoutGroupAttribute;
			if (foldoutGroupAttribute.HasDefinedExpanded)
			{
				this.HasDefinedExpanded = true;
				this.Expanded = foldoutGroupAttribute.Expanded;
			}
			if (this.HasDefinedExpanded)
			{
				foldoutGroupAttribute.HasDefinedExpanded = true;
				foldoutGroupAttribute.Expanded = this.Expanded;
			}
		}

		private bool expanded;
	}
}
