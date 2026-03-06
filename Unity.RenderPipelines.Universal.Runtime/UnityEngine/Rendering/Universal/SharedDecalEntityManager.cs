using System;

namespace UnityEngine.Rendering.Universal
{
	internal class SharedDecalEntityManager : IDisposable
	{
		public DecalEntityManager Get()
		{
			if (this.m_DecalEntityManager == null)
			{
				this.m_DecalEntityManager = new DecalEntityManager();
				foreach (DecalProjector decalProjector in Object.FindObjectsByType<DecalProjector>(FindObjectsSortMode.InstanceID))
				{
					if (decalProjector.isActiveAndEnabled && !this.m_DecalEntityManager.IsValid(decalProjector.decalEntity))
					{
						decalProjector.decalEntity = this.m_DecalEntityManager.CreateDecalEntity(decalProjector);
					}
				}
				DecalProjector.onDecalAdd += this.OnDecalAdd;
				DecalProjector.onDecalRemove += this.OnDecalRemove;
				DecalProjector.onDecalPropertyChange += this.OnDecalPropertyChange;
				DecalProjector.onDecalMaterialChange += this.OnDecalMaterialChange;
				DecalProjector.onAllDecalPropertyChange += this.OnAllDecalPropertyChange;
			}
			this.m_ReferenceCounter++;
			return this.m_DecalEntityManager;
		}

		public void Release(DecalEntityManager decalEntityManager)
		{
			if (this.m_ReferenceCounter == 0)
			{
				return;
			}
			this.m_ReferenceCounter--;
			if (this.m_ReferenceCounter == 0)
			{
				this.Dispose();
			}
		}

		public void Dispose()
		{
			this.m_DecalEntityManager.Dispose();
			this.m_DecalEntityManager = null;
			this.m_ReferenceCounter = 0;
			DecalProjector.onDecalAdd -= this.OnDecalAdd;
			DecalProjector.onDecalRemove -= this.OnDecalRemove;
			DecalProjector.onDecalPropertyChange -= this.OnDecalPropertyChange;
			DecalProjector.onDecalMaterialChange -= this.OnDecalMaterialChange;
			DecalProjector.onAllDecalPropertyChange -= this.OnAllDecalPropertyChange;
		}

		private void OnDecalAdd(DecalProjector decalProjector)
		{
			if (!this.m_DecalEntityManager.IsValid(decalProjector.decalEntity))
			{
				decalProjector.decalEntity = this.m_DecalEntityManager.CreateDecalEntity(decalProjector);
			}
		}

		private void OnDecalRemove(DecalProjector decalProjector)
		{
			this.m_DecalEntityManager.DestroyDecalEntity(decalProjector.decalEntity);
		}

		private void OnDecalPropertyChange(DecalProjector decalProjector)
		{
			if (this.m_DecalEntityManager.IsValid(decalProjector.decalEntity))
			{
				this.m_DecalEntityManager.UpdateDecalEntityData(decalProjector.decalEntity, decalProjector);
			}
		}

		private void OnAllDecalPropertyChange()
		{
			this.m_DecalEntityManager.UpdateAllDecalEntitiesData();
		}

		private void OnDecalMaterialChange(DecalProjector decalProjector)
		{
			this.OnDecalRemove(decalProjector);
			this.OnDecalAdd(decalProjector);
		}

		private DecalEntityManager m_DecalEntityManager;

		private int m_ReferenceCounter;
	}
}
