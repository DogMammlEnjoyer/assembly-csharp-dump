using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RigEventVolumeObserver : MonoBehaviour
{
	private void Awake()
	{
		for (int i = 0; i < this.tMP_Texts.Length; i++)
		{
			this.formats.Add(this.tMP_Texts[i].text);
		}
	}

	private void OnEnable()
	{
		this.Observed_OnCountChanged();
		this.observed.OnCountChanged += this.Observed_OnCountChanged;
	}

	private void OnDisable()
	{
		this.observed.OnCountChanged -= this.Observed_OnCountChanged;
	}

	private void Observed_OnCountChanged()
	{
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			this.gameObjects[i].ApplyActiveState(this.observed);
		}
		for (int j = 0; j < this.tMP_Texts.Length; j++)
		{
			this.tMP_Texts[j].text = this.Format(this.formats[j]);
		}
	}

	private string Format(string s)
	{
		return s.Replace("\\c", this.observed.RigCount.ToString());
	}

	[SerializeField]
	private RigEventVolume observed;

	[SerializeField]
	private RigEventVolumeObserver.RigEventVolumeObserverGameObject[] gameObjects;

	[SerializeField]
	private TMP_Text[] tMP_Texts;

	private List<string> formats = new List<string>();

	[Serializable]
	private class RigEventVolumeObserverGameObject
	{
		public bool Check(RigEventVolume rev)
		{
			switch (this.comparison)
			{
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.EQ:
				return rev.RigCount == this.value;
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.LT:
				return rev.RigCount < this.value;
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.GT:
				return rev.RigCount > this.value;
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.LT_EQ:
				return rev.RigCount <= this.value;
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.GT_EQ:
				return rev.RigCount >= this.value;
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.NEQ:
				return rev.RigCount != this.value;
			default:
				return false;
			}
		}

		public void ApplyActiveState(RigEventVolume rev)
		{
			this.gameObject.SetActive(this.Check(rev));
		}

		[SerializeField]
		private GameObject gameObject;

		[SerializeField]
		public RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison comparison;

		[SerializeField]
		public int value;

		public enum Comparison
		{
			EQ,
			LT,
			GT,
			LT_EQ,
			GT_EQ,
			NEQ
		}
	}
}
