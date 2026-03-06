using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[DontApplyToListElements]
	[Conditional("UNITY_EDITOR")]
	public class PropertySpaceAttribute : Attribute
	{
		public PropertySpaceAttribute()
		{
			this.SpaceBefore = 8f;
			this.SpaceAfter = 0f;
		}

		public PropertySpaceAttribute(float spaceBefore)
		{
			this.SpaceBefore = spaceBefore;
			this.SpaceAfter = 0f;
		}

		public PropertySpaceAttribute(float spaceBefore, float spaceAfter)
		{
			this.SpaceBefore = spaceBefore;
			this.SpaceAfter = spaceAfter;
		}

		public float SpaceBefore;

		public float SpaceAfter;
	}
}
