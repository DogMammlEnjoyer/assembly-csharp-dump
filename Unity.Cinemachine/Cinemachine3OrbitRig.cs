using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	public static class Cinemachine3OrbitRig
	{
		[Serializable]
		public struct Orbit
		{
			[Tooltip("Horizontal radius of the orbit")]
			public float Radius;

			[Tooltip("Height of the horizontal orbit circle, relative to the target position")]
			public float Height;
		}

		[Serializable]
		public struct Settings
		{
			public static Cinemachine3OrbitRig.Settings Default
			{
				get
				{
					return new Cinemachine3OrbitRig.Settings
					{
						SplineCurvature = 0.5f,
						Top = new Cinemachine3OrbitRig.Orbit
						{
							Height = 5f,
							Radius = 2f
						},
						Center = new Cinemachine3OrbitRig.Orbit
						{
							Height = 2.25f,
							Radius = 4f
						},
						Bottom = new Cinemachine3OrbitRig.Orbit
						{
							Height = 0.1f,
							Radius = 2.5f
						}
					};
				}
			}

			[Tooltip("Value to take at the top of the axis range")]
			public Cinemachine3OrbitRig.Orbit Top;

			[Tooltip("Value to take at the center of the axis range")]
			public Cinemachine3OrbitRig.Orbit Center;

			[Tooltip("Value to take at the bottom of the axis range")]
			public Cinemachine3OrbitRig.Orbit Bottom;

			[Tooltip("Controls how taut is the line that connects the rigs' orbits, which determines final placement on the Y axis")]
			[Range(0f, 1f)]
			public float SplineCurvature;
		}

		internal struct OrbitSplineCache
		{
			public bool SettingsChanged(in Cinemachine3OrbitRig.Settings other)
			{
				return this.OrbitSettings.SplineCurvature != other.SplineCurvature || this.OrbitSettings.Top.Height != other.Top.Height || this.OrbitSettings.Top.Radius != other.Top.Radius || this.OrbitSettings.Center.Height != other.Center.Height || this.OrbitSettings.Center.Radius != other.Center.Radius || this.OrbitSettings.Bottom.Height != other.Bottom.Height || this.OrbitSettings.Bottom.Radius != other.Bottom.Radius;
			}

			public void UpdateOrbitCache(in Cinemachine3OrbitRig.Settings orbits)
			{
				this.OrbitSettings = orbits;
				float splineCurvature = orbits.SplineCurvature;
				this.CachedKnots = new Vector4[5];
				this.CachedCtrl1 = new Vector4[5];
				this.CachedCtrl2 = new Vector4[5];
				this.CachedKnots[1] = new Vector4(0f, orbits.Bottom.Height, -orbits.Bottom.Radius, -1f);
				this.CachedKnots[2] = new Vector4(0f, orbits.Center.Height, -orbits.Center.Radius, 0f);
				this.CachedKnots[3] = new Vector4(0f, orbits.Top.Height, -orbits.Top.Radius, 1f);
				this.CachedKnots[0] = Vector4.Lerp(this.CachedKnots[1] + (this.CachedKnots[1] - this.CachedKnots[2]) * 0.5f, Vector4.zero, splineCurvature);
				this.CachedKnots[4] = Vector4.Lerp(this.CachedKnots[3] + (this.CachedKnots[3] - this.CachedKnots[2]) * 0.5f, Vector4.zero, splineCurvature);
				SplineHelpers.ComputeSmoothControlPoints(ref this.CachedKnots, ref this.CachedCtrl1, ref this.CachedCtrl2);
			}

			public Vector4 SplineValue(float t)
			{
				if (this.CachedKnots == null)
				{
					return Vector4.zero;
				}
				int num = 1;
				if (t > 0.5f)
				{
					t -= 0.5f;
					num = 2;
				}
				Vector4 result = SplineHelpers.Bezier3(t * 2f, this.CachedKnots[num], this.CachedCtrl1[num], this.CachedCtrl2[num], this.CachedKnots[num + 1]);
				result.w = SplineHelpers.Bezier1(t * 2f, this.CachedKnots[num].w, this.CachedCtrl1[num].w, this.CachedCtrl2[num].w, this.CachedKnots[num + 1].w);
				return result;
			}

			private Cinemachine3OrbitRig.Settings OrbitSettings;

			private Vector4[] CachedKnots;

			private Vector4[] CachedCtrl1;

			private Vector4[] CachedCtrl2;
		}
	}
}
