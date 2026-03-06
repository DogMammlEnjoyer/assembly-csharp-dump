using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[DisallowMultipleComponent]
[HelpURL("https://developer.oculus.com/documentation/unity/VK-unity-IntegratePrefab/")]
[Feature(Feature.VirtualKeyboard)]
public class OVRVirtualKeyboard : MonoBehaviour
{
	[Obsolete("Use CommitTextEvent", false)]
	public event Action<string> CommitText;

	[Obsolete("Use BackspaceEvent", false)]
	public event Action Backspace;

	[Obsolete("Use EnterEvent", false)]
	public event Action Enter;

	[Obsolete("Use KeyboardShownEvent", false)]
	public event Action KeyboardShown;

	[Obsolete("Use KeyboardHiddenEvent", false)]
	public event Action KeyboardHidden;

	public Collider Collider { get; private set; }

	[Obsolete("TextCommitField has been replaced with TextHandler for more flexibility.")]
	public InputField TextCommitField
	{
		get
		{
			Debug.LogWarning("Migrate to TextHandler for better performance.");
			OVRVirtualKeyboardInputFieldTextHandler ovrvirtualKeyboardInputFieldTextHandler = this.textHandler as OVRVirtualKeyboardInputFieldTextHandler;
			if (ovrvirtualKeyboardInputFieldTextHandler == null)
			{
				return null;
			}
			return ovrvirtualKeyboardInputFieldTextHandler.InputField;
		}
		set
		{
			Debug.LogWarning("Migrate to TextHandler for better performance.");
			OVRVirtualKeyboardInputFieldTextHandler ovrvirtualKeyboardInputFieldTextHandler = this.TextHandler as OVRVirtualKeyboardInputFieldTextHandler;
			if (ovrvirtualKeyboardInputFieldTextHandler != null)
			{
				ovrvirtualKeyboardInputFieldTextHandler.InputField = value;
			}
		}
	}

	public OVRVirtualKeyboard.ITextHandler TextHandler
	{
		get
		{
			return this._runtimeTextHandler;
		}
		set
		{
			if (this._runtimeTextHandler == value)
			{
				return;
			}
			if (this._runtimeTextHandler != null)
			{
				OVRVirtualKeyboard.ITextHandler runtimeTextHandler = this._runtimeTextHandler;
				runtimeTextHandler.OnTextChanged = (Action<string>)Delegate.Remove(runtimeTextHandler.OnTextChanged, new Action<string>(this.OnTextHandlerChange));
			}
			this._runtimeTextHandler = value;
			if (this._runtimeTextHandler != null)
			{
				OVRVirtualKeyboard.ITextHandler runtimeTextHandler2 = this._runtimeTextHandler;
				runtimeTextHandler2.OnTextChanged = (Action<string>)Delegate.Combine(runtimeTextHandler2.OnTextChanged, new Action<string>(this.OnTextHandlerChange));
				this.ChangeTextContextInternal(this._runtimeTextHandler.Text);
			}
		}
	}

	private void Awake()
	{
		if (this.keyboardModelShader == null)
		{
			Debug.LogWarning("keyboardModelShader not specified; falling back to " + OVRVirtualKeyboard._defaultShaderName);
			this.keyboardModelShader = Shader.Find(OVRVirtualKeyboard._defaultShaderName);
		}
		if (this.keyboardModelAlphaBlendShader == null)
		{
			Debug.LogWarning("keyboardModelAlphaBlendShader not specified; falling back to " + OVRVirtualKeyboard._defaultAlphaBlendShaderName);
			this.keyboardModelAlphaBlendShader = Shader.Find(OVRVirtualKeyboard._defaultAlphaBlendShaderName);
		}
		if (OVRVirtualKeyboard.singleton_ != null)
		{
			Object.Destroy(this);
			throw new Exception("OVRVirtualKeyboard only supports a single instance");
		}
		if (this.leftControllerDirectTransform == null && this.leftControllerRootTransform != null)
		{
			if (this.controllerDirectInteraction)
			{
				Debug.LogWarning("Missing left controller direct transform for virtual keyboard input; falling back to the root!");
			}
			this.leftControllerDirectTransform = this.leftControllerRootTransform;
		}
		if (this.rightControllerDirectTransform == null && this.rightControllerRootTransform != null)
		{
			if (this.controllerDirectInteraction)
			{
				Debug.LogWarning("Missing right controller direct transform for virtual keyboard input; falling back to the root!");
			}
			this.rightControllerDirectTransform = this.rightControllerRootTransform;
		}
		OVRVirtualKeyboard.singleton_ = this;
		if (OVRManager.instance)
		{
			this.keyboardEventListener_ = new OVRVirtualKeyboard.KeyboardEventListener(this);
			OVRManager.instance.RegisterEventListener(this.keyboardEventListener_);
		}
		this.TextHandler = this.textHandler;
		this.CommitTextEvent.AddListener(new UnityAction<string>(this.OnCommitText));
		this.BackspaceEvent.AddListener(new UnityAction(this.OnBackspace));
		this.EnterEvent.AddListener(new UnityAction(this.OnEnter));
		this.KeyboardShownEvent.AddListener(new UnityAction(this.OnKeyboardShown));
		this.KeyboardHiddenEvent.AddListener(new UnityAction(this.OnKeyboardHidden));
	}

	private void OnDestroy()
	{
		if (!OVRPlugin.initialized)
		{
			return;
		}
		this.CommitTextEvent.RemoveListener(new UnityAction<string>(this.OnCommitText));
		this.BackspaceEvent.RemoveListener(new UnityAction(this.OnBackspace));
		this.EnterEvent.RemoveListener(new UnityAction(this.OnEnter));
		this.KeyboardShownEvent.RemoveListener(new UnityAction(this.OnKeyboardShown));
		this.KeyboardHiddenEvent.RemoveListener(new UnityAction(this.OnKeyboardHidden));
		foreach (OVRVirtualKeyboard.VirtualKeyboardTextureInfo virtualKeyboardTextureInfo in this.virtualKeyboardTextures_.Values)
		{
			if (virtualKeyboardTextureInfo.buffer != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(virtualKeyboardTextureInfo.buffer);
			}
		}
		this.virtualKeyboardTextures_.Clear();
		this.TextHandler = null;
		if (OVRVirtualKeyboard.singleton_ == this)
		{
			if (OVRManager.instance != null)
			{
				OVRManager.instance.DeregisterEventListener(this.keyboardEventListener_);
			}
			OVRVirtualKeyboard.singleton_ = null;
		}
		this.keyboardEventListener_ = null;
		this.DestroyKeyboard();
	}

	private void OnEnable()
	{
		this.ShowKeyboard();
	}

