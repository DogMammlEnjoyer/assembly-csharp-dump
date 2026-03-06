using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Throw
{
	public class HandPoseInputDevice : MonoBehaviour, IPoseInputDevice
	{
		public IHand Hand { get; private set; }

		public float BufferLengthSeconds
		{
			get
			{
				return this._bufferLengthSeconds;
			}
			set
			{
				this._bufferLengthSeconds = value;
			}
		}

		public float SampleFrequency
		{
			get
			{
				return this._sampleFrequency;
			}
			set
			{
				this._sampleFrequency = value;
			}
		}

		public bool IsInputValid
		{
			get
			{
				return this.Hand.IsTrackedDataValid;
			}
		}

		public bool IsHighConfidence
		{
			get
			{
				return this.Hand.IsHighConfidence;
			}
		}

		public bool GetRootPose(out Pose pose)
		{
			pose = Pose.identity;
			if (!this.IsInputValid)
			{
				return false;
			}
			if (!this.Hand.GetJointPose(HandJointId.HandWristRoot, out pose))
			{
				return false;
			}
			Pose pose2;
			if (!this.Hand.GetPalmPoseLocal(out pose2))
			{
				return false;
			}
			ref pose2.Postmultiply(pose);
			pose = pose2;
			return true;
		}

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
		}

		protected virtual void Start()
		{
			this._bufferSize = Mathf.CeilToInt(this._bufferLengthSeconds * this._sampleFrequency);
		}

		protected virtual void LateUpdate()
		{
			this.BufferFingerVelocities();
		}

		private void BufferFingerVelocities()
		{
			if (!this.IsInputValid)
			{
				return;
			}
			this.AllocateFingerBonesArrayIfNecessary();
			this.BufferFingerBoneVelocities();
		}

		private void AllocateFingerBonesArrayIfNecessary()
		{
			if (this._jointPoseInfoArray != null)
			{
				return;
			}
			this._jointPoseInfoArray = new HandPoseInputDevice.HandJointPoseMetaData[]
			{
				new HandPoseInputDevice.HandJointPoseMetaData(HandFinger.Thumb, HandJointId.HandThumb3, this._bufferSize),
				new HandPoseInputDevice.HandJointPoseMetaData(HandFinger.Index, HandJointId.HandIndex3, this._bufferSize),
				new HandPoseInputDevice.HandJointPoseMetaData(HandFinger.Middle, HandJointId.HandMiddle3, this._bufferSize),
				new HandPoseInputDevice.HandJointPoseMetaData(HandFinger.Ring, HandJointId.HandRing3, this._bufferSize),
				new HandPoseInputDevice.HandJointPoseMetaData(HandFinger.Pinky, HandJointId.HandPinky3, this._bufferSize)
			};
		}

		private bool GetFingerIsHighConfidence(HandFinger handFinger)
		{
			return this.Hand.IsTrackedDataValid && this.Hand.GetFingerIsHighConfidence(handFinger);
		}

		private bool GetJointPose(HandJointId handJointId, out Pose pose)
		{
			pose = Pose.identity;
			return this.Hand.IsTrackedDataValid && this.Hand.GetJointPose(handJointId, out pose);
		}

		private void BufferFingerBoneVelocities()
		{
			float deltaTime = Time.deltaTime;
			foreach (HandPoseInputDevice.HandJointPoseMetaData handJointPoseMetaData in this._jointPoseInfoArray)
			{
				Pose newPose;
				if (this.GetFingerIsHighConfidence(handJointPoseMetaData.Finger) && this.GetJointPose(handJointPoseMetaData.JointId, out newPose))
				{
					handJointPoseMetaData.BufferNewValue(newPose, deltaTime);
				}
			}
		}

		public ValueTuple<Vector3, Vector3> GetExternalVelocities()
		{
			if (this._jointPoseInfoArray == null || this._jointPoseInfoArray.Length == 0)
			{
				return new ValueTuple<Vector3, Vector3>(Vector3.zero, Vector3.zero);
			}
			Vector3 vector = Vector3.zero;
			foreach (HandPoseInputDevice.HandJointPoseMetaData handJointPoseMetaData in this._jointPoseInfoArray)
			{
				vector += handJointPoseMetaData.GetAverageVelocityVector();
			}
			vector /= (float)this._jointPoseInfoArray.Length;
			HandPoseInputDevice.HandJointPoseMetaData[] jointPoseInfoArray = this._jointPoseInfoArray;
			for (int i = 0; i < jointPoseInfoArray.Length; i++)
			{
				jointPoseInfoArray[i].ResetSpeedsBuffer();
			}
			return new ValueTuple<Vector3, Vector3>(vector, Vector3.zero);
		}

		public void InjectAllHandPoseInputDevice(IHand hand)
		{
			this.InjectHand(hand);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[SerializeField]
		private float _bufferLengthSeconds = 0.1f;

		[SerializeField]
		private float _sampleFrequency = 90f;

		private int _bufferSize = -1;

		private HandPoseInputDevice.HandJointPoseMetaData[] _jointPoseInfoArray;

		private class HandJointPoseMetaData
		{
			public HandJointPoseMetaData(HandFinger finger, HandJointId joint, int bufferLength)
			{
				this.Finger = finger;
				this.JointId = joint;
				this.Velocities = new List<Vector3>();
				this._previousPosition = null;
				this._lastWritePos = -1;
				this._bufferLength = bufferLength;
			}

			public void BufferNewValue(Pose newPose, float delta)
			{
				Vector3 position = newPose.position;
				Vector3 vector = Vector3.zero;
				if (delta > Mathf.Epsilon && this._previousPosition != null)
				{
					vector = (position - this._previousPosition.Value) / delta;
				}
				int num = (this._lastWritePos < 0) ? 0 : ((this._lastWritePos + 1) % this._bufferLength);
				if (this.Velocities.Count <= num)
				{
					this.Velocities.Add(vector);
				}
				else
				{
					this.Velocities[num] = vector;
				}
				this._previousPosition = new Vector3?(position);
				this._lastWritePos = num;
			}

			public Vector3 GetAverageVelocityVector()
			{
				int count = this.Velocities.Count;
				if (count == 0)
				{
					return Vector3.zero;
				}
				Vector3 vector = Vector3.zero;
				foreach (Vector3 b in this.Velocities)
				{
					vector += b;
				}
				vector /= (float)count;
				return vector;
			}

			public void ResetSpeedsBuffer()
			{
				this.Velocities.Clear();
				this._lastWritePos = -1;
				this._previousPosition = null;
			}

			public readonly HandFinger Finger;

			public readonly HandJointId JointId;

			public readonly List<Vector3> Velocities;

			private Vector3? _previousPosition;

			private int _lastWritePos;

			private int _bufferLength;
		}
	}
}
