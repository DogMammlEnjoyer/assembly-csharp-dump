using System;
using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class DistanceHaptics : MonoBehaviour
	{
		private IEnumerator Start()
		{
			for (;;)
			{
				float time = Vector3.Distance(this.firstTransform.position, this.secondTransform.position);
				Hand componentInParent = base.GetComponentInParent<Hand>();
				if (componentInParent != null)
				{
					float num = this.distanceIntensityCurve.Evaluate(time);
					componentInParent.TriggerHapticPulse((ushort)num);
				}
				float seconds = this.pulseIntervalCurve.Evaluate(time);
				yield return new WaitForSeconds(seconds);
			}
			yield break;
		}

		public Transform firstTransform;

		public Transform secondTransform;

		public AnimationCurve distanceIntensityCurve = AnimationCurve.Linear(0f, 800f, 1f, 800f);

		public AnimationCurve pulseIntervalCurve = AnimationCurve.Linear(0f, 0.01f, 1f, 0f);
	}
}
