using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class TeleportCandidateComputer : MonoBehaviour
	{
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

		public Transform BlockCheckOrigin
		{
			get
			{
				return this._blockCheckOrigin;
			}
			set
			{
				this._blockCheckOrigin = value;
			}
		}

		protected virtual void Awake()
		{
			if (this._teleportInteractor != null)
			{
				this._teleportInteractor.InjectOptionalCandidateComputer(new TeleportInteractor.ComputeCandidateDelegate(this.ComputeCandidate));
			}
		}

		public virtual TeleportInteractable ComputeCandidate(IPolyline TeleportArc, in InteractableRegistry<TeleportInteractor, TeleportInteractable>.InteractableSet interactables, TeleportInteractor.ComputeCandidateTiebreakerDelegate tiebreaker, out TeleportHit hitPose)
		{
			TeleportCandidateComputer.<>c__DisplayClass10_0 CS$<>8__locals1;
			CS$<>8__locals1.TeleportArc = TeleportArc;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.tiebreaker = tiebreaker;
			CS$<>8__locals1.bestCandidate = null;
			CS$<>8__locals1.bestScore = float.PositiveInfinity;
			CS$<>8__locals1.arcOrigin = CS$<>8__locals1.TeleportArc.PointAtIndex(0);
			Vector3 position = CS$<>8__locals1.TeleportArc.PointAtIndex(CS$<>8__locals1.TeleportArc.PointsCount - 1);
			CS$<>8__locals1.bestHit = new TeleportHit(null, position, Vector3.up);
			InteractableRegistry<TeleportInteractor, TeleportInteractable>.InteractableSet interactableSet;
			if (this._blockCheckOrigin != null)
			{
				bool flag = false;
				interactableSet = interactables;
				foreach (TeleportInteractable teleportInteractable in interactableSet)
				{
					if (!teleportInteractable.AllowTeleport)
					{
						flag |= this.<ComputeCandidate>g__CheckOriginBlockers|10_0(this._blockCheckOrigin.position, CS$<>8__locals1.arcOrigin, teleportInteractable, ref CS$<>8__locals1);
					}
				}
				if (flag)
				{
					hitPose = CS$<>8__locals1.bestHit;
					return CS$<>8__locals1.bestCandidate;
				}
			}
			interactableSet = interactables;
			foreach (TeleportInteractable candidate in interactableSet)
			{
				this.<ComputeCandidate>g__CheckCandidate|10_1(candidate, ref CS$<>8__locals1);
			}
			hitPose = CS$<>8__locals1.bestHit;
			return CS$<>8__locals1.bestCandidate;
		}

		[CompilerGenerated]
		private bool <ComputeCandidate>g__CheckOriginBlockers|10_0(Vector3 from, Vector3 to, TeleportInteractable candidate, ref TeleportCandidateComputer.<>c__DisplayClass10_0 A_4)
		{
			TeleportHit hit;
			if (candidate.DetectHit(from, to, out hit))
			{
				float score = -Vector3.Distance(to, hit.Point);
				return this.<ComputeCandidate>g__TrySetScore|10_2(candidate, hit, score, ref A_4);
			}
			return false;
		}

		[CompilerGenerated]
		private void <ComputeCandidate>g__CheckCandidate|10_1(TeleportInteractable candidate, ref TeleportCandidateComputer.<>c__DisplayClass10_0 A_2)
		{
			Vector3 vector = A_2.arcOrigin;
			float num = 0f;
			int num2 = 1;
			while (num2 < A_2.TeleportArc.PointsCount && num <= A_2.bestScore)
			{
				Vector3 vector2 = A_2.TeleportArc.PointAtIndex(num2);
				TeleportHit hit;
				if (candidate.DetectHit(vector, vector2, out hit))
				{
					float score = num + Vector3.Distance(vector, hit.Point);
					if (this.<ComputeCandidate>g__TrySetScore|10_2(candidate, hit, score, ref A_2))
					{
						break;
					}
				}
				num += Vector3.Distance(vector, vector2);
				vector = vector2;
				num2++;
			}
		}

		[CompilerGenerated]
		private bool <ComputeCandidate>g__TrySetScore|10_2(TeleportInteractable candidate, TeleportHit hit, float score, ref TeleportCandidateComputer.<>c__DisplayClass10_0 A_4)
		{
			bool flag = Mathf.Abs(A_4.bestScore - score) <= this._equalDistanceThreshold;
			if (A_4.bestCandidate == null || (!flag && score < A_4.bestScore) || (flag && this.<ComputeCandidate>g__Tiebreak|10_3(candidate, A_4.bestCandidate, ref A_4) > 0))
			{
				A_4.bestScore = score;
				A_4.bestHit = hit;
				A_4.bestCandidate = candidate;
				return true;
			}
			return false;
		}

		[CompilerGenerated]
		private int <ComputeCandidate>g__Tiebreak|10_3(TeleportInteractable a, TeleportInteractable b, ref TeleportCandidateComputer.<>c__DisplayClass10_0 A_3)
		{
			if (A_3.tiebreaker != null)
			{
				return A_3.tiebreaker(a, b);
			}
			return a.TieBreakerScore.CompareTo(b.TieBreakerScore);
		}

		[SerializeField]
		[Tooltip("(Meters, World) The threshold below which distances to a interactable are treated as equal for the purposes of ranking.")]
		private float _equalDistanceThreshold = 0.1f;

		[SerializeField]
		[Tooltip("When provided, the Interactor will perform an extra check to ensurenothing is blocking the line between this point and the teleport origin")]
		private Transform _blockCheckOrigin;

		[SerializeField]
		[Optional]
		[Tooltip("When assigned in Editor, this component will inject itself into the Interactor during Awake.")]
		private TeleportInteractor _teleportInteractor;
	}
}
