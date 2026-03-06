using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Cinemachine
{
	[Serializable]
	public struct SplineAutoDolly
	{
		[Tooltip("If set, will enable the selected automatic dolly along the spline")]
		public bool Enabled;

		[SerializeReference]
		public SplineAutoDolly.ISplineAutoDolly Method;

		public interface ISplineAutoDolly
		{
			void Validate();

			void Reset();

			bool RequiresTrackingTarget { get; }

			float GetSplinePosition(MonoBehaviour sender, Transform target, SplineContainer spline, float currentPosition, PathIndexUnit positionUnits, float deltaTime);
		}

		[Serializable]
		public class FixedSpeed : SplineAutoDolly.ISplineAutoDolly
		{
			void SplineAutoDolly.ISplineAutoDolly.Validate()
			{
			}

			void SplineAutoDolly.ISplineAutoDolly.Reset()
			{
			}

			bool SplineAutoDolly.ISplineAutoDolly.RequiresTrackingTarget
			{
				get
				{
					return false;
				}
			}

			float SplineAutoDolly.ISplineAutoDolly.GetSplinePosition(MonoBehaviour sender, Transform target, SplineContainer spline, float currentPosition, PathIndexUnit positionUnits, float deltaTime)
			{
				if (Application.isPlaying && spline.IsValid() && deltaTime > 0f)
				{
					return currentPosition + this.Speed * deltaTime;
				}
				return currentPosition;
			}

			[Tooltip("Speed of travel, in current position units per second.")]
			public float Speed;
		}

		[Serializable]
		public class NearestPointToTarget : SplineAutoDolly.ISplineAutoDolly
		{
			void SplineAutoDolly.ISplineAutoDolly.Validate()
			{
				this.SearchResolution = Mathf.Max(this.SearchResolution, 1);
				this.SearchIteration = Mathf.Max(this.SearchIteration, 1);
			}

			void SplineAutoDolly.ISplineAutoDolly.Reset()
			{
			}

			bool SplineAutoDolly.ISplineAutoDolly.RequiresTrackingTarget
			{
				get
				{
					return true;
				}
			}

			float SplineAutoDolly.ISplineAutoDolly.GetSplinePosition(MonoBehaviour sender, Transform target, SplineContainer spline, float currentPosition, PathIndexUnit positionUnits, float deltaTime)
			{
				if (target == null || !spline.IsValid())
				{
					return currentPosition;
				}
				float3 @float;
				float value;
				SplineUtility.GetNearestPoint<Spline>(spline.Spline, spline.transform.InverseTransformPoint(target.position), out @float, out value, this.SearchResolution, this.SearchIteration);
				value = Mathf.Clamp01(value);
				return spline.Spline.ConvertIndexUnit(value, PathIndexUnit.Normalized, positionUnits) + this.PositionOffset;
			}

			[Tooltip("Offset, in current position units, from the closest point on the spline to the follow target")]
			public float PositionOffset;

			[Tooltip("Affects how many segments to split a spline into when calculating the nearest point.  Higher values mean smaller and more segments, which increases accuracy at the cost of processing time.  In most cases, the default value (4) is appropriate. Use with SearchIteration to fine-tune point accuracy.")]
			public int SearchResolution = 4;

			[Tooltip("The nearest point is calculated by finding the nearest point on the entire length of the spline using SearchResolution to divide into equally spaced line segments. Successive iterations will then subdivide further the nearest segment, producing more accurate results. In most cases, the default value (2) is sufficient.")]
			public int SearchIteration = 2;
		}
	}
}
