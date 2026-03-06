using System;

namespace UnityEngine.Localization.Metadata
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class MetadataTypeAttribute : PropertyAttribute
	{
		public MetadataType Type { get; set; }

		public MetadataTypeAttribute(MetadataType type)
		{
			this.Type = type;
		}
	}
}
