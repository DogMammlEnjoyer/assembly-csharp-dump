using System;

namespace UnityEngine.Rendering.LookDev
{
	public class StageRuntimeInterface
	{
		public StageRuntimeInterface(Func<bool, GameObject> AddGameObject, Func<Camera> GetCamera, Func<Light> GetSunLight)
		{
			this.m_AddGameObject = AddGameObject;
			this.m_GetCamera = GetCamera;
			this.m_GetSunLight = GetSunLight;
		}

		public GameObject AddGameObject(bool persistent = false)
		{
			Func<bool, GameObject> addGameObject = this.m_AddGameObject;
			if (addGameObject == null)
			{
				return null;
			}
			return addGameObject(persistent);
		}

		public Camera camera
		{
			get
			{
				Func<Camera> getCamera = this.m_GetCamera;
				if (getCamera == null)
				{
					return null;
				}
				return getCamera();
			}
		}

		public Light sunLight
		{
			get
			{
				Func<Light> getSunLight = this.m_GetSunLight;
				if (getSunLight == null)
				{
					return null;
				}
				return getSunLight();
			}
		}

		private Func<bool, GameObject> m_AddGameObject;

		private Func<Camera> m_GetCamera;

		private Func<Light> m_GetSunLight;

		public object SRPData;
	}
}
