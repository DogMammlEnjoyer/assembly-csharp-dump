using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineImpulseFixedSignals.html")]
	public class CinemachineFixedSignal : SignalSourceAsset
	{
		public override float SignalDuration
		{
			get
			{
				return Mathf.Max(this.AxisDuration(this.XCurve), Mathf.Max(this.AxisDuration(this.YCurve), this.AxisDuration(this.ZCurve)));
			}
		}

		private float AxisDuration(AnimationCurve axis)
		{
			float result = 0f;
			if (axis != null && axis.length > 1)
			{
				float time = axis[0].time;
				result = axis[axis.length - 1].time - time;
			}
			return result;
		}

		public override void GetSignal(float timeSinceSignalStart, out Vector3 pos, out Quaternion rot)
		{
			rot = Quaternion.identity;
			pos = new Vector3(this.AxisValue(this.XCurve, timeSinceSignalStart), this.AxisValue(this.YCurve, timeSinceSignalStart), this.AxisValue(this.ZCurve, timeSinceSignalStart));
		}

		private float AxisValue(AnimationCurve axis, float time)
		{
			if (axis == null || axis.length == 0)
			{
				return 0f;
			}
			return axis.Evaluate(time);
		}

		[Tooltip("The raw signal shape along the X axis")]
		[FormerlySerializedAs("m_XCurve")]
		public AnimationCurve XCurve;

		[Tooltip("The raw signal shape along the Y axis")]
		[FormerlySerializedAs("m_YCurve")]
		public AnimationCurve YCurve;

		[Tooltip("The raw signal shape along the Z axis")]
		[FormerlySerializedAs("m_ZCurve")]
		public AnimationCurve ZCurve;
	}
}
