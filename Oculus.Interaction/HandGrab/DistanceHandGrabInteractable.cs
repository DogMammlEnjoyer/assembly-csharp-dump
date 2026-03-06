using System;
using System.Collections.Generic;
using Oculus.Interaction.Grab;
using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.HandGrab
{
	[Serializable]
	public class DistanceHandGrabInteractable : PointerInteractable<DistanceHandGrabInteractor, DistanceHandGrabInteractable>, IHandGrabInteractable, IRelativeToRef, IRigidbodyRef, ICollidersRef
	{
		public Rigidbody Rigidbody
		{
			get
			{
				return this._rigidbody;
			}
		}

		public bool ResetGrabOnGrabsUpdated
		{
			get
			{
				return this._resetGrabOnGrabsUpdated;
			}
			set
			{
				this._resetGrabOnGrabsUpdated = value;
			}
		}

		public float Slippiness
		{
			get
			{
				return this._slippiness;
			}
			set
			{
				this._slippiness = value;
			}
		}

		public IMovementProvider MovementProvider { get; set; }

		public HandAlignType HandAlignment
		{
			get
			{
				return this._handAligment;
			}
			set
			{
				this._handAligment = value;
			}
		}

		public Transform RelativeTo
		{
			get
			{
				return this._rigidbody.transform;
			}
		}

		public GrabTypeFlags SupportedGrabTypes
		{
			get
			{
				return this._supportedGrabTypes;
			}
		}

		public GrabbingRule PinchGrabRules
		{
			get
			{
				return this._pinchGrabRules;
			}
		}

		public GrabbingRule PalmGrabRules
		{
			get
			{
				return this._palmGrabRules;
			}
		}

		public List<HandGrabPose> HandGrabPoses
		{
			get
			{
				return this._handGrabPoses;
			}
		}

		public Collider[] Colliders { get; private set; }

		protected virtual void Reset()
		{
			HandGrabInteractable handGrabInteractable;
			if (base.TryGetComponent<HandGrabInteractable>(out handGrabInteractable))
			{
				this.InjectAllDistanceHandGrabInteractable(handGrabInteractable.SupportedGrabTypes, handGrabInteractable.Rigidbody, handGrabInteractable.PinchGrabRules, handGrabInteractable.PalmGrabRules);
				this.InjectOptionalHandGrabPoses(new List<HandGrabPose>(handGrabInteractable.HandGrabPoses));
				base.InjectOptionalPointableElement(handGrabInteractable.PointableElement);
				return;
			}
			this.InjectRigidbody(base.GetComponentInParent<Rigidbody>());
			base.InjectOptionalPointableElement(base.GetComponentInParent<Grabbable>());
		}

		protected override void Awake()
		{
			base.Awake();
			this.MovementProvider = (this._movementProvider as IMovementProvider);
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.Colliders = this.Rigidbody.GetComponentsInChildren<Collider>();
			if (this.MovementProvider == null)
			{
				MoveTowardsTargetProvider provider = base.gameObject.AddComponent<MoveTowardsTargetProvider>();
				this.InjectOptionalMovementProvider(provider);
			}
			this._grabPoseFinder = new GrabPoseFinder(this._handGrabPoses, this.RelativeTo);
			this.EndStart(ref this._started);
		}

		public IMovement GenerateMovement(in Pose from, in Pose to)
		{
			IMovement movement = this.MovementProvider.CreateMovement();
			movement.StopAndSetPose(from);
			movement.MoveTo(to);
			return movement;
		}

		[Obsolete("Use Grabbable instead")]
		public void ApplyVelocities(Vector3 linearVelocity, Vector3 angularVelocity)
		{
			if (this._physicsGrabbable == null)
			{
				return;
			}
			this._physicsGrabbable.ApplyVelocities(linearVelocity, angularVelocity);
		}

		public bool CalculateBestPose(Pose userPose, float handScale, Handedness handedness, ref HandGrabResult result)
		{
			Pose identity = Pose.identity;
			this.CalculateBestPose(userPose, identity, this.RelativeTo, handScale, handedness, ref result);
			return true;
		}

		public void CalculateBestPose(in Pose userPose, in Pose offset, Transform relativeTo, float handScale, Handedness handedness, ref HandGrabResult result)
		{
			if (!this._grabPoseFinder.FindBestPose(userPose, offset, handScale, handedness, this.SCORE_MODIFIER, ref result))
			{
				Pose pose = PoseUtils.Multiply(userPose, offset);
				result.HasHandPose = false;
				result.Score = new GrabPoseScore(pose.position, base.transform.position, false);
				result.RelativePose = new Pose(this.RelativeTo.Delta(base.transform).position, Quaternion.Inverse(this.RelativeTo.rotation) * pose.rotation);
			}
		}

		public bool UsesHandPose
		{
			get
			{
				return this._grabPoseFinder.UsesHandPose;
			}
		}

		public bool SupportsHandedness(Handedness handedness)
		{
			return this._grabPoseFinder.SupportsHandedness(handedness);
		}

		public void InjectAllDistanceHandGrabInteractable(GrabTypeFlags supportedGrabTypes, Rigidbody rigidbody, GrabbingRule pinchGrabRules, GrabbingRule palmGrabRules)
		{
			this.InjectSupportedGrabTypes(supportedGrabTypes);
			this.InjectRigidbody(rigidbody);
			this.InjectPinchGrabRules(pinchGrabRules);
			this.InjectPalmGrabRules(palmGrabRules);
		}

		[Obsolete("Use Grabbable instead")]
		public void InjectOptionalPhysicsGrabbable(PhysicsGrabbable physicsObject)
		{
			this._physicsGrabbable = physicsObject;
		}

		public void InjectSupportedGrabTypes(GrabTypeFlags supportedGrabTypes)
		{
			this._supportedGrabTypes = supportedGrabTypes;
		}

		public void InjectPinchGrabRules(GrabbingRule pinchGrabRules)
		{
			this._pinchGrabRules = pinchGrabRules;
		}

		public void InjectPalmGrabRules(GrabbingRule palmGrabRules)
		{
			this._palmGrabRules = palmGrabRules;
		}

		public void InjectRigidbody(Rigidbody rigidbody)
		{
			this._rigidbody = rigidbody;
		}

		public void InjectOptionalHandGrabPoses(List<HandGrabPose> handGrabPoses)
		{
			this._handGrabPoses = handGrabPoses;
		}

		public void InjectOptionalMovementProvider(IMovementProvider provider)
		{
			this._movementProvider = (provider as Object);
			this.MovementProvider = provider;
		}

		IMovement IHandGrabInteractable.GenerateMovement(in Pose from, in Pose to)
		{
			return this.GenerateMovement(from, to);
		}

		void IHandGrabInteractable.CalculateBestPose(in Pose userPose, in Pose offset, Transform relativeTo, float handScale, Handedness handedness, ref HandGrabResult result)
		{
			this.CalculateBestPose(userPose, offset, relativeTo, handScale, handedness, ref result);
		}

		[SerializeField]
		private Rigidbody _rigidbody;

		[SerializeField]
		[Optional(OptionalAttribute.Flag.Obsolete)]
		[Obsolete("Use Grabbable and/or RigidbodyKinematicLocker instead")]
		private PhysicsGrabbable _physicsGrabbable;

		[SerializeField]
		private bool _resetGrabOnGrabsUpdated = true;

		[Space]
		[SerializeField]
		[Optional]
		[Range(0f, 1f)]
		[Tooltip("Defines the slippiness threshold so the interactor can slide along the interactable based on thestrength of the grip. GrabSurfaces are required to slide. At min slippiness = 0, the interactor never moves.")]
		private float _slippiness;

		[Space]
		[SerializeField]
		private GrabTypeFlags _supportedGrabTypes = GrabTypeFlags.Pinch;

		[SerializeField]
		private GrabbingRule _pinchGrabRules = GrabbingRule.DefaultPinchRule;

		[SerializeField]
		private GrabbingRule _palmGrabRules = GrabbingRule.DefaultPalmRule;

		[SerializeField]
		[Interface(typeof(IMovementProvider), new Type[]
		{

		})]
		[Optional(OptionalAttribute.Flag.AutoGenerated)]
		private Object _movementProvider;

		[SerializeField]
		private HandAlignType _handAligment = HandAlignType.AlignOnGrab;

		[SerializeField]
		[Optional(OptionalAttribute.Flag.DontHide)]
		[FormerlySerializedAs("_handGrabPoints")]
		private List<HandGrabPose> _handGrabPoses = new List<HandGrabPose>();

		private GrabPoseFinder _grabPoseFinder;

		private readonly PoseMeasureParameters SCORE_MODIFIER = new PoseMeasureParameters(1f);
	}
}
