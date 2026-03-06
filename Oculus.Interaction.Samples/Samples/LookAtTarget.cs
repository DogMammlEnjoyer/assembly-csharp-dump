using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class LookAtTarget : MonoBehaviour
	{
		protected virtual void Start()
		{
		}

		protected virtual void Update()
		{
			Vector3 normalized = (this._target.position - this._toRotate.position).normalized;
			this._toRotate.LookAt(this._toRotate.position - normalized, Vector3.up);
		}

		[SerializeField]
		private Transform _toRotate;

		[SerializeField]
		private Transform _target;
	}
}
