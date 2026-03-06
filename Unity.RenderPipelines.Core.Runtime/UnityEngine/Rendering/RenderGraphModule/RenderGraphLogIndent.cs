using System;

namespace UnityEngine.Rendering.RenderGraphModule
{
	internal struct RenderGraphLogIndent : IDisposable
	{
		public RenderGraphLogIndent(RenderGraphLogger logger, int indentation = 1)
		{
			this.m_Disposed = false;
			this.m_Indentation = indentation;
			this.m_Logger = logger;
			this.m_Logger.IncrementIndentation(this.m_Indentation);
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (this.m_Disposed)
			{
				return;
			}
			if (disposing && this.m_Logger != null)
			{
				this.m_Logger.DecrementIndentation(this.m_Indentation);
			}
			this.m_Disposed = true;
		}

		private int m_Indentation;

		private RenderGraphLogger m_Logger;

		private bool m_Disposed;
	}
}
