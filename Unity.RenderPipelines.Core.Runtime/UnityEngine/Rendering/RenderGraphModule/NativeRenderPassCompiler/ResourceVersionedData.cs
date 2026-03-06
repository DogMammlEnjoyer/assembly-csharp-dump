using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler
{
	internal struct ResourceVersionedData
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetWritingPass(CompilerContextData ctx, ResourceHandle h, int passId)
		{
			this.writePassId = passId;
			this.written = true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RegisterReadingPass(CompilerContextData ctx, ResourceHandle h, int passId, int index)
		{
			ctx.resources.readerData[h.iType][ctx.resources.IndexReader(h, this.numReaders)] = new ResourceReaderData
			{
				passId = passId,
				inputSlot = index
			};
			this.numReaders++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveReadingPass(CompilerContextData ctx, ResourceHandle h, int passId)
		{
			int i = 0;
			while (i < this.numReaders)
			{
				ref ResourceReaderData ptr = ref ctx.resources.readerData[h.iType].ElementAt(ctx.resources.IndexReader(h, i));
				if (ptr.passId == passId)
				{
					if (i < this.numReaders - 1)
					{
						ptr = ctx.resources.readerData[h.iType][ctx.resources.IndexReader(h, this.numReaders - 1)];
					}
					this.numReaders--;
				}
				else
				{
					i++;
				}
			}
		}

		public bool written;

		public int writePassId;

		public int numReaders;
	}
}
