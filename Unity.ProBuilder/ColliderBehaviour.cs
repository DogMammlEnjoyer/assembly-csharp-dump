using System;
using UnityEngine.SceneManagement;

namespace UnityEngine.ProBuilder
{
	[DisallowMultipleComponent]
	internal sealed class ColliderBehaviour : EntityBehaviour
	{
		public override void Initialize()
		{
			Collider collider = base.gameObject.GetComponent<Collider>();
			if (!collider)
			{
				collider = base.gameObject.AddComponent<MeshCollider>();
			}
			collider.isTrigger = false;
			base.SetMaterial(BuiltinMaterials.colliderMaterial);
		}

		public override void OnEnterPlayMode()
		{
			Renderer component = base.GetComponent<Renderer>();
			if (component != null)
			{
				component.enabled = false;
			}
		}

		public override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			Renderer component = base.GetComponent<Renderer>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
	}
}
