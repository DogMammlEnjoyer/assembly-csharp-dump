using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class DominantHandRef : MonoBehaviour, IHand, IActiveState
	{
		public IHand LeftHand { get; private set; }

		public IHand RightHand { get; private set; }

		public bool SelectDominant
		{
			get
			{
				return this._selectDominant;
			}
			set
			{
				this._selectDominant = value;
			}
		}

		public IHand Hand
		{
			get
			{
				if (this.LeftHand.IsDominantHand != this._selectDominant)
				{
					return this.RightHand;
				}
				return this.LeftHand;
			}
		}

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
				this._whenHandUpdated = (Action)Delegate.Combine(this._whenHandUpdated, value);
			}
			remove
			{
				this._whenHandUpdated = (Action)Delegate.Remove(this._whenHandUpdated, value);
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
			this.LeftHand = (this._leftHand as IHand);
			this.RightHand = (this._rightHand as IHand);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.LeftHand.WhenHandUpdated += this.HandleLeftHandUpdated;
				this.RightHand.WhenHandUpdated += this.HandleRightHandUpdated;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.LeftHand.WhenHandUpdated -= this.HandleLeftHandUpdated;
				this.RightHand.WhenHandUpdated -= this.HandleRightHandUpdated;
			}
		}

		private void HandleLeftHandUpdated()
		{
			if (this.LeftHand.IsDominantHand == this._selectDominant)
			{
				this._whenHandUpdated();
			}
		}

		private void HandleRightHandUpdated()
		{
			if (this.RightHand.IsDominantHand == this._selectDominant)
			{
				this._whenHandUpdated();
			}
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

		public void InjectAllDominantHandRef(IHand leftHand, IHand rightHand)
		{
			this.InjectLeftHand(leftHand);
			this.InjectRightHand(rightHand);
		}

		public void InjectLeftHand(IHand leftHand)
		{
			this._leftHand = (leftHand as Object);
			this.LeftHand = leftHand;
		}

		public void InjectRightHand(IHand rightHand)
		{
			this._rightHand = (rightHand as Object);
			this.RightHand = rightHand;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _leftHand;

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _rightHand;

		[SerializeField]
		[Tooltip("If true, the HandRef will point to the Dominant hand. If false it will point to the Non Dominant Hand")]
		private bool _selectDominant = true;

		private Action _whenHandUpdated = delegate()
		{
		};

		protected bool _started;
	}
}
