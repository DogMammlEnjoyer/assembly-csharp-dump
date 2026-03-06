using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering
{
	[SupportedOnRenderPipeline(new Type[]
	{

	})]
	[CategoryInfo(Name = "H: RP Assets Inclusion", Order = 990)]
	[HideInInspector]
	[Serializable]
	public class IncludeAdditionalRPAssets : IRenderPipelineGraphicsSettings
	{
		int IRenderPipelineGraphicsSettings.version
		{
			get
			{
				return (int)this.m_version;
			}
		}

		public bool includeReferencedInScenes
		{
			get
			{
				return this.m_IncludeReferencedInScenes;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_IncludeReferencedInScenes, value, "m_IncludeReferencedInScenes");
			}
		}

		public bool includeAssetsByLabel
		{
			get
			{
				return this.m_IncludeAssetsByLabel;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_IncludeAssetsByLabel, value, "m_IncludeAssetsByLabel");
			}
		}

		public string labelToInclude
		{
			get
			{
				return this.m_LabelToInclude;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_LabelToInclude, value, "m_LabelToInclude");
			}
		}

		[SerializeField]
		[HideInInspector]
		private IncludeAdditionalRPAssets.Version m_version;

		[SerializeField]
		private bool m_IncludeReferencedInScenes;

		[SerializeField]
		private bool m_IncludeAssetsByLabel;

		[SerializeField]
		private string m_LabelToInclude;

		private enum Version
		{
			Initial,
			Count,
			Last = 0
		}
	}
}
