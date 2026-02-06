using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal struct ElementReaderWriterNetworkBehaviourId : IElementReaderWriter<NetworkBehaviourId>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe NetworkBehaviourId Read(byte* data, int index)
		{
			return *(NetworkBehaviourId*)(data + index * 8);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref NetworkBehaviourId ReadRef(byte* data, int index)
		{
			return ref *(NetworkBehaviourId*)(data + index * 8);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, NetworkBehaviourId val)
		{
			*(NetworkBehaviourId*)(data + index * 8) = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementHashCode(NetworkBehaviourId val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<NetworkBehaviourId> GetInstance()
		{
			bool flag = ElementReaderWriterNetworkBehaviourId._instance == null;
			if (flag)
			{
				ElementReaderWriterNetworkBehaviourId._instance = default(ElementReaderWriterNetworkBehaviourId);
			}
			return ElementReaderWriterNetworkBehaviourId._instance;
		}

		private static IElementReaderWriter<NetworkBehaviourId> _instance;
	}
}
