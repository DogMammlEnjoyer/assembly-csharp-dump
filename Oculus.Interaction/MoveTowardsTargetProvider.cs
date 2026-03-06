using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class MoveTowardsTargetProvider : MonoBehaviour, IMovementProvider
	{
		public IMovement CreateMovement()
		{
			return new MoveTowardsTarget(this._travellingData);
		}

		public void InjectAllMoveTowardsTargetProvider(PoseTravelData travellingData)
		{
			this.InjectTravellingData(travellingData);
		}

		public void InjectTravellingData(PoseTravelData travellingData)
		{
			this._travellingData = travellingData;
		}

		[SerializeField]
		private PoseTravelData _travellingData = PoseTravelData.FAST;
	}
}
