using System;

namespace Fusion
{
	public sealed class NetworkObjectSpawnException : Exception
	{
		public NetworkObjectSpawnException(NetworkSpawnStatus status, NetworkObjectTypeId? id = null)
		{
			this.TypeId = id;
			this.Status = status;
		}

		public NetworkObjectTypeId? TypeId { get; }

		public NetworkSpawnStatus Status { get; }

		public override string Message
		{
			get
			{
				bool flag = this.Status == NetworkSpawnStatus.FailedToLoadPrefabSynchronously;
				string text;
				if (flag)
				{
					text = "Failed to load prefab synchronously. Use async spawn instead or enable EnqueueIncompleteSynchronousSpawns";
				}
				else
				{
					text = string.Format("Failed to spawn: {0}", this.Status);
				}
				bool flag2 = this.TypeId != null;
				if (flag2)
				{
					text += string.Format(" (prefab: {0})", this.TypeId);
				}
				return text;
			}
		}
	}
}
