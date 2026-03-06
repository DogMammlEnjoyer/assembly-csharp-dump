using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	[NullableContext(2)]
	[Nullable(0)]
	public class GrabbableEntity : MapEntity
	{
		public override long GetPackedCreateData()
		{
			return (long)((ulong)this.entityTypeId + (ulong)((long)((long)this.lua_EntityID << 8)));
		}

		public static void UnpackCreateData(long data, out byte entityTypeID, out short luaAgentID)
		{
			entityTypeID = (byte)(data & 255L);
			luaAgentID = (short)(data >> 8 & 65535L);
		}

		public AudioSource audioSource;

		public AudioClip catchSound;

		public float catchSoundVolume;

		public AudioClip throwSound;

		public float throwSoundVolume;
	}
}
