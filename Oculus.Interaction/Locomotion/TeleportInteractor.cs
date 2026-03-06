using System;
using Oculus.Interaction.Input;
using Oculus.Interaction.PoseDetection;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class TeleportInteractor : Interactor<TeleportInteractor, TeleportInteractable>, ILocomotionEventBroadcaster
	{
		public IPolyline TeleportArc { get; private set; }

		[Obsolete("This property is obsolete, create a ComputeCandidateDelegate if you need custom candidate computing logic")]
		public float EqualDistanceThreshold
		{
			get
			{
				return this._equalDistanceThreshold;
			}
			set
			{
				this._equalDistanceThreshold = value;
			}
		}

		[Obsolete("This property is obsolete, create a ComputeCandidateDelegate if you need custom candidate computing logic")]
		private IHmd Hmd { get; set; }

		public Pose ArcOrigin
		{
			get
			{
				Vector3 vector = this.TeleportArc.PointAtIndex(0);
				Vector3 a = this.TeleportArc.PointAtIndex(1);
				return new Pose(vector, Quaternion.LookRotation(a - vector));
			}
		}

		public TeleportHit ArcEnd
		{
			get
			{
				return this._arcEnd;
			}
		}

		public Pose TeleportTarget
		{
			get
			{
				Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(this.ArcOrigin.forward, this._arcEnd.Normal), this._arcEnd.Normal);
				Pose pose = new Pose(this._arcEnd.Point, rotation);
				if (base.HasInteractable)
				{
					return base.Interactable.TargetPose(pose);
				}
				return pose;
			}
		}

		public event Action<LocomotionEvent> WhenLocomotionPerformed
		{
			add
			{
				this._whenLocomotionPerformed = (Action<LocomotionEvent>)Delegate.Combine(this._whenLocomotionPerformed, value);
			}
			remove
			{
				this._whenLocomotionPerformed = (Action<LocomotionEvent>)Delegate.Remove(this._whenLocomotionPerformed, value);
			}
		}

		public TeleportInteractor.AcceptDestinationComputer AcceptDestination
		{
			get
			{
				return this._acceptDestination;
			}
			set
			{
				this._acceptDestination = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			if (this.TeleportArc == null)
			{
				this.TeleportArc = (this._teleportArc as IPolyline);
			}
			if (base.Selector == null)
			{
				base.Selector = (this._selector as ISelector);
			}
			this.Hmd = (this._hmd as IHmd);
			this._nativeId = 5507730199525946473UL;
			this._computeCandidateTiebreaker = new TeleportInteractor.ComputeCandidateTiebreakerDelegate(this.ComputeCandidateTiebreaker);
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			if (this.TeleportArc == null)
			{
				TeleportArcGravity teleportArc = base.gameObject.AddComponent<TeleportArcGravity>();
				this.InjectOptionalTeleportArc(teleportArc);
			}
			if (this._computeCandidate == null)
			{
				GameObject gameObject = new GameObject("Default CandidateComputer");
				TeleportCandidateComputer teleportCandidateComputer = gameObject.AddComponent<TeleportCandidateComputer>();
				teleportCandidateComputer.EqualDistanceThreshold = this._equalDistanceThreshold;
				if (this.Hmd != null)
				{
					gameObject.AddComponent<HmdOffset>().InjectAllHmdOffset(this.Hmd);
					teleportCandidateComputer.BlockCheckOrigin = gameObject.transform;
				}
				this._computeCandidate = new TeleportInteractor.ComputeCandidateDelegate(teleportCandidateComputer.ComputeCandidate);
			}
			this.EndStart(ref this._started);
		}

		public override bool CanSelect(TeleportInteractable interactable)
		{
			Pose arcOrigin = this.ArcOrigin;
			float sqrMagnitude = (this.TeleportArc.PointAtIndex(0) - this.TeleportArc.PointAtIndex(this.TeleportArc.PointsCount - 1)).sqrMagnitude;
			return interactable.IsInRange(arcOrigin, sqrMagnitude) && base.CanSelect(interactable);
		}

		public bool HasValidDestination()
		{
			Pose teleportTarget = this.TeleportTarget;
			return base.Interactable != null && base.Interactable.AllowTeleport && (this._acceptDestination == null || this._acceptDestination(base.Interactable, teleportTarget));
		}

		protected override void InteractableSelected(TeleportInteractable interactable)
		{
			base.InteractableSelected(interactable);
			Pose teleportTarget = this.TeleportTarget;
			if (!this.HasValidDestination())
			{
				LocomotionEvent obj = new LocomotionEvent(base.Identifier, teleportTarget, LocomotionEvent.TranslationType.None, LocomotionEvent.RotationType.None);
				this._whenLocomotionPerformed(obj);
				return;
			}
			LocomotionEvent obj2 = new LocomotionEvent(base.Identifier, teleportTarget, interactable.EyeLevel ? LocomotionEvent.TranslationType.AbsoluteEyeLevel : LocomotionEvent.TranslationType.Absolute, interactable.FaceTargetDirection ? LocomotionEvent.RotationType.Absolute : LocomotionEvent.RotationType.None);
			this._whenLocomotionPerformed(obj2);
		}

		protected override TeleportInteractable ComputeCandidate()
		{
			InteractableRegistry<TeleportInteractor, TeleportInteractable>.InteractableSet interactableSet = Interactable<TeleportInteractor, TeleportInteractable>.Registry.List(this);
			return this._computeCandidate(this.TeleportArc, interactableSet, this._computeCandidateTiebreaker, out this._arcEnd);
		}

		protected override int ComputeCandidateTiebreaker(TeleportInteractable a, TeleportInteractable b)
		{
			int num = base.ComputeCandidateTiebreaker(a, b);
			if (num != 0)
			{
				return num;
			}
			return a.TieBreakerScore.CompareTo(b.TieBreakerScore);
		}

		public void InjectAllTeleportInteractor(ISelector selector)
		{
			this.InjectSelector(selector);
		}

		public void InjectSelector(ISelector selector)
		{
			this._selector = (selector as Object);
			base.Selector = selector;
		}

		[Obsolete("This property is no longer in use, create a ComputeCandidateDelegate if you need custom candidate computing logic")]
		public void InjectOptionalHmd(IHmd hmd)
		{
			this._hmd = (hmd as Object);
			this.Hmd = hmd;
		}

		public void InjectOptionalTeleportArc(IPolyline teleportArc)
		{
			this._teleportArc = (teleportArc as Object);
			this.TeleportArc = teleportArc;
		}

		public void InjectOptionalCandidateComputer(TeleportInteractor.ComputeCandidateDelegate candidateComputer)
		{
			this._computeCandidate = candidateComputer;
		}

		[SerializeField]
		[Interface(typeof(ISelector), new Type[]
		{

		})]
		[Tooltip("A selector indicating when the Interactor shouldSelect or Unselect the best available interactable.Typically when using controllers this selector is driven by the joystick value,and for hands it is driven by the index pinch value.")]
		private Object _selector;

		[SerializeField]
		[Interface(typeof(IPolyline), new Type[]
		{

		})]
		[Optional(OptionalAttribute.Flag.AutoGenerated)]
		[Tooltip("Specifies the shape of the arc used for detecting available interactables.If none is provided TeleportArcGravity will be used.")]
		private Object _teleportArc;

		[SerializeField]
		[Optional(OptionalAttribute.Flag.Obsolete)]
		[Tooltip("(Meters, World) The threshold below which distances to a interactable are treated as equal for the purposes of ranking.")]
		private float _equalDistanceThreshold = 0.1f;

		[SerializeField]
		[Optional(OptionalAttribute.Flag.Obsolete)]
		[Interface(typeof(IHmd), new Type[]
		{

		})]
		[Tooltip("When provided, the Interactor will perform an extra check to ensurenothing is blocking the line between the Hmd and the teleport origin")]
		private Object _hmd;

		private TeleportHit _arcEnd;

		private Action<LocomotionEvent> _whenLocomotionPerformed = delegate(LocomotionEvent <p0>)
		{
		};

		private TeleportInteractor.AcceptDestinationComputer _acceptDestination;

		private TeleportInteractor.ComputeCandidateDelegate _computeCandidate;

		private TeleportInteractor.ComputeCandidateTiebreakerDelegate _computeCandidateTiebreaker;

		public delegate bool AcceptDestinationComputer(TeleportInteractable interactable, Pose destination);

		public delegate int ComputeCandidateTiebreakerDelegate(TeleportInteractable a, TeleportInteractable b);

		public delegate TeleportInteractable ComputeCandidateDelegate(IPolyline TeleportArc, in InteractableRegistry<TeleportInteractor, TeleportInteractable>.InteractableSet interactables, TeleportInteractor.ComputeCandidateTiebreakerDelegate tiebreaker, out TeleportHit hitPose);
	}
}
