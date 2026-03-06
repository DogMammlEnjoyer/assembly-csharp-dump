using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class HandPhysicsCapsules : MonoBehaviour
	{
		public event Action WhenCapsulesGenerated
		{
			add
			{
				this._whenCapsulesGenerated = (Action)Delegate.Combine(this._whenCapsulesGenerated, value);
				if (this._capsulesGenerated)
				{
					value();
				}
			}
			remove
			{
				this._whenCapsulesGenerated = (Action)Delegate.Remove(this._whenCapsulesGenerated, value);
			}
		}

		public Transform RootTransform
		{
			get
			{
				return this._rootTransform;
			}
		}

		public IList<BoneCapsule> Capsules { get; private set; }

		protected virtual void Reset()
		{
			this._useLayer = base.gameObject.layer;
		}

		protected virtual void Awake()
		{
			this.HandVisual = (this._handVisual as IHandVisual);
			this.Hand = (this._hand as IHand);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			if (this.Hand == null && this.HandVisual != null)
			{
				this.Hand = this.HandVisual.Hand;
			}
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._capsulesAreActive = true;
				this.Hand.WhenHandUpdated += this.HandleHandUpdated;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated -= this.HandleHandUpdated;
				this.DisableRigidbodies();
			}
		}

		private void GenerateCapsules()
		{
			if (!this.Hand.IsTrackedDataValid)
			{
				return;
			}
			this._rigidbodies = new Rigidbody[26];
			Transform transform = new GameObject("Capsules").transform;
			transform.SetParent(base.transform, false);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.gameObject.layer = this._useLayer;
			int capacity = 26;
			this._capsules = new List<BoneCapsule>(capacity);
			this.Capsules = this._capsules.AsReadOnly();
			for (int i = 2; i < 26; i++)
			{
				HandJointId handJointId = (HandJointId)i;
				HandJointId handJointId2 = HandJointUtils.JointParentList[i];
				if (handJointId2 != HandJointId.Invalid && (1 << (int)handJointId & (int)this._mask) != 0)
				{
					Pose pose;
					this.Hand.GetJointPose(handJointId2, out pose);
					Rigidbody rigidbody;
					if (!this.TryGetJointRigidbody(handJointId2, out rigidbody))
					{
						rigidbody = this.CreateJointRigidbody(handJointId2, transform, pose);
						this._rigidbodies[(int)handJointId2] = rigidbody;
					}
					string name = string.Format("{0}-{1} CapsuleCollider", handJointId2, handJointId);
					float jointRadius = this._jointsRadiusFeature.GetJointRadius(handJointId2);
					float offset = HandJointUtils.IsFingerTip(handJointId) ? (-jointRadius) : ((handJointId2 == HandJointId.HandStart) ? jointRadius : 0f);
					Pose pose2;
					this.Hand.GetJointPose(handJointId, out pose2);
					CapsuleCollider collider = this.CreateCollider(name, rigidbody.transform, pose.position, pose2.position, jointRadius, offset);
					BoneCapsule item = new BoneCapsule(handJointId2, handJointId, rigidbody, collider);
					this._capsules.Add(item);
				}
			}
			this.IgnoreSelfCollisions();
			this._capsulesGenerated = true;
			this._whenCapsulesGenerated();
		}

		private void IgnoreSelfCollisions()
		{
			for (int i = 0; i < this._capsules.Count; i++)
			{
				for (int j = i + 1; j < this._capsules.Count; j++)
				{
					Physics.IgnoreCollision(this._capsules[i].CapsuleCollider, this._capsules[j].CapsuleCollider);
				}
			}
		}

		private bool TryGetJointRigidbody(HandJointId joint, out Rigidbody body)
		{
			if (this._rigidbodies == null || joint < HandJointId.HandStart || joint >= (HandJointId)this._rigidbodies.Length)
			{
				body = null;
				return false;
			}
			body = this._rigidbodies[(int)joint];
			return body != null;
		}

		private Rigidbody CreateJointRigidbody(HandJointId joint, Transform holder, Pose pose)
		{
			Rigidbody rigidbody = new GameObject(string.Format("{0} Rigidbody", joint)).AddComponent<Rigidbody>();
			rigidbody.mass = 1f;
			rigidbody.isKinematic = true;
			rigidbody.useGravity = false;
			rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			rigidbody.transform.SetParent(holder, false);
			rigidbody.transform.SetPose(pose, Space.World);
			rigidbody.Sleep();
			rigidbody.gameObject.SetActive(false);
			rigidbody.gameObject.layer = this._useLayer;
			return rigidbody;
		}

		private CapsuleCollider CreateCollider(string name, Transform holder, Vector3 from, Vector3 to, float radius, float offset)
		{
			CapsuleCollider capsuleCollider = new GameObject(name).AddComponent<CapsuleCollider>();
			capsuleCollider.isTrigger = this._asTriggers;
			Vector3 forward = to - from;
			Quaternion rotation = Quaternion.LookRotation(forward);
			float num = forward.magnitude - Mathf.Abs(offset);
			capsuleCollider.radius = radius;
			capsuleCollider.height = num + radius * 2f;
			capsuleCollider.direction = 2;
			capsuleCollider.center = Vector3.forward * (num * 0.5f + Mathf.Max(0f, offset));
			Transform transform = capsuleCollider.transform;
			transform.SetParent(holder, false);
			transform.SetPositionAndRotation(from, rotation);
			capsuleCollider.gameObject.layer = this._useLayer;
			return capsuleCollider;
		}

		private void DisableRigidbodies()
		{
			if (!this._capsulesAreActive)
			{
				return;
			}
			for (HandJointId handJointId = HandJointId.HandStart; handJointId < HandJointId.HandEnd; handJointId++)
			{
				Rigidbody rigidbody;
				if (this.TryGetJointRigidbody(handJointId, out rigidbody))
				{
					rigidbody.Sleep();
					rigidbody.gameObject.SetActive(false);
				}
			}
			this._capsulesAreActive = false;
		}

		private void HandleHandUpdated()
		{
			if (!this._capsulesGenerated)
			{
				this.GenerateCapsules();
			}
			if (this._capsulesGenerated)
			{
				this.UpdateRigidbodies();
				this.UpdateColliders();
			}
		}

		private void UpdateColliders()
		{
			foreach (BoneCapsule boneCapsule in this._capsules)
			{
				boneCapsule.CapsuleCollider.radius = this._jointsRadiusFeature.GetJointRadius(boneCapsule.StartJoint);
			}
		}

		private void UpdateRigidbodies()
		{
			for (HandJointId handJointId = HandJointId.HandStart; handJointId < HandJointId.HandEnd; handJointId++)
			{
				Rigidbody rigidbody;
				if (this.TryGetJointRigidbody(handJointId, out rigidbody))
				{
					GameObject gameObject = rigidbody.gameObject;
					Pose pose;
					if (this._capsulesAreActive && this.Hand.GetJointPose(handJointId, out pose))
					{
						if (!gameObject.activeSelf)
						{
							rigidbody.position = (gameObject.transform.position = pose.position);
							rigidbody.rotation = (gameObject.transform.rotation = pose.rotation);
							gameObject.SetActive(true);
							rigidbody.WakeUp();
						}
						else
						{
							rigidbody.MovePosition(pose.position);
							rigidbody.MoveRotation(pose.rotation);
						}
					}
					else if (gameObject.activeSelf)
					{
						rigidbody.Sleep();
						gameObject.SetActive(false);
					}
				}
			}
		}

		public void InjectAllOVRHandPhysicsCapsules(IHand hand, bool asTriggers, int useLayer)
		{
			this.InjectHand(hand);
			this.InjectAsTriggers(asTriggers);
			this.InjectUseLayer(useLayer);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectAsTriggers(bool asTriggers)
		{
			this._asTriggers = asTriggers;
		}

		public void InjectUseLayer(int useLayer)
		{
			this._useLayer = useLayer;
		}

		public void InjectMask(HandFingerJointFlags mask)
		{
			this._mask = mask;
		}

		public void InjectJointsRadiusFeature(JointsRadiusFeature jointsRadiusFeature)
		{
			this._jointsRadiusFeature = jointsRadiusFeature;
		}

		[SerializeField]
		[Interface(typeof(IHandVisual), new Type[]
		{

		})]
		[Obsolete("Replaced by _hand")]
		private Object _handVisual;

		[Obsolete("Replaced by Hand")]
		private IHandVisual HandVisual;

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		private IHand Hand;

		[Tooltip("Indicates how \"thick\" the fingers are at each bone. This information creates a capsule collider that wraps the bones accurately.")]
		[SerializeField]
		private JointsRadiusFeature _jointsRadiusFeature;

		[Space]
		[SerializeField]
		[Tooltip("If  checked, capsules will be generated as triggers.")]
		private bool _asTriggers;

		[SerializeField]
		[Tooltip("Capsules will be generated in this layer. The default layer is 0.")]
		private int _useLayer;

		[SerializeField]
		[Tooltip("A joint. Capsules reaching this joint will not be generated.")]
		private HandFingerJointFlags _mask = HandFingerJointFlags.All;

		private Action _whenCapsulesGenerated = delegate()
		{
		};

		private Transform _rootTransform;

		private List<BoneCapsule> _capsules;

		private Rigidbody[] _rigidbodies;

		private bool _capsulesAreActive;

		private bool _capsulesGenerated;

		protected bool _started;
	}
}
