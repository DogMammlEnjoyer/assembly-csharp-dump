using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.XR.CoreUtils.Bindings.Variables
{
	[Serializable]
	public abstract class BindableVariableBase<T> : IReadOnlyBindableVariable<T>
	{
		private event Action<T> valueUpdated;

		public T Value
		{
			get
			{
				return this.m_InternalValue;
			}
			set
			{
				if (this.SetValueWithoutNotify(value))
				{
					this.BroadcastValue();
				}
			}
		}

		public int BindingCount
		{
			get
			{
				return this.m_BindingCount;
			}
		}

		public bool SetValueWithoutNotify(T value)
		{
			if (this.m_BindingCount == 0)
			{
				this.m_IsInitialized = true;
				this.m_InternalValue = value;
				return false;
			}
			if (this.m_IsInitialized && this.m_CheckEquality)
			{
				Func<T, T, bool> equalityMethod = this.m_EqualityMethod;
				if ((equalityMethod != null) ? equalityMethod(this.m_InternalValue, value) : this.ValueEquals(value))
				{
					return false;
				}
			}
			this.m_IsInitialized = true;
			this.m_InternalValue = value;
			return true;
		}

		public IEventBinding Subscribe(Action<T> callback)
		{
			EventBinding eventBinding = default(EventBinding);
			if (callback != null)
			{
				Action<T> callbackReference = callback;
				eventBinding.BindAction = delegate()
				{
					this.valueUpdated += callbackReference;
					this.IncrementReferenceCount();
				};
				eventBinding.UnbindAction = delegate()
				{
					this.valueUpdated -= callbackReference;
					this.DecrementReferenceCount();
				};
				eventBinding.Bind();
			}
			return eventBinding;
		}

		public IEventBinding SubscribeAndUpdate(Action<T> callback)
		{
			if (callback != null)
			{
				callback(this.m_InternalValue);
			}
			return this.Subscribe(callback);
		}

		public void Unsubscribe(Action<T> callback)
		{
			if (callback != null)
			{
				this.valueUpdated -= callback;
				this.DecrementReferenceCount();
			}
		}

		private void IncrementReferenceCount()
		{
			this.m_BindingCount++;
		}

		private void DecrementReferenceCount()
		{
			this.m_BindingCount = Mathf.Max(0, this.m_BindingCount - 1);
		}

		protected BindableVariableBase(T initialValue = default(T), bool checkEquality = true, Func<T, T, bool> equalityMethod = null, bool startInitialized = false)
		{
			this.m_IsInitialized = startInitialized;
			this.m_InternalValue = initialValue;
			this.m_CheckEquality = checkEquality;
			this.m_EqualityMethod = equalityMethod;
			this.m_BindingCount = 0;
		}

		public void BroadcastValue()
		{
			Action<T> action = this.valueUpdated;
			if (action == null)
			{
				return;
			}
			action(this.m_InternalValue);
		}

		public Task<T> Task(Func<T, bool> awaitPredicate, CancellationToken token = default(CancellationToken))
		{
			if (awaitPredicate != null && awaitPredicate(this.m_InternalValue))
			{
				return System.Threading.Tasks.Task.FromResult<T>(this.m_InternalValue);
			}
			return new BindableVariableTaskPredicate<T>(this, awaitPredicate, token).Task;
		}

		public Task<T> Task(T awaitState, CancellationToken token = default(CancellationToken))
		{
			if (this.ValueEquals(awaitState))
			{
				return System.Threading.Tasks.Task.FromResult<T>(this.m_InternalValue);
			}
			return new BindableVariableTaskState<T>(this, awaitState, token).task;
		}

		public virtual bool ValueEquals(T other)
		{
			return this.m_InternalValue.Equals(other);
		}

		private T m_InternalValue;

		private readonly bool m_CheckEquality;

		private bool m_IsInitialized;

		private readonly Func<T, T, bool> m_EqualityMethod;

		private int m_BindingCount;
	}
}
