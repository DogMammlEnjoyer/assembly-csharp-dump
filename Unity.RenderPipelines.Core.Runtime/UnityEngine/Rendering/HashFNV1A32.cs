using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering
{
	internal ref struct HashFNV1A32
	{
		public static HashFNV1A32 Create()
		{
			return new HashFNV1A32
			{
				m_Hash = 2166136261U
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(in int input)
		{
			this.m_Hash = (this.m_Hash ^ (uint)input) * 16777619U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(in uint input)
		{
			this.m_Hash = (this.m_Hash ^ input) * 16777619U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(in bool input)
		{
			this.m_Hash = (this.m_Hash ^ (input ? 1U : 0U)) * 16777619U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(in float input)
		{
			this.m_Hash = (this.m_Hash ^ (uint)input.GetHashCode()) * 16777619U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(in double input)
		{
			this.m_Hash = (this.m_Hash ^ (uint)input.GetHashCode()) * 16777619U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(in Vector2 input)
		{
			uint hash = this.m_Hash;
			Vector2 vector = input;
			this.m_Hash = (hash ^ (uint)vector.GetHashCode()) * 16777619U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(in Vector3 input)
		{
			uint hash = this.m_Hash;
			Vector3 vector = input;
			this.m_Hash = (hash ^ (uint)vector.GetHashCode()) * 16777619U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(in Vector4 input)
		{
			uint hash = this.m_Hash;
			Vector4 vector = input;
			this.m_Hash = (hash ^ (uint)vector.GetHashCode()) * 16777619U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append<T>(T input) where T : struct
		{
			this.m_Hash = (this.m_Hash ^ (uint)input.GetHashCode()) * 16777619U;
		}

		public int value
		{
			get
			{
				return (int)this.m_Hash;
			}
		}

		public override int GetHashCode()
		{
			return this.value;
		}

		private const uint k_Prime = 16777619U;

		private const uint k_OffsetBasis = 2166136261U;

		private uint m_Hash;
	}
}
