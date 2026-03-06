using System;
using UnityEngine;

internal class MetaXRAcousticMaterialMapping : ScriptableObject
{
	internal MetaXRAcousticMaterialProperties findAcousticMaterial(PhysicsMaterial pmat)
	{
		if (pmat == null || this.mapping == null || this.mapping.Length == 0)
		{
			return this.fallbackMaterial;
		}
		MetaXRAcousticMaterialMapping.Pair pair2 = Array.Find<MetaXRAcousticMaterialMapping.Pair>(this.mapping, (MetaXRAcousticMaterialMapping.Pair pair) => object.Equals(pair.physicMaterial, pmat));
		if (pair2 == null)
		{
			return null;
		}
		return pair2.acousticMaterial;
	}

	internal static MetaXRAcousticMaterialMapping Instance
	{
		get
		{
			if (MetaXRAcousticMaterialMapping.instance == null)
			{
				MetaXRAcousticMaterialMapping.instance = Resources.Load<MetaXRAcousticMaterialMapping>("MetaXRAcousticMaterialMapping");
				if (MetaXRAcousticMaterialMapping.instance == null)
				{
					MetaXRAcousticMaterialMapping.instance = ScriptableObject.CreateInstance<MetaXRAcousticMaterialMapping>();
				}
			}
			return MetaXRAcousticMaterialMapping.instance;
		}
	}

	[HideInInspector]
	[SerializeField]
	internal MetaXRAcousticMaterialMapping.Pair[] mapping;

	[HideInInspector]
	[SerializeField]
	[Tooltip("Acoustic material to apply when there is no physics material.")]
	internal MetaXRAcousticMaterialProperties fallbackMaterial;

	private static MetaXRAcousticMaterialMapping instance;

	[Serializable]
	internal class Pair
	{
		[SerializeField]
		internal PhysicsMaterial physicMaterial;

		[SerializeField]
		internal MetaXRAcousticMaterialProperties acousticMaterial;
	}
}
