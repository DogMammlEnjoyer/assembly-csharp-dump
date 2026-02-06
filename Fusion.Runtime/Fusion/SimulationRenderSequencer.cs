using System;

namespace Fusion
{
	internal struct SimulationRenderSequencer
	{
		public bool ConsumeRenderUpdate(NetworkRunner runner)
		{
			return this.ConsumeRenderUpdate(runner._simulation);
		}

		public bool ConsumeRenderUpdate(Simulation simulation)
		{
			bool flag = simulation.InterpolateSequence != this._sequence;
			bool flag2 = flag;
			if (flag2)
			{
				this._sequence = simulation.InterpolateSequence;
			}
			return flag;
		}

		private ulong _sequence;
	}
}
