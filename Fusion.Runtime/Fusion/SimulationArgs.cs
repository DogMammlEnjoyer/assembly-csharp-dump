using System;
using Fusion.Sockets;

namespace Fusion
{
	internal struct SimulationArgs
	{
		public bool IsPlayer
		{
			get
			{
				return this.Mode == SimulationModes.Client || this.Mode == SimulationModes.Host;
			}
		}

		public bool IsServer
		{
			get
			{
				return this.Mode == SimulationModes.Server || this.Mode == SimulationModes.Host;
			}
		}

		public SimulationModes Mode;

		public NetAddress Address;

		public INetSocket Socket;

		public NetworkProjectConfig Config;

		public Simulation.ICallbacks Callbacks;

		public Tick ResumeTick;

		public byte[] ResumeState;

		public NetworkId ResumeNetworkId;
	}
}
