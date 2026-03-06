using System;
using System.Runtime.CompilerServices;
using Meta.XR.MultiplayerBlocks.Colocation.Fusion;

namespace Fusion.CodeGen
{
	[WeaverGenerated]
	internal struct ReaderWriter@Meta_XR_MultiplayerBlocks_Colocation_Fusion_FusionAnchor : IElementReaderWriter<FusionAnchor>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe FusionAnchor Read(byte* data, int index)
		{
			return *(FusionAnchor*)(data + index * 280);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe ref FusionAnchor ReadRef(byte* data, int index)
		{
			return ref *(FusionAnchor*)(data + index * 280);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, FusionAnchor val)
		{
			*(FusionAnchor*)(data + index * 280) = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 70;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public int GetElementHashCode(FusionAnchor val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<FusionAnchor> GetInstance()
		{
			if (ReaderWriter@Meta_XR_MultiplayerBlocks_Colocation_Fusion_FusionAnchor.Instance == null)
			{
				ReaderWriter@Meta_XR_MultiplayerBlocks_Colocation_Fusion_FusionAnchor.Instance = default(ReaderWriter@Meta_XR_MultiplayerBlocks_Colocation_Fusion_FusionAnchor);
			}
			return ReaderWriter@Meta_XR_MultiplayerBlocks_Colocation_Fusion_FusionAnchor.Instance;
		}

		[WeaverGenerated]
		public static IElementReaderWriter<FusionAnchor> Instance;
	}
}
