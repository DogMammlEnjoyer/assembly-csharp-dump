using System;
using System.Runtime.CompilerServices;
using Unity.XR.CoreUtils;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Collections;

namespace UnityEngine.XR.Interaction.Toolkit.Attachment
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit.Interaction")]
	public class AttachPointVelocityTracker : IAttachPointVelocityTracker, IAttachPointVelocityProvider
	{
		public void UpdateAttachPointVelocityData(Transform attachTransform)
		{
			this.UpdateAttachPointVelocityData(attachTransform, false, null);
		}

		public void UpdateAttachPointVelocityData(Transform attachTransform, Transform xrOriginTransform)
		{
			this.UpdateAttachPointVelocityData(attachTransform, true, xrOriginTransform);
		}

		private void UpdateAttachPointVelocityData(Transform attachTransform, bool useXROriginTransform, Transform xrOriginTransform = null)
		{
			float unscaledTime = Time.unscaledTime;
			object obj = useXROriginTransform && xrOriginTransform != null;
			Pose worldPose = attachTransform.GetWorldPose();
			object obj2 = obj;
			Vector3 item = (obj2 != null) ? xrOriginTransform.InverseTransformPoint(worldPose.position) : worldPose.position;
			Quaternion item2 = (obj2 != null) ? (Quaternion.Inverse(xrOriginTransform.rotation) * worldPose.rotation) : worldPose.rotation;
			this.m_PositionTimeBuffer.Add(new ValueTuple<Vector3, float>(item, unscaledTime));
			this.m_RotationTimeBuffer.Add(new ValueTuple<Quaternion, float>(item2, unscaledTime));
			this.m_AttachPointVelocity = ((this.m_PositionTimeBuffer.count > 1) ? this.CalculateVelocityWithWeightedLinearRegression() : Vector3.zero);
			this.m_AttachPointAngularVelocity = ((this.m_RotationTimeBuffer.count > 1) ? this.CalculateAngularVelocityWithWeightedRegression() : Vector3.zero);
		}

		private Vector3 CalculateVelocityWithWeightedLinearRegression()
		{
			int count = this.m_PositionTimeBuffer.count;
			if (count < 2)
			{
				return Vector3.zero;
			}
			Vector3 a = Vector3.zero;
			float num = 0f;
			Vector3 a2 = Vector3.zero;
			float num2 = 0f;
			float num3 = 0f;
			float item = this.m_PositionTimeBuffer[0].Item2;
			float num4 = this.m_PositionTimeBuffer[count - 1].Item2 - item;
			if (num4 < 1E-05f)
			{
				if (!Mathf.Approximately(num4, 0f))
				{
					return (this.m_PositionTimeBuffer[count - 1].Item1 - this.m_PositionTimeBuffer[0].Item1) / num4;
				}
				return Vector3.zero;
			}
			else
			{
				for (int i = 0; i < count; i++)
				{
					float num5 = this.m_PositionTimeBuffer[i].Item2 - item;
					Vector3 item2 = this.m_PositionTimeBuffer[i].Item1;
					float num6 = 1f + num5 / num4;
					a += item2 * num6;
					num += num5 * num6;
					a2 += item2 * (num5 * num6);
					num2 += num5 * num5 * num6;
					num3 += num6;
				}
				float num7 = num3 * num2 - num * num;
				if (Mathf.Approximately(num7, 0f))
				{
					return Vector3.zero;
				}
				return (num3 * a2 - a * num) / num7;
			}
		}

		private Vector3 CalculateAngularVelocityWithWeightedRegression()
		{
			int count = this.m_RotationTimeBuffer.count;
			if (count < 2)
			{
				return Vector3.zero;
			}
			Vector3 a = Vector3.zero;
			float num = 0f;
			Vector3 a2 = Vector3.zero;
			float num2 = 0f;
			float num3 = 0f;
			float item = this.m_RotationTimeBuffer[0].Item2;
			float num4 = this.m_RotationTimeBuffer[count - 1].Item2 - item;
			if (num4 < 1E-05f)
			{
				return Vector3.zero;
			}
			Quaternion item2 = this.m_RotationTimeBuffer[0].Item1;
			for (int i = 1; i < count; i++)
			{
				float num5 = this.m_RotationTimeBuffer[i].Item2 - item;
				float num6;
				Vector3 a3;
				(this.m_RotationTimeBuffer[i].Item1 * Quaternion.Inverse(item2)).ToAngleAxis(out num6, out a3);
				if (num6 > 180f)
				{
					num6 -= 360f;
				}
				Vector3 a4 = a3 * (num6 * 0.017453292f);
				float num7 = 1f + num5 / num4;
				a += a4 * num7;
				num += num5 * num7;
				a2 += a4 * (num5 * num7);
				num2 += num5 * num5 * num7;
				num3 += num7;
			}
			float num8 = num3 * num2 - num * num;
			if (Mathf.Approximately(num8, 0f))
			{
				return Vector3.zero;
			}
			return (num3 * a2 - a * num) / num8;
		}

		public void ResetVelocityTracking()
		{
			this.m_PositionTimeBuffer.Clear();
			this.m_RotationTimeBuffer.Clear();
			this.m_AttachPointVelocity = Vector3.zero;
			this.m_AttachPointAngularVelocity = Vector3.zero;
		}

		public Vector3 GetAttachPointVelocity()
		{
			return this.m_AttachPointVelocity;
		}

		public Vector3 GetAttachPointAngularVelocity()
		{
			return this.m_AttachPointAngularVelocity;
		}

		private const int k_BufferSize = 20;

		private const float k_MinimumDeltaTime = 1E-05f;

		[TupleElementNames(new string[]
		{
			"position",
			"time"
		})]
		private readonly CircularBuffer<ValueTuple<Vector3, float>> m_PositionTimeBuffer = new CircularBuffer<ValueTuple<Vector3, float>>(20);

		[TupleElementNames(new string[]
		{
			"rotation",
			"time"
		})]
		private readonly CircularBuffer<ValueTuple<Quaternion, float>> m_RotationTimeBuffer = new CircularBuffer<ValueTuple<Quaternion, float>>(20);

		private Vector3 m_AttachPointVelocity;

		private Vector3 m_AttachPointAngularVelocity;
	}
}
