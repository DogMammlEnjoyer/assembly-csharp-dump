using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Oculus.Interaction.Samples
{
	public class SceneLoader : MonoBehaviour
	{
		public void Load(string sceneName)
		{
			if (this._loading)
			{
				return;
			}
			this._loading = true;
			this._waitingCount = this.WhenLoadingScene.GetInvocationList().Length - 1;
			if (this._waitingCount == 0)
			{
				this.HandleReadyToLoad(sceneName);
				return;
			}
			this.WhenLoadingScene(sceneName);
		}

		public void HandleReadyToLoad(string sceneName)
		{
			this._waitingCount--;
			if (this._waitingCount <= 0)
			{
				base.StartCoroutine(this.LoadSceneAsync(sceneName));
			}
		}

		private IEnumerator LoadSceneAsync(string sceneName)
		{
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
			while (!asyncLoad.isDone)
			{
				yield return null;
			}
			this.WhenSceneLoaded(sceneName);
			yield break;
		}

		private bool _loading;

		public Action<string> WhenLoadingScene = delegate(string <p0>)
		{
		};

		public Action<string> WhenSceneLoaded = delegate(string <p0>)
		{
		};

		private int _waitingCount;
	}
}
