using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Meta.XR.Util;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(OVRSceneAnchor))]
[HelpURL("https://developer.oculus.com/documentation/unity/unity-scene-use-scene-anchors/#further-scene-model-unity-components")]
[Obsolete("OVRSceneManager and associated classes are deprecated (v65), please use MR Utility Kit instead (https://developer.oculus.com/documentation/unity/unity-mr-utility-kit-overview)")]
[Feature(Feature.Scene)]
public class OVRSceneRoom : MonoBehaviour, IOVRSceneComponent
{
	public OVRScenePlane Floor { get; private set; }

	public OVRScenePlane Ceiling { get; private set; }

	public OVRScenePlane[] Walls { get; private set; } = Array.Empty<OVRScenePlane>();

	private void Awake()
	{
		this._sceneAnchor = base.GetComponent<OVRSceneAnchor>();
		this._sceneManager = Object.FindAnyObjectByType<OVRSceneManager>();
		this._uuid = this._sceneAnchor.Uuid;
		if (this._sceneAnchor.Space.Valid)
		{
			((IOVRSceneComponent)this).Initialize();
		}
	}

	void IOVRSceneComponent.Initialize()
	{
		OVRSceneRoom.SceneRooms[this._uuid] = this;
		OVRSceneRoom.SceneRoomsList.Add(this);
	}

	internal OVRTask<bool> LoadRoom(Guid floor, Guid ceiling, Guid[] walls)
	{
		OVRSceneRoom.<LoadRoom>d__19 <LoadRoom>d__;
		<LoadRoom>d__.<>t__builder = OVRTaskBuilder<bool>.Create();
		<LoadRoom>d__.<>4__this = this;
		<LoadRoom>d__.floor = floor;
		<LoadRoom>d__.ceiling = ceiling;
		<LoadRoom>d__.walls = walls;
		<LoadRoom>d__.<>1__state = -1;
		<LoadRoom>d__.<>t__builder.Start<OVRSceneRoom.<LoadRoom>d__19>(ref <LoadRoom>d__);
		return <LoadRoom>d__.<>t__builder.Task;
	}

	private void OnDestroy()
	{
		OVRSceneRoom.SceneRooms.Remove(this._uuid);
		OVRSceneRoom.SceneRoomsList.Remove(this);
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void Log(string message)
	{
		Debug.Log("[OVRSceneRoom] " + message, base.gameObject);
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void LogWarning(string message)
	{
		Debug.LogWarning("[OVRSceneRoom] " + message, base.gameObject);
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void LogError(string message)
	{
		Debug.LogError("[OVRSceneRoom] " + message, base.gameObject);
	}

	[CompilerGenerated]
	internal static bool <LoadRoom>g__TryGetPlane|19_0(Guid uuid, out OVRScenePlane plane)
	{
		plane = null;
		OVRSceneAnchor ovrsceneAnchor;
		return OVRSceneAnchor.SceneAnchors.TryGetValue(uuid, out ovrsceneAnchor) && ovrsceneAnchor.TryGetComponent<OVRScenePlane>(out plane);
	}

	[CompilerGenerated]
	internal static OVRScenePlane <LoadRoom>g__GetPlane|19_1(Guid uuid)
	{
		OVRScenePlane result;
		if (!OVRSceneRoom.<LoadRoom>g__TryGetPlane|19_0(uuid, out result))
		{
			return null;
		}
		return result;
	}

	private OVRSceneAnchor _sceneAnchor;

	private OVRSceneManager _sceneManager;

	private Guid _uuid;

	internal static readonly Dictionary<Guid, OVRSceneRoom> SceneRooms = new Dictionary<Guid, OVRSceneRoom>();

	internal static readonly List<OVRSceneRoom> SceneRoomsList = new List<OVRSceneRoom>();
}
