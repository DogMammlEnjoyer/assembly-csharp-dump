using System;
using UnityEngine;

public class RandomLocalColliders : MonoBehaviour
{
	private void Start()
	{
		this.seekFreq = RandomLocalColliders.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
	}

	private void Update()
	{
		if (this.colliderFound == null)
		{
			return;
		}
		this.timeSinceSeek += Time.deltaTime;
		if (this.timeSinceSeek > this.seekFreq)
		{
			this.seek();
			this.timeSinceSeek = 0f;
			this.seekFreq = RandomLocalColliders.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
		}
	}

	private void seek()
	{
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		int num2 = Physics.RaycastNonAlloc(base.transform.position, RandomLocalColliders.rand.NextPointOnSphere(1f), this.raycastHits, this.maxRadias * num);
		if (num2 <= 0)
		{
			return;
		}
		int num3 = RandomLocalColliders.rand.NextInt(num2);
		for (int i = 0; i < num2; i++)
		{
			if (this.raycastHits[(i + num3) % num2].distance >= this.minRadias * num)
			{
				this.colliderFound.Invoke(base.transform.position, this.raycastHits[(i + num3) % num2].point);
				return;
			}
		}
	}

	private static SRand rand = new SRand("RandomLocalColliders");

	[SerializeField]
	private float minseekFreq = 3f;

	[SerializeField]
	private float maxseekFreq = 6f;

	[SerializeField]
	private float minRadias = 1f;

	[SerializeField]
	private float maxRadias = 10f;

	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	private float timeSinceSeek;

	private float seekFreq;

	private RaycastHit[] raycastHits = new RaycastHit[100];
}
