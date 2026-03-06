using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-scene-use-scene-anchors/")]
[Obsolete("OVRSceneManager and associated classes are deprecated (v65), please use MR Utility Kit instead (https://developer.oculus.com/documentation/unity/unity-mr-utility-kit-overview)")]
[Serializable]
public class OVRScenePrefabOverride : ISerializationCallbackReceiver
{
	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		this.UpdateEditorClassificationIndex();
	}

	internal void UpdateEditorClassificationIndex()
	{
		if (this.ClassificationLabel != "")
		{
			this._editorClassificationIndex = OVRScenePrefabOverride.<UpdateEditorClassificationIndex>g__IndexOf|5_0(this.ClassificationLabel, OVRSceneManager.Classification.List);
			if (this._editorClassificationIndex < 0)
			{
				Debug.LogError("[OVRScenePrefabOverride] OnAfterDeserialize() " + this.ClassificationLabel + " not found. The Classification list in OVRSceneManager has likely changed");
				return;
			}
		}
		else
		{
			this._editorClassificationIndex = 0;
		}
	}

	[CompilerGenerated]
	internal static int <UpdateEditorClassificationIndex>g__IndexOf|5_0(string label, IEnumerable<string> collection)
	{
		int num = 0;
		using (IEnumerator<string> enumerator = collection.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == label)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	[FormerlySerializedAs("prefab")]
	public OVRSceneAnchor Prefab;

	[FormerlySerializedAs("classificationLabel")]
	public string ClassificationLabel = "";

	[FormerlySerializedAs("editorClassificationIndex")]
	[SerializeField]
	private int _editorClassificationIndex;
}
