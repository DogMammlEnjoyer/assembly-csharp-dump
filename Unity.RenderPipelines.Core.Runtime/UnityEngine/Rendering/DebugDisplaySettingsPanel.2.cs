using System;

namespace UnityEngine.Rendering
{
	public abstract class DebugDisplaySettingsPanel<T> : DebugDisplaySettingsPanel where T : IDebugDisplaySettingsData
	{
		public T data
		{
			get
			{
				return this.m_Data;
			}
			internal set
			{
				this.m_Data = value;
			}
		}

		protected DebugDisplaySettingsPanel(T data)
		{
			this.m_Data = data;
		}

		internal T m_Data;
	}
}
