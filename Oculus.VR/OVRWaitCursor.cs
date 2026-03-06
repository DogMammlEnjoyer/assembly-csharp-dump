using System;
using UnityEngine;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-core-overview/#scripts")]
public class OVRWaitCursor : MonoBehaviour
{
	private void Update()
	{
		base.transform.Rotate(this.rotateSpeeds * Time.smoothDeltaTime);
	}

	public Vector3 rotateSpeeds = new Vector3(0f, 0f, -60f);
}
