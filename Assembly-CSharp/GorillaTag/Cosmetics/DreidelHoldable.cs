using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	public class DreidelHoldable : TransferrableObject
	{
		internal override void OnEnable()
		{
			base.OnEnable();
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? (base.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
				else
				{
					Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
				}
			}
			if (this._events != null)
			{
				this._events.Activate.reliable = true;
				this._events.Activate += this.OnDreidelSpin;
			}
		}

		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= this.OnDreidelSpin;
				this._events.Dispose();
				this._events = null;
			}
		}

		private void OnDreidelSpin(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			GorillaNot.IncrementRPCCall(info, "OnDreidelSpin");
			if (sender != target)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			Vector3 surfacePoint = (Vector3)args[0];
			Vector3 surfaceNormal = (Vector3)args[1];
			float num = (float)args[2];
			double num2 = (double)args[6];
			float num3 = 10000f;
			if (surfacePoint.IsValid(num3))
			{
				float num4 = 10000f;
				if (surfaceNormal.IsValid(num4) && float.IsFinite(num) && double.IsFinite(num2))
				{
					bool counterClockwise = (bool)args[3];
					Dreidel.Side side = (Dreidel.Side)args[4];
					Dreidel.Variation variation = (Dreidel.Variation)args[5];
					this.StartSpinLocal(surfacePoint, surfaceNormal, num, counterClockwise, side, variation, num2);
					return;
				}
			}
		}

		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			base.OnGrab(pointGrabbed, grabbingHand);
			if (this.dreidelAnimation != null)
			{
				this.dreidelAnimation.TryCheckForSurfaces();
			}
		}

		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			if (this.dreidelAnimation != null)
			{
				this.dreidelAnimation.TrySetIdle();
			}
			return true;
		}

		public override void OnActivate()
		{
			base.OnActivate();
			Vector3 vector;
			Vector3 vector2;
			float num;
			Dreidel.Side side;
			Dreidel.Variation variation;
			double num2;
			if (this.dreidelAnimation != null && this.dreidelAnimation.TryGetSpinStartData(out vector, out vector2, out num, out side, out variation, out num2))
			{
				bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					object[] args = new object[]
					{
						vector,
						vector2,
						num,
						flag,
						(int)side,
						(int)variation,
						num2
					};
					this._events.Activate.RaiseAll(args);
					return;
				}
				this.StartSpinLocal(vector, vector2, num, flag, side, variation, num2);
			}
		}

		private void StartSpinLocal(Vector3 surfacePoint, Vector3 surfaceNormal, float duration, bool counterClockwise, Dreidel.Side side, Dreidel.Variation variation, double startTime)
		{
			if (this.dreidelAnimation != null)
			{
				this.dreidelAnimation.SetSpinStartData(surfacePoint, surfaceNormal, duration, counterClockwise, side, variation, startTime);
				this.dreidelAnimation.Spin();
			}
		}

		public void DebugSpinDreidel()
		{
			Transform transform = GTPlayer.Instance.headCollider.transform;
			Vector3 origin = transform.position + transform.forward * 0.5f;
			float maxDistance = 2f;
			RaycastHit raycastHit;
			if (Physics.Raycast(origin, Vector3.down, out raycastHit, maxDistance, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				Vector3 point = raycastHit.point;
				Vector3 normal = raycastHit.normal;
				float num = Random.Range(7f, 10f);
				Dreidel.Side side = (Dreidel.Side)Random.Range(0, 4);
				Dreidel.Variation variation = (Dreidel.Variation)Random.Range(0, 5);
				bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
				double num2 = PhotonNetwork.InRoom ? PhotonNetwork.Time : -1.0;
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					object[] args = new object[]
					{
						point,
						normal,
						num,
						flag,
						(int)side,
						(int)variation,
						num2
					};
					this._events.Activate.RaiseAll(args);
					return;
				}
				this.StartSpinLocal(point, normal, num, flag, side, variation, num2);
			}
		}

		[SerializeField]
		private Dreidel dreidelAnimation;

		private RubberDuckEvents _events;
	}
}
