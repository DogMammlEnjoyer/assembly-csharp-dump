using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using Meta.XR.Acoustics;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

internal class MetaXRAcousticMap : MonoBehaviour
{
	internal bool StaticOnly
	{
		get
		{
			return (this.Flags & AcousticMapFlags.STATIC_ONLY) > AcousticMapFlags.NONE;
		}
		set
		{
			if (value)
			{
				this.Flags |= AcousticMapFlags.STATIC_ONLY;
				return;
			}
			this.Flags &= ~AcousticMapFlags.STATIC_ONLY;
		}
	}

	internal bool NoFloating
	{
		get
		{
			return (this.Flags & AcousticMapFlags.NO_FLOATING) > AcousticMapFlags.NONE;
		}
		set
		{
			if (value)
			{
				this.Flags |= AcousticMapFlags.NO_FLOATING;
				return;
			}
			this.Flags &= ~AcousticMapFlags.NO_FLOATING;
		}
	}

	internal bool Diffraction
	{
		get
		{
			return (this.Flags & AcousticMapFlags.DIFFRACTION) > AcousticMapFlags.NONE;
		}
		set
		{
			if (value)
			{
				this.Flags |= AcousticMapFlags.DIFFRACTION;
				return;
			}
			this.Flags &= ~AcousticMapFlags.DIFFRACTION;
		}
	}

	internal Vector3 GravityVector
	{
		get
		{
			return this.gravityVector;
		}
		set
		{
			this.gravityVector = value.normalized;
		}
	}

	internal string RelativeFilePath
	{
		get
		{
			return this.relativeFilePath;
		}
	}

	internal string AbsoluteFilePath
	{
		get
		{
			return Path.GetFullPath(Path.Combine(Application.dataPath, this.RelativeFilePath));
		}
		set
		{
			string text = value.Replace('\\', '/');
			if (text.StartsWith(Application.dataPath))
			{
				this.relativeFilePath = text.Substring(Application.dataPath.Length + 1);
				if (File.Exists(this.AbsoluteFilePath))
				{
					this.DestroyInternal();
					this.StartInternal(true);
					return;
				}
			}
			else
			{
				Debug.LogError("invalid path " + value + ", outside application path " + Application.dataPath);
			}
		}
	}

	private void Start()
	{
		this.StartInternal(true);
	}

	internal void StartInternal(bool autoLoad = true)
	{
		if (this.mapHandle != IntPtr.Zero)
		{
			return;
		}
		if (MetaXRAcousticNativeInterface.Interface.CreateAudioSceneIR(out this.mapHandle) != 0)
		{
			Debug.LogError("Unable to create internal Acoustic Map", base.gameObject);
			return;
		}
		if (Application.isPlaying)
		{
			if (!string.IsNullOrEmpty(this.relativeFilePath))
			{
				string text = this.relativeFilePath;
				if (this.relativeFilePath.StartsWith("StreamingAssets"))
				{
					string streamingAssetsSubPath = text.Substring("StreamingAssets/".Length);
					base.StartCoroutine(this.LoadMapAsync(streamingAssetsSubPath));
				}
			}
		}
		else if (autoLoad)
		{
			bool flag = !string.IsNullOrEmpty(this.relativeFilePath) && !string.IsNullOrEmpty(base.name) && File.Exists(this.AbsoluteFilePath);
			if (flag)
			{
				Debug.Log("Loading Acoustic Map " + base.name + " from File " + this.AbsoluteFilePath);
			}
			int num = MetaXRAcousticNativeInterface.Interface.AudioSceneIRReadFile(this.mapHandle, this.AbsoluteFilePath);
			if (num != 0)
			{
				if (flag)
				{
					Debug.LogError(string.Format("Error {0}: Unable to load the Acoustic Map from file: {1}", num, this.AbsoluteFilePath));
				}
				return;
			}
			if (!flag)
			{
				Debug.Log("Found data in default location: " + this.RelativeFilePath);
				this.relativeFilePath = this.RelativeFilePath;
			}
		}
		this.ApplyTransform();
	}

