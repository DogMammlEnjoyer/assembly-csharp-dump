using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Cinemachine
{
	internal static class SplineContainerExtensions
	{
		public static bool IsValid(this ISplineContainer spline)
		{
			return spline != null && spline.Splines != null && spline.Splines.Count > 0;
		}

		public static bool LocalEvaluateSplineWithRoll(this ISpline spline, float tNormalized, CinemachineSplineRoll roll, out Vector3 position, out Quaternion rotation)
		{
			float3 v;
			float3 v2;
			float3 v3;
			if (spline == null || !spline.Evaluate(tNormalized, out v, out v2, out v3))
			{
				position = Vector3.zero;
				rotation = Quaternion.identity;
				return false;
			}
			Vector3 v4 = Vector3.Cross(v2, v3);
			if (v4.AlmostZero() || v4.IsNaN())
			{
				v2 = Vector3.forward;
				v3 = Vector3.up;
			}
			if (roll == null || !roll.enabled)
			{
				rotation = Quaternion.LookRotation(v2, v3);
			}
			else
			{
				float angle = roll.Roll.Evaluate<ISpline, IInterpolator<CinemachineSplineRoll.RollData>>(spline, tNormalized, PathIndexUnit.Normalized, roll.GetInterpolator());
				rotation = Quaternion.LookRotation(v2, v3) * SplineContainerExtensions.<LocalEvaluateSplineWithRoll>g__RollAroundForward|1_0(angle);
			}
			position = v;
			return true;
		}

		public static bool EvaluateSplineWithRoll(this ISpline spline, Transform transform, float tNormalized, CinemachineSplineRoll roll, out Vector3 position, out Quaternion rotation)
		{
			bool result = spline.LocalEvaluateSplineWithRoll(tNormalized, roll, out position, out rotation);
			position = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).MultiplyPoint3x4(position);
			rotation = transform.rotation * rotation;
			return result;
		}

		public static Vector3 EvaluateSplinePosition(this ISpline spline, Transform transform, float tNormalized)
		{
			float3 v = (spline == null) ? default(float3) : spline.EvaluatePosition(tNormalized);
			if (!(transform == null))
			{
				return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).MultiplyPoint3x4(v);
			}
			return v;
		}

		public static float GetMaxPosition(this ISpline spline, PathIndexUnit unit)
		{
			if (unit == PathIndexUnit.Distance)
			{
				return spline.GetLength();
			}
			if (unit != PathIndexUnit.Knot)
			{
				return 1f;
			}
			int count = spline.Count;
			return (float)((!spline.Closed || count < 2) ? Mathf.Max(0, count - 1) : count);
		}

		public static float StandardizePosition(this ISpline spline, float t, PathIndexUnit unit, out float maxPos)
		{
			maxPos = spline.GetMaxPosition(unit);
			if (float.IsNaN(t))
			{
				return 0f;
			}
			if (!spline.Closed)
			{
				return Mathf.Clamp(t, 0f, maxPos);
			}
			t %= maxPos;
			if (t < 0f)
			{
				t += maxPos;
			}
			return t;
		}

		[CompilerGenerated]
		internal static Quaternion <LocalEvaluateSplineWithRoll>g__RollAroundForward|1_0(float angle)
		{
			float f = angle * 0.5f * 0.017453292f;
			return new Quaternion(0f, 0f, Mathf.Sin(f), Mathf.Cos(f));
		}
	}
}
