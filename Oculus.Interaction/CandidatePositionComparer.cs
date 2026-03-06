using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class CandidatePositionComparer : CandidateComparer<ICandidatePosition>
	{
		public override int Compare(ICandidatePosition a, ICandidatePosition b)
		{
			float sqrMagnitude = (a.CandidatePosition - this._compareOrigin.position).sqrMagnitude;
			float sqrMagnitude2 = (b.CandidatePosition - this._compareOrigin.position).sqrMagnitude;
			if (sqrMagnitude == sqrMagnitude2)
			{
				return 0;
			}
			if (sqrMagnitude >= sqrMagnitude2)
			{
				return 1;
			}
			return -1;
		}

		[SerializeField]
		private Transform _compareOrigin;
	}
}
