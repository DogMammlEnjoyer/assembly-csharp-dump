using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion
{
	public readonly struct SceneLoadDoneArgs
	{
		public SceneLoadDoneArgs(SceneRef sceneRef, NetworkObject[] sceneObjects, Scene scene = default(Scene), GameObject[] rootGameObjects = null)
		{
			this.SceneRef = sceneRef;
			this.SceneObjects = sceneObjects;
			this.Scene = scene;
			this.RootGameObjects = rootGameObjects;
		}

		public readonly SceneRef SceneRef;

		public readonly NetworkObject[] SceneObjects;

		public readonly Scene Scene;

		public readonly GameObject[] RootGameObjects;
	}
}
