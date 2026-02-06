using System;

namespace Fusion
{
	[Flags]
	public enum NetworkObjectFlags
	{
		None = 0,
		MaskVersion = 255,
		V1 = 1,
		Ignore = 65536,
		MasterClientObject = 131072,
		DestroyWhenStateAuthorityLeaves = 262144,
		AllowStateAuthorityOverride = 524288
	}
}
