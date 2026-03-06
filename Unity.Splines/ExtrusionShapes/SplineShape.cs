using System;
using Unity.Mathematics;

namespace UnityEngine.Splines.ExtrusionShapes
{
	[Serializable]
	public class SplineShape : IExtrudeShape
	{
		public int SideCount
		{
			get
			{
				return this.m_SideCount;
			}
			set
			{
				this.m_SideCount = value;
			}
		}

		public SplineContainer SplineContainer
		{
			get
			{
				return this.m_Template;
			}
			set
			{
				this.m_Template = value;
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
				this.m_SplineIndex = math.max(0, value);
			}
		}

		public Spline Spline
		{
			get
			{
				if (!(this.m_Template != null))
				{
					return null;
				}
				return this.m_Template[this.m_SplineIndex % this.m_Template.Splines.Count];
			}
		}

		public float2 GetPosition(float t, int index)
		{
			if (this.Spline == null)
			{
				return 0f;
			}
			if (t == 1f)
			{
				t = 0.9999f;
			}
			else if (t == 0f)
			{
				t = 0.0001f;
			}
			float2 result;
			switch (this.m_Axis)
			{
			case SplineShape.Axis.X:
				result = this.Spline.EvaluatePosition(1f - t).zy;
				break;
			case SplineShape.Axis.Y:
				result = this.Spline.EvaluatePosition(1f - t).xz;
				break;
			case SplineShape.Axis.Z:
				result = this.Spline.EvaluatePosition(1f - t).xy;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}

		[SerializeField]
		private SplineContainer m_Template;

		[SerializeField]
		[SplineIndex("m_Template")]
		private int m_SplineIndex;

		[SerializeField]
		[Min(2f)]
		private int m_SideCount = 12;

		[SerializeField]
		[Tooltip("The axis of the template spline to be used when winding the vertices along the extruded mesh.")]
		public SplineShape.Axis m_Axis = SplineShape.Axis.Y;

		public enum Axis
		{
			X,
			Y,
			Z
		}
	}
}
