using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	public class StickObjectToPlayer : MonoBehaviour, ITickSystemTick
	{
		public bool TickRunning { get; set; }

		public void Tick()
		{
			if (!this.canSpawn && Time.time - this.lastSpawnedTime >= this.cooldown)
			{
				this.canSpawn = true;
			}
		}

		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
			this.canSpawn = true;
		}

		private void OnDisable()
		{
			TickSystem<object>.RemoveTickCallback(this);
		}

		public void SetOwner(NetPlayer player)
		{
			this.ownerPlayer = player;
		}

		private Transform MakeOrGetStickyContainer(Transform parent)
		{
			Transform transform = parent;
			foreach (Transform transform2 in parent.GetComponentsInChildren<Transform>(true))
			{
				if (!this.firstPersonView && transform2.CompareTag(this.parentTag))
				{
					transform = transform2;
					break;
				}
			}
			string text = "StickyObjects_" + this.objectToSpawn.name;
			Transform transform3 = transform.Find(text);
			if (transform3 != null)
			{
				return transform3;
			}
			GameObject gameObject = new GameObject(text);
			gameObject.transform.SetParent(transform, false);
			return gameObject.transform;
		}

		public void Stick(bool leftHand, Collider other)
		{
			if (!this.canSpawn || other == null || !base.enabled)
			{
				return;
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			if (!componentInParent)
			{
				return;
			}
			if (this.ownerPlayer != null && componentInParent.creator == this.ownerPlayer)
			{
				return;
			}
			Vector3 a = (this.spawnerRigidbody != null) ? this.spawnerRigidbody.linearVelocity : Vector3.zero;
			Vector3 b = Time.fixedDeltaTime * 2f * a;
			Vector3 vector = b.normalized;
			if (vector == Vector3.zero)
			{
				vector = base.transform.forward;
				b = vector * 0.01f;
			}
			Vector3 vector2 = base.transform.position - b;
			Vector3 a2;
			if (this.alignToHitNormal)
			{
				float magnitude = b.magnitude;
				RaycastHit raycastHit;
				if (other.Raycast(new Ray(vector2, vector), out raycastHit, 2f * magnitude))
				{
					a2 = raycastHit.point;
				}
				else
				{
					a2 = other.ClosestPoint(vector2);
				}
			}
			else
			{
				a2 = other.ClosestPoint(vector2);
			}
			Vector3 vector3 = this.GetSpawnPosition(this.spawnLocation, componentInParent).TransformPoint(this.positionOffset);
			if ((a2 - vector3).magnitude <= this.stickRadius * componentInParent.scaleFactor)
			{
				if (NetworkSystem.Instance.LocalPlayer == componentInParent.creator)
				{
					if (this.firstPersonView && this.spawnLocation == StickObjectToPlayer.SpawnLocation.Head)
					{
						this.StickFirstPersonView();
					}
				}
				else
				{
					if (!this.thirdPersonView)
					{
						return;
					}
					Transform parent = this.MakeOrGetStickyContainer(componentInParent.transform);
					this.StickTo(parent, vector3, this.localEulerAngles);
				}
				UnityEvent onStickShared = this.OnStickShared;
				if (onStickShared == null)
				{
					return;
				}
				onStickShared.Invoke();
			}
		}

		private void StickFirstPersonView()
		{
			Transform cosmeticsHeadTarget = GTPlayer.Instance.CosmeticsHeadTarget;
			Vector3 position = cosmeticsHeadTarget.TransformPoint(this.FPVOffset);
			Transform parent = this.MakeOrGetStickyContainer(cosmeticsHeadTarget);
			this.StickTo(parent, position, this.FPVlocalEulerAngles);
		}

		private void StickTo(Transform parent, Vector3 position, Vector3 eulerAngle)
		{
			int num = 0;
			for (int i = 0; i < parent.childCount; i++)
			{
				if (parent.GetChild(i).gameObject.activeInHierarchy)
				{
					num++;
				}
			}
			if (num >= this.maxActiveStickies)
			{
				return;
			}
			this.stickyObject = ObjectPools.instance.Instantiate(this.objectToSpawn, true);
			if (this.stickyObject == null)
			{
				return;
			}
			this.stickyObject.transform.SetParent(parent, false);
			this.stickyObject.transform.position = position;
			this.stickyObject.transform.localEulerAngles = eulerAngle;
			this.lastSpawnedTime = Time.time;
			this.canSpawn = false;
		}

		private Transform GetSpawnPosition(StickObjectToPlayer.SpawnLocation spawnType, VRRig hitRig)
		{
			switch (spawnType)
			{
			case StickObjectToPlayer.SpawnLocation.Head:
				return hitRig.head.rigTarget.transform;
			case StickObjectToPlayer.SpawnLocation.RightHand:
				return hitRig.rightHand.rigTarget.transform;
			case StickObjectToPlayer.SpawnLocation.LeftHand:
				return hitRig.leftHand.rigTarget.transform;
			default:
				return null;
			}
		}

		public void Debug_StickToLocalPlayer()
		{
			Vector3 position = this.GetSpawnPosition(this.spawnLocation, VRRig.LocalRig).TransformPoint(this.positionOffset);
			this.StickTo(VRRig.LocalRig.transform, position, this.localEulerAngles);
		}

		public void Debug_StickToLocalPlayerFPV()
		{
			this.StickFirstPersonView();
		}

		[Header("Shared Settings")]
		[Tooltip("Must be in the global object pool and have a tag.")]
		[SerializeField]
		private GameObject objectToSpawn;

		[Tooltip("Optional: how many objects can be active at once")]
		[SerializeField]
		private int maxActiveStickies = 1;

		[SerializeField]
		private StickObjectToPlayer.SpawnLocation spawnLocation;

		[SerializeField]
		private float stickRadius = 0.5f;

		[SerializeField]
		private bool alignToHitNormal = true;

		[SerializeField]
		private Rigidbody spawnerRigidbody;

		[SerializeField]
		private string parentTag = "GorillaHead";

		[SerializeField]
		private float cooldown;

		[Header("Third Person View")]
		[Tooltip("If you are only interested in the FPV, don't check this box so that others don't see it.")]
		[SerializeField]
		private bool thirdPersonView = true;

		[SerializeField]
		private Vector3 positionOffset = new Vector3(0f, 0.02f, 0.17f);

		[Tooltip("Local rotation to apply to the spawned object (Euler angles, degrees)")]
		[SerializeField]
		private Vector3 localEulerAngles = Vector3.zero;

		[Header("First Person View")]
		[SerializeField]
		private bool firstPersonView;

		[SerializeField]
		private Vector3 FPVOffset = new Vector3(0f, 0.02f, 0.17f);

		[Tooltip("Local rotation to apply to the spawned object (Euler angles, degrees)")]
		[SerializeField]
		private Vector3 FPVlocalEulerAngles = Vector3.zero;

		[Header("Events")]
		public UnityEvent OnStickShared;

		private GameObject stickyObject;

		private float lastSpawnedTime;

		private bool canSpawn = true;

		private NetPlayer ownerPlayer;

		private enum SpawnLocation
		{
			Head,
			RightHand,
			LeftHand
		}
	}
}
