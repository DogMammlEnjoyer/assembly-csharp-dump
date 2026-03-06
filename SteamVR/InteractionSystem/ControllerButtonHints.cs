using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Valve.VR.InteractionSystem
{
	public class ControllerButtonHints : MonoBehaviour
	{
		public Material usingMaterial
		{
			get
			{
				return this.urpControllerMaterial;
			}
		}

		public bool initialized { get; private set; }

		private void Awake()
		{
			this.renderModelLoadedAction = SteamVR_Events.RenderModelLoadedAction(new UnityAction<SteamVR_RenderModel, bool>(this.OnRenderModelLoaded));
			this.colorID = Shader.PropertyToID("_BaseColor");
		}

		private void Start()
		{
			this.player = Player.instance;
		}

		private void HintDebugLog(string msg)
		{
			if (this.debugHints)
			{
				Debug.Log("<b>[SteamVR Interaction]</b> Hints: " + msg);
			}
		}

		private void OnEnable()
		{
			this.renderModelLoadedAction.enabled = true;
		}

		private void OnDisable()
		{
			this.renderModelLoadedAction.enabled = false;
			this.Clear();
		}

		private void OnParentHandInputFocusLost()
		{
			this.HideAllButtonHints();
			this.HideAllText();
		}

		public virtual void SetInputSource(SteamVR_Input_Sources newInputSource)
		{
			this.inputSource = newInputSource;
			if (this.renderModel != null)
			{
				this.renderModel.SetInputSource(newInputSource);
			}
		}

		private void OnHandInitialized(int deviceIndex)
		{
			this.renderModel = new GameObject("SteamVR_RenderModel").AddComponent<SteamVR_RenderModel>();
			this.renderModel.transform.parent = base.transform;
			this.renderModel.transform.localPosition = Vector3.zero;
			this.renderModel.transform.localRotation = Quaternion.identity;
			this.renderModel.transform.localScale = Vector3.one;
			this.renderModel.SetInputSource(this.inputSource);
			this.renderModel.SetDeviceIndex(deviceIndex);
			if (!this.initialized)
			{
				this.renderModel.gameObject.SetActive(true);
			}
		}

		private void OnRenderModelLoaded(SteamVR_RenderModel renderModel, bool succeess)
		{
			if (renderModel == this.renderModel)
			{
				if (this.initialized)
				{
					Object.Destroy(this.textHintParent.gameObject);
					this.componentTransformMap.Clear();
					this.flashingRenderers.Clear();
				}
				renderModel.SetMeshRendererState(false);
				base.StartCoroutine(this.DoInitialize(renderModel));
			}
		}

		private IEnumerator DoInitialize(SteamVR_RenderModel renderModel)
		{
			while (!renderModel.initializedAttachPoints)
			{
				yield return null;
			}
			this.textHintParent = new GameObject("Text Hints").transform;
			this.textHintParent.SetParent(base.transform);
			this.textHintParent.localPosition = Vector3.zero;
			this.textHintParent.localRotation = Quaternion.identity;
			this.textHintParent.localScale = Vector3.one;
			if (OpenVR.RenderModels != null)
			{
				string text = "";
				if (this.debugHints)
				{
					text = "Components for render model " + renderModel.index.ToString();
				}
				for (int i = 0; i < renderModel.transform.childCount; i++)
				{
					Transform child = renderModel.transform.GetChild(i);
					if (this.componentTransformMap.ContainsKey(child.name))
					{
						if (this.debugHints)
						{
							text = text + "\n\t!    Child component already exists with name: " + child.name;
						}
					}
					else
					{
						this.componentTransformMap.Add(child.name, child);
					}
					if (this.debugHints)
					{
						text = text + "\n\t" + child.name + ".";
					}
				}
				this.HintDebugLog(text);
			}
			this.actionHintInfos = new Dictionary<ISteamVR_Action_In_Source, ControllerButtonHints.ActionHintInfo>();
			for (int j = 0; j < SteamVR_Input.actionsNonPoseNonSkeletonIn.Length; j++)
			{
				ISteamVR_Action_In steamVR_Action_In = SteamVR_Input.actionsNonPoseNonSkeletonIn[j];
				if (steamVR_Action_In.GetActive(this.inputSource))
				{
					this.CreateAndAddButtonInfo(steamVR_Action_In, this.inputSource);
				}
			}
			this.ComputeTextEndTransforms();
			this.initialized = true;
			renderModel.SetMeshRendererState(true);
			renderModel.gameObject.SetActive(false);
			yield break;
		}

		private void CreateAndAddButtonInfo(ISteamVR_Action_In action, SteamVR_Input_Sources inputSource)
		{
			Transform transform = null;
			List<MeshRenderer> list = new List<MeshRenderer>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Looking for action: ");
			stringBuilder.AppendLine(action.GetShortName());
			stringBuilder.Append("Action localized origin: ");
			stringBuilder.AppendLine(action.GetLocalizedOrigin(inputSource));
			string renderModelComponentName = action.GetRenderModelComponentName(inputSource);
			if (this.componentTransformMap.ContainsKey(renderModelComponentName))
			{
				stringBuilder.AppendLine(string.Format("Found component: {0} for {1}", renderModelComponentName, action.GetShortName()));
				Transform transform2 = this.componentTransformMap[renderModelComponentName];
				transform = transform2;
				stringBuilder.AppendLine(string.Format("Found componentTransform: {0}. buttonTransform: {1}", transform2, transform));
				list.AddRange(transform2.GetComponentsInChildren<MeshRenderer>());
			}
			else
			{
				stringBuilder.AppendLine(string.Format("Can't find component transform for action: {0}. Component name: \"{1}\"", action.GetShortName(), renderModelComponentName));
			}
			stringBuilder.AppendLine(string.Format("Found {0} renderers for {1}", list.Count, action.GetShortName()));
			foreach (MeshRenderer meshRenderer in list)
			{
				stringBuilder.Append("\t");
				stringBuilder.AppendLine(meshRenderer.name);
			}
			this.HintDebugLog(stringBuilder.ToString());
			if (transform == null)
			{
				this.HintDebugLog("Couldn't find buttonTransform for " + action.GetShortName());
				return;
			}
			ControllerButtonHints.ActionHintInfo actionHintInfo = new ControllerButtonHints.ActionHintInfo();
			this.actionHintInfos.Add(action, actionHintInfo);
			actionHintInfo.componentName = transform.name;
			actionHintInfo.renderers = list;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.name == "attach")
				{
					actionHintInfo.localTransform = child;
				}
			}
			switch (1)
			{
			case 0:
				actionHintInfo.textEndOffsetDir = actionHintInfo.localTransform.up;
				break;
			case 1:
				actionHintInfo.textEndOffsetDir = actionHintInfo.localTransform.right;
				break;
			case 2:
				actionHintInfo.textEndOffsetDir = actionHintInfo.localTransform.forward;
				break;
			case 3:
				actionHintInfo.textEndOffsetDir = -actionHintInfo.localTransform.forward;
				break;
			}
			Vector3 position = actionHintInfo.localTransform.position + actionHintInfo.localTransform.forward * 0.01f;
			actionHintInfo.textHintObject = Object.Instantiate<GameObject>(this.textHintPrefab, position, Quaternion.identity);
			actionHintInfo.textHintObject.name = "Hint_" + actionHintInfo.componentName + "_Start";
			actionHintInfo.textHintObject.transform.SetParent(this.textHintParent);
			actionHintInfo.textHintObject.layer = base.gameObject.layer;
			actionHintInfo.textHintObject.tag = base.gameObject.tag;
			actionHintInfo.textStartAnchor = actionHintInfo.textHintObject.transform.Find("Start");
			actionHintInfo.textEndAnchor = actionHintInfo.textHintObject.transform.Find("End");
			actionHintInfo.canvasOffset = actionHintInfo.textHintObject.transform.Find("CanvasOffset");
			actionHintInfo.line = actionHintInfo.textHintObject.transform.Find("Line").GetComponent<LineRenderer>();
			actionHintInfo.textCanvas = actionHintInfo.textHintObject.GetComponentInChildren<Canvas>();
			actionHintInfo.text = actionHintInfo.textCanvas.GetComponentInChildren<Text>();
			actionHintInfo.textMesh = actionHintInfo.textCanvas.GetComponentInChildren<TextMesh>();
			actionHintInfo.textHintObject.SetActive(false);
			actionHintInfo.textStartAnchor.position = position;
			if (actionHintInfo.text != null)
			{
				actionHintInfo.text.text = actionHintInfo.componentName;
			}
			if (actionHintInfo.textMesh != null)
			{
				actionHintInfo.textMesh.text = actionHintInfo.componentName;
			}
			this.centerPosition += actionHintInfo.textStartAnchor.position;
			actionHintInfo.textCanvas.transform.localScale = Vector3.Scale(actionHintInfo.textCanvas.transform.localScale, this.player.transform.localScale);
			actionHintInfo.textStartAnchor.transform.localScale = Vector3.Scale(actionHintInfo.textStartAnchor.transform.localScale, this.player.transform.localScale);
			actionHintInfo.textEndAnchor.transform.localScale = Vector3.Scale(actionHintInfo.textEndAnchor.transform.localScale, this.player.transform.localScale);
			actionHintInfo.line.transform.localScale = Vector3.Scale(actionHintInfo.line.transform.localScale, this.player.transform.localScale);
		}

		private void ComputeTextEndTransforms()
		{
			this.centerPosition /= (float)this.actionHintInfos.Count;
			float num = 0f;
			foreach (KeyValuePair<ISteamVR_Action_In_Source, ControllerButtonHints.ActionHintInfo> keyValuePair in this.actionHintInfos)
			{
				keyValuePair.Value.distanceFromCenter = Vector3.Distance(keyValuePair.Value.textStartAnchor.position, this.centerPosition);
				if (keyValuePair.Value.distanceFromCenter > num)
				{
					num = keyValuePair.Value.distanceFromCenter;
				}
			}
			foreach (KeyValuePair<ISteamVR_Action_In_Source, ControllerButtonHints.ActionHintInfo> keyValuePair2 in this.actionHintInfos)
			{
				Vector3 vector = keyValuePair2.Value.textStartAnchor.position - this.centerPosition;
				vector.Normalize();
				vector = Vector3.Project(vector, this.renderModel.transform.forward);
				float num2 = keyValuePair2.Value.distanceFromCenter / num;
				float d = keyValuePair2.Value.distanceFromCenter * Mathf.Pow(2f, 10f * (num2 - 1f)) * 20f;
				float d2 = 0.1f;
				Vector3 vector2 = keyValuePair2.Value.textStartAnchor.position + keyValuePair2.Value.textEndOffsetDir * d2 + vector * d * 0.1f;
				if (SteamVR_Utils.IsValid(vector2))
				{
					keyValuePair2.Value.textEndAnchor.position = vector2;
					keyValuePair2.Value.canvasOffset.position = vector2;
				}
				else
				{
					Debug.LogWarning("<b>[SteamVR Interaction]</b> Invalid end position for: " + keyValuePair2.Value.textStartAnchor.name, keyValuePair2.Value.textStartAnchor.gameObject);
				}
				keyValuePair2.Value.canvasOffset.localRotation = Quaternion.identity;
			}
		}

		private void ShowButtonHint(params ISteamVR_Action_In_Source[] actions)
		{
			this.renderModel.gameObject.SetActive(true);
			this.renderModel.GetComponentsInChildren<MeshRenderer>(this.renderers);
			for (int i = 0; i < this.renderers.Count; i++)
			{
				Texture mainTexture = this.renderers[i].material.mainTexture;
				this.renderers[i].sharedMaterial = this.usingMaterial;
				this.renderers[i].material.mainTexture = mainTexture;
				this.renderers[i].material.renderQueue = this.usingMaterial.renderQueue;
			}
			for (int j = 0; j < actions.Length; j++)
			{
				if (this.actionHintInfos.ContainsKey(actions[j]))
				{
					foreach (MeshRenderer item in this.actionHintInfos[actions[j]].renderers)
					{
						if (!this.flashingRenderers.Contains(item))
						{
							this.flashingRenderers.Add(item);
						}
					}
				}
			}
			this.startTime = Time.realtimeSinceStartup;
			this.tickCount = 0f;
		}

		private void HideAllButtonHints()
		{
			this.Clear();
			if (this.renderModel != null && this.renderModel.gameObject != null)
			{
				this.renderModel.gameObject.SetActive(false);
			}
		}

		private void HideButtonHint(params ISteamVR_Action_In_Source[] actions)
		{
			Color color = this.usingMaterial.GetColor(this.colorID);
			for (int i = 0; i < actions.Length; i++)
			{
				if (this.actionHintInfos.ContainsKey(actions[i]))
				{
					foreach (MeshRenderer meshRenderer in this.actionHintInfos[actions[i]].renderers)
					{
						meshRenderer.material.color = color;
						this.flashingRenderers.Remove(meshRenderer);
					}
				}
			}
			if (this.flashingRenderers.Count == 0)
			{
				this.renderModel.gameObject.SetActive(false);
			}
		}

		private bool IsButtonHintActive(ISteamVR_Action_In_Source action)
		{
			if (this.actionHintInfos.ContainsKey(action))
			{
				foreach (MeshRenderer item in this.actionHintInfos[action].renderers)
				{
					if (this.flashingRenderers.Contains(item))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private IEnumerator TestButtonHints()
		{
			for (;;)
			{
				int num;
				for (int actionIndex = 0; actionIndex < SteamVR_Input.actionsNonPoseNonSkeletonIn.Length; actionIndex = num + 1)
				{
					ISteamVR_Action_In steamVR_Action_In = SteamVR_Input.actionsNonPoseNonSkeletonIn[actionIndex];
					if (steamVR_Action_In.GetActive(this.inputSource))
					{
						this.ShowButtonHint(new ISteamVR_Action_In_Source[]
						{
							steamVR_Action_In
						});
						yield return new WaitForSeconds(1f);
					}
					yield return null;
					num = actionIndex;
				}
			}
			yield break;
		}

		private IEnumerator TestTextHints()
		{
			for (;;)
			{
				int num;
				for (int actionIndex = 0; actionIndex < SteamVR_Input.actionsNonPoseNonSkeletonIn.Length; actionIndex = num + 1)
				{
					ISteamVR_Action_In steamVR_Action_In = SteamVR_Input.actionsNonPoseNonSkeletonIn[actionIndex];
					if (steamVR_Action_In.GetActive(this.inputSource))
					{
						this.ShowText(steamVR_Action_In, steamVR_Action_In.GetShortName(), true);
						yield return new WaitForSeconds(3f);
					}
					yield return null;
					num = actionIndex;
				}
				this.HideAllText();
				yield return new WaitForSeconds(3f);
			}
			yield break;
		}

		private void Update()
		{
			if (this.renderModel != null && this.renderModel.gameObject.activeInHierarchy && this.flashingRenderers.Count > 0)
			{
				Color color = this.usingMaterial.GetColor(this.colorID);
				float num = (Time.realtimeSinceStartup - this.startTime) * 3.1415927f * 2f;
				num = Mathf.Cos(num);
				num = Util.RemapNumberClamped(num, -1f, 1f, 0f, 1f);
				if (Time.realtimeSinceStartup - this.startTime - this.tickCount > 1f)
				{
					this.tickCount += 1f;
					this.hapticFlash.Execute(0f, 0.005f, 0.005f, 1f, this.inputSource);
				}
				for (int i = 0; i < this.flashingRenderers.Count; i++)
				{
					this.flashingRenderers[i].material.SetColor(this.colorID, Color.Lerp(color, this.flashColor, num));
				}
				if (this.initialized)
				{
					foreach (KeyValuePair<ISteamVR_Action_In_Source, ControllerButtonHints.ActionHintInfo> keyValuePair in this.actionHintInfos)
					{
						if (keyValuePair.Value.textHintActive)
						{
							this.UpdateTextHint(keyValuePair.Value);
						}
					}
				}
			}
		}

		private void UpdateTextHint(ControllerButtonHints.ActionHintInfo hintInfo)
		{
			Transform hmdTransform = this.player.hmdTransform;
			Vector3 forward = hmdTransform.position - hintInfo.canvasOffset.position;
			Quaternion a = Quaternion.LookRotation(forward, Vector3.up);
			Quaternion b = Quaternion.LookRotation(forward, hmdTransform.up);
			float t;
			if (hmdTransform.forward.y > 0f)
			{
				t = Util.RemapNumberClamped(hmdTransform.forward.y, 0.6f, 0.4f, 1f, 0f);
			}
			else
			{
				t = Util.RemapNumberClamped(hmdTransform.forward.y, -0.8f, -0.6f, 1f, 0f);
			}
			hintInfo.canvasOffset.rotation = Quaternion.Slerp(a, b, t);
			Transform transform = hintInfo.line.transform;
			hintInfo.line.useWorldSpace = false;
			hintInfo.line.SetPosition(0, transform.InverseTransformPoint(hintInfo.textStartAnchor.position));
			hintInfo.line.SetPosition(1, transform.InverseTransformPoint(hintInfo.textEndAnchor.position));
		}

		private void Clear()
		{
			this.renderers.Clear();
			this.flashingRenderers.Clear();
		}

		private void ShowText(ISteamVR_Action_In_Source action, string text, bool highlightButton = true)
		{
			if (this.actionHintInfos.ContainsKey(action))
			{
				ControllerButtonHints.ActionHintInfo actionHintInfo = this.actionHintInfos[action];
				actionHintInfo.textHintObject.SetActive(true);
				actionHintInfo.textHintActive = true;
				if (actionHintInfo.text != null)
				{
					actionHintInfo.text.text = text;
				}
				if (actionHintInfo.textMesh != null)
				{
					actionHintInfo.textMesh.text = text;
				}
				this.UpdateTextHint(actionHintInfo);
				if (highlightButton)
				{
					this.ShowButtonHint(new ISteamVR_Action_In_Source[]
					{
						action
					});
				}
				this.renderModel.gameObject.SetActive(true);
			}
		}

		private void HideText(ISteamVR_Action_In_Source action)
		{
			if (this.actionHintInfos.ContainsKey(action))
			{
				ControllerButtonHints.ActionHintInfo actionHintInfo = this.actionHintInfos[action];
				actionHintInfo.textHintObject.SetActive(false);
				actionHintInfo.textHintActive = false;
				this.HideButtonHint(new ISteamVR_Action_In_Source[]
				{
					action
				});
			}
		}

		private void HideAllText()
		{
			if (this.actionHintInfos != null)
			{
				foreach (KeyValuePair<ISteamVR_Action_In_Source, ControllerButtonHints.ActionHintInfo> keyValuePair in this.actionHintInfos)
				{
					keyValuePair.Value.textHintObject.SetActive(false);
					keyValuePair.Value.textHintActive = false;
				}
				this.HideAllButtonHints();
			}
		}

		private string GetActiveHintText(ISteamVR_Action_In_Source action)
		{
			if (this.actionHintInfos.ContainsKey(action))
			{
				ControllerButtonHints.ActionHintInfo actionHintInfo = this.actionHintInfos[action];
				if (actionHintInfo.textHintActive)
				{
					return actionHintInfo.text.text;
				}
			}
			return string.Empty;
		}

		private static ControllerButtonHints GetControllerButtonHints(Hand hand)
		{
			if (hand != null)
			{
				ControllerButtonHints componentInChildren = hand.GetComponentInChildren<ControllerButtonHints>();
				if (componentInChildren != null && componentInChildren.initialized)
				{
					return componentInChildren;
				}
			}
			return null;
		}

		public static void ShowButtonHint(Hand hand, params ISteamVR_Action_In_Source[] actions)
		{
			ControllerButtonHints controllerButtonHints = ControllerButtonHints.GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.ShowButtonHint(actions);
			}
		}

		public static void HideButtonHint(Hand hand, params ISteamVR_Action_In_Source[] actions)
		{
			ControllerButtonHints controllerButtonHints = ControllerButtonHints.GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.HideButtonHint(actions);
			}
		}

		public static void HideAllButtonHints(Hand hand)
		{
			ControllerButtonHints controllerButtonHints = ControllerButtonHints.GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.HideAllButtonHints();
			}
		}

		public static bool IsButtonHintActive(Hand hand, ISteamVR_Action_In_Source action)
		{
			ControllerButtonHints controllerButtonHints = ControllerButtonHints.GetControllerButtonHints(hand);
			return controllerButtonHints != null && controllerButtonHints.IsButtonHintActive(action);
		}

		public static void ShowTextHint(Hand hand, ISteamVR_Action_In_Source action, string text, bool highlightButton = true)
		{
			ControllerButtonHints controllerButtonHints = ControllerButtonHints.GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.ShowText(action, text, highlightButton);
				if (hand != null && controllerButtonHints.autoSetWithControllerRangeOfMotion)
				{
					hand.SetTemporarySkeletonRangeOfMotion(SkeletalMotionRangeChange.WithController, 0.1f);
				}
			}
		}

		public static void HideTextHint(Hand hand, ISteamVR_Action_In_Source action)
		{
			ControllerButtonHints controllerButtonHints = ControllerButtonHints.GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.HideText(action);
				if (hand != null && controllerButtonHints.autoSetWithControllerRangeOfMotion)
				{
					hand.ResetTemporarySkeletonRangeOfMotion(0.1f);
				}
			}
		}

		public static void HideAllTextHints(Hand hand)
		{
			ControllerButtonHints controllerButtonHints = ControllerButtonHints.GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.HideAllText();
			}
		}

		public static string GetActiveHintText(Hand hand, ISteamVR_Action_In_Source action)
		{
			ControllerButtonHints controllerButtonHints = ControllerButtonHints.GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				return controllerButtonHints.GetActiveHintText(action);
			}
			return string.Empty;
		}

		public Material controllerMaterial;

		public Material urpControllerMaterial;

		public Color flashColor = new Color(1f, 0.557f, 0f);

		public GameObject textHintPrefab;

		public SteamVR_Action_Vibration hapticFlash = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic", false);

		public bool autoSetWithControllerRangeOfMotion = true;

		[Header("Debug")]
		public bool debugHints;

		private SteamVR_RenderModel renderModel;

		private Player player;

		private List<MeshRenderer> renderers = new List<MeshRenderer>();

		private List<MeshRenderer> flashingRenderers = new List<MeshRenderer>();

		private float startTime;

		private float tickCount;

		private Dictionary<ISteamVR_Action_In_Source, ControllerButtonHints.ActionHintInfo> actionHintInfos;

		private Transform textHintParent;

		private int colorID;

		private Vector3 centerPosition = Vector3.zero;

		private SteamVR_Events.Action renderModelLoadedAction;

		protected SteamVR_Input_Sources inputSource;

		private Dictionary<string, Transform> componentTransformMap = new Dictionary<string, Transform>();

		private enum OffsetType
		{
			Up,
			Right,
			Forward,
			Back
		}

		private class ActionHintInfo
		{
			public string componentName;

			public List<MeshRenderer> renderers;

			public Transform localTransform;

			public GameObject textHintObject;

			public Transform textStartAnchor;

			public Transform textEndAnchor;

			public Vector3 textEndOffsetDir;

			public Transform canvasOffset;

			public Text text;

			public TextMesh textMesh;

			public Canvas textCanvas;

			public LineRenderer line;

			public float distanceFromCenter;

			public bool textHintActive;
		}
	}
}
