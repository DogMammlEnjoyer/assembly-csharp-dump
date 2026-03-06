using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler
{
	public struct FixedAttachmentArray<[IsUnmanaged] DataType> where DataType : struct, ValueType
	{
		public FixedAttachmentArray(int numAttachments)
		{
			this.a0 = (this.a1 = (this.a2 = (this.a3 = (this.a4 = (this.a5 = (this.a6 = (this.a7 = Activator.CreateInstance<DataType>())))))));
			this.activeAttachments = numAttachments;
		}

		public unsafe FixedAttachmentArray(DataType[] attachments)
		{
			this = new FixedAttachmentArray<DataType>(attachments.Length);
			for (int i = 0; i < this.activeAttachments; i++)
			{
				*this[i] = attachments[i];
			}
		}

		public unsafe FixedAttachmentArray(NativeArray<DataType> attachments)
		{
			this = new FixedAttachmentArray<DataType>(attachments.Length);
			for (int i = 0; i < this.activeAttachments; i++)
			{
				*this[i] = attachments[i];
			}
		}

		public int size
		{
			get
			{
				return this.activeAttachments;
			}
		}

		public void Clear()
		{
			this.activeAttachments = 0;
		}

		public unsafe int Add(in DataType data)
		{
			int num = this.activeAttachments;
			fixed (FixedAttachmentArray<DataType>* ptr = (FixedAttachmentArray<DataType>*)(&this))
			{
				DataType* ptr2 = (DataType*)ptr;
				ptr2[(IntPtr)num * (IntPtr)sizeof(DataType) / (IntPtr)sizeof(DataType)] = data;
			}
			this.activeAttachments++;
			return num;
		}

		public unsafe DataType this[int index]
		{
			get
			{
				fixed (FixedAttachmentArray<DataType>* ptr = (FixedAttachmentArray<DataType>*)(&this))
				{
					DataType* ptr2 = (DataType*)ptr;
					return ref ptr2[(IntPtr)index * (IntPtr)sizeof(DataType) / (IntPtr)sizeof(DataType)];
				}
			}
		}

		public static FixedAttachmentArray<DataType> Empty = new FixedAttachmentArray<DataType>(0);

		public const int MaxAttachments = 8;

		private DataType a0;

		private DataType a1;

		private DataType a2;

		private DataType a3;

		private DataType a4;

		private DataType a5;

		private DataType a6;

		private DataType a7;

		private int activeAttachments;
	}
}
