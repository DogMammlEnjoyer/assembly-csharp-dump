using System;
using System.Collections.Generic;

namespace g3
{
	public class PointAABBTree3
	{
		public PointAABBTree3(IPointSet pointsIn, bool autoBuild = true)
		{
			this.points = pointsIn;
			if (autoBuild)
			{
				this.Build(PointAABBTree3.BuildStrategy.TopDownMidpoint);
			}
		}

		public IPointSet Points
		{
			get
			{
				return this.points;
			}
		}

		public void Build(PointAABBTree3.BuildStrategy eStrategy = PointAABBTree3.BuildStrategy.TopDownMidpoint)
		{
			if (eStrategy == PointAABBTree3.BuildStrategy.TopDownMedian)
			{
				this.build_top_down(true);
			}
			else if (eStrategy == PointAABBTree3.BuildStrategy.TopDownMidpoint)
			{
				this.build_top_down(false);
			}
			else if (eStrategy == PointAABBTree3.BuildStrategy.Default)
			{
				this.build_top_down(false);
			}
			this.points_timestamp = this.points.Timestamp;
		}

		public virtual int FindNearestPoint(Vector3d p, double fMaxDist = 1.7976931348623157E+308)
		{
			if (this.points_timestamp != this.points.Timestamp)
			{
				throw new Exception("PointAABBTree3.FindNearestPoint: mesh has been modified since tree construction");
			}
			double num = (fMaxDist < double.MaxValue) ? (fMaxDist * fMaxDist) : double.MaxValue;
			int result = -1;
			this.find_nearest_point(this.root_index, p, ref num, ref result);
			return result;
		}

		protected void find_nearest_point(int iBox, Vector3d p, ref double fNearestSqr, ref int tID)
		{
			int num = this.box_to_index[iBox];
			if (num < this.points_end)
			{
				int num2 = this.index_list[num];
				for (int i = 1; i <= num2; i++)
				{
					int num3 = this.index_list[num + i];
					if (this.PointFilterF == null || this.PointFilterF(num3))
					{
						double num4 = this.points.GetVertex(num3).DistanceSquared(p);
						if (num4 < fNearestSqr)
						{
							fNearestSqr = num4;
							tID = num3;
						}
					}
				}
				return;
			}
			int num5 = this.index_list[num];
			if (num5 < 0)
			{
				num5 = -num5 - 1;
				if (this.box_distance_sqr(num5, ref p) <= fNearestSqr)
				{
					this.find_nearest_point(num5, p, ref fNearestSqr, ref tID);
					return;
				}
			}
			else
			{
				num5--;
				int iBox2 = this.index_list[num + 1] - 1;
				double num6 = this.box_distance_sqr(num5, ref p);
				double num7 = this.box_distance_sqr(iBox2, ref p);
				if (num6 < num7)
				{
					if (num6 < fNearestSqr)
					{
						this.find_nearest_point(num5, p, ref fNearestSqr, ref tID);
						if (num7 < fNearestSqr)
						{
							this.find_nearest_point(iBox2, p, ref fNearestSqr, ref tID);
							return;
						}
					}
				}
				else if (num7 < fNearestSqr)
				{
					this.find_nearest_point(iBox2, p, ref fNearestSqr, ref tID);
					if (num6 < fNearestSqr)
					{
						this.find_nearest_point(num5, p, ref fNearestSqr, ref tID);
					}
				}
			}
		}

		public virtual void DoTraversal(PointAABBTree3.TreeTraversal traversal)
		{
			if (this.points_timestamp != this.points.Timestamp)
			{
				throw new Exception("PointAABBTree3.FindNearestPoint: mesh has been modified since tree construction");
			}
			this.tree_traversal(this.root_index, 0, traversal);
		}

		protected virtual void tree_traversal(int iBox, int depth, PointAABBTree3.TreeTraversal traversal)
		{
			int num = this.box_to_index[iBox];
			if (num < this.points_end)
			{
				int num2 = this.index_list[num];
				for (int i = 1; i <= num2; i++)
				{
					int num3 = this.index_list[num + i];
					if (this.PointFilterF == null || this.PointFilterF(num3))
					{
						traversal.NextPointF(num3);
					}
				}
				return;
			}
			int num4 = this.index_list[num];
			if (num4 < 0)
			{
				num4 = -num4 - 1;
				if (traversal.NextBoxF(this.get_box(num4), depth + 1))
				{
					this.tree_traversal(num4, depth + 1, traversal);
					return;
				}
			}
			else
			{
				num4--;
				if (traversal.NextBoxF(this.get_box(num4), depth + 1))
				{
					this.tree_traversal(num4, depth + 1, traversal);
				}
				int iBox2 = this.index_list[num + 1] - 1;
				if (traversal.NextBoxF(this.get_box(iBox2), depth + 1))
				{
					this.tree_traversal(iBox2, depth + 1, traversal);
				}
			}
		}

