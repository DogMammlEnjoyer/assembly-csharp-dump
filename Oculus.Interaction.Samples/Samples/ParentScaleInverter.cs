using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class ParentScaleInverter : MonoBehaviour
	{
		private void Start()
		{
			this._initialLocalScale = base.transform.localScale;
			this._initialParentScale = base.transform.parent.localScale;
		}

		private void LateUpdate()
		{
			base.transform.localScale = new Vector3(this._initialParentScale.x * this._initialLocalScale.x / base.transform.parent.localScale.x, this._initialParentScale.y * this._initialLocalScale.y / base.transform.parent.localScale.y, this._initialParentScale.z * this._initialLocalScale.z / base.transform.parent.localScale.z);
		}

		private Vector3 _initialLocalScale;

		private Vector3 _initialParentScale;
	}
}
