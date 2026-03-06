using System;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
	[Serializable]
	public sealed class InputFeatureUsageString<T> where T : struct
	{
		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
			}
		}

		public InputFeatureUsageString()
		{
		}

		public InputFeatureUsageString(string usageName)
		{
			this.m_Name = usageName;
		}

		[SerializeField]
		private string m_Name;
	}
}
