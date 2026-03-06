using System;
using UnityEngine;

public class MB_SwitchBakedObjectsTexture : MonoBehaviour
{
	public void OnGUI()
	{
		GUILayout.Label("Press space to switch the material on one of the cubes. This scene reuses the Texture Bake Result from the SceneBasic example.", Array.Empty<GUILayoutOption>());
	}

	public void Start()
	{
		this.meshBaker.ClearMesh();
		if (this.meshBaker.AddDeleteGameObjects(this.meshBaker.GetObjectsToCombine().ToArray(), null, true))
		{
			this.meshBaker.Apply(null);
		}
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Material sharedMaterial = this.targetRenderer.sharedMaterial;
			int num = -1;
			for (int i = 0; i < this.materials.Length; i++)
			{
				if (this.materials[i] == sharedMaterial)
				{
					num = i;
				}
			}
			num++;
			if (num >= this.materials.Length)
			{
				num = 0;
			}
			if (num != -1)
			{
				this.targetRenderer.sharedMaterial = this.materials[num];
				string str = "Updating Material to: ";
				Material sharedMaterial2 = this.targetRenderer.sharedMaterial;
				Debug.Log(str + ((sharedMaterial2 != null) ? sharedMaterial2.ToString() : null));
				GameObject[] gos = new GameObject[]
				{
					this.targetRenderer.gameObject
				};
				if (this.meshBaker.UpdateGameObjects(gos, false, false, false, false, true, false, false, false, false))
				{
					this.meshBaker.Apply(null);
				}
			}
		}
	}

	public MeshRenderer targetRenderer;

	public Material[] materials;

	public MB3_MeshBaker meshBaker;
}
