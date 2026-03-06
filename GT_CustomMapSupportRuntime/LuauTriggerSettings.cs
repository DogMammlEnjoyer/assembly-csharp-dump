using System;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	[RequireComponent(typeof(Collider))]
	public class LuauTriggerSettings : TriggerSettings
	{
		public override void PropagateProperties()
		{
			this.syncedToAllPlayers_private = this.syncedToAllPlayers;
		}

		[Tooltip("Should this Trigger sync to all players, or only be processed for the person who triggered it?\nLuau Triggers generally shouldn't need to do this, but doing so will sync it's internal TriggerCount to all players.")]
		public bool syncedToAllPlayers;
	}
}
