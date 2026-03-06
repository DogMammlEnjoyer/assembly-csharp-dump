using System;
using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class WheelDust : MonoBehaviour
	{
		private void Start()
		{
			this.col = base.GetComponent<WheelCollider>();
			base.StartCoroutine(this.emitter());
		}

		private void Update()
		{
			this.slip = Vector3.zero;
			if (this.col.isGrounded)
			{
				WheelHit wheelHit;
				this.col.GetGroundHit(out wheelHit);
				this.slip += Vector3.right * wheelHit.sidewaysSlip;
				this.slip += Vector3.forward * -wheelHit.forwardSlip;
			}
			this.amt = this.slip.magnitude;
		}

		private IEnumerator emitter()
		{
			for (;;)
			{
				if (this.emitTimer >= 1f)
				{
					this.emitTimer = 0f;
					this.DoEmit();
				}
				else
				{
					yield return null;
					if (this.amt > this.minSlip)
					{
						this.emitTimer += Mathf.Clamp(this.EmissionMul * this.amt, 0.01f, this.maxEmission);
					}
				}
			}
			yield break;
		}

		private void DoEmit()
		{
			this.p.transform.rotation = Quaternion.LookRotation(base.transform.TransformDirection(this.slip));
			this.p.main.startSpeed = this.velocityMul * this.amt;
			this.p.Emit(1);
		}

		private WheelCollider col;

		public ParticleSystem p;

		public float EmissionMul;

		public float velocityMul = 2f;

		public float maxEmission;

		public float minSlip;

		[HideInInspector]
		public float amt;

		[HideInInspector]
		public Vector3 slip;

		private float emitTimer;
	}
}
