using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Grab.GrabSurfaces
{
	[Serializable]
	public class BezierGrabSurface : MonoBehaviour, IGrabSurface
	{
		public List<BezierControlPoint> ControlPoints
		{
			get
			{
				return this._controlPoints;
			}
		}

		protected virtual void Reset()
		{
			IRelativeToRef componentInParent = base.GetComponentInParent<IRelativeToRef>();
			this._relativeTo = ((componentInParent != null) ? componentInParent.RelativeTo : null);
		}

		protected virtual void Start()
		{
		}

		public GrabPoseScore CalculateBestPoseAtSurface(in Pose targetPose, out Pose bestPose, in PoseMeasureParameters scoringModifier, Transform relativeTo)
		{
			Pose identity = Pose.identity;
			return this.CalculateBestPoseAtSurface(targetPose, identity, out bestPose, scoringModifier, relativeTo);
		}

		public GrabPoseScore CalculateBestPoseAtSurface(in Pose targetPose, in Pose offset, out Pose bestPose, in PoseMeasureParameters scoringModifier, Transform relativeTo)
		{
			Pose identity = Pose.identity;
			Pose identity2 = Pose.identity;
			bestPose = targetPose;
			GrabPoseScore grabPoseScore = GrabPoseScore.Max;
			for (int i = 0; i < this._controlPoints.Count; i++)
			{
				BezierControlPoint bezierControlPoint = this._controlPoints[i];
				BezierControlPoint bezierControlPoint2 = this._controlPoints[(i + 1) % this._controlPoints.Count];
				if (bezierControlPoint.Disconnected || !bezierControlPoint2.Disconnected)
				{
					GrabPoseScore grabPoseScore2;
					if ((bezierControlPoint.Disconnected && bezierControlPoint2.Disconnected) || this._controlPoints.Count == 1)
					{
						Pose pose = bezierControlPoint.GetPose(relativeTo2);
						ref identity.CopyFrom(pose);
						grabPoseScore2 = new GrabPoseScore(ref targetPose, ref identity, ref offset, scoringModifier);
					}
					else
					{
						Pose start = bezierControlPoint.GetPose(relativeTo2);
						Pose end = bezierControlPoint2.GetPose(relativeTo2);
						Vector3 tangent = bezierControlPoint.GetTangent(relativeTo2);
						float positionT;
						this.NearestPointInTriangle(targetPose.position, start.position, tangent, end.position, out positionT);
						float rotationT = this.ProgressForRotation(targetPose.rotation, start.rotation, end.rotation);
						grabPoseScore2 = GrabPoseHelper.CalculateBestPoseAtSurface(targetPose, offset, out identity, scoringModifier, relativeTo2, delegate(in Pose target, Transform relativeTo)
						{
							Pose result;
							result.position = BezierGrabSurface.EvaluateBezier(start.position, tangent, end.position, positionT);
							result.rotation = Quaternion.Slerp(start.rotation, end.rotation, positionT);
							return result;
						}, delegate(in Pose target, Transform relativeTo)
						{
							Pose result;
							result.position = BezierGrabSurface.EvaluateBezier(start.position, tangent, end.position, rotationT);
							result.rotation = Quaternion.Slerp(start.rotation, end.rotation, rotationT);
							return result;
						});
					}
					if (grabPoseScore2.IsBetterThan(grabPoseScore))
					{
						grabPoseScore = grabPoseScore2;
						ref bestPose.CopyFrom(identity);
					}
				}
			}
			return grabPoseScore;
		}

		public bool CalculateBestPoseAtSurface(Ray targetRay, out Pose bestPose, Transform relativeTo)
		{
			Pose identity = Pose.identity;
			Pose identity2 = Pose.identity;
			bestPose = Pose.identity;
			bool result = false;
			GrabPoseScore referenceScore = GrabPoseScore.Max;
			for (int i = 0; i < this._controlPoints.Count; i++)
			{
				BezierControlPoint bezierControlPoint = this._controlPoints[i];
				BezierControlPoint bezierControlPoint2 = this._controlPoints[(i + 1) % this._controlPoints.Count];
				if (bezierControlPoint.Disconnected || !bezierControlPoint2.Disconnected)
				{
					if ((bezierControlPoint.Disconnected && bezierControlPoint2.Disconnected) || this._controlPoints.Count == 1)
					{
						Pose pose = bezierControlPoint.GetPose(relativeTo);
						Plane plane = new Plane(-targetRay.direction, pose.position);
						float distance;
						if (!plane.Raycast(targetRay, out distance))
						{
							goto IL_1BE;
						}
						identity2.position = targetRay.GetPoint(distance);
						ref identity.CopyFrom(pose);
					}
					else
					{
						Pose pose2 = bezierControlPoint.GetPose(relativeTo);
						Pose pose3 = bezierControlPoint2.GetPose(relativeTo);
						Vector3 tangent = bezierControlPoint.GetTangent(relativeTo);
						float distance2;
						if (!this.GenerateRaycastPlane(pose2.position, tangent, pose3.position, -targetRay.direction).Raycast(targetRay, out distance2))
						{
							goto IL_1BE;
						}
						identity2.position = targetRay.GetPoint(distance2);
						float t;
						this.NearestPointInTriangle(identity2.position, pose2.position, tangent, pose3.position, out t);
						identity.position = BezierGrabSurface.EvaluateBezier(pose2.position, tangent, pose3.position, t);
						identity.rotation = Quaternion.Slerp(pose2.rotation, pose3.rotation, t);
					}
					GrabPoseScore grabPoseScore = new GrabPoseScore(identity2.position, identity.position, false);
					if (grabPoseScore.IsBetterThan(referenceScore))
					{
						referenceScore = grabPoseScore;
						ref bestPose.CopyFrom(identity);
						result = true;
					}
				}
				IL_1BE:;
			}
			return result;
		}

		private Plane GenerateRaycastPlane(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 fallbackDir)
		{
			Vector3 normalized = (p1 - p0).normalized;
			Vector3 normalized2 = (p2 - p0).normalized;
			Plane result;
			if (Mathf.Abs(Vector3.Dot(normalized, normalized2)) > 0.95f)
			{
				result = new Plane(fallbackDir, (p0 + p2 + p1) / 3f);
			}
			else
			{
				result = new Plane(p0, p1, p2);
			}
			return result;
		}

		private float ProgressForRotation(Quaternion targetRotation, Quaternion from, Quaternion to)
		{
			Vector3 from2 = targetRotation * Vector3.forward;
			Vector3 vector = from * Vector3.forward;
			Vector3 vector2 = to * Vector3.forward;
			Vector3 normalized = Vector3.Cross(vector, vector2).normalized;
			float num = Vector3.SignedAngle(from2, vector, normalized);
			float num2 = Vector3.SignedAngle(from2, vector2, normalized);
			if (num < 0f && num2 < 0f)
			{
				return 1f;
			}
			if (num > 0f && num2 > 0f)
			{
				return 0f;
			}
			return Mathf.Abs(num) / Vector3.Angle(vector, vector2);
		}

		private Vector3 NearestPointInTriangle(Vector3 point, Vector3 p0, Vector3 p1, Vector3 p2, out float t)
		{
			Vector3 vector = (p0 + p1 + p2) / 3f;
			float num;
			Vector3 vector2 = this.NearestPointToSegment(point, p0, vector, out num);
			float num2;
			Vector3 vector3 = this.NearestPointToSegment(point, vector, p2, out num2);
			float num3 = Vector3.Distance(p0, vector);
			float num4 = Vector3.Distance(p2, vector);
			float num5 = num4 / (num3 + num4);
			float sqrMagnitude = (vector2 - point).sqrMagnitude;
			float sqrMagnitude2 = (vector3 - point).sqrMagnitude;
			if (sqrMagnitude < sqrMagnitude2)
			{
				t = num * num5;
				return vector2;
			}
			t = num5 + num2 * (1f - num5);
			return vector3;
		}

		private Vector3 NearestPointToSegment(Vector3 point, Vector3 start, Vector3 end, out float progress)
		{
			Vector3 lhs = end - start;
			Vector3 vector = Vector3.Project(point - start, lhs.normalized);
			Vector3 result;
			if (Vector3.Dot(lhs, vector) <= 0f)
			{
				result = start;
				progress = 0f;
			}
			else if (vector.sqrMagnitude >= lhs.sqrMagnitude)
			{
				result = end;
				progress = 1f;
			}
			else
			{
				result = start + vector;
				progress = vector.magnitude / lhs.magnitude;
			}
			return result;
		}

		public IGrabSurface CreateDuplicatedSurface(GameObject gameObject)
		{
			BezierGrabSurface bezierGrabSurface = gameObject.AddComponent<BezierGrabSurface>();
			bezierGrabSurface._controlPoints = new List<BezierControlPoint>(this._controlPoints);
			return bezierGrabSurface;
		}

		public IGrabSurface CreateMirroredSurface(GameObject gameObject)
		{
			BezierGrabSurface bezierGrabSurface = gameObject.AddComponent<BezierGrabSurface>();
			bezierGrabSurface._controlPoints = new List<BezierControlPoint>();
			foreach (BezierControlPoint item in this._controlPoints)
			{
				Pose pose = item.GetPose(this._relativeTo);
				pose.rotation *= Quaternion.Euler(180f, 180f, 0f);
				item.SetPose(pose, this._relativeTo);
				bezierGrabSurface._controlPoints.Add(item);
			}
			return bezierGrabSurface;
		}

		public Pose MirrorPose(in Pose gripPose, Transform relativeTo)
		{
			return gripPose;
		}

		public static Vector3 EvaluateBezier(Vector3 start, Vector3 middle, Vector3 end, float t)
		{
			t = Mathf.Clamp01(t);
			float num = 1f - t;
			return num * num * start + 2f * num * t * middle + t * t * end;
		}

		public void InjectAllBezierSurface(List<BezierControlPoint> controlPoints, Transform relativeTo)
		{
			this.InjectControlPoints(controlPoints);
			this.InjectRelativeTo(relativeTo);
		}

		public void InjectControlPoints(List<BezierControlPoint> controlPoints)
		{
			this._controlPoints = controlPoints;
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
		private List<BezierControlPoint> _controlPoints = new List<BezierControlPoint>();

		[SerializeField]
		[Tooltip("Transform used as a reference to measure the local data of the grab surface")]
		private Transform _relativeTo;

		private const float MAX_PLANE_DOT = 0.95f;
	}
}
