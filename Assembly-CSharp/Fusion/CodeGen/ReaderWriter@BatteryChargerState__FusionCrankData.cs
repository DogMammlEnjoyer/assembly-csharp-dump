using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	[WeaverGenerated]
	internal struct ReaderWriter@BatteryChargerState__FusionCrankData : IElementReaderWriter<BatteryChargerState.FusionCrankData>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe BatteryChargerState.FusionCrankData Read(byte* data, int index)
		{
			return *(BatteryChargerState.FusionCrankData*)(data + index * 12);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe ref BatteryChargerState.FusionCrankData ReadRef(byte* data, int index)
		{
			return ref *(BatteryChargerState.FusionCrankData*)(data + index * 12);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, BatteryChargerState.FusionCrankData val)
		{
			*(BatteryChargerState.FusionCrankData*)(data + index * 12) = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public int GetElementHashCode(BatteryChargerState.FusionCrankData val)
		{
			return val.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<BatteryChargerState.FusionCrankData> GetInstance()
		{
			if (ReaderWriter@BatteryChargerState__FusionCrankData.Instance == null)
			{
				ReaderWriter@BatteryChargerState__FusionCrankData.Instance = default(ReaderWriter@BatteryChargerState__FusionCrankData);
			}
			return ReaderWriter@BatteryChargerState__FusionCrankData.Instance;
		}

		[WeaverGenerated]
		public static IElementReaderWriter<BatteryChargerState.FusionCrankData> Instance;
	}
}
