using System;
using System.Buffers;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class TouchHandGrabInteractorVisual : MonoBehaviour
	{
		public void InjectSyntheticHand(SyntheticHand syntheticHand)
		{
			this._syntheticHand = syntheticHand;
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
				this._interactor.WhenFingerLocked += this.UpdateLocks;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._interactor.WhenFingerLocked -= this.UpdateLocks;
			}
		}

		private void UpdateLocks()
		{
			bool flag = false;
			for (int i = 0; i < 5; i++)
			{
				HandFinger finger = (HandFinger)i;
				if (this._interactor.IsFingerLocked(finger))
				{
					Pose[] fingerJoints = this._interactor.GetFingerJoints(finger);
					Quaternion[] array = ArrayPool<Quaternion>.Shared.Rent(fingerJoints.Length);
					for (int j = 0; j < fingerJoints.Length; j++)
					{
						array[j] = fingerJoints[j].rotation;
					}
					this._syntheticHand.OverrideFingerRotations(finger, array, 1f);
					SyntheticHand syntheticHand = this._syntheticHand;
					JointFreedom jointFreedom = JointFreedom.Locked;
					syntheticHand.SetFingerFreedom(finger, jointFreedom, true);
					ArrayPool<Quaternion>.Shared.Return(array, false);
					flag = true;
				}
				else
				{
					SyntheticHand syntheticHand2 = this._syntheticHand;
					JointFreedom jointFreedom = JointFreedom.Free;
					syntheticHand2.SetFingerFreedom(finger, jointFreedom, false);
				}
			}
			if (flag)
			{
				this._syntheticHand.MarkInputDataRequiresUpdate();
			}
		}

		protected virtual void Update()
		{
			this.UpdateLocks();
		}

		[SerializeField]
		private TouchHandGrabInteractor _interactor;

		[SerializeField]
		private SyntheticHand _syntheticHand;

		protected bool _started;
	}
}
