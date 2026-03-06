using System;
using System.Runtime.InteropServices;

namespace Drawing
{
	public struct RedrawScope : IDisposable
	{
		internal RedrawScope(DrawingData gizmos, int id)
		{
			this.gizmos = gizmos.gizmosHandle;
			this.id = id;
		}

		internal RedrawScope(DrawingData gizmos)
		{
			this.gizmos = gizmos.gizmosHandle;
			this.id = RedrawScope.idCounter++;
		}

		internal void Draw()
		{
			if (this.gizmos.IsAllocated)
			{
				DrawingData drawingData = this.gizmos.Target as DrawingData;
				if (drawingData != null)
				{
					drawingData.Draw(this);
				}
			}
		}

		public void Rewind()
		{
			this.Dispose();
			this = DrawingManager.GetRedrawScope();
		}

		internal void DrawUntilDispose()
		{
			DrawingData drawingData = this.gizmos.Target as DrawingData;
			if (drawingData != null)
			{
				drawingData.DrawUntilDisposed(this);
			}
		}

		public void Dispose()
		{
			if (this.gizmos.IsAllocated)
			{
				DrawingData drawingData = this.gizmos.Target as DrawingData;
				if (drawingData != null)
				{
					drawingData.DisposeRedrawScope(this);
				}
			}
			this.gizmos = default(GCHandle);
		}

		internal GCHandle gizmos;

		internal int id;

		private static int idCounter = 1;
	}
}
