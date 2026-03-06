using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	public class ColliderContainsHandJointActiveState : MonoBehaviour, IActiveState
	{
		public bool Active { get; private set; }

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
			this.Active = false;
		}

		protected virtual void Start()
		{
		}

		protected virtual void Update()
		{
			Pose jointPose;
			if (this.Hand.GetJointPose(this._jointToTest, out jointPose))
			{
				this.Active = this.JointPassesTests(jointPose);
				return;
			}
			this.Active = false;
		}

		private bool JointPassesTests(Pose jointPose)
		{
			bool flag;
			if (this._active)
			{
				flag = this.IsPointWithinColliders(jointPose.position, this._exitColliders);
			}
			else
			{
				flag = this.IsPointWithinColliders(jointPose.position, this._entryColliders);
			}
			this._active = flag;
			return flag;
		}

		private bool IsPointWithinColliders(Vector3 point, Collider[] colliders)
		{
			foreach (Collider collider in colliders)
			{
				if (!Collisions.IsPointWithinCollider(point, collider))
				{
					return false;
				}
			}
			return true;
		}

		public void InjectAllColliderContainsHandJointActiveState(IHand hand, Collider[] entryColliders, Collider[] exitColliders, HandJointId jointToTest)
		{
			this.InjectHand(hand);
			this.InjectEntryColliders(entryColliders);
			this.InjectExitColliders(exitColliders);
			this.InjectJointToTest(jointToTest);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectEntryColliders(Collider[] entryColliders)
		{
			this._entryColliders = entryColliders;
		}

		public void InjectExitColliders(Collider[] exitColliders)
		{
			this._exitColliders = exitColliders;
		}

		public void InjectJointToTest(HandJointId jointToTest)
		{
			this._jointToTest = jointToTest;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		private IHand Hand;

		[SerializeField]
		private Collider[] _entryColliders;

		[SerializeField]
		private Collider[] _exitColliders;

		[SerializeField]
		private HandJointId _jointToTest = HandJointId.HandWristRoot;

		private bool _active;
	}
}
