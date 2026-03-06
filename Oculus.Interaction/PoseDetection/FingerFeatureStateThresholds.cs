using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	[CreateAssetMenu(menuName = "Meta/Interaction/SDK/Pose Detection/Finger Thresholds")]
	public class FingerFeatureStateThresholds : ScriptableObject, IFeatureThresholds<FingerFeature, string>
	{
		public void Construct(List<FingerFeatureThresholds> featureThresholds, double minTimeInState)
		{
			this._featureThresholds = featureThresholds;
			this._minTimeInState = minTimeInState;
		}

		public IReadOnlyList<IFeatureStateThresholds<FingerFeature, string>> FeatureStateThresholds
		{
			get
			{
				return this._featureThresholds;
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
		[Tooltip("List of all supported finger features, along with the state entry/exit thresholds.")]
		private List<FingerFeatureThresholds> _featureThresholds;

		[SerializeField]
		[Tooltip("Length of time that the finger must be in the new state before the feature state provider will use the new value.")]
		private double _minTimeInState;
	}
}
