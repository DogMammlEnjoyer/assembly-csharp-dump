using System;

namespace UnityEngine.XR.LegacyInputHelpers
{
	[Serializable]
	public class ArmModelTransition
	{
		public string transitionKeyName
		{
			get
			{
				return this.m_KeyName;
			}
			set
			{
				this.m_KeyName = value;
			}
		}

		public ArmModel armModel
		{
			get
			{
				return this.m_ArmModel;
			}
			set
			{
				this.m_ArmModel = value;
			}
		}

		[SerializeField]
		private string m_KeyName;

		[SerializeField]
		private ArmModel m_ArmModel;
	}
}
