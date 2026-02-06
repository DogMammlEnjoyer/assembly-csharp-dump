using System;
using System.Collections.Generic;
using JetBrains.Annotations;

public class GhostReactorLevelGeneratorV2
{
	[Serializable]
	public struct TreeLevelConfig
	{
		public bool ValidateDatetime([CanBeNull] string timestamp)
		{
			DateTime dateTime;
			return string.IsNullOrEmpty(timestamp) || DateTime.TryParse(timestamp, out dateTime);
		}

		[CanBeNull]
		public string EnableAfterDatetime;

		[CanBeNull]
		public string DisableAfterDatetime;

		public int minHubs;

		public int maxHubs;

		public int minCaps;

		public int maxCaps;

		public List<GhostReactorSpawnConfig> sectionSpawnConfigs;

		public List<GhostReactorSpawnConfig> endCapSpawnConfigs;

		public List<GhostReactorLevelSection> hubs;

		public List<GhostReactorLevelSection> endCaps;

		public List<GhostReactorLevelSection> blockers;

		public List<GhostReactorLevelSectionConnector> connectors;
	}
}
