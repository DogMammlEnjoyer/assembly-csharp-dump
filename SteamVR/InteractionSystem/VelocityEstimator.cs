using System;
using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class VelocityEstimator : MonoBehaviour
	{
		public void BeginEstimatingVelocity()
		{
			this.FinishEstimatingVelocity();
			this.routine = base.StartCoroutine(this.EstimateVelocityCoroutine());
		}

		public void FinishEstimatingVelocity()
		{
			if (this.routine != null)
			{
				base.StopCoroutine(this.routine);
				this.routine = null;
			}
		}

		public Vector3 GetVelocityEstimate()
		{
			Vector3 vector = Vector3.zero;
			int num = Mathf.Min(this.sampleCount, this.velocitySamples.Length);
			if (num != 0)
			{
				for (int i = 0; i < num; i++)
				{
					vector += this.velocitySamples[i];
				}
				vector *= 1f / (float)num;
			}
			return vector;
		}

		public Vector3 GetAngularVelocityEstimate()
		{
			Vector3 vector = Vector3.zero;
			int num = Mathf.Min(this.sampleCount, this.angularVelocitySamples.Length);
			if (num != 0)
			{
				for (int i = 0; i < num; i++)
				{
					vector += this.angularVelocitySamples[i];
				}
				vector *= 1f / (float)num;
			}
			return vector;
		}

		public Vector3 GetAccelerationEstimate()
		{
			Vector3 a = Vector3.zero;
			for (int i = 2 + this.sampleCount - this.velocitySamples.Length; i < this.sampleCount; i++)
			{
				if (i >= 2)
				{
					int num = i - 2;
					int num2 = i - 1;
					Vector3 b = this.velocitySamples[num % this.velocitySamples.Length];
					Vector3 a2 = this.velocitySamples[num2 % this.velocitySamples.Length];
					a += a2 - b;
				}
			}
			return a * (1f / Time.deltaTime);
		}

		private void Awake()
		{
			this.velocitySamples = new Vector3[this.velocityAverageFrames];
			this.angularVelocitySamples = new Vector3[this.angularVelocityAverageFrames];
			if (this.estimateOnAwake)
			{
				this.BeginEstimatingVelocity();
			}
		}

		private IEnumerator EstimateVelocityCoroutine()
		{
			this.sampleCount = 0;
			Vector3 previousPosition = base.transform.position;
			Quaternion previousRotation = base.transform.rotation;
			for (;;)
			{
				yield return new WaitForEndOfFrame();
				float num = 1f / Time.deltaTime;
				int num2 = this.sampleCount % this.velocitySamples.Length;
				int num3 = this.sampleCount % this.angularVelocitySamples.Length;
				this.sampleCount++;
				this.velocitySamples[num2] = num * (base.transform.position - previousPosition);
				Quaternion quaternion = base.transform.rotation * Quaternion.Inverse(previousRotation);
				float num4 = 2f * Mathf.Acos(Mathf.Clamp(quaternion.w, -1f, 1f));
				if (num4 > 3.1415927f)
				{
					num4 -= 6.2831855f;
				}
				Vector3 vector = new Vector3(quaternion.x, quaternion.y, quaternion.z);
				if (vector.sqrMagnitude > 0f)
				{
					vector = num4 * num * vector.normalized;
				}
				this.angularVelocitySamples[num3] = vector;
				previousPosition = base.transform.position;
				previousRotation = base.transform.rotation;
			}
			yield break;
		}

		[Tooltip("How many frames to average over for computing velocity")]
		public int velocityAverageFrames = 5;

		[Tooltip("How many frames to average over for computing angular velocity")]
		public int angularVelocityAverageFrames = 11;

		public bool estimateOnAwake;

		private Coroutine routine;

		private int sampleCount;

		private Vector3[] velocitySamples;

		private Vector3[] angularVelocitySamples;
	}
}
