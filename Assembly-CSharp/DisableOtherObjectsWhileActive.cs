using System;
using UnityEngine;

public class DisableOtherObjectsWhileActive : MonoBehaviour
{
	private void OnEnable()
	{
		this.SetAllActive(false);
	}

	private void OnDisable()
	{
		this.SetAllActive(true);
	}

	private void SetAllActive(bool active)
	{
		for (int i = 0; i < this.otherObjects.Length; i++)
		{
			GameObject gameObject = this.otherObjects[i];
			if (gameObject != null)
			{
				gameObject.SetActive(active);
			}
		}
		for (int j = 0; j < this.otherXSceneObjects.Length; j++)
		{
			XSceneRef xsceneRef = this.otherXSceneObjects[j];
			GameObject gameObject2;
			if (xsceneRef.TryResolve(out gameObject2) && gameObject2 != null)
			{
				gameObject2.SetActive(active);
			}
		}
	}

	public const string preErr = "[GT/DisableOtherObjectsWhileActive]  ERROR!!!  ";

	public GameObject[] otherObjects;

	public XSceneRef[] otherXSceneObjects;
}
