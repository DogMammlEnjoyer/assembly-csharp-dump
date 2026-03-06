using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Serializable]
	public struct LensSettings
	{
		public bool Orthographic
		{
			get
			{
				return this.ModeOverride == LensSettings.OverrideModes.Orthographic || (this.ModeOverride == LensSettings.OverrideModes.None && this.m_OrthoFromCamera);
			}
		}

		public bool IsPhysicalCamera
		{
			get
			{
				return this.ModeOverride == LensSettings.OverrideModes.Physical || (this.ModeOverride == LensSettings.OverrideModes.None && this.m_PhysicalFromCamera);
			}
		}

		public float Aspect
		{
			get
			{
				if (!this.IsPhysicalCamera)
				{
					return this.m_AspectFromCamera;
				}
				return this.PhysicalProperties.SensorSize.x / this.PhysicalProperties.SensorSize.y;
			}
		}

		public static LensSettings Default
		{
			get
			{
				return new LensSettings
				{
					FieldOfView = 40f,
					OrthographicSize = 10f,
					NearClipPlane = 0.1f,
					FarClipPlane = 5000f,
					Dutch = 0f,
					ModeOverride = LensSettings.OverrideModes.None,
					PhysicalProperties = new LensSettings.PhysicalSettings
					{
						SensorSize = new Vector2(21.946f, 16.002f),
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
					},
					m_AspectFromCamera = 1f
				};
			}
		}

		public static LensSettings FromCamera(Camera fromCamera)
		{
			LensSettings @default = LensSettings.Default;
			if (fromCamera != null)
			{
				@default.PullInheritedPropertiesFromCamera(fromCamera);
				@default.FieldOfView = fromCamera.fieldOfView;
				@default.OrthographicSize = fromCamera.orthographicSize;
				@default.NearClipPlane = fromCamera.nearClipPlane;
				@default.FarClipPlane = fromCamera.farClipPlane;
				if (@default.IsPhysicalCamera)
				{
					@default.FieldOfView = Camera.FocalLengthToFieldOfView(Mathf.Max(0.01f, fromCamera.focalLength), fromCamera.sensorSize.y);
					@default.PhysicalProperties.SensorSize = fromCamera.sensorSize;
					@default.PhysicalProperties.LensShift = fromCamera.lensShift;
					@default.PhysicalProperties.GateFit = fromCamera.gateFit;
					@default.PhysicalProperties.FocusDistance = fromCamera.focusDistance;
					@default.PhysicalProperties.Iso = fromCamera.iso;
					@default.PhysicalProperties.ShutterSpeed = fromCamera.shutterSpeed;
					@default.PhysicalProperties.Aperture = fromCamera.aperture;
					@default.PhysicalProperties.BladeCount = fromCamera.bladeCount;
					@default.PhysicalProperties.Curvature = fromCamera.curvature;
					@default.PhysicalProperties.BarrelClipping = fromCamera.barrelClipping;
					@default.PhysicalProperties.Anamorphism = fromCamera.anamorphism;
				}
			}
			return @default;
		}

		public void PullInheritedPropertiesFromCamera(Camera camera)
		{
			if (this.ModeOverride == LensSettings.OverrideModes.None)
			{
				this.m_OrthoFromCamera = camera.orthographic;
				this.m_PhysicalFromCamera = camera.usePhysicalProperties;
			}
			this.m_AspectFromCamera = camera.aspect;
		}

		public void CopyCameraMode(ref LensSettings fromLens)
		{
			this.ModeOverride = fromLens.ModeOverride;
			if (this.ModeOverride == LensSettings.OverrideModes.None)
			{
				this.m_OrthoFromCamera = fromLens.Orthographic;
				this.m_PhysicalFromCamera = fromLens.IsPhysicalCamera;
			}
			this.m_AspectFromCamera = fromLens.m_AspectFromCamera;
		}

		public static LensSettings Lerp(LensSettings lensA, LensSettings lensB, float t)
		{
			t = Mathf.Clamp01(t);
			if (t < 0.5f)
			{
				LensSettings result = lensA;
				result.Lerp(lensB, t);
				return result;
			}
			LensSettings result2 = lensB;
			result2.Lerp(lensA, 1f - t);
			return result2;
		}

		public void Lerp(in LensSettings lensB, float t)
		{
			this.FarClipPlane = Mathf.Lerp(this.FarClipPlane, lensB.FarClipPlane, t);
			this.NearClipPlane = Mathf.Lerp(this.NearClipPlane, lensB.NearClipPlane, t);
			this.FieldOfView = Mathf.Lerp(this.FieldOfView, lensB.FieldOfView, t);
			this.OrthographicSize = Mathf.Lerp(this.OrthographicSize, lensB.OrthographicSize, t);
			this.Dutch = Mathf.Lerp(this.Dutch, lensB.Dutch, t);
			this.PhysicalProperties.SensorSize = Vector2.Lerp(this.PhysicalProperties.SensorSize, lensB.PhysicalProperties.SensorSize, t);
			this.PhysicalProperties.LensShift = Vector2.Lerp(this.PhysicalProperties.LensShift, lensB.PhysicalProperties.LensShift, t);
			this.PhysicalProperties.FocusDistance = Mathf.Lerp(this.PhysicalProperties.FocusDistance, lensB.PhysicalProperties.FocusDistance, t);
			this.PhysicalProperties.Iso = Mathf.RoundToInt(Mathf.Lerp((float)this.PhysicalProperties.Iso, (float)lensB.PhysicalProperties.Iso, t));
			this.PhysicalProperties.ShutterSpeed = Mathf.Lerp(this.PhysicalProperties.ShutterSpeed, lensB.PhysicalProperties.ShutterSpeed, t);
			this.PhysicalProperties.Aperture = Mathf.Lerp(this.PhysicalProperties.Aperture, lensB.PhysicalProperties.Aperture, t);
			this.PhysicalProperties.BladeCount = Mathf.RoundToInt(Mathf.Lerp((float)this.PhysicalProperties.BladeCount, (float)lensB.PhysicalProperties.BladeCount, t));
			this.PhysicalProperties.Curvature = Vector2.Lerp(this.PhysicalProperties.Curvature, lensB.PhysicalProperties.Curvature, t);
			this.PhysicalProperties.BarrelClipping = Mathf.Lerp(this.PhysicalProperties.BarrelClipping, lensB.PhysicalProperties.BarrelClipping, t);
			this.PhysicalProperties.Anamorphism = Mathf.Lerp(this.PhysicalProperties.Anamorphism, lensB.PhysicalProperties.Anamorphism, t);
		}

		public void Validate()
		{
			this.FarClipPlane = Mathf.Max(this.FarClipPlane, this.NearClipPlane + 0.001f);
			this.FieldOfView = Mathf.Clamp(this.FieldOfView, 0.01f, 179f);
			this.PhysicalProperties.SensorSize.x = Mathf.Max(this.PhysicalProperties.SensorSize.x, 0.1f);
			this.PhysicalProperties.SensorSize.y = Mathf.Max(this.PhysicalProperties.SensorSize.y, 0.1f);
			this.PhysicalProperties.FocusDistance = Mathf.Max(this.PhysicalProperties.FocusDistance, 0.01f);
			if (this.m_AspectFromCamera == 0f)
			{
				this.m_AspectFromCamera = 1f;
			}
			this.PhysicalProperties.ShutterSpeed = Mathf.Max(0f, this.PhysicalProperties.ShutterSpeed);
			this.PhysicalProperties.Aperture = Mathf.Clamp(this.PhysicalProperties.Aperture, 0.7f, 32f);
			this.PhysicalProperties.BladeCount = Mathf.Clamp(this.PhysicalProperties.BladeCount, 3, 11);
			this.PhysicalProperties.BarrelClipping = Mathf.Clamp01(this.PhysicalProperties.BarrelClipping);
			this.PhysicalProperties.Curvature.x = Mathf.Clamp(this.PhysicalProperties.Curvature.x, 0.7f, 32f);
			this.PhysicalProperties.Curvature.y = Mathf.Clamp(this.PhysicalProperties.Curvature.y, this.PhysicalProperties.Curvature.x, 32f);
			this.PhysicalProperties.Anamorphism = Mathf.Clamp(this.PhysicalProperties.Anamorphism, -1f, 1f);
		}

		public static bool AreEqual(ref LensSettings a, ref LensSettings b)
		{
			return Mathf.Approximately(a.NearClipPlane, b.NearClipPlane) && Mathf.Approximately(a.FarClipPlane, b.FarClipPlane) && Mathf.Approximately(a.OrthographicSize, b.OrthographicSize) && Mathf.Approximately(a.FieldOfView, b.FieldOfView) && Mathf.Approximately(a.Dutch, b.Dutch) && Mathf.Approximately(a.PhysicalProperties.LensShift.x, b.PhysicalProperties.LensShift.x) && Mathf.Approximately(a.PhysicalProperties.LensShift.y, b.PhysicalProperties.LensShift.y) && Mathf.Approximately(a.PhysicalProperties.SensorSize.x, b.PhysicalProperties.SensorSize.x) && Mathf.Approximately(a.PhysicalProperties.SensorSize.y, b.PhysicalProperties.SensorSize.y) && a.PhysicalProperties.GateFit == b.PhysicalProperties.GateFit && Mathf.Approximately(a.PhysicalProperties.FocusDistance, b.PhysicalProperties.FocusDistance) && Mathf.Approximately((float)a.PhysicalProperties.Iso, (float)b.PhysicalProperties.Iso) && Mathf.Approximately(a.PhysicalProperties.ShutterSpeed, b.PhysicalProperties.ShutterSpeed) && Mathf.Approximately(a.PhysicalProperties.Aperture, b.PhysicalProperties.Aperture) && a.PhysicalProperties.BladeCount == b.PhysicalProperties.BladeCount && Mathf.Approximately(a.PhysicalProperties.Curvature.x, b.PhysicalProperties.Curvature.x) && Mathf.Approximately(a.PhysicalProperties.Curvature.y, b.PhysicalProperties.Curvature.y) && Mathf.Approximately(a.PhysicalProperties.BarrelClipping, b.PhysicalProperties.BarrelClipping) && Mathf.Approximately(a.PhysicalProperties.Anamorphism, b.PhysicalProperties.Anamorphism);
		}

		[Tooltip("This setting controls the Field of View or Local Length of the lens, depending on whether the camera mode is physical or nonphysical.  Field of View can be either horizontal or vertical, depending on the setting in the Camera component.")]
		public float FieldOfView;

		[Tooltip("When using an orthographic camera, this defines the half-height, in world coordinates, of the camera view.")]
		public float OrthographicSize;

		[Tooltip("This defines the near region in the renderable range of the camera frustum. Raising this value will stop the game from drawing things near the camera, which can sometimes come in handy.  Larger values will also increase your shadow resolution.")]
		public float NearClipPlane;

		[Tooltip("This defines the far region of the renderable range of the camera frustum. Typically you want to set this value as low as possible without cutting off desired distant objects")]
		public float FarClipPlane;

		[Tooltip("Camera Z roll, or tilt, in degrees.")]
		public float Dutch;

		[Tooltip("Allows you to select a different camera mode to apply to the Camera component when Cinemachine activates this Virtual Camera.")]
		public LensSettings.OverrideModes ModeOverride;

		public LensSettings.PhysicalSettings PhysicalProperties;

		private bool m_OrthoFromCamera;

		private bool m_PhysicalFromCamera;

		private float m_AspectFromCamera;

		public enum OverrideModes
		{
			None,
			Orthographic,
			Perspective,
			Physical
		}

		[Tooltip("These are settings that are used only if IsPhysicalCamera is true")]
		[Serializable]
		public struct PhysicalSettings
		{
			[Tooltip("How the image is fitted to the sensor if the aspect ratios differ")]
			public Camera.GateFitMode GateFit;

			[SensorSizeProperty]
			[Tooltip("This is the actual size of the image sensor (in mm)")]
			public Vector2 SensorSize;

			[Tooltip("Position of the gate relative to the film back")]
			public Vector2 LensShift;

			[Tooltip("Distance from the camera lens at which focus is sharpest.  The Depth of Field Volume override uses this value if you set FocusDistanceMode to Camera")]
			public float FocusDistance;

			[Tooltip("The sensor sensitivity (ISO)")]
			public int Iso;

			[Tooltip("The exposure time, in seconds")]
			public float ShutterSpeed;

			[Tooltip("The aperture number, in f-stop")]
			[Range(0.7f, 32f)]
			public float Aperture;

			[Tooltip("The number of diaphragm blades")]
			[Range(3f, 11f)]
			public int BladeCount;

			[Tooltip("Maps an aperture range to blade curvature")]
			[MinMaxRangeSlider(0.7f, 32f)]
			public Vector2 Curvature;

			[Tooltip("The strength of the \"cat-eye\" effect on bokeh (optical vignetting)")]
			[Range(0f, 1f)]
			public float BarrelClipping;

			[Tooltip("Stretches the sensor to simulate an anamorphic look.  Positive values distort the camera vertically, negative values distort the camera horizontally")]
			[Range(-1f, 1f)]
			public float Anamorphism;
		}
	}
}
