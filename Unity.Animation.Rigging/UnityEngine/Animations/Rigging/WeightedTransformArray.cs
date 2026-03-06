using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct WeightedTransformArray : IList<WeightedTransform>, ICollection<WeightedTransform>, IEnumerable<WeightedTransform>, IEnumerable, IList, ICollection
	{
		public WeightedTransformArray(int size)
		{
			this.m_Length = WeightedTransformArray.ClampSize(size);
			this.m_Item0 = default(WeightedTransform);
			this.m_Item1 = default(WeightedTransform);
			this.m_Item2 = default(WeightedTransform);
			this.m_Item3 = default(WeightedTransform);
			this.m_Item4 = default(WeightedTransform);
			this.m_Item5 = default(WeightedTransform);
			this.m_Item6 = default(WeightedTransform);
			this.m_Item7 = default(WeightedTransform);
		}

		public IEnumerator<WeightedTransform> GetEnumerator()
		{
			return new WeightedTransformArray.Enumerator(ref this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new WeightedTransformArray.Enumerator(ref this);
		}

		int IList.Add(object value)
		{
			this.Add((WeightedTransform)value);
			return this.m_Length - 1;
		}

		public void Add(WeightedTransform value)
		{
			if (this.m_Length >= WeightedTransformArray.k_MaxLength)
			{
				throw new ArgumentException(string.Format("This array cannot have more than '{0}' items.", WeightedTransformArray.k_MaxLength));
			}
			this.Set(this.m_Length, value);
			this.m_Length++;
		}

		public void Clear()
		{
			this.m_Length = 0;
		}

		int IList.IndexOf(object value)
		{
			return this.IndexOf((WeightedTransform)value);
		}

		public int IndexOf(WeightedTransform value)
		{
			for (int i = 0; i < this.m_Length; i++)
			{
				if (this.Get(i).Equals(value))
				{
					return i;
				}
			}
			return -1;
		}

		bool IList.Contains(object value)
		{
			return this.Contains((WeightedTransform)value);
		}

		public bool Contains(WeightedTransform value)
		{
			for (int i = 0; i < this.m_Length; i++)
			{
				if (this.Get(i).Equals(value))
				{
					return true;
				}
			}
			return false;
		}

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("The array cannot be null.");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
			}
			if (this.Count > array.Length - arrayIndex + 1)
			{
				throw new ArgumentException("The destination array has fewer elements than the collection.");
			}
			for (int i = 0; i < this.m_Length; i++)
			{
				array.SetValue(this.Get(i), i + arrayIndex);
			}
		}

		public void CopyTo(WeightedTransform[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("The array cannot be null.");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
			}
			if (this.Count > array.Length - arrayIndex + 1)
			{
				throw new ArgumentException("The destination array has fewer elements than the collection.");
			}
			for (int i = 0; i < this.m_Length; i++)
			{
				array[i + arrayIndex] = this.Get(i);
			}
		}

		void IList.Remove(object value)
		{
			this.Remove((WeightedTransform)value);
		}

		public bool Remove(WeightedTransform value)
		{
			for (int i = 0; i < this.m_Length; i++)
			{
				if (this.Get(i).Equals(value))
				{
					while (i < this.m_Length - 1)
					{
						this.Set(i, this.Get(i + 1));
						i++;
					}
					this.m_Length--;
					return true;
				}
			}
			return false;
		}

		public void RemoveAt(int index)
		{
			this.CheckOutOfRangeIndex(index);
			for (int i = index; i < this.m_Length - 1; i++)
			{
				this.Set(i, this.Get(i + 1));
			}
			this.m_Length--;
		}

		void IList.Insert(int index, object value)
		{
			this.Insert(index, (WeightedTransform)value);
		}

		public void Insert(int index, WeightedTransform value)
		{
			if (this.m_Length >= WeightedTransformArray.k_MaxLength)
			{
				throw new ArgumentException(string.Format("This array cannot have more than '{0}' items.", WeightedTransformArray.k_MaxLength));
			}
			this.CheckOutOfRangeIndex(index);
			if (index >= this.m_Length)
			{
				this.Add(value);
				return;
			}
			for (int i = this.m_Length; i > index; i--)
			{
				this.Set(i, this.Get(i - 1));
			}
			this.Set(index, value);
			this.m_Length++;
		}

		private static int ClampSize(int size)
		{
			return Mathf.Clamp(size, 0, WeightedTransformArray.k_MaxLength);
		}

		private void CheckOutOfRangeIndex(int index)
		{
			if (index < 0 || index >= WeightedTransformArray.k_MaxLength)
			{
				throw new IndexOutOfRangeException(string.Format("Index {0} is out of range of '{1}' Length.", index, this.m_Length));
			}
		}

		private WeightedTransform Get(int index)
		{
			this.CheckOutOfRangeIndex(index);
			switch (index)
			{
			case 0:
				return this.m_Item0;
			case 1:
				return this.m_Item1;
			case 2:
				return this.m_Item2;
			case 3:
				return this.m_Item3;
			case 4:
				return this.m_Item4;
			case 5:
				return this.m_Item5;
			case 6:
				return this.m_Item6;
			case 7:
				return this.m_Item7;
			default:
				return this.m_Item0;
			}
		}

		private void Set(int index, WeightedTransform value)
		{
			this.CheckOutOfRangeIndex(index);
			switch (index)
			{
			case 0:
				this.m_Item0 = value;
				return;
			case 1:
				this.m_Item1 = value;
				return;
			case 2:
				this.m_Item2 = value;
				return;
			case 3:
				this.m_Item3 = value;
				return;
			case 4:
				this.m_Item4 = value;
				return;
			case 5:
				this.m_Item5 = value;
				return;
			case 6:
				this.m_Item6 = value;
				return;
			case 7:
				this.m_Item7 = value;
				return;
			default:
				return;
			}
		}

		public void SetWeight(int index, float weight)
		{
			WeightedTransform value = this.Get(index);
			value.weight = weight;
			this.Set(index, value);
		}

		public float GetWeight(int index)
		{
			return this.Get(index).weight;
		}

		public void SetTransform(int index, Transform transform)
		{
			WeightedTransform value = this.Get(index);
			value.transform = transform;
			this.Set(index, value);
		}

		public Transform GetTransform(int index)
		{
			return this.Get(index).transform;
		}

		public static void OnValidate(ref WeightedTransformArray array, float min = 0f, float max = 1f)
		{
			for (int i = 0; i < WeightedTransformArray.k_MaxLength; i++)
			{
				array.SetWeight(i, Mathf.Clamp(array.GetWeight(i), min, max));
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this.Get(index);
			}
			set
			{
				this.Set(index, (WeightedTransform)value);
			}
		}

		public WeightedTransform this[int index]
		{
			get
			{
				return this.Get(index);
			}
			set
			{
				this.Set(index, value);
			}
		}

		public int Count
		{
			get
			{
				return this.m_Length;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return true;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return null;
			}
		}

		public static readonly int k_MaxLength = 8;

		[SerializeField]
		[NotKeyable]
		private int m_Length;

		[SerializeField]
		private WeightedTransform m_Item0;

		[SerializeField]
		private WeightedTransform m_Item1;

		[SerializeField]
		private WeightedTransform m_Item2;

		[SerializeField]
		private WeightedTransform m_Item3;

		[SerializeField]
		private WeightedTransform m_Item4;

		[SerializeField]
		private WeightedTransform m_Item5;

		[SerializeField]
		private WeightedTransform m_Item6;

		[SerializeField]
		private WeightedTransform m_Item7;

		[Serializable]
		private struct Enumerator : IEnumerator<WeightedTransform>, IEnumerator, IDisposable
		{
			public Enumerator(ref WeightedTransformArray array)
			{
				this.m_Array = array;
				this.m_Index = -1;
			}

			public bool MoveNext()
			{
				this.m_Index++;
				return this.m_Index < this.m_Array.Count;
			}

			public void Reset()
			{
				this.m_Index = -1;
			}

			void IDisposable.Dispose()
			{
			}

			public WeightedTransform Current
			{
				get
				{
					return this.m_Array.Get(this.m_Index);
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			private WeightedTransformArray m_Array;

			private int m_Index;
		}
	}
}
