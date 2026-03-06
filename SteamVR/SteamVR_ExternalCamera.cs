using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

namespace Valve.VR
{
	public class SteamVR_ExternalCamera : MonoBehaviour
	{
		public void ReadConfig()
		{
			try
			{
				HmdMatrix34_t pose = default(HmdMatrix34_t);
				bool flag = false;
				object obj = this.config;
				string[] array = File.ReadAllLines(this.configPath);
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split('=', StringSplitOptions.None);
					if (array2.Length == 2)
					{
						string text = array2[0];
						if (text == "m")
						{
							string[] array3 = array2[1].Split(',', StringSplitOptions.None);
							if (array3.Length == 12)
							{
								pose.m0 = float.Parse(array3[0]);
								pose.m1 = float.Parse(array3[1]);
								pose.m2 = float.Parse(array3[2]);
								pose.m3 = float.Parse(array3[3]);
								pose.m4 = float.Parse(array3[4]);
								pose.m5 = float.Parse(array3[5]);
								pose.m6 = float.Parse(array3[6]);
								pose.m7 = float.Parse(array3[7]);
								pose.m8 = float.Parse(array3[8]);
								pose.m9 = float.Parse(array3[9]);
								pose.m10 = float.Parse(array3[10]);
								pose.m11 = float.Parse(array3[11]);
								flag = true;
							}
						}
						else if (text == "disableStandardAssets")
						{
							FieldInfo field = obj.GetType().GetField(text);
							if (field != null)
							{
								field.SetValue(obj, bool.Parse(array2[1]));
							}
						}
						else
						{
							FieldInfo field2 = obj.GetType().GetField(text);
							if (field2 != null)
							{
								field2.SetValue(obj, float.Parse(array2[1]));
							}
						}
					}
				}
				this.config = (SteamVR_ExternalCamera.Config)obj;
				if (flag)
				{
					SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(pose);
					this.config.x = rigidTransform.pos.x;
					this.config.y = rigidTransform.pos.y;
					this.config.z = rigidTransform.pos.z;
					Vector3 eulerAngles = rigidTransform.rot.eulerAngles;
					this.config.rx = eulerAngles.x;
					this.config.ry = eulerAngles.y;
					this.config.rz = eulerAngles.z;
				}
			}
			catch
			{
			}
			this.target = null;
			if (this.watcher == null)
			{
				FileInfo fileInfo = new FileInfo(this.configPath);
				this.watcher = new FileSystemWatcher(fileInfo.DirectoryName, fileInfo.Name);
				this.watcher.NotifyFilter = NotifyFilters.LastWrite;
				this.watcher.Changed += this.OnChanged;
				this.watcher.EnableRaisingEvents = true;
			}
		}

		public void SetupPose(SteamVR_Action_Pose newCameraPose, SteamVR_Input_Sources newCameraSource)
		{
			this.cameraPose = newCameraPose;
			this.cameraInputSource = newCameraSource;
			this.AutoEnableActionSet();
			SteamVR_Behaviour_Pose steamVR_Behaviour_Pose = base.gameObject.AddComponent<SteamVR_Behaviour_Pose>();
			steamVR_Behaviour_Pose.poseAction = newCameraPose;
			steamVR_Behaviour_Pose.inputSource = newCameraSource;
		}

		public void SetupDeviceIndex(int deviceIndex)
		{
			base.gameObject.AddComponent<SteamVR_TrackedObject>().SetDeviceIndex(deviceIndex);
		}

		private void OnChanged(object source, FileSystemEventArgs e)
		{
			this.ReadConfig();
		}

