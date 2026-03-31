using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Internal;

namespace Cysharp.Threading.Tasks
{
	public static class Progress
	{
		public static IProgress<T> Create<T>(Action<T> handler)
		{
			if (handler == null)
			{
				return Progress.NullProgress<T>.Instance;
			}
			return new Progress.AnonymousProgress<T>(handler);
		}

		public static IProgress<T> CreateOnlyValueChanged<T>(Action<T> handler, IEqualityComparer<T> comparer = null)
		{
			if (handler == null)
			{
				return Progress.NullProgress<T>.Instance;
			}
			return new Progress.OnlyValueChangedProgress<T>(handler, comparer ?? UnityEqualityComparer.GetDefault<T>());
		}

		private sealed class NullProgress<T> : IProgress<T>
		{
			private NullProgress()
			{
			}

			public void Report(T value)
			{
			}

			public static readonly IProgress<T> Instance = new Progress.NullProgress<T>();
		}

		private sealed class AnonymousProgress<T> : IProgress<T>
		{
			public AnonymousProgress(Action<T> action)
			{
				this.action = action;
			}

			public void Report(T value)
			{
				this.action(value);
			}

			private readonly Action<T> action;
		}

		private sealed class OnlyValueChangedProgress<T> : IProgress<T>
		{
			public OnlyValueChangedProgress(Action<T> action, IEqualityComparer<T> comparer)
			{
				this.action = action;
				this.comparer = comparer;
				this.isFirstCall = true;
			}

			public void Report(T value)
			{
				if (this.isFirstCall)
				{
					this.isFirstCall = false;
				}
				else if (this.comparer.Equals(value, this.latestValue))
				{
					return;
				}
				this.latestValue = value;
				this.action(value);
			}

			private readonly Action<T> action;

			private readonly IEqualityComparer<T> comparer;

			private bool isFirstCall;

			private T latestValue;
		}
	}
}
