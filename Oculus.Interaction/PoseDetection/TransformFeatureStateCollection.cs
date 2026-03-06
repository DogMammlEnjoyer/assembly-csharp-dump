using System;
using System.Collections.Generic;

namespace Oculus.Interaction.PoseDetection
{
	internal class TransformFeatureStateCollection
	{
		public void RegisterConfig(TransformConfig transformConfig, TransformJointData jointData, Func<float> timeProvider)
		{
			this._idToTransformStateInfo.ContainsKey(transformConfig.InstanceId);
			FeatureStateProvider<TransformFeature, string> featureStateProvider = new FeatureStateProvider<TransformFeature, string>((TransformFeature feature) => new float?(TransformFeatureValueProvider.GetValue(feature, jointData, transformConfig)), (TransformFeature feature) => (int)feature, timeProvider);
			TransformFeatureStateCollection.TransformStateInfo value = new TransformFeatureStateCollection.TransformStateInfo(transformConfig, featureStateProvider);
			featureStateProvider.InitializeThresholds(transformConfig.FeatureThresholds);
			this._idToTransformStateInfo.Add(transformConfig.InstanceId, value);
		}

		public void UnRegisterConfig(TransformConfig transformConfig)
		{
			this._idToTransformStateInfo.Remove(transformConfig.InstanceId);
		}

		public FeatureStateProvider<TransformFeature, string> GetStateProvider(TransformConfig transformConfig)
		{
			return this._idToTransformStateInfo[transformConfig.InstanceId].StateProvider;
		}

		public void SetConfig(int configId, TransformConfig config)
		{
			this._idToTransformStateInfo[configId].Config = config;
		}

		public TransformConfig GetConfig(int configId)
		{
			return this._idToTransformStateInfo[configId].Config;
		}

		public void UpdateFeatureStates(int lastUpdatedFrameId, bool disableProactiveEvaluation)
		{
			foreach (TransformFeatureStateCollection.TransformStateInfo transformStateInfo in this._idToTransformStateInfo.Values)
			{
				FeatureStateProvider<TransformFeature, string> stateProvider = transformStateInfo.StateProvider;
				if (!disableProactiveEvaluation)
				{
					stateProvider.LastUpdatedFrameId = lastUpdatedFrameId;
					stateProvider.ReadTouchedFeatureStates();
				}
				else
				{
					stateProvider.LastUpdatedFrameId = lastUpdatedFrameId;
				}
			}
		}

		private Dictionary<int, TransformFeatureStateCollection.TransformStateInfo> _idToTransformStateInfo = new Dictionary<int, TransformFeatureStateCollection.TransformStateInfo>();

		public class TransformStateInfo
		{
			public TransformStateInfo(TransformConfig transformConfig, FeatureStateProvider<TransformFeature, string> stateProvider)
			{
				this.Config = transformConfig;
				this.StateProvider = stateProvider;
			}

			public TransformConfig Config;

			public FeatureStateProvider<TransformFeature, string> StateProvider;
		}
	}
}
