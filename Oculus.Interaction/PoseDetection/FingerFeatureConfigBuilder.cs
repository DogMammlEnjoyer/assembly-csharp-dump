using System;

namespace Oculus.Interaction.PoseDetection
{
	public class FingerFeatureConfigBuilder : FeatureConfigBuilder
	{
		public static FeatureConfigBuilder.BuildCondition<FingerFeatureConfigBuilder.OpenCloseStateBuilder> Curl { get; } = new FeatureConfigBuilder.BuildCondition<FingerFeatureConfigBuilder.OpenCloseStateBuilder>((FeatureStateActiveMode mode) => new FingerFeatureConfigBuilder.OpenCloseStateBuilder(mode, FingerFeature.Curl));

		public static FeatureConfigBuilder.BuildCondition<FingerFeatureConfigBuilder.OpenCloseStateBuilder> Flexion { get; } = new FeatureConfigBuilder.BuildCondition<FingerFeatureConfigBuilder.OpenCloseStateBuilder>((FeatureStateActiveMode mode) => new FingerFeatureConfigBuilder.OpenCloseStateBuilder(mode, FingerFeature.Flexion));

		public static FeatureConfigBuilder.BuildCondition<FingerFeatureConfigBuilder.AbductionStateBuilder> Abduction { get; } = new FeatureConfigBuilder.BuildCondition<FingerFeatureConfigBuilder.AbductionStateBuilder>((FeatureStateActiveMode mode) => new FingerFeatureConfigBuilder.AbductionStateBuilder(mode));

		public static FeatureConfigBuilder.BuildCondition<FingerFeatureConfigBuilder.OppositionStateBuilder> Opposition { get; } = new FeatureConfigBuilder.BuildCondition<FingerFeatureConfigBuilder.OppositionStateBuilder>((FeatureStateActiveMode mode) => new FingerFeatureConfigBuilder.OppositionStateBuilder(mode));

		public class OpenCloseStateBuilder
		{
			public OpenCloseStateBuilder(FeatureStateActiveMode featureStateActiveMode, FingerFeature fingerFeature)
			{
				this._mode = featureStateActiveMode;
				this._fingerFeature = fingerFeature;
				this._states = FingerFeatureProperties.FeatureDescriptions[this._fingerFeature].FeatureStates;
			}

			public ShapeRecognizer.FingerFeatureConfig Open
			{
				get
				{
					return new ShapeRecognizer.FingerFeatureConfig
					{
						Feature = this._fingerFeature,
						Mode = this._mode,
						State = this._states[0].Id
					};
				}
			}

			public ShapeRecognizer.FingerFeatureConfig Neutral
			{
				get
				{
					return new ShapeRecognizer.FingerFeatureConfig
					{
						Feature = this._fingerFeature,
						Mode = this._mode,
						State = this._states[1].Id
					};
				}
			}

			public ShapeRecognizer.FingerFeatureConfig Closed
			{
				get
				{
					return new ShapeRecognizer.FingerFeatureConfig
					{
						Feature = this._fingerFeature,
						Mode = this._mode,
						State = this._states[2].Id
					};
				}
			}

			private readonly FeatureStateActiveMode _mode;

			private readonly FingerFeature _fingerFeature;

			private readonly FeatureStateDescription[] _states;
		}

		public class AbductionStateBuilder
		{
			public AbductionStateBuilder(FeatureStateActiveMode mode)
			{
				this._mode = mode;
			}

			public ShapeRecognizer.FingerFeatureConfig None
			{
				get
				{
					return new ShapeRecognizer.FingerFeatureConfig
					{
						Feature = FingerFeature.Abduction,
						Mode = this._mode,
						State = FingerFeatureProperties.AbductionFeatureStates[0].Id
					};
				}
			}

			public ShapeRecognizer.FingerFeatureConfig Closed
			{
				get
				{
					return new ShapeRecognizer.FingerFeatureConfig
					{
						Feature = FingerFeature.Abduction,
						Mode = this._mode,
						State = FingerFeatureProperties.AbductionFeatureStates[1].Id
					};
				}
			}

			public ShapeRecognizer.FingerFeatureConfig Open
			{
				get
				{
					return new ShapeRecognizer.FingerFeatureConfig
					{
						Feature = FingerFeature.Abduction,
						Mode = this._mode,
						State = FingerFeatureProperties.AbductionFeatureStates[2].Id
					};
				}
			}

			private readonly FeatureStateActiveMode _mode;
		}

		public class OppositionStateBuilder
		{
			public OppositionStateBuilder(FeatureStateActiveMode mode)
			{
				this._mode = mode;
			}

			public ShapeRecognizer.FingerFeatureConfig Touching
			{
				get
				{
					return new ShapeRecognizer.FingerFeatureConfig
					{
						Feature = FingerFeature.Opposition,
						Mode = this._mode,
						State = FingerFeatureProperties.OppositionFeatureStates[0].Id
					};
				}
			}

			public ShapeRecognizer.FingerFeatureConfig Near
			{
				get
				{
					return new ShapeRecognizer.FingerFeatureConfig
					{
						Feature = FingerFeature.Opposition,
						Mode = this._mode,
						State = FingerFeatureProperties.OppositionFeatureStates[1].Id
					};
				}
			}

			public ShapeRecognizer.FingerFeatureConfig None
			{
				get
				{
					return new ShapeRecognizer.FingerFeatureConfig
					{
						Feature = FingerFeature.Opposition,
						Mode = this._mode,
						State = FingerFeatureProperties.OppositionFeatureStates[2].Id
					};
				}
			}

			private readonly FeatureStateActiveMode _mode;
		}
	}
}
