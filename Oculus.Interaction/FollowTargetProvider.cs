using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class FollowTargetProvider : MonoBehaviour, IMovementProvider
	{
		private void Awake()
		{
			this._space = base.transform;
		}

		public IMovement CreateMovement()
		{
			return new FollowTarget(this._speed, this._space);
		}

		[SerializeField]
		private float _speed = 5f;

		private Transform _space;
	}
}
