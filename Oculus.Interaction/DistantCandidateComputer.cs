using System;
using UnityEngine;

namespace Oculus.Interaction
{
	[Serializable]
	public class DistantCandidateComputer<TInteractor, TInteractable> where TInteractor : Interactor<TInteractor, TInteractable> where TInteractable : Interactable<TInteractor, TInteractable>, ICollidersRef
	{
		public DistantPointDetectorFrustums DetectionFrustums
		{
			get
			{
				return this._detectionFrustums;
			}
			set
			{
				this._detectionFrustums = value;
				this._detector = new DistantPointDetector(value);
			}
		}

		public float DetectionDelay
		{
			get
			{
				return this._detectionDelay;
			}
			set
			{
				this._detectionDelay = value;
			}
		}

		public virtual Pose Origin
		{
			get
			{
				return new Pose(this._detectionFrustums.SelectionFrustum.StartPoint, Quaternion.LookRotation(this._detectionFrustums.SelectionFrustum.Direction));
			}
		}

		public virtual TInteractable ComputeCandidate(InteractableRegistry<TInteractor, TInteractable> registry, TInteractor interactor, out Vector3 bestHitPoint)
		{
			if (this._detector == null)
			{
				this._detector = new DistantPointDetector(this.DetectionFrustums);
			}
			if (this._stableCandidate != null && this._detector.IsPointingWithoutAid(this._stableCandidate.Colliders, out bestHitPoint))
			{
				return this._stableCandidate;
			}
			float num;
			Vector3 vector;
			if (this._stableCandidate != null && !this._detector.ComputeIsPointing(this._stableCandidate.Colliders, false, out num, out vector))
			{
				this._stableCandidate = default(TInteractable);
			}
			InteractableRegistry<TInteractor, TInteractable>.InteractableSet interactableSet = registry.List(interactor);
			TInteractable tinteractable = this.ComputeBestInteractable(interactableSet, this._stableCandidate == null, out bestHitPoint);
			if (tinteractable != this._pointedCandidate)
			{
				this._pointedCandidate = tinteractable;
				if (tinteractable != null)
				{
					this._hoverStartTime = Time.time;
				}
			}
			if ((this._stableCandidate == null && tinteractable != null) || (this._stableCandidate != null && tinteractable != null && this._stableCandidate != tinteractable && Time.time - this._hoverStartTime >= this._detectionDelay))
			{
				this._pointedCandidate = default(TInteractable);
				this._stableCandidate = tinteractable;
			}
			return this._stableCandidate;
		}

		private TInteractable ComputeBestInteractable(in InteractableRegistry<TInteractor, TInteractable>.InteractableSet candidates, bool narrowSearch, out Vector3 bestHitPoint)
		{
			TInteractable result = default(TInteractable);
			float num = float.NegativeInfinity;
			bestHitPoint = Vector3.zero;
			InteractableRegistry<TInteractor, TInteractable>.InteractableSet interactableSet = candidates;
			foreach (TInteractable tinteractable in interactableSet)
			{
				float num2;
				Vector3 vector;
				if (this._detector.ComputeIsPointing(tinteractable.Colliders, narrowSearch, out num2, out vector) && num2 > num)
				{
					num = num2;
					result = tinteractable;
					bestHitPoint = vector;
				}
			}
			return result;
		}

		[Tooltip("Frustum used to detect and select objects.")]
		[SerializeField]
		private DistantPointDetectorFrustums _detectionFrustums;

		[Tooltip("How long you must hover over an object before it's considered a candidate for interaction.")]
		[SerializeField]
		private float _detectionDelay;

		private float _hoverStartTime;

		private DistantPointDetector _detector;

		private TInteractable _stableCandidate;

		private TInteractable _pointedCandidate;
	}
}
