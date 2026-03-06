using System;
using UnityEngine;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-scene-samples-overview/")]
public class OVRSceneSampleController : MonoBehaviour
{
	private void Awake()
	{
		OVRCameraRig[] componentsInChildren = base.gameObject.GetComponentsInChildren<OVRCameraRig>();
		if (componentsInChildren.Length == 0)
		{
			Debug.LogWarning("OVRMainMenu: No OVRCameraRig attached.");
		}
		else if (componentsInChildren.Length > 1)
		{
			Debug.LogWarning("OVRMainMenu: More then 1 OVRCameraRig attached.");
		}
		else
		{
			this.cameraController = componentsInChildren[0];
		}
		OVRPlayerController[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<OVRPlayerController>();
		if (componentsInChildren2.Length == 0)
		{
			Debug.LogWarning("OVRMainMenu: No OVRPlayerController attached.");
			return;
		}
		if (componentsInChildren2.Length > 1)
		{
			Debug.LogWarning("OVRMainMenu: More then 1 OVRPlayerController attached.");
			return;
		}
		this.playerController = componentsInChildren2[0];
	}

	private void Start()
	{
		if (!Application.isEditor)
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
		if (this.cameraController != null)
		{
			this.gridCube = base.gameObject.AddComponent<OVRGridCube>();
			this.gridCube.SetOVRCameraController(ref this.cameraController);
		}
	}

	private void Update()
	{
		this.UpdateRecenterPose();
		this.UpdateVisionMode();
		if (this.playerController != null)
		{
			this.UpdateSpeedAndRotationScaleMultiplier();
		}
	}

	private void UpdateVisionMode()
	{
	}

	private void UpdateSpeedAndRotationScaleMultiplier()
	{
		float moveScaleMultiplier = 0f;
		this.playerController.GetMoveScaleMultiplier(ref moveScaleMultiplier);
		this.playerController.SetMoveScaleMultiplier(moveScaleMultiplier);
		float rotationScaleMultiplier = 0f;
		this.playerController.GetRotationScaleMultiplier(ref rotationScaleMultiplier);
		this.playerController.SetRotationScaleMultiplier(rotationScaleMultiplier);
	}

	private void UpdateRecenterPose()
	{
	}

	public KeyCode quitKey = KeyCode.Escape;

	public Texture fadeInTexture;

	public float speedRotationIncrement = 0.05f;

	private OVRPlayerController playerController;

	private OVRCameraRig cameraController;

	public string layerName = "Default";

	private bool visionMode = true;

	private OVRGridCube gridCube;
}
