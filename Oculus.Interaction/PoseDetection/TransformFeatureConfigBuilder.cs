using System;

namespace Oculus.Interaction.PoseDetection
{
	public class TransformFeatureConfigBuilder : FeatureConfigBuilder
	{
		public static FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder> WristUp { get; } = new FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder>((FeatureStateActiveMode mode) => new TransformFeatureConfigBuilder.TrueFalseStateBuilder(mode, TransformFeature.WristUp));

		public static FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder> WristDown { get; } = new FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder>((FeatureStateActiveMode mode) => new TransformFeatureConfigBuilder.TrueFalseStateBuilder(mode, TransformFeature.WristDown));

		public static FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder> PalmDown { get; } = new FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder>((FeatureStateActiveMode mode) => new TransformFeatureConfigBuilder.TrueFalseStateBuilder(mode, TransformFeature.PalmDown));

		public static FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder> PalmUp { get; } = new FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder>((FeatureStateActiveMode mode) => new TransformFeatureConfigBuilder.TrueFalseStateBuilder(mode, TransformFeature.PalmUp));

		public static FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder> PalmTowardsFace { get; } = new FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder>((FeatureStateActiveMode mode) => new TransformFeatureConfigBuilder.TrueFalseStateBuilder(mode, TransformFeature.PalmTowardsFace));

		public static FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder> PalmAwayFromFace { get; } = new FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder>((FeatureStateActiveMode mode) => new TransformFeatureConfigBuilder.TrueFalseStateBuilder(mode, TransformFeature.PalmAwayFromFace));

		public static FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder> FingersUp { get; } = new FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder>((FeatureStateActiveMode mode) => new TransformFeatureConfigBuilder.TrueFalseStateBuilder(mode, TransformFeature.FingersUp));

		public static FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder> FingersDown { get; } = new FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder>((FeatureStateActiveMode mode) => new TransformFeatureConfigBuilder.TrueFalseStateBuilder(mode, TransformFeature.FingersDown));

		public static FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder> PinchClear { get; } = new FeatureConfigBuilder.BuildCondition<TransformFeatureConfigBuilder.TrueFalseStateBuilder>((FeatureStateActiveMode mode) => new TransformFeatureConfigBuilder.TrueFalseStateBuilder(mode, TransformFeature.PinchClear));

		public class TrueFalseStateBuilder
		{
			public TrueFalseStateBuilder(FeatureStateActiveMode featureStateActiveMode, TransformFeature transformFeature)
			{
				this._mode = featureStateActiveMode;
				this._transformFeature = transformFeature;
				this._states = TransformFeatureProperties.FeatureDescriptions[this._transformFeature].FeatureStates;
			}

			public TransformFeatureConfig Open
			{
				get
				{
					return new TransformFeatureConfig
					{
						Feature = this._transformFeature,
						Mode = this._mode,
						State = this._states[0].Id
					};
				}
			}

			public TransformFeatureConfig Closed
			{
				get
				{
					return new TransformFeatureConfig
					{
						Feature = this._transformFeature,
						Mode = this._mode,
						State = this._states[1].Id
					};
				}
			}

			private readonly FeatureStateActiveMode _mode;

			private readonly TransformFeature _transformFeature;

			private readonly FeatureStateDescription[] _states;
		}
	}
}
