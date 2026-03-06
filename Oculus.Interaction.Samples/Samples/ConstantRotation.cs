using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class ConstantRotation : MonoBehaviour
	{
		public float RotationSpeed
		{
			get
			{
				return this._rotationSpeed;
			}
			set
			{
				this._rotationSpeed = value;
			}
		}

		public Vector3 LocalAxis
		{
			get
			{
				return this._localAxis;
			}
			set
			{
				this._localAxis = value;
			}
		}

		protected virtual void Update()
		{
			base.transform.Rotate(this._localAxis, this._rotationSpeed * Time.deltaTime, Space.Self);
		}

		[SerializeField]
		private float _rotationSpeed;

		[SerializeField]
		private Vector3 _localAxis = Vector3.up;
	}
}
