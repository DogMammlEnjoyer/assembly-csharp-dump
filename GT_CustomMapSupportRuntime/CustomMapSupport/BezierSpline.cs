using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CustomMapSupport
{
	[NullableContext(2)]
	[Nullable(0)]
	public class BezierSpline : MonoBehaviour
	{
		private void Awake()
		{
			float num = 0f;
			int num2 = 1;
			for (;;)
			{
				int num3 = num2;
				Vector3[] array = this.points;
				int? num4 = (array != null) ? new int?(array.Length) : null;
				if (!(num3 < num4.GetValueOrDefault() & num4 != null))
				{
					break;
				}
				num += (this.points[num2] - this.points[num2 - 1]).magnitude;
				num2++;
			}
			int subdivisions = Mathf.RoundToInt(num / 0.1f);
			this.buildTimesLengthsTables(subdivisions);
		}

		private void buildTimesLengthsTables(int subdivisions)
		{
			this._totalArcLength = 0f;
			float num = 1f / (float)subdivisions;
			this._timesTable = new float[subdivisions];
			this._lengthsTable = new float[subdivisions];
			Vector3 b = this.GetPoint(0f);
			for (int i = 1; i <= subdivisions; i++)
			{
				float num2 = num * (float)i;
				Vector3 point = this.GetPoint(num2);
				this._totalArcLength += Vector3.Distance(point, b);
				b = point;
				this._timesTable[i - 1] = num2;
				this._lengthsTable[i - 1] = this._totalArcLength;
			}
		}

		private float getPathFromTime(float t)
		{
			if (float.IsNaN(this._totalArcLength) || this._totalArcLength == 0f)
			{
				return t;
			}
			if (t > 0f && t < 1f)
			{
				float num = this._totalArcLength * t;
				float num2 = 0f;
				float num3 = 0f;
				float num4 = 0f;
				float num5 = 0f;
				int num6 = 0;
				for (;;)
				{
					int num7 = num6;
					float[] lengthsTable = this._lengthsTable;
					int? num8 = (lengthsTable != null) ? new int?(lengthsTable.Length) : null;
					if (!(num7 < num8.GetValueOrDefault() & num8 != null))
					{
						goto IL_F2;
					}
					if (this._lengthsTable[num6] > num)
					{
						break;
					}
					num2 = ((this._timesTable == null) ? 0f : this._timesTable[num6]);
					num6++;
				}
				num4 = ((this._timesTable == null) ? 0f : this._timesTable[num6]);
				num5 = this._lengthsTable[num6];
				if (num6 > 0)
				{
					num3 = this._lengthsTable[num6 - 1];
				}
				IL_F2:
				t = num2 + (num - num3) / (num5 - num3) * (num4 - num2);
			}
			if (t > 1f)
			{
				t = 1f;
			}
			else if (t < 0f)
			{
				t = 0f;
			}
			return t;
		}

		public bool Loop
		{
			get
			{
				return this.loop;
			}
			set
			{
				this.loop = value;
				if (value)
				{
					if (this.modes != null)
					{
						this.modes[this.modes.Length - 1] = this.modes[0];
					}
					if (this.points != null)
					{
						this.SetControlPoint(0, this.points[0]);
					}
				}
			}
		}

		public Vector3[] GetControlPoints()
		{
			return this.points;
		}

		public BezierControlPointMode[] GetControlPointModes()
		{
			return this.modes;
		}

		public int ControlPointCount
		{
			get
			{
				Vector3[] array = this.points;
				if (array == null)
				{
					return 0;
				}
				return array.Length;
			}
		}

		public Vector3 GetControlPoint(int index)
		{
			if (this.points == null)
			{
				return Vector3.zero;
			}
			return this.points[index];
		}

		public void SetControlPoint(int index, Vector3 point)
		{
			if (this.points == null || this.points.Length <= index)
			{
				return;
			}
			if (index % 3 == 0)
			{
				Vector3 b = point - this.points[index];
				if (this.loop)
				{
					if (index == 0)
					{
						this.points[1] += b;
						this.points[this.points.Length - 2] += b;
						this.points[this.points.Length - 1] = point;
					}
					else if (index == this.points.Length - 1)
					{
						this.points[0] = point;
						this.points[1] += b;
						this.points[index - 1] += b;
					}
					else
					{
						this.points[index - 1] += b;
						this.points[index + 1] += b;
					}
				}
				else
				{
					if (index > 0)
					{
						this.points[index - 1] += b;
					}
					if (index + 1 < this.points.Length)
					{
						this.points[index + 1] += b;
					}
				}
			}
			this.points[index] = point;
			this.EnforceMode(index);
		}

		public BezierControlPointMode GetControlPointMode(int index)
		{
			if (this.modes == null)
			{
				return BezierControlPointMode.Free;
			}
			return this.modes[(index + 1) / 3];
		}

		public void SetControlPointMode(int index, BezierControlPointMode mode)
		{
			if (this.modes == null)
			{
				return;
			}
			int num = (index + 1) / 3;
			this.modes[num] = mode;
			if (this.loop)
			{
				if (num == 0)
				{
					this.modes[this.modes.Length - 1] = mode;
				}
				else if (num == this.modes.Length - 1)
				{
					this.modes[0] = mode;
				}
			}
			this.EnforceMode(index);
		}

		private void EnforceMode(int index)
		{
			if (this.modes == null || this.points == null)
			{
				return;
			}
			int num = (index + 1) / 3;
			BezierControlPointMode bezierControlPointMode = this.modes[num];
			if (bezierControlPointMode == BezierControlPointMode.Free || (!this.loop && (num == 0 || num == this.modes.Length - 1)))
			{
				return;
			}
			int num2 = num * 3;
			int num3;
			int num4;
			if (index <= num2)
			{
				num3 = num2 - 1;
				if (num3 < 0)
				{
					num3 = this.points.Length - 2;
				}
				num4 = num2 + 1;
				if (num4 >= this.points.Length)
				{
					num4 = 1;
				}
			}
			else
			{
				num3 = num2 + 1;
				if (num3 >= this.points.Length)
				{
					num3 = 1;
				}
				num4 = num2 - 1;
				if (num4 < 0)
				{
					num4 = this.points.Length - 2;
				}
			}
			Vector3 a = this.points[num2];
			Vector3 b = a - this.points[num3];
			if (bezierControlPointMode == BezierControlPointMode.Aligned)
			{
				b = b.normalized * Vector3.Distance(a, this.points[num4]);
			}
			this.points[num4] = a + b;
		}

		public int CurveCount
		{
			get
			{
				if (this.points != null)
				{
					return (this.points.Length - 1) / 3;
				}
				return 0;
			}
		}

		public Vector3 GetPoint(float t, bool ConstantVelocity)
		{
			if (ConstantVelocity)
			{
				return this.GetPoint(this.getPathFromTime(t));
			}
			return this.GetPoint(t);
		}

		public Vector3 GetPoint(float t)
		{
			if (this.points == null)
			{
				return Vector3.zero;
			}
			int num;
			if (t >= 1f)
			{
				t = 1f;
				num = this.points.Length - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * (float)this.CurveCount;
				num = (int)t;
				t -= (float)num;
				num *= 3;
			}
			return base.transform.TransformPoint(Bezier.GetPoint(this.points[num], this.points[num + 1], this.points[num + 2], this.points[num + 3], t));
		}

		public Vector3 GetPointLocal(float t)
		{
			if (this.points == null)
			{
				return Vector3.zero;
			}
			int num;
			if (t >= 1f)
			{
				t = 1f;
				num = this.points.Length - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * (float)this.CurveCount;
				num = (int)t;
				t -= (float)num;
				num *= 3;
			}
			return Bezier.GetPoint(this.points[num], this.points[num + 1], this.points[num + 2], this.points[num + 3], t);
		}

		private Vector3 GetVelocity(float t)
		{
			if (this.points == null)
			{
				return Vector3.zero;
			}
			int num;
			if (t >= 1f)
			{
				t = 1f;
				num = this.points.Length - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * (float)this.CurveCount;
				num = (int)t;
				t -= (float)num;
				num *= 3;
			}
			return base.transform.TransformPoint(Bezier.GetFirstDerivative(this.points[num], this.points[num + 1], this.points[num + 2], this.points[num + 3], t)) - base.transform.position;
		}

		public Vector3 GetDirection(float t, bool ConstantVelocity)
		{
			if (ConstantVelocity)
			{
				return this.GetDirection(this.getPathFromTime(t));
			}
			return this.GetDirection(t);
		}

		public Vector3 GetDirection(float t)
		{
			return this.GetVelocity(t).normalized;
		}

		public void AddCurve()
		{
			if (this.modes == null || this.points == null)
			{
				return;
			}
			Vector3 vector = this.points[this.points.Length - 1];
			Array.Resize<Vector3>(ref this.points, this.points.Length + 3);
			vector.x += 1f;
			this.points[this.points.Length - 3] = vector;
			vector.x += 1f;
			this.points[this.points.Length - 2] = vector;
			vector.x += 1f;
			this.points[this.points.Length - 1] = vector;
			Array.Resize<BezierControlPointMode>(ref this.modes, this.modes.Length + 1);
			this.modes[this.modes.Length - 1] = this.modes[this.modes.Length - 2];
			this.EnforceMode(this.points.Length - 4);
			if (this.loop)
			{
				this.points[this.points.Length - 1] = this.points[0];
				this.modes[this.modes.Length - 1] = this.modes[0];
				this.EnforceMode(0);
			}
		}

		public void RemoveLastCurve()
		{
			if (this.modes == null || this.points == null)
			{
				return;
			}
			if (this.points.Length <= 4)
			{
				return;
			}
			Array.Resize<Vector3>(ref this.points, this.points.Length - 3);
			Array.Resize<BezierControlPointMode>(ref this.modes, this.modes.Length - 1);
		}

		public void RemoveCurve(int index)
		{
			if (this.modes == null || this.points == null)
			{
				return;
			}
			if (this.points.Length <= 4)
			{
				return;
			}
			List<Vector3> list = this.points.ToList<Vector3>();
			int num = 4;
			while (num < this.points.Length && index - 3 > num)
			{
				num += 3;
			}
			for (int i = 0; i < 3; i++)
			{
				list.RemoveAt(num);
			}
			this.points = list.ToArray();
			int index2 = (num - 4) / 3;
			List<BezierControlPointMode> list2 = this.modes.ToList<BezierControlPointMode>();
			list2.RemoveAt(index2);
			this.modes = list2.ToArray();
		}

		public void Reset()
		{
			this.points = new Vector3[]
			{
				new Vector3(0f, -1f, 0f),
				new Vector3(0f, -1f, 2f),
				new Vector3(0f, -1f, 4f),
				new Vector3(0f, -1f, 6f)
			};
			this.modes = new BezierControlPointMode[2];
		}

		public Vector3[] points;

		public BezierControlPointMode[] modes;

		public bool loop;

		private float _totalArcLength;

		private float[] _timesTable;

		private float[] _lengthsTable;
	}
}
