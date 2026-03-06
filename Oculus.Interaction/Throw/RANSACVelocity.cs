using System;
using UnityEngine;

namespace Oculus.Interaction.Throw
{
	public class RANSACVelocity
	{
		public float MaxSyntheticSpeed
		{
			get
			{
				return this._maxSyntheticSpeed;
			}
			set
			{
				this._maxSyntheticSpeed = Mathf.Max(0.0001f, value);
			}
		}

		[Obsolete("The minHighConfidenceSamples parameter will be ignored. Use the constructor without it")]
		public RANSACVelocity(int samplesCount = 10, int samplesDeadZone = 2, int minHighConfidenceSamples = 2) : this(samplesCount, samplesDeadZone)
		{
		}

		public RANSACVelocity(int samplesCount = 10, int samplesDeadZone = 2)
		{
			this._poses = new RingBuffer<RANSACVelocity.TimedPose>(samplesCount);
			this._ransac = new RandomSampleConsensus<Vector3>(samplesCount, samplesDeadZone);
		}

		public void Initialize()
		{
			this._poses.Clear();
			this._highConfidenceStreak = false;
		}

		public void Process(Pose pose, float time, bool isHighConfidence = true)
		{
			if (this._poses.Count > 0 && this._poses.Peek(0).time == time)
			{
				return;
			}
			if (!isHighConfidence)
			{
				this._highConfidenceStreak = false;
			}
			else
			{
				if (!this._highConfidenceStreak && this._poses.Count > 0)
				{
					RANSACVelocity.TimedPose timedPose = this._poses.Peek(0);
					this._poses.Clear();
					float num = Vector3.Distance(pose.position, timedPose.pose.position);
					float num2 = time - this._lastProcessedTime;
					if (Mathf.Approximately(num2, 0f) || num / num2 > this._maxSyntheticSpeed)
					{
						timedPose.time = time - num / this._maxSyntheticSpeed;
					}
					else
					{
						timedPose.time = this._lastProcessedTime;
					}
					this._poses.Add(timedPose);
				}
				this._highConfidenceStreak = true;
				RANSACVelocity.TimedPose item = new RANSACVelocity.TimedPose(time, pose);
				this._poses.Add(item);
			}
			this._lastProcessedTime = time;
		}

		public void GetVelocities(out Vector3 velocity, out Vector3 torque)
		{
			if (this._poses.Count >= 2)
			{
				velocity = this._ransac.FindOptimalModel(new RandomSampleConsensus<Vector3>.GenerateModel(this.CalculateVelocityFromSamples), new RandomSampleConsensus<Vector3>.EvaluateModelScore(this.ScoreDistance), this._poses.Count);
				torque = this._ransac.FindOptimalModel(new RandomSampleConsensus<Vector3>.GenerateModel(this.CalculateTorqueFromSamples), new RandomSampleConsensus<Vector3>.EvaluateModelScore(this.ScoreAngularDistance), this._poses.Count);
				return;
			}
			velocity = Vector3.zero;
			torque = Vector3.zero;
		}

		private Vector3 CalculateVelocityFromSamples(int idx1, int idx2)
		{
			RANSACVelocity.TimedPose timedPose;
			RANSACVelocity.TimedPose timedPose2;
			this.GetSortedTimePoses(idx1, idx2, out timedPose, out timedPose2);
			float d = timedPose2.time - timedPose.time;
			return this.PositionOffset(timedPose2.pose, timedPose.pose) / d;
		}

		private Vector3 CalculateTorqueFromSamples(int idx1, int idx2)
		{
			RANSACVelocity.TimedPose older;
			RANSACVelocity.TimedPose younger;
			this.GetSortedTimePoses(idx1, idx2, out older, out younger);
			return RANSACVelocity.GetTorque(older, younger);
		}

		protected virtual Vector3 PositionOffset(Pose youngerPose, Pose olderPose)
		{
			return youngerPose.position - olderPose.position;
		}

		private float ScoreDistance(Vector3 distance, Vector3[,] distances)
		{
			float num = 0f;
			for (int i = 0; i < this._poses.Count; i++)
			{
				for (int j = i + 1; j < this._poses.Count; j++)
				{
					num += (distance - distances[i, j]).sqrMagnitude;
				}
			}
			return num;
		}

		protected void GetSortedTimePoses(int idx1, int idx2, out RANSACVelocity.TimedPose older, out RANSACVelocity.TimedPose younger)
		{
			int index = idx1;
			int index2 = idx2;
			if (idx2 > idx1)
			{
				index = idx2;
				index2 = idx1;
			}
			older = this._poses[index2];
			younger = this._poses[index];
		}

		private float ScoreAngularDistance(Vector3 angularDistance, Vector3[,] angularDistances)
		{
			float num = 0f;
			Quaternion a = Quaternion.Euler(angularDistance);
			for (int i = 0; i < this._poses.Count; i++)
			{
				for (int j = i + 1; j < this._poses.Count; j++)
				{
					Quaternion b = Quaternion.Euler(angularDistances[i, j]);
					num += Mathf.Abs(Quaternion.Dot(a, b));
				}
			}
			return num;
		}

		protected static Vector3 GetTorque(RANSACVelocity.TimedPose older, RANSACVelocity.TimedPose younger)
		{
			float num = younger.time - older.time;
			Quaternion rotation = older.pose.rotation;
			Quaternion rotation2 = younger.pose.rotation;
			if (Quaternion.Dot(rotation, rotation2) < 0f)
			{
				rotation2.x = -rotation2.x;
				rotation2.y = -rotation2.y;
				rotation2.z = -rotation2.z;
				rotation2.w = -rotation2.w;
			}
			float num2;
			Vector3 a;
			(rotation2 * Quaternion.Inverse(rotation)).ToAngleAxis(out num2, out a);
			num2 = num2 * 0.017453292f / num;
			return a * num2;
		}

		private bool _highConfidenceStreak;

		private float _lastProcessedTime;

		private float _maxSyntheticSpeed = 5f;

		private const float _minSyntheticSpeed = 0.0001f;

		private RandomSampleConsensus<Vector3> _ransac;

		private RingBuffer<RANSACVelocity.TimedPose> _poses;

		protected struct TimedPose
		{
			public TimedPose(float time, Pose pose)
			{
				this.time = time;
				this.pose = pose;
			}

			public float time;

			public Pose pose;
		}
	}
}
