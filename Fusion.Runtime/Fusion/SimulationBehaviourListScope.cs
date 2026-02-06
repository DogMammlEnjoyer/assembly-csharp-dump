using System;

namespace Fusion
{
	public struct SimulationBehaviourListScope : IDisposable
	{
		internal SimulationBehaviourListScope(SimulationBehaviourUpdater.BehaviourList list)
		{
			this._list = list;
			this._list.LockCount++;
		}

		public void Dispose()
		{
			SimulationBehaviourUpdater.BehaviourList list = this._list;
			int num = list.LockCount - 1;
			list.LockCount = num;
			bool flag = num == 0;
			if (flag)
			{
				this._list.RemoveAllPending();
			}
		}

		private SimulationBehaviourUpdater.BehaviourList _list;
	}
}
