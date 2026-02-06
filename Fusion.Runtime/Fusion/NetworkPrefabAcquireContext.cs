using System;

namespace Fusion
{
	public readonly struct NetworkPrefabAcquireContext
	{
		public NetworkPrefabAcquireContext(NetworkPrefabId prefabId, NetworkObjectMeta meta = null, bool isSynchronous = true, bool dontDestroyOnLoad = false)
		{
			this.PrefabId = prefabId;
			this.Meta = meta;
			this.IsSynchronous = isSynchronous;
			this.DontDestroyOnLoad = dontDestroyOnLoad;
		}

		public bool HasHeader
		{
			get
			{
				return this.Meta != null;
			}
		}

		public Span<int> Data
		{
			get
			{
				return (this.Meta != null) ? this.Meta.Data : default(Span<int>);
			}
		}

		public readonly NetworkPrefabId PrefabId;

		public readonly NetworkObjectMeta Meta;

		public readonly bool IsSynchronous;

		public readonly bool DontDestroyOnLoad;
	}
}
