using System;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
	[ScriptableSettingsPath("Assets/XRI/Settings")]
	internal class XRDeviceSimulatorSettings : ScriptableSettings<XRDeviceSimulatorSettings>
	{
		internal static XRDeviceSimulatorSettings GetInstanceOrLoadOnly()
		{
			if (ScriptableSettingsBase<XRDeviceSimulatorSettings>.BaseInstance != null)
			{
				return ScriptableSettingsBase<XRDeviceSimulatorSettings>.BaseInstance;
			}
			ScriptableSettingsBase<XRDeviceSimulatorSettings>.BaseInstance = (Resources.Load(ScriptableSettingsBase<XRDeviceSimulatorSettings>.GetFilePath(), typeof(XRDeviceSimulatorSettings)) as XRDeviceSimulatorSettings);
			return ScriptableSettingsBase<XRDeviceSimulatorSettings>.BaseInstance;
		}

		internal bool automaticallyInstantiateSimulatorPrefab
		{
			get
			{
				return this.m_AutomaticallyInstantiateSimulatorPrefab;
			}
			set
			{
				this.m_AutomaticallyInstantiateSimulatorPrefab = value;
			}
		}

		internal bool automaticallyInstantiateInEditorOnly
		{
			get
			{
				return this.m_AutomaticallyInstantiateInEditorOnly;
			}
			set
			{
				this.m_AutomaticallyInstantiateInEditorOnly = value;
			}
		}

		internal bool useClassic
		{
			get
			{
				return this.m_UseClassic;
			}
			set
			{
				this.m_UseClassic = value;
			}
		}

		internal GameObject simulatorPrefab
		{
			get
			{
				return this.m_SimulatorPrefab;
			}
			set
			{
				this.m_SimulatorPrefab = value;
			}
		}

		[SerializeField]
		private bool m_AutomaticallyInstantiateSimulatorPrefab;

		[SerializeField]
		private bool m_AutomaticallyInstantiateInEditorOnly = true;

		[SerializeField]
		private bool m_UseClassic;

		[SerializeField]
		private GameObject m_SimulatorPrefab;
	}
}
