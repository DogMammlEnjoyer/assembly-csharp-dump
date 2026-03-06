using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	[RequireComponent(typeof(Collider))]
	[DisallowMultipleComponent]
	public class TeleporterSettings : TriggerSettings
	{
		public override void PropagateProperties()
		{
			this.syncedToAllPlayers_private = this.syncedToAllPlayers;
		}

		[Tooltip("Should this Trigger sync to all players, or only be processed for the person who triggered it?\nTeleporters generally shouldn't need to do this, but doing so will sync it's internal TriggerCount to all players.")]
		public bool syncedToAllPlayers;

		[Nullable(1)]
		[Tooltip("Teleport points used for this teleporter. Chosen at random.")]
		[SerializeField]
		public List<Transform> TeleportPoints = new List<Transform>();

		[Tooltip("Should the teleporter change the players rotation to match the chosen Teleport Point's rotation?")]
		[SerializeField]
		public bool matchTeleportPointRotation = true;

		[Tooltip("Should the teleporter maintain the players current velocity after teleporting?")]
		[SerializeField]
		public bool maintainVelocity = true;
	}
}