	private void OnDisable()
	{
		if (!OVRPlugin.initialized)
		{
			return;
		}
		this.HideKeyboard();
	}

	private void Reset()
	{
		this.keyboardModelShader = Shader.Find(OVRVirtualKeyboard._defaultShaderName);
		this.keyboardModelAlphaBlendShader = Shader.Find(OVRVirtualKeyboard._defaultAlphaBlendShaderName);
	}

	public void UseSuggestedLocation(OVRVirtualKeyboard.KeyboardPosition position)
	{
		OVRPlugin.VirtualKeyboardLocationInfo locationInfo = default(OVRPlugin.VirtualKeyboardLocationInfo);
		switch (position)
		{
		case OVRVirtualKeyboard.KeyboardPosition.Far:
			locationInfo.locationType = OVRPlugin.VirtualKeyboardLocationType.Far;
			break;
		case OVRVirtualKeyboard.KeyboardPosition.Near:
			locationInfo.locationType = OVRPlugin.VirtualKeyboardLocationType.Direct;
			break;
		case OVRVirtualKeyboard.KeyboardPosition.Custom:
			locationInfo = this.ComputeLocation(base.transform);
			break;
		default:
			Debug.LogError("Unknown KeyboardInputMode: " + position.ToString());
			return;
		}
		this.InitialPosition = position;
		if (this.keyboardSpace_ == 0UL)
		{
			return;
		}
		locationInfo.trackingOriginType = OVRPlugin.GetTrackingOriginType();
		OVRPlugin.Result result = OVRPlugin.SuggestVirtualKeyboardLocation(locationInfo);
		if (result != OVRPlugin.Result.Success)
		{
			Debug.LogError("SuggestVirtualKeyboardLocation failed: " + result.ToString());
			return;
		}
		base.transform.hasChanged = false;
		this.SyncKeyboardLocation();
	}

	public void SendVirtualKeyboardRayInput(Transform inputTransform, OVRVirtualKeyboard.InputSource source, bool isPressed, bool useRaycastMask = true)
	{
		OVRPlugin.VirtualKeyboardInputSource virtualKeyboardInputSource;
		switch (source)
		{
		case OVRVirtualKeyboard.InputSource.ControllerLeft:
			virtualKeyboardInputSource = OVRPlugin.VirtualKeyboardInputSource.ControllerRayLeft;
			break;
		case OVRVirtualKeyboard.InputSource.ControllerRight:
			virtualKeyboardInputSource = OVRPlugin.VirtualKeyboardInputSource.ControllerRayRight;
			break;
		case OVRVirtualKeyboard.InputSource.HandLeft:
			virtualKeyboardInputSource = OVRPlugin.VirtualKeyboardInputSource.HandRayLeft;
			break;
		case OVRVirtualKeyboard.InputSource.HandRight:
			virtualKeyboardInputSource = OVRPlugin.VirtualKeyboardInputSource.HandRayRight;
			break;
		default:
			throw new Exception("Unknown input source: " + source.ToString());
		}
		OVRPlugin.VirtualKeyboardInputSource inputSource = virtualKeyboardInputSource;
		OVRPhysicsRaycaster ovrphysicsRaycaster = (source == OVRVirtualKeyboard.InputSource.ControllerLeft || source == OVRVirtualKeyboard.InputSource.ControllerRight) ? this.controllerRaycaster : this.handRaycaster;
		if (ovrphysicsRaycaster)
		{
			OVRPointerEventData eventData = new OVRPointerEventData(EventSystem.current)
			{
				worldSpaceRay = new Ray(inputTransform.position, inputTransform.forward)
			};
			List<RaycastResult> list = new List<RaycastResult>();
			ovrphysicsRaycaster.Raycast(eventData, list);
			if (list.Count <= 0 || list[0].gameObject != this.Collider.gameObject)
			{
				return;
			}
		}
		this.SendVirtualKeyboardInput(inputSource, inputTransform.ToOVRPose(false), isPressed, null);
	}

	public void SendVirtualKeyboardDirectInput(Vector3 position, OVRVirtualKeyboard.InputSource source, bool isPressed, Transform interactorRootTransform = null)
	{
		OVRPlugin.VirtualKeyboardInputSource virtualKeyboardInputSource;
		switch (source)
		{
		case OVRVirtualKeyboard.InputSource.ControllerLeft:
			virtualKeyboardInputSource = OVRPlugin.VirtualKeyboardInputSource.ControllerDirectLeft;
			break;
		case OVRVirtualKeyboard.InputSource.ControllerRight:
			virtualKeyboardInputSource = OVRPlugin.VirtualKeyboardInputSource.ControllerDirectRight;
			break;
		case OVRVirtualKeyboard.InputSource.HandLeft:
			virtualKeyboardInputSource = OVRPlugin.VirtualKeyboardInputSource.HandDirectIndexTipLeft;
			break;
		case OVRVirtualKeyboard.InputSource.HandRight:
			virtualKeyboardInputSource = OVRPlugin.VirtualKeyboardInputSource.HandDirectIndexTipRight;
			break;
		default:
			throw new Exception("Unknown input source: " + source.ToString());
		}
		OVRPlugin.VirtualKeyboardInputSource inputSource = virtualKeyboardInputSource;
		this.SendVirtualKeyboardInput(inputSource, new OVRPose
		{
			position = position
		}, isPressed, interactorRootTransform);
	}

	public void ChangeTextContext(string textContext)
	{
		if (this.TextHandler != null && this.TextHandler.Text != textContext)
		{
			Debug.LogWarning("TextHandler text out of sync with Keyboard text context");
		}
		this.ChangeTextContextInternal(textContext);
	}

	private void LoadRuntimeVirtualKeyboardMesh()
	{
		this.modelAvailable_ = false;
		this.gltfModelCoroutine_ = base.StartCoroutine(this.InitializeGlTFModel());
	}

