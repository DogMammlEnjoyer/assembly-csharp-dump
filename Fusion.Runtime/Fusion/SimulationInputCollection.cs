using System;
using System.Collections.Generic;

namespace Fusion
{
	internal class SimulationInputCollection
	{
		public int Count
		{
			get
			{
				return this._count;
			}
		}

		public SimulationInputCollection(int playerCount)
		{
			this._byIndex = new SimulationInput[playerCount];
			this._byPlayer = new Dictionary<PlayerRef, SimulationInput>(PlayerRef.Comparer);
		}

		public SimulationInput GetByIndex(int index)
		{
			bool flag = index >= 0 && index < this._count;
			SimulationInput result;
			if (flag)
			{
				result = this._byIndex[index];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public SimulationInput GetByPlayer(PlayerRef player)
		{
			SimulationInput simulationInput;
			bool flag = this._byPlayer.TryGetValue(player, out simulationInput);
			SimulationInput result;
			if (flag)
			{
				result = simulationInput;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void Clear()
		{
			this._count = 0;
			Array.Clear(this._byIndex, 0, this._byIndex.Length);
			this._byPlayer.Clear();
		}

		public void AddInput(SimulationInput input)
		{
			int count = this._count;
			this._count = count + 1;
			int num = count;
			Assert.Check(this._byIndex[num] == null);
			Assert.Check(!this._byPlayer.ContainsKey(input.Player));
			this._byIndex[num] = input;
			this._byPlayer.Add(input.Player, input);
		}

		private int _count;

		private SimulationInput[] _byIndex;

		private Dictionary<PlayerRef, SimulationInput> _byPlayer;
	}
}
