using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

[NetworkStructWeaved(21)]
[StructLayout(LayoutKind.Explicit, Size = 84)]
public struct GhostLabData : INetworkStruct
{
	public int DoorState { readonly get; set; }

	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterNetworkBool))]
	[NetworkedWeaved(1, 20)]
	public NetworkArray<NetworkBool> OpenDoors
	{
		get
		{
			return new NetworkArray<NetworkBool>(Native.ReferenceToPointer<FixedStorage@20>(ref this._OpenDoors), 20, ElementReaderWriterNetworkBool.GetInstance());
		}
	}

	public GhostLabData(int state, bool[] openDoors)
	{
		this.DoorState = state;
		for (int i = 0; i < openDoors.Length; i++)
		{
			bool val = openDoors[i];
			this.OpenDoors.Set(i, val);
		}
	}

	[FixedBufferProperty(typeof(NetworkArray<NetworkBool>), typeof(UnityArraySurrogate@ElementReaderWriterNetworkBool), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@20 _OpenDoors;
}
