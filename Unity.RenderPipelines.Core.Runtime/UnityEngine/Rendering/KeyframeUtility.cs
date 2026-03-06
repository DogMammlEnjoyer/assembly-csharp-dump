using System;
using System.Diagnostics.CodeAnalysis;
using Unity.Collections;

namespace UnityEngine.Rendering
{
	public class KeyframeUtility
	{
		public static void ResetAnimationCurve(AnimationCurve curve)
		{
			curve.ClearKeys();
		}

		private static Keyframe LerpSingleKeyframe(Keyframe lhs, Keyframe rhs, float t)
		{
			return new Keyframe
			{
				time = Mathf.Lerp(lhs.time, rhs.time, t),
				value = Mathf.Lerp(lhs.value, rhs.value, t),
				inTangent = Mathf.Lerp(lhs.inTangent, rhs.inTangent, t),
				outTangent = Mathf.Lerp(lhs.outTangent, rhs.outTangent, t),
				inWeight = Mathf.Lerp(lhs.inWeight, rhs.inWeight, t),
				outWeight = Mathf.Lerp(lhs.outWeight, rhs.outWeight, t),
				weightedMode = lhs.weightedMode
			};
		}

		private static Keyframe GetKeyframeAndClampEdge([DisallowNull] NativeArray<Keyframe> keys, int index)
		{
			int num = keys.Length - 1;
			if (index < 0 || index > num)
			{
				Debug.LogWarning("Invalid index in GetKeyframeAndClampEdge. This is likely a bug.");
				return default(Keyframe);
			}
			Keyframe result = keys[index];
			if (index == 0)
			{
				result.inTangent = 0f;
			}
			if (index == num)
			{
				result.outTangent = 0f;
			}
			return result;
		}

		private static Keyframe FetchKeyFromIndexClampEdge([DisallowNull] NativeArray<Keyframe> keys, int index, float segmentStartTime, float segmentEndTime)
		{
			float time = Mathf.Min(segmentStartTime, keys[0].time);
			float time2 = Mathf.Max(segmentEndTime, keys[keys.Length - 1].time);
			float value = keys[0].value;
			float value2 = keys[keys.Length - 1].value;
			Keyframe keyframeAndClampEdge;
			if (index < 0)
			{
				keyframeAndClampEdge = new Keyframe(time, value, 0f, 0f);
			}
			else if (index >= keys.Length)
			{
				Keyframe keyframe = keys[keys.Length - 1];
				keyframeAndClampEdge = new Keyframe(time2, value2, 0f, 0f);
			}
			else
			{
				keyframeAndClampEdge = KeyframeUtility.GetKeyframeAndClampEdge(keys, index);
			}
			return keyframeAndClampEdge;
		}

		private static void EvalCurveSegmentAndDeriv(out float dstValue, out float dstDeriv, Keyframe lhsKey, Keyframe rhsKey, float desiredTime)
		{
			float num = Mathf.Clamp(desiredTime, lhsKey.time, rhsKey.time);
			float num2 = Mathf.Max(rhsKey.time - lhsKey.time, 0.0001f);
			float num3 = rhsKey.value - lhsKey.value;
			float num4 = 1f / num2;
			float num5 = num4 * num4;
			float outTangent = lhsKey.outTangent;
			float inTangent = rhsKey.inTangent;
			float num6 = outTangent * num2;
			float num7 = inTangent * num2;
			float num8 = (num6 + num7 - num3 - num3) * num5 * num4;
			float num9 = (num3 + num3 + num3 - num6 - num6 - num7) * num5;
			float num10 = outTangent;
			float value = lhsKey.value;
			float num11 = Mathf.Clamp(num - lhsKey.time, 0f, num2);
			dstValue = num11 * (num11 * (num11 * num8 + num9) + num10) + value;
			dstDeriv = num11 * (3f * num11 * num8 + 2f * num9) + num10;
		}

