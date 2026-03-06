using System;

namespace Fusion.Protocol
{
	internal enum ProtocolMessageVersion : byte
	{
		Invalid,
		V1_0_0,
		V1_1_0,
		V1_2_0,
		V1_2_1,
		V1_2_2,
		V1_2_3,
		V1_3_0,
		V1_4_0,
		V1_5_0,
		V1_6_0,
		LATEST = 10
	}
}
