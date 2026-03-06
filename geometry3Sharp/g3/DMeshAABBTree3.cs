using System;
using System.Collections.Generic;

namespace g3
{
	public class DMeshAABBTree3 : ISpatial
	{
		public DMeshAABBTree3(DMesh3 m, bool autoBuild = false)
		{
			this.mesh = m;
			if (autoBuild)
			{
				this.Build(DMeshAABBTree3.BuildStrategy.TopDownMidpoint, DMeshAABBTree3.ClusterPolicy.Default);
			}
		}

		public DMesh3 Mesh
		{
			get
			{
				return this.mesh;
			}
		}

		public void Build(DMeshAABBTree3.BuildStrategy eStrategy = DMeshAABBTree3.BuildStrategy.TopDownMidpoint, DMeshAABBTree3.ClusterPolicy ePolicy = DMeshAABBTree3.ClusterPolicy.Default)
		{
			if (eStrategy == DMeshAABBTree3.BuildStrategy.BottomUpFromOneRings)
			{
				this.build_by_one_rings(ePolicy);
			}
			else if (eStrategy == DMeshAABBTree3.BuildStrategy.TopDownMedian)
			{
				this.build_top_down(true);
			}
			else if (eStrategy == DMeshAABBTree3.BuildStrategy.TopDownMidpoint)
			{
				this.build_top_down(false);
			}
			else if (eStrategy == DMeshAABBTree3.BuildStrategy.Default)
			{
				this.build_top_down(false);
			}
			this.mesh_timestamp = this.mesh.ShapeTimestamp;
		}

		public bool IsValid
		{
			get
			{
				return this.mesh_timestamp == this.mesh.ShapeTimestamp;
			}
		}

		public bool SupportsNearestTriangle
		{
			get
			{
				return true;
			}
		}

		public virtual int FindNearestTriangle(Vector3d p, double fMaxDist = 1.7976931348623157E+308)
		{
			if (this.mesh_timestamp != this.mesh.ShapeTimestamp)
			{
				throw new Exception("DMeshAABBTree3.FindNearestTriangle: mesh has been modified since tree construction");
			}
			double num = (fMaxDist < double.MaxValue) ? (fMaxDist * fMaxDist) : double.MaxValue;
			int result = -1;
			this.find_nearest_tri(this.root_index, p, ref num, ref result);
			return result;
		}

		public virtual int FindNearestTriangle(Vector3d p, out double fNearestDistSqr, double fMaxDist = 1.7976931348623157E+308)
		{
			if (this.mesh_timestamp != this.mesh.ShapeTimestamp)
			{
				throw new Exception("DMeshAABBTree3.FindNearestTriangle: mesh has been modified since tree construction");
			}
			fNearestDistSqr = ((fMaxDist < double.MaxValue) ? (fMaxDist * fMaxDist) : double.MaxValue);
			int result = -1;
			this.find_nearest_tri(this.root_index, p, ref fNearestDistSqr, ref result);
			return result;
		}

