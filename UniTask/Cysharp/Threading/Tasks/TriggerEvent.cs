using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks
{
	public struct TriggerEvent<T>
	{
		private void LogError(Exception ex)
		{
			Debug.LogException(ex);
		}

		public void SetResult(T value)
		{
			if (this.iteratingNode != null)
			{
				throw new InvalidOperationException("Can not trigger itself in iterating.");
			}
			ITriggerHandler<T> triggerHandler = this.head;
			while (triggerHandler != null)
			{
				this.iteratingNode = triggerHandler;
				try
				{
					triggerHandler.OnNext(value);
				}
				catch (Exception ex)
				{
					this.LogError(ex);
					this.Remove(triggerHandler);
				}
				if (this.preserveRemoveSelf)
				{
					this.preserveRemoveSelf = false;
					this.iteratingNode = null;
					ITriggerHandler<T> next = triggerHandler.Next;
					this.Remove(triggerHandler);
					triggerHandler = next;
				}
				else
				{
					triggerHandler = triggerHandler.Next;
				}
			}
			this.iteratingNode = null;
			if (this.iteratingHead != null)
			{
				this.Add(this.iteratingHead);
				this.iteratingHead = null;
			}
		}

		public void SetCanceled(CancellationToken cancellationToken)
		{
			if (this.iteratingNode != null)
			{
				throw new InvalidOperationException("Can not trigger itself in iterating.");
			}
			ITriggerHandler<T> next;
			for (ITriggerHandler<T> triggerHandler = this.head; triggerHandler != null; triggerHandler = next)
			{
				this.iteratingNode = triggerHandler;
				try
				{
					triggerHandler.OnCanceled(cancellationToken);
				}
				catch (Exception ex)
				{
					this.LogError(ex);
				}
				this.preserveRemoveSelf = false;
				this.iteratingNode = null;
				next = triggerHandler.Next;
				this.Remove(triggerHandler);
			}
			this.iteratingNode = null;
			if (this.iteratingHead != null)
			{
				this.Add(this.iteratingHead);
				this.iteratingHead = null;
			}
		}

		public void SetCompleted()
		{
			if (this.iteratingNode != null)
			{
				throw new InvalidOperationException("Can not trigger itself in iterating.");
			}
			ITriggerHandler<T> next;
			for (ITriggerHandler<T> triggerHandler = this.head; triggerHandler != null; triggerHandler = next)
			{
				this.iteratingNode = triggerHandler;
				try
				{
					triggerHandler.OnCompleted();
				}
				catch (Exception ex)
				{
					this.LogError(ex);
				}
				this.preserveRemoveSelf = false;
				this.iteratingNode = null;
				next = triggerHandler.Next;
				this.Remove(triggerHandler);
			}
			this.iteratingNode = null;
			if (this.iteratingHead != null)
			{
				this.Add(this.iteratingHead);
				this.iteratingHead = null;
			}
		}

		public void SetError(Exception exception)
		{
			if (this.iteratingNode != null)
			{
				throw new InvalidOperationException("Can not trigger itself in iterating.");
			}
			ITriggerHandler<T> next;
			for (ITriggerHandler<T> triggerHandler = this.head; triggerHandler != null; triggerHandler = next)
			{
				this.iteratingNode = triggerHandler;
				try
				{
					triggerHandler.OnError(exception);
				}
				catch (Exception ex)
				{
					this.LogError(ex);
				}
				this.preserveRemoveSelf = false;
				this.iteratingNode = null;
				next = triggerHandler.Next;
				this.Remove(triggerHandler);
			}
			this.iteratingNode = null;
			if (this.iteratingHead != null)
			{
				this.Add(this.iteratingHead);
				this.iteratingHead = null;
			}
		}

		public void Add(ITriggerHandler<T> handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			if (this.head == null)
			{
				this.head = handler;
				return;
			}
			if (this.iteratingNode != null)
			{
				if (this.iteratingHead == null)
				{
					this.iteratingHead = handler;
					return;
				}
				ITriggerHandler<T> prev = this.iteratingHead.Prev;
				if (prev == null)
				{
					this.iteratingHead.Prev = handler;
					this.iteratingHead.Next = handler;
					handler.Prev = this.iteratingHead;
					return;
				}
				this.iteratingHead.Prev = handler;
				prev.Next = handler;
				handler.Prev = prev;
				return;
			}
			else
			{
				ITriggerHandler<T> prev2 = this.head.Prev;
				if (prev2 == null)
				{
					this.head.Prev = handler;
					this.head.Next = handler;
					handler.Prev = this.head;
					return;
				}
				this.head.Prev = handler;
				prev2.Next = handler;
				handler.Prev = prev2;
				return;
			}
		}

		public void Remove(ITriggerHandler<T> handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			if (this.iteratingNode != null && this.iteratingNode == handler)
			{
				this.preserveRemoveSelf = true;
				return;
			}
			ITriggerHandler<T> prev = handler.Prev;
			ITriggerHandler<T> next = handler.Next;
			if (next != null)
			{
				next.Prev = prev;
			}
			if (handler == this.head)
			{
				this.head = next;
			}
			else if (handler == this.iteratingHead)
			{
				this.iteratingHead = next;
			}
			else if (prev != null)
			{
				prev.Next = next;
			}
			if (this.head != null && this.head.Prev == handler)
			{
				if (prev != this.head)
				{
					this.head.Prev = prev;
				}
				else
				{
					this.head.Prev = null;
				}
			}
			if (this.iteratingHead != null && this.iteratingHead.Prev == handler)
			{
				if (prev != this.iteratingHead.Prev)
				{
					this.iteratingHead.Prev = prev;
				}
				else
				{
					this.iteratingHead.Prev = null;
				}
			}
			handler.Prev = null;
			handler.Next = null;
		}

		private ITriggerHandler<T> head;

		private ITriggerHandler<T> iteratingHead;

		private bool preserveRemoveSelf;

		private ITriggerHandler<T> iteratingNode;
	}
}
