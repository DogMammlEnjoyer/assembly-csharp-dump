using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Position Control/Cinemachine Spline Dolly")]
	[SaveDuringPlay]
	[DisallowMultipleComponent]
	[CameraPipeline(CinemachineCore.Stage.Body)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineSplineDolly.html")]
	public class CinemachineSplineDolly : CinemachineComponentBase, ISplineReferencer
	{
		private void PerformLegacyUpgrade()
		{
			if (this.m_LegacyPosition != -1f)
			{
				this.m_SplineSettings.Position = this.m_LegacyPosition;
				this.m_SplineSettings.Units = this.m_LegacyUnits;
				this.m_LegacyPosition = -1f;
				this.m_LegacyUnits = PathIndexUnit.Distance;
			}
			if (this.m_LegacySpline != null)
			{
				this.m_SplineSettings.Spline = this.m_LegacySpline;
				this.m_LegacySpline = null;
			}
		}

		public ref SplineSettings SplineSettings
		{
			get
			{
				return ref this.m_SplineSettings;
			}
		}

		public SplineContainer Spline
		{
			get
			{
				return this.m_SplineSettings.Spline;
			}
			set
			{
				this.m_SplineSettings.Spline = value;
			}
		}

		public float CameraPosition
		{
			get
			{
				return this.m_SplineSettings.Position;
			}
			set
			{
				this.m_SplineSettings.Position = value;
			}
		}

		public PathIndexUnit PositionUnits
		{
			get
			{
				return this.m_SplineSettings.Units;
			}
			set
			{
				this.m_SplineSettings.ChangeUnitPreservePosition(value);
			}
		}

		private void OnValidate()
		{
			this.PerformLegacyUpgrade();
			this.Damping.Position.x = Mathf.Clamp(this.Damping.Position.x, 0f, 20f);
			this.Damping.Position.y = Mathf.Clamp(this.Damping.Position.y, 0f, 20f);
			this.Damping.Position.z = Mathf.Clamp(this.Damping.Position.z, 0f, 20f);
			this.Damping.Angular = Mathf.Clamp(this.Damping.Angular, 0f, 20f);
			SplineAutoDolly.ISplineAutoDolly method = this.AutomaticDolly.Method;
			if (method == null)
			{
				return;
			}
			method.Validate();
		}

		private void Reset()
		{
			this.m_SplineSettings = new SplineSettings
			{
				Units = PathIndexUnit.Normalized
			};
			this.SplineOffset = Vector3.zero;
			this.CameraRotation = CinemachineSplineDolly.RotationMode.Default;
			this.Damping = default(CinemachineSplineDolly.DampingSettings);
			this.AutomaticDolly.Method = null;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_RollCache.Refresh(this);
			SplineAutoDolly.ISplineAutoDolly method = this.AutomaticDolly.Method;
			if (method == null)
			{
				return;
			}
			method.Reset();
		}

		protected override void OnDisable()
		{
			this.m_SplineSettings.InvalidateCache();
			base.OnDisable();
		}

		public override bool IsValid
		{
			get
			{
				return base.enabled && this.Spline != null;
			}
		}

		public override CinemachineCore.Stage Stage
		{
			get
			{
				return CinemachineCore.Stage.Body;
			}
		}

		public override float GetMaxDampTime()
		{
			if (this.Damping.Enabled)
			{
				return Mathf.Max(Mathf.Max(this.Damping.Position.x, Mathf.Max(this.Damping.Position.y, this.Damping.Position.z)), this.Damping.Angular);
			}
			return 0f;
		}

		public override void MutateCameraState(ref CameraState curState, float deltaTime)
		{
			if (!this.IsValid)
			{
				return;
			}
			CachedScaledSpline cachedSpline = this.m_SplineSettings.GetCachedSpline();
			if (cachedSpline == null)
			{
				return;
			}
			float num2;
			float num = cachedSpline.StandardizePosition(this.CameraPosition, this.PositionUnits, out num2);
			if (deltaTime < 0f || !base.VirtualCamera.PreviousStateIsValid)
			{
				this.m_PreviousSplinePosition = num;
				this.m_PreviousPosition = curState.RawPosition;
				this.m_PreviousRotation = curState.RawOrientation;
				this.m_RollCache.Refresh(this);
			}
			if (this.AutomaticDolly.Enabled && this.AutomaticDolly.Method != null)
			{
				num = this.AutomaticDolly.Method.GetSplinePosition(this, base.FollowTarget, this.Spline, num, this.PositionUnits, deltaTime);
			}
			if (this.Damping.Enabled && deltaTime >= 0f && base.VirtualCamera.PreviousStateIsValid)
			{
				float num3 = this.m_PreviousSplinePosition;
				if (cachedSpline.Closed && Mathf.Abs(num - num3) > num2 * 0.5f)
				{
					num3 += ((num > num3) ? num2 : (-num2));
				}
				num = num3 + Damper.Damp(num - num3, this.Damping.Position.z, deltaTime);
			}
			this.m_PreviousSplinePosition = (this.CameraPosition = num);
			Vector3 vector;
			Quaternion quaternion;
			cachedSpline.EvaluateSplineWithRoll(this.Spline.transform, cachedSpline.ConvertIndexUnit(num, this.PositionUnits, PathIndexUnit.Normalized), this.m_RollCache.GetSplineRoll(this), out vector, out quaternion);
			Vector3 a = quaternion * Vector3.right;
			Vector3 vector2 = quaternion * Vector3.up;
			Vector3 a2 = quaternion * Vector3.forward;
			vector += this.SplineOffset.x * a;
			vector += this.SplineOffset.y * vector2;
			vector += this.SplineOffset.z * a2;
			if (this.Damping.Enabled && deltaTime >= 0f && base.VirtualCamera.PreviousStateIsValid)
			{
				Vector3 previousPosition = this.m_PreviousPosition;
				Vector3 vector3 = previousPosition - vector;
				Vector3 vector4 = Vector3.Dot(vector3, vector2) * vector2;
				Vector3 vector5 = vector3 - vector4;
				vector5 = Damper.Damp(vector5, this.Damping.Position.x, deltaTime);
				vector4 = Damper.Damp(vector4, this.Damping.Position.y, deltaTime);
				vector = previousPosition - (vector5 + vector4);
			}
			curState.RawPosition = (this.m_PreviousPosition = vector);
			bool flag;
			Quaternion quaternion2 = this.GetCameraRotationAtSplinePoint(quaternion, curState.ReferenceUp, out flag);
			if (this.Damping.Enabled && deltaTime >= 0f && base.VirtualCamera.PreviousStateIsValid)
			{
				float t = base.VirtualCamera.DetachedFollowTargetDamp(1f, this.Damping.Angular, deltaTime);
				quaternion2 = Quaternion.Slerp(this.m_PreviousRotation, quaternion2, t);
			}
			this.m_PreviousRotation = quaternion2;
			curState.RawOrientation = quaternion2;
			if (!flag)
			{
				curState.ReferenceUp = curState.RawOrientation * Vector3.up;
			}
		}

		private Quaternion GetCameraRotationAtSplinePoint(Quaternion splineOrientation, Vector3 up, out bool isDefault)
		{
			isDefault = false;
			switch (this.CameraRotation)
			{
			case CinemachineSplineDolly.RotationMode.Spline:
				return splineOrientation;
			case CinemachineSplineDolly.RotationMode.SplineNoRoll:
				return Quaternion.LookRotation(splineOrientation * Vector3.forward, up);
			case CinemachineSplineDolly.RotationMode.FollowTarget:
				if (base.FollowTarget != null)
				{
					return base.FollowTargetRotation;
				}
				break;
			case CinemachineSplineDolly.RotationMode.FollowTargetNoRoll:
				if (base.FollowTarget != null)
				{
					return Quaternion.LookRotation(base.FollowTargetRotation * Vector3.forward, up);
				}
				break;
			}
			isDefault = true;
			return Quaternion.LookRotation(base.VirtualCamera.transform.rotation * Vector3.forward, up);
		}

		[SerializeField]
		[FormerlySerializedAs("SplineSettings")]
		private SplineSettings m_SplineSettings = new SplineSettings
		{
			Units = PathIndexUnit.Normalized
		};

		[Tooltip("Where to put the camera relative to the spline position.  X is perpendicular to the spline, Y is up, and Z is parallel to the spline.")]
		public Vector3 SplineOffset = Vector3.zero;

		[Tooltip("How to set the camera's rotation and Up.  This will affect the screen composition, because the camera Aim behaviours will always try to respect the Up direction.")]
		[FormerlySerializedAs("CameraUp")]
		public CinemachineSplineDolly.RotationMode CameraRotation;

		[FoldoutWithEnabledButton("Enabled")]
		[Tooltip("Settings for controlling damping, which causes the camera to move gradually towards the desired spline position")]
		public CinemachineSplineDolly.DampingSettings Damping;

		[NoSaveDuringPlay]
		[FoldoutWithEnabledButton("Enabled")]
		[Tooltip("Controls how automatic dolly occurs.  A tracking target may be necessary to use this feature.")]
		public SplineAutoDolly AutomaticDolly;

		private float m_PreviousSplinePosition;

		private Quaternion m_PreviousRotation;

		private Vector3 m_PreviousPosition;

		private CinemachineSplineRoll.RollCache m_RollCache;

		[SerializeField]
		[HideInInspector]
		[NoSaveDuringPlay]
		[FormerlySerializedAs("CameraPosition")]
		private float m_LegacyPosition = -1f;

		[SerializeField]
		[HideInInspector]
		[NoSaveDuringPlay]
		[FormerlySerializedAs("PositionUnits")]
		private PathIndexUnit m_LegacyUnits;

		[SerializeField]
		[HideInInspector]
		[NoSaveDuringPlay]
		[FormerlySerializedAs("Spline")]
		private SplineContainer m_LegacySpline;

		public enum RotationMode
		{
			Default,
			Spline,
			SplineNoRoll,
			FollowTarget,
			FollowTargetNoRoll
		}

		[Serializable]
		public struct DampingSettings
		{
			[Tooltip("Enables damping, which causes the camera to move gradually towards the desired spline position")]
			public bool Enabled;

			[Tooltip("How aggressively the camera tries to maintain the offset along the x, y, or z directions in spline local space. \n- x represents the axis that is perpendicular to the spline. Use this to smooth out imperfections in the path. This may move the camera off the spline.\n- y represents the axis that is defined by the spline-local up direction. Use this to smooth out imperfections in the path. This may move the camera off the spline.\n- z represents the axis that is parallel to the spline. This won't move the camera off the spline.\n\nSmaller numbers are more responsive, larger numbers give a heavier more slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
			public Vector3 Position;

			[Range(0f, 20f)]
			[Tooltip("How aggressively the camera tries to maintain the desired rotation.  This is only used if Camera Rotation is not Default.")]
			public float Angular;
		}
	}
}
