using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class StayInView : MonoBehaviour
	{
		private void Update()
		{
			base.transform.rotation = Quaternion.identity;
			base.transform.position = this._eyeCenter.position;
			base.transform.Rotate(0f, this._eyeCenter.rotation.eulerAngles.y, 0f, Space.Self);
			base.transform.position = this._eyeCenter.position + base.transform.forward.normalized * this._extraDistanceForward;
			if (this._zeroOutEyeHeight)
			{
				base.transform.position = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
			}
		}

		[SerializeField]
		private Transform _eyeCenter;

		[SerializeField]
		private float _extraDistanceForward;

		[SerializeField]
		private bool _zeroOutEyeHeight = true;
	}
}
