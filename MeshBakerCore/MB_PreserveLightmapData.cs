using System;
using UnityEngine;

[ExecuteInEditMode]
public class MB_PreserveLightmapData : MonoBehaviour
{
	private void Awake()
	{
		MeshRenderer component = base.GetComponent<MeshRenderer>();
		if (component == null)
		{
			Debug.LogError("The MB_PreserveLightmapData script must be on a GameObject with a MeshRenderer. There was no MeshRenderer on object: " + base.name);
			return;
		}
		if (component.lightmapIndex != -1)
		{
			this.lightmapIndex = component.lightmapIndex;
		}
		if (component.lightmapIndex == -1)
		{
			component.lightmapIndex = this.lightmapIndex;
		}
		this.lightmapScaleOffset = new Vector4(1f, 1f, 0f, 0f);
		component.lightmapScaleOffset = this.lightmapScaleOffset;
	}

	public int lightmapIndex;

	public Vector4 lightmapScaleOffset;
}
