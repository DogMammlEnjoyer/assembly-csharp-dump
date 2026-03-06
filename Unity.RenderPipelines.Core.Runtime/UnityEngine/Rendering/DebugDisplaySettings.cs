using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	public abstract class DebugDisplaySettings<T> : IDebugDisplaySettings where T : IDebugDisplaySettings, new()
	{
		public static T Instance
		{
			get
			{
				return DebugDisplaySettings<T>.s_Instance.Value;
			}
		}

		public virtual bool AreAnySettingsActive
		{
			get
			{
				using (HashSet<IDebugDisplaySettingsData>.Enumerator enumerator = this.m_Settings.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.AreAnySettingsActive)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public virtual bool IsPostProcessingAllowed
		{
			get
			{
				bool flag = true;
				foreach (IDebugDisplaySettingsData debugDisplaySettingsData in this.m_Settings)
				{
					flag &= debugDisplaySettingsData.IsPostProcessingAllowed;
				}
				return flag;
			}
		}

		public virtual bool IsLightingActive
		{
			get
			{
				bool flag = true;
				foreach (IDebugDisplaySettingsData debugDisplaySettingsData in this.m_Settings)
				{
					flag &= debugDisplaySettingsData.IsLightingActive;
				}
				return flag;
			}
		}

		protected TData Add<TData>(TData newData) where TData : IDebugDisplaySettingsData
		{
			this.m_Settings.Add(newData);
			return newData;
		}

		IDebugDisplaySettingsData IDebugDisplaySettings.Add(IDebugDisplaySettingsData newData)
		{
			this.m_Settings.Add(newData);
			return newData;
		}

		public void ForEach(Action<IDebugDisplaySettingsData> onExecute)
		{
			foreach (IDebugDisplaySettingsData obj in this.m_Settings)
			{
				onExecute(obj);
			}
		}

		public virtual void Reset()
		{
			foreach (IDebugDisplaySettingsData debugDisplaySettingsData in this.m_Settings)
			{
				debugDisplaySettingsData.Reset();
			}
			this.m_Settings.Clear();
		}

		public virtual bool TryGetScreenClearColor(ref Color color)
		{
			using (HashSet<IDebugDisplaySettingsData>.Enumerator enumerator = this.m_Settings.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.TryGetScreenClearColor(ref color))
					{
						return true;
					}
				}
			}
			return false;
		}

		protected readonly HashSet<IDebugDisplaySettingsData> m_Settings = new HashSet<IDebugDisplaySettingsData>(new DebugDisplaySettings<T>.IDebugDisplaySettingsDataComparer());

		private static readonly Lazy<T> s_Instance = new Lazy<T>(delegate()
		{
			T result = Activator.CreateInstance<T>();
			result.Reset();
			return result;
		});

		private class IDebugDisplaySettingsDataComparer : IEqualityComparer<IDebugDisplaySettingsData>
		{
			public bool Equals(IDebugDisplaySettingsData x, IDebugDisplaySettingsData y)
			{
				return x == y || (x != null && y != null && x.GetType() == y.GetType());
			}

			public int GetHashCode(IDebugDisplaySettingsData obj)
			{
				return 17 * 23 + obj.GetType().GetHashCode();
			}
		}
	}
}
