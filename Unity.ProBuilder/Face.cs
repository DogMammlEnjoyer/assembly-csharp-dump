using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine.Serialization;

namespace UnityEngine.ProBuilder
{
	[Serializable]
	public sealed class Face
	{
		public bool manualUV
		{
			get
			{
				return this.m_ManualUV;
			}
			set
			{
				this.m_ManualUV = value;
			}
		}

		public int textureGroup
		{
			get
			{
				return this.m_TextureGroup;
			}
			set
			{
				this.m_TextureGroup = value;
			}
		}

		internal int[] indexesInternal
		{
			get
			{
				return this.m_Indexes;
			}
			set
			{
				if (this.m_Indexes == null)
				{
					throw new ArgumentNullException("value");
				}
				if (this.m_Indexes.Length % 3 != 0)
				{
					throw new ArgumentException("Face indexes must be a multiple of 3.");
				}
				this.m_Indexes = value;
				this.InvalidateCache();
			}
		}

		public ReadOnlyCollection<int> indexes
		{
			get
			{
				return new ReadOnlyCollection<int>(this.m_Indexes);
			}
		}

		public void SetIndexes(IEnumerable<int> indices)
		{
			if (indices == null)
			{
				throw new ArgumentNullException("indices");
			}
			int[] array = indices.ToArray<int>();
			if (array.Length % 3 != 0)
			{
				throw new ArgumentException("Face indexes must be a multiple of 3.");
			}
			this.m_Indexes = array;
			this.InvalidateCache();
		}

		internal int[] distinctIndexesInternal
		{
			get
			{
				if (this.m_DistinctIndexes != null)
				{
					return this.m_DistinctIndexes;
				}
				return this.CacheDistinctIndexes();
			}
		}

		public ReadOnlyCollection<int> distinctIndexes
		{
			get
			{
				return new ReadOnlyCollection<int>(this.distinctIndexesInternal);
			}
		}

		internal Edge[] edgesInternal
		{
			get
			{
				if (this.m_Edges != null)
				{
					return this.m_Edges;
				}
				return this.CacheEdges();
			}
		}

		public ReadOnlyCollection<Edge> edges
		{
			get
			{
				return new ReadOnlyCollection<Edge>(this.edgesInternal);
			}
		}

		public int smoothingGroup
		{
			get
			{
				return this.m_SmoothingGroup;
			}
			set
			{
				this.m_SmoothingGroup = value;
			}
		}

