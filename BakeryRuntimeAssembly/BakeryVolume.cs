using System;
using UnityEngine;

[ExecuteInEditMode]
public class BakeryVolume : MonoBehaviour
{
	public Vector3 GetMin()
	{
		return this.bounds.min;
	}

	public Vector3 GetInvSize()
	{
		Bounds bounds = this.bounds;
		return new Vector3(1f / bounds.size.x, 1f / bounds.size.y, 1f / bounds.size.z);
	}

	public Matrix4x4 GetMatrix()
	{
		if (this.tform == null)
		{
			this.tform = base.transform;
		}
		return Matrix4x4.TRS(this.tform.position, this.tform.rotation, Vector3.one).inverse;
	}

	public void SetGlobalParams()
	{
		Shader.SetGlobalTexture("_Volume0", this.bakedTexture0);
		Shader.SetGlobalTexture("_Volume1", this.bakedTexture1);
		Shader.SetGlobalTexture("_Volume2", this.bakedTexture2);
		if (this.bakedTexture3 != null)
		{
			Shader.SetGlobalTexture("_Volume3", this.bakedTexture3);
		}
		Shader.SetGlobalTexture("_VolumeMask", this.bakedMask);
		Bounds bounds = this.bounds;
		Vector3 min = bounds.min;
		Vector3 v = new Vector3(1f / bounds.size.x, 1f / bounds.size.y, 1f / bounds.size.z);
		Shader.SetGlobalVector("_GlobalVolumeMin", min);
		Shader.SetGlobalVector("_GlobalVolumeInvSize", v);
		if (this.supportRotationAfterBake)
		{
			Shader.SetGlobalMatrix("_GlobalVolumeMatrix", this.GetMatrix());
		}
	}

	public void UpdateBounds()
	{
		Vector3 position = base.transform.position;
		Vector3 size = this.bounds.size;
		this.bounds = new Bounds(position, size);
	}

	public void OnEnable()
	{
		if (this.isGlobal)
		{
			BakeryVolume.globalVolume = this;
			this.SetGlobalParams();
		}
	}

	public bool enableBaking = true;

	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	public bool adaptiveRes = true;

	public float voxelsPerUnit = 0.5f;

	public int resolutionX = 16;

	public int resolutionY = 16;

	public int resolutionZ = 16;

	public BakeryVolume.Encoding encoding;

	public BakeryVolume.ShadowmaskEncoding shadowmaskEncoding;

	public bool denoise;

	public bool isGlobal;

	public Texture3D bakedTexture0;

	public Texture3D bakedTexture1;

	public Texture3D bakedTexture2;

	public Texture3D bakedTexture3;

	public Texture3D bakedMask;

	public bool supportRotationAfterBake;

	public static BakeryVolume globalVolume;

	private Transform tform;

	public enum Encoding
	{
		Half4,
		RGBA8,
		RGBA8Mono
	}

	public enum ShadowmaskEncoding
	{
		RGBA8,
		A8
	}
}
