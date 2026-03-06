using System;
using UnityEngine;

[Serializable]
public class MB_TextureArrayReference
{
	public MB_TextureArrayReference(string formatSetName, Texture2DArray ta)
	{
		this.texFromatSetName = formatSetName;
		this.texArray = ta;
	}

	public string texFromatSetName;

	public Texture2DArray texArray;
}
