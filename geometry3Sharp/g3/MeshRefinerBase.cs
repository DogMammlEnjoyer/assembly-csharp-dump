using System;

namespace g3
{
	public class MeshRefinerBase
	{
		public double EdgeFlipTolerance
		{
			get
			{
				return this.edge_flip_tol;
			}
			set
			{
				this.edge_flip_tol = MathUtil.Clamp(value, -1.0, 1.0);
			}
		}

		public MeshRefinerBase(DMesh3 mesh)
		{
			this.mesh = mesh;
		}

		protected MeshRefinerBase()
		{
		}

		public DMesh3 Mesh
		{
			get
			{
				return this.mesh;
			}
		}

		public MeshConstraints Constraints
		{
			get
			{
				return this.constraints;
			}
		}

		public void SetExternalConstraints(MeshConstraints cons)
		{
			this.constraints = cons;
		}

		protected virtual bool Cancelled()
		{
			return this.Progress != null && this.Progress.Cancelled();
		}

		protected double edge_flip_metric(ref Vector3d n0, ref Vector3d n1)
		{
			if (this.edge_flip_tol == 0.0)
			{
				return n0.Dot(n1);
			}
			return n0.Normalized.Dot(n1.Normalized);
		}

		protected bool collapse_creates_flip_or_invalid(int vid, int vother, ref Vector3d newv, int tc, int td)
		{
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			foreach (int num in this.mesh.VtxTrianglesItr(vid))
			{
				if (num != tc && num != td)
				{
					Index3i triangle = this.mesh.GetTriangle(num);
					if (triangle.a == vother || triangle.b == vother || triangle.c == vother)
					{
						return true;
					}
					this.mesh.GetTriVertices(num, ref zero, ref zero2, ref zero3);
					Vector3d vector3d = (zero2 - zero).Cross(zero3 - zero);
					double num2;
					if (triangle.a == vid)
					{
						Vector3d vector3d2 = (zero2 - newv).Cross(zero3 - newv);
						num2 = this.edge_flip_metric(ref vector3d, ref vector3d2);
					}
					else if (triangle.b == vid)
					{
						Vector3d vector3d3 = (newv - zero).Cross(zero3 - zero);
						num2 = this.edge_flip_metric(ref vector3d, ref vector3d3);
					}
					else
					{
						if (triangle.c != vid)
						{
							throw new Exception("should never be here!");
						}
						Vector3d vector3d4 = (zero2 - zero).Cross(newv - zero);
						num2 = this.edge_flip_metric(ref vector3d, ref vector3d4);
					}
					if (num2 <= this.edge_flip_tol)
					{
						return true;
					}
				}
			}
			return false;
		}

		protected bool flip_inverts_normals(int a, int b, int c, int d, int t0)
		{
			Vector3d vertex = this.mesh.GetVertex(c);
			Vector3d vertex2 = this.mesh.GetVertex(d);
			Index3i triangle = this.mesh.GetTriangle(t0);
			int vID = a;
			int vID2 = b;
			IndexUtil.orient_tri_edge(ref vID, ref vID2, ref triangle);
			Vector3d vertex3 = this.mesh.GetVertex(vID);
			Vector3d vertex4 = this.mesh.GetVertex(vID2);
			Vector3d vector3d = MathUtil.FastNormalDirection(ref vertex3, ref vertex4, ref vertex);
			Vector3d vector3d2 = MathUtil.FastNormalDirection(ref vertex4, ref vertex3, ref vertex2);
			Vector3d vector3d3 = MathUtil.FastNormalDirection(ref vertex, ref vertex2, ref vertex4);
			if (this.edge_flip_metric(ref vector3d, ref vector3d3) <= this.edge_flip_tol || this.edge_flip_metric(ref vector3d2, ref vector3d3) <= this.edge_flip_tol)
			{
				return true;
			}
			Vector3d vector3d4 = MathUtil.FastNormalDirection(ref vertex2, ref vertex, ref vertex3);
			return this.edge_flip_metric(ref vector3d, ref vector3d4) <= this.edge_flip_tol || this.edge_flip_metric(ref vector3d2, ref vector3d4) <= this.edge_flip_tol;
		}

