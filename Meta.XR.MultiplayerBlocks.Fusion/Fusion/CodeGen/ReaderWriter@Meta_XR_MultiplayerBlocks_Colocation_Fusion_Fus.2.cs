using System;
using System.Runtime.CompilerServices;
using Meta.XR.MultiplayerBlocks.Colocation.Fusion;

namespace Fusion.CodeGen
{
	[WeaverGenerated]
	internal struct ReaderWriter@Meta_XR_MultiplayerBlocks_Colocation_Fusion_FusionPlayer : IElementReaderWriter<FusionPlayer>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe FusionPlayer Read(byte* data, int index)
		{
			return *(FusionPlayer*)(data + index * 20);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe ref FusionPlayer ReadRef(byte* data, int index)
		{
			return ref *(FusionPlayer*)(data + index * 20);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, FusionPlayer val)
		{
			*(FusionPlayer*)(data + index * 20) = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 5;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public int GetElementHashCode(FusionPlayer val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<FusionPlayer> GetInstance()
		{
			if (ReaderWriter@Meta_XR_MultiplayerBlocks_Colocation_Fusion_FusionPlayer.Instance == null)
			{
				ReaderWriter@Meta_XR_MultiplayerBlocks_Colocation_Fusion_FusionPlayer.Instance = default(ReaderWriter@Meta_XR_MultiplayerBlocks_Colocation_Fusion_FusionPlayer);
			}
			return ReaderWriter@Meta_XR_MultiplayerBlocks_Colocation_Fusion_FusionPlayer.Instance;
		}

		[WeaverGenerated]
		public static IElementReaderWriter<FusionPlayer> Instance;
	}
}
