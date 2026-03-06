using System;
using System.Collections.Generic;
using UnityEngine;

namespace Technie.PhysicsCreator.Skinned
{
	public class SkinnedColliderEditorData : ScriptableObject, IEditorData
	{
		public Hash160 CachedHash
		{
			get
			{
				return this.sourceMeshHash;
			}
			set
			{
				this.sourceMeshHash = value;
			}
		}

		public bool HasCachedData
		{
			get
			{
				return this.sourceMeshHash != null && this.sourceMeshHash.IsValid();
			}
		}

		public Mesh SourceMesh
		{
			get
			{
				return this.sourceMesh;
			}
		}

		public IHull[] Hulls
		{
			get
			{
				return this.boneHullData.ToArray();
			}
		}

		public bool HasSuppressMeshModificationWarning
		{
			get
			{
				return this.suppressMeshModificationWarning;
			}
		}

		public void SetSelection(BoneData bone)
		{
			for (int i = 0; i < this.boneData.Count; i++)
			{
				if (this.boneData[i] == bone)
				{
					this.selectedBoneIndex = i;
					this.selectedHullIndex = -1;
					break;
				}
			}
			this.MarkDirty();
		}

		public void SetSelection(BoneHullData hull)
		{
			for (int i = 0; i < this.boneHullData.Count; i++)
			{
				if (this.boneHullData[i] == hull)
				{
					this.selectedHullIndex = i;
					this.selectedBoneIndex = -1;
					break;
				}
			}
			this.MarkDirty();
		}

		public void ClearSelection()
		{
			this.selectedBoneIndex = -1;
			this.selectedHullIndex = -1;
			this.MarkDirty();
		}

		public BoneData GetSelectedBone()
		{
			if (this.selectedBoneIndex >= 0 && this.selectedBoneIndex < this.boneData.Count)
			{
				return this.boneData[this.selectedBoneIndex];
			}
			return null;
		}

		public BoneHullData GetSelectedHull()
		{
			if (this.selectedHullIndex >= 0 && this.selectedHullIndex < this.boneHullData.Count)
			{
				return this.boneHullData[this.selectedHullIndex];
			}
			return null;
		}

		public BoneData GetBoneData(Transform bone)
		{
			if (bone == null)
			{
				return null;
			}
			return this.GetBoneData(bone.name);
		}

		public BoneData GetBoneData(string boneName)
		{
			foreach (BoneData boneData in this.boneData)
			{
				if (boneData.targetBoneName == boneName)
				{
					return boneData;
				}
			}
			return null;
		}

		public BoneHullData[] GetBoneHullData(Transform bone)
		{
			if (bone == null)
			{
				return new BoneHullData[0];
			}
			return this.GetBoneHullData(bone.name);
		}

		public BoneHullData[] GetBoneHullData(string boneName)
		{
			List<BoneHullData> list = new List<BoneHullData>();
			foreach (BoneHullData boneHullData in this.boneHullData)
			{
				if (boneHullData.targetBoneName == boneName)
				{
					list.Add(boneHullData);
				}
			}
			return list.ToArray();
		}

		public void SetAssetDirty()
		{
			this.MarkDirty();
		}

		public void MarkDirty()
		{
		}

		public int GetLastModifiedFrame()
		{
			return this.lastModifiedFrame;
		}

		public void Add(BoneData data)
		{
			this.boneData.Add(data);
			this.MarkDirty();
		}

		public void Remove(BoneData data)
		{
			this.boneData.Remove(data);
			this.MarkDirty();
		}

		public void Add(BoneHullData data)
		{
			this.boneHullData.Add(data);
			this.MarkDirty();
		}

		public void Remove(BoneHullData data)
		{
			this.boneHullData.Remove(data);
			this.MarkDirty();
		}

		public const int INVALID_INDEX = -1;

		public SkinnedColliderRuntimeData runtimeData;

		public float defaultMass = 1f;

		public float defaultLinearDrag;

		public float defaultAngularDrag = 0.05f;

		public float defaultLinearDamping;

		public float defaultAngularDamping;

		public PhysicsMaterial defaultMaterial;

		public ColliderType defaultColliderType;

		public List<BoneData> boneData = new List<BoneData>();

		public List<BoneHullData> boneHullData = new List<BoneHullData>();

		private int selectedBoneIndex = -1;

		private int selectedHullIndex = -1;

		private int lastModifiedFrame;

		public Mesh sourceMesh;

		public Hash160 sourceMeshHash;

		public bool suppressMeshModificationWarning;
	}
}
