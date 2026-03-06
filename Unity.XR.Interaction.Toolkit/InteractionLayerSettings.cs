using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[ScriptableSettingsPath("Assets/XRI/Settings")]
	internal class InteractionLayerSettings : ScriptableSettings<InteractionLayerSettings>, ISerializationCallbackReceiver
	{
		internal static InteractionLayerSettings GetInstanceOrLoadOnly()
		{
			if (ScriptableSettingsBase<InteractionLayerSettings>.BaseInstance != null)
			{
				return ScriptableSettingsBase<InteractionLayerSettings>.BaseInstance;
			}
			ScriptableSettingsBase<InteractionLayerSettings>.BaseInstance = (Resources.Load(ScriptableSettingsBase<InteractionLayerSettings>.GetFilePath(), typeof(InteractionLayerSettings)) as InteractionLayerSettings);
			return ScriptableSettingsBase<InteractionLayerSettings>.BaseInstance;
		}

		internal bool IsLayerEmpty(int index)
		{
			return this.m_LayerNames == null || string.IsNullOrEmpty(this.m_LayerNames[index]);
		}

		internal void SetLayerNameAt(int index, string layerName)
		{
			if (this.m_LayerNames == null || index >= this.m_LayerNames.Length)
			{
				return;
			}
			this.m_LayerNames[index] = layerName;
		}

		internal string GetLayerNameAt(int index)
		{
			if (this.m_LayerNames == null || index >= this.m_LayerNames.Length)
			{
				return string.Empty;
			}
			return this.m_LayerNames[index];
		}

		internal int GetLayer(string layerName)
		{
			if (this.m_LayerNames == null)
			{
				return -1;
			}
			for (int i = 0; i < this.m_LayerNames.Length; i++)
			{
				if (string.Equals(layerName, this.m_LayerNames[i]))
				{
					return i;
				}
			}
			return -1;
		}

		internal void GetLayerNamesAndValues(List<string> names, List<int> values)
		{
			if (this.m_LayerNames == null)
			{
				return;
			}
			for (int i = 0; i < this.m_LayerNames.Length; i++)
			{
				string text = this.m_LayerNames[i];
				if (!string.IsNullOrEmpty(text))
				{
					names.Add(text);
					values.Add(i);
				}
			}
		}

		public void OnBeforeSerialize()
		{
			if (this.m_LayerNames == null)
			{
				this.m_LayerNames = new string[32];
			}
			if (this.m_LayerNames.Length != 32)
			{
				Array.Resize<string>(ref this.m_LayerNames, 32);
			}
			if (!string.Equals(this.m_LayerNames[0], "Default"))
			{
				this.m_LayerNames[0] = "Default";
			}
		}

		public void OnAfterDeserialize()
		{
		}

		private const string k_DefaultLayerName = "Default";

		internal const int layerSize = 32;

		internal const int builtInLayerSize = 1;

		[SerializeField]
		private string[] m_LayerNames;
	}
}
