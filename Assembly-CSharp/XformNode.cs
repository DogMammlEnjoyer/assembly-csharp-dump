using System;
using UnityEngine;

[Serializable]
public class XformNode
{
	public Vector4 worldPosition
	{
		get
		{
			if (!this.parent)
			{
				return this.localPosition;
			}
			Matrix4x4 localToWorldMatrix = this.parent.localToWorldMatrix;
			Vector4 result = this.localPosition;
			MatrixUtils.MultiplyXYZ3x4(ref localToWorldMatrix, ref result);
			return result;
		}
	}

	public float radius
	{
		get
		{
			return this.localPosition.w;
		}
		set
		{
			this.localPosition.w = value;
		}
	}

	public Matrix4x4 LocalTRS()
	{
		return Matrix4x4.TRS(this.localPosition, Quaternion.identity, Vector3.one);
	}

	public Vector4 localPosition;

	public Transform parent;
}
