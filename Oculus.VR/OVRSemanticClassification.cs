using System;
using System.Collections.Generic;
using Meta.XR.Util;
using UnityEngine;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-scene-use-scene-anchors/#further-scene-model-unity-components")]
[DisallowMultipleComponent]
[RequireComponent(typeof(OVRSceneAnchor))]
[Obsolete("OVRSceneManager and associated classes are deprecated (v65), please use MR Utility Kit instead (https://developer.oculus.com/documentation/unity/unity-mr-utility-kit-overview)")]
[Feature(Feature.Scene)]
public class OVRSemanticClassification : MonoBehaviour, IOVRSceneComponent
{
	public IReadOnlyList<string> Labels
	{
		get
		{
			return this._labels;
		}
	}

	public bool Contains(string label)
	{
		using (List<string>.Enumerator enumerator = this._labels.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == label)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void Awake()
	{
		if (base.GetComponent<OVRSceneAnchor>().Space.Valid)
		{
			((IOVRSceneComponent)this).Initialize();
		}
	}

	void IOVRSceneComponent.Initialize()
	{
		string labels;
		if (OVRPlugin.GetSpaceSemanticLabels(base.GetComponent<OVRSceneAnchor>().Space, out labels))
		{
			this._labels.Clear();
			this._labels.AddRange(OVRSemanticClassification.ValidateAndUpgradeLabels(labels).Split(',', StringSplitOptions.None));
		}
	}

	internal static string ValidateAndUpgradeLabels(string labels)
	{
		List<string> list;
		string result;
		using (new OVRObjectPool.ListScope<string>(ref list))
		{
			string[] array = labels.Split(',', StringSplitOptions.None);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			foreach (string text in array)
			{
				list.Add(text);
				if (text == "TABLE")
				{
					flag = true;
				}
				else if (text == "DESK")
				{
					flag2 = true;
				}
				else if (text == "INVISIBLE_WALL_FACE")
				{
					flag3 = true;
				}
				else if (text == "WALL_FACE")
				{
					flag4 = true;
				}
			}
			if (flag && !flag2)
			{
				list.Add("DESK");
			}
			else if (flag2 && !flag)
			{
				list.Add("TABLE");
			}
			if (flag3 && !flag4)
			{
				list.Add("WALL_FACE");
			}
			result = string.Join(','.ToString(), list);
		}
		return result;
	}

	public const char LabelSeparator = ',';

	private readonly List<string> _labels = new List<string>();
}
