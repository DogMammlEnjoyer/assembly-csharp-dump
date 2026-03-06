using System;
using System.Collections.Generic;

namespace Oculus.Interaction.PoseDetection
{
	public static class TransformFeatureProperties
	{
		public static IReadOnlyDictionary<TransformFeature, FeatureDescription> FeatureDescriptions { get; } = TransformFeatureProperties.CreateFeatureDescriptions();

		private static IReadOnlyDictionary<TransformFeature, FeatureDescription> CreateFeatureDescriptions()
		{
			int num = 0;
			Dictionary<TransformFeature, FeatureDescription> dictionary = new Dictionary<TransformFeature, FeatureDescription>();
			dictionary[TransformFeature.WristUp] = TransformFeatureProperties.CreateDesc(ref num);
			dictionary[TransformFeature.WristDown] = TransformFeatureProperties.CreateDesc(ref num);
			dictionary[TransformFeature.PalmDown] = TransformFeatureProperties.CreateDesc(ref num);
			dictionary[TransformFeature.PalmUp] = TransformFeatureProperties.CreateDesc(ref num);
			dictionary[TransformFeature.PalmTowardsFace] = TransformFeatureProperties.CreateDesc(ref num);
			dictionary[TransformFeature.PalmAwayFromFace] = TransformFeatureProperties.CreateDesc(ref num);
			dictionary[TransformFeature.FingersUp] = TransformFeatureProperties.CreateDesc(ref num);
			dictionary[TransformFeature.FingersDown] = TransformFeatureProperties.CreateDesc(ref num);
			dictionary[TransformFeature.PinchClear] = TransformFeatureProperties.CreateDesc(ref num);
			return dictionary;
		}

		private static FeatureDescription CreateDesc(ref int startIndex)
		{
			FeatureDescription result = new FeatureDescription("", "", 0f, 180f, new FeatureStateDescription[]
			{
				new FeatureStateDescription(startIndex.ToString(), "True"),
				new FeatureStateDescription((startIndex + 2).ToString(), "False")
			});
			startIndex += 3;
			return result;
		}

		public const string FeatureStateThresholdMidpointHelpText = "The value at which a state will transition from A > B (or B > A)";

		public const string FeatureStateThresholdWidthHelpText = "How far the transform value must exceed the midpoint until the transition can occur. This is to prevent rapid flickering at transition edges.";
	}
}
