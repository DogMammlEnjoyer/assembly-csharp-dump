using System;
using System.Collections.Generic;
using System.IO;
using GorillaNetworking;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PhotoBoothCamera : MonoBehaviour
{
	public void SetSaveImageToDevice(bool b)
	{
		this.saveImageToDevice = b;
	}

	public void Clear()
	{
		this.rt.Clear();
	}

	public void Capture(float FOV)
	{
		this.cam.fieldOfView = FOV;
		this.cam.Render();
		this.rt.Add(new RenderTexture(this.renderTexture.width, this.renderTexture.height, 1));
		Graphics.Blit(this.renderTexture, this.rt[this.rt.Count - 1]);
		this.OnCapture(this.rt[this.rt.Count - 1], this.rt.Count - 1);
	}

	public void Print()
	{
		if (this.saveImageToDevice)
		{
			string fileName = this.saveName;
			if (this.appendDateToFile)
			{
				DateTime dateTime = DateTime.UtcNow;
				if (GorillaComputer.instance != null)
				{
					dateTime = GorillaComputer.instance.GetServerTime();
				}
				fileName += dateTime.ToString("yyyyMMddHHmmss");
			}
			RenderTexture print = new RenderTexture(this.renderTexture.width, this.renderTexture.height * this.rt.Count, 1);
			for (int i = 0; i < this.rt.Count; i++)
			{
				Graphics.CopyTexture(this.rt[i], 0, 0, 0, 0, this.rt[i].width, this.rt[i].height, print, 0, 0, 0, this.rt[i].height * i);
			}
			NativeArray<byte> narray = new NativeArray<byte>(print.width * print.height * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			AsyncGPUReadback.RequestIntoNativeArray<byte>(ref narray, print, 0, delegate(AsyncGPUReadbackRequest request)
			{
				if (!request.hasError)
				{
					this.SaveImage(print, narray, fileName, this.imageDescription);
				}
				narray.Dispose();
			});
		}
	}

	private void SaveImage(RenderTexture rt, NativeArray<byte> narray, string fileName, string desc)
	{
		NativeArray<byte> nativeArray = ImageConversion.EncodeNativeArrayToJPG<byte>(narray, rt.graphicsFormat, (uint)rt.width, (uint)rt.height, 0U, 75);
		File.WriteAllBytes(Path.Combine(Application.persistentDataPath, fileName + ".jpg"), nativeArray.ToArray());
		nativeArray.Dispose();
	}

	[SerializeField]
	private Camera cam;

	[SerializeField]
	private RenderTexture renderTexture;

	[SerializeField]
	private TMP_Text imageLabel;

	[SerializeField]
	private Image imageImage;

	[SerializeField]
	private string saveName = "img";

	[SerializeField]
	private bool appendDateToFile;

	[SerializeField]
	private string imageDescription = "";

	private List<RenderTexture> rt = new List<RenderTexture>();

	public Action<Texture, int> OnCapture;

	private bool saveImageToDevice;
}
