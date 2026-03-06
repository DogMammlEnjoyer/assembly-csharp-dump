using System;

namespace Meta.XR.Util
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class FeatureAttribute : Attribute
	{
		public FeatureAttribute(Feature feature)
		{
			this.Feature = feature;
		}

		public Feature Feature { get; }
	}
}