		public virtual double FastWindingNumber(Vector3d p)
		{
			if (this.points_timestamp != this.points.Timestamp)
			{
				throw new Exception("PointAABBTree3.FindNearestPoint: mesh has been modified since tree construction");
			}
			if (this.FastWindingCache == null || this.fast_winding_cache_timestamp != this.points.Timestamp)
			{
				this.build_fast_winding_cache();
				this.fast_winding_cache_timestamp = this.points.Timestamp;
			}
			return this.branch_fast_winding_num(this.root_index, p);
		}

		protected double branch_fast_winding_num(int iBox, Vector3d p)
		{
			double num = 0.0;
			int num2 = this.box_to_index[iBox];
			if (num2 < this.points_end)
			{
				int num3 = this.index_list[num2];
				for (int i = 1; i <= num3; i++)
				{
					int num4 = this.index_list[num2 + i];
					Vector3d vertex = this.Points.GetVertex(num4);
					Vector3d vector3d = this.Points.GetVertexNormal(num4);
					double xA = this.FastWindingAreaCache[num4];
					num += FastPointWinding.ExactEval(ref vertex, ref vector3d, xA, ref p);
				}
			}
			else
			{
				int num5 = this.index_list[num2];
				if (num5 < 0)
				{
					num5 = -num5 - 1;
					if (!this.box_contains(num5, p) && this.can_use_fast_winding_cache(num5, ref p))
					{
						num += this.evaluate_box_fast_winding_cache(num5, ref p);
					}
					else
					{
						num += this.branch_fast_winding_num(num5, p);
					}
				}
				else
				{
					num5--;
					int iBox2 = this.index_list[num2 + 1] - 1;
					if (!this.box_contains(num5, p) && this.can_use_fast_winding_cache(num5, ref p))
					{
						num += this.evaluate_box_fast_winding_cache(num5, ref p);
					}
					else
					{
						num += this.branch_fast_winding_num(num5, p);
					}
					if (!this.box_contains(iBox2, p) && this.can_use_fast_winding_cache(iBox2, ref p))
					{
						num += this.evaluate_box_fast_winding_cache(iBox2, ref p);
					}
					else
					{
						num += this.branch_fast_winding_num(iBox2, p);
					}
				}
			}
			return num;
		}

		protected void build_fast_winding_cache()
		{
			int pt_count_thresh = 1;
			this.FastWindingAreaCache = new double[this.Points.MaxVertexID];
			foreach (int num in this.Points.VertexIndices())
			{
				this.FastWindingAreaCache[num] = this.FWNAreaEstimateF(num);
			}
			this.FastWindingCache = new Dictionary<int, PointAABBTree3.FWNInfo>();
			HashSet<int> hashSet;
			this.build_fast_winding_cache(this.root_index, 0, pt_count_thresh, out hashSet);
		}

		protected int build_fast_winding_cache(int iBox, int depth, int pt_count_thresh, out HashSet<int> pts_hash)
		{
			pts_hash = null;
			int num = this.box_to_index[iBox];
			if (num < this.points_end)
			{
				return this.index_list[num];
			}
			int num2 = this.index_list[num];
			if (num2 < 0)
			{
				num2 = -num2 - 1;
				return this.build_fast_winding_cache(num2, depth + 1, pt_count_thresh, out pts_hash);
			}
			num2--;
			int iBox2 = this.index_list[num + 1] - 1;
			int num3 = this.build_fast_winding_cache(num2, depth + 1, pt_count_thresh, out pts_hash);
			HashSet<int> hashSet;
			int num4 = this.build_fast_winding_cache(iBox2, depth + 1, pt_count_thresh, out hashSet);
			bool flag = num3 + num4 > pt_count_thresh;
			if (depth == 0)
			{
				return num3 + num4;
			}
			if (pts_hash != null || hashSet != null || flag)
			{
				if (pts_hash == null && hashSet != null)
				{
					this.collect_points(num2, hashSet);
					pts_hash = hashSet;
				}
				else
				{
					if (pts_hash == null)
					{
						pts_hash = new HashSet<int>();
						this.collect_points(num2, pts_hash);
					}
					if (hashSet == null)
					{
						this.collect_points(iBox2, pts_hash);
					}
					else
					{
						pts_hash.UnionWith(hashSet);
					}
				}
			}
			if (flag)
			{
				this.make_box_fast_winding_cache(iBox, pts_hash);
			}
			return num3 + num4;
		}

