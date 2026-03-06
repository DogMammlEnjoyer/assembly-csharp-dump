using System;
using UnityEngine.Serialization;

namespace UnityEngine.ProBuilder
{
	[Serializable]
	public sealed class UnwrapParameters
	{
		public float hardAngle
		{
			get
			{
				return this.m_HardAngle;
			}
			set
			{
				this.m_HardAngle = value;
			}
		}

		public float packMargin
		{
			get
			{
				return this.m_PackMargin;
			}
			set
			{
				this.m_PackMargin = value;
			}
		}

		public float angleError
		{
			get
			{
				return this.m_AngleError;
			}
			set
			{
				this.m_AngleError = value;
			}
		}

		public float areaError
		{
			get
			{
				return this.m_AreaError;
			}
			set
			{
				this.m_AreaError = value;
			}
		}

		public UnwrapParameters()
		{
			this.Reset();
		}

		public UnwrapParameters(UnwrapParameters other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this.hardAngle = other.hardAngle;
			this.packMargin = other.packMargin;
			this.angleError = other.angleError;
			this.areaError = other.areaError;
		}

		public void Reset()
		{
			this.hardAngle = 88f;
			this.packMargin = 20f;
			this.angleError = 8f;
			this.areaError = 15f;
		}

		public override string ToString()
		{
			return string.Format("hardAngle: {0}\npackMargin: {1}\nangleError: {2}\nareaError: {3}", new object[]
			{
				this.hardAngle,
				this.packMargin,
				this.angleError,
				this.areaError
			});
		}

		internal const float k_HardAngle = 88f;

		internal const float k_PackMargin = 20f;

		internal const float k_AngleError = 8f;

		internal const float k_AreaError = 15f;

		[Tooltip("Angle between neighbor triangles that will generate seam.")]
		[Range(1f, 180f)]
		[SerializeField]
		[FormerlySerializedAs("hardAngle")]
		private float m_HardAngle = 88f;

		[Tooltip("Measured in pixels, assuming mesh will cover an entire 1024x1024 lightmap.")]
		[Range(1f, 64f)]
		[SerializeField]
		[FormerlySerializedAs("packMargin")]
		private float m_PackMargin = 20f;

		[Tooltip("Measured in percents. Angle error measures deviation of UV angles from geometry angles. Area error measures deviation of UV triangles area from geometry triangles if they were uniformly scaled.")]
		[Range(1f, 75f)]
		[SerializeField]
		[FormerlySerializedAs("angleError")]
		private float m_AngleError = 8f;

		[Range(1f, 75f)]
		[SerializeField]
		[FormerlySerializedAs("areaError")]
		private float m_AreaError = 15f;
	}
}
