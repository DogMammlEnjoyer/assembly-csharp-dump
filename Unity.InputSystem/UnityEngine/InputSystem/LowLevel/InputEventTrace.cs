using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[Serializable]
	public sealed class InputEventTrace : IDisposable, IEnumerable<InputEventPtr>, IEnumerable
	{
		public static FourCC FrameMarkerEvent
		{
			get
			{
				return new FourCC('F', 'R', 'M', 'E');
			}
		}

		public int deviceId
		{
			get
			{
				return this.m_DeviceId;
			}
			set
			{
				this.m_DeviceId = value;
			}
		}

		public bool enabled
		{
			get
			{
				return this.m_Enabled;
			}
		}

		public bool recordFrameMarkers
		{
			get
			{
				return this.m_RecordFrameMarkers;
			}
			set
			{
				if (this.m_RecordFrameMarkers == value)
				{
					return;
				}
				this.m_RecordFrameMarkers = value;
				if (this.m_Enabled)
				{
					if (value)
					{
						InputSystem.onBeforeUpdate += this.OnBeforeUpdate;
						return;
					}
					InputSystem.onBeforeUpdate -= this.OnBeforeUpdate;
				}
			}
		}

		public long eventCount
		{
			get
			{
				return this.m_EventCount;
			}
		}

		public long totalEventSizeInBytes
		{
			get
			{
				return this.m_EventSizeInBytes;
			}
		}

		public long allocatedSizeInBytes
		{
			get
			{
				if (this.m_EventBuffer == null)
				{
					return 0L;
				}
				return this.m_EventBufferSize;
			}
		}

		public long maxSizeInBytes
		{
			get
			{
				return this.m_MaxEventBufferSize;
			}
		}

		public ReadOnlyArray<InputEventTrace.DeviceInfo> deviceInfos
		{
			get
			{
				return this.m_DeviceInfos;
			}
		}

		public Func<InputEventPtr, InputDevice, bool> onFilterEvent
		{
			get
			{
				return this.m_OnFilterEvent;
			}
			set
			{
				this.m_OnFilterEvent = value;
			}
		}

		public event Action<InputEventPtr> onEvent
		{
			add
			{
				this.m_EventListeners.AddCallback(value);
			}
			remove
			{
				this.m_EventListeners.RemoveCallback(value);
			}
		}

		public InputEventTrace(InputDevice device, long bufferSizeInBytes = 1048576L, bool growBuffer = false, long maxBufferSizeInBytes = -1L, long growIncrementSizeInBytes = -1L) : this(bufferSizeInBytes, growBuffer, maxBufferSizeInBytes, growIncrementSizeInBytes)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			this.m_DeviceId = device.deviceId;
		}

		public InputEventTrace(long bufferSizeInBytes = 1048576L, bool growBuffer = false, long maxBufferSizeInBytes = -1L, long growIncrementSizeInBytes = -1L)
		{
			this.m_EventBufferSize = (long)((ulong)((uint)bufferSizeInBytes));
			if (!growBuffer)
			{
				this.m_MaxEventBufferSize = this.m_EventBufferSize;
				return;
			}
			if (maxBufferSizeInBytes < 0L)
			{
				this.m_MaxEventBufferSize = 268435456L;
			}
			else
			{
				this.m_MaxEventBufferSize = maxBufferSizeInBytes;
			}
			if (growIncrementSizeInBytes < 0L)
			{
				this.m_GrowIncrementSize = 1048576L;
				return;
			}
			this.m_GrowIncrementSize = growIncrementSizeInBytes;
		}

		public void WriteTo(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentNullException("filePath");
			}
			using (FileStream fileStream = File.OpenWrite(filePath))
			{
				this.WriteTo(fileStream);
			}
		}

		public unsafe void WriteTo(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanSeek)
			{
				throw new ArgumentException("Stream does not support seeking", "stream");
			}
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			InputEventTrace.FileFlags fileFlags = (InputEventTrace.FileFlags)0;
			if (InputSystem.settings.updateMode == InputSettings.UpdateMode.ProcessEventsInFixedUpdate)
			{
				fileFlags |= InputEventTrace.FileFlags.FixedUpdate;
			}
			binaryWriter.Write(InputEventTrace.kFileFormat);
			binaryWriter.Write(InputEventTrace.kFileVersion);
			binaryWriter.Write((int)fileFlags);
			binaryWriter.Write((int)Application.platform);
			binaryWriter.Write((ulong)this.m_EventCount);
			binaryWriter.Write((ulong)this.m_EventSizeInBytes);
			foreach (InputEventPtr inputEventPtr in this)
			{
				uint sizeInBytes = inputEventPtr.sizeInBytes;
				byte[] array = new byte[sizeInBytes];
				try
				{
					byte[] array2;
					byte* destination;
					if ((array2 = array) == null || array2.Length == 0)
					{
						destination = null;
					}
					else
					{
						destination = &array2[0];
					}
					UnsafeUtility.MemCpy((void*)destination, (void*)inputEventPtr.data, (long)((ulong)sizeInBytes));
					binaryWriter.Write(array);
				}
				finally
				{
					byte[] array2 = null;
				}
			}
			binaryWriter.Flush();
			long position = stream.Position;
			int num = this.m_DeviceInfos.LengthSafe<InputEventTrace.DeviceInfo>();
			binaryWriter.Write(num);
			for (int i = 0; i < num; i++)
			{
				ref InputEventTrace.DeviceInfo ptr = ref this.m_DeviceInfos[i];
				binaryWriter.Write(ptr.deviceId);
				binaryWriter.Write(ptr.layout);
				binaryWriter.Write(ptr.stateFormat);
				binaryWriter.Write(ptr.stateSizeInBytes);
				binaryWriter.Write(ptr.m_FullLayoutJson ?? string.Empty);
			}
			binaryWriter.Flush();
			long value = stream.Position - position;
			binaryWriter.Write(value);
		}

		public void ReadFrom(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentNullException("filePath");
			}
			using (FileStream fileStream = File.OpenRead(filePath))
			{
				this.ReadFrom(fileStream);
			}
		}

		public unsafe void ReadFrom(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException("Stream does not support reading", "stream");
			}
			BinaryReader binaryReader = new BinaryReader(stream);
			if (binaryReader.ReadInt32() != InputEventTrace.kFileFormat)
			{
				throw new IOException(string.Format("Stream does not appear to be an InputEventTrace (no '{0}' code)", InputEventTrace.kFileFormat));
			}
			if (binaryReader.ReadInt32() > InputEventTrace.kFileVersion)
			{
				throw new IOException(string.Format("Stream is an InputEventTrace but a newer version (expected version {0} or below)", InputEventTrace.kFileVersion));
			}
			binaryReader.ReadInt32();
			binaryReader.ReadInt32();
			ulong num = binaryReader.ReadUInt64();
			ulong num2 = binaryReader.ReadUInt64();
			byte* eventBuffer = this.m_EventBuffer;
			if (num > 0UL && num2 > 0UL)
			{
				byte* ptr;
				if (this.m_EventBuffer != null && this.m_EventBufferSize >= (long)num2)
				{
					ptr = this.m_EventBuffer;
				}
				else
				{
					ptr = (byte*)UnsafeUtility.Malloc((long)num2, 4, Allocator.Persistent);
					this.m_EventBufferSize = (long)num2;
				}
				try
				{
					byte* ptr2 = ptr;
					byte* ptr3 = ptr2 + num2;
					long num3 = 0L;
					for (ulong num4 = 0UL; num4 < num; num4 += 1UL)
					{
						int num5 = binaryReader.ReadInt32();
						uint num6 = (uint)binaryReader.ReadUInt16();
						uint num7 = (uint)binaryReader.ReadUInt16();
						if ((ulong)num6 > (ulong)((long)(ptr3 - ptr2)))
						{
							break;
						}
						*(int*)ptr2 = num5;
						ptr2 += 4;
						*(short*)ptr2 = (short)((ushort)num6);
						ptr2 += 2;
						*(short*)ptr2 = (short)((ushort)num7);
						ptr2 += 2;
						int num8 = (int)(num6 - 4U - 2U - 2U);
						byte[] array = binaryReader.ReadBytes(num8);
						try
						{
							byte[] array2;
							byte* source;
							if ((array2 = array) == null || array2.Length == 0)
							{
								source = null;
							}
							else
							{
								source = &array2[0];
							}
							UnsafeUtility.MemCpy((void*)ptr2, (void*)source, (long)num8);
						}
						finally
						{
							byte[] array2 = null;
						}
						ptr2 += num8.AlignToMultipleOf(4);
						num3 += (long)((ulong)num6.AlignToMultipleOf(4U));
						if (ptr2 >= ptr3)
						{
							break;
						}
					}
					int num9 = binaryReader.ReadInt32();
					InputEventTrace.DeviceInfo[] array3 = new InputEventTrace.DeviceInfo[num9];
					for (int i = 0; i < num9; i++)
					{
						array3[i] = new InputEventTrace.DeviceInfo
						{
							deviceId = binaryReader.ReadInt32(),
							layout = binaryReader.ReadString(),
							stateFormat = binaryReader.ReadInt32(),
							stateSizeInBytes = binaryReader.ReadInt32(),
							m_FullLayoutJson = binaryReader.ReadString()
						};
					}
					this.m_EventBuffer = ptr;
					this.m_EventBufferHead = this.m_EventBuffer;
					this.m_EventBufferTail = ptr3;
					this.m_EventCount = (long)num;
					this.m_EventSizeInBytes = num3;
					this.m_DeviceInfos = array3;
					goto IL_296;
				}
				catch
				{
					if (ptr != eventBuffer)
					{
						UnsafeUtility.Free((void*)ptr, Allocator.Persistent);
					}
					throw;
				}
			}
			this.m_EventBuffer = null;
			this.m_EventBufferHead = null;
			this.m_EventBufferTail = null;
			IL_296:
			if (this.m_EventBuffer != eventBuffer && eventBuffer != null)
			{
				UnsafeUtility.Free((void*)eventBuffer, Allocator.Persistent);
			}
			this.m_ChangeCounter++;
		}

		public static InputEventTrace LoadFrom(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentNullException("filePath");
			}
			InputEventTrace result;
			using (FileStream fileStream = File.OpenRead(filePath))
			{
				result = InputEventTrace.LoadFrom(fileStream);
			}
			return result;
		}

		public static InputEventTrace LoadFrom(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException("Stream must be readable", "stream");
			}
			InputEventTrace inputEventTrace = new InputEventTrace(1048576L, false, -1L, -1L);
			inputEventTrace.ReadFrom(stream);
			return inputEventTrace;
		}

		public InputEventTrace.ReplayController Replay()
		{
			this.Disable();
			return new InputEventTrace.ReplayController(this);
		}

		public unsafe bool Resize(long newBufferSize, long newMaxBufferSize = -1L)
		{
			if (newBufferSize <= 0L)
			{
				throw new ArgumentException("Size must be positive", "newBufferSize");
			}
			if (this.m_EventBufferSize == newBufferSize)
			{
				return true;
			}
			if (newMaxBufferSize < newBufferSize)
			{
				newMaxBufferSize = newBufferSize;
			}
			byte* ptr = (byte*)UnsafeUtility.Malloc(newBufferSize, 4, Allocator.Persistent);
			if (ptr == null)
			{
				return false;
			}
			if (this.m_EventCount > 0L)
			{
				if (newBufferSize < this.m_EventBufferSize || this.m_HasWrapped)
				{
					InputEventPtr inputEventPtr = new InputEventPtr((InputEvent*)this.m_EventBufferHead);
					InputEvent* ptr2 = (InputEvent*)ptr;
					int num = 0;
					int num2 = 0;
					long num3 = this.m_EventSizeInBytes;
					int num4 = 0;
					while ((long)num4 < this.m_EventCount)
					{
						uint sizeInBytes = inputEventPtr.sizeInBytes;
						uint num5 = sizeInBytes.AlignToMultipleOf(4U);
						if (num3 <= newBufferSize)
						{
							UnsafeUtility.MemCpy((void*)ptr2, (void*)inputEventPtr.ToPointer(), (long)((ulong)sizeInBytes));
							ptr2 = InputEvent.GetNextInMemory(ptr2);
							num2 += (int)num5;
							num++;
						}
						num3 -= (long)((ulong)num5);
						if (!this.GetNextEvent(ref inputEventPtr))
						{
							break;
						}
						num4++;
					}
					this.m_HasWrapped = false;
					this.m_EventCount = (long)num;
					this.m_EventSizeInBytes = (long)num2;
				}
				else
				{
					UnsafeUtility.MemCpy((void*)ptr, (void*)this.m_EventBufferHead, this.m_EventSizeInBytes);
				}
			}
			if (this.m_EventBuffer != null)
			{
				UnsafeUtility.Free((void*)this.m_EventBuffer, Allocator.Persistent);
			}
			this.m_EventBufferSize = newBufferSize;
			this.m_EventBuffer = ptr;
			this.m_EventBufferHead = ptr;
			this.m_EventBufferTail = this.m_EventBuffer + this.m_EventSizeInBytes;
			this.m_MaxEventBufferSize = newMaxBufferSize;
			this.m_ChangeCounter++;
			return true;
		}

		public unsafe void Clear()
		{
			byte* ptr = default(byte*);
			this.m_EventBufferTail = ptr;
			this.m_EventBufferHead = ptr;
			this.m_EventCount = 0L;
			this.m_EventSizeInBytes = 0L;
			this.m_ChangeCounter++;
			this.m_DeviceInfos = null;
		}

		public void Enable()
		{
			if (this.m_Enabled)
			{
				return;
			}
			if (this.m_EventBuffer == null)
			{
				this.Allocate();
			}
			InputSystem.onEvent += new Action<InputEventPtr, InputDevice>(this.OnInputEvent);
			if (this.m_RecordFrameMarkers)
			{
				InputSystem.onBeforeUpdate += this.OnBeforeUpdate;
			}
			this.m_Enabled = true;
		}

		public void Disable()
		{
			if (!this.m_Enabled)
			{
				return;
			}
			InputSystem.onEvent -= new Action<InputEventPtr, InputDevice>(this.OnInputEvent);
			InputSystem.onBeforeUpdate -= this.OnBeforeUpdate;
			this.m_Enabled = false;
		}

		public unsafe bool GetNextEvent(ref InputEventPtr current)
		{
			if (this.m_EventBuffer == null)
			{
				return false;
			}
			if (this.m_EventBufferHead == null)
			{
				return false;
			}
			if (!current.valid)
			{
				current = new InputEventPtr((InputEvent*)this.m_EventBufferHead);
				return true;
			}
			byte* ptr = (byte*)current.Next().data;
			byte* ptr2 = this.m_EventBuffer + this.m_EventBufferSize;
			if (ptr == this.m_EventBufferTail)
			{
				return false;
			}
			if ((long)(ptr2 - ptr) < 20L || ((InputEvent*)ptr)->sizeInBytes == 0U)
			{
				ptr = this.m_EventBuffer;
				if (ptr == (byte*)current.ToPointer())
				{
					return false;
				}
			}
			current = new InputEventPtr((InputEvent*)ptr);
			return true;
		}

		public IEnumerator<InputEventPtr> GetEnumerator()
		{
			return new InputEventTrace.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Dispose()
		{
			this.Disable();
			this.Release();
		}

		private unsafe byte* m_EventBuffer
		{
			get
			{
				return this.m_EventBufferStorage;
			}
			set
			{
				this.m_EventBufferStorage = value;
			}
		}

		private unsafe byte* m_EventBufferHead
		{
			get
			{
				return this.m_EventBufferHeadStorage;
			}
			set
			{
				this.m_EventBufferHeadStorage = value;
			}
		}

		private unsafe byte* m_EventBufferTail
		{
			get
			{
				return this.m_EventBufferTailStorage;
			}
			set
			{
				this.m_EventBufferTailStorage = value;
			}
		}

		private unsafe void Allocate()
		{
			this.m_EventBuffer = (byte*)UnsafeUtility.Malloc(this.m_EventBufferSize, 4, Allocator.Persistent);
		}

		private unsafe void Release()
		{
			this.Clear();
			if (this.m_EventBuffer != null)
			{
				UnsafeUtility.Free((void*)this.m_EventBuffer, Allocator.Persistent);
				this.m_EventBuffer = null;
			}
		}

		private unsafe void OnBeforeUpdate()
		{
			if (this.m_RecordFrameMarkers)
			{
				InputEvent inputEvent = new InputEvent
				{
					type = InputEventTrace.FrameMarkerEvent,
					internalTime = InputRuntime.s_Instance.currentTime,
					sizeInBytes = (uint)UnsafeUtility.SizeOf<InputEvent>()
				};
				this.OnInputEvent(new InputEventPtr((InputEvent*)UnsafeUtility.AddressOf<InputEvent>(ref inputEvent)), null);
			}
		}

		private unsafe void OnInputEvent(InputEventPtr inputEvent, InputDevice device)
		{
			if (inputEvent.handled)
			{
				return;
			}
			if (this.m_DeviceId != 0 && inputEvent.deviceId != this.m_DeviceId && inputEvent.type != InputEventTrace.FrameMarkerEvent)
			{
				return;
			}
			if (this.m_OnFilterEvent != null && !this.m_OnFilterEvent(inputEvent, device))
			{
				return;
			}
			if (this.m_EventBuffer == null)
			{
				return;
			}
			uint num = inputEvent.sizeInBytes.AlignToMultipleOf(4U);
			if ((ulong)num > (ulong)this.m_MaxEventBufferSize)
			{
				return;
			}
			if (this.m_EventBufferTail == null)
			{
				this.m_EventBufferHead = this.m_EventBuffer;
				this.m_EventBufferTail = this.m_EventBuffer;
			}
			byte* ptr = this.m_EventBufferTail + num;
			bool flag = ptr != this.m_EventBufferHead && this.m_EventBufferHead != this.m_EventBuffer;
			if (ptr != this.m_EventBuffer + this.m_EventBufferSize)
			{
				if (this.m_EventBufferSize < this.m_MaxEventBufferSize && !this.m_HasWrapped)
				{
					long num2 = Math.Max(this.m_GrowIncrementSize, (long)((ulong)num.AlignToMultipleOf(4U)));
					long num3 = this.m_EventBufferSize + num2;
					if (num3 > this.m_MaxEventBufferSize)
					{
						num3 = this.m_MaxEventBufferSize;
					}
					if (num3 < (long)((ulong)num))
					{
						return;
					}
					this.Resize(num3, -1L);
					ptr = this.m_EventBufferTail + num;
				}
				long num4 = this.m_EventBufferSize - (long)(this.m_EventBufferTail - this.m_EventBuffer);
				if (num4 < (long)((ulong)num))
				{
					this.m_HasWrapped = true;
					if (num4 >= 20L)
					{
						UnsafeUtility.MemClear((void*)this.m_EventBufferTail, 20L);
					}
					this.m_EventBufferTail = this.m_EventBuffer;
					ptr = this.m_EventBuffer + num;
					if (flag)
					{
						this.m_EventBufferHead = this.m_EventBuffer;
					}
					flag = (ptr != this.m_EventBufferHead);
				}
			}
			if (flag)
			{
				byte* ptr2 = this.m_EventBufferHead;
				byte* ptr3 = this.m_EventBuffer + this.m_EventBufferSize - 20;
				while (ptr2 < ptr)
				{
					uint sizeInBytes = ((InputEvent*)ptr2)->sizeInBytes;
					ptr2 += sizeInBytes;
					this.m_EventCount -= 1L;
					this.m_EventSizeInBytes -= (long)((ulong)sizeInBytes);
					if (ptr2 != ptr3 || ((InputEvent*)ptr2)->sizeInBytes == 0U)
					{
						ptr2 = this.m_EventBuffer;
						break;
					}
				}
				this.m_EventBufferHead = ptr2;
			}
			byte* eventBufferTail = this.m_EventBufferTail;
			this.m_EventBufferTail = ptr;
			UnsafeUtility.MemCpy((void*)eventBufferTail, (void*)inputEvent.data, (long)((ulong)inputEvent.sizeInBytes));
			this.m_ChangeCounter++;
			this.m_EventCount += 1L;
			this.m_EventSizeInBytes += (long)((ulong)num);
			if (device != null)
			{
				bool flag2 = false;
				if (this.m_DeviceInfos != null)
				{
					for (int i = 0; i < this.m_DeviceInfos.Length; i++)
					{
						if (this.m_DeviceInfos[i].deviceId == device.deviceId)
						{
							flag2 = true;
							break;
						}
					}
				}
				if (!flag2)
				{
					ArrayHelpers.Append<InputEventTrace.DeviceInfo>(ref this.m_DeviceInfos, new InputEventTrace.DeviceInfo
					{
						m_DeviceId = device.deviceId,
						m_Layout = device.layout,
						m_StateFormat = device.stateBlock.format,
						m_StateSizeInBytes = (int)device.stateBlock.alignedSizeInBytes,
						m_FullLayoutJson = (InputControlLayout.s_Layouts.IsGeneratedLayout(device.m_Layout) ? InputSystem.LoadLayout(device.layout).ToJson() : null)
					});
				}
			}
			if (this.m_EventListeners.length > 0)
			{
				DelegateHelpers.InvokeCallbacksSafe<InputEventPtr>(ref this.m_EventListeners, new InputEventPtr((InputEvent*)eventBufferTail), "InputEventTrace.onEvent", null);
			}
		}

		private static FourCC kFileFormat
		{
			get
			{
				return new FourCC('I', 'E', 'V', 'T');
			}
		}

		private const int kDefaultBufferSize = 1048576;

		private static readonly ProfilerMarker k_InputEvenTraceMarker = new ProfilerMarker("InputEventTrace");

		[NonSerialized]
		private int m_ChangeCounter;

		[NonSerialized]
		private bool m_Enabled;

		[NonSerialized]
		private Func<InputEventPtr, InputDevice, bool> m_OnFilterEvent;

		[SerializeField]
		private int m_DeviceId;

		[NonSerialized]
		private CallbackArray<Action<InputEventPtr>> m_EventListeners;

		[SerializeField]
		private long m_EventBufferSize;

		[SerializeField]
		private long m_MaxEventBufferSize;

		[SerializeField]
		private long m_GrowIncrementSize;

		[SerializeField]
		private long m_EventCount;

		[SerializeField]
		private long m_EventSizeInBytes;

		[SerializeField]
		private ulong m_EventBufferStorage;

		[SerializeField]
		private ulong m_EventBufferHeadStorage;

		[SerializeField]
		private ulong m_EventBufferTailStorage;

		[SerializeField]
		private bool m_HasWrapped;

		[SerializeField]
		private bool m_RecordFrameMarkers;

		[SerializeField]
		private InputEventTrace.DeviceInfo[] m_DeviceInfos;

		private static int kFileVersion = 1;

		private class Enumerator : IEnumerator<InputEventPtr>, IEnumerator, IDisposable
		{
			public Enumerator(InputEventTrace trace)
			{
				this.m_Trace = trace;
				this.m_ChangeCounter = trace.m_ChangeCounter;
			}

			public void Dispose()
			{
				this.m_Trace = null;
				this.m_Current = default(InputEventPtr);
			}

			public bool MoveNext()
			{
				if (this.m_Trace == null)
				{
					throw new ObjectDisposedException(this.ToString());
				}
				if (this.m_Trace.m_ChangeCounter != this.m_ChangeCounter)
				{
					throw new InvalidOperationException("Trace has been modified while enumerating!");
				}
				return this.m_Trace.GetNextEvent(ref this.m_Current);
			}

			public void Reset()
			{
				this.m_Current = default(InputEventPtr);
				this.m_ChangeCounter = this.m_Trace.m_ChangeCounter;
			}

			public InputEventPtr Current
			{
				get
				{
					return this.m_Current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			private InputEventTrace m_Trace;

			private int m_ChangeCounter;

			internal InputEventPtr m_Current;
		}

		[Flags]
		private enum FileFlags
		{
			FixedUpdate = 1
		}

		public class ReplayController : IDisposable
		{
			public InputEventTrace trace
			{
				get
				{
					return this.m_EventTrace;
				}
			}

			public bool finished { get; private set; }

			public bool paused { get; set; }

			public int position { get; private set; }

			public IEnumerable<InputDevice> createdDevices
			{
				get
				{
					return this.m_CreatedDevices;
				}
			}

			internal ReplayController(InputEventTrace trace)
			{
				if (trace == null)
				{
					throw new ArgumentNullException("trace");
				}
				this.m_EventTrace = trace;
			}

			public void Dispose()
			{
				InputSystem.onBeforeUpdate -= this.OnBeginFrame;
				this.finished = true;
				foreach (InputDevice device in this.m_CreatedDevices)
				{
					InputSystem.RemoveDevice(device);
				}
				this.m_CreatedDevices = default(InlinedArray<InputDevice>);
			}

			public InputEventTrace.ReplayController WithDeviceMappedFromTo(InputDevice recordedDevice, InputDevice playbackDevice)
			{
				if (recordedDevice == null)
				{
					throw new ArgumentNullException("recordedDevice");
				}
				if (playbackDevice == null)
				{
					throw new ArgumentNullException("playbackDevice");
				}
				this.WithDeviceMappedFromTo(recordedDevice.deviceId, playbackDevice.deviceId);
				return this;
			}

			public InputEventTrace.ReplayController WithDeviceMappedFromTo(int recordedDeviceId, int playbackDeviceId)
			{
				for (int i = 0; i < this.m_DeviceIDMappings.length; i++)
				{
					if (this.m_DeviceIDMappings[i].Key == recordedDeviceId)
					{
						if (recordedDeviceId == playbackDeviceId)
						{
							this.m_DeviceIDMappings.RemoveAtWithCapacity(i);
						}
						else
						{
							this.m_DeviceIDMappings[i] = new KeyValuePair<int, int>(recordedDeviceId, playbackDeviceId);
						}
						return this;
					}
				}
				if (recordedDeviceId == playbackDeviceId)
				{
					return this;
				}
				this.m_DeviceIDMappings.AppendWithCapacity(new KeyValuePair<int, int>(recordedDeviceId, playbackDeviceId), 10);
				return this;
			}

			public InputEventTrace.ReplayController WithAllDevicesMappedToNewInstances()
			{
				this.m_CreateNewDevices = true;
				return this;
			}

			public InputEventTrace.ReplayController OnFinished(Action action)
			{
				this.m_OnFinished = action;
				return this;
			}

			public InputEventTrace.ReplayController OnEvent(Action<InputEventPtr> action)
			{
				this.m_OnEvent = action;
				return this;
			}

			public InputEventTrace.ReplayController PlayOneEvent()
			{
				InputEventPtr eventPtr;
				if (!this.MoveNext(true, out eventPtr))
				{
					throw new InvalidOperationException("No more events");
				}
				this.QueueEvent(eventPtr);
				return this;
			}

			public InputEventTrace.ReplayController Rewind()
			{
				this.m_Enumerator = null;
				this.m_AllEventsByTime = null;
				this.m_AllEventsByTimeIndex = -1;
				this.position = 0;
				return this;
			}

			public InputEventTrace.ReplayController PlayAllFramesOneByOne()
			{
				this.finished = false;
				InputSystem.onBeforeUpdate += this.OnBeginFrame;
				return this;
			}

			public InputEventTrace.ReplayController PlayAllEvents()
			{
				this.finished = false;
				try
				{
					InputEventPtr eventPtr;
					while (this.MoveNext(true, out eventPtr))
					{
						this.QueueEvent(eventPtr);
					}
				}
				finally
				{
					this.Finished();
				}
				return this;
			}

			public InputEventTrace.ReplayController PlayAllEventsAccordingToTimestamps()
			{
				List<InputEventPtr> list = new List<InputEventPtr>();
				InputEventPtr item;
				while (this.MoveNext(true, out item))
				{
					list.Add(item);
				}
				list.Sort((InputEventPtr a, InputEventPtr b) => a.time.CompareTo(b.time));
				this.m_Enumerator.Dispose();
				this.m_Enumerator = null;
				this.m_AllEventsByTime = list;
				this.position = 0;
				this.finished = false;
				this.m_StartTimeAsPerFirstEvent = -1.0;
				this.m_AllEventsByTimeIndex = -1;
				InputSystem.onBeforeUpdate += this.OnBeginFrame;
				return this;
			}

			private void OnBeginFrame()
			{
				if (this.paused)
				{
					return;
				}
				InputEventPtr inputEventPtr;
				if (!this.MoveNext(false, out inputEventPtr))
				{
					if (this.m_AllEventsByTime == null || this.m_AllEventsByTimeIndex >= this.m_AllEventsByTime.Count)
					{
						this.Finished();
					}
					return;
				}
				int position;
				if (inputEventPtr.type == InputEventTrace.FrameMarkerEvent)
				{
					InputEventPtr inputEventPtr2;
					if (!this.MoveNext(false, out inputEventPtr2))
					{
						this.Finished();
						return;
					}
					if (inputEventPtr2.type == InputEventTrace.FrameMarkerEvent)
					{
						position = this.position - 1;
						this.position = position;
						this.m_Enumerator.m_Current = inputEventPtr;
						return;
					}
					inputEventPtr = inputEventPtr2;
				}
				for (;;)
				{
					this.QueueEvent(inputEventPtr);
					InputEventPtr inputEventPtr3;
					if (!this.MoveNext(false, out inputEventPtr3))
					{
						break;
					}
					if (inputEventPtr3.type == InputEventTrace.FrameMarkerEvent)
					{
						goto Block_9;
					}
					inputEventPtr = inputEventPtr3;
				}
				if (this.m_AllEventsByTime == null || this.m_AllEventsByTimeIndex >= this.m_AllEventsByTime.Count)
				{
					this.Finished();
					return;
				}
				return;
				Block_9:
				this.m_Enumerator.m_Current = inputEventPtr;
				position = this.position - 1;
				this.position = position;
			}

			private void Finished()
			{
				this.finished = true;
				InputSystem.onBeforeUpdate -= this.OnBeginFrame;
				Action onFinished = this.m_OnFinished;
				if (onFinished == null)
				{
					return;
				}
				onFinished();
			}

			private void QueueEvent(InputEventPtr eventPtr)
			{
				double internalTime = eventPtr.internalTime;
				if (this.m_AllEventsByTime != null)
				{
					eventPtr.internalTime = this.m_StartTimeAsPerRuntime + (eventPtr.internalTime - this.m_StartTimeAsPerFirstEvent);
				}
				else
				{
					eventPtr.internalTime = InputRuntime.s_Instance.currentTime;
				}
				int id = eventPtr.id;
				int deviceId = eventPtr.deviceId;
				eventPtr.deviceId = this.ApplyDeviceMapping(deviceId);
				Action<InputEventPtr> onEvent = this.m_OnEvent;
				if (onEvent != null)
				{
					onEvent(eventPtr);
				}
				try
				{
					InputSystem.QueueEvent(eventPtr);
				}
				finally
				{
					eventPtr.internalTime = internalTime;
					eventPtr.id = id;
					eventPtr.deviceId = deviceId;
				}
			}

			private bool MoveNext(bool skipFrameEvents, out InputEventPtr eventPtr)
			{
				eventPtr = default(InputEventPtr);
				int position;
				if (this.m_AllEventsByTime == null)
				{
					if (this.m_Enumerator == null)
					{
						this.m_Enumerator = new InputEventTrace.Enumerator(this.m_EventTrace);
					}
					while (this.m_Enumerator.MoveNext())
					{
						position = this.position + 1;
						this.position = position;
						eventPtr = this.m_Enumerator.Current;
						if (!skipFrameEvents || !(eventPtr.type == InputEventTrace.FrameMarkerEvent))
						{
							return true;
						}
					}
					return false;
				}
				if (this.m_AllEventsByTimeIndex + 1 >= this.m_AllEventsByTime.Count)
				{
					this.position = this.m_AllEventsByTime.Count;
					this.m_AllEventsByTimeIndex = this.m_AllEventsByTime.Count;
					return false;
				}
				if (this.m_AllEventsByTimeIndex < 0)
				{
					this.m_StartTimeAsPerFirstEvent = this.m_AllEventsByTime[0].internalTime;
					this.m_StartTimeAsPerRuntime = InputRuntime.s_Instance.currentTime;
				}
				else if (this.m_AllEventsByTimeIndex < this.m_AllEventsByTime.Count - 1 && this.m_AllEventsByTime[this.m_AllEventsByTimeIndex + 1].internalTime > this.m_StartTimeAsPerFirstEvent + (InputRuntime.s_Instance.currentTime - this.m_StartTimeAsPerRuntime))
				{
					return false;
				}
				this.m_AllEventsByTimeIndex++;
				position = this.position + 1;
				this.position = position;
				eventPtr = this.m_AllEventsByTime[this.m_AllEventsByTimeIndex];
				return true;
			}

			private int ApplyDeviceMapping(int originalDeviceId)
			{
				for (int i = 0; i < this.m_DeviceIDMappings.length; i++)
				{
					KeyValuePair<int, int> keyValuePair = this.m_DeviceIDMappings[i];
					if (keyValuePair.Key == originalDeviceId)
					{
						return keyValuePair.Value;
					}
				}
				if (this.m_CreateNewDevices)
				{
					try
					{
						int num = this.m_EventTrace.deviceInfos.IndexOf((InputEventTrace.DeviceInfo x) => x.deviceId == originalDeviceId);
						if (num != -1)
						{
							InputEventTrace.DeviceInfo deviceInfo = this.m_EventTrace.deviceInfos[num];
							InternedString internedString = new InternedString(deviceInfo.layout);
							if (!InputControlLayout.s_Layouts.HasLayout(internedString))
							{
								if (string.IsNullOrEmpty(deviceInfo.m_FullLayoutJson))
								{
									return originalDeviceId;
								}
								InputSystem.RegisterLayout(deviceInfo.m_FullLayoutJson, null, null);
							}
							InputDevice inputDevice = InputSystem.AddDevice(internedString, null, null);
							this.WithDeviceMappedFromTo(originalDeviceId, inputDevice.deviceId);
							this.m_CreatedDevices.AppendWithCapacity(inputDevice, 10);
							return inputDevice.deviceId;
						}
					}
					catch
					{
					}
				}
				return originalDeviceId;
			}

			private InputEventTrace m_EventTrace;

			private InputEventTrace.Enumerator m_Enumerator;

			private InlinedArray<KeyValuePair<int, int>> m_DeviceIDMappings;

			private bool m_CreateNewDevices;

			private InlinedArray<InputDevice> m_CreatedDevices;

			private Action m_OnFinished;

			private Action<InputEventPtr> m_OnEvent;

			private double m_StartTimeAsPerFirstEvent;

			private double m_StartTimeAsPerRuntime;

			private int m_AllEventsByTimeIndex;

			private List<InputEventPtr> m_AllEventsByTime;
		}

		[Serializable]
		public struct DeviceInfo
		{
			public int deviceId
			{
				get
				{
					return this.m_DeviceId;
				}
				set
				{
					this.m_DeviceId = value;
				}
			}

			public string layout
			{
				get
				{
					return this.m_Layout;
				}
				set
				{
					this.m_Layout = value;
				}
			}

			public FourCC stateFormat
			{
				get
				{
					return this.m_StateFormat;
				}
				set
				{
					this.m_StateFormat = value;
				}
			}

			public int stateSizeInBytes
			{
				get
				{
					return this.m_StateSizeInBytes;
				}
				set
				{
					this.m_StateSizeInBytes = value;
				}
			}

			[SerializeField]
			internal int m_DeviceId;

			[SerializeField]
			internal string m_Layout;

			[SerializeField]
			internal FourCC m_StateFormat;

			[SerializeField]
			internal int m_StateSizeInBytes;

			[SerializeField]
			internal string m_FullLayoutJson;
		}
	}
}
