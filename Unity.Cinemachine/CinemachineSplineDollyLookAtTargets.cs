using System;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Cinemachine
{
	[ExecuteAlways]
	[SaveDuringPlay]
	[CameraPipeline(CinemachineCore.Stage.Aim)]
	[AddComponentMenu("Cinemachine/Procedural/Rotation Control/Cinemachine Spline Dolly LookAt Targets")]
	[DisallowMultipleComponent]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineSplineDollyLookAtTargets.html")]
	public class CinemachineSplineDollyLookAtTargets : CinemachineComponentBase
	{
		private void Reset()
		{
			this.Targets = new SplineData<CinemachineSplineDollyLookAtTargets.Item>
			{
				DefaultValue = new CinemachineSplineDollyLookAtTargets.Item
				{
					Easing = 1f
				}
			};
		}

		public override bool IsValid
		{
			get
			{
				SplineContainer splineContainer;
				CinemachineSplineDolly cinemachineSplineDolly;
				return base.enabled && this.Targets != null && this.GetGetSplineAndDolly(out splineContainer, out cinemachineSplineDolly);
			}
		}

		public override CinemachineCore.Stage Stage
		{
			get
			{
				return CinemachineCore.Stage.Aim;
			}
		}

		public override void MutateCameraState(ref CameraState state, float deltaTime)
		{
			SplineContainer splineContainer;
			CinemachineSplineDolly cinemachineSplineDolly;
			if (!this.GetGetSplineAndDolly(out splineContainer, out cinemachineSplineDolly))
			{
				return;
			}
			CachedScaledSpline cachedSpline = cinemachineSplineDolly.SplineSettings.GetCachedSpline();
			CinemachineSplineDollyLookAtTargets.Item item = this.Targets.Evaluate<CachedScaledSpline, CinemachineSplineDollyLookAtTargets.LerpItem>(cachedSpline, cinemachineSplineDolly.CameraPosition, cinemachineSplineDolly.PositionUnits, default(CinemachineSplineDollyLookAtTargets.LerpItem));
			Vector3 vector = item.WorldLookAt - state.RawPosition;
			if (vector.sqrMagnitude > 0.0001f)
			{
				Vector3 vector2 = state.ReferenceUp;
				if (Vector3.Cross(vector, vector2).sqrMagnitude < 0.0001f)
				{
					vector2 = state.RawOrientation * Vector3.back;
					if (Vector3.Cross(vector, vector2).sqrMagnitude < 0.0001f)
					{
						vector2 = state.RawOrientation * Vector3.left;
					}
				}
				state.RawOrientation = Quaternion.LookRotation(vector, vector2);
			}
			state.ReferenceLookAt = item.Offset;
		}

		internal bool GetGetSplineAndDolly(out SplineContainer spline, out CinemachineSplineDolly dolly)
		{
			dolly = null;
			if (this != null && base.TryGetComponent<CinemachineSplineDolly>(out dolly))
			{
				spline = dolly.Spline;
				return spline != null && spline.Spline != null;
			}
			spline = null;
			return false;
		}

		[Tooltip("LookAt targets for the camera at specific positions on the Spline")]
		public SplineData<CinemachineSplineDollyLookAtTargets.Item> Targets = new SplineData<CinemachineSplineDollyLookAtTargets.Item>
		{
			DefaultValue = new CinemachineSplineDollyLookAtTargets.Item
			{
				Easing = 1f
			}
		};

		[Serializable]
		public struct Item
		{
			public Vector3 WorldLookAt
			{
				readonly get
				{
					if (!(this.LookAt == null))
					{
						return this.LookAt.TransformPoint(this.Offset);
					}
					return this.Offset;
				}
				set
				{
					this.Offset = ((this.LookAt == null) ? value : this.LookAt.InverseTransformPoint(value));
				}
			}

			[Tooltip("The target object to look at.  It may be None, in which case the Offset will specify a point in world space.")]
			public Transform LookAt;

			[Tooltip("The offset (in local coords) from the LookAt target's origin.  If LookAt target is None, this will specify a world-space point.")]
			public Vector3 Offset;

			[Tooltip("Controls how to ease in and out of this data point.  A value of 0 will linearly interpolate between LookAt points, while a value of 1 will slow down and briefly pause the rotation to look at the target.")]
			[Range(0f, 1f)]
			public float Easing;
		}

		internal struct LerpItem : IInterpolator<CinemachineSplineDollyLookAtTargets.Item>
		{
			public CinemachineSplineDollyLookAtTargets.Item Interpolate(CinemachineSplineDollyLookAtTargets.Item a, CinemachineSplineDollyLookAtTargets.Item b, float t)
			{
				float num = t * t;
				float num2 = 1f - t;
				t = 3f * num2 * num2 * t * Mathf.Lerp(0.3333f, 0f, a.Easing) + 3f * num2 * num * Mathf.Lerp(0.6666f, 1f, b.Easing) + t * num;
				return new CinemachineSplineDollyLookAtTargets.Item
				{
					Offset = Vector3.Lerp(a.WorldLookAt, b.WorldLookAt, t)
				};
			}
		}
	}
}
