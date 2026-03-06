using System;
using UnityEngine;

namespace Oculus.Interaction
{
	[Serializable]
	public struct PoseTravelData
	{
		public static PoseTravelData DEFAULT
		{
			get
			{
				return new PoseTravelData
				{
					_travelSpeed = 1f,
					_useFixedTravelTime = false,
					_travelCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f)
				};
			}
		}

		public static PoseTravelData FAST
		{
			get
			{
				return new PoseTravelData
				{
					_travelSpeed = 0.1f,
					_useFixedTravelTime = true,
					_travelCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f)
				};
			}
		}

		public Tween CreateTween(in Pose from, in Pose to)
		{
			float num = this._travelSpeed;
			if (!this._useFixedTravelTime && this._travelSpeed != 0f)
			{
				num = PoseTravelData.PerceivedDistance(from, to) / this._travelSpeed;
			}
			Tween tween = new Tween(from, num, num * 0.5f, this._travelCurve);
			tween.MoveTo(to);
			return tween;
		}

		private static float PerceivedDistance(in Pose from, in Pose to)
		{
			float magnitude = PoseUtils.Delta(from, to).position.magnitude;
			float num = 0.0013888889f;
			float[] array = new float[1];
			int num2 = 0;
			float[] array2 = new float[3];
			int num3 = 0;
			Pose pose = from;
			Vector3 forward = pose.forward;
			pose = to;
			array2[num3] = Vector3.Angle(forward, pose.forward);
			int num4 = 1;
			pose = from;
			Vector3 up = pose.up;
			pose = to;
			array2[num4] = Vector3.Angle(up, pose.up);
			int num5 = 2;
			pose = from;
			Vector3 right = pose.right;
			pose = to;
			array2[num5] = Vector3.Angle(right, pose.right);
			array[num2] = Mathf.Max(array2);
			float b = num * Mathf.Max(array);
			return Mathf.Max(magnitude, b);
		}

		[Tooltip("When attracting the object, indicates the rate (in m/s, or seconds if UseFixedTravelTime is enabled) for the object to realign with the hand after a grab.")]
		[SerializeField]
		private float _travelSpeed;

		[Tooltip("Changes the units of the TravelSpeed, disabled means m/s while enabled is fixed seconds")]
		[SerializeField]
		private bool _useFixedTravelTime;

		[Tooltip("Animation to use in conjunction with TravelSpeed to define the traveling motion.")]
		[SerializeField]
		private AnimationCurve _travelCurve;

		private const float DEGREES_TO_PERCEIVED_METERS = 0.0013888889f;
	}
}
