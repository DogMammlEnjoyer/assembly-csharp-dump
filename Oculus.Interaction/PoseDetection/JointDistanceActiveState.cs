using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	public class JointDistanceActiveState : MonoBehaviour, IActiveState
	{
		public HandJointId JointIdA
		{
			get
			{
				return this._jointA;
			}
			set
			{
				this._jointA = value;
			}
		}

		public HandJointId JointIdB
		{
			get
			{
				return this._jointB;
			}
			set
			{
				this._jointB = value;
			}
		}

		public bool Active
		{
			get
			{
				if (!base.isActiveAndEnabled)
				{
					return false;
				}
				this.UpdateActiveState();
				return this._activeState;
			}
		}

		protected virtual void Awake()
		{
			this.HandA = (this._handA as IHand);
			this.HandB = (this._handB as IHand);
		}

		protected virtual void Start()
		{
		}

		protected virtual void Update()
		{
			this.UpdateActiveState();
		}

		private void UpdateActiveState()
		{
			if (Time.frameCount <= this._lastStateUpdateFrame)
			{
				return;
			}
			this._lastStateUpdateFrame = Time.frameCount;
			bool flag = this.JointDistanceWithinThreshold();
			if (flag != this._internalState)
			{
				this._internalState = flag;
				this._lastStateChangeTime = Time.unscaledTime;
			}
			if (Time.unscaledTime - this._lastStateChangeTime >= this._minTimeInState)
			{
				this._activeState = this._internalState;
			}
		}

		private bool JointDistanceWithinThreshold()
		{
			Pose pose;
			Pose pose2;
			if (this.HandA.GetJointPose(this.JointIdA, out pose) && this.HandB.GetJointPose(this.JointIdB, out pose2))
			{
				float num = this._internalState ? (this._distance + this._thresholdWidth * 0.5f) : (this._distance - this._thresholdWidth * 0.5f);
				return Vector3.Distance(pose.position, pose2.position) <= num;
			}
			return false;
		}

		public void InjectAllJointDistanceActiveState(IHand handA, IHand handB)
		{
			this.InjectHandA(handA);
			this.InjectHandB(handB);
		}

		public void InjectHandA(IHand handA)
		{
			this._handA = (handA as Object);
			this.HandA = handA;
		}

		[Obsolete("Use the JointIdA setter instead")]
		public void InjectJointIdA(HandJointId jointIdA)
		{
			this.JointIdA = jointIdA;
		}

		public void InjectHandB(IHand handB)
		{
			this._handB = (handB as Object);
			this.HandB = handB;
		}

		[Obsolete("Use the JointIdB setter instead")]
		public void InjectJointIdB(HandJointId jointIdB)
		{
			this.JointIdB = jointIdB;
		}

		public void InjectOptionalDistance(float val)
		{
			this._distance = val;
		}

		public void InjectOptionalThresholdWidth(float val)
		{
			this._thresholdWidth = val;
		}

		public void InjectOptionalMinTimeInState(float val)
		{
			this._minTimeInState = val;
		}

		[Tooltip("The IHand that JointIdA will be sourced from.")]
		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _handA;

		private IHand HandA;

		[Tooltip("The joint of HandA to use for distance check.")]
		[SerializeField]
		private HandJointId _jointIdA;

		[Tooltip("The joint of HandA to use for distance check.")]
		[SerializeField]
		private HandJointId _jointA;

		[Tooltip("The IHand that JointIdB will be sourced from.")]
		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _handB;

		private IHand HandB;

		[Tooltip("The joint of HandB to use for distance check.")]
		[SerializeField]
		private HandJointId _jointIdB;

		[Tooltip("The joint of HandB to use for distance check.")]
		[SerializeField]
		private HandJointId _jointB;

		[Tooltip("The ActiveState will become Active when joints are within this distance from each other.")]
		[SerializeField]
		private float _distance = 0.05f;

		[Tooltip("The distance value will be modified by this width to create differing enter/exit thresholds. Used to prevent chattering at the threshold edge.")]
		[SerializeField]
		private float _thresholdWidth = 0.02f;

		[Tooltip("A new state must be maintaned for at least this many seconds before the Active property changes.")]
		[SerializeField]
		private float _minTimeInState = 0.05f;

		private bool _activeState;

		private bool _internalState;

		private float _lastStateChangeTime;

		private int _lastStateUpdateFrame;
	}
}
