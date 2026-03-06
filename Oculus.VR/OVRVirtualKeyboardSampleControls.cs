using System;
using System.Collections;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(OVRVirtualKeyboardSampleInputHandler))]
[HelpURL("https://developer.oculus.com/documentation/unity/VK-unity-sample/")]
[Feature(Feature.VirtualKeyboard)]
public class OVRVirtualKeyboardSampleControls : MonoBehaviour
{
	private void Start()
	{
		this.inputHandler = base.GetComponent<OVRVirtualKeyboardSampleInputHandler>();
		this.keyboard.KeyboardHiddenEvent.AddListener(new UnityAction(this.OnHideKeyboard));
		if (this.MoveNearButton)
		{
			this.MoveNearButton.onClick.AddListener(new UnityAction(this.MoveKeyboardNear));
		}
		if (this.MoveFarButton)
		{
			this.MoveFarButton.onClick.AddListener(new UnityAction(this.MoveKeyboardFar));
		}
		if (this.DestroyKeyboardButton)
		{
			this.DestroyKeyboardButton.onClick.AddListener(new UnityAction(this.DestroyKeyboard));
		}
		base.StartCoroutine(this.CreateKeyboard());
	}

	private void OnDestroy()
	{
		if (this.keyboard == null)
		{
			return;
		}
		this.keyboard.KeyboardHiddenEvent.RemoveListener(new UnityAction(this.OnHideKeyboard));
		if (this.MoveNearButton)
		{
			this.MoveNearButton.onClick.RemoveListener(new UnityAction(this.MoveKeyboardNear));
		}
		if (this.MoveFarButton)
		{
			this.MoveFarButton.onClick.RemoveListener(new UnityAction(this.MoveKeyboardFar));
		}
		if (this.DestroyKeyboardButton)
		{
			this.DestroyKeyboardButton.onClick.RemoveListener(new UnityAction(this.DestroyKeyboard));
		}
	}

	public void ShowKeyboard()
	{
		if (this.keyboard == null)
		{
			base.StartCoroutine(this.CreateKeyboard());
			return;
		}
		this.keyboard.gameObject.SetActive(true);
		this.UpdateButtonInteractable();
	}

	private IEnumerator CreateKeyboard()
	{
		Text showButtonText = null;
		if (this.ShowButton && this.HideButton && this.DestroyKeyboardButton)
		{
			showButtonText = this.ShowButton.GetComponentInChildren<Text>();
			showButtonText.text = "Creating Keyboard...";
			this.ShowButton.interactable = false;
			this.HideButton.interactable = false;
			this.DestroyKeyboardButton.interactable = false;
		}
		if (this.keyboard == null)
		{
			if (this.keyboardPrefab)
			{
				this.keyboard = Object.Instantiate<OVRVirtualKeyboard>(this.keyboardPrefab);
			}
			if (!this.keyboard)
			{
				GameObject gameObject = new GameObject();
				this.keyboard = gameObject.AddComponent<OVRVirtualKeyboard>();
			}
			this.keyboardBackup.RestoreTo(this.keyboard);
			this.inputHandler.OVRVirtualKeyboard = this.keyboard;
		}
		yield return new OVRVirtualKeyboard.WaitUntilKeyboardVisible(this.keyboard);
		if (showButtonText)
		{
			showButtonText.text = "Show Keyboard";
		}
		this.UpdateButtonInteractable();
		yield break;
	}

	public void MoveKeyboard()
	{
		if (!this.keyboard.gameObject.activeSelf)
		{
			return;
		}
		this.isMovingKeyboard_ = true;
		Transform transform = this.keyboard.transform;
		this.keyboardMoveDistance_ = (this.inputHandler.InputRayPosition - transform.position).magnitude;
		this.keyboardScale_ = transform.localScale.x;
		this.UpdateButtonInteractable();
		this.keyboard.InputEnabled = false;
	}

	public void MoveKeyboardNear()
	{
		if (!this.keyboard.gameObject.activeSelf)
		{
			return;
		}
		this.keyboard.UseSuggestedLocation(OVRVirtualKeyboard.KeyboardPosition.Near);
	}