	private IEnumerator InitializeGlTFModel()
	{
		Func<MemoryStream> deferredStream = delegate()
		{
			Debug.Log("LoadRuntimeVirtualKeyboardMesh");
			string[] renderModelPaths = OVRPlugin.GetRenderModelPaths();
			string text;
			if (renderModelPaths == null)
			{
				text = null;
			}
			else
			{
				text = renderModelPaths.FirstOrDefault((string p) => p.Equals("/model_fb/virtual_keyboard") || p.Equals("/model_meta/keyboard/virtual"));
			}
			string text2 = text;
			if (string.IsNullOrEmpty(text2))
			{
				Debug.LogError("Failed to find keyboard model.  Check Render Model support.");
				return null;
			}
			OVRPlugin.RenderModelProperties renderModelProperties = default(OVRPlugin.RenderModelProperties);
			if (!OVRPlugin.GetRenderModelProperties(text2, ref renderModelProperties))
			{
				Debug.LogError("Failed to find keyboard model properties.  Check Render Model support.");
				return null;
			}
			if (renderModelProperties.ModelKey == 0UL)
			{
				Debug.LogError("Failed to find keyboard model key.  Check Render Model support.");
				return null;
			}
			this.virtualKeyboardModelKey_ = renderModelProperties.ModelKey;
			return new MemoryStream(OVRPlugin.LoadRenderModel(renderModelProperties.ModelKey));
		};
		this._gltfLoader = new OVRGLTFLoader(deferredStream);
		this._gltfLoader.textureUriHandler = delegate(string rawUri, Material mat)
		{
			Uri uri = new Uri(rawUri);
			if (!uri.Scheme.Equals("metaVirtualKeyboard", StringComparison.OrdinalIgnoreCase) || uri.Host != "texture")
			{
				return null;
			}
			ulong key = ulong.Parse(uri.LocalPath.Substring(1));
			OVRVirtualKeyboard.VirtualKeyboardTextureInfo virtualKeyboardTextureInfo;
			if (!this.virtualKeyboardTextures_.TryGetValue(key, out virtualKeyboardTextureInfo))
			{
				virtualKeyboardTextureInfo.materials = new List<Material>();
			}
			virtualKeyboardTextureInfo.materials.Add(mat);
			this.virtualKeyboardTextures_[key] = virtualKeyboardTextureInfo;
			return null;
		};
		this._gltfLoader.SetModelShader(this.keyboardModelShader);
		this._gltfLoader.SetModelAlphaBlendShader(this.keyboardModelAlphaBlendShader);
		IEnumerator loadGlbCoroutine = this._gltfLoader.LoadGLBCoroutine(true, true);
		while (loadGlbCoroutine.MoveNext())
		{
			object obj = loadGlbCoroutine.Current;
			yield return obj;
		}
		this.virtualKeyboardScene_ = this._gltfLoader.scene;
		this._gltfLoader = null;
		this.gltfModelCoroutine_ = null;
		this.modelAvailable_ = (this.virtualKeyboardScene_.root != null);
		if (this.modelAvailable_)
		{
			this.virtualKeyboardScene_.root.transform.SetParent(base.transform, false);
			this.virtualKeyboardScene_.root.gameObject.name = "OVRVirtualKeyboardModel";
			OVRVirtualKeyboard.ApplyHideFlags(this.virtualKeyboardScene_.root.transform);
			this.SetKeyboardVisibility(true);
			this.UseSuggestedLocation(this.InitialPosition);
			this.UpdateAnimationState();
			this.PopulateCollision();
		}
		yield break;
	}

