using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Shot Quality Evaluator")]
	[SaveDuringPlay]
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[RequiredTarget(RequiredTargetAttribute.RequiredTargets.Tracking)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineShotQualityEvaluator.html")]
	public class CinemachineShotQualityEvaluator : CinemachineExtension, IShotQualityEvaluator
	{
		private void OnValidate()
		{
			this.CameraRadius = Mathf.Max(0f, this.CameraRadius);
			this.MinimumDistanceFromTarget = Mathf.Max(0.01f, this.MinimumDistanceFromTarget);
			this.CameraRadius = Mathf.Max(0f, this.CameraRadius);
			this.DistanceEvaluation.NearLimit = Mathf.Max(0.1f, this.DistanceEvaluation.NearLimit);
			this.DistanceEvaluation.FarLimit = Mathf.Max(this.DistanceEvaluation.NearLimit, this.DistanceEvaluation.FarLimit);
			this.DistanceEvaluation.OptimalDistance = Mathf.Clamp(this.DistanceEvaluation.OptimalDistance, this.DistanceEvaluation.NearLimit, this.DistanceEvaluation.FarLimit);
		}

		private void Reset()
		{
			this.OcclusionLayers = 1;
			this.IgnoreTag = string.Empty;
			this.MinimumDistanceFromTarget = 0.2f;
			this.CameraRadius = 0f;
			this.DistanceEvaluation = CinemachineShotQualityEvaluator.DistanceEvaluationSettings.Default;
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (stage == CinemachineCore.Stage.Finalize && state.HasLookAt())
			{
				if (state.IsTargetOffscreen() || this.IsTargetObscured(state))
				{
					state.ShotQuality *= 0.2f;
				}
				if (this.DistanceEvaluation.Enabled)
				{
					float num = 0f;
					if (this.DistanceEvaluation.OptimalDistance > 0f)
					{
						float num2 = Vector3.Magnitude(state.ReferenceLookAt - state.GetFinalPosition());
						if (num2 <= this.DistanceEvaluation.OptimalDistance)
						{
							if (num2 >= this.DistanceEvaluation.NearLimit)
							{
								num = this.DistanceEvaluation.MaxQualityBoost * (num2 - this.DistanceEvaluation.NearLimit) / (this.DistanceEvaluation.OptimalDistance - this.DistanceEvaluation.NearLimit);
							}
						}
						else
						{
							num2 -= this.DistanceEvaluation.OptimalDistance;
							if (num2 < this.DistanceEvaluation.FarLimit)
							{
								num = this.DistanceEvaluation.MaxQualityBoost * (1f - num2 / this.DistanceEvaluation.FarLimit);
							}
						}
						state.ShotQuality *= 1f + num;
					}
				}
			}
		}

		private bool IsTargetObscured(CameraState state)
		{
			Vector3 referenceLookAt = state.ReferenceLookAt;
			Vector3 correctedPosition = state.GetCorrectedPosition();
			Vector3 vector = referenceLookAt - correctedPosition;
			float magnitude = vector.magnitude;
			RaycastHit raycastHit;
			return magnitude < Mathf.Max(this.MinimumDistanceFromTarget, 0.0001f) || RuntimeUtility.SphereCastIgnoreTag(new Ray(correctedPosition, vector.normalized), this.CameraRadius, out raycastHit, magnitude - this.MinimumDistanceFromTarget, this.OcclusionLayers, this.IgnoreTag);
		}

		[Tooltip("Objects on these layers will be detected")]
		public LayerMask OcclusionLayers = 1;

		[TagField]
		[Tooltip("Obstacles with this tag will be ignored.  It is a good idea to set this field to the target's tag")]
		public string IgnoreTag = string.Empty;

		[Tooltip("Obstacles closer to the target than this will be ignored")]
		public float MinimumDistanceFromTarget = 0.2f;

		[Tooltip("Radius of the spherecast that will be done to check for occlusions.")]
		public float CameraRadius;

		[FoldoutWithEnabledButton("Enabled")]
		public CinemachineShotQualityEvaluator.DistanceEvaluationSettings DistanceEvaluation = CinemachineShotQualityEvaluator.DistanceEvaluationSettings.Default;

		[Serializable]
		public struct DistanceEvaluationSettings
		{
			internal static CinemachineShotQualityEvaluator.DistanceEvaluationSettings Default
			{
				get
				{
					return new CinemachineShotQualityEvaluator.DistanceEvaluationSettings
					{
						NearLimit = 5f,
						FarLimit = 30f,
						OptimalDistance = 10f,
						MaxQualityBoost = 0.2f
					};
				}
			}

			[Tooltip("If enabled, will evaluate shot quality based on target distance")]
			public bool Enabled;

			[Tooltip("If greater than zero, maximum quality boost will occur when target is this far from the camera")]
			public float OptimalDistance;

			[Tooltip("Shots with targets closer to the camera than this will not get a quality boost")]
			public float NearLimit;

			[Tooltip("Shots with targets farther from the camera than this will not get a quality boost")]
			public float FarLimit;

			[Tooltip("High quality shots will be boosted by this fraction of their normal quality")]
			public float MaxQualityBoost;
		}
	}
}
