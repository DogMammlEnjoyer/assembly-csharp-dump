using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Fusion
{
	[NetworkStructWeaved(22)]
	[StructLayout(LayoutKind.Explicit)]
	public struct NetworkCCData : INetworkStruct
	{
		public bool Grounded
		{
			get
			{
				return this._grounded == 1;
			}
			set
			{
				this._grounded = (value ? 1 : 0);
			}
		}

		public Vector3 Velocity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._velocityData;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this._velocityData = value;
			}
		}

		public const int WORDS = 18;

		public const int SIZE = 72;

		[FieldOffset(0)]
		public NetworkTRSPData TRSPData;

		[FieldOffset(56)]
		private int _grounded;

		[FieldOffset(60)]
		private Vector3Compressed _velocityData;
	}
}
