using System;

namespace UnityEngine.Localization.Metadata
{
	[Metadata]
	[Serializable]
	public class Comment : IMetadata
	{
		public string CommentText
		{
			get
			{
				return this.m_CommentText;
			}
			set
			{
				this.m_CommentText = value;
			}
		}

		public override string ToString()
		{
			return this.CommentText;
		}

		[SerializeField]
		[TextArea(1, 2147483647)]
		private string m_CommentText = "Comment Text";
	}
}
