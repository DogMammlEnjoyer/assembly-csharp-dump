using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class WristAngleActiveState : MonoBehaviour, IActiveState
	{
		public IHand Hand { get; private set; }

		public float MinAngle
		{
			get
			{
				return this._minAngle;
			}
			set
			{
				this._minAngle = value;
			}
		}

		public float MaxAngle
		{
			get
			{
				return this._maxAngle;
			}
			set
			{
				this._maxAngle = value;
			}
		}

		public bool Active { get; private set; }

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void Update()
		{
			this._currentAngle = this.CalculateAngle();
			this.Active = (this._currentAngle > this._minAngle && this._currentAngle < this._maxAngle);
		}

		private float CalculateAngle()
		{
			Pose pose;
			if (!this.Hand.GetJointPose(HandJointId.HandWristRoot, out pose))
			{
				return this._currentAngle;
			}
			bool flag = this.Hand.Handedness == Handedness.Right;
			Vector3 up = Vector3.up;
			Vector3 normalized = (pose.position - this._shoulder.position).normalized;
			Vector3 vector = Vector3.Cross(up, normalized).normalized;
			vector = (flag ? vector : (-vector));
			float num = Vector3.SignedAngle(Vector3.ProjectOnPlane(pose.rotation * (flag ? Constants.RightThumbSide : Constants.LeftThumbSide), normalized).normalized, vector, normalized);
			num = ((this.Hand.Handedness == Handedness.Right) ? (-num) : num);
			if (num < -70f)
			{
				num += 360f;
			}
			return num;
		}

		public void InjectAllWristAngleActiveState(IHand hand, Transform shoulder)
		{
			this.InjectHand(hand);
			this.InjectShoulder(shoulder);
		}

		public void InjectHand(IHand hand)
		{
			this.Hand = hand;
			this._hand = (hand as Object);
		}

		public void InjectShoulder(Transform shoulder)
		{
			this._shoulder = shoulder;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[SerializeField]
		private Transform _shoulder;

		[SerializeField]
		private float _minAngle = -70f;

		[SerializeField]
		private float _maxAngle = 170f;

		private float _currentAngle;

		private const float _wristLimit = -70f;

		protected bool _started;
	}
}
