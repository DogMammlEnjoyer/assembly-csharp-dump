using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Grab.GrabSurfaces
{
	[Serializable]
	public class BoxGrabSurface : MonoBehaviour, IGrabSurface
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

		public float GetWidthOffset(Transform relativeTo)
		{
			return this._data.widthOffset * relativeTo.lossyScale.x;
		}

		public void SetWidthOffset(float widthOffset, Transform relativeTo)
		{
			this._data.widthOffset = widthOffset / relativeTo.lossyScale.x;
		}

		public Vector4 GetSnapOffset(Transform relativeTo)
		{
			return this._data.snapOffset * relativeTo.lossyScale.x;
		}

		public void SetSnapOffset(Vector4 snapOffset, Transform relativeTo)
		{
			this._data.snapOffset = snapOffset / relativeTo.lossyScale.x;
		}

		public Vector3 GetSize(Transform relativeTo)
		{
			return this._data.size * relativeTo.lossyScale.x;
		}

		public void SetSize(Vector3 size, Transform relativeTo)
		{
			this._data.size = size / relativeTo.lossyScale.x;
		}

		public Quaternion GetRotation(Transform relativeTo)
		{
			return relativeTo.rotation * Quaternion.Euler(this._data.eulerAngles);
		}

		public void SetRotation(Quaternion rotation, Transform relativeTo)
		{
			this._data.eulerAngles = (Quaternion.Inverse(relativeTo.rotation) * rotation).eulerAngles;
		}

		public Vector3 GetDirection(Transform relativeTo)
		{
			return this.GetRotation(relativeTo) * Vector3.forward;
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
			Vector3 normal = Quaternion.Euler(this._data.eulerAngles) * Vector3.right;
			Quaternion rotation = HandMirroring.Reflect(pose.rotation, normal);
			return new Pose(pose.position, rotation);
		}

		public IGrabSurface CreateMirroredSurface(GameObject gameObject)
		{
			BoxGrabSurface boxGrabSurface = gameObject.AddComponent<BoxGrabSurface>();
			boxGrabSurface._data = this._data.Mirror();
			return boxGrabSurface;
		}

		public IGrabSurface CreateDuplicatedSurface(GameObject gameObject)
		{
			BoxGrabSurface boxGrabSurface = gameObject.AddComponent<BoxGrabSurface>();
			boxGrabSurface._data = this._data;
			return boxGrabSurface;
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

		private void CalculateCorners(out Vector3 bottomLeft, out Vector3 bottomRight, out Vector3 topLeft, out Vector3 topRight, Transform relativeTo)
		{
			Pose referencePose = this.GetReferencePose(relativeTo);
			Vector3 size = this.GetSize(relativeTo);
			float widthOffset = this.GetWidthOffset(relativeTo);
			Vector3 a = this.GetRotation(relativeTo) * Vector3.right;
			bottomLeft = referencePose.position - a * size.x * (1f - widthOffset);
			bottomRight = referencePose.position + a * size.x * widthOffset;
			Vector3 b = this.GetRotation(relativeTo) * Vector3.forward * size.z;
			topLeft = bottomLeft + b;
			topRight = bottomRight + b;
		}

		private Vector3 ProjectOnSegment(Vector3 point, ValueTuple<Vector3, Vector3> segment)
		{
			Vector3 vector = segment.Item2 - segment.Item1;
			Vector3 vector2 = Vector3.Project(point - segment.Item1, vector);
			if (Vector3.Dot(vector2, vector) < 0f)
			{
				vector2 = segment.Item1;
			}
			else if (vector2.magnitude > vector.magnitude)
			{
				vector2 = segment.Item2;
			}
			else
			{
				vector2 += segment.Item1;
			}
			return vector2;
		}

		public bool CalculateBestPoseAtSurface(Ray targetRay, out Pose bestPose, Transform relativeTo)
		{
			Pose referencePose = this.GetReferencePose(relativeTo);
			Plane plane = new Plane(this.GetRotation(relativeTo) * Vector3.up, base.transform.position);
			float d;
			plane.Raycast(targetRay, out d);
			Vector3 targetPosition = targetRay.origin + targetRay.direction * d;
			Vector3 position = this.NearestPointInSurface(targetPosition, relativeTo);
			Pose pose = new Pose(position, referencePose.rotation);
			bestPose = this.MinimalTranslationPoseAtSurface(pose, relativeTo);
			return true;
		}

		protected Vector3 NearestPointInSurface(Vector3 targetPosition, Transform relativeTo)
		{
			Vector3 result;
			float num;
			this.NearestPointAndAngleInSurface(targetPosition, out result, out num, relativeTo);
			return result;
		}

		private void NearestPointAndAngleInSurface(Vector3 targetPosition, out Vector3 surfacePoint, out float angle, Transform relativeTo)
		{
			Quaternion rotation = this.GetRotation(relativeTo);
			Vector4 snapOffset = this.GetSnapOffset(relativeTo);
			Vector3 a = rotation * Vector3.right;
			Vector3 a2 = rotation * Vector3.forward;
			Vector3 a3;
			Vector3 a4;
			Vector3 a5;
			Vector3 a6;
			this.CalculateCorners(out a3, out a4, out a5, out a6, relativeTo);
			Vector3 vector = this.ProjectOnSegment(targetPosition, new ValueTuple<Vector3, Vector3>(a3 + a * snapOffset.y, a4 + a * snapOffset.x));
			Vector3 vector2 = this.ProjectOnSegment(targetPosition, new ValueTuple<Vector3, Vector3>(a5 - a * snapOffset.x, a6 - a * snapOffset.y));
			Vector3 vector3 = this.ProjectOnSegment(targetPosition, new ValueTuple<Vector3, Vector3>(a3 - a2 * snapOffset.z, a5 - a2 * snapOffset.w));
			Vector3 vector4 = this.ProjectOnSegment(targetPosition, new ValueTuple<Vector3, Vector3>(a4 + a2 * snapOffset.w, a6 + a2 * snapOffset.z));
			float sqrMagnitude = (vector - targetPosition).sqrMagnitude;
			float sqrMagnitude2 = (vector2 - targetPosition).sqrMagnitude;
			float sqrMagnitude3 = (vector3 - targetPosition).sqrMagnitude;
			float sqrMagnitude4 = (vector4 - targetPosition).sqrMagnitude;
			float num = Mathf.Min(sqrMagnitude, Mathf.Min(sqrMagnitude2, Mathf.Min(sqrMagnitude3, sqrMagnitude4)));
			if (sqrMagnitude == num)
			{
				surfacePoint = vector;
				angle = 0f;
				return;
			}
			if (sqrMagnitude2 == num)
			{
				surfacePoint = vector2;
				angle = 180f;
				return;
			}
			if (sqrMagnitude3 == num)
			{
				surfacePoint = vector3;
				angle = 90f;
				return;
			}
			surfacePoint = vector4;
			angle = -90f;
		}

		protected Pose MinimalRotationPoseAtSurface(in Pose userPose, Transform relativeTo)
		{
			Quaternion rotation = this.GetRotation(relativeTo);
			ref Pose referencePose = this.GetReferencePose(relativeTo);
			Vector4 snapOffset = this.GetSnapOffset(relativeTo);
			Vector3 position = userPose.position;
			Quaternion rotation2 = referencePose.rotation;
			Quaternion rotation3 = userPose.rotation;
			Vector3 axis = rotation * Vector3.up;
			Quaternion rotation4 = rotation2;
			Quaternion rotation5 = Quaternion.AngleAxis(180f, axis) * rotation2;
			Quaternion rotation6 = Quaternion.AngleAxis(90f, axis) * rotation2;
			Quaternion rotation7 = Quaternion.AngleAxis(-90f, axis) * rotation2;
			float num = BoxGrabSurface.RotationalScore(rotation4, rotation3);
			float num2 = BoxGrabSurface.RotationalScore(rotation5, rotation3);
			float num3 = BoxGrabSurface.RotationalScore(rotation6, rotation3);
			float b = BoxGrabSurface.RotationalScore(rotation7, rotation3);
			Vector3 a = rotation * Vector3.right;
			Vector3 a2 = rotation * Vector3.forward;
			Vector3 a3;
			Vector3 a4;
			Vector3 a5;
			Vector3 a6;
			this.CalculateCorners(out a3, out a4, out a5, out a6, relativeTo);
			float num4 = Mathf.Max(num, Mathf.Max(num2, Mathf.Max(num3, b)));
			if (num == num4)
			{
				return new Pose(this.ProjectOnSegment(position, new ValueTuple<Vector3, Vector3>(a3 + a * snapOffset.y, a4 + a * snapOffset.x)), rotation4);
			}
			if (num2 == num4)
			{
				return new Pose(this.ProjectOnSegment(position, new ValueTuple<Vector3, Vector3>(a5 - a * snapOffset.x, a6 - a * snapOffset.y)), rotation5);
			}
			if (num3 == num4)
			{
				return new Pose(this.ProjectOnSegment(position, new ValueTuple<Vector3, Vector3>(a3 - a2 * snapOffset.z, a5 - a2 * snapOffset.w)), rotation6);
			}
			return new Pose(this.ProjectOnSegment(position, new ValueTuple<Vector3, Vector3>(a4 + a2 * snapOffset.w, a6 + a2 * snapOffset.z)), rotation7);
		}

		protected Pose MinimalTranslationPoseAtSurface(in Pose userPose, Transform relativeTo)
		{
			ref Pose referencePose = this.GetReferencePose(relativeTo);
			Quaternion rotation = this.GetRotation(relativeTo);
			Vector3 position = userPose.position;
			Quaternion rotation2 = referencePose.rotation;
			Vector3 position2;
			float angle;
			this.NearestPointAndAngleInSurface(position, out position2, out angle, relativeTo);
			Quaternion rotation3 = Quaternion.AngleAxis(angle, rotation * Vector3.up) * rotation2;
			return new Pose(position2, rotation3);
		}

		private static float RotationalScore(in Quaternion from, in Quaternion to)
		{
			float num = Vector3.Dot(from * Vector3.forward, to * Vector3.forward) * 0.5f + 0.5f;
			float num2 = Vector3.Dot(from * Vector3.up, to * Vector3.up) * 0.5f + 0.5f;
			return num * num2;
		}

		public void InjectAllBoxSurface(BoxGrabSurfaceData data, Transform relativeTo)
		{
			this.InjectData(data);
			this.InjectRelativeTo(relativeTo);
		}

		public void InjectData(BoxGrabSurfaceData data)
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
		protected BoxGrabSurfaceData _data = new BoxGrabSurfaceData();

		[SerializeField]
		[Tooltip("Transform used as a reference to measure the local data of the grab surface")]
		private Transform _relativeTo;
	}
}
