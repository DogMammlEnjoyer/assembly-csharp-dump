using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Cinemachine
{
	[Serializable]
	public struct SplineSettings
	{
		public void ChangeUnitPreservePosition(PathIndexUnit newUnits)
		{
			if (this.Spline.IsValid() && newUnits != this.Units)
			{
				this.Position = this.GetCachedSpline().ConvertIndexUnit(this.Position, this.Units, newUnits);
			}
			this.Units = newUnits;
		}

		internal CachedScaledSpline GetCachedSpline()
		{
			if (!this.Spline.IsValid())
			{
				this.InvalidateCache();
			}
			else
			{
				if (this.m_CachedSpline == null || (Time.frameCount != this.m_CachedFrame && !this.m_CachedSpline.IsCrudelyValid(this.Spline.Spline, this.Spline.transform)))
				{
					this.InvalidateCache();
					this.m_CachedSpline = new CachedScaledSpline(this.Spline.Spline, this.Spline.transform, Allocator.Persistent);
				}
				this.m_CachedFrame = Time.frameCount;
			}
			return this.m_CachedSpline;
		}

		public void InvalidateCache()
		{
			CachedScaledSpline cachedSpline = this.m_CachedSpline;
			if (cachedSpline != null)
			{
				cachedSpline.Dispose();
			}
			this.m_CachedSpline = null;
		}

		[Tooltip("The Spline container to which the position will apply.")]
		public SplineContainer Spline;

		[NoSaveDuringPlay]
		[Tooltip("The position along the spline.  The actual value corresponding to a given point on the spline will depend on the unity type.")]
		public float Position;

		[Tooltip("How to interpret the Spline Position:\n- <b>Distance</b>: Values range from 0 (start of Spline) to Length of the Spline (end of Spline).\n- <b>Normalized</b>: Values range from 0 (start of Spline) to 1 (end of Spline).\n- <b>Knot</b>: Values are defined by knot indices and a fractional value representing the normalized interpolation between the specific knot index and the next knot.\n")]
		public PathIndexUnit Units;

		private CachedScaledSpline m_CachedSpline;

		private int m_CachedFrame;
	}
}