		protected bool can_use_fast_winding_cache(int iBox, ref Vector3d q)
		{
			PointAABBTree3.FWNInfo fwninfo;
			return this.FastWindingCache.TryGetValue(iBox, out fwninfo) && fwninfo.Center.Distance(ref q) > this.FWNBeta * fwninfo.R;
		}

		protected void make_box_fast_winding_cache(int iBox, IEnumerable<int> pointIndices)
		{
			PointAABBTree3.FWNInfo value = default(PointAABBTree3.FWNInfo);
			FastPointWinding.ComputeCoeffs(this.points, pointIndices, this.FastWindingAreaCache, ref value.Center, ref value.R, ref value.Order1Vec, ref value.Order2Mat);
			this.FastWindingCache[iBox] = value;
		}

		protected double evaluate_box_fast_winding_cache(int iBox, ref Vector3d q)
		{
			PointAABBTree3.FWNInfo fwninfo = this.FastWindingCache[iBox];
			if (this.FWNApproxOrder == 2)
			{
				return FastPointWinding.EvaluateOrder2Approx(ref fwninfo.Center, ref fwninfo.Order1Vec, ref fwninfo.Order2Mat, ref q);
			}
			return FastPointWinding.EvaluateOrder1Approx(ref fwninfo.Center, ref fwninfo.Order1Vec, ref q);
		}

		protected void collect_points(int iBox, HashSet<int> points)
		{
			int num = this.box_to_index[iBox];
			if (num < this.points_end)
			{
				int num2 = this.index_list[num];
				for (int i = 1; i <= num2; i++)
				{
					points.Add(this.index_list[num + i]);
				}
				return;
			}
			int num3 = this.index_list[num];
			if (num3 < 0)
			{
				this.collect_points(-num3 - 1, points);
				return;
			}
			this.collect_points(num3 - 1, points);
			this.collect_points(this.index_list[num + 1] - 1, points);
		}

		public double TotalVolume()
		{
			double volSum = 0.0;
			PointAABBTree3.TreeTraversal traversal = new PointAABBTree3.TreeTraversal
			{
				NextBoxF = delegate(AxisAlignedBox3d box, int depth)
				{
					volSum += box.Volume;
					return true;
				}
			};
			this.DoTraversal(traversal);
			return volSum;
		}

		public double TotalExtentSum()
		{
			double extSum = 0.0;
			PointAABBTree3.TreeTraversal traversal = new PointAABBTree3.TreeTraversal
			{
				NextBoxF = delegate(AxisAlignedBox3d box, int depth)
				{
					extSum += box.Extents.LengthL1;
					return true;
				}
			};
			this.DoTraversal(traversal);
			return extSum;
		}

		public AxisAlignedBox3d Bounds
		{
			get
			{
				return this.get_box(this.root_index);
			}
		}

		private void build_top_down(bool bSorted)
		{
			int i = 0;
			int vertexCount = this.points.VertexCount;
			int[] array = new int[vertexCount];
			Vector3d[] array2 = new Vector3d[vertexCount];
			foreach (int num in this.points.VertexIndices())
			{
				Vector3d vertex = this.points.GetVertex(num);
				double lengthSquared = vertex.LengthSquared;
				if (!double.IsNaN(lengthSquared) && !double.IsInfinity(lengthSquared))
				{
					array[i] = num;
					array2[i] = vertex;
					i++;
				}
			}
			PointAABBTree3.boxes_set boxes_set = new PointAABBTree3.boxes_set();
			PointAABBTree3.boxes_set boxes_set2 = new PointAABBTree3.boxes_set();
			AxisAlignedBox3d axisAlignedBox3d;
			int num2 = bSorted ? this.split_point_set_sorted(array, array2, 0, vertexCount, 0, this.LeafMaxPointCount, boxes_set, boxes_set2, out axisAlignedBox3d) : this.split_point_set_midpoint(array, array2, 0, vertexCount, 0, this.LeafMaxPointCount, boxes_set, boxes_set2, out axisAlignedBox3d);
			this.box_to_index = boxes_set.box_to_index;
			this.box_centers = boxes_set.box_centers;
			this.box_extents = boxes_set.box_extents;
			this.index_list = boxes_set.index_list;
			this.points_end = boxes_set.iIndicesCur;
			int num3 = this.points_end;
			int iBoxCur = boxes_set.iBoxCur;
			for (i = 0; i < boxes_set2.iBoxCur; i++)
			{
				this.box_centers.insert(boxes_set2.box_centers[i], iBoxCur + i);
				this.box_extents.insert(boxes_set2.box_extents[i], iBoxCur + i);
				this.box_to_index.insert(num3 + boxes_set2.box_to_index[i], iBoxCur + i);
			}
			for (i = 0; i < boxes_set2.iIndicesCur; i++)
			{
				int num4 = boxes_set2.index_list[i];
				if (num4 < 0)
				{
					num4 = -num4 - 1;
				}
				else
				{
					num4 += iBoxCur;
				}
				num4++;
				this.index_list.insert(num4, num3 + i);
			}
			this.root_index = num2 + iBoxCur;
		}

