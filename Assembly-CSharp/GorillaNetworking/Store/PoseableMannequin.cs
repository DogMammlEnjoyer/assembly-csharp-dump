using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking.Store
{
	public class PoseableMannequin : MonoBehaviour
	{
		public void Start()
		{
			if (this.skinnedMeshRenderer)
			{
				this.skinnedMeshRenderer.gameObject.SetActive(false);
			}
			if (this.staticGorillaMesh)
			{
				this.staticGorillaMesh.gameObject.SetActive(true);
			}
		}

		private string GetPrefabPathFromCurrentPrefabStage()
		{
			return "";
		}

		private string GetMeshPathFromPrefabPath(string prefabPath)
		{
			return "";
		}

		public void BakeSkinnedMesh()
		{
			this.BakeAndSaveMeshInPath(this.GetMeshPathFromPrefabPath(this.GetPrefabPathFromCurrentPrefabStage()));
		}

		public void BakeAndSaveMeshInPath(string meshPath)
		{
		}

		private void UpdateStaticMeshMannequin()
		{
			this.staticGorillaMesh.sharedMesh = this.BakedColliderMesh;
			this.staticGorillaMeshRenderer.sharedMaterials = this.skinnedMeshRenderer.sharedMaterials;
			this.staticGorillaMeshCollider.sharedMesh = this.BakedColliderMesh;
		}

		private void UpdateSkinnedMeshCollider()
		{
			this.skinnedMeshCollider.sharedMesh = this.BakedColliderMesh;
		}

		public void UpdateGTPosRotConstraints()
		{
			GTPosRotConstraints[] array = this.cosmeticConstraints;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].constraints.ForEach(delegate(GorillaPosRotConstraint c)
				{
					c.follower.rotation = c.source.rotation;
					c.follower.position = c.source.position;
				});
			}
		}

		private void HookupCosmeticConstraints()
		{
			this.cosmeticConstraints = base.GetComponentsInChildren<GTPosRotConstraints>();
			foreach (GTPosRotConstraints gtposRotConstraints in this.cosmeticConstraints)
			{
				for (int j = 0; j < gtposRotConstraints.constraints.Length; j++)
				{
					gtposRotConstraints.constraints[j].source = this.FindBone(gtposRotConstraints.constraints[j].follower.name);
				}
			}
		}

		private Transform FindBone(string boneName)
		{
			foreach (Transform transform in this.skinnedMeshRenderer.bones)
			{
				if (transform.name == boneName)
				{
					return transform;
				}
			}
			return null;
		}

		public void CreasteTestClip()
		{
		}

		public void SerializeVRRig()
		{
			base.StartCoroutine(this.SaveLocalPlayerPose());
		}

		public IEnumerator SaveLocalPlayerPose()
		{
			yield return null;
			yield break;
		}

		public void SerializeOutBonesFromSkinnedMesh(SkinnedMeshRenderer paramSkinnedMeshRenderer)
		{
		}

		public void SetCurvesForBone(SkinnedMeshRenderer paramSkinnedMeshRenderer, AnimationClip clip, Transform bone)
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.x)
			};
			Keyframe[] keys2 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.y)
			};
			Keyframe[] keys3 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.z)
			};
			Keyframe[] keys4 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.w)
			};
			AnimationCurve curve = new AnimationCurve(keys);
			AnimationCurve curve2 = new AnimationCurve(keys2);
			AnimationCurve curve3 = new AnimationCurve(keys3);
			AnimationCurve curve4 = new AnimationCurve(keys4);
			string relativePath = "";
			string b = bone.name.Replace("_new", "");
			foreach (Transform transform in this.skinnedMeshRenderer.bones)
			{
				if (transform.name == b)
				{
					relativePath = transform.GetPath(this.skinnedMeshRenderer.transform.parent).TrimStart('/');
					break;
				}
			}
			clip.SetCurve(relativePath, typeof(Transform), "m_LocalRotation.x", curve);
			clip.SetCurve(relativePath, typeof(Transform), "m_LocalRotation.y", curve2);
			clip.SetCurve(relativePath, typeof(Transform), "m_LocalRotation.z", curve3);
			clip.SetCurve(relativePath, typeof(Transform), "m_LocalRotation.w", curve4);
		}

		public void UpdatePrefabWithAnimationClip(string AnimationFileName)
		{
		}

		public void LoadPoseOntoMannequin(AnimationClip clip, float frameTime = 0f)
		{
		}

		public void OnValidate()
		{
		}

		public SkinnedMeshRenderer skinnedMeshRenderer;

		[FormerlySerializedAs("meshCollider")]
		public MeshCollider skinnedMeshCollider;

		public GTPosRotConstraints[] cosmeticConstraints;

		public Mesh BakedColliderMesh;

		[SerializeField]
		[FormerlySerializedAs("liveAssetPath")]
		protected string prefabAssetPath;

		[SerializeField]
		protected string prefabFolderPath;

		[SerializeField]
		protected string prefabAssetName;

		public MeshFilter staticGorillaMesh;

		public MeshCollider staticGorillaMeshCollider;

		public MeshRenderer staticGorillaMeshRenderer;
	}
}
