using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	public class InputStateHistory : IDisposable, IEnumerable<InputStateHistory.Record>, IEnumerable, IInputStateChangeMonitor
	{
		public int Count
		{
			get
			{
				return this.m_RecordCount;
			}
		}

		public uint version
		{
			get
			{
				return this.m_CurrentVersion;
			}
		}

		public int historyDepth
		{
			get
			{
				return this.m_HistoryDepth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("History depth cannot be negative", "value");
				}
				if (this.m_RecordBuffer.IsCreated)
				{
					throw new NotImplementedException();
				}
				this.m_HistoryDepth = value;
			}
		}

		public int extraMemoryPerRecord
		{
			get
			{
				return this.m_ExtraMemoryPerRecord;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Memory size cannot be negative", "value");
				}
				if (this.m_RecordBuffer.IsCreated)
				{
					throw new NotImplementedException();
				}
				this.m_ExtraMemoryPerRecord = value;
			}
		}

		public InputUpdateType updateMask
		{
			get
			{
				InputUpdateType? updateMask = this.m_UpdateMask;
				if (updateMask == null)
				{
					return InputSystem.s_Manager.updateMask & ~InputUpdateType.Editor;
				}
				return updateMask.GetValueOrDefault();
			}
			set
			{
				if (value == InputUpdateType.None)
				{
					throw new ArgumentException("'InputUpdateType.None' is not a valid update mask", "value");
				}
				this.m_UpdateMask = new InputUpdateType?(value);
			}
		}

		public ReadOnlyArray<InputControl> controls
		{
			get
			{
				return new ReadOnlyArray<InputControl>(this.m_Controls, 0, this.m_ControlCount);
			}
		}

		public InputStateHistory.Record this[int index]
		{
			get
			{
				if (index < 0 || index >= this.m_RecordCount)
				{
					throw new ArgumentOutOfRangeException(string.Format("Index {0} is out of range for history with {1} entries", index, this.m_RecordCount), "index");
				}
				int index2 = this.UserIndexToRecordIndex(index);
				return new InputStateHistory.Record(this, index2, this.GetRecord(index2));
			}
			set
			{
				if (index < 0 || index >= this.m_RecordCount)
				{
					throw new ArgumentOutOfRangeException(string.Format("Index {0} is out of range for history with {1} entries", index, this.m_RecordCount), "index");
				}
				int index2 = this.UserIndexToRecordIndex(index);
				new InputStateHistory.Record(this, index2, this.GetRecord(index2)).CopyFrom(value);
			}
		}

		public Action<InputStateHistory.Record> onRecordAdded { get; set; }

		public Func<InputControl, double, InputEventPtr, bool> onShouldRecordStateChange { get; set; }

		public InputStateHistory(int maxStateSizeInBytes)
		{
			if (maxStateSizeInBytes <= 0)
			{
				throw new ArgumentException("State size must be >= 0", "maxStateSizeInBytes");
			}
			this.m_AddNewControls = true;
			this.m_StateSizeInBytes = maxStateSizeInBytes.AlignToMultipleOf(4);
		}

		public InputStateHistory(string path)
		{
			using (InputControlList<InputControl> inputControlList = InputSystem.FindControls(path))
			{
				this.m_Controls = inputControlList.ToArray(false);
				this.m_ControlCount = this.m_Controls.Length;
			}
		}

		public InputStateHistory(InputControl control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			this.m_Controls = new InputControl[]
			{
				control
			};
			this.m_ControlCount = 1;
		}

		public InputStateHistory(IEnumerable<InputControl> controls)
		{
			if (controls != null)
			{
				this.m_Controls = controls.ToArray<InputControl>();
				this.m_ControlCount = this.m_Controls.Length;
			}
		}

		~InputStateHistory()
		{
			this.Dispose();
		}

		public void Clear()
		{
			this.m_HeadIndex = 0;
			this.m_RecordCount = 0;
			this.m_CurrentVersion += 1U;
		}

		public unsafe InputStateHistory.Record AddRecord(InputStateHistory.Record record)
		{
			int index;
			InputStateHistory.RecordHeader* header = this.AllocateRecord(out index);
			InputStateHistory.Record result = new InputStateHistory.Record(this, index, header);
			result.CopyFrom(record);
			return result;
		}

		public void StartRecording()
		{
			foreach (InputControl control in this.controls)
			{
				InputState.AddChangeMonitor(control, this, -1L, 0U);
			}
		}

		public void StopRecording()
		{
			foreach (InputControl control in this.controls)
			{
				InputState.RemoveChangeMonitor(control, this, -1L);
			}
		}

		public unsafe InputStateHistory.Record RecordStateChange(InputControl control, InputEventPtr eventPtr)
		{
			if (eventPtr.IsA<DeltaStateEvent>())
			{
				throw new NotImplementedException();
			}
			if (!eventPtr.IsA<StateEvent>())
			{
				throw new ArgumentException(string.Format("Event must be a state event but is '{0}' instead", eventPtr), "eventPtr");
			}
			byte* statePtr = (byte*)StateEvent.From(eventPtr)->state - control.device.stateBlock.byteOffset;
			return this.RecordStateChange(control, (void*)statePtr, eventPtr.time);
		}

		public unsafe InputStateHistory.Record RecordStateChange(InputControl control, void* statePtr, double time)
		{
			int num = this.m_Controls.IndexOfReference(control, this.m_ControlCount);
			if (num == -1)
			{
				if (!this.m_AddNewControls)
				{
					throw new ArgumentException(string.Format("Control '{0}' is not part of InputStateHistory", control), "control");
				}
				if ((ulong)control.stateBlock.alignedSizeInBytes > (ulong)((long)this.m_StateSizeInBytes))
				{
					throw new InvalidOperationException(string.Format("Cannot add control '{0}' with state larger than {1} bytes", control, this.m_StateSizeInBytes));
				}
				num = ArrayHelpers.AppendWithCapacity<InputControl>(ref this.m_Controls, ref this.m_ControlCount, control, 10);
			}
			int index;
			InputStateHistory.RecordHeader* ptr = this.AllocateRecord(out index);
			ptr->time = time;
			ref InputStateHistory.RecordHeader ptr2 = ref *ptr;
			uint num2 = this.m_CurrentVersion + 1U;
			this.m_CurrentVersion = num2;
			ptr2.version = num2;
			byte* destination = ptr->statePtrWithoutControlIndex;
			if (this.m_ControlCount > 1 || this.m_AddNewControls)
			{
				ptr->controlIndex = num;
				destination = ptr->statePtrWithControlIndex;
			}
			uint alignedSizeInBytes = control.stateBlock.alignedSizeInBytes;
			uint byteOffset = control.stateBlock.byteOffset;
			UnsafeUtility.MemCpy((void*)destination, (void*)((byte*)statePtr + byteOffset), (long)((ulong)alignedSizeInBytes));
			InputStateHistory.Record record = new InputStateHistory.Record(this, index, ptr);
			Action<InputStateHistory.Record> onRecordAdded = this.onRecordAdded;
			if (onRecordAdded != null)
			{
				onRecordAdded(record);
			}
			return record;
		}

		public IEnumerator<InputStateHistory.Record> GetEnumerator()
		{
			return new InputStateHistory.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Dispose()
		{
			this.StopRecording();
			this.Destroy();
			GC.SuppressFinalize(this);
		}

		protected void Destroy()
		{
			if (this.m_RecordBuffer.IsCreated)
			{
				this.m_RecordBuffer.Dispose();
				this.m_RecordBuffer = default(NativeArray<byte>);
			}
		}

		private void Allocate()
		{
			if (!this.m_AddNewControls)
			{
				this.m_StateSizeInBytes = 0;
				foreach (InputControl inputControl in this.controls)
				{
					this.m_StateSizeInBytes = (int)Math.Max((uint)this.m_StateSizeInBytes, inputControl.stateBlock.alignedSizeInBytes);
				}
			}
			int length = this.bytesPerRecord * this.m_HistoryDepth;
			this.m_RecordBuffer = new NativeArray<byte>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		}

		protected internal int RecordIndexToUserIndex(int index)
		{
			if (index < this.m_HeadIndex)
			{
				return this.m_HistoryDepth - this.m_HeadIndex + index;
			}
			return index - this.m_HeadIndex;
		}

		protected internal int UserIndexToRecordIndex(int index)
		{
			return (this.m_HeadIndex + index) % this.m_HistoryDepth;
		}

		protected internal unsafe InputStateHistory.RecordHeader* GetRecord(int index)
		{
			if (!this.m_RecordBuffer.IsCreated)
			{
				throw new InvalidOperationException("History buffer has been disposed");
			}
			if (index < 0 || index >= this.m_HistoryDepth)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return this.GetRecordUnchecked(index);
		}

		internal unsafe InputStateHistory.RecordHeader* GetRecordUnchecked(int index)
		{
			return (InputStateHistory.RecordHeader*)((byte*)this.m_RecordBuffer.GetUnsafePtr<byte>() + index * this.bytesPerRecord);
		}

		protected internal unsafe InputStateHistory.RecordHeader* AllocateRecord(out int index)
		{
			if (!this.m_RecordBuffer.IsCreated)
			{
				this.Allocate();
			}
			index = (this.m_HeadIndex + this.m_RecordCount) % this.m_HistoryDepth;
			if (this.m_RecordCount == this.m_HistoryDepth)
			{
				this.m_HeadIndex = (this.m_HeadIndex + 1) % this.m_HistoryDepth;
			}
			else
			{
				this.m_RecordCount++;
			}
			return (InputStateHistory.RecordHeader*)((byte*)this.m_RecordBuffer.GetUnsafePtr<byte>() + this.bytesPerRecord * index);
		}

		protected unsafe TValue ReadValue<TValue>(InputStateHistory.RecordHeader* data) where TValue : struct
		{
			bool flag = this.m_ControlCount == 1 && !this.m_AddNewControls;
			InputControl inputControl = flag ? this.controls[0] : this.controls[data->controlIndex];
			InputControl<TValue> inputControl2 = inputControl as InputControl<TValue>;
			if (inputControl2 == null)
			{
				throw new InvalidOperationException(string.Format("Cannot read value of type '{0}' from control '{1}' with value type '{2}'", typeof(TValue).GetNiceTypeName(), inputControl, inputControl.valueType.GetNiceTypeName()));
			}
			byte* ptr = flag ? data->statePtrWithoutControlIndex : data->statePtrWithControlIndex;
			ptr -= inputControl.stateBlock.byteOffset;
			return inputControl2.ReadValueFromState((void*)ptr);
		}

		protected unsafe object ReadValueAsObject(InputStateHistory.RecordHeader* data)
		{
			bool flag = this.m_ControlCount == 1 && !this.m_AddNewControls;
			InputControl inputControl = flag ? this.controls[0] : this.controls[data->controlIndex];
			byte* ptr = flag ? data->statePtrWithoutControlIndex : data->statePtrWithControlIndex;
			ptr -= inputControl.stateBlock.byteOffset;
			return inputControl.ReadValueFromStateAsObject((void*)ptr);
		}

		void IInputStateChangeMonitor.NotifyControlStateChanged(InputControl control, double time, InputEventPtr eventPtr, long monitorIndex)
		{
			bool currentUpdateType = InputState.currentUpdateType != InputUpdateType.None;
			InputUpdateType updateMask = this.updateMask;
			if (((currentUpdateType ? InputUpdateType.Dynamic : InputUpdateType.None) & updateMask) == InputUpdateType.None)
			{
				return;
			}
			if (this.onShouldRecordStateChange != null && !this.onShouldRecordStateChange(control, time, eventPtr))
			{
				return;
			}
			this.RecordStateChange(control, control.currentStatePtr, time);
		}

		void IInputStateChangeMonitor.NotifyTimerExpired(InputControl control, double time, long monitorIndex, int timerIndex)
		{
		}

		internal int bytesPerRecord
		{
			get
			{
				return (this.m_StateSizeInBytes + this.m_ExtraMemoryPerRecord + ((this.m_ControlCount == 1 && !this.m_AddNewControls) ? 12 : 16)).AlignToMultipleOf(4);
			}
		}

		private const int kDefaultHistorySize = 128;

		internal InputControl[] m_Controls;

		internal int m_ControlCount;

		private NativeArray<byte> m_RecordBuffer;

		private int m_StateSizeInBytes;

		private int m_RecordCount;

		private int m_HistoryDepth = 128;

		private int m_ExtraMemoryPerRecord;

		internal int m_HeadIndex;

		internal uint m_CurrentVersion;

		private InputUpdateType? m_UpdateMask;

		internal readonly bool m_AddNewControls;

		private struct Enumerator : IEnumerator<InputStateHistory.Record>, IEnumerator, IDisposable
		{
			public Enumerator(InputStateHistory history)
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

			public InputStateHistory.Record Current
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

			private readonly InputStateHistory m_History;

			private int m_Index;
		}

		[StructLayout(LayoutKind.Explicit)]
		protected internal struct RecordHeader
		{
			public unsafe byte* statePtrWithControlIndex
			{
				get
				{
					fixed (byte* ptr = &this.m_StateWithControlIndex.FixedElementField)
					{
						return ptr;
					}
				}
			}

			public unsafe byte* statePtrWithoutControlIndex
			{
				get
				{
					fixed (byte* ptr = &this.m_StateWithoutControlIndex.FixedElementField)
					{
						return ptr;
					}
				}
			}

			[FieldOffset(0)]
			public double time;

			[FieldOffset(8)]
			public uint version;

			[FieldOffset(12)]
			public int controlIndex;

			[FixedBuffer(typeof(byte), 1)]
			[FieldOffset(12)]
			private InputStateHistory.RecordHeader.<m_StateWithoutControlIndex>e__FixedBuffer m_StateWithoutControlIndex;

			[FixedBuffer(typeof(byte), 1)]
			[FieldOffset(16)]
			private InputStateHistory.RecordHeader.<m_StateWithControlIndex>e__FixedBuffer m_StateWithControlIndex;

			public const int kSizeWithControlIndex = 16;

			public const int kSizeWithoutControlIndex = 12;

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 1)]
			public struct <m_StateWithControlIndex>e__FixedBuffer
			{
				public byte FixedElementField;
			}

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 1)]
			public struct <m_StateWithoutControlIndex>e__FixedBuffer
			{
				public byte FixedElementField;
			}
		}

		public struct Record : IEquatable<InputStateHistory.Record>
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

			internal uint version
			{
				get
				{
					return this.m_Version;
				}
			}

			public unsafe bool valid
			{
				get
				{
					return this.m_Owner != null && this.m_IndexPlusOne != 0 && this.header->version == this.m_Version;
				}
			}

			public InputStateHistory owner
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

			public unsafe InputControl control
			{
				get
				{
					this.CheckValid();
					ReadOnlyArray<InputControl> controls = this.m_Owner.controls;
					if (controls.Count == 1 && !this.m_Owner.m_AddNewControls)
					{
						return controls[0];
					}
					return controls[this.header->controlIndex];
				}
			}

			public InputStateHistory.Record next
			{
				get
				{
					this.CheckValid();
					int num = this.m_Owner.RecordIndexToUserIndex(this.recordIndex);
					if (num + 1 >= this.m_Owner.Count)
					{
						return default(InputStateHistory.Record);
					}
					int index = this.m_Owner.UserIndexToRecordIndex(num + 1);
					return new InputStateHistory.Record(this.m_Owner, index, this.m_Owner.GetRecord(index));
				}
			}

			public InputStateHistory.Record previous
			{
				get
				{
					this.CheckValid();
					int num = this.m_Owner.RecordIndexToUserIndex(this.recordIndex);
					if (num - 1 < 0)
					{
						return default(InputStateHistory.Record);
					}
					int index = this.m_Owner.UserIndexToRecordIndex(num - 1);
					return new InputStateHistory.Record(this.m_Owner, index, this.m_Owner.GetRecord(index));
				}
			}

			internal unsafe Record(InputStateHistory owner, int index, InputStateHistory.RecordHeader* header)
			{
				this.m_Owner = owner;
				this.m_IndexPlusOne = index + 1;
				this.m_Version = header->version;
			}

			public TValue ReadValue<TValue>() where TValue : struct
			{
				this.CheckValid();
				return this.m_Owner.ReadValue<TValue>(this.header);
			}

			public object ReadValueAsObject()
			{
				this.CheckValid();
				return this.m_Owner.ReadValueAsObject(this.header);
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

			public unsafe void CopyFrom(InputStateHistory.Record record)
			{
				if (!record.valid)
				{
					throw new ArgumentException("Given history record is not valid", "record");
				}
				this.CheckValid();
				InputControl control = record.control;
				int num = this.m_Owner.controls.IndexOfReference(control);
				if (num == -1)
				{
					if (!this.m_Owner.m_AddNewControls)
					{
						throw new InvalidOperationException(string.Format("Control '{0}' is not tracked by target history", record.control));
					}
					num = ArrayHelpers.AppendWithCapacity<InputControl>(ref this.m_Owner.m_Controls, ref this.m_Owner.m_ControlCount, control, 10);
				}
				int stateSizeInBytes = this.m_Owner.m_StateSizeInBytes;
				if (stateSizeInBytes != record.m_Owner.m_StateSizeInBytes)
				{
					throw new InvalidOperationException(string.Format("Cannot copy record from owner with state size '{0}' to owner with state size '{1}'", record.m_Owner.m_StateSizeInBytes, stateSizeInBytes));
				}
				InputStateHistory.RecordHeader* header = this.header;
				InputStateHistory.RecordHeader* header2 = record.header;
				UnsafeUtility.MemCpy((void*)header, (void*)header2, 12L);
				ref InputStateHistory.RecordHeader ptr = ref *header;
				InputStateHistory owner = this.m_Owner;
				uint num2 = owner.m_CurrentVersion + 1U;
				owner.m_CurrentVersion = num2;
				ptr.version = num2;
				this.m_Version = header->version;
				byte* destination = header->statePtrWithoutControlIndex;
				if (this.m_Owner.controls.Count > 1 || this.m_Owner.m_AddNewControls)
				{
					header->controlIndex = num;
					destination = header->statePtrWithControlIndex;
				}
				byte* source = (record.m_Owner.m_ControlCount > 1 || record.m_Owner.m_AddNewControls) ? header2->statePtrWithControlIndex : header2->statePtrWithoutControlIndex;
				UnsafeUtility.MemCpy((void*)destination, (void*)source, (long)stateSizeInBytes);
				int extraMemoryPerRecord = this.m_Owner.m_ExtraMemoryPerRecord;
				if (extraMemoryPerRecord > 0 && extraMemoryPerRecord == record.m_Owner.m_ExtraMemoryPerRecord)
				{
					UnsafeUtility.MemCpy(this.GetUnsafeExtraMemoryPtr(), record.GetUnsafeExtraMemoryPtr(), (long)extraMemoryPerRecord);
				}
				Action<InputStateHistory.Record> onRecordAdded = this.m_Owner.onRecordAdded;
				if (onRecordAdded == null)
				{
					return;
				}
				onRecordAdded(this);
			}

			internal unsafe void CheckValid()
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

			public bool Equals(InputStateHistory.Record other)
			{
				return this.m_Owner == other.m_Owner && this.m_IndexPlusOne == other.m_IndexPlusOne && this.m_Version == other.m_Version;
			}

			public override bool Equals(object obj)
			{
				if (obj is InputStateHistory.Record)
				{
					InputStateHistory.Record other = (InputStateHistory.Record)obj;
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
				return string.Format("{{ control={0} value={1} time={2} }}", this.control, this.ReadValueAsObject(), this.time);
			}

			private readonly InputStateHistory m_Owner;

			private readonly int m_IndexPlusOne;

			private uint m_Version;
		}
	}
}
