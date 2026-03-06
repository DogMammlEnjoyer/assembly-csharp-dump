using System;
using UnityEngine;

namespace Meta.XR.MRUtilityKit
{
	internal struct Float3X3
	{
		internal Float3X3(Vector3 row0, Vector3 row1, Vector3 row2)
		{
			this.Row0 = row0;
			this.Row1 = row1;
			this.Row2 = row2;
		}

		internal Float3X3(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22)
		{
			this.Row0 = new Vector3(m00, m01, m02);
			this.Row1 = new Vector3(m10, m11, m12);
			this.Row2 = new Vector3(m20, m21, m22);
		}

		internal static Float3X3 Multiply(Float3X3 a, Float3X3 b)
		{
			Float3X3 result = default(Float3X3);
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					result[i, j] = a[i, 0] * b[0, j] + a[i, 1] * b[1, j] + a[i, 2] * b[2, j];
				}
			}
			return result;
		}

		internal static Vector3 Multiply(Float3X3 a, Vector3 b)
		{
			return new Vector3(Vector3.Dot(a.Row0, b), Vector3.Dot(a.Row1, b), Vector3.Dot(a.Row2, b));
		}

		private float this[int row, int column]
		{
			get
			{
				switch (row)
				{
				case 0:
					return this.Row0[column];
				case 1:
					return this.Row1[column];
				case 2:
					return this.Row2[column];
				default:
					throw new IndexOutOfRangeException("Row index out of range: " + row.ToString());
				}
			}
			set
			{
				switch (row)
				{
				case 0:
					this.Row0[column] = value;
					return;
				case 1:
					this.Row1[column] = value;
					return;
				case 2:
					this.Row2[column] = value;
					return;
				default:
					throw new IndexOutOfRangeException("Row index out of range: " + row.ToString());
				}
			}
		}

		private Vector3 Row0;

		private Vector3 Row1;

		private Vector3 Row2;
	}
}
