using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Cinemachine
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class ClipperBase
	{
		public bool PreserveCollinear { get; set; }

		public bool ReverseSolution { get; set; }

		public ClipperBase()
		{
			this._minimaList = new List<LocalMinima>();
			this._intersectList = new List<IntersectNode>();
			this._vertexList = new List<Vertex>();
			this._outrecList = new List<OutRec>();
			this._joinerList = new List<Joiner>();
			this._scanlineList = new List<long>();
			this.PreserveCollinear = true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsOdd(int val)
		{
			return (val & 1) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsHotEdge(Active ae)
		{
			return ae.outrec != null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsOpen(Active ae)
		{
			return ae.localMin.isOpen;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsOpenEnd(Active ae)
		{
			return ae.localMin.isOpen && ClipperBase.IsOpenEnd(ae.vertexTop);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsOpenEnd(Vertex v)
		{
			return (v.flags & (VertexFlags)3) > VertexFlags.None;
		}

		[return: Nullable(2)]
		private static Active GetPrevHotEdge(Active ae)
		{
			Active prevInAEL = ae.prevInAEL;
			while (prevInAEL != null && (ClipperBase.IsOpen(prevInAEL) || !ClipperBase.IsHotEdge(prevInAEL)))
			{
				prevInAEL = prevInAEL.prevInAEL;
			}
			return prevInAEL;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsFront(Active ae)
		{
			return ae == ae.outrec.frontEdge;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static double GetDx(Point64 pt1, Point64 pt2)
		{
			double num = (double)(pt2.Y - pt1.Y);
			if (num != 0.0)
			{
				return (double)(pt2.X - pt1.X) / num;
			}
			if (pt2.X > pt1.X)
			{
				return double.NegativeInfinity;
			}
			return double.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long TopX(Active ae, long currentY)
		{
			if (currentY == ae.top.Y || ae.top.X == ae.bot.X)
			{
				return ae.top.X;
			}
			if (currentY == ae.bot.Y)
			{
				return ae.bot.X;
			}
			return ae.bot.X + (long)Math.Round(ae.dx * (double)(currentY - ae.bot.Y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsHorizontal(Active ae)
		{
			return ae.top.Y == ae.bot.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsHeadingRightHorz(Active ae)
		{
			return double.IsNegativeInfinity(ae.dx);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsHeadingLeftHorz(Active ae)
		{
			return double.IsPositiveInfinity(ae.dx);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void SwapActives(ref Active ae1, ref Active ae2)
		{
			Active active = ae1;
			Active active2 = ae2;
			ae2 = active;
			ae1 = active2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static PathType GetPolyType(Active ae)
		{
			return ae.localMin.polytype;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsSamePolyType(Active ae1, Active ae2)
		{
			return ae1.localMin.polytype == ae2.localMin.polytype;
		}

		private static Point64 GetIntersectPoint(Active ae1, Active ae2)
		{
			if (ae1.dx == ae2.dx)
			{
				return ae1.top;
			}
			if (ae1.dx == 0.0)
			{
				if (ClipperBase.IsHorizontal(ae2))
				{
					return new Point64(ae1.bot.X, ae2.bot.Y);
				}
				double num = (double)ae2.bot.Y - (double)ae2.bot.X / ae2.dx;
				return new Point64(ae1.bot.X, (long)Math.Round((double)ae1.bot.X / ae2.dx + num));
			}
			else if (ae2.dx == 0.0)
			{
				if (ClipperBase.IsHorizontal(ae1))
				{
					return new Point64(ae2.bot.X, ae1.bot.Y);
				}
				double num2 = (double)ae1.bot.Y - (double)ae1.bot.X / ae1.dx;
				return new Point64(ae2.bot.X, (long)Math.Round((double)ae2.bot.X / ae1.dx + num2));
			}
			else
			{
				double num2 = (double)ae1.bot.X - (double)ae1.bot.Y * ae1.dx;
				double num = (double)ae2.bot.X - (double)ae2.bot.Y * ae2.dx;
				double num3 = (num - num2) / (ae1.dx - ae2.dx);
				if (Math.Abs(ae1.dx) >= Math.Abs(ae2.dx))
				{
					return new Point64((long)Math.Round(ae2.dx * num3 + num), (long)Math.Round(num3));
				}
				return new Point64((long)Math.Round(ae1.dx * num3 + num2), (long)Math.Round(num3));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void SetDx(Active ae)
		{
			ae.dx = ClipperBase.GetDx(ae.bot, ae.top);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vertex NextVertex(Active ae)
		{
			if (ae.windDx > 0)
			{
				return ae.vertexTop.next;
			}
			return ae.vertexTop.prev;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Vertex PrevPrevVertex(Active ae)
		{
			if (ae.windDx > 0)
			{
				return ae.vertexTop.prev.prev;
			}
			return ae.vertexTop.next.next;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsMaxima(Vertex vertex)
		{
			return (vertex.flags & VertexFlags.LocalMax) > VertexFlags.None;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsMaxima(Active ae)
		{
			return ClipperBase.IsMaxima(ae.vertexTop);
		}

		[return: Nullable(2)]
		private Active GetMaximaPair(Active ae)
		{
			for (Active nextInAEL = ae.nextInAEL; nextInAEL != null; nextInAEL = nextInAEL.nextInAEL)
			{
				if (nextInAEL.vertexTop == ae.vertexTop)
				{
					return nextInAEL;
				}
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: Nullable(2)]
		private static Vertex GetCurrYMaximaVertex(Active ae)
		{
			Vertex vertex = ae.vertexTop;
			if (ae.windDx > 0)
			{
				while (vertex.next.pt.Y == vertex.pt.Y)
				{
					vertex = vertex.next;
				}
			}
			else
			{
				while (vertex.prev.pt.Y == vertex.pt.Y)
				{
					vertex = vertex.prev;
				}
			}
			if (!ClipperBase.IsMaxima(vertex))
			{
				vertex = null;
			}
			return vertex;
		}

		[return: Nullable(2)]
		private static Active GetHorzMaximaPair(Active horz, Vertex maxVert)
		{
			Active active = horz.prevInAEL;
			while (active != null && active.curX >= maxVert.pt.X)
			{
				if (active.vertexTop == maxVert)
				{
					return active;
				}
				active = active.prevInAEL;
			}
			active = horz.nextInAEL;
			while (active != null && ClipperBase.TopX(active, horz.top.Y) <= maxVert.pt.X)
			{
				if (active.vertexTop == maxVert)
				{
					return active;
				}
				active = active.nextInAEL;
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void SetSides(OutRec outrec, Active startEdge, Active endEdge)
		{
			outrec.frontEdge = startEdge;
			outrec.backEdge = endEdge;
		}

		private static void SwapOutrecs(Active ae1, Active ae2)
		{
			OutRec outrec = ae1.outrec;
			OutRec outrec2 = ae2.outrec;
			if (outrec == outrec2)
			{
				Active frontEdge = outrec.frontEdge;
				outrec.frontEdge = outrec.backEdge;
				outrec.backEdge = frontEdge;
				return;
			}
			if (outrec != null)
			{
				if (ae1 == outrec.frontEdge)
				{
					outrec.frontEdge = ae2;
				}
				else
				{
					outrec.backEdge = ae2;
				}
			}
			if (outrec2 != null)
			{
				if (ae2 == outrec2.frontEdge)
				{
					outrec2.frontEdge = ae1;
				}
				else
				{
					outrec2.backEdge = ae1;
				}
			}
			ae1.outrec = outrec2;
			ae2.outrec = outrec;
		}

		private static double Area(OutPt op)
		{
			double num = 0.0;
			OutPt outPt = op;
			do
			{
				num += (double)(outPt.prev.pt.Y + outPt.pt.Y) * (double)(outPt.prev.pt.X - outPt.pt.X);
				outPt = outPt.next;
			}
			while (outPt != op);
			return num * 0.5;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static double AreaTriangle(Point64 pt1, Point64 pt2, Point64 pt3)
		{
			return (double)(pt3.Y + pt1.Y) * (double)(pt3.X - pt1.X) + (double)(pt1.Y + pt2.Y) * (double)(pt1.X - pt2.X) + (double)(pt2.Y + pt3.Y) * (double)(pt2.X - pt3.X);
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static OutRec GetRealOutRec(OutRec outRec)
		{
			while (outRec != null && outRec.pts == null)
			{
				outRec = outRec.owner;
			}
			return outRec;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void UncoupleOutRec(Active ae)
		{
			OutRec outrec = ae.outrec;
			if (outrec == null)
			{
				return;
			}
			outrec.frontEdge.outrec = null;
			outrec.backEdge.outrec = null;
			outrec.frontEdge = null;
			outrec.backEdge = null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool OutrecIsAscending(Active hotEdge)
		{
			return hotEdge == hotEdge.outrec.frontEdge;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void SwapFrontBackSides(OutRec outrec)
		{
			Active frontEdge = outrec.frontEdge;
			outrec.frontEdge = outrec.backEdge;
			outrec.backEdge = frontEdge;
			outrec.pts = outrec.pts.next;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool EdgesAdjacentInAEL(IntersectNode inode)
		{
			return inode.edge1.nextInAEL == inode.edge2 || inode.edge1.prevInAEL == inode.edge2;
		}

		protected void ClearSolution()
		{
			while (this._actives != null)
			{
				this.DeleteFromAEL(this._actives);
			}
			this._scanlineList.Clear();
			this.DisposeIntersectNodes();
			this._joinerList.Clear();
			this._horzJoiners = null;
			this._outrecList.Clear();
		}

		public void Clear()
		{
			this.ClearSolution();
			this._minimaList.Clear();
			this._vertexList.Clear();
			this._currentLocMin = 0;
			this._isSortedMinimaList = false;
			this._hasOpenPaths = false;
		}

		protected void Reset()
		{
			if (!this._isSortedMinimaList)
			{
				this._minimaList.Sort(default(LocMinSorter));
				this._isSortedMinimaList = true;
			}
			this._scanlineList.Capacity = this._minimaList.Count;
			for (int i = this._minimaList.Count - 1; i >= 0; i--)
			{
				this._scanlineList.Add(this._minimaList[i].vertex.pt.Y);
			}
			this._currentBotY = 0L;
			this._currentLocMin = 0;
			this._actives = null;
			this._sel = null;
			this._succeeded = true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void InsertScanline(long y)
		{
			int num = this._scanlineList.BinarySearch(y);
			if (num >= 0)
			{
				return;
			}
			num = ~num;
			this._scanlineList.Insert(num, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool PopScanline(out long y)
		{
			int num = this._scanlineList.Count - 1;
			if (num < 0)
			{
				y = 0L;
				return false;
			}
			y = this._scanlineList[num];
			this._scanlineList.RemoveAt(num--);
			while (num >= 0 && y == this._scanlineList[num])
			{
				this._scanlineList.RemoveAt(num--);
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool HasLocMinAtY(long y)
		{
			return this._currentLocMin < this._minimaList.Count && this._minimaList[this._currentLocMin].vertex.pt.Y == y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private LocalMinima PopLocalMinima()
		{
			List<LocalMinima> minimaList = this._minimaList;
			int currentLocMin = this._currentLocMin;
			this._currentLocMin = currentLocMin + 1;
			return minimaList[currentLocMin];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddLocMin(Vertex vert, PathType polytype, bool isOpen)
		{
			if ((vert.flags & VertexFlags.LocalMin) != VertexFlags.None)
			{
				return;
			}
			vert.flags |= VertexFlags.LocalMin;
			LocalMinima item = new LocalMinima(vert, polytype, isOpen);
			this._minimaList.Add(item);
		}

		protected void AddPathsToVertexList(List<List<Point64>> paths, PathType polytype, bool isOpen)
		{
			int num = 0;
			foreach (List<Point64> list in paths)
			{
				num += list.Count;
			}
			this._vertexList.Capacity = this._vertexList.Count + num;
			foreach (List<Point64> list2 in paths)
			{
				Vertex vertex = null;
				Vertex vertex2 = null;
				foreach (Point64 point in list2)
				{
					if (vertex == null)
					{
						vertex = new Vertex(point, VertexFlags.None, null);
						this._vertexList.Add(vertex);
						vertex2 = vertex;
					}
					else if (vertex2.pt != point)
					{
						Vertex vertex3 = new Vertex(point, VertexFlags.None, vertex2);
						this._vertexList.Add(vertex3);
						vertex2.next = vertex3;
						vertex2 = vertex3;
					}
				}
				if (vertex2 != null && vertex2.prev != null)
				{
					if (!isOpen && vertex2.pt == vertex.pt)
					{
						vertex2 = vertex2.prev;
					}
					vertex2.next = vertex;
					vertex.prev = vertex2;
					if (isOpen || vertex2.next != vertex2)
					{
						bool flag;
						if (isOpen)
						{
							Vertex vertex3 = vertex.next;
							while (vertex3 != vertex && vertex3.pt.Y == vertex.pt.Y)
							{
								vertex3 = vertex3.next;
							}
							flag = (vertex3.pt.Y <= vertex.pt.Y);
							if (flag)
							{
								vertex.flags = VertexFlags.OpenStart;
								this.AddLocMin(vertex, polytype, true);
							}
							else
							{
								vertex.flags = (VertexFlags)5;
							}
						}
						else
						{
							vertex2 = vertex.prev;
							while (vertex2 != vertex && vertex2.pt.Y == vertex.pt.Y)
							{
								vertex2 = vertex2.prev;
							}
							if (vertex2 == vertex)
							{
								continue;
							}
							flag = (vertex2.pt.Y > vertex.pt.Y);
						}
						bool flag2 = flag;
						vertex2 = vertex;
						for (Vertex vertex3 = vertex.next; vertex3 != vertex; vertex3 = vertex3.next)
						{
							if (vertex3.pt.Y > vertex2.pt.Y && flag)
							{
								vertex2.flags |= VertexFlags.LocalMax;
								flag = false;
							}
							else if (vertex3.pt.Y < vertex2.pt.Y && !flag)
							{
								flag = true;
								this.AddLocMin(vertex2, polytype, isOpen);
							}
							vertex2 = vertex3;
						}
						if (isOpen)
						{
							vertex2.flags |= VertexFlags.OpenEnd;
							if (flag)
							{
								vertex2.flags |= VertexFlags.LocalMax;
							}
							else
							{
								this.AddLocMin(vertex2, polytype, isOpen);
							}
						}
						else if (flag != flag2)
						{
							if (flag2)
							{
								this.AddLocMin(vertex2, polytype, false);
							}
							else
							{
								vertex2.flags |= VertexFlags.LocalMax;
							}
						}
					}
				}
			}
		}

		public void AddSubject(List<Point64> path)
		{
			this.AddPath(path, PathType.Subject, false);
		}

		public void AddOpenSubject(List<Point64> path)
		{
			this.AddPath(path, PathType.Subject, true);
		}

		public void AddClip(List<Point64> path)
		{
			this.AddPath(path, PathType.Clip, false);
		}

		protected void AddPath(List<Point64> path, PathType polytype, bool isOpen = false)
		{
			List<List<Point64>> paths = new List<List<Point64>>(1)
			{
				path
			};
			this.AddPaths(paths, polytype, isOpen);
		}

		protected void AddPaths(List<List<Point64>> paths, PathType polytype, bool isOpen = false)
		{
			if (isOpen)
			{
				this._hasOpenPaths = true;
			}
			this._isSortedMinimaList = false;
			this.AddPathsToVertexList(paths, polytype, isOpen);
		}

		private bool IsContributingClosed(Active ae)
		{
			switch (this._fillrule)
			{
			case FillRule.NonZero:
				if (Math.Abs(ae.windCount) != 1)
				{
					return false;
				}
				break;
			case FillRule.Positive:
				if (ae.windCount != 1)
				{
					return false;
				}
				break;
			case FillRule.Negative:
				if (ae.windCount != -1)
				{
					return false;
				}
				break;
			}
			switch (this._cliptype)
			{
			case ClipType.Intersection:
			{
				FillRule fillrule = this._fillrule;
				bool flag;
				if (fillrule != FillRule.Positive)
				{
					if (fillrule != FillRule.Negative)
					{
						flag = (ae.windCount2 != 0);
					}
					else
					{
						flag = (ae.windCount2 < 0);
					}
				}
				else
				{
					flag = (ae.windCount2 > 0);
				}
				return flag;
			}
			case ClipType.Union:
			{
				FillRule fillrule = this._fillrule;
				bool flag;
				if (fillrule != FillRule.Positive)
				{
					if (fillrule != FillRule.Negative)
					{
						flag = (ae.windCount2 == 0);
					}
					else
					{
						flag = (ae.windCount2 >= 0);
					}
				}
				else
				{
					flag = (ae.windCount2 <= 0);
				}
				return flag;
			}
			case ClipType.Difference:
			{
				FillRule fillrule = this._fillrule;
				bool flag;
				if (fillrule != FillRule.Positive)
				{
					if (fillrule != FillRule.Negative)
					{
						flag = (ae.windCount2 == 0);
					}
					else
					{
						flag = (ae.windCount2 >= 0);
					}
				}
				else
				{
					flag = (ae.windCount2 <= 0);
				}
				bool flag2 = flag;
				if (ClipperBase.GetPolyType(ae) != PathType.Subject)
				{
					return !flag2;
				}
				return flag2;
			}
			case ClipType.Xor:
				return true;
			default:
				return false;
			}
		}

		private bool IsContributingOpen(Active ae)
		{
			FillRule fillrule = this._fillrule;
			bool flag;
			bool flag2;
			if (fillrule != FillRule.Positive)
			{
				if (fillrule != FillRule.Negative)
				{
					flag = (ae.windCount != 0);
					flag2 = (ae.windCount2 != 0);
				}
				else
				{
					flag = (ae.windCount < 0);
					flag2 = (ae.windCount2 < 0);
				}
			}
			else
			{
				flag = (ae.windCount > 0);
				flag2 = (ae.windCount2 > 0);
			}
			ClipType cliptype = this._cliptype;
			bool result;
			if (cliptype != ClipType.Intersection)
			{
				if (cliptype != ClipType.Union)
				{
					result = !flag2;
				}
				else
				{
					result = (!flag && !flag2);
				}
			}
			else
			{
				result = flag2;
			}
			return result;
		}

		private void SetWindCountForClosedPathEdge(Active ae)
		{
			Active active = ae.prevInAEL;
			PathType polyType = ClipperBase.GetPolyType(ae);
			while (active != null && (ClipperBase.GetPolyType(active) != polyType || ClipperBase.IsOpen(active)))
			{
				active = active.prevInAEL;
			}
			if (active == null)
			{
				ae.windCount = ae.windDx;
				active = this._actives;
			}
			else if (this._fillrule == FillRule.EvenOdd)
			{
				ae.windCount = ae.windDx;
				ae.windCount2 = active.windCount2;
				active = active.nextInAEL;
			}
			else
			{
				if (active.windCount * active.windDx < 0)
				{
					if (Math.Abs(active.windCount) > 1)
					{
						if (active.windDx * ae.windDx < 0)
						{
							ae.windCount = active.windCount;
						}
						else
						{
							ae.windCount = active.windCount + ae.windDx;
						}
					}
					else
					{
						ae.windCount = (ClipperBase.IsOpen(ae) ? 1 : ae.windDx);
					}
				}
				else if (active.windDx * ae.windDx < 0)
				{
					ae.windCount = active.windCount;
				}
				else
				{
					ae.windCount = active.windCount + ae.windDx;
				}
				ae.windCount2 = active.windCount2;
				active = active.nextInAEL;
			}
			if (this._fillrule == FillRule.EvenOdd)
			{
				while (active != ae)
				{
					if (ClipperBase.GetPolyType(active) != polyType && !ClipperBase.IsOpen(active))
					{
						ae.windCount2 = ((ae.windCount2 == 0) ? 1 : 0);
					}
					active = active.nextInAEL;
				}
				return;
			}
			while (active != ae)
			{
				if (ClipperBase.GetPolyType(active) != polyType && !ClipperBase.IsOpen(active))
				{
					ae.windCount2 += active.windDx;
				}
				active = active.nextInAEL;
			}
		}

		private void SetWindCountForOpenPathEdge(Active ae)
		{
			Active active = this._actives;
			if (this._fillrule == FillRule.EvenOdd)
			{
				int num = 0;
				int num2 = 0;
				while (active != ae)
				{
					if (ClipperBase.GetPolyType(active) == PathType.Clip)
					{
						num2++;
					}
					else if (!ClipperBase.IsOpen(active))
					{
						num++;
					}
					active = active.nextInAEL;
				}
				ae.windCount = (ClipperBase.IsOdd(num) ? 1 : 0);
				ae.windCount2 = (ClipperBase.IsOdd(num2) ? 1 : 0);
				return;
			}
			while (active != ae)
			{
				if (ClipperBase.GetPolyType(active) == PathType.Clip)
				{
					ae.windCount2 += active.windDx;
				}
				else if (!ClipperBase.IsOpen(active))
				{
					ae.windCount += active.windDx;
				}
				active = active.nextInAEL;
			}
		}

		private bool IsValidAelOrder(Active resident, Active newcomer)
		{
			if (newcomer.curX != resident.curX)
			{
				return newcomer.curX > resident.curX;
			}
			double num = InternalClipper.CrossProduct(resident.top, newcomer.bot, newcomer.top);
			if (num != 0.0)
			{
				return num < 0.0;
			}
			if (!ClipperBase.IsMaxima(resident) && resident.top.Y > newcomer.top.Y)
			{
				return InternalClipper.CrossProduct(newcomer.bot, resident.top, ClipperBase.NextVertex(resident).pt) <= 0.0;
			}
			if (!ClipperBase.IsMaxima(newcomer) && newcomer.top.Y > resident.top.Y)
			{
				return InternalClipper.CrossProduct(newcomer.bot, newcomer.top, ClipperBase.NextVertex(newcomer).pt) >= 0.0;
			}
			long y = newcomer.bot.Y;
			bool isLeftBound = newcomer.isLeftBound;
			if (resident.bot.Y != y || resident.localMin.vertex.pt.Y != y)
			{
				return newcomer.isLeftBound;
			}
			if (resident.isLeftBound != isLeftBound)
			{
				return isLeftBound;
			}
			return InternalClipper.CrossProduct(this.PrevPrevVertex(resident).pt, resident.bot, resident.top) == 0.0 || InternalClipper.CrossProduct(this.PrevPrevVertex(resident).pt, newcomer.bot, this.PrevPrevVertex(newcomer).pt) > 0.0 == isLeftBound;
		}

		private void InsertLeftEdge(Active ae)
		{
			if (this._actives == null)
			{
				ae.prevInAEL = null;
				ae.nextInAEL = null;
				this._actives = ae;
				return;
			}
			if (!this.IsValidAelOrder(this._actives, ae))
			{
				ae.prevInAEL = null;
				ae.nextInAEL = this._actives;
				this._actives.prevInAEL = ae;
				this._actives = ae;
				return;
			}
			Active active = this._actives;
			while (active.nextInAEL != null && this.IsValidAelOrder(active.nextInAEL, ae))
			{
				active = active.nextInAEL;
			}
			ae.nextInAEL = active.nextInAEL;
			if (active.nextInAEL != null)
			{
				active.nextInAEL.prevInAEL = ae;
			}
			ae.prevInAEL = active;
			active.nextInAEL = ae;
		}

		private void InsertRightEdge(Active ae, Active ae2)
		{
			ae2.nextInAEL = ae.nextInAEL;
			if (ae.nextInAEL != null)
			{
				ae.nextInAEL.prevInAEL = ae2;
			}
			ae2.prevInAEL = ae;
			ae.nextInAEL = ae2;
		}

		private void InsertLocalMinimaIntoAEL(long botY)
		{
			while (this.HasLocMinAtY(botY))
			{
				LocalMinima localMinima = this.PopLocalMinima();
				Active active;
				if ((localMinima.vertex.flags & VertexFlags.OpenStart) != VertexFlags.None)
				{
					active = null;
				}
				else
				{
					active = new Active
					{
						bot = localMinima.vertex.pt,
						curX = localMinima.vertex.pt.X,
						windDx = -1,
						vertexTop = localMinima.vertex.prev,
						top = localMinima.vertex.prev.pt,
						outrec = null,
						localMin = localMinima
					};
					ClipperBase.SetDx(active);
				}
				Active active2;
				if ((localMinima.vertex.flags & VertexFlags.OpenEnd) != VertexFlags.None)
				{
					active2 = null;
				}
				else
				{
					active2 = new Active
					{
						bot = localMinima.vertex.pt,
						curX = localMinima.vertex.pt.X,
						windDx = 1,
						vertexTop = localMinima.vertex.next,
						top = localMinima.vertex.next.pt,
						outrec = null,
						localMin = localMinima
					};
					ClipperBase.SetDx(active2);
				}
				if (active != null && active2 != null)
				{
					if (ClipperBase.IsHorizontal(active))
					{
						if (ClipperBase.IsHeadingRightHorz(active))
						{
							ClipperBase.SwapActives(ref active, ref active2);
						}
					}
					else if (ClipperBase.IsHorizontal(active2))
					{
						if (ClipperBase.IsHeadingLeftHorz(active2))
						{
							ClipperBase.SwapActives(ref active, ref active2);
						}
					}
					else if (active.dx < active2.dx)
					{
						ClipperBase.SwapActives(ref active, ref active2);
					}
				}
				else if (active == null)
				{
					active = active2;
					active2 = null;
				}
				active.isLeftBound = true;
				this.InsertLeftEdge(active);
				bool flag;
				if (ClipperBase.IsOpen(active))
				{
					this.SetWindCountForOpenPathEdge(active);
					flag = this.IsContributingOpen(active);
				}
				else
				{
					this.SetWindCountForClosedPathEdge(active);
					flag = this.IsContributingClosed(active);
				}
				if (active2 != null)
				{
					active2.windCount = active.windCount;
					active2.windCount2 = active.windCount2;
					this.InsertRightEdge(active, active2);
					if (flag)
					{
						this.AddLocalMinPoly(active, active2, active.bot, true);
						if (!ClipperBase.IsHorizontal(active) && this.TestJoinWithPrev1(active, botY))
						{
							OutPt op = this.AddOutPt(active.prevInAEL, active.bot);
							this.AddJoin(op, active.outrec.pts);
						}
					}
					while (active2.nextInAEL != null && this.IsValidAelOrder(active2.nextInAEL, active2))
					{
						this.IntersectEdges(active2, active2.nextInAEL, active2.bot);
						this.SwapPositionsInAEL(active2, active2.nextInAEL);
					}
					if (!ClipperBase.IsHorizontal(active2) && this.TestJoinWithNext1(active2, botY))
					{
						OutPt op2 = this.AddOutPt(active2.nextInAEL, active2.bot);
						this.AddJoin(active2.outrec.pts, op2);
					}
					if (ClipperBase.IsHorizontal(active2))
					{
						this.PushHorz(active2);
					}
					else
					{
						this.InsertScanline(active2.top.Y);
					}
				}
				else if (flag)
				{
					this.StartOpenPath(active, active.bot);
				}
				if (ClipperBase.IsHorizontal(active))
				{
					this.PushHorz(active);
				}
				else
				{
					this.InsertScanline(active.top.Y);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void PushHorz(Active ae)
		{
			ae.nextInSEL = this._sel;
			this._sel = ae;
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool PopHorz(out Active ae)
		{
			ae = this._sel;
			if (this._sel == null)
			{
				return false;
			}
			this._sel = this._sel.nextInSEL;
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TestJoinWithPrev1(Active e, long currY)
		{
			return ClipperBase.IsHotEdge(e) && !ClipperBase.IsOpen(e) && e.prevInAEL != null && e.prevInAEL.curX == e.curX && ClipperBase.IsHotEdge(e.prevInAEL) && !ClipperBase.IsOpen(e.prevInAEL) && currY - e.top.Y > 1L && currY - e.prevInAEL.top.Y > 1L && InternalClipper.CrossProduct(e.prevInAEL.top, e.bot, e.top) == 0.0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TestJoinWithPrev2(Active e, Point64 currPt)
		{
			return ClipperBase.IsHotEdge(e) && !ClipperBase.IsOpen(e) && e.prevInAEL != null && !ClipperBase.IsOpen(e.prevInAEL) && ClipperBase.IsHotEdge(e.prevInAEL) && e.prevInAEL.top.Y < e.bot.Y && Math.Abs(ClipperBase.TopX(e.prevInAEL, currPt.Y) - currPt.X) < 2L && InternalClipper.CrossProduct(e.prevInAEL.top, currPt, e.top) == 0.0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TestJoinWithNext1(Active e, long currY)
		{
			return ClipperBase.IsHotEdge(e) && !ClipperBase.IsOpen(e) && e.nextInAEL != null && e.nextInAEL.curX == e.curX && ClipperBase.IsHotEdge(e.nextInAEL) && !ClipperBase.IsOpen(e.nextInAEL) && currY - e.top.Y > 1L && currY - e.nextInAEL.top.Y > 1L && InternalClipper.CrossProduct(e.nextInAEL.top, e.bot, e.top) == 0.0;
		}

		private bool TestJoinWithNext2(Active e, Point64 currPt)
		{
			return ClipperBase.IsHotEdge(e) && !ClipperBase.IsOpen(e) && e.nextInAEL != null && !ClipperBase.IsOpen(e.nextInAEL) && ClipperBase.IsHotEdge(e.nextInAEL) && e.nextInAEL.top.Y < e.bot.Y && Math.Abs(ClipperBase.TopX(e.nextInAEL, currPt.Y) - currPt.X) < 2L && InternalClipper.CrossProduct(e.nextInAEL.top, currPt, e.top) == 0.0;
		}

		private OutPt AddLocalMinPoly(Active ae1, Active ae2, Point64 pt, bool isNew = false)
		{
			OutRec outRec = new OutRec();
			this._outrecList.Add(outRec);
			outRec.idx = this._outrecList.Count - 1;
			outRec.pts = null;
			outRec.polypath = null;
			ae1.outrec = outRec;
			ae2.outrec = outRec;
			if (ClipperBase.IsOpen(ae1))
			{
				outRec.owner = null;
				outRec.isOpen = true;
				if (ae1.windDx > 0)
				{
					ClipperBase.SetSides(outRec, ae1, ae2);
				}
				else
				{
					ClipperBase.SetSides(outRec, ae2, ae1);
				}
			}
			else
			{
				outRec.isOpen = false;
				Active prevHotEdge = ClipperBase.GetPrevHotEdge(ae1);
				if (prevHotEdge != null)
				{
					outRec.owner = prevHotEdge.outrec;
					if (this.OutrecIsAscending(prevHotEdge) == isNew)
					{
						ClipperBase.SetSides(outRec, ae2, ae1);
					}
					else
					{
						ClipperBase.SetSides(outRec, ae1, ae2);
					}
				}
				else
				{
					outRec.owner = null;
					if (isNew)
					{
						ClipperBase.SetSides(outRec, ae1, ae2);
					}
					else
					{
						ClipperBase.SetSides(outRec, ae2, ae1);
					}
				}
			}
			OutPt outPt = new OutPt(pt, outRec);
			outRec.pts = outPt;
			return outPt;
		}

		[return: Nullable(2)]
		private OutPt AddLocalMaxPoly(Active ae1, Active ae2, Point64 pt)
		{
			if (ClipperBase.IsFront(ae1) == ClipperBase.IsFront(ae2))
			{
				if (ClipperBase.IsOpenEnd(ae1))
				{
					ClipperBase.SwapFrontBackSides(ae1.outrec);
				}
				else
				{
					if (!ClipperBase.IsOpenEnd(ae2))
					{
						this._succeeded = false;
						return null;
					}
					ClipperBase.SwapFrontBackSides(ae2.outrec);
				}
			}
			OutPt outPt = this.AddOutPt(ae1, pt);
			if (ae1.outrec == ae2.outrec)
			{
				OutRec outrec = ae1.outrec;
				outrec.pts = outPt;
				ClipperBase.UncoupleOutRec(ae1);
				if (!ClipperBase.IsOpen(ae1))
				{
					this.CleanCollinear(outrec);
				}
				outPt = outrec.pts;
				outrec.owner = ClipperBase.GetRealOutRec(outrec.owner);
				if (this._using_polytree && outrec.owner != null && outrec.owner.frontEdge == null)
				{
					outrec.owner = ClipperBase.GetRealOutRec(outrec.owner.owner);
				}
			}
			else if (ClipperBase.IsOpen(ae1))
			{
				if (ae1.windDx < 0)
				{
					this.JoinOutrecPaths(ae1, ae2);
				}
				else
				{
					this.JoinOutrecPaths(ae2, ae1);
				}
			}
			else if (ae1.outrec.idx < ae2.outrec.idx)
			{
				this.JoinOutrecPaths(ae1, ae2);
			}
			else
			{
				this.JoinOutrecPaths(ae2, ae1);
			}
			return outPt;
		}

		private void JoinOutrecPaths(Active ae1, Active ae2)
		{
			OutPt pts = ae1.outrec.pts;
			OutPt pts2 = ae2.outrec.pts;
			OutPt next = pts.next;
			OutPt next2 = pts2.next;
			if (ClipperBase.IsFront(ae1))
			{
				next2.prev = pts;
				pts.next = next2;
				pts2.next = next;
				next.prev = pts2;
				ae1.outrec.pts = pts2;
				ae1.outrec.frontEdge = ae2.outrec.frontEdge;
				if (ae1.outrec.frontEdge != null)
				{
					ae1.outrec.frontEdge.outrec = ae1.outrec;
				}
			}
			else
			{
				next.prev = pts2;
				pts2.next = next;
				pts.next = next2;
				next2.prev = pts;
				ae1.outrec.backEdge = ae2.outrec.backEdge;
				if (ae1.outrec.backEdge != null)
				{
					ae1.outrec.backEdge.outrec = ae1.outrec;
				}
			}
			if (ae2.outrec.owner != null && ae2.outrec.owner.idx < ae1.outrec.idx && (ae1.outrec.owner == null || ae2.outrec.owner.idx < ae1.outrec.owner.idx))
			{
				ae1.outrec.owner = ae2.outrec.owner;
			}
			ae2.outrec.frontEdge = null;
			ae2.outrec.backEdge = null;
			ae2.outrec.pts = null;
			ae2.outrec.owner = ae1.outrec;
			if (ClipperBase.IsOpenEnd(ae1))
			{
				ae2.outrec.pts = ae1.outrec.pts;
				ae1.outrec.pts = null;
			}
			ae1.outrec = null;
			ae2.outrec = null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private OutPt AddOutPt(Active ae, Point64 pt)
		{
			OutRec outrec = ae.outrec;
			bool flag = ClipperBase.IsFront(ae);
			OutPt pts = outrec.pts;
			OutPt next = pts.next;
			OutPt outPt;
			if (flag && pt == pts.pt)
			{
				outPt = pts;
			}
			else if (!flag && pt == next.pt)
			{
				outPt = next;
			}
			else
			{
				outPt = new OutPt(pt, outrec);
				next.prev = outPt;
				outPt.prev = pts;
				outPt.next = next;
				pts.next = outPt;
				if (flag)
				{
					outrec.pts = outPt;
				}
			}
			return outPt;
		}

		private OutPt StartOpenPath(Active ae, Point64 pt)
		{
			OutRec outRec = new OutRec();
			this._outrecList.Add(outRec);
			outRec.idx = this._outrecList.Count - 1;
			outRec.owner = null;
			outRec.isOpen = true;
			outRec.pts = null;
			outRec.polypath = null;
			if (ae.windDx > 0)
			{
				outRec.frontEdge = ae;
				outRec.backEdge = null;
			}
			else
			{
				outRec.frontEdge = null;
				outRec.backEdge = ae;
			}
			ae.outrec = outRec;
			OutPt outPt = new OutPt(pt, outRec);
			outRec.pts = outPt;
			return outPt;
		}

		private void UpdateEdgeIntoAEL(Active ae)
		{
			ae.bot = ae.top;
			ae.vertexTop = ClipperBase.NextVertex(ae);
			ae.top = ae.vertexTop.pt;
			ae.curX = ae.bot.X;
			ClipperBase.SetDx(ae);
			if (ClipperBase.IsHorizontal(ae))
			{
				return;
			}
			this.InsertScanline(ae.top.Y);
			if (this.TestJoinWithPrev1(ae, ae.bot.Y))
			{
				OutPt op = this.AddOutPt(ae.prevInAEL, ae.bot);
				OutPt op2 = this.AddOutPt(ae, ae.bot);
				this.AddJoin(op, op2);
			}
		}

		[return: Nullable(2)]
		private Active FindEdgeWithMatchingLocMin(Active e)
		{
			Active active = e.nextInAEL;
			while (active != null)
			{
				if (active.localMin == e.localMin)
				{
					return active;
				}
				if (!ClipperBase.IsHorizontal(active) && e.bot != active.bot)
				{
					active = null;
				}
				else
				{
					active = active.nextInAEL;
				}
			}
			for (active = e.prevInAEL; active != null; active = active.prevInAEL)
			{
				if (active.localMin == e.localMin)
				{
					return active;
				}
				if (!ClipperBase.IsHorizontal(active) && e.bot != active.bot)
				{
					return null;
				}
			}
			return active;
		}

		[return: Nullable(2)]
		private OutPt IntersectEdges(Active ae1, Active ae2, Point64 pt)
		{
			OutPt outPt = null;
			if (this._hasOpenPaths && (ClipperBase.IsOpen(ae1) || ClipperBase.IsOpen(ae2)))
			{
				if (ClipperBase.IsOpen(ae1) && ClipperBase.IsOpen(ae2))
				{
					return null;
				}
				if (ClipperBase.IsOpen(ae2))
				{
					ClipperBase.SwapActives(ref ae1, ref ae2);
				}
				if (this._cliptype == ClipType.Union)
				{
					if (!ClipperBase.IsHotEdge(ae2))
					{
						return null;
					}
				}
				else if (ae2.localMin.polytype == PathType.Subject)
				{
					return null;
				}
				FillRule fillrule = this._fillrule;
				if (fillrule != FillRule.Positive)
				{
					if (fillrule != FillRule.Negative)
					{
						if (Math.Abs(ae2.windCount) != 1)
						{
							return null;
						}
					}
					else if (ae2.windCount != -1)
					{
						return null;
					}
				}
				else if (ae2.windCount != 1)
				{
					return null;
				}
				if (ClipperBase.IsHotEdge(ae1))
				{
					outPt = this.AddOutPt(ae1, pt);
					if (ClipperBase.IsFront(ae1))
					{
						ae1.outrec.frontEdge = null;
					}
					else
					{
						ae1.outrec.backEdge = null;
					}
					ae1.outrec = null;
				}
				else if (pt == ae1.localMin.vertex.pt && !ClipperBase.IsOpenEnd(ae1.localMin.vertex))
				{
					Active active = this.FindEdgeWithMatchingLocMin(ae1);
					if (active != null && ClipperBase.IsHotEdge(active))
					{
						ae1.outrec = active.outrec;
						if (ae1.windDx > 0)
						{
							ClipperBase.SetSides(active.outrec, ae1, active);
						}
						else
						{
							ClipperBase.SetSides(active.outrec, active, ae1);
						}
						return active.outrec.pts;
					}
					outPt = this.StartOpenPath(ae1, pt);
				}
				else
				{
					outPt = this.StartOpenPath(ae1, pt);
				}
				return outPt;
			}
			else
			{
				int num;
				if (ae1.localMin.polytype == ae2.localMin.polytype)
				{
					if (this._fillrule == FillRule.EvenOdd)
					{
						num = ae1.windCount;
						ae1.windCount = ae2.windCount;
						ae2.windCount = num;
					}
					else
					{
						if (ae1.windCount + ae2.windDx == 0)
						{
							ae1.windCount = -ae1.windCount;
						}
						else
						{
							ae1.windCount += ae2.windDx;
						}
						if (ae2.windCount - ae1.windDx == 0)
						{
							ae2.windCount = -ae2.windCount;
						}
						else
						{
							ae2.windCount -= ae1.windDx;
						}
					}
				}
				else
				{
					if (this._fillrule != FillRule.EvenOdd)
					{
						ae1.windCount2 += ae2.windDx;
					}
					else
					{
						ae1.windCount2 = ((ae1.windCount2 == 0) ? 1 : 0);
					}
					if (this._fillrule != FillRule.EvenOdd)
					{
						ae2.windCount2 -= ae1.windDx;
					}
					else
					{
						ae2.windCount2 = ((ae2.windCount2 == 0) ? 1 : 0);
					}
				}
				FillRule fillrule = this._fillrule;
				int num2;
				if (fillrule != FillRule.Positive)
				{
					if (fillrule != FillRule.Negative)
					{
						num = Math.Abs(ae1.windCount);
						num2 = Math.Abs(ae2.windCount);
					}
					else
					{
						num = -ae1.windCount;
						num2 = -ae2.windCount;
					}
				}
				else
				{
					num = ae1.windCount;
					num2 = ae2.windCount;
				}
				bool flag = num == 0 || num == 1;
				bool flag2 = num2 == 0 || num2 == 1;
				if ((!ClipperBase.IsHotEdge(ae1) && !flag) || (!ClipperBase.IsHotEdge(ae2) && !flag2))
				{
					return null;
				}
				if (ClipperBase.IsHotEdge(ae1) && ClipperBase.IsHotEdge(ae2))
				{
					if ((num != 0 && num != 1) || (num2 != 0 && num2 != 1) || (ae1.localMin.polytype != ae2.localMin.polytype && this._cliptype != ClipType.Xor))
					{
						outPt = this.AddLocalMaxPoly(ae1, ae2, pt);
					}
					else if (ClipperBase.IsFront(ae1) || ae1.outrec == ae2.outrec)
					{
						outPt = this.AddLocalMaxPoly(ae1, ae2, pt);
						OutPt outPt2 = this.AddLocalMinPoly(ae1, ae2, pt, false);
						if (outPt != null && outPt.pt == outPt2.pt && !ClipperBase.IsHorizontal(ae1) && !ClipperBase.IsHorizontal(ae2) && InternalClipper.CrossProduct(ae1.bot, outPt.pt, ae2.bot) == 0.0)
						{
							this.AddJoin(outPt, outPt2);
						}
					}
					else
					{
						outPt = this.AddOutPt(ae1, pt);
						this.AddOutPt(ae2, pt);
						ClipperBase.SwapOutrecs(ae1, ae2);
					}
				}
				else if (ClipperBase.IsHotEdge(ae1))
				{
					outPt = this.AddOutPt(ae1, pt);
					ClipperBase.SwapOutrecs(ae1, ae2);
				}
				else if (ClipperBase.IsHotEdge(ae2))
				{
					outPt = this.AddOutPt(ae2, pt);
					ClipperBase.SwapOutrecs(ae1, ae2);
				}
				else
				{
					fillrule = this._fillrule;
					long num3;
					long num4;
					if (fillrule != FillRule.Positive)
					{
						if (fillrule != FillRule.Negative)
						{
							num3 = (long)Math.Abs(ae1.windCount2);
							num4 = (long)Math.Abs(ae2.windCount2);
						}
						else
						{
							num3 = (long)(-(long)ae1.windCount2);
							num4 = (long)(-(long)ae2.windCount2);
						}
					}
					else
					{
						num3 = (long)ae1.windCount2;
						num4 = (long)ae2.windCount2;
					}
					if (!ClipperBase.IsSamePolyType(ae1, ae2))
					{
						outPt = this.AddLocalMinPoly(ae1, ae2, pt, false);
					}
					else if (num == 1 && num2 == 1)
					{
						outPt = null;
						switch (this._cliptype)
						{
						case ClipType.Union:
							if (num3 > 0L && num4 > 0L)
							{
								return null;
							}
							outPt = this.AddLocalMinPoly(ae1, ae2, pt, false);
							break;
						case ClipType.Difference:
							if ((ClipperBase.GetPolyType(ae1) == PathType.Clip && num3 > 0L && num4 > 0L) || (ClipperBase.GetPolyType(ae1) == PathType.Subject && num3 <= 0L && num4 <= 0L))
							{
								outPt = this.AddLocalMinPoly(ae1, ae2, pt, false);
							}
							break;
						case ClipType.Xor:
							outPt = this.AddLocalMinPoly(ae1, ae2, pt, false);
							break;
						default:
							if (num3 <= 0L || num4 <= 0L)
							{
								return null;
							}
							outPt = this.AddLocalMinPoly(ae1, ae2, pt, false);
							break;
						}
					}
				}
				return outPt;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DeleteFromAEL(Active ae)
		{
			Active prevInAEL = ae.prevInAEL;
			Active nextInAEL = ae.nextInAEL;
			if (prevInAEL == null && nextInAEL == null && ae != this._actives)
			{
				return;
			}
			if (prevInAEL != null)
			{
				prevInAEL.nextInAEL = nextInAEL;
			}
			else
			{
				this._actives = nextInAEL;
			}
			if (nextInAEL != null)
			{
				nextInAEL.prevInAEL = prevInAEL;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AdjustCurrXAndCopyToSEL(long topY)
		{
			Active active = this._actives;
			this._sel = active;
			while (active != null)
			{
				active.prevInSEL = active.prevInAEL;
				active.nextInSEL = active.nextInAEL;
				active.jump = active.nextInSEL;
				active.curX = ClipperBase.TopX(active, topY);
				active = active.nextInAEL;
			}
		}

		protected void ExecuteInternal(ClipType ct, FillRule fillRule)
		{
			if (ct == ClipType.None)
			{
				return;
			}
			this._fillrule = fillRule;
			this._cliptype = ct;
			this.Reset();
			long num;
			if (!this.PopScanline(out num))
			{
				return;
			}
			while (this._succeeded)
			{
				this.InsertLocalMinimaIntoAEL(num);
				Active horz;
				while (this.PopHorz(out horz))
				{
					this.DoHorizontal(horz);
				}
				this.ConvertHorzTrialsToJoins();
				this._currentBotY = num;
				if (!this.PopScanline(out num))
				{
					break;
				}
				this.DoIntersections(num);
				this.DoTopOfScanbeam(num);
				while (this.PopHorz(out horz))
				{
					this.DoHorizontal(horz);
				}
			}
			if (this._succeeded)
			{
				this.ProcessJoinList();
			}
		}

		private void DoIntersections(long topY)
		{
			if (this.BuildIntersectList(topY))
			{
				this.ProcessIntersectList();
				this.DisposeIntersectNodes();
			}
		}

		private void DisposeIntersectNodes()
		{
			this._intersectList.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddNewIntersectNode(Active ae1, Active ae2, long topY)
		{
			Point64 intersectPoint = ClipperBase.GetIntersectPoint(ae1, ae2);
			if (intersectPoint.Y > this._currentBotY)
			{
				if (Math.Abs(ae1.dx) < Math.Abs(ae2.dx))
				{
					intersectPoint = new Point64(ClipperBase.TopX(ae1, this._currentBotY), this._currentBotY);
				}
				else
				{
					intersectPoint = new Point64(ClipperBase.TopX(ae2, this._currentBotY), this._currentBotY);
				}
			}
			else if (intersectPoint.Y < topY)
			{
				if (ae1.top.Y == topY)
				{
					intersectPoint = new Point64(ae1.top.X, topY);
				}
				else if (ae2.top.Y == topY)
				{
					intersectPoint = new Point64(ae2.top.X, topY);
				}
				else if (Math.Abs(ae1.dx) < Math.Abs(ae2.dx))
				{
					intersectPoint = new Point64(ae1.curX, topY);
				}
				else
				{
					intersectPoint = new Point64(ae2.curX, topY);
				}
			}
			IntersectNode item = new IntersectNode(intersectPoint, ae1, ae2);
			this._intersectList.Add(item);
		}

		[return: Nullable(2)]
		private Active ExtractFromSEL(Active ae)
		{
			Active nextInSEL = ae.nextInSEL;
			if (nextInSEL != null)
			{
				nextInSEL.prevInSEL = ae.prevInSEL;
			}
			ae.prevInSEL.nextInSEL = nextInSEL;
			return nextInSEL;
		}

		private void Insert1Before2InSEL(Active ae1, Active ae2)
		{
			ae1.prevInSEL = ae2.prevInSEL;
			if (ae1.prevInSEL != null)
			{
				ae1.prevInSEL.nextInSEL = ae1;
			}
			ae1.nextInSEL = ae2;
			ae2.prevInSEL = ae1;
		}

		private bool BuildIntersectList(long topY)
		{
			if (this._actives == null || this._actives.nextInAEL == null)
			{
				return false;
			}
			this.AdjustCurrXAndCopyToSEL(topY);
			Active active = this._sel;
			while (active.jump != null)
			{
				Active active2 = null;
				while (active != null && active.jump != null)
				{
					Active active3 = active;
					Active active4 = active.jump;
					Active active5 = active4;
					Active jump = active4.jump;
					active.jump = jump;
					while (active != active5 && active4 != jump)
					{
						if (active4.curX < active.curX)
						{
							Active active6 = active4.prevInSEL;
							for (;;)
							{
								this.AddNewIntersectNode(active6, active4, topY);
								if (active6 == active)
								{
									break;
								}
								active6 = active6.prevInSEL;
							}
							active6 = active4;
							active4 = this.ExtractFromSEL(active6);
							active5 = active4;
							this.Insert1Before2InSEL(active6, active);
							if (active == active3)
							{
								active3 = active6;
								active3.jump = jump;
								if (active2 == null)
								{
									this._sel = active3;
								}
								else
								{
									active2.jump = active3;
								}
							}
						}
						else
						{
							active = active.nextInSEL;
						}
					}
					active2 = active3;
					active = jump;
				}
				active = this._sel;
			}
			return this._intersectList.Count > 0;
		}

		private void ProcessIntersectList()
		{
			this._intersectList.Sort(default(ClipperBase.IntersectListSort));
			for (int i = 0; i < this._intersectList.Count; i++)
			{
				if (!ClipperBase.EdgesAdjacentInAEL(this._intersectList[i]))
				{
					int num = i + 1;
					while (num < this._intersectList.Count && !ClipperBase.EdgesAdjacentInAEL(this._intersectList[num]))
					{
						num++;
					}
					if (num < this._intersectList.Count)
					{
						List<IntersectNode> intersectList = this._intersectList;
						int index = num;
						List<IntersectNode> intersectList2 = this._intersectList;
						int index2 = i;
						IntersectNode value = this._intersectList[i];
						IntersectNode value2 = this._intersectList[num];
						intersectList[index] = value;
						intersectList2[index2] = value2;
					}
				}
				IntersectNode intersectNode = this._intersectList[i];
				this.IntersectEdges(intersectNode.edge1, intersectNode.edge2, intersectNode.pt);
				this.SwapPositionsInAEL(intersectNode.edge1, intersectNode.edge2);
				if (this.TestJoinWithPrev2(intersectNode.edge2, intersectNode.pt))
				{
					OutPt outPt = this.AddOutPt(intersectNode.edge2.prevInAEL, intersectNode.pt);
					OutPt outPt2 = this.AddOutPt(intersectNode.edge2, intersectNode.pt);
					if (outPt != outPt2)
					{
						this.AddJoin(outPt, outPt2);
					}
				}
				else if (this.TestJoinWithNext2(intersectNode.edge1, intersectNode.pt))
				{
					OutPt outPt3 = this.AddOutPt(intersectNode.edge1, intersectNode.pt);
					OutPt outPt4 = this.AddOutPt(intersectNode.edge1.nextInAEL, intersectNode.pt);
					if (outPt3 != outPt4)
					{
						this.AddJoin(outPt3, outPt4);
					}
				}
			}
		}

		private void SwapPositionsInAEL(Active ae1, Active ae2)
		{
			Active nextInAEL = ae2.nextInAEL;
			if (nextInAEL != null)
			{
				nextInAEL.prevInAEL = ae1;
			}
			Active prevInAEL = ae1.prevInAEL;
			if (prevInAEL != null)
			{
				prevInAEL.nextInAEL = ae2;
			}
			ae2.prevInAEL = prevInAEL;
			ae2.nextInAEL = ae1;
			ae1.prevInAEL = ae2;
			ae1.nextInAEL = nextInAEL;
			if (ae2.prevInAEL == null)
			{
				this._actives = ae2;
			}
		}

		private bool ResetHorzDirection(Active horz, [Nullable(2)] Active maxPair, out long leftX, out long rightX)
		{
			if (horz.bot.X == horz.top.X)
			{
				leftX = horz.curX;
				rightX = horz.curX;
				Active nextInAEL = horz.nextInAEL;
				while (nextInAEL != null && nextInAEL != maxPair)
				{
					nextInAEL = nextInAEL.nextInAEL;
				}
				return nextInAEL != null;
			}
			if (horz.curX < horz.top.X)
			{
				leftX = horz.curX;
				rightX = horz.top.X;
				return true;
			}
			leftX = horz.top.X;
			rightX = horz.curX;
			return false;
		}

		private bool HorzIsSpike(Active horz)
		{
			Point64 pt = ClipperBase.NextVertex(horz).pt;
			return horz.bot.X < horz.top.X != horz.top.X < pt.X;
		}

		private bool TrimHorz(Active horzEdge, bool preserveCollinear)
		{
			bool flag = false;
			Point64 pt = ClipperBase.NextVertex(horzEdge).pt;
			while (pt.Y == horzEdge.top.Y && (!preserveCollinear || pt.X < horzEdge.top.X == horzEdge.bot.X < horzEdge.top.X))
			{
				horzEdge.vertexTop = ClipperBase.NextVertex(horzEdge);
				horzEdge.top = pt;
				flag = true;
				if (ClipperBase.IsMaxima(horzEdge))
				{
					break;
				}
				pt = ClipperBase.NextVertex(horzEdge).pt;
			}
			if (flag)
			{
				ClipperBase.SetDx(horzEdge);
			}
			return flag;
		}

		private void DoHorizontal(Active horz)
		{
			bool flag = ClipperBase.IsOpen(horz);
			long y = horz.bot.Y;
			Vertex vertex = null;
			Active active = null;
			if (!flag)
			{
				vertex = ClipperBase.GetCurrYMaximaVertex(horz);
				if (vertex != null)
				{
					active = ClipperBase.GetHorzMaximaPair(horz, vertex);
					if (vertex != horz.vertexTop)
					{
						this.TrimHorz(horz, this.PreserveCollinear);
					}
				}
			}
			long num;
			long num2;
			bool flag2 = this.ResetHorzDirection(horz, active, out num, out num2);
			if (ClipperBase.IsHotEdge(horz))
			{
				this.AddOutPt(horz, new Point64(horz.curX, y));
			}
			Active active2;
			OutPt outPt;
			for (;;)
			{
				if (flag && ClipperBase.IsMaxima(horz) && !ClipperBase.IsOpenEnd(horz))
				{
					vertex = ClipperBase.GetCurrYMaximaVertex(horz);
					if (vertex != null)
					{
						active = ClipperBase.GetHorzMaximaPair(horz, vertex);
					}
				}
				if (flag2)
				{
					active2 = horz.nextInAEL;
				}
				else
				{
					active2 = horz.prevInAEL;
				}
				while (active2 != null)
				{
					if (active2 == active)
					{
						goto Block_10;
					}
					Point64 pt;
					if (vertex != horz.vertexTop || ClipperBase.IsOpenEnd(horz))
					{
						if ((flag2 && active2.curX > num2) || (!flag2 && active2.curX < num))
						{
							break;
						}
						if (active2.curX == horz.top.X && !ClipperBase.IsHorizontal(active2))
						{
							pt = ClipperBase.NextVertex(horz).pt;
							if (flag2)
							{
								if (ClipperBase.IsOpen(active2) && !ClipperBase.IsSamePolyType(active2, horz) && !ClipperBase.IsHotEdge(active2))
								{
									if (ClipperBase.TopX(active2, pt.Y) > pt.X)
									{
										break;
									}
								}
								else if (ClipperBase.TopX(active2, pt.Y) >= pt.X)
								{
									break;
								}
							}
							else if (ClipperBase.IsOpen(active2) && !ClipperBase.IsSamePolyType(active2, horz) && !ClipperBase.IsHotEdge(active2))
							{
								if (ClipperBase.TopX(active2, pt.Y) < pt.X)
								{
									break;
								}
							}
							else if (ClipperBase.TopX(active2, pt.Y) <= pt.X)
							{
								break;
							}
						}
					}
					pt = new Point64(active2.curX, y);
					if (flag2)
					{
						outPt = this.IntersectEdges(horz, active2, pt);
						this.SwapPositionsInAEL(horz, active2);
						if (ClipperBase.IsHotEdge(horz) && outPt != null && !ClipperBase.IsOpen(horz) && outPt.pt == pt)
						{
							this.AddTrialHorzJoin(outPt);
						}
						if (!ClipperBase.IsHorizontal(active2) && this.TestJoinWithPrev1(active2, y))
						{
							outPt = this.AddOutPt(active2.prevInAEL, pt);
							OutPt op = this.AddOutPt(active2, pt);
							this.AddJoin(outPt, op);
						}
						horz.curX = active2.curX;
						active2 = horz.nextInAEL;
					}
					else
					{
						outPt = this.IntersectEdges(active2, horz, pt);
						this.SwapPositionsInAEL(active2, horz);
						if (ClipperBase.IsHotEdge(horz) && outPt != null && !ClipperBase.IsOpen(horz) && outPt.pt == pt)
						{
							this.AddTrialHorzJoin(outPt);
						}
						if (!ClipperBase.IsHorizontal(active2) && this.TestJoinWithNext1(active2, y))
						{
							outPt = this.AddOutPt(active2, pt);
							OutPt op2 = this.AddOutPt(active2.nextInAEL, pt);
							this.AddJoin(outPt, op2);
						}
						horz.curX = active2.curX;
						active2 = horz.prevInAEL;
					}
				}
				if (flag && ClipperBase.IsOpenEnd(horz))
				{
					goto Block_45;
				}
				if (ClipperBase.NextVertex(horz).pt.Y != horz.top.Y)
				{
					goto IL_432;
				}
				if (ClipperBase.IsHotEdge(horz))
				{
					this.AddOutPt(horz, horz.top);
				}
				this.UpdateEdgeIntoAEL(horz);
				if (this.PreserveCollinear && this.HorzIsSpike(horz))
				{
					this.TrimHorz(horz, true);
				}
				flag2 = this.ResetHorzDirection(horz, active, out num, out num2);
			}
			Block_10:
			if (ClipperBase.IsHotEdge(horz))
			{
				while (horz.vertexTop != active2.vertexTop)
				{
					this.AddOutPt(horz, horz.top);
					this.UpdateEdgeIntoAEL(horz);
				}
				outPt = this.AddLocalMaxPoly(horz, active2, horz.top);
				if (outPt != null && !ClipperBase.IsOpen(horz) && outPt.pt == horz.top)
				{
					this.AddTrialHorzJoin(outPt);
				}
			}
			this.DeleteFromAEL(active2);
			this.DeleteFromAEL(horz);
			return;
			Block_45:
			if (ClipperBase.IsHotEdge(horz))
			{
				this.AddOutPt(horz, horz.top);
				if (ClipperBase.IsFront(horz))
				{
					horz.outrec.frontEdge = null;
				}
				else
				{
					horz.outrec.backEdge = null;
				}
			}
			horz.outrec = null;
			this.DeleteFromAEL(horz);
			return;
			IL_432:
			if (ClipperBase.IsHotEdge(horz))
			{
				outPt = this.AddOutPt(horz, horz.top);
				if (!ClipperBase.IsOpen(horz))
				{
					this.AddTrialHorzJoin(outPt);
				}
			}
			else
			{
				outPt = null;
			}
			if ((flag && !ClipperBase.IsOpenEnd(horz)) || (!flag && vertex != horz.vertexTop))
			{
				this.UpdateEdgeIntoAEL(horz);
				if (ClipperBase.IsOpen(horz))
				{
					return;
				}
				if (flag2 && this.TestJoinWithNext1(horz, y))
				{
					OutPt op3 = this.AddOutPt(horz.nextInAEL, horz.bot);
					this.AddJoin(outPt, op3);
					return;
				}
				if (!flag2 && this.TestJoinWithPrev1(horz, y))
				{
					OutPt op4 = this.AddOutPt(horz.prevInAEL, horz.bot);
					this.AddJoin(op4, outPt);
					return;
				}
			}
			else
			{
				if (ClipperBase.IsHotEdge(horz))
				{
					this.AddLocalMaxPoly(horz, active, horz.top);
					return;
				}
				this.DeleteFromAEL(active);
				this.DeleteFromAEL(horz);
			}
		}

		private void DoTopOfScanbeam(long y)
		{
			this._sel = null;
			for (Active active = this._actives; active != null; active = active.nextInAEL)
			{
				if (active.top.Y == y)
				{
					active.curX = active.top.X;
					if (ClipperBase.IsMaxima(active))
					{
						active = this.DoMaxima(active);
						continue;
					}
					if (ClipperBase.IsHotEdge(active))
					{
						this.AddOutPt(active, active.top);
					}
					this.UpdateEdgeIntoAEL(active);
					if (ClipperBase.IsHorizontal(active))
					{
						this.PushHorz(active);
					}
				}
				else
				{
					active.curX = ClipperBase.TopX(active, y);
				}
			}
		}

		[return: Nullable(2)]
		private Active DoMaxima(Active ae)
		{
			Active prevInAEL = ae.prevInAEL;
			Active nextInAEL = ae.nextInAEL;
			if (ClipperBase.IsOpenEnd(ae))
			{
				if (ClipperBase.IsHotEdge(ae))
				{
					this.AddOutPt(ae, ae.top);
				}
				if (!ClipperBase.IsHorizontal(ae))
				{
					if (ClipperBase.IsHotEdge(ae))
					{
						if (ClipperBase.IsFront(ae))
						{
							ae.outrec.frontEdge = null;
						}
						else
						{
							ae.outrec.backEdge = null;
						}
						ae.outrec = null;
					}
					this.DeleteFromAEL(ae);
				}
				return nextInAEL;
			}
			Active maximaPair = this.GetMaximaPair(ae);
			if (maximaPair == null)
			{
				return nextInAEL;
			}
			while (nextInAEL != maximaPair)
			{
				this.IntersectEdges(ae, nextInAEL, ae.top);
				this.SwapPositionsInAEL(ae, nextInAEL);
				nextInAEL = ae.nextInAEL;
			}
			if (ClipperBase.IsOpen(ae))
			{
				if (ClipperBase.IsHotEdge(ae))
				{
					this.AddLocalMaxPoly(ae, maximaPair, ae.top);
				}
				this.DeleteFromAEL(maximaPair);
				this.DeleteFromAEL(ae);
				if (prevInAEL == null)
				{
					return this._actives;
				}
				return prevInAEL.nextInAEL;
			}
			else
			{
				if (ClipperBase.IsHotEdge(ae))
				{
					this.AddLocalMaxPoly(ae, maximaPair, ae.top);
				}
				this.DeleteFromAEL(ae);
				this.DeleteFromAEL(maximaPair);
				if (prevInAEL == null)
				{
					return this._actives;
				}
				return prevInAEL.nextInAEL;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsValidPath(OutPt op)
		{
			return op.next != op;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool AreReallyClose(Point64 pt1, Point64 pt2)
		{
			return Math.Abs(pt1.X - pt2.X) < 2L && Math.Abs(pt1.Y - pt2.Y) < 2L;
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsValidClosedPath(OutPt op)
		{
			return op != null && op.next != op && op.next != op.prev && (op.next.next != op.prev || (!ClipperBase.AreReallyClose(op.pt, op.next.pt) && !ClipperBase.AreReallyClose(op.pt, op.prev.pt)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool ValueBetween(long val, long end1, long end2)
		{
			return val != end1 == (val != end2) && val > end1 == val < end2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool ValueEqualOrBetween(long val, long end1, long end2)
		{
			return val == end1 || val == end2 || val > end1 == val < end2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool PointBetween(Point64 pt, Point64 corner1, Point64 corner2)
		{
			return ClipperBase.ValueEqualOrBetween(pt.X, corner1.X, corner2.X) && ClipperBase.ValueEqualOrBetween(pt.Y, corner1.Y, corner2.Y);
		}

		private static bool CollinearSegsOverlap(Point64 seg1a, Point64 seg1b, Point64 seg2a, Point64 seg2b)
		{
			if (seg1a.X == seg1b.X)
			{
				if (seg2a.X != seg1a.X || seg2a.X != seg2b.X)
				{
					return false;
				}
			}
			else if (seg1a.X < seg1b.X)
			{
				if (seg2a.X < seg2b.X)
				{
					if (seg2a.X >= seg1b.X || seg2b.X <= seg1a.X)
					{
						return false;
					}
				}
				else if (seg2b.X >= seg1b.X || seg2a.X <= seg1a.X)
				{
					return false;
				}
			}
			else if (seg2a.X < seg2b.X)
			{
				if (seg2a.X >= seg1a.X || seg2b.X <= seg1b.X)
				{
					return false;
				}
			}
			else if (seg2b.X >= seg1a.X || seg2a.X <= seg1b.X)
			{
				return false;
			}
			if (seg1a.Y == seg1b.Y)
			{
				if (seg2a.Y != seg1a.Y || seg2a.Y != seg2b.Y)
				{
					return false;
				}
			}
			else if (seg1a.Y < seg1b.Y)
			{
				if (seg2a.Y < seg2b.Y)
				{
					if (seg2a.Y >= seg1b.Y || seg2b.Y <= seg1a.Y)
					{
						return false;
					}
				}
				else if (seg2b.Y >= seg1b.Y || seg2a.Y <= seg1a.Y)
				{
					return false;
				}
			}
			else if (seg2a.Y < seg2b.Y)
			{
				if (seg2a.Y >= seg1a.Y || seg2b.Y <= seg1b.Y)
				{
					return false;
				}
			}
			else if (seg2b.Y >= seg1a.Y || seg2a.Y <= seg1b.Y)
			{
				return false;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool HorzEdgesOverlap(long x1a, long x1b, long x2a, long x2b)
		{
			if (x1a > x1b + 2L)
			{
				if (x2a > x2b + 2L)
				{
					return x1a > x2b && x2a > x1b;
				}
				return x1a > x2a && x2b > x1b;
			}
			else
			{
				if (x1b <= x1a + 2L)
				{
					return false;
				}
				if (x2a > x2b + 2L)
				{
					return x1b > x2b && x2a > x1a;
				}
				return x1b > x2a && x2b > x1a;
			}
		}

		[return: Nullable(2)]
		private Joiner GetHorzTrialParent(OutPt op)
		{
			Joiner joiner = op.joiner;
			while (joiner != null)
			{
				if (joiner.op1 == op)
				{
					if (joiner.next1 != null && joiner.next1.idx < 0)
					{
						return joiner;
					}
					joiner = joiner.next1;
				}
				else
				{
					if (joiner.next2 != null && joiner.next2.idx < 0)
					{
						return joiner;
					}
					joiner = joiner.next1;
				}
			}
			return joiner;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool OutPtInTrialHorzList(OutPt op)
		{
			return op.joiner != null && (op.joiner.idx < 0 || this.GetHorzTrialParent(op) != null);
		}

		[NullableContext(2)]
		private bool ValidateClosedPathEx(ref OutPt op)
		{
			if (ClipperBase.IsValidClosedPath(op))
			{
				return true;
			}
			if (op != null)
			{
				this.SafeDisposeOutPts(ref op);
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static OutPt InsertOp(Point64 pt, OutPt insertAfter)
		{
			OutPt outPt = new OutPt(pt, insertAfter.outrec)
			{
				next = insertAfter.next
			};
			insertAfter.next.prev = outPt;
			insertAfter.next = outPt;
			outPt.prev = insertAfter;
			return outPt;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: Nullable(2)]
		private static OutPt DisposeOutPt(OutPt op)
		{
			OutPt result = (op.next == op) ? null : op.next;
			op.prev.next = op.next;
			op.next.prev = op.prev;
			return result;
		}

		private void SafeDisposeOutPts(ref OutPt op)
		{
			OutRec realOutRec = ClipperBase.GetRealOutRec(op.outrec);
			if (realOutRec.frontEdge != null)
			{
				realOutRec.frontEdge.outrec = null;
			}
			if (realOutRec.backEdge != null)
			{
				realOutRec.backEdge.outrec = null;
			}
			op.prev.next = null;
			for (OutPt outPt = op; outPt != null; outPt = outPt.next)
			{
				this.SafeDeleteOutPtJoiners(outPt);
			}
			realOutRec.pts = null;
		}

		private void SafeDeleteOutPtJoiners(OutPt op)
		{
			Joiner joiner = op.joiner;
			if (joiner == null)
			{
				return;
			}
			while (joiner != null)
			{
				if (joiner.idx < 0)
				{
					this.DeleteTrialHorzJoin(op);
				}
				else if (this._horzJoiners != null)
				{
					if (this.OutPtInTrialHorzList(joiner.op1))
					{
						this.DeleteTrialHorzJoin(joiner.op1);
					}
					if (this.OutPtInTrialHorzList(joiner.op2))
					{
						this.DeleteTrialHorzJoin(joiner.op2);
					}
					this.DeleteJoin(joiner);
				}
				else
				{
					this.DeleteJoin(joiner);
				}
				joiner = op.joiner;
			}
		}

		private void AddTrialHorzJoin(OutPt op)
		{
			if (!op.outrec.isOpen && !this.OutPtInTrialHorzList(op))
			{
				this._horzJoiners = new Joiner(null, op, null, this._horzJoiners);
			}
		}

		[return: Nullable(2)]
		private static Joiner FindTrialJoinParent(ref Joiner joiner, OutPt op)
		{
			Joiner joiner2 = joiner;
			while (joiner2 != null)
			{
				if (op == joiner2.op1)
				{
					if (joiner2.next1 != null && joiner2.next1.idx < 0)
					{
						joiner = joiner2.next1;
						return joiner2;
					}
					joiner2 = joiner2.next1;
				}
				else
				{
					if (joiner2.next2 != null && joiner2.next2.idx < 0)
					{
						joiner = joiner2.next2;
						return joiner2;
					}
					joiner2 = joiner2.next2;
				}
			}
			return null;
		}

		private void DeleteTrialHorzJoin(OutPt op)
		{
			if (this._horzJoiners == null)
			{
				return;
			}
			Joiner joiner = op.joiner;
			Joiner joiner2 = null;
			while (joiner != null)
			{
				if (joiner.idx < 0)
				{
					if (joiner == this._horzJoiners)
					{
						this._horzJoiners = joiner.nextH;
					}
					else
					{
						Joiner joiner3 = this._horzJoiners;
						while (joiner3.nextH != joiner)
						{
							joiner3 = joiner3.nextH;
						}
						joiner3.nextH = joiner.nextH;
					}
					if (joiner2 == null)
					{
						op.joiner = joiner.next1;
						joiner = op.joiner;
					}
					else
					{
						if (op == joiner2.op1)
						{
							joiner2.next1 = joiner.next1;
						}
						else
						{
							joiner2.next2 = joiner.next1;
						}
						joiner = joiner2;
					}
				}
				else
				{
					joiner2 = ClipperBase.FindTrialJoinParent(ref joiner, op);
					if (joiner2 == null)
					{
						break;
					}
				}
			}
		}

		private bool GetHorzExtendedHorzSeg(ref OutPt op, out OutPt op2)
		{
			OutRec realOutRec = ClipperBase.GetRealOutRec(op.outrec);
			op2 = op;
			if (realOutRec.frontEdge != null)
			{
				while (op.prev != realOutRec.pts)
				{
					if (op.prev.pt.Y != op.pt.Y)
					{
						break;
					}
					op = op.prev;
				}
				while (op2 != realOutRec.pts && op2.next.pt.Y == op2.pt.Y)
				{
					op2 = op2.next;
				}
				return op2 != op;
			}
			while (op.prev != op2)
			{
				if (op.prev.pt.Y != op.pt.Y)
				{
					break;
				}
				op = op.prev;
			}
			while (op2.next != op && op2.next.pt.Y == op2.pt.Y)
			{
				op2 = op2.next;
			}
			return op2 != op && op2.next != op;
		}

		private void ConvertHorzTrialsToJoins()
		{
			while (this._horzJoiners != null)
			{
				Joiner joiner = this._horzJoiners;
				this._horzJoiners = this._horzJoiners.nextH;
				OutPt op = joiner.op1;
				if (op.joiner == joiner)
				{
					op.joiner = joiner.next1;
				}
				else
				{
					Joiner joiner2 = ClipperBase.FindJoinParent(joiner, op);
					if (joiner2.op1 == op)
					{
						joiner2.next1 = joiner.next1;
					}
					else
					{
						joiner2.next2 = joiner.next1;
					}
				}
				OutPt outPt;
				if (!this.GetHorzExtendedHorzSeg(ref op, out outPt))
				{
					if (op.outrec.frontEdge == null)
					{
						this.CleanCollinear(op.outrec);
					}
				}
				else
				{
					bool flag = false;
					joiner = this._horzJoiners;
					while (joiner != null)
					{
						OutPt op2 = joiner.op1;
						OutPt outPt2;
						if (this.GetHorzExtendedHorzSeg(ref op2, out outPt2) && ClipperBase.HorzEdgesOverlap(op.pt.X, outPt.pt.X, op2.pt.X, outPt2.pt.X))
						{
							flag = true;
							if (op.pt == outPt2.pt)
							{
								this.AddJoin(op, outPt2);
								break;
							}
							if (outPt.pt == op2.pt)
							{
								this.AddJoin(outPt, op2);
								break;
							}
							if (op.pt == op2.pt)
							{
								this.AddJoin(op, op2);
								break;
							}
							if (outPt.pt == outPt2.pt)
							{
								this.AddJoin(outPt, outPt2);
								break;
							}
							if (ClipperBase.ValueBetween(op.pt.X, op2.pt.X, outPt2.pt.X))
							{
								this.AddJoin(op, ClipperBase.InsertOp(op.pt, op2));
								break;
							}
							if (ClipperBase.ValueBetween(outPt.pt.X, op2.pt.X, outPt2.pt.X))
							{
								this.AddJoin(outPt, ClipperBase.InsertOp(outPt.pt, op2));
								break;
							}
							if (ClipperBase.ValueBetween(op2.pt.X, op.pt.X, outPt.pt.X))
							{
								this.AddJoin(op2, ClipperBase.InsertOp(op2.pt, op));
								break;
							}
							if (ClipperBase.ValueBetween(outPt2.pt.X, op.pt.X, outPt.pt.X))
							{
								this.AddJoin(outPt2, ClipperBase.InsertOp(outPt2.pt, op));
								break;
							}
							break;
						}
						else
						{
							joiner = joiner.nextH;
						}
					}
					if (!flag)
					{
						this.CleanCollinear(op.outrec);
					}
				}
			}
		}

		private void AddJoin(OutPt op1, OutPt op2)
		{
			if (op1.outrec == op2.outrec && (op1 == op2 || (op1.next == op2 && op1 != op1.outrec.pts) || (op2.next == op1 && op2 != op1.outrec.pts)))
			{
				return;
			}
			new Joiner(this._joinerList, op1, op2, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Joiner FindJoinParent(Joiner joiner, OutPt op)
		{
			Joiner joiner2 = op.joiner;
			for (;;)
			{
				if (op == joiner2.op1)
				{
					if (joiner2.next1 == joiner)
					{
						break;
					}
					joiner2 = joiner2.next1;
				}
				else
				{
					if (joiner2.next2 == joiner)
					{
						return joiner2;
					}
					joiner2 = joiner2.next2;
				}
			}
			return joiner2;
		}

		private void DeleteJoin(Joiner joiner)
		{
			OutPt op = joiner.op1;
			OutPt op2 = joiner.op2;
			if (op.joiner != joiner)
			{
				Joiner joiner2 = ClipperBase.FindJoinParent(joiner, op);
				if (joiner2.op1 == op)
				{
					joiner2.next1 = joiner.next1;
				}
				else
				{
					joiner2.next2 = joiner.next1;
				}
			}
			else
			{
				op.joiner = joiner.next1;
			}
			if (op2.joiner != joiner)
			{
				Joiner joiner2 = ClipperBase.FindJoinParent(joiner, op2);
				if (joiner2.op1 == op2)
				{
					joiner2.next1 = joiner.next2;
				}
				else
				{
					joiner2.next2 = joiner.next2;
				}
			}
			else
			{
				op2.joiner = joiner.next2;
			}
			this._joinerList[joiner.idx] = null;
		}

		private void ProcessJoinList()
		{
			for (int i = 0; i < this._joinerList.Count; i++)
			{
				Joiner joiner = this._joinerList[i];
				if (joiner != null)
				{
					OutRec outrec = this.ProcessJoin(joiner);
					this.CleanCollinear(outrec);
				}
			}
			this._joinerList.Clear();
		}

		private static bool CheckDisposeAdjacent(ref OutPt op, OutPt guard, OutRec outRec)
		{
			bool result = false;
			while (op.prev != op)
			{
				if (!(op.pt == op.prev.pt) || op == guard || op.prev.joiner == null || op.joiner != null)
				{
					IL_DB:
					while (op.next != op && op.pt == op.next.pt && op != guard && op.next.joiner != null && op.joiner == null)
					{
						if (op == outRec.pts)
						{
							outRec.pts = op.prev;
						}
						op = ClipperBase.DisposeOutPt(op);
						op = op.prev;
					}
					return result;
				}
				if (op == outRec.pts)
				{
					outRec.pts = op.prev;
				}
				op = ClipperBase.DisposeOutPt(op);
				op = op.prev;
			}
			goto IL_DB;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static double DistanceFromLineSqrd(Point64 pt, Point64 linePt1, Point64 linePt2)
		{
			double num = (double)(linePt1.Y - linePt2.Y);
			double num2 = (double)(linePt2.X - linePt1.X);
			double num3 = num * (double)linePt1.X + num2 * (double)linePt1.Y;
			double num4 = num * (double)pt.X + num2 * (double)pt.Y - num3;
			return num4 * num4 / (num * num + num2 * num2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static double DistanceSqr(Point64 pt1, Point64 pt2)
		{
			return (double)(pt1.X - pt2.X) * (double)(pt1.X - pt2.X) + (double)(pt1.Y - pt2.Y) * (double)(pt1.Y - pt2.Y);
		}

		private OutRec ProcessJoin(Joiner j)
		{
			OutPt op = j.op1;
			OutPt op2 = j.op2;
			OutRec realOutRec = ClipperBase.GetRealOutRec(op.outrec);
			OutRec realOutRec2 = ClipperBase.GetRealOutRec(op2.outrec);
			this.DeleteJoin(j);
			if (realOutRec2.pts == null)
			{
				return realOutRec;
			}
			if (!ClipperBase.IsValidClosedPath(op2))
			{
				this.SafeDisposeOutPts(ref op2);
				return realOutRec;
			}
			if (realOutRec.pts == null || !ClipperBase.IsValidClosedPath(op))
			{
				this.SafeDisposeOutPts(ref op);
				return realOutRec2;
			}
			if (realOutRec == realOutRec2 && (op == op2 || op.next == op2 || op.prev == op2))
			{
				return realOutRec;
			}
			ClipperBase.CheckDisposeAdjacent(ref op, op2, realOutRec);
			ClipperBase.CheckDisposeAdjacent(ref op2, op, realOutRec2);
			if (op.next == op2 || op2.next == op)
			{
				return realOutRec;
			}
			OutRec result = realOutRec;
			while (ClipperBase.IsValidPath(op) && ClipperBase.IsValidPath(op2) && (realOutRec != realOutRec2 || (op.prev != op2 && op.next != op2)))
			{
				if (op.prev.pt == op2.next.pt || (InternalClipper.CrossProduct(op.prev.pt, op.pt, op2.next.pt) == 0.0 && ClipperBase.CollinearSegsOverlap(op.prev.pt, op.pt, op2.pt, op2.next.pt)))
				{
					if (realOutRec == realOutRec2)
					{
						if (op.prev.pt != op2.next.pt)
						{
							if (ClipperBase.PointBetween(op.prev.pt, op2.pt, op2.next.pt))
							{
								op2.next = ClipperBase.InsertOp(op.prev.pt, op2);
							}
							else
							{
								op.prev = ClipperBase.InsertOp(op2.next.pt, op.prev);
							}
						}
						OutPt prev = op.prev;
						OutPt next = op2.next;
						prev.next = next;
						next.prev = prev;
						op.prev = op2;
						op2.next = op;
						this.CompleteSplit(op, prev, realOutRec);
					}
					else
					{
						OutPt prev2 = op.prev;
						OutPt next2 = op2.next;
						prev2.next = next2;
						next2.prev = prev2;
						op.prev = op2;
						op2.next = op;
						if (realOutRec.idx < realOutRec2.idx)
						{
							realOutRec.pts = op;
							realOutRec2.pts = null;
							if (realOutRec.owner != null && (realOutRec2.owner == null || realOutRec2.owner.idx < realOutRec.owner.idx))
							{
								realOutRec.owner = realOutRec2.owner;
							}
							realOutRec2.owner = realOutRec;
						}
						else
						{
							result = realOutRec2;
							realOutRec2.pts = op;
							realOutRec.pts = null;
							if (realOutRec2.owner != null && (realOutRec.owner == null || realOutRec.owner.idx < realOutRec2.owner.idx))
							{
								realOutRec2.owner = realOutRec.owner;
							}
							realOutRec.owner = realOutRec2;
						}
					}
				}
				else if (op.next.pt == op2.prev.pt || (InternalClipper.CrossProduct(op.next.pt, op2.pt, op2.prev.pt) == 0.0 && ClipperBase.CollinearSegsOverlap(op.next.pt, op.pt, op2.pt, op2.prev.pt)))
				{
					if (realOutRec == realOutRec2)
					{
						if (op2.prev.pt != op.next.pt)
						{
							if (ClipperBase.PointBetween(op2.prev.pt, op.pt, op.next.pt))
							{
								op.next = ClipperBase.InsertOp(op2.prev.pt, op);
							}
							else
							{
								op2.prev = ClipperBase.InsertOp(op.next.pt, op2.prev);
							}
						}
						OutPt prev3 = op2.prev;
						OutPt next3 = op.next;
						prev3.next = next3;
						next3.prev = prev3;
						op2.prev = op;
						op.next = op2;
						this.CompleteSplit(op, prev3, realOutRec);
					}
					else
					{
						OutPt next4 = op.next;
						OutPt prev4 = op2.prev;
						next4.prev = prev4;
						prev4.next = next4;
						op.next = op2;
						op2.prev = op;
						if (realOutRec.idx < realOutRec2.idx)
						{
							realOutRec.pts = op;
							realOutRec2.pts = null;
							if (realOutRec.owner != null && (realOutRec2.owner == null || realOutRec2.owner.idx < realOutRec.owner.idx))
							{
								realOutRec.owner = realOutRec2.owner;
							}
							realOutRec2.owner = realOutRec;
						}
						else
						{
							result = realOutRec2;
							realOutRec2.pts = op;
							realOutRec.pts = null;
							if (realOutRec2.owner != null && (realOutRec.owner == null || realOutRec.owner.idx < realOutRec2.owner.idx))
							{
								realOutRec2.owner = realOutRec.owner;
							}
							realOutRec.owner = realOutRec2;
						}
					}
				}
				else
				{
					if (ClipperBase.PointBetween(op.next.pt, op2.pt, op2.prev.pt) && ClipperBase.DistanceFromLineSqrd(op.next.pt, op2.pt, op2.prev.pt) < 2.01)
					{
						ClipperBase.InsertOp(op.next.pt, op2.prev);
						continue;
					}
					if (ClipperBase.PointBetween(op2.next.pt, op.pt, op.prev.pt) && ClipperBase.DistanceFromLineSqrd(op2.next.pt, op.pt, op.prev.pt) < 2.01)
					{
						ClipperBase.InsertOp(op2.next.pt, op.prev);
						continue;
					}
					if (ClipperBase.PointBetween(op.prev.pt, op2.pt, op2.next.pt) && ClipperBase.DistanceFromLineSqrd(op.prev.pt, op2.pt, op2.next.pt) < 2.01)
					{
						ClipperBase.InsertOp(op.prev.pt, op2);
						continue;
					}
					if (ClipperBase.PointBetween(op2.prev.pt, op.pt, op.next.pt) && ClipperBase.DistanceFromLineSqrd(op2.prev.pt, op.pt, op.next.pt) < 2.01)
					{
						ClipperBase.InsertOp(op2.prev.pt, op);
						continue;
					}
					if (ClipperBase.CheckDisposeAdjacent(ref op, op2, realOutRec) || ClipperBase.CheckDisposeAdjacent(ref op2, op, realOutRec))
					{
						continue;
					}
					if (op.prev.pt != op2.next.pt && ClipperBase.DistanceSqr(op.prev.pt, op2.next.pt) < 2.01)
					{
						op.prev.pt = op2.next.pt;
						continue;
					}
					if (op.next.pt != op2.prev.pt && ClipperBase.DistanceSqr(op.next.pt, op2.prev.pt) < 2.01)
					{
						op2.prev.pt = op.next.pt;
						continue;
					}
					realOutRec.pts = op;
					if (realOutRec2 != realOutRec)
					{
						realOutRec2.pts = op2;
						this.CleanCollinear(realOutRec2);
					}
				}
				return result;
			}
			return realOutRec;
		}

		private static void UpdateOutrecOwner(OutRec outrec)
		{
			OutPt outPt = outrec.pts;
			do
			{
				outPt.outrec = outrec;
				outPt = outPt.next;
			}
			while (outPt != outrec.pts);
		}

		[NullableContext(2)]
		private void CompleteSplit(OutPt op1, OutPt op2, [Nullable(1)] OutRec outrec)
		{
			double num = ClipperBase.Area(op1);
			double num2 = ClipperBase.Area(op2);
			bool flag = num > 0.0 == num2 < 0.0;
			if (num == 0.0 || (flag && Math.Abs(num) < 2.0))
			{
				this.SafeDisposeOutPts(ref op1);
				outrec.pts = op2;
				return;
			}
			if (num2 == 0.0 || (flag && Math.Abs(num2) < 2.0))
			{
				this.SafeDisposeOutPts(ref op2);
				outrec.pts = op1;
				return;
			}
			OutRec outRec = new OutRec
			{
				idx = this._outrecList.Count
			};
			this._outrecList.Add(outRec);
			outRec.polypath = null;
			if (this._using_polytree)
			{
				if (outrec.splits == null)
				{
					outrec.splits = new List<OutRec>();
				}
				outrec.splits.Add(outRec);
			}
			if (Math.Abs(num) >= Math.Abs(num2))
			{
				outrec.pts = op1;
				outRec.pts = op2;
			}
			else
			{
				outrec.pts = op2;
				outRec.pts = op1;
			}
			if (num > 0.0 == num2 > 0.0)
			{
				outRec.owner = outrec.owner;
			}
			else
			{
				outRec.owner = outrec;
			}
			ClipperBase.UpdateOutrecOwner(outRec);
			this.CleanCollinear(outRec);
		}

		[NullableContext(2)]
		private void CleanCollinear(OutRec outrec)
		{
			outrec = ClipperBase.GetRealOutRec(outrec);
			if (outrec == null || outrec.isOpen || outrec.frontEdge != null || !this.ValidateClosedPathEx(ref outrec.pts))
			{
				return;
			}
			OutPt outPt = outrec.pts;
			OutPt outPt2 = outPt;
			while (outPt2.joiner == null)
			{
				if (InternalClipper.CrossProduct(outPt2.prev.pt, outPt2.pt, outPt2.next.pt) == 0.0 && (outPt2.pt == outPt2.prev.pt || outPt2.pt == outPt2.next.pt || !this.PreserveCollinear || InternalClipper.DotProduct(outPt2.prev.pt, outPt2.pt, outPt2.next.pt) < 0.0))
				{
					if (outPt2 == outrec.pts)
					{
						outrec.pts = outPt2.prev;
					}
					outPt2 = ClipperBase.DisposeOutPt(outPt2);
					if (!this.ValidateClosedPathEx(ref outPt2))
					{
						outrec.pts = null;
						return;
					}
					outPt = outPt2;
				}
				else
				{
					outPt2 = outPt2.next;
					if (outPt2 == outPt)
					{
						this.FixSelfIntersects(ref outrec.pts);
						return;
					}
				}
			}
		}

		private OutPt DoSplitOp(ref OutPt outRecOp, OutPt splitOp)
		{
			OutPt prev = splitOp.prev;
			OutPt next = splitOp.next.next;
			OutPt result = prev;
			PointD pt;
			InternalClipper.GetIntersectPoint(prev.pt, splitOp.pt, splitOp.next.pt, next.pt, out pt);
			Point64 point = new Point64(pt);
			double num = ClipperBase.Area(outRecOp);
			double num2 = ClipperBase.AreaTriangle(point, splitOp.pt, splitOp.next.pt);
			if (point == prev.pt || point == next.pt)
			{
				next.prev = prev;
				prev.next = next;
			}
			else
			{
				OutPt outPt = new OutPt(point, prev.outrec)
				{
					prev = prev,
					next = next
				};
				next.prev = outPt;
				prev.next = outPt;
			}
			this.SafeDeleteOutPtJoiners(splitOp.next);
			this.SafeDeleteOutPtJoiners(splitOp);
			if (Math.Abs(num2) >= 1.0 && (Math.Abs(num2) > Math.Abs(num) || num2 > 0.0 == num > 0.0))
			{
				OutRec outRec = new OutRec
				{
					idx = this._outrecList.Count
				};
				this._outrecList.Add(outRec);
				outRec.owner = prev.outrec.owner;
				outRec.polypath = null;
				splitOp.outrec = outRec;
				splitOp.next.outrec = outRec;
				OutPt outPt2 = new OutPt(point, outRec)
				{
					prev = splitOp.next,
					next = splitOp
				};
				outRec.pts = outPt2;
				splitOp.prev = outPt2;
				splitOp.next.next = outPt2;
			}
			return result;
		}

		private void FixSelfIntersects(ref OutPt op)
		{
			if (!ClipperBase.IsValidClosedPath(op))
			{
				return;
			}
			OutPt outPt = op;
			while (outPt.prev != outPt.next.next)
			{
				if (InternalClipper.SegmentsIntersect(outPt.prev.pt, outPt.pt, outPt.next.pt, outPt.next.next.pt))
				{
					if (outPt == op || outPt.next == op)
					{
						op = outPt.prev;
					}
					outPt = this.DoSplitOp(ref op, outPt);
					op = outPt;
				}
				else
				{
					outPt = outPt.next;
					if (outPt == op)
					{
						break;
					}
				}
			}
		}

		internal bool BuildPath(OutPt op, bool reverse, bool isOpen, List<Point64> path)
		{
			if (op.next == op || (!isOpen && op.next == op.prev))
			{
				return false;
			}
			path.Clear();
			Point64 pt;
			OutPt outPt;
			if (reverse)
			{
				pt = op.pt;
				outPt = op.prev;
			}
			else
			{
				op = op.next;
				pt = op.pt;
				outPt = op.next;
			}
			path.Add(pt);
			while (outPt != op)
			{
				if (outPt.pt != pt)
				{
					pt = outPt.pt;
					path.Add(pt);
				}
				if (reverse)
				{
					outPt = outPt.prev;
				}
				else
				{
					outPt = outPt.next;
				}
			}
			return true;
		}

		protected bool BuildPaths(List<List<Point64>> solutionClosed, List<List<Point64>> solutionOpen)
		{
			solutionClosed.Clear();
			solutionOpen.Clear();
			solutionClosed.Capacity = this._outrecList.Count;
			solutionOpen.Capacity = this._outrecList.Count;
			foreach (OutRec outRec in this._outrecList)
			{
				if (outRec.pts != null)
				{
					List<Point64> list = new List<Point64>();
					if (outRec.isOpen)
					{
						if (this.BuildPath(outRec.pts, this.ReverseSolution, true, list))
						{
							solutionOpen.Add(list);
						}
					}
					else if (this.BuildPath(outRec.pts, this.ReverseSolution, false, list))
					{
						solutionClosed.Add(list);
					}
				}
			}
			return true;
		}

		private bool Path1InsidePath2(OutRec or1, OutRec or2)
		{
			OutPt outPt = or1.pts;
			PointInPolygonResult pointInPolygonResult;
			do
			{
				pointInPolygonResult = InternalClipper.PointInPolygon(outPt.pt, or2.path);
				if (pointInPolygonResult != PointInPolygonResult.IsOn)
				{
					break;
				}
				outPt = outPt.next;
			}
			while (outPt != or1.pts);
			return pointInPolygonResult == PointInPolygonResult.IsInside;
		}

		private Rect64 GetBounds(List<Point64> path)
		{
			if (path.Count == 0)
			{
				return default(Rect64);
			}
			Rect64 rect = new Rect64(long.MaxValue, long.MaxValue, -9223372036854775807L, -9223372036854775807L);
			foreach (Point64 point in path)
			{
				if (point.X < rect.left)
				{
					rect.left = point.X;
				}
				if (point.X > rect.right)
				{
					rect.right = point.X;
				}
				if (point.Y < rect.top)
				{
					rect.top = point.Y;
				}
				if (point.Y > rect.bottom)
				{
					rect.bottom = point.Y;
				}
			}
			return rect;
		}

		private bool DeepCheckOwner(OutRec outrec, OutRec owner)
		{
			if (owner.bounds.IsEmpty())
			{
				owner.bounds = this.GetBounds(owner.path);
			}
			bool flag = owner.bounds.Contains(outrec.bounds);
			if (owner.splits != null)
			{
				foreach (OutRec outRec in owner.splits)
				{
					OutRec realOutRec = ClipperBase.GetRealOutRec(outRec);
					if (realOutRec != null && realOutRec.idx > owner.idx && realOutRec != outrec)
					{
						if (realOutRec.splits != null && this.DeepCheckOwner(outrec, realOutRec))
						{
							return true;
						}
						if (realOutRec.path.Count == 0)
						{
							this.BuildPath(realOutRec.pts, this.ReverseSolution, false, realOutRec.path);
						}
						if (realOutRec.bounds.IsEmpty())
						{
							realOutRec.bounds = this.GetBounds(realOutRec.path);
						}
						if (realOutRec.bounds.Contains(outrec.bounds) && this.Path1InsidePath2(outrec, realOutRec))
						{
							outrec.owner = realOutRec;
							return true;
						}
					}
				}
			}
			if (owner != outrec.owner)
			{
				return false;
			}
			while (!flag || !this.Path1InsidePath2(outrec, outrec.owner))
			{
				outrec.owner = outrec.owner.owner;
				if (outrec.owner == null)
				{
					return false;
				}
				flag = outrec.owner.bounds.Contains(outrec.bounds);
			}
			return true;
		}

		protected bool BuildTree(PolyPathBase polytree, List<List<Point64>> solutionOpen)
		{
			polytree.Clear();
			solutionOpen.Clear();
			solutionOpen.Capacity = this._outrecList.Count;
			for (int i = 0; i < this._outrecList.Count; i++)
			{
				OutRec outRec = this._outrecList[i];
				if (outRec.pts != null)
				{
					if (outRec.isOpen)
					{
						List<Point64> list = new List<Point64>();
						if (this.BuildPath(outRec.pts, this.ReverseSolution, true, list))
						{
							solutionOpen.Add(list);
						}
					}
					else if (this.BuildPath(outRec.pts, this.ReverseSolution, false, outRec.path))
					{
						if (outRec.bounds.IsEmpty())
						{
							outRec.bounds = this.GetBounds(outRec.path);
						}
						outRec.owner = ClipperBase.GetRealOutRec(outRec.owner);
						if (outRec.owner != null)
						{
							this.DeepCheckOwner(outRec, outRec.owner);
						}
						if (outRec.owner != null && outRec.owner.idx > outRec.idx)
						{
							int idx = outRec.owner.idx;
							outRec.owner.idx = i;
							outRec.idx = idx;
							this._outrecList[i] = this._outrecList[idx];
							this._outrecList[idx] = outRec;
							outRec = this._outrecList[i];
							outRec.owner = ClipperBase.GetRealOutRec(outRec.owner);
							this.BuildPath(outRec.pts, this.ReverseSolution, false, outRec.path);
							if (outRec.bounds.IsEmpty())
							{
								outRec.bounds = this.GetBounds(outRec.path);
							}
							if (outRec.owner != null)
							{
								this.DeepCheckOwner(outRec, outRec.owner);
							}
						}
						PolyPathBase polyPathBase;
						if (outRec.owner != null && outRec.owner.polypath != null)
						{
							polyPathBase = outRec.owner.polypath;
						}
						else
						{
							polyPathBase = polytree;
						}
						outRec.polypath = polyPathBase.AddChild(outRec.path);
					}
				}
			}
			return true;
		}

		public Rect64 GetBounds()
		{
			Rect64 maxInvalidRect = Clipper.MaxInvalidRect64;
			foreach (Vertex vertex in this._vertexList)
			{
				Vertex vertex2 = vertex;
				do
				{
					if (vertex2.pt.X < maxInvalidRect.left)
					{
						maxInvalidRect.left = vertex2.pt.X;
					}
					if (vertex2.pt.X > maxInvalidRect.right)
					{
						maxInvalidRect.right = vertex2.pt.X;
					}
					if (vertex2.pt.Y < maxInvalidRect.top)
					{
						maxInvalidRect.top = vertex2.pt.Y;
					}
					if (vertex2.pt.Y > maxInvalidRect.bottom)
					{
						maxInvalidRect.bottom = vertex2.pt.Y;
					}
					vertex2 = vertex2.next;
				}
				while (vertex2 != vertex);
			}
			if (maxInvalidRect.IsEmpty())
			{
				return new Rect64(0L, 0L, 0L, 0L);
			}
			return maxInvalidRect;
		}

		private ClipType _cliptype;

		private FillRule _fillrule;

		[Nullable(2)]
		private Active _actives;

		[Nullable(2)]
		private Active _sel;

		[Nullable(2)]
		private Joiner _horzJoiners;

		private readonly List<LocalMinima> _minimaList;

		private readonly List<IntersectNode> _intersectList;

		private readonly List<Vertex> _vertexList;

		private readonly List<OutRec> _outrecList;

		[Nullable(new byte[]
		{
			1,
			2
		})]
		private readonly List<Joiner> _joinerList;

		private readonly List<long> _scanlineList;

		private int _currentLocMin;

		private long _currentBotY;

		private bool _isSortedMinimaList;

		private bool _hasOpenPaths;

		internal bool _using_polytree;

		internal bool _succeeded;

		[NullableContext(0)]
		private struct IntersectListSort : IComparer<IntersectNode>
		{
			public int Compare(IntersectNode a, IntersectNode b)
			{
				if (a.pt.Y == b.pt.Y)
				{
					if (a.pt.X >= b.pt.X)
					{
						return 1;
					}
					return -1;
				}
				else
				{
					if (a.pt.Y <= b.pt.Y)
					{
						return 1;
					}
					return -1;
				}
			}
		}
	}
}
