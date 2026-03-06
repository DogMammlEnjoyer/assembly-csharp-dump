using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	[DebuggerDisplay("Count = {Count}")]
	public struct InputControlList<TControl> : IList<TControl>, ICollection<TControl>, IEnumerable<TControl>, IEnumerable, IReadOnlyList<TControl>, IReadOnlyCollection<TControl>, IDisposable where TControl : InputControl
	{
		public int Count
		{
			get
			{
				return this.m_Count;
			}
		}

		public int Capacity
		{
			get
			{
				if (!this.m_Indices.IsCreated)
				{
					return 0;
				}
				return this.m_Indices.Length;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Capacity cannot be negative", "value");
				}
				if (value == 0)
				{
					if (this.m_Count != 0)
					{
						this.m_Indices.Dispose();
					}
					this.m_Count = 0;
					return;
				}
				Allocator allocator = (this.m_Allocator != Allocator.Invalid) ? this.m_Allocator : Allocator.Persistent;
				ArrayHelpers.Resize<ulong>(ref this.m_Indices, value, allocator);
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public TControl this[int index]
		{
			get
			{
				if (index < 0 || index >= this.m_Count)
				{
					throw new ArgumentOutOfRangeException("index", string.Format("Index {0} is out of range in list with {1} entries", index, this.m_Count));
				}
				return InputControlList<TControl>.FromIndex(this.m_Indices[index]);
			}
			set
			{
				if (index < 0 || index >= this.m_Count)
				{
					throw new ArgumentOutOfRangeException("index", string.Format("Index {0} is out of range in list with {1} entries", index, this.m_Count));
				}
				this.m_Indices[index] = InputControlList<TControl>.ToIndex(value);
			}
		}

		public InputControlList(Allocator allocator, int initialCapacity = 0)
		{
			this.m_Allocator = allocator;
			this.m_Indices = default(NativeArray<ulong>);
			this.m_Count = 0;
			if (initialCapacity != 0)
			{
				this.Capacity = initialCapacity;
			}
		}

		public InputControlList(IEnumerable<TControl> values, Allocator allocator = Allocator.Persistent)
		{
			this = new InputControlList<TControl>(allocator, 0);
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			foreach (TControl item in values)
			{
				this.Add(item);
			}
		}

		public InputControlList(params TControl[] values)
		{
			this = default(InputControlList<TControl>);
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			int num = values.Length;
			this.Capacity = Mathf.Max(num, 10);
			for (int i = 0; i < num; i++)
			{
				this.Add(values[i]);
			}
		}

		public unsafe void Resize(int size)
		{
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size", "Size cannot be negative");
			}
			if (this.Capacity < size)
			{
				this.Capacity = size;
			}
			if (size > this.Count)
			{
				UnsafeUtility.MemSet((void*)((byte*)this.m_Indices.GetUnsafePtr<ulong>() + this.Count * 8), byte.MaxValue, (long)(size - this.Count));
			}
			this.m_Count = size;
		}

		public void Add(TControl item)
		{
			ulong value = InputControlList<TControl>.ToIndex(item);
			Allocator allocator = (this.m_Allocator != Allocator.Invalid) ? this.m_Allocator : Allocator.Persistent;
			ArrayHelpers.AppendWithCapacity<ulong>(ref this.m_Indices, ref this.m_Count, value, 10, allocator);
		}

		public void AddSlice<TList>(TList list, int count = -1, int destinationIndex = -1, int sourceIndex = 0) where TList : IReadOnlyList<TControl>
		{
			if (count < 0)
			{
				count = list.Count;
			}
			if (destinationIndex < 0)
			{
				destinationIndex = this.Count;
			}
			if (count == 0)
			{
				return;
			}
			if (sourceIndex + count > list.Count)
			{
				throw new ArgumentOutOfRangeException("count", string.Format("Count of {0} elements starting at index {1} exceeds length of list of {2}", count, sourceIndex, list.Count));
			}
			if (this.Capacity < this.m_Count + count)
			{
				this.Capacity = Math.Max(this.m_Count + count, 10);
			}
			if (destinationIndex < this.Count)
			{
				NativeArray<ulong>.Copy(this.m_Indices, destinationIndex, this.m_Indices, destinationIndex + count, this.Count - destinationIndex);
			}
			for (int i = 0; i < count; i++)
			{
				this.m_Indices[destinationIndex + i] = InputControlList<TControl>.ToIndex(list[sourceIndex + i]);
			}
			this.m_Count += count;
		}

		public void AddRange(IEnumerable<TControl> list, int count = -1, int destinationIndex = -1)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (count < 0)
			{
				count = list.Count<TControl>();
			}
			if (destinationIndex < 0)
			{
				destinationIndex = this.Count;
			}
			if (count == 0)
			{
				return;
			}
			if (this.Capacity < this.m_Count + count)
			{
				this.Capacity = Math.Max(this.m_Count + count, 10);
			}
			if (destinationIndex < this.Count)
			{
				NativeArray<ulong>.Copy(this.m_Indices, destinationIndex, this.m_Indices, destinationIndex + count, this.Count - destinationIndex);
			}
			foreach (TControl control in list)
			{
				this.m_Indices[destinationIndex++] = InputControlList<TControl>.ToIndex(control);
				this.m_Count++;
				count--;
				if (count == 0)
				{
					break;
				}
			}
		}

		public bool Remove(TControl item)
		{
			if (this.m_Count == 0)
			{
				return false;
			}
			ulong num = InputControlList<TControl>.ToIndex(item);
			for (int i = 0; i < this.m_Count; i++)
			{
				if (this.m_Indices[i] == num)
				{
					ArrayHelpers.EraseAtWithCapacity<ulong>(this.m_Indices, ref this.m_Count, i);
					return true;
				}
			}
			return false;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this.m_Count)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Index {0} is out of range in list with {1} elements", index, this.m_Count));
			}
			ArrayHelpers.EraseAtWithCapacity<ulong>(this.m_Indices, ref this.m_Count, index);
		}

		public void CopyTo(TControl[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public int IndexOf(TControl item)
		{
			return this.IndexOf(item, 0, -1);
		}

		public unsafe int IndexOf(TControl item, int startIndex, int count = -1)
		{
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "startIndex cannot be negative");
			}
			if (this.m_Count == 0)
			{
				return -1;
			}
			if (count < 0)
			{
				count = Mathf.Max(this.m_Count - startIndex, 0);
			}
			if (startIndex + count > this.m_Count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			ulong num = InputControlList<TControl>.ToIndex(item);
			ulong* unsafeReadOnlyPtr = (ulong*)this.m_Indices.GetUnsafeReadOnlyPtr<ulong>();
			for (int i = 0; i < count; i++)
			{
				if (unsafeReadOnlyPtr[startIndex + i] == num)
				{
					return startIndex + i;
				}
			}
			return -1;
		}

		public void Insert(int index, TControl item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			this.m_Count = 0;
		}

		public bool Contains(TControl item)
		{
			return this.IndexOf(item) != -1;
		}

		public bool Contains(TControl item, int startIndex, int count = -1)
		{
			return this.IndexOf(item, startIndex, count) != -1;
		}

		public void SwapElements(int index1, int index2)
		{
			if (index1 < 0 || index1 >= this.m_Count)
			{
				throw new ArgumentOutOfRangeException("index1");
			}
			if (index2 < 0 || index2 >= this.m_Count)
			{
				throw new ArgumentOutOfRangeException("index2");
			}
			if (index1 != index2)
			{
				this.m_Indices.SwapElements(index1, index2);
			}
		}

		public void Sort<TCompare>(int startIndex, int count, TCompare comparer) where TCompare : IComparer<TControl>
		{
			if (startIndex < 0 || startIndex >= this.Count)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (startIndex + count >= this.Count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			for (int i = 1; i < count; i++)
			{
				int num = i;
				while (num > 0 && comparer.Compare(this[num - 1], this[num]) < 0)
				{
					this.SwapElements(num, num - 1);
					num--;
				}
			}
		}

		public TControl[] ToArray(bool dispose = false)
		{
			TControl[] array = new TControl[this.m_Count];
			for (int i = 0; i < this.m_Count; i++)
			{
				array[i] = this[i];
			}
			if (dispose)
			{
				this.Dispose();
			}
			return array;
		}

		internal void AppendTo(ref TControl[] array, ref int count)
		{
			for (int i = 0; i < this.m_Count; i++)
			{
				ArrayHelpers.AppendWithCapacity<TControl>(ref array, ref count, this[i], 10);
			}
		}

		public void Dispose()
		{
			if (this.m_Indices.IsCreated)
			{
				this.m_Indices.Dispose();
			}
		}

		public IEnumerator<TControl> GetEnumerator()
		{
			return new InputControlList<TControl>.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public override string ToString()
		{
			if (this.Count == 0)
			{
				return "()";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('(');
			for (int i = 0; i < this.Count; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(this[i]);
			}
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}

		private static ulong ToIndex(TControl control)
		{
			if (control == null)
			{
				return ulong.MaxValue;
			}
			InputDevice device = control.device;
			int deviceId = device.m_DeviceId;
			int num = (device != control) ? (device.m_ChildrenForEachControl.IndexOfReference(control, -1) + 1) : 0;
			ulong num2 = (ulong)((ulong)((long)deviceId) << 32);
			ulong num3 = (ulong)((long)num);
			return num2 | num3;
		}

		private static TControl FromIndex(ulong index)
		{
			if (index == 18446744073709551615UL)
			{
				return default(TControl);
			}
			int deviceId = (int)(index >> 32);
			int num = (int)(index & (ulong)-1);
			InputDevice deviceById = InputSystem.GetDeviceById(deviceId);
			if (deviceById == null)
			{
				return default(TControl);
			}
			if (num == 0)
			{
				return (TControl)((object)deviceById);
			}
			return (TControl)((object)deviceById.m_ChildrenForEachControl[num - 1]);
		}

		private int m_Count;

		private NativeArray<ulong> m_Indices;

		private readonly Allocator m_Allocator;

		private const ulong kInvalidIndex = 18446744073709551615UL;

		private struct Enumerator : IEnumerator<TControl>, IEnumerator, IDisposable
		{
			public unsafe Enumerator(InputControlList<TControl> list)
			{
				this.m_Count = list.m_Count;
				this.m_Current = -1;
				this.m_Indices = (ulong*)((this.m_Count > 0) ? list.m_Indices.GetUnsafeReadOnlyPtr<ulong>() : null);
			}

			public bool MoveNext()
			{
				if (this.m_Current >= this.m_Count)
				{
					return false;
				}
				this.m_Current++;
				return this.m_Current != this.m_Count;
			}

			public void Reset()
			{
				this.m_Current = -1;
			}

			public unsafe TControl Current
			{
				get
				{
					if (this.m_Indices == null)
					{
						throw new InvalidOperationException("Enumerator is not valid");
					}
					return InputControlList<TControl>.FromIndex(this.m_Indices[this.m_Current]);
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

			private unsafe readonly ulong* m_Indices;

			private readonly int m_Count;

			private int m_Current;
		}
	}
}