		private int split_point_set_sorted(int[] pt_indices, Vector3d[] positions, int iStart, int iCount, int depth, int minIndexCount, PointAABBTree3.boxes_set leafs, PointAABBTree3.boxes_set nodes, out AxisAlignedBox3d box)
		{
			box = AxisAlignedBox3d.Empty;
			int num;
			int num2;
			if (iCount < minIndexCount)
			{
				num = leafs.iBoxCur;
				leafs.iBoxCur = num + 1;
				num2 = num;
				leafs.box_to_index.insert(leafs.iIndicesCur, num2);
				DVector<int> dvector = leafs.index_list;
				num = leafs.iIndicesCur;
				leafs.iIndicesCur = num + 1;
				dvector.insert(iCount, num);
				for (int i = 0; i < iCount; i++)
				{
					DVector<int> dvector2 = leafs.index_list;
					int value = pt_indices[iStart + i];
					num = leafs.iIndicesCur;
					leafs.iIndicesCur = num + 1;
					dvector2.insert(value, num);
					box.Contain(this.points.GetVertex(pt_indices[iStart + i]));
				}
				leafs.box_centers.insert(box.Center, num2);
				leafs.box_extents.insert(box.Extents, num2);
				return -(num2 + 1);
			}
			PointAABBTree3.AxisComp comparer = new PointAABBTree3.AxisComp
			{
				Axis = depth % 3
			};
			Array.Sort<Vector3d, int>(positions, pt_indices, iStart, iCount, comparer);
			int num3 = iCount / 2;
			int iCount2 = num3;
			int iCount3 = iCount - num3;
			int num4 = this.split_point_set_sorted(pt_indices, positions, iStart, iCount2, depth + 1, minIndexCount, leafs, nodes, out box);
			AxisAlignedBox3d box2;
			int num5 = this.split_point_set_sorted(pt_indices, positions, iStart + num3, iCount3, depth + 1, minIndexCount, leafs, nodes, out box2);
			box.Contain(box2);
			num = nodes.iBoxCur;
			nodes.iBoxCur = num + 1;
			num2 = num;
			nodes.box_to_index.insert(nodes.iIndicesCur, num2);
			DVector<int> dvector3 = nodes.index_list;
			int value2 = num4;
			num = nodes.iIndicesCur;
			nodes.iIndicesCur = num + 1;
			dvector3.insert(value2, num);
			DVector<int> dvector4 = nodes.index_list;
			int value3 = num5;
			num = nodes.iIndicesCur;
			nodes.iIndicesCur = num + 1;
			dvector4.insert(value3, num);
			nodes.box_centers.insert(box.Center, num2);
			nodes.box_extents.insert(box.Extents, num2);
			return num2;
		}

