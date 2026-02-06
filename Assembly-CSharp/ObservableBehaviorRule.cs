using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ObservableBehaviorRule", menuName = "Utilities/ObservableBehaviorRule")]
public class ObservableBehaviorRule : ScriptableObject
{
	public Vector2 ObservableDistanceRange
	{
		get
		{
			return this.observableDistanceRange;
		}
	}

	public Vector2 ObservableDotRange
	{
		get
		{
			return this.observableDotRange;
		}
	}

	[SerializeField]
	private Vector2 observableDistanceRange = new Vector2(0f, 15f);

	[SerializeField]
	private Vector2 observableDotRange = new Vector2(-1f, 0f);
}
