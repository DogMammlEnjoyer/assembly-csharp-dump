using System;
using UnityEngine.Serialization;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Terrain Binder")]
	[VFXBinder("Utility/Terrain")]
	internal class VFXTerrainBinder : VFXBinderBase
	{
		public string Property
		{
			get
			{
				return (string)this.m_Property;
			}
			set
			{
				this.m_Property = value;
				this.UpdateSubProperties();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.UpdateSubProperties();
		}

		private void OnValidate()
		{
			this.UpdateSubProperties();
		}

		private void UpdateSubProperties()
		{
			this.Terrain_Bounds_center = this.m_Property + "_Bounds_center";
			this.Terrain_Bounds_size = this.m_Property + "_Bounds_size";
			this.Terrain_HeightMap = this.m_Property + "_HeightMap";
			this.Terrain_Height = this.m_Property + "_Height";
		}

		public override bool IsValid(VisualEffect component)
		{
			return this.Terrain != null && component.HasVector3(this.Terrain_Bounds_center) && component.HasVector3(this.Terrain_Bounds_size) && component.HasTexture(this.Terrain_HeightMap) && component.HasFloat(this.Terrain_Height);
		}

		public override void UpdateBinding(VisualEffect component)
		{
			Bounds bounds = this.Terrain.terrainData.bounds;
			component.SetVector3(this.Terrain_Bounds_center, bounds.center);
			component.SetVector3(this.Terrain_Bounds_size, bounds.size);
			component.SetTexture(this.Terrain_HeightMap, this.Terrain.terrainData.heightmapTexture);
			component.SetFloat(this.Terrain_Height, this.Terrain.terrainData.heightmapScale.y);
		}

		public override string ToString()
		{
			return string.Format("Terrain : '{0}' -> {1}", this.m_Property, (this.Terrain == null) ? "(null)" : this.Terrain.name);
		}

		[VFXPropertyBinding(new string[]
		{
			"UnityEditor.VFX.TerrainType"
		})]
		[FormerlySerializedAs("TerrainParameter")]
		public ExposedProperty m_Property = "Terrain";

		public Terrain Terrain;

		private ExposedProperty Terrain_Bounds_center;

		private ExposedProperty Terrain_Bounds_size;

		private ExposedProperty Terrain_HeightMap;

		private ExposedProperty Terrain_Height;
	}
}
