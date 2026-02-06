using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropSelector : MonoBehaviour
{
	private void Start()
	{
		foreach (GameObject gameObject in new List<GameObject>((from x in this._props
		orderby PropSelector._gRandom.Next()
		select x).Take(this._desiredActivePropsNum)))
		{
			gameObject.SetActive(true);
		}
	}

	[SerializeField]
	private List<GameObject> _props = new List<GameObject>();

	[SerializeField]
	private int _desiredActivePropsNum = 1;

	private static readonly Random _gRandom = new Random();
}