		public void AttachToCamera(SteamVR_Camera steamVR_Camera)
		{
			Camera camera;
			if (steamVR_Camera == null)
			{
				camera = Camera.main;
				if (this.target == camera.transform)
				{
					return;
				}
				this.target = camera.transform;
			}
			else
			{
				camera = steamVR_Camera.camera;
				if (this.target == steamVR_Camera.head)
				{
					return;
				}
				this.target = steamVR_Camera.head;
			}
			Transform parent = base.transform.parent;
			Transform parent2 = this.target.parent;
			parent.parent = parent2;
			parent.localPosition = Vector3.zero;
			parent.localRotation = Quaternion.identity;
			parent.localScale = Vector3.one;
			camera.enabled = false;
			GameObject gameObject = Object.Instantiate<GameObject>(camera.gameObject);
			camera.enabled = true;
			gameObject.name = "camera";
			Object.DestroyImmediate(gameObject.GetComponent<SteamVR_Camera>());
			Object.DestroyImmediate(gameObject.GetComponent<SteamVR_Fade>());
			this.cam = gameObject.GetComponent<Camera>();
			this.cam.stereoTargetEye = StereoTargetEyeMask.None;
			this.cam.fieldOfView = this.config.fov;
			this.cam.useOcclusionCulling = false;
			this.cam.enabled = false;
			this.cam.rect = new Rect(0f, 0f, 1f, 1f);
			this.colorMat = new Material(Shader.Find("Custom/SteamVR_ColorOut"));
			this.alphaMat = new Material(Shader.Find("Custom/SteamVR_AlphaOut"));
			this.clipMaterial = new Material(Shader.Find("Custom/SteamVR_ClearAll"));
			Transform transform = gameObject.transform;
			transform.parent = base.transform;
			transform.localPosition = new Vector3(this.config.x, this.config.y, this.config.z);
			transform.localRotation = Quaternion.Euler(this.config.rx, this.config.ry, this.config.rz);
			transform.localScale = Vector3.one;
			while (transform.childCount > 0)
			{
				Object.DestroyImmediate(transform.GetChild(0).gameObject);
			}
			this.clipQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			this.clipQuad.name = "ClipQuad";
			Object.DestroyImmediate(this.clipQuad.GetComponent<MeshCollider>());
			MeshRenderer component = this.clipQuad.GetComponent<MeshRenderer>();
			component.material = this.clipMaterial;
			component.shadowCastingMode = ShadowCastingMode.Off;
			component.receiveShadows = false;
			component.lightProbeUsage = LightProbeUsage.Off;
			component.reflectionProbeUsage = ReflectionProbeUsage.Off;
			Transform transform2 = this.clipQuad.transform;
			transform2.parent = transform;
			transform2.localScale = new Vector3(1000f, 1000f, 1f);
			transform2.localRotation = Quaternion.identity;
			this.clipQuad.SetActive(false);
		}

		public float GetTargetDistance()
		{
			if (this.target == null)
			{
				return this.config.near + 0.01f;
			}
			Transform transform = this.cam.transform;
			Vector3 normalized = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
			Vector3 inPoint = this.target.position + new Vector3(this.target.forward.x, 0f, this.target.forward.z).normalized * this.config.hmdOffset;
			return Mathf.Clamp(-new Plane(normalized, inPoint).GetDistanceToPoint(transform.position), this.config.near + 0.01f, this.config.far - 0.01f);
		}