	private static void ApplyHideFlags(Transform t)
	{
		t.gameObject.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset);
		for (int i = 0; i < t.childCount; i++)
		{
			OVRVirtualKeyboard.ApplyHideFlags(t.GetChild(i));
		}
	}

	private void PopulateCollision()
	{
		if (!this.modelAvailable_)
		{
			throw new Exception("Keyboard Model Unavailable");
		}
		MeshFilter meshFilter = (from mesh in this.virtualKeyboardScene_.root.GetComponentsInChildren<MeshFilter>()
		where mesh.gameObject.name == "collision"
		select mesh).FirstOrDefault<MeshFilter>();
		if (meshFilter != null)
		{
			MeshCollider meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
			meshCollider.convex = true;
			this.Collider = meshCollider;
			MeshRenderer component = meshFilter.gameObject.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
	}

	private void ShowKeyboard()
	{
		if (!this.isKeyboardCreated_)
		{
			OVRPlugin.Result result = OVRPlugin.CreateVirtualKeyboard(default(OVRPlugin.VirtualKeyboardCreateInfo));
			if (result != OVRPlugin.Result.Success)
			{
				Debug.LogError("Create failed: '" + result.ToString() + "'. Check for Virtual Keyboard Support.");
				return;
			}
			this.isKeyboardCreated_ = true;
		}
		if (!this.modelInitialized_)
		{
			this.modelInitialized_ = true;
			this.LoadRuntimeVirtualKeyboardMesh();
		}
		else
		{
			this.SetKeyboardVisibility(true);
		}
		if (this.TextHandler != null)
		{
			this.ChangeTextContextInternal(this.TextHandler.Text);
		}
	}

	private void SetKeyboardVisibility(bool visible)
	{
		if (!this.modelAvailable_)
		{
			return;
		}
		OVRPlugin.VirtualKeyboardModelVisibility virtualKeyboardModelVisibility = default(OVRPlugin.VirtualKeyboardModelVisibility);
		virtualKeyboardModelVisibility.Visible = visible;
		OVRPlugin.Result result = OVRPlugin.SetVirtualKeyboardModelVisibility(ref virtualKeyboardModelVisibility);
		if (result != OVRPlugin.Result.Success)
		{
			Debug.LogError("SetVirtualKeyboardModelVisibility failed: " + result.ToString());
		}
	}

	private void HideKeyboard()
	{
		if (this.modelInitialized_ && !this.modelAvailable_)
		{
			this.UnloadModel();
		}
		this.SetKeyboardVisibility(false);
	}

	private void UnloadModel()
	{
		if (this.gltfModelCoroutine_ != null)
		{
			base.StopCoroutine(this.gltfModelCoroutine_);
			this.gltfModelCoroutine_ = null;
		}
		if (this._gltfLoader != null && this._gltfLoader.scene.root != null)
		{
			Object.Destroy(this._gltfLoader.scene.root);
			this._gltfLoader = null;
		}
		if (this.modelAvailable_)
		{
			Object.Destroy(this.virtualKeyboardScene_.root);
			this.modelAvailable_ = false;
		}
		this.modelInitialized_ = false;
	}

	private void DestroyKeyboard()
	{
		this.UnloadModel();
		this.InputEnabled = false;
		if (this.isKeyboardCreated_)
		{
			if (OVRPlugin.DestroyVirtualKeyboard() != OVRPlugin.Result.Success)
			{
				Debug.LogError("Destroy failed");
			}
			else
			{
				Debug.Log("Destroy success");
			}
			this.isKeyboardCreated_ = false;
		}
		if (this._inputSources != null)
		{
			foreach (OVRVirtualKeyboard.IInputSource inputSource in this._inputSources)
			{
				inputSource.Dispose();
			}
		}
		this._inputSources = null;
	}

	private float MaxElement(Vector3 vec)
	{
		return Mathf.Max(Mathf.Max(vec.x, vec.y), vec.z);
	}

	private OVRPlugin.VirtualKeyboardLocationInfo ComputeLocation(Transform transform)
	{
		OVRPlugin.VirtualKeyboardLocationInfo result = default(OVRPlugin.VirtualKeyboardLocationInfo);
		result.locationType = OVRPlugin.VirtualKeyboardLocationType.Custom;
		result.pose.Position = transform.position.ToFlippedZVector3f();
		result.pose.Orientation = transform.rotation.ToFlippedZQuatf();
		result.scale = this.MaxElement(transform.localScale);
		return result;
	}

	private void Update()
	{
		if (!OVRPlugin.initialized)
		{
			return;
		}
		if (this.modelAvailable_)
		{
			this.UpdateInputs();
		}
		if (this.isKeyboardCreated_)
		{
			this.SyncKeyboardLocation();
		}
		if (this.modelAvailable_)
		{
			this.UpdateAnimationState();
		}
	}

	private void LateUpdate()
	{
		this._interactorRootTransformOverride.LateApply(this);
	}

	private void SendVirtualKeyboardInput(OVRPlugin.VirtualKeyboardInputSource inputSource, OVRPose pose, bool isPressed, Transform interactorRootTransform = null)
	{
		OVRPlugin.VirtualKeyboardInputInfo inputInfo = default(OVRPlugin.VirtualKeyboardInputInfo);
		inputInfo.inputTrackingOriginType = (OVRPlugin.TrackingOrigin)OVRManager.instance.trackingOriginType;
		inputInfo.inputSource = inputSource;
		inputInfo.inputPose = pose.ToPosef();
		inputInfo.inputState = (isPressed ? OVRPlugin.VirtualKeyboardInputStateFlags.IsPressed : ((OVRPlugin.VirtualKeyboardInputStateFlags)0UL));
		OVRPlugin.Posef interactorRootPose = (!(interactorRootTransform != null)) ? pose.ToPosef() : interactorRootTransform.ToOVRPose(false).ToPosef();
		if (OVRPlugin.SendVirtualKeyboardInput(inputInfo, ref interactorRootPose) != OVRPlugin.Result.Success)
		{
			return;
		}
		if (interactorRootTransform != null)
		{
			this._interactorRootTransformOverride.Enqueue(interactorRootTransform, interactorRootPose);
		}
	}

	private void UpdateInputs()
	{
		if (!this.InputEnabled || !this.modelAvailable_)
		{
			return;
		}
		if (this._inputSources == null)
		{
			this._inputSources = new List<OVRVirtualKeyboard.IInputSource>();
			if (this.leftControllerRootTransform)
			{
				this._inputSources.Add(new OVRVirtualKeyboard.ControllerInputSource(this, OVRVirtualKeyboard.InputSource.ControllerLeft, OVRInput.Controller.LTouch, this.leftControllerRootTransform, this.leftControllerDirectTransform));
			}
			if (this.rightControllerRootTransform)
			{
				this._inputSources.Add(new OVRVirtualKeyboard.ControllerInputSource(this, OVRVirtualKeyboard.InputSource.ControllerRight, OVRInput.Controller.RTouch, this.rightControllerRootTransform, this.rightControllerDirectTransform));
			}
			if (this.handLeft)
			{
				this._inputSources.Add(new OVRVirtualKeyboard.HandInputSource(this, OVRVirtualKeyboard.InputSource.HandLeft, this.handLeft));
			}
			if (this.handRight)
			{
				this._inputSources.Add(new OVRVirtualKeyboard.HandInputSource(this, OVRVirtualKeyboard.InputSource.HandRight, this.handRight));
			}
		}
		foreach (OVRVirtualKeyboard.IInputSource inputSource in this._inputSources)
		{
			inputSource.Update();
		}
	}

	private ulong GetKeyboardSpace()
	{
		if (this.keyboardSpace_ != 0UL)
		{
			return this.keyboardSpace_;
		}
		OVRPlugin.VirtualKeyboardSpaceCreateInfo createInfo = default(OVRPlugin.VirtualKeyboardSpaceCreateInfo);
		OVRPlugin.VirtualKeyboardLocationInfo virtualKeyboardLocationInfo = this.ComputeLocation(base.transform);
		createInfo.locationType = virtualKeyboardLocationInfo.locationType;
		createInfo.trackingOriginType = OVRPlugin.GetTrackingOriginType();
		createInfo.pose = virtualKeyboardLocationInfo.pose;
		OVRPlugin.Result result = OVRPlugin.CreateVirtualKeyboardSpace(createInfo, out this.keyboardSpace_);
		if (result != OVRPlugin.Result.Success)
		{
			Debug.LogError("Create failed to create keyboard space: " + result.ToString());
			this.DestroyKeyboard();
		}
		this.UseSuggestedLocation(this.InitialPosition);
		return this.keyboardSpace_;
	}

	private void SyncKeyboardLocation()
	{
		if (this.keyboardSpace_ != 0UL && base.transform.hasChanged)
		{
			float d = this.MaxElement(base.transform.localScale);
			Vector3 localScale = Vector3.one * d;
			base.transform.localScale = localScale;
			this.UseSuggestedLocation(OVRVirtualKeyboard.KeyboardPosition.Custom);
		}
		OVRPlugin.Posef posef;
		if (!OVRPlugin.TryLocateSpace(this.GetKeyboardSpace(), OVRPlugin.GetTrackingOriginType(), out posef))
		{
			Debug.LogError("Failed to locate the virtual keyboard space.");
			return;
		}
		float d2;
		if (OVRPlugin.GetVirtualKeyboardScale(out d2) != OVRPlugin.Result.Success)
		{
			Debug.LogError("Failed to get virtual keyboard scale.");
			return;
		}
		Transform transform = base.transform;
		transform.SetPositionAndRotation(posef.Position.FromFlippedZVector3f(), posef.Orientation.FromFlippedZQuatf());
		transform.localScale = Vector3.one * d2;
		transform.hasChanged = false;
	}

	private void UpdateAnimationState()
	{
		if (!this.modelAvailable_)
		{
			return;
		}
		OVRPlugin.VirtualKeyboardTextureIds virtualKeyboardTextureIds;
		OVRPlugin.GetVirtualKeyboardDirtyTextures(out virtualKeyboardTextureIds);
		foreach (ulong num in virtualKeyboardTextureIds.TextureIds)
		{
			OVRVirtualKeyboard.VirtualKeyboardTextureInfo virtualKeyboardTextureInfo;
			if (this.virtualKeyboardTextures_.TryGetValue(num, out virtualKeyboardTextureInfo))
			{
				OVRPlugin.VirtualKeyboardTextureData virtualKeyboardTextureData = default(OVRPlugin.VirtualKeyboardTextureData);
				OVRPlugin.GetVirtualKeyboardTextureData(num, ref virtualKeyboardTextureData);
				if (virtualKeyboardTextureData.BufferCountOutput > 0U)
				{
					if (virtualKeyboardTextureInfo.bufferLength < virtualKeyboardTextureData.BufferCountOutput && virtualKeyboardTextureInfo.buffer != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(virtualKeyboardTextureInfo.buffer);
						virtualKeyboardTextureInfo.buffer = IntPtr.Zero;
					}
					if (virtualKeyboardTextureInfo.buffer == IntPtr.Zero)
					{
						virtualKeyboardTextureInfo.bufferLength = virtualKeyboardTextureData.BufferCountOutput;
						virtualKeyboardTextureInfo.buffer = Marshal.AllocHGlobal((int)virtualKeyboardTextureData.BufferCountOutput);
					}
					virtualKeyboardTextureData.Buffer = virtualKeyboardTextureInfo.buffer;
					virtualKeyboardTextureData.BufferCapacityInput = virtualKeyboardTextureInfo.bufferLength;
					OVRPlugin.GetVirtualKeyboardTextureData(num, ref virtualKeyboardTextureData);
					if (virtualKeyboardTextureInfo.hasTexture && ((long)virtualKeyboardTextureInfo.texture.width != (long)((ulong)virtualKeyboardTextureData.TextureWidth) || (long)virtualKeyboardTextureInfo.texture.height != (long)((ulong)virtualKeyboardTextureData.TextureHeight)))
					{
						virtualKeyboardTextureInfo.hasTexture = false;
					}
					if (!virtualKeyboardTextureInfo.hasTexture)
					{
						virtualKeyboardTextureInfo.texture = new Texture2D((int)virtualKeyboardTextureData.TextureWidth, (int)virtualKeyboardTextureData.TextureHeight, TextureFormat.RGBA32, false);
						virtualKeyboardTextureInfo.texture.filterMode = FilterMode.Trilinear;
						virtualKeyboardTextureInfo.hasTexture = true;
					}
					virtualKeyboardTextureInfo.texture.LoadRawTextureData(virtualKeyboardTextureInfo.buffer, (int)virtualKeyboardTextureInfo.bufferLength);
					virtualKeyboardTextureInfo.texture.Apply(true, false);
					this.virtualKeyboardTextures_[num] = virtualKeyboardTextureInfo;
					foreach (Material material in virtualKeyboardTextureInfo.materials)
					{
						material.mainTexture = virtualKeyboardTextureInfo.texture;
					}
				}
			}
		}
		this._animationStateCount = 0;
		OVRPlugin.GetVirtualKeyboardModelAnimationStates(new OVRPlugin.VirtualKeyboardModelAnimationStateBufferProvider(this.AnimationStatesBufferProvider), new OVRPlugin.VirtualKeyboardModelAnimationStateHandler(this.AnimationStateHandler));
		if (this._animationStateCount > 0)
		{
			foreach (OVRGLTFAnimationNodeMorphTargetHandler ovrgltfanimationNodeMorphTargetHandler in this.virtualKeyboardScene_.morphTargetHandlers)
			{
				ovrgltfanimationNodeMorphTargetHandler.Update();
			}
		}
	}

	private IntPtr AnimationStatesBufferProvider(int bufferLength, int count)
	{
		if (this._animationStateBufferLength < bufferLength)
		{
			Marshal.FreeHGlobal(this._animationStateBuffer);
			this._animationStateBufferLength = bufferLength;
			this._animationStateBuffer = Marshal.AllocHGlobal(this._animationStateBufferLength);
		}
		this._animationStateCount = count;
		return this._animationStateBuffer;
	}

	private void AnimationStateHandler(ref OVRPlugin.VirtualKeyboardModelAnimationState state)
	{
		if (state.AnimationIndex >= this.virtualKeyboardScene_.animationNodeLookup.Count)
		{
			Debug.LogWarning(string.Format("Unknown Animation State Index {0}", state.AnimationIndex));
			return;
		}
		OVRGLTFAnimatinonNode[] array = this.virtualKeyboardScene_.animationNodeLookup[state.AnimationIndex];
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdatePose(state.Fraction, false);
		}
	}

	private void OnCommitText(string text)
	{
		if (this.TextHandler == null)
		{
			return;
		}
		using (OVRVirtualKeyboard.TextHandlerScope textHandlerScope = new OVRVirtualKeyboard.TextHandlerScope(this.TextHandler, new Action<string>(this.OnTextHandlerChange)))
		{
			textHandlerScope.AppendText(text);
		}
	}

	private void OnTextHandlerChange(string textContext)
	{
		this.ChangeTextContextInternal(textContext);
	}

	private void ChangeTextContextInternal(string textContext)
	{
		if (!this.isKeyboardCreated_)
		{
			return;
		}
		if (OVRPlugin.ChangeVirtualKeyboardTextContext(textContext) != OVRPlugin.Result.Success)
		{
			Debug.LogError("Failed to set keyboard text context");
		}
	}

	private void OnBackspace()
	{
		if (this.TextHandler == null)
		{
			return;
		}
		using (OVRVirtualKeyboard.TextHandlerScope textHandlerScope = new OVRVirtualKeyboard.TextHandlerScope(this.TextHandler, new Action<string>(this.OnTextHandlerChange)))
		{
			textHandlerScope.ApplyBackspace();
		}
	}

	private void OnEnter()
	{
		if (this.TextHandler == null)
		{
			return;
		}
		using (OVRVirtualKeyboard.TextHandlerScope textHandlerScope = new OVRVirtualKeyboard.TextHandlerScope(this.TextHandler, new Action<string>(this.OnTextHandlerChange)))
		{
			if (textHandlerScope.SubmitOnEnter)
			{
				textHandlerScope.Submit();
			}
			else
			{
				this.OnCommitText("\n");
			}
		}
	}

	private void OnKeyboardShown()
	{
		if (!this.keyboardVisible_)
		{
			this.keyboardVisible_ = true;
			this.UpdateVisibleState();
		}
	}

	private void OnKeyboardHidden()
	{
		if (this.keyboardVisible_)
		{
			this.keyboardVisible_ = false;
			this.UpdateVisibleState();
		}
	}

	private void UpdateVisibleState()
	{
		base.gameObject.SetActive(this.keyboardVisible_);
		if (this.modelAvailable_)
		{
			this.virtualKeyboardScene_.root.gameObject.SetActive(this.keyboardVisible_);
		}
	}

	[ContextMenu("Autofill Input Roots")]
	public void AutoPopulate()
	{
		OVRCameraRig ovrcameraRig = Object.FindAnyObjectByType<OVRCameraRig>();
		if (ovrcameraRig == null)
		{
			Debug.LogWarning("Couldn't auto fill input transforms as we didn't have an OVRCameraRig.");
			return;
		}
		if (this.handRight == null || this.handLeft == null)
		{
			OVRHand[] componentsInChildren = ovrcameraRig.GetComponentsInChildren<OVRHand>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].HandType == OVRHand.Hand.HandLeft && this.handLeft == null)
				{
					this.handLeft = componentsInChildren[i];
				}
				if (componentsInChildren[i].HandType == OVRHand.Hand.HandRight && this.handRight == null)
				{
					this.handRight = componentsInChildren[i];
				}
				if (this.handRight && this.handLeft)
				{
					break;
				}
			}
		}
		if (this.leftControllerRootTransform == null || this.rightControllerRootTransform == null)
		{
			OVRControllerHelper[] componentsInChildren2 = ovrcameraRig.GetComponentsInChildren<OVRControllerHelper>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				if (this.leftControllerRootTransform == null && componentsInChildren2[j].m_controller == OVRInput.Controller.LTouch)
				{
					this.leftControllerRootTransform = componentsInChildren2[j].transform;
				}
				if (this.rightControllerRootTransform == null && componentsInChildren2[j].m_controller == OVRInput.Controller.RTouch)
				{
					this.rightControllerRootTransform = componentsInChildren2[j].transform;
				}
				if (this.leftControllerRootTransform && this.rightControllerRootTransform)
				{
					break;
				}
			}
		}
	}

	public OVRVirtualKeyboard()
	{
		this.CommitTextEvent = new OVRVirtualKeyboard.CommitTextUnityEvent();
		this.BackspaceEvent = new UnityEvent();
		this.EnterEvent = new UnityEvent();
		this.KeyboardShownEvent = new UnityEvent();
		this.KeyboardHiddenEvent = new UnityEvent();
		this.virtualKeyboardTextures_ = new Dictionary<ulong, OVRVirtualKeyboard.VirtualKeyboardTextureInfo>();
		this._interactorRootTransformOverride = new OVRVirtualKeyboard.InteractorRootTransformOverride();
		base..ctor();
	}

	private static readonly string _defaultShaderName = "Unlit/Color";

	private static readonly string _defaultAlphaBlendShaderName = "Unlit/Transparent";

	private static OVRVirtualKeyboard singleton_;

	[SerializeField]
	private OVRVirtualKeyboard.KeyboardPosition InitialPosition = OVRVirtualKeyboard.KeyboardPosition.Custom;

	[SerializeField]
	[FormerlySerializedAs("TextCommitField")]
	[Obsolete]
	[HideInInspector]
	private InputField textCommitField;

	[SerializeField]
	private OVRVirtualKeyboard.AbstractTextHandler textHandler;

	private OVRVirtualKeyboard.ITextHandler _runtimeTextHandler;

	[Header("Controller Input")]
	[FormerlySerializedAs("leftControllerInputTransform")]
	public Transform leftControllerRootTransform;

	public Transform leftControllerDirectTransform;

	[FormerlySerializedAs("rightControllerInputTransform")]
	public Transform rightControllerRootTransform;

	public Transform rightControllerDirectTransform;

	public bool controllerDirectInteraction = true;

	public bool controllerRayInteraction = true;

	public OVRPhysicsRaycaster controllerRaycaster;

	[Header("Hand Input")]
	public OVRHand handLeft;

	public OVRHand handRight;

	public bool handDirectInteraction = true;

	public bool handRayInteraction = true;

	public OVRPhysicsRaycaster handRaycaster;

	[Header("Graphics")]
	public Shader keyboardModelShader;

	public Shader keyboardModelAlphaBlendShader;

	[NonSerialized]
	public bool InputEnabled = true;

	private bool isKeyboardCreated_;

	private ulong keyboardSpace_;

	private Dictionary<ulong, OVRVirtualKeyboard.VirtualKeyboardTextureInfo> virtualKeyboardTextures_;

	private OVRGLTFScene virtualKeyboardScene_;

	private ulong virtualKeyboardModelKey_;

	private bool modelInitialized_;

	private bool modelAvailable_;

	private bool keyboardVisible_;

	private OVRVirtualKeyboard.InteractorRootTransformOverride _interactorRootTransformOverride;

	private List<OVRVirtualKeyboard.IInputSource> _inputSources;

	private OVRVirtualKeyboard.KeyboardEventListener keyboardEventListener_;

	private Coroutine gltfModelCoroutine_;

	private OVRGLTFLoader _gltfLoader;

	private int _animationStateCount;

	private int _animationStateBufferLength;

	private IntPtr _animationStateBuffer;

	public enum KeyboardPosition
	{
		Far,
		Near,
		[Obsolete]
		Direct = 1,
		Custom
	}

	public interface ITextHandler
	{
		Action<string> OnTextChanged { get; set; }

		string Text { get; }

		bool SubmitOnEnter { get; }

		bool IsFocused { get; }

		void Submit();

		void AppendText(string s);

		void ApplyBackspace();

		void MoveTextEnd();
	}

	public abstract class AbstractTextHandler : MonoBehaviour, OVRVirtualKeyboard.ITextHandler
	{
		public abstract Action<string> OnTextChanged { get; set; }

		public abstract string Text { get; }

		public abstract bool SubmitOnEnter { get; }

		public abstract bool IsFocused { get; }

		public abstract void Submit();

		public abstract void AppendText(string s);

		public abstract void ApplyBackspace();

		public abstract void MoveTextEnd();
	}

	private class TextHandlerScope : OVRVirtualKeyboard.ITextHandler, IDisposable
	{
		public TextHandlerScope(OVRVirtualKeyboard.ITextHandler textHandler, Action<string> textChangeHandler)
		{
			this._textHandler = textHandler;
			this._textChangeHandler = textChangeHandler;
			OVRVirtualKeyboard.ITextHandler textHandler2 = this._textHandler;
			textHandler2.OnTextChanged = (Action<string>)Delegate.Remove(textHandler2.OnTextChanged, this._textChangeHandler);
		}

		public void Dispose()
		{
			OVRVirtualKeyboard.ITextHandler textHandler = this._textHandler;
			textHandler.OnTextChanged = (Action<string>)Delegate.Combine(textHandler.OnTextChanged, this._textChangeHandler);
		}

		public Action<string> OnTextChanged
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string Text
		{
			get
			{
				return this._textHandler.Text;
			}
		}

		public bool SubmitOnEnter
		{
			get
			{
				return this._textHandler.SubmitOnEnter;
			}
		}

		public bool IsFocused
		{
			get
			{
				return this._textHandler.IsFocused;
			}
		}

		public void Submit()
		{
			OVRVirtualKeyboard.ITextHandler textHandler = this._textHandler;
			if (textHandler == null)
			{
				return;
			}
			textHandler.Submit();
		}

		public void AppendText(string s)
		{
			if (this._textHandler == null)
			{
				return;
			}
			this._textHandler.AppendText(s);
			if (this._textHandler.IsFocused)
			{
				this._textHandler.MoveTextEnd();
			}
		}

		public void ApplyBackspace()
		{
			if (this._textHandler == null)
			{
				return;
			}
			this._textHandler.ApplyBackspace();
			if (this._textHandler.IsFocused)
			{
				this._textHandler.MoveTextEnd();
			}
		}

		public void MoveTextEnd()
		{
			OVRVirtualKeyboard.ITextHandler textHandler = this._textHandler;
			if (textHandler == null)
			{
				return;
			}
			textHandler.MoveTextEnd();
		}

		private readonly OVRVirtualKeyboard.ITextHandler _textHandler;

		private readonly Action<string> _textChangeHandler;
	}

	public class WaitUntilKeyboardVisible : CustomYieldInstruction
	{
		public override bool keepWaiting
		{
			get
			{
				return !this._keyboard.modelAvailable_ || !this._keyboard.keyboardVisible_;
			}
		}

		public WaitUntilKeyboardVisible(OVRVirtualKeyboard keyboard)
		{
			this._keyboard = keyboard;
		}

		private readonly OVRVirtualKeyboard _keyboard;
	}

	public class InteractorRootTransformOverride
	{
		public void Enqueue(Transform interactorRootTransform, OVRPlugin.Posef interactorRootPose)
		{
			if (interactorRootTransform == null)
			{
				throw new Exception("Transform is undefined");
			}
			this.applyQueue.Enqueue(new OVRVirtualKeyboard.InteractorRootTransformOverride.InteractorRootOverrideData
			{
				root = interactorRootTransform,
				originalPose = interactorRootTransform.ToOVRPose(false),
				targetPose = interactorRootPose.ToOVRPose()
			});
		}

		public void LateApply(MonoBehaviour coroutineRunner)
		{
			while (this.applyQueue.Count > 0)
			{
				OVRVirtualKeyboard.InteractorRootTransformOverride.InteractorRootOverrideData interactorRootOverrideData = this.applyQueue.Dequeue();
				OVRPose targetPose = interactorRootOverrideData.root.ToOVRPose(false);
				if (OVRVirtualKeyboard.InteractorRootTransformOverride.ApplyOverride(interactorRootOverrideData))
				{
					interactorRootOverrideData.originalPose = interactorRootOverrideData.root.ToOVRPose(false);
					interactorRootOverrideData.targetPose = targetPose;
					this.revertQueue.Enqueue(interactorRootOverrideData);
				}
			}
			if (this.revertQueue.Count > 0 && coroutineRunner != null)
			{
				coroutineRunner.StartCoroutine(this.RevertInteractorOverrides());
			}
		}

		public void Reset()
		{
			while (this.revertQueue.Count > 0)
			{
				OVRVirtualKeyboard.InteractorRootTransformOverride.ApplyOverride(this.revertQueue.Dequeue());
			}
		}

		private IEnumerator RevertInteractorOverrides()
		{
			yield return new WaitForEndOfFrame();
			this.Reset();
			yield break;
		}

		private static bool ApplyOverride(OVRVirtualKeyboard.InteractorRootTransformOverride.InteractorRootOverrideData interactorOverride)
		{
			if (interactorOverride.root.position != interactorOverride.originalPose.position || interactorOverride.root.rotation != interactorOverride.originalPose.orientation)
			{
				return false;
			}
			interactorOverride.root.position = interactorOverride.targetPose.position;
			interactorOverride.root.rotation = interactorOverride.targetPose.orientation;
			return true;
		}

		private Queue<OVRVirtualKeyboard.InteractorRootTransformOverride.InteractorRootOverrideData> applyQueue = new Queue<OVRVirtualKeyboard.InteractorRootTransformOverride.InteractorRootOverrideData>();

		private Queue<OVRVirtualKeyboard.InteractorRootTransformOverride.InteractorRootOverrideData> revertQueue = new Queue<OVRVirtualKeyboard.InteractorRootTransformOverride.InteractorRootOverrideData>();

		private struct InteractorRootOverrideData
		{
			public Transform root;

			public OVRPose originalPose;

			public OVRPose targetPose;
		}
	}

	public enum InputSource
	{
		ControllerLeft,
		ControllerRight,
		HandLeft,
		HandRight
	}

	private interface IInputSource : IDisposable
	{
		void Update();
	}

	private abstract class BaseInputSource : OVRVirtualKeyboard.IInputSource, IDisposable
	{
		protected BaseInputSource()
		{
			this._rig = Object.FindAnyObjectByType<OVRCameraRig>();
			if (this._rig == null)
			{
				return;
			}
			this._rig.UpdatedAnchors += this.OnUpdatedAnchors;
			this._operatingWithoutOVRCameraRig = false;
		}

		private void OnUpdatedAnchors(OVRCameraRig obj)
		{
			if (this._disposed)
			{
				throw new Exception("Virtual Keyboard Input Source Disposed");
			}
			this.UpdateInput();
		}

		public void Update()
		{
			if (this._operatingWithoutOVRCameraRig && !this._disposed)
			{
				this.UpdateInput();
			}
		}

		protected abstract void UpdateInput();

		public void Dispose()
		{
			this._disposed = true;
			if (this._rig != null)
			{
				this._rig.UpdatedAnchors -= this.OnUpdatedAnchors;
			}
		}

		protected readonly bool _operatingWithoutOVRCameraRig;

		private readonly OVRCameraRig _rig;

		private bool _disposed;
	}

	private class ControllerInputSource : OVRVirtualKeyboard.BaseInputSource
	{
		private bool TriggerIsPressed
		{
			get
			{
				return OVRInput.Get((this._controllerType == OVRInput.Controller.LTouch) ? (OVRInput.RawButton.X | OVRInput.RawButton.LIndexTrigger) : (OVRInput.RawButton.A | OVRInput.RawButton.RIndexTrigger), OVRInput.Controller.Active);
			}
		}

		public ControllerInputSource(OVRVirtualKeyboard keyboard, OVRVirtualKeyboard.InputSource inputSource, OVRInput.Controller controllerType, Transform rootTransform, Transform directTransform)
		{
			this._keyboard = keyboard;
			this._inputSource = inputSource;
			this._controllerType = controllerType;
			this._rootTransform = rootTransform;
			this._directTransform = directTransform;
		}

		protected override void UpdateInput()
		{
			if (!this._keyboard.InputEnabled || !OVRInput.GetControllerPositionValid(this._controllerType) || !this._rootTransform)
			{
				return;
			}
			if (Time.frameCount == this._lastFrameCount)
			{
				return;
			}
			this._lastFrameCount = Time.frameCount;
			if (this._keyboard.controllerRayInteraction)
			{
				this._keyboard.SendVirtualKeyboardRayInput(this._directTransform, this._inputSource, this.TriggerIsPressed, true);
			}
			if (this._keyboard.controllerDirectInteraction)
			{
				this._keyboard.SendVirtualKeyboardDirectInput(this._directTransform.position, this._inputSource, this.TriggerIsPressed, this._rootTransform);
			}
		}

		private readonly Transform _rootTransform;

		private readonly Transform _directTransform;

		private readonly OVRVirtualKeyboard.InputSource _inputSource;

		private readonly OVRInput.Controller _controllerType;

		private readonly OVRVirtualKeyboard _keyboard;

		private int _lastFrameCount;
	}

	private class HandInputSource : OVRVirtualKeyboard.BaseInputSource
	{
		public HandInputSource(OVRVirtualKeyboard keyboard, OVRVirtualKeyboard.InputSource inputSource, OVRHand hand)
		{
			if (!keyboard)
			{
				throw new ArgumentNullException("keyboard");
			}
			this._keyboard = keyboard;
			if (!hand)
			{
				throw new ArgumentNullException("hand");
			}
			this._hand = hand;
			this._skeleton = this._hand.GetComponent<OVRSkeleton>();
			if (!this._skeleton && this._keyboard.handDirectInteraction)
			{
				Debug.LogWarning("Hand Direct Interaction requires an OVRSkeleton on the OVRHand");
			}
			this._inputSource = inputSource;
		}

		protected override void UpdateInput()
		{
			if (!this._keyboard.InputEnabled || !this._hand)
			{
				return;
			}
			if (Time.frameCount == this._lastFrameCount)
			{
				return;
			}
			this._lastFrameCount = Time.frameCount;
			if (this._keyboard.handRayInteraction && this._hand.IsPointerPoseValid)
			{
				this._keyboard.SendVirtualKeyboardRayInput(this._hand.PointerPose, this._inputSource, this._hand.GetFingerIsPinching(OVRHand.HandFinger.Index), true);
			}
			if (this._keyboard.handDirectInteraction && this._skeleton && this._skeleton.IsDataValid)
			{
				OVRSkeleton.BoneId indexTipJoint = (this._skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.XRHandLeft || this._skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.XRHandRight) ? OVRSkeleton.BoneId.Hand_Middle2 : OVRSkeleton.BoneId.Hand_IndexTip;
				OVRSkeleton.BoneId wristRootJoint = (this._skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.XRHandLeft || this._skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.XRHandRight) ? OVRSkeleton.BoneId.Hand_ForearmStub : OVRSkeleton.BoneId.Hand_Start;
				OVRBone ovrbone = this._skeleton.Bones.First((OVRBone b) => b.Id == indexTipJoint);
				OVRBone ovrbone2 = this._skeleton.Bones.First((OVRBone b) => b.Id == wristRootJoint);
				this._keyboard.SendVirtualKeyboardDirectInput(ovrbone.Transform.position, this._inputSource, this._hand.GetFingerIsPinching(OVRHand.HandFinger.Index), ovrbone2.Transform);
			}
		}

		private readonly OVRHand _hand;

		private readonly OVRVirtualKeyboard.InputSource _inputSource;

		private readonly OVRVirtualKeyboard _keyboard;

		private readonly OVRSkeleton _skeleton;

		private int _lastFrameCount;
	}

	private class KeyboardEventListener : OVRManager.EventListener
	{
		public KeyboardEventListener(OVRVirtualKeyboard keyboard)
		{
			this.keyboard_ = keyboard;
		}

		public void OnEvent(OVRPlugin.EventDataBuffer eventDataBuffer)
		{
			switch (eventDataBuffer.EventType)
			{
			case OVRPlugin.EventType.VirtualKeyboardCommitText:
				if (this.keyboard_.CommitTextEvent != null || this.keyboard_.CommitText != null)
				{
					string text = Encoding.UTF8.GetString(eventDataBuffer.EventData).Replace("\0", "");
					OVRVirtualKeyboard.CommitTextUnityEvent commitTextEvent = this.keyboard_.CommitTextEvent;
					if (commitTextEvent != null)
					{
						commitTextEvent.Invoke(text);
					}
					Action<string> commitText = this.keyboard_.CommitText;
					if (commitText == null)
					{
						return;
					}
					commitText(text);
					return;
				}
				break;
			case OVRPlugin.EventType.VirtualKeyboardBackspace:
			{
				UnityEvent backspaceEvent = this.keyboard_.BackspaceEvent;
				if (backspaceEvent != null)
				{
					backspaceEvent.Invoke();
				}
				Action backspace = this.keyboard_.Backspace;
				if (backspace == null)
				{
					return;
				}
				backspace();
				return;
			}
			case OVRPlugin.EventType.VirtualKeyboardEnter:
			{
				UnityEvent enterEvent = this.keyboard_.EnterEvent;
				if (enterEvent != null)
				{
					enterEvent.Invoke();
				}
				Action enter = this.keyboard_.Enter;
				if (enter == null)
				{
					return;
				}
				enter();
				return;
			}
			case OVRPlugin.EventType.VirtualKeyboardShown:
			{
				UnityEvent keyboardShownEvent = this.keyboard_.KeyboardShownEvent;
				if (keyboardShownEvent != null)
				{
					keyboardShownEvent.Invoke();
				}
				Action keyboardShown = this.keyboard_.KeyboardShown;
				if (keyboardShown == null)
				{
					return;
				}
				keyboardShown();
				return;
			}
			case OVRPlugin.EventType.VirtualKeyboardHidden:
			{
				UnityEvent keyboardHiddenEvent = this.keyboard_.KeyboardHiddenEvent;
				if (keyboardHiddenEvent != null)
				{
					keyboardHiddenEvent.Invoke();
				}
				Action keyboardHidden = this.keyboard_.KeyboardHidden;
				if (keyboardHidden == null)
				{
					return;
				}
				keyboardHidden();
				break;
			}
			default:
				return;
			}
		}

		private readonly OVRVirtualKeyboard keyboard_;
	}

	private struct VirtualKeyboardTextureInfo
	{
		public IntPtr buffer;

		public uint bufferLength;

		public Texture2D texture;

		public bool hasTexture;

		public List<Material> materials;
	}

	[Serializable]
	public class CommitTextUnityEvent : UnityEvent<string>
	{
	}
}
