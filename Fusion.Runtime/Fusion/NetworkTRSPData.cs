using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Fusion
{
	[NetworkStructWeaved(14)]
	[StructLayout(LayoutKind.Explicit)]
	public struct NetworkTRSPData : INetworkStruct
	{
		public static NetworkBehaviourId NonNetworkedParent
		{
			get
			{
				NetworkBehaviourId result;
				result.Object = default(NetworkId);
				result.Behaviour = 1;
				return result;
			}
		}

		public const int WORDS = 14;

		public const int SIZE = 56;

		public const int POSITION_OFFSET = 2;

		[FieldOffset(0)]
		public NetworkBehaviourId Parent;

		[FieldOffset(8)]
		public Vector3 Position;

		[FieldOffset(20)]
		public Quaternion Rotation;

		[FieldOffset(36)]
		public Vector3Compressed Scale;

		[FieldOffset(48)]
		public int TeleportKey;

		[FieldOffset(52)]
		public NetworkId AreaOfInterestOverride;
	}
}
