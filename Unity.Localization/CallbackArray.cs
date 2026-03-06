using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.Localization
{
	internal struct CallbackArray<TDelegate> where TDelegate : Delegate
	{
		public TDelegate SingleDelegate
		{
			get
			{
				return this.m_SingleDelegate;
			}
		}

		public TDelegate[] MultiDelegates
		{
			get
			{
				return this.m_MultipleDelegates;
			}
		}

		public int Length
		{
			get
			{
				return this.m_Length;
			}
		}

		public void Add(TDelegate callback, int capacityIncrement = 5)
		{
			if (callback == null)
			{
				return;
			}
			if (this.m_CannotMutateCallbacksArray)
			{
				if (this.m_AddCallbacks == null)
				{
					this.m_AddCallbacks = CollectionPool<List<TDelegate>, TDelegate>.Get();
				}
				this.m_AddCallbacks.Add(callback);
				this.m_MutatedDuringCallback = true;
				return;
			}
			if (this.m_Length == 0)
			{
				this.m_SingleDelegate = callback;
			}
			else if (this.m_Length == 1)
			{
				this.m_MultipleDelegates = new TDelegate[capacityIncrement];
				this.m_MultipleDelegates[0] = this.m_SingleDelegate;
				this.m_MultipleDelegates[1] = callback;
				this.m_SingleDelegate = default(TDelegate);
			}
			else
			{
				if (this.m_MultipleDelegates.Length == this.m_Length)
				{
					Array.Resize<TDelegate>(ref this.m_MultipleDelegates, this.m_Length + capacityIncrement);
				}
				this.m_MultipleDelegates[this.m_Length] = callback;
			}
			this.m_Length++;
		}

		public void RemoveByMovingTail(TDelegate callback)
		{
			if (callback == null)
			{
				return;
			}
			if (this.m_CannotMutateCallbacksArray)
			{
				if (this.m_RemoveCallbacks == null)
				{
					this.m_RemoveCallbacks = CollectionPool<List<TDelegate>, TDelegate>.Get();
				}
				this.m_RemoveCallbacks.Add(callback);
				this.m_MutatedDuringCallback = true;
				return;
			}
			if (this.m_Length <= 1)
			{
				if (object.Equals(this.m_SingleDelegate, callback))
				{
					this.m_SingleDelegate = default(TDelegate);
					this.m_Length = 0;
					return;
				}
			}
			else
			{
				for (int i = 0; i < this.m_Length; i++)
				{
					if (object.Equals(this.m_MultipleDelegates[i], callback))
					{
						this.m_MultipleDelegates[i] = this.m_MultipleDelegates[this.m_Length - 1];
						this.m_Length--;
						break;
					}
				}
				if (this.m_Length == 1)
				{
					this.m_SingleDelegate = this.m_MultipleDelegates[0];
					this.m_MultipleDelegates = null;
				}
			}
		}

		public void LockForChanges()
		{
			this.m_CannotMutateCallbacksArray = true;
		}

		public void UnlockForChanges()
		{
			this.m_CannotMutateCallbacksArray = false;
			if (this.m_MutatedDuringCallback)
			{
				if (this.m_AddCallbacks != null)
				{
					foreach (TDelegate callback in this.m_AddCallbacks)
					{
						this.Add(callback, 5);
					}
					CollectionPool<List<TDelegate>, TDelegate>.Release(this.m_AddCallbacks);
					this.m_AddCallbacks = null;
				}
				if (this.m_RemoveCallbacks != null)
				{
					foreach (TDelegate callback2 in this.m_RemoveCallbacks)
					{
						this.RemoveByMovingTail(callback2);
					}
					CollectionPool<List<TDelegate>, TDelegate>.Release(this.m_RemoveCallbacks);
					this.m_RemoveCallbacks = null;
				}
				this.m_MutatedDuringCallback = true;
			}
		}

		public void Clear()
		{
			this.m_SingleDelegate = default(TDelegate);
			this.m_MultipleDelegates = null;
			this.m_Length = 0;
		}

		private const int k_AllocationIncrement = 5;

		private TDelegate m_SingleDelegate;

		private TDelegate[] m_MultipleDelegates;

		private List<TDelegate> m_AddCallbacks;

		private List<TDelegate> m_RemoveCallbacks;

		private int m_Length;

		private bool m_CannotMutateCallbacksArray;

		private bool m_MutatedDuringCallback;
	}
}
