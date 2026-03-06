using System;
using UnityEngine;

namespace Unity.XR.CoreUtils
{
	public class UndoBlock : IDisposable
	{
		public UndoBlock(string undoLabel, bool testMode = false)
		{
			this.m_UndoGroup = -1;
		}

		public void RegisterCreatedObject(Object objectToUndo)
		{
		}

		public void RecordObject(Object objectToUndo)
		{
		}

		public void SetTransformParent(Transform transform, Transform newParent)
		{
			transform.parent = newParent;
		}

		public T AddComponent<T>(GameObject gameObject) where T : Component
		{
			return gameObject.AddComponent<T>();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.m_DisposedValue)
			{
				if (disposing)
				{
					int undoGroup = this.m_UndoGroup;
				}
				this.m_DisposedValue = true;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		private int m_UndoGroup;

		private bool m_DisposedValue;
	}
}
