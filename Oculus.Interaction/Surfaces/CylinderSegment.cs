using System;
using UnityEngine;

namespace Oculus.Interaction.Surfaces
{
	[Serializable]
	public struct CylinderSegment
	{
		public float ArcDegrees
		{
			get
			{
				return this._arcDegrees;
			}
		}

		public float Rotation
		{
			get
			{
				return this._rotation;
			}
		}

		public float Bottom
		{
			get
			{
				return this._bottom;
			}
		}

		public float Top
		{
			get
			{
				return this._top;
			}
		}

		public bool IsInfiniteHeight
		{
			get
			{
				return this.Bottom > this.Top;
			}
		}

		public bool IsInfiniteArc
		{
			get
			{
				return this.ArcDegrees >= 360f;
			}
		}

		public CylinderSegment(float rotation, float arcDegrees, float bottom, float top)
		{
			this._rotation = rotation;
			this._arcDegrees = arcDegrees;
			this._bottom = bottom;
			this._top = top;
		}

		public static CylinderSegment Default()
		{
			return new CylinderSegment
			{
				_rotation = 0f,
				_arcDegrees = 360f,
				_bottom = -1f,
				_top = 1f
			};
		}

		public static CylinderSegment Infinite()
		{
			return new CylinderSegment
			{
				_rotation = 0f,
				_arcDegrees = 360f,
				_bottom = 1f,
				_top = -1f
			};
		}

		[SerializeField]
		[Range(-180f, 180f)]
		private float _rotation;

		[SerializeField]
		[Range(0f, 360f)]
		private float _arcDegrees;

		[SerializeField]
		private float _bottom;

		[SerializeField]
		private float _top;
	}
}
