using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class HandRef : MonoBehaviour, IHand, IActiveState
	{
		public IHand Hand { get; private set; }

		public Handedness Handedness
		{
			get
			{
				return this.Hand.Handedness;
			}
		}

		public bool IsConnected
		{
			get
			{
				return this.Hand.IsConnected;
			}
		}

		public bool IsHighConfidence
		{
			get
			{
				return this.Hand.IsHighConfidence;
			}
		}

		public bool IsDominantHand
		{
			get
			{
				return this.Hand.IsDominantHand;
			}
		}

		public float Scale
		{
			get
			{
				return this.Hand.Scale;
			}
		}

		public bool IsPointerPoseValid
		{
			get
			{
				return this.Hand.IsPointerPoseValid;
			}
		}

		public bool IsTrackedDataValid
		{
			get
			{
				return this.Hand.IsTrackedDataValid;
			}
		}

		public int CurrentDataVersion
		{
			get
			{
				return this.Hand.CurrentDataVersion;
			}
		}

		public event Action WhenHandUpdated
		{
			add
			{
				this.Hand.WhenHandUpdated += value;
			}
			remove
			{
				this.Hand.WhenHandUpdated -= value;
			}
		}

		public bool Active
		{
			get
			{
				return this.IsConnected;
			}
		}

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
		}

		protected virtual void Start()
		{
		}

		public bool GetFingerIsPinching(HandFinger finger)
		{
			return this.Hand.GetFingerIsPinching(finger);
		}

		public bool GetIndexFingerIsPinching()
		{
			return this.Hand.GetIndexFingerIsPinching();
		}

		public bool GetPointerPose(out Pose pose)
		{
			return this.Hand.GetPointerPose(out pose);
		}

		public bool GetJointPose(HandJointId handJointId, out Pose pose)
		{
			return this.Hand.GetJointPose(handJointId, out pose);
		}

		public bool GetJointPoseLocal(HandJointId handJointId, out Pose pose)
		{
			return this.Hand.GetJointPoseLocal(handJointId, out pose);
		}

		public bool GetJointPosesLocal(out ReadOnlyHandJointPoses jointPosesLocal)
		{
			return this.Hand.GetJointPosesLocal(out jointPosesLocal);
		}

		public bool GetJointPoseFromWrist(HandJointId handJointId, out Pose pose)
		{
			return this.Hand.GetJointPoseFromWrist(handJointId, out pose);
		}

		public bool GetJointPosesFromWrist(out ReadOnlyHandJointPoses jointPosesFromWrist)
		{
			return this.Hand.GetJointPosesFromWrist(out jointPosesFromWrist);
		}

		public bool GetPalmPoseLocal(out Pose pose)
		{
			return this.Hand.GetPalmPoseLocal(out pose);
		}

		public bool GetFingerIsHighConfidence(HandFinger finger)
		{
			return this.Hand.GetFingerIsHighConfidence(finger);
		}

		public float GetFingerPinchStrength(HandFinger finger)
		{
			return this.Hand.GetFingerPinchStrength(finger);
		}

		public bool GetRootPose(out Pose pose)
		{
			return this.Hand.GetRootPose(out pose);
		}

		public void InjectAllHandRef(IHand hand)
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
	}
}
