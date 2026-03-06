using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	internal class PathReturnQueue
	{
		public PathReturnQueue(object pathsClaimedSilentlyBy)
		{
			this.pathsClaimedSilentlyBy = pathsClaimedSilentlyBy;
		}

		public void Enqueue(Path path)
		{
			Queue<Path> obj = this.pathReturnQueue;
			lock (obj)
			{
				this.pathReturnQueue.Enqueue(path);
			}
		}

		public void ReturnPaths(bool timeSlice)
		{
			long num = timeSlice ? (DateTime.UtcNow.Ticks + 10000L) : 0L;
			int num2 = 0;
			for (;;)
			{
				Queue<Path> obj = this.pathReturnQueue;
				Path path;
				lock (obj)
				{
					if (this.pathReturnQueue.Count == 0)
					{
						break;
					}
					path = this.pathReturnQueue.Dequeue();
				}
				try
				{
					((IPathInternals)path).ReturnPath();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
				((IPathInternals)path).AdvanceState(PathState.Returned);
				path.Release(this.pathsClaimedSilentlyBy, true);
				num2++;
				if (num2 > 5 && timeSlice)
				{
					num2 = 0;
					if (DateTime.UtcNow.Ticks >= num)
					{
						break;
					}
				}
			}
		}

		private Queue<Path> pathReturnQueue = new Queue<Path>();

		private object pathsClaimedSilentlyBy;
	}
}
