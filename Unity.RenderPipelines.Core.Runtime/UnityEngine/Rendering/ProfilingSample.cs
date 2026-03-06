using System;
using Unity.Profiling;
using UnityEngine.Profiling;

namespace UnityEngine.Rendering
{
	[Obsolete("Please use ProfilingScope")]
	[IgnoredByDeepProfiler]
	public struct ProfilingSample : IDisposable
	{
		public ProfilingSample(CommandBuffer cmd, string name, CustomSampler sampler = null)
		{
			this.m_Cmd = cmd;
			this.m_Name = name;
			this.m_Disposed = false;
			if (cmd != null && name != "")
			{
				cmd.BeginSample(name);
			}
			this.m_Sampler = sampler;
		}

		public ProfilingSample(CommandBuffer cmd, string format, object arg)
		{
			this = new ProfilingSample(cmd, string.Format(format, arg), null);
		}

		public ProfilingSample(CommandBuffer cmd, string format, params object[] args)
		{
			this = new ProfilingSample(cmd, string.Format(format, args), null);
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
			if (disposing && this.m_Cmd != null && this.m_Name != "")
			{
				this.m_Cmd.EndSample(this.m_Name);
			}
			this.m_Disposed = true;
		}

		private readonly CommandBuffer m_Cmd;

		private readonly string m_Name;

		private bool m_Disposed;

		private CustomSampler m_Sampler;
	}
}
