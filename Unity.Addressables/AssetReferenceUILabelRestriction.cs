using System;
using System.Text;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class AssetReferenceUILabelRestriction : AssetReferenceUIRestriction
	{
		public AssetReferenceUILabelRestriction(params string[] allowedLabels)
		{
			this.m_AllowedLabels = allowedLabels;
		}

		public override bool ValidateAsset(Object obj)
		{
			return true;
		}

		public override bool ValidateAsset(string path)
		{
			return true;
		}

		public override string ToString()
		{
			if (this.m_CachedToString == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = true;
				foreach (string value in this.m_AllowedLabels)
				{
					if (!flag)
					{
						stringBuilder.Append(',');
					}
					flag = false;
					stringBuilder.Append(value);
				}
				this.m_CachedToString = stringBuilder.ToString();
			}
			return this.m_CachedToString;
		}

		public string[] m_AllowedLabels;

		public string m_CachedToString;
	}
}
