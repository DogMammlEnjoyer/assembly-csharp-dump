using System;
using UnityEngine.Serialization;

namespace UnityEngine.Splines
{
	[Serializable]
	public struct DataPoint<TDataType> : IComparable<DataPoint<TDataType>>, IComparable<float>, IDataPoint
	{
		public float Index
		{
			get
			{
				return this.m_Index;
			}
			set
			{
				this.m_Index = value;
			}
		}

		public TDataType Value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = value;
			}
		}

		public DataPoint(float index, TDataType value)
		{
			this.m_Index = index;
			this.m_Value = value;
		}

		public int CompareTo(DataPoint<TDataType> other)
		{
			return this.Index.CompareTo(other.Index);
		}

		public int CompareTo(float other)
		{
			return this.Index.CompareTo(other);
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", this.Index, this.Value);
		}

		[FormerlySerializedAs("m_Time")]
		[SerializeField]
		private float m_Index;

		[SerializeField]
		private TDataType m_Value;
	}
}
