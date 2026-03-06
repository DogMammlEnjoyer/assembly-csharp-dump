using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.Android;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-scene-use-scene-anchors/#what-does-ovrscenemanager-do")]
[RequireComponent(typeof(OVRSceneManager))]
[Obsolete("OVRSceneManager and associated classes are deprecated (v65), please use MR Utility Kit instead (https://developer.oculus.com/documentation/unity/unity-mr-utility-kit-overview)")]
[Feature(Feature.Scene)]
public class OVRSceneModelLoader : MonoBehaviour
{
	private protected OVRSceneManager SceneManager { protected get; private set; }

	protected virtual void Start()
	{
		OVRTelemetry.SendEvent(163059869, OVRPlugin.Qpl.ResultType.Success);
		this.SceneManager = base.GetComponent<OVRSceneManager>();
		OVRSceneManager sceneManager = this.SceneManager;
		sceneManager.SceneModelLoadedSuccessfully = (Action)Delegate.Combine(sceneManager.SceneModelLoadedSuccessfully, new Action(this.OnSceneModelLoadedSuccessfully));
		OVRSceneManager sceneManager2 = this.SceneManager;
		sceneManager2.NoSceneModelToLoad = (Action)Delegate.Combine(sceneManager2.NoSceneModelToLoad, new Action(this.OnNoSceneModelToLoad));
		OVRSceneManager sceneManager3 = this.SceneManager;
		sceneManager3.NewSceneModelAvailable = (Action)Delegate.Combine(sceneManager3.NewSceneModelAvailable, new Action(this.OnNewSceneModelAvailable));
		this.SceneManager.LoadSceneModelFailedPermissionNotGranted += this.OnLoadSceneModelFailedPermissionNotGranted;
		OVRSceneManager sceneManager4 = this.SceneManager;
		sceneManager4.SceneCaptureReturnedWithoutError = (Action)Delegate.Combine(sceneManager4.SceneCaptureReturnedWithoutError, new Action(this.OnSceneCaptureReturnedWithoutError));
		OVRSceneManager sceneManager5 = this.SceneManager;
		sceneManager5.UnexpectedErrorWithSceneCapture = (Action)Delegate.Combine(sceneManager5.UnexpectedErrorWithSceneCapture, new Action(this.OnUnexpectedErrorWithSceneCapture));
		this.OnStart();
	}

	private IEnumerator AttemptToLoadSceneModel()
	{
		float timeSinceReminder = 0f;
		do
		{
			timeSinceReminder += Time.deltaTime;
			if (timeSinceReminder >= 10f)
			{
				timeSinceReminder = 0f;
			}
			yield return null;
		}
		while (!this.SceneManager.LoadSceneModel());
		yield break;
	}

	protected virtual void OnStart()
	{
		this.LoadSceneModel();
	}

	protected static OVRTask<bool> RequestScenePermissionAsync()
	{
		return OVRTask.FromResult<bool>(true);
	}

	protected virtual void OnLoadSceneModelFailedPermissionNotGranted()
	{
		OVRSceneModelLoader.<OnLoadSceneModelFailedPermissionNotGranted>d__10 <OnLoadSceneModelFailedPermissionNotGranted>d__;
		<OnLoadSceneModelFailedPermissionNotGranted>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnLoadSceneModelFailedPermissionNotGranted>d__.<>4__this = this;
		<OnLoadSceneModelFailedPermissionNotGranted>d__.<>1__state = -1;
		<OnLoadSceneModelFailedPermissionNotGranted>d__.<>t__builder.Start<OVRSceneModelLoader.<OnLoadSceneModelFailedPermissionNotGranted>d__10>(ref <OnLoadSceneModelFailedPermissionNotGranted>d__);
	}

	private void LoadSceneModel()
	{
		if (this.SceneManager.Verbose != null)
		{
			OVRSceneManager.LogForwarder? logForwarder;
			logForwarder.GetValueOrDefault().Log("OVRSceneModelLoader", "OnStart() calling OVRSceneManager.LoadSceneModel()", null);
		}
		if (!this.SceneManager.LoadSceneModel() && OVRManager.isHmdPresent)
		{
			base.StartCoroutine(this.AttemptToLoadSceneModel());
		}
	}

	protected virtual void OnSceneModelLoadedSuccessfully()
	{
		if (this.SceneManager.Verbose == null)
		{
			return;
		}
		OVRSceneManager.LogForwarder? logForwarder;
		logForwarder.GetValueOrDefault().Log("OVRSceneModelLoader", "OVRSceneManager.LoadSceneModel() completed successfully.", null);
	}

	protected virtual void OnNoSceneModelToLoad()
	{
		OVRSceneManager.LogForwarder? verbose;
		if (!this._sceneCaptureRequested)
		{
			verbose = this.SceneManager.Verbose;
			if (verbose != null)
			{
				verbose.GetValueOrDefault().Log("OVRSceneModelLoader", "OnNoSceneModelToLoad() calling OVRSceneManager.RequestSceneCapture()", null);
			}
			this._sceneCaptureRequested = this.SceneManager.RequestSceneCapture();
			return;
		}
		verbose = this.SceneManager.Verbose;
		if (verbose == null)
		{
			return;
		}
		verbose.GetValueOrDefault().Log("OVRSceneModelLoader", "OnSceneCaptureReturnedWithoutError() There is no scene model, but we have already requested scene capture once. No further action will be taken.", null);
	}

	protected virtual void OnNewSceneModelAvailable()
	{
		if (this.SceneManager.Verbose != null)
		{
			OVRSceneManager.LogForwarder? logForwarder;
			logForwarder.GetValueOrDefault().Log("OVRSceneModelLoader", "OnNewSceneModelAvailable() calling OVRSceneManager.LoadSceneModel()", null);
		}
		this.SceneManager.LoadSceneModel();
	}

	protected virtual void OnSceneCaptureReturnedWithoutError()
	{
		if (this.SceneManager.Verbose == null)
		{
			return;
		}
		OVRSceneManager.LogForwarder? logForwarder;
		logForwarder.GetValueOrDefault().Log("OVRSceneModelLoader", "Room setup returned without errors.", null);
	}

	protected virtual void OnUnexpectedErrorWithSceneCapture()
	{
		if (this.SceneManager.Verbose == null)
		{
			return;
		}
		OVRSceneManager.LogForwarder? logForwarder;
		logForwarder.GetValueOrDefault().LogError("OVRSceneModelLoader", "Requesting the Room Setup failed. The Scene Model cannot be loaded.", null);
	}

	[CompilerGenerated]
	internal static OVRTask<bool> <RequestScenePermissionAsync>g__RequestPermissionOnAndroid|9_0()
	{
		Guid taskId = Guid.NewGuid();
		PermissionCallbacks permissionCallbacks = new PermissionCallbacks();
		permissionCallbacks.PermissionGranted += delegate(string _)
		{
			OVRTask.SetResult<bool>(taskId, true);
		};
		permissionCallbacks.PermissionDenied += delegate(string _)
		{
			OVRTask.SetResult<bool>(taskId, false);
		};
		OVRTask<bool> result = OVRTask.Create<bool>(taskId);
		Permission.RequestUserPermission("com.oculus.permission.USE_SCENE", permissionCallbacks);
		return result;
	}

	private const float RetryingReminderDelay = 10f;

	private bool _sceneCaptureRequested;
}
