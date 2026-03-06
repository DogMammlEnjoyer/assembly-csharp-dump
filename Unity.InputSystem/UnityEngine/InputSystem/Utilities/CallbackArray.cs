using System;

namespace UnityEngine.InputSystem.Utilities
{
	internal struct CallbackArray<TDelegate> where TDelegate : Delegate
	{
		public int length
		{
			get
			{
				return this.m_Callbacks.length;
			}
		}

		public TDelegate this[int index]
		{
			get
			{
				return this.m_Callbacks[index];
			}
		}

		public void Clear()
		{
			this.m_Callbacks.Clear();
			this.m_CallbacksToAdd.Clear();
			this.m_CallbacksToRemove.Clear();
		}

		public void AddCallback(TDelegate dlg)
		{
			if (!this.m_CannotMutateCallbacksArray)
			{
				if (!this.m_Callbacks.Contains(dlg))
				{
					this.m_Callbacks.AppendWithCapacity(dlg, 4);
				}
				return;
			}
			if (this.m_CallbacksToAdd.Contains(dlg))
			{
				return;
			}
			int num = this.m_CallbacksToRemove.IndexOf(dlg);
			if (num != -1)
			{
				this.m_CallbacksToRemove.RemoveAtByMovingTailWithCapacity(num);
			}
			this.m_CallbacksToAdd.AppendWithCapacity(dlg, 10);
		}

		public void RemoveCallback(TDelegate dlg)
		{
			if (!this.m_CannotMutateCallbacksArray)
			{
				int num = this.m_Callbacks.IndexOf(dlg);
				if (num >= 0)
				{
					this.m_Callbacks.RemoveAtWithCapacity(num);
				}
				return;
			}
			if (this.m_CallbacksToRemove.Contains(dlg))
			{
				return;
			}
			int num2 = this.m_CallbacksToAdd.IndexOf(dlg);
			if (num2 != -1)
			{
				this.m_CallbacksToAdd.RemoveAtByMovingTailWithCapacity(num2);
			}
			this.m_CallbacksToRemove.AppendWithCapacity(dlg, 10);
		}

		public void LockForChanges()
		{
			this.m_CannotMutateCallbacksArray = true;
		}

		public void UnlockForChanges()
		{
			this.m_CannotMutateCallbacksArray = false;
			for (int i = 0; i < this.m_CallbacksToRemove.length; i++)
			{
				this.RemoveCallback(this.m_CallbacksToRemove[i]);
			}
			for (int j = 0; j < this.m_CallbacksToAdd.length; j++)
			{
				this.AddCallback(this.m_CallbacksToAdd[j]);
			}
			this.m_CallbacksToAdd.Clear();
			this.m_CallbacksToRemove.Clear();
		}

		private bool m_CannotMutateCallbacksArray;

		private InlinedArray<TDelegate> m_Callbacks;

		private InlinedArray<TDelegate> m_CallbacksToAdd;

		private InlinedArray<TDelegate> m_CallbacksToRemove;
	}
}
