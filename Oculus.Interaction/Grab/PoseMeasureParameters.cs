using System;
using UnityEngine;

namespace Oculus.Interaction.Grab
{
	[Serializable]
	public struct PoseMeasureParameters
	{
		public float PositionRotationWeight
		{
			get
			{
				return this._positionRotationWeight;
			}
		}

		public PoseMeasureParameters(float positionRotationWeight)
		{
			this._positionRotationWeight = positionRotationWeight;
		}

		public static PoseMeasureParameters Lerp(in PoseMeasureParameters from, in PoseMeasureParameters to, float t)
		{
			return new PoseMeasureParameters(Mathf.Lerp(from._positionRotationWeight, to._positionRotationWeight, t));
		}

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Weights the scoring of the pose based more in the amount of translationor rotation needed to align the interactor with the desired pose.")]
		private float _positionRotationWeight;

		public static readonly PoseMeasureParameters DEFAULT = new PoseMeasureParameters(0f);
	}
}
