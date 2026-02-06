using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fusion
{
	[Serializable]
	public class SimulationConfig
	{
		public bool SchedulingEnabled
		{
			get
			{
				return (this.ReplicationFeatures & NetworkProjectConfig.ReplicationFeatures.Scheduling) == NetworkProjectConfig.ReplicationFeatures.Scheduling;
			}
		}

		public bool AreaOfInterestEnabled
		{
			get
			{
				return (this.ReplicationFeatures & NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement) == NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement;
			}
		}

		public bool SchedulingWithoutAOI
		{
			get
			{
				return this.SchedulingEnabled && !this.AreaOfInterestEnabled;
			}
		}

		public int InputTotalWordCount
		{
			get
			{
				return this.InputDataWordCount + 4;
			}
		}

		internal SimulationConfig Init(int? playerCountOverride, int? inputWordCount)
		{
			SimulationConfig simulationConfig = this.Copy();
			bool flag = playerCountOverride != null;
			if (flag)
			{
				simulationConfig.PlayerCount = playerCountOverride.Value;
			}
			bool flag2 = inputWordCount != null;
			if (flag2)
			{
				simulationConfig.InputDataWordCount = inputWordCount.Value;
			}
			return simulationConfig;
		}

		internal SimulationConfig Copy()
		{
			return (SimulationConfig)base.MemberwiseClone();
		}

		[HideInInspector]
		[InlineHelp]
		public int InputDataWordCount;

		public NetworkProjectConfig.ReplicationFeatures ReplicationFeatures = NetworkProjectConfig.ReplicationFeatures.Scheduling;

		[FormerlySerializedAs("inputTransferMode")]
		[InlineHelp]
		public SimulationConfig.InputTransferModes InputTransferMode;

		[NonSerialized]
		public SimulationConfig.DataConsistency ObjectDataConsistency;

		[InlineHelp]
		public SimulationConfig.SimulationTimeMode SimulationUpdateTimeMode = SimulationConfig.SimulationTimeMode.UnscaledDeltaTime;

		[FormerlySerializedAs("DefaultPlayerCount")]
		[FormerlySerializedAs("DefaultPlayers")]
		[FormerlySerializedAs("Players")]
		[Unit(Units.None)]
		[InlineHelp]
		[RangeEx(1.0, 255.0)]
		public int PlayerCount = 10;

		public TickRate.Selection TickRateSelection = TickRate.Default;

		[NonSerialized]
		public Topologies Topology;

		[NonSerialized]
		public bool HostMigration;

		[HideInInspector]
		public byte MaxObjectDestroysSentPerPacket = 32;

		internal bool EnableSerializers = true;

		public enum InputTransferModes
		{
			Redundancy,
			RedundancyUncompressed = 2,
			LatestState = 1
		}

		public enum DataConsistency
		{
			Full,
			Eventual
		}

		public enum SimulationTimeMode
		{
			UnscaledDeltaTime,
			DeltaTime
		}
	}
}
