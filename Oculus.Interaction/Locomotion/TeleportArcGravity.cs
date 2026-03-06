using System;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class TeleportArcGravity : MonoBehaviour, IPolyline
	{
		public AnimationCurve RangeCurve
		{
			get
			{
				return this._rangeCurve;
			}
			set
			{
				this._rangeCurve = value;
			}
		}

		public AnimationCurve StabilizationMixCurve
		{
			get
			{
				return this._stabilizationMixCurve;
			}
			set
			{
				this._stabilizationMixCurve = value;
			}
		}

		public AnimationCurve PitchCurve
		{
			get
			{
				return this._pitchCurve;
			}
			set
			{
				this._pitchCurve = value;
			}
		}

		public float GravityModifier
		{
			get
			{
				return this._gravityModifier;
			}
			set
			{
				this._gravityModifier = value;
			}
		}

		public int PointsCount
		{
			get
			{
				return this._arcPointsCount;
			}
			set
			{
				this._arcPointsCount = value;
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.UpdateArcParameters();
			this.EndStart(ref this._started);
		}

		protected virtual void Update()
		{
			this.UpdateArcParameters();
		}

		public Vector3 PointAtIndex(int index)
		{
			float t = (float)index / ((float)this._arcPointsCount - 1f);
			return this.EvaluateGravityArc(this._pose, this._speed, t);
		}

		private Vector3 EvaluateGravityArc(Pose origin, float speed, float t)
		{
			Vector3 vector = origin.position + origin.forward * speed * t + 0.5f * t * t * TeleportArcGravity.GRAVITY * this._gravityModifier;
			if (t >= 1f && vector.y > origin.position.y - TeleportArcGravity.GROUND_MARGIN)
			{
				vector.y = origin.position.y - TeleportArcGravity.GROUND_MARGIN;
			}
			return vector;
		}

		public void UpdateArcParameters()
		{
			this._pose = this.CalculatePose();
			this._speed = this.CalculateSpeed(this._pose);
		}

		private Pose CalculatePose()
		{
			Pose pose = this._origin.GetPose(Space.World);
			this.StabilizeDirection(ref pose);
			this.RemapPitch(ref pose);
			return pose;
		}

		private float CalculateSpeed(Pose pose)
		{
			Vector3 vector = pose.position - this._stabilizationPoint.position;
			vector.y = 0f;
			float magnitude = vector.magnitude;
			return this._rangeCurve.Evaluate(magnitude);
		}

		private void StabilizeDirection(ref Pose pose)
		{
			Vector3 up = this._stabilizationPoint.up;
			Vector3 vector = (pose.position - this._stabilizationPoint.position).normalized;
			if (vector.sqrMagnitude == 0f)
			{
				vector = this._stabilizationPoint.forward;
			}
			Quaternion b = Quaternion.LookRotation(vector);
			float num = Vector3.Dot(vector, up) * 0.5f + 0.5f;
			num = this._stabilizationMixCurve.Evaluate(num);
			Quaternion rotation = Quaternion.Lerp(pose.rotation, b, num);
			pose.rotation = rotation;
		}

		private void RemapPitch(ref Pose pose)
		{
			Vector3 up = this._stabilizationPoint.up;
			Vector3 forward = pose.forward;
			Vector3 normalized = Vector3.ProjectOnPlane(forward, up).normalized;
			Vector3 normalized2 = Vector3.Cross(normalized, up).normalized;
			float num = Vector3.SignedAngle(normalized, forward, normalized2);
			num = this._pitchCurve.Evaluate(num);
			Vector3 forward2 = Quaternion.AngleAxis(num, normalized2) * normalized;
			if (forward2.sqrMagnitude != 0f)
			{
				pose.rotation = Quaternion.LookRotation(forward2, pose.up);
			}
		}

		public void InjectAllTeleportArcGravity(Transform origin, Transform stabilizationPoint)
		{
			this.InjectOrigin(origin);
			this.InjectStabilizationPoint(stabilizationPoint);
		}

		public void InjectOrigin(Transform origin)
		{
			this._origin = origin;
		}

		public void InjectStabilizationPoint(Transform stabilizationPoint)
		{
			this._stabilizationPoint = stabilizationPoint;
		}

		[SerializeField]
		[Tooltip("The transform from which the arc will be casted")]
		private Transform _origin;

		[SerializeField]
		[Tooltip("A point behind the origin used to stabilize the aiming direction.")]
		private Transform _stabilizationPoint;

		[SerializeField]
		[Tooltip("Increases the range of the arc based on the distance from the origin to the stabilization point.")]
		private AnimationCurve _rangeCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 5f),
			new Keyframe(1f, 20f)
		});

		[SerializeField]
		[Tooltip("Mixes the direction of the origin with the stabilized direction based on the pitch.")]
		private AnimationCurve _stabilizationMixCurve = AnimationCurve.Constant(0f, 1f, 1f);

		[SerializeField]
		[Tooltip("Alters the pitch of the origin based on the entry pitch")]
		private AnimationCurve _pitchCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(-90f, -90f),
			new Keyframe(90f, 90f)
		});

		[SerializeField]
		[Tooltip("Multiplier for the gravity force")]
		private float _gravityModifier = 2.3f;

		[SerializeField]
		[Min(2f)]
		private int _arcPointsCount = 30;

		private static readonly Vector3 GRAVITY = new Vector3(0f, -9.81f, 0f);

		private static readonly float GROUND_MARGIN = 2f;

		private Pose _pose = Pose.identity;

		private float _speed;

		protected bool _started;
	}
}
