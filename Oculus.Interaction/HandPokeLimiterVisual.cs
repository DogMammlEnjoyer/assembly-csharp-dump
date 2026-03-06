using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandPokeLimiterVisual : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
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
				this._pokeInteractor.WhenStateChanged += this.HandleStateChanged;
				PokeInteractor pokeInteractor = this._pokeInteractor;
				pokeInteractor.WhenPassedSurfaceChanged = (Action<bool>)Delegate.Combine(pokeInteractor.WhenPassedSurfaceChanged, new Action<bool>(this.HandlePassedSurfaceChanged));
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				if (this._isTouching)
				{
					this.UnlockWrist();
				}
				this._pokeInteractor.WhenStateChanged -= this.HandleStateChanged;
				PokeInteractor pokeInteractor = this._pokeInteractor;
				pokeInteractor.WhenPassedSurfaceChanged = (Action<bool>)Delegate.Remove(pokeInteractor.WhenPassedSurfaceChanged, new Action<bool>(this.HandlePassedSurfaceChanged));
			}
		}

		private void HandlePassedSurfaceChanged(bool passed)
		{
			this.CheckPassedSurface();
		}

		private void HandleStateChanged(InteractorStateChangeArgs args)
		{
			this.CheckPassedSurface();
		}

		private void CheckPassedSurface()
		{
			if (this._pokeInteractor.IsPassedSurface)
			{
				this.LockWrist();
				return;
			}
			this.UnlockWrist();
		}

		protected virtual void LateUpdate()
		{
			this.UpdateWrist();
		}

		private void LockWrist()
		{
			bool isTouching = this._isTouching;
			this._isTouching = true;
			if (!isTouching && this._isTouching)
			{
				NativeMethods.isdk_NativeComponent_Activate(5795969328266964340UL);
			}
		}

		private void UnlockWrist()
		{
			this._syntheticHand.FreeWrist(SyntheticHand.WristLockMode.Full);
			this._isTouching = false;
		}

		private void UpdateWrist()
		{
			if (!this._isTouching)
			{
				return;
			}
			Pose pose;
			if (!this.Hand.GetRootPose(out pose))
			{
				return;
			}
			Vector3 b = pose.position - this._pokeInteractor.Origin;
			Vector3 position = this._pokeInteractor.TouchPoint + b + this._pokeInteractor.Radius * this._pokeInteractor.TouchNormal;
			Pose wristPose = new Pose(position, pose.rotation);
			this._syntheticHand.LockWristPose(wristPose, 1f, SyntheticHand.WristLockMode.Full, true, true);
			this._syntheticHand.MarkInputDataRequiresUpdate();
		}

		public void InjectAllHandPokeLimiterVisual(IHand hand, PokeInteractor pokeInteractor, SyntheticHand syntheticHand)
		{
			this.InjectHand(hand);
			this.InjectPokeInteractor(pokeInteractor);
			this.InjectSyntheticHand(syntheticHand);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectPokeInteractor(PokeInteractor pokeInteractor)
		{
			this._pokeInteractor = pokeInteractor;
		}

		public void InjectSyntheticHand(SyntheticHand syntheticHand)
		{
			this._syntheticHand = syntheticHand;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		private IHand Hand;

		[SerializeField]
		private PokeInteractor _pokeInteractor;

		[SerializeField]
		private SyntheticHand _syntheticHand;

		private bool _isTouching;

		protected bool _started;
	}
}
