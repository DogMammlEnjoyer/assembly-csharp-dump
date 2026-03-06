using System;

namespace UnityEngine.Splines
{
	public class SplineIndexAttribute : PropertyAttribute
	{
		public SplineIndexAttribute(string splineContainerProperty)
		{
			this.SplineContainerProperty = splineContainerProperty;
		}

		public readonly string SplineContainerProperty;
	}
}