		private int split_point_set_midpoint(int[] pt_indices, Vector3d[] positions, int iStart, int iCount, int depth, int minIndexCount, PointAABBTree3.boxes_set leafs, PointAABBTree3.boxes_set nodes, out AxisAlignedBox3d box)
		{
			box = AxisAlignedBox3d.Empty;
			int num;
			int num2;
			if (iCount < minIndexCount)
			{
				num = leafs.iBoxCur;
				leafs.iBoxCur = num + 1;
				num2 = num;
				leafs.box_to_index.insert(leafs.iIndicesCur, num2);
				DVector<int> dvector = leafs.index_list;
				num = leafs.iIndicesCur;
				leafs.iIndicesCur = num + 1;
				dvector.insert(iCount, num);
				for (int i = 0; i < iCount; i++)
				{
					DVector<int> dvector2 = leafs.index_list;
					int value = pt_indices[iStart + i];
					num = leafs.iIndicesCur;
					leafs.iIndicesCur = num + 1;
					dvector2.insert(value, num);
					box.Contain(this.points.GetVertex(pt_indices[iStart + i]));
				}
				leafs.box_centers.insert(box.Center, num2);
				leafs.box_extents.insert(box.Extents, num2);
				return -(num2 + 1);
			}
			int key = depth % 3;
			Interval1d empty = Interval1d.Empty;
			for (int j = 0; j < iCount; j++)
			{
				empty.Contain(positions[iStart + j][key]);
			}
			double center = empty.Center;
			int num5;
			int iCount2;
			if (Math.Abs(empty.a - empty.b) > 1E-08)
			{
				int k = 0;
				int num3 = iCount - 1;
				while (k < num3)
				{
					while (positions[iStart + k][key] <= center)
					{
						k++;
					}
					while (positions[iStart + num3][key] > center)
					{
						num3--;
					}
					if (k >= num3)
					{
						break;
					}
					Vector3d vector3d = positions[iStart + k];
					positions[iStart + k] = positions[iStart + num3];
					positions[iStart + num3] = vector3d;
					int num4 = pt_indices[iStart + k];
					pt_indices[iStart + k] = pt_indices[iStart + num3];
					pt_indices[iStart + num3] = num4;
				}
				num5 = k;
				iCount2 = iCount - num5;
			}
			else
			{
				num5 = iCount / 2;
				iCount2 = iCount - num5;
			}
			int num6 = this.split_point_set_midpoint(pt_indices, positions, iStart, num5, depth + 1, minIndexCount, leafs, nodes, out box);
			AxisAlignedBox3d box2;
			int num7 = this.split_point_set_midpoint(pt_indices, positions, iStart + num5, iCount2, depth + 1, minIndexCount, leafs, nodes, out box2);
			box.Contain(box2);
			num = nodes.iBoxCur;
			nodes.iBoxCur = num + 1;
			num2 = num;
			nodes.box_to_index.insert(nodes.iIndicesCur, num2);
			DVector<int> dvector3 = nodes.index_list;
			int value2 = num6;
			num = nodes.iIndicesCur;
			nodes.iIndicesCur = num + 1;
			dvector3.insert(value2, num);
			DVector<int> dvector4 = nodes.index_list;
			int value3 = num7;
			num = nodes.iIndicesCur;
			nodes.iIndicesCur = num + 1;
			dvector4.insert(value3, num);
			nodes.box_centers.insert(box.Center, num2);
			nodes.box_extents.insert(box.Extents, num2);
			return num2;
		}

		private AxisAlignedBox3d get_box(int iBox)
		{
			Vector3d vector3d = this.box_centers[iBox];
			Vector3d vector3d2 = this.box_extents[iBox];
			return new AxisAlignedBox3d(ref vector3d, vector3d2.x, vector3d2.y, vector3d2.z);
		}

		private double box_distance_sqr(int iBox, ref Vector3d p)
		{
			Vector3d vector3d = this.box_centers[iBox];
			Vector3d vector3d2 = this.box_extents[iBox];
			double num = Math.Abs(p.x - vector3d.x);
			num = ((num < vector3d2.x) ? 0.0 : (num - vector3d2.x));
			double num2 = Math.Abs(p.y - vector3d.y);
			num2 = ((num2 < vector3d2.y) ? 0.0 : (num2 - vector3d2.y));
			double num3 = Math.Abs(p.z - vector3d.z);
			num3 = ((num3 < vector3d2.z) ? 0.0 : (num3 - vector3d2.z));
			return num * num + num2 * num2 + num3 * num3;
		}

		protected bool box_contains(int iBox, Vector3d p)
		{
			Vector3d vector3d = this.box_centers[iBox];
			Vector3d vector3d2 = this.box_extents[iBox];
			return new AxisAlignedBox3d(ref vector3d, vector3d2.x + 1.1102230246251565E-14, vector3d2.y + 1.1102230246251565E-14, vector3d2.z + 1.1102230246251565E-14).Contains(p);
		}

