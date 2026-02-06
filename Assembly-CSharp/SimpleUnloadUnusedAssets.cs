using System;
using System.Collections;
using UnityEngine;

public class SimpleUnloadUnusedAssets : MonoBehaviour
{
	private void OnEnable()
	{
		base.StartCoroutine(this.UnloadUnusedAssets());
	}

	private IEnumerator UnloadUnusedAssets()
	{
		yield return new WaitForSeconds(this.WaitForUnload);
		Debug.Log(string.Format("SimpleUnloadUnusedAssets: Forcing unload unused assets after waiting {0} seconds!", this.WaitForUnload));
		Resources.UnloadUnusedAssets();
		yield return new WaitForSeconds(this.WaitForUnload);
		Debug.Log(string.Format("SimpleUnloadUnusedAssets: Forcing unload unused assets after waiting {0} seconds!", this.WaitForUnload));
		Resources.UnloadUnusedAssets();
		yield break;
	}

	public float WaitForUnload = 5f;
}