	public void MoveKeyboardFar()
	{
		if (!this.keyboard.gameObject.activeSelf)
		{
			return;
		}
		this.keyboard.UseSuggestedLocation(OVRVirtualKeyboard.KeyboardPosition.Far);
	}

	public void HideKeyboard()
	{
		this.keyboard.gameObject.SetActive(false);
		this.isMovingKeyboard_ = false;
		this.UpdateButtonInteractable();
	}

	public void DestroyKeyboard()
	{
		if (this.keyboard != null)
		{
			this.keyboardBackup = new OVRVirtualKeyboardSampleControls.OVRVirtualKeyboardBackup(this.keyboard);
			Object.Destroy(this.keyboard.gameObject);
			this.keyboard = null;
			this.UpdateButtonInteractable();
		}
	}

	private void OnHideKeyboard()
	{
		this.UpdateButtonInteractable();
	}

	private void UpdateButtonInteractable()
	{
		bool flag = this.keyboard != null;
		bool interactable = flag && this.keyboard.gameObject.activeSelf && !this.isMovingKeyboard_;
		if (this.ShowButton)
		{
			this.ShowButton.interactable = (!flag || !this.keyboard.gameObject.activeSelf);
		}
		if (this.MoveButton)
		{
			this.MoveButton.interactable = interactable;
		}
		if (this.MoveNearButton)
		{
			this.MoveNearButton.interactable = interactable;
		}
		if (this.MoveFarButton)
		{
			this.MoveFarButton.interactable = interactable;
		}
		if (this.HideButton)
		{
			this.HideButton.interactable = interactable;
		}
		if (this.DestroyKeyboardButton)
		{
			this.DestroyKeyboardButton.interactable = flag;
		}
	}

	private void Update()
	{
		bool flag = OVRInput.Get(OVRInput.Button.One | OVRInput.Button.Three | OVRInput.Button.PrimaryIndexTrigger | OVRInput.Button.SecondaryIndexTrigger, OVRInput.Controller.All);
		if (this.isMovingKeyboardFinished_ && !flag)
		{
			this.keyboard.InputEnabled = true;
			this.isMovingKeyboard_ = false;
			this.isMovingKeyboardFinished_ = false;
			this.UpdateButtonInteractable();
		}
		if (this.isMovingKeyboard_ && !this.isMovingKeyboardFinished_)
		{
			this.keyboardMoveDistance_ *= 1f + this.inputHandler.AnalogStickY * 0.01f;
			this.keyboardMoveDistance_ = Mathf.Clamp(this.keyboardMoveDistance_, 0.1f, 100f);
			this.keyboardScale_ += this.inputHandler.AnalogStickX * 0.01f;
			this.keyboardScale_ = Mathf.Clamp(this.keyboardScale_, 0.25f, 2f);
			Quaternion inputRayRotation = this.inputHandler.InputRayRotation;
			Transform transform = this.keyboard.transform;
			transform.SetPositionAndRotation(this.inputHandler.InputRayPosition + this.keyboardMoveDistance_ * (inputRayRotation * Vector3.forward), inputRayRotation);
			transform.localScale = Vector3.one * this.keyboardScale_;
			if (flag)
			{
				this.isMovingKeyboardFinished_ = true;
			}
		}
	}

	private const float THUMBSTICK_DEADZONE = 0.2f;

	[SerializeField]
	private Button ShowButton;

	[SerializeField]
	private Button MoveButton;

	[SerializeField]
	private Button HideButton;

	[SerializeField]
	private Button MoveNearButton;

	[SerializeField]
	private Button MoveFarButton;

	[SerializeField]
	private Button DestroyKeyboardButton;

	[SerializeField]
	private OVRVirtualKeyboard keyboard;

	[SerializeField]
	private OVRVirtualKeyboard keyboardPrefab;

	private OVRVirtualKeyboardSampleInputHandler inputHandler;

	private bool isMovingKeyboard_;

	private bool isMovingKeyboardFinished_;

	private float keyboardMoveDistance_;

	private float keyboardScale_ = 1f;

