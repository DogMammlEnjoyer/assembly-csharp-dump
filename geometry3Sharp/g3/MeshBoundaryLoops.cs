using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public class MeshBoundaryLoops : IEnumerable<EdgeLoop>, IEnumerable
	{
		public MeshBoundaryLoops(DMesh3 mesh, bool bAutoCompute = true)
		{
			this.Mesh = mesh;
			if (bAutoCompute)
			{
				this.Compute();
			}
		}

		public int Count
		{
			get
			{
				return this.Loops.Count;
			}
		}

		public int SpanCount
		{
			get
			{
				return this.Spans.Count;
			}
		}

		public EdgeLoop this[int index]
		{
			get
			{
				return this.Loops[index];
			}
		}

		public IEnumerator<EdgeLoop> GetEnumerator()
		{
			return this.Loops.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Loops.GetEnumerator();
		}

		public int MaxVerticesLoopIndex
		{
			get
			{
				int num = 0;
				for (int i = 1; i < this.Loops.Count; i++)
				{
					if (this.Loops[i].Vertices.Length > this.Loops[num].Vertices.Length)
					{
						num = i;
					}
				}
				return num;
			}
		}

		public Index2i FindVertexIndex(int vID)
		{
			int count = this.Loops.Count;
			for (int i = 0; i < count; i++)
			{
				int num = this.Loops[i].FindVertexIndex(vID);
				if (num >= 0)
				{
					return new Index2i(i, num);
				}
			}
			return Index2i.Max;
		}

		public int FindLoopContainingVertex(int vid)
		{
			int count = this.Loops.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.Loops[i].Vertices.Contains(vid))
				{
					return i;
				}
			}
			return -1;
		}

		public int FindLoopContainingEdge(int eid)
		{
			int count = this.Loops.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.Loops[i].Edges.Contains(eid))
				{
					return i;
				}
			}
			return -1;
		}

		public bool Compute()
		{
			this.Loops = new List<EdgeLoop>();
			this.Spans = new List<EdgeSpan>();
			if (this.Mesh.CachedIsClosed)
			{
				return true;
			}
			int maxEdgeID = this.Mesh.MaxEdgeID;
			BitArray bitArray = new BitArray(maxEdgeID);
			bitArray.SetAll(false);
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			List<int> list3 = new List<int>();
			int[] array = new int[16];
			for (int i = 0; i < maxEdgeID; i++)
			{
				if (this.Mesh.IsEdge(i) && !bitArray[i] && this.Mesh.IsBoundaryEdge(i))
				{
					if (this.EdgeFilterF != null && !this.EdgeFilterF(i))
					{
						bitArray[i] = true;
					}
					else
					{
						int num = i;
						bitArray[num] = true;
						list.Add(num);
						int num2 = i;
						bool flag = false;
						bool flag2 = false;
						while (!flag)
						{
							Index2i orientedBoundaryEdgeV = this.Mesh.GetOrientedBoundaryEdgeV(num2);
							int item = orientedBoundaryEdgeV.a;
							int num3 = orientedBoundaryEdgeV.b;
							if (flag2)
							{
								item = orientedBoundaryEdgeV.b;
								num3 = orientedBoundaryEdgeV.a;
							}
							else
							{
								list2.Add(item);
							}
							int num4 = -1;
							int num5 = 1;
							int num6 = this.Mesh.VtxBoundaryEdges(num3, ref num4, ref num5);
							if (this.EdgeFilterF != null)
							{
								if (num6 > 2)
								{
									if (num6 >= array.Length)
									{
										array = new int[num6];
									}
									int max_i = this.Mesh.VtxAllBoundaryEdges(num3, array);
									max_i = BufferUtil.CountValid<int>(array, this.EdgeFilterF, max_i);
								}
								else
								{
									if (!this.EdgeFilterF(num4))
									{
										num6--;
									}
									if (!this.EdgeFilterF(num5))
									{
										num6--;
									}
								}
							}
							if (num6 < 2)
							{
								if (this.SpanBehavior == MeshBoundaryLoops.SpanBehaviors.ThrowException)
								{
									throw new MeshBoundaryLoopsException("MeshBoundaryLoops.Compute: found open span at vertex " + num3.ToString())
									{
										UnclosedLoop = true
									};
								}
								if (flag2)
								{
									flag = true;
								}
								else
								{
									flag2 = true;
									num2 = list[0];
									list.Reverse();
								}
							}
							else
							{
								int num7;
								if (num6 > 2)
								{
									if (num3 == list2[0])
									{
										num7 = -2;
									}
									else
									{
										if (num6 >= array.Length)
										{
											array = new int[2 * num6];
										}
										int num8 = this.Mesh.VtxAllBoundaryEdges(num3, array);
										if (this.EdgeFilterF != null)
										{
											num8 = BufferUtil.FilterInPlace<int>(array, this.EdgeFilterF, num8);
										}
										num7 = this.find_left_turn_edge(num2, num3, array, num8, bitArray);
										if (num7 == -1)
										{
											if (this.FailureBehavior == MeshBoundaryLoops.FailureBehaviors.ThrowException || this.SpanBehavior == MeshBoundaryLoops.SpanBehaviors.ThrowException)
											{
												throw new MeshBoundaryLoopsException("MeshBoundaryLoops.Compute: cannot find valid outgoing edge at bowtie vertex " + num3.ToString())
												{
													BowtieFailure = true
												};
											}
											if (flag2)
											{
												flag = true;
												continue;
											}
											flag2 = true;
											flag = true;
											continue;
										}
									}
									if (!list3.Contains(num3))
									{
										list3.Add(num3);
									}
								}
								else
								{
									num7 = ((num4 == num2) ? num5 : num4);
								}
								if (num7 == -2)
								{
									flag = true;
								}
								else if (num7 == num)
								{
									flag = true;
								}
								else if (bitArray[num7])
								{
									if (this.FailureBehavior == MeshBoundaryLoops.FailureBehaviors.ThrowException || this.SpanBehavior == MeshBoundaryLoops.SpanBehaviors.ThrowException)
									{
										throw new MeshBoundaryLoopsException("MeshBoundaryLoops.Compute: encountered repeated edge " + num7.ToString())
										{
											RepeatedEdge = true
										};
									}
									flag2 = true;
									flag = true;
								}
								else
								{
									list.Add(num7);
									bitArray[num7] = true;
									num2 = num7;
								}
							}
						}
						if (flag2)
						{
							this.SawOpenSpans = true;
							if (this.SpanBehavior == MeshBoundaryLoops.SpanBehaviors.Compute)
							{
								list.Reverse();
								EdgeSpan item2 = EdgeSpan.FromEdges(this.Mesh, list);
								this.Spans.Add(item2);
							}
						}
						else
						{
							if (list3.Count > 0)
							{
								MeshBoundaryLoops.Subloops subloops = this.extract_subloops(list2, list, list3);
								foreach (EdgeLoop item3 in subloops.Loops)
								{
									this.Loops.Add(item3);
								}
								if (subloops.Spans.Count <= 0)
								{
									goto IL_45F;
								}
								this.FellBackToSpansOnFailure = true;
								using (List<EdgeSpan>.Enumerator enumerator2 = subloops.Spans.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										EdgeSpan item4 = enumerator2.Current;
										this.Spans.Add(item4);
									}
									goto IL_45F;
								}
							}
							EdgeLoop edgeLoop = new EdgeLoop(this.Mesh);
							edgeLoop.Vertices = list2.ToArray();
							edgeLoop.Edges = list.ToArray();
							this.Loops.Add(edgeLoop);
						}
						IL_45F:
						list.Clear();
						list2.Clear();
						list3.Clear();
					}
				}
			}
			return true;
		}

		private Vector3d get_vtx_normal(int vid)
		{
			Vector3d vector3d = Vector3d.Zero;
			foreach (int tID in this.Mesh.VtxTrianglesItr(vid))
			{
				vector3d += this.Mesh.GetTriNormal(tID);
			}
			vector3d.Normalize(2.220446049250313E-16);
			return vector3d;
		}

		private int find_left_turn_edge(int incoming_e, int bowtie_v, int[] bdry_edges, int bdry_edges_count, BitArray used_edges)
		{
			Vector3d v = this.get_vtx_normal(bowtie_v);
			int vID = this.Mesh.edge_other_v(incoming_e, bowtie_v);
			Vector3d v2 = this.Mesh.GetVertex(bowtie_v) - this.Mesh.GetVertex(vID);
			int result = -1;
			double num = double.MaxValue;
			for (int i = 0; i < bdry_edges_count; i++)
			{
				int num2 = bdry_edges[i];
				if (!used_edges[num2])
				{
					Index2i orientedBoundaryEdgeV = this.Mesh.GetOrientedBoundaryEdgeV(num2);
					if (orientedBoundaryEdgeV.a == bowtie_v)
					{
						Vector3d v3 = this.Mesh.GetVertex(orientedBoundaryEdgeV.b) - this.Mesh.GetVertex(bowtie_v);
						float num3 = MathUtil.PlaneAngleSignedD((Vector3f)v2, (Vector3f)v3, (Vector3f)v);
						if (num == 1.7976931348623157E+308 || (double)num3 < num)
						{
							num = (double)num3;
							result = num2;
						}
					}
				}
			}
			return result;
		}

		private MeshBoundaryLoops.Subloops extract_subloops(List<int> loopV, List<int> loopE, List<int> bowties)
		{
			MeshBoundaryLoops.Subloops subloops = default(MeshBoundaryLoops.Subloops);
			subloops.Loops = new List<EdgeLoop>();
			subloops.Spans = new List<EdgeSpan>();
			List<int> list = new List<int>();
			foreach (int item in bowties)
			{
				if (this.count_in_list(loopV, item) > 1)
				{
					list.Add(item);
				}
			}
			if (list.Count == 0)
			{
				subloops.Loops.Add(new EdgeLoop(this.Mesh)
				{
					Vertices = loopV.ToArray(),
					Edges = loopE.ToArray(),
					BowtieVertices = bowties.ToArray()
				});
				return subloops;
			}
			while (list.Count > 0)
			{
				int i = 0;
				int num = 0;
				int i2 = -1;
				int i3 = -1;
				int num2 = -1;
				int num3 = int.MaxValue;
				while (i < list.Count)
				{
					num = list[i];
					if (this.is_simple_bowtie_loop(loopV, list, num, out i2, out i3))
					{
						int num4 = this.count_span(loopV, i2, i3);
						if (num4 < num3)
						{
							num2 = num;
							num3 = num4;
						}
					}
					i++;
				}
				if (num2 == -1)
				{
					if (this.FailureBehavior == MeshBoundaryLoops.FailureBehaviors.ThrowException)
					{
						this.FailureBowties = list;
						throw new MeshBoundaryLoopsException("MeshBoundaryLoops.Compute: Cannot find a valid simple loop");
					}
					EdgeSpan edgeSpan = new EdgeSpan(this.Mesh);
					List<int> list2 = new List<int>();
					for (int j = 0; j < loopV.Count; j++)
					{
						if (loopV[j] != -1)
						{
							list2.Add(loopV[j]);
						}
					}
					edgeSpan.Vertices = list2.ToArray();
					edgeSpan.Edges = EdgeSpan.VerticesToEdges(this.Mesh, edgeSpan.Vertices);
					edgeSpan.BowtieVertices = bowties.ToArray();
					subloops.Spans.Add(edgeSpan);
					return subloops;
				}
				else
				{
					if (num != num2)
					{
						num = num2;
						this.is_simple_bowtie_loop(loopV, list, num, out i2, out i3);
					}
					EdgeLoop edgeLoop = new EdgeLoop(this.Mesh);
					edgeLoop.Vertices = this.extract_span(loopV, i2, i3, true);
					edgeLoop.Edges = EdgeLoop.VertexLoopToEdgeLoop(this.Mesh, edgeLoop.Vertices);
					edgeLoop.BowtieVertices = bowties.ToArray();
					subloops.Loops.Add(edgeLoop);
					if (this.count_in_list(loopV, num) < 2)
					{
						list.Remove(num);
					}
				}
			}
			int num5 = 0;
			for (int k = 0; k < loopV.Count; k++)
			{
				if (loopV[k] != -1)
				{
					num5++;
				}
			}
			if (num5 > 0)
			{
				EdgeLoop edgeLoop2 = new EdgeLoop(this.Mesh);
				edgeLoop2.Vertices = new int[num5];
				int num6 = 0;
				for (int l = 0; l < loopV.Count; l++)
				{
					if (loopV[l] != -1)
					{
						edgeLoop2.Vertices[num6++] = loopV[l];
					}
				}
				edgeLoop2.Edges = EdgeLoop.VertexLoopToEdgeLoop(this.Mesh, edgeLoop2.Vertices);
				edgeLoop2.BowtieVertices = bowties.ToArray();
				subloops.Loops.Add(edgeLoop2);
			}
			return subloops;
		}

		private bool is_simple_bowtie_loop(List<int> loopV, List<int> bowties, int bowtieV, out int start_i, out int end_i)
		{
			start_i = this.find_index(loopV, 0, bowtieV);
			end_i = this.find_index(loopV, start_i + 1, bowtieV);
			if (this.is_simple_path(loopV, bowties, bowtieV, start_i, end_i))
			{
				return true;
			}
			if (this.is_simple_path(loopV, bowties, bowtieV, end_i, start_i))
			{
				int num = start_i;
				start_i = end_i;
				end_i = num;
				return true;
			}
			return false;
		}

		private bool is_simple_path(List<int> loopV, List<int> bowties, int bowtieV, int i1, int i2)
		{
			int count = loopV.Count;
			for (int num = i1; num != i2; num = (num + 1) % count)
			{
				int num2 = loopV[num];
				if (num2 != -1 && num2 != bowtieV && bowties.Contains(num2))
				{
					return false;
				}
			}
			return true;
		}

		private int[] extract_span(List<int> loop, int i0, int i1, bool bMarkInvalid)
		{
			int[] array = new int[this.count_span(loop, i0, i1)];
			int num = 0;
			int count = loop.Count;
			for (int num2 = i0; num2 != i1; num2 = (num2 + 1) % count)
			{
				if (loop[num2] != -1)
				{
					array[num++] = loop[num2];
					if (bMarkInvalid)
					{
						loop[num2] = -1;
					}
				}
			}
			return array;
		}

		private int count_span(List<int> l, int i0, int i1)
		{
			int num = 0;
			int count = l.Count;
			for (int num2 = i0; num2 != i1; num2 = (num2 + 1) % count)
			{
				if (l[num2] != -1)
				{
					num++;
				}
			}
			return num;
		}

		private int find_index(List<int> loop, int start, int item)
		{
			for (int i = start; i < loop.Count; i++)
			{
				if (loop[i] == item)
				{
					return i;
				}
			}
			return -1;
		}

		private int count_in_list(List<int> loop, int item)
		{
			int num = 0;
			for (int i = 0; i < loop.Count; i++)
			{
				if (loop[i] == item)
				{
					num++;
				}
			}
			return num;
		}

		public DMesh3 Mesh;

		public List<EdgeLoop> Loops;

		public List<EdgeSpan> Spans;

		public bool SawOpenSpans;

		public bool FellBackToSpansOnFailure;

		public MeshBoundaryLoops.SpanBehaviors SpanBehavior = MeshBoundaryLoops.SpanBehaviors.Compute;

		public MeshBoundaryLoops.FailureBehaviors FailureBehavior = MeshBoundaryLoops.FailureBehaviors.ConvertToOpenSpan;

		public Func<int, bool> EdgeFilterF;

		public List<int> FailureBowties;

		public enum SpanBehaviors
		{
			Ignore,
			ThrowException,
			Compute
		}

		public enum FailureBehaviors
		{
			ThrowException,
			ConvertToOpenSpan
		}

		private struct Subloops
		{
			public List<EdgeLoop> Loops;

			public List<EdgeSpan> Spans;
		}
	}
}
