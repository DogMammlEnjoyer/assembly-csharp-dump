using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Cinemachine
{
	[ExecuteAlways]
	[AddComponentMenu("Cinemachine/Helpers/Cinemachine Spline Smoother")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineSplineSmoother.html")]
	[RequireComponent(typeof(SplineContainer))]
	public class CinemachineSplineSmoother : MonoBehaviour
	{
		public void SmoothSplineNow()
		{
			SplineContainer splineContainer;
			if (base.TryGetComponent<SplineContainer>(out splineContainer) && splineContainer.Spline != null)
			{
				Spline spline = splineContainer.Spline;
				int count = spline.Count;
				if (count < 3)
				{
					return;
				}
				float3[] array = new float3[count];
				float3[] array2 = new float3[count];
				float3[] array3 = new float3[count];
				for (int i = 0; i < count; i++)
				{
					array3[i] = spline[i].Position;
				}
				if (spline.Closed)
				{
					SplineHelpers.ComputeSmoothControlPointsLooped(ref array3, ref array, ref array2);
				}
				else
				{
					SplineHelpers.ComputeSmoothControlPoints(ref array3, ref array, ref array2);
				}
				for (int j = 0; j < count; j++)
				{
					spline.SetTangentMode(j, TangentMode.Mirrored, BezierTangent.Out);
					BezierKnot bezierKnot = spline[j];
					float3 up = math.mul(bezierKnot.Rotation, new float3(0f, 1f, 0f));
					float3 @float = array[j] - array3[j];
					float num = (math.length(array2[(j > 0) ? (j - 1) : (count - 1)] - array3[j]) + math.length(@float)) * 0.5f;
					bezierKnot.Rotation = quaternion.LookRotationSafe(@float, up);
					bezierKnot.TangentIn = ((j == 0 && !spline.Closed) ? default(float3) : new float3(0f, 0f, -num));
					bezierKnot.TangentOut = ((j == count - 1 && !spline.Closed) ? default(float3) : new float3(0f, 0f, num));
					spline[j] = bezierKnot;
				}
			}
		}

		[Tooltip("If checked, the spline will be automatically smoothed whenever it is modified (editor only).")]
		public bool AutoSmooth = true;
	}
}
