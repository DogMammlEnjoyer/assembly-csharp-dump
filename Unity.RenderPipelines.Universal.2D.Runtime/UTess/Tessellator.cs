using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal.UTess
{
	internal struct Tessellator
	{
		private static float FindSplit(UHull hull, UEvent edge)
		{
			float num;
			if (hull.a.x < edge.a.x)
			{
				num = ModuleHandle.OrientFast(hull.a, hull.b, edge.a);
			}
			else
			{
				num = ModuleHandle.OrientFast(edge.b, edge.a, hull.a);
			}
			if (0f != num)
			{
				return num;
			}
			if (edge.b.x < hull.b.x)
			{
				num = ModuleHandle.OrientFast(hull.a, hull.b, edge.b);
			}
			else
			{
				num = ModuleHandle.OrientFast(edge.b, edge.a, hull.b);
			}
			if (0f != num)
			{
				return num;
			}
			return (float)(hull.idx - edge.idx);
		}

		private void SetAllocator(Allocator allocator)
		{
			this.m_Allocator = allocator;
		}

		private bool AddPoint(NativeArray<UHull> hulls, int hullCount, NativeArray<float2> points, float2 p, int idx)
		{
			int lower = ModuleHandle.GetLower<UHull, float2, Tessellator.TestHullPointL>(hulls, hullCount, p, default(Tessellator.TestHullPointL));
			int upper = ModuleHandle.GetUpper<UHull, float2, Tessellator.TestHullPointU>(hulls, hullCount, p, default(Tessellator.TestHullPointU));
			if (lower < 0 || upper < 0)
			{
				return false;
			}
			for (int i = lower; i < upper; i++)
			{
				UHull uhull = hulls[i];
				int num = uhull.ilcount;
				while (num > 1 && ModuleHandle.OrientFast(points[uhull.ilarray[num - 2]], points[uhull.ilarray[num - 1]], p) > 0f)
				{
					int3 value = default(int3);
					value.x = uhull.ilarray[num - 1];
					value.y = uhull.ilarray[num - 2];
					value.z = idx;
					int cellCount = this.m_CellCount;
					this.m_CellCount = cellCount + 1;
					this.m_Cells[cellCount] = value;
					num--;
				}
				uhull.ilcount = num + 1;
				if (uhull.ilcount > uhull.ilarray.Length)
				{
					return false;
				}
				uhull.ilarray[num] = idx;
				num = uhull.iucount;
				while (num > 1 && ModuleHandle.OrientFast(points[uhull.iuarray[num - 2]], points[uhull.iuarray[num - 1]], p) < 0f)
				{
					int3 value2 = default(int3);
					value2.x = uhull.iuarray[num - 2];
					value2.y = uhull.iuarray[num - 1];
					value2.z = idx;
					int cellCount = this.m_CellCount;
					this.m_CellCount = cellCount + 1;
					this.m_Cells[cellCount] = value2;
					num--;
				}
				uhull.iucount = num + 1;
				if (uhull.iucount > uhull.iuarray.Length)
				{
					return false;
				}
				uhull.iuarray[num] = idx;
				hulls[i] = uhull;
			}
			return true;
		}

		private static void InsertHull(NativeArray<UHull> Hulls, int Pos, ref int Count, UHull Value)
		{
			if (Count < Hulls.Length - 1)
			{
				for (int i = Count; i > Pos; i--)
				{
					Hulls[i] = Hulls[i - 1];
				}
				Hulls[Pos] = Value;
				Count++;
			}
		}

		private static void EraseHull(NativeArray<UHull> Hulls, int Pos, ref int Count)
		{
			if (Count < Hulls.Length)
			{
				for (int i = Pos; i < Count - 1; i++)
				{
					Hulls[i] = Hulls[i + 1];
				}
				Count--;
			}
		}

		private bool SplitHulls(NativeArray<UHull> hulls, ref int hullCount, NativeArray<float2> points, UEvent evt)
		{
			int lower = ModuleHandle.GetLower<UHull, UEvent, Tessellator.TestHullEventLe>(hulls, hullCount, evt, default(Tessellator.TestHullEventLe));
			if (lower < 0)
			{
				return false;
			}
			UHull uhull = hulls[lower];
			UHull uhull2;
			uhull2.a = evt.a;
			uhull2.b = evt.b;
			uhull2.idx = evt.idx;
			int value = uhull.iuarray[uhull.iucount - 1];
			uhull2.iuarray = new ArraySlice<int>(this.m_IUArray, uhull2.idx * this.m_NumHulls, this.m_NumHulls);
			uhull2.iucount = uhull.iucount;
			for (int i = 0; i < uhull2.iucount; i++)
			{
				uhull2.iuarray[i] = uhull.iuarray[i];
			}
			uhull.iuarray[0] = value;
			uhull.iucount = 1;
			hulls[lower] = uhull;
			uhull2.ilarray = new ArraySlice<int>(this.m_ILArray, uhull2.idx * this.m_NumHulls, this.m_NumHulls);
			uhull2.ilarray[0] = value;
			uhull2.ilcount = 1;
			Tessellator.InsertHull(hulls, lower + 1, ref hullCount, uhull2);
			return true;
		}

		private bool MergeHulls(NativeArray<UHull> hulls, ref int hullCount, NativeArray<float2> points, UEvent evt)
		{
			float2 a = evt.a;
			evt.a = evt.b;
			evt.b = a;
			int equal = ModuleHandle.GetEqual<UHull, UEvent, Tessellator.TestHullEventE>(hulls, hullCount, evt, default(Tessellator.TestHullEventE));
			if (equal < 0)
			{
				return false;
			}
			UHull uhull = hulls[equal];
			UHull uhull2 = hulls[equal - 1];
			uhull2.iucount = uhull.iucount;
			for (int i = 0; i < uhull2.iucount; i++)
			{
				uhull2.iuarray[i] = uhull.iuarray[i];
			}
			hulls[equal - 1] = uhull2;
			Tessellator.EraseHull(hulls, equal, ref hullCount);
			return true;
		}

		private static void InsertUniqueEdge(NativeArray<int2> edges, int2 e, ref int edgeCount)
		{
			TessEdgeCompare tessEdgeCompare = default(TessEdgeCompare);
			bool flag = true;
			int num = 0;
			while (flag && num < edgeCount)
			{
				if (tessEdgeCompare.Compare(e, edges[num]) == 0)
				{
					flag = false;
				}
				num++;
			}
			if (flag)
			{
				int num2 = edgeCount;
				edgeCount = num2 + 1;
				edges[num2] = e;
			}
		}

		private void PrepareDelaunay(NativeArray<int2> edges, int edgeCount)
		{
			this.m_StarCount = this.m_CellCount * 3;
			this.m_Stars = new NativeArray<UStar>(this.m_StarCount, this.m_Allocator, NativeArrayOptions.ClearMemory);
			this.m_SPArray = new NativeArray<int>(this.m_StarCount * this.m_StarCount, this.m_Allocator, NativeArrayOptions.ClearMemory);
			int num = 0;
			NativeArray<int2> edges2 = new NativeArray<int2>(this.m_StarCount, this.m_Allocator, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < edgeCount; i++)
			{
				int2 @int = edges[i];
				@int.x = ((edges[i].x < edges[i].y) ? edges[i].x : edges[i].y);
				@int.y = ((edges[i].x > edges[i].y) ? edges[i].x : edges[i].y);
				edges[i] = @int;
				Tessellator.InsertUniqueEdge(edges2, @int, ref num);
			}
			this.m_Edges = new NativeArray<int2>(num, this.m_Allocator, NativeArrayOptions.ClearMemory);
			for (int j = 0; j < num; j++)
			{
				this.m_Edges[j] = edges2[j];
			}
			edges2.Dispose();
			ModuleHandle.InsertionSort<int2, TessEdgeCompare>(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int2>(this.m_Edges), 0, this.m_Edges.Length - 1, default(TessEdgeCompare));
			for (int k = 0; k < this.m_StarCount; k++)
			{
				UStar value = this.m_Stars[k];
				value.points = new ArraySlice<int>(this.m_SPArray, k * this.m_StarCount, this.m_StarCount);
				value.pointCount = 0;
				this.m_Stars[k] = value;
			}
			for (int l = 0; l < this.m_CellCount; l++)
			{
				int x = this.m_Cells[l].x;
				int y = this.m_Cells[l].y;
				int z = this.m_Cells[l].z;
				UStar value2 = this.m_Stars[x];
				UStar value3 = this.m_Stars[y];
				UStar value4 = this.m_Stars[z];
				int pointCount = value2.pointCount;
				value2.pointCount = pointCount + 1;
				value2.points[pointCount] = y;
				pointCount = value2.pointCount;
				value2.pointCount = pointCount + 1;
				value2.points[pointCount] = z;
				pointCount = value3.pointCount;
				value3.pointCount = pointCount + 1;
				value3.points[pointCount] = z;
				pointCount = value3.pointCount;
				value3.pointCount = pointCount + 1;
				value3.points[pointCount] = x;
				pointCount = value4.pointCount;
				value4.pointCount = pointCount + 1;
				value4.points[pointCount] = x;
				pointCount = value4.pointCount;
				value4.pointCount = pointCount + 1;
				value4.points[pointCount] = y;
				this.m_Stars[x] = value2;
				this.m_Stars[y] = value3;
				this.m_Stars[z] = value4;
			}
		}

		private int OppositeOf(int a, int b)
		{
			ArraySlice<int> points = this.m_Stars[b].points;
			int i = 1;
			int pointCount = this.m_Stars[b].pointCount;
			while (i < pointCount)
			{
				if (points[i] == a)
				{
					return points[i - 1];
				}
				i += 2;
			}
			return -1;
		}

		private int FindConstraint(int a, int b)
		{
			int2 check;
			check.x = ((a < b) ? a : b);
			check.y = ((a > b) ? a : b);
			return ModuleHandle.GetEqual<int2, int2, Tessellator.TestEdgePointE>(this.m_Edges, this.m_Edges.Length, check, default(Tessellator.TestEdgePointE));
		}

		private void AddTriangle(int i, int j, int k)
		{
			UStar value = this.m_Stars[i];
			UStar value2 = this.m_Stars[j];
			UStar value3 = this.m_Stars[k];
			int pointCount = value.pointCount;
			value.pointCount = pointCount + 1;
			value.points[pointCount] = j;
			pointCount = value.pointCount;
			value.pointCount = pointCount + 1;
			value.points[pointCount] = k;
			pointCount = value2.pointCount;
			value2.pointCount = pointCount + 1;
			value2.points[pointCount] = k;
			pointCount = value2.pointCount;
			value2.pointCount = pointCount + 1;
			value2.points[pointCount] = i;
			pointCount = value3.pointCount;
			value3.pointCount = pointCount + 1;
			value3.points[pointCount] = i;
			pointCount = value3.pointCount;
			value3.pointCount = pointCount + 1;
			value3.points[pointCount] = j;
			this.m_Stars[i] = value;
			this.m_Stars[j] = value2;
			this.m_Stars[k] = value3;
		}

		private void RemovePair(int r, int j, int k)
		{
			UStar ustar = this.m_Stars[r];
			ArraySlice<int> points = ustar.points;
			int i = 1;
			int pointCount = ustar.pointCount;
			while (i < pointCount)
			{
				if (points[i - 1] == j && points[i] == k)
				{
					points[i - 1] = points[pointCount - 2];
					points[i] = points[pointCount - 1];
					ustar.points = points;
					ustar.pointCount -= 2;
					this.m_Stars[r] = ustar;
					return;
				}
				i += 2;
			}
		}

		private void RemoveTriangle(int i, int j, int k)
		{
			this.RemovePair(i, j, k);
			this.RemovePair(j, k, i);
			this.RemovePair(k, i, j);
		}

		private void EdgeFlip(int i, int j)
		{
			int num = this.OppositeOf(i, j);
			int num2 = this.OppositeOf(j, i);
			this.RemoveTriangle(i, j, num);
			this.RemoveTriangle(j, i, num2);
			this.AddTriangle(i, num2, num);
			this.AddTriangle(j, num, num2);
		}

		private bool Flip(NativeArray<float2> points, ref NativeArray<int> stack, ref int stackCount, int a, int b, int x)
		{
			int num = this.OppositeOf(a, b);
			if (num < 0)
			{
				return true;
			}
			if (b < a)
			{
				int num2 = a;
				a = b;
				b = num2;
				int num3 = x;
				x = num;
				num = num3;
			}
			if (this.FindConstraint(a, b) != -1)
			{
				return true;
			}
			if (ModuleHandle.IsInsideCircle(points[a], points[b], points[x], points[num]))
			{
				if (2 + stackCount >= stack.Length)
				{
					return false;
				}
				int num4 = stackCount;
				stackCount = num4 + 1;
				stack[num4] = a;
				num4 = stackCount;
				stackCount = num4 + 1;
				stack[num4] = b;
			}
			return true;
		}

		private NativeArray<int3> GetCells(ref int count)
		{
			NativeArray<int3> result = new NativeArray<int3>(this.m_NumPoints * (this.m_NumPoints + 1), this.m_Allocator, NativeArrayOptions.ClearMemory);
			count = 0;
			int i = 0;
			int length = this.m_Stars.Length;
			while (i < length)
			{
				ArraySlice<int> points = this.m_Stars[i].points;
				int j = 0;
				int pointCount = this.m_Stars[i].pointCount;
				while (j < pointCount)
				{
					int num = points[j];
					int num2 = points[j + 1];
					if (i < math.min(num, num2))
					{
						int3 value = default(int3);
						value.x = i;
						value.y = num;
						value.z = num2;
						int num3 = count;
						count = num3 + 1;
						result[num3] = value;
					}
					j += 2;
				}
				i++;
			}
			return result;
		}

		internal bool ApplyDelaunay(NativeArray<float2> points, NativeArray<int2> edges)
		{
			if (this.m_CellCount == 0)
			{
				return false;
			}
			NativeArray<int> nativeArray = new NativeArray<int>(this.m_NumPoints * (this.m_NumPoints + 1), this.m_Allocator, NativeArrayOptions.ClearMemory);
			int num = 0;
			bool flag = true;
			this.PrepareDelaunay(edges, this.m_NumEdges);
			int num2 = 0;
			while (flag && num2 < this.m_NumPoints)
			{
				UStar ustar = this.m_Stars[num2];
				for (int i = 1; i < ustar.pointCount; i += 2)
				{
					int num3 = ustar.points[i];
					if (num3 >= num2 && this.FindConstraint(num2, num3) < 0)
					{
						int index = ustar.points[i - 1];
						int num4 = -1;
						for (int j = 1; j < ustar.pointCount; j += 2)
						{
							if (ustar.points[j - 1] == num3)
							{
								num4 = ustar.points[j];
								break;
							}
						}
						if (num4 >= 0 && ModuleHandle.IsInsideCircle(points[num2], points[num3], points[index], points[num4]))
						{
							if (2 + num >= nativeArray.Length)
							{
								flag = false;
								break;
							}
							nativeArray[num++] = num2;
							nativeArray[num++] = num3;
						}
					}
				}
				num2++;
			}
			int num5 = this.m_NumPoints * this.m_NumPoints;
			while (num > 0 && flag)
			{
				int num6 = nativeArray[num - 1];
				num--;
				int num7 = nativeArray[num - 1];
				num--;
				int num8 = -1;
				int num9 = -1;
				UStar ustar2 = this.m_Stars[num7];
				for (int k = 1; k < ustar2.pointCount; k += 2)
				{
					int num10 = ustar2.points[k - 1];
					int num11 = ustar2.points[k];
					if (num10 == num6)
					{
						num9 = num11;
					}
					else if (num11 == num6)
					{
						num8 = num10;
					}
				}
				if (num8 >= 0 && num9 >= 0 && ModuleHandle.IsInsideCircle(points[num7], points[num6], points[num8], points[num9]))
				{
					this.EdgeFlip(num7, num6);
					flag = this.Flip(points, ref nativeArray, ref num, num8, num7, num9);
					flag = (flag && this.Flip(points, ref nativeArray, ref num, num7, num9, num8));
					flag = (flag && this.Flip(points, ref nativeArray, ref num, num9, num6, num8));
					flag = (flag && this.Flip(points, ref nativeArray, ref num, num6, num8, num9));
					flag = (flag && --num5 > 0);
				}
			}
			nativeArray.Dispose();
			return flag;
		}

		private int FindNeighbor(NativeArray<int3> cells, int count, int a, int b, int c)
		{
			int num = a;
			int y = b;
			int z = c;
			if (b < c)
			{
				if (b < a)
				{
					num = b;
					y = c;
					z = a;
				}
			}
			else if (c < a)
			{
				num = c;
				y = a;
				z = b;
			}
			if (num < 0)
			{
				return -1;
			}
			int3 check;
			check.x = num;
			check.y = y;
			check.z = z;
			return ModuleHandle.GetEqual<int3, int3, Tessellator.TestCellE>(cells, count, check, default(Tessellator.TestCellE));
		}

		private NativeArray<int3> Constrain(ref int count)
		{
			NativeArray<int3> cells = this.GetCells(ref count);
			int num = count;
			for (int i = 0; i < num; i++)
			{
				int3 @int = cells[i];
				int x = @int.x;
				int y = @int.y;
				int z = @int.z;
				if (y < z)
				{
					if (y < x)
					{
						@int.x = y;
						@int.y = z;
						@int.z = x;
					}
				}
				else if (z < x)
				{
					@int.x = z;
					@int.y = x;
					@int.z = y;
				}
				cells[i] = @int;
			}
			ModuleHandle.InsertionSort<int3, TessCellCompare>(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int3>(cells), 0, this.m_CellCount - 1, default(TessCellCompare));
			this.m_Flags = new NativeArray<int>(num, this.m_Allocator, NativeArrayOptions.ClearMemory);
			this.m_Neighbors = new NativeArray<int>(num * 3, this.m_Allocator, NativeArrayOptions.ClearMemory);
			this.m_Constraints = new NativeArray<int>(num * 3, this.m_Allocator, NativeArrayOptions.ClearMemory);
			NativeArray<int> nativeArray = new NativeArray<int>(num * 3, this.m_Allocator, NativeArrayOptions.ClearMemory);
			NativeArray<int> nativeArray2 = new NativeArray<int>(num * 3, this.m_Allocator, NativeArrayOptions.ClearMemory);
			int num2 = 1;
			int num3 = 0;
			int j = 0;
			for (int k = 0; k < num; k++)
			{
				int3 int2 = cells[k];
				for (int l = 0; l < 3; l++)
				{
					int num4 = l;
					int num5 = (l + 1) % 3;
					num4 = ((num4 == 0) ? int2.x : ((l == 1) ? int2.y : int2.z));
					num5 = ((num5 == 0) ? int2.x : ((num5 == 1) ? int2.y : int2.z));
					int c = this.OppositeOf(num5, num4);
					int num6 = this.m_Neighbors[3 * k + l] = this.FindNeighbor(cells, count, num5, num4, c);
					int num7 = this.m_Constraints[3 * k + l] = ((-1 != this.FindConstraint(num4, num5)) ? 1 : 0);
					if (num6 < 0)
					{
						if (num7 != 0)
						{
							nativeArray[num3++] = k;
						}
						else
						{
							nativeArray2[j++] = k;
							this.m_Flags[k] = 1;
						}
					}
				}
			}
			while (j > 0 || num3 > 0)
			{
				while (j > 0)
				{
					int num8 = nativeArray2[j - 1];
					j--;
					if (this.m_Flags[num8] != -num2)
					{
						this.m_Flags[num8] = num2;
						int3 int3 = cells[num8];
						for (int m = 0; m < 3; m++)
						{
							int num9 = this.m_Neighbors[3 * num8 + m];
							if (num9 >= 0 && this.m_Flags[num9] == 0)
							{
								if (this.m_Constraints[3 * num8 + m] != 0)
								{
									nativeArray[num3++] = num9;
								}
								else
								{
									nativeArray2[j++] = num9;
									this.m_Flags[num9] = num2;
								}
							}
						}
					}
				}
				for (int n = 0; n < num3; n++)
				{
					nativeArray2[n] = nativeArray[n];
				}
				j = num3;
				num3 = 0;
				num2 = -num2;
			}
			nativeArray2.Dispose();
			nativeArray.Dispose();
			return cells;
		}

		internal NativeArray<int3> RemoveExterior(ref int cellCount)
		{
			int num = 0;
			NativeArray<int3> nativeArray = this.Constrain(ref num);
			NativeArray<int3> result = new NativeArray<int3>(num, this.m_Allocator, NativeArrayOptions.ClearMemory);
			cellCount = 0;
			for (int i = 0; i < num; i++)
			{
				if (this.m_Flags[i] == -1)
				{
					int num2 = cellCount;
					cellCount = num2 + 1;
					result[num2] = nativeArray[i];
				}
			}
			nativeArray.Dispose();
			return result;
		}

		internal NativeArray<int3> RemoveInterior(int cellCount)
		{
			int num = 0;
			NativeArray<int3> nativeArray = this.Constrain(ref num);
			NativeArray<int3> result = new NativeArray<int3>(num, this.m_Allocator, NativeArrayOptions.ClearMemory);
			cellCount = 0;
			for (int i = 0; i < num; i++)
			{
				if (this.m_Flags[i] == 1)
				{
					result[cellCount++] = nativeArray[i];
				}
			}
			nativeArray.Dispose();
			return result;
		}

		internal bool Triangulate(NativeArray<float2> points, int pointCount, NativeArray<int2> edges, int edgeCount)
		{
			this.m_NumEdges = edgeCount;
			this.m_NumHulls = edgeCount * 2;
			this.m_NumPoints = pointCount;
			this.m_CellCount = 0;
			this.m_Cells = new NativeArray<int3>(ModuleHandle.kMaxTriangleCount, this.m_Allocator, NativeArrayOptions.ClearMemory);
			this.m_ILArray = new NativeArray<int>(this.m_NumHulls * (this.m_NumHulls + 1), this.m_Allocator, NativeArrayOptions.ClearMemory);
			this.m_IUArray = new NativeArray<int>(this.m_NumHulls * (this.m_NumHulls + 1), this.m_Allocator, NativeArrayOptions.ClearMemory);
			NativeArray<UHull> hulls = new NativeArray<UHull>(this.m_NumPoints * 8, this.m_Allocator, NativeArrayOptions.ClearMemory);
			int hullCount = 0;
			NativeArray<UEvent> nativeArray = new NativeArray<UEvent>(this.m_NumPoints + this.m_NumEdges * 2, this.m_Allocator, NativeArrayOptions.ClearMemory);
			int num = 0;
			for (int i = 0; i < this.m_NumPoints; i++)
			{
				UEvent value = default(UEvent);
				value.a = points[i];
				value.b = default(float2);
				value.idx = i;
				value.type = 0;
				nativeArray[num++] = value;
			}
			for (int j = 0; j < this.m_NumEdges; j++)
			{
				int2 @int = edges[j];
				float2 @float = points[@int.x];
				float2 float2 = points[@int.y];
				if (@float.x < float2.x)
				{
					UEvent value2 = default(UEvent);
					value2.a = @float;
					value2.b = float2;
					value2.idx = j;
					value2.type = 2;
					UEvent value3 = default(UEvent);
					value3.a = float2;
					value3.b = @float;
					value3.idx = j;
					value3.type = 1;
					nativeArray[num++] = value2;
					nativeArray[num++] = value3;
				}
				else if (@float.x > float2.x)
				{
					UEvent value4 = default(UEvent);
					value4.a = float2;
					value4.b = @float;
					value4.idx = j;
					value4.type = 2;
					UEvent value5 = default(UEvent);
					value5.a = @float;
					value5.b = float2;
					value5.idx = j;
					value5.type = 1;
					nativeArray[num++] = value4;
					nativeArray[num++] = value5;
				}
			}
			ModuleHandle.InsertionSort<UEvent, TessEventCompare>(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<UEvent>(nativeArray), 0, num - 1, default(TessEventCompare));
			bool flag = true;
			float x = nativeArray[0].a.x - (1f + math.abs(nativeArray[0].a.x)) * math.pow(2f, -16f);
			UHull value6;
			value6.a.x = x;
			value6.a.y = 1f;
			value6.b.x = x;
			value6.b.y = 0f;
			value6.idx = -1;
			value6.ilarray = new ArraySlice<int>(this.m_ILArray, this.m_NumHulls * this.m_NumHulls, this.m_NumHulls);
			value6.iuarray = new ArraySlice<int>(this.m_IUArray, this.m_NumHulls * this.m_NumHulls, this.m_NumHulls);
			value6.ilcount = 0;
			value6.iucount = 0;
			hulls[hullCount++] = value6;
			int k = 0;
			int num2 = num;
			while (k < num2)
			{
				int type = nativeArray[k].type;
				if (type != 0)
				{
					if (type != 2)
					{
						flag = this.MergeHulls(hulls, ref hullCount, points, nativeArray[k]);
					}
					else
					{
						flag = this.SplitHulls(hulls, ref hullCount, points, nativeArray[k]);
					}
				}
				else
				{
					flag = this.AddPoint(hulls, hullCount, points, nativeArray[k].a, nativeArray[k].idx);
				}
				if (!flag)
				{
					break;
				}
				k++;
			}
			nativeArray.Dispose();
			hulls.Dispose();
			return flag;
		}

		internal static bool Tessellate(Allocator allocator, NativeArray<float2> pgPoints, int pgPointCount, NativeArray<int2> pgEdges, int pgEdgeCount, ref NativeArray<float2> outputVertices, ref int vertexCount, ref NativeArray<int> outputIndices, ref int indexCount)
		{
			Tessellator tessellator = default(Tessellator);
			tessellator.SetAllocator(allocator);
			int num = 0;
			int num2 = 0;
			bool flag = tessellator.Triangulate(pgPoints, pgPointCount, pgEdges, pgEdgeCount);
			flag = (flag && tessellator.ApplyDelaunay(pgPoints, pgEdges));
			if (flag)
			{
				NativeArray<int3> nativeArray = tessellator.RemoveExterior(ref num2);
				for (int i = 0; i < num2; i++)
				{
					ushort num3 = (ushort)nativeArray[i].x;
					ushort num4 = (ushort)nativeArray[i].y;
					ushort num5 = (ushort)nativeArray[i].z;
					if (num3 != num4 && num4 != num5 && num3 != num5)
					{
						int num6 = indexCount;
						indexCount = num6 + 1;
						outputIndices[num6] = (int)num3;
						num6 = indexCount;
						indexCount = num6 + 1;
						outputIndices[num6] = (int)num5;
						num6 = indexCount;
						indexCount = num6 + 1;
						outputIndices[num6] = (int)num4;
					}
					num = math.max(math.max(math.max(nativeArray[i].x, nativeArray[i].y), nativeArray[i].z), num);
				}
				num = ((num != 0) ? (num + 1) : 0);
				for (int j = 0; j < num; j++)
				{
					int num6 = vertexCount;
					vertexCount = num6 + 1;
					outputVertices[num6] = pgPoints[j];
				}
				nativeArray.Dispose();
			}
			tessellator.Cleanup();
			return flag;
		}

		internal void Cleanup()
		{
			if (this.m_Edges.IsCreated)
			{
				this.m_Edges.Dispose();
			}
			if (this.m_Stars.IsCreated)
			{
				this.m_Stars.Dispose();
			}
			if (this.m_SPArray.IsCreated)
			{
				this.m_SPArray.Dispose();
			}
			if (this.m_Cells.IsCreated)
			{
				this.m_Cells.Dispose();
			}
			if (this.m_ILArray.IsCreated)
			{
				this.m_ILArray.Dispose();
			}
			if (this.m_IUArray.IsCreated)
			{
				this.m_IUArray.Dispose();
			}
			if (this.m_Flags.IsCreated)
			{
				this.m_Flags.Dispose();
			}
			if (this.m_Neighbors.IsCreated)
			{
				this.m_Neighbors.Dispose();
			}
			if (this.m_Constraints.IsCreated)
			{
				this.m_Constraints.Dispose();
			}
		}

		private NativeArray<int2> m_Edges;

		private NativeArray<UStar> m_Stars;

		private NativeArray<int3> m_Cells;

		private int m_CellCount;

		private NativeArray<int> m_ILArray;

		private NativeArray<int> m_IUArray;

		private NativeArray<int> m_SPArray;

		private int m_NumEdges;

		private int m_NumHulls;

		private int m_NumPoints;

		private int m_StarCount;

		private NativeArray<int> m_Flags;

		private NativeArray<int> m_Neighbors;

		private NativeArray<int> m_Constraints;

		private Allocator m_Allocator;

		private struct TestHullPointL : ICondition2<UHull, float2>
		{
			public bool Test(UHull h, float2 p, ref float t)
			{
				t = ModuleHandle.OrientFast(h.a, h.b, p);
				return t < 0f;
			}
		}

		private struct TestHullPointU : ICondition2<UHull, float2>
		{
			public bool Test(UHull h, float2 p, ref float t)
			{
				t = ModuleHandle.OrientFast(h.a, h.b, p);
				return t > 0f;
			}
		}

		private struct TestHullEventLe : ICondition2<UHull, UEvent>
		{
			public bool Test(UHull h, UEvent p, ref float t)
			{
				t = Tessellator.FindSplit(h, p);
				return t <= 0f;
			}
		}

		private struct TestHullEventE : ICondition2<UHull, UEvent>
		{
			public bool Test(UHull h, UEvent p, ref float t)
			{
				t = Tessellator.FindSplit(h, p);
				return t == 0f;
			}
		}

		private struct TestEdgePointE : ICondition2<int2, int2>
		{
			public bool Test(int2 h, int2 p, ref float t)
			{
				t = (float)default(TessEdgeCompare).Compare(h, p);
				return t == 0f;
			}
		}

		private struct TestCellE : ICondition2<int3, int3>
		{
			public bool Test(int3 h, int3 p, ref float t)
			{
				t = (float)default(TessCellCompare).Compare(h, p);
				return t == 0f;
			}
		}
	}
}
