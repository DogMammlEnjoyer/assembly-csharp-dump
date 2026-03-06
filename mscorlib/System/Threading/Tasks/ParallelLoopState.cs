using System;
using System.Diagnostics;
using Unity;

namespace System.Threading.Tasks
{
	/// <summary>Enables iterations of parallel loops to interact with other iterations. An instance of this class is provided by the <see cref="T:System.Threading.Tasks.Parallel" /> class to each loop; you can not create instances in your code.</summary>
	[DebuggerDisplay("ShouldExitCurrentIteration = {ShouldExitCurrentIteration}")]
	public class ParallelLoopState
	{
		internal ParallelLoopState(ParallelLoopStateFlags fbase)
		{
			this._flagsBase = fbase;
		}

		internal virtual bool InternalShouldExitCurrentIteration
		{
			get
			{
				throw new NotSupportedException("This method is not supported.");
			}
		}

		/// <summary>Gets whether the current iteration of the loop should exit based on requests made by this or other iterations.</summary>
		/// <returns>
		///   <see langword="true" /> if the current iteration should exit; otherwise, <see langword="false" />.</returns>
		public bool ShouldExitCurrentIteration
		{
			get
			{
				return this.InternalShouldExitCurrentIteration;
			}
		}

		/// <summary>Gets whether any iteration of the loop has called the <see cref="M:System.Threading.Tasks.ParallelLoopState.Stop" /> method.</summary>
		/// <returns>
		///   <see langword="true" /> if any iteration has stopped the loop by calling the <see cref="M:System.Threading.Tasks.ParallelLoopState.Stop" /> method; otherwise, <see langword="false" />.</returns>
		public bool IsStopped
		{
			get
			{
				return (this._flagsBase.LoopStateFlags & 4) != 0;
			}
		}

		/// <summary>Gets whether any iteration of the loop has thrown an exception that went unhandled by that iteration.</summary>
		/// <returns>
		///   <see langword="true" /> if an unhandled exception was thrown; otherwise, <see langword="false" />.</returns>
		public bool IsExceptional
		{
			get
			{
				return (this._flagsBase.LoopStateFlags & 1) != 0;
			}
		}

		internal virtual long? InternalLowestBreakIteration
		{
			get
			{
				throw new NotSupportedException("This method is not supported.");
			}
		}

		/// <summary>Gets the lowest iteration of the loop from which <see cref="M:System.Threading.Tasks.ParallelLoopState.Break" /> was called.</summary>
		/// <returns>The lowest iteration from which <see cref="M:System.Threading.Tasks.ParallelLoopState.Break" /> was called. In the case of a <see cref="M:System.Threading.Tasks.Parallel.ForEach``1(System.Collections.Concurrent.Partitioner{``0},System.Action{``0})" /> loop, the value is based on an internally-generated index.</returns>
		public long? LowestBreakIteration
		{
			get
			{
				return this.InternalLowestBreakIteration;
			}
		}

		/// <summary>Communicates that the <see cref="T:System.Threading.Tasks.Parallel" /> loop should cease execution at the system's earliest convenience.</summary>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="M:System.Threading.Tasks.ParallelLoopState.Break" /> method was called previously. <see cref="M:System.Threading.Tasks.ParallelLoopState.Break" /> and <see cref="M:System.Threading.Tasks.ParallelLoopState.Stop" /> may not be used in combination by iterations of the same loop.</exception>
		public void Stop()
		{
			this._flagsBase.Stop();
		}

		internal virtual void InternalBreak()
		{
			throw new NotSupportedException("This method is not supported.");
		}

		/// <summary>Communicates that the <see cref="T:System.Threading.Tasks.Parallel" /> loop should cease execution of iterations beyond the current iteration at the system's earliest convenience.</summary>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="M:System.Threading.Tasks.ParallelLoopState.Stop" /> method was previously called. <see cref="M:System.Threading.Tasks.ParallelLoopState.Break" /> and <see cref="M:System.Threading.Tasks.ParallelLoopState.Stop" /> may not be used in combination by iterations of the same loop.</exception>
		public void Break()
		{
			this.InternalBreak();
		}

		internal static void Break(int iteration, ParallelLoopStateFlags32 pflags)
		{
			int num = 0;
			if (pflags.AtomicLoopStateUpdate(2, 13, ref num))
			{
				int lowestBreakIteration = pflags._lowestBreakIteration;
				if (iteration < lowestBreakIteration)
				{
					SpinWait spinWait = default(SpinWait);
					while (Interlocked.CompareExchange(ref pflags._lowestBreakIteration, iteration, lowestBreakIteration) != lowestBreakIteration)
					{
						spinWait.SpinOnce();
						lowestBreakIteration = pflags._lowestBreakIteration;
						if (iteration > lowestBreakIteration)
						{
							break;
						}
					}
				}
				return;
			}
			if ((num & 4) != 0)
			{
				throw new InvalidOperationException("Break was called after Stop was called.");
			}
		}

		internal static void Break(long iteration, ParallelLoopStateFlags64 pflags)
		{
			int num = 0;
			if (pflags.AtomicLoopStateUpdate(2, 13, ref num))
			{
				long lowestBreakIteration = pflags.LowestBreakIteration;
				if (iteration < lowestBreakIteration)
				{
					SpinWait spinWait = default(SpinWait);
					while (Interlocked.CompareExchange(ref pflags._lowestBreakIteration, iteration, lowestBreakIteration) != lowestBreakIteration)
					{
						spinWait.SpinOnce();
						lowestBreakIteration = pflags.LowestBreakIteration;
						if (iteration > lowestBreakIteration)
						{
							break;
						}
					}
				}
				return;
			}
			if ((num & 4) != 0)
			{
				throw new InvalidOperationException("Break was called after Stop was called.");
			}
		}

		internal ParallelLoopState()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly ParallelLoopStateFlags _flagsBase;
	}
}