		protected void find_nearest_tri(int iBox, Vector3d p, ref double fNearestSqr, ref int tID)
		{
			int num = this.box_to_index[iBox];
			if (num < this.triangles_end)
			{
				int num2 = this.index_list[num];
				for (int i = 1; i <= num2; i++)
				{
					int num3 = this.index_list[num + i];
					if (this.TriangleFilterF == null || this.TriangleFilterF(num3))
					{
						double num4 = MeshQueries.TriDistanceSqr(this.mesh, num3, p);
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
				if (this.box_distance_sqr(num5, p) <= fNearestSqr)
				{
					this.find_nearest_tri(num5, p, ref fNearestSqr, ref tID);
					return;
				}
			}
			else
			{
				num5--;
				int iBox2 = this.index_list[num + 1] - 1;
				double num6 = this.box_distance_sqr(num5, p);
				double num7 = this.box_distance_sqr(iBox2, p);
				if (num6 < num7)
				{
					if (num6 < fNearestSqr)
					{
						this.find_nearest_tri(num5, p, ref fNearestSqr, ref tID);
						if (num7 < fNearestSqr)
						{
							this.find_nearest_tri(iBox2, p, ref fNearestSqr, ref tID);
							return;
						}
					}
				}
				else if (num7 < fNearestSqr)
				{
					this.find_nearest_tri(iBox2, p, ref fNearestSqr, ref tID);
					if (num6 < fNearestSqr)
					{
						this.find_nearest_tri(num5, p, ref fNearestSqr, ref tID);
					}
				}
			}
		}

		public virtual int FindNearestVertex(Vector3d p, double fMaxDist = 1.7976931348623157E+308)
		{
			if (this.mesh_timestamp != this.mesh.ShapeTimestamp)
			{
				throw new Exception("DMeshAABBTree3.FindNearestVertex: mesh has been modified since tree construction");
			}
			double num = (fMaxDist < double.MaxValue) ? (fMaxDist * fMaxDist) : double.MaxValue;
			int result = -1;
			this.find_nearest_vtx(this.root_index, p, ref num, ref result);
			return result;
		}

		protected void find_nearest_vtx(int iBox, Vector3d p, ref double fNearestSqr, ref int vid)
		{
			int num = this.box_to_index[iBox];
			if (num < this.triangles_end)
			{
				int num2 = this.index_list[num];
				for (int i = 1; i <= num2; i++)
				{
					int num3 = this.index_list[num + i];
					if (this.TriangleFilterF == null || this.TriangleFilterF(num3))
					{
						Vector3i vector3i = this.mesh.GetTriangle(num3);
						for (int j = 0; j < 3; j++)
						{
							double num4 = this.mesh.GetVertex(vector3i[j]).DistanceSquared(ref p);
							if (num4 < fNearestSqr)
							{
								fNearestSqr = num4;
								vid = vector3i[j];
							}
						}
					}
				}
				return;
			}
			int num5 = this.index_list[num];
			if (num5 < 0)
			{
				num5 = -num5 - 1;
				if (this.box_distance_sqr(num5, p) <= fNearestSqr)
				{
					this.find_nearest_vtx(num5, p, ref fNearestSqr, ref vid);
					return;
				}
			}
			else
			{
				num5--;
				int iBox2 = this.index_list[num + 1] - 1;
				double num6 = this.box_distance_sqr(num5, p);
				double num7 = this.box_distance_sqr(iBox2, p);
				if (num6 < num7)
				{
					if (num6 < fNearestSqr)
					{
						this.find_nearest_vtx(num5, p, ref fNearestSqr, ref vid);
						if (num7 < fNearestSqr)
						{
							this.find_nearest_vtx(iBox2, p, ref fNearestSqr, ref vid);
							return;
						}
					}
				}
				else if (num7 < fNearestSqr)
				{
					this.find_nearest_vtx(iBox2, p, ref fNearestSqr, ref vid);
					if (num6 < fNearestSqr)
					{
						this.find_nearest_vtx(num5, p, ref fNearestSqr, ref vid);
					}
				}
			}
		}

		public bool SupportsTriangleRayIntersection
		{
			get
			{
				return true;
			}
		}

		public virtual int FindNearestHitTriangle(Ray3d ray, double fMaxDist = 1.7976931348623157E+308)
		{
			if (this.mesh_timestamp != this.mesh.ShapeTimestamp)
			{
				throw new Exception("DMeshAABBTree3.FindNearestHitTriangle: mesh has been modified since tree construction");
			}
			if (!ray.Direction.IsNormalized)
			{
				throw new Exception("DMeshAABBTree3.FindNearestHitTriangle: ray direction is not normalized");
			}
			double num = (fMaxDist < double.MaxValue) ? fMaxDist : 3.4028234663852886E+38;
			int result = -1;
			this.find_hit_triangle(this.root_index, ref ray, ref num, ref result);
			return result;
		}

		protected void find_hit_triangle(int iBox, ref Ray3d ray, ref double fNearestT, ref int tID)
		{
			int num = this.box_to_index[iBox];
			if (num < this.triangles_end)
			{
				Triangle3d triangle3d = default(Triangle3d);
				int num2 = this.index_list[num];
				for (int i = 1; i <= num2; i++)
				{
					int num3 = this.index_list[num + i];
					if (this.TriangleFilterF == null || this.TriangleFilterF(num3))
					{
						this.mesh.GetTriVertices(num3, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
						double num4;
						if (IntrRay3Triangle3.Intersects(ref ray, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2, out num4) && num4 < fNearestT)
						{
							fNearestT = num4;
							tID = num3;
						}
					}
				}
				return;
			}
			double num5 = 9.999999974752427E-07;
			int num6 = this.index_list[num];
			if (num6 < 0)
			{
				num6 = -num6 - 1;
				if (this.box_ray_intersect_t(num6, ray) <= fNearestT + num5)
				{
					this.find_hit_triangle(num6, ref ray, ref fNearestT, ref tID);
					return;
				}
			}
			else
			{
				num6--;
				int iBox2 = this.index_list[num + 1] - 1;
				double num7 = this.box_ray_intersect_t(num6, ray);
				double num8 = this.box_ray_intersect_t(iBox2, ray);
				if (num7 < num8)
				{
					if (num7 <= fNearestT + num5)
					{
						this.find_hit_triangle(num6, ref ray, ref fNearestT, ref tID);
						if (num8 <= fNearestT + num5)
						{
							this.find_hit_triangle(iBox2, ref ray, ref fNearestT, ref tID);
							return;
						}
					}
				}
				else if (num8 <= fNearestT + num5)
				{
					this.find_hit_triangle(iBox2, ref ray, ref fNearestT, ref tID);
					if (num7 <= fNearestT + num5)
					{
						this.find_hit_triangle(num6, ref ray, ref fNearestT, ref tID);
					}
				}
			}
		}

		public virtual int FindAllHitTriangles(Ray3d ray, List<int> hitTriangles = null, double fMaxDist = 1.7976931348623157E+308)
		{
			if (this.mesh_timestamp != this.mesh.ShapeTimestamp)
			{
				throw new Exception("DMeshAABBTree3.FindNearestHitTriangle: mesh has been modified since tree construction");
			}
			if (!ray.Direction.IsNormalized)
			{
				throw new Exception("DMeshAABBTree3.FindNearestHitTriangle: ray direction is not normalized");
			}
			double fMaxDist2 = (fMaxDist < double.MaxValue) ? fMaxDist : 3.4028234663852886E+38;
			return this.find_all_hit_triangles(this.root_index, hitTriangles, ref ray, fMaxDist2);
		}

		protected int find_all_hit_triangles(int iBox, List<int> hitTriangles, ref Ray3d ray, double fMaxDist)
		{
			int num = 0;
			int num2 = this.box_to_index[iBox];
			if (num2 < this.triangles_end)
			{
				Triangle3d triangle3d = default(Triangle3d);
				int num3 = this.index_list[num2];
				for (int i = 1; i <= num3; i++)
				{
					int num4 = this.index_list[num2 + i];
					if (this.TriangleFilterF == null || this.TriangleFilterF(num4))
					{
						this.mesh.GetTriVertices(num4, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
						double num5;
						if (IntrRay3Triangle3.Intersects(ref ray, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2, out num5) && num5 < fMaxDist)
						{
							if (hitTriangles != null)
							{
								hitTriangles.Add(num4);
							}
							num++;
						}
					}
				}
			}
			else
			{
				double num6 = 9.999999974752427E-07;
				int num7 = this.index_list[num2];
				if (num7 < 0)
				{
					num7 = -num7 - 1;
					if (this.box_ray_intersect_t(num7, ray) <= fMaxDist + num6)
					{
						num += this.find_all_hit_triangles(num7, hitTriangles, ref ray, fMaxDist);
					}
				}
				else
				{
					num7--;
					int iBox2 = this.index_list[num2 + 1] - 1;
					if (this.box_ray_intersect_t(num7, ray) <= fMaxDist + num6)
					{
						num += this.find_all_hit_triangles(num7, hitTriangles, ref ray, fMaxDist);
					}
					if (this.box_ray_intersect_t(iBox2, ray) <= fMaxDist + num6)
					{
						num += this.find_all_hit_triangles(iBox2, hitTriangles, ref ray, fMaxDist);
					}
				}
			}
			return num;
		}

		public virtual bool TestIntersection(IMesh testMesh, Func<Vector3d, Vector3d> TransformF = null, bool bBoundsCheck = true)
		{
			if (this.mesh_timestamp != this.mesh.ShapeTimestamp)
			{
				throw new Exception("DMeshAABBTree3.TestIntersection: mesh has been modified since tree construction");
			}
			if (bBoundsCheck)
			{
				AxisAlignedBox3d axisAlignedBox3d = MeshMeasurements.Bounds(testMesh, TransformF);
				if (!this.box_box_intersect(this.root_index, ref axisAlignedBox3d))
				{
					return false;
				}
			}
			if (TransformF == null)
			{
				TransformF = ((Vector3d x) => x);
			}
			Triangle3d triangle = default(Triangle3d);
			foreach (int i in testMesh.TriangleIndices())
			{
				Index3i triangle2 = testMesh.GetTriangle(i);
				triangle.V0 = TransformF(testMesh.GetVertex(triangle2.a));
				triangle.V1 = TransformF(testMesh.GetVertex(triangle2.b));
				triangle.V2 = TransformF(testMesh.GetVertex(triangle2.c));
				if (this.TestIntersection(triangle))
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool TestIntersection(Triangle3d triangle)
		{
			if (this.mesh_timestamp != this.mesh.ShapeTimestamp)
			{
				throw new Exception("DMeshAABBTree3.TestIntersection: mesh has been modified since tree construction");
			}
			AxisAlignedBox3d axisAlignedBox3d = BoundsUtil.Bounds(ref triangle);
			return this.find_any_intersection(this.root_index, ref triangle, ref axisAlignedBox3d) >= 0;
		}

		protected int find_any_intersection(int iBox, ref Triangle3d triangle, ref AxisAlignedBox3d triBounds)
		{
			int num = this.box_to_index[iBox];
			if (num < this.triangles_end)
			{
				Triangle3d triangle3d = default(Triangle3d);
				int num2 = this.index_list[num];
				for (int i = 1; i <= num2; i++)
				{
					int num3 = this.index_list[num + i];
					if (this.TriangleFilterF == null || this.TriangleFilterF(num3))
					{
						this.mesh.GetTriVertices(num3, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
						if (IntrTriangle3Triangle3.Intersects(ref triangle, ref triangle3d))
						{
							return num3;
						}
					}
				}
			}
			else
			{
				int num4 = this.index_list[num];
				if (num4 >= 0)
				{
					num4--;
					int iBox2 = this.index_list[num + 1] - 1;
					int num5 = -1;
					if (this.box_box_intersect(num4, ref triBounds))
					{
						num5 = this.find_any_intersection(num4, ref triangle, ref triBounds);
					}
					if (num5 == -1 && this.box_box_intersect(iBox2, ref triBounds))
					{
						num5 = this.find_any_intersection(iBox2, ref triangle, ref triBounds);
					}
					return num5;
				}
				num4 = -num4 - 1;
				if (this.box_box_intersect(num4, ref triBounds))
				{
					return this.find_any_intersection(num4, ref triangle, ref triBounds);
				}
			}
			return -1;
		}

		public virtual bool TestIntersection(DMeshAABBTree3 otherTree, Func<Vector3d, Vector3d> TransformF = null)
		{
			if (this.mesh_timestamp != this.mesh.ShapeTimestamp)
			{
				throw new Exception("DMeshAABBTree3.TestIntersection: mesh has been modified since tree construction");
			}
			return this.find_any_intersection(this.root_index, otherTree, TransformF, otherTree.root_index, 0);
		}

		protected bool find_any_intersection(int iBox, DMeshAABBTree3 otherTree, Func<Vector3d, Vector3d> TransformF, int oBox, int depth)
		{
			int num = this.box_to_index[iBox];
			int num2 = otherTree.box_to_index[oBox];
			if (num < this.triangles_end && num2 < otherTree.triangles_end)
			{
				Triangle3d triangle3d = default(Triangle3d);
				Triangle3d triangle3d2 = default(Triangle3d);
				int num3 = this.index_list[num];
				int num4 = otherTree.index_list[num2];
				for (int i = 1; i <= num4; i++)
				{
					int num5 = otherTree.index_list[num2 + i];
					if (otherTree.TriangleFilterF == null || otherTree.TriangleFilterF(num5))
					{
						otherTree.mesh.GetTriVertices(num5, ref triangle3d2.V0, ref triangle3d2.V1, ref triangle3d2.V2);
						if (TransformF != null)
						{
							triangle3d2.V0 = TransformF(triangle3d2.V0);
							triangle3d2.V1 = TransformF(triangle3d2.V1);
							triangle3d2.V2 = TransformF(triangle3d2.V2);
						}
						for (int j = 1; j <= num3; j++)
						{
							int num6 = this.index_list[num + j];
							if (this.TriangleFilterF == null || this.TriangleFilterF(num6))
							{
								this.mesh.GetTriVertices(num6, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
								if (IntrTriangle3Triangle3.Intersects(ref triangle3d2, ref triangle3d))
								{
									return true;
								}
							}
						}
					}
				}
				return false;
			}
			bool flag = num < this.triangles_end || depth % 2 == 0;
			if (flag && num2 < otherTree.triangles_end)
			{
				flag = false;
			}
			if (flag)
			{
				AxisAlignedBox3d box = this.get_boxd(iBox);
				int num7 = otherTree.index_list[num2];
				if (num7 >= 0)
				{
					num7--;
					int num8 = otherTree.index_list[num2 + 1] - 1;
					bool flag2 = false;
					if (otherTree.get_boxd(num7, TransformF).Intersects(box))
					{
						flag2 = this.find_any_intersection(iBox, otherTree, TransformF, num7, depth + 1);
					}
					if (!flag2 && otherTree.get_boxd(num8, TransformF).Intersects(box))
					{
						flag2 = this.find_any_intersection(iBox, otherTree, TransformF, num8, depth + 1);
					}
					return flag2;
				}
				num7 = -num7 - 1;
				if (otherTree.get_boxd(num7, TransformF).Intersects(box))
				{
					return this.find_any_intersection(iBox, otherTree, TransformF, oBox, depth + 1);
				}
			}
			else
			{
				AxisAlignedBox3d axisAlignedBox3d = otherTree.get_boxd(oBox, TransformF);
				int num9 = this.index_list[num];
				if (num9 >= 0)
				{
					num9--;
					int iBox2 = this.index_list[num + 1] - 1;
					bool flag3 = false;
					if (this.box_box_intersect(num9, ref axisAlignedBox3d))
					{
						flag3 = this.find_any_intersection(num9, otherTree, TransformF, oBox, depth + 1);
					}
					if (!flag3 && this.box_box_intersect(iBox2, ref axisAlignedBox3d))
					{
						flag3 = this.find_any_intersection(iBox2, otherTree, TransformF, oBox, depth + 1);
					}
					return flag3;
				}
				num9 = -num9 - 1;
				if (this.box_box_intersect(num9, ref axisAlignedBox3d))
				{
					return this.find_any_intersection(num9, otherTree, TransformF, oBox, depth + 1);
				}
			}
			return false;
		}

		public virtual DMeshAABBTree3.IntersectionsQueryResult FindAllIntersections(DMeshAABBTree3 otherTree, Func<Vector3d, Vector3d> TransformF = null)
		{
			if (this.mesh_timestamp != this.mesh.ShapeTimestamp)
			{
				throw new Exception("DMeshAABBTree3.FindIntersections: mesh has been modified since tree construction");
			}
			DMeshAABBTree3.IntersectionsQueryResult intersectionsQueryResult = new DMeshAABBTree3.IntersectionsQueryResult();
			intersectionsQueryResult.Points = new List<DMeshAABBTree3.PointIntersection>();
			intersectionsQueryResult.Segments = new List<DMeshAABBTree3.SegmentIntersection>();
			IntrTriangle3Triangle3 intr = new IntrTriangle3Triangle3(default(Triangle3d), default(Triangle3d));
			this.find_intersections(this.root_index, otherTree, TransformF, otherTree.root_index, 0, intr, intersectionsQueryResult);
			return intersectionsQueryResult;
		}

		protected void find_intersections(int iBox, DMeshAABBTree3 otherTree, Func<Vector3d, Vector3d> TransformF, int oBox, int depth, IntrTriangle3Triangle3 intr, DMeshAABBTree3.IntersectionsQueryResult result)
		{
			int num = this.box_to_index[iBox];
			int num2 = otherTree.box_to_index[oBox];
			if (num < this.triangles_end && num2 < otherTree.triangles_end)
			{
				Triangle3d triangle = default(Triangle3d);
				Triangle3d triangle3d = default(Triangle3d);
				int num3 = this.index_list[num];
				int num4 = otherTree.index_list[num2];
				for (int i = 1; i <= num4; i++)
				{
					int num5 = otherTree.index_list[num2 + i];
					if (otherTree.TriangleFilterF == null || otherTree.TriangleFilterF(num5))
					{
						otherTree.mesh.GetTriVertices(num5, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
						if (TransformF != null)
						{
							triangle3d.V0 = TransformF(triangle3d.V0);
							triangle3d.V1 = TransformF(triangle3d.V1);
							triangle3d.V2 = TransformF(triangle3d.V2);
						}
						intr.Triangle0 = triangle3d;
						for (int j = 1; j <= num3; j++)
						{
							int num6 = this.index_list[num + j];
							if (this.TriangleFilterF == null || this.TriangleFilterF(num6))
							{
								this.mesh.GetTriVertices(num6, ref triangle.V0, ref triangle.V1, ref triangle.V2);
								intr.Triangle1 = triangle;
								if (intr.Test() && intr.Find())
								{
									if (intr.Quantity == 1)
									{
										result.Points.Add(new DMeshAABBTree3.PointIntersection
										{
											t0 = num6,
											t1 = num5,
											point = intr.Points[0]
										});
									}
									else
									{
										if (intr.Quantity != 2)
										{
											throw new Exception("DMeshAABBTree.find_intersections: found quantity " + intr.Quantity.ToString());
										}
										result.Segments.Add(new DMeshAABBTree3.SegmentIntersection
										{
											t0 = num6,
											t1 = num5,
											point0 = intr.Points[0],
											point1 = intr.Points[1]
										});
									}
								}
							}
						}
					}
				}
				return;
			}
			bool flag = num < this.triangles_end || depth % 2 == 0;
			if (flag && num2 < otherTree.triangles_end)
			{
				flag = false;
			}
			if (flag)
			{
				AxisAlignedBox3d box = this.get_boxd(iBox);
				int num7 = otherTree.index_list[num2];
				if (num7 < 0)
				{
					num7 = -num7 - 1;
					if (otherTree.get_boxd(num7, TransformF).Intersects(box))
					{
						this.find_intersections(iBox, otherTree, TransformF, num7, depth + 1, intr, result);
						return;
					}
				}
				else
				{
					num7--;
					if (otherTree.get_boxd(num7, TransformF).Intersects(box))
					{
						this.find_intersections(iBox, otherTree, TransformF, num7, depth + 1, intr, result);
					}
					int num8 = otherTree.index_list[num2 + 1] - 1;
					if (otherTree.get_boxd(num8, TransformF).Intersects(box))
					{
						this.find_intersections(iBox, otherTree, TransformF, num8, depth + 1, intr, result);
						return;
					}
				}
			}
			else
			{
				AxisAlignedBox3d axisAlignedBox3d = otherTree.get_boxd(oBox, TransformF);
				int num9 = this.index_list[num];
				if (num9 < 0)
				{
					num9 = -num9 - 1;
					if (this.box_box_intersect(num9, ref axisAlignedBox3d))
					{
						this.find_intersections(num9, otherTree, TransformF, oBox, depth + 1, intr, result);
						return;
					}
				}
				else
				{
					num9--;
					if (this.box_box_intersect(num9, ref axisAlignedBox3d))
					{
						this.find_intersections(num9, otherTree, TransformF, oBox, depth + 1, intr, result);
					}
					int iBox2 = this.index_list[num + 1] - 1;
					if (this.box_box_intersect(iBox2, ref axisAlignedBox3d))
					{
						this.find_intersections(iBox2, otherTree, TransformF, oBox, depth + 1, intr, result);
					}
				}
			}
		}

		public virtual Index2i FindNearestTriangles(DMeshAABBTree3 otherTree, Func<Vector3d, Vector3d> TransformF, out double distance, double max_dist = 1.7976931348623157E+308)
		{
			if (this.mesh_timestamp != this.mesh.ShapeTimestamp)
			{
				throw new Exception("DMeshAABBTree3.TestIntersection: mesh has been modified since tree construction");
			}
			double num = double.MaxValue;
			if (max_dist < 1.7976931348623157E+308)
			{
				num = max_dist * max_dist;
			}
			Index2i max = Index2i.Max;
			this.find_nearest_triangles(this.root_index, otherTree, TransformF, otherTree.root_index, 0, ref num, ref max);
			distance = ((num < double.MaxValue) ? Math.Sqrt(num) : double.MaxValue);
			return max;
		}

		protected void find_nearest_triangles(int iBox, DMeshAABBTree3 otherTree, Func<Vector3d, Vector3d> TransformF, int oBox, int depth, ref double nearest_sqr, ref Index2i nearest_pair)
		{
			int num = this.box_to_index[iBox];
			int num2 = otherTree.box_to_index[oBox];
			if (num < this.triangles_end && num2 < otherTree.triangles_end)
			{
				Triangle3d triangle = default(Triangle3d);
				Triangle3d triangle3d = default(Triangle3d);
				int num3 = this.index_list[num];
				int num4 = otherTree.index_list[num2];
				DistTriangle3Triangle3 distTriangle3Triangle = new DistTriangle3Triangle3(default(Triangle3d), default(Triangle3d));
				for (int i = 1; i <= num4; i++)
				{
					int num5 = otherTree.index_list[num2 + i];
					if (otherTree.TriangleFilterF == null || otherTree.TriangleFilterF(num5))
					{
						otherTree.mesh.GetTriVertices(num5, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
						if (TransformF != null)
						{
							triangle3d.V0 = TransformF(triangle3d.V0);
							triangle3d.V1 = TransformF(triangle3d.V1);
							triangle3d.V2 = TransformF(triangle3d.V2);
						}
						distTriangle3Triangle.Triangle0 = triangle3d;
						for (int j = 1; j <= num3; j++)
						{
							int num6 = this.index_list[num + j];
							if (this.TriangleFilterF == null || this.TriangleFilterF(num6))
							{
								this.mesh.GetTriVertices(num6, ref triangle.V0, ref triangle.V1, ref triangle.V2);
								distTriangle3Triangle.Triangle1 = triangle;
								double squared = distTriangle3Triangle.GetSquared();
								if (squared < nearest_sqr)
								{
									nearest_sqr = squared;
									nearest_pair = new Index2i(num6, num5);
								}
							}
						}
					}
				}
				return;
			}
			bool flag = num < this.triangles_end || depth % 2 == 0;
			if (flag && num2 < otherTree.triangles_end)
			{
				flag = false;
			}
			if (flag)
			{
				AxisAlignedBox3d axisAlignedBox3d = this.get_boxd(iBox);
				int num7 = otherTree.index_list[num2];
				if (num7 < 0)
				{
					num7 = -num7 - 1;
					if (otherTree.get_boxd(num7, TransformF).DistanceSquared(ref axisAlignedBox3d) < nearest_sqr)
					{
						this.find_nearest_triangles(iBox, otherTree, TransformF, num7, depth + 1, ref nearest_sqr, ref nearest_pair);
						return;
					}
				}
				else
				{
					num7--;
					int num8 = otherTree.index_list[num2 + 1] - 1;
					AxisAlignedBox3d axisAlignedBox3d2 = otherTree.get_boxd(num7, TransformF);
					AxisAlignedBox3d axisAlignedBox3d3 = otherTree.get_boxd(num8, TransformF);
					double num9 = axisAlignedBox3d2.DistanceSquared(ref axisAlignedBox3d);
					double num10 = axisAlignedBox3d3.DistanceSquared(ref axisAlignedBox3d);
					if (num10 < num9)
					{
						if (num10 < nearest_sqr)
						{
							this.find_nearest_triangles(iBox, otherTree, TransformF, num8, depth + 1, ref nearest_sqr, ref nearest_pair);
						}
						if (num9 < nearest_sqr)
						{
							this.find_nearest_triangles(iBox, otherTree, TransformF, num7, depth + 1, ref nearest_sqr, ref nearest_pair);
							return;
						}
					}
					else
					{
						if (num9 < nearest_sqr)
						{
							this.find_nearest_triangles(iBox, otherTree, TransformF, num7, depth + 1, ref nearest_sqr, ref nearest_pair);
						}
						if (num10 < nearest_sqr)
						{
							this.find_nearest_triangles(iBox, otherTree, TransformF, num8, depth + 1, ref nearest_sqr, ref nearest_pair);
							return;
						}
					}
				}
			}
			else
			{
				AxisAlignedBox3d axisAlignedBox3d4 = otherTree.get_boxd(oBox, TransformF);
				int num11 = this.index_list[num];
				if (num11 < 0)
				{
					num11 = -num11 - 1;
					if (this.box_box_distsqr(num11, ref axisAlignedBox3d4) < nearest_sqr)
					{
						this.find_nearest_triangles(num11, otherTree, TransformF, oBox, depth + 1, ref nearest_sqr, ref nearest_pair);
						return;
					}
				}
				else
				{
					num11--;
					int iBox2 = this.index_list[num + 1] - 1;
					double num12 = this.box_box_distsqr(num11, ref axisAlignedBox3d4);
					double num13 = this.box_box_distsqr(iBox2, ref axisAlignedBox3d4);
					if (num13 < num12)
					{
						if (num13 < nearest_sqr)
						{
							this.find_nearest_triangles(iBox2, otherTree, TransformF, oBox, depth + 1, ref nearest_sqr, ref nearest_pair);
						}
						if (num12 < nearest_sqr)
						{
							this.find_nearest_triangles(num11, otherTree, TransformF, oBox, depth + 1, ref nearest_sqr, ref nearest_pair);
							return;
						}
					}
					else
					{
						if (num12 < nearest_sqr)
						{
							this.find_nearest_triangles(num11, otherTree, TransformF, oBox, depth + 1, ref nearest_sqr, ref nearest_pair);
						}
						if (num13 < nearest_sqr)
						{
							this.find_nearest_triangles(iBox2, otherTree, TransformF, oBox, depth + 1, ref nearest_sqr, ref nearest_pair);
						}
					}
				}
			}
		}

		public bool SupportsPointContainment
		{
			get
			{
				return true;
			}
		}

		public virtual bool IsInside(Vector3d p)
		{
			Vector3d direction = new Vector3d(0.331960519038825, 0.462531727525156, 0.822111072077288);
			Ray3d ray = new Ray3d(p, direction, false);
			return this.FindAllHitTriangles(ray, null, double.MaxValue) % 2 != 0;
		}

		public virtual void DoTraversal(DMeshAABBTree3.TreeTraversal traversal)
		{
			if (this.mesh_timestamp != this.mesh.ShapeTimestamp)
			{
				throw new Exception("DMeshAABBTree3.DoTraversal: mesh has been modified since tree construction");
			}
			this.tree_traversal(this.root_index, 0, traversal);
		}

		protected virtual void tree_traversal(int iBox, int depth, DMeshAABBTree3.TreeTraversal traversal)
		{
			int num = this.box_to_index[iBox];
			if (num < this.triangles_end)
			{
				int num2 = this.index_list[num];
				for (int i = 1; i <= num2; i++)
				{
					int num3 = this.index_list[num + i];
					if (this.TriangleFilterF == null || this.TriangleFilterF(num3))
					{
						traversal.NextTriangleF(num3);
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

		public virtual double WindingNumber(Vector3d p)
		{
			if (this.mesh_timestamp != this.mesh.ShapeTimestamp)
			{
				throw new Exception("DMeshAABBTree3.WindingNumber: mesh has been modified since tree construction");
			}
			if (this.WindingCache == null || this.winding_cache_timestamp != this.mesh.ShapeTimestamp)
			{
				this.build_winding_cache();
				this.winding_cache_timestamp = this.mesh.ShapeTimestamp;
			}
			return this.branch_winding_num(this.root_index, p) / 12.566370614359172;
		}

		protected double branch_winding_num(int iBox, Vector3d p)
		{
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			double num = 0.0;
			int num2 = this.box_to_index[iBox];
			if (num2 < this.triangles_end)
			{
				int num3 = this.index_list[num2];
				for (int i = 1; i <= num3; i++)
				{
					int tID = this.index_list[num2 + i];
					this.mesh.GetTriVertices(tID, ref zero, ref zero2, ref zero3);
					num += MathUtil.TriSolidAngle(zero, zero2, zero3, ref p);
				}
			}
			else
			{
				int num4 = this.index_list[num2];
				if (num4 < 0)
				{
					num4 = -num4 - 1;
					if (!this.box_contains(num4, p) && this.WindingCache.ContainsKey(num4))
					{
						num += this.evaluate_box_winding_cache(num4, p);
					}
					else
					{
						num += this.branch_winding_num(num4, p);
					}
				}
				else
				{
					num4--;
					int num5 = this.index_list[num2 + 1] - 1;
					if (!this.box_contains(num4, p) && this.WindingCache.ContainsKey(num4))
					{
						num += this.evaluate_box_winding_cache(num4, p);
					}
					else
					{
						num += this.branch_winding_num(num4, p);
					}
					if (!this.box_contains(num5, p) && this.WindingCache.ContainsKey(num5))
					{
						num += this.evaluate_box_winding_cache(num5, p);
					}
					else
					{
						num += this.branch_winding_num(num5, p);
					}
				}
			}
			return num;
		}

		protected void build_winding_cache()
		{
			int tri_count_thresh = 100;
			if (this.Mesh.TriangleCount > 250000)
			{
				tri_count_thresh = 500;
			}
			if (this.Mesh.TriangleCount > 1000000)
			{
				tri_count_thresh = 1000;
			}
			this.WindingCache = new Dictionary<int, List<int>>();
			HashSet<int> hashSet;
			this.build_winding_cache(this.root_index, 0, tri_count_thresh, out hashSet);
		}

		protected int build_winding_cache(int iBox, int depth, int tri_count_thresh, out HashSet<int> tri_hash)
		{
			tri_hash = null;
			int num = this.box_to_index[iBox];
			if (num < this.triangles_end)
			{
				return this.index_list[num];
			}
			int num2 = this.index_list[num];
			if (num2 < 0)
			{
				num2 = -num2 - 1;
				return this.build_winding_cache(num2, depth + 1, tri_count_thresh, out tri_hash);
			}
			num2--;
			int iBox2 = this.index_list[num + 1] - 1;
			int num3 = this.build_winding_cache(num2, depth + 1, tri_count_thresh, out tri_hash);
			HashSet<int> hashSet;
			int num4 = this.build_winding_cache(iBox2, depth + 1, tri_count_thresh, out hashSet);
			bool flag = num3 + num4 > tri_count_thresh;
			if (depth == 0)
			{
				return num3 + num4;
			}
			if (tri_hash != null || hashSet != null || flag)
			{
				if (tri_hash == null && hashSet != null)
				{
					this.collect_triangles(num2, hashSet);
					tri_hash = hashSet;
				}
				else
				{
					if (tri_hash == null)
					{
						tri_hash = new HashSet<int>();
						this.collect_triangles(num2, tri_hash);
					}
					if (hashSet == null)
					{
						this.collect_triangles(iBox2, tri_hash);
					}
					else
					{
						tri_hash.UnionWith(hashSet);
					}
				}
			}
			if (flag)
			{
				this.make_box_winding_cache(iBox, tri_hash);
			}
			return num3 + num4;
		}

		protected void make_box_winding_cache(int iBox, HashSet<int> triangles)
		{
			List<int> list = new List<int>();
			foreach (int tID in triangles)
			{
				Index3i triangle = this.Mesh.GetTriangle(tID);
				Index3i triNeighbourTris = this.Mesh.GetTriNeighbourTris(tID);
				for (int i = 0; i < 3; i++)
				{
					if (triNeighbourTris[i] == -1 || !triangles.Contains(triNeighbourTris[i]))
					{
						list.Add(triangle[(i + 1) % 3]);
						list.Add(triangle[i]);
					}
				}
			}
			this.WindingCache[iBox] = list;
		}

		protected double evaluate_box_winding_cache(int iBox, Vector3d p)
		{
			List<int> list = this.WindingCache[iBox];
			int num = list.Count / 2;
			Vector3d c = this.box_centers[iBox];
			double num2 = 0.0;
			for (int i = 0; i < num; i++)
			{
				Vector3d vertex = this.Mesh.GetVertex(list[2 * i]);
				Vector3d vertex2 = this.Mesh.GetVertex(list[2 * i + 1]);
				num2 += MathUtil.TriSolidAngle(vertex, vertex2, c, ref p);
			}
			return -num2;
		}

		protected void collect_triangles(int iBox, HashSet<int> triangles)
		{
			int num = this.box_to_index[iBox];
			if (num < this.triangles_end)
			{
				int num2 = this.index_list[num];
				for (int i = 1; i <= num2; i++)
				{
					triangles.Add(this.index_list[num + i]);
				}
				return;
			}
			int num3 = this.index_list[num];
			if (num3 < 0)
			{
				this.collect_triangles(-num3 - 1, triangles);
				return;
			}
			this.collect_triangles(num3 - 1, triangles);
			this.collect_triangles(this.index_list[num + 1] - 1, triangles);
		}

		public virtual double FastWindingNumber(Vector3d p)
		{
			if (this.mesh_timestamp != this.mesh.ShapeTimestamp)
			{
				throw new Exception("DMeshAABBTree3.FastWindingNumber: mesh has been modified since tree construction");
			}
			if (this.FastWindingCache == null || this.fast_winding_cache_timestamp != this.mesh.ShapeTimestamp)
			{
				this.build_fast_winding_cache();
				this.fast_winding_cache_timestamp = this.mesh.ShapeTimestamp;
			}
			return this.branch_fast_winding_num(this.root_index, p);
		}

		protected double branch_fast_winding_num(int iBox, Vector3d p)
		{
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			double num = 0.0;
			int num2 = this.box_to_index[iBox];
			if (num2 < this.triangles_end)
			{
				int num3 = this.index_list[num2];
				for (int i = 1; i <= num3; i++)
				{
					int tID = this.index_list[num2 + i];
					this.mesh.GetTriVertices(tID, ref zero, ref zero2, ref zero3);
					num += MathUtil.TriSolidAngle(zero, zero2, zero3, ref p) / 12.566370614359172;
				}
			}
			else
			{
				int num4 = this.index_list[num2];
				if (num4 < 0)
				{
					num4 = -num4 - 1;
					if (!this.box_contains(num4, p) && this.can_use_fast_winding_cache(num4, ref p))
					{
						num += this.evaluate_box_fast_winding_cache(num4, ref p);
					}
					else
					{
						num += this.branch_fast_winding_num(num4, p);
					}
				}
				else
				{
					num4--;
					int iBox2 = this.index_list[num2 + 1] - 1;
					if (!this.box_contains(num4, p) && this.can_use_fast_winding_cache(num4, ref p))
					{
						num += this.evaluate_box_fast_winding_cache(num4, ref p);
					}
					else
					{
						num += this.branch_fast_winding_num(num4, p);
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
			int tri_count_thresh = 1;
			MeshTriInfoCache triCache = new MeshTriInfoCache(this.mesh);
			this.FastWindingCache = new Dictionary<int, DMeshAABBTree3.FWNInfo>();
			HashSet<int> hashSet;
			this.build_fast_winding_cache(this.root_index, 0, tri_count_thresh, out hashSet, triCache);
		}

		protected int build_fast_winding_cache(int iBox, int depth, int tri_count_thresh, out HashSet<int> tri_hash, MeshTriInfoCache triCache)
		{
			tri_hash = null;
			int num = this.box_to_index[iBox];
			if (num < this.triangles_end)
			{
				return this.index_list[num];
			}
			int num2 = this.index_list[num];
			if (num2 < 0)
			{
				num2 = -num2 - 1;
				return this.build_fast_winding_cache(num2, depth + 1, tri_count_thresh, out tri_hash, triCache);
			}
			num2--;
			int iBox2 = this.index_list[num + 1] - 1;
			int num3 = this.build_fast_winding_cache(num2, depth + 1, tri_count_thresh, out tri_hash, triCache);
			HashSet<int> hashSet;
			int num4 = this.build_fast_winding_cache(iBox2, depth + 1, tri_count_thresh, out hashSet, triCache);
			bool flag = num3 + num4 > tri_count_thresh;
			if (depth == 0)
			{
				return num3 + num4;
			}
			if (tri_hash != null || hashSet != null || flag)
			{
				if (tri_hash == null && hashSet != null)
				{
					this.collect_triangles(num2, hashSet);
					tri_hash = hashSet;
				}
				else
				{
					if (tri_hash == null)
					{
						tri_hash = new HashSet<int>();
						this.collect_triangles(num2, tri_hash);
					}
					if (hashSet == null)
					{
						this.collect_triangles(iBox2, tri_hash);
					}
					else
					{
						tri_hash.UnionWith(hashSet);
					}
				}
			}
			if (flag)
			{
				this.make_box_fast_winding_cache(iBox, tri_hash, triCache);
			}
			return num3 + num4;
		}

		protected bool can_use_fast_winding_cache(int iBox, ref Vector3d q)
		{
			DMeshAABBTree3.FWNInfo fwninfo;
			return this.FastWindingCache.TryGetValue(iBox, out fwninfo) && fwninfo.Center.Distance(ref q) > this.FWNBeta * fwninfo.R;
		}

		protected void make_box_fast_winding_cache(int iBox, IEnumerable<int> triangles, MeshTriInfoCache triCache)
		{
			DMeshAABBTree3.FWNInfo value = default(DMeshAABBTree3.FWNInfo);
			FastTriWinding.ComputeCoeffs(this.Mesh, triangles, ref value.Center, ref value.R, ref value.Order1Vec, ref value.Order2Mat, triCache);
			this.FastWindingCache[iBox] = value;
		}

		protected double evaluate_box_fast_winding_cache(int iBox, ref Vector3d q)
		{
			DMeshAABBTree3.FWNInfo fwninfo = this.FastWindingCache[iBox];
			if (this.FWNApproxOrder == 2)
			{
				return FastTriWinding.EvaluateOrder2Approx(ref fwninfo.Center, ref fwninfo.Order1Vec, ref fwninfo.Order2Mat, ref q);
			}
			return FastTriWinding.EvaluateOrder1Approx(ref fwninfo.Center, ref fwninfo.Order1Vec, ref q);
		}

		public double TotalVolume()
		{
			double volSum = 0.0;
			DMeshAABBTree3.TreeTraversal traversal = new DMeshAABBTree3.TreeTraversal
			{
				NextBoxF = delegate(AxisAlignedBox3f box, int depth)
				{
					volSum += (double)box.Volume;
					return true;
				}
			};
			this.DoTraversal(traversal);
			return volSum;
		}

		public double TotalExtentSum()
		{
			double extSum = 0.0;
			DMeshAABBTree3.TreeTraversal traversal = new DMeshAABBTree3.TreeTraversal
			{
				NextBoxF = delegate(AxisAlignedBox3f box, int depth)
				{
					extSum += (double)box.Extents.LengthL1;
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
			int[] array = new int[this.mesh.TriangleCount];
			Vector3d[] array2 = new Vector3d[this.mesh.TriangleCount];
			foreach (int num in this.mesh.TriangleIndices())
			{
				double lengthSquared = this.mesh.GetTriCentroid(num).LengthSquared;
				if (!double.IsNaN(lengthSquared) && !double.IsInfinity(lengthSquared))
				{
					array[i] = num;
					array2[i] = this.mesh.GetTriCentroid(num);
					i++;
				}
			}
			DMeshAABBTree3.boxes_set boxes_set = new DMeshAABBTree3.boxes_set();
			DMeshAABBTree3.boxes_set boxes_set2 = new DMeshAABBTree3.boxes_set();
			AxisAlignedBox3f axisAlignedBox3f;
			int num2 = bSorted ? this.split_tri_set_sorted(array, array2, 0, this.mesh.TriangleCount, 0, this.TopDownLeafMaxTriCount, boxes_set, boxes_set2, out axisAlignedBox3f) : this.split_tri_set_midpoint(array, array2, 0, this.mesh.TriangleCount, 0, this.TopDownLeafMaxTriCount, boxes_set, boxes_set2, out axisAlignedBox3f);
			this.box_to_index = boxes_set.box_to_index;
			this.box_centers = boxes_set.box_centers;
			this.box_extents = boxes_set.box_extents;
			this.index_list = boxes_set.index_list;
			this.triangles_end = boxes_set.iIndicesCur;
			int num3 = this.triangles_end;
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

		private int split_tri_set_sorted(int[] triangles, Vector3d[] centers, int iStart, int iCount, int depth, int minTriCount, DMeshAABBTree3.boxes_set tris, DMeshAABBTree3.boxes_set nodes, out AxisAlignedBox3f box)
		{
			box = AxisAlignedBox3f.Empty;
			int num;
			int num2;
			if (iCount < minTriCount)
			{
				num = tris.iBoxCur;
				tris.iBoxCur = num + 1;
				num2 = num;
				tris.box_to_index.insert(tris.iIndicesCur, num2);
				DVector<int> dvector = tris.index_list;
				num = tris.iIndicesCur;
				tris.iIndicesCur = num + 1;
				dvector.insert(iCount, num);
				for (int i = 0; i < iCount; i++)
				{
					DVector<int> dvector2 = tris.index_list;
					int value = triangles[iStart + i];
					num = tris.iIndicesCur;
					tris.iIndicesCur = num + 1;
					dvector2.insert(value, num);
					box.Contain(this.mesh.GetTriBounds(triangles[iStart + i]));
				}
				tris.box_centers.insert(box.Center, num2);
				tris.box_extents.insert(box.Extents, num2);
				return -(num2 + 1);
			}
			DMeshAABBTree3.AxisComp comparer = new DMeshAABBTree3.AxisComp
			{
				Axis = depth % 3
			};
			Array.Sort<Vector3d, int>(centers, triangles, iStart, iCount, comparer);
			int num3 = iCount / 2;
			int iCount2 = num3;
			int iCount3 = iCount - num3;
			int num4 = this.split_tri_set_sorted(triangles, centers, iStart, iCount2, depth + 1, minTriCount, tris, nodes, out box);
			AxisAlignedBox3f box2;
			int num5 = this.split_tri_set_sorted(triangles, centers, iStart + num3, iCount3, depth + 1, minTriCount, tris, nodes, out box2);
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

		private int split_tri_set_midpoint(int[] triangles, Vector3d[] centers, int iStart, int iCount, int depth, int minTriCount, DMeshAABBTree3.boxes_set tris, DMeshAABBTree3.boxes_set nodes, out AxisAlignedBox3f box)
		{
			box = AxisAlignedBox3f.Empty;
			int num;
			int num2;
			if (iCount < minTriCount)
			{
				num = tris.iBoxCur;
				tris.iBoxCur = num + 1;
				num2 = num;
				tris.box_to_index.insert(tris.iIndicesCur, num2);
				DVector<int> dvector = tris.index_list;
				num = tris.iIndicesCur;
				tris.iIndicesCur = num + 1;
				dvector.insert(iCount, num);
				for (int i = 0; i < iCount; i++)
				{
					DVector<int> dvector2 = tris.index_list;
					int value = triangles[iStart + i];
					num = tris.iIndicesCur;
					tris.iIndicesCur = num + 1;
					dvector2.insert(value, num);
					box.Contain(this.mesh.GetTriBounds(triangles[iStart + i]));
				}
				tris.box_centers.insert(box.Center, num2);
				tris.box_extents.insert(box.Extents, num2);
				return -(num2 + 1);
			}
			int key = depth % 3;
			Interval1d empty = Interval1d.Empty;
			for (int j = 0; j < iCount; j++)
			{
				empty.Contain(centers[iStart + j][key]);
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
					while (centers[iStart + k][key] <= center)
					{
						k++;
					}
					while (centers[iStart + num3][key] > center)
					{
						num3--;
					}
					if (k >= num3)
					{
						break;
					}
					Vector3d vector3d = centers[iStart + k];
					centers[iStart + k] = centers[iStart + num3];
					centers[iStart + num3] = vector3d;
					int num4 = triangles[iStart + k];
					triangles[iStart + k] = triangles[iStart + num3];
					triangles[iStart + num3] = num4;
				}
				num5 = k;
				iCount2 = iCount - num5;
			}
			else
			{
				num5 = iCount / 2;
				iCount2 = iCount - num5;
			}
			int num6 = this.split_tri_set_midpoint(triangles, centers, iStart, num5, depth + 1, minTriCount, tris, nodes, out box);
			AxisAlignedBox3f box2;
			int num7 = this.split_tri_set_midpoint(triangles, centers, iStart + num5, iCount2, depth + 1, minTriCount, tris, nodes, out box2);
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

		private void build_by_one_rings(DMeshAABBTree3.ClusterPolicy ePolicy)
		{
			this.box_to_index = new DVector<int>();
			this.box_centers = new DVector<Vector3f>();
			this.box_extents = new DVector<Vector3f>();
			int num = 0;
			this.index_list = new DVector<int>();
			int num2 = 0;
			byte[] array = new byte[this.mesh.MaxTriangleID];
			Array.Clear(array, 0, array.Length);
			int maxVtxEdgeCount = this.mesh.GetMaxVtxEdgeCount();
			int[] temp_tris = new int[2 * maxVtxEdgeCount];
			DVector<int> dvector = new DVector<int>();
			foreach (int num3 in this.mesh.VertexIndices())
			{
				if (this.add_one_ring_box(num3, array, temp_tris, ref num, ref num2, dvector, 3) < 3)
				{
					dvector.Add(num3);
				}
			}
			int length = dvector.Length;
			for (int i = 0; i < length; i++)
			{
				int vid = dvector[i];
				this.add_one_ring_box(vid, array, temp_tris, ref num, ref num2, null, 0);
			}
			this.triangles_end = num2;
			DMeshAABBTree3.ClusterFunctionType clusterFunctionType = new DMeshAABBTree3.ClusterFunctionType(this.cluster_boxes_nearsearch);
			if (ePolicy == DMeshAABBTree3.ClusterPolicy.Fastest)
			{
				clusterFunctionType = new DMeshAABBTree3.ClusterFunctionType(this.cluster_boxes);
			}
			else if (ePolicy == DMeshAABBTree3.ClusterPolicy.MinimalVolume)
			{
				clusterFunctionType = new DMeshAABBTree3.ClusterFunctionType(this.cluster_boxes_matrix);
			}
			else if (ePolicy == DMeshAABBTree3.ClusterPolicy.FastVolumeMetric)
			{
				clusterFunctionType = new DMeshAABBTree3.ClusterFunctionType(this.cluster_boxes_nearsearch);
			}
			int num4 = num;
			int j = clusterFunctionType(0, num, ref num, ref num2);
			int iStart = num4;
			int iCount = num - num4;
			while (j > 1)
			{
				num4 = num;
				j = clusterFunctionType(iStart, iCount, ref num, ref num2);
				iStart = num4;
				iCount = num - num4;
			}
			this.root_index = num - 1;
		}

		private int add_one_ring_box(int vid, byte[] used_triangles, int[] temp_tris, ref int iBoxCur, ref int iIndicesCur, DVector<int> spill, int nSpillThresh)
		{
			int num = 0;
			foreach (int num2 in this.mesh.VtxTrianglesItr(vid))
			{
				if (used_triangles[num2] == 0)
				{
					temp_tris[num++] = num2;
				}
			}
			if (num == 0)
			{
				return 0;
			}
			if (num < nSpillThresh)
			{
				spill.Add(vid);
				return num;
			}
			AxisAlignedBox3f empty = AxisAlignedBox3f.Empty;
			int num3 = iBoxCur;
			iBoxCur = num3 + 1;
			int index = num3;
			this.box_to_index.insert(iIndicesCur, index);
			DVector<int> dvector = this.index_list;
			int value = num;
			num3 = iIndicesCur;
			iIndicesCur = num3 + 1;
			dvector.insert(value, num3);
			for (int i = 0; i < num; i++)
			{
				DVector<int> dvector2 = this.index_list;
				int value2 = temp_tris[i];
				num3 = iIndicesCur;
				iIndicesCur = num3 + 1;
				dvector2.insert(value2, num3);
				int num4 = temp_tris[i];
				used_triangles[num4] += 1;
				empty.Contain(this.mesh.GetTriBounds(temp_tris[i]));
			}
			this.box_centers.insert(empty.Center, index);
			this.box_extents.insert(empty.Extents, index);
			return num;
		}

		private int cluster_boxes(int iStart, int iCount, ref int iBoxCur, ref int iIndicesCur)
		{
			int[] array = new int[iCount];
			for (int i = 0; i < iCount; i++)
			{
				array[i] = iStart + i;
			}
			int nDim = 0;
			Array.Sort<int>(array, delegate(int a, int b)
			{
				float num6 = this.box_centers[a][nDim] - this.box_extents[a][nDim];
				float num7 = this.box_centers[b][nDim] - this.box_extents[b][nDim];
				if (num6 == num7)
				{
					return 0;
				}
				if (num6 >= num7)
				{
					return 1;
				}
				return -1;
			});
			int num = iCount / 2;
			int num2 = iCount - 2 * num;
			for (int j = 0; j < num; j++)
			{
				int num3 = array[2 * j];
				int num4 = array[2 * j + 1];
				Vector3f value;
				Vector3f value2;
				this.get_combined_box(num3, num4, out value, out value2);
				int num5 = iBoxCur;
				iBoxCur = num5 + 1;
				int index = num5;
				this.box_to_index.insert(iIndicesCur, index);
				DVector<int> dvector = this.index_list;
				int value3 = num3 + 1;
				num5 = iIndicesCur;
				iIndicesCur = num5 + 1;
				dvector.insert(value3, num5);
				DVector<int> dvector2 = this.index_list;
				int value4 = num4 + 1;
				num5 = iIndicesCur;
				iIndicesCur = num5 + 1;
				dvector2.insert(value4, num5);
				this.box_centers.insert(value, index);
				this.box_extents.insert(value2, index);
			}
			if (num2 > 0)
			{
				if (num2 > 1)
				{
					Util.gBreakToDebugger();
				}
				int i2 = array[2 * num];
				this.duplicate_box(i2, ref iBoxCur, ref iIndicesCur);
			}
			return num + num2;
		}

		private int cluster_boxes_nearsearch(int iStart, int iCount, ref int iBoxCur, ref int iIndicesCur)
		{
			int[] array = new int[iCount];
			for (int i = 0; i < iCount; i++)
			{
				array[i] = iStart + i;
			}
			Func<int, int, double> func = new Func<int, int, double>(this.combined_box_volume);
			int nDim = 0;
			Array.Sort<int>(array, delegate(int a, int b)
			{
				float num11 = this.box_centers[a][nDim] - this.box_extents[a][nDim];
				float num12 = this.box_centers[b][nDim] - this.box_extents[b][nDim];
				if (num11 == num12)
				{
					return 0;
				}
				if (num11 >= num12)
				{
					return 1;
				}
				return -1;
			});
			int num = iCount / 2;
			int num2 = iCount - 2 * num;
			int bottomUpClusterLookahead = this.BottomUpClusterLookahead;
			int[] array2 = new int[bottomUpClusterLookahead];
			double[] array3 = new double[bottomUpClusterLookahead];
			for (int j = 0; j < iCount - 1; j++)
			{
				int num3 = array[j];
				if (num3 >= 0)
				{
					int num4 = Math.Min(bottomUpClusterLookahead, iCount - j - 1);
					for (int k = 0; k < num4; k++)
					{
						int num5 = j + k + 1;
						array2[k] = num5;
						int num6 = array[num5];
						if (num6 < 0)
						{
							array3[k] = double.MaxValue;
						}
						else
						{
							array3[k] = func(num3, num6);
						}
					}
					Array.Sort<double, int>(array3, array2, 0, num4);
					if (array3[0] != 1.7976931348623157E+308)
					{
						int num5 = array2[0];
						int num7 = array[num5];
						if (num7 < 0)
						{
							Util.gBreakToDebugger();
						}
						Vector3f value;
						Vector3f value2;
						this.get_combined_box(num3, num7, out value, out value2);
						int num8 = iBoxCur;
						iBoxCur = num8 + 1;
						int index = num8;
						this.box_to_index.insert(iIndicesCur, index);
						DVector<int> dvector = this.index_list;
						int value3 = num3 + 1;
						num8 = iIndicesCur;
						iIndicesCur = num8 + 1;
						dvector.insert(value3, num8);
						DVector<int> dvector2 = this.index_list;
						int value4 = num7 + 1;
						num8 = iIndicesCur;
						iIndicesCur = num8 + 1;
						dvector2.insert(value4, num8);
						this.box_centers.insert(value, index);
						this.box_extents.insert(value2, index);
						array[j] = -(array[j] + 1);
						array[num5] = -(array[num5] + 1);
					}
				}
			}
			if (num2 > 0)
			{
				int num9 = -1;
				int num10 = 0;
				while (num9 < 0 && num10 < array.Length)
				{
					if (array[num10] >= 0)
					{
						num9 = array[num10];
					}
					num10++;
				}
				this.duplicate_box(num9, ref iBoxCur, ref iIndicesCur);
			}
			return num + num2;
		}

		private static double find_smallest_upper(double[,] m, ref int ii, ref int jj)
		{
			double num = double.MaxValue;
			int length = m.GetLength(0);
			int length2 = m.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				for (int j = i + 1; j < length2; j++)
				{
					if (m[i, j] < num)
					{
						num = m[i, j];
						ii = i;
						jj = j;
					}
				}
			}
			return num;
		}

		private int cluster_boxes_matrix(int iStart, int iCount, ref int iBoxCur, ref int iIndicesCur)
		{
			int[] array = new int[iCount];
			for (int i = 0; i < iCount; i++)
			{
				array[i] = iStart + i;
			}
			Func<int, int, double> func = new Func<int, int, double>(this.combined_box_volume);
			double[,] array2 = new double[iCount, iCount];
			for (int j = 0; j < iCount; j++)
			{
				for (int k = 0; k <= j; k++)
				{
					array2[j, k] = double.MaxValue;
				}
				for (int l = j + 1; l < iCount; l++)
				{
					array2[j, l] = func(array[j], array[l]);
				}
			}
			int num = iCount / 2;
			int num2 = iCount - 2 * num;
			for (int m = 0; m < num; m++)
			{
				int num3 = 0;
				int num4 = 0;
				bool flag = false;
				while (!flag)
				{
					DMeshAABBTree3.find_smallest_upper(array2, ref num3, ref num4);
					if (array[num3] >= 0 && array[num4] >= 0)
					{
						flag = true;
					}
					array2[num3, num4] = double.MaxValue;
				}
				int num5 = array[num3];
				int num6 = array[num4];
				Vector3f value;
				Vector3f value2;
				this.get_combined_box(num5, num6, out value, out value2);
				int num7 = iBoxCur;
				iBoxCur = num7 + 1;
				int index = num7;
				this.box_to_index.insert(iIndicesCur, index);
				DVector<int> dvector = this.index_list;
				int value3 = num5 + 1;
				num7 = iIndicesCur;
				iIndicesCur = num7 + 1;
				dvector.insert(value3, num7);
				DVector<int> dvector2 = this.index_list;
				int value4 = num6 + 1;
				num7 = iIndicesCur;
				iIndicesCur = num7 + 1;
				dvector2.insert(value4, num7);
				this.box_centers.insert(value, index);
				this.box_extents.insert(value2, index);
				array[num3] = -(array[num3] + 1);
				array[num4] = -(array[num4] + 1);
			}
			if (num2 > 0)
			{
				int num8 = -1;
				int num9 = 0;
				while (num8 < 0 && num9 < array.Length)
				{
					if (array[num9] >= 0)
					{
						num8 = array[num9];
					}
					num9++;
				}
				this.duplicate_box(num8, ref iBoxCur, ref iIndicesCur);
			}
			return num + num2;
		}

		private void duplicate_box(int i, ref int iBoxCur, ref int iIndicesCur)
		{
			int num = iBoxCur;
			iBoxCur = num + 1;
			int index = num;
			this.box_to_index.insert(iIndicesCur, index);
			DVector<int> dvector = this.index_list;
			int value = -(i + 1);
			num = iIndicesCur;
			iIndicesCur = num + 1;
			dvector.insert(value, num);
			this.box_centers.insert(this.box_centers[i], index);
			this.box_extents.insert(this.box_extents[i], index);
		}

		private void get_combined_box(int b0, int b1, out Vector3f center, out Vector3f extent)
		{
			Vector3f vector3f = this.box_centers[b0];
			Vector3f vector3f2 = this.box_extents[b0];
			Vector3f vector3f3 = this.box_centers[b1];
			Vector3f vector3f4 = this.box_extents[b1];
			float num = Math.Min(vector3f.x - vector3f2.x, vector3f3.x - vector3f4.x);
			float num2 = Math.Max(vector3f.x + vector3f2.x, vector3f3.x + vector3f4.x);
			float num3 = Math.Min(vector3f.y - vector3f2.y, vector3f3.y - vector3f4.y);
			float num4 = Math.Max(vector3f.y + vector3f2.y, vector3f3.y + vector3f4.y);
			float num5 = Math.Min(vector3f.z - vector3f2.z, vector3f3.z - vector3f4.z);
			float num6 = Math.Max(vector3f.z + vector3f2.z, vector3f3.z + vector3f4.z);
			center = new Vector3f(0.5f * (num + num2), 0.5f * (num3 + num4), 0.5f * (num5 + num6));
			extent = new Vector3f(0.5f * (num2 - num), 0.5f * (num4 - num3), 0.5f * (num6 - num5));
		}

		private AxisAlignedBox3f get_box(int iBox)
		{
			Vector3f vector3f = this.box_centers[iBox];
			Vector3f vector3f2 = this.box_extents[iBox];
			return new AxisAlignedBox3f(ref vector3f, vector3f2.x + 5.9604645E-06f, vector3f2.y + 5.9604645E-06f, vector3f2.z + 5.9604645E-06f);
		}

		private AxisAlignedBox3d get_boxd(int iBox)
		{
			Vector3d vector3d = this.box_centers[iBox];
			Vector3f vector3f = this.box_extents[iBox];
			return new AxisAlignedBox3d(ref vector3d, (double)(vector3f.x + 5.9604645E-06f), (double)(vector3f.y + 5.9604645E-06f), (double)(vector3f.z + 5.9604645E-06f));
		}

		private AxisAlignedBox3d get_boxd(int iBox, Func<Vector3d, Vector3d> TransformF)
		{
			if (TransformF != null)
			{
				AxisAlignedBox3d axisAlignedBox3d = this.get_boxd(iBox);
				return BoundsUtil.Bounds(ref axisAlignedBox3d, TransformF);
			}
			return this.get_boxd(iBox);
		}

		private double box_ray_intersect_t(int iBox, Ray3d ray)
		{
			Vector3d vector3d = this.box_centers[iBox];
			Vector3f vector3f = this.box_extents[iBox];
			AxisAlignedBox3d axisAlignedBox3d = new AxisAlignedBox3d(ref vector3d, (double)(vector3f.x + 5.9604645E-06f), (double)(vector3f.y + 5.9604645E-06f), (double)(vector3f.z + 5.9604645E-06f));
			double maxValue = double.MaxValue;
			if (IntrRay3AxisAlignedBox3.FindRayIntersectT(ref ray, ref axisAlignedBox3d, out maxValue))
			{
				return maxValue;
			}
			return double.MaxValue;
		}

		private bool box_box_intersect(int iBox, ref AxisAlignedBox3d testBox)
		{
			Vector3d vector3d = this.box_centers[iBox];
			Vector3f vector3f = this.box_extents[iBox];
			return new AxisAlignedBox3d(ref vector3d, (double)(vector3f.x + 5.9604645E-06f), (double)(vector3f.y + 5.9604645E-06f), (double)(vector3f.z + 5.9604645E-06f)).Intersects(testBox);
		}

		private double box_box_distsqr(int iBox, ref AxisAlignedBox3d testBox)
		{
			Vector3d vector3d = this.box_centers[iBox];
			Vector3f vector3f = this.box_extents[iBox];
			return new AxisAlignedBox3d(ref vector3d, (double)(vector3f.x + 5.9604645E-06f), (double)(vector3f.y + 5.9604645E-06f), (double)(vector3f.z + 5.9604645E-06f)).DistanceSquared(ref testBox);
		}

		private double box_distance_sqr(int iBox, Vector3d p)
		{
			Vector3d vector3d = this.box_centers[iBox];
			Vector3f vector3f = this.box_extents[iBox];
			return new AxisAlignedBox3d(ref vector3d, (double)(vector3f.x + 5.9604645E-06f), (double)(vector3f.y + 5.9604645E-06f), (double)(vector3f.z + 5.9604645E-06f)).DistanceSquared(p);
		}

		protected bool box_contains(int iBox, Vector3d p)
		{
			Vector3d vector3d = this.box_centers[iBox];
			Vector3f vector3f = this.box_extents[iBox];
			return new AxisAlignedBox3d(ref vector3d, (double)(vector3f.x + 5.9604645E-06f), (double)(vector3f.y + 5.9604645E-06f), (double)(vector3f.z + 5.9604645E-06f)).Contains(p);
		}

		private double combined_box_volume(int b0, int b1)
		{
			Vector3f vector3f = this.box_centers[b0];
			Vector3f vector3f2 = this.box_extents[b0];
			Vector3f vector3f3 = this.box_centers[b1];
			Vector3f vector3f4 = this.box_extents[b1];
			float num = Math.Min(vector3f.x - vector3f2.x, vector3f3.x - vector3f4.x);
			float num2 = Math.Max(vector3f.x + vector3f2.x, vector3f3.x + vector3f4.x);
			float num3 = Math.Min(vector3f.y - vector3f2.y, vector3f3.y - vector3f4.y);
			float num4 = Math.Max(vector3f.y + vector3f2.y, vector3f3.y + vector3f4.y);
			float num5 = Math.Min(vector3f.z - vector3f2.z, vector3f3.z - vector3f4.z);
			float num6 = Math.Max(vector3f.z + vector3f2.z, vector3f3.z + vector3f4.z);
			return (double)((num2 - num) * (num4 - num3) * (num6 - num5));
		}

		private double combined_box_length(int b0, int b1)
		{
			Vector3f vector3f = this.box_centers[b0];
			Vector3f vector3f2 = this.box_extents[b0];
			Vector3f vector3f3 = this.box_centers[b1];
			Vector3f vector3f4 = this.box_extents[b1];
			float num = Math.Min(vector3f.x - vector3f2.x, vector3f3.x - vector3f4.x);
			float num2 = Math.Max(vector3f.x + vector3f2.x, vector3f3.x + vector3f4.x);
			float num3 = Math.Min(vector3f.y - vector3f2.y, vector3f3.y - vector3f4.y);
			float num4 = Math.Max(vector3f.y + vector3f2.y, vector3f3.y + vector3f4.y);
			float num5 = Math.Min(vector3f.z - vector3f2.z, vector3f3.z - vector3f4.z);
			float num6 = Math.Max(vector3f.z + vector3f2.z, vector3f3.z + vector3f4.z);
			return (double)((num2 - num) * (num2 - num) + (num4 - num3) * (num4 - num3) + (num6 - num5) * (num6 - num5));
		}

		public void TestCoverage()
		{
			int[] array = new int[this.mesh.MaxTriangleID];
			Array.Clear(array, 0, array.Length);
			int[] array2 = new int[this.box_to_index.Length];
			Array.Clear(array2, 0, array2.Length);
			this.test_coverage(array, array2, this.root_index);
			foreach (int num in this.mesh.TriangleIndices())
			{
				if (array[num] != 1)
				{
					Util.gBreakToDebugger();
				}
			}
		}

		private void test_coverage(int[] tri_counts, int[] parent_indices, int iBox)
		{
			int num = this.box_to_index[iBox];
			this.debug_check_child_tris_in_box(iBox);
			if (num < this.triangles_end)
			{
				int num2 = this.index_list[num];
				AxisAlignedBox3f axisAlignedBox3f = this.get_box(iBox);
				for (int i = 1; i <= num2; i++)
				{
					int num3 = this.index_list[num + i];
					tri_counts[num3]++;
					Index3i triangle = this.mesh.GetTriangle(num3);
					for (int j = 0; j < 3; j++)
					{
						Vector3f v = (Vector3f)this.mesh.GetVertex(triangle[j]);
						if (!axisAlignedBox3f.Contains(v))
						{
							Util.gBreakToDebugger();
						}
					}
				}
				return;
			}
			int num4 = this.index_list[num];
			if (num4 < 0)
			{
				num4 = -num4 - 1;
				parent_indices[num4] = iBox;
				this.test_coverage(tri_counts, parent_indices, num4);
				return;
			}
			num4--;
			parent_indices[num4] = iBox;
			this.test_coverage(tri_counts, parent_indices, num4);
			int num5 = this.index_list[num + 1];
			num5--;
			parent_indices[num5] = iBox;
			this.test_coverage(tri_counts, parent_indices, num5);
		}

		private void debug_check_child_tri_distances(int iBox, Vector3d p)
		{
			double fBoxDistSqr = this.box_distance_sqr(iBox, p);
			DMeshAABBTree3.TreeTraversal traversal = new DMeshAABBTree3.TreeTraversal
			{
				NextTriangleF = delegate(int tID)
				{
					double num = MeshQueries.TriDistanceSqr(this.mesh, tID, p);
					if (num < fBoxDistSqr && Math.Abs(num - fBoxDistSqr) > 1E-06)
					{
						Util.gBreakToDebugger();
					}
				}
			};
			this.tree_traversal(iBox, 0, traversal);
		}

		private void debug_check_child_tris_in_box(int iBox)
		{
			AxisAlignedBox3f box = this.get_box(iBox);
			DMeshAABBTree3.TreeTraversal traversal = new DMeshAABBTree3.TreeTraversal
			{
				NextTriangleF = delegate(int tID)
				{
					Index3i triangle = this.mesh.GetTriangle(tID);
					for (int i = 0; i < 3; i++)
					{
						Vector3f v = (Vector3f)this.mesh.GetVertex(triangle[i]);
						if (!box.Contains(v))
						{
							Util.gBreakToDebugger();
						}
					}
				}
			};
			this.tree_traversal(iBox, 0, traversal);
		}

		protected DMesh3 mesh;

		protected int mesh_timestamp;

		public Func<int, bool> TriangleFilterF;

		public int TopDownLeafMaxTriCount = 4;

		public int BottomUpClusterLookahead = 10;

		private Dictionary<int, List<int>> WindingCache;

		private int winding_cache_timestamp = -1;

		public double FWNBeta = 2.0;

		public int FWNApproxOrder = 2;

		private Dictionary<int, DMeshAABBTree3.FWNInfo> FastWindingCache;

		private int fast_winding_cache_timestamp = -1;

		protected DVector<int> box_to_index;

		protected DVector<Vector3f> box_centers;

		protected DVector<Vector3f> box_extents;

		protected DVector<int> index_list;

		protected int triangles_end = -1;

		protected int root_index = -1;

		private const float box_eps = 5.9604645E-06f;

		public enum BuildStrategy
		{
			Default,
			TopDownMidpoint,
			BottomUpFromOneRings,
			TopDownMedian
		}

		public enum ClusterPolicy
		{
			Default,
			Fastest,
			FastVolumeMetric,
			MinimalVolume
		}

		public struct PointIntersection
		{
			public int t0;

			public int t1;

			public Vector3d point;
		}

		public struct SegmentIntersection
		{
			public int t0;

			public int t1;

			public Vector3d point0;

			public Vector3d point1;
		}

		public class IntersectionsQueryResult
		{
			public List<DMeshAABBTree3.PointIntersection> Points;

			public List<DMeshAABBTree3.SegmentIntersection> Segments;
		}

		public class TreeTraversal
		{
			public Func<AxisAlignedBox3f, int, bool> NextBoxF = (AxisAlignedBox3f box, int depth) => true;

			public Action<int> NextTriangleF = delegate(int tID)
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

			public DVector<Vector3f> box_centers = new DVector<Vector3f>();

			public DVector<Vector3f> box_extents = new DVector<Vector3f>();

			public DVector<int> index_list = new DVector<int>();

			public int iBoxCur;

			public int iIndicesCur;
		}

		public delegate int ClusterFunctionType(int iStart, int iCount, ref int iBoxCur, ref int iIndicesCur);
	}
}
