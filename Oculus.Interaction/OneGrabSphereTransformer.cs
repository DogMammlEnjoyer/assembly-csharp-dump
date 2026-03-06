using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class OneGrabSphereTransformer : MonoBehaviour, ITransformer
	{
		public float MinAngle
		{
			get
			{
				return this._minAngle;
			}
			set
			{
				this._minAngle = value;
				this.ClampMinMax();
			}
		}

		public float MaxAngle
		{
			get
			{
				return this._maxAngle;
			}
			set
			{
				this._maxAngle = value;
				this.ClampMinMax();
			}
		}

		private void ClampMinMax()
		{
			this._minAngle = Mathf.Clamp(this._minAngle, -90f, 90f);
			this._maxAngle = Mathf.Clamp(this._maxAngle, this._minAngle, 90f);
		}

		public bool ScaleWithRadius
		{
			get
			{
				return this._scaleWithRadius;
			}
			set
			{
				this._scaleWithRadius = value;
			}
		}

		public Vector3 RadiusToScaleRatio
		{
			get
			{
				return this._radiusToScaleRatio;
			}
			set
			{
				this._radiusToScaleRatio = value;
			}
		}

		public void Initialize(IGrabbable grabbable)
		{
			this._grabbable = grabbable;
			this.ClampMinMax();
		}

		public void BeginTransform()
		{
			Pose pose = this._grabbable.GrabPoints[0];
			Transform transform = this._grabbable.Transform;
			this._localToTransform = new Pose(transform.InverseTransformPoint(pose.position), Quaternion.Inverse(transform.rotation) * pose.rotation);
		}

		public void UpdateTransform()
		{
			ref Pose ptr = this._grabbable.GrabPoints[0];
			Transform transform = this._grabbable.Transform;
			Vector3 vector = ptr.position - this._sphereCenter.position;
			float sqrMagnitude = vector.sqrMagnitude;
			float num5;
			float num6;
			float d;
			if (this._scaleWithRadius)
			{
				Vector3 vector2 = new Vector3(this._localToTransform.position.x * this._radiusToScaleRatio.x, this._localToTransform.position.y * this._radiusToScaleRatio.y, 0f);
				float num = 1f - -this._localToTransform.position.z * this._radiusToScaleRatio.z;
				float num2 = num * num;
				float num3 = vector2.x * vector2.x / num2;
				float num4 = vector2.y * vector2.y / num2;
				num5 = sqrMagnitude / (1f + num3 + num4);
				num6 = Mathf.Sqrt(num5);
				d = num6 / (1f - -this._localToTransform.position.z * this._radiusToScaleRatio.z);
				Vector3 vector3 = (transform.parent != null) ? transform.parent.lossyScale : Vector3.one;
				Vector3 vector4 = d * this._radiusToScaleRatio;
				transform.localScale = new Vector3(vector4.x / vector3.x, vector4.y / vector3.y, vector4.z / vector3.z);
			}
			else
			{
				float sqrMagnitude2 = transform.TransformVector(new Vector3(this._localToTransform.position.x, this._localToTransform.position.y, 0f)).sqrMagnitude;
				num5 = sqrMagnitude - sqrMagnitude2;
				if (num5 <= 0f)
				{
					return;
				}
				num6 = Mathf.Sqrt(num5);
				float num7 = transform.TransformVector(new Vector3(0f, 0f, this._localToTransform.position.z)).magnitude;
				if (this._localToTransform.position.z < 0f)
				{
					num7 *= -1f;
				}
				d = num6 - num7;
			}
			float sqrMagnitude3 = transform.TransformVector(new Vector3(0f, this._localToTransform.position.y, 0f)).sqrMagnitude;
			float num8 = Mathf.Sqrt(sqrMagnitude3 + num5);
			float num9 = Mathf.Asin(Mathf.Clamp(vector.y / num8, -1f, 1f)) * 57.29578f;
			float num10 = Mathf.Sqrt(sqrMagnitude3);
			if (this._localToTransform.position.y < 0f)
			{
				num10 *= -1f;
			}
			float num11 = Mathf.Atan2(-num10, num6) * 57.29578f;
			num9 += num11;
			num9 = Mathf.Clamp(num9, this._minAngle, this._maxAngle);
			Quaternion quaternion = Quaternion.AngleAxis(-num9, Vector3.right);
			Pose pose = new Pose(quaternion * (d * Vector3.forward), quaternion);
			Vector3 lossyScale = transform.lossyScale;
			Vector3 point = new Vector3(lossyScale.x * this._localToTransform.position.x, lossyScale.y * this._localToTransform.position.y, lossyScale.z * this._localToTransform.position.z);
			Vector3 vector5 = pose.position + pose.rotation * point;
			Vector3 from = new Vector3(vector5.x, 0f, vector5.z);
			Vector3 to = new Vector3(vector.x, 0f, vector.z);
			quaternion = Quaternion.AngleAxis(Vector3.SignedAngle(from, to, Vector3.up), Vector3.up) * quaternion;
			transform.position = this._sphereCenter.position + quaternion * (d * Vector3.forward);
			transform.rotation = quaternion;
		}

		public void EndTransform()
		{
		}

		[SerializeField]
		private Transform _sphereCenter;

		[SerializeField]
		[Range(-90f, 90f)]
		private float _minAngle = -90f;

		[SerializeField]
		[Range(-90f, 90f)]
		private float _maxAngle = 90f;

		[SerializeField]
		private bool _scaleWithRadius;

		[SerializeField]
		private Vector3 _radiusToScaleRatio = new Vector3(1f, 1f, 1f);

		private IGrabbable _grabbable;

		private Pose _localToTransform;
	}
}
