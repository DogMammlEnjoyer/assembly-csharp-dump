using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[DefaultExecutionOrder(-99)]
public class OVROverlayCanvasManager : MonoBehaviour
{
	public static OVROverlayCanvasManager Instance
	{
		get
		{
			if (OVROverlayCanvasManager._instance != null)
			{
				return OVROverlayCanvasManager._instance;
			}
			if (!Application.isPlaying)
			{
				return null;
			}
			return OVROverlayCanvasManager._instance = new GameObject("OVROverlayCanvasManager").AddComponent<OVROverlayCanvasManager>();
		}
	}

	public static void AddCanvas(OVROverlayCanvas canvas)
	{
		OVROverlayCanvasManager instance = OVROverlayCanvasManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance._canvases.Add(canvas);
	}

	public static void RemoveCanvas(OVROverlayCanvas canvas)
	{
		OVROverlayCanvasManager instance = OVROverlayCanvasManager._instance;
		if (instance == null)
		{
			return;
		}
		instance._canvases.Remove(canvas);
	}

	public bool IsCanvasPriority(OVROverlayCanvas canvas)
	{
		return canvas.GetViewPriorityScore() != null && this._canvases.IndexOf(canvas) < OVROverlayCanvasSettings.Instance.MaxSimultaneousCanvases;
	}

	public IEnumerable<OVROverlayCanvas> Canvases
	{
		get
		{
			return this._canvases;
		}
	}

	protected void Awake()
	{
		OVROverlayCanvasManager._instance = this;
		Object.DontDestroyOnLoad(this);
	}

	protected void Update()
	{
		this._canvases.Sort(delegate(OVROverlayCanvas a, OVROverlayCanvas b)
		{
			float valueOrDefault = a.GetViewPriorityScore().GetValueOrDefault();
			float valueOrDefault2 = b.GetViewPriorityScore().GetValueOrDefault();
			if (!Mathf.Approximately(valueOrDefault, valueOrDefault2))
			{
				return (int)((valueOrDefault2 - valueOrDefault) * 10000f);
			}
			return b.GetHashCode() - a.GetHashCode();
		});
	}

	protected void OnDestroy()
	{
		if (OVROverlayCanvasManager._instance == this)
		{
			OVROverlayCanvasManager._instance = null;
		}
	}

	private static OVROverlayCanvasManager _instance;

	private List<OVROverlayCanvas> _canvases = new List<OVROverlayCanvas>();
}
