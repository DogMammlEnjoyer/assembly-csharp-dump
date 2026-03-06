using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	[Serializable]
	public class RenderPipelineGraphicsSettingsContainer : ISerializationCallbackReceiver
	{
		public List<IRenderPipelineGraphicsSettings> settingsList
		{
			get
			{
				return this.m_RuntimeSettings.settingsList;
			}
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
		}

		[SerializeField]
		[HideInInspector]
		private RenderPipelineGraphicsSettingsCollection m_RuntimeSettings = new RenderPipelineGraphicsSettingsCollection();
	}
}
