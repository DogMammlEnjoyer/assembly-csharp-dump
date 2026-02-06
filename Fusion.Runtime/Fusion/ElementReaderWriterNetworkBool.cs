using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal struct ElementReaderWriterNetworkBool : IElementReaderWriter<NetworkBool>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe NetworkBool Read(byte* data, int index)
		{
			return *(NetworkBool*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref NetworkBool ReadRef(byte* data, int index)
		{
			return ref *(NetworkBool*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, NetworkBool val)
		{
			*(NetworkBool*)(data + index * 4) = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementHashCode(NetworkBool val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<NetworkBool> GetInstance()
		{
			bool flag = ElementReaderWriterNetworkBool._instance == null;
			if (flag)
			{
				ElementReaderWriterNetworkBool._instance = default(ElementReaderWriterNetworkBool);
			}
			return ElementReaderWriterNetworkBool._instance;
		}

		private static IElementReaderWriter<NetworkBool> _instance;
	}
}
