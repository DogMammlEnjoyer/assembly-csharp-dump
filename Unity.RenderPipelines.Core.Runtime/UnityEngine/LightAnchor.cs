using System;

namespace UnityEngine
{
	[AddComponentMenu("Rendering/Light Anchor")]
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class LightAnchor : MonoBehaviour
	{
		public float yaw
		{
			get
			{
				return this.m_Yaw;
			}
			set
			{
				this.m_Yaw = LightAnchor.NormalizeAngleDegree(value);
			}
		}

		public float pitch
		{
			get
			{
				return this.m_Pitch;
			}
			set
			{
				this.m_Pitch = LightAnchor.NormalizeAngleDegree(value);
			}
		}

		public float roll
		{
			get
			{
				return this.m_Roll;
			}
			set
			{
				this.m_Roll = LightAnchor.NormalizeAngleDegree(value);
			}
		}

		public float distance
		{
			get
			{
				return this.m_Distance;
			}
			set
			{
				this.m_Distance = Mathf.Clamp(value, 0f, 10000f);
			}
		}

		public LightAnchor.UpDirection frameSpace
		{
			get
			{
				return this.m_FrameSpace;
			}
			set
			{
				this.m_FrameSpace = value;
			}
		}

		public Vector3 anchorPosition
		{
			get
			{
				if (this.anchorPositionOverride != null)
				{
					return this.anchorPositionOverride.position + this.anchorPositionOverride.TransformDirection(this.anchorPositionOffset);
				}
				return base.transform.position + base.transform.forward * this.distance;
			}
		}

		public Transform anchorPositionOverride
		{
			get
			{
				return this.m_AnchorPositionOverride;
			}
			set
			{
				this.m_AnchorPositionOverride = value;
			}
		}

		public Vector3 anchorPositionOffset
		{
			get
			{
				return this.m_AnchorPositionOffset;
			}
			set
			{
				this.m_AnchorPositionOffset = value;
			}
		}

		public static float NormalizeAngleDegree(float angle)
		{
			float num = angle - -180f;
			return num - Mathf.Floor(num / 360f) * 360f + -180f;
		}

		public void SynchronizeOnTransform(Camera camera)
		{
			LightAnchor.Axes worldSpaceAxes = this.GetWorldSpaceAxes(camera, this.anchorPosition);
			Vector3 vector = base.transform.position - this.anchorPosition;
			if (vector.magnitude == 0f)
			{
				vector = -base.transform.forward;
			}
			Vector3 vector2 = Vector3.ProjectOnPlane(vector, worldSpaceAxes.up);
			if (vector2.magnitude < 0.0001f)
			{
				vector2 = Vector3.ProjectOnPlane(vector, worldSpaceAxes.up + worldSpaceAxes.right * 0.0001f);
			}
			vector2.Normalize();
			float num = Vector3.SignedAngle(worldSpaceAxes.forward, vector2, worldSpaceAxes.up);
			Vector3 axis = Quaternion.AngleAxis(num, worldSpaceAxes.up) * worldSpaceAxes.right;
			float pitch = Vector3.SignedAngle(vector2, vector, axis);
			this.yaw = num;
			this.pitch = pitch;
			this.roll = base.transform.rotation.eulerAngles.z;
		}

		public void UpdateTransform(Camera camera, Vector3 anchor)
		{
			LightAnchor.Axes worldSpaceAxes = this.GetWorldSpaceAxes(camera, anchor);
			this.UpdateTransform(worldSpaceAxes.up, worldSpaceAxes.right, worldSpaceAxes.forward, anchor);
		}

		private LightAnchor.Axes GetWorldSpaceAxes(Camera camera, Vector3 anchor)
		{
			if (base.transform.IsChildOf(camera.transform))
			{
				return new LightAnchor.Axes
				{
					up = Vector3.up,
					right = Vector3.right,
					forward = Vector3.forward
				};
			}
			Matrix4x4 lhs = camera.cameraToWorldMatrix;
			if (this.m_FrameSpace == LightAnchor.UpDirection.Local)
			{
				Vector3 up = Camera.main.transform.up;
				lhs = (Matrix4x4.Scale(new Vector3(1f, 1f, -1f)) * Matrix4x4.LookAt(camera.transform.position, anchor, up).inverse).inverse;
			}
			else if (!camera.orthographic && camera.transform.position != anchor)
			{
				Quaternion q = Quaternion.LookRotation((anchor - camera.transform.position).normalized);
				lhs = (Matrix4x4.Scale(new Vector3(1f, 1f, -1f)) * Matrix4x4.TRS(camera.transform.position, q, Vector3.one).inverse).inverse;
			}
			Vector3 up2 = (lhs * Vector3.up).normalized;
			Vector3 right = (lhs * Vector3.right).normalized;
			Vector3 forward = (lhs * Vector3.forward).normalized;
			return new LightAnchor.Axes
			{
				up = up2,
				right = right,
				forward = forward
			};
		}

		private void Update()
		{
			if (this.anchorPositionOverride == null || Camera.main == null)
			{
				return;
			}
			if (this.anchorPositionOverride.hasChanged || Camera.main.transform.hasChanged)
			{
				this.UpdateTransform(Camera.main, this.anchorPosition);
			}
		}

		private void OnDrawGizmosSelected()
		{
			Camera main = Camera.main;
			if (main == null)
			{
				return;
			}
			Vector3 anchorPosition = this.anchorPosition;
			LightAnchor.Axes worldSpaceAxes = this.GetWorldSpaceAxes(main, anchorPosition);
			Vector3.ProjectOnPlane(base.transform.position - anchorPosition, worldSpaceAxes.up);
			Mathf.Min(this.distance * 0.25f, 5f);
			Mathf.Min(this.distance * 0.5f, 10f);
		}

		private void UpdateTransform(Vector3 up, Vector3 right, Vector3 forward, Vector3 anchor)
		{
			Quaternion lhs = Quaternion.AngleAxis(this.m_Yaw, up);
			Quaternion rhs = Quaternion.AngleAxis(this.m_Pitch, right);
			Vector3 position = anchor + lhs * rhs * forward * this.distance;
			base.transform.position = position;
			Vector3 eulerAngles = Quaternion.LookRotation(-(lhs * rhs * forward).normalized, up).eulerAngles;
			eulerAngles.z = this.m_Roll;
			base.transform.eulerAngles = eulerAngles;
		}

		private const float k_ArcRadius = 5f;

		private const float k_AxisLength = 10f;

		internal const float k_MaxDistance = 10000f;

		[SerializeField]
		[Min(0f)]
		private float m_Distance;

		[SerializeField]
		private LightAnchor.UpDirection m_FrameSpace;

		[SerializeField]
		private Transform m_AnchorPositionOverride;

		[SerializeField]
		private Vector3 m_AnchorPositionOffset;

		[SerializeField]
		private float m_Yaw;

		[SerializeField]
		private float m_Pitch;

		[SerializeField]
		private float m_Roll;

		public enum UpDirection
		{
			World,
			Local
		}

		private struct Axes
		{
			public Vector3 up;

			public Vector3 right;

			public Vector3 forward;
		}
	}
}
