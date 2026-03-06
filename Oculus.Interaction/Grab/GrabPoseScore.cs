using System;
using UnityEngine;

namespace Oculus.Interaction.Grab
{
	public struct GrabPoseScore
	{
		public GrabPoseScore(float translationScore, float rotationScore, PoseMeasureParameters measureParameters)
		{
			this._translationScore = translationScore;
			this._rotationScore = rotationScore;
			this._measureParameters = measureParameters;
		}

		public GrabPoseScore(Vector3 fromPoint, Vector3 toPoint, bool isInside = false)
		{
			this._translationScore = GrabPoseScore.PositionalScore(fromPoint, toPoint);
			this._rotationScore = 0f;
			this._measureParameters = PoseMeasureParameters.DEFAULT;
			if (isInside)
			{
				this._translationScore = -Mathf.Abs(this._translationScore);
			}
		}

		public GrabPoseScore(in Pose poseA, in Pose poseB, in Pose offset, PoseMeasureParameters measureParameters)
		{
			Pose pose = PoseUtils.Multiply(poseA, offset);
			Pose pose2 = PoseUtils.Multiply(poseB, offset);
			this._translationScore = GrabPoseScore.PositionalScore(pose.position, pose2.position);
			this._rotationScore = GrabPoseScore.RotationalScore(pose.rotation, pose2.rotation);
			this._measureParameters = measureParameters;
		}

		public bool IsValid()
		{
			return this._translationScore != float.PositiveInfinity && this._rotationScore != float.PositiveInfinity;
		}

		private float Score(float maxDistance)
		{
			return Mathf.Lerp(this._translationScore, this._rotationScore * maxDistance, this._measureParameters.PositionRotationWeight);
		}

		private static float PositionalScore(in Vector3 from, in Vector3 to)
		{
			return (from - to).sqrMagnitude;
		}

		private static float RotationalScore(in Quaternion from, in Quaternion to)
		{
			float num = Vector3.Dot(from * Vector3.forward, to * Vector3.forward) * 0.5f + 0.5f;
			float num2 = Vector3.Dot(from * Vector3.up, to * Vector3.up) * 0.5f + 0.5f;
			return 1f - num * num2;
		}

		public static GrabPoseScore Lerp(in GrabPoseScore from, in GrabPoseScore to, float t)
		{
			return new GrabPoseScore(Mathf.Lerp(from._translationScore, to._translationScore, t), Mathf.Lerp(from._rotationScore, to._rotationScore, t), PoseMeasureParameters.Lerp(from._measureParameters, to._measureParameters, t));
		}

		public bool IsBetterThan(GrabPoseScore referenceScore)
		{
			if (this._translationScore == float.PositiveInfinity)
			{
				return false;
			}
			if (referenceScore._translationScore == float.PositiveInfinity)
			{
				return true;
			}
			float maxDistance = Mathf.Max(this._translationScore, referenceScore._translationScore);
			float num = this.Score(maxDistance);
			float num2 = referenceScore.Score(maxDistance);
			return (num < 0f && num2 > 0f) || (num < 0f && num2 < 0f && num > num2) || (num > 0f && num2 > 0f && num < num2);
		}

		private float _translationScore;

		private float _rotationScore;

		private PoseMeasureParameters _measureParameters;

		public static readonly GrabPoseScore Max = new GrabPoseScore(float.PositiveInfinity, float.PositiveInfinity, PoseMeasureParameters.DEFAULT);
	}
}
