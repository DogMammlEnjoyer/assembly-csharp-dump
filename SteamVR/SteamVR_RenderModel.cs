using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Valve.VR
{
	[ExecuteInEditMode]
	public class SteamVR_RenderModel : MonoBehaviour
	{
		public string renderModelName { get; private set; }

		public bool initializedAttachPoints { get; set; }

		private void OnModelSkinSettingsHaveChanged(VREvent_t vrEvent)
		{
			if (!string.IsNullOrEmpty(this.renderModelName))
			{
				this.renderModelName = "";
				this.UpdateModel();
			}
		}

		public void SetMeshRendererState(bool state)
		{
			for (int i = 0; i < this.meshRenderers.Count; i++)
			{
				MeshRenderer meshRenderer = this.meshRenderers[i];
				if (meshRenderer != null)
				{
					meshRenderer.enabled = state;
				}
			}
		}

		private void OnHideRenderModels(bool hidden)
		{
			this.SetMeshRendererState(!hidden);
		}

		private void OnDeviceConnected(int i, bool connected)
		{
			if (i != (int)this.index)
			{
				return;
			}
			if (connected)
			{
				this.UpdateModel();
			}
		}

		public void UpdateModel()
		{
			CVRSystem system = OpenVR.System;
			if (system == null || this.index == SteamVR_TrackedObject.EIndex.None)
			{
				return;
			}
			ETrackedPropertyError etrackedPropertyError = ETrackedPropertyError.TrackedProp_Success;
			uint stringTrackedDeviceProperty = system.GetStringTrackedDeviceProperty((uint)this.index, ETrackedDeviceProperty.Prop_RenderModelName_String, null, 0U, ref etrackedPropertyError);
			if (stringTrackedDeviceProperty <= 1U)
			{
				Debug.LogError("<b>[SteamVR]</b> Failed to get render model name for tracked object " + this.index.ToString());
				return;
			}
			StringBuilder stringBuilder = new StringBuilder((int)stringTrackedDeviceProperty);
			system.GetStringTrackedDeviceProperty((uint)this.index, ETrackedDeviceProperty.Prop_RenderModelName_String, stringBuilder, stringTrackedDeviceProperty, ref etrackedPropertyError);
			string text = stringBuilder.ToString();
			if (this.renderModelName != text)
			{
				base.StartCoroutine(this.SetModelAsync(text));
			}
		}

		private IEnumerator SetModelAsync(string newRenderModelName)
		{
			this.meshRenderers.Clear();
			if (string.IsNullOrEmpty(newRenderModelName))
			{
				yield break;
			}
			using (SteamVR_RenderModel.RenderModelInterfaceHolder holder = new SteamVR_RenderModel.RenderModelInterfaceHolder())
			{
				CVRRenderModels renderModels = holder.instance;
				if (renderModels == null)
				{
					yield break;
				}
				uint componentCount = renderModels.GetComponentCount(newRenderModelName);
				string[] renderModelNames;
				if (componentCount > 0U)
				{
					renderModelNames = new string[componentCount];
					int num = 0;
					while ((long)num < (long)((ulong)componentCount))
					{
						uint num2 = renderModels.GetComponentName(newRenderModelName, (uint)num, null, 0U);
						if (num2 != 0U)
						{
							StringBuilder stringBuilder = new StringBuilder((int)num2);
							if (renderModels.GetComponentName(newRenderModelName, (uint)num, stringBuilder, num2) != 0U)
							{
								string pchComponentName = stringBuilder.ToString();
								num2 = renderModels.GetComponentRenderModelName(newRenderModelName, pchComponentName, null, 0U);
								if (num2 != 0U)
								{
									StringBuilder stringBuilder2 = new StringBuilder((int)num2);
									if (renderModels.GetComponentRenderModelName(newRenderModelName, pchComponentName, stringBuilder2, num2) != 0U)
									{
										string text = stringBuilder2.ToString();
										SteamVR_RenderModel.RenderModel renderModel = SteamVR_RenderModel.models[text] as SteamVR_RenderModel.RenderModel;
										if (renderModel == null || renderModel.mesh == null)
										{
											renderModelNames[num] = text;
										}
									}
								}
							}
						}
						num++;
					}
				}
				else
				{
					SteamVR_RenderModel.RenderModel renderModel2 = SteamVR_RenderModel.models[newRenderModelName] as SteamVR_RenderModel.RenderModel;
					if (renderModel2 == null || renderModel2.mesh == null)
					{
						renderModelNames = new string[]
						{
							newRenderModelName
						};
					}
					else
					{
						renderModelNames = new string[0];
					}
				}
				for (;;)
				{
					bool flag = false;
					for (int i = 0; i < renderModelNames.Length; i++)
					{
						if (!string.IsNullOrEmpty(renderModelNames[i]))
						{
							IntPtr zero = IntPtr.Zero;
							EVRRenderModelError evrrenderModelError = renderModels.LoadRenderModel_Async(renderModelNames[i], ref zero);
							if (evrrenderModelError == EVRRenderModelError.Loading)
							{
								flag = true;
							}
							else if (evrrenderModelError == EVRRenderModelError.None)
							{
								RenderModel_t renderModel_t = this.MarshalRenderModel(zero);
								Material material = SteamVR_RenderModel.materials[renderModel_t.diffuseTextureId] as Material;
								if (material == null || material.mainTexture == null)
								{
									IntPtr zero2 = IntPtr.Zero;
									evrrenderModelError = renderModels.LoadTexture_Async(renderModel_t.diffuseTextureId, ref zero2);
									if (evrrenderModelError == EVRRenderModelError.Loading)
									{
										flag = true;
									}
								}
							}
						}
					}
					if (!flag)
					{
						break;
					}
					yield return new WaitForSecondsRealtime(0.1f);
				}
				renderModels = null;
				renderModelNames = null;
			}
			SteamVR_RenderModel.RenderModelInterfaceHolder holder = null;
			bool arg = this.SetModel(newRenderModelName);
			this.renderModelName = newRenderModelName;
			SteamVR_Events.RenderModelLoaded.Send(this, arg);
			yield break;
			yield break;
		}

		private bool SetModel(string renderModelName)
		{
			this.StripMesh(base.gameObject);
			using (SteamVR_RenderModel.RenderModelInterfaceHolder renderModelInterfaceHolder = new SteamVR_RenderModel.RenderModelInterfaceHolder())
			{
				if (this.createComponents)
				{
					this.componentAttachPoints.Clear();
					if (this.LoadComponents(renderModelInterfaceHolder, renderModelName))
					{
						this.UpdateComponents(renderModelInterfaceHolder.instance);
						return true;
					}
					Debug.Log("<b>[SteamVR]</b> [" + base.gameObject.name + "] Render model does not support components, falling back to single mesh.");
				}
				if (!string.IsNullOrEmpty(renderModelName))
				{
					SteamVR_RenderModel.RenderModel renderModel = SteamVR_RenderModel.models[renderModelName] as SteamVR_RenderModel.RenderModel;
					if (renderModel == null || renderModel.mesh == null)
					{
						CVRRenderModels instance = renderModelInterfaceHolder.instance;
						if (instance == null)
						{
							return false;
						}
						if (this.verbose)
						{
							Debug.Log("<b>[SteamVR]</b> Loading render model " + renderModelName);
						}
						renderModel = this.LoadRenderModel(instance, renderModelName, renderModelName);
						if (renderModel == null)
						{
							return false;
						}
						SteamVR_RenderModel.models[renderModelName] = renderModel;
					}
					base.gameObject.AddComponent<MeshFilter>().mesh = renderModel.mesh;
					MeshRenderer meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
					meshRenderer.sharedMaterial = renderModel.material;
					this.meshRenderers.Add(meshRenderer);
					return true;
				}
			}
			return false;
		}

		private SteamVR_RenderModel.RenderModel LoadRenderModel(CVRRenderModels renderModels, string renderModelName, string baseName)
		{
			IntPtr zero = IntPtr.Zero;
			EVRRenderModelError evrrenderModelError;
			for (;;)
			{
				evrrenderModelError = renderModels.LoadRenderModel_Async(renderModelName, ref zero);
				if (evrrenderModelError != EVRRenderModelError.Loading)
				{
					break;
				}
				SteamVR_RenderModel.Sleep();
			}
			if (evrrenderModelError != EVRRenderModelError.None)
			{
				Debug.LogError(string.Format("<b>[SteamVR]</b> Failed to load render model {0} - {1}", renderModelName, evrrenderModelError.ToString()));
				return null;
			}
			RenderModel_t renderModel_t = this.MarshalRenderModel(zero);
			Vector3[] array = new Vector3[renderModel_t.unVertexCount];
			Vector3[] array2 = new Vector3[renderModel_t.unVertexCount];
			Vector2[] array3 = new Vector2[renderModel_t.unVertexCount];
			Type typeFromHandle = typeof(RenderModel_Vertex_t);
			int num = 0;
			while ((long)num < (long)((ulong)renderModel_t.unVertexCount))
			{
				RenderModel_Vertex_t renderModel_Vertex_t = (RenderModel_Vertex_t)Marshal.PtrToStructure(new IntPtr(renderModel_t.rVertexData.ToInt64() + (long)(num * Marshal.SizeOf(typeFromHandle))), typeFromHandle);
				array[num] = new Vector3(renderModel_Vertex_t.vPosition.v0, renderModel_Vertex_t.vPosition.v1, -renderModel_Vertex_t.vPosition.v2);
				array2[num] = new Vector3(renderModel_Vertex_t.vNormal.v0, renderModel_Vertex_t.vNormal.v1, -renderModel_Vertex_t.vNormal.v2);
				array3[num] = new Vector2(renderModel_Vertex_t.rfTextureCoord0, renderModel_Vertex_t.rfTextureCoord1);
				num++;
			}
			uint num2 = renderModel_t.unTriangleCount * 3U;
			short[] array4 = new short[num2];
			Marshal.Copy(renderModel_t.rIndexData, array4, 0, array4.Length);
			int[] array5 = new int[num2];
			int num3 = 0;
			while ((long)num3 < (long)((ulong)renderModel_t.unTriangleCount))
			{
				array5[num3 * 3] = (int)array4[num3 * 3 + 2];
				array5[num3 * 3 + 1] = (int)array4[num3 * 3 + 1];
				array5[num3 * 3 + 2] = (int)array4[num3 * 3];
				num3++;
			}
			Mesh mesh = new Mesh();
			mesh.vertices = array;
			mesh.normals = array2;
			mesh.uv = array3;
			mesh.triangles = array5;
			Material material = SteamVR_RenderModel.materials[renderModel_t.diffuseTextureId] as Material;
			if (material == null || material.mainTexture == null)
			{
				IntPtr zero2 = IntPtr.Zero;
				for (;;)
				{
					evrrenderModelError = renderModels.LoadTexture_Async(renderModel_t.diffuseTextureId, ref zero2);
					if (evrrenderModelError != EVRRenderModelError.Loading)
					{
						break;
					}
					SteamVR_RenderModel.Sleep();
				}
				if (evrrenderModelError == EVRRenderModelError.None)
				{
					RenderModel_TextureMap_t renderModel_TextureMap_t = this.MarshalRenderModel_TextureMap(zero2);
					Texture2D texture2D = new Texture2D((int)renderModel_TextureMap_t.unWidth, (int)renderModel_TextureMap_t.unHeight, TextureFormat.RGBA32, false);
					if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11)
					{
						texture2D.Apply();
						IntPtr nativeTexturePtr = texture2D.GetNativeTexturePtr();
						for (;;)
						{
							evrrenderModelError = renderModels.LoadIntoTextureD3D11_Async(renderModel_t.diffuseTextureId, nativeTexturePtr);
							if (evrrenderModelError != EVRRenderModelError.Loading)
							{
								break;
							}
							SteamVR_RenderModel.Sleep();
						}
					}
					else
					{
						byte[] array6 = new byte[(int)(renderModel_TextureMap_t.unWidth * renderModel_TextureMap_t.unHeight * 4)];
						Marshal.Copy(renderModel_TextureMap_t.rubTextureMapData, array6, 0, array6.Length);
						Color32[] array7 = new Color32[(int)(renderModel_TextureMap_t.unWidth * renderModel_TextureMap_t.unHeight)];
						int num4 = 0;
						for (int i = 0; i < (int)renderModel_TextureMap_t.unHeight; i++)
						{
							for (int j = 0; j < (int)renderModel_TextureMap_t.unWidth; j++)
							{
								byte r = array6[num4++];
								byte g = array6[num4++];
								byte b = array6[num4++];
								byte a = array6[num4++];
								array7[i * (int)renderModel_TextureMap_t.unWidth + j] = new Color32(r, g, b, a);
							}
						}
						texture2D.SetPixels32(array7);
						texture2D.Apply();
					}
					material = new Material((this.shader != null) ? this.shader : Shader.Find("Universal Render Pipeline/Lit"));
					material.mainTexture = texture2D;
					SteamVR_RenderModel.materials[renderModel_t.diffuseTextureId] = material;
					renderModels.FreeTexture(zero2);
				}
				else
				{
					Debug.Log("<b>[SteamVR]</b> Failed to load render model texture for render model " + renderModelName + ". Error: " + evrrenderModelError.ToString());
				}
			}
			base.StartCoroutine(this.FreeRenderModel(zero));
			return new SteamVR_RenderModel.RenderModel(mesh, material);
		}

		private IEnumerator FreeRenderModel(IntPtr pRenderModel)
		{
			yield return new WaitForSeconds(1f);
			using (SteamVR_RenderModel.RenderModelInterfaceHolder renderModelInterfaceHolder = new SteamVR_RenderModel.RenderModelInterfaceHolder())
			{
				renderModelInterfaceHolder.instance.FreeRenderModel(pRenderModel);
				yield break;
			}
			yield break;
		}

		public Transform FindTransformByName(string componentName, Transform inTransform = null)
		{
			if (inTransform == null)
			{
				inTransform = base.transform;
			}
			for (int i = 0; i < inTransform.childCount; i++)
			{
				Transform child = inTransform.GetChild(i);
				if (child.name == componentName)
				{
					return child;
				}
			}
			return null;
		}

		public Transform GetComponentTransform(string componentName)
		{
			if (componentName == null)
			{
				return base.transform;
			}
			if (this.componentAttachPoints.ContainsKey(componentName))
			{
				return this.componentAttachPoints[componentName];
			}
			return null;
		}

		private void StripMesh(GameObject go)
		{
			MeshRenderer component = go.GetComponent<MeshRenderer>();
			if (component != null)
			{
				Object.DestroyImmediate(component);
			}
			MeshFilter component2 = go.GetComponent<MeshFilter>();
			if (component2 != null)
			{
				Object.DestroyImmediate(component2);
			}
		}

		private bool LoadComponents(SteamVR_RenderModel.RenderModelInterfaceHolder holder, string renderModelName)
		{
			Transform transform = base.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				child.gameObject.SetActive(false);
				this.StripMesh(child.gameObject);
			}
			if (string.IsNullOrEmpty(renderModelName))
			{
				return true;
			}
			CVRRenderModels instance = holder.instance;
			if (instance == null)
			{
				return false;
			}
			uint componentCount = instance.GetComponentCount(renderModelName);
			if (componentCount == 0U)
			{
				return false;
			}
			int num = 0;
			while ((long)num < (long)((ulong)componentCount))
			{
				uint num2 = instance.GetComponentName(renderModelName, (uint)num, null, 0U);
				if (num2 != 0U)
				{
					StringBuilder stringBuilder = new StringBuilder((int)num2);
					if (instance.GetComponentName(renderModelName, (uint)num, stringBuilder, num2) != 0U)
					{
						string text = stringBuilder.ToString();
						transform = this.FindTransformByName(text, null);
						if (transform != null)
						{
							transform.gameObject.SetActive(true);
							this.componentAttachPoints[text] = this.FindTransformByName("attach", transform);
						}
						else
						{
							transform = new GameObject(text).transform;
							transform.parent = base.transform;
							transform.gameObject.layer = base.gameObject.layer;
							Transform transform2 = new GameObject("attach").transform;
							transform2.parent = transform;
							transform2.localPosition = Vector3.zero;
							transform2.localRotation = Quaternion.identity;
							transform2.localScale = Vector3.one;
							transform2.gameObject.layer = base.gameObject.layer;
							this.componentAttachPoints[text] = transform2;
						}
						transform.localPosition = Vector3.zero;
						transform.localRotation = Quaternion.identity;
						transform.localScale = Vector3.one;
						num2 = instance.GetComponentRenderModelName(renderModelName, text, null, 0U);
						if (num2 != 0U)
						{
							StringBuilder stringBuilder2 = new StringBuilder((int)num2);
							if (instance.GetComponentRenderModelName(renderModelName, text, stringBuilder2, num2) != 0U)
							{
								string text2 = stringBuilder2.ToString();
								SteamVR_RenderModel.RenderModel renderModel = SteamVR_RenderModel.models[text2] as SteamVR_RenderModel.RenderModel;
								if (renderModel == null || renderModel.mesh == null)
								{
									if (this.verbose)
									{
										Debug.Log("<b>[SteamVR]</b> Loading render model " + text2);
									}
									renderModel = this.LoadRenderModel(instance, text2, renderModelName);
									if (renderModel == null)
									{
										goto IL_262;
									}
									SteamVR_RenderModel.models[text2] = renderModel;
								}
								transform.gameObject.AddComponent<MeshFilter>().mesh = renderModel.mesh;
								MeshRenderer meshRenderer = transform.gameObject.AddComponent<MeshRenderer>();
								meshRenderer.sharedMaterial = renderModel.material;
								this.meshRenderers.Add(meshRenderer);
							}
						}
					}
				}
				IL_262:
				num++;
			}
			return true;
		}

		private SteamVR_RenderModel()
		{
			this.deviceConnectedAction = SteamVR_Events.DeviceConnectedAction(new UnityAction<int, bool>(this.OnDeviceConnected));
			this.hideRenderModelsAction = SteamVR_Events.HideRenderModelsAction(new UnityAction<bool>(this.OnHideRenderModels));
			this.modelSkinSettingsHaveChangedAction = SteamVR_Events.SystemAction(EVREventType.VREvent_ModelSkinSettingsHaveChanged, new UnityAction<VREvent_t>(this.OnModelSkinSettingsHaveChanged));
		}

		private void OnEnable()
		{
			if (!string.IsNullOrEmpty(this.modelOverride))
			{
				Debug.Log("<b>[SteamVR]</b> Model override is really only meant to be used in the scene view for lining things up; using it at runtime is discouraged.  Use tracked device index instead to ensure the correct model is displayed for all users.");
				base.enabled = false;
				return;
			}
			CVRSystem system = OpenVR.System;
			if (system != null && system.IsTrackedDeviceConnected((uint)this.index))
			{
				this.UpdateModel();
			}
			this.deviceConnectedAction.enabled = true;
			this.hideRenderModelsAction.enabled = true;
			this.modelSkinSettingsHaveChangedAction.enabled = true;
		}

		private void OnDisable()
		{
			this.deviceConnectedAction.enabled = false;
			this.hideRenderModelsAction.enabled = false;
			this.modelSkinSettingsHaveChangedAction.enabled = false;
		}

		private void Update()
		{
			if (this.updateDynamically)
			{
				this.UpdateComponents(OpenVR.RenderModels);
			}
		}

		public void UpdateComponents(CVRRenderModels renderModels)
		{
			if (renderModels == null)
			{
				return;
			}
			if (base.transform.childCount == 0)
			{
				return;
			}
			if (this.nameCache == null)
			{
				this.nameCache = new Dictionary<int, string>();
			}
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				string name;
				if (!this.nameCache.TryGetValue(child.GetInstanceID(), out name))
				{
					name = child.name;
					this.nameCache.Add(child.GetInstanceID(), name);
				}
				RenderModel_ComponentState_t renderModel_ComponentState_t = default(RenderModel_ComponentState_t);
				if (renderModels.GetComponentStateForDevicePath(this.renderModelName, name, SteamVR_Input_Source.GetHandle(this.inputSource), ref this.controllerModeState, ref renderModel_ComponentState_t))
				{
					child.localPosition = renderModel_ComponentState_t.mTrackingToComponentRenderModel.GetPosition();
					child.localRotation = renderModel_ComponentState_t.mTrackingToComponentRenderModel.GetRotation();
					Transform transform = null;
					for (int j = 0; j < child.childCount; j++)
					{
						Transform child2 = child.GetChild(j);
						int instanceID = child2.GetInstanceID();
						string name2;
						if (!this.nameCache.TryGetValue(instanceID, out name2))
						{
							name2 = child2.name;
							this.nameCache.Add(instanceID, name);
						}
						if (name2 == "attach")
						{
							transform = child2;
						}
					}
					if (transform != null)
					{
						transform.position = base.transform.TransformPoint(renderModel_ComponentState_t.mTrackingToComponentLocal.GetPosition());
						transform.rotation = base.transform.rotation * renderModel_ComponentState_t.mTrackingToComponentLocal.GetRotation();
						this.initializedAttachPoints = true;
					}
					bool flag = (renderModel_ComponentState_t.uProperties & 2U) > 0U;
					if (flag != child.gameObject.activeSelf)
					{
						child.gameObject.SetActive(flag);
					}
				}
			}
		}

		public void SetDeviceIndex(int newIndex)
		{
			this.index = (SteamVR_TrackedObject.EIndex)newIndex;
			this.modelOverride = "";
			if (base.enabled)
			{
				this.UpdateModel();
			}
		}

		public void SetInputSource(SteamVR_Input_Sources newInputSource)
		{
			this.inputSource = newInputSource;
		}

		private static void Sleep()
		{
			Thread.Sleep(1);
		}

		private RenderModel_t MarshalRenderModel(IntPtr pRenderModel)
		{
			if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
			{
				RenderModel_t_Packed renderModel_t_Packed = (RenderModel_t_Packed)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_t_Packed));
				RenderModel_t result = default(RenderModel_t);
				renderModel_t_Packed.Unpack(ref result);
				return result;
			}
			return (RenderModel_t)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_t));
		}

		private RenderModel_TextureMap_t MarshalRenderModel_TextureMap(IntPtr pRenderModel)
		{
			if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
			{
				RenderModel_TextureMap_t_Packed renderModel_TextureMap_t_Packed = (RenderModel_TextureMap_t_Packed)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_TextureMap_t_Packed));
				RenderModel_TextureMap_t result = default(RenderModel_TextureMap_t);
				renderModel_TextureMap_t_Packed.Unpack(ref result);
				return result;
			}
			return (RenderModel_TextureMap_t)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_TextureMap_t));
		}

		public SteamVR_TrackedObject.EIndex index = SteamVR_TrackedObject.EIndex.None;

		protected SteamVR_Input_Sources inputSource;

		public const string modelOverrideWarning = "Model override is really only meant to be used in the scene view for lining things up; using it at runtime is discouraged.  Use tracked device index instead to ensure the correct model is displayed for all users.";

		[Tooltip("Model override is really only meant to be used in the scene view for lining things up; using it at runtime is discouraged.  Use tracked device index instead to ensure the correct model is displayed for all users.")]
		public string modelOverride;

		[Tooltip("Shader to apply to model.")]
		public Shader shader;

		[Tooltip("Enable to print out when render models are loaded.")]
		public bool verbose;

		[Tooltip("If available, break down into separate components instead of loading as a single mesh.")]
		public bool createComponents = true;

		[Tooltip("Update transforms of components at runtime to reflect user action.")]
		public bool updateDynamically = true;

		public RenderModel_ControllerMode_State_t controllerModeState;

		public const string k_localTransformName = "attach";

		private Dictionary<string, Transform> componentAttachPoints = new Dictionary<string, Transform>();

		private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

		public static Hashtable models = new Hashtable();

		public static Hashtable materials = new Hashtable();

		private SteamVR_Events.Action deviceConnectedAction;

		private SteamVR_Events.Action hideRenderModelsAction;

		private SteamVR_Events.Action modelSkinSettingsHaveChangedAction;

		private Dictionary<int, string> nameCache;

		public class RenderModel
		{
			public RenderModel(Mesh mesh, Material material)
			{
				this.mesh = mesh;
				this.material = material;
			}

			public Mesh mesh { get; private set; }

			public Material material { get; private set; }
		}

		public sealed class RenderModelInterfaceHolder : IDisposable
		{
			public CVRRenderModels instance
			{
				get
				{
					if (this._instance == null && !this.failedLoadInterface)
					{
						if (Application.isEditor && !Application.isPlaying)
						{
							this.needsShutdown = SteamVR.InitializeTemporarySession(false);
						}
						this._instance = OpenVR.RenderModels;
						if (this._instance == null)
						{
							Debug.LogError("<b>[SteamVR]</b> Failed to load IVRRenderModels interface version IVRRenderModels_006");
							this.failedLoadInterface = true;
						}
					}
					return this._instance;
				}
			}

			public void Dispose()
			{
				if (this.needsShutdown)
				{
					SteamVR.ExitTemporarySession();
				}
			}

			private bool needsShutdown;

			private bool failedLoadInterface;

			private CVRRenderModels _instance;
		}
	}
}
