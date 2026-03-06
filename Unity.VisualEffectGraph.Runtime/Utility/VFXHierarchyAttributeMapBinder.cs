using System;
using System.Collections.Generic;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Hierarchy to Attribute Map Binder")]
	[VFXBinder("Point Cache/Hierarchy to Attribute Map")]
	internal class VFXHierarchyAttributeMapBinder : VFXBinderBase
	{
		protected override void OnEnable()
		{
			base.OnEnable();
			this.UpdateHierarchy();
		}

		private void OnValidate()
		{
			this.UpdateHierarchy();
		}

		private void UpdateHierarchy()
		{
			this.bones = this.ChildrenOf(this.HierarchyRoot, this.MaximumDepth);
			int count = this.bones.Count;
			this.position = new Texture2D(count, 1, TextureFormat.RGBAHalf, false, true);
			this.targetPosition = new Texture2D(count, 1, TextureFormat.RGBAHalf, false, true);
			this.radius = new Texture2D(count, 1, TextureFormat.RHalf, false, true);
			this.UpdateData();
		}

		private List<VFXHierarchyAttributeMapBinder.Bone> ChildrenOf(Transform source, uint depth)
		{
			List<VFXHierarchyAttributeMapBinder.Bone> list = new List<VFXHierarchyAttributeMapBinder.Bone>();
			if (source == null)
			{
				return list;
			}
			foreach (object obj in source)
			{
				Transform transform = (Transform)obj;
				list.Add(new VFXHierarchyAttributeMapBinder.Bone
				{
					source = source.transform,
					target = transform.transform,
					sourceRadius = this.DefaultRadius,
					targetRadius = this.DefaultRadius
				});
				if (depth > 0U)
				{
					list.AddRange(this.ChildrenOf(transform, depth - 1U));
				}
			}
			return list;
		}

		private void UpdateData()
		{
			int count = this.bones.Count;
			if (this.position.width != count)
			{
				return;
			}
			List<Color> list = new List<Color>();
			List<Color> list2 = new List<Color>();
			List<Color> list3 = new List<Color>();
			for (int i = 0; i < count; i++)
			{
				VFXHierarchyAttributeMapBinder.Bone bone = this.bones[i];
				list.Add(new Color(bone.source.position.x, bone.source.position.y, bone.source.position.z, 1f));
				list2.Add(new Color(bone.target.position.x, bone.target.position.y, bone.target.position.z, 1f));
				list3.Add(new Color(bone.sourceRadius, 0f, 0f, 1f));
			}
			this.position.SetPixels(list.ToArray());
			this.targetPosition.SetPixels(list2.ToArray());
			this.radius.SetPixels(list3.ToArray());
			this.position.Apply();
			this.targetPosition.Apply();
			this.radius.Apply();
		}

		public override bool IsValid(VisualEffect component)
		{
			return this.HierarchyRoot != null && component.HasTexture(this.m_PositionMap) && component.HasTexture(this.m_TargetPositionMap) && component.HasTexture(this.m_RadiusPositionMap) && component.HasUInt(this.m_BoneCount);
		}

		public override void UpdateBinding(VisualEffect component)
		{
			this.UpdateData();
			component.SetTexture(this.m_PositionMap, this.position);
			component.SetTexture(this.m_TargetPositionMap, this.targetPosition);
			component.SetTexture(this.m_RadiusPositionMap, this.radius);
			component.SetUInt(this.m_BoneCount, (uint)this.bones.Count);
		}

		public override string ToString()
		{
			return string.Format("Hierarchy: {0} -> {1}", (this.HierarchyRoot == null) ? "(null)" : this.HierarchyRoot.name, this.m_PositionMap);
		}

		[VFXPropertyBinding(new string[]
		{
			"System.UInt32"
		})]
		[SerializeField]
		protected ExposedProperty m_BoneCount = "BoneCount";

		[VFXPropertyBinding(new string[]
		{
			"UnityEngine.Texture2D"
		})]
		[SerializeField]
		protected ExposedProperty m_PositionMap = "PositionMap";

		[VFXPropertyBinding(new string[]
		{
			"UnityEngine.Texture2D"
		})]
		[SerializeField]
		protected ExposedProperty m_TargetPositionMap = "TargetPositionMap";

		[VFXPropertyBinding(new string[]
		{
			"UnityEngine.Texture2D"
		})]
		[SerializeField]
		protected ExposedProperty m_RadiusPositionMap = "RadiusPositionMap";

		public Transform HierarchyRoot;

		public float DefaultRadius = 0.1f;

		public uint MaximumDepth = 3U;

		public VFXHierarchyAttributeMapBinder.RadiusMode Radius;

		private Texture2D position;

		private Texture2D targetPosition;

		private Texture2D radius;

		private List<VFXHierarchyAttributeMapBinder.Bone> bones;

		public enum RadiusMode
		{
			Fixed,
			Interpolate
		}

		private struct Bone
		{
			public Transform source;

			public float sourceRadius;

			public Transform target;

			public float targetRadius;
		}
	}
}