		public void TestCoverage()
		{
			int[] array = new int[this.points.MaxVertexID];
			Array.Clear(array, 0, array.Length);
			int[] array2 = new int[this.box_to_index.Length];
			Array.Clear(array2, 0, array2.Length);
			this.test_coverage(array, array2, this.root_index);
			foreach (int num in this.points.VertexIndices())
			{
				if (array[num] != 1)
				{
					Util.gBreakToDebugger();
				}
			}
		}

		private void test_coverage(int[] point_counts, int[] parent_indices, int iBox)
		{
			int num = this.box_to_index[iBox];
			this.debug_check_child_points_in_box(iBox);
			if (num < this.points_end)
			{
				int num2 = this.index_list[num];
				AxisAlignedBox3d axisAlignedBox3d = this.get_box(iBox);
				for (int i = 1; i <= num2; i++)
				{
					int num3 = this.index_list[num + i];
					point_counts[num3]++;
					Vector3d vertex = this.points.GetVertex(num3);
					if (!axisAlignedBox3d.Contains(vertex))
					{
						Util.gBreakToDebugger();
					}
				}
				return;
			}
			int num4 = this.index_list[num];
			if (num4 < 0)
			{
				num4 = -num4 - 1;
				parent_indices[num4] = iBox;
				this.test_coverage(point_counts, parent_indices, num4);
				return;
			}
			num4--;
			parent_indices[num4] = iBox;
			this.test_coverage(point_counts, parent_indices, num4);
			int num5 = this.index_list[num + 1];
			num5--;
			parent_indices[num5] = iBox;
			this.test_coverage(point_counts, parent_indices, num5);
		}

		private void debug_check_child_point_distances(int iBox, Vector3d p)
		{
			double fBoxDistSqr = this.box_distance_sqr(iBox, ref p);
			PointAABBTree3.TreeTraversal traversal = new PointAABBTree3.TreeTraversal
			{
				NextPointF = delegate(int vID)
				{
					Vector3d vertex = this.points.GetVertex(vID);
					double num = p.DistanceSquared(vertex);
					if (num < fBoxDistSqr && Math.Abs(num - fBoxDistSqr) > 1E-06)
					{
						Util.gBreakToDebugger();
					}
				}
			};
			this.tree_traversal(iBox, 0, traversal);
		}

		private void debug_check_child_points_in_box(int iBox)
		{
			AxisAlignedBox3d box = this.get_box(iBox);
			PointAABBTree3.TreeTraversal traversal = new PointAABBTree3.TreeTraversal
			{
				NextPointF = delegate(int vID)
				{
					Vector3d vertex = this.points.GetVertex(vID);
					if (!box.Contains(vertex))
					{
						Util.gBreakToDebugger();
					}
				}
			};
			this.tree_traversal(iBox, 0, traversal);
		}

		private IPointSet points;

		private int points_timestamp;

		public Func<int, bool> PointFilterF;

		public int LeafMaxPointCount = 32;

		public double FWNBeta = 2.0;

		public int FWNApproxOrder = 2;

		public Func<int, double> FWNAreaEstimateF = (int vid) => 1.0;

		private Dictionary<int, PointAABBTree3.FWNInfo> FastWindingCache;

		private double[] FastWindingAreaCache;

		private int fast_winding_cache_timestamp = -1;

		private DVector<int> box_to_index;

		private DVector<Vector3d> box_centers;

		private DVector<Vector3d> box_extents;

		private DVector<int> index_list;

		private int points_end = -1;

		private int root_index = -1;

		private const double box_eps = 1.1102230246251565E-14;

		public enum BuildStrategy
		{
			Default,
			TopDownMidpoint,
			TopDownMedian
		}

		public class TreeTraversal
		{
			public Func<AxisAlignedBox3d, int, bool> NextBoxF = (AxisAlignedBox3d box, int depth) => true;

			public Action<int> NextPointF = delegate(int vID)
			{
			};
		}

		private struct FWNInfo
		{
			public Vector3d Center;

			public double R;

			public Vector3d Order1Vec;

			public Matrix3d Order2Mat;
		}

		private class AxisComp : IComparer<Vector3d>
		{
			public int Compare(Vector3d a, Vector3d b)
			{
				return a[this.Axis].CompareTo(b[this.Axis]);
			}

			public int Axis;
		}

		private class boxes_set
		{
			public DVector<int> box_to_index = new DVector<int>();

			public DVector<Vector3d> box_centers = new DVector<Vector3d>();

			public DVector<Vector3d> box_extents = new DVector<Vector3d>();

			public DVector<int> index_list = new DVector<int>();

			public int iBoxCur;

			public int iIndicesCur;
		}
	}
}
