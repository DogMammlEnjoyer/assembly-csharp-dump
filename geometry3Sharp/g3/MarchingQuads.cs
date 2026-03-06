using System;
using System.Collections;

namespace g3
{
	public class MarchingQuads
	{
		public MarchingQuads(int nSubdivisions, AxisAlignedBox2f bounds, float fIsoValue)
		{
			this.m_stroke = new DPolyLine2f();
			this.m_bounds = default(AxisAlignedBox2f);
			this.m_nCells = nSubdivisions;
			this.SetBounds(bounds);
			this.m_cells = null;
			this.InitializeCells();
			this.m_seedPoints = new ArrayList();
			this.m_cellStack = new ArrayList();
			this.m_bEdgeSigns = new bool[4];
			this.m_fIsoValue = fIsoValue;
		}

		public int Subdivisions
		{
			get
			{
				return this.m_nCells;
			}
			set
			{
				this.m_nCells = value;
				this.SetBounds(this.m_bounds);
				this.InitializeCells();
			}
		}

		public AxisAlignedBox2f Bounds
		{
			get
			{
				return this.m_bounds;
			}
			set
			{
				this.SetBounds(value);
			}
		}

		public DPolyLine2f Stroke
		{
			get
			{
				return this.m_stroke;
			}
		}

		public AxisAlignedBox2f GetBounds()
		{
			return this.m_bounds;
		}

		public void AddSeedPoint(float x, float y)
		{
			this.m_seedPoints.Add(new MarchingQuads.SeedPoint(x - this.m_fXShift, y - this.m_fYShift));
		}

		public void ClearSeedPoints()
		{
			this.m_seedPoints.Clear();
		}

		public void ClearStroke()
		{
			this.m_stroke.Clear();
		}

