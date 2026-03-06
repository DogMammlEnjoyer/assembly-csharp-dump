using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities.Collections
{
	public struct NativeCurve : IDisposable
	{
		public bool isCreated
		{
			get
			{
				return this.m_Values.IsCreated;
			}
		}

		private void InitializeValues(int count, Allocator allocator = Allocator.Persistent)
		{
			if (this.m_Values.IsCreated)
			{
				this.m_Values.Dispose();
			}
			this.m_Values = new NativeArray<float>(count, allocator, NativeArrayOptions.UninitializedMemory);
		}

		public void Update(AnimationCurve curve, int resolution)
		{
			if (curve == null)
			{
				return;
			}
			this.m_PreWrapMode = curve.preWrapMode;
			this.m_PostWrapMode = curve.postWrapMode;
			if (!this.m_Values.IsCreated || this.m_Values.Length != resolution)
			{
				this.InitializeValues(resolution, Allocator.Persistent);
			}
			for (int i = 0; i < resolution; i++)
			{
				this.m_Values[i] = curve.Evaluate((float)i / (float)resolution);
			}
		}

		public float Evaluate(float t)
		{
			int length = this.m_Values.Length;
			if (length == 1)
			{
				return this.m_Values[0];
			}
			if (t < 0f)
			{
				WrapMode wrapMode = this.m_PreWrapMode;
				if (wrapMode != WrapMode.Loop)
				{
					if (wrapMode != WrapMode.PingPong)
					{
						return this.m_Values[0];
					}
					t = this.PingPong(t, 1f);
				}
				else
				{
					t = 1f - math.abs(t) % 1f;
				}
			}
			else if (t > 1f)
			{
				WrapMode wrapMode = this.m_PostWrapMode;
				if (wrapMode != WrapMode.Loop)
				{
					if (wrapMode != WrapMode.PingPong)
					{
						return this.m_Values[length - 1];
					}
					t = this.PingPong(t, 1f);
				}
				else
				{
					t %= 1f;
				}
			}
			float num = t * (float)(length - 1);
			int num2 = (int)num;
			int num3 = num2 + 1;
			if (num3 >= length)
			{
				num3 = length - 1;
			}
			return math.lerp(this.m_Values[num2], this.m_Values[num3], num - (float)num2);
		}

		public void Dispose()
		{
			if (this.m_Values.IsCreated)
			{
				this.m_Values.Dispose();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float Repeat(float t, float length)
		{
			return math.clamp(t - math.floor(t / length) * length, 0f, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float PingPong(float t, float length)
		{
			t = this.Repeat(t, length * 2f);
			return length - math.abs(t - length);
		}

		private NativeArray<float> m_Values;

		private WrapMode m_PreWrapMode;

		private WrapMode m_PostWrapMode;
	}
}
