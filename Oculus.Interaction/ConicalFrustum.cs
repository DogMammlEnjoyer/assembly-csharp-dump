using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ConicalFrustum : MonoBehaviour
	{
		public Pose Pose
		{
			get
			{
				return base.transform.GetPose(Space.World);
			}
		}

		public float MinLength
		{
			get
			{
				return this._minLength;
			}
			set
			{
				this._minLength = value;
			}
		}

		public float MaxLength
		{
			get
			{
				return this._maxLength;
			}
			set
			{
				this._maxLength = value;
			}
		}

		public float RadiusStart
		{
			get
			{
				return this._radiusStart;
			}
			set
			{
				this._radiusStart = value;
			}
		}

		public float ApertureDegrees
		{
			get
			{
				return this._apertureDegrees;
			}
			set
			{
				this._apertureDegrees = value;
			}
		}

		public Vector3 StartPoint
		{
			get
			{
				return base.transform.position + this.Direction * this.MinLength;
			}
		}

		public Vector3 EndPoint
		{
			get
			{
				return base.transform.position + this.Direction * this.MaxLength;
			}
		}

		public Vector3 Direction
		{
			get
			{
				return base.transform.forward;
			}
		}

		public bool IsPointInConeFrustum(Vector3 point)
		{
			Vector3 vector = Vector3.Project(point - base.transform.position, this.Direction);
			if (Vector3.Dot(vector, this.Direction) < 0f)
			{
				return false;
			}
			float magnitude = vector.magnitude;
			return magnitude >= this._minLength && magnitude <= this._maxLength && Vector3.Distance(this.Pose.position + vector, point) <= this.ConeFrustumRadiusAtLength(magnitude);
		}

		public float ConeFrustumRadiusAtLength(float length)
		{
			float b = this._maxLength * Mathf.Tan(this._apertureDegrees * 0.017453292f);
			float t = length / this._maxLength;
			return Mathf.Lerp(this._radiusStart, b, t);
		}

		public bool HitsCollider(Collider collider, out float score, out Vector3 point)
		{
			Vector3 center = collider.bounds.center;
			Vector3 position = this.Pose.position + Vector3.Project(center - this.Pose.position, this.Pose.forward);
			point = collider.ClosestPointOnBounds(position);
			if (!this.IsPointInConeFrustum(point))
			{
				score = 0f;
				return false;
			}
			float num = Vector3.Angle((point - this.Pose.position).normalized, this.Pose.forward);
			score = 1f - Mathf.Clamp01(num / this._apertureDegrees);
			return true;
		}

		public Vector3 NearestColliderHit(Collider collider, out float score)
		{
			Vector3 center = collider.bounds.center;
			Vector3 position = this.Pose.position + Vector3.Project(center - this.Pose.position, this.Pose.forward);
			Vector3 vector = collider.ClosestPointOnBounds(position);
			float num = Vector3.Angle((vector - this.Pose.position).normalized, this.Pose.forward);
			score = 1f - Mathf.Clamp01(num / this._apertureDegrees);
			return vector;
		}

		[SerializeField]
		[Min(0f)]
		private float _minLength;

		[SerializeField]
		[Min(0f)]
		private float _maxLength = 5f;

		[SerializeField]
		[Min(0f)]
		private float _radiusStart = 0.03f;

		[SerializeField]
		[Range(0f, 90f)]
		private float _apertureDegrees = 20f;
	}
}
