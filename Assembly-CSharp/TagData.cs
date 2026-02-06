using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

[NetworkStructWeaved(22)]
[StructLayout(LayoutKind.Explicit, Size = 88)]
public struct TagData : INetworkStruct
{
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(2, 20)]
	public NetworkArray<int> infectedPlayerList
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@20>(ref this._infectedPlayerList), 20, ElementReaderWriterInt32.GetInstance());
		}
	}

	public int currentItID { readonly get; set; }

	[FieldOffset(4)]
	public NetworkBool isCurrentlyTag;

	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(8)]
	private FixedStorage@20 _infectedPlayerList;
}
