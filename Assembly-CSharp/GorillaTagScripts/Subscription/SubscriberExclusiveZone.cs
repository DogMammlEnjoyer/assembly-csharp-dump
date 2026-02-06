using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription
{
	public class SubscriberExclusiveZone : MonoBehaviour, IGorillaSliceableSimple
	{
		private void Awake()
		{
			if (this.restrictedZone != null)
			{
				this.restrictedZoneCollider = this.restrictedZone.GetComponent<Collider>();
				if (this.restrictedZoneCollider != null && !this.restrictedZoneCollider.isTrigger)
				{
					Debug.LogError("restrictedZone must be a trigger collider!", this);
					base.enabled = false;
					return;
				}
				SubscriberZoneTrigger subscriberZoneTrigger = this.restrictedZone.GetComponent<SubscriberZoneTrigger>();
				if (subscriberZoneTrigger == null)
				{
					subscriberZoneTrigger = this.restrictedZone.AddComponent<SubscriberZoneTrigger>();
				}
				subscriberZoneTrigger.parentZone = this;
				subscriberZoneTrigger.isRestrictedZone = true;
			}
			if (this.warningZone != null)
			{
				this.influenceZoneCollider = this.warningZone.GetComponent<Collider>();
				if (this.influenceZoneCollider != null && !this.influenceZoneCollider.isTrigger)
				{
					Debug.LogError("influenceZone must be a trigger collider!", this);
					base.enabled = false;
					return;
				}
				SubscriberZoneTrigger subscriberZoneTrigger2 = this.warningZone.GetComponent<SubscriberZoneTrigger>();
				if (subscriberZoneTrigger2 == null)
				{
					subscriberZoneTrigger2 = this.warningZone.AddComponent<SubscriberZoneTrigger>();
				}
				subscriberZoneTrigger2.parentZone = this;
				subscriberZoneTrigger2.isRestrictedZone = false;
			}
			if (this.ejectionPoint == null)
			{
				Debug.LogError("Assign an ejectionPoint!", this);
				base.enabled = false;
				return;
			}
			this.UpdateDoor();
		}

		private void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this);
		}

		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this);
			this.ClearAllRigOverrides();
		}

		private void Update()
		{
			this.UpdateDoor();
			if (!SubscriptionManager.IsLocalSubscribed())
			{
				this.HandleZoneBehavior();
				return;
			}
			if (this.bodyColliderWasDisabled)
			{
				this.SetBodyCollider(GTPlayer.Instance, true);
				this.bodyColliderWasDisabled = false;
			}
		}

		public void OnZoneEnter(bool isRestricted)
		{
			if (isRestricted)
			{
				this.insideRestricted = true;
				if (this.showDebugInfo)
				{
					Debug.Log("[Zone] Entered restricted zone");
					return;
				}
			}
			else
			{
				this.insideInfluence = true;
				if (this.showDebugInfo)
				{
					Debug.Log("[Zone] Entered warning zone");
				}
			}
		}

		public void OnZoneExit(bool isRestricted)
		{
			if (isRestricted)
			{
				this.insideRestricted = false;
				if (this.showDebugInfo)
				{
					Debug.Log("[Zone] Exited restricted zone");
					return;
				}
			}
			else
			{
				this.insideInfluence = false;
				if (this.showDebugInfo)
				{
					Debug.Log("[Zone] Exited warning zone");
				}
			}
		}

		private void HandleZoneBehavior()
		{
			GTPlayer instance = GTPlayer.Instance;
			if (instance == null)
			{
				return;
			}
			if (this.insideRestricted)
			{
				if (Time.time - this.lastShoveTime >= this.shoveCooldown)
				{
					this.lastShoveTime = Time.time;
					instance.TeleportTo(this.ejectionPoint.position, instance.transform.rotation, true, false);
					UnityEvent onEnterRestrictedZone = this.OnEnterRestrictedZone;
					if (onEnterRestrictedZone == null)
					{
						return;
					}
					onEnterRestrictedZone.Invoke();
				}
				return;
			}
			if (this.insideInfluence)
			{
				this.DisplaceToward(instance, this.ejectionPoint, this.driftSpeed);
				UnityEvent onWarning = this.OnWarning;
				if (onWarning == null)
				{
					return;
				}
				onWarning.Invoke();
			}
		}

		private Vector3 FindSafeEjectionPosition(Vector3 playerPos)
		{
			if (this.restrictedZoneCollider == null)
			{
				return this.ejectionPoint.position;
			}
			Bounds bounds = this.restrictedZoneCollider.bounds;
			Vector3 a = bounds.ClosestPoint(playerPos);
			Vector3 normalized = (a - bounds.center).normalized;
			Vector3 vector = a + normalized * (this.safetyCheckRadius + 0.5f);
			float maxDistance = Vector3.Distance(playerPos, vector);
			RaycastHit raycastHit;
			if (Physics.SphereCast(playerPos + Vector3.up * this.safetyCheckRadius, this.safetyCheckRadius, normalized, out raycastHit, maxDistance, this.obstacleLayers))
			{
				vector = playerPos + normalized * Mathf.Max(0.1f, raycastHit.distance - this.safetyCheckRadius - 0.2f);
			}
			return vector;
		}

		private void DisplaceToward(GTPlayer player, Transform target, float speed)
		{
			Vector3 normalized = (target.position - player.transform.position).normalized;
			player.transform.position += normalized * speed * Time.deltaTime;
		}

		private void SetBodyCollider(GTPlayer player, bool enabled)
		{
			if (player != null && player.bodyCollider != null)
			{
				player.bodyCollider.enabled = enabled;
				this.bodyColliderWasDisabled = !enabled;
				if (this.showDebugInfo && player.bodyCollider.enabled != enabled)
				{
					Debug.Log(string.Format("[Zone] Body collider: {0}", enabled));
				}
			}
		}

		private void UpdateDoor()
		{
			bool flag = SubscriptionManager.IsLocalSubscribed();
			if (this.nonSubscribeDoorObject.activeSelf == flag)
			{
				this.nonSubscribeDoorObject.SetActive(!flag);
			}
			if (this.subscriberDoorObject.activeSelf != flag)
			{
				this.subscriberDoorObject.SetActive(SubscriptionManager.IsLocalSubscribed());
			}
		}

		public void SliceUpdate()
		{
			VRRigCache.Instance.GetActiveRigs(this.rigs);
			if (this.restrictedZoneCollider == null)
			{
				return;
			}
			for (int i = 0; i < this.rigs.Count; i++)
			{
				if (!this.rigs[i].isOfflineVRRig)
				{
					Vector3 vector = this.restrictedZoneCollider.transform.InverseTransformPoint(this.rigs[i].syncPos);
					Vector3 vector2 = ((BoxCollider)this.restrictedZoneCollider).size / 2f;
					Vector3 center = ((BoxCollider)this.restrictedZoneCollider).center;
					if (vector.x < vector2.x + center.x && vector.x > -vector2.x + center.x && vector.y < vector2.y + center.y && vector.y > -vector2.y + center.y && vector.z < vector2.z + center.z && vector.z > -vector2.z + center.z)
					{
						this.rigs[i].InOverrideSubscriptionZone = true;
						this.rigs[i].OverrideSubscriptionZoneLocation = this.ejectionPoint.position;
					}
					else
					{
						this.rigs[i].InOverrideSubscriptionZone = false;
						this.rigs[i].OverrideSubscriptionZoneLocation = Vector3.zero;
					}
				}
			}
		}

		public void ClearAllRigOverrides()
		{
			VRRigCache.Instance.GetActiveRigs(this.rigs);
			for (int i = 0; i < this.rigs.Count; i++)
			{
				this.rigs[i].InOverrideSubscriptionZone = false;
				this.rigs[i].OverrideSubscriptionZoneLocation = Vector3.zero;
			}
		}

		private void OnDestroy()
		{
			if (this.tempEjectionObject != null)
			{
				Object.Destroy(this.tempEjectionObject);
			}
		}

		private void OnDrawGizmos()
		{
			if (this.restrictedZoneCollider != null)
			{
				Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
				Gizmos.DrawCube(this.restrictedZoneCollider.bounds.center, this.restrictedZoneCollider.bounds.size);
			}
			if (this.influenceZoneCollider != null)
			{
				Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
				Gizmos.DrawCube(this.influenceZoneCollider.bounds.center, this.influenceZoneCollider.bounds.size);
			}
			if (this.ejectionPoint != null)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(this.ejectionPoint.position, 0.5f);
			}
		}

		[Header("Zones")]
		[Tooltip("Inner restricted zone - hard pushback")]
		[SerializeField]
		private GameObject restrictedZone;

		[Tooltip("Outer influence zone - gentle drift")]
		[SerializeField]
		private GameObject warningZone;

		[Header("Safe Exit Point")]
		[SerializeField]
		private Transform ejectionPoint;

		[Header("Tuning")]
		[SerializeField]
		private float driftSpeed = 3f;

		[SerializeField]
		private float shoveCooldown = 0.5f;

		[Header("Safety")]
		[SerializeField]
		private float safetyCheckRadius = 0.5f;

		[SerializeField]
		private LayerMask obstacleLayers;

		[Header("Door Visuals")]
		[SerializeField]
		private GameObject nonSubscribeDoorObject;

		[SerializeField]
		private GameObject subscriberDoorObject;

		public UnityEvent OnWarning;

		public UnityEvent OnEnterRestrictedZone;

		[Header("Debug")]
		[SerializeField]
		private bool showDebugInfo;

		private bool insideRestricted;

		private bool insideInfluence;

		private float lastShoveTime;

		private GameObject tempEjectionObject;

		private Collider restrictedZoneCollider;

		private Collider influenceZoneCollider;

		private bool bodyColliderWasDisabled;

		private List<VRRig> rigs = new List<VRRig>();
	}
}
