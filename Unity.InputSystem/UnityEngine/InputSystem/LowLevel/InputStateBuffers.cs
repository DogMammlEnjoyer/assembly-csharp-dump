using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	internal struct InputStateBuffers
	{
		public InputStateBuffers.DoubleBuffers GetDoubleBuffersFor(InputUpdateType updateType)
		{
			if (updateType - InputUpdateType.Dynamic <= 1 || updateType == InputUpdateType.BeforeRender || updateType == InputUpdateType.Manual)
			{
				return this.m_PlayerStateBuffers;
			}
			throw new ArgumentException("Unrecognized InputUpdateType: " + updateType.ToString(), "updateType");
		}

		public unsafe static void* GetFrontBufferForDevice(int deviceIndex)
		{
			return InputStateBuffers.s_CurrentBuffers.GetFrontBuffer(deviceIndex);
		}

		public unsafe static void* GetBackBufferForDevice(int deviceIndex)
		{
			return InputStateBuffers.s_CurrentBuffers.GetBackBuffer(deviceIndex);
		}

		public static void SwitchTo(InputStateBuffers buffers, InputUpdateType update)
		{
			InputStateBuffers.s_CurrentBuffers = buffers.GetDoubleBuffersFor(update);
		}

		public unsafe void AllocateAll(InputDevice[] devices, int deviceCount)
		{
			this.sizePerBuffer = InputStateBuffers.ComputeSizeOfSingleStateBuffer(devices, deviceCount);
			if (this.sizePerBuffer == 0U)
			{
				return;
			}
			this.sizePerBuffer = this.sizePerBuffer.AlignToMultipleOf(4U);
			uint num = (uint)(deviceCount * sizeof(void*) * 2);
			this.totalSize = 0U;
			this.totalSize += this.sizePerBuffer * 2U;
			this.totalSize += num;
			this.totalSize += this.sizePerBuffer * 3U;
			this.m_AllBuffers = UnsafeUtility.Malloc((long)((ulong)this.totalSize), 4, Allocator.Persistent);
			UnsafeUtility.MemClear(this.m_AllBuffers, (long)((ulong)this.totalSize));
			byte* allBuffers = (byte*)this.m_AllBuffers;
			this.m_PlayerStateBuffers = InputStateBuffers.SetUpDeviceToBufferMappings(deviceCount, ref allBuffers, this.sizePerBuffer, num);
			this.defaultStateBuffer = (void*)allBuffers;
			this.noiseMaskBuffer = (void*)(allBuffers + this.sizePerBuffer);
			this.resetMaskBuffer = (void*)(allBuffers + this.sizePerBuffer * 2U);
		}

		private unsafe static InputStateBuffers.DoubleBuffers SetUpDeviceToBufferMappings(int deviceCount, ref byte* bufferPtr, uint sizePerBuffer, uint mappingTableSizePerBuffer)
		{
			byte* ptr = bufferPtr;
			byte* ptr2 = bufferPtr + sizePerBuffer;
			void** deviceToBufferMapping = bufferPtr / (IntPtr)sizeof(void*) + sizePerBuffer * 2U;
			bufferPtr += (IntPtr)((UIntPtr)(sizePerBuffer * 2U + mappingTableSizePerBuffer));
			InputStateBuffers.DoubleBuffers result = new InputStateBuffers.DoubleBuffers
			{
				deviceToBufferMapping = deviceToBufferMapping,
				deviceCount = deviceCount
			};
			for (int i = 0; i < deviceCount; i++)
			{
				int deviceIndex = i;
				result.SetFrontBuffer(deviceIndex, (void*)ptr);
				result.SetBackBuffer(deviceIndex, (void*)ptr2);
			}
			return result;
		}

		public void FreeAll()
		{
			if (this.m_AllBuffers != null)
			{
				UnsafeUtility.Free(this.m_AllBuffers, Allocator.Persistent);
				this.m_AllBuffers = null;
			}
			this.m_PlayerStateBuffers = default(InputStateBuffers.DoubleBuffers);
			InputStateBuffers.s_CurrentBuffers = default(InputStateBuffers.DoubleBuffers);
			if (InputStateBuffers.s_DefaultStateBuffer == this.defaultStateBuffer)
			{
				InputStateBuffers.s_DefaultStateBuffer = null;
			}
			this.defaultStateBuffer = null;
			if (InputStateBuffers.s_NoiseMaskBuffer == this.noiseMaskBuffer)
			{
				InputStateBuffers.s_NoiseMaskBuffer = null;
			}
			if (InputStateBuffers.s_ResetMaskBuffer == this.resetMaskBuffer)
			{
				InputStateBuffers.s_ResetMaskBuffer = null;
			}
			this.noiseMaskBuffer = null;
			this.resetMaskBuffer = null;
			this.totalSize = 0U;
			this.sizePerBuffer = 0U;
		}

		public void MigrateAll(InputDevice[] devices, int deviceCount, InputStateBuffers oldBuffers)
		{
			if (oldBuffers.totalSize > 0U)
			{
				InputStateBuffers.MigrateDoubleBuffer(this.m_PlayerStateBuffers, devices, deviceCount, oldBuffers.m_PlayerStateBuffers);
				InputStateBuffers.MigrateSingleBuffer(this.defaultStateBuffer, devices, deviceCount, oldBuffers.defaultStateBuffer);
				InputStateBuffers.MigrateSingleBuffer(this.noiseMaskBuffer, devices, deviceCount, oldBuffers.noiseMaskBuffer);
				InputStateBuffers.MigrateSingleBuffer(this.resetMaskBuffer, devices, deviceCount, oldBuffers.resetMaskBuffer);
			}
			uint num = 0U;
			for (int i = 0; i < deviceCount; i++)
			{
				InputDevice inputDevice = devices[i];
				uint byteOffset = inputDevice.m_StateBlock.byteOffset;
				if (byteOffset == 4294967295U)
				{
					inputDevice.m_StateBlock.byteOffset = 0U;
					if (num != 0U)
					{
						inputDevice.BakeOffsetIntoStateBlockRecursive(num);
					}
				}
				else
				{
					uint num2 = num - byteOffset;
					if (num2 != 0U)
					{
						inputDevice.BakeOffsetIntoStateBlockRecursive(num2);
					}
				}
				num = InputStateBuffers.NextDeviceOffset(num, inputDevice);
			}
		}

		private unsafe static void MigrateDoubleBuffer(InputStateBuffers.DoubleBuffers newBuffer, InputDevice[] devices, int deviceCount, InputStateBuffers.DoubleBuffers oldBuffer)
		{
			if (!newBuffer.valid)
			{
				return;
			}
			if (!oldBuffer.valid)
			{
				return;
			}
			uint num = 0U;
			for (int i = 0; i < deviceCount; i++)
			{
				InputDevice inputDevice = devices[i];
				if (inputDevice.m_StateBlock.byteOffset == 4294967295U)
				{
					break;
				}
				int deviceIndex = inputDevice.m_DeviceIndex;
				int deviceIndex2 = i;
				uint alignedSizeInBytes = inputDevice.m_StateBlock.alignedSizeInBytes;
				byte* source = (byte*)oldBuffer.GetFrontBuffer(deviceIndex) + inputDevice.m_StateBlock.byteOffset;
				byte* source2 = (byte*)oldBuffer.GetBackBuffer(deviceIndex) + inputDevice.m_StateBlock.byteOffset;
				byte* destination = (byte*)newBuffer.GetFrontBuffer(deviceIndex2) + num;
				void* destination2 = (void*)((byte*)newBuffer.GetBackBuffer(deviceIndex2) + num);
				UnsafeUtility.MemCpy((void*)destination, (void*)source, (long)((ulong)alignedSizeInBytes));
				UnsafeUtility.MemCpy(destination2, (void*)source2, (long)((ulong)alignedSizeInBytes));
				num = InputStateBuffers.NextDeviceOffset(num, inputDevice);
			}
		}

		private unsafe static void MigrateSingleBuffer(void* newBuffer, InputDevice[] devices, int deviceCount, void* oldBuffer)
		{
			uint num = 0U;
			for (int i = 0; i < deviceCount; i++)
			{
				InputDevice inputDevice = devices[i];
				if (inputDevice.m_StateBlock.byteOffset == 4294967295U)
				{
					break;
				}
				uint alignedSizeInBytes = inputDevice.m_StateBlock.alignedSizeInBytes;
				byte* source = (byte*)oldBuffer + inputDevice.m_StateBlock.byteOffset;
				UnsafeUtility.MemCpy((void*)((byte*)newBuffer + num), (void*)source, (long)((ulong)alignedSizeInBytes));
				num = InputStateBuffers.NextDeviceOffset(num, inputDevice);
			}
		}

		private static uint ComputeSizeOfSingleStateBuffer(InputDevice[] devices, int deviceCount)
		{
			uint num = 0U;
			for (int i = 0; i < deviceCount; i++)
			{
				num = InputStateBuffers.NextDeviceOffset(num, devices[i]);
			}
			return num;
		}

		private static uint NextDeviceOffset(uint currentOffset, InputDevice device)
		{
			uint alignedSizeInBytes = device.m_StateBlock.alignedSizeInBytes;
			if (alignedSizeInBytes == 0U)
			{
				throw new ArgumentException(string.Format("Device '{0}' has a zero-size state buffer", device), "device");
			}
			return currentOffset + alignedSizeInBytes.AlignToMultipleOf(4U);
		}

		public uint sizePerBuffer;

		public uint totalSize;

		public unsafe void* defaultStateBuffer;

		public unsafe void* noiseMaskBuffer;

		public unsafe void* resetMaskBuffer;

		private unsafe void* m_AllBuffers;

		internal InputStateBuffers.DoubleBuffers m_PlayerStateBuffers;

		internal unsafe static void* s_DefaultStateBuffer;

		internal unsafe static void* s_NoiseMaskBuffer;

		internal unsafe static void* s_ResetMaskBuffer;

		internal static InputStateBuffers.DoubleBuffers s_CurrentBuffers;

		[Serializable]
		internal struct DoubleBuffers
		{
			public bool valid
			{
				get
				{
					return this.deviceToBufferMapping != null;
				}
			}

			public unsafe void SetFrontBuffer(int deviceIndex, void* ptr)
			{
				if (deviceIndex < this.deviceCount)
				{
					*(IntPtr*)(this.deviceToBufferMapping + (IntPtr)(deviceIndex * 2) * (IntPtr)sizeof(void*) / (IntPtr)sizeof(void*)) = ptr;
				}
			}

			public unsafe void SetBackBuffer(int deviceIndex, void* ptr)
			{
				if (deviceIndex < this.deviceCount)
				{
					*(IntPtr*)(this.deviceToBufferMapping + (IntPtr)(deviceIndex * 2 + 1) * (IntPtr)sizeof(void*) / (IntPtr)sizeof(void*)) = ptr;
				}
			}

			public unsafe void* GetFrontBuffer(int deviceIndex)
			{
				if (deviceIndex < this.deviceCount)
				{
					return *(IntPtr*)(this.deviceToBufferMapping + (IntPtr)(deviceIndex * 2) * (IntPtr)sizeof(void*) / (IntPtr)sizeof(void*));
				}
				return null;
			}

			public unsafe void* GetBackBuffer(int deviceIndex)
			{
				if (deviceIndex < this.deviceCount)
				{
					return *(IntPtr*)(this.deviceToBufferMapping + (IntPtr)(deviceIndex * 2 + 1) * (IntPtr)sizeof(void*) / (IntPtr)sizeof(void*));
				}
				return null;
			}

			public unsafe void SwapBuffers(int deviceIndex)
			{
				if (!this.valid)
				{
					return;
				}
				void* frontBuffer = this.GetFrontBuffer(deviceIndex);
				void* backBuffer = this.GetBackBuffer(deviceIndex);
				this.SetFrontBuffer(deviceIndex, backBuffer);
				this.SetBackBuffer(deviceIndex, frontBuffer);
			}

			public unsafe void** deviceToBufferMapping;

			public int deviceCount;
		}
	}
}
