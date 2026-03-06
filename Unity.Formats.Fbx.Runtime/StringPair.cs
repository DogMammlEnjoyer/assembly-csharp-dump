using System;

namespace UnityEngine.Formats.Fbx.Exporter
{
	[Serializable]
	internal struct StringPair
	{
		public string FBXObjectName
		{
			get
			{
				return this.m_fbxObjectName;
			}
			set
			{
				this.m_fbxObjectName = value;
			}
		}

		public string UnityObjectName
		{
			get
			{
				return this.m_unityObjectName;
			}
			set
			{
				this.m_unityObjectName = value;
			}
		}

		private string m_fbxObjectName;

		private string m_unityObjectName;
	}
}
