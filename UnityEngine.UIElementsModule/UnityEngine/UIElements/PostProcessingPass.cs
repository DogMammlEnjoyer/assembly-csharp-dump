using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	internal struct PostProcessingPass
	{
		public Material material
		{
			get
			{
				return this.m_Material;
			}
			set
			{
				this.m_Material = value;
			}
		}

		public int passIndex
		{
			get
			{
				return this.m_PassIndex;
			}
			set
			{
				this.m_PassIndex = value;
			}
		}

		public ParameterBinding[] parameterBindings
		{
			get
			{
				return this.m_ParameterBindings;
			}
			set
			{
				this.m_ParameterBindings = value;
			}
		}

		internal PostProcessingMargins readMargins
		{
			get
			{
				return this.m_ReadMargins;
			}
			set
			{
				this.m_ReadMargins = value;
			}
		}

		public PostProcessingMargins writeMargins
		{
			get
			{
				return this.m_WriteMargins;
			}
			set
			{
				this.m_WriteMargins = value;
			}
		}

		public PostProcessingPass.PrepareMaterialPropertyBlockDelegate prepareMaterialPropertyBlockCallback { readonly get; set; }

		public PostProcessingPass.ComputeRequiredMarginsDelegate computeRequiredReadMarginsCallback { readonly get; set; }

		public PostProcessingPass.ComputeRequiredMarginsDelegate computeRequiredWriteMarginsCallback { readonly get; set; }

		[SerializeField]
		private Material m_Material;

		[SerializeField]
		private int m_PassIndex;

		[SerializeField]
		private ParameterBinding[] m_ParameterBindings;

		[SerializeField]
		private PostProcessingMargins m_ReadMargins;

		[SerializeField]
		private PostProcessingMargins m_WriteMargins;

		public delegate void PrepareMaterialPropertyBlockDelegate(MaterialPropertyBlock mpb, FilterFunction func);

		public delegate PostProcessingMargins ComputeRequiredMarginsDelegate(FilterFunction func);
	}
}
