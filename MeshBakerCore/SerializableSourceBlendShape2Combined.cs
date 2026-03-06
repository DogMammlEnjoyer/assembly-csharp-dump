using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public class SerializableSourceBlendShape2Combined
	{
		public void SetBuffers(GameObject[] srcGameObjs, int[] srcBlendShapeIdxs, GameObject[] targGameObjs, int[] targBlendShapeIdx)
		{
			this.srcGameObject = srcGameObjs;
			this.srcBlendShapeIdx = srcBlendShapeIdxs;
			this.combinedMeshTargetGameObject = targGameObjs;
			this.blendShapeIdx = targBlendShapeIdx;
		}

		public void DebugPrint()
		{
			if (this.srcGameObject == null)
			{
				Debug.LogError("Empty");
				return;
			}
			for (int i = 0; i < this.srcGameObject.Length; i++)
			{
				Debug.LogFormat("{0} {1} {2} {3}", new object[]
				{
					this.srcGameObject[i],
					this.srcBlendShapeIdx[i],
					this.combinedMeshTargetGameObject[i],
					this.blendShapeIdx[i]
				});
			}
		}

		public Dictionary<MB3_MeshCombiner.MBBlendShapeKey, MB3_MeshCombiner.MBBlendShapeValue> GenerateMapFromSerializedData()
		{
			if (this.srcGameObject == null || this.srcBlendShapeIdx == null || this.combinedMeshTargetGameObject == null || this.blendShapeIdx == null || this.srcGameObject.Length != this.srcBlendShapeIdx.Length || this.srcGameObject.Length != this.combinedMeshTargetGameObject.Length || this.srcGameObject.Length != this.blendShapeIdx.Length)
			{
				Debug.LogError("Error GenerateMapFromSerializedData. Serialized data was malformed or missing.");
				return null;
			}
			Dictionary<MB3_MeshCombiner.MBBlendShapeKey, MB3_MeshCombiner.MBBlendShapeValue> dictionary = new Dictionary<MB3_MeshCombiner.MBBlendShapeKey, MB3_MeshCombiner.MBBlendShapeValue>();
			for (int i = 0; i < this.srcGameObject.Length; i++)
			{
				GameObject gameObject = this.srcGameObject[i];
				GameObject gameObject2 = this.combinedMeshTargetGameObject[i];
				if (gameObject == null || gameObject2 == null)
				{
					Debug.LogError("Error GenerateMapFromSerializedData. There were null references in the serialized data to source or target game objects. This can happen if the SerializableSourceBlendShape2Combined was serialized in a prefab but the source and target SkinnedMeshRenderer GameObjects  were not.");
					return null;
				}
				dictionary.Add(new MB3_MeshCombiner.MBBlendShapeKey(gameObject, this.srcBlendShapeIdx[i]), new MB3_MeshCombiner.MBBlendShapeValue
				{
					combinedMeshGameObject = gameObject2,
					blendShapeIndex = this.blendShapeIdx[i]
				});
			}
			return dictionary;
		}

		public GameObject[] srcGameObject;

		public int[] srcBlendShapeIdx;

		public GameObject[] combinedMeshTargetGameObject;

		public int[] blendShapeIdx;
	}
}
