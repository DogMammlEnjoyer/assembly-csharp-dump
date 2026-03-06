using System;
using UnityEngine;

namespace Oculus.Interaction.DistanceReticles
{
	public class ReticleDataGhost : MonoBehaviour, IReticleData
	{
		public Vector3 ProcessHitPoint(Vector3 hitPoint)
		{
			if (!(this._targetPoint != null))
			{
				return base.transform.position;
			}
			return this._targetPoint.position;
		}

		[Tooltip("The GameObject that the ghost hand can interact with.")]
		[SerializeField]
		[Optional]
		private Transform _targetPoint;
	}
}
