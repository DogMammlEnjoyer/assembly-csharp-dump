using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class MothershipHttpRunner : MonoBehaviour
{
	public static MothershipHttpRunner instance
	{
		get
		{
			MothershipHttpRunner.CreateInstance();
			return MothershipHttpRunner._instance;
		}
	}

	private static void CreateInstance()
	{
		if (MothershipHttpRunner._instance == null)
		{
			MothershipHttpRunner._instance = new GameObject(typeof(MothershipHttpRunner).Name).AddComponent<MothershipHttpRunner>();
		}
	}

	public virtual void Awake()
	{
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(this);
		}
		if (MothershipHttpRunner._instance != null)
		{
			MothershipHttpRunner._instance = null;
			Object.DestroyImmediate(base.gameObject);
		}
	}

	public void SendRequest(UnityWebRequest uwr, MothershipHTTPRequest request, Action<MothershipHTTPResponse> responseCallback)
	{
		base.StartCoroutine(this.SendRequestInternal(uwr, request, responseCallback));
	}

	private IEnumerator SendRequestInternal(UnityWebRequest uwr, MothershipHTTPRequest request, Action<MothershipHTTPResponse> responseCallback)
	{
		yield return uwr.SendWebRequest();
		responseCallback(new MothershipHTTPResponse
		{
			statusCode = (int)uwr.responseCode,
			Body = uwr.downloadHandler.text,
			cbData = request.cbData
		});
		yield break;
	}

	private static MothershipHttpRunner _instance;
}
