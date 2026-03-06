using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.XR.CoreUtils.Bindings.Variables
{
	internal struct BindableVariableTaskPredicate<T>
	{
		public Task<T> Task
		{
			get
			{
				return this.m_Tcs.Task;
			}
		}

		public BindableVariableTaskPredicate(IReadOnlyBindableVariable<T> bindableVariable, Func<T, bool> awaitPredicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			this.m_Tcs = new TaskCompletionSource<T>();
			this.m_AwaitPredicate = awaitPredicate;
			this.m_BindableVariable = bindableVariable;
			if (this.m_AwaitPredicate != null && this.m_AwaitPredicate(this.m_BindableVariable.Value))
			{
				this.m_Tcs.SetResult(this.m_BindableVariable.Value);
				return;
			}
			cancellationToken.Register(new Action(this.Cancelled));
			this.m_BindableVariable.Subscribe(new Action<T>(this.Await));
		}

		private void Cancelled()
		{
			this.m_BindableVariable.Unsubscribe(new Action<T>(this.Await));
			this.m_Tcs.SetResult(this.m_BindableVariable.Value);
		}

		private void Await(T state)
		{
			if (this.m_AwaitPredicate != null)
			{
				if (this.m_AwaitPredicate(state))
				{
					this.m_BindableVariable.Unsubscribe(new Action<T>(this.Await));
					this.m_Tcs.SetResult(state);
					return;
				}
			}
			else
			{
				this.m_BindableVariable.Unsubscribe(new Action<T>(this.Await));
				this.m_Tcs.SetResult(state);
			}
		}

		private readonly TaskCompletionSource<T> m_Tcs;

		private readonly Func<T, bool> m_AwaitPredicate;

		private readonly IReadOnlyBindableVariable<T> m_BindableVariable;
	}
}
