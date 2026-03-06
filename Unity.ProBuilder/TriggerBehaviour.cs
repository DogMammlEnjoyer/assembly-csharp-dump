using System;
using UnityEngine.SceneManagement;

namespace UnityEngine.ProBuilder
{
	[DisallowMultipleComponent]
	internal sealed class TriggerBehaviour : EntityBehaviour
	{
		public override void Initialize()
		{
			Collider collider = base.gameObject.GetComponent<Collider>();
			if (!collider)
			{
				collider = base.gameObject.AddComponent<MeshCollider>();
			}
			MeshCollider meshCollider = collider as MeshCollider;
			if (meshCollider)
			{
				meshCollider.convex = true;
			}
			collider.isTrigger = true;
			base.SetMaterial(BuiltinMaterials.triggerMaterial);
		}

		public override void OnEnterPlayMode()
		{
			Renderer renderer;
			if (base.TryGetComponent<Renderer>(out renderer))
			{
				renderer.enabled = false;
			}
		}

		public override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			Renderer renderer;
			if (base.TryGetComponent<Renderer>(out renderer))
			{
				renderer.enabled = false;
			}
		}
	}
}
