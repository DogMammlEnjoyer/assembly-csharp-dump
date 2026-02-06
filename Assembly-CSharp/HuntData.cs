using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

[NetworkStructWeaved(43)]
[StructLayout(LayoutKind.Explicit, Size = 172)]
public struct HuntData : INetworkStruct
{
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(3, 20)]
	public NetworkArray<int> currentHuntedArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@20>(ref this._currentHuntedArray), 20, ElementReaderWriterInt32.GetInstance());
		}
	}

	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(23, 20)]
	public NetworkArray<int> currentTargetArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@20>(ref this._currentTargetArray), 20, ElementReaderWriterInt32.GetInstance());
		}
	}

	[FieldOffset(0)]
	public NetworkBool huntStarted;

	[FieldOffset(4)]
	public NetworkBool waitingToStartNextHuntGame;

	[FieldOffset(8)]
	public int countDownTime;

	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(12)]
	private FixedStorage@20 _currentHuntedArray;

	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(92)]
	private FixedStorage@20 _currentTargetArray;
}
