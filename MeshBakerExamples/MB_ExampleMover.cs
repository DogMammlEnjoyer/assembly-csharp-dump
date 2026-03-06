using System;
using UnityEngine;

public class MB_ExampleMover : MonoBehaviour
{
	private void Update()
	{
		Vector3 position = new Vector3(5f, 5f, 5f);
		ref Vector3 ptr = ref position;
		int index = this.axis;
		ptr[index] *= Mathf.Sin(Time.time);
		base.transform.position = position;
	}

	public int axis;
}
