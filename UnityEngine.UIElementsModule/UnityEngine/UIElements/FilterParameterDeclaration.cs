using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	internal struct FilterParameterDeclaration
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

		public FilterParameter interpolationDefaultValue
		{
			get
			{
				return this.m_InterpolationDefaultValue;
			}
			set
			{
				this.m_InterpolationDefaultValue = value;
			}
		}

		[SerializeField]
		private string m_Name;

		[SerializeField]
		private FilterParameter m_InterpolationDefaultValue;

		internal FilterParameter defaultValue;
	}
}
