using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	public class InputStateHistory<TValue> : InputStateHistory, IReadOnlyList<InputStateHistory<TValue>.Record>, IEnumerable<InputStateHistory<TValue>.Record>, IEnumerable, IReadOnlyCollection<InputStateHistory<TValue>.Record> where TValue : struct
	{
		public InputStateHistory(int? maxStateSizeInBytes = null) : base(maxStateSizeInBytes ?? UnsafeUtility.SizeOf<TValue>())
		{
			int? num = maxStateSizeInBytes;
			int num2 = UnsafeUtility.SizeOf<TValue>();
			if (num.GetValueOrDefault() < num2 & num != null)
			{
				throw new ArgumentException("Max state size cannot be smaller than sizeof(TValue)", "maxStateSizeInBytes");
			}
		}

		public InputStateHistory(InputControl<TValue> control) : base(control)
		{
		}

		public InputStateHistory(string path) : base(path)
		{
			foreach (InputControl inputControl in base.controls)
			{
				if (!typeof(TValue).IsAssignableFrom(inputControl.valueType))
				{
					throw new ArgumentException(string.Format("Control '{0}' matched by '{1}' has value type '{2}' which is incompatible with '{3}'", new object[]
					{
						inputControl,
						path,
						inputControl.valueType.GetNiceTypeName(),
						typeof(TValue).GetNiceTypeName()
					}));
				}
			}
		}

		~InputStateHistory()
		{
			base.Destroy();
		}

		public unsafe InputStateHistory<TValue>.Record AddRecord(InputStateHistory<TValue>.Record record)
		{
			int index;
			InputStateHistory.RecordHeader* header = base.AllocateRecord(out index);
			InputStateHistory<TValue>.Record result = new InputStateHistory<TValue>.Record(this, index, header);
			result.CopyFrom(record);
			return result;
		}

		public unsafe InputStateHistory<TValue>.Record RecordStateChange(InputControl<TValue> control, TValue value, double time = -1.0)
		{
			InputEventPtr inputEventPtr;
			InputStateHistory<TValue>.Record result;
			using (StateEvent.From(control.device, out inputEventPtr, Allocator.Temp))
			{
				byte* statePtr = (byte*)StateEvent.From(inputEventPtr)->state - control.device.stateBlock.byteOffset;
				control.WriteValueIntoState(value, (void*)statePtr);
				if (time >= 0.0)
				{
					inputEventPtr.time = time;
				}
				InputStateHistory.Record record = base.RecordStateChange(control, inputEventPtr);
				result = new InputStateHistory<TValue>.Record(this, record.recordIndex, record.header);
			}
			return result;
		}

		public new IEnumerator<InputStateHistory<TValue>.Record> GetEnumerator()
		{
			return new InputStateHistory<TValue>.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public InputStateHistory<TValue>.Record this[int index]
		{
			get
			{
				if (index < 0 || index >= base.Count)
				{
					throw new ArgumentOutOfRangeException(string.Format("Index {0} is out of range for history with {1} entries", index, base.Count), "index");
				}
				int index2 = base.UserIndexToRecordIndex(index);
				return new InputStateHistory<TValue>.Record(this, index2, base.GetRecord(index2));
			}
			set
			{
				if (index < 0 || index >= base.Count)
				{
					throw new ArgumentOutOfRangeException(string.Format("Index {0} is out of range for history with {1} entries", index, base.Count), "index");
				}
				int index2 = base.UserIndexToRecordIndex(index);
				new InputStateHistory<TValue>.Record(this, index2, base.GetRecord(index2)).CopyFrom(value);
			}
		}

		private struct Enumerator : IEnumerator<InputStateHistory<TValue>.Record>, IEnumerator, IDisposable
		{
			public Enumerator(InputStateHistory<TValue> history)
			{
				this.m_History = history;
				this.m_Index = -1;
			}

			public bool MoveNext()
			{
				if (this.m_Index + 1 >= this.m_History.Count)
				{
					return false;
				}
				this.m_Index++;
				return true;
			}

			public void Reset()
			{
				this.m_Index = -1;
			}

			public InputStateHistory<TValue>.Record Current
			{
				get
				{
					return this.m_History[this.m_Index];
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

			private readonly InputStateHistory<TValue> m_History;

			private int m_Index;
		}

		public new struct Record : IEquatable<InputStateHistory<TValue>.Record>
		{
			internal unsafe InputStateHistory.RecordHeader* header
			{
				get
				{
					return this.m_Owner.GetRecord(this.recordIndex);
				}
			}

			internal int recordIndex
			{
				get
				{
					return this.m_IndexPlusOne - 1;
				}
			}

			public unsafe bool valid
			{
				get
				{
					return this.m_Owner != null && this.m_IndexPlusOne != 0 && this.header->version == this.m_Version;
				}
			}

			public InputStateHistory<TValue> owner
			{
				get
				{
					return this.m_Owner;
				}
			}

			public int index
			{
				get
				{
					this.CheckValid();
					return this.m_Owner.RecordIndexToUserIndex(this.recordIndex);
				}
			}

			public unsafe double time
			{
				get
				{
					this.CheckValid();
					return this.header->time;
				}
			}

			public unsafe InputControl<TValue> control
			{
				get
				{
					this.CheckValid();
					ReadOnlyArray<InputControl> controls = this.m_Owner.controls;
					if (controls.Count == 1 && !this.m_Owner.m_AddNewControls)
					{
						return (InputControl<TValue>)controls[0];
					}
					return (InputControl<TValue>)controls[this.header->controlIndex];
				}
			}

			public InputStateHistory<TValue>.Record next
			{
				get
				{
					this.CheckValid();
					int num = this.m_Owner.RecordIndexToUserIndex(this.recordIndex);
					if (num + 1 >= this.m_Owner.Count)
					{
						return default(InputStateHistory<TValue>.Record);
					}
					int index = this.m_Owner.UserIndexToRecordIndex(num + 1);
					return new InputStateHistory<TValue>.Record(this.m_Owner, index, this.m_Owner.GetRecord(index));
				}
			}

			public InputStateHistory<TValue>.Record previous
			{
				get
				{
					this.CheckValid();
					int num = this.m_Owner.RecordIndexToUserIndex(this.recordIndex);
					if (num - 1 < 0)
					{
						return default(InputStateHistory<TValue>.Record);
					}
					int index = this.m_Owner.UserIndexToRecordIndex(num - 1);
					return new InputStateHistory<TValue>.Record(this.m_Owner, index, this.m_Owner.GetRecord(index));
				}
			}

			internal unsafe Record(InputStateHistory<TValue> owner, int index, InputStateHistory.RecordHeader* header)
			{
				this.m_Owner = owner;
				this.m_IndexPlusOne = index + 1;
				this.m_Version = header->version;
			}

			internal Record(InputStateHistory<TValue> owner, int index)
			{
				this.m_Owner = owner;
				this.m_IndexPlusOne = index + 1;
				this.m_Version = 0U;
			}

			public TValue ReadValue()
			{
				this.CheckValid();
				return this.m_Owner.ReadValue<TValue>(this.header);
			}

			public unsafe void* GetUnsafeMemoryPtr()
			{
				this.CheckValid();
				return this.GetUnsafeMemoryPtrUnchecked();
			}

			internal unsafe void* GetUnsafeMemoryPtrUnchecked()
			{
				if (this.m_Owner.controls.Count == 1 && !this.m_Owner.m_AddNewControls)
				{
					return (void*)this.header->statePtrWithoutControlIndex;
				}
				return (void*)this.header->statePtrWithControlIndex;
			}

			public unsafe void* GetUnsafeExtraMemoryPtr()
			{
				this.CheckValid();
				return this.GetUnsafeExtraMemoryPtrUnchecked();
			}

			internal unsafe void* GetUnsafeExtraMemoryPtrUnchecked()
			{
				if (this.m_Owner.extraMemoryPerRecord == 0)
				{
					throw new InvalidOperationException("No extra memory has been set up for history records; set extraMemoryPerRecord");
				}
				return (void*)(this.header + this.m_Owner.bytesPerRecord / sizeof(InputStateHistory.RecordHeader) - this.m_Owner.extraMemoryPerRecord / sizeof(InputStateHistory.RecordHeader));
			}

			public void CopyFrom(InputStateHistory<TValue>.Record record)
			{
				this.CheckValid();
				if (!record.valid)
				{
					throw new ArgumentException("Given history record is not valid", "record");
				}
				InputStateHistory.Record record2 = new InputStateHistory.Record(this.m_Owner, this.recordIndex, this.header);
				record2.CopyFrom(new InputStateHistory.Record(record.m_Owner, record.recordIndex, record.header));
				this.m_Version = record2.version;
			}

			private unsafe void CheckValid()
			{
				if (this.m_Owner == null || this.m_IndexPlusOne == 0)
				{
					throw new InvalidOperationException("Value not initialized");
				}
				if (this.header->version != this.m_Version)
				{
					throw new InvalidOperationException("Record is no longer valid");
				}
			}

			public bool Equals(InputStateHistory<TValue>.Record other)
			{
				return this.m_Owner == other.m_Owner && this.m_IndexPlusOne == other.m_IndexPlusOne && this.m_Version == other.m_Version;
			}

			public override bool Equals(object obj)
			{
				if (obj is InputStateHistory<TValue>.Record)
				{
					InputStateHistory<TValue>.Record other = (InputStateHistory<TValue>.Record)obj;
					return this.Equals(other);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return (((this.m_Owner != null) ? this.m_Owner.GetHashCode() : 0) * 397 ^ this.m_IndexPlusOne) * 397 ^ (int)this.m_Version;
			}

			public override string ToString()
			{
				if (!this.valid)
				{
					return "<Invalid>";
				}
				return string.Format("{{ control={0} value={1} time={2} }}", this.control, this.ReadValue(), this.time);
			}

			private readonly InputStateHistory<TValue> m_Owner;

			private readonly int m_IndexPlusOne;

			private uint m_Version;
		}
	}
}
