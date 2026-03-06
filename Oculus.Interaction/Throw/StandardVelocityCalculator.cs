using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Throw
{
	[Obsolete("Use RANSACVelocityCalculator instead")]
	public class StandardVelocityCalculator : MonoBehaviour, IVelocityCalculator, IThrowVelocityCalculator, ITimeConsumer
	{
		public IPoseInputDevice ThrowInputDevice { get; private set; }

		public float UpdateFrequency
		{
			get
			{
				return this._updateFrequency;
			}
		}

		public Vector3 ReferenceOffset
		{
			get
			{
				return this._referenceOffset;
			}
			set
			{
				this._referenceOffset = value;
			}
		}

		public float InstantVelocityInfluence
		{
			get
			{
				return this._instantVelocityInfluence;
			}
			set
			{
				this._instantVelocityInfluence = value;
			}
		}

		public float TrendVelocityInfluence
		{
			get
			{
				return this._trendVelocityInfluence;
			}
			set
			{
				this._trendVelocityInfluence = value;
			}
		}

		public float TangentialVelocityInfluence
		{
			get
			{
				return this._tangentialVelocityInfluence;
			}
			set
			{
				this._tangentialVelocityInfluence = value;
			}
		}

		public float ExternalVelocityInfluence
		{
			get
			{
				return this._externalVelocityInfluence;
			}
			set
			{
				this._externalVelocityInfluence = value;
			}
		}

		public float StepBackTime
		{
			get
			{
				return this._stepBackTime;
			}
			set
			{
				this._stepBackTime = value;
			}
		}

		public float MaxPercentZeroSamplesTrendVeloc
		{
			get
			{
				return this._maxPercentZeroSamplesTrendVeloc;
			}
			set
			{
				this._maxPercentZeroSamplesTrendVeloc = value;
			}
		}

		public void SetTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		public Vector3 AddedInstantLinearVelocity { get; private set; }

		public Vector3 AddedTrendLinearVelocity { get; private set; }

		public Vector3 AddedTangentialLinearVelocity { get; private set; }

		public Vector3 AxisOfRotation { get; private set; }

		public Vector3 CenterOfMassToObject { get; private set; }

		public Vector3 TangentialDirection { get; private set; }

		public Vector3 AxisOfRotationOrigin { get; private set; }

		public event Action<List<ReleaseVelocityInformation>> WhenThrowVelocitiesChanged = delegate(List<ReleaseVelocityInformation> <p0>)
		{
		};

		public event Action<ReleaseVelocityInformation> WhenNewSampleAvailable = delegate(ReleaseVelocityInformation <p0>)
		{
		};

		protected virtual void Awake()
		{
			this.ThrowInputDevice = (this._throwInputDevice as IPoseInputDevice);
		}

		protected virtual void Start()
		{
			this._bufferingParams.Validate();
			this._bufferSize = Mathf.CeilToInt(this._bufferingParams.BufferLengthSeconds * this._bufferingParams.SampleFrequency);
			this._bufferedPoses.Capacity = this._bufferSize;
			this._linearVelocityFilter = OneEuroFilter.CreateVector3();
		}

		public ReleaseVelocityInformation CalculateThrowVelocity(Transform objectThrown)
		{
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			this.IncludeInstantVelocities(this._timeProvider(), ref zero, ref zero2);
			this.IncludeTrendVelocities(ref zero, ref zero2);
			this.IncludeTangentialInfluence(ref zero, objectThrown.position);
			this.IncludeExternalVelocities(ref zero, ref zero2);
			this._currentThrowVelocities.Clear();
			int count = this._bufferedPoses.Count;
			int num = this._lastWritePos;
			for (int i = 0; i < count; i++)
			{
				if (num < 0)
				{
					num = count - 1;
				}
				StandardVelocityCalculator.SamplePoseData samplePoseData = this._bufferedPoses[num];
				ReleaseVelocityInformation item = new ReleaseVelocityInformation(samplePoseData.LinearVelocity, samplePoseData.AngularVelocity, samplePoseData.TransformPose.position, false);
				this._currentThrowVelocities.Add(item);
				num--;
			}
			ReleaseVelocityInformation releaseVelocityInformation = new ReleaseVelocityInformation(zero, zero2, (this._previousReferencePosition != null) ? this._previousReferencePosition.Value : Vector3.zero, true);
			this._currentThrowVelocities.Add(releaseVelocityInformation);
			this.WhenThrowVelocitiesChanged(this._currentThrowVelocities);
			this._bufferedPoses.Clear();
			this._lastWritePos = -1;
			this._linearVelocityFilter.Reset();
			return releaseVelocityInformation;
		}

		private void IncludeInstantVelocities(float currentTime, ref Vector3 linearVelocity, ref Vector3 angularVelocity)
		{
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			this.IncludeEstimatedReleaseVelocities(currentTime, ref zero, ref zero2);
			this.AddedInstantLinearVelocity = zero * this._instantVelocityInfluence;
			linearVelocity += this.AddedInstantLinearVelocity;
			angularVelocity += zero2 * this._instantVelocityInfluence;
		}

		private void IncludeEstimatedReleaseVelocities(float currentTime, ref Vector3 linearVelocity, ref Vector3 angularVelocity)
		{
			linearVelocity = this._linearVelocity;
			angularVelocity = this._angularVelocity;
			if (this._stepBackTime < Mathf.Epsilon)
			{
				return;
			}
			float num = currentTime - this._stepBackTime;
			ValueTuple<int, int> valueTuple = this.FindPoseIndicesAdjacentToTime(num);
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			if (item < 0 || item2 < 0)
			{
				return;
			}
			StandardVelocityCalculator.SamplePoseData samplePoseData = this._bufferedPoses[item];
			StandardVelocityCalculator.SamplePoseData samplePoseData2 = this._bufferedPoses[item2];
			float time = samplePoseData.Time;
			float time2 = samplePoseData2.Time;
			float t = (num - time) / (time2 - time);
			Vector3 vector = Vector3.Lerp(samplePoseData.LinearVelocity, samplePoseData2.LinearVelocity, t);
			Quaternion a = VelocityCalculatorUtilMethods.AngularVelocityToQuat(samplePoseData.AngularVelocity);
			Quaternion b = VelocityCalculatorUtilMethods.AngularVelocityToQuat(samplePoseData2.AngularVelocity);
			Vector3 vector2 = VelocityCalculatorUtilMethods.QuatToAngularVeloc(Quaternion.Slerp(a, b, t));
			linearVelocity = vector;
			angularVelocity = vector2;
		}

		private void IncludeTrendVelocities(ref Vector3 linearVelocity, ref Vector3 angularVelocity)
		{
			ValueTuple<Vector3, Vector3> valueTuple = this.ComputeTrendVelocities();
			Vector3 item = valueTuple.Item1;
			Vector3 item2 = valueTuple.Item2;
			this.AddedTrendLinearVelocity = item * this._trendVelocityInfluence;
			linearVelocity += this.AddedTrendLinearVelocity;
			angularVelocity += item2 * this._trendVelocityInfluence;
		}

		private void IncludeTangentialInfluence(ref Vector3 linearVelocity, Vector3 interactablePosition)
		{
			Vector3 a = this.CalculateTangentialVector(interactablePosition);
			this.AddedTangentialLinearVelocity = a * this._tangentialVelocityInfluence;
			linearVelocity += this.AddedTangentialLinearVelocity;
		}

		private void IncludeExternalVelocities(ref Vector3 linearVelocity, ref Vector3 angularVelocity)
		{
			ValueTuple<Vector3, Vector3> externalVelocities = this.ThrowInputDevice.GetExternalVelocities();
			Vector3 item = externalVelocities.Item1;
			Vector3 item2 = externalVelocities.Item2;
			float d = item.magnitude * this._externalVelocityInfluence;
			linearVelocity += linearVelocity.normalized * d;
			float d2 = item2.magnitude * this._externalVelocityInfluence;
			angularVelocity += angularVelocity.normalized * d2;
		}

		private ValueTuple<int, int> FindPoseIndicesAdjacentToTime(float time)
		{
			if (this._lastWritePos < 0)
			{
				return new ValueTuple<int, int>(-1, -1);
			}
			int item = -1;
			int item2 = -1;
			int count = this._bufferedPoses.Count;
			int num = this._lastWritePos;
			for (int i = 0; i < count; i++)
			{
				if (num < 0)
				{
					num = count - 1;
				}
				int num2 = num - 1;
				if (num2 < 0)
				{
					num2 = count - 1;
				}
				ref StandardVelocityCalculator.SamplePoseData ptr = this._bufferedPoses[num];
				StandardVelocityCalculator.SamplePoseData samplePoseData = this._bufferedPoses[num2];
				if (ptr.Time > time && samplePoseData.Time < time)
				{
					item = num2;
					item2 = num;
				}
				num--;
			}
			return new ValueTuple<int, int>(item, item2);
		}

		private ValueTuple<Vector3, Vector3> ComputeTrendVelocities()
		{
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			if (this._bufferedPoses.Count == 0)
			{
				return new ValueTuple<Vector3, Vector3>(Vector3.zero, Vector3.zero);
			}
			if (this.BufferedVelocitiesValid())
			{
				this.FindLargestWindowWithMovement();
				int count = this._windowWithMovement.Count;
				if (count == 0)
				{
					return new ValueTuple<Vector3, Vector3>(Vector3.zero, Vector3.zero);
				}
				foreach (StandardVelocityCalculator.SamplePoseData samplePoseData in this._windowWithMovement)
				{
					vector += samplePoseData.LinearVelocity;
					vector2 += samplePoseData.AngularVelocity;
				}
				vector /= (float)count;
				vector2 /= (float)count;
			}
			else
			{
				ValueTuple<Vector3, Vector3> valueTuple = this.FindMostRecentBufferedSampleWithMovement();
				vector = valueTuple.Item1;
				vector2 = valueTuple.Item2;
			}
			return new ValueTuple<Vector3, Vector3>(vector, vector2);
		}

		private bool BufferedVelocitiesValid()
		{
			int num = 0;
			using (List<StandardVelocityCalculator.SamplePoseData>.Enumerator enumerator = this._bufferedPoses.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.LinearVelocity.sqrMagnitude < Mathf.Epsilon)
					{
						num++;
					}
				}
			}
			int count = this._bufferedPoses.Count;
			return (float)num / (float)count <= this._maxPercentZeroSamplesTrendVeloc;
		}

		private void FindLargestWindowWithMovement()
		{
			int count = this._bufferedPoses.Count;
			bool flag = false;
			this._windowWithMovement.Clear();
			this._tempWindow.Clear();
			Vector3 vector = Vector3.zero;
			int num = this._lastWritePos;
			for (int i = 0; i < count; i++)
			{
				if (num < 0)
				{
					num = count - 1;
				}
				StandardVelocityCalculator.SamplePoseData samplePoseData = this._bufferedPoses[num];
				bool flag2 = samplePoseData.LinearVelocity.sqrMagnitude > 0f;
				if (flag2)
				{
					if (!flag)
					{
						flag = true;
						this._tempWindow.Clear();
						vector = samplePoseData.LinearVelocity;
					}
					if (Vector3.Dot(vector.normalized, samplePoseData.LinearVelocity.normalized) > 0.6f)
					{
						this._tempWindow.Add(samplePoseData);
					}
				}
				else if (!flag2 && flag)
				{
					flag = false;
					if (this._tempWindow.Count > this._windowWithMovement.Count)
					{
						this.TransferToDestBuffer(this._tempWindow, this._windowWithMovement);
					}
				}
				num--;
			}
			if (flag && this._tempWindow.Count > this._windowWithMovement.Count)
			{
				this.TransferToDestBuffer(this._tempWindow, this._windowWithMovement);
			}
		}

		private ValueTuple<Vector3, Vector3> FindMostRecentBufferedSampleWithMovement()
		{
			int count = this._bufferedPoses.Count;
			Vector3 item = Vector3.zero;
			Vector3 item2 = Vector3.zero;
			int num = this._lastWritePos;
			for (int i = 0; i < count; i++)
			{
				if (num < 0)
				{
					num = count - 1;
				}
				StandardVelocityCalculator.SamplePoseData samplePoseData = this._bufferedPoses[num];
				Vector3 linearVelocity = samplePoseData.LinearVelocity;
				Vector3 angularVelocity = samplePoseData.AngularVelocity;
				if (linearVelocity.sqrMagnitude > Mathf.Epsilon && angularVelocity.sqrMagnitude > Mathf.Epsilon)
				{
					item = linearVelocity;
					item2 = angularVelocity;
					break;
				}
				num--;
			}
			return new ValueTuple<Vector3, Vector3>(item, item2);
		}

		private void TransferToDestBuffer(List<StandardVelocityCalculator.SamplePoseData> source, List<StandardVelocityCalculator.SamplePoseData> dest)
		{
			dest.Clear();
			foreach (StandardVelocityCalculator.SamplePoseData item in source)
			{
				dest.Add(item);
			}
		}

		private Vector3 CalculateTangentialVector(Vector3 objectPosition)
		{
			if (this._previousReferencePosition == null)
			{
				return Vector3.zero;
			}
			float magnitude = this._angularVelocity.magnitude;
			if (magnitude < Mathf.Epsilon)
			{
				return Vector3.zero;
			}
			Vector3 vector = objectPosition - this._previousReferencePosition.Value;
			float magnitude2 = vector.magnitude;
			if (magnitude2 < Mathf.Epsilon)
			{
				return Vector3.zero;
			}
			Vector3 normalized = vector.normalized;
			Vector3 normalized2 = this._angularVelocity.normalized;
			Vector3 vector2 = Vector3.Cross(normalized2, normalized);
			this.AxisOfRotation = normalized2;
			this.TangentialDirection = vector2;
			this.CenterOfMassToObject = normalized * magnitude2;
			this.AxisOfRotationOrigin = objectPosition;
			return vector2 * magnitude2 * magnitude;
		}

		public IReadOnlyList<ReleaseVelocityInformation> LastThrowVelocities()
		{
			return this._currentThrowVelocities;
		}

		public void SetUpdateFrequency(float frequency)
		{
			if (frequency < Mathf.Epsilon)
			{
				Debug.LogError(string.Format("Provided frequency ${0} must be ", frequency) + "greater than or equal to zero.");
				return;
			}
			this._updateFrequency = frequency;
			this._updateLatency = 1f / this._updateFrequency;
		}

		protected virtual void LateUpdate()
		{
			float num = this._timeProvider();
			if (this._updateLatency > 0f && this._lastUpdateTime > 0f && num - this._lastUpdateTime < this._updateLatency)
			{
				return;
			}
			Pose pose;
			if (!this.ThrowInputDevice.IsInputValid || !this.ThrowInputDevice.IsHighConfidence || !this.ThrowInputDevice.GetRootPose(out pose))
			{
				return;
			}
			float delta = num - this._lastUpdateTime;
			this._lastUpdateTime = num;
			pose = new Pose(this._referenceOffset + pose.position, pose.rotation);
			this.CalculateLatestVelocitiesAndUpdateBuffer(delta, num, pose);
		}

		private void CalculateLatestVelocitiesAndUpdateBuffer(float delta, float currentTime, Pose referencePose)
		{
			this._accumulatedDelta += delta;
			this.UpdateLatestVelocitiesAndPoseValues(referencePose, this._accumulatedDelta);
			this._accumulatedDelta = 0f;
			int num = (this._lastWritePos < 0) ? 0 : ((this._lastWritePos + 1) % this._bufferSize);
			StandardVelocityCalculator.SamplePoseData samplePoseData = new StandardVelocityCalculator.SamplePoseData(referencePose, this._linearVelocity, this._angularVelocity, currentTime);
			if (this._bufferedPoses.Count <= num)
			{
				this._bufferedPoses.Add(samplePoseData);
			}
			else
			{
				this._bufferedPoses[num] = samplePoseData;
			}
			this._lastWritePos = num;
		}

		private void UpdateLatestVelocitiesAndPoseValues(Pose referencePose, float delta)
		{
			ValueTuple<Vector3, Vector3> latestLinearAndAngularVelocities = this.GetLatestLinearAndAngularVelocities(referencePose, delta);
			this._linearVelocity = latestLinearAndAngularVelocities.Item1;
			this._angularVelocity = latestLinearAndAngularVelocities.Item2;
			this._linearVelocity = this._linearVelocityFilter.Step(this._linearVelocity, 0.016666668f);
			ReleaseVelocityInformation obj = new ReleaseVelocityInformation(this._linearVelocity, this._angularVelocity, referencePose.position, false);
			this.WhenNewSampleAvailable(obj);
			this._previousReferencePosition = new Vector3?(referencePose.position);
			this._previousReferenceRotation = new Quaternion?(referencePose.rotation);
		}

		private ValueTuple<Vector3, Vector3> GetLatestLinearAndAngularVelocities(Pose referencePose, float delta)
		{
			if (this._previousReferencePosition == null || delta < Mathf.Epsilon)
			{
				return new ValueTuple<Vector3, Vector3>(Vector3.zero, Vector3.zero);
			}
			Vector3 item = (referencePose.position - this._previousReferencePosition.Value) / delta;
			Vector3 item2 = VelocityCalculatorUtilMethods.ToAngularVelocity(this._previousReferenceRotation.Value, referencePose.rotation, delta);
			return new ValueTuple<Vector3, Vector3>(item, item2);
		}

		public void InjectAllStandardVelocityCalculator(IPoseInputDevice poseInputDevice, StandardVelocityCalculator.BufferingParams bufferingParams)
		{
			this.InjectPoseInputDevice(poseInputDevice);
			this.InjectBufferingParams(bufferingParams);
		}

		public void InjectPoseInputDevice(IPoseInputDevice poseInputDevice)
		{
			this._throwInputDevice = (poseInputDevice as Object);
			this.ThrowInputDevice = poseInputDevice;
		}

		public void InjectBufferingParams(StandardVelocityCalculator.BufferingParams bufferingParams)
		{
			this._bufferingParams = bufferingParams;
		}

		[Obsolete("Use SetTimeProvider()")]
		public void InjectOptionalTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		[SerializeField]
		[Interface(typeof(IPoseInputDevice), new Type[]
		{

		})]
		private Object _throwInputDevice;

		[SerializeField]
		[Tooltip("The reference position is the center of mass of the hand or controller. Use this offset this in case the computed center of mass is not entirely correct.")]
		private Vector3 _referenceOffset = Vector3.zero;

		[SerializeField]
		[Tooltip("Related to buffering velocities; used for final velocity calculation.")]
		private StandardVelocityCalculator.BufferingParams _bufferingParams;

		[SerializeField]
		[Tooltip("Influence of latest velocities upon release.")]
		[Range(0f, 1f)]
		private float _instantVelocityInfluence = 1f;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Influence of derived velocities trend upon release.")]
		private float _trendVelocityInfluence = 1f;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Influence of tangential velcities upon release, which can be affected by rotational motion.")]
		private float _tangentialVelocityInfluence = 1f;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Influence of external velocities upon release. For hands, this can include fingers.")]
		private float _externalVelocityInfluence;

		[SerializeField]
		[Tooltip("Time of anticipated release. Hand tracking might experience greater latency compared to controllers.")]
		private float _stepBackTime = 0.08f;

		[SerializeField]
		[Tooltip("Trend velocity uses a window of velocities, assuming not too many of those velocities are zero. If they exceed a max percentage then a last resort method is used.")]
		private float _maxPercentZeroSamplesTrendVeloc = 0.5f;

		[Header("Sampling filtering.")]
		[SerializeField]
		private OneEuroFilterPropertyBlock _filterProps = OneEuroFilterPropertyBlock.Default;

		private float _updateFrequency = -1f;

		private float _updateLatency = -1f;

		private float _lastUpdateTime = -1f;

		private IOneEuroFilter<Vector3> _linearVelocityFilter;

		private Func<float> _timeProvider = () => Time.time;

		private List<ReleaseVelocityInformation> _currentThrowVelocities = new List<ReleaseVelocityInformation>();

		private Vector3 _linearVelocity = Vector3.zero;

		private Vector3 _angularVelocity = Vector3.zero;

		private Vector3? _previousReferencePosition;

		private Quaternion? _previousReferenceRotation;

		private float _accumulatedDelta;

		private List<StandardVelocityCalculator.SamplePoseData> _bufferedPoses = new List<StandardVelocityCalculator.SamplePoseData>();

		private int _lastWritePos = -1;

		private int _bufferSize = -1;

		private List<StandardVelocityCalculator.SamplePoseData> _windowWithMovement = new List<StandardVelocityCalculator.SamplePoseData>();

		private List<StandardVelocityCalculator.SamplePoseData> _tempWindow = new List<StandardVelocityCalculator.SamplePoseData>();

		private const float _TREND_DOT_THRESHOLD = 0.6f;

		[Serializable]
		public class BufferingParams
		{
			public void Validate()
			{
			}

			public float BufferLengthSeconds = 0.4f;

			public float SampleFrequency = 90f;
		}

		private struct SamplePoseData
		{
			public SamplePoseData(Pose transformPose, Vector3 linearVelocity, Vector3 angularVelocity, float time)
			{
				this.TransformPose = transformPose;
				this.LinearVelocity = linearVelocity;
				this.AngularVelocity = angularVelocity;
				this.Time = time;
			}

			public readonly Pose TransformPose;

			public readonly Vector3 LinearVelocity;

			public readonly Vector3 AngularVelocity;

			public readonly float Time;
		}
	}
}
