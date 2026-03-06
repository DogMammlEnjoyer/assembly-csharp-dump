using System;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	[Serializable]
	public class FingerFeatureStateThreshold : IFeatureStateThreshold<string>
	{
		public FingerFeatureStateThreshold()
		{
		}

		public FingerFeatureStateThreshold(float thresholdMidpoint, float thresholdWidth, string firstState, string secondState)
		{
			this._thresholdMidpoint = thresholdMidpoint;
			this._thresholdWidth = thresholdWidth;
			this._firstState = firstState;
			this._secondState = secondState;
		}

		public float ThresholdMidpoint
		{
			get
			{
				return this._thresholdMidpoint;
			}
		}

		public float ThresholdWidth
		{
			get
			{
				return this._thresholdWidth;
			}
		}

		public float ToFirstWhenBelow
		{
			get
			{
				return this._thresholdMidpoint - this._thresholdWidth * 0.5f;
			}
		}

		public float ToSecondWhenAbove
		{
			get
			{
				return this._thresholdMidpoint + this._thresholdWidth * 0.5f;
			}
		}

		public string FirstState
		{
			get
			{
				return this._firstState;
			}
		}

		public string SecondState
		{
			get
			{
				return this._secondState;
			}
		}

		[SerializeField]
		[Tooltip("The angle at which a state will transition from A > B (or B > A)")]
		private float _thresholdMidpoint;

		[SerializeField]
		[Tooltip("How far the angle must exceed the midpoint until the transition can occur. This is to prevent rapid flickering at transition edges.")]
		private float _thresholdWidth;

		[SerializeField]
		[Tooltip("State to transition to when value passes below the threshold")]
		private string _firstState;

		[SerializeField]
		[Tooltip("State to transition to when value passes above the threshold")]
		private string _secondState;
	}
}
