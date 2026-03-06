using System;

namespace Oculus.Interaction.PoseDetection
{
	public class FeatureConfigBuilder
	{
		public class BuildCondition<TBuildState>
		{
			public BuildCondition(FeatureConfigBuilder.BuildCondition<TBuildState>.BuildStateDelegate buildStateFn)
			{
				this._buildStateFn = buildStateFn;
			}

			public TBuildState Is
			{
				get
				{
					return this._buildStateFn(FeatureStateActiveMode.Is);
				}
			}

			public TBuildState IsNot
			{
				get
				{
					return this._buildStateFn(FeatureStateActiveMode.IsNot);
				}
			}

			private readonly FeatureConfigBuilder.BuildCondition<TBuildState>.BuildStateDelegate _buildStateFn;

			public delegate TBuildState BuildStateDelegate(FeatureStateActiveMode mode);
		}
	}
}
