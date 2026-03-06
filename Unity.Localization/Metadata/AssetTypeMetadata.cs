using System;

namespace UnityEngine.Localization.Metadata
{
	[HideInInspector]
	internal class AssetTypeMetadata : SharedTableCollectionMetadata
	{
		public Type Type { get; set; }

		public override void OnBeforeSerialize()
		{
			base.OnBeforeSerialize();
			Type type = this.Type;
			this.m_TypeString = ((type != null) ? type.AssemblyQualifiedName : null);
		}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();
			if (!string.IsNullOrEmpty(this.m_TypeString))
			{
				this.Type = Type.GetType(this.m_TypeString);
			}
		}

		[SerializeField]
		[HideInInspector]
		internal string m_TypeString;
	}
}
