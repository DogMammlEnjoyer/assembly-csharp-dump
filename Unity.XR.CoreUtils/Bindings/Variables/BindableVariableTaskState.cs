using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.XR.CoreUtils.Bindings.Variables
{
	internal struct BindableVariableTaskState<T>
	{
		public Task<T> task
		{
			get
			{
				return this.m_Tcs.Task;
			}
		}

		public BindableVariableTaskState(IReadOnlyBindableVariable<T> bindableVariable, T awaitState, CancellationToken cancellationToken = default(CancellationToken))
		{
			this.m_Tcs = new TaskCompletionSource<T>();
			this.m_AwaitState = awaitState;
			this.m_BindableVariable = bindableVariable;
			if (this.m_BindableVariable.ValueEquals(awaitState))
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
			if (this.m_BindableVariable.ValueEquals(this.m_AwaitState))
			{
				this.m_BindableVariable.Unsubscribe(new Action<T>(this.Await));
				this.m_Tcs.SetResult(state);
			}
		}

		private readonly TaskCompletionSource<T> m_Tcs;

		private readonly T m_AwaitState;

		private readonly IReadOnlyBindableVariable<T> m_BindableVariable;
	}
}
