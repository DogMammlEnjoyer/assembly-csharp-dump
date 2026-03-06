using System;
using UnityEngine;

namespace Liv.Lck.DependencyInjection
{
	[DefaultExecutionOrder(-800)]
	public class LckDependencyResolver : MonoBehaviour
	{
		private void Awake()
		{
			LckMonoBehaviourDependencyInjector injector = LckDiContainer.Instance.GetInjector();
			if (injector == null)
			{
				Debug.LogError("LCK initialization error: Ensure LckServiceInitializer is in the scene");
				return;
			}
			foreach (MonoBehaviour instance in base.gameObject.GetComponents<MonoBehaviour>())
			{
				injector.Inject(instance);
			}
		}
	}
}
