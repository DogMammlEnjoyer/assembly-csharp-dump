using System;
using UnityEngine;

public class RockPiles : MonoBehaviour
{
	public void Show(int visiblePercentage)
	{
		if (visiblePercentage <= 0)
		{
			this.ShowRock(-1);
			return;
		}
		int rockToShow = -1;
		int num = -1;
		for (int i = 0; i < this._rocks.Length; i++)
		{
			RockPiles.RockPile rockPile = this._rocks[i];
			if (visiblePercentage >= rockPile.threshold && num < rockPile.threshold)
			{
				rockToShow = i;
				num = rockPile.threshold;
			}
		}
		this.ShowRock(rockToShow);
	}

	private void ShowRock(int rockToShow)
	{
		for (int i = 0; i < this._rocks.Length; i++)
		{
			this._rocks[i].visual.SetActive(i == rockToShow);
		}
	}

	[SerializeField]
	private RockPiles.RockPile[] _rocks;

	[Serializable]
	public struct RockPile
	{
		public GameObject visual;

		public int threshold;
	}
}
