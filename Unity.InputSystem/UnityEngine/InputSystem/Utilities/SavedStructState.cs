using System;

namespace UnityEngine.InputSystem.Utilities
{
	internal sealed class SavedStructState<T> : ISavedState where T : struct
	{
		internal SavedStructState(ref T state, SavedStructState<T>.TypedRestore restoreAction, Action staticDisposeCurrentState = null)
		{
			this.m_State = state;
			this.m_RestoreAction = restoreAction;
			this.m_StaticDisposeCurrentState = staticDisposeCurrentState;
		}

		public void StaticDisposeCurrentState()
		{
			if (this.m_StaticDisposeCurrentState != null)
			{
				this.m_StaticDisposeCurrentState();
				this.m_StaticDisposeCurrentState = null;
			}
		}

		public void RestoreSavedState()
		{
			this.m_RestoreAction(ref this.m_State);
			this.m_RestoreAction = null;
		}

		private T m_State;

		private SavedStructState<T>.TypedRestore m_RestoreAction;

		private Action m_StaticDisposeCurrentState;

		public delegate void TypedRestore(ref T state);
	}
}
