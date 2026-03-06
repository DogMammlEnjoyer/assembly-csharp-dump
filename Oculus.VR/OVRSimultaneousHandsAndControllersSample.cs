using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class OVRSimultaneousHandsAndControllersSample : MonoBehaviour
{
	private void Update()
	{
		this.displayText.text = OVRInput.GetActiveController().ToString();
	}

	public void EnableSimultaneousHandsAndControllers()
	{
		OVRInput.EnableSimultaneousHandsAndControllers();
		this.enableButton.interactable = false;
		this.disableButton.interactable = true;
	}

	public void DisableSimultaneousHandsAndControllers()
	{
		OVRInput.DisableSimultaneousHandsAndControllers();
		this.enableButton.interactable = true;
		this.disableButton.interactable = false;
	}

	[SerializeField]
	private Button enableButton;

	[SerializeField]
	private Button disableButton;

	[SerializeField]
	public Text displayText;
}
