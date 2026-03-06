using System;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Filtering
{
	[Serializable]
	public class XRDistanceEvaluator : XRTargetEvaluator
	{
		public float maxDistance
		{
			get
			{
				return this.m_MaxDistance;
			}
			set
			{
				this.m_MaxDistance = value;
			}
		}

		public override void Reset()
		{
			base.Reset();
			base.weight = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.5f),
				new Keyframe(1f, 1f, 2f, 2f)
			});
		}

		protected override float CalculateNormalizedScore(IXRInteractor interactor, IXRInteractable target)
		{
			if (Mathf.Approximately(this.m_MaxDistance, 0f))
			{
				return 0f;
			}
			float result;
			using (new XRInteractableUtility.AllowTriggerCollidersScope(true))
			{
				result = 1f - Mathf.Clamp01(target.GetDistanceSqrToInteractor(interactor) / (this.m_MaxDistance * this.m_MaxDistance));
			}
			return result;
		}

		[Tooltip("The maximum distance from the Interactor. Any target from this distance will receive a 0 normalized score.")]
		[SerializeField]
		private float m_MaxDistance = 1f;
	}
}
