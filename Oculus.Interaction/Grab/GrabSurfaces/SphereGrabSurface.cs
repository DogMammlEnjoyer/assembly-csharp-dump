using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Grab.GrabSurfaces
{
	[Serializable]
	public class SphereGrabSurface : MonoBehaviour, IGrabSurface
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

		public Vector3 GetCentre(Transform relativeTo)
		{
			return relativeTo.TransformPoint(this._data.centre);
		}

		public void SetCentre(Vector3 point, Transform relativeTo)
		{
			this._data.centre = relativeTo.InverseTransformPoint(point);
		}

		public float GetRadius(Transform relativeTo)
		{
			Vector3 centre = this.GetCentre(relativeTo);
			Pose referencePose = this.GetReferencePose(relativeTo);
			return Vector3.Distance(centre, referencePose.position);
		}

		public Vector3 GetDirection(Transform relativeTo)
		{
			Vector3 centre = this.GetCentre(relativeTo);
			return (this.GetReferencePose(relativeTo).position - centre).normalized;
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
			Vector3 normalized = Vector3.Cross(pose.position, Vector3.up).normalized;
			Quaternion rotation = HandMirroring.Reflect(pose.rotation, normalized);
			return new Pose(pose.position, rotation);
		}

		public bool CalculateBestPoseAtSurface(Ray targetRay, out Pose bestPose, Transform relativeTo)
		{
			Vector3 centre = this.GetCentre(relativeTo);
			Vector3 b = Vector3.Project(centre - targetRay.origin, targetRay.direction);
			Vector3 vector = targetRay.origin + b;
			float radius = this.GetRadius(relativeTo);
			float num = Mathf.Max(new float[]
			{
				Vector3.Distance(centre, vector) - radius
			});
			if (num < radius)
			{
				float d = Mathf.Sqrt(radius * radius - num * num);
				vector -= targetRay.direction * d;
			}
			Pose referencePose = this.GetReferencePose(relativeTo);
			Vector3 position = this.NearestPointInSurface(vector, relativeTo);
			Pose pose = new Pose(position, referencePose.rotation);
			bestPose = this.MinimalTranslationPoseAtSurface(pose, relativeTo);
			return true;
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
			SphereGrabSurface sphereGrabSurface = gameObject.AddComponent<SphereGrabSurface>();
			sphereGrabSurface._data = this._data.Mirror();
			return sphereGrabSurface;
		}

		public IGrabSurface CreateDuplicatedSurface(GameObject gameObject)
		{
			SphereGrabSurface sphereGrabSurface = gameObject.AddComponent<SphereGrabSurface>();
			sphereGrabSurface._data = this._data;
			return sphereGrabSurface;
		}

		protected Vector3 NearestPointInSurface(Vector3 targetPosition, Transform relativeTo)
		{
			Vector3 centre = this.GetCentre(relativeTo);
			Vector3 normalized = (targetPosition - centre).normalized;
			float radius = this.GetRadius(relativeTo);
			return centre + normalized * radius;
		}

		protected Pose MinimalRotationPoseAtSurface(in Pose userPose, Transform relativeTo)
		{
			Vector3 centre = this.GetCentre(relativeTo);
			Pose referencePose = this.GetReferencePose(relativeTo);
			float radius = this.GetRadius(relativeTo);
			Vector3 a = userPose.rotation * Quaternion.Inverse(referencePose.rotation) * this.GetDirection(relativeTo);
			Vector3 vector = this.NearestPointInSurface(centre + a * radius, relativeTo);
			Quaternion rotation = this.RotationAtPoint(vector, referencePose.rotation, userPose.rotation, relativeTo);
			return new Pose(vector, rotation);
		}

		protected Pose MinimalTranslationPoseAtSurface(in Pose userPose, Transform relativeTo)
		{
			ref Pose referencePose = this.GetReferencePose(relativeTo);
			Vector3 position = userPose.position;
			Quaternion rotation = referencePose.rotation;
			Vector3 vector = this.NearestPointInSurface(position, relativeTo);
			Quaternion rotation2 = this.RotationAtPoint(vector, rotation, userPose.rotation, relativeTo);
			return new Pose(vector, rotation2);
		}

		protected Quaternion RotationAtPoint(Vector3 surfacePoint, Quaternion baseRot, Quaternion desiredRotation, Transform relativeTo)
		{
			Vector3 normalized = (surfacePoint - this.GetCentre(relativeTo)).normalized;
			Quaternion quaternion = Quaternion.FromToRotation(this.GetDirection(relativeTo), normalized) * baseRot;
			Vector3 normalized2 = Vector3.ProjectOnPlane(quaternion * Vector3.forward, normalized).normalized;
			Vector3 normalized3 = Vector3.ProjectOnPlane(desiredRotation * Vector3.forward, normalized).normalized;
			return Quaternion.FromToRotation(normalized2, normalized3) * quaternion;
		}

		public void InjectAllSphereSurface(SphereGrabSurfaceData data, Transform relativeTo)
		{
			this.InjectData(data);
			this.InjectRelativeTo(relativeTo);
		}

		public void InjectData(SphereGrabSurfaceData data)
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
		protected SphereGrabSurfaceData _data = new SphereGrabSurfaceData();

		[SerializeField]
		[Tooltip("Transform used as a reference to measure the local data of the grab surface")]
		private Transform _relativeTo;
	}
}
