using System;
using UnityEngine;

public class OVRRayHelper : MonoBehaviour
{
	private void Start()
	{
		if (this.Renderer != null)
		{
			this._initialScale = this.Renderer.transform.localScale;
		}
		if (this.Cursor != null)
		{
			this._cursorIntitialSize = this.Cursor.transform.localScale;
		}
	}

	public void UpdatePointerRay(OVRInputRayData rayData)
	{
		if (this.Renderer != null)
		{
			float num = rayData.IsOverCanvas ? rayData.DistanceToCanvas : this.DefaultLength;
			this.Renderer.transform.localPosition = Vector3.forward * (num * 0.5f + 0.05f);
			this.Renderer.transform.localScale = new Vector3(this._initialScale.x, num * 0.5f - 0.025f, this._initialScale.z);
			this.Renderer.sharedMaterial = (rayData.IsActive ? this.PinchMaterial : this.NormalMaterial);
		}
		if (this.Cursor != null)
		{
			this.Cursor.SetActive(rayData.IsOverCanvas);
			this.Cursor.transform.localScale = Mathf.Lerp(1f, 0.5f, rayData.ActivationStrength) * this._cursorIntitialSize;
			if (this.CursorFill != null)
			{
				float a = Mathf.Lerp(0f, 1f, rayData.ActivationStrength);
				this.CursorFill.color = new Color(1f, 1f, 1f, a);
			}
			if (rayData.IsOverCanvas)
			{
				this.Cursor.transform.position = rayData.WorldPosition + rayData.WorldNormal * 0.05f;
				this.Cursor.transform.forward = rayData.WorldNormal;
			}
		}
	}

	public MeshRenderer Renderer;

	public Material NormalMaterial;

	public Material PinchMaterial;

	public GameObject Cursor;

	public SpriteRenderer CursorFill;

	private Vector3 _initialScale;

	public float DefaultLength;

	private Vector3 _cursorIntitialSize;

	private const float _cursorSelectedScaleFactor = 0.5f;
}
