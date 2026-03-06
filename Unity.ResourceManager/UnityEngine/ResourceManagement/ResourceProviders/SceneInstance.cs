using System;
using UnityEngine.SceneManagement;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	public struct SceneInstance
	{
		public Scene Scene
		{
			get
			{
				return this.m_Scene;
			}
			internal set
			{
				this.m_Scene = value;
			}
		}

		internal bool ReleaseSceneOnSceneUnloaded
		{
			get
			{
				return this.m_ReleaseOnSceneUnloaded;
			}
			set
			{
				this.m_ReleaseOnSceneUnloaded = value;
			}
		}

		public AsyncOperation ActivateAsync()
		{
			this.m_Operation.allowSceneActivation = true;
			return this.m_Operation;
		}

		public override int GetHashCode()
		{
			return this.Scene.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is SceneInstance && this.Scene.Equals(((SceneInstance)obj).Scene);
		}

		private Scene m_Scene;

		private bool m_ReleaseOnSceneUnloaded;

		internal AsyncOperation m_Operation;
	}
}
