using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public class DSparseGrid3<ElemType> : IGrid3 where ElemType : class, IGridElement3
	{
		public DSparseGrid3(ElemType toDuplicate)
		{
			this.exemplar = toDuplicate;
			this.elements = new Dictionary<Vector3i, ElemType>();
			this.bounds = AxisAlignedBox3i.Empty;
		}

		public bool Has(Vector3i index)
		{
			return this.elements.ContainsKey(index);
		}

		public ElemType Get(Vector3i index, bool allocateIfMissing = true)
		{
			ElemType result;
			if (this.elements.TryGetValue(index, out result))
			{
				return result;
			}
			if (allocateIfMissing)
			{
				return this.allocate(index);
			}
			return default(ElemType);
		}

		public bool Free(Vector3i index)
		{
			if (this.elements.ContainsKey(index))
			{
				this.elements.Remove(index);
				return true;
			}
			return false;
		}

		public void FreeAll()
		{
			while (this.elements.Count > 0)
			{
				this.elements.Remove(this.elements.First<KeyValuePair<Vector3i, ElemType>>().Key);
			}
		}

		public int Count
		{
			get
			{
				return this.elements.Count;
			}
		}

		public double Density
		{
			get
			{
				return (double)this.elements.Count / (double)this.bounds.Volume;
			}
		}

		public AxisAlignedBox3i BoundsInclusive
		{
			get
			{
				return this.bounds;
			}
		}

		public Vector3i Dimensions
		{
			get
			{
				return this.bounds.Diagonal + Vector3i.One;
			}
		}

		public IEnumerable<Vector3i> AllocatedIndices()
		{
			foreach (KeyValuePair<Vector3i, ElemType> keyValuePair in this.elements)
			{
				yield return keyValuePair.Key;
			}
			Dictionary<Vector3i, ElemType>.Enumerator enumerator = default(Dictionary<Vector3i, ElemType>.Enumerator);
			yield break;
			yield break;
		}

		public IEnumerable<KeyValuePair<Vector3i, ElemType>> Allocated()
		{
			return this.elements;
		}

		private ElemType allocate(Vector3i index)
		{
			ElemType elemType = this.exemplar.CreateNewGridElement(false) as ElemType;
			this.elements.Add(index, elemType);
			this.bounds.Contain(index);
			return elemType;
		}

		private ElemType exemplar;

		private Dictionary<Vector3i, ElemType> elements;

		private AxisAlignedBox3i bounds;
	}
}