		public void RenderNear()
		{
			int num = Screen.width / 2;
			int num2 = Screen.height / 2;
			if (this.cam.targetTexture == null || this.cam.targetTexture.width != num || this.cam.targetTexture.height != num2)
			{
				RenderTexture renderTexture = new RenderTexture(num, num2, 24, RenderTextureFormat.ARGB32);
				renderTexture.antiAliasing = ((QualitySettings.antiAliasing == 0) ? 1 : QualitySettings.antiAliasing);
				this.cam.targetTexture = renderTexture;
			}
			this.cam.nearClipPlane = this.config.near;
			this.cam.farClipPlane = this.config.far;
			CameraClearFlags clearFlags = this.cam.clearFlags;
			Color backgroundColor = this.cam.backgroundColor;
			this.cam.clearFlags = CameraClearFlags.Color;
			this.cam.backgroundColor = Color.clear;
			this.clipMaterial.color = new Color(this.config.r, this.config.g, this.config.b, this.config.a);
			float d = Mathf.Clamp(this.GetTargetDistance() + this.config.nearOffset, this.config.near, this.config.far);
			Transform parent = this.clipQuad.transform.parent;
			this.clipQuad.transform.position = parent.position + parent.forward * d;
			MonoBehaviour[] array = null;
			bool[] array2 = null;
			if (this.config.disableStandardAssets)
			{
				array = this.cam.gameObject.GetComponents<MonoBehaviour>();
				array2 = new bool[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					MonoBehaviour monoBehaviour = array[i];
					if (monoBehaviour.enabled && monoBehaviour.GetType().ToString().StartsWith("UnityStandardAssets."))
					{
						monoBehaviour.enabled = false;
						array2[i] = true;
					}
				}
			}
			this.clipQuad.SetActive(true);
			this.cam.Render();
			Graphics.DrawTexture(new Rect(0f, 0f, (float)num, (float)num2), this.cam.targetTexture, this.colorMat);
			MonoBehaviour monoBehaviour2 = this.cam.gameObject.GetComponent("PostProcessingBehaviour") as MonoBehaviour;
			if (monoBehaviour2 != null && monoBehaviour2.enabled)
			{
				monoBehaviour2.enabled = false;
				this.cam.Render();
				monoBehaviour2.enabled = true;
			}
			Graphics.DrawTexture(new Rect((float)num, 0f, (float)num, (float)num2), this.cam.targetTexture, this.alphaMat);
			this.clipQuad.SetActive(false);
			if (array != null)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if (array2[j])
					{
						array[j].enabled = true;
					}
				}
			}
			this.cam.clearFlags = clearFlags;
			this.cam.backgroundColor = backgroundColor;
		}

		public void RenderFar()
		{
			this.cam.nearClipPlane = this.config.near;
			this.cam.farClipPlane = this.config.far;
			this.cam.Render();
			int num = Screen.width / 2;
			int num2 = Screen.height / 2;
			Graphics.DrawTexture(new Rect(0f, (float)num2, (float)num, (float)num2), this.cam.targetTexture, this.colorMat);
		}

		private void OnGUI()
		{
		}

		private void OnEnable()
		{
			this.cameras = Object.FindObjectsOfType<Camera>();
			if (this.cameras != null)
			{
				int num = this.cameras.Length;
				this.cameraRects = new Rect[num];
				for (int i = 0; i < num; i++)
				{
					Camera camera = this.cameras[i];
					this.cameraRects[i] = camera.rect;
					if (!(camera == this.cam) && !(camera.targetTexture != null) && !(camera.GetComponent<SteamVR_Camera>() != null))
					{
						camera.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
					}
				}
			}
			if (this.config.sceneResolutionScale > 0f)
			{
				this.sceneResolutionScale = SteamVR_Camera.sceneResolutionScale;
				SteamVR_Camera.sceneResolutionScale = this.config.sceneResolutionScale;
			}
			this.AutoEnableActionSet();
		}

		private void AutoEnableActionSet()
		{
			if (this.autoEnableDisableActionSet && this.cameraPose != null && !this.cameraPose.actionSet.IsActive(this.cameraInputSource))
			{
				this.activatedActionSet = this.cameraPose.actionSet;
				this.activatedInputSource = this.cameraInputSource;
				this.cameraPose.actionSet.Activate(this.cameraInputSource, 0, false);
			}
		}

		private void OnDisable()
		{
			if (this.autoEnableDisableActionSet && this.activatedActionSet != null)
			{
				this.activatedActionSet.Deactivate(this.activatedInputSource);
				this.activatedActionSet = null;
			}
			if (this.cameras != null)
			{
				int num = this.cameras.Length;
				for (int i = 0; i < num; i++)
				{
					Camera camera = this.cameras[i];
					if (camera != null)
					{
						camera.rect = this.cameraRects[i];
					}
				}
				this.cameras = null;
				this.cameraRects = null;
			}
			if (this.config.sceneResolutionScale > 0f)
			{
				SteamVR_Camera.sceneResolutionScale = this.sceneResolutionScale;
			}
		}

		private SteamVR_Action_Pose cameraPose;

		private SteamVR_Input_Sources cameraInputSource = SteamVR_Input_Sources.Camera;

		[Space]
		public SteamVR_ExternalCamera.Config config;

		public string configPath;

		[Tooltip("This will automatically activate the action set the specified pose belongs to. And deactivate it when this component is disabled.")]
		public bool autoEnableDisableActionSet = true;

		private FileSystemWatcher watcher;

		private Camera cam;

		private Transform target;

		private GameObject clipQuad;

		private Material clipMaterial;

		protected SteamVR_ActionSet activatedActionSet;

		protected SteamVR_Input_Sources activatedInputSource;

		private Material colorMat;

		private Material alphaMat;

		private Camera[] cameras;

		private Rect[] cameraRects;

		private float sceneResolutionScale;

		[Serializable]
		public struct Config
		{
			public float x;

			public float y;

			public float z;

			public float rx;

			public float ry;

			public float rz;

			public float fov;

			public float near;

			public float far;

			public float sceneResolutionScale;

			public float frameSkip;

			public float nearOffset;

			public float farOffset;

			public float hmdOffset;

			public float r;

			public float g;

			public float b;

			public float a;

			public bool disableStandardAssets;
		}
	}
}
