using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Obsolete("LegacyLensSettings is deprecated. Use LensSettings instead.")]
	[Serializable]
	public struct LegacyLensSettings
	{
		public LensSettings ToLensSettings()
		{
			LensSettings result = new LensSettings
			{
				FieldOfView = this.FieldOfView,
				OrthographicSize = this.OrthographicSize,
				NearClipPlane = this.NearClipPlane,
				FarClipPlane = this.FarClipPlane,
				Dutch = this.Dutch,
				ModeOverride = this.ModeOverride,
				PhysicalProperties = LensSettings.Default.PhysicalProperties
			};
			result.PhysicalProperties.GateFit = this.GateFit;
			result.PhysicalProperties.SensorSize = this.m_SensorSize;
			result.PhysicalProperties.LensShift = this.LensShift;
			result.PhysicalProperties.FocusDistance = this.FocusDistance;
			result.PhysicalProperties.Iso = this.Iso;
			result.PhysicalProperties.ShutterSpeed = this.ShutterSpeed;
			result.PhysicalProperties.Aperture = this.Aperture;
			result.PhysicalProperties.BladeCount = this.BladeCount;
			result.PhysicalProperties.Curvature = this.Curvature;
			result.PhysicalProperties.BarrelClipping = this.BarrelClipping;
			result.PhysicalProperties.Anamorphism = this.Anamorphism;
			return result;
		}

		public void SetFromLensSettings(LensSettings src)
		{
			this.FieldOfView = src.FieldOfView;
			this.OrthographicSize = src.OrthographicSize;
			this.NearClipPlane = src.NearClipPlane;
			this.FarClipPlane = src.FarClipPlane;
			this.Dutch = src.Dutch;
			this.ModeOverride = src.ModeOverride;
			this.GateFit = src.PhysicalProperties.GateFit;
			this.m_SensorSize = src.PhysicalProperties.SensorSize;
			this.LensShift = src.PhysicalProperties.LensShift;
			this.FocusDistance = src.PhysicalProperties.FocusDistance;
			this.Iso = src.PhysicalProperties.Iso;
			this.ShutterSpeed = src.PhysicalProperties.ShutterSpeed;
			this.Aperture = src.PhysicalProperties.Aperture;
			this.BladeCount = src.PhysicalProperties.BladeCount;
			this.Curvature = src.PhysicalProperties.Curvature;
			this.BarrelClipping = src.PhysicalProperties.BarrelClipping;
			this.Anamorphism = src.PhysicalProperties.Anamorphism;
		}

		public void Validate()
		{
			this.FarClipPlane = Mathf.Max(this.FarClipPlane, this.NearClipPlane + 0.001f);
			this.FieldOfView = Mathf.Clamp(this.FieldOfView, 0.01f, 179f);
			this.FocusDistance = Mathf.Max(this.FocusDistance, 0.01f);
			this.ShutterSpeed = Mathf.Max(0f, this.ShutterSpeed);
			this.Aperture = Mathf.Clamp(this.Aperture, 0.7f, 32f);
			this.BladeCount = Mathf.Clamp(this.BladeCount, 3, 11);
			this.BarrelClipping = Mathf.Clamp01(this.BarrelClipping);
			this.Curvature.x = Mathf.Clamp(this.Curvature.x, 0.7f, 32f);
			this.Curvature.y = Mathf.Clamp(this.Curvature.y, this.Curvature.x, 32f);
			this.Anamorphism = Mathf.Clamp(this.Anamorphism, -1f, 1f);
		}

		public static LegacyLensSettings Default
		{
			get
			{
				return new LegacyLensSettings
				{
					FieldOfView = 40f,
					OrthographicSize = 10f,
					NearClipPlane = 0.1f,
					FarClipPlane = 5000f,
					Dutch = 0f,
					ModeOverride = LensSettings.OverrideModes.None,
					m_SensorSize = new Vector2(21.946f, 16.002f),
					GateFit = Camera.GateFitMode.Horizontal,
					FocusDistance = 10f,
					LensShift = Vector2.zero,
					Iso = 200,
					ShutterSpeed = 0.005f,
					Aperture = 16f,
					BladeCount = 5,
					Curvature = new Vector2(2f, 11f),
					BarrelClipping = 0.25f,
					Anamorphism = 0f
				};
			}
		}

		public float FieldOfView;

		public float OrthographicSize;

		public float NearClipPlane;

		public float FarClipPlane;

		public float Dutch;

		public LensSettings.OverrideModes ModeOverride;

		public Camera.GateFitMode GateFit;

		[HideInInspector]
		public Vector2 m_SensorSize;

		public Vector2 LensShift;

		public float FocusDistance;

		public int Iso;

		public float ShutterSpeed;

		public float Aperture;

		public int BladeCount;

		public Vector2 Curvature;

		public float BarrelClipping;

		public float Anamorphism;
	}
}
