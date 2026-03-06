using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	internal sealed class FilterFunctionDefinition : ScriptableObject
	{
		public string filterName
		{
			get
			{
				return this.m_FilterName;
			}
			set
			{
				this.m_FilterName = value;
			}
		}

		public FilterParameterDeclaration[] parameters
		{
			get
			{
				return this.m_Parameters;
			}
			set
			{
				this.m_Parameters = value;
			}
		}

		public PostProcessingPass[] passes
		{
			get
			{
				return this.m_Passes;
			}
			set
			{
				this.m_Passes = value;
			}
		}

		[SerializeField]
		private string m_FilterName;

		[SerializeField]
		private FilterParameterDeclaration[] m_Parameters;

		[SerializeField]
		private PostProcessingPass[] m_Passes;
	}
}
