using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Grab.GrabSurfaces
{
	[Serializable]
	public class CylinderGrabSurface : MonoBehaviour, IGrabSurface
	{
		private Pose RelativePose
		{
			get
			{
				return PoseUtils.DeltaScaled(this._relativeTo, base.transform);
			}
		}

		public Pose GetReferencePose(Transform relativeTo)
		{
			return PoseUtils.GlobalPoseScaled(relativeTo, this.RelativePose);
		}

		public float ArcOffset
		{
			get
			{
				return this._data.arcOffset;
			}
			set
			{
				if (value != 0f && value % 360f == 0f)
				{
					this._data.arcOffset = 360f;
					return;
				}
				this._data.arcOffset = Mathf.Repeat(value, 360f);
			}
		}

		public float ArcLength
		{
			get
			{
				return this._data.arcLength;
			}
			set
			{
				if (value != 0f && value % 360f == 0f)
				{
					this._data.arcLength = 360f;
					return;
				}
				this._data.arcLength = Mathf.Repeat(value, 360f);
			}
		}

		private Vector3 LocalPerpendicularDir
		{
			get
			{
				return Vector3.ProjectOnPlane(this.RelativePose.position - this._data.startPoint, this.LocalDirection).normalized;
			}
		}

		private Vector3 LocalDirection
		{
			get
			{
				Vector3 vector = this._data.endPoint - this._data.startPoint;
				if (vector.sqrMagnitude <= 1E-06f)
				{
					return Vector3.up;
				}
				return vector.normalized;
			}
		}

		public Vector3 GetPerpendicularDir(Transform relativeTo)
		{
			return relativeTo.TransformDirection(this.LocalPerpendicularDir);
		}

		public Vector3 GetStartArcDir(Transform relativeTo)
		{
			Vector3 direction = Quaternion.AngleAxis(this.ArcOffset, this.LocalDirection) * this.LocalPerpendicularDir;
			return relativeTo.TransformDirection(direction);
		}

		public Vector3 GetEndArcDir(Transform relativeTo)
		{
			Vector3 direction = Quaternion.AngleAxis(this.ArcLength, this.LocalDirection) * Quaternion.AngleAxis(this.ArcOffset, this.LocalDirection) * this.LocalPerpendicularDir;
			return relativeTo.TransformDirection(direction);
		}

		public Vector3 GetStartPoint(Transform relativeTo)
		{
			return relativeTo.TransformPoint(this._data.startPoint);
		}

		public void SetStartPoint(Vector3 point, Transform relativeTo)
		{
			this._data.startPoint = relativeTo.InverseTransformPoint(point);
		}

		public Vector3 GetEndPoint(Transform relativeTo)
		{
			return relativeTo.TransformPoint(this._data.endPoint);
		}

		public void SetEndPoint(Vector3 point, Transform relativeTo)
		{
			this._data.endPoint = relativeTo.InverseTransformPoint(point);
		}

		public float GetRadius(Transform relativeTo)
		{
			Vector3 startPoint = this.GetStartPoint(relativeTo);
			Pose referencePose = this.GetReferencePose(relativeTo);
			Vector3 direction = this.GetDirection(relativeTo);
			return Vector3.Distance(startPoint + Vector3.Project(referencePose.position - startPoint, direction), referencePose.position);
		}

		public Vector3 GetDirection(Transform relativeTo)
		{
			return relativeTo.TransformDirection(this.LocalDirection);
		}

		private float GetHeight(Transform relativeTo)
		{
			Vector3 startPoint = this.GetStartPoint(relativeTo);
			Vector3 endPoint = this.GetEndPoint(relativeTo);
			return Vector3.Distance(startPoint, endPoint);
		}

		private Quaternion GetRotation(Transform relativeTo)
		{
			if (this._data.startPoint == this._data.endPoint)
			{
				return relativeTo.rotation;
			}
			return relativeTo.rotation * Quaternion.LookRotation(this.LocalPerpendicularDir, this.LocalDirection);
		}

		protected virtual void Reset()
		{
			IRelativeToRef componentInParent = base.GetComponentInParent<IRelativeToRef>();
			this._relativeTo = ((componentInParent != null) ? componentInParent.RelativeTo : null);
		}

		protected virtual void Start()
		{
		}

		public Pose MirrorPose(in Pose pose, Transform relativeTo)
		{
			Vector3 normalized = Vector3.Cross(this.LocalPerpendicularDir, this.LocalDirection).normalized;
			Quaternion rotation = HandMirroring.Reflect(pose.rotation, normalized);
			return new Pose(pose.position, rotation);
		}

		private Vector3 PointAltitude(Vector3 point, Transform relativeTo)
		{
			Vector3 startPoint = this.GetStartPoint(relativeTo);
			Vector3 direction = this.GetDirection(relativeTo);
			return startPoint + Vector3.Project(point - startPoint, direction);
		}

		public GrabPoseScore CalculateBestPoseAtSurface(in Pose targetPose, out Pose bestPose, in PoseMeasureParameters scoringModifier, Transform relativeTo)
		{
			Pose identity = Pose.identity;
			return this.CalculateBestPoseAtSurface(targetPose, identity, out bestPose, scoringModifier, relativeTo);
		}

		public GrabPoseScore CalculateBestPoseAtSurface(in Pose targetPose, in Pose offset, out Pose bestPose, in PoseMeasureParameters scoringModifier, Transform relativeTo)
		{
			return GrabPoseHelper.CalculateBestPoseAtSurface(targetPose, offset, out bestPose, scoringModifier, relativeTo, new GrabPoseHelper.PoseCalculator(this.MinimalTranslationPoseAtSurface), new GrabPoseHelper.PoseCalculator(this.MinimalRotationPoseAtSurface));
		}

		public IGrabSurface CreateMirroredSurface(GameObject gameObject)
		{
			CylinderGrabSurface cylinderGrabSurface = gameObject.AddComponent<CylinderGrabSurface>();
			cylinderGrabSurface._data = this._data.Mirror();
			return cylinderGrabSurface;
		}

		public IGrabSurface CreateDuplicatedSurface(GameObject gameObject)
		{
			CylinderGrabSurface cylinderGrabSurface = gameObject.AddComponent<CylinderGrabSurface>();
			cylinderGrabSurface._data = (this._data.Clone() as CylinderSurfaceData);
			return cylinderGrabSurface;
		}

		protected Vector3 NearestPointInSurface(Vector3 targetPosition, Transform relativeTo)
		{
			Vector3 startPoint = this.GetStartPoint(relativeTo);
			Vector3 direction = this.GetDirection(relativeTo);
			Vector3 vector = Vector3.Project(targetPosition - startPoint, direction);
			float height = this.GetHeight(relativeTo);
			if (vector.magnitude > height)
			{
				vector = vector.normalized * height;
			}
			if (Vector3.Dot(vector, direction) < 0f)
			{
				vector = Vector3.zero;
			}
			Vector3 vector2 = startPoint + vector;
			Vector3 vector3 = Vector3.ProjectOnPlane(targetPosition - vector2, direction).normalized;
			Vector3 startArcDir = this.GetStartArcDir(relativeTo);
			float num = Mathf.Repeat(Vector3.SignedAngle(startArcDir, vector3, direction), 360f);
			if (num > this.ArcLength)
			{
				if (Mathf.Abs(num - this.ArcLength) >= Mathf.Abs(360f - num))
				{
					vector3 = startArcDir;
				}
				else
				{
					vector3 = this.GetEndArcDir(relativeTo);
				}
			}
			return vector2 + vector3 * this.GetRadius(relativeTo);
		}

		public bool CalculateBestPoseAtSurface(Ray targetRay, out Pose bestPose, Transform relativeTo)
		{
			Pose referencePose = this.GetReferencePose(relativeTo);
			Vector3 startPoint = this.GetStartPoint(relativeTo);
			Vector3 direction = this.GetDirection(relativeTo);
			Vector3 lhs = startPoint - targetRay.origin;
			float num = Vector3.Dot(targetRay.direction, direction);
			float num2 = Vector3.Dot(lhs, targetRay.direction);
			float num3 = Vector3.Dot(lhs, direction);
			float num4 = 1f / (num * num - 1f);
			float d = (num * num3 - num2) * num4;
			float d2 = (num3 - num * num2) * num4;
			float radius = this.GetRadius(relativeTo);
			Vector3 vector = targetRay.origin + targetRay.direction * d;
			Vector3 a = startPoint + direction * d2;
			float num5 = Mathf.Max(new float[]
			{
				Vector3.Distance(a, vector) - radius
			});
			if (num5 < radius)
			{
				float d3 = Mathf.Sqrt(radius * radius - num5 * num5);
				vector -= targetRay.direction * d3;
			}
			Vector3 position = this.NearestPointInSurface(vector, relativeTo);
			Pose pose = new Pose(position, referencePose.rotation);
			bestPose = this.MinimalTranslationPoseAtSurface(pose, relativeTo);
			return true;
		}

		protected Pose MinimalRotationPoseAtSurface(in Pose userPose, Transform relativeTo)
		{
			Pose referencePose = this.GetReferencePose(relativeTo);
			Vector3 direction = this.GetDirection(relativeTo);
			Quaternion rotation = this.GetRotation(relativeTo);
			float radius = this.GetRadius(relativeTo);
			Vector3 position = userPose.position;
			Quaternion rotation2 = userPose.rotation;
			Quaternion rotation3 = referencePose.rotation;
			Vector3 normalized = Vector3.ProjectOnPlane(rotation2 * Quaternion.Inverse(rotation3) * rotation * Vector3.forward, direction).normalized;
			Vector3 a = this.PointAltitude(position, relativeTo);
			Vector3 vector = this.NearestPointInSurface(a + normalized * radius, relativeTo);
			Quaternion rotation4 = this.CalculateRotationOffset(vector, relativeTo) * rotation3;
			return new Pose(vector, rotation4);
		}

		protected Pose MinimalTranslationPoseAtSurface(in Pose userPose, Transform relativeTo)
		{
			ref Pose referencePose = this.GetReferencePose(relativeTo);
			Vector3 position = userPose.position;
			Quaternion rotation = referencePose.rotation;
			Vector3 vector = this.NearestPointInSurface(position, relativeTo);
			Quaternion rotation2 = this.CalculateRotationOffset(vector, relativeTo) * rotation;
			return new Pose(vector, rotation2);
		}

		protected Quaternion CalculateRotationOffset(Vector3 surfacePoint, Transform relativeTo)
		{
			Vector3 startPoint = this.GetStartPoint(relativeTo);
			Vector3 direction = this.GetDirection(relativeTo);
			Vector3 fromDirection = Vector3.ProjectOnPlane(this.GetPerpendicularDir(relativeTo), direction);
			Vector3 toDirection = Vector3.ProjectOnPlane(surfacePoint - startPoint, direction);
			return Quaternion.FromToRotation(fromDirection, toDirection);
		}

		public void InjectAllCylinderSurface(CylinderSurfaceData data, Transform relativeTo)
		{
			this.InjectData(data);
			this.InjectRelativeTo(relativeTo);
		}

		public void InjectData(CylinderSurfaceData data)
		{
			this._data = data;
		}

		public void InjectRelativeTo(Transform relativeTo)
		{
			this._relativeTo = relativeTo;
		}

		GrabPoseScore IGrabSurface.CalculateBestPoseAtSurface(in Pose targetPose, out Pose bestPose, in PoseMeasureParameters scoringModifier, Transform relativeTo)
		{
			return this.CalculateBestPoseAtSurface(targetPose, out bestPose, scoringModifier, relativeTo);
		}

		GrabPoseScore IGrabSurface.CalculateBestPoseAtSurface(in Pose targetPose, in Pose offset, out Pose bestPose, in PoseMeasureParameters scoringModifier, Transform relativeTo)
		{
			return this.CalculateBestPoseAtSurface(targetPose, offset, out bestPose, scoringModifier, relativeTo);
		}

		Pose IGrabSurface.MirrorPose(in Pose gripPose, Transform relativeTo)
		{
			return this.MirrorPose(gripPose, relativeTo);
		}

		[SerializeField]
		protected CylinderSurfaceData _data = new CylinderSurfaceData();

		[SerializeField]
		[Tooltip("Transform used as a reference to measure the local data of the grab surface")]
		private Transform _relativeTo;

		private const float Epsilon = 1E-06f;
	}
}
