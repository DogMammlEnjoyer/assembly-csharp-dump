using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Liv.Lck.Smoothing
{
	[DefaultExecutionOrder(1000)]
	public class LckStabilizer : MonoBehaviour
	{
		public UpdateTimingMode StabilizationUpdateTimingMode
		{
			get
			{
				return this._stabilizationUpdateTimingMode;
			}
			set
			{
				this._stabilizationUpdateTimingMode = value;
			}
		}

		public Transform StabilizationTarget
		{
			get
			{
				return this._stabilizationTarget;
			}
			set
			{
				this._stabilizationTarget = value;
			}
		}

		public Transform TargetToFollow
		{
			get
			{
				return this._targetToFollow;
			}
			set
			{
				this._targetToFollow = value;
			}
		}

		public Transform StabilizationSpaceOrigin
		{
			get
			{
				return this._stabilizationSpaceOrigin;
			}
			set
			{
				if (this._stabilizationSpaceOrigin == value)
				{
					return;
				}
				this._stabilizationSpaceOrigin = value;
				this.HandleStabilizationSpaceChanged();
			}
		}

		public float PositionalSmoothing
		{
			get
			{
				return this._positionalSmoothing;
			}
			set
			{
				this._positionalSmoothing = value;
			}
		}

		public float RotationalSmoothing
		{
			get
			{
				return this._rotationalSmoothing;
			}
			set
			{
				this._rotationalSmoothing = value;
			}
		}

		public bool AffectPosition
		{
			get
			{
				return this._affectPosition;
			}
			set
			{
				this._affectPosition = value;
			}
		}

		public bool AffectRotation
		{
			get
			{
				return this._affectRotation;
			}
			set
			{
				this._affectRotation = value;
			}
		}

		private KalmanFilterVector3 PositionFilter
		{
			get
			{
				KalmanFilterVector3 result;
				if ((result = this._positionFilter) == null)
				{
					result = (this._positionFilter = new KalmanFilterVector3(this.GetStabilizationSpacePosition(this.TargetToFollow.position), 1f));
				}
				return result;
			}
		}

		private KalmanFilterQuaternion RotationFilter
		{
			get
			{
				KalmanFilterQuaternion result;
				if ((result = this._rotationFilter) == null)
				{
					result = (this._rotationFilter = new KalmanFilterQuaternion(this.GetStabilizationSpaceRotation(this.TargetToFollow.rotation), 1f));
				}
				return result;
			}
		}

		private bool HasCustomStabilizationSpace
		{
			get
			{
				return this._stabilizationSpaceOrigin;
			}
		}

		private void LateUpdate()
		{
			if (this.StabilizationUpdateTimingMode == UpdateTimingMode.LateUpdate)
			{
				this.DoStabilizationUpdate(this.PositionalSmoothing, this.RotationalSmoothing);
			}
		}

		private void Update()
		{
			if (this.StabilizationUpdateTimingMode == UpdateTimingMode.Update)
			{
				this.DoStabilizationUpdate(this.PositionalSmoothing, this.RotationalSmoothing);
			}
		}

		private void FixedUpdate()
		{
			if (this.StabilizationUpdateTimingMode == UpdateTimingMode.FixedUpdate)
			{
				this.DoStabilizationUpdate(this.PositionalSmoothing, this.RotationalSmoothing);
			}
		}

		public void ReachTargetInstantly()
		{
			this.DoStabilizationUpdate(0f, 0f);
		}

		private void DoStabilizationUpdate(float positionalSmoothing, float rotationalSmoothing)
		{
			if (this.AffectPosition)
			{
				Vector3 stabilizationSpacePosition = this.GetStabilizationSpacePosition(this.TargetToFollow.position);
				Vector3 stabilizationSpacePosition2 = this.PositionFilter.Update(stabilizationSpacePosition, Time.deltaTime, positionalSmoothing);
				this.StabilizationTarget.position = this.GetWorldPosition(stabilizationSpacePosition2);
			}
			if (this.AffectRotation)
			{
				Quaternion stabilizationSpaceRotation = this.GetStabilizationSpaceRotation(this.TargetToFollow.rotation);
				Quaternion stabilizationSpaceRotation2 = this.RotationFilter.Update(stabilizationSpaceRotation, Time.deltaTime, rotationalSmoothing);
				this.StabilizationTarget.rotation = this.GetWorldRotation(stabilizationSpaceRotation2);
			}
		}

		private void HandleStabilizationSpaceChanged()
		{
			if (this.AffectPosition)
			{
				Vector3 stabilizationSpacePosition = this.GetStabilizationSpacePosition(this.StabilizationTarget.position);
				this.PositionFilter.Update(stabilizationSpacePosition, Time.deltaTime, 0f);
			}
			if (this.AffectRotation)
			{
				Quaternion stabilizationSpaceRotation = this.GetStabilizationSpaceRotation(this.StabilizationTarget.rotation);
				this.RotationFilter.Update(stabilizationSpaceRotation, Time.deltaTime, 0f);
			}
		}

		private Vector3 GetWorldPosition(Vector3 stabilizationSpacePosition)
		{
			if (!this.HasCustomStabilizationSpace)
			{
				return stabilizationSpacePosition;
			}
			return this.StabilizationSpaceOrigin.TransformPoint(stabilizationSpacePosition);
		}

		private Quaternion GetWorldRotation(Quaternion stabilizationSpaceRotation)
		{
			if (!this.HasCustomStabilizationSpace)
			{
				return stabilizationSpaceRotation;
			}
			return this.StabilizationSpaceOrigin.rotation * stabilizationSpaceRotation;
		}

		private Vector3 GetStabilizationSpacePosition(Vector3 worldPosition)
		{
			if (!this.HasCustomStabilizationSpace)
			{
				return worldPosition;
			}
			return this.StabilizationSpaceOrigin.InverseTransformPoint(worldPosition);
		}

		private Quaternion GetStabilizationSpaceRotation(Quaternion worldRotation)
		{
			if (!this.HasCustomStabilizationSpace)
			{
				return worldRotation;
			}
			return Quaternion.Inverse(this.StabilizationSpaceOrigin.rotation) * worldRotation;
		}

		[Header("Transform References")]
		[SerializeField]
		[FormerlySerializedAs("StabilizationTarget")]
		private Transform _stabilizationTarget;

		[SerializeField]
		[FormerlySerializedAs("TargetToFollow")]
		private Transform _targetToFollow;

		[Header("Stabilization Settings")]
		[SerializeField]
		[FormerlySerializedAs("PositionalSmoothing")]
		private float _positionalSmoothing = 0.1f;

		[SerializeField]
		[FormerlySerializedAs("RotationalSmoothing")]
		private float _rotationalSmoothing = 0.1f;

		[SerializeField]
		[FormerlySerializedAs("AffectPosition")]
		private bool _affectPosition = true;

		[SerializeField]
		[FormerlySerializedAs("AffectRotation")]
		private bool _affectRotation = true;

		[SerializeField]
		private UpdateTimingMode _stabilizationUpdateTimingMode = UpdateTimingMode.LateUpdate;

		[Header("Optional References")]
		[SerializeField]
		[Tooltip("(Optional) Follow target movement relative to this transform will be stabilized. If left unspecified, will stabilize follow target movement in world space.")]
		private Transform _stabilizationSpaceOrigin;

		private KalmanFilterVector3 _positionFilter;

		private KalmanFilterQuaternion _rotationFilter;
	}
}
