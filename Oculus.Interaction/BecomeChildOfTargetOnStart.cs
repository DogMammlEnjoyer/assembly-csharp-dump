using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class BecomeChildOfTargetOnStart : MonoBehaviour
	{
		protected virtual void Start()
		{
			base.transform.SetParent(this._target, this._keepWorldPosition);
		}

		public void InjectAllChildToTransform(Transform target)
		{
			this.InjectTarget(target);
		}

		public void InjectTarget(Transform target)
		{
			this._target = target;
		}

		public void InjectOptionalKeepWorldPosition(bool keepWorldPosition)
		{
			this._keepWorldPosition = keepWorldPosition;
		}

		[SerializeField]
		private Transform _target;

		[SerializeField]
		private bool _keepWorldPosition = true;
	}
}
