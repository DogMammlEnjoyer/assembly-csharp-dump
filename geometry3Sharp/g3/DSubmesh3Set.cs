using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace g3
{
	public class DSubmesh3Set : IEnumerable<DSubmesh3>, IEnumerable
	{
		public DSubmesh3Set(DMesh3 mesh, IEnumerable<object> keys, Func<object, IEnumerable<int>> indexSetsF)
		{
			this.Mesh = mesh;
			this.TriangleSetKeys = keys;
			this.TriangleSetF = indexSetsF;
			this.ComputeSubMeshes();
		}

		public DSubmesh3Set(DMesh3 mesh, MeshConnectedComponents components)
		{
			this.Mesh = mesh;
			this.TriangleSetF = ((object idx) => components.Components[(int)idx].Indices);
			List<object> list = new List<object>();
			for (int i = 0; i < components.Count; i++)
			{
				list.Add(i);
			}
			this.TriangleSetKeys = list;
			this.ComputeSubMeshes();
		}

		public IEnumerator<DSubmesh3> GetEnumerator()
		{
			return this.Submeshes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Submeshes.GetEnumerator();
		}

		protected virtual void ComputeSubMeshes()
		{
			this.Submeshes = new List<DSubmesh3>();
			this.KeyToMesh = new Dictionary<object, DSubmesh3>();
			SpinLock data_lock = default(SpinLock);
			gParallel.ForEach<object>(this.TriangleSetKeys, delegate(object obj)
			{
				DSubmesh3 dsubmesh = new DSubmesh3(this.Mesh, this.TriangleSetF(obj), 0);
				bool flag = false;
				data_lock.Enter(ref flag);
				this.Submeshes.Add(dsubmesh);
				this.KeyToMesh[obj] = dsubmesh;
				data_lock.Exit();
			});
		}

		public DMesh3 Mesh;

		public IEnumerable<object> TriangleSetKeys;

		public Func<object, IEnumerable<int>> TriangleSetF;

		public List<DSubmesh3> Submeshes;

		public Dictionary<object, DSubmesh3> KeyToMesh;
	}
}
