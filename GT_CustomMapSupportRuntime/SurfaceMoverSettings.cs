using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	public class SurfaceMoverSettings : MonoBehaviour
	{
		public void OnEnable()
		{
			if (this.hasBeenExported)
			{
				return;
			}
			if (this.moveType == SurfaceMoverSettings.MoveType.Translation && this.start != null && this.end != null)
			{
				this.distance = Vector3.Distance(this.end.position, this.start.position);
				float num = this.distance / this.velocity;
				this.cycleDuration = num + this.cycleDelay;
			}
			else
			{
				if (this.rotationRelativeToStarting)
				{
					this.startingRotation = base.transform.localRotation.eulerAngles;
				}
				this.cycleDuration = this.rotationAmount / 360f / this.velocity;
				this.cycleDuration += this.cycleDelay;
			}
			float num2 = this.cycleDelay / this.cycleDuration;
			Vector2 vector = new Vector2(num2 / 2f, 0f);
			Vector2 vector2 = new Vector2(1f - num2 / 2f, 1f);
			float num3 = (vector2.y - vector.y) / (vector2.x - vector.x);
			this.lerpAlpha = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(num2 / 2f, 0f, 0f, num3),
				new Keyframe(1f - num2 / 2f, 1f, num3, 0f)
			});
		}

		private void FixedUpdate()
		{
			if (this.hasBeenExported)
			{
				return;
			}
			this.Move();
		}

		private long NetworkTimeMs()
		{
			return (long)(Time.time * 1000f);
		}

		private long CycleLengthMs()
		{
			return (long)(this.cycleDuration * 1000f);
		}

		private double PlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		private int CycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
		}

		private float CycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.PlatformTime() / (double)this.cycleDuration), 0f, 1f);
		}

		private bool IsEvenCycle()
		{
			return this.CycleCount() % 2 == 0;
		}

		public void Move()
		{
			this.Progress();
			SurfaceMoverSettings.MoveType moveType = this.moveType;
			if (moveType == SurfaceMoverSettings.MoveType.Translation)
			{
				base.transform.localPosition = this.UpdatePointToPoint(this.percent);
				return;
			}
			if (moveType != SurfaceMoverSettings.MoveType.Rotation)
			{
				return;
			}
			this.UpdateRotation(this.percent);
		}

		private Vector3 UpdatePointToPoint(float percentage)
		{
			if (this.lerpAlpha == null || this.start == null || this.end == null)
			{
				return base.transform.localPosition;
			}
			float t = this.lerpAlpha.Evaluate(percentage);
			return Vector3.Lerp(this.start.localPosition, this.end.localPosition, t);
		}

		private void UpdateRotation(float percentage)
		{
			if (this.lerpAlpha == null)
			{
				return;
			}
			float num = this.lerpAlpha.Evaluate(percentage) * this.rotationAmount;
			if (this.rotationRelativeToStarting)
			{
				Vector3 euler = this.startingRotation;
				switch (this.rotationAxis)
				{
				case SurfaceMoverSettings.RotationAxis.X:
					euler.x += num;
					break;
				case SurfaceMoverSettings.RotationAxis.Y:
					euler.y += num;
					break;
				case SurfaceMoverSettings.RotationAxis.Z:
					euler.z += num;
					break;
				}
				base.transform.localRotation = Quaternion.Euler(euler);
				return;
			}
			switch (this.rotationAxis)
			{
			case SurfaceMoverSettings.RotationAxis.X:
				base.transform.localRotation = Quaternion.AngleAxis(num, Vector3.right);
				return;
			case SurfaceMoverSettings.RotationAxis.Y:
				base.transform.localRotation = Quaternion.AngleAxis(num, Vector3.up);
				return;
			case SurfaceMoverSettings.RotationAxis.Z:
				base.transform.localRotation = Quaternion.AngleAxis(num, Vector3.forward);
				return;
			default:
				return;
			}
		}

		private void Progress()
		{
			this.currT = this.CycleCompletionPercent();
			this.currForward = this.IsEvenCycle();
			this.percent = this.currT;
			if (this.reverseDirOnCycle)
			{
				this.percent = (this.currForward ? this.currT : (1f - this.currT));
			}
			if (this.reverseDir)
			{
				this.percent = 1f - this.percent;
			}
		}

		[SerializeField]
		public SurfaceMoverSettings.MoveType moveType;

		[Range(0.001f, 3.4028235E+38f)]
		[Tooltip("Meters per second for Translation | Revolutions per second for Rotation")]
		[SerializeField]
		public float velocity = 0.001f;

		[Range(0f, 3.4028235E+38f)]
		[Tooltip("How long in seconds should the cycle be delayed?")]
		[SerializeField]
		public float cycleDelay;

		[Tooltip("If TRUE, Translation mode will move from End to Start; Rotation mode will rotate in the negative direction.")]
		[SerializeField]
		public bool reverseDir;

		[Tooltip("If TRUE, Translation mode movement direction will be reversed when it reaches Start or End; Rotation mode rotation direction will be reversed once it's rotated the full Rotation Amount")]
		[SerializeField]
		public bool reverseDirOnCycle = true;

		[Nullable(2)]
		[SerializeField]
		public Transform start;

		[Nullable(2)]
		[SerializeField]
		public Transform end;

		[Tooltip("Which local axis should the object rotate around?")]
		[SerializeField]
		public SurfaceMoverSettings.RotationAxis rotationAxis = SurfaceMoverSettings.RotationAxis.Y;

		[Range(0.001f, 360f)]
		[Tooltip("How far should the object rotate per-cycle (in degrees)")]
		[SerializeField]
		public float rotationAmount = 360f;

		[Tooltip("If TRUE the rotation starting point will be the initial Y-axis rotation value of the object when the map is loaded, otherwise it will start at 0")]
		[SerializeField]
		public bool rotationRelativeToStarting = true;

		public bool hasBeenExported;

		[Nullable(2)]
		private AnimationCurve lerpAlpha;

		private Vector3 startingRotation;

		private float cycleDuration;

		private float distance;

		private float currT;

		private float percent;

		private bool currForward;

		public enum MoveType
		{
			Translation,
			Rotation
		}

		public enum RotationAxis
		{
			X,
			Y,
			Z
		}
	}
}
