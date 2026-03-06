using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UnityEngine.ProBuilder
{
	[Serializable]
	public sealed class Submesh
	{
		public IEnumerable<int> indexes
		{
			get
			{
				return new ReadOnlyCollection<int>(this.m_Indexes);
			}
			set
			{
				this.m_Indexes = value.ToArray<int>();
			}
		}

		public MeshTopology topology
		{
			get
			{
				return this.m_Topology;
			}
			set
			{
				this.m_Topology = value;
			}
		}

		public int submeshIndex
		{
			get
			{
				return this.m_SubmeshIndex;
			}
			set
			{
				this.m_SubmeshIndex = value;
			}
		}

		public Submesh(int submeshIndex, MeshTopology topology, IEnumerable<int> indexes)
		{
			if (indexes == null)
			{
				throw new ArgumentNullException("indexes");
			}
			this.m_Indexes = indexes.ToArray<int>();
			this.m_Topology = topology;
			this.m_SubmeshIndex = submeshIndex;
		}

		public Submesh(Mesh mesh, int subMeshIndex)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			this.m_Indexes = mesh.GetIndices(subMeshIndex);
			this.m_Topology = mesh.GetTopology(subMeshIndex);
			this.m_SubmeshIndex = subMeshIndex;
		}

		public override string ToString()
		{
			return string.Format("{0}, {1}, {2}", this.m_SubmeshIndex, this.m_Topology.ToString(), (this.m_Indexes != null) ? this.m_Indexes.Length.ToString() : "0");
		}

		internal static int GetSubmeshCount(ProBuilderMesh mesh)
		{
			int num = 0;
			foreach (Face face in mesh.facesInternal)
			{
				num = Mathf.Max(num, face.submeshIndex);
			}
			return num + 1;
		}

		public static Submesh[] GetSubmeshes(IEnumerable<Face> faces, int submeshCount, MeshTopology preferredTopology = MeshTopology.Triangles)
		{
			if (preferredTopology != MeshTopology.Triangles && preferredTopology != MeshTopology.Quads)
			{
				throw new NotImplementedException("Currently only Quads and Triangles are supported.");
			}
			if (faces == null)
			{
				throw new ArgumentNullException("faces");
			}
			bool flag = preferredTopology == MeshTopology.Quads;
			List<int>[] array = flag ? new List<int>[submeshCount] : null;
			List<int>[] array2 = new List<int>[submeshCount];
			int upperBound = submeshCount - 1;
			int num = -1;
			for (int i = 0; i < submeshCount; i++)
			{
				if (flag)
				{
					array[i] = new List<int>();
				}
				array2[i] = new List<int>();
			}
			foreach (Face face in faces)
			{
				if (face.indexesInternal != null && face.indexesInternal.Length >= 1)
				{
					int num2 = Math.Clamp(face.submeshIndex, 0, upperBound);
					num = Mathf.Max(num2, num);
					if (flag && face.IsQuad())
					{
						array[num2].AddRange(face.ToQuad());
					}
					else
					{
						array2[num2].AddRange(face.indexesInternal);
					}
				}
			}
			submeshCount = num + 1;
			Submesh[] array3 = new Submesh[submeshCount];
			if (preferredTopology != MeshTopology.Triangles)
			{
				if (preferredTopology == MeshTopology.Quads)
				{
					for (int j = 0; j < submeshCount; j++)
					{
						if (array2[j].Count > 0)
						{
							List<int> list = array2[j];
							List<int> list2 = array[j];
							int count = list.Count;
							int count2 = list2.Count;
							int[] array4 = new int[count + count2 / 4 * 6];
							for (int k = 0; k < count; k++)
							{
								array4[k] = list[k];
							}
							int l = 0;
							int num3 = count;
							while (l < count2)
							{
								array4[num3] = list2[l];
								array4[num3 + 1] = list2[l + 1];
								array4[num3 + 2] = list2[l + 2];
								array4[num3 + 3] = list2[l + 2];
								array4[num3 + 4] = list2[l + 3];
								array4[num3 + 5] = list2[l];
								l += 4;
								num3 += 6;
							}
							array3[j] = new Submesh(j, MeshTopology.Triangles, array4);
						}
						else
						{
							array3[j] = new Submesh(j, MeshTopology.Quads, array[j]);
						}
					}
				}
			}
			else
			{
				for (int m = 0; m < submeshCount; m++)
				{
					array3[m] = new Submesh(m, MeshTopology.Triangles, array2[m]);
				}
			}
			return array3;
		}

		internal static void MapFaceMaterialsToSubmeshIndex(ProBuilderMesh mesh)
		{
			Material[] sharedMaterials = mesh.renderer.sharedMaterials;
			int num = sharedMaterials.Length;
			foreach (Face face in mesh.facesInternal)
			{
				if (!(face.material == null))
				{
					int value = Array.IndexOf<Material>(sharedMaterials, face.material);
					face.submeshIndex = Math.Clamp(value, 0, num - 1);
					face.material = null;
				}
			}
		}

		[SerializeField]
		internal int[] m_Indexes;

		[SerializeField]
		internal MeshTopology m_Topology;

		[SerializeField]
		internal int m_SubmeshIndex;
	}
}
