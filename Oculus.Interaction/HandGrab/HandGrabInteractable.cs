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
	public class HandGrabInteractable : PointerInteractable<HandGrabInteractor, HandGrabInteractable>, IHandGrabInteractable, IRelativeToRef, IRigidbodyRef, ICollidersRef
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

		private IMovementProvider MovementProvider { get; set; }

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

		public List<HandGrabPose> HandGrabPoses
		{
			get
			{
				return this._handGrabPoses;
			}
		}

		public Transform RelativeTo
		{
			get
			{
				return this._rigidbody.transform;
			}
		}

		public PoseMeasureParameters ScoreModifier
		{
			get
			{
				return this._scoringModifier;
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

		public Collider[] Colliders { get; private set; }

		protected virtual void Reset()
		{
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
			if (HandGrabInteractable._registry == null)
			{
				HandGrabInteractable._registry = new CollisionInteractionRegistry<HandGrabInteractor, HandGrabInteractable>();
				this.SetRegistry(HandGrabInteractable._registry);
			}
			this.Colliders = this.Rigidbody.GetComponentsInChildren<Collider>();
			if (this.MovementProvider == null)
			{
				IMovementProvider provider = base.gameObject.AddComponent<MoveTowardsTargetProvider>();
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
			if (!this._grabPoseFinder.FindBestPose(userPose, offset, handScale, handedness, this._scoringModifier, ref result))
			{
				Pose pose = PoseUtils.Multiply(userPose, offset);
				result.HasHandPose = false;
				Vector3 position;
				result.Score = GrabPoseHelper.CollidersScore(pose.position, this.Colliders, out position);
				Pose pose2 = new Pose(position, pose.rotation);
				result.RelativePose = this.RelativeTo.Delta(pose2);
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

		public void InjectAllHandGrabInteractable(GrabTypeFlags supportedGrabTypes, Rigidbody rigidbody, GrabbingRule pinchGrabRules, GrabbingRule palmGrabRules)
		{
			this.InjectSupportedGrabTypes(supportedGrabTypes);
			this.InjectRigidbody(rigidbody);
			this.InjectPinchGrabRules(pinchGrabRules);
			this.InjectPalmGrabRules(palmGrabRules);
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

		public void InjectOptionalScoreModifier(PoseMeasureParameters scoreModifier)
		{
			this._scoringModifier = scoreModifier;
		}

		[Obsolete("Use Grabbable instead")]
		public void InjectOptionalPhysicsGrabbable(PhysicsGrabbable physicsGrabbable)
		{
			this._physicsGrabbable = physicsGrabbable;
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

		[Tooltip("The Rigidbody of the object.")]
		[SerializeField]
		private Rigidbody _rigidbody;

		[Tooltip("The PhysicsGrabbable used when you grab the object.")]
		[SerializeField]
		[Optional(OptionalAttribute.Flag.Obsolete)]
		[Obsolete("Use Grabbable and/or RigidbodyKinematicLocker instead")]
		private PhysicsGrabbable _physicsGrabbable;

		[Tooltip("Forces a release on all other grabbing interactors when grabbed by a new interactor.")]
		[SerializeField]
		private bool _resetGrabOnGrabsUpdated = true;

		[Tooltip("A PoseMeasureParameters used to modify the score of a pose.")]
		[SerializeField]
		[Optional]
		private PoseMeasureParameters _scoringModifier = new PoseMeasureParameters(0.8f);

		[SerializeField]
		[Optional]
		[Range(0f, 1f)]
		[Tooltip("Defines the slippiness threshold so the interactor can slide along the interactable based on thestrength of the grip. GrabSurfaces are required to slide. At min slippiness = 0, the interactor never moves.")]
		private float _slippiness;

		[Tooltip("The grab types that the object supports.")]
		[Space]
		[SerializeField]
		private GrabTypeFlags _supportedGrabTypes = GrabTypeFlags.All;

		[Tooltip("Uses the state of the fingers to define when a pinch grab starts and ends.")]
		[SerializeField]
		private GrabbingRule _pinchGrabRules = GrabbingRule.DefaultPinchRule;

		[Tooltip("Uses the state of the fingers to define when a palm grab starts and ends.")]
		[SerializeField]
		private GrabbingRule _palmGrabRules = GrabbingRule.DefaultPalmRule;

		[Header("Movement", order = -1)]
		[Tooltip("Determines how the object will move when selected.")]
		[SerializeField]
		[Interface(typeof(IMovementProvider), new Type[]
		{

		})]
		[Optional(OptionalAttribute.Flag.AutoGenerated)]
		private Object _movementProvider;

		[Tooltip("Determines when the hand will be aligned with the object.")]
		[SerializeField]
		private HandAlignType _handAligment = HandAlignType.AlignOnGrab;

		[Tooltip(" ")]
		[SerializeField]
		[Optional(OptionalAttribute.Flag.DontHide)]
		[FormerlySerializedAs("_handGrabPoints")]
		private List<HandGrabPose> _handGrabPoses = new List<HandGrabPose>();

		private GrabPoseFinder _grabPoseFinder;

		private static CollisionInteractionRegistry<HandGrabInteractor, HandGrabInteractable> _registry;
	}
}
