using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	[Serializable]
	public class FingerFeatureThresholds : IFeatureStateThresholds<FingerFeature, string>
	{
		public FingerFeatureThresholds()
		{
		}

		public FingerFeatureThresholds(FingerFeature feature, IEnumerable<FingerFeatureStateThreshold> thresholds)
		{
			this._feature = feature;
			this._thresholds = new List<FingerFeatureStateThreshold>(thresholds);
		}

		public FingerFeature Feature
		{
			get
			{
				return this._feature;
			}
		}

		public IReadOnlyList<IFeatureStateThreshold<string>> Thresholds
		{
			get
			{
				return this._thresholds;
			}
		}

		[SerializeField]
		[Tooltip("Which feature this collection of thresholds controls. Each feature should exist at most once.")]
		private FingerFeature _feature;

		[SerializeField]
		[Tooltip("List of state transitions, with thresold settings. The entries in this list must be in ascending order, based on their 'midpoint' values.")]
		private List<FingerFeatureStateThreshold> _thresholds;
	}
}
