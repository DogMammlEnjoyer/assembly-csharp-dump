using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	internal struct ParameterBinding
	{
		public int index
		{
			get
			{
				return this.m_Index;
			}
			set
			{
				this.m_Index = value;
			}
		}

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

		[SerializeField]
		private int m_Index;

		[SerializeField]
		private string m_Name;
	}
}
