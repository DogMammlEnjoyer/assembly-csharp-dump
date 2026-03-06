using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Follow Zoom")]
	[SaveDuringPlay]
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[RequiredTarget(RequiredTargetAttribute.RequiredTargets.LookAt)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineFollowZoom.html")]
	public class CinemachineFollowZoom : CinemachineExtension
	{
		private void Reset()
		{
			this.Width = 2f;
			this.Damping = 1f;
			this.FovRange = new Vector2(3f, 60f);
		}

		private void OnValidate()
		{
			this.Width = Mathf.Max(0f, this.Width);
			this.FovRange.y = Mathf.Clamp(this.FovRange.y, 1f, 179f);
			this.FovRange.x = Mathf.Clamp(this.FovRange.x, 1f, this.FovRange.y);
		}

		public override float GetMaxDampTime()
		{
			return this.Damping;
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			CinemachineFollowZoom.VcamExtraState extraState = base.GetExtraState<CinemachineFollowZoom.VcamExtraState>(vcam);
			if (deltaTime < 0f || !vcam.PreviousStateIsValid)
			{
				extraState.m_PreviousFrameZoom = state.Lens.FieldOfView;
			}
			if (stage == CinemachineCore.Stage.Body)
			{
				float num = Mathf.Max(this.Width, 0f);
				float value = 179f;
				float num2 = Vector3.Distance(state.GetCorrectedPosition(), state.ReferenceLookAt);
				if (num2 > 0.0001f)
				{
					float min = num2 * 2f * Mathf.Tan(this.FovRange.x * 0.017453292f / 2f);
					float max = num2 * 2f * Mathf.Tan(this.FovRange.y * 0.017453292f / 2f);
					num = Mathf.Clamp(num, min, max);
					if (deltaTime >= 0f && this.Damping > 0f && vcam.PreviousStateIsValid)
					{
						float num3 = num2 * 2f * Mathf.Tan(extraState.m_PreviousFrameZoom * 0.017453292f / 2f);
						float num4 = num - num3;
						num4 = vcam.DetachedLookAtTargetDamp(num4, this.Damping, deltaTime);
						num = num3 + num4;
					}
					value = 2f * Mathf.Atan(num / (2f * num2)) * 57.29578f;
				}
				LensSettings lens = state.Lens;
				lens.FieldOfView = (extraState.m_PreviousFrameZoom = Mathf.Clamp(value, this.FovRange.x, this.FovRange.y));
				state.Lens = lens;
			}
		}

		[Tooltip("The shot width to maintain, in world units, at target distance.")]
		[FormerlySerializedAs("m_Width")]
		public float Width = 2f;

		[Range(0f, 20f)]
		[Tooltip("Increase this value to soften the aggressiveness of the follow-zoom.  Small numbers are more responsive, larger numbers give a more heavy slowly responding camera.")]
		[FormerlySerializedAs("m_Damping")]
		public float Damping = 1f;

		[MinMaxRangeSlider(1f, 179f)]
		[Tooltip("Range for the FOV that this behaviour will generate.")]
		public Vector2 FovRange = new Vector2(3f, 60f);

		private class VcamExtraState : CinemachineExtension.VcamExtraStateBase
		{
			public float m_PreviousFrameZoom;
		}
	}
}
