using System;
using UnityEngine;

namespace Unity.XR.CoreUtils
{
	[Serializable]
	public struct SerializableGuid : IEquatable<SerializableGuid>
	{
		public static SerializableGuid Empty
		{
			get
			{
				return SerializableGuid.k_Empty;
			}
		}

		public Guid Guid
		{
			get
			{
				return GuidUtil.Compose(this.m_GuidLow, this.m_GuidHigh);
			}
		}

		public SerializableGuid(ulong guidLow, ulong guidHigh)
		{
			this.m_GuidLow = guidLow;
			this.m_GuidHigh = guidHigh;
		}

		public override int GetHashCode()
		{
			return this.m_GuidLow.GetHashCode() * 486187739 + this.m_GuidHigh.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is SerializableGuid)
			{
				SerializableGuid other = (SerializableGuid)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override string ToString()
		{
			return this.Guid.ToString();
		}

		public string ToString(string format)
		{
			return this.Guid.ToString(format);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return this.Guid.ToString(format, provider);
		}

		public bool Equals(SerializableGuid other)
		{
			return this.m_GuidLow == other.m_GuidLow && this.m_GuidHigh == other.m_GuidHigh;
		}

		public static bool operator ==(SerializableGuid lhs, SerializableGuid rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SerializableGuid lhs, SerializableGuid rhs)
		{
			return !lhs.Equals(rhs);
		}

		private static readonly SerializableGuid k_Empty = new SerializableGuid(0UL, 0UL);

		[SerializeField]
		[HideInInspector]
		private ulong m_GuidLow;

		[SerializeField]
		[HideInInspector]
		private ulong m_GuidHigh;
	}
}
