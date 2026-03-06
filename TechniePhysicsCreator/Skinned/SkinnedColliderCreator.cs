using System;
using UnityEngine;

namespace Technie.PhysicsCreator.Skinned
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class SkinnedColliderCreator : MonoBehaviour, ICreatorComponent
	{
		private void OnDestroy()
		{
		}

		private void OnEnable()
		{
			this.targetSkinnedRenderer = base.gameObject.GetComponent<SkinnedMeshRenderer>();
		}

		public GameObject GetGameObject()
		{
			return base.gameObject;
		}

		public bool HasEditorData()
		{
			return this.editorData != null;
		}

		public IEditorData GetEditorData()
		{
			return this.editorData;
		}

		public Transform FindBone(BoneData boneData)
		{
			if (boneData == null)
			{
				return null;
			}
			return SkinnedColliderCreator.FindBone(this.targetSkinnedRenderer, boneData.targetBoneName);
		}

		public Transform FindBone(BoneHullData hullData)
		{
			if (hullData == null)
			{
				return null;
			}
			return SkinnedColliderCreator.FindBone(this.targetSkinnedRenderer, hullData.targetBoneName);
		}

		public static Transform FindBone(SkinnedMeshRenderer skinnedRenderer, string nameToFind)
		{
			if (skinnedRenderer == null)
			{
				return null;
			}
			if (nameToFind == null)
			{
				return null;
			}
			foreach (Transform transform in skinnedRenderer.bones)
			{
				if (transform.name == nameToFind)
				{
					return transform;
				}
			}
			return null;
		}

		public SkinnedMeshRenderer targetSkinnedRenderer;

		public SkinnedColliderEditorData editorData;
	}
}
