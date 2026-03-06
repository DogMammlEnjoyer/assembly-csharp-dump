using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoundedBoxUIProperties : UIBehaviour, IMeshModifier
{
	protected override void OnEnable()
	{
		this._image = base.gameObject.GetComponent<Image>();
	}

	protected override void OnDisable()
	{
		this._image = null;
	}

	protected override void Start()
	{
		base.StartCoroutine(this.DelayVertexGeneration());
	}

	private IEnumerator DelayVertexGeneration()
	{
		yield return new WaitForSeconds(0.1f);
		if (this._image == null)
		{
			this._image = base.gameObject.GetComponent<Image>();
			if (this._image == null)
			{
				yield break;
			}
		}
		this._image.SetAllDirty();
		yield break;
	}

	public void ModifyMesh(Mesh mesh)
	{
	}

	public void ModifyMesh(VertexHelper verts)
	{
		if (this._image == null)
		{
			this._image = base.gameObject.GetComponent<Image>();
			if (this._image == null)
			{
				return;
			}
		}
		Rect rect = ((RectTransform)base.transform).rect;
		Vector4 vector = new Vector4(rect.x, rect.y, Mathf.Abs(rect.width), Mathf.Abs(rect.height));
		UIVertex uivertex = default(UIVertex);
		for (int i = 0; i < verts.currentVertCount; i++)
		{
			verts.PopulateUIVertex(ref uivertex, i);
			Vector4 uv = uivertex.uv0;
			uv.z = vector.z;
			uv.w = vector.w;
			uivertex.uv0 = uv;
			uivertex.uv1 = this.borderRadius * 0.5f;
			verts.SetUIVertex(uivertex, i);
		}
	}

	private Image _image;

	public Vector4 borderRadius;
}
