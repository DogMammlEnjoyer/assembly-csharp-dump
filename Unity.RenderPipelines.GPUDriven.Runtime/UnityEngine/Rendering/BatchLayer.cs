using System;

namespace UnityEngine.Rendering
{
	public class BatchLayer
	{
		public const byte InstanceCullingDirect = 29;

		public const byte InstanceCullingIndirect = 28;

		public const uint InstanceCullingDirectMask = 536870912U;

		public const uint InstanceCullingIndirectMask = 268435456U;

		public const uint InstanceCullingMask = 805306368U;
	}
}
