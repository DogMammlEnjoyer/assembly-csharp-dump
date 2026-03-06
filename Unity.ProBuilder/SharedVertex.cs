using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

namespace UnityEngine.ProBuilder
{
	[Serializable]
	public sealed class SharedVertex : ICollection<int>, IEnumerable<int>, IEnumerable
	{
		internal int[] arrayInternal
		{
			get
			{
				return this.m_Vertices;
			}
		}

		public SharedVertex(IEnumerable<int> indexes)
		{
			if (indexes == null)
			{
				throw new ArgumentNullException("indexes");
			}
			this.m_Vertices = indexes.ToArray<int>();
		}

		public SharedVertex(SharedVertex sharedVertex)
		{
			if (sharedVertex == null)
			{
				throw new ArgumentNullException("sharedVertex");
			}
			this.m_Vertices = new int[sharedVertex.Count];
			Array.Copy(sharedVertex.m_Vertices, this.m_Vertices, this.m_Vertices.Length);
		}

		public int this[int i]
		{
			get
			{
				return this.m_Vertices[i];
			}
			set
			{
				this.m_Vertices[i] = value;
			}
		}

		public IEnumerator<int> GetEnumerator()
		{
			return ((IEnumerable<int>)this.m_Vertices).GetEnumerator();
		}

		public override string ToString()
		{
			return this.m_Vertices.ToString(",");
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Add(int item)
		{
			this.m_Vertices = this.m_Vertices.Add(item);
		}

		public void Clear()
		{
			this.m_Vertices = new int[0];
		}

		public bool Contains(int item)
		{
			return Array.IndexOf<int>(this.m_Vertices, item) > -1;
		}

		public void CopyTo(int[] array, int arrayIndex)
		{
			this.m_Vertices.CopyTo(array, arrayIndex);
		}

		public bool Remove(int item)
		{
			if (Array.IndexOf<int>(this.m_Vertices, item) < 0)
			{
				return false;
			}
			this.m_Vertices = this.m_Vertices.RemoveAt(item);
			return true;
		}

		public int Count
		{
			get
			{
				return this.m_Vertices.Length;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.m_Vertices.IsReadOnly;
			}
		}

		public static void GetSharedVertexLookup(IList<SharedVertex> sharedVertices, Dictionary<int, int> lookup)
		{
			lookup.Clear();
			int i = 0;
			int count = sharedVertices.Count;
			while (i < count)
			{
				foreach (int key in sharedVertices[i])
				{
					if (!lookup.ContainsKey(key))
					{
						lookup.Add(key, i);
					}
				}
				i++;
			}
		}

		internal void ShiftIndexes(int offset)
		{
			int i = 0;
			int count = this.Count;
			while (i < count)
			{
				this.m_Vertices[i] += offset;
				i++;
			}
		}

		internal static SharedVertex[] ToSharedVertices(IEnumerable<KeyValuePair<int, int>> lookup)
		{
			if (lookup == null)
			{
				return new SharedVertex[0];
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			List<List<int>> list = new List<List<int>>();
			foreach (KeyValuePair<int, int> keyValuePair in lookup)
			{
				if (keyValuePair.Value < 0)
				{
					list.Add(new List<int>
					{
						keyValuePair.Key
					});
				}
				else
				{
					int index = -1;
					if (dictionary.TryGetValue(keyValuePair.Value, out index))
					{
						list[index].Add(keyValuePair.Key);
					}
					else
					{
						dictionary.Add(keyValuePair.Value, list.Count);
						list.Add(new List<int>
						{
							keyValuePair.Key
						});
					}
				}
			}
			return SharedVertex.ToSharedVertices(list);
		}

		private static SharedVertex[] ToSharedVertices(List<List<int>> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			SharedVertex[] array = new SharedVertex[list.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new SharedVertex(list[i]);
			}
			return array;
		}

		public static SharedVertex[] GetSharedVerticesWithPositions(IList<Vector3> positions)
		{
			if (positions == null)
			{
				throw new ArgumentNullException("positions");
			}
			Dictionary<IntVec3, List<int>> dictionary = new Dictionary<IntVec3, List<int>>();
			for (int i = 0; i < positions.Count; i++)
			{
				List<int> list;
				if (dictionary.TryGetValue(positions[i], out list))
				{
					list.Add(i);
				}
				else
				{
					dictionary.Add(new IntVec3(positions[i]), new List<int>
					{
						i
					});
				}
			}
			SharedVertex[] array = new SharedVertex[dictionary.Count];
			int num = 0;
			foreach (KeyValuePair<IntVec3, List<int>> keyValuePair in dictionary)
			{
				array[num++] = new SharedVertex(keyValuePair.Value.ToArray());
			}
			return array;
		}

		internal static SharedVertex[] RemoveAndShift(Dictionary<int, int> lookup, IEnumerable<int> remove)
		{
			List<int> list = new List<int>(remove);
			list.Sort();
			return SharedVertex.SortedRemoveAndShift(lookup, list);
		}

		internal static SharedVertex[] SortedRemoveAndShift(Dictionary<int, int> lookup, List<int> remove)
		{
			foreach (int key in remove)
			{
				lookup[key] = -1;
			}
			SharedVertex[] array = SharedVertex.ToSharedVertices(from x in lookup
			where x.Value > -1
			select x);
			int i = 0;
			int num = array.Length;
			while (i < num)
			{
				int j = 0;
				int count = array[i].Count;
				while (j < count)
				{
					int num2 = ArrayUtility.NearestIndexPriorToValue<int>(remove, array[i][j]);
					SharedVertex sharedVertex = array[i];
					int i2 = j;
					sharedVertex[i2] -= num2 + 1;
					j++;
				}
				i++;
			}
			return array;
		}

		internal static void SetCoincident(ref Dictionary<int, int> lookup, IEnumerable<int> vertices)
		{
			int count = lookup.Count;
			foreach (int key in vertices)
			{
				lookup[key] = count;
			}
		}

		[SerializeField]
		[FormerlySerializedAs("array")]
		[FormerlySerializedAs("m_Vertexes")]
		private int[] m_Vertices;
	}
}
