using System;
using UnityEngine.Serialization;

namespace UnityEngine.AddressableAssets
{
	[Serializable]
	public class AssetLabelReference : IKeyEvaluator
	{
		public string labelString
		{
			get
			{
				return this.m_LabelString;
			}
			set
			{
				this.m_LabelString = value;
			}
		}

		public object RuntimeKey
		{
			get
			{
				if (this.labelString == null)
				{
					this.labelString = string.Empty;
				}
				return this.labelString;
			}
		}

		public bool RuntimeKeyIsValid()
		{
			return !string.IsNullOrEmpty(this.RuntimeKey.ToString());
		}

		public override int GetHashCode()
		{
			return this.labelString.GetHashCode();
		}

		[FormerlySerializedAs("m_labelString")]
		[SerializeField]
		private string m_LabelString;
	}
}
