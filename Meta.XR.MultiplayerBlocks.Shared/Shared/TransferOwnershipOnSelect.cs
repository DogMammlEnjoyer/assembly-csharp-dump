using System;
using Meta.XR.BuildingBlocks;
using Oculus.Interaction;
using UnityEngine;

namespace Meta.XR.MultiplayerBlocks.Shared
{
	public class TransferOwnershipOnSelect : MonoBehaviour
	{
		private void Awake()
		{
			this._grabbable = base.GetComponentInChildren<Grabbable>();
			if (this._grabbable == null)
			{
				throw new InvalidOperationException("Object requires a Grabbable component");
			}
			this._grabbable.WhenPointerEventRaised += this.OnPointerEventRaised;
			this._transferOwnership = this.GetInterfaceComponent<ITransferOwnership>();
			if (this._transferOwnership == null)
			{
				throw new InvalidOperationException("Object requires an ITransferOwnership component");
			}
			if (!this.UseGravity)
			{
				return;
			}
			this._rigidbody = base.GetComponent<Rigidbody>();
			if (this._rigidbody == null)
			{
				throw new InvalidOperationException("Object requires a Rigidbody component when useGravity enabled");
			}
		}

		private void OnDestroy()
		{
			if (this._grabbable != null)
			{
				this._grabbable.WhenPointerEventRaised -= this.OnPointerEventRaised;
			}
		}

		private void OnPointerEventRaised(PointerEvent pointerEvent)
		{
			if (this._grabbable == null || pointerEvent.Type != PointerEventType.Select)
			{
				return;
			}
			if (this._grabbable.SelectingPointsCount == 1 && !this._transferOwnership.HasOwnership())
			{
				this._transferOwnership.TransferOwnershipToLocalPlayer();
			}
		}

		private void LateUpdate()
		{
			if (this._transferOwnership.HasOwnership() && this.UseGravity)
			{
				this._rigidbody.isKinematic = this._rigidbody.IsLocked();
			}
		}

		public bool UseGravity;

		private Grabbable _grabbable;

		private Rigidbody _rigidbody;

		private ITransferOwnership _transferOwnership;
	}
}
