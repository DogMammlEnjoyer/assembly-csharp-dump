using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.Universal
{
	internal class DecalEntityIndexer
	{
		public bool IsValid(DecalEntity decalEntity)
		{
			return this.m_Entities.Count > decalEntity.index && this.m_Entities[decalEntity.index].version == decalEntity.version;
		}

		public DecalEntity CreateDecalEntity(int arrayIndex, int chunkIndex)
		{
			if (this.m_FreeIndices.Count != 0)
			{
				int index = this.m_FreeIndices.Dequeue();
				int version = this.m_Entities[index].version + 1;
				this.m_Entities[index] = new DecalEntityIndexer.DecalEntityItem
				{
					arrayIndex = arrayIndex,
					chunkIndex = chunkIndex,
					version = version
				};
				return new DecalEntity
				{
					index = index,
					version = version
				};
			}
			int count = this.m_Entities.Count;
			int version2 = 1;
			this.m_Entities.Add(new DecalEntityIndexer.DecalEntityItem
			{
				arrayIndex = arrayIndex,
				chunkIndex = chunkIndex,
				version = version2
			});
			return new DecalEntity
			{
				index = count,
				version = version2
			};
		}

		public void DestroyDecalEntity(DecalEntity decalEntity)
		{
			this.m_FreeIndices.Enqueue(decalEntity.index);
			DecalEntityIndexer.DecalEntityItem value = this.m_Entities[decalEntity.index];
			value.version++;
			this.m_Entities[decalEntity.index] = value;
		}

		public DecalEntityIndexer.DecalEntityItem GetItem(DecalEntity decalEntity)
		{
			return this.m_Entities[decalEntity.index];
		}

		public void UpdateIndex(DecalEntity decalEntity, int newArrayIndex)
		{
			DecalEntityIndexer.DecalEntityItem value = this.m_Entities[decalEntity.index];
			value.arrayIndex = newArrayIndex;
			value.version = decalEntity.version;
			this.m_Entities[decalEntity.index] = value;
		}

		public void RemapChunkIndices(List<int> remaper)
		{
			for (int i = 0; i < this.m_Entities.Count; i++)
			{
				int chunkIndex = remaper[this.m_Entities[i].chunkIndex];
				DecalEntityIndexer.DecalEntityItem value = this.m_Entities[i];
				value.chunkIndex = chunkIndex;
				this.m_Entities[i] = value;
			}
		}

		public void Clear()
		{
			this.m_Entities.Clear();
			this.m_FreeIndices.Clear();
		}

		private List<DecalEntityIndexer.DecalEntityItem> m_Entities = new List<DecalEntityIndexer.DecalEntityItem>();

		private Queue<int> m_FreeIndices = new Queue<int>();

		public struct DecalEntityItem
		{
			public int chunkIndex;

			public int arrayIndex;

			public int version;
		}
	}
}
