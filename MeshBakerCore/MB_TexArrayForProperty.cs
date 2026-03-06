using System;
using UnityEngine;

[Serializable]
public class MB_TexArrayForProperty
{
	public MB_TexArrayForProperty(string name, MB_TextureArrayReference[] texRefs)
	{
		this.texPropertyName = name;
		this.formats = texRefs;
	}

	public string texPropertyName;

	[NonReorderable]
	public MB_TextureArrayReference[] formats = new MB_TextureArrayReference[0];
}
