using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	[NullableContext(2)]
	[Nullable(0)]
	public class MultiPartFire : MonoBehaviour
	{
		private void Start()
		{
			this.lastAngleBottom = 0f;
			this.lastAngleMiddle = 0f;
			this.lastAngleTop = 0f;
			this.perlinBottom = (float)Random.Range(0, 100);
			this.perlinMiddle = (float)Random.Range(200, 300);
			this.perlinTop = (float)Random.Range(400, 500);
			this.tempVec = new Vector3(0f, 0f, 0f);
			this.mergedBottom = false;
			this.mergedMiddle = false;
			this.mergedTop = false;
			this.lastTime = Time.time;
		}

		public void Update()
		{
			this.Flap(ref this.perlinBottom, this.perlinStepBottom, ref this.lastAngleBottom, ref this.baseFire, this.bottomRange, this.baseMultiplier, ref this.mergedBottom);
			this.Flap(ref this.perlinMiddle, this.perlinStepMiddle, ref this.lastAngleMiddle, ref this.middleFire, this.middleRange, this.middleMultiplier, ref this.mergedMiddle);
			this.Flap(ref this.perlinTop, this.perlinStepTop, ref this.lastAngleTop, ref this.topFire, this.topRange, this.topMultiplier, ref this.mergedTop);
			this.lastTime = Time.time;
		}

		private void Flap(ref float perlinValue, float perlinStep, ref float lastAngle, ref Transform flameTransform, float range, float multiplier, ref bool isMerged)
		{
			if (flameTransform == null)
			{
				return;
			}
			perlinValue += perlinStep;
			lastAngle += (Time.time - this.lastTime) * Mathf.PerlinNoise(perlinValue, 0f);
			this.tempVec.x = range * Mathf.Sin(lastAngle * multiplier);
			if (Mathf.Abs(this.tempVec.x - flameTransform.localEulerAngles.x) > 180f)
			{
				if (this.tempVec.x > flameTransform.localEulerAngles.x)
				{
					this.tempVec.x = this.tempVec.x - 360f;
				}
				else
				{
					this.tempVec.x = this.tempVec.x + 360f;
				}
			}
			if (isMerged)
			{
				flameTransform.localEulerAngles = this.tempVec;
				return;
			}
			if (Mathf.Abs(flameTransform.localEulerAngles.x - this.tempVec.x) < 1f)
			{
				isMerged = true;
				flameTransform.localEulerAngles = this.tempVec;
				return;
			}
			this.tempVec.x = (this.tempVec.x - flameTransform.localEulerAngles.x) * this.slerp + flameTransform.localEulerAngles.x;
			flameTransform.localEulerAngles = this.tempVec;
		}

		public Transform baseFire;

		public Transform middleFire;

		public Transform topFire;

		public float baseMultiplier;

		public float middleMultiplier;

		public float topMultiplier;

		public float bottomRange;

		public float middleRange;

		public float topRange;

		public float perlinStepBottom;

		public float perlinStepMiddle;

		public float perlinStepTop;

		public float slerp = 0.01f;

		private float lastAngleBottom;

		private float lastAngleMiddle;

		private float lastAngleTop;

		private float perlinBottom;

		private float perlinMiddle;

		private float perlinTop;

		private bool mergedBottom;

		private bool mergedMiddle;

		private bool mergedTop;

		private Vector3 tempVec;

		private float lastTime;
	}
}
