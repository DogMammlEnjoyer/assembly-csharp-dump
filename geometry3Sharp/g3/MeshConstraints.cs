using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class MeshConstraints
	{
		public MeshConstraints()
		{
			this.set_id_counter = 0;
		}

		public int AllocateSetID()
		{
			int num = this.set_id_counter;
			this.set_id_counter = num + 1;
			return num;
		}

		public bool HasEdgeConstraint(int eid)
		{
			return this.Edges.ContainsKey(eid);
		}

		public EdgeConstraint GetEdgeConstraint(int eid)
		{
			EdgeConstraint result;
			if (this.Edges.TryGetValue(eid, out result))
			{
				return result;
			}
			return EdgeConstraint.Unconstrained;
		}

		public void SetOrUpdateEdgeConstraint(int eid, EdgeConstraint ec)
		{
			this.Edges[eid] = ec;
		}

		public void ClearEdgeConstraint(int eid)
		{
			this.Edges.Remove(eid);
		}

		public List<int> FindConstrainedEdgesBySetID(int setID)
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, EdgeConstraint> keyValuePair in this.Edges)
			{
				if (keyValuePair.Value.TrackingSetID == setID)
				{
					list.Add(keyValuePair.Key);
				}
			}
			return list;
		}

		public bool HasVertexConstraint(int vid)
		{
			return this.Vertices.ContainsKey(vid);
		}

		public VertexConstraint GetVertexConstraint(int vid)
		{
			VertexConstraint result;
			if (this.Vertices.TryGetValue(vid, out result))
			{
				return result;
			}
			return VertexConstraint.Unconstrained;
		}

		public bool GetVertexConstraint(int vid, ref VertexConstraint vc)
		{
			return this.Vertices.TryGetValue(vid, out vc);
		}

		public void SetOrUpdateVertexConstraint(int vid, VertexConstraint vc)
		{
			this.Vertices[vid] = vc;
		}

		public void ClearVertexConstraint(int vid)
		{
			this.Vertices.Remove(vid);
		}

		public IEnumerable VertexConstraintsItr()
		{
			foreach (KeyValuePair<int, VertexConstraint> keyValuePair in this.Vertices)
			{
				yield return keyValuePair;
			}
			Dictionary<int, VertexConstraint>.Enumerator enumerator = default(Dictionary<int, VertexConstraint>.Enumerator);
			yield break;
			yield break;
		}

		public bool HasConstraints
		{
			get
			{
				return this.Edges.Count > 0 || this.Vertices.Count > 0;
			}
		}

		private Dictionary<int, EdgeConstraint> Edges = new Dictionary<int, EdgeConstraint>();

		private int set_id_counter;

		private Dictionary<int, VertexConstraint> Vertices = new Dictionary<int, VertexConstraint>();
	}
}
