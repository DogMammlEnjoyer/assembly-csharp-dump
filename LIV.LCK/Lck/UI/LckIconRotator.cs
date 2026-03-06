using System;
using UnityEngine;

namespace Liv.Lck.UI
{
	public class LckIconRotator : MonoBehaviour
	{
		public void Rotate()
		{
			float z = this._iconTransform.localEulerAngles.z + this._rotationOffset;
			this._iconTransform.localEulerAngles = new Vector3(0f, 0f, z);
		}

		[SerializeField]
		private float _rotationOffset;

		[SerializeField]
		private Transform _iconTransform;
	}
}
