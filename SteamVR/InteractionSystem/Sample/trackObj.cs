using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class trackObj : MonoBehaviour
	{
		private void Update()
		{
			Vector3 vector = this.target.position - base.transform.position;
			if (this.negative)
			{
				vector = -vector;
			}
			if (this.speed == 0f)
			{
				base.transform.rotation = Quaternion.LookRotation(vector);
				return;
			}
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(vector), this.speed * Time.deltaTime);
		}

		public Transform target;

		public float speed;

		public bool negative;
	}
}
