using System;
using UnityEngine;

namespace Oculus.Interaction.Surfaces
{
	public class CylinderClipper : MonoBehaviour, ICylinderClipper
	{
		public float ArcDegrees
		{
			get
			{
				return this._arcDegrees;
			}
			set
			{
				this._arcDegrees = value;
			}
		}

		public float Rotation
		{
			get
			{
				return this._rotation;
			}
			set
			{
				this._rotation = value;
			}
		}

		public float Bottom
		{
			get
			{
				return this._bottom;
			}
			set
			{
				this._bottom = value;
			}
		}

		public float Top
		{
			get
			{
				return this._top;
			}
			set
			{
				this._top = value;
			}
		}

		public bool GetCylinderSegment(out CylinderSegment segment)
		{
			segment = new CylinderSegment(this._rotation, this._arcDegrees, this._bottom, this._top);
			return base.isActiveAndEnabled;
		}

		[Tooltip("The rotation of the center of the clip area around the y axis, in degrees.")]
		[SerializeField]
		[Range(-180f, 180f)]
		private float _rotation;

		[Tooltip("The arc degrees of the clip area, centered at the rotation value.")]
		[SerializeField]
		[Range(0f, 360f)]
		private float _arcDegrees = 360f;

		[Tooltip("The bottom extent of the clip area, along the y axis.")]
		[SerializeField]
		private float _bottom = -1f;

		[Tooltip("The top extent of the clip area, along the y axis.")]
		[SerializeField]
		private float _top = 1f;
	}
}
