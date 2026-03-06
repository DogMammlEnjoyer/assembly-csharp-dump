using System;
using UnityEngine;

namespace Oculus.Interaction.Demo
{
	public class WaterSprayNozzleTransformer : MonoBehaviour, ITransformer
	{
		public void Initialize(IGrabbable grabbable)
		{
			this._grabbable = grabbable;
		}

		public void BeginTransform()
		{
			this._previousGrabPose = this._grabbable.GrabPoints[0];
			this._relativeAngle = 0f;
			this._stepsCount = 0;
		}

		public void UpdateTransform()
		{
			Pose previousGrabPose = this._grabbable.GrabPoints[0];
			Transform transform = this._grabbable.Transform;
			Vector3 forward = Vector3.forward;
			Vector3 vector = transform.TransformDirection(forward);
			Vector3 normalized = Vector3.ProjectOnPlane(this._previousGrabPose.right, vector).normalized;
			Vector3 normalized2 = Vector3.ProjectOnPlane(previousGrabPose.right, vector).normalized;
			float num = Vector3.SignedAngle(normalized, normalized2, vector) * this._factor;
			this._relativeAngle += num;
			if (Mathf.Abs(this._relativeAngle) > this._snapAngle * (1f - this._snappiness) && Mathf.Abs((float)this._stepsCount + Mathf.Sign(this._relativeAngle)) <= (float)this._maxSteps)
			{
				int num2 = Mathf.FloorToInt((transform.localEulerAngles.z + this._snappiness * 0.5f) / this._snapAngle);
				float num3 = Mathf.Sign(this._relativeAngle);
				float z = (num3 > 0f) ? (this._snapAngle * (float)(num2 + 1)) : (this._snapAngle * (float)num2);
				Vector3 localEulerAngles = transform.localEulerAngles;
				localEulerAngles.z = z;
				transform.localEulerAngles = localEulerAngles;
				this._relativeAngle = 0f;
				this._stepsCount += (int)num3;
			}
			else
			{
				transform.Rotate(vector, num, Space.World);
			}
			this._previousGrabPose = previousGrabPose;
		}

		public void EndTransform()
		{
		}

		[SerializeField]
		private float _factor = 3f;

		[SerializeField]
		private float _snapAngle = 90f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _snappiness = 0.8f;

		[SerializeField]
		private int _maxSteps = 1;

		private float _relativeAngle;

		private int _stepsCount;

		private IGrabbable _grabbable;

		private Pose _previousGrabPose;
	}
}
