using System;
using System.Collections.Generic;
using g3;

namespace gs
{
	public class MeshStitchLoops
	{
		public MeshStitchLoops(DMesh3 mesh, EdgeLoop l0, EdgeLoop l1)
		{
			this.Mesh = mesh;
			this.Loop0 = new EdgeLoop(l0);
			this.Loop1 = new EdgeLoop(l1);
			MeshStitchLoops.span item = new MeshStitchLoops.span
			{
				span0 = new Interval1i(0, 0),
				span1 = new Interval1i(0, 0)
			};
			this.spans.Add(item);
		}

		public void AddKnownCorrespondences(int[] verts0, int[] verts1)
		{
			int num = verts0.Length;
			if (num != verts1.Length)
			{
				throw new Exception("MeshStitchLoops.AddKnownCorrespondence: lengths not the same!");
			}
			List<Index2i> list = new List<Index2i>();
			for (int i = 0; i < num; i++)
			{
				int ii = this.Loop0.FindVertexIndex(verts0[i]);
				int jj = this.Loop1.FindVertexIndex(verts1[i]);
				list.Add(new Index2i(ii, jj));
			}
			list.Sort((Index2i pair1, Index2i pair2) => pair1.a.CompareTo(pair2.a));
			List<MeshStitchLoops.span> list2 = new List<MeshStitchLoops.span>();
			for (int j = 0; j < list.Count; j++)
			{
				Index2i index2i = list[j];
				Index2i index2i2 = list[(j + 1) % list.Count];
				MeshStitchLoops.span item = new MeshStitchLoops.span
				{
					span0 = new Interval1i(index2i.a, index2i2.a),
					span1 = new Interval1i(index2i.b, index2i2.b)
				};
				list2.Add(item);
			}
			this.spans = list2;
		}

		public bool Stitch()
		{
			if (this.spans.Count == 1)
			{
				throw new Exception("MeshStitchLoops.Stitch: blind stitching not supported yet...");
			}
			int groupID = this.Group.GetGroupID(this.Mesh);
			bool result = true;
			int count = this.spans.Count;
			for (int i = 0; i < count; i++)
			{
				MeshStitchLoops.span s = this.spans[i];
				if (!this.stitch_span_simple(s, groupID))
				{
					result = false;
				}
			}
			return result;
		}

		private bool stitch_span_simple(MeshStitchLoops.span s, int gid)
		{
			bool result = true;
			int num = this.Loop0.Vertices.Length;
			int num2 = this.Loop1.Vertices.Length;
			int num3 = s.span0.a;
			int b = s.span0.b;
			int num4 = s.span1.a;
			int b2 = s.span1.b;
			while (num3 != b && num4 != b2)
			{
				int num5 = (num3 + 1) % num;
				int num6 = (num4 + 1) % num2;
				int b3 = this.Loop0.Vertices[num3];
				int num7 = this.Loop0.Vertices[num5];
				int num8 = this.Loop1.Vertices[num4];
				int b4 = this.Loop1.Vertices[num6];
				if (!this.add_triangle(num7, b3, num8, gid))
				{
					result = false;
				}
				if (!this.add_triangle(num8, b4, num7, gid))
				{
					result = false;
				}
				num3 = num5;
				num4 = num6;
			}
			int c = this.Loop1.Vertices[num4];
			while (num3 != b)
			{
				int num9 = (num3 + 1) % num;
				int b5 = this.Loop0.Vertices[num3];
				int a = this.Loop0.Vertices[num9];
				if (!this.add_triangle(a, b5, c, gid))
				{
					result = false;
				}
				num3 = num9;
			}
			int c2 = this.Loop0.Vertices[num3];
			while (num4 != b2)
			{
				int num10 = (num4 + 1) % num2;
				int a2 = this.Loop1.Vertices[num4];
				int b6 = this.Loop1.Vertices[num10];
				if (!this.add_triangle(a2, b6, c2, gid))
				{
					result = false;
				}
				num4 = num10;
			}
			return result;
		}

		private bool add_triangle(int a, int b, int c, int gid)
		{
			int num;
			if (!this.TrustLoopOrientations)
			{
				int eID = this.Mesh.FindEdge(a, b);
				Index2i orientedBoundaryEdgeV = this.Mesh.GetOrientedBoundaryEdgeV(eID);
				num = this.Mesh.AppendTriangle(orientedBoundaryEdgeV.b, orientedBoundaryEdgeV.a, c, gid);
			}
			else
			{
				num = this.Mesh.AppendTriangle(a, b, c, gid);
			}
			return num >= 0;
		}

		public DMesh3 Mesh;

		public EdgeLoop Loop0;

		public EdgeLoop Loop1;

		public bool TrustLoopOrientations = true;

		public SetGroupBehavior Group = SetGroupBehavior.AutoGenerate;

		private List<MeshStitchLoops.span> spans = new List<MeshStitchLoops.span>();

		private struct span
		{
			public Interval1i span0;

			public Interval1i span1;
		}
	}
}
