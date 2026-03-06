using System;
using UnityEngine.SceneManagement;

namespace UnityEngine.ProBuilder
{
	internal abstract class EntityBehaviour : MonoBehaviour
	{
		public abstract void Initialize();

		public abstract void OnEnterPlayMode();

		public abstract void OnSceneLoaded(Scene scene, LoadSceneMode mode);

		protected void SetMaterial(Material material)
		{
			if (base.GetComponent<Renderer>())
			{
				base.GetComponent<Renderer>().sharedMaterial = material;
				return;
			}
			base.gameObject.AddComponent<MeshRenderer>().sharedMaterial = material;
		}

		[Tooltip("Allow ProBuilder to automatically hide and show this object when entering or exiting play mode.")]
		public bool manageVisibility = true;
	}
}
