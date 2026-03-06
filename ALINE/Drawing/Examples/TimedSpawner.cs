using System;
using System.Collections;
using UnityEngine;

namespace Drawing.Examples
{
	public class TimedSpawner : MonoBehaviour
	{
		private IEnumerator Start()
		{
			for (;;)
			{
				GameObject go = Object.Instantiate<GameObject>(this.prefab, base.transform.position + Random.insideUnitSphere * 0.01f, Random.rotation);
				base.StartCoroutine(this.DestroyAfter(go, this.lifeTime));
				yield return new WaitForSeconds(this.interval);
			}
			yield break;
		}

		private IEnumerator DestroyAfter(GameObject go, float delay)
		{
			yield return new WaitForSeconds(delay);
			Object.Destroy(go);
			yield break;
		}

		public float interval = 1f;

		public float lifeTime = 5f;

		public GameObject prefab;
	}
}
