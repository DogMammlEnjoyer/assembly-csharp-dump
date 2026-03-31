using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.InputSystem.Utilities
{
	internal struct InlinedArray<TValue> : IEnumerable<!0>, IEnumerable
	{
		public int Capacity
		{
			get
			{
				TValue[] array = this.additionalValues;
				if (array == null)
				{
					return 1;
				}
				return array.Length + 1;
			}
		}

		public InlinedArray(TValue value)
		{
			this.length = 1;
			this.firstValue = value;
			this.additionalValues = null;
		}

		public InlinedArray(TValue firstValue, params TValue[] additionalValues)
		{
			this.length = 1 + additionalValues.Length;
			this.firstValue = firstValue;
			this.additionalValues = additionalValues;
		}

		public InlinedArray(IEnumerable<TValue> values)
		{
			this = default(InlinedArray<TValue>);
			this.length = values.Count<TValue>();
			if (this.length > 1)
			{
				this.additionalValues = new TValue[this.length - 1];
			}
			else
			{
				this.additionalValues = null;
			}
			int num = 0;
			foreach (TValue tvalue in values)
			{
				if (num == 0)
				{
					this.firstValue = tvalue;
				}
				else
				{
					this.additionalValues[num - 1] = tvalue;
				}
				num++;
			}
		}

		public TValue this[int index]
		{
			get
			{
				if (index < 0 || index >= this.length)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				if (index == 0)
				{
					return this.firstValue;
				}
				return this.additionalValues[index - 1];
			}
			set
			{
				if (index < 0 || index >= this.length)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				if (index == 0)
				{
					this.firstValue = value;
					return;
				}
				this.additionalValues[index - 1] = value;
			}
		}

		public void Clear()
		{
			this.length = 0;
			this.firstValue = default(TValue);
			this.additionalValues = null;
		}

		public void ClearWithCapacity()
		{
			this.firstValue = default(TValue);
			for (int i = 0; i < this.length - 1; i++)
			{
				this.additionalValues[i] = default(TValue);
			}
			this.length = 0;
		}

		public InlinedArray<TValue> Clone()
		{
			return new InlinedArray<TValue>
			{
				length = this.length,
				firstValue = this.firstValue,
				additionalValues = ((this.additionalValues != null) ? ArrayHelpers.Copy<TValue>(this.additionalValues) : null)
			};
		}

		public void SetLength(int size)
		{
			if (size < this.length)
			{
				for (int i = size; i < this.length; i++)
				{
					this[i] = default(TValue);
				}
			}
			this.length = size;
			if (size > 1 && (this.additionalValues == null || this.additionalValues.Length < size - 1))
			{
				Array.Resize<TValue>(ref this.additionalValues, size - 1);
			}
		}

		public TValue[] ToArray()
		{
			return ArrayHelpers.Join<TValue>(this.firstValue, this.additionalValues);
		}

		public TOther[] ToArray<TOther>(Func<TValue, TOther> mapFunction)
		{
			if (this.length == 0)
			{
				return null;
			}
			TOther[] array = new TOther[this.length];
			for (int i = 0; i < this.length; i++)
			{
				array[i] = mapFunction(this[i]);
			}
			return array;
		}

		public int IndexOf(TValue value)
		{
			EqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
			if (this.length > 0)
			{
				if (@default.Equals(this.firstValue, value))
				{
					return 0;
				}
				if (this.additionalValues != null)
				{
					for (int i = 0; i < this.length - 1; i++)
					{
						if (@default.Equals(this.additionalValues[i], value))
						{
							return i + 1;
						}
					}
				}
			}
			return -1;
		}

		public int Append(TValue value)
		{
			if (this.length == 0)
			{
				this.firstValue = value;
			}
			else if (this.additionalValues == null)
			{
				this.additionalValues = new TValue[1];
				this.additionalValues[0] = value;
			}
			else
			{
				Array.Resize<TValue>(ref this.additionalValues, this.length);
				this.additionalValues[this.length - 1] = value;
			}
			int result = this.length;
			this.length++;
			return result;
		}

		public int AppendWithCapacity(TValue value, int capacityIncrement = 10)
		{
			if (this.length == 0)
			{
				this.firstValue = value;
			}
			else
			{
				int num = this.length - 1;
				ArrayHelpers.AppendWithCapacity<TValue>(ref this.additionalValues, ref num, value, capacityIncrement);
			}
			int result = this.length;
			this.length++;
			return result;
		}

		public void AssignWithCapacity(InlinedArray<TValue> values)
		{
			if (this.Capacity < values.length && values.length > 1)
			{
				this.additionalValues = new TValue[values.length - 1];
			}
			this.length = values.length;
			if (this.length > 0)
			{
				this.firstValue = values.firstValue;
			}
			if (this.length > 1)
			{
				Array.Copy(values.additionalValues, this.additionalValues, this.length - 1);
			}
		}

		public void Append(IEnumerable<TValue> values)
		{
			foreach (TValue value in values)
			{
				this.Append(value);
			}
		}

		public void Remove(TValue value)
		{
			if (this.length < 1)
			{
				return;
			}
			if (EqualityComparer<TValue>.Default.Equals(this.firstValue, value))
			{
				this.RemoveAt(0);
				return;
			}
			if (this.additionalValues != null)
			{
				for (int i = 0; i < this.length - 1; i++)
				{
					if (EqualityComparer<TValue>.Default.Equals(this.additionalValues[i], value))
					{
						this.RemoveAt(i + 1);
						return;
					}
				}
			}
		}

		public void RemoveAtWithCapacity(int index)
		{
			if (index < 0 || index >= this.length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (index == 0)
			{
				if (this.length == 1)
				{
					this.firstValue = default(TValue);
				}
				else if (this.length == 2)
				{
					this.firstValue = this.additionalValues[0];
					this.additionalValues[0] = default(TValue);
				}
				else
				{
					this.firstValue = this.additionalValues[0];
					int num = this.length - 1;
					this.additionalValues.EraseAtWithCapacity(ref num, 0);
				}
			}
			else
			{
				int num2 = this.length - 1;
				this.additionalValues.EraseAtWithCapacity(ref num2, index - 1);
			}
			this.length--;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this.length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (index == 0)
			{
				if (this.additionalValues != null)
				{
					this.firstValue = this.additionalValues[0];
					if (this.additionalValues.Length == 1)
					{
						this.additionalValues = null;
					}
					else
					{
						Array.Copy(this.additionalValues, 1, this.additionalValues, 0, this.additionalValues.Length - 1);
						Array.Resize<TValue>(ref this.additionalValues, this.additionalValues.Length - 1);
					}
				}
				else
				{
					this.firstValue = default(TValue);
				}
			}
			else
			{
				int num = this.length - 1;
				if (num == 1)
				{
					this.additionalValues = null;
				}
				else if (index == this.length - 1)
				{
					Array.Resize<TValue>(ref this.additionalValues, num - 1);
				}
				else
				{
					TValue[] destinationArray = new TValue[num - 1];
					if (index >= 2)
					{
						Array.Copy(this.additionalValues, 0, destinationArray, 0, index - 1);
					}
					Array.Copy(this.additionalValues, index + 1 - 1, destinationArray, index - 1, this.length - index - 1);
					this.additionalValues = destinationArray;
				}
			}
			this.length--;
		}

		public void RemoveAtByMovingTailWithCapacity(int index)
		{
			if (index < 0 || index >= this.length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			int num = this.length - 1;
			if (index == 0)
			{
				if (this.length > 1)
				{
					this.firstValue = this.additionalValues[num - 1];
					this.additionalValues[num - 1] = default(TValue);
				}
				else
				{
					this.firstValue = default(TValue);
				}
			}
			else
			{
				ArrayHelpers.EraseAtByMovingTail<TValue>(this.additionalValues, ref num, index - 1);
			}
			this.length--;
		}

		public bool RemoveByMovingTailWithCapacity(TValue value)
		{
			int num = this.IndexOf(value);
			if (num == -1)
			{
				return false;
			}
			this.RemoveAtByMovingTailWithCapacity(num);
			return true;
		}

		public bool Contains(TValue value, IEqualityComparer<TValue> comparer)
		{
			for (int i = 0; i < this.length; i++)
			{
				if (comparer.Equals(this[i], value))
				{
					return true;
				}
			}
			return false;
		}

		public void Merge(InlinedArray<TValue> other)
		{
			EqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
			for (int i = 0; i < other.length; i++)
			{
				TValue value = other[i];
				if (!this.Contains(value, @default))
				{
					this.Append(value);
				}
			}
		}

		public IEnumerator<TValue> GetEnumerator()
		{
			return new InlinedArray<TValue>.Enumerator
			{
				array = this,
				index = -1
			};
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public int length;

		public TValue firstValue;

		public TValue[] additionalValues;

		private struct Enumerator : IEnumerator<TValue>, IEnumerator, IDisposable
		{
			public bool MoveNext()
			{
				if (this.index >= this.array.length)
				{
					return false;
				}
				this.index++;
				return this.index < this.array.length;
			}

			public void Reset()
			{
				this.index = -1;
			}

			public TValue Current
			{
				get
				{
					return this.array[this.index];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Dispose()
			{
			}

			public InlinedArray<TValue> array;

			public int index;
		}
	}
}