		protected bool can_collapse_constraints(int eid, int a, int b, int c, int d, int tc, int td, out int collapse_to)
		{
			collapse_to = -1;
			if (this.constraints == null)
			{
				return true;
			}
			if (!this.can_collapse_vtx(eid, a, b, out collapse_to))
			{
				return false;
			}
			int vA = (collapse_to == a) ? b : a;
			if (c != -1)
			{
				int eid2 = this.mesh.FindEdgeFromTri(vA, c, tc);
				if (!this.constraints.GetEdgeConstraint(eid2).IsUnconstrained)
				{
					return false;
				}
			}
			if (d != -1)
			{
				int eid3 = this.mesh.FindEdgeFromTri(vA, d, td);
				if (!this.constraints.GetEdgeConstraint(eid3).IsUnconstrained)
				{
					return false;
				}
			}
			return true;
		}

		protected bool can_collapse_vtx(int eid, int a, int b, out int collapse_to)
		{
			collapse_to = -1;
			if (this.constraints == null)
			{
				return true;
			}
			VertexConstraint vertexConstraint = this.constraints.GetVertexConstraint(a);
			VertexConstraint vertexConstraint2 = this.constraints.GetVertexConstraint(b);
			if (!vertexConstraint.Fixed && !vertexConstraint2.Fixed && vertexConstraint.Target == null && vertexConstraint2.Target == null)
			{
				return true;
			}
			if (vertexConstraint.Fixed && !vertexConstraint2.Fixed)
			{
				if (vertexConstraint2.Target != null && vertexConstraint2.Target != vertexConstraint.Target)
				{
					return false;
				}
				collapse_to = a;
				return true;
			}
			else if (vertexConstraint2.Fixed && !vertexConstraint.Fixed)
			{
				if (vertexConstraint.Target != null && vertexConstraint.Target != vertexConstraint2.Target)
				{
					return false;
				}
				collapse_to = b;
				return true;
			}
			else
			{
				if (this.AllowCollapseFixedVertsWithSameSetID && vertexConstraint.FixedSetID >= 0 && vertexConstraint.FixedSetID == vertexConstraint2.FixedSetID)
				{
					return true;
				}
				if (vertexConstraint.Target != null && vertexConstraint2.Target == null)
				{
					collapse_to = a;
					return true;
				}
				if (vertexConstraint2.Target != null && vertexConstraint.Target == null)
				{
					collapse_to = b;
					return true;
				}
				return vertexConstraint2.Target != null && vertexConstraint.Target != null && vertexConstraint.Target == vertexConstraint2.Target && this.constraints.GetEdgeConstraint(eid).Target == vertexConstraint.Target;
			}
		}

		protected bool vertex_is_fixed(int vid)
		{
			return this.constraints != null && this.constraints.GetVertexConstraint(vid).Fixed;
		}

		protected bool vertex_is_constrained(int vid)
		{
			if (this.constraints != null)
			{
				VertexConstraint vertexConstraint = this.constraints.GetVertexConstraint(vid);
				if (vertexConstraint.Fixed || vertexConstraint.Target != null)
				{
					return true;
				}
			}
			return false;
		}

		protected VertexConstraint get_vertex_constraint(int vid)
		{
			if (this.constraints != null)
			{
				return this.constraints.GetVertexConstraint(vid);
			}
			return VertexConstraint.Unconstrained;
		}

		protected bool get_vertex_constraint(int vid, ref VertexConstraint vc)
		{
			return this.constraints != null && this.constraints.GetVertexConstraint(vid, ref vc);
		}

		protected DMesh3 mesh;

		protected MeshConstraints constraints;

		public bool AllowCollapseFixedVertsWithSameSetID = true;

		protected double edge_flip_tol;

		public ProgressCancel Progress;
	}
}
