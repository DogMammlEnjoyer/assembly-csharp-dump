using System;
using System.Linq;
using UnityEngine;

namespace Modio.Unity.Examples
{
	public class ModioUnityPlatformExampleLoader : MonoBehaviour
	{
		private void Awake()
		{
			RuntimePlatform platform = Application.platform;
			foreach (ModioUnityPlatformExampleLoader.PlatformExamples platformExamples in this.platformExamplesPerPlatform)
			{
				if (platformExamples.platforms.Contains(platform))
				{
					foreach (string text in platformExamples.prefabNames)
					{
						GameObject gameObject = Resources.Load<GameObject>(text);
						if (gameObject != null)
						{
							Debug.Log(string.Format("Instantiating platform {0} for platform {1}", text, platform));
							Object.Instantiate<GameObject>(gameObject, base.transform);
						}
						else
						{
							Debug.LogError(string.Format("Couldn't find expected platformExample {0} for platform {1}", text, platform));
						}
					}
				}
			}
		}

		[ContextMenu("TestAllPrefabNamesAreFound")]
		private void TestAllPrefabNamesAreFound()
		{
			bool flag = false;
			foreach (ModioUnityPlatformExampleLoader.PlatformExamples platformExamples in this.platformExamplesPerPlatform)
			{
				foreach (string text in platformExamples.prefabNames)
				{
					if (Resources.Load<GameObject>(text) == null)
					{
						Debug.LogError(string.Format("Couldn't find expected platformExample {0} for platform {1}", text, platformExamples.platforms.FirstOrDefault<RuntimePlatform>()));
						flag = true;
					}
				}
			}
			if (!flag)
			{
				Debug.Log("No issues found");
			}
		}

		[SerializeField]
		private ModioUnityPlatformExampleLoader.PlatformExamples[] platformExamplesPerPlatform;

		[Serializable]
		private class PlatformExamples
		{
			public RuntimePlatform[] platforms;

			public string[] prefabNames;
		}
	}
}
