using System;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	public interface ITriggerHandler<T>
	{
		void OnNext(T value);

		void OnError(Exception ex);

		void OnCompleted();

		void OnCanceled(CancellationToken cancellationToken);

		ITriggerHandler<T> Prev { get; set; }

		ITriggerHandler<T> Next { get; set; }
	}
}
