using System;

namespace UnityEngine.ProBuilder
{
	[Serializable]
	internal class ChangelogEntry
	{
		public SemVer versionInfo
		{
			get
			{
				return this.m_VersionInfo;
			}
		}

		public string releaseNotes
		{
			get
			{
				return this.m_ReleaseNotes;
			}
		}

		public ChangelogEntry(SemVer version, string releaseNotes)
		{
			this.m_VersionInfo = version;
			this.m_ReleaseNotes = releaseNotes;
		}

		public override string ToString()
		{
			return this.m_VersionInfo.ToString() + "\n\n" + this.m_ReleaseNotes;
		}

		[SerializeField]
		private SemVer m_VersionInfo;

		[SerializeField]
		private string m_ReleaseNotes;
	}
}