	private OVRVirtualKeyboardSampleControls.OVRVirtualKeyboardBackup keyboardBackup;

	private struct OVRVirtualKeyboardBackup
	{
		public OVRVirtualKeyboardBackup(OVRVirtualKeyboard keyboard)
		{
			this._position = keyboard.transform.position;
			this._rotation = keyboard.transform.rotation;
			this._scale = keyboard.transform.localScale;
			this._rightControllerDirectTransform = keyboard.rightControllerDirectTransform;
			this._rightControllerRootTransform = keyboard.rightControllerRootTransform;
			this._leftControllerDirectTransform = keyboard.leftControllerDirectTransform;
			this._leftControllerRootTransform = keyboard.leftControllerRootTransform;
			this._controllerRayInteraction = keyboard.controllerRayInteraction;
			this._controllerDirectInteraction = keyboard.controllerDirectInteraction;
			this._controllerRaycaster = keyboard.controllerRaycaster;
			this._handLeft = keyboard.handLeft;
			this._handRight = keyboard.handRight;
			this._handRayInteraction = keyboard.handRayInteraction;
			this._handDirectInteraction = keyboard.handDirectInteraction;
			this._handRaycaster = keyboard.handRaycaster;
			this._textHandlerField = null;
			OVRVirtualKeyboardInputFieldTextHandler ovrvirtualKeyboardInputFieldTextHandler = keyboard.TextHandler as OVRVirtualKeyboardInputFieldTextHandler;
			if (ovrvirtualKeyboardInputFieldTextHandler != null)
			{
				this._textHandlerField = ovrvirtualKeyboardInputFieldTextHandler.InputField;
			}
		}

		public void RestoreTo(OVRVirtualKeyboard keyboard)
		{
			keyboard.transform.SetPositionAndRotation(this._position, this._rotation);
			keyboard.transform.localScale = this._scale;
			keyboard.rightControllerDirectTransform = this._rightControllerDirectTransform;
			keyboard.rightControllerRootTransform = this._rightControllerRootTransform;
			keyboard.leftControllerDirectTransform = this._leftControllerDirectTransform;
			keyboard.leftControllerRootTransform = this._leftControllerRootTransform;
			keyboard.controllerRayInteraction = this._controllerRayInteraction;
			keyboard.controllerDirectInteraction = this._controllerDirectInteraction;
			keyboard.controllerRaycaster = this._controllerRaycaster;
			keyboard.handLeft = this._handLeft;
			keyboard.handRight = this._handRight;
			keyboard.handRayInteraction = this._handRayInteraction;
			keyboard.handDirectInteraction = this._handDirectInteraction;
			keyboard.handRaycaster = this._handRaycaster;
			if (keyboard.TextHandler == null)
			{
				keyboard.TextHandler = keyboard.gameObject.AddComponent<OVRVirtualKeyboardInputFieldTextHandler>();
			}
			OVRVirtualKeyboardInputFieldTextHandler ovrvirtualKeyboardInputFieldTextHandler = keyboard.TextHandler as OVRVirtualKeyboardInputFieldTextHandler;
			if (ovrvirtualKeyboardInputFieldTextHandler)
			{
				ovrvirtualKeyboardInputFieldTextHandler.InputField = this._textHandlerField;
			}
		}

		private readonly Vector3 _position;

		private readonly Quaternion _rotation;

		private readonly Vector3 _scale;

		private readonly Transform _rightControllerDirectTransform;

		private readonly Transform _rightControllerRootTransform;

		private readonly Transform _leftControllerDirectTransform;

		private readonly Transform _leftControllerRootTransform;

		private readonly bool _controllerRayInteraction;

		private readonly bool _controllerDirectInteraction;

		private readonly OVRHand _handLeft;

		private readonly OVRHand _handRight;

		private readonly bool _handRayInteraction;

		private readonly bool _handDirectInteraction;

		private readonly OVRPhysicsRaycaster _controllerRaycaster;

		private readonly OVRPhysicsRaycaster _handRaycaster;

		private readonly InputField _textHandlerField;
	}
}
