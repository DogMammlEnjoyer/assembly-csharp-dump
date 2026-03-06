using System;

namespace UnityEngine.Rendering.Universal
{
	internal class DecalDrawErrorSystem : DecalDrawSystem
	{
		public DecalDrawErrorSystem(DecalEntityManager entityManager, DecalTechnique technique) : base("DecalDrawErrorSystem.Execute", entityManager)
		{
			this.m_Technique = technique;
		}

		protected override int GetPassIndex(DecalCachedChunk decalCachedChunk)
		{
			switch (this.m_Technique)
			{
			case DecalTechnique.Invalid:
				return 0;
			case DecalTechnique.DBuffer:
				if (decalCachedChunk.passIndexDBuffer != -1 || decalCachedChunk.passIndexEmissive != -1)
				{
					return -1;
				}
				return 0;
			case DecalTechnique.ScreenSpace:
				if (decalCachedChunk.passIndexScreenSpace != -1)
				{
					return -1;
				}
				return 0;
			case DecalTechnique.GBuffer:
				if (decalCachedChunk.passIndexGBuffer != -1)
				{
					return -1;
				}
				return 0;
			default:
				return 0;
			}
		}

		protected override Material GetMaterial(DecalEntityChunk decalEntityChunk)
		{
			return this.m_EntityManager.errorMaterial;
		}

		private DecalTechnique m_Technique;
	}
}
