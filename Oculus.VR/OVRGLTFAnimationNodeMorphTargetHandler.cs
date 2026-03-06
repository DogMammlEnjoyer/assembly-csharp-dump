using System;
using UnityEngine;

public class OVRGLTFAnimationNodeMorphTargetHandler
{
	public OVRGLTFAnimationNodeMorphTargetHandler(OVRMeshData meshData)
	{
		this._meshData = meshData;
		this._meshModifiableData.vertices = new Vector3[this._meshData.baseAttributes.vertices.Length];
		this._meshModifiableData.texcoords = new Vector2[this._meshData.baseAttributes.texcoords.Length];
	}

	public void Update()
	{
		if (!this._modified)
		{
			return;
		}
		Array.Copy(this._meshData.baseAttributes.vertices, this._meshModifiableData.vertices, this._meshData.baseAttributes.vertices.Length);
		Array.Copy(this._meshData.baseAttributes.texcoords, this._meshModifiableData.texcoords, this._meshData.baseAttributes.texcoords.Length);
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < this._meshData.morphTargets.Length; i++)
		{
			if (this._meshData.morphTargets[i].vertices != null)
			{
				flag = true;
				int num = i / 2;
				if (i % 2 == 0)
				{
					float num2 = this._meshData.morphTargets[i].vertices[num].x * this.Weights[i];
					Vector3[] vertices = this._meshModifiableData.vertices;
					int num3 = num;
					vertices[num3].x = vertices[num3].x + num2;
				}
				else
				{
					float num4 = this._meshData.morphTargets[i].vertices[num].y * this.Weights[i];
					Vector3[] vertices2 = this._meshModifiableData.vertices;
					int num5 = num;
					vertices2[num5].y = vertices2[num5].y + num4;
				}
			}
			if (this._meshData.morphTargets[i].texcoords != null)
			{
				flag2 = true;
				int num6 = (i - 8) / 2;
				if (i % 2 == 0)
				{
					Vector2[] texcoords = this._meshModifiableData.texcoords;
					int num7 = num6;
					texcoords[num7].x = texcoords[num7].x + this._meshData.morphTargets[i].texcoords[num6].x * this.Weights[i];
				}
				else
				{
					Vector2[] texcoords2 = this._meshModifiableData.texcoords;
					int num8 = num6;
					texcoords2[num8].y = texcoords2[num8].y + this._meshData.morphTargets[i].texcoords[num6].y * this.Weights[i];
				}
			}
		}
		if (flag)
		{
			this._meshData.mesh.vertices = this._meshModifiableData.vertices;
			this._meshData.mesh.RecalculateBounds();
		}
		if (flag2)
		{
			this._meshData.mesh.uv = this._meshModifiableData.texcoords;
		}
		if (flag || flag2)
		{
			this._meshData.mesh.MarkModified();
		}
		this._modified = false;
	}

	public void MarkModified()
	{
		this._modified = true;
	}

	private OVRMeshData _meshData;

	public float[] Weights;

	private bool _modified;

	private OVRMeshAttributes _meshModifiableData;
}