		public void Polygonize(ImplicitField2d field)
		{
			this.m_field = field;
			this.ResetCells();
			this.m_cellStack.Clear();
			for (int i = 0; i < this.m_seedPoints.Count; i++)
			{
				MarchingQuads.SeedPoint seedPoint = (MarchingQuads.SeedPoint)this.m_seedPoints[i];
				int num = (int)(seedPoint.x / this.m_fCellSize);
				int num2 = (int)(seedPoint.y / this.m_fCellSize);
				bool flag = false;
				while (!flag && num2 > 0 && num2 < this.m_cells.Length - 1 && num > 0)
				{
					if (num >= this.m_cells[0].Length - 1)
					{
						break;
					}
					if (!this.m_cells[num2][num].bTouched)
					{
						if (this.ProcessCell(num, num2))
						{
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
					num--;
				}
				while (this.m_cellStack.Count != 0)
				{
					MarchingQuads.Cell cell = (MarchingQuads.Cell)this.m_cellStack[this.m_cellStack.Count - 1];
					this.m_cellStack.RemoveAt(this.m_cellStack.Count - 1);
					if (!this.m_cells[(int)cell.y][(int)cell.x].bTouched)
					{
						this.ProcessCell((int)cell.x, (int)cell.y);
					}
				}
			}
		}

		private void SubdivideStep(ref float fValue1, ref float fValue2, ref float fX1, ref float fY1, ref float fX2, ref float fY2, bool bVerticalEdge)
		{
			float num = 0.5f;
			float num2;
			float num3;
			if (bVerticalEdge)
			{
				num2 = fX1;
				num3 = num * fY1 + (1f - num) * fY2;
			}
			else
			{
				num2 = num * fX1 + (1f - num) * fX2;
				num3 = fY1;
			}
			float num4 = this.m_field.Value(num2, num3);
			if (num4 < this.m_fIsoValue)
			{
				fValue1 = num4;
				fX1 = num2;
				fY1 = num3;
				return;
			}
			fValue2 = num4;
			fX2 = num2;
			fY2 = num3;
		}

		private int LerpAndAddStrokeVertex(float fValue1, float fValue2, int x1, int y1, int x2, int y2, bool bVerticalEdge)
		{
			if (fValue1 > fValue2)
			{
				int num = x1;
				x1 = x2;
				x2 = num;
				int num2 = y1;
				y1 = y2;
				y2 = num2;
				float num3 = fValue1;
				fValue1 = fValue2;
				fValue2 = num3;
			}
			float value = fValue1;
			float value2 = fValue2;
			float fX = (float)x1 * this.m_fCellSize + this.m_fXShift;
			float fY = (float)y1 * this.m_fCellSize + this.m_fYShift;
			float fX2 = (float)x2 * this.m_fCellSize + this.m_fXShift;
			float fY2 = (float)y2 * this.m_fCellSize + this.m_fYShift;
			for (int i = 0; i < 10; i++)
			{
				this.SubdivideStep(ref value, ref value2, ref fX, ref fY, ref fX2, ref fY2, bVerticalEdge);
			}
			if (Math.Abs(value) < Math.Abs(value2))
			{
				return this.m_stroke.AddVertex(fX, fY);
			}
			return this.m_stroke.AddVertex(fX2, fY2);
		}

		private int GetLeftEdgeVertex(int xi, int yi)
		{
			MarchingQuads.Cell cell = this.m_cells[yi][xi];
			if (cell.nLeftVertex != -1)
			{
				return cell.nLeftVertex;
			}
			this.m_cells[yi][xi].nLeftVertex = this.LerpAndAddStrokeVertex(cell.fValue, this.m_cells[yi + 1][xi].fValue, xi, yi, xi, yi + 1, true);
			return this.m_cells[yi][xi].nLeftVertex;
		}

		private int GetRightEdgeVertex(int xi, int yi)
		{
			MarchingQuads.Cell cell = this.m_cells[yi][xi + 1];
			if (cell.nLeftVertex != -1)
			{
				return cell.nLeftVertex;
			}
			this.m_cells[yi][xi + 1].nLeftVertex = this.LerpAndAddStrokeVertex(cell.fValue, this.m_cells[yi + 1][xi + 1].fValue, xi + 1, yi, xi + 1, yi + 1, true);
			return this.m_cells[yi][xi + 1].nLeftVertex;
		}

		private int GetTopEdgeVertex(int xi, int yi)
		{
			MarchingQuads.Cell cell = this.m_cells[yi][xi];
			if (cell.nTopVertex != -1)
			{
				return cell.nTopVertex;
			}
			this.m_cells[yi][xi].nTopVertex = this.LerpAndAddStrokeVertex(cell.fValue, this.m_cells[yi][xi + 1].fValue, xi, yi, xi + 1, yi, false);
			return this.m_cells[yi][xi].nTopVertex;
		}

		private int GetBottomEdgeVertex(int xi, int yi)
		{
			MarchingQuads.Cell cell = this.m_cells[yi + 1][xi];
			if (cell.nTopVertex != -1)
			{
				return cell.nTopVertex;
			}
			this.m_cells[yi + 1][xi].nTopVertex = this.LerpAndAddStrokeVertex(cell.fValue, this.m_cells[yi + 1][xi + 1].fValue, xi, yi + 1, xi + 1, yi + 1, false);
			return this.m_cells[yi + 1][xi].nTopVertex;
		}

		private bool ProcessCell(int xi, int yi)
		{
			this.m_cells[yi][xi].bTouched = true;
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				int num2 = xi + (i & 1);
				int num3 = yi + (i >> 1 & 1);
				if (this.m_cells[num3][num2].fValue == MarchingQuads.s_fValueSentinel)
				{
					this.m_cells[num3][num2].fValue = this.m_field.Value((float)num2 * this.m_fCellSize + this.m_fXShift, (float)num3 * this.m_fCellSize + this.m_fYShift);
				}
				this.m_bEdgeSigns[i] = (this.m_cells[num3][num2].fValue > this.m_fIsoValue);
				num |= (this.m_bEdgeSigns[i] ? 1 : 0) << i;
			}
			if (num == 0 || num == 15)
			{
				return false;
			}
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			if (this.m_bEdgeSigns[0] != this.m_bEdgeSigns[2])
			{
				num4 = this.GetLeftEdgeVertex(xi, yi);
			}
			if (this.m_bEdgeSigns[1] != this.m_bEdgeSigns[3])
			{
				num5 = this.GetRightEdgeVertex(xi, yi);
			}
			if (this.m_bEdgeSigns[0] != this.m_bEdgeSigns[1])
			{
				num6 = this.GetTopEdgeVertex(xi, yi);
			}
			if (this.m_bEdgeSigns[2] != this.m_bEdgeSigns[3])
			{
				num7 = this.GetBottomEdgeVertex(xi, yi);
			}
			float num8 = 0f;
			if (num == 6 || num == 9)
			{
				num8 = this.m_field.Value((float)xi * this.m_fCellSize + this.m_fCellSize / 2f + this.m_fXShift, (float)yi * this.m_fCellSize + this.m_fCellSize / 2f + this.m_fYShift);
			}
			int num9 = 0;
			switch (num)
			{
			case 1:
			case 14:
				this.m_stroke.AddEdge(num4, num6);
				num9 = (MarchingQuads.LEFT | MarchingQuads.TOP);
				break;
			case 2:
			case 13:
				this.m_stroke.AddEdge(num6, num5);
				num9 = (MarchingQuads.RIGHT | MarchingQuads.TOP);
				break;
			case 3:
			case 12:
				this.m_stroke.AddEdge(num5, num4);
				num9 = (MarchingQuads.LEFT | MarchingQuads.RIGHT);
				break;
			case 4:
			case 11:
				this.m_stroke.AddEdge(num7, num4);
				num9 = (MarchingQuads.LEFT | MarchingQuads.BOTTOM);
				break;
			case 5:
			case 10:
				this.m_stroke.AddEdge(num6, num7);
				num9 = (MarchingQuads.BOTTOM | MarchingQuads.TOP);
				break;
			case 6:
				if (num8 > this.m_fIsoValue)
				{
					this.m_stroke.AddEdge(num4, num6);
					this.m_stroke.AddEdge(num7, num5);
				}
				else
				{
					this.m_stroke.AddEdge(num4, num7);
					this.m_stroke.AddEdge(num6, num5);
				}
				num9 = MarchingQuads.ALL;
				break;
			case 7:
			case 8:
				this.m_stroke.AddEdge(num5, num7);
				num9 = (MarchingQuads.RIGHT | MarchingQuads.BOTTOM);
				break;
			case 9:
				if (num8 > this.m_fIsoValue)
				{
					this.m_stroke.AddEdge(num4, num7);
					this.m_stroke.AddEdge(num6, num5);
				}
				else
				{
					this.m_stroke.AddEdge(num4, num6);
					this.m_stroke.AddEdge(num7, num5);
				}
				num9 = MarchingQuads.ALL;
				break;
			}
			if ((num9 & MarchingQuads.LEFT) != 0 && xi - 1 >= 0 && !this.m_cells[yi][xi - 1].bTouched)
			{
				this.m_cellStack.Add(this.m_cells[yi][xi - 1]);
			}
			if ((num9 & MarchingQuads.RIGHT) != 0 && xi + 1 < this.m_nCells && !this.m_cells[yi][xi + 1].bTouched)
			{
				this.m_cellStack.Add(this.m_cells[yi][xi + 1]);
			}
			if ((num9 & MarchingQuads.BOTTOM) != 0 && yi + 1 < this.m_nCells && !this.m_cells[yi + 1][xi].bTouched)
			{
				this.m_cellStack.Add(this.m_cells[yi + 1][xi]);
			}
			if ((num9 & MarchingQuads.TOP) != 0 && yi - 1 >= 0 && !this.m_cells[yi - 1][xi].bTouched)
			{
				this.m_cellStack.Add(this.m_cells[yi - 1][xi]);
			}
			return true;
		}

		private void ResetCells()
		{
			uint num = 0U;
			while ((ulong)num < (ulong)((long)this.m_cells.Length))
			{
				uint num2 = 0U;
				while ((ulong)num2 < (ulong)((long)this.m_cells.Length))
				{
					this.m_cells[(int)num][(int)num2].bTouched = false;
					this.m_cells[(int)num][(int)num2].nLeftVertex = (this.m_cells[(int)num][(int)num2].nTopVertex = -1);
					num2 += 1U;
				}
				num += 1U;
			}
		}

		private void InitializeCells()
		{
			this.m_cells = new MarchingQuads.Cell[this.m_nCells + 1][];
			uint num = 0U;
			while ((ulong)num < (ulong)((long)this.m_cells.Length))
			{
				this.m_cells[(int)num] = new MarchingQuads.Cell[this.m_nCells + 1];
				uint num2 = 0U;
				while ((ulong)num2 < (ulong)((long)this.m_cells.Length))
				{
					this.m_cells[(int)num][(int)num2].Initialize(num2, num);
					num2 += 1U;
				}
				num += 1U;
			}
		}

		private void SetBounds(AxisAlignedBox2f bounds)
		{
			this.m_bounds = bounds;
			this.m_fXShift = ((bounds.Min.x < 0f) ? bounds.Min.x : (-bounds.Min.x));
			this.m_fYShift = ((bounds.Min.y < 0f) ? bounds.Min.y : (-bounds.Min.y));
			this.m_fScale = ((bounds.Width > bounds.Height) ? bounds.Width : bounds.Height);
			this.m_fCellSize = this.m_fScale / (float)this.m_nCells;
		}

		private DPolyLine2f m_stroke;

		private AxisAlignedBox2f m_bounds;

		private float m_fXShift;

		private float m_fYShift;

		private float m_fScale;

		private int m_nCells;

		private float m_fCellSize;

		private static float s_fValueSentinel = 9999999f;

		private float m_fIsoValue;

		private static int LEFT = 1;

		private static int TOP = 2;

		private static int RIGHT = 4;

		private static int BOTTOM = 8;

		private static int ALL = 15;

		private MarchingQuads.Cell[][] m_cells;

		private ArrayList m_seedPoints;

		private ImplicitField2d m_field;

		private ArrayList m_cellStack;

		private bool[] m_bEdgeSigns;

		private struct Cell
		{
			public void Initialize(uint x, uint y)
			{
				this.x = x;
				this.y = y;
				this.fValue = MarchingQuads.s_fValueSentinel;
				this.nLeftVertex = (this.nTopVertex = -1);
				this.bTouched = false;
			}

			public uint x
			{
				get
				{
					return this.nPosition & 65535U;
				}
				set
				{
					this.nPosition = (this.y << 16 | (value & 65535U));
				}
			}

			public uint y
			{
				get
				{
					return this.nPosition >> 16 & 65535U;
				}
				set
				{
					this.nPosition = ((value & 65535U) << 16 | this.x);
				}
			}

			private uint nPosition;

			public float fValue;

			public int nLeftVertex;

			public int nTopVertex;

			public bool bTouched;
		}

		private struct SeedPoint
		{
			public SeedPoint(float fX, float fY)
			{
				this.x = fX;
				this.y = fY;
			}

			public float x;

			public float y;
		}
	}
}
