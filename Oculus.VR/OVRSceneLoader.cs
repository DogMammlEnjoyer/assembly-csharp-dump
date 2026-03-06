using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Feature(Feature.Scene)]
public class OVRSceneLoader : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		string path = Path.Combine("/sdcard/Android/data", Application.identifier);
		this.scenePath = Path.Combine(path, "cache/scenes");
		this.sceneLoadDataPath = Path.Combine(this.scenePath, "SceneLoadData.txt");
		this.closeLogDialogue = false;
		base.StartCoroutine(this.DelayCanvasPosUpdate());
		this.currentSceneInfo = this.GetSceneInfo();
		if (this.currentSceneInfo.version != 0L && !string.IsNullOrEmpty(this.currentSceneInfo.scenes[0]))
		{
			this.LoadScene(this.currentSceneInfo);
		}
	}

	private void LoadScene(OVRSceneLoader.SceneInfo sceneInfo)
	{
		AssetBundle assetBundle = null;
		Debug.Log("[OVRSceneLoader] Loading main scene: " + sceneInfo.scenes[0] + " with version " + sceneInfo.version.ToString());
		Text text = this.logTextBox;
		text.text = text.text + "Target Scene: " + sceneInfo.scenes[0] + "\n";
		Text text2 = this.logTextBox;
		text2.text = text2.text + "Version: " + sceneInfo.version.ToString() + "\n";
		Debug.Log("[OVRSceneLoader] Loading scene bundle files.");
		string[] files = Directory.GetFiles(this.scenePath, "*_*");
		Text text3 = this.logTextBox;
		text3.text = text3.text + "Loading " + files.Length.ToString() + " bundle(s) . . . ";
		string b = "scene_" + sceneInfo.scenes[0].ToLower();
		try
		{
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				AssetBundle assetBundle2 = AssetBundle.LoadFromFile(array[i]);
				if (assetBundle2 != null)
				{
					Debug.Log(("[OVRSceneLoader] Loading file bundle: " + assetBundle2.name == null) ? "null" : assetBundle2.name);
					this.loadedAssetBundles.Add(assetBundle2);
				}
				else
				{
					Debug.LogError("[OVRSceneLoader] Loading file bundle failed");
				}
				if (assetBundle2.name == b)
				{
					assetBundle = assetBundle2;
				}
				if (assetBundle2.name == "asset_resources")
				{
					OVRResources.SetResourceBundle(assetBundle2);
				}
			}
		}
		catch (Exception ex)
		{
			Text text4 = this.logTextBox;
			text4.text = text4.text + "<color=red>" + ex.Message + "</color>";
			return;
		}
		Text text5 = this.logTextBox;
		text5.text += "<color=green>DONE\n</color>";
		if (assetBundle != null)
		{
			Text text6 = this.logTextBox;
			text6.text += "Loading Scene: {0:P0}\n";
			this.formattedLogText = this.logTextBox.text;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetBundle.GetAllScenePaths()[0]);
			this.loadSceneOperation = SceneManager.LoadSceneAsync(fileNameWithoutExtension);
			this.loadSceneOperation.completed += this.LoadSceneOperation_completed;
			return;
		}
		Text text7 = this.logTextBox;
		text7.text += "<color=red>Failed to get main scene bundle.\n</color>";
	}

	private void LoadSceneOperation_completed(AsyncOperation obj)
	{
		base.StartCoroutine(this.onCheckSceneCoroutine());
		base.StartCoroutine(this.DelayCanvasPosUpdate());
		this.closeLogTimer = 0f;
		this.closeLogDialogue = true;
		Text text = this.logTextBox;
		text.text += "Log closing in {0} seconds.\n";
		this.formattedLogText = this.logTextBox.text;
	}

	public void Update()
	{
		if (this.loadSceneOperation != null && !this.loadSceneOperation.isDone)
		{
			this.logTextBox.text = string.Format(this.formattedLogText, this.loadSceneOperation.progress + 0.1f);
			if (this.loadSceneOperation.progress >= 0.9f)
			{
				this.logTextBox.text = this.formattedLogText.Replace("{0:P0}", "<color=green>DONE</color>");
				Text text = this.logTextBox;
				text.text += "Transitioning to new scene.\nLoad times will vary depending on scene complexity.\n";
			}
		}
		this.UpdateCanvasPosition();
		if (this.closeLogDialogue)
		{
			if (this.closeLogTimer < this.logCloseTime)
			{
				this.closeLogTimer += Time.deltaTime;
				this.logTextBox.text = string.Format(this.formattedLogText, (int)(this.logCloseTime - this.closeLogTimer));
				return;
			}
			this.mainCanvas.gameObject.SetActive(false);
			this.closeLogDialogue = false;
		}
	}

	private void UpdateCanvasPosition()
	{
		if (this.mainCanvas.worldCamera != Camera.main)
		{
			this.mainCanvas.worldCamera = Camera.main;
			if (Camera.main != null)
			{
				Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * 0.3f;
				base.gameObject.transform.position = position;
				base.gameObject.transform.rotation = Camera.main.transform.rotation;
			}
		}
	}

	private OVRSceneLoader.SceneInfo GetSceneInfo()
	{
		OVRSceneLoader.SceneInfo result = default(OVRSceneLoader.SceneInfo);
		try
		{
			StreamReader streamReader = new StreamReader(this.sceneLoadDataPath);
			result.version = Convert.ToInt64(streamReader.ReadLine());
			List<string> list = new List<string>();
			while (!streamReader.EndOfStream)
			{
				list.Add(streamReader.ReadLine());
			}
			result.scenes = list;
		}
		catch
		{
			Text text = this.logTextBox;
			text.text += "<color=red>Failed to get scene info data.\n</color>";
		}
		return result;
	}

	private IEnumerator DelayCanvasPosUpdate()
	{
		yield return new WaitForSeconds(0.1f);
		this.UpdateCanvasPosition();
		yield break;
	}

	private IEnumerator onCheckSceneCoroutine()
	{
		while (this.GetSceneInfo().version == this.currentSceneInfo.version)
		{
			yield return new WaitForSeconds(this.sceneCheckIntervalSeconds);
		}
		Debug.Log("[OVRSceneLoader] Scene change detected.");
		foreach (AssetBundle assetBundle in this.loadedAssetBundles)
		{
			if (assetBundle != null)
			{
				assetBundle.Unload(true);
			}
		}
		this.loadedAssetBundles.Clear();
		int sceneCount = SceneManager.sceneCount;
		for (int i = 0; i < sceneCount; i++)
		{
			SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
		}
		this.DestroyAllGameObjects();
		SceneManager.LoadSceneAsync("OVRTransitionScene");
		yield break;
	}

	private void DestroyAllGameObjects()
	{
		GameObject[] array = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i]);
		}
	}

	public const string externalStoragePath = "/sdcard/Android/data";

	public const string sceneLoadDataName = "SceneLoadData.txt";

	public const string resourceBundleName = "asset_resources";

	public float sceneCheckIntervalSeconds = 1f;

	public float logCloseTime = 5f;

	public Canvas mainCanvas;

	public Text logTextBox;

	private AsyncOperation loadSceneOperation;

	private string formattedLogText;

	private float closeLogTimer;

	private bool closeLogDialogue;

	private bool canvasPosUpdated;

	private string scenePath = "";

	private string sceneLoadDataPath = "";

	private List<AssetBundle> loadedAssetBundles = new List<AssetBundle>();

	private OVRSceneLoader.SceneInfo currentSceneInfo;

	private struct SceneInfo
	{
		public SceneInfo(List<string> sceneList, long currentSceneEpochVersion)
		{
			this.scenes = sceneList;
			this.version = currentSceneEpochVersion;
		}

		public List<string> scenes;

		public long version;
	}
}
