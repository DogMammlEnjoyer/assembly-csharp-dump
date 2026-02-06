using System;
using UnityEngine;

public class GRHealthMeterNode : MonoBehaviour
{
	public void Setup()
	{
		this.isEmpty = true;
		this.SetEmpty(false);
	}

	public void SetEmpty(bool empty)
	{
		if (this.isEmpty == empty)
		{
			return;
		}
		this.isEmpty = empty;
		if (this.showFull != null)
		{
			this.showFull.SetActive(!this.isEmpty);
		}
		if (this.showEmpty != null)
		{
			this.showEmpty.SetActive(this.isEmpty);
		}
	}

	public GameObject showFull;

	public GameObject showEmpty;

	private bool isEmpty;
}
