using System;
using UnityEngine;
using UnityEngine.UI;

public class OVRBuildInfo : MonoBehaviour
{
	public void OnEnable()
	{
		this.LoadBuildInfo();
	}

	private void OnValidate()
	{
		this.LoadBuildInfo();
	}

	public void LoadBuildInfo()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("BuildInfo");
		this.BuildInfo.text = (((textAsset != null) ? textAsset.text : null) ?? "");
	}

	public Text BuildInfo;
}
