using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace Unity.Cinemachine
{
	[Obsolete("CinemachineTrackedDolly has been deprecated. Use CinemachineSplineDolly instead.")]
	[AddComponentMenu("")]
	[SaveDuringPlay]
	[CameraPipeline(CinemachineCore.Stage.Body)]
	public class CinemachineTrackedDolly : CinemachineComponentBase
	{
		public override bool IsValid
		{
			get
			{
				return base.enabled && this.m_Path != null;
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
			Vector3 angularDamping = this.AngularDamping;
			float a = Mathf.Max(this.m_XDamping, Mathf.Max(this.m_YDamping, this.m_ZDamping));
			float b = Mathf.Max(angularDamping.x, Mathf.Max(angularDamping.y, angularDamping.z));
			return Mathf.Max(a, b);
		}

		public override void MutateCameraState(ref CameraState curState, float deltaTime)
		{
			if (deltaTime < 0f || !base.VirtualCamera.PreviousStateIsValid)
			{
				this.m_PreviousPathPosition = this.m_PathPosition;
				this.m_PreviousCameraPosition = curState.RawPosition;
				this.m_PreviousOrientation = curState.RawOrientation;
			}
			if (!this.IsValid)
			{
				return;
			}
			if (this.m_AutoDolly.m_Enabled && base.FollowTarget != null)
			{
				float f = this.m_Path.ToNativePathUnits(this.m_PreviousPathPosition, this.m_PositionUnits);
				this.m_PathPosition = this.m_Path.FindClosestPoint(base.FollowTargetPosition, Mathf.FloorToInt(f), (deltaTime < 0f || this.m_AutoDolly.m_SearchRadius <= 0) ? -1 : this.m_AutoDolly.m_SearchRadius, this.m_AutoDolly.m_SearchResolution);
				this.m_PathPosition = this.m_Path.FromPathNativeUnits(this.m_PathPosition, this.m_PositionUnits);
				this.m_PathPosition += this.m_AutoDolly.m_PositionOffset;
			}
			float num = this.m_PathPosition;
			if (deltaTime >= 0f && base.VirtualCamera.PreviousStateIsValid)
			{
				float num2 = this.m_Path.MaxUnit(this.m_PositionUnits);
				if (num2 > 0f)
				{
					float num3 = this.m_Path.StandardizeUnit(this.m_PreviousPathPosition, this.m_PositionUnits);
					float num4 = this.m_Path.StandardizeUnit(num, this.m_PositionUnits);
					if (this.m_Path.Looped && Mathf.Abs(num4 - num3) > num2 / 2f)
					{
						if (num4 > num3)
						{
							num3 += num2;
						}
						else
						{
							num3 -= num2;
						}
					}
					this.m_PreviousPathPosition = num3;
					num = num4;
				}
				float num5 = this.m_PreviousPathPosition - num;
				num5 = Damper.Damp(num5, this.m_ZDamping, deltaTime);
				num = this.m_PreviousPathPosition - num5;
			}
			this.m_PreviousPathPosition = num;
			Quaternion quaternion = this.m_Path.EvaluateOrientationAtUnit(num, this.m_PositionUnits);
			Vector3 vector = this.m_Path.EvaluatePositionAtUnit(num, this.m_PositionUnits);
			Vector3 a = quaternion * Vector3.right;
			Vector3 vector2 = quaternion * Vector3.up;
			Vector3 a2 = quaternion * Vector3.forward;
			vector += this.m_PathOffset.x * a;
			vector += this.m_PathOffset.y * vector2;
			vector += this.m_PathOffset.z * a2;
			if (deltaTime >= 0f && base.VirtualCamera.PreviousStateIsValid)
			{
				Vector3 previousCameraPosition = this.m_PreviousCameraPosition;
				Vector3 vector3 = previousCameraPosition - vector;
				Vector3 vector4 = Vector3.Dot(vector3, vector2) * vector2;
				Vector3 vector5 = vector3 - vector4;
				vector5 = Damper.Damp(vector5, this.m_XDamping, deltaTime);
				vector4 = Damper.Damp(vector4, this.m_YDamping, deltaTime);
				vector = previousCameraPosition - (vector5 + vector4);
			}
			curState.RawPosition = (this.m_PreviousCameraPosition = vector);
			Quaternion quaternion2 = this.GetCameraOrientationAtPathPoint(quaternion, curState.ReferenceUp);
			if (deltaTime >= 0f && base.VirtualCamera.PreviousStateIsValid)
			{
				Vector3 vector6 = (Quaternion.Inverse(this.m_PreviousOrientation) * quaternion2).eulerAngles;
				for (int i = 0; i < 3; i++)
				{
					if (vector6[i] > 180f)
					{
						ref Vector3 ptr = ref vector6;
						int index = i;
						ptr[index] -= 360f;
					}
				}
				vector6 = Damper.Damp(vector6, this.AngularDamping, deltaTime);
				quaternion2 = this.m_PreviousOrientation * Quaternion.Euler(vector6);
			}
			this.m_PreviousOrientation = quaternion2;
			curState.RawOrientation = quaternion2;
			if (this.m_CameraUp != CinemachineTrackedDolly.CameraUpMode.Default)
			{
				curState.ReferenceUp = curState.RawOrientation * Vector3.up;
			}
		}

		private Quaternion GetCameraOrientationAtPathPoint(Quaternion pathOrientation, Vector3 up)
		{
			switch (this.m_CameraUp)
			{
			case CinemachineTrackedDolly.CameraUpMode.Path:
				return pathOrientation;
			case CinemachineTrackedDolly.CameraUpMode.PathNoRoll:
				return Quaternion.LookRotation(pathOrientation * Vector3.forward, up);
			case CinemachineTrackedDolly.CameraUpMode.FollowTarget:
				if (base.FollowTarget != null)
				{
					return base.FollowTargetRotation;
				}
				break;
			case CinemachineTrackedDolly.CameraUpMode.FollowTargetNoRoll:
				if (base.FollowTarget != null)
				{
					return Quaternion.LookRotation(base.FollowTargetRotation * Vector3.forward, up);
				}
				break;
			}
			return Quaternion.LookRotation(base.VirtualCamera.transform.rotation * Vector3.forward, up);
		}

		private Vector3 AngularDamping
		{
			get
			{
				switch (this.m_CameraUp)
				{
				case CinemachineTrackedDolly.CameraUpMode.Default:
					return Vector3.zero;
				case CinemachineTrackedDolly.CameraUpMode.PathNoRoll:
				case CinemachineTrackedDolly.CameraUpMode.FollowTargetNoRoll:
					return new Vector3(this.m_PitchDamping, this.m_YawDamping, 0f);
				}
				return new Vector3(this.m_PitchDamping, this.m_YawDamping, this.m_RollDamping);
			}
		}

		internal void UpgradeToCm3(CinemachineSplineDolly c)
		{
			c.Damping.Enabled = true;
			c.Damping.Position = new Vector3(this.m_XDamping, this.m_YDamping, this.m_ZDamping);
			c.Damping.Angular = Mathf.Max(this.m_YawDamping, Mathf.Max(this.m_RollDamping, this.m_PitchDamping));
			c.CameraRotation = (CinemachineSplineDolly.RotationMode)this.m_CameraUp;
			c.AutomaticDolly.Enabled = this.m_AutoDolly.m_Enabled;
			if (this.m_AutoDolly.m_Enabled)
			{
				c.AutomaticDolly.Method = new SplineAutoDolly.NearestPointToTarget
				{
					PositionOffset = this.m_AutoDolly.m_PositionOffset,
					SearchResolution = this.m_AutoDolly.m_SearchResolution,
					SearchIteration = 2
				};
			}
			if (this.m_Path != null)
			{
				c.Spline = this.m_Path.GetComponent<SplineContainer>();
			}
			c.CameraPosition = this.m_PathPosition;
			switch (this.m_PositionUnits)
			{
			case CinemachinePathBase.PositionUnits.PathUnits:
				c.PositionUnits = PathIndexUnit.Knot;
				break;
			case CinemachinePathBase.PositionUnits.Distance:
				c.PositionUnits = PathIndexUnit.Distance;
				break;
			case CinemachinePathBase.PositionUnits.Normalized:
				c.PositionUnits = PathIndexUnit.Normalized;
				break;
			}
			c.SplineOffset = this.m_PathOffset;
		}

		[Tooltip("The path to which the camera will be constrained.  This must be non-null.")]
		public CinemachinePathBase m_Path;

		[Tooltip("The position along the path at which the camera will be placed.  This can be animated directly, or set automatically by the Auto-Dolly feature to get as close as possible to the Follow target.  The value is interpreted according to the Position Units setting.")]
		public float m_PathPosition;

		[Tooltip("How to interpret Path Position.  If set to Path Units, values are as follows: 0 represents the first waypoint on the path, 1 is the second, and so on.  Values in-between are points on the path in between the waypoints.  If set to Distance, then Path Position represents distance along the path.")]
		public CinemachinePathBase.PositionUnits m_PositionUnits;

		[Tooltip("Where to put the camera relative to the path position.  X is perpendicular to the path, Y is up, and Z is parallel to the path.  This allows the camera to be offset from the path itself (as if on a tripod, for example).")]
		public Vector3 m_PathOffset = Vector3.zero;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to maintain its position in a direction perpendicular to the path.  Small numbers are more responsive, rapidly translating the camera to keep the target's x-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
		public float m_XDamping;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to maintain its position in the path-local up direction.  Small numbers are more responsive, rapidly translating the camera to keep the target's y-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
		public float m_YDamping;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to maintain its position in a direction parallel to the path.  Small numbers are more responsive, rapidly translating the camera to keep the target's z-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
		public float m_ZDamping = 1f;

		[Tooltip("How to set the virtual camera's Up vector.  This will affect the screen composition, because the camera Aim behaviours will always try to respect the Up direction.")]
		public CinemachineTrackedDolly.CameraUpMode m_CameraUp;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to track the target rotation's X angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
		public float m_PitchDamping;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to track the target rotation's Y angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
		public float m_YawDamping;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to track the target rotation's Z angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
		public float m_RollDamping;

		[Tooltip("Controls how automatic dollying occurs.  A Follow target is necessary to use this feature.")]
		public CinemachineTrackedDolly.AutoDolly m_AutoDolly = new CinemachineTrackedDolly.AutoDolly(false, 0f, 2, 5);

		private float m_PreviousPathPosition;

		private Quaternion m_PreviousOrientation = Quaternion.identity;

		private Vector3 m_PreviousCameraPosition = Vector3.zero;

		public enum CameraUpMode
		{
			Default,
			Path,
			PathNoRoll,
			FollowTarget,
			FollowTargetNoRoll
		}

		[Serializable]
		public struct AutoDolly
		{
			public AutoDolly(bool enabled, float positionOffset, int searchRadius, int stepsPerSegment)
			{
				this.m_Enabled = enabled;
				this.m_PositionOffset = positionOffset;
				this.m_SearchRadius = searchRadius;
				this.m_SearchResolution = stepsPerSegment;
			}

			[Tooltip("If checked, will enable automatic dolly, which chooses a path position that is as close as possible to the Follow target.  Note: this can have significant performance impact")]
			public bool m_Enabled;

			[Tooltip("Offset, in current position units, from the closest point on the path to the follow target")]
			public float m_PositionOffset;

			[Tooltip("Search up to this many waypoints on either side of the current position.  Use 0 for Entire path.")]
			public int m_SearchRadius;

			[FormerlySerializedAs("m_StepsPerSegment")]
			[Tooltip("We search between waypoints by dividing the segment into this many straight pieces.  he higher the number, the more accurate the result, but performance is proportionally slower for higher numbers")]
			public int m_SearchResolution;
		}
	}
}
