using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ObjectPullProvider : MonoBehaviour, IMovementProvider
	{
		public float Speed
		{
			get
			{
				return this._speed;
			}
			set
			{
				this._speed = value;
			}
		}

		public float DeadZone
		{
			get
			{
				return this._deadZone;
			}
			set
			{
				this._deadZone = value;
			}
		}

		public IMovement CreateMovement()
		{
			return new ObjectPull(this._speed, this._deadZone);
		}

		[SerializeField]
		[Min(0f)]
		private float _speed = 1f;

		[SerializeField]
		[Min(0f)]
		private float _deadZone = 0.02f;
	}
}
