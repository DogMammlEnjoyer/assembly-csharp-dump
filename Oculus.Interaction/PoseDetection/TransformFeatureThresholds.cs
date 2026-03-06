using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	[Serializable]
	public class TransformFeatureThresholds : IFeatureStateThresholds<TransformFeature, string>
	{
		public TransformFeatureThresholds()
		{
		}

		public TransformFeatureThresholds(TransformFeature featureTransform, IEnumerable<TransformFeatureStateThreshold> thresholds)
		{
			this._feature = featureTransform;
			this._thresholds = new List<TransformFeatureStateThreshold>(thresholds);
		}

		public TransformFeature Feature
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

		public double MinTimeInState
		{
			get
			{
				return this._minTimeInState;
			}
		}

		[SerializeField]
		[Tooltip("Which feature this collection of thresholds controls. Each feature should exist at most once.")]
		private TransformFeature _feature;

		[SerializeField]
		[Tooltip("List of state transitions, with thresold settings. The entries in this list must be in ascending order, based on their 'midpoint' values.")]
		private List<TransformFeatureStateThreshold> _thresholds;

		[SerializeField]
		[Tooltip("Length of time that the transform must be in the new state before the feature state provider will use the new value.")]
		private double _minTimeInState;
	}
}
