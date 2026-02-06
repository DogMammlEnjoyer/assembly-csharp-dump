using System;
using System.Collections.Generic;
using UnityEngine;

public class GRHealthMeter : MonoBehaviour
{
	public void Setup(int maxHP)
	{
		this.maxHP = maxHP;
	}

	public void SetHP(int hp)
	{
		int num = Mathf.CeilToInt((float)hp / (float)this.maxHP * (float)this.nodes.Count);
		for (int i = 0; i < this.nodes.Count; i++)
		{
			this.nodes[i].SetEmpty(i >= num);
		}
	}

	public List<GRHealthMeterNode> nodes;

	private int maxHP;
}
