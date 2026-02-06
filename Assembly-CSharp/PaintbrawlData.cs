using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

[NetworkStructWeaved(61)]
[StructLayout(LayoutKind.Explicit, Size = 244)]
public struct PaintbrawlData : INetworkStruct
{
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(1, 20)]
	public NetworkArray<int> playerLivesArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@20>(ref this._playerLivesArray), 20, ElementReaderWriterInt32.GetInstance());
		}
	}

	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(21, 20)]
	public NetworkArray<int> playerActorNumberArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@20>(ref this._playerActorNumberArray), 20, ElementReaderWriterInt32.GetInstance());
		}
	}

	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus))]
	[NetworkedWeaved(41, 20)]
	public NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus> playerStatusArray
	{
		get
		{
			return new NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus>(Native.ReferenceToPointer<FixedStorage@20>(ref this._playerStatusArray), 20, ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus.GetInstance());
		}
	}

	[FieldOffset(0)]
	public GorillaPaintbrawlManager.PaintbrawlState currentPaintbrawlState;

	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@20 _playerLivesArray;

	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(84)]
	private FixedStorage@20 _playerActorNumberArray;

	[FixedBufferProperty(typeof(NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus>), typeof(UnityArraySurrogate@ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(164)]
	private FixedStorage@20 _playerStatusArray;
}
