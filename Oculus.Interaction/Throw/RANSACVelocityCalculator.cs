using System;
using UnityEngine;

namespace Oculus.Interaction.Throw
{
	[Obsolete("Use Grabbable instead")]
	public class RANSACVelocityCalculator : MonoBehaviour, IThrowVelocityCalculator, ITimeConsumer
	{
		public IPoseInputDevice PoseInputDevice { get; private set; }

		public void SetTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		protected virtual void Awake()
		{
			this.PoseInputDevice = (this._poseInputDevice as IPoseInputDevice);
		}

		protected virtual void Start()
		{
			this._ransac.Initialize();
		}

		private void Update()
		{
			this.ProcessInput();
		}

		public ReleaseVelocityInformation CalculateThrowVelocity(Transform objectThrown)
		{
			this.ProcessInput();
			return this.GetThrowInformation(objectThrown.GetPose(Space.World));
		}

		private void ProcessInput()
		{
			Pose pose;
			this.PoseInputDevice.GetRootPose(out pose);
			bool isHighConfidence = this.PoseInputDevice.IsInputValid && this.PoseInputDevice.IsHighConfidence && pose.position.sqrMagnitude != this._previousPositionId;
			this._ransac.Process(pose, this._timeProvider(), isHighConfidence);
			this._previousPositionId = pose.position.sqrMagnitude;
		}

		private ReleaseVelocityInformation GetThrowInformation(Pose grabPoint)
		{
			Vector3 position = grabPoint.position;
			Pose pose;
			this.PoseInputDevice.GetRootPose(out pose);
			Pose offset = PoseUtils.Delta(pose, grabPoint);
			Vector3 linearVelocity;
			Vector3 angularVelocity;
			this._ransac.GetOffsettedVelocities(offset, out linearVelocity, out angularVelocity);
			return new ReleaseVelocityInformation(linearVelocity, angularVelocity, position, true);
		}

		public void InjectAllRANSACVelocityCalculator(IPoseInputDevice poseInputDevice)
		{
			this.InjectPoseInputDevice(poseInputDevice);
		}

		public void InjectPoseInputDevice(IPoseInputDevice poseInputDevice)
		{
			this.PoseInputDevice = poseInputDevice;
			this._poseInputDevice = (poseInputDevice as Object);
		}

		[SerializeField]
		[Interface(typeof(IPoseInputDevice), new Type[]
		{

		})]
		private Object _poseInputDevice;

		private Func<float> _timeProvider = () => Time.time;

		private float _previousPositionId;

		private RANSACVelocityCalculator.RANSACOffsettedVelocity _ransac = new RANSACVelocityCalculator.RANSACOffsettedVelocity(8, 2);

		private class RANSACOffsettedVelocity : RANSACVelocity
		{
			[Obsolete("The minHighConfidenceSamples parameter will be ignored. Use the constructor without it")]
			public RANSACOffsettedVelocity(int samplesCount = 10, int samplesDeadZone = 2, int minHighConfidenceSamples = 2) : base(samplesCount, samplesDeadZone)
			{
			}

			public RANSACOffsettedVelocity(int samplesCount = 10, int samplesDeadZone = 2) : base(samplesCount, samplesDeadZone)
			{
			}

			public void GetOffsettedVelocities(Pose offset, out Vector3 velocity, out Vector3 torque)
			{
				this._offset = offset;
				base.GetVelocities(out velocity, out torque);
				this._offset = Pose.identity;
			}

			protected override Vector3 PositionOffset(Pose youngerPose, Pose olderPose)
			{
				return PoseUtils.Multiply(youngerPose, this._offset).position - PoseUtils.Multiply(olderPose, this._offset).position;
			}

			private Pose _offset = Pose.identity;
		}
	}
}
