using System;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	[Serializable]
	public class EmbeddedSplineData
	{
		public SplineContainer Container
		{
			get
			{
				return this.m_Container;
			}
			set
			{
				this.m_Container = value;
			}
		}

		public int SplineIndex
		{
			get
			{
				return this.m_SplineIndex;
			}
			set
			{
				this.m_SplineIndex = value;
			}
		}

		public EmbeddedSplineDataType Type
		{
			get
			{
				return this.m_Type;
			}
			set
			{
				this.m_Type = value;
			}
		}

		public string Key
		{
			get
			{
				return this.m_Key;
			}
			set
			{
				this.m_Key = value;
			}
		}

		public EmbeddedSplineData() : this(null, EmbeddedSplineDataType.Float, null, 0)
		{
		}

		public EmbeddedSplineData(string key, EmbeddedSplineDataType type, SplineContainer container = null, int splineIndex = 0)
		{
			this.m_Container = container;
			this.m_SplineIndex = splineIndex;
			this.m_Key = key;
			this.m_Type = type;
		}

		public bool TryGetSpline(out Spline spline)
		{
			if (this.Container == null || this.SplineIndex < 0 || this.SplineIndex >= this.Container.Splines.Count)
			{
				spline = null;
			}
			else
			{
				spline = this.Container.Splines[this.SplineIndex];
			}
			return spline != null;
		}

		public bool TryGetFloatData(out SplineData<float> data)
		{
			if (this.Type != EmbeddedSplineDataType.Float)
			{
				throw new InvalidCastException(string.Format("EmbeddedSplineDataType {0} does not match requested SplineData collection: {1}", this.Type, typeof(float)));
			}
			return this.Container.Splines[this.SplineIndex].TryGetFloatData(this.Key, out data);
		}

		public bool TryGetFloat4Data(out SplineData<float4> data)
		{
			if (this.Type != EmbeddedSplineDataType.Float4)
			{
				throw new InvalidCastException(string.Format("EmbeddedSplineDataType {0} does not match requested SplineData collection: {1}", this.Type, typeof(float4)));
			}
			return this.Container.Splines[this.SplineIndex].TryGetFloat4Data(this.Key, out data);
		}

		public bool TryGetIntData(out SplineData<int> data)
		{
			if (this.Type != EmbeddedSplineDataType.Int)
			{
				throw new InvalidCastException(string.Format("EmbeddedSplineDataType {0} does not match requested SplineData collection: {1}", this.Type, typeof(int)));
			}
			return this.Container.Splines[this.SplineIndex].TryGetIntData(this.Key, out data);
		}

		public bool TryGetObjectData(out SplineData<Object> data)
		{
			if (this.Type != EmbeddedSplineDataType.Object)
			{
				throw new InvalidCastException(string.Format("EmbeddedSplineDataType {0} does not match requested SplineData collection: {1}", this.Type, typeof(Object)));
			}
			return this.Container.Splines[this.SplineIndex].TryGetObjectData(this.Key, out data);
		}

		public SplineData<float> GetOrCreateFloatData()
		{
			if (this.Type != EmbeddedSplineDataType.Float)
			{
				throw new InvalidCastException(string.Format("EmbeddedSplineDataType {0} does not match requested SplineData collection: {1}", this.Type, typeof(float)));
			}
			return this.Container.Splines[this.SplineIndex].GetOrCreateFloatData(this.Key);
		}

		public SplineData<float4> GetOrCreateFloat4Data()
		{
			if (this.Type != EmbeddedSplineDataType.Float4)
			{
				throw new InvalidCastException(string.Format("EmbeddedSplineDataType {0} does not match requested SplineData collection: {1}", this.Type, typeof(float4)));
			}
			return this.Container.Splines[this.SplineIndex].GetOrCreateFloat4Data(this.Key);
		}

		public SplineData<int> GetOrCreateIntData()
		{
			if (this.Type != EmbeddedSplineDataType.Int)
			{
				throw new InvalidCastException(string.Format("EmbeddedSplineDataType {0} does not match requested SplineData collection: {1}", this.Type, typeof(int)));
			}
			return this.Container.Splines[this.SplineIndex].GetOrCreateIntData(this.Key);
		}

		public SplineData<Object> GetOrCreateObjectData()
		{
			if (this.Type != EmbeddedSplineDataType.Object)
			{
				throw new InvalidCastException(string.Format("EmbeddedSplineDataType {0} does not match requested SplineData collection: {1}", this.Type, typeof(Object)));
			}
			return this.Container.Splines[this.SplineIndex].GetOrCreateObjectData(this.Key);
		}

		[SerializeField]
		private SplineContainer m_Container;

		[SerializeField]
		private int m_SplineIndex;

		[SerializeField]
		private EmbeddedSplineDataType m_Type;

		[SerializeField]
		private string m_Key;
	}
}
