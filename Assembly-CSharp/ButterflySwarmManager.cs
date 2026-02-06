using System;
using System.Collections.Generic;
using UnityEngine;

public class ButterflySwarmManager : MonoBehaviour
{
	public float PerchedFlapSpeed { get; private set; }

	public float PerchedFlapPhase { get; private set; }

	public float BeeSpeed { get; private set; }

	public float BeeMaxTravelTime { get; private set; }

	public float BeeAcceleration { get; private set; }

	public float BeeJitterStrength { get; private set; }

	public float BeeJitterDamping { get; private set; }

	public float BeeMaxJitterRadius { get; private set; }

	public float BeeNearDestinationRadius { get; private set; }

	public float DestRotationAlignmentSpeed { get; private set; }

	public Vector3 TravellingLocalRotationEuler { get; private set; }

	public Quaternion TravellingLocalRotation { get; private set; }

	public float AvoidPointRadius { get; private set; }

	public float BeeMinFlowerDuration { get; private set; }

	public float BeeMaxFlowerDuration { get; private set; }

	public Color[] BeeColors { get; private set; }

	private void Awake()
	{
		this.TravellingLocalRotation = Quaternion.Euler(this.TravellingLocalRotationEuler);
		this.butterflies = new List<AnimatedButterfly>(this.numBees);
		for (int i = 0; i < this.numBees; i++)
		{
			AnimatedButterfly item = default(AnimatedButterfly);
			item.InitVisual(this.beePrefab, this);
			if (this.BeeColors.Length != 0)
			{
				item.SetColor(this.BeeColors[i % this.BeeColors.Length]);
			}
			this.butterflies.Add(item);
		}
	}

	private void Start()
	{
		foreach (XSceneRef xsceneRef in this.perchSections)
		{
			GameObject gameObject;
			if (xsceneRef.TryResolve(out gameObject))
			{
				List<GameObject> list = new List<GameObject>();
				this.allPerchZones.Add(list);
				foreach (object obj in gameObject.transform)
				{
					Transform transform = (Transform)obj;
					list.Add(transform.gameObject);
				}
			}
		}
		this.OnSeedChange();
		RandomTimedSeedManager.instance.AddCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	private void OnDestroy()
	{
		RandomTimedSeedManager.instance.RemoveCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	private void Update()
	{
		for (int i = 0; i < this.butterflies.Count; i++)
		{
			AnimatedButterfly value = this.butterflies[i];
			value.UpdateVisual(RandomTimedSeedManager.instance.currentSyncTime, this);
			this.butterflies[i] = value;
		}
	}

	private void OnSeedChange()
	{
		SRand srand = new SRand(RandomTimedSeedManager.instance.seed);
		List<List<GameObject>> list = new List<List<GameObject>>(this.allPerchZones.Count);
		for (int i = 0; i < this.allPerchZones.Count; i++)
		{
			List<GameObject> list2 = new List<GameObject>();
			list2.AddRange(this.allPerchZones[i]);
			list.Add(list2);
		}
		List<GameObject> list3 = new List<GameObject>(this.loopSizePerBee);
		List<float> list4 = new List<float>(this.loopSizePerBee);
		for (int j = 0; j < this.butterflies.Count; j++)
		{
			AnimatedButterfly value = this.butterflies[j];
			value.SetFlapSpeed(srand.NextFloat(this.minFlapSpeed, this.maxFlapSpeed));
			list3.Clear();
			list4.Clear();
			this.PickPoints(this.loopSizePerBee, list, ref srand, list3);
			for (int k = 0; k < list3.Count; k++)
			{
				list4.Add(srand.NextFloat(this.BeeMinFlowerDuration, this.BeeMaxFlowerDuration));
			}
			if (list3.Count == 0)
			{
				this.butterflies.Clear();
				return;
			}
			value.InitRoute(list3, list4, this);
			this.butterflies[j] = value;
		}
	}

	private void PickPoints(int n, List<List<GameObject>> pickBuffer, ref SRand rand, List<GameObject> resultBuffer)
	{
		int exclude = rand.NextInt(0, pickBuffer.Count);
		int num = -1;
		int num2 = n - 2;
		while (resultBuffer.Count < n)
		{
			int num3;
			if (resultBuffer.Count < num2)
			{
				num3 = rand.NextIntWithExclusion(0, pickBuffer.Count, num);
			}
			else
			{
				num3 = rand.NextIntWithExclusion2(0, pickBuffer.Count, num, exclude);
			}
			int num4 = 10;
			while (num3 == num || pickBuffer[num3].Count == 0)
			{
				num3 = (num3 + 1) % pickBuffer.Count;
				num4--;
				if (num4 <= 0)
				{
					return;
				}
			}
			num = num3;
			List<GameObject> list = pickBuffer[num];
			while (list.Count == 0)
			{
				num = (num + 1) % pickBuffer.Count;
				list = pickBuffer[num];
			}
			resultBuffer.Add(list[list.Count - 1]);
			list.RemoveAt(list.Count - 1);
		}
	}

	[SerializeField]
	private XSceneRef[] perchSections;

	[SerializeField]
	private int loopSizePerBee;

	[SerializeField]
	private int numBees;

	[SerializeField]
	private MeshRenderer beePrefab;

	[SerializeField]
	private float maxFlapSpeed;

	[SerializeField]
	private float minFlapSpeed;

	private List<AnimatedButterfly> butterflies;

	private List<List<GameObject>> allPerchZones = new List<List<GameObject>>();
}