	private IEnumerator LoadMapAsync(string streamingAssetsSubPath)
	{
		string text = Application.streamingAssetsPath + "/" + streamingAssetsSubPath;
		Debug.Log("Loading Acoustic Map " + base.name + " from StreamingAssets " + text);
		float startTime = Time.realtimeSinceStartup;
		UnityWebRequest unityWebRequest = UnityWebRequest.Get(text);
		yield return unityWebRequest.SendWebRequest();
		if (!string.IsNullOrEmpty(unityWebRequest.error))
		{
			Debug.LogError(string.Format("web request: done={0}: {1}", unityWebRequest.isDone, unityWebRequest.error), base.gameObject);
		}
		float num = Time.realtimeSinceStartup - startTime;
		Debug.Log(string.Format("Acoustic Map {0}, read time = {1}", base.name, num), base.gameObject);
		this.LoadMapFromMemory(unityWebRequest.downloadHandler.nativeData);
		yield break;
	}

	private void LoadMapFromMemory(NativeArray<byte>.ReadOnly data)
	{
		MetaXRAcousticMap.<LoadMapFromMemory>d__36 <LoadMapFromMemory>d__;
		<LoadMapFromMemory>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<LoadMapFromMemory>d__.<>4__this = this;
		<LoadMapFromMemory>d__.data = data;
		<LoadMapFromMemory>d__.<>1__state = -1;
		<LoadMapFromMemory>d__.<>t__builder.Start<MetaXRAcousticMap.<LoadMapFromMemory>d__36>(ref <LoadMapFromMemory>d__);
	}

	private void OnDestroy()
	{
		this.DestroyInternal();
	}

	internal void DestroyInternal()
	{
		lock (this)
		{
			if (this.mapHandle != IntPtr.Zero)
			{
				if (MetaXRAcousticNativeInterface.Interface.DestroyAudioSceneIR(this.mapHandle) != 0)
				{
					Debug.LogError("Unable to destroy Acoustic Map", base.gameObject);
				}
				this.mapHandle = IntPtr.Zero;
			}
		}
	}

	private void OnEnable()
	{
		if (this.mapHandle == IntPtr.Zero)
		{
			return;
		}
		Debug.Log("Enabling AcousticMap: " + this.RelativeFilePath);
		MetaXRAcousticNativeInterface.Interface.AudioSceneIRSetEnabled(this.mapHandle, true);
	}

	private void OnDisable()
	{
		if (this.mapHandle == IntPtr.Zero)
		{
			return;
		}
		MetaXRAcousticGeometry.OnAnyGeometryEnabled -= this.delayedEnable;
		Debug.Log("Disabling AcousticMap: " + this.RelativeFilePath);
		MetaXRAcousticNativeInterface.Interface.AudioSceneIRSetEnabled(this.mapHandle, false);
	}

	private void LateUpdate()
	{
		if (this.mapHandle == IntPtr.Zero)
		{
			return;
		}
		if (base.transform.hasChanged)
		{
			this.ApplyTransform();
			base.transform.hasChanged = false;
		}
	}

	private void ApplyTransform()
	{
		MetaXRAcousticNativeInterface.INativeInterface @interface = MetaXRAcousticNativeInterface.Interface;
		IntPtr sceneIR = this.mapHandle;
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		@interface.AudioSceneIRSetTransform(sceneIR, localToWorldMatrix);
	}

	internal const string FILE_EXTENSION = "xramap";

	[SerializeField]
	internal MetaXRAcousticSceneGroup SceneGroup;

	[SerializeField]
	internal bool customPointsEnabled;

	[NonSerialized]
	internal bool IsLoaded;

	[SerializeField]
	internal AcousticMapFlags Flags = AcousticMapFlags.NO_FLOATING | AcousticMapFlags.DIFFRACTION;

	internal const float DISTANCE_PARAMETER_MAX = 10000f;

	[SerializeField]
	internal uint ReflectionCount = 6U;

	[SerializeField]
	[Range(0f, 10000f)]
	internal float MinSpacing = 1f;

	[SerializeField]
	[Range(0f, 10000f)]
	internal float MaxSpacing = 10f;

	[SerializeField]
	[Range(0f, 10000f)]
	internal float HeadHeight = 1.5f;

	[SerializeField]
	[Range(0f, 10000f)]
	internal float MaxHeight = 3f;

	[SerializeField]
	private Vector3 gravityVector = new Vector3(0f, -1f, 0f);

	[FormerlySerializedAs("relativeFilePath_")]
	[SerializeField]
	private string relativeFilePath = "";

	[NonSerialized]
	internal IntPtr mapHandle = IntPtr.Zero;

	[NonSerialized]
	private Action delayedEnable;

	internal const int Success = 0;
}
