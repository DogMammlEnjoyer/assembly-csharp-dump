using System;

namespace Oculus.Interaction.PoseDetection
{
	public class FeatureDescription
	{
		public FeatureDescription(string shortDescription, string description, float minValueHint, float maxValueHint, FeatureStateDescription[] featureStates)
		{
			this.ShortDescription = shortDescription;
			this.Description = description;
			this.MinValueHint = minValueHint;
			this.MaxValueHint = maxValueHint;
			this.FeatureStates = featureStates;
		}

		public string ShortDescription { get; }

		public string Description { get; }

		public float MinValueHint { get; }

		public float MaxValueHint { get; }

		public FeatureStateDescription[] FeatureStates { get; }
	}
}