		private static Keyframe EvalKeyAtTime([DisallowNull] NativeArray<Keyframe> keys, int lhsIndex, int rhsIndex, float startTime, float endTime, float currTime)
		{
			Keyframe lhsKey = KeyframeUtility.FetchKeyFromIndexClampEdge(keys, lhsIndex, startTime, endTime);
			Keyframe rhsKey = KeyframeUtility.FetchKeyFromIndexClampEdge(keys, rhsIndex, startTime, endTime);
			float value;
			float num;
			KeyframeUtility.EvalCurveSegmentAndDeriv(out value, out num, lhsKey, rhsKey, currTime);
			return new Keyframe(currTime, value, num, num);
		}

		public static void InterpAnimationCurve(ref AnimationCurve lhsAndResultCurve, [DisallowNull] AnimationCurve rhsCurve, float t)
		{
			if (t > 0f && rhsCurve.length != 0)
			{
				if (t >= 1f || lhsAndResultCurve.length == 0)
				{
					lhsAndResultCurve.CopyFrom(rhsCurve);
					return;
				}
				NativeArray<Keyframe> keys = new NativeArray<Keyframe>(lhsAndResultCurve.length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				NativeArray<Keyframe> keys2 = new NativeArray<Keyframe>(rhsCurve.length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				for (int i = 0; i < lhsAndResultCurve.length; i++)
				{
					keys[i] = lhsAndResultCurve[i];
				}
				for (int j = 0; j < rhsCurve.length; j++)
				{
					keys2[j] = rhsCurve[j];
				}
				float startTime = Mathf.Min(keys[0].time, keys2[0].time);
				float endTime = Mathf.Max(keys[lhsAndResultCurve.length - 1].time, keys2[rhsCurve.length - 1].time);
				int length = lhsAndResultCurve.length + rhsCurve.length;
				int num = 0;
				NativeArray<Keyframe> nativeArray = new NativeArray<Keyframe>(length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				int num2 = 0;
				int num3 = 0;
				while (num2 < keys.Length || num3 < keys2.Length)
				{
					bool flag = num2 < keys.Length;
					bool flag2 = num3 < keys2.Length;
					Keyframe lhs = default(Keyframe);
					Keyframe rhs = default(Keyframe);
					if (flag && flag2)
					{
						lhs = KeyframeUtility.GetKeyframeAndClampEdge(keys, num2);
						rhs = KeyframeUtility.GetKeyframeAndClampEdge(keys2, num3);
						if (lhs.time == rhs.time)
						{
							num2++;
							num3++;
						}
						else if (lhs.time < rhs.time)
						{
							rhs = KeyframeUtility.EvalKeyAtTime(keys2, num3 - 1, num3, startTime, endTime, lhs.time);
							num2++;
						}
						else
						{
							lhs = KeyframeUtility.EvalKeyAtTime(keys, num2 - 1, num2, startTime, endTime, rhs.time);
							num3++;
						}
					}
					else if (flag)
					{
						lhs = KeyframeUtility.GetKeyframeAndClampEdge(keys, num2);
						rhs = KeyframeUtility.EvalKeyAtTime(keys2, num3 - 1, num3, startTime, endTime, lhs.time);
						num2++;
					}
					else
					{
						rhs = KeyframeUtility.GetKeyframeAndClampEdge(keys2, num3);
						lhs = KeyframeUtility.EvalKeyAtTime(keys, num2 - 1, num2, startTime, endTime, rhs.time);
						num3++;
					}
					Keyframe value = KeyframeUtility.LerpSingleKeyframe(lhs, rhs, t);
					nativeArray[num] = value;
					num++;
				}
				KeyframeUtility.ResetAnimationCurve(lhsAndResultCurve);
				for (int k = 0; k < num; k++)
				{
					lhsAndResultCurve.AddKey(nativeArray[k]);
				}
				nativeArray.Dispose();
			}
		}
	}
}
