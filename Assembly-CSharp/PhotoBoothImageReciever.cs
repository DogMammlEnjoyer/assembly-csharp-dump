using System;
using UnityEngine;

public class PhotoBoothImageReciever : MonoBehaviour
{
	private void OnEnable()
	{
		PhotoBoothCamera photoBoothCamera = this.photoBoothCamera;
		photoBoothCamera.OnCapture = (Action<Texture, int>)Delegate.Combine(photoBoothCamera.OnCapture, new Action<Texture, int>(this.photoBoothCamera_OnCapture));
	}

	private void photoBoothCamera_OnCapture(Texture texture, int i)
	{
		if (this.index < 0 || this.index == i)
		{
			base.GetComponent<Renderer>().material.mainTexture = texture;
		}
	}

	private void OnDisable()
	{
		PhotoBoothCamera photoBoothCamera = this.photoBoothCamera;
		photoBoothCamera.OnCapture = (Action<Texture, int>)Delegate.Remove(photoBoothCamera.OnCapture, new Action<Texture, int>(this.photoBoothCamera_OnCapture));
	}

	[SerializeField]
	private PhotoBoothCamera photoBoothCamera;

	[SerializeField]
	private int index = -1;
}