		[Obsolete("Face.material is deprecated. Please use submeshIndex instead.")]
		public Material material
		{
			get
			{
				return this.m_Material;
			}
			set
			{
				this.m_Material = value;
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

		public AutoUnwrapSettings uv
		{
			get
			{
				return this.m_Uv;
			}
			set
			{
				this.m_Uv = value;
			}
		}

		public int this[int i]
		{
			get
			{
				return this.indexesInternal[i];
			}
		}

		public Face()
		{
			this.m_SubmeshIndex = 0;
		}

		public Face(IEnumerable<int> indices)
		{
			this.SetIndexes(indices);
			this.m_Uv = AutoUnwrapSettings.tile;
			this.m_Material = BuiltinMaterials.defaultMaterial;
			this.m_SmoothingGroup = 0;
			this.m_SubmeshIndex = 0;
			this.textureGroup = -1;
			this.elementGroup = 0;
		}

		[Obsolete("Face.material is deprecated. Please use \"submeshIndex\" instead.")]
		internal Face(int[] triangles, Material m, AutoUnwrapSettings u, int smoothing, int texture, int element, bool manualUVs)
		{
			this.SetIndexes(triangles);
			this.m_Uv = new AutoUnwrapSettings(u);
			this.m_Material = m;
			this.m_SmoothingGroup = smoothing;
			this.textureGroup = texture;
			this.elementGroup = element;
			this.manualUV = manualUVs;
			this.m_SubmeshIndex = 0;
		}

		internal Face(IEnumerable<int> triangles, int submeshIndex, AutoUnwrapSettings u, int smoothing, int texture, int element, bool manualUVs)
		{
			this.SetIndexes(triangles);
			this.m_Uv = new AutoUnwrapSettings(u);
			this.m_SmoothingGroup = smoothing;
			this.textureGroup = texture;
			this.elementGroup = element;
			this.manualUV = manualUVs;
			this.m_SubmeshIndex = submeshIndex;
		}

		public Face(Face other)
		{
			this.CopyFrom(other);
		}

		public void CopyFrom(Face other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			int num = other.indexesInternal.Length;
			this.m_Indexes = new int[num];
			Array.Copy(other.indexesInternal, this.m_Indexes, num);
			this.m_SmoothingGroup = other.smoothingGroup;
			this.m_Uv = new AutoUnwrapSettings(other.uv);
			this.m_Material = other.material;
			this.manualUV = other.manualUV;
			this.m_TextureGroup = other.textureGroup;
			this.elementGroup = other.elementGroup;
			this.m_SubmeshIndex = other.m_SubmeshIndex;
			this.InvalidateCache();
		}

		internal void InvalidateCache()
		{
			this.m_Edges = null;
			this.m_DistinctIndexes = null;
		}

		private Edge[] CacheEdges()
		{
			if (this.m_Indexes == null)
			{
				return null;
			}
			HashSet<Edge> hashSet = new HashSet<Edge>();
			List<Edge> list = new List<Edge>();
			for (int i = 0; i < this.indexesInternal.Length; i += 3)
			{
				Edge item = new Edge(this.indexesInternal[i], this.indexesInternal[i + 1]);
				Edge item2 = new Edge(this.indexesInternal[i + 1], this.indexesInternal[i + 2]);
				Edge item3 = new Edge(this.indexesInternal[i + 2], this.indexesInternal[i]);
				if (!hashSet.Add(item))
				{
					list.Add(item);
				}
				if (!hashSet.Add(item2))
				{
					list.Add(item2);
				}
				if (!hashSet.Add(item3))
				{
					list.Add(item3);
				}
			}
			hashSet.ExceptWith(list);
			this.m_Edges = hashSet.ToArray<Edge>();
			return this.m_Edges;
		}

		private int[] CacheDistinctIndexes()
		{
			if (this.m_Indexes == null)
			{
				return null;
			}
			this.m_DistinctIndexes = this.m_Indexes.Distinct<int>().ToArray<int>();
			return this.distinctIndexesInternal;
		}

		public bool Contains(int a, int b, int c)
		{
			int i = 0;
			int num = this.indexesInternal.Length;
			while (i < num)
			{
				if (a == this.indexesInternal[i] && b == this.indexesInternal[i + 1] && c == this.indexesInternal[i + 2])
				{
					return true;
				}
				i += 3;
			}
			return false;
		}

		public bool IsQuad()
		{
			return this.edgesInternal != null && this.edgesInternal.Length == 4;
		}

		public int[] ToQuad()
		{
			if (!this.IsQuad())
			{
				throw new InvalidOperationException("Face is not representable as a quad. Use Face.IsQuad to check for validity.");
			}
			int[] array = new int[]
			{
				this.edgesInternal[0].a,
				this.edgesInternal[0].b,
				-1,
				-1
			};
			if (this.edgesInternal[1].a == array[1])
			{
				array[2] = this.edgesInternal[1].b;
			}
			else if (this.edgesInternal[2].a == array[1])
			{
				array[2] = this.edgesInternal[2].b;
			}
			else if (this.edgesInternal[3].a == array[1])
			{
				array[2] = this.edgesInternal[3].b;
			}
			if (this.edgesInternal[1].a == array[2])
			{
				array[3] = this.edgesInternal[1].b;
			}
			else if (this.edgesInternal[2].a == array[2])
			{
				array[3] = this.edgesInternal[2].b;
			}
			else if (this.edgesInternal[3].a == array[2])
			{
				array[3] = this.edgesInternal[3].b;
			}
			return array;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.indexesInternal.Length; i += 3)
			{
				stringBuilder.Append("[");
				stringBuilder.Append(this.indexesInternal[i]);
				stringBuilder.Append(", ");
				stringBuilder.Append(this.indexesInternal[i + 1]);
				stringBuilder.Append(", ");
				stringBuilder.Append(this.indexesInternal[i + 2]);
				stringBuilder.Append("]");
				if (i < this.indexesInternal.Length - 3)
				{
					stringBuilder.Append(", ");
				}
			}
			return stringBuilder.ToString();
		}

		public void ShiftIndexes(int offset)
		{
			int i = 0;
			int num = this.m_Indexes.Length;
			while (i < num)
			{
				this.m_Indexes[i] += offset;
				i++;
			}
			this.InvalidateCache();
		}

		private int SmallestIndexValue()
		{
			int num = this.m_Indexes[0];
			for (int i = 1; i < this.m_Indexes.Length; i++)
			{
				if (this.m_Indexes[i] < num)
				{
					num = this.m_Indexes[i];
				}
			}
			return num;
		}

		public void ShiftIndexesToZero()
		{
			int num = this.SmallestIndexValue();
			for (int i = 0; i < this.m_Indexes.Length; i++)
			{
				this.m_Indexes[i] -= num;
			}
			this.InvalidateCache();
		}

		public void Reverse()
		{
			Array.Reverse<int>(this.m_Indexes);
			this.InvalidateCache();
		}

		internal static void GetIndices(IEnumerable<Face> faces, List<int> indices)
		{
			indices.Clear();
			foreach (Face face in faces)
			{
				int i = 0;
				int num = face.indexesInternal.Length;
				while (i < num)
				{
					indices.Add(face.indexesInternal[i]);
					i++;
				}
			}
		}

		internal static void GetDistinctIndices(IEnumerable<Face> faces, List<int> indices)
		{
			indices.Clear();
			foreach (Face face in faces)
			{
				int i = 0;
				int num = face.distinctIndexesInternal.Length;
				while (i < num)
				{
					indices.Add(face.distinctIndexesInternal[i]);
					i++;
				}
			}
		}

		internal bool TryGetNextEdge(Edge source, int index, ref Edge nextEdge, ref int nextIndex)
		{
			int i = 0;
			int num = this.edgesInternal.Length;
			while (i < num)
			{
				if (!(this.edgesInternal[i] == source))
				{
					nextEdge = this.edgesInternal[i];
					if (nextEdge.Contains(index))
					{
						nextIndex = ((nextEdge.a == index) ? nextEdge.b : nextEdge.a);
						return true;
					}
				}
				i++;
			}
			return false;
		}

		[FormerlySerializedAs("_indices")]
		[SerializeField]
		private int[] m_Indexes;

		[SerializeField]
		[FormerlySerializedAs("_smoothingGroup")]
		private int m_SmoothingGroup;

		[SerializeField]
		[FormerlySerializedAs("_uv")]
		private AutoUnwrapSettings m_Uv;

		[SerializeField]
		[FormerlySerializedAs("_mat")]
		private Material m_Material;

		[SerializeField]
		private int m_SubmeshIndex;

		[SerializeField]
		[FormerlySerializedAs("manualUV")]
		private bool m_ManualUV;

		[SerializeField]
		internal int elementGroup;

		[SerializeField]
		private int m_TextureGroup;

		[NonSerialized]
		private int[] m_DistinctIndexes;

		[NonSerialized]
		private Edge[] m_Edges;
	}
}
