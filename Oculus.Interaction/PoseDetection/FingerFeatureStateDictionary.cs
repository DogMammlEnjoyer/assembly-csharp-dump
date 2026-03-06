using System;
using Oculus.Interaction.Input;

namespace Oculus.Interaction.PoseDetection
{
	internal class FingerFeatureStateDictionary
	{
		public void InitializeFinger(HandFinger finger, FeatureStateProvider<FingerFeature, string> stateProvider)
		{
			this._fingerState[(int)finger] = new FingerFeatureStateDictionary.HandFingerState
			{
				StateProvider = stateProvider
			};
		}

		public FeatureStateProvider<FingerFeature, string> GetStateProvider(HandFinger finger)
		{
			return this._fingerState[(int)finger].StateProvider;
		}

		private readonly FingerFeatureStateDictionary.HandFingerState[] _fingerState = new FingerFeatureStateDictionary.HandFingerState[5];

		private struct HandFingerState
		{
			public FeatureStateProvider<FingerFeature, string> StateProvider;
		}
	}
}
