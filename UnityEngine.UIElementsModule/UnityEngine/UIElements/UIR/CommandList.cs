using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.UIElements.UIR
{
	internal class CommandList : IDisposable
	{
		public CommandList(VisualElement owner, IntPtr vertexDecl, IntPtr stencilState, Material material)
		{
			this.m_Owner = owner;
			this.m_VertexDecl = vertexDecl;
			this.m_StencilState = stencilState;
			this.m_DrawRanges = new NativeList<DrawBufferRange>(1024);
			this.handle = GCHandle.Alloc(this);
			this.m_Material = material;
		}

		public int Count
		{
			get
			{
				return this.m_Commands.Count;
			}
		}

		public void Reset(VisualElement newOwner, Material material)
		{
			this.m_Owner = newOwner;
			this.m_Commands.Clear();
			this.m_DrawRanges.Clear();
			this.m_Material = material;
			for (int i = 0; i < this.m_GpuTextureData.Length; i++)
			{
				this.m_GpuTextureData[i] = Vector4.zero;
			}
		}

		public unsafe void Execute()
		{
			IntPtr* ptr = stackalloc IntPtr[checked(unchecked((UIntPtr)1) * (UIntPtr)sizeof(IntPtr))];
			Utility.SetPropertyBlock(this.constantProps);
			Utility.SetStencilState(this.m_StencilState, 0);
			int num = 0;
			int* ptr2 = stackalloc int[(UIntPtr)32];
			IntPtr* ptr3 = stackalloc IntPtr[checked(unchecked((UIntPtr)8) * (UIntPtr)sizeof(IntPtr))];
			IntPtr shaderPropertySheet = Utility.AllocateShaderPropertySheet();
			try
			{
				for (int i = 0; i < this.m_Commands.Count; i++)
				{
					SerializedCommand serializedCommand = this.m_Commands[i];
					switch (serializedCommand.type)
					{
					case SerializedCommandType.DrawRanges:
						*ptr = serializedCommand.vertexBuffer;
						Utility.DrawRanges(serializedCommand.indexBuffer, ptr, 1, new IntPtr(this.m_DrawRanges.GetSlice(serializedCommand.firstRange, serializedCommand.rangeCount).GetUnsafePtr<DrawBufferRange>()), serializedCommand.rangeCount, this.m_VertexDecl);
						break;
					case SerializedCommandType.SetTexture:
						ptr2[num] = serializedCommand.textureName;
						ptr3[(IntPtr)num * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = serializedCommand.texturePtr;
						num++;
						this.m_GpuTextureData[serializedCommand.gpuDataOffset] = serializedCommand.gpuData0;
						this.m_GpuTextureData[serializedCommand.gpuDataOffset + 1] = serializedCommand.gpuData1;
						break;
					case SerializedCommandType.ApplyBatchProps:
						Utility.SetAllTextures(shaderPropertySheet, new IntPtr((void*)ptr2), new IntPtr((void*)ptr3), num);
						num = 0;
						Utility.SetVectorArray(shaderPropertySheet, TextureSlotManager.textureTableId, this.m_GpuTextureData);
						Utility.ApplyShaderPropertySheet(shaderPropertySheet);
						break;
					default:
						throw new NotImplementedException();
					}
				}
			}
			finally
			{
				Utility.ReleasePropertySheet(shaderPropertySheet);
			}
		}

		public void SetTexture(int name, Texture texture, int gpuDataOffset, Vector4 gpuData0, Vector4 gpuData1)
		{
			SerializedCommand item = new SerializedCommand
			{
				type = SerializedCommandType.SetTexture,
				textureName = name,
				texturePtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(texture),
				gpuDataOffset = gpuDataOffset,
				gpuData0 = gpuData0,
				gpuData1 = gpuData1
			};
			this.m_Commands.Add(item);
		}

		public void ApplyBatchProps()
		{
			SerializedCommand item = new SerializedCommand
			{
				type = SerializedCommandType.ApplyBatchProps
			};
			this.m_Commands.Add(item);
		}

		public void DrawRanges(Utility.GPUBuffer<ushort> ib, Utility.GPUBuffer<Vertex> vb, NativeSlice<DrawBufferRange> ranges)
		{
			SerializedCommand item = new SerializedCommand
			{
				type = SerializedCommandType.DrawRanges,
				vertexBuffer = vb.BufferPointer,
				indexBuffer = ib.BufferPointer,
				firstRange = this.m_DrawRanges.Count,
				rangeCount = ranges.Length
			};
			this.m_Commands.Add(item);
			this.m_DrawRanges.Add(ranges);
		}

		private protected bool disposed { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.m_DrawRanges.Dispose();
					this.m_DrawRanges = null;
					bool isAllocated = this.handle.IsAllocated;
					if (isAllocated)
					{
						this.handle.Free();
					}
				}
				this.disposed = true;
			}
		}

		public VisualElement m_Owner;

		private readonly IntPtr m_VertexDecl;

		private readonly IntPtr m_StencilState;

		public MaterialPropertyBlock constantProps = new MaterialPropertyBlock();

		public GCHandle handle;

		public Material m_Material;

		private List<SerializedCommand> m_Commands = new List<SerializedCommand>();

		private Vector4[] m_GpuTextureData = new Vector4[TextureSlotManager.k_SlotSize * TextureSlotManager.k_SlotCount];

		private NativeList<DrawBufferRange> m_DrawRanges;
	}
}
