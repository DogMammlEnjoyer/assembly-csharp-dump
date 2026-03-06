using System;
using UnityEngine;
using UnityEngine.Rendering;

[HelpURL("https://developer.oculus.com/reference/unity/latest/class_o_v_r_grid_cube")]
public class OVRGridCube : MonoBehaviour
{
	private void Update()
	{
		this.UpdateCubeGrid();
	}

	public void SetOVRCameraController(ref OVRCameraRig cameraController)
	{
		this.CameraController = cameraController;
	}

	private void UpdateCubeGrid()
	{
		if (this.CubeGrid != null)
		{
			this.CubeSwitchColor = !OVRManager.tracker.isPositionTracked;
			if (this.CubeSwitchColor != this.CubeSwitchColorOld)
			{
				this.CubeGridSwitchColor(this.CubeSwitchColor);
			}
			this.CubeSwitchColorOld = this.CubeSwitchColor;
		}
	}

	private void CreateCubeGrid()
	{
		Debug.LogWarning("Create CubeGrid");
		this.CubeGrid = new GameObject("CubeGrid");
		this.CubeGrid.layer = this.CameraController.gameObject.layer;
		for (int i = -this.gridSizeX; i <= this.gridSizeX; i++)
		{
			for (int j = -this.gridSizeY; j <= this.gridSizeY; j++)
			{
				for (int k = -this.gridSizeZ; k <= this.gridSizeZ; k++)
				{
					int num = 0;
					if ((i == 0 && j == 0) || (i == 0 && k == 0) || (j == 0 && k == 0))
					{
						if (i == 0 && j == 0 && k == 0)
						{
							num = 2;
						}
						else
						{
							num = 1;
						}
					}
					GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
					gameObject.GetComponent<BoxCollider>().enabled = false;
					gameObject.layer = this.CameraController.gameObject.layer;
					Renderer component = gameObject.GetComponent<Renderer>();
					component.shadowCastingMode = ShadowCastingMode.Off;
					component.receiveShadows = false;
					if (num == 0)
					{
						component.material.color = Color.red;
					}
					else if (num == 1)
					{
						component.material.color = Color.white;
					}
					else
					{
						component.material.color = Color.yellow;
					}
					gameObject.transform.position = new Vector3((float)i * this.gridScale, (float)j * this.gridScale, (float)k * this.gridScale);
					float num2 = 0.7f;
					if (num == 1)
					{
						num2 = 1f;
					}
					if (num == 2)
					{
						num2 = 2f;
					}
					gameObject.transform.localScale = new Vector3(this.cubeScale * num2, this.cubeScale * num2, this.cubeScale * num2);
					gameObject.transform.parent = this.CubeGrid.transform;
				}
			}
		}
	}

	private void CubeGridSwitchColor(bool CubeSwitchColor)
	{
		Color color = Color.red;
		if (CubeSwitchColor)
		{
			color = Color.blue;
		}
		Transform transform = this.CubeGrid.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Material material = transform.GetChild(i).GetComponent<Renderer>().material;
			if (material.color == Color.red || material.color == Color.blue)
			{
				material.color = color;
			}
		}
	}

	public KeyCode GridKey = KeyCode.G;

	private GameObject CubeGrid;

	private bool CubeGridOn;

	private bool CubeSwitchColorOld;

	private bool CubeSwitchColor;

	private int gridSizeX = 6;

	private int gridSizeY = 4;

	private int gridSizeZ = 6;

	private float gridScale = 0.3f;

	private float cubeScale = 0.03f;

	private OVRCameraRig CameraController;
}
