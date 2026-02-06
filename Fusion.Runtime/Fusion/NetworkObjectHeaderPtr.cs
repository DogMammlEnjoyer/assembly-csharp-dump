using System;

namespace Fusion
{
	public struct NetworkObjectHeaderPtr
	{
		public unsafe NetworkObjectHeaderPtr(NetworkObjectHeader* ptr)
		{
			this.Ptr = ptr;
		}

		public unsafe NetworkObjectTypeId Type
		{
			get
			{
				return this.Ptr->Type;
			}
		}

		public unsafe NetworkId Id
		{
			get
			{
				return this.Ptr->Id;
			}
		}

		public unsafe Span<int> Data
		{
			get
			{
				return new Span<int>((void*)(this.Ptr + (IntPtr)20 * 4 / (IntPtr)sizeof(NetworkObjectHeader)), (int)(this.Ptr->WordCount - 20));
			}
		}

		public unsafe NetworkObjectHeader* Ptr;
	}
}
