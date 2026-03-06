using System;
using System.Collections;
using System.Reflection;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.UI;

namespace UnityEngine.Rendering
{
	internal class DebugUpdater : MonoBehaviour
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void RuntimeInit()
		{
		}

		internal static void SetEnabled(bool enabled)
		{
			if (enabled)
			{
				DebugUpdater.EnableRuntime();
				return;
			}
			DebugUpdater.DisableRuntime();
		}

		private static void EnableRuntime()
		{
			if (DebugUpdater.s_Instance != null)
			{
				return;
			}
			GameObject gameObject = new GameObject();
			gameObject.name = "[Debug Updater]";
			DebugUpdater.s_Instance = gameObject.AddComponent<DebugUpdater>();
			DebugUpdater.s_Instance.m_Orientation = Screen.orientation;
			Object.DontDestroyOnLoad(gameObject);
			DebugManager.instance.EnableInputActions();
			EnhancedTouchSupport.Enable();
		}

		private static void DisableRuntime()
		{
			DebugManager instance = DebugManager.instance;
			instance.displayRuntimeUI = false;
			instance.displayPersistentRuntimeUI = false;
			if (DebugUpdater.s_Instance != null)
			{
				CoreUtils.Destroy(DebugUpdater.s_Instance.gameObject);
				DebugUpdater.s_Instance = null;
			}
		}

		internal static void HandleInternalEventSystemComponents(bool uiEnabled)
		{
			if (DebugUpdater.s_Instance == null)
			{
				return;
			}
			if (uiEnabled)
			{
				DebugUpdater.s_Instance.EnsureExactlyOneEventSystem();
				return;
			}
			DebugUpdater.s_Instance.DestroyDebugEventSystem();
		}

		private void EnsureExactlyOneEventSystem()
		{
			EventSystem[] array = Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
			EventSystem component = base.GetComponent<EventSystem>();
			if (array.Length > 1 && component != null)
			{
				Debug.Log("More than one EventSystem detected in scene. Destroying EventSystem owned by DebugUpdater.");
				this.DestroyDebugEventSystem();
				return;
			}
			if (array.Length == 0)
			{
				Debug.Log("No EventSystem available. Creating a new EventSystem to enable Rendering Debugger runtime UI.");
				this.CreateDebugEventSystem();
				return;
			}
			base.StartCoroutine(this.DoAfterInputModuleUpdated(new Action(this.CheckInputModuleExists)));
		}

		private IEnumerator DoAfterInputModuleUpdated(Action action)
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			action();
			yield break;
		}

		private void CheckInputModuleExists()
		{
			if (EventSystem.current != null && EventSystem.current.currentInputModule == null)
			{
				Debug.LogWarning("Found a game object with EventSystem component but no corresponding BaseInputModule component - Debug UI input might not work correctly.");
			}
		}

		private void AssignDefaultActions()
		{
			if (EventSystem.current != null)
			{
				InputSystemUIInputModule inputSystemUIInputModule = EventSystem.current.currentInputModule as InputSystemUIInputModule;
				if (inputSystemUIInputModule != null)
				{
					MethodInfo method = inputSystemUIInputModule.GetType().GetMethod("AssignDefaultActions");
					if (method != null)
					{
						method.Invoke(inputSystemUIInputModule, null);
					}
				}
			}
			this.CheckInputModuleExists();
		}

		private void CreateDebugEventSystem()
		{
			base.gameObject.AddComponent<EventSystem>();
			base.gameObject.AddComponent<InputSystemUIInputModule>();
			base.StartCoroutine(this.DoAfterInputModuleUpdated(new Action(this.AssignDefaultActions)));
		}

		private void DestroyDebugEventSystem()
		{
			Object component = base.GetComponent<EventSystem>();
			InputSystemUIInputModule component2 = base.GetComponent<InputSystemUIInputModule>();
			if (component2)
			{
				CoreUtils.Destroy(component2);
				base.StartCoroutine(this.DoAfterInputModuleUpdated(new Action(this.AssignDefaultActions)));
			}
			CoreUtils.Destroy(component);
		}

		private void Update()
		{
			DebugManager instance = DebugManager.instance;
			if (this.m_RuntimeUiWasVisibleLastFrame != instance.displayRuntimeUI)
			{
				DebugUpdater.HandleInternalEventSystemComponents(instance.displayRuntimeUI);
			}
			instance.UpdateActions();
			if (instance.GetAction(DebugAction.EnableDebugMenu) != 0f || instance.GetActionToggleDebugMenuWithTouch())
			{
				instance.displayRuntimeUI = !instance.displayRuntimeUI;
			}
			if (instance.displayRuntimeUI)
			{
				if (instance.GetAction(DebugAction.ResetAll) != 0f)
				{
					instance.Reset();
				}
				if (instance.GetActionReleaseScrollTarget())
				{
					instance.SetScrollTarget(null);
				}
			}
			if (this.m_Orientation != Screen.orientation)
			{
				base.StartCoroutine(DebugUpdater.RefreshRuntimeUINextFrame());
				this.m_Orientation = Screen.orientation;
			}
			this.m_RuntimeUiWasVisibleLastFrame = instance.displayRuntimeUI;
		}

		private static IEnumerator RefreshRuntimeUINextFrame()
		{
			yield return null;
			DebugManager.instance.ReDrawOnScreenDebug();
			yield break;
		}

		private static DebugUpdater s_Instance;

		private ScreenOrientation m_Orientation;

		private bool m_RuntimeUiWasVisibleLastFrame;
	}
}
