using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	[DisallowMultipleComponent]
	public class MapBoundarySettings : TriggerSettings
	{
		public override void PropagateProperties()
		{
			this.syncedToAllPlayers_private = this.syncedToAllPlayers;
		}

		[Tooltip("Should this Trigger sync to all players, or only be processed for the person who triggered it?\nMapBoundary triggers generally shouldn't need to do this, but doing so will sync it's internal TriggerCount to all players.")]
		public bool syncedToAllPlayers;

		[Nullable(1)]
		[Tooltip("Teleport points used to return the player to the map. Chosen at random.")]
		[SerializeField]
		public List<Transform> TeleportPoints = new List<Transform>();

		[Tooltip("Should the player get Tagged when they hit this Boundary?")]
		public bool ShouldTagPlayer = true;
	}
}
